using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Compartment.FormMain;
using System.Windows.Forms;
using System.Collections.Concurrent;

namespace Compartment
{
    public class DevFeedMulti
    {
        public EDevState devStateVal { set; get; }
        public EDevState devStateValSaved { set; get; }
        public EDevCmd devCmdVal { set; get; }
        public EDevResult devResultVal { set; get; }
        public EDevResult devResultValSaved { set; get; }

        // Unused fields - commented out to remove warnings
        // private int pulseTimeCount;
        // private int timeoutCount;
        private int[] param = new int[10];
        private String[] stringParam = new string[10];
        private int[] paramForInside = new int[10];
        private String[] stringParamForInside = new string[10];

        private Stopwatch stopwatch = new Stopwatch();
        // 1回のみ行う初期化の時に使用
        // trueに設定しておき、初期化を行った後、falseとする
        public bool firstFlag;

        private IoBoardBase ioBoardDevice;

        public Action<string> UpdateFeedStatus = (x) => { };

        private ConcurrentQueue<DevCmdPkt> concurrentQueueDevCmdPktFeed = new ConcurrentQueue<DevCmdPkt>();
        /// <summary>
        /// 給餌中ステータス
        /// </summary>
        public bool Feeding { get => _Feeding.Value; set => _Feeding.Value = value; }

        private readonly SyncObject<bool> _Feeding = new SyncObject<bool>(false);
        readonly ParentHelper<FormMain> mainForm = new ParentHelper<FormMain>();

        private int IoNum = 0;
        public DevFeedMulti(IoBoardBase ioMicrochip, FormMain baseForm, int ioNum)
        {
            ioBoardDevice = ioMicrochip;
            mainForm.SetParent(baseForm);
            CallbackInitForDevFeed();
            IoNum = ioNum;
        }
        private ref PreferencesDat PreferencesDatOriginal
        {
            get
            {
                return ref mainForm.Parent.preferencesDatOriginal;
            }
        }
        private void CallbackInitForDevFeed()
        {
            // devFeed: 初期化
            devStateVal = EDevState.Init;
            devStateValSaved = EDevState.OverRange;
            devCmdVal = EDevCmd.None;
            devResultVal = EDevResult.None;
            devResultValSaved = EDevResult.OverRange;
            firstFlag = true;
        }

        public void CommandEnqueue(DevCmdPkt cmdPkt)
        {
            concurrentQueueDevCmdPktFeed.Enqueue(cmdPkt);
        }

