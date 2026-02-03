using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Compartment
{
    class UcOperationInternal
    {
        readonly ParentHelper<FormMain> mainForm = new ParentHelper<FormMain>();
        DateTime dt = new DateTime();
        //private readonly PreferencesDat preferencesDatOriginal;

        public enum EProgrammableState
        {

            [EnumDisplayName("アイドル")]
            Idle,
            /// <summary>
            /// 処理待ち
            /// </summary>
            [EnumDisplayName("処理待ち")]
            Wait,
            [EnumDisplayName("タイムアウト")]
            Timeout,
            [EnumDisplayName("タッチ待ち")]
            TouchWait,
            [EnumDisplayName("退出待ち")]
            ExitWait,

        }


        private ref PreferencesDat PreferencesDatOriginal
        {
            get
            {
                return ref mainForm.Parent.preferencesDatOriginal;
            }
        }
        private ref OpCollection opCollection => ref mainForm.Parent.opCollection;

        private OpCollection.Sequencer.EState lastState = OpCollection.Sequencer.EState.Init;
        private int debugLogCounter = 0; // デバッグログ用カウンター
        /// <summary>
        /// UcOperationInternalコンストラクタ
        /// </summary>
        /// <param name="baseForm">BaseFormインスタンス</param>
        public UcOperationInternal(FormMain baseForm)
        {
            mainForm.SetParent(baseForm);
            //preferencesDatOriginal = mainForm.Parent.preferencesDatOriginal;

            //opCollection = mainForm.Parent.opCollection;
        }

        /// <summary>
        /// Invoke必要があったら呼ぶ
        /// </summary>
        /// <param name="act"></param>
        private void InvokeMethod(Action act)
        {
            if (mainForm.Parent.InvokeRequired)
                mainForm.Parent.Invoke(new System.Windows.Forms.MethodInvoker(act));
            else
                act();
        }

        public Action OperationExecute = () => { };

        /// <summary>
        /// 全体ステートマシン
        /// </summary>
        public void OnOperationStateMachineProc()
        {
            // デバッグ：状態とコマンドを定期的にログ出力（大量出力に注意）
            // 1秒に1回だけログを出力
            // 注意：opCollection.Command は読み取ると Nop にリセットされるので、診断ログでは読まない
            if (debugLogCounter++ % 1000 == 0)
            {
                System.Diagnostics.Debug.WriteLine($"[OnOperationStateMachineProc] State={opCollection.sequencer.State}, IsBusy={opCollection.IsBusy.Value}");
            }

            switch (opCollection.sequencer.State)
            {
                // State部とプログラマブル部
                case OpCollection.Sequencer.EState.Init:
                    Init();
                    break;

                case OpCollection.Sequencer.EState.Idle:
                    Idle();
                    break;

                #region デバイススタンバイ

                case OpCollection.Sequencer.EState.DeviceStandbyBegin:
                    DeviceStandbyBegin();
                    break;
                case OpCollection.Sequencer.EState.DeviceStandby2:
                    DeviceStandby2();
                    break;
                case OpCollection.Sequencer.EState.DeviceStandbyEnd:
                    DeviceStandbyEnd();
                    break;

                #endregion

                #region 停止系
                case OpCollection.Sequencer.EState.Stop:
                    // 停止
                    Stop();
                    OperationTaskOnceRun = false;
                    UserStopProgrammableProc();
                    break;

                case OpCollection.Sequencer.EState.EmergencyStop:
                    // 緊急停止
                    EmergencyStop();
                    OperationTaskOnceRun = false;
                    UserStopProgrammableProc();
                    break;

                case OpCollection.Sequencer.EState.IncorrectTouchExit:
                    // 不正解タッチ停止
                    //OperationTaskOnceRun = false;
                    opCollection.sequencer.State = OpCollection.Sequencer.EState.IncorrectTouchAfterWait;
                    break;
                #endregion 停止系

                #region 入室
                case OpCollection.Sequencer.EState.PreEnterCageProc:
                    if (CheckInteruptStop())
                        break;
                    // 入室前処理
                    PreEnterCageProc();
                    break;

                case OpCollection.Sequencer.EState.WaitingForEnterCage:
                    if (CheckInteruptStop())
                        break;
                    // 入室待ち
                    WaitingForEnterCage();
                    break;

                case OpCollection.Sequencer.EState.WaitingForOutLever:
                    if (CheckInteruptStop())
                        break;
                    if (CheckIllegalExit())
                        break;
                    WaitingForOutLever();
                    break;

                case OpCollection.Sequencer.EState.WaitingForDownLever:
                    if (CheckInteruptStop())
                        break;
                    if (CheckIllegalExit())
                        break;
                    // レバーDOWN待ち
                    WaitingForDownLever();
                    break;

                case OpCollection.Sequencer.EState.WaitingForInLever:
                    if (CheckInteruptStop())
                        break;
                    if (CheckIllegalExit())
                        break;
                    WaitingForInLever();
                    break;

                case OpCollection.Sequencer.EState.WaitingForCloseDoor:
                    if (CheckInteruptStop())
                        break;
                    if (CheckIllegalExit())
                        break;
                    // ドアCLOSE待ち
                    WaitingForCloseDoor();
                    break;

                #endregion

                #region 退室
                case OpCollection.Sequencer.EState.PreOpenDoorProc:
                    if (CheckInteruptStop())
                        break;
                    PreOpenDoorProc();
                    break;
                case OpCollection.Sequencer.EState.WaitingForOpenDoor:
                    if (CheckInteruptStop())
                        break;
                    if (CheckLeaveTimeout())
                        break;
                    WaitingForOpenDoor();
                    break;
                case OpCollection.Sequencer.EState.PreLeaveCageProc:
                    if (CheckInteruptStop())
                        break;
                    if (CheckLeaveTimeout())
                        break;
                    PreLeaveCageProc();
                    break;
                case OpCollection.Sequencer.EState.WaitingForLeaveCage:
                    if (CheckInteruptStop())
                        break;
                    if (CheckLeaveTimeout())
                        break;
                    WaitingForLeaveCage();
                    break;
                case OpCollection.Sequencer.EState.IllegalExitDetection:
                    IllegalExitDetection();
                    //Block側イリーガル検出したことを伝搬する
                    mainForm.Parent.uob.DetectIllegalExit();
                    break;
                case OpCollection.Sequencer.EState.ExitAfterFeedingDetection:
                    mainForm.Parent.uob.DetectIllegalExit();
                    ExitAfterFeedingDetection();
                    OnStopStateProgrammableProc();
                    break;
                #endregion 退室

                #region イリーガル退室後待ち
                case OpCollection.Sequencer.EState.IllegalExitDetection_WaitReEnter:
                    IllegalExitDetection_WaitReEnter();
                    OperationTaskOnceRun = false;
                    if (CheckInteruptStop())
                    {
                        OnStopStateProgrammableProc();
                        break;
                    }
                    break;

                case OpCollection.Sequencer.EState.IllegalExitDetection_TrainingWait:
                    IllegalExitDetection_TrainingWait();
                    OperationTaskOnceRun = false;
                    break;
                #endregion

                case OpCollection.Sequencer.EState.Error:
                    break;
                case OpCollection.Sequencer.EState.WaitingForTimeoutLeaveCage:
                    break;
                default:
                    if (CheckInteruptStop())
                    {
                        OnStopStateProgrammableProc();
                        break;
                    }
                    if (CheckIllegalExit())
                    {
                        OnStopStateProgrammableProc();
                        break;
                    }
                    // 指定ステート以外はCodeBuilder処理を実行
                    if (!mainForm.Parent.uob.OperationStatus || !OperationTaskOnceRun)
                    {
                        if (opCollection.trialCount < PreferencesDatOriginal.OpeNumberOfTrial || PreferencesDatOriginal.OpeNumberOfTrial == 0)
                        {
                            try
                            {
                                // マーモセット検出時ここで検出行う？入室後
                                //OnOperationStateProgrammableProc();
                                if (opCollection.idCode != "")
                                {
                                    OnOperationStateProgrammableProcId(opCollection.idCode);
                                }
                                else
                                {
                                    // const値を上位から受け取る
                                    // EnableNoIdOperation時のみ実行
                                    if (PreferencesDatOriginal.EnableNoIDOperation)
                                    {
                                        OnOperationStateProgrammableProcId("0");
                                    }
                                    else
                                    {
                                        // メッセージ表示はWaitingForEnterCage()側で

                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                opCollection.callbackMessageError(ex.Message + " " + ex.InnerException);
                                opCollection.sequencer.State = OpCollection.Sequencer.EState.EmergencyStop;
                                return;
                            }
                            OperationTaskOnceRun = true;
                        }
                    }
                    else
                    {
                        Thread.Sleep(10);
                        //opCollection.callbackMessageDebug("Task実行中");
                    }

                    // 設定試行回数により終了
                    if (opCollection.trialCount >= PreferencesDatOriginal.OpeNumberOfTrial && PreferencesDatOriginal.OpeNumberOfTrial != 0)
                    {
                        opCollection.callbackMessageNormal(string.Format("設定試行回数({0}/{1})により給餌終了", opCollection.trialCount, PreferencesDatOriginal.OpeNumberOfTrial));
                        opCollection.sequencer.callbackOutputFile(opCollection.sequencer.State); // ファイル出力
                        opCollection.sequencer.State = OpCollection.Sequencer.EState.PreLeaveCageProc;
                    }
                    // 入室報酬用
                    else if (mainForm.Parent.OpeGetTypeOfTask() == ECpTask.UnConditionalFeeding && !Program.EnableNewEngine)
                    {
                        opCollection.callbackMessageNormal(string.Format("入室給餌終了"));
                        opCollection.sequencer.callbackOutputFile(opCollection.sequencer.State); // ファイル出力
                        opCollection.sequencer.State = OpCollection.Sequencer.EState.PreLeaveCageProc;
                    }
                    // 試行継続
                    else
                    {
                        //opCollection.callbackMessageNormal(string.Format("給餌完了試行継続({0}/{1})", opCollection.trialCount, preferencesDatOriginal.OpeNumberOfTrial));
                        //opCollection.sequencer.State = OpCollection.Sequencer.EState.InitInterval;
                    }

                    break;
            }
        }
        bool OperationTaskOnceRun = false;
        /// <summary>
        /// プログラム可能部ステートマシン
        /// 内部にCodeBuilderを展開する
        /// ステート内包をやめる為仕様を変える
        /// </summary>
        public Action OnOperationStateProgrammableProc = () => { };
        public Action<string> OnOperationStateProgrammableProcId = (x) => { };

        public void SetStateFunctionId()
        {
            if (PreferencesDatOriginal.OpeNumberOfTrial == 0)
                mainForm.Parent.uob.Infinity = true;
            else
                mainForm.Parent.uob.Infinity = false;
            OnOperationStateProgrammableProcId = (id) =>
            {
                try
                {
                    bool result = mainForm.Parent.uob.OperationProcIds.TryGetValue(id, out var proc);
                    if (result)
                    {
                        proc();
                    }
                    else
                    {
                        // 登録IDが無い場合はデフォルトIDを実行
                        if (PreferencesDatOriginal.EnableNoEntryIDOperation)
                        {
                            result = mainForm.Parent.uob.OperationProcIds.TryGetValue("0", out proc);
                            proc();
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            };
        }
        public void SetStateFunction()
        {
            //bool a = true;
            if (PreferencesDatOriginal.OpeNumberOfTrial == 0)
                mainForm.Parent.uob.Infinity = true;
            else
                mainForm.Parent.uob.Infinity = false;
            OnOperationStateProgrammableProc = () =>
            {
                mainForm.Parent.uob.OperationProc();
                //mainForm.Parent.uob.DrawScreenReset();
                ////mainForm.Parent.uob.TouchDelay(2000);

                ////mainForm.Parent.uob.DrawScreenBackColor();
                ////mainForm.Parent.uob.Delay(1000);
                ////mainForm.Parent.uob.ViewCorrectImage();
                ////mainForm.Parent.uob.Delay(1000);
                ////mainForm.Parent.uob.DrawScreenBackColor();
                ////mainForm.Parent.uob.Delay(1000);
                ////mainForm.Parent.uob.ViewCorrectWrongImage();
                ////mainForm.Parent.uob.WaitCorrectTouchTrigger();
                ////mainForm.Parent.uob.PlaySound();

                //mainForm.Parent.uob.ViewTriggerImage();
                //mainForm.Parent.uob.WaitTouchTrigger();

                //mainForm.Parent.uob.DrawScreenReset();
                //mainForm.Parent.uob.Delay(1500);
                //mainForm.Parent.uob.PlaySound();
                //mainForm.Parent.uob.Delay(2000);
                //mainForm.Parent.uob.FeedLamp(2000);
                //mainForm.Parent.uob.Delay(1500);
                //mainForm.Parent.uob.Feed(2000);
                //mainForm.Parent.uob.FeedSound(1500);
                ////mainForm.Parent.uob.Delay(1000);
                //mainForm.Parent.uob.OutputResult();
                //mainForm.Parent.uob.TouchDelay(1000);
                //mainForm.Parent.uob.Start();

                ////OnOperationStateProgrammableProc();
            };
        }

        public void OnStopStateProgrammableProc()
        {
            mainForm.Parent.camImage.StopCapture();
            mainForm.Parent.uob.Stop();
        }
        public void UserStopProgrammableProc()
        {
            // 停止時Episode記憶時間内回数リセット 既実行だったらcount=1とする

            // ボタン停止時 非常停止時のみ処理したい
            if (mainForm.Parent.idControlHelper.GetCount(opCollection.idCode) > 0)
            {
                mainForm.Parent.idControlHelper.ResetCount(opCollection.idCode);
                // +1にしておく
                mainForm.Parent.idControlHelper.AddCount(opCollection.idCode);
            }
            // Camera ImageDetection stop
            if (mainForm.Parent.camImage.DetectionStarted)
            {
                mainForm.Parent.camImage.StopDetectFromCamImage();
            }
            else if (mainForm.Parent.camImage.CaptureStarted)
            {
                mainForm.Parent.camImage.StopSavingFromCamImage();
            }
        }

        // block用カテゴリ

        #region For Block Function

        internal static int GetRandomTime(int min, int max)
        {
            int minTime;
            int maxTime;
            //globalじゃないから同じTick時同じ数値
            Random randomValue = new System.Random();

            minTime = min;
            if (min <= 0)
            {
                minTime = 0;
            }
            maxTime = max;
            if (max <= 0)
            {
                maxTime = 0;
            }
            // ださい
            if (minTime > maxTime)
            {
                maxTime = minTime;
            }
            int ret = randomValue.Next(minTime, maxTime + 1);
            return ret;
        }

        void OnEventFunc(EProgrammableState state)
        {
            switch (state)
            {
                case EProgrammableState.Idle:
                    break;
                case EProgrammableState.Wait:
                    break;
                case EProgrammableState.Timeout:
                    break;
                case EProgrammableState.TouchWait:
                    // タッチ待ち動作
                    break;
                case EProgrammableState.ExitWait:
                    //退出待ち動作
                    break;
                default:
                    break;
            }

        }

        #endregion

        // 切り替えるOperationFunctionは
        // PreTouchTriggerScreenProc ～ Illegal部
        // 固定ステートマシン部は従来通りとして
        // 非常停止時イリーガル退出時の処理考慮する

        #region OperationFunction

        private void Init()
        {
            // カメラ初期化時有効
            opCollection.EnableCamera = true;
            opCollection.sequencer.State = OpCollection.Sequencer.EState.Idle;
        }
        private void Idle()
        {
            // 注意：opCollection.Command は読み取ると Nop にリセットされるので、一度だけ読み取る
            OpCollection.ECommand command = opCollection.Command;

            // デバッグ：Idle()が呼ばれているか確認（大量出力される）
            if (debugLogCounter % 1000 == 0)
            {
                System.Diagnostics.Debug.WriteLine($"[UcOperationInternal Idle()] Called. Command={command}");
            }

            // 開始
            if (command == OpCollection.ECommand.Start)
            {
                System.Diagnostics.Debug.WriteLine($"[UcOperationInternal Idle] Start command received. EnableDebugMode={PreferencesDatOriginal.EnableDebugMode}");

                // 多層化対応 id 認識時に変更
                //SetStateFunction();
                SetStateFunctionId();
                opCollection.IsBusy.Value = true;

                //eDoor自動開始（デバッグモードでも有効化して、IoMicrochipDummyExとの連携を動かす）
                mainForm.Parent.eDoor.Enable = true;
                System.Diagnostics.Debug.WriteLine("[UcOperationInternal Idle] eDoor enabled");

                // ファイルオープン（デバッグモード時はエラーハンドリング）
                try
                {
                    opCollection.file.Open(PreferencesDatOriginal.OutputResultFile);
                    System.Diagnostics.Debug.WriteLine($"[UcOperationInternal Idle] File opened: {PreferencesDatOriginal.OutputResultFile}");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[UcOperationInternal Idle] File open error: {ex.Message}");
                    if (PreferencesDatOriginal.EnableDebugMode)
                    {
                        System.Diagnostics.Debug.WriteLine("[UcOperationInternal Idle] デバッグモードのため、ファイルオープンエラーを無視して続行");
                        opCollection.callbackMessageNormal("[デバッグモード] ファイルオープンに失敗しましたが続行します");
                    }
                    else
                    {
                        throw; // デバッグモードでない場合はエラーを再スロー
                    }
                }

                //初期回数表示
                opCollection.trialCount = 0; // 試行回数カウンタをクリア
                opCollection.callbackSetUiCurentNumberOfTrial(opCollection.trialCount);
                opCollection.callbackMessageNormal("スタート押された認識(Internal)");

                System.Diagnostics.Debug.WriteLine("[UcOperationInternal Idle] Transitioned to PreEnterCageProc");
                opCollection.sequencer.State = OpCollection.Sequencer.EState.PreEnterCageProc;
            }

        }
        private void DeviceStandbyBegin()
        {
            System.Diagnostics.Debug.WriteLine($"[UcOperationInternal DeviceStandbyBegin] EnableDebugMode={PreferencesDatOriginal.EnableDebugMode}");

            // デバッグモードの場合、デバイススタンバイをスキップ
            if (PreferencesDatOriginal.EnableDebugMode)
            {
                opCollection.callbackMessageNormal("[デバッグモード] デバイススタンバイをスキップ");
                System.Diagnostics.Debug.WriteLine("[UcOperationInternal デバッグモード] デバイススタンバイをスキップ");

                // レバーとランプを初期化
                InvokeMethod(() =>
                {
                    mainForm.Parent.OpMoveLeverIn(); // レバーをIN
                    mainForm.Parent.OpSetRoomLampOff(); // 天井ランプをOFF
                    mainForm.Parent.OpDrawBackColorBlackOnTouchPanel(); // タッチパネル画面を黒
                    mainForm.Parent.OpSetFeedLampOff(); // フィードランプをOFF
                    mainForm.Parent.OpSetLeverLampOff(); // レバーランプをOFF
                    System.Diagnostics.Debug.WriteLine("[UcOperationInternal デバッグモード] デバイスを初期化（レバー、ランプ）");
                });

                // 課題開始時にドアを開く（重要！）
                // デバッグモードでは DisableDoor 設定を無視して常にドアを開く
                InvokeMethod(() =>
                {
                    mainForm.Parent.OpOpenDoor(); // ドアをOPEN
                    System.Diagnostics.Debug.WriteLine("[UcOperationInternal デバッグモード] ドアを開きます");
                });

                System.Diagnostics.Debug.WriteLine("[UcOperationInternal デバッグモード] DeviceStandbyEnd に遷移");
                opCollection.sequencer.State = OpCollection.Sequencer.EState.DeviceStandbyEnd;
                return;
            }

            opCollection.callbackMessageNormal("デバイススタンバイ開始");
            InvokeMethod(() =>
            // 処理内容
            {
                mainForm.Parent.OpMoveLeverIn(); // レバーをIN

                mainForm.Parent.OpSetRoomLampOff(); // 天井ランプをOFF
                mainForm.Parent.OpDrawBackColorBlackOnTouchPanel(); // タッチパネル画面を黒
                mainForm.Parent.OpSetFeedLampOff(); // フィードランプをOFF
                mainForm.Parent.OpSetLeverLampOff(); // レバーランプをOFF
                mainForm.Parent.OpAirPuffOff(); // エアパフをOFF
                mainForm.Parent.OpSetFeedOff(); // FeederをOFF
                mainForm.Parent.OpSetFeed2Off(); // Feeder2をOFF

                if (!mainForm.Parent.camImage.CamOpened
                && (PreferencesDatOriginal.EnableMamosetDetection || PreferencesDatOriginal.EnableMamosetDetectionSaveImageMode || PreferencesDatOriginal.EnableMamosetDetectionSaveOnlyMode))
                {
                    // カメラ接続 出来なかったら使用しない
                    opCollection.callbackMessageNormal("カメラ接続開始");
                    try
                    {
                        if (PreferencesDatOriginal.SelectCamUri != "" && PreferencesDatOriginal.SelectCamUri != null && opCollection.EnableCamera)
                        {
                            mainForm.Parent.camImage.ConnectCam(new UriBuilder(PreferencesDatOriginal.SelectCamUri));
                            opCollection.callbackMessageNormal("カメラ接続完了 X: " + mainForm.Parent.camImage.ResolutionX.ToString() + " Y: " + mainForm.Parent.camImage.ResolutionY.ToString()
                                    + " framerate: " + mainForm.Parent.camImage.FramerateLimit.ToString() + "fps");
                        }
                        else if (!opCollection.EnableCamera)
                        {
                            opCollection.callbackMessageNormal("カメラ未検出の為、カメラ接続スキップ.再度接続する場合はソフトを再起動してください.");
                        }
                        else
                        {
                            opCollection.callbackMessageNormal("カメラ設定が無効です.");
                        }
                    }
                    catch (Exception ex)
                    {
                        //エラー時無効 Init時に再度有効化
                        opCollection.EnableCamera = false;
                        opCollection.callbackMessageError(ex.Message);
                        opCollection.callbackMessageError("カメラに接続できませんでした.");
                    }
                }
            });
            if (PreferencesDatOriginal.DisableLever)
            {
                // DisableDoor時操作しない
                if (!PreferencesDatOriginal.DisableDoor)
                {
                    InvokeMethod(() =>
                    {
                        mainForm.Parent.OpOpenDoor(); // ドアをOPEN
                    });
                }
                else
                {
                    opCollection.callbackMessageNormal("ドア無効ユーザー入室");
                }
                opCollection.sequencer.State = OpCollection.Sequencer.EState.DeviceStandbyEnd;
            }
            else
            {
                opCollection.sequencer.State = OpCollection.Sequencer.EState.DeviceStandby2;
            }
        }
        private void DeviceStandby2()
        {
            // デバイススタンバイ中

            // レバーIN待ち
            if (mainForm.Parent.OpFlagMoveLeverIn == true)
            {
                if (!PreferencesDatOriginal.DisableLever)
                {
                    opCollection.callbackMessageNormal("レバーIN完了");
                }

                if (!PreferencesDatOriginal.DisableDoor)
                {
                    InvokeMethod(() =>
                    {
                        mainForm.Parent.OpOpenDoor(); // ドアをOPEN
                    });
                }

                opCollection.sequencer.State = OpCollection.Sequencer.EState.DeviceStandbyEnd;
            }

        }
        private void DeviceStandbyEnd()
        {
            // デバッグモードの場合、即座に完了
            if (PreferencesDatOriginal.EnableDebugMode)
            {
                opCollection.callbackMessageNormal("[デバッグモード] デバイススタンバイ完了");
                System.Diagnostics.Debug.WriteLine("[UcOperationInternal デバッグモード] デバイススタンバイ完了");

                opCollection.sequencer.LoadState();

                // Stop状態の場合、停止処理を完了してIdleに戻る
                if (opCollection.sequencer.State == OpCollection.Sequencer.EState.Stop)
                {
                    System.Diagnostics.Debug.WriteLine("[UcOperationInternal デバッグモード] Stop状態検出、停止完了処理を実行");
                    opCollection.callbackMessageNormal("停止完了");
                    opCollection.file.Close();
                    opCollection.IsBusy.Value = false;
                    opCollection.sequencer.State = OpCollection.Sequencer.EState.Idle;
                }

                return;
            }

            if (mainForm.Parent.OpFlagOpenDoor == true)
            {
                opCollection.callbackMessageNormal("ドアOPEN完了");
                opCollection.sequencer.LoadState();
            }
            if (PreferencesDatOriginal.DisableDoor)
            {
                opCollection.callbackMessageNormal("ドア無効待機完了");
                opCollection.sequencer.LoadState();
            }
        }
        private void Stop()
        {
            opCollection.callbackMessageNormal("Stop認識(Internal)");
            mainForm.Parent.UpdateIllegalStatus();

            //eDoor自動停止
            mainForm.Parent.eDoor.Enable = false;

            //CamImage camera停止
            mainForm.Parent.camImage.StopMediaPlay();

            if (!mainForm.Parent.uob.OperationStatus)
            {
                if (opCollection.sequencer.BeforeState == OpCollection.Sequencer.EState.DeviceStandbyEnd)
                {
                    opCollection.file.Close();
                    opCollection.IsBusy.Value = false;
                    opCollection.sequencer.State = OpCollection.Sequencer.EState.Idle;
                }
                else
                {
                    opCollection.sequencer.StoreStateStack();
                    opCollection.sequencer.State = OpCollection.Sequencer.EState.DeviceStandbyBegin;
                }
            }
        }
        private void EmergencyStop()
        {
            mainForm.Parent.UpdateIllegalStatus();
            if (!mainForm.Parent.uob.OperationStatus)
            {
                opCollection.file.Close();
                opCollection.IsBusy.Value = false;
                opCollection.sequencer.State = OpCollection.Sequencer.EState.Idle;
            }
        }
        private void PreEnterCageProc()
        {
            if (opCollection.sequencer.BeforeState == OpCollection.Sequencer.EState.DeviceStandbyEnd)
            {
                InvokeMethod(() =>
                {
                    mainForm.Parent.OpeClearIdCode();

                    mainForm.Parent.ClearEpisodeStatus();
                });
                DateTime dt = DateTime.Now;

                opCollection.trialCount = 0; // 試行回数カウンタをクリア
                opCollection.callbackSetUiCurentNumberOfTrial(opCollection.trialCount);

                opCollection.sequencer.State = OpCollection.Sequencer.EState.WaitingForEnterCage;
                mainForm.Parent.OpFlagRoomIn = false;
            }
            else
            {
                opCollection.sequencer.StoreStateStack();
                opCollection.sequencer.State = OpCollection.Sequencer.EState.DeviceStandbyBegin;
            }
        }
        private void WaitingForEnterCage()
        {
            // モニタースタンバイ
            if ((DateTime.Now - dt).TotalMinutes > PreferencesDatOriginal.MonitorSaveTime && PreferencesDatOriginal.EnableMonitorSave)
            {
                opCollection.callbackMessageNormal(string.Format("待機時間{0}[秒]経過 モニターオフ)", (DateTime.Now - dt).TotalMinutes));
                dt = DateTime.Now;
                MonitorPower.Monitor.PowerOff();
            }
#if NO_TRIGGER_TOUCH
                        //debug
                        opCollection.sequencer.State = OpCollection.Sequencer.EState.WaitingForOutLever;
#endif
            // 入室検知 (ドア無効時 ドア手動閉追加)
            if (mainForm.Parent.OpFlagRoomIn == true || (PreferencesDatOriginal.DisableDoor && (mainForm.Parent.devInDoorClose.dInCurrent == true || mainForm.Parent.devInDoorOpen.dInCurrent == false)))
            {
                MonitorPower.Monitor.PowerOn();
                opCollection.callbackMessageNormal("入室検知");
                opCollection.TimeEnterCage = DateTime.Now;

                // 入室検知時に 回数カウント表示
                // opCollection.callbackSetUiCurentNumberOfTrial(opCollection.trialCount);
                InvokeMethod(() =>
                {
                    opCollection.idCode = mainForm.Parent.OpeGetIdCode();
                    opCollection.callbackSetUiCurrentIdCode(opCollection.idCode);

                    if (!PreferencesDatOriginal.DisableLever)
                    {
                        mainForm.Parent.OpMoveLeverOut(); // レバーをOUT
                    }
                });

                ////入室時 IdControlHelperID更新
                //if (opCollection.idCode != "")
                //{
                //    mainForm.Parent.idControlHelper.UpdateEntry(opCollection.idCode);
                //}

                mainForm.Parent.OpFlagRoomOut = false;


                // IlegalExit時ステートを再開
                if (lastState != OpCollection.Sequencer.EState.Init)
                {
                    // lastState初期化
                    lastState = OpCollection.Sequencer.EState.Init;
                }

                // Camera ImageDetection
                if (mainForm.Parent.camImage.CamOpened)
                {
                    if (PreferencesDatOriginal.EnableMamosetDetection && !mainForm.Parent.camImage.DetectionStarted)
                    {
                        Task detectTask = new Task(() =>
                        {
                            mainForm.Parent.camImage.StartDetectFromCamImage(PreferencesDatOriginal.SelectCamUri);
                        });
                        detectTask.Start();
                    }
                    else if (PreferencesDatOriginal.EnableMamosetDetectionSaveImageMode && !mainForm.Parent.camImage.CaptureStarted)
                    {
                        Task detectSaveTask = new Task(() =>
                        {
                            mainForm.Parent.camImage.StartSavingFromCamImage(PreferencesDatOriginal.SelectCamUri, true);
                        });
                        detectSaveTask.Start();
                    }

                    else if (PreferencesDatOriginal.EnableMamosetDetectionSaveOnlyMode && !mainForm.Parent.camImage.CaptureStarted)
                    {
                        Task detectSaveTask = new Task(() =>
                        {
                            mainForm.Parent.camImage.StartSavingFromCamImage(PreferencesDatOriginal.SelectCamUri, false);
                        });
                        detectSaveTask.Start();
                    }
                    mainForm.Parent.camImage.StartCapture();
                }

                //Task止める

                if (mainForm.Parent.uob.OperationStatus)
                {
                    // uob側動作していたら止める？
                    OnStopStateProgrammableProc();
                }
                else
                {
                    opCollection.sequencer.State = OpCollection.Sequencer.EState.PreTouchTriggerScreenProc;
                    //opCollection.sequencer.State = OpCollection.Sequencer.EState.RunProcessBlock;
                }

                //else if (lastState == OpCollection.Sequencer.EState.WaitingForTouchTriggerScreen)
                //{
                //    opCollection.sequencer.State = OpCollection.Sequencer.EState.PreTouchTriggerScreenProc;
                //}
                if (!PreferencesDatOriginal.DisableLever)
                {
                    opCollection.sequencer.State = OpCollection.Sequencer.EState.WaitingForOutLever;
                }

                // RecentID
                if (opCollection.idCode != "")
                {
                    mainForm.Parent.recentIdHelper.AddEntry(opCollection.idCode);
                }

                // MultiID アナウンス
                if (opCollection.idCode != "")
                {
                    var fileRelatedActionParam = mainForm.Parent.ucOperationDataStore.GetEntry(opCollection.idCode);
                    if (fileRelatedActionParam != null)
                    {
                        string fileName = Path.GetFileName(fileRelatedActionParam.FilePath);
                        opCollection.MultiIdProgName = fileName;
                        opCollection.callbackMessageNormal("入室ID:" + opCollection.idCode + " 実行ファイル:" + fileName);
                    }
                    else
                    {
                        // 登録のないID検知時
                        if (PreferencesDatOriginal.EnableNoEntryIDOperation)
                        {
                            fileRelatedActionParam = mainForm.Parent.ucOperationDataStore.GetEntry("0");
                            string fileName = Path.GetFileName(fileRelatedActionParam.FilePath);
                            opCollection.MultiIdProgName = fileName;
                            opCollection.callbackMessageNormal("入室ID:" + opCollection.idCode + " 未登録 実行ファイル:" + fileName);
                        }
                        else
                        {
                            opCollection.callbackMessageNormal("未登録ID実行無効");
                            opCollection.sequencer.State = OpCollection.Sequencer.EState.PreLeaveCageProc;
                        }
                    }
                }
                else
                {
                    if (PreferencesDatOriginal.EnableNoIDOperation)
                    {
                        string defaultProgID = "0";
                        var fileRelatedActionParam = mainForm.Parent.ucOperationDataStore.GetEntry(defaultProgID);
                        fileRelatedActionParam = mainForm.Parent.ucOperationDataStore.GetEntry(defaultProgID);
                        string fileName = Path.GetFileName(fileRelatedActionParam.FilePath);
                        opCollection.MultiIdProgName = fileName;

                        opCollection.callbackMessageNormal("ID無し実行有効" + " 実行ファイル:" + fileName);
                    }
                    else
                    {
                        opCollection.callbackMessageNormal("ID無し実行無効");
                        opCollection.sequencer.State = OpCollection.Sequencer.EState.PreLeaveCageProc;
                    }
                }
            }
        }

        private void WaitingForOutLever()
        {

            if (PreferencesDatOriginal.DisableLever)
            {
                opCollection.stopwatch.Restart(); // ストップウォッチをリスタート
                mainForm.Parent.OpFlagLeverSw = false;
                opCollection.sequencer.State = OpCollection.Sequencer.EState.WaitingForDownLever;
            }

            // 動作完了
            if (mainForm.Parent.OpFlagMoveLeverOut == true)
            {
                // 結果正常
                if (mainForm.Parent.OpResultMoveLeverOut == FormMain.EDeviceResult.Done)
                {
                    if (!PreferencesDatOriginal.DisableLever)
                    {
                        opCollection.callbackMessageNormal("レバーOUT正常");
                        opCollection.callbackMessageNormal(string.Format("レバーDOWN開始(タイムアウト時間={0}[分])", PreferencesDatOriginal.OpeTimeoutOfStart));
                    }
                    opCollection.stopwatch.Restart(); // ストップウォッチをリスタート
                    mainForm.Parent.OpFlagLeverSw = false;
                    opCollection.sequencer.State = OpCollection.Sequencer.EState.WaitingForDownLever;
                }

                // 結果異常
                else
                {
                    opCollection.callbackMessageNormal("レバーOUT異常");
                    opCollection.sequencer.State = OpCollection.Sequencer.EState.PreLeaveCageProc;
                }
            }
        }

        private void WaitingForDownLever()
        {
            // タイムアウト
            if (opCollection.stopwatch.Elapsed.TotalMinutes >= PreferencesDatOriginal.OpeTimeoutOfStart)
            {
                opCollection.callbackMessageNormal("レバーDOWNタイムアウト");
                opCollection.stopwatch.Stop(); // ストップウォッチを停止
                if (!PreferencesDatOriginal.DisableLever)
                {
                    InvokeMethod(() =>
                    {
                        mainForm.Parent.OpMoveLeverIn(); // レバーをIN
                    });
                }

                opCollection.sequencer.State = OpCollection.Sequencer.EState.PreLeaveCageProc;
                return;
            }
            if (PreferencesDatOriginal.DisableLever)
            {
                opCollection.sequencer.State = OpCollection.Sequencer.EState.WaitingForInLever;
            }

            // レバーDOWN
            if (mainForm.Parent.OpFlagLeverSw == true)
            {
                if (!PreferencesDatOriginal.DisableLever)
                {
                    opCollection.callbackMessageNormal("レバーDOWN正常");
                }
                InvokeMethod(() =>
                {
                    mainForm.Parent.OpMoveLeverIn(); // レバーをIN
                });
                opCollection.sequencer.State = OpCollection.Sequencer.EState.WaitingForInLever;
            }
        }

        private void WaitingForInLever()
        {
            if (PreferencesDatOriginal.DisableLever)
            {
                opCollection.stopwatch.Restart(); // ストップウォッチをリスタート
                if (!PreferencesDatOriginal.DisableDoor)
                {
                    InvokeMethod(() =>
                    {
                        mainForm.Parent.OpCloseDoor(); // ドアをCLOSE
                    });
                    opCollection.sequencer.State = OpCollection.Sequencer.EState.WaitingForCloseDoor;
                }
                else
                {
                    InvokeMethod(() =>
                    {
                        mainForm.Parent.OpMakeScheduleOfFeed(); // フィード・スケジュールを行う
                    });
                    opCollection.trialCount = 0; // 試行回数カウンタをクリア
                    opCollection.callbackSetUiCurentNumberOfTrial(opCollection.trialCount);
                    opCollection.sequencer.State = OpCollection.Sequencer.EState.PreTouchTriggerScreenProc;
                    //opCollection.sequencer.State = OpCollection.Sequencer.EState.RunProcessBlock;
                }
                return;
            }
            // レバーIN動作完了
            if (mainForm.Parent.OpFlagMoveLeverIn == true)
            {

                // レバーIN異常
                if (mainForm.Parent.OpResultMoveLeverIn != FormMain.EDeviceResult.Done)
                {
                    opCollection.callbackMessageNormal("レバーIN異常");
                    opCollection.sequencer.State = OpCollection.Sequencer.EState.PreLeaveCageProc;
                    return;
                }
                // レバーIN正常
                {
                    if (!PreferencesDatOriginal.DisableLever)
                    {
                        opCollection.callbackMessageNormal("レバーIN正常");
                    }

                    opCollection.stopwatch.Restart(); // ストップウォッチをリスタート
                    if (!PreferencesDatOriginal.DisableDoor)
                    {
                        InvokeMethod(() =>
                        {
                            mainForm.Parent.OpCloseDoor(); // ドアをCLOSE
                        });
                        opCollection.sequencer.State = OpCollection.Sequencer.EState.WaitingForCloseDoor;
                    }
                    else
                    {
                        InvokeMethod(() =>
                        {
                            mainForm.Parent.OpMakeScheduleOfFeed(); // フィード・スケジュールを行う
                        });
                        opCollection.trialCount = 0; // 試行回数カウンタをクリア
                        opCollection.callbackSetUiCurentNumberOfTrial(opCollection.trialCount);
                        opCollection.sequencer.State = OpCollection.Sequencer.EState.PreTouchTriggerScreenProc;
                        //opCollection.sequencer.State = OpCollection.Sequencer.EState.RunProcessBlock;
                    }
                }
            }
        }

        private void WaitingForCloseDoor()
        {
            // CLOSE動作完了
            if (mainForm.Parent.OpFlagCloseDoor == true)
            {
                // ドアCLOSE異常
                if (mainForm.Parent.OpResultCloseDoor != FormMain.EDeviceResult.Done)
                {
                    opCollection.callbackMessageNormal("ドアCLOSE異常");
                    if (!PreferencesDatOriginal.IgnoreDoorError)
                    {
                        opCollection.sequencer.State = OpCollection.Sequencer.EState.PreLeaveCageProc;
                    }
                    else
                    {
                        InvokeMethod(() =>
                        {
                            mainForm.Parent.OpMakeScheduleOfFeed(); // フィード・スケジュールを行う
                        });
                        opCollection.trialCount = 0; // 試行回数カウンタをクリア
                        opCollection.callbackSetUiCurentNumberOfTrial(opCollection.trialCount);
                        opCollection.sequencer.State = OpCollection.Sequencer.EState.PreTouchTriggerScreenProc;
                        //opCollection.sequencer.State = OpCollection.Sequencer.EState.RunProcessBlock;
                    }
                    return;
                }
                // ドアCLOSE正常
                {
                    opCollection.callbackMessageNormal("ドアCLOSE正常処理");
                    InvokeMethod(() =>
                    {
                        mainForm.Parent.OpMakeScheduleOfFeed(); // フィード・スケジュールを行う
                    });
                    opCollection.trialCount = 0; // 試行回数カウンタをクリア
                    opCollection.callbackSetUiCurentNumberOfTrial(opCollection.trialCount);
                    opCollection.sequencer.State = OpCollection.Sequencer.EState.PreTouchTriggerScreenProc;
                    //opCollection.sequencer.State = OpCollection.Sequencer.EState.RunProcessBlock;
                }
            }
        }

        private void PreOpenDoorProc()
        {
            // ドアOPEN前処理
            InvokeMethod(() =>
            {
                mainForm.Parent.OpDrawBackColorBlackOnTouchPanel(); // タッチパネル画面を黒
            });
            if (!PreferencesDatOriginal.DisableDoor)
            {
                mainForm.Parent.OpOpenDoor(); // ドアをOPEN
            }

            opCollection.sequencer.State = OpCollection.Sequencer.EState.WaitingForOpenDoor;
        }
        private void WaitingForOpenDoor()
        {
            // ドアOPEN待ち
            // ドアOPEN動作完了
            if (mainForm.Parent.devInDoorOpen.dInCurrent == true)
            {
                // 結果正常
                if (mainForm.Parent.OpResultOpenDoor == FormMain.EDeviceResult.Done)
                {
                    opCollection.callbackMessageNormal("ドアOPEN正常");
                    opCollection.sequencer.State = OpCollection.Sequencer.EState.PreLeaveCageProc;
                }
                // 結果異常
                else
                {
                    opCollection.callbackMessageNormal("ドアOPEN異常");
                    opCollection.sequencer.State = OpCollection.Sequencer.EState.PreLeaveCageProc;
                }
            }

            if (PreferencesDatOriginal.DisableDoor && mainForm.Parent.devInDoorOpen.dInCurrent == false)
            {
                opCollection.callbackMessageNormal("ドア無効停止待機");
                opCollection.sequencer.State = OpCollection.Sequencer.EState.WaitingForLeaveCage;
            }
        }
        private void PreLeaveCageProc()
        {
            // 退室前処理
            {
                //InvokeMethod(() =>
                //{
                mainForm.Parent.OpSetRoomLampOff(); // 天井ランプをOFF
                mainForm.Parent.OpPlaySoundOfEnd(); // 終了音を再生
                //});
                opCollection.stopwatch.Restart(); // ストップウォッチをリスタート
                mainForm.Parent.OpFlagRoomOut = false;
                if (PreferencesDatOriginal.DisableDoor)
                {
                    opCollection.callbackMessageNormal(string.Format("退室タイムアウト有効(時間={0}[分])", PreferencesDatOriginal.OpeTimeoutOfLeaveCage));
                }

                opCollection.sequencer.State = OpCollection.Sequencer.EState.WaitingForLeaveCage;
            }
        }
        private void WaitingForLeaveCage()
        {
            // 退室待ち
            // タイムアウト
            if (opCollection.stopwatch.Elapsed.TotalMinutes >= PreferencesDatOriginal.OpeTimeoutOfLeaveCage && PreferencesDatOriginal.DisableDoor)
            {
                opCollection.callbackMessageNormal("退室タイムアウト");
                opCollection.stopwatch.Stop(); // ストップウォッチを停止
                opCollection.trialCount = 0; // 試行回数カウンタをクリア
                opCollection.callbackSetUiCurentNumberOfTrial(opCollection.trialCount);
                // タイムアウト時の処理がおかしい

                if (mainForm.Parent.uob.OperationStatus)
                {
                    // uob側動作していたら止める？
                    OnStopStateProgrammableProc();
                }

                opCollection.sequencer.State = OpCollection.Sequencer.EState.PreEnterCageProc;
                return;
            }

            // 退室検知
            if (mainForm.Parent.OpFlagRoomOut == true)
            {
                opCollection.callbackMessageNormal("退室検知");
                opCollection.stopwatch.Stop(); // ストップウォッチを停止
                opCollection.trialCount = 0; // 試行回数カウンタをクリア
                opCollection.callbackSetUiCurentNumberOfTrial(opCollection.trialCount);

                if (mainForm.Parent.uob.OperationStatus)
                {
                    // uob側動作していたら止める？
                    OnStopStateProgrammableProc();
                }

                opCollection.sequencer.State = OpCollection.Sequencer.EState.PreEnterCageProc;
            }

            // ドア制御無効時 手動停止待ち
            if (PreferencesDatOriginal.DisableDoor)
            {

            }

        }
        private void IllegalExitDetection()
        {
            opCollection.callbackMessageNormal("イリーガル退室検知: " + lastState.ToString());
            //opCollection.taskResultVal = OpCollection.ETaskResult.IllegalExit;
            opCollection.dateTimeout = DateTime.Now;                             // タイムアウト？退出時刻を保存(結果ファイル出力用)
            opCollection.sequencer.callbackOutputFile(opCollection.sequencer.State); // ファイル出力

            mainForm.Parent.OpFlagRoomIn = false;

            InvokeMethod(() =>
            {
                mainForm.Parent.OpDrawBackColorBlackOnTouchPanel(); // タッチパネル画面を黒
            });

            // 設定試行回数により終了

            opCollection.stopwatch.Restart(); // ストップウォッチをリスタート
            //イリーガル再入機能 切り替え
            if (PreferencesDatOriginal.OpeEnableReEntry)
            {
                // 給餌終わってるか 終わってたら再入せずに待機へ
                if (mainForm.Parent.OpFlagFeedOn)
                {
                    mainForm.Parent.OpFlagRoomOut = false;
                    opCollection.sequencer.State = OpCollection.Sequencer.EState.PreEnterCageProc;
                }
                else
                {
                    opCollection.sequencer.State = OpCollection.Sequencer.EState.IllegalExitDetection_WaitReEnter;

                    mainForm.Parent.UpdateIllegalStatus();
                }
            }
            else
            {
                //退出終わってるからフラグリセット UcOperationではステートマシン側でリセットしてるので問題出てない
                mainForm.Parent.OpFlagRoomOut = false;
                opCollection.sequencer.State = OpCollection.Sequencer.EState.PreEnterCageProc;
            }

            // 退出時 設定試行回数より多ければ終了
            // トライアル回数リセット
            if (opCollection.trialCount >= PreferencesDatOriginal.OpeNumberOfTrial && PreferencesDatOriginal.OpeNumberOfTrial != 0)
            {
                opCollection.callbackMessageNormal(string.Format("設定試行回数({0}/{1})により給餌終了", opCollection.trialCount, PreferencesDatOriginal.OpeNumberOfTrial));
                opCollection.trialCount = 0; // 試行回数カウンタをクリア
                opCollection.callbackSetUiCurentNumberOfTrial(opCollection.trialCount);
            }
        }

        private void ExitAfterFeedingDetection()
        {
            opCollection.callbackMessageNormal("フィード中退室検知: " + lastState.ToString());

            opCollection.trialCount++; // 試行回数カウンタをインクリメント
            opCollection.taskResultVal = OpCollection.ETaskResult.ExitAfterFeeding;
            opCollection.dateTimeout = DateTime.Now;                             // タイムアウト？退出時刻を保存(結果ファイル出力用)
            opCollection.sequencer.callbackOutputFile(opCollection.sequencer.State); // ファイル出力

            mainForm.Parent.OpFlagRoomIn = false;

            mainForm.Parent.OpDrawBackColorBlackOnTouchPanel(); // タッチパネル画面を黒
                                                                // 設定試行回数により終了
                                                                //mainForm.Parent.Feeding = false;

            mainForm.Parent.OpFlagRoomOut = false;
            opCollection.sequencer.State = OpCollection.Sequencer.EState.PreEnterCageProc;


            // 退出時 設定試行回数より多ければ終了
            // トライアル回数リセット
            if (opCollection.trialCount >= PreferencesDatOriginal.OpeNumberOfTrial && PreferencesDatOriginal.OpeNumberOfTrial != 0)
            {
                opCollection.callbackMessageNormal(string.Format("設定試行回数({0}/{1})により給餌終了", opCollection.trialCount, PreferencesDatOriginal.OpeNumberOfTrial));
                opCollection.trialCount = 0; // 試行回数カウンタをクリア
                opCollection.callbackSetUiCurentNumberOfTrial(opCollection.trialCount);
            }
        }

        #region イリーガル退出後待ち

        private void IllegalExitDetection_WaitReEnter()
        {
            // 再入タイムアウト
            if (opCollection.stopwatch.Elapsed.TotalSeconds >= PreferencesDatOriginal.OpeReEntryTimeout)
            {
                opCollection.callbackMessageNormal("再入タイムアウト");
                opCollection.stopwatch.Stop(); // ストップウォッチを停止

                //BlockAction停止
                mainForm.Parent.uob.Stop();

                opCollection.sequencer.State = OpCollection.Sequencer.EState.PreEnterCageProc;

                mainForm.Parent.UpdateIllegalStatus();
            }

            // 入室検知 (ドア無効時 ドア手動閉追加)
            if (mainForm.Parent.OpFlagRoomIn == true || (PreferencesDatOriginal.DisableDoor && (mainForm.Parent.devInDoorClose.dInCurrent == true || mainForm.Parent.devInDoorOpen.dInCurrent == false)))
            {
                MonitorPower.Monitor.PowerOn();
                opCollection.callbackMessageNormal("入室検知");
                opCollection.TimeEnterCage = DateTime.Now;
                //opCollection.callbackSetUiCurentNumberOfTrial(opCollection.trialCount);


                opCollection.idCode = mainForm.Parent.OpeGetIdCode();
                opCollection.callbackSetUiCurrentIdCode(opCollection.idCode);

                //イリーガル再入室時はレバー制御しない（レバーINのまま）

                mainForm.Parent.OpFlagRoomOut = false;

                opCollection.sequencer.State = OpCollection.Sequencer.EState.IllegalExitDetection_TrainingWait;
                InvokeMethod(() =>
                {
                    mainForm.Parent.UpdateIllegalStatus();
                });
            }
        }
        // イリーガル再入時処理
        private void IllegalExitDetection_TrainingWait()
        {
            opCollection.stopwatch.Restart(); // ストップウォッチをリスタート
            InvokeMethod(() =>
            {
                //ライト類消灯
                mainForm.Parent.OpSetRoomLampOff();
                mainForm.Parent.OpSetFeedLampOff();
                //フィード中であれば停止
                mainForm.Parent.OpSetFeedOff();
                mainForm.Parent.OpAirPuffOff();
            });

            // 直前動作から復帰？ 再再生フラグ設定
            // プログラムブロック状態を保存後再生するようにする
            // lastStateは未使用
            // 強制PreTouchTriggerScreenProc
            mainForm.Parent.uob.ReEntry = true;
            opCollection.sequencer.State = OpCollection.Sequencer.EState.PreTouchTriggerScreenProc;
            opCollection.callbackMessageNormal("イリーガル後再入室検知: " + opCollection.sequencer.State.ToString());
        }
        #endregion
        private void Error()
        {
            opCollection.callbackMessageError("エラーステート");
        }

        #endregion

        #region IllegalExitStopCheck
        /// <summary>
        /// 非常停止 停止チェック
        /// </summary>
        /// <returns>bool</returns>
        private bool CheckInteruptStop()
        {
            // 一回読むとNopになるのでステートマシン内のみ参照
            OpCollection.ECommand command = opCollection.Command;
            if (command == OpCollection.ECommand.EmergencyStop)
            {
                opCollection.sequencer.State = OpCollection.Sequencer.EState.EmergencyStop;
                return true;
            } // 緊急停止
            if (command == OpCollection.ECommand.Stop)
            {
                opCollection.sequencer.State = OpCollection.Sequencer.EState.Stop;
                return true;
            } // 中止
            else
                return false;
        }
        /// <summary>
        /// イリーガル退出および停止検査
        /// 給餌中 給餌後退室も検出
        /// </summary>
        /// <returns>bool</returns>
        private bool CheckIllegalExit()
        {
            if (mainForm.Parent.OpFlagRoomOut)
            {
                if (mainForm.Parent.Feeding)
                {
                    opCollection.flagFeed = true;
                    //mainForm.Parent.OpFlagFeedOn = false;
                    lastState = opCollection.sequencer.State;
                    opCollection.sequencer.State = OpCollection.Sequencer.EState.ExitAfterFeedingDetection;

                    return false;
                }
                else
                {
                    opCollection.flagFeed = false;
                    lastState = opCollection.sequencer.State;
                    opCollection.sequencer.State = OpCollection.Sequencer.EState.IllegalExitDetection;
                    return true;
                }
            } // イリーガル退室検知
            return false;
        }
        private bool CheckLeaveTimeout()
        {
            if (opCollection.stopwatch.Elapsed.TotalMinutes >= PreferencesDatOriginal.OpeTimeoutOfLeaveCage && PreferencesDatOriginal.DisableDoor)
            {
                opCollection.callbackMessageNormal("退室タイムアウト");
                opCollection.stopwatch.Stop(); // ストップウォッチを停止
                opCollection.trialCount = 0; // 試行回数カウンタをクリア
                opCollection.callbackSetUiCurentNumberOfTrial(opCollection.trialCount);
                opCollection.sequencer.State = OpCollection.Sequencer.EState.PreEnterCageProc;
                return true;
            }
            else
                return false;
        }
        #endregion

        #region Programable Section 廃止
        private void PreTouchTriggerScreenProc()
        {
            opCollection.callbackMessageNormal(string.Format("トリガ画面タッチ待ち(タイムアウト時間={0}[分])", PreferencesDatOriginal.OpeTimeoutOfTrial));
            opCollection.flagFeed = false; // 給餌したか否かのフラグ:初期化(結果ファイル出力用)
            opCollection.intervalTimeTotal = 0; // インターバル値合計:初期化(結果ファイル出力用)
            opCollection.intervalNum = 0; ; // インターバル回数:初期化(結果ファイル出力用)
            opCollection.dateTimeTriggerTouch = new DateTime();      // トリガ画面タッチ時刻:初期化(結果ファイル出力用)
            opCollection.dateTimeCorrectTouch = new DateTime();      // 正解図形タッチ時刻:初期化(結果ファイル出力用)
            opCollection.dateTimeout = new DateTime();               // タイムアウト時刻:初期化(結果ファイル出力用))
            opCollection.pointCorrectShape = new Point(0, 0); // 正解図形の座標: 初期化(結果ファイル出力用)
            opCollection.pointWrongShape = new Point(0, 0); // 不正解図形の座標: 初期化(結果ファイル出力用)

            //描画画像初期化
            {
                mainForm.Parent.opImage.SetParamOfShapeOpeImage(
                    PreferencesDatOriginal.TypeOfShape,
                    PreferencesDatOriginal.SizeOfShapeInStep,
                    PreferencesDatOriginal.SizeOfShapeInPixel);
            }
            //この部分を外へ
            {
                // 外部で見る //廃止
                if (mainForm.Parent.OpeGetTypeOfTask() == ECpTask.UnConditionalFeeding)
                {
                    opCollection.callbackMessageNormal("入室報酬");
                    opCollection.callbackMessageNormal(string.Format("訓練(タイムアウト時間={0}[分])", PreferencesDatOriginal.OpeTimeoutOfTrial));
                    opCollection.taskResultVal = OpCollection.ETaskResult.Ok;       // 結果: 正解を保存
                    opCollection.dateTimeCorrectTouch = DateTime.Now; // 正解時刻時刻を保存
                    //opCollection.sequencer.State = OpCollection.Sequencer.EState.CorrectAnswer; // 廃止
                }
                else
                {
                    mainForm.Parent.OpSetRoomLampOff(); // 天井ランプをOFF
                    mainForm.Parent.OpDrawTriggerImageOnTouchPanel(); // トリガ画面表示
                    mainForm.Parent.OpStartToTouchAnyOnTouchPanel(); // どこでもタッチ検知を開始
                    opCollection.stopwatch.Restart(); // ストップウォッチをリスタート

                    //opCollection.sequencer.State = OpCollection.Sequencer.EState.WaitingForTouchTriggerScreen; //廃止
                }
            }
        }

        /// <summary>
        /// スクリーントリガー待ち
        /// 設定による分岐は廃止して後段処理による
        /// 内部は分解
        /// </summary>
        private void WaitingForTouchTriggerScreen()
        {
            // タイムアウト
            if (opCollection.stopwatch.Elapsed.TotalMinutes >= PreferencesDatOriginal.OpeTimeoutOfTrial)
            {
                opCollection.callbackMessageNormal("トリガ画面タッチタイムアウト");
                opCollection.stopwatch.Stop(); // ストップウォッチを停止
                mainForm.Parent.OpEndToTouchAnyOnTouchPanel(); // どこでもタッチを停止
                opCollection.taskResultVal = OpCollection.ETaskResult.StartTimeout;       // 結果: 開始タイムアウトを保存(結果ファイル出力用)
                opCollection.dateTimeout = DateTime.Now;                             // タイムアウト時刻を保存(結果ファイル出力用)
                opCollection.sequencer.callbackOutputFile(opCollection.sequencer.State); // ファイル出力

                //opCollection.sequencer.State = OpCollection.Sequencer.EState.PreOpenDoorProc; //廃止
                return;
            }
            // 画面タッチ
#if NO_TRIGGER_TOUCH
                        if (OpFlagTouchAnyOnTouchPanel != true)
#else
            if (mainForm.Parent.OpFlagTouchAnyOnTouchPanel == true)
#endif
            {
                // 開始時表示だけインクリメント 完了時にカウント
                opCollection.callbackSetUiCurentNumberOfTrial(opCollection.trialCount + 1);

                opCollection.dateTimeTriggerTouch = DateTime.Now; // トリガ・タッチ時刻を保存
                opCollection.callbackMessageNormal("画面タッチ");
                opCollection.stopwatch.Stop(); // ストップウォッチを停止
                mainForm.Parent.OpEndToTouchAnyOnTouchPanel(); // どこでもタッチを停止
                switch (mainForm.Parent.OpeGetTypeOfTask())
                {
                    case ECpTask.DelayMatch:

                        if (!PreferencesDatOriginal.EnableRandomTime)
                        {
                            mainForm.Parent.OpDrawCorrectShapeOnTouchPanel(out opCollection.trainingShape); // 訓練正解図形表示
                        }
                        // ディレイマッチ
                        opCollection.callbackMessageNormal(string.Format("ディレイマッチ(正解図形表示時間={0}[秒])", PreferencesDatOriginal.OpeTimeToDisplayCorrectImage));
                        opCollection.stopwatch.Restart(); // ストップウォッチをリスタート
                        if (PreferencesDatOriginal.EnableRandomTime)
                        {
                            mainForm.Parent.OpDrawDelayBackColorOnTouchPanel(); // タッチパネル画面をDelay指定色でクリア
                            opCollection.beforeDelayMatchRandomTime = mainForm.Parent.OpSubGetAfterMatchRandomTime();
                            opCollection.callbackMessageNormal(string.Format("ディレイ(ランダム遅延時間={0}[秒])", opCollection.beforeDelayMatchRandomTime));
                            //opCollection.sequencer.State = OpCollection.Sequencer.EState.DelayBeforeMatch;
                        }
                        else
                        {
                            //opCollection.sequencer.State = OpCollection.Sequencer.EState.DelayMatchImage;
                        }
                        break;
                    case ECpTask.Training:
                        // 訓練
                        opCollection.callbackMessageNormal("画面タッチ(訓練)");
                        opCollection.callbackMessageNormal(string.Format("訓練(タイムアウト時間={0}[分])", PreferencesDatOriginal.OpeTimeoutOfTrial));
                        mainForm.Parent.OpStartToTouchCorrectOnTouchPanel(); // タッチコレクトを開始
                        opCollection.stopwatch.Restart(); // ストップウォッチをリスタート
                        if (PreferencesDatOriginal.EnableRandomTime)
                        {
                            mainForm.Parent.OpDrawDelayBackColorOnTouchPanel(); // タッチパネル画面をDelay指定色でクリア
                            opCollection.beforeDelayMatchRandomTime = mainForm.Parent.OpSubGetAfterMatchRandomTime();
                            opCollection.callbackMessageNormal(string.Format("ディレイ(ランダム遅延時間={0}[秒])", opCollection.beforeDelayMatchRandomTime));

                            //opCollection.sequencer.State = OpCollection.Sequencer.EState.DelayAfterTraning;
                        }
                        else
                        {
                            mainForm.Parent.OpDrawTrainShapeOnTouchPanel(out opCollection.trainingShape); // 訓練図形表示

                            //opCollection.sequencer.State = OpCollection.Sequencer.EState.Training;
                        }
                        break;
                    case ECpTask.TrainingEasy:
                        // かんたん訓練
                        opCollection.callbackMessageNormal("画面タッチ(かんたん訓練)");
                        opCollection.callbackMessageNormal(string.Format("訓練(タイムアウト時間={0}[分])", PreferencesDatOriginal.OpeTimeoutOfTrial));
                        opCollection.taskResultVal = OpCollection.ETaskResult.Ok;       // 結果: 正解を保存
                        opCollection.dateTimeCorrectTouch = DateTime.Now; // 正解時刻時刻を保存

                        //opCollection.sequencer.State = OpCollection.Sequencer.EState.CorrectAnswer;
                        break;

                    default:
                        opCollection.callbackMessageError("存在しないタスク");
                        //opCollection.sequencer.State = OpCollection.Sequencer.EState.Error;
                        break;
                }
            }
        }

        // 図表示のみのオペレーションに変更 関数名も
        // 実際のオペレーションは図表示と待ち1セット

        private void DelayBeforeMatch()
        {
            // タイムアウト
            if (opCollection.stopwatch.Elapsed.TotalSeconds >= opCollection.beforeDelayMatchRandomTime)
            {
                opCollection.stopwatch.Stop(); // ストップウォッチを停止
                opCollection.stopwatch.Restart(); // ストップウォッチをリスタート
                mainForm.Parent.OpDrawCorrectShapeOnTouchPanel(out opCollection.trainingShape); // 訓練正解図形表示
                //opCollection.sequencer.State = OpCollection.Sequencer.EState.DelayMatchImage;

            }
        }
        // DelayBeforeMatchと1セット動作に
        private void DelayMatchImage()
        {
            // タイムアウト
            if (opCollection.stopwatch.Elapsed.TotalSeconds >= PreferencesDatOriginal.OpeTimeToDisplayCorrectImage)
            {
                opCollection.callbackMessageNormal("ディレイマッチ(正解図形表示)正常完了");
                opCollection.callbackMessageNormal(string.Format("ディレイマッチ(タッチパネル画面を黒にする時間={0}[秒])", PreferencesDatOriginal.OpeTimeToDisplayNoImage));
                opCollection.stopwatch.Stop(); // ストップウォッチを停止
                opCollection.stopwatch.Restart(); // ストップウォッチをリスタート
                mainForm.Parent.OpDrawBackColorOnTouchPanel(); // タッチパネル画面を背景色でクリア

                //opCollection.sequencer.State = OpCollection.Sequencer.EState.DelayMatchBackColor;
            }
        }
        // スクリプト上のバリエーションなので廃止
        // ImageFile設定かは初期呼び出し設定による
        private void AfterIllegalExit_DelayMatchImage()
        {
            opCollection.stopwatch.Restart(); // ストップウォッチをリスタート
            mainForm.Parent.OpDrawBackColorOnTouchPanel(); // タッチパネル画面を背景色でクリア

            // 背景クリアと描画遅延なしで行う
            try
            {
                mainForm.Parent.OpDrawCorrectAndWrongShapeOnTouchPanel(out opCollection.correctShape, out opCollection.incorrectShapeList); // 正解図形・不正解図形表示
            }
            catch (Exception ex)
            {
                opCollection.callbackMessageError(ex.Message);
            }
            // ImageFile 有効時ファイル名ログ伝達
            if (PreferencesDatOriginal.EnableImageShape)
            {
                if (PreferencesDatOriginal.RandomCorrectImage)
                {
                    opCollection.correctImageFile = System.IO.Path.GetFileName(mainForm.Parent.opImage.CorrectImageFile);
                }
                else
                {
                    opCollection.correctImageFile = System.IO.Path.GetFileName(PreferencesDatOriginal.CorrectImageFile);
                }
            }

            mainForm.Parent.OpStartToTouchCorrectOnTouchPanel(); // タッチコレクトを開始
            opCollection.sequencer.State = OpCollection.Sequencer.EState.Training;

        }
        // 訓練図表示に統一
        private void DelayAfterTraning()
        {
            // タイムアウト
            if (opCollection.stopwatch.Elapsed.TotalSeconds >= opCollection.beforeDelayMatchRandomTime)
            {
                opCollection.stopwatch.Stop(); // ストップウォッチを停止
                opCollection.stopwatch.Restart(); // ストップウォッチをリスタート
                mainForm.Parent.OpDrawTrainShapeOnTouchPanel(out opCollection.trainingShape); // 訓練図形表示
                //opCollection.sequencer.State = OpCollection.Sequencer.EState.Training;

            }
        }
        // イリーガル再入時は直前動作に統一
        private void AfterIlegalExit_Training()
        {
            opCollection.stopwatch.Restart(); // ストップウォッチをリスタート
            mainForm.Parent.OpDrawTrainShapeOnTouchPanelAfterIllegalExit(opCollection.trainingShape); // 訓練図形表示
            //opCollection.sequencer.State = OpCollection.Sequencer.EState.Training;
        }
        // 正答不正答図表示に統一
        private void DelayMatchBackColor()
        {
            // タイムアウト
            if (opCollection.stopwatch.Elapsed.TotalSeconds >= PreferencesDatOriginal.OpeTimeToDisplayNoImage)
            {
                opCollection.callbackMessageNormal("ディレイマッチ(タッチパネル画面を黒)正常完了");
                opCollection.callbackMessageNormal(string.Format("訓練(タイムアウト時間={0}[分])", PreferencesDatOriginal.OpeTimeoutOfTrial));
                opCollection.stopwatch.Stop(); // ストップウォッチを停止
                opCollection.stopwatch.Restart(); // ストップウォッチをリスタート

                try
                {
                    mainForm.Parent.OpDrawCorrectAndWrongShapeOnTouchPanel(out opCollection.correctShape, out opCollection.incorrectShapeList); // 正解図形・不正解図形表示
                }
                catch (Exception ex)
                {
                    opCollection.callbackMessageError(ex.Message);
                }
                // ImageFile 有効時ファイル名ログ伝達
                if (PreferencesDatOriginal.EnableImageShape)
                {
                    if (PreferencesDatOriginal.RandomCorrectImage)
                    {
                        opCollection.correctImageFile = System.IO.Path.GetFileName(mainForm.Parent.opImage.CorrectImageFile);
                    }
                    else
                    {
                        opCollection.correctImageFile = System.IO.Path.GetFileName(PreferencesDatOriginal.CorrectImageFile);
                    }
                }

                mainForm.Parent.OpStartToTouchCorrectOnTouchPanel(); // タッチコレクトを開始
                //opCollection.sequencer.State = OpCollection.Sequencer.EState.Training;
            }
        }
        // 表示待機に統一
        private void Training()
        {
            // タイムアウト
            if (opCollection.stopwatch.Elapsed.TotalMinutes >= PreferencesDatOriginal.OpeTimeoutOfTrial)
            {
                opCollection.callbackMessageNormal("訓練タイムアウト");
                opCollection.stopwatch.Stop(); // ストップウォッチを停止
                mainForm.Parent.OpEndToTouchCorrectOnTouchPanel(); // タッチコレクトを停止
                opCollection.taskResultVal = OpCollection.ETaskResult.TrialTimeout;       // 結果: 試行タイムアウトを保存(結果ファイル出力用)
                opCollection.dateTimeout = DateTime.Now;                             // タイムアウト時刻を保存(結果ファイル出力用)
                opCollection.sequencer.callbackOutputFile(opCollection.sequencer.State); // ファイル出力
                //opCollection.sequencer.State = OpCollection.Sequencer.EState.PreOpenDoorProc;
                return;
            }
            // 正解図形タッチ
            if (mainForm.Parent.OpFlagTouchCorrectOnTouchPanel == true)
            {
                opCollection.taskResultVal = OpCollection.ETaskResult.Ok;       // 結果: 正解を保存
                opCollection.dateTimeCorrectTouch = DateTime.Now; // 正解時刻時刻を保存
                opCollection.callbackMessageNormal("訓練正解図形タッチ");
                opCollection.stopwatch.Stop(); // ストップウォッチを停止
                mainForm.Parent.OpEndToTouchCorrectOnTouchPanel(); // タッチコレクトを停止
                //opCollection.sequencer.State = OpCollection.Sequencer.EState.CorrectAnswer;
            }
        }
        // 表示待機から次の動作に統一
        private void CorrectAnswer()
        {
            {
                mainForm.Parent.OpDrawBackColorBlackOnTouchPanel(); // タッチパネル画面を黒
                mainForm.Parent.OpSetRoomLampOff(); // 天井ランプをOFF
                mainForm.Parent.OpPlaySoundOfCorrect(); //正解音ならす
            }

            {
                opCollection.stopwatch.Restart(); // ストップウォッチをリスタート
                //opCollection.sequencer.State = OpCollection.Sequencer.EState.DelayFeed;
            }
        }
        // 単純Delayに統一
        private void DelayFeed()
        {
            if (opCollection.stopwatch.Elapsed.TotalMilliseconds >= PreferencesDatOriginal.OpeDelayFeedLamp)
            {
                if (PreferencesDatOriginal.EnableFeedLamp)
                {
                    if (!mainForm.Parent.OpFlagFeedLampON)
                    {
                        mainForm.Parent.
                           OpSetFeedLampOn(); // フィードランプをON
                        mainForm.Parent.OpFlagFeedLampON = true;
                    }
                }
            }
            // ディレイ
            if (opCollection.stopwatch.Elapsed.TotalMilliseconds >= PreferencesDatOriginal.OpeDelayFeed && FormMain.PlaySoundEnded)
            {
                opCollection.stopwatch.Stop(); // ストップウォッチを停止

                mainForm.Parent.OpPlaySoundOfReward(); // 報酬音を再生

                opCollection.stopwatch.Restart(); // ストップウォッチをリスタート
                //opCollection.sequencer.State = OpCollection.Sequencer.EState.WaitingForReward;
            }
        }
        // 給餌実行に統一
        private void WaitingForReward()
        {
            // 時間経過後給餌を実行
            if (opCollection.stopwatch.Elapsed.TotalMilliseconds >= PreferencesDatOriginal.OpeDelayFeed)
            {
                opCollection.stopwatch.Stop(); // ストップウォッチを停止

                mainForm.Parent.OpSetFeedOn(opCollection.trialCount, out opCollection.flagFeed); // 給餌を実行 

                // イリーガル再入有効時はフィード直後に試行回数インクリメント後表示
                {
                    if (PreferencesDatOriginal.OpeEnableReEntry)
                    {
                        opCollection.trialCount++; // 試行回数カウンタをインクリメント
                        opCollection.callbackSetUiCurentNumberOfTrial(opCollection.trialCount);
                    }
                }
                opCollection.stopwatch.Restart(); // ストップウォッチをリスタート
                //opCollection.sequencer.State = OpCollection.Sequencer.EState.WaitingDelayForRoomLump;
            }

        }
        // 単純Delayに統一
        private void WaitingDelayForRoomLump()
        {
            //フィード完了待ち
            // 給餌が終わったらFeedランプ消灯
            if (mainForm.Parent.OpFlagFeedOn == true)
            {
                opCollection.stopwatch.Stop(); // ストップウォッチをリスタート
                mainForm.Parent.OpSetFeedLampOff(); // フィードランプをOFF
                mainForm.Parent.OpFlagFeedLampON = false;

                opCollection.stopwatch.Restart(); // ストップウォッチをリスタート
                //opCollection.sequencer.State = OpCollection.Sequencer.EState.TurnOnRoomLump;
            }
        }
        //条件式外してそのまま使える
        private void TurnOnRoomLump()
        {
            //ルームランプ点灯
            // 給餌完了 給餌ランプOFF 報酬音声停止→ 時間経過
            if (mainForm.Parent.OpFlagFeedOn && !mainForm.Parent.OpFlagFeedLampON && FormMain.PlaySoundEnded && opCollection.stopwatch.Elapsed.TotalMilliseconds > PreferencesDatOriginal.OpeDelayRoomLampOnTime)
            {
                opCollection.stopwatch.Stop(); // ストップウォッチをリスタート

                mainForm.Parent.OpSetRoomLampOn(); // 天井ランプをON

                opCollection.stopwatch.Restart(); // ストップウォッチをリスタート
                //opCollection.sequencer.State = OpCollection.Sequencer.EState.WaitingForFeedComplete;
            }
        }
        // 試験繰り返し設定に統一
        private void WaitingForFeedComplete()
        {
            // 給餌完了 ルームランプ点灯時間経過
            if (mainForm.Parent.OpFlagFeedOn == true && opCollection.stopwatch.Elapsed.TotalMilliseconds > 100)
            {
                if (!PreferencesDatOriginal.OpeEnableReEntry)
                {
                    opCollection.trialCount++; // 試行回数カウンタをインクリメント
                    opCollection.callbackSetUiCurentNumberOfTrial(opCollection.trialCount);
                }

                // 設定試行回数により終了
                if (opCollection.trialCount >= PreferencesDatOriginal.OpeNumberOfTrial && PreferencesDatOriginal.OpeNumberOfTrial != 0)
                {
                    opCollection.callbackMessageNormal(string.Format("設定試行回数({0}/{1})により給餌終了", opCollection.trialCount, PreferencesDatOriginal.OpeNumberOfTrial));
                    opCollection.sequencer.callbackOutputFile(opCollection.sequencer.State); // ファイル出力
                    //opCollection.sequencer.State = OpCollection.Sequencer.EState.PreOpenDoorProc;
                }

                //外に出す
                // 入室報酬用
                else if (mainForm.Parent.OpeGetTypeOfTask() == ECpTask.UnConditionalFeeding)
                {
                    opCollection.callbackMessageNormal(string.Format("入室給餌終了"));
                    opCollection.sequencer.callbackOutputFile(opCollection.sequencer.State); // ファイル出力
                    //opCollection.sequencer.State = OpCollection.Sequencer.EState.PreOpenDoorProc;
                }
                // 試行継続
                else
                {
                    opCollection.callbackMessageNormal(string.Format("給餌完了試行継続({0}/{1})", opCollection.trialCount, PreferencesDatOriginal.OpeNumberOfTrial));
                    //opCollection.sequencer.State = OpCollection.Sequencer.EState.InitInterval;
                }

            }
        }
        private void InitInterval()
        {
            {
                opCollection.intervalTime = mainForm.Parent.OpSubGetIntervalTime(); // インターバル値を取得
                opCollection.beforeDelayMatchRandomTime = mainForm.Parent.OpSubGetAfterMatchRandomTime();
                opCollection.intervalTimeTotal += opCollection.intervalTime; // インターバル値合計を保存(結果ファイル出力用)
                opCollection.intervalNum++; // インターバル回数をカウントし、保存(結果ファイル出力用)
                opCollection.callbackMessageNormal(string.Format("インターバル開始(時間={0}[秒])", opCollection.intervalTime));
                opCollection.stopwatch.Restart(); // ストップウォッチをリスタート
                mainForm.Parent.OpStartToTouchAnyOnTouchPanel(); // どこでもタッチ検知を開始
                //opCollection.sequencer.State = OpCollection.Sequencer.EState.IntervalProc;
            }
        }
        private void IntervalProc()
        {                        // タイムアウト
            if (opCollection.stopwatch.Elapsed.TotalSeconds >= opCollection.intervalTime)
            {
                // 画面タッチ
                if (mainForm.Parent.OpFlagTouchAnyOnTouchPanel == true)
                {
                    opCollection.callbackMessageNormal("画面をタッチしたのでインターバル期間継続");
                    //opCollection.sequencer.State = OpCollection.Sequencer.EState.InitInterval;
                }
                // 画面非タッチ
                else
                {
                    opCollection.callbackMessageNormal("画面タッチしなかったのでインターバル期間完了");
                    opCollection.sequencer.callbackOutputFile(opCollection.sequencer.State); // ファイル出力
                    InvokeMethod(() =>
                    {
                        mainForm.Parent.OpEndToTouchAnyOnTouchPanel();
                    });
                    //opCollection.sequencer.State = OpCollection.Sequencer.EState.PreTouchTriggerScreenProc;
                }
            }
        }
        #endregion

    }
}
