using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Compartment
{
    /// <summary>
    /// PsychoPyエンジン - ハイブリッド状態機械
    /// C#が入室/退室/給餌のフレームワークを制御し、Pythonは課題のみ実行する
    ///
    /// フロー:
    /// C#:     入室検知 → RFID読取 → ドア閉 → Python起動 → [trial毎: 結果受取 → 給餌 → CONTINUE送信] → SESSION_END → ドア開 → 退室検知 → ループ
    /// Python: 各trial毎に TRIAL_RESULT:CORRECT/INCORRECT/TIMEOUT を出力 → stdin で CONTINUE 待ち → 全trial完了で SESSION_END → 終了
    /// </summary>
    class UcOperationPsychoPy
    {
        #region フェーズ定義
        private enum EPhase
        {
            Init,
            Idle,
            OpenDoorForEntry,
            WaitDoorOpen,
            WaitForEntry,
            ReadRFID,
            CloseDoor,
            WaitDoorClose,
            StartPython,
            WaitForPython,
            TrialFeeding,
            WaitTrialFeedComplete,
            ParseResult,
            Feeding,
            WaitFeedComplete,
            OpenDoorForExit,
            WaitDoorOpenForExit,
            WaitForExit,
            Stopping
        }
        #endregion

        readonly ParentHelper<FormMain> mainForm = new ParentHelper<FormMain>();

        private ref OpCollection opCollection => ref mainForm.Parent.opCollection;
        private ref PreferencesDat PreferencesDatOriginal
        {
            get { return ref mainForm.Parent.preferencesDatOriginal; }
        }

        private EPhase _phase = EPhase.Init;
        private bool _eventLoggerEnabled = false;
        private Process _pythonProcess = null;
        private StringBuilder _stdoutBuffer = new StringBuilder();
        private StringBuilder _stderrBuffer = new StringBuilder();
        private volatile bool _pythonExited = false;
        private volatile int _pythonExitCode = 0;
        private string _currentRfid = "";
        private ConcurrentQueue<string> _trialResultQueue = new ConcurrentQueue<string>();
        private Stopwatch _phaseTimer = new Stopwatch();
        private Stopwatch _doorTimer = new Stopwatch();
        private const int DOOR_TIMEOUT_MS = 30000; // ドア操作の安全タイムアウト（30秒）
        private DateTime _monitorStandbyTimer = DateTime.Now;

        public UcOperationPsychoPy(FormMain baseForm)
        {
            mainForm.SetParent(baseForm);
            Debug.WriteLine("[PsychoPy] Hybrid engine initialized");
        }

        /// <summary>
        /// 状態機械処理（PsychoPyハイブリッドエンジン）
        /// BackgroundWorkerから繰り返し呼ばれる
        /// </summary>
        public void OnOperationStateMachineProc()
        {
            // 毎tick Stop/EmergencyStop を検査
            if (_phase != EPhase.Idle && _phase != EPhase.Init)
            {
                if (CheckInteruptStop())
                    return;
            }

            switch (_phase)
            {
                case EPhase.Init:
                    PhaseInit();
                    break;
                case EPhase.Idle:
                    PhaseIdle();
                    break;
                case EPhase.OpenDoorForEntry:
                    PhaseOpenDoorForEntry();
                    break;
                case EPhase.WaitDoorOpen:
                    PhaseWaitDoorOpen();
                    break;
                case EPhase.WaitForEntry:
                    PhaseWaitForEntry();
                    break;
                case EPhase.ReadRFID:
                    PhaseReadRFID();
                    break;
                case EPhase.CloseDoor:
                    PhaseCloseDoor();
                    break;
                case EPhase.WaitDoorClose:
                    PhaseWaitDoorClose();
                    break;
                case EPhase.StartPython:
                    PhaseStartPython();
                    break;
                case EPhase.WaitForPython:
                    PhaseWaitForPython();
                    break;
                case EPhase.TrialFeeding:
                    PhaseTrialFeeding();
                    break;
                case EPhase.WaitTrialFeedComplete:
                    PhaseWaitTrialFeedComplete();
                    break;
                case EPhase.ParseResult:
                    PhaseParseResult();
                    break;
                case EPhase.Feeding:
                    PhaseFeeding();
                    break;
                case EPhase.WaitFeedComplete:
                    PhaseWaitFeedComplete();
                    break;
                case EPhase.OpenDoorForExit:
                    PhaseOpenDoorForExit();
                    break;
                case EPhase.WaitDoorOpenForExit:
                    PhaseWaitDoorOpenForExit();
                    break;
                case EPhase.WaitForExit:
                    PhaseWaitForExit();
                    break;
                case EPhase.Stopping:
                    PhaseStopping();
                    break;
            }
        }

        #region 各フェーズ実装

        private void PhaseInit()
        {
            _phase = EPhase.Idle;
        }

        /// <summary>
        /// Idle: Start コマンド待ち
        /// </summary>
        private void PhaseIdle()
        {
            OpCollection.ECommand command = opCollection.Command;

            if (command == OpCollection.ECommand.Start)
            {
                opCollection.IsBusy.Value = true;
                opCollection.trialCount = 0;
                opCollection.callbackMessageNormal("PsychoPyハイブリッドエンジン開始");

                mainForm.Parent._hardwareService?.EventLogger.Enable();
                _eventLoggerEnabled = true;
                Debug.WriteLine("[PsychoPy] EventLogger enabled");

                // CSV出力ファイルオープン（Block式に統一）
                try
                {
                    opCollection.file.Open(PreferencesDatOriginal.OutputResultFile);
                    Debug.WriteLine("[PsychoPy] File opened: " + PreferencesDatOriginal.OutputResultFile);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("[PsychoPy] File open error: " + ex.Message);
                    if (PreferencesDatOriginal.EnableDebugMode)
                    {
                        opCollection.callbackMessageNormal("[デバッグモード] ファイルオープンに失敗しましたが続行します");
                    }
                    else
                    {
                        opCollection.callbackMessageError("CSVファイルオープン失敗: " + ex.Message);
                        _phase = EPhase.Stopping;
                        return;
                    }
                }

                // フラグリセット
                mainForm.Parent.OpFlagRoomIn = false;
                mainForm.Parent.OpFlagRoomOut = false;

                _phase = EPhase.OpenDoorForEntry;
            }
        }

        /// <summary>
        /// ドアを開けて入室を待つ準備
        /// </summary>
        private void PhaseOpenDoorForEntry()
        {
            opCollection.sequencer.State = OpCollection.Sequencer.EState.PreEnterCageProc;
            opCollection.callbackMessageNormal("入室準備中...");

            // RFID・フラグリセット（Block式: PreEnterCageProc でクリア）
            mainForm.Parent.OpeClearIdCode();
            mainForm.Parent.OpFlagRoomIn = false;
            mainForm.Parent.OpFlagRoomOut = false;

            // モニタースタンバイタイマー初期化
            _monitorStandbyTimer = DateTime.Now;

            if (PreferencesDatOriginal.DisableDoor)
            {
                // ドア無効時はスキップ
                Debug.WriteLine("[PsychoPy] Door disabled, skipping open");
                _phase = EPhase.WaitForEntry;
                opCollection.sequencer.State = OpCollection.Sequencer.EState.WaitingForEnterCage;
                opCollection.callbackMessageNormal("入室待ち（ドア無効）...");
            }
            else
            {
                mainForm.Parent.OpOpenDoor();
                _doorTimer.Restart();
                _phase = EPhase.WaitDoorOpen;
                opCollection.callbackMessageNormal("ドアOPEN中...");
            }
        }

        /// <summary>
        /// ドアが開くのを待つ
        /// </summary>
        private void PhaseWaitDoorOpen()
        {
            opCollection.sequencer.State = OpCollection.Sequencer.EState.WaitingForOpenDoor;

            // 安全タイムアウト: DevDoorスレッドが応答しない場合のフォールバック
            if (!mainForm.Parent.OpFlagOpenDoor && _doorTimer.ElapsedMilliseconds > DOOR_TIMEOUT_MS)
            {
                Debug.WriteLine("[PsychoPy] Door open safety timeout exceeded");
                if (!PreferencesDatOriginal.IgnoreDoorError)
                {
                    opCollection.callbackMessageError("ドアOPENタイムアウト（安全制限）");
                    ShowErrorMessageBox("ドアOPENが" + (DOOR_TIMEOUT_MS / 1000) + "秒以内に完了しませんでした。\nハードウェアを確認してください。");
                    _phase = EPhase.Stopping;
                    return;
                }
                Debug.WriteLine("[PsychoPy] Door open timeout ignored (IgnoreDoorError=true)");
                _phase = EPhase.WaitForEntry;
                opCollection.sequencer.State = OpCollection.Sequencer.EState.WaitingForEnterCage;
                opCollection.callbackMessageNormal("入室待ち（ドアタイムアウト無視）...");
                return;
            }

            if (mainForm.Parent.OpFlagOpenDoor)
            {
                // ドアOPEN異常（Block式: OpResult != Done でエラー判定）
                if (mainForm.Parent.OpResultOpenDoor != FormMain.EDeviceResult.Done)
                {
                    opCollection.callbackMessageNormal("ドアOPEN異常");
                    if (!PreferencesDatOriginal.IgnoreDoorError)
                    {
                        _phase = EPhase.Stopping;
                        return;
                    }
                }
                // ドアOPEN正常
                Debug.WriteLine("[PsychoPy] Door opened");
                _phase = EPhase.WaitForEntry;
                opCollection.sequencer.State = OpCollection.Sequencer.EState.WaitingForEnterCage;
                opCollection.callbackMessageNormal("入室待ち...");
            }
        }

        /// <summary>
        /// 入室を待つ
        /// </summary>
        private void PhaseWaitForEntry()
        {
            // モニタースタンバイ（Block式: 待機時間経過でモニターOFF）
            if ((DateTime.Now - _monitorStandbyTimer).TotalMinutes > PreferencesDatOriginal.MonitorSaveTime
                && PreferencesDatOriginal.EnableMonitorSave)
            {
                _monitorStandbyTimer = DateTime.Now;
                MonitorPower.Monitor.PowerOff();
            }

            if (mainForm.Parent.OpFlagRoomIn)
            {
                // 入室検知時にモニターON
                MonitorPower.Monitor.PowerOn();

                Debug.WriteLine("[PsychoPy] Entry detected");
                opCollection.callbackMessageNormal("入室検知");
                opCollection.TimeEnterCage = DateTime.Now;

                mainForm.Parent.OpFlagRoomIn = false;
                _phase = EPhase.ReadRFID;
            }
        }

        /// <summary>
        /// RFID読取
        /// デバッグモード: rfidReaderDummyから読取（デバッグパネルで設定）。
        ///                 一度設定すれば毎回の入室で再利用される。未設定ならNO_ID。
        /// 通常モード: ハードウェアRFIDリーダーからコールバック経由で読取を待つ。
        /// </summary>
        private bool _rfidWaitMessageShown = false;

        private void PhaseReadRFID()
        {
            // イリーガル退室チェック: RFID読取中に動物が逃げた場合
            if (CheckIllegalExit()) return;

            // コールバック経由で設定されたID（実機: シリアル受信, デバッグ: rfidReaderDummy.SetRFID()）
            _currentRfid = mainForm.Parent.OpeGetIdCode();

            // デバッグモード: コールバック未着の場合、rfidReaderDummyの保持値も確認
            if (string.IsNullOrEmpty(_currentRfid) && PreferencesDatOriginal.EnableDebugMode)
            {
                if (mainForm.Parent.rfidReaderDummy != null
                    && !string.IsNullOrEmpty(mainForm.Parent.rfidReaderDummy.RFID))
                {
                    _currentRfid = mainForm.Parent.rfidReaderDummy.RFID;
                }
            }

            if (!string.IsNullOrEmpty(_currentRfid))
            {
                // RFIDが取得できた → そのまま使用
                _rfidWaitMessageShown = false;
            }
            else if (PreferencesDatOriginal.EnableNoIDOperation
                     || PreferencesDatOriginal.EnableDebugMode)
            {
                // ID無し動作許可 or デバッグモード → NO_IDで進む
                _currentRfid = "NO_ID";
                _rfidWaitMessageShown = false;
            }
            else
            {
                // 通常モード: RFID読取を待ち続ける
                if (!_rfidWaitMessageShown)
                {
                    opCollection.callbackMessageNormal("RFID読取待ち...");
                    _rfidWaitMessageShown = true;
                }
                return;
            }

            Debug.WriteLine("[PsychoPy] RFID: " + _currentRfid);
            opCollection.idCode = _currentRfid;
            opCollection.callbackSetUiCurrentIdCode(_currentRfid);
            opCollection.callbackMessageNormal("RFID: " + _currentRfid);

            // 実機モード: 読取後にクリアして次の個体と混同しないようにする
            // デバッグモード: クリアしない（設定したRFIDを再利用するため）
            if (!PreferencesDatOriginal.EnableDebugMode)
            {
                mainForm.Parent.OpeClearIdCode();
            }

            _phase = EPhase.CloseDoor;
        }

        /// <summary>
        /// ドアを閉める
        /// </summary>
        private void PhaseCloseDoor()
        {
            opCollection.callbackMessageNormal("ドアCLOSE中...");

            if (PreferencesDatOriginal.DisableDoor)
            {
                Debug.WriteLine("[PsychoPy] Door disabled, skipping close");
                _phase = EPhase.StartPython;
            }
            else
            {
                mainForm.Parent.OpCloseDoor();
                _doorTimer.Restart();
                _phase = EPhase.WaitDoorClose;
                opCollection.sequencer.State = OpCollection.Sequencer.EState.WaitingForCloseDoor;
            }
        }

        /// <summary>
        /// ドアが閉まるのを待つ
        /// </summary>
        private void PhaseWaitDoorClose()
        {
            // イリーガル退室チェック: ドアが閉まる前に動物が逃げた場合
            if (CheckIllegalExit()) return;

            // 安全タイムアウト
            if (!mainForm.Parent.OpFlagCloseDoor && _doorTimer.ElapsedMilliseconds > DOOR_TIMEOUT_MS)
            {
                Debug.WriteLine("[PsychoPy] Door close safety timeout exceeded");
                if (!PreferencesDatOriginal.IgnoreDoorError)
                {
                    opCollection.callbackMessageError("ドアCLOSEタイムアウト（安全制限）");
                    ShowErrorMessageBox("ドアCLOSEが" + (DOOR_TIMEOUT_MS / 1000) + "秒以内に完了しませんでした。\nハードウェアを確認してください。");
                    _phase = EPhase.Stopping;
                    return;
                }
                Debug.WriteLine("[PsychoPy] Door close timeout ignored (IgnoreDoorError=true)");
                _phase = EPhase.StartPython;
                return;
            }

            if (mainForm.Parent.OpFlagCloseDoor)
            {
                // ドアCLOSE異常（Block式: OpResult != Done でエラー判定）
                if (mainForm.Parent.OpResultCloseDoor != FormMain.EDeviceResult.Done)
                {
                    opCollection.callbackMessageNormal("ドアCLOSE異常");
                    if (!PreferencesDatOriginal.IgnoreDoorError)
                    {
                        _phase = EPhase.Stopping;
                        return;
                    }
                }
                // ドアCLOSE正常
                Debug.WriteLine("[PsychoPy] Door closed");
                opCollection.callbackMessageNormal("ドアCLOSE正常処理");
                _phase = EPhase.StartPython;
            }
        }

        /// <summary>
        /// Pythonスクリプトを起動
        /// </summary>
        private void PhaseStartPython()
        {
            string scriptPath = Program.PsychoPyScriptPath;

            if (string.IsNullOrEmpty(scriptPath) || !File.Exists(scriptPath))
            {
                opCollection.callbackMessageError("スクリプトが見つかりません: " + scriptPath);
                ShowErrorMessageBox("Pythonスクリプトが見つかりません:\n" + scriptPath);
                _phase = EPhase.Stopping;
                return;
            }

            try
            {
                _stdoutBuffer.Clear();
                _stderrBuffer.Clear();
                _pythonExited = false;
                _pythonExitCode = 0;

                // キューをリセット
                while (_trialResultQueue.TryDequeue(out _)) { }

                _pythonProcess = new Process();
                _pythonProcess.StartInfo.FileName = "python";
                _pythonProcess.StartInfo.Arguments = "-u \"" + scriptPath + "\" " + _currentRfid;
                _pythonProcess.StartInfo.WorkingDirectory = Path.GetDirectoryName(scriptPath);
                _pythonProcess.StartInfo.UseShellExecute = false;
                _pythonProcess.StartInfo.RedirectStandardOutput = true;
                _pythonProcess.StartInfo.RedirectStandardError = true;
                _pythonProcess.StartInfo.RedirectStandardInput = true;
                _pythonProcess.StartInfo.CreateNoWindow = true;

                _pythonProcess.OutputDataReceived += (sender, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                    {
                        Debug.WriteLine("[PsychoPy:stdout] " + e.Data);
                        lock (_stdoutBuffer)
                        {
                            _stdoutBuffer.AppendLine(e.Data);
                        }

                        // per-trial結果またはセッション終了をキューに追加
                        if (e.Data.StartsWith("TRIAL_RESULT:") || e.Data == "SESSION_END")
                        {
                            _trialResultQueue.Enqueue(e.Data);
                        }
                    }
                };

                _pythonProcess.ErrorDataReceived += (sender, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                    {
                        Debug.WriteLine("[PsychoPy:stderr] " + e.Data);
                        lock (_stderrBuffer)
                        {
                            _stderrBuffer.AppendLine(e.Data);
                        }
                    }
                };

                _pythonProcess.EnableRaisingEvents = true;
                _pythonProcess.Exited += (sender, e) =>
                {
                    try { _pythonExitCode = _pythonProcess.ExitCode; } catch { _pythonExitCode = -1; }
                    _pythonExited = true;
                    Debug.WriteLine("[PsychoPy] Python exited with code: " + _pythonExitCode);
                };

                _pythonProcess.Start();
                _pythonProcess.BeginOutputReadLine();
                _pythonProcess.BeginErrorReadLine();

                Debug.WriteLine("[PsychoPy] Python started: " + scriptPath + " " + _currentRfid);
                opCollection.callbackMessageNormal("Python課題実行中: " + Path.GetFileName(scriptPath));
                mainForm.Parent.EpisodeMode.Value = true;

                // CSV出力用タイムスタンプ初期化
                opCollection.dateTimeTriggerTouch = DateTime.Now; // Python開始時刻 = トリガ時刻
                opCollection.dateTimeCorrectTouch = new DateTime();
                opCollection.dateTimeout = new DateTime();
                opCollection.flagFeed = false;

                _phase = EPhase.WaitForPython;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("[PsychoPy] Failed to start Python: " + ex.Message);
                opCollection.callbackMessageError("Python起動失敗: " + ex.Message);
                ShowErrorMessageBox("Pythonスクリプトの起動に失敗しました。\n\n" + ex.Message);
                _pythonProcess = null;
                _phase = EPhase.Stopping;
            }
        }

        /// <summary>
        /// Python実行中: per-trial結果を受信 → 給餌判定 → CONTINUE送信 のループ
        /// SESSION_END受信で全trial完了 → ParseResultへ
        /// </summary>
        private void PhaseWaitForPython()
        {
            // イリーガル退室チェック
            if (CheckIllegalExit()) return;

            // trial結果キューをチェック
            if (_trialResultQueue.TryDequeue(out string trialResult))
            {
                if (trialResult == "SESSION_END")
                {
                    // 全trial完了 → Python終了を待ってからParseResultへ
                    Debug.WriteLine("[PsychoPy] SESSION_END received, waiting for Python exit");
                    // PythonはSESSION_END出力後すぐに終了するので少し待つ
                    if (_pythonExited)
                    {
                        try { _pythonProcess?.WaitForExit(); } catch { }
                        mainForm.Parent.EpisodeMode.Value = false;
                        _phase = EPhase.ParseResult;
                    }
                    else
                    {
                        // まだ終了していない場合はキューに戻して次のtickで再試行
                        _trialResultQueue.Enqueue(trialResult);
                    }
                    return;
                }

                // per-trial結果処理
                opCollection.trialCount++;
                opCollection.callbackSetUiCurentNumberOfTrial(opCollection.trialCount);

                if (trialResult == "TRIAL_RESULT:CORRECT")
                {
                    Debug.WriteLine("[PsychoPy] Trial " + opCollection.trialCount + ": CORRECT → feeding");
                    opCollection.callbackMessageNormal("Trial " + opCollection.trialCount + ": 正解 → 給餌");
                    opCollection.taskResultVal = OpCollection.ETaskResult.Ok;
                    opCollection.dateTimeCorrectTouch = DateTime.Now;
                    opCollection.flagFeed = false;

                    // CSV出力
                    opCollection.sequencer.callbackOutputFile(OpCollection.Sequencer.EState.BlockOutput);

                    _phase = EPhase.TrialFeeding;
                }
                else
                {
                    // INCORRECT or TIMEOUT
                    Debug.WriteLine("[PsychoPy] Trial " + opCollection.trialCount + ": " + trialResult);
                    opCollection.callbackMessageNormal("Trial " + opCollection.trialCount + ": 不正解");
                    opCollection.taskResultVal = OpCollection.ETaskResult.Ng;
                    opCollection.dateTimeout = DateTime.Now;
                    opCollection.flagFeed = false;

                    // CSV出力
                    opCollection.sequencer.callbackOutputFile(OpCollection.Sequencer.EState.BlockOutput);

                    // 給餌なし → 即CONTINUE
                    SendContinueToPython();
                }
                return;
            }

            // Python異常終了チェック（TRIAL_RESULTもSESSION_ENDも来ずに終了した場合）
            if (_pythonExited)
            {
                try { _pythonProcess?.WaitForExit(); } catch { }
                Debug.WriteLine("[PsychoPy] Python exited unexpectedly, moving to parse");
                mainForm.Parent.EpisodeMode.Value = false;
                _phase = EPhase.ParseResult;
            }
        }

        /// <summary>
        /// Python終了後の処理
        /// per-trial結果は WaitForPython で処理済み。ここではPython正常終了確認のみ。
        /// </summary>
        private void PhaseParseResult()
        {
            // 異常終了チェック
            if (_pythonExitCode != 0)
            {
                opCollection.callbackMessageError("Pythonスクリプト異常終了 (exit code: " + _pythonExitCode + ")");
                string stderr = "";
                lock (_stderrBuffer) { stderr = _stderrBuffer.ToString().Trim(); }
                string userMessage = BuildErrorMessage(_pythonExitCode, stderr);
                ShowErrorMessageBox(userMessage);
            }

            _pythonProcess = null;

            Debug.WriteLine("[PsychoPy] Session complete, total trials: " + opCollection.trialCount);
            opCollection.callbackMessageNormal("セッション完了 (全" + opCollection.trialCount + "試行)");

            // 全trial完了 → 退室ドアへ
            _phase = EPhase.OpenDoorForExit;
        }

        /// <summary>
        /// 給餌実行（正解時のみ）
        /// </summary>
        private void PhaseFeeding()
        {
            opCollection.sequencer.State = OpCollection.Sequencer.EState.WaitingForReward;
            opCollection.callbackMessageNormal("給餌中...");

            // FeedLamp ON
            if (PreferencesDatOriginal.EnableFeedLamp)
            {
                mainForm.Parent.OpSetFeedLampOn();
            }

            // 給餌実行
            int feedTime = PreferencesDatOriginal.OpeTimeToFeed;
            mainForm.Parent.OpSetFeedOn(feedTime);
            opCollection.flagFeed = true;

            Debug.WriteLine("[PsychoPy] Feeding started: " + feedTime + "ms");
            _phase = EPhase.WaitFeedComplete;
            opCollection.sequencer.State = OpCollection.Sequencer.EState.WaitingForFeedComplete;
        }

        /// <summary>
        /// 給餌完了を待つ
        /// </summary>
        private void PhaseWaitFeedComplete()
        {
            // イリーガル退室チェック: 給餌中に動物が逃げた場合
            if (CheckIllegalExit()) return;

            if (mainForm.Parent.OpFlagFeedOn)
            {
                Debug.WriteLine("[PsychoPy] Feeding complete");

                // FeedLamp OFF
                if (PreferencesDatOriginal.EnableFeedLamp)
                {
                    mainForm.Parent.OpSetFeedLampOff();
                }

                _phase = EPhase.OpenDoorForExit;
            }
        }

        /// <summary>
        /// per-trial給餌（正解時のみ）: PythonにCONTINUEを送信して次のtrialへ
        /// </summary>
        private void PhaseTrialFeeding()
        {
            opCollection.sequencer.State = OpCollection.Sequencer.EState.WaitingForReward;
            opCollection.callbackMessageNormal("給餌中 (trial " + opCollection.trialCount + ")...");

            // FeedLamp ON
            if (PreferencesDatOriginal.EnableFeedLamp)
            {
                mainForm.Parent.OpSetFeedLampOn();
            }

            // 給餌実行
            int feedTime = PreferencesDatOriginal.OpeTimeToFeed;
            mainForm.Parent.OpSetFeedOn(feedTime);
            opCollection.flagFeed = true;

            Debug.WriteLine("[PsychoPy] Trial feeding started: " + feedTime + "ms");
            _phase = EPhase.WaitTrialFeedComplete;
        }

        /// <summary>
        /// per-trial給餌完了を待つ → CONTINUE送信 → 次のtrial待ちへ
        /// </summary>
        private void PhaseWaitTrialFeedComplete()
        {
            // イリーガル退室チェック: 給餌中に動物が逃げた場合
            if (CheckIllegalExit()) return;

            if (mainForm.Parent.OpFlagFeedOn)
            {
                Debug.WriteLine("[PsychoPy] Trial feeding complete");

                // FeedLamp OFF
                if (PreferencesDatOriginal.EnableFeedLamp)
                {
                    mainForm.Parent.OpSetFeedLampOff();
                }

                // 給餌完了 → PythonにCONTINUE送信して次のtrialへ
                SendContinueToPython();
                _phase = EPhase.WaitForPython;
            }
        }

        /// <summary>
        /// Pythonの stdin に CONTINUE を送信して次のtrialを開始させる
        /// </summary>
        private void SendContinueToPython()
        {
            try
            {
                if (_pythonProcess != null && !_pythonProcess.HasExited)
                {
                    _pythonProcess.StandardInput.WriteLine("CONTINUE");
                    _pythonProcess.StandardInput.Flush();
                    Debug.WriteLine("[PsychoPy] Sent CONTINUE to Python");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("[PsychoPy] SendContinue failed: " + ex.Message);
            }
        }

        /// <summary>
        /// 退室用にドアを開ける + 終了音再生
        /// </summary>
        private void PhaseOpenDoorForExit()
        {
            opCollection.sequencer.State = OpCollection.Sequencer.EState.PreOpenDoorProc;
            opCollection.callbackMessageNormal("退室準備中...");

            // 終了音再生
            mainForm.Parent.OpPlaySoundOfEnd();

            // ルームランプ ON
            mainForm.Parent.OpSetRoomLampOn();

            if (PreferencesDatOriginal.DisableDoor)
            {
                Debug.WriteLine("[PsychoPy] Door disabled, skipping open for exit");
                _phase = EPhase.WaitForExit;
                opCollection.sequencer.State = OpCollection.Sequencer.EState.WaitingForLeaveCage;
                opCollection.callbackMessageNormal("退室待ち（ドア無効）...");
                _phaseTimer.Restart();
            }
            else
            {
                mainForm.Parent.OpOpenDoor();
                _doorTimer.Restart();
                _phase = EPhase.WaitDoorOpenForExit;
            }
        }

        /// <summary>
        /// 退室用ドアオープン待ち
        /// </summary>
        private void PhaseWaitDoorOpenForExit()
        {
            opCollection.sequencer.State = OpCollection.Sequencer.EState.WaitingForOpenDoor;

            // 安全タイムアウト
            if (!mainForm.Parent.OpFlagOpenDoor && _doorTimer.ElapsedMilliseconds > DOOR_TIMEOUT_MS)
            {
                Debug.WriteLine("[PsychoPy] Exit door open safety timeout exceeded");
                if (!PreferencesDatOriginal.IgnoreDoorError)
                {
                    opCollection.callbackMessageError("退室ドアOPENタイムアウト（安全制限）");
                    ShowErrorMessageBox("退室用ドアOPENが" + (DOOR_TIMEOUT_MS / 1000) + "秒以内に完了しませんでした。");
                    _phase = EPhase.Stopping;
                    return;
                }
                Debug.WriteLine("[PsychoPy] Exit door open timeout ignored");
                _phase = EPhase.WaitForExit;
                opCollection.sequencer.State = OpCollection.Sequencer.EState.WaitingForLeaveCage;
                opCollection.callbackMessageNormal("退室待ち（ドアタイムアウト無視）...");
                _phaseTimer.Restart();
                return;
            }

            if (mainForm.Parent.OpFlagOpenDoor)
            {
                // ドアOPEN異常（Block式: OpResult != Done でエラー判定）
                if (mainForm.Parent.OpResultOpenDoor != FormMain.EDeviceResult.Done)
                {
                    opCollection.callbackMessageNormal("退室ドアOPEN異常");
                    if (!PreferencesDatOriginal.IgnoreDoorError)
                    {
                        _phase = EPhase.Stopping;
                        return;
                    }
                }
                // ドアOPEN正常
                Debug.WriteLine("[PsychoPy] Door opened for exit");
                _phase = EPhase.WaitForExit;
                opCollection.sequencer.State = OpCollection.Sequencer.EState.WaitingForLeaveCage;
                opCollection.callbackMessageNormal("退室待ち...");
                _phaseTimer.Restart();
            }
        }

        /// <summary>
        /// 退室を待つ
        /// </summary>
        private void PhaseWaitForExit()
        {
            // 退室タイムアウトチェック (分→ミリ秒)
            long timeoutMs = PreferencesDatOriginal.OpeTimeoutOfLeaveCage * 60 * 1000;
            if (_phaseTimer.ElapsedMilliseconds > timeoutMs)
            {
                Debug.WriteLine("[PsychoPy] Exit timeout");
                opCollection.callbackMessageNormal("退室タイムアウト - 次の入室待ちへ");
                opCollection.sequencer.State = OpCollection.Sequencer.EState.WaitingForTimeoutLeaveCage;

                mainForm.Parent.OpFlagRoomOut = false;
                mainForm.Parent.OpSetRoomLampOff();

                _phase = EPhase.OpenDoorForEntry;
                return;
            }

            if (mainForm.Parent.OpFlagRoomOut)
            {
                Debug.WriteLine("[PsychoPy] Exit detected");
                opCollection.callbackMessageNormal("退室検知");

                mainForm.Parent.OpFlagRoomOut = false;
                mainForm.Parent.OpSetRoomLampOff();

                // 次の入室待ちへループ
                _phase = EPhase.OpenDoorForEntry;
            }
        }

        /// <summary>
        /// 停止処理
        /// </summary>
        private void PhaseStopping()
        {
            opCollection.callbackMessageNormal("PsychoPyエンジン停止中...");

            KillPythonProcess();

            mainForm.Parent.EpisodeMode.Value = false;

            if (_eventLoggerEnabled)
            {
                mainForm.Parent._hardwareService?.EventLogger.Disable();
                _eventLoggerEnabled = false;
            }

            mainForm.Parent.OpSetRoomLampOff();
            if (PreferencesDatOriginal.EnableFeedLamp)
            {
                mainForm.Parent.OpSetFeedLampOff();
            }

            // モニターON復帰
            MonitorPower.Monitor.PowerOn();

            // CSV出力ファイルクローズ（Block式に統一）
            opCollection.file.Close();

            opCollection.IsBusy.Value = false;
            opCollection.callbackMessageNormal("PsychoPyエンジン停止完了");

            _phase = EPhase.Idle;
        }

        #endregion

        #region Stop/IllegalExit チェック

        /// <summary>
        /// 非常停止 停止チェック（Block式に統一）
        /// </summary>
        /// <returns>Stop/EmergencyStopが検出された場合 true</returns>
        private bool CheckInteruptStop()
        {
            // 一回読むとNopになるのでステートマシン内のみ参照
            OpCollection.ECommand command = opCollection.Command;
            if (command == OpCollection.ECommand.EmergencyStop)
            {
                opCollection.sequencer.State = OpCollection.Sequencer.EState.EmergencyStop;
                _phase = EPhase.Stopping;
                return true;
            } // 緊急停止
            if (command == OpCollection.ECommand.Stop)
            {
                opCollection.sequencer.State = OpCollection.Sequencer.EState.Stop;
                _phase = EPhase.Stopping;
                return true;
            } // 中止
            else
                return false;
        }

        /// <summary>
        /// イリーガル退出および停止検査（Block式に統一）
        /// 給餌中退室も区別して検出
        /// </summary>
        /// <returns>イリーガル退室が検出された場合 true</returns>
        private bool CheckIllegalExit()
        {
            if (mainForm.Parent.OpFlagRoomOut)
            {
                if (mainForm.Parent.Feeding)
                {
                    // 給餌中退室
                    Debug.WriteLine("[PsychoPy] Exit after feeding detected");
                    opCollection.callbackMessageError("フィード中退室検知");
                    opCollection.flagFeed = true;
                    opCollection.sequencer.State = OpCollection.Sequencer.EState.ExitAfterFeedingDetection;
                }
                else
                {
                    // イリーガル退室
                    Debug.WriteLine("[PsychoPy] Illegal exit detected");
                    opCollection.callbackMessageError("イリーガル退室検知");
                    opCollection.flagFeed = false;
                    opCollection.sequencer.State = OpCollection.Sequencer.EState.IllegalExitDetection;
                }

                // CSV出力（イリーガル退室記録）
                opCollection.dateTimeout = DateTime.Now;
                opCollection.sequencer.callbackOutputFile(opCollection.sequencer.State);

                mainForm.Parent.OpFlagRoomIn = false;
                mainForm.Parent.OpFlagRoomOut = false;
                if (PreferencesDatOriginal.EnableFeedLamp) mainForm.Parent.OpSetFeedLampOff();
                KillPythonProcess();
                _phase = EPhase.OpenDoorForEntry;
                return true;
            }
            return false;
        }

        #endregion

        /// <summary>
        /// 外部からのクリーンアップ（BackgroundWorkerループ終了時に呼ばれる）
        /// </summary>
        public void Cleanup()
        {
            KillPythonProcess();
        }

        #region Pythonプロセス管理

        /// <summary>
        /// Pythonプロセスを強制終了
        /// </summary>
        private void KillPythonProcess()
        {
            if (_pythonProcess == null)
                return;

            try
            {
                if (!_pythonProcess.HasExited)
                {
                    _pythonProcess.Kill();
                    _pythonProcess.WaitForExit(3000);
                    Debug.WriteLine("[PsychoPy] Python process killed");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("[PsychoPy] Error killing Python process: " + ex.Message);
            }
            finally
            {
                _pythonProcess = null;
            }
        }

        #endregion

        #region エラーメッセージ

        /// <summary>
        /// stderrの内容を解析してユーザー向けエラーメッセージを構築する
        /// </summary>
        private string BuildErrorMessage(int exitCode, string stderr)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Pythonスクリプトがエラーで終了しました。");
            sb.AppendLine();

            var moduleMatch = Regex.Match(stderr, @"ModuleNotFoundError: No module named '([^']+)'");
            if (moduleMatch.Success)
            {
                string moduleName = moduleMatch.Groups[1].Value;
                string packageName = moduleName.Split('.')[0];

                sb.AppendLine("必要なパッケージ '" + packageName + "' がインストールされていません。");
                sb.AppendLine();
                sb.AppendLine("対処法:");
                sb.AppendLine("コマンドプロンプトで以下を実行してください:");
                sb.AppendLine();
                sb.AppendLine("  pip install " + packageName);
                sb.AppendLine();
                sb.AppendLine("--- エラー詳細 ---");
                sb.AppendLine(TruncateStderr(stderr));
                return sb.ToString();
            }

            if (stderr.Contains("SyntaxError"))
            {
                sb.AppendLine("スクリプトに構文エラーがあります。");
                sb.AppendLine("スクリプトの内容を確認してください。");
                sb.AppendLine();
                sb.AppendLine("--- エラー詳細 ---");
                sb.AppendLine(TruncateStderr(stderr));
                return sb.ToString();
            }

            if (stderr.Contains("FileNotFoundError"))
            {
                sb.AppendLine("スクリプトが参照するファイルが見つかりません。");
                sb.AppendLine();
                sb.AppendLine("--- エラー詳細 ---");
                sb.AppendLine(TruncateStderr(stderr));
                return sb.ToString();
            }

            sb.AppendLine("終了コード: " + exitCode);
            sb.AppendLine();
            if (!string.IsNullOrEmpty(stderr))
            {
                sb.AppendLine("--- エラー詳細 ---");
                sb.AppendLine(TruncateStderr(stderr));
            }
            else
            {
                sb.AppendLine("エラー詳細はありません。");
            }

            return sb.ToString();
        }

        private string TruncateStderr(string stderr)
        {
            const int maxLength = 1500;
            if (stderr.Length <= maxLength)
                return stderr;

            return "...(省略)...\n" + stderr.Substring(stderr.Length - maxLength);
        }

        /// <summary>
        /// UIスレッドでMessageBoxを表示
        /// </summary>
        private void ShowErrorMessageBox(string message)
        {
            try
            {
                mainForm.Parent.BeginInvoke((MethodInvoker)(() =>
                {
                    MessageBox.Show(mainForm.Parent, message,
                        "PsychoPyエンジンエラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }));
            }
            catch (Exception ex)
            {
                Debug.WriteLine("[PsychoPy] Failed to show error dialog: " + ex.Message);
            }
        }

        #endregion
    }
}