        SyncObject<bool> feedStop = new SyncObject<bool>(false);
        //		public Action<int> callbackExeForDevLever = (a_iInArg) =>
        public void CallbackExeForDevFeed()
        {
            bool funcRet;
            DevCmdPkt devCmdPktLast = null;
            DevCmdPkt devCmdPktCur;

            // コマンド受信: 最後のコマンドが有効
            while (concurrentQueueDevCmdPktFeed.TryDequeue(out devCmdPktCur))
            {
                devCmdPktLast = devCmdPktCur;
            }
            if (devCmdPktLast != null)
            {
                switch (devCmdPktLast.DevCmdVal)
                {
                    case EDevCmd.FeedForward:
                        paramForInside[0] = devCmdPktLast.iParam[0];
                        devStateVal = EDevState.FeedForwardInit;
                        break;
                    case EDevCmd.FeedReverse:
                        paramForInside[0] = devCmdPktLast.iParam[0];
                        devStateVal = EDevState.FeedReverseInit;
                        break;
                    case EDevCmd.FeedStop:
                        devStateVal = EDevState.FeedStopInit;
                        break;
                    default:
                        break;
                }
            }
            switch (devStateVal)
            {
                case EDevState.FeedInit:
                    devResultVal = EDevResult.None;
                    devStateVal = EDevState.FeedStopInit;
                    break;
                case EDevState.FeedStopInit:
                    funcRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.FeedStop);
                    if (funcRet != true)
                    {
                        Debug.WriteLine("Feed:State[FeedStopInit]FeedStop時、IoBoardDevice.SetUpperStateOfDOut()エラー");
                    }
                    else
                    {
                        Feeding = false;
                    }
                    devResultVal = EDevResult.FeedStop;
                    devStateVal = EDevState.FeedStop;
                    break;
                case EDevState.FeedStop:
                    // 出っぱなしやばいので強制停止 早いタイミングでIOアクセスすると反応しない？
                    if (!feedStop.Value)
                    {
                        funcRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.FeedStop);
                        feedStop.Value = true;
                        if (funcRet)
                        {
                            Feeding = false;
                        }
                    }
                    break;
                case EDevState.FeedForwardInit:
                    // 結果: クリア
                    devResultVal = EDevResult.None;
                    //開始時IO送信しない
                    //funcRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.FeedStop);
                    //if (funcRet != true)
                    //{
                    //    Debug.WriteLine("Feed:State[FeedForwardInit]FeedStop時、IoBoardDevice.SetUpperStateOfDOut()エラー");
                    //}
                    // 指定時間==0の時、停止
                    if (paramForInside[0] == 0 && (!PreferencesDatOriginal.EnableConveyor || !ioBoardDevice.GetData(IoMicrochip.IoBoardDInStatusCode.ConveyorBusy, false)))
                    {
                        // Feed完了
                        devResultVal = EDevResult.FeedStop;
                        devStateVal = EDevState.FeedStop;
                        mainForm.Parent.OpFlagFeedOn = true;
                        feedStop.Value = false;
                        break;
                    }
                    if (PreferencesDatOriginal.EnableExtraFeeder)
                    {
                        if (IoNum == 1)
                        {
                            ioBoardDevice.SetOutBit((byte)IoMicrochip.IoBoardOutPortNum.ExtraFeederStart, true, 0x4);
                        }
                        else
                        {
                            ioBoardDevice.SetOutBit((byte)IoMicrochip.IoBoardOutPortNum.ExtraFeeder2Start, true, 0x4);
                        }
                        funcRet = true;
                    }
                    else if (PreferencesDatOriginal.EnableConveyor && IoNum == 1)
                    {
                        funcRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.FeedConveyor);
                    }
                    else
                    {
                        funcRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.FeedForward);
                    }
                    if (funcRet != true)
                    {
                        Debug.WriteLine("Feed:State[FeedForwardInit]FeedFoward時、IoBoardDevice.SetUpperStateOfDOut()エラー");
                    }
                    else
                    {
                        Feeding = true;
                    }
                    if (paramForInside[0] < 0)
                    {
                        // 連続動作時
                        stopwatch.Restart();
                        devResultVal = EDevResult.FeedForward;
                        devStateVal = EDevState.FeedForward;
                    }
                    else
                    {
                        stopwatch.Restart();
                        devResultVal = EDevResult.FeedForward;
                        if (PreferencesDatOriginal.EnableConveyor && IoNum == 1)
                        {
                            if (ioBoardDevice.GetData(IoMicrochip.IoBoardDInStatusCode.ConveyorBusy, false))
                            {
                                //Conveyor接点落とす
                                funcRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.FeedStop);
                                if (funcRet != true)
                                {
                                    Debug.WriteLine("Feed:State[FeedForwardWithTime]FeedStop時、IoBoardDevice.SetUpperStateOfDOut()エラー");
                                }
                                devStateVal = EDevState.FeedConveyorWithTime;
                            }
                        }
                        else if (PreferencesDatOriginal.EnableExtraFeeder)
                        {
                            devStateVal = EDevState.FeedForwardWithTime;
                        }
                        else
                        {
                            devStateVal = EDevState.FeedForwardWithTime;
                        }
                    }
                    break;
                case EDevState.FeedForwardWithTime:
                    {

                        if (PreferencesDatOriginal.EnableExtraFeeder)
                        {
                            // Pulse時間 300ms固定
                            if (stopwatch.ElapsedMilliseconds >= 300)
                            {
                                if (IoNum == 1)
                                {
                                    ioBoardDevice.SetOutBit((byte)IoMicrochip.IoBoardOutPortNum.ExtraFeederStart, false, 0x4);
                                    if (ioBoardDevice.GetRecieveData((int)IoMicrochip.IoBoardInPortNum.ExtraFeederError))
                                    {
                                        Feeding = false;
                                    }
                                }
                                else
                                {
                                    ioBoardDevice.SetOutBit((byte)IoMicrochip.IoBoardOutPortNum.ExtraFeeder2Start, false, 0x4);
                                    if (ioBoardDevice.GetRecieveData((int)IoMicrochip.IoBoardInPortNum.ExtraFeeder2Error))
                                    {
                                        Feeding = false;
                                    }
                                }
                                // Feed完了
                                mainForm.Parent.OpFlagFeedOn = true;
                                devResultVal = EDevResult.FeedStop;
                                devStateVal = EDevState.FeedStop;
                                feedStop.Value = false;
                                break;
                            }
                        }
                        if (stopwatch.ElapsedMilliseconds >= paramForInside[0])
                        {
                            stopwatch.Stop();
                            funcRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.FeedStop);
                            if (funcRet != true)
                            {
                                Debug.WriteLine("Feed:State[FeedForwardWithTime]FeedStop時、IoBoardDevice.SetUpperStateOfDOut()エラー");
                            }
                            else
                            {
                                Feeding = false;
                            }
                            // Feed完了
                            mainForm.Parent.OpFlagFeedOn = true;
                            devResultVal = EDevResult.FeedStop;
                            devStateVal = EDevState.FeedStop;
                            feedStop.Value = false;
                        }
                        break;
                    }
                case EDevState.FeedConveyorWithTime:
                    //コンベアBusyじゃなくなったら
                    if (!ioBoardDevice.GetData(IoMicrochip.IoBoardDInStatusCode.ConveyorBusy, false))
                    {
                        stopwatch.Stop();
                        funcRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.FeedStop);
                        if (funcRet != true)
                        {
                            Debug.WriteLine("Feed:State[FeedForwardWithTime]FeedStop時、IoBoardDevice.SetUpperStateOfDOut()エラー");
                        }

                        // FeedConveyor完了
                        mainForm.Parent.OpFlagFeedOn = true;
                        devResultVal = EDevResult.FeedStop;
                        devStateVal = EDevState.FeedStop;
                        feedStop.Value = false;
                    }
                    //コンベアエラー時は給餌したことにして復帰 エラーメッセージのみ
                    if (ioBoardDevice.GetData(IoMicrochip.IoBoardDInStatusCode.ConveyorError, false))
                    {
                        stopwatch.Stop();
                        Debug.WriteLine("Feedコンベアエラー");
                        mainForm.Parent.opCollection.callbackMessageError("コンベアフィードエラー");

                        mainForm.Parent.OpFlagFeedOn = true;
                        devResultVal = EDevResult.FeedStop;
                        devStateVal = EDevState.FeedStop;
                        feedStop.Value = false;
                    }
                    break;
                case EDevState.FeedForward:
                    if (PreferencesDatOriginal.EnableExtraFeeder)
                    {
                        // Pulse時間 300ms固定
                        if (stopwatch.ElapsedMilliseconds >= 300)
                        {
                            if (IoNum == 1)
                            {
                                ioBoardDevice.SetOutBit((byte)IoMicrochip.IoBoardOutPortNum.ExtraFeederStart, false, 0x4);
                            }
                            else
                            {
                                ioBoardDevice.SetOutBit((byte)IoMicrochip.IoBoardOutPortNum.ExtraFeeder2Start, false, 0x4);
                            }
                            break;
                        }
                    }
                    break;
                case EDevState.FeedReverseInit:
                    // 結果: クリア
                    devResultVal = EDevResult.None;
                    funcRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.FeedStop);
                    if (funcRet != true)
                    {
                        Debug.WriteLine("Feed:State[FeedReverseInit]FeedStop時、IoBoardDevice.SetUpperStateOfDOut()エラー");
                    }
                    else
                    {
                        Feeding = false;
                    }
                    // 指定時間==0の時、停止
                    if (paramForInside[0] == 0)
                    {
                        // Reverse動作完了
                        devResultVal = EDevResult.FeedStop;
                        devStateVal = EDevState.FeedStop;
                        break;
                    }
                    funcRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.FeedReverse);
                    if (funcRet != true)
                    {
                        Debug.WriteLine("Feed:State[FeedReverseInit]FeedReverse時、IoBoardDevice.SetUpperStateOfDOut()エラー");
                    }
                    else
                    {
                        Feeding = true;
                    }
                    if (paramForInside[0] < 0)
                    {
                        // 連続動作時
                        stopwatch.Restart();
                        devResultVal = EDevResult.FeedReverse;
                        devStateVal = EDevState.FeedReverse;
                    }
                    else
                    {
                        stopwatch.Restart();
                        devResultVal = EDevResult.FeedReverse;
                        devStateVal = EDevState.FeedRevserseWithTime;
                    }
                    break;
                case EDevState.FeedRevserseWithTime:
                    {
                        if (stopwatch.ElapsedMilliseconds >= paramForInside[0])
                        {
                            stopwatch.Stop();
                            funcRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.FeedStop);
                            if (funcRet != true)
                            {
                                Debug.WriteLine("Feed:State[FeedReverseWithTime]FeedStop時、IoBoardDevice.SetUpperStateOfDOut()エラー");
                            }
                            else
                            {
                                Feeding = false;
                            }
                            // Reverse動作完了
                            devResultVal = EDevResult.FeedStop;
                            devStateVal = EDevState.FeedStop;
                        }
                        break;
                    }
                case EDevState.FeedReverse:
                    break;
                case EDevState.FeedError:
                    funcRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.FeedStop);
                    if (funcRet != true)
                    {
                        Debug.WriteLine("Lever:State[FeedError] IoBoardDevice.SetUpperStateOfDOut()エラー");
                    }
                    else
                    {
                        Feeding = false;
                    }
                    devResultVal = EDevResult.FeedStop;
                    devStateVal = EDevState.FeedStop;
                    break;
                default:
                    break;
            }
        }
        public void CallbackInitForDevFeedInCheckDevice()
        {
            firstFlag = true;
        }
        public void CallbackExeForDevFeedInCheckDevice()
        {
            // 変化した時、表示更新
            if (devStateVal != devStateValSaved)
            {
                mainForm.Parent.Invoke((MethodInvoker)(() =>
                {
                    UpdateFeedStatus(devResultVal.ToString());
                }));

                devStateValSaved = devStateVal;
            }
            if (devResultVal != devResultValSaved)
            {
                // devFeed.devResultVal.ToString()
                mainForm.Parent.Invoke((MethodInvoker)(() =>
                {
                    UpdateFeedStatus(devResultVal.ToString());
                }));

                devResultValSaved = devResultVal;
            }
            // 最初の初期化は終了
            firstFlag = false;

        }
    }
}
