using System;
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
    /// C#:     入室検知 → RFID読取 → ドア閉 → Python起動 → 結果受取 → 給餌判定 → ドア開 → 退室検知 → ループ
    /// Python: 課題だけ実行 → stdout に RESULT:CORRECT or RESULT:INCORRECT を出力 → 終了
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
        private bool _pythonExited = false;
        private int _pythonExitCode = 0;
        private string _currentRfid = "";
        private Stopwatch _phaseTimer = new Stopwatch();

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
                if (CheckStopCommand())
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

                // フラグリセット
                mainForm.Parent.OpFlagRoomIn = false;
                mainForm.Parent.OpFlagRoomOut = false;
                mainForm.Parent.OpeClearIdCode();

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

            // フラグリセット
            mainForm.Parent.OpFlagRoomIn = false;
            mainForm.Parent.OpFlagRoomOut = false;
            mainForm.Parent.OpeClearIdCode();

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

            if (mainForm.Parent.OpFlagOpenDoor)
            {
                // エラーチェック
                if (mainForm.Parent.OpResultOpenDoor != FormMain.EDeviceResult.None
                    && mainForm.Parent.OpResultOpenDoor != FormMain.EDeviceResult.Done)
                {
                    if (!PreferencesDatOriginal.IgnoreDoorError)
                    {
                        opCollection.callbackMessageError("ドアOPENエラー: " + mainForm.Parent.OpResultOpenDoor.ToString());
                        ShowErrorMessageBox("ドアOPENに失敗しました。\n結果: " + mainForm.Parent.OpResultOpenDoor.ToString());
                        _phase = EPhase.Stopping;
                        return;
                    }
                    Debug.WriteLine("[PsychoPy] Door open error ignored: " + mainForm.Parent.OpResultOpenDoor.ToString());
                }

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
            if (mainForm.Parent.OpFlagRoomIn)
            {
                Debug.WriteLine("[PsychoPy] Entry detected");
                opCollection.callbackMessageNormal("入室検知");
                opCollection.TimeEnterCage = DateTime.Now;

                mainForm.Parent.OpFlagRoomIn = false;
                _phase = EPhase.ReadRFID;
            }
        }

        /// <summary>
        /// RFID読取
        /// </summary>
        private void PhaseReadRFID()
        {
            _currentRfid = mainForm.Parent.OpeGetIdCode();

            if (string.IsNullOrEmpty(_currentRfid))
            {
                if (PreferencesDatOriginal.EnableNoIDOperation)
                {
                    _currentRfid = "NO_ID";
                    Debug.WriteLine("[PsychoPy] No RFID, EnableNoIDOperation=true, using NO_ID");
                }
                else
                {
                    // RFIDが読めない場合は繰り返しチェック（次のtickで再度チェック）
                    return;
                }
            }

            Debug.WriteLine("[PsychoPy] RFID: " + _currentRfid);
            opCollection.idCode = _currentRfid;
            opCollection.callbackSetUiCurrentIdCode(_currentRfid);
            opCollection.callbackMessageNormal("RFID: " + _currentRfid);

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
                _phase = EPhase.WaitDoorClose;
                opCollection.sequencer.State = OpCollection.Sequencer.EState.WaitingForCloseDoor;
            }
        }

        /// <summary>
        /// ドアが閉まるのを待つ
        /// </summary>
        private void PhaseWaitDoorClose()
        {
            if (mainForm.Parent.OpFlagCloseDoor)
            {
                // エラーチェック
                if (mainForm.Parent.OpResultCloseDoor != FormMain.EDeviceResult.None
                    && mainForm.Parent.OpResultCloseDoor != FormMain.EDeviceResult.Done)
                {
                    if (!PreferencesDatOriginal.IgnoreDoorError)
                    {
                        opCollection.callbackMessageError("ドアCLOSEエラー: " + mainForm.Parent.OpResultCloseDoor.ToString());
                        ShowErrorMessageBox("ドアCLOSEに失敗しました。\n結果: " + mainForm.Parent.OpResultCloseDoor.ToString());
                        _phase = EPhase.Stopping;
                        return;
                    }
                    Debug.WriteLine("[PsychoPy] Door close error ignored: " + mainForm.Parent.OpResultCloseDoor.ToString());
                }

                Debug.WriteLine("[PsychoPy] Door closed");
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

                _pythonProcess = new Process();
                _pythonProcess.StartInfo.FileName = "python";
                _pythonProcess.StartInfo.Arguments = "\"" + scriptPath + "\" " + _currentRfid;
                _pythonProcess.StartInfo.WorkingDirectory = Path.GetDirectoryName(scriptPath);
                _pythonProcess.StartInfo.UseShellExecute = false;
                _pythonProcess.StartInfo.RedirectStandardOutput = true;
                _pythonProcess.StartInfo.RedirectStandardError = true;
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
        /// Pythonプロセスの終了を待つ + イリーガル退室チェック
        /// </summary>
        private void PhaseWaitForPython()
        {
            // イリーガル退室チェック
            if (mainForm.Parent.OpFlagRoomOut)
            {
                Debug.WriteLine("[PsychoPy] Illegal exit detected during Python execution");
                opCollection.callbackMessageError("イリーガル退室検知 - Python中断");
                opCollection.sequencer.State = OpCollection.Sequencer.EState.IllegalExitDetection;

                KillPythonProcess();
                mainForm.Parent.OpFlagRoomOut = false;

                // 次の入室待ちへ
                _phase = EPhase.OpenDoorForEntry;
                return;
            }

            if (_pythonExited)
            {
                Debug.WriteLine("[PsychoPy] Python finished, moving to parse");
                mainForm.Parent.EpisodeMode.Value = false;
                _phase = EPhase.ParseResult;
            }
        }

        /// <summary>
        /// Pythonの結果をパース
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

                _pythonProcess = null;
                // エラーでも次の入室待ちへ続行
                _phase = EPhase.OpenDoorForExit;
                return;
            }

            // stdoutからRESULTをパース
            string stdout;
            lock (_stdoutBuffer) { stdout = _stdoutBuffer.ToString(); }

            bool isCorrect = false;
            bool resultFound = false;

            // 最後に出現した RESULT: 行を採用
            string[] lines = stdout.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = lines.Length - 1; i >= 0; i--)
            {
                string line = lines[i].Trim();
                if (line == "RESULT:CORRECT")
                {
                    isCorrect = true;
                    resultFound = true;
                    break;
                }
                else if (line == "RESULT:INCORRECT")
                {
                    isCorrect = false;
                    resultFound = true;
                    break;
                }
            }

            if (!resultFound)
            {
                Debug.WriteLine("[PsychoPy] No RESULT line found in stdout");
                opCollection.callbackMessageError("Python出力にRESULT行がありません");
                // RESULT行がない場合はINCORRECTとして扱う
                isCorrect = false;
            }

            opCollection.trialCount++;
            opCollection.callbackSetUiCurentNumberOfTrial(opCollection.trialCount);

            _pythonProcess = null;

            if (isCorrect)
            {
                Debug.WriteLine("[PsychoPy] Result: CORRECT");
                opCollection.callbackMessageNormal("課題結果: 正解 (trial " + opCollection.trialCount + ")");
                opCollection.taskResultVal = OpCollection.ETaskResult.Ok;
                _phase = EPhase.Feeding;
            }
            else
            {
                Debug.WriteLine("[PsychoPy] Result: INCORRECT");
                opCollection.callbackMessageNormal("課題結果: 不正解 (trial " + opCollection.trialCount + ")");
                opCollection.taskResultVal = OpCollection.ETaskResult.Ng;
                _phase = EPhase.OpenDoorForExit;
            }
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
                _phase = EPhase.WaitDoorOpenForExit;
            }
        }

        /// <summary>
        /// 退室用ドアオープン待ち
        /// </summary>
        private void PhaseWaitDoorOpenForExit()
        {
            opCollection.sequencer.State = OpCollection.Sequencer.EState.WaitingForOpenDoor;

            if (mainForm.Parent.OpFlagOpenDoor)
            {
                if (mainForm.Parent.OpResultOpenDoor != FormMain.EDeviceResult.None
                    && mainForm.Parent.OpResultOpenDoor != FormMain.EDeviceResult.Done)
                {
                    if (!PreferencesDatOriginal.IgnoreDoorError)
                    {
                        opCollection.callbackMessageError("退室ドアOPENエラー: " + mainForm.Parent.OpResultOpenDoor.ToString());
                        ShowErrorMessageBox("退室用ドアOPENに失敗しました。\n結果: " + mainForm.Parent.OpResultOpenDoor.ToString());
                        _phase = EPhase.Stopping;
                        return;
                    }
                }

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

            opCollection.IsBusy.Value = false;
            opCollection.callbackMessageNormal("PsychoPyエンジン停止完了");

            _phase = EPhase.Idle;
        }

        #endregion

        #region Stop/Emergency処理

        /// <summary>
        /// 毎tick呼ばれ、Stop/EmergencyStop コマンドを検査
        /// </summary>
        /// <returns>Stopが検出された場合 true</returns>
        private bool CheckStopCommand()
        {
            OpCollection.ECommand command = opCollection.Command;

            if (command == OpCollection.ECommand.Stop || command == OpCollection.ECommand.EmergencyStop)
            {
                Debug.WriteLine("[PsychoPy] Stop command received: " + command.ToString());

                if (command == OpCollection.ECommand.EmergencyStop)
                {
                    opCollection.sequencer.State = OpCollection.Sequencer.EState.EmergencyStop;
                }
                else
                {
                    opCollection.sequencer.State = OpCollection.Sequencer.EState.Stop;
                }

                _phase = EPhase.Stopping;
                return true;
            }

            return false;
        }

        #endregion

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
