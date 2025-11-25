using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Compartment
{
    public class EDoor : IDisposable
    {
        readonly SyncObject<bool> StateEnable = new SyncObject<bool>(true);
        readonly Task eDoorStateMachineTask;
        Task eDoorOpenTask;
        Task eDoorCloseTask;
        Task eDoorMiddleTask;

        int openMiddleTime = 300;

        CancellationTokenSource cancellationTokenSource;
        IoBoardBase ioboard;

        public Action CallbackAction = () => { };

        readonly SyncObject<bool> _Enable = new SyncObject<bool>(false);
        public bool Enable { get => _Enable.Value; set => _Enable.Value = value; }

        readonly SyncObject<eDoorState> _State = new SyncObject<eDoorState>(eDoorState.Init);
        eDoorState State { get => _State.Value; set => _State.Value = value; }

        public bool CwLim { get => ioboard.GetRecieveData((int)IoMicrochip.IoBoardInPortNum.DoorMotorCWLimit_B); }
        public bool CCwLim { get => ioboard.GetRecieveData((int)IoMicrochip.IoBoardInPortNum.DoorMotorCCWLimit_B); }

        public bool OutDirection { get => !ioboard.GetRecieveData((int)IoMicrochip.IoBoardInPortNum.DetectOutDirection); }

        public bool InDirection { get => !ioboard.GetRecieveData((int)IoMicrochip.IoBoardInPortNum.DetectInDirection); }

        public bool OutsideSensor { get => ioboard.GetRecieveData((int)IoMicrochip.IoBoardInPortNum.DoorOutsideSensor); }
        public bool InsideSensor { get => ioboard.GetRecieveData((int)IoMicrochip.IoBoardInPortNum.DoorInsideSensor); }

        readonly SyncObject<bool> _MiddleCancel = new SyncObject<bool>(false);
        public bool MiddleCancel { get => _MiddleCancel.Value; set => _MiddleCancel.Value = value; }

        private int EDoorMiddleTime = 300;
        private enum eDoorState
        {
            [EnumDisplayName("初期化")]
            Init,
            [EnumDisplayName("Idle")]
            Idle,
            [EnumDisplayName("DoorOpen")]
            DoorOpen,
            [EnumDisplayName("DoorOpenStay")]
            DoorOpenS,
            [EnumDisplayName("DoorMiddle")]
            DoorMiddle,
            [EnumDisplayName("DoorMiddleStay")]
            DoorMiddleS,
            [EnumDisplayName("DoorClose")]
            DoorClose,
            [EnumDisplayName("DoorCloseStay")]
            DoorCloseS,
        }
        private Stopwatch StateMachineSw = new Stopwatch();
        private Stopwatch PresenceSw = new Stopwatch();

        readonly ParentHelper<FormMain> mainForm = new ParentHelper<FormMain>();

        private ref PreferencesDat PreferencesDatOriginal
        {
            get
            {
                return ref mainForm.Parent.preferencesDatOriginal;
            }
        }

        public EDoor(IoBoardBase ioboard, FormMain baseForm)
        {
            mainForm.SetParent(baseForm);
            this.ioboard = ioboard;

            InitializeOpenDoorTask();
            InitializeMiddleDoorTask();
            InitializeCloseDoorTask();


            cancellationTokenSource = new CancellationTokenSource();
            eDoorStateMachineTask = new Task(new Action(() =>
            {
                while (StateEnable.Value)
                {
                    Thread.Sleep(1);
                    if (Enable)
                    {
                        StateOperation();
                    }
                    else
                    {
                        State = eDoorState.Init;
                        Thread.Sleep(100);
                    }
                }
            }), cancellationTokenSource.Token);
        }
        public void Start()
        {
            eDoorStateMachineTask?.Start();
        }
        void IDisposable.Dispose()
        {
            StateEnable.Value = false;
            eDoorStateMachineTask?.Wait();
            cancellationTokenSource?.Cancel();
        }

        eDoorState lastState;
        bool detectOpen = false;
        bool detectClose = false;
        bool detectMiddle = false;

        long detectMiddleSensorStatusTime = 0;
        bool statusMiddle { get => _statusMiddle.Value; set => _statusMiddle.Value = value; }
        readonly SyncObject<bool> _statusMiddle = new SyncObject<bool>(false);
        bool detectInCage { get => _statusMiddle.Value; set => _statusMiddle.Value = value; }
        readonly SyncObject<bool> _detectInCage = new SyncObject<bool>(false);
        private void StateOperation()
        {
            //ioboard.SetMotorSpeed(235);
            switch (State)
            {
                case eDoorState.Init:
                    StateMachineSw.Restart();
                    //ドア位置リセット動作
                    DoorOpen();
                    // 入室完了
                    if (!ioboard.GetRecieveData((int)IoMicrochip.IoBoardInPortNum.DoorMotorCWLimit_B))
                    {
                        State = eDoorState.DoorOpen;
                    }
                    break;

                case eDoorState.DoorOpen:
                    lastState = State;
                    if (ioboard.GetRecieveData((int)IoMicrochip.IoBoardInPortNum.DoorMotorCWLimit_B))// && !ioboard.GetRecieveData((int)IoMicrochip.IoBoardInPortNum.DoorOutsideSensor))
                    {
                        DoorOpen();
                        detectOpen = false;
                        detectClose = false;
                        detectMiddle = false;

                        StateMachineSw.Restart();
                        PresenceSw.Restart();
                        State = eDoorState.DoorOpenS;
                    }
                    else if (!ioboard.GetRecieveData((int)IoMicrochip.IoBoardInPortNum.DoorMotorCWLimit_B))
                    {
                        StateMachineSw.Restart();
                        PresenceSw.Restart();
                        detectMiddle = false;
                        State = eDoorState.DoorOpenS;
                    }
                    break;
                case eDoorState.DoorOpenS:
                    detectOpen = false;
                    {
                        if (StateMachineSw.ElapsedMilliseconds > PreferencesDatOriginal.EDoorOpenIntervalTime)
                        {
                            if (StateMachineSw.ElapsedMilliseconds > PreferencesDatOriginal.EDoorMonitorStartTime)
                            {
                                //if (lastState == eDoorState.DoorOpen && StateMachineSw.ElapsedMilliseconds < PreferencesDatOriginal.EDoorReEntryTime)
                                //{

                                //}
                                //else
                                //{
                                CheckDetectIllegalMiddleStatus();
                                if (ioboard.GetEdge((int)IoMicrochip.IoBoardInPortNum.DoorOutsideSensor))
                                {
                                    detectOpen = false;
                                    detectClose = false;
                                    detectMiddle = true;
                                }
                                if (ioboard.GetEdge((int)IoMicrochip.IoBoardInPortNum.DoorInsideSensor)
                                    && StateMachineSw.ElapsedMilliseconds > PreferencesDatOriginal.EDoorInsideDetectCloseTime) // 再入検知時間
                                {
                                    mainForm.Parent.TraceMessage("eDoor 開き時内センサー検知閉じ指令");
                                    detectOpen = false;
                                    detectClose = true;
                                    detectMiddle = false;
                                    detectInCage = true;
                                }
                                //}
                                //すぐ見ない ReEntryTime分マスク
                                if (!ioboard.GetData(IoMicrochip.IoBoardDInCode.PresenceSensor) && PresenceSw.ElapsedMilliseconds > PreferencesDatOriginal.EDoorReEntryTime)
                                {

                                    mainForm.Parent.TraceMessage("eDoor　入室センサー検知閉じ指令");
                                    detectOpen = false;
                                    detectClose = true;
                                    detectMiddle = false;
                                }
                            }

                            if (!ioboard.GetRecieveData((int)IoMicrochip.IoBoardInPortNum.DoorMotorCCWLimit_B))
                            {
                                mainForm.Parent.TraceMessage("eDoor detected error status." + "open");
                            }

                            if (detectMiddle)
                                State = eDoorState.DoorMiddle;
                            else if (detectClose)
                            {
                                State = eDoorState.DoorClose;
                                StateMachineSw.Restart();
                            }
                            else if (detectOpen)
                                State = eDoorState.DoorOpen;
                        }
                    }
                    break;
                case eDoorState.DoorMiddle:
                    if (lastState == eDoorState.DoorClose)
                    {
                        //パラメータ切り替え
                        EDoorMiddleTime = PreferencesDatOriginal.EDoorCloseToMiddleTime;
                    }
                    else if (lastState == eDoorState.DoorOpen)
                    {
                        //パラメータ切り替え
                        EDoorMiddleTime = PreferencesDatOriginal.EDoorOpenToMiddleTime;
                    }
                    else
                    {
                    }
                    DoorMiddleOpen();
                    detectOpen = false;
                    detectClose = false;
                    detectMiddle = false;

                    StateMachineSw.Restart();

                    State = eDoorState.DoorMiddleS;

                    break;
                case eDoorState.DoorMiddleS:
                    //// middle動かない時対策
                    //if (!ioboard.GetRecieveData((int)IoMicrochip.IoBoardInPortNum.DoorMotorCWLimit_B) || !ioboard.GetRecieveData((int)IoMicrochip.IoBoardInPortNum.DoorMotorCCWLimit_B))
                    //{
                    //    State = eDoorState.DoorMiddle;
                    //    break;
                    //}

                    if (ioboard.GetFEdge((int)IoMicrochip.IoBoardInPortNum.DoorOutsideSensor) && ioboard.GetFEdge((int)IoMicrochip.IoBoardInPortNum.DoorInsideSensor) && (lastState == eDoorState.DoorCloseS || lastState == eDoorState.DoorClose))
                    {
                        //State = eDoorState.DoorClose;
                        detectClose = false;
                        detectOpen = true;
                        detectMiddle = false;
                    }
                    else if (!ioboard.GetRecieveData((int)IoMicrochip.IoBoardInPortNum.DoorOutsideSensor) && ioboard.GetFEdge((int)IoMicrochip.IoBoardInPortNum.DoorInsideSensor) && StateMachineSw.ElapsedMilliseconds > PreferencesDatOriginal.EDoorMiddleCancelCloseTime && (lastState == eDoorState.DoorCloseS || lastState == eDoorState.DoorClose) && !detectOpen)
                    {
                        //State = eDoorState.DoorClose;

                        mainForm.Parent.TraceMessage("eDoor　中間扉DirectionOutIn閉じ指令");
                        detectClose = true;
                        detectOpen = false;
                        detectMiddle = false;
                    }
                    // 閉めからOutsideONだけで開く
                    else if (ioboard.GetEdge((int)IoMicrochip.IoBoardInPortNum.DoorOutsideSensor)
                        && (lastState == eDoorState.DoorCloseS || lastState == eDoorState.DoorClose))
                    {
                        //State = eDoorState.DoorOpen;
                        detectClose = false;
                        detectOpen = true;
                        detectMiddle = false;
                    }
                    //else if (ioboard.GetRecieveData((int)IoMicrochip.IoBoardInPortNum.DoorOutsideSensor) && ioboard.GetFEdge((int)IoMicrochip.IoBoardInPortNum.DoorInsideSensor) && (lastState == eDoorState.DoorCloseS || lastState == eDoorState.DoorClose))
                    //{
                    //    //State = eDoorState.DoorOpen;
                    //    detectClose = true;
                    //    detectOpen = false;
                    //    detectMiddle = false;
                    //}
                    //中に入って待機状態経過時間で閉め
                    else if (!ioboard.GetRecieveData((int)IoMicrochip.IoBoardInPortNum.DoorInsideSensor)
                        && !ioboard.GetRecieveData((int)IoMicrochip.IoBoardInPortNum.DetectInDirection)
                        && StateMachineSw.ElapsedMilliseconds > PreferencesDatOriginal.EDoorMiddleCloseTimeout
                        && (lastState == eDoorState.DoorOpen || lastState == eDoorState.DoorOpenS))
                    {

                        mainForm.Parent.TraceMessage("eDoor　中間扉待機時間閉じ指令");
                        detectClose = true;
                        detectOpen = false;
                        detectMiddle = false;
                    }
                    else if (ioboard.GetEdge((int)IoMicrochip.IoBoardInPortNum.DoorInsideSensor)
                        && !ioboard.GetRecieveData((int)IoMicrochip.IoBoardInPortNum.DetectInDirection)
                        && (lastState == eDoorState.DoorOpen || lastState == eDoorState.DoorOpenS))
                    {
                        //State = eDoorState.DoorClose;

                        mainForm.Parent.TraceMessage("eDoor　中間扉DirectionIn閉じ指令");
                        detectClose = true;
                        detectOpen = false;
                        detectMiddle = false;
                    }
                    //素早く通過した状態
                    //       else if (ioboard.GetFEdge((int)IoMicrochip.IoBoardInPortNum.DoorInsideSensor) && !ioboard.GetRecieveData((int)IoMicrochip.IoBoardInPortNum.DoorOutsideSensor) && lastState == eDoorState.DoorOpen
                    //           && ioboard.GetRecieveData((int)IoMicrochip.IoBoardInPortNum.DoorMotorCWLimit_B) && ioboard.GetRecieveData((int)IoMicrochip.IoBoardInPortNum.DoorMotorCCWLimit_B)
                    //)
                    //       {
                    //           //State = eDoorState.DoorClose;
                    //           detectClose = true;
                    //           detectOpen = false;
                    //           detectMiddle = false;
                    //       }
                    //else if (ioboard.GetFEdge((int)IoMicrochip.IoBoardInPortNum.DoorOutsideSensor) && !ioboard.GetRecieveData((int)IoMicrochip.IoBoardInPortNum.DoorInsideSensor) && lastState == eDoorState.DoorOpen)
                    else if (ioboard.GetFEdge((int)IoMicrochip.IoBoardInPortNum.DoorOutsideSensor)
                        && !ioboard.GetRecieveData((int)IoMicrochip.IoBoardInPortNum.DoorInsideSensor)
                        && (lastState == eDoorState.DoorOpen || lastState == eDoorState.DoorOpenS))
                    {
                        detectClose = false;
                        detectOpen = true;
                        detectMiddle = false;
                    }
                    else if (!ioboard.GetData(IoMicrochip.IoBoardDInCode.PresenceSensor) 
                        && !ioboard.GetRecieveData((int)IoMicrochip.IoBoardInPortNum.DoorInsideSensor)
                        && PresenceSw.ElapsedMilliseconds > PreferencesDatOriginal.EDoorPresenceMiddleDetectTime)
                    {

                        mainForm.Parent.TraceMessage("eDoor 中間扉入室センサー検知閉じ指令");
                        detectOpen = false;
                        detectClose = true;
                        detectMiddle = false;
                    }
                    //else if (ioboard.GetRecieveData((int)IoMicrochip.IoBoardInPortNum.DoorOutsideSensor) && ioboard.GetFEdge((int)IoMicrochip.IoBoardInPortNum.DoorInsideSensor))
                    //{
                    //    detectClose = false;
                    //    detectOpen = true;
                    //    detectMiddle = false;
                    //    // 中間扉即時停止
                    //    MiddleCancel = true;
                    //}

                    //入室時は少し時間を置く
                    if (StateMachineSw.ElapsedMilliseconds > PreferencesDatOriginal.EDoorMiddleIntervalTime)
                    {
                        if (detectClose)
                        {
                            State = eDoorState.DoorClose;
                            StateMachineSw.Restart();
                        }
                        else if (detectOpen)
                            State = eDoorState.DoorOpen;
                    }
                    break;

                case eDoorState.DoorClose:
                    lastState = State;
                    if (ioboard.GetRecieveData((int)IoMicrochip.IoBoardInPortNum.DoorMotorCCWLimit_B))
                    {
                        DoorClose();
                        detectOpen = false;
                        detectClose = false;
                        detectMiddle = false;

                        StateMachineSw.Restart();
                        PresenceSw.Restart();
                        State = eDoorState.DoorCloseS;
                    }
                    else if (StateMachineSw.ElapsedMilliseconds > PreferencesDatOriginal.EDoorCloseMonitorTime
                        && !ioboard.GetRecieveData((int)IoMicrochip.IoBoardInPortNum.DoorMotorCCWLimit_B))
                    {
                        detectMiddle = false;
                        PresenceSw.Restart();
                        State = eDoorState.DoorCloseS;
                    }

                    //if (ioboard.GetEdge((int)IoMicrochip.IoBoardInPortNum.DoorInsideSensor))
                    //{
                    //    State = eDoorState.DoorMiddle;
                    //}
                    break;
                case eDoorState.DoorCloseS:
                    detectClose = false;
                    if (StateMachineSw.ElapsedMilliseconds > PreferencesDatOriginal.EDoorCloseIntervalTime)
                    {
                        if (StateMachineSw.ElapsedMilliseconds > PreferencesDatOriginal.EDoorMonitorStartTime)
                        {
                            lastState = State;

                            // 開きチェックも含む
                            CheckDetectIllegalMiddleStatus();

                            if (ioboard.GetEdge((int)IoMicrochip.IoBoardInPortNum.DoorInsideSensor) && !ioboard.GetRecieveData((int)IoMicrochip.IoBoardInPortNum.DoorOutsideSensor))
                            {
                                //State = eDoorState.DoorMiddle;
                                detectOpen = false;
                                detectClose = false;
                                detectMiddle = true;

                                StateMachineSw.Restart();
                            }
                            // InsideON 状態でOutsideOFFになってからしばらくしてから開くように
                            else if (ioboard.GetRecieveData((int)IoMicrochip.IoBoardInPortNum.DoorInsideSensor) && !ioboard.GetRecieveData((int)IoMicrochip.IoBoardInPortNum.DoorOutsideSensor))
                            {
                                if (StateMachineSw.ElapsedMilliseconds > PreferencesDatOriginal.EDoorReMonitorTime)
                                {
                                    detectOpen = false;
                                    detectClose = false;
                                    detectMiddle = true;

                                    StateMachineSw.Restart();
                                }
                            }
                            else if (ioboard.GetRecieveData((int)IoMicrochip.IoBoardInPortNum.DoorInsideSensor) && detectInCage)
                            {
                                mainForm.Parent.TraceMessage("eDoor 閉じInCage後中間扉指令");
                                detectInCage = false;

                                detectOpen = false;
                                detectClose = false;
                                detectMiddle = true;

                                StateMachineSw.Restart();

                            }
                            else
                            {
                            }
                            if ((ioboard.GetData(IoMicrochip.IoBoardDInCode.PresenceSensor))
                                && PresenceSw.ElapsedMilliseconds > PreferencesDatOriginal.EDoorPresenceOpenTime)
                            {

                                mainForm.Parent.TraceMessage("eDoor 未入室検知ドア開き指令");
                                detectOpen = true;
                                detectClose = false;
                                detectMiddle = false;
                            }
                        }
                        // CloseS時にOpen側LIM本来無い条件
                        if (!ioboard.GetRecieveData((int)IoMicrochip.IoBoardInPortNum.DoorMotorCWLimit_B))
                        {
                            mainForm.Parent.TraceMessage("eDoor detected error status." + "close");
                        }
                        if (detectMiddle)
                            State = eDoorState.DoorMiddle;
                        else if (detectClose)
                        {
                            State = eDoorState.DoorClose;
                            StateMachineSw.Restart();
                        }
                        else if (detectOpen)
                            State = eDoorState.DoorOpen;
                    }
                    if (StateMachineSw.ElapsedMilliseconds > PreferencesDatOriginal.EDoorCloseTimeout) // In Timeout
                    {
                        State = eDoorState.DoorOpen;
                    }
                    break;


                default:
                    break;
            }
        }

        //Middle時には呼ばない
        private void CheckDetectIllegalMiddleStatus()
        {
            if (ioboard.GetRecieveData((int)IoMicrochip.IoBoardInPortNum.DoorMotorCWLimit_B) && ioboard.GetRecieveData((int)IoMicrochip.IoBoardInPortNum.DoorMotorCCWLimit_B)
                        && !statusMiddle)
            {
                detectMiddleSensorStatusTime = StateMachineSw.ElapsedMilliseconds;
                if (detectMiddleSensorStatusTime > 0)
                {
                    // CloseStay時 センサーOFFなら閉め動作
                    if (State != eDoorState.DoorCloseS && State != eDoorState.DoorOpenS)
                    {
                        //CloseS OpenS内から呼ばれる限りここに来ない
                        statusMiddle = true;
                    }
                    else if (State == eDoorState.DoorOpenS && StateMachineSw.ElapsedMilliseconds > PreferencesDatOriginal.EDoorCloseDoorMonitorStartTime)
                    {
                        detectOpen = true;
                        detectClose = false;
                        detectMiddle = false;
                    }
                    //else if (!ioboard.GetData(IoMicrochip.IoBoardDInCode.PresenceSensor) && StateMachineSw.ElapsedMilliseconds > PreferencesDatOriginal.EDoorPresenceMonitorStartTime)
                    else if (StateMachineSw.ElapsedMilliseconds > PreferencesDatOriginal.EDoorCloseDoorMonitorStartTime
                        && !ioboard.GetData(IoMicrochip.IoBoardDInCode.PresenceSensor)
                        && !ioboard.GetRecieveData((int)IoMicrochip.IoBoardInPortNum.DoorInsideSensor))
                    {
                        detectOpen = false;
                        detectClose = true;

                        mainForm.Parent.TraceMessage("eDoor 入室検知 CheckDetect中閉じ指令");
                        detectMiddle = false;
                    }
                }
                if (StateMachineSw.ElapsedMilliseconds > PreferencesDatOriginal.EDoorMiddleTimeout)
                {
                    detectMiddleSensorStatusTime = 0;
                    statusMiddle = false;

                    detectOpen = true;
                    detectClose = false;
                    detectMiddle = false;
                }
            }
            else if (!ioboard.GetRecieveData((int)IoMicrochip.IoBoardInPortNum.DoorMotorCWLimit_B) || !ioboard.GetRecieveData((int)IoMicrochip.IoBoardInPortNum.DoorMotorCCWLimit_B))
            {
                statusMiddle = false;
            }
            else
            {
                detectMiddleSensorStatusTime = 0;
            }
        }

        SyncObject<bool> DoorOperationBusy = new SyncObject<bool>(false);

        /// <summary>
        /// ドア中間位置 タイムアウト追加予定
        /// </summary>
        public void DoorMiddleOpen()
        {
            mainForm.Parent.TraceMessage("eDoor 中間扉動作");
            if (EDoorMiddleTime > 0)
            {
                DoorMiddleOpen(EDoorMiddleTime, PreferencesDatOriginal.EDoorMiddleSpeed);
            }
            else
            {
                DoorMiddleOpen(PreferencesDatOriginal.EDoorOpenToMiddleTime, PreferencesDatOriginal.EDoorMiddleSpeed);
            }
        }

        /// <summary>
        /// ドア中間位置 タイムアウト追加予定 実動作部
        /// </summary>
        /// <param name="openTime">オープンタイム[ms]</param>
        /// <param name="motorSpeed">モータースピード[180-255]</param>
        public void DoorMiddleOpen(int openTime, int motorSpeed)
        {
            StopDoorTask();
            ioboard.SetMotorSpeed(motorSpeed);

            Stopwatch sw = new Stopwatch();
            DoorOperationBusy.Value = true;
            if (!ioboard.GetRecieveData((int)IoMicrochip.IoBoardInPortNum.DoorMotorCWLimit_B))
            {
                ioboard.SetOutBit((byte)IoMicrochip.IoBoardOutPortNum.DoorMotorReverse, true, 0x4);
                ioboard.SetOutBit((byte)IoMicrochip.IoBoardOutPortNum.DoorMotorForward, false, 0x4);
            }
            else if (!ioboard.GetRecieveData((int)IoMicrochip.IoBoardInPortNum.DoorMotorCCWLimit_B))
            {
                ioboard.SetOutBit((byte)IoMicrochip.IoBoardOutPortNum.DoorMotorForward, true, 0x4);
                ioboard.SetOutBit((byte)IoMicrochip.IoBoardOutPortNum.DoorMotorReverse, false, 0x4);
            }
            else
            {
                // センサーオフなら何もしない
                //ioboard.SetOutBit((byte)IoMicrochip.IoBoardOutPortNum.DoorMotorReverse, true, 0x4);
            }
            //sw.Start();
            openMiddleTime = openTime;
            InitializeMiddleDoorTask();
            eDoorMiddleTask.Start();
            //var task = Task.Run(() =>
            //{
            //    while (DoorOperationBusy.Value)
            //    {
            //        // 高速入室お試し
            //        if (!ioboard.GetRecieveData((int)IoMicrochip.IoBoardInPortNum.DoorOutsideSensor) && ioboard.GetFEdge((int)IoMicrochip.IoBoardInPortNum.DoorInsideSensor))
            //        {
            //            ioboard.SetOutBit((byte)IoMicrochip.IoBoardOutPortNum.DoorMotorReverse, false, 0x4);
            //            ioboard.SetOutBit((byte)IoMicrochip.IoBoardOutPortNum.DoorMotorForward, false, 0x4);
            //            DoorOperationBusy.Value = false;
            //            //ここでステート変えても変わらない
            //            State = eDoorState.DoorClose;
            //            DoorOperationBusy.Value = false;
            //            break;
            //        }
            //        Thread.Sleep(100);
            //        // 設定値確認
            //        if (sw.ElapsedMilliseconds > openTime)
            //        {
            //            ioboard.SetOutBit((byte)IoMicrochip.IoBoardOutPortNum.DoorMotorReverse, false, 0x4);
            //            ioboard.SetOutBit((byte)IoMicrochip.IoBoardOutPortNum.DoorMotorForward, false, 0x4);
            //            DoorOperationBusy.Value = false;
            //            break;
            //        }

            //    }
            //});
        }

        /// <summary>
        /// ドアオープン方向移動 設定値による速度
        /// </summary>
        public void DoorOpen()
        {
            mainForm.Parent.TraceMessage("eDoor 開き動作");
            DoorOpen(PreferencesDatOriginal.EDoorOpenSpeed);

        }
        /// <summary>
        /// ドアオープン方向移動 タイムアウト追加予定 実動作部
        /// </summary>
        /// <param name="motorSpeed">モータースピード[180-255]</param>
        public void DoorOpen(int motorSpeed)
        {
            StopDoorTask();
            ioboard.SetMotorSpeed(motorSpeed);

            DoorOperationBusy.Value = true;
            if (ioboard.GetRecieveData((int)IoMicrochip.IoBoardInPortNum.DoorMotorCWLimit_B))
            {
                ioboard.SetOutBit((byte)IoMicrochip.IoBoardOutPortNum.DoorMotorForward, true, 0x4);
                ioboard.SetOutBit((byte)IoMicrochip.IoBoardOutPortNum.DoorMotorReverse, false, 0x4);
            }
            else
            {
                return;
            }
            InitializeOpenDoorTask();
            eDoorOpenTask.Start();
            //var task = Task.Run(() =>
            //{
            //    while (DoorOperationBusy.Value)
            //    {
            //        Thread.Sleep(100);
            //        if (!ioboard.GetRecieveData((int)IoMicrochip.IoBoardInPortNum.DoorMotorCWLimit_B))
            //        {
            //            ioboard.SetOutBit((byte)IoMicrochip.IoBoardOutPortNum.DoorMotorReverse, false, 0x4);
            //            ioboard.SetOutBit((byte)IoMicrochip.IoBoardOutPortNum.DoorMotorForward, false, 0x4);
            //            DoorOperationBusy.Value = false;
            //            break;
            //        }
            //    }
            //});
        }

        /// <summary>
        /// ドアクローズ方向移動 設定値による速度
        /// </summary>
        public void DoorClose()
        {
            mainForm.Parent.TraceMessage("eDoor 閉じ動作");
            DoorClose(PreferencesDatOriginal.EDoorCloseSpeed);
        }
        /// <summary>
        /// ドアクローズ方向移動 タイムアウトつける
        /// </summary>
        /// <param name="motorSpeed">モータースピード[180-255]</param>
        public void DoorClose(int motorSpeed)
        {
            StopDoorTask();
            ioboard.SetMotorSpeed(motorSpeed);

            DoorOperationBusy.Value = true;
            if (ioboard.GetRecieveData((int)IoMicrochip.IoBoardInPortNum.DoorMotorCCWLimit_B))
            {
                ioboard.SetOutBit((byte)IoMicrochip.IoBoardOutPortNum.DoorMotorReverse, true, 0x4);
                ioboard.SetOutBit((byte)IoMicrochip.IoBoardOutPortNum.DoorMotorForward, false, 0x4);
            }
            else
            {
                return;
            }
            InitializeCloseDoorTask();
            eDoorCloseTask.Start();
            //var task = Task.Run(() =>
            //{
            //    while (DoorOperationBusy.Value)
            //    {
            //        // 途中退出時
            //        if (ioboard.GetRecieveData((int)IoMicrochip.IoBoardInPortNum.DoorOutsideSensor) && ioboard.GetFEdge((int)IoMicrochip.IoBoardInPortNum.DoorInsideSensor))
            //        {
            //            ioboard.SetOutBit((byte)IoMicrochip.IoBoardOutPortNum.DoorMotorReverse, false, 0x4);
            //            ioboard.SetOutBit((byte)IoMicrochip.IoBoardOutPortNum.DoorMotorForward, false, 0x4);
            //            DoorOperationBusy.Value = false;
            //            //ここでステート変えたくない
            //            State = eDoorState.DoorOpen;
            //            break;
            //        }
            //        Thread.Sleep(100);
            //        // 通常停止
            //        if (!ioboard.GetRecieveData((int)IoMicrochip.IoBoardInPortNum.DoorMotorCCWLimit_B) || MiddleCancel)
            //        {
            //            ioboard.SetOutBit((byte)IoMicrochip.IoBoardOutPortNum.DoorMotorReverse, false, 0x4);
            //            ioboard.SetOutBit((byte)IoMicrochip.IoBoardOutPortNum.DoorMotorForward, false, 0x4);
            //            DoorOperationBusy.Value = false;
            //            MiddleCancel = false;
            //            break;
            //        }
            //    }
            //});

        }
        private async void StopDoorTask()
        {
            DoorOperationBusy.Value = false;
            await Task.WhenAll(eDoorOpenTask, eDoorMiddleTask, eDoorCloseTask);
        }
        private void InitializeOpenDoorTask()
        {
            eDoorOpenTask = null;
            eDoorOpenTask = new Task(new Action(() =>
            {
                while (DoorOperationBusy.Value)
                {
                    Thread.Sleep(10);
                    if (!ioboard.GetRecieveData((int)IoMicrochip.IoBoardInPortNum.DoorMotorCWLimit_B))
                    {
                        ioboard.SetOutBit((byte)IoMicrochip.IoBoardOutPortNum.DoorMotorReverse, false, 0x4);
                        ioboard.SetOutBit((byte)IoMicrochip.IoBoardOutPortNum.DoorMotorForward, false, 0x4);
                        DoorOperationBusy.Value = false;
                        break;
                    }
                }
            }));
        }
        private void InitializeCloseDoorTask()
        {
            eDoorCloseTask = null;
            eDoorCloseTask = new Task(new Action(() =>
            {
                while (DoorOperationBusy.Value)
                {
                    // 途中退出時 閉待機時からのみ
                    //if (ioboard.GetFEdge((int)IoMicrochip.IoBoardInPortNum.DoorOutsideSensor) && ioboard.GetFEdge((int)IoMicrochip.IoBoardInPortNum.DoorInsideSensor) && (lastState == eDoorState.DoorCloseS))
                    //{
                    //    ioboard.SetOutBit((byte)IoMicrochip.IoBoardOutPortNum.DoorMotorReverse, false, 0x4);
                    //    ioboard.SetOutBit((byte)IoMicrochip.IoBoardOutPortNum.DoorMotorForward, false, 0x4);
                    //    DoorOperationBusy.Value = false;
                    //    //ここでステート変えたくない
                    //    State = eDoorState.DoorOpen;
                    //    break;
                    //}
                    //else
                    if (ioboard.GetRecieveData((int)IoMicrochip.IoBoardInPortNum.DoorOutsideSensor) && ioboard.GetFEdge((int)IoMicrochip.IoBoardInPortNum.DoorInsideSensor) && (lastState == eDoorState.DoorCloseS))
                    {
                        ioboard.SetOutBit((byte)IoMicrochip.IoBoardOutPortNum.DoorMotorReverse, false, 0x4);
                        ioboard.SetOutBit((byte)IoMicrochip.IoBoardOutPortNum.DoorMotorForward, false, 0x4);
                        DoorOperationBusy.Value = false;
                        //ここでステート変えたくない
                        State = eDoorState.DoorOpen;
                        break;
                    }
                    Thread.Sleep(10);
                    // 通常停止
                    if (!ioboard.GetRecieveData((int)IoMicrochip.IoBoardInPortNum.DoorMotorCCWLimit_B) || MiddleCancel)
                    {
                        ioboard.SetOutBit((byte)IoMicrochip.IoBoardOutPortNum.DoorMotorReverse, false, 0x4);
                        ioboard.SetOutBit((byte)IoMicrochip.IoBoardOutPortNum.DoorMotorForward, false, 0x4);
                        DoorOperationBusy.Value = false;
                        MiddleCancel = false;
                        break;
                    }
                }
            }));
        }
        private void InitializeMiddleDoorTask()
        {
            eDoorMiddleTask = null;
            eDoorMiddleTask = new Task(new Action(() =>
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                while (DoorOperationBusy.Value)
                {
                    if (ioboard.GetRecieveData((int)IoMicrochip.IoBoardInPortNum.DoorOutsideSensor) && ioboard.GetFEdge((int)IoMicrochip.IoBoardInPortNum.DoorInsideSensor) && (lastState == eDoorState.DoorCloseS))
                    {
                        ioboard.SetOutBit((byte)IoMicrochip.IoBoardOutPortNum.DoorMotorReverse, false, 0x4);
                        ioboard.SetOutBit((byte)IoMicrochip.IoBoardOutPortNum.DoorMotorForward, false, 0x4);
                        DoorOperationBusy.Value = false;
                        //ここでステート変えたくない
                        State = eDoorState.DoorOpen;
                        break;
                    }
                    // 高速入室お試し
                    //if (!ioboard.GetRecieveData((int)IoMicrochip.IoBoardInPortNum.DoorOutsideSensor) && ioboard.GetFEdge((int)IoMicrochip.IoBoardInPortNum.DoorInsideSensor))
                    //{
                    //    ioboard.SetOutBit((byte)IoMicrochip.IoBoardOutPortNum.DoorMotorReverse, false, 0x4);
                    //    ioboard.SetOutBit((byte)IoMicrochip.IoBoardOutPortNum.DoorMotorForward, false, 0x4);
                    //    DoorOperationBusy.Value = false;
                    //    //ここでステート変えても変わらない
                    //    State = eDoorState.DoorClose;
                    //    DoorOperationBusy.Value = false;
                    //    break;
                    //}
                    Thread.Sleep(10);
                    // 設定値確認
                    if (sw.ElapsedMilliseconds > openMiddleTime)
                    {
                        ioboard.SetOutBit((byte)IoMicrochip.IoBoardOutPortNum.DoorMotorReverse, false, 0x4);
                        ioboard.SetOutBit((byte)IoMicrochip.IoBoardOutPortNum.DoorMotorForward, false, 0x4);
                        DoorOperationBusy.Value = false;
                        break;
                    }

                }
            }));
        }
    }

}
