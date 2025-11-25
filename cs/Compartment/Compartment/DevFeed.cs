using System.Diagnostics;
using System.Windows.Forms;

namespace Compartment
{
    class DevFeed
    {
    }
#if NO_USE_FEED_MULTI
    public partial class FormMain : Form
    {
        /// <summary>
        /// 給餌中ステータス
        /// </summary>
        public bool Feeding { get => _Feeding.Value; set => _Feeding.Value = value; }

        private readonly SyncObject<bool> _Feeding = new SyncObject<bool>(false);
        private void CallbackInitForDevFeed()
        {
            // devFeed: 初期化
            devFeed.devStateVal = EDevState.Init;
            devFeed.devStateValSaved = EDevState.OverRange;
            devFeed.devCmdVal = EDevCmd.None;
            devFeed.devResultVal = EDevResult.None;
            devFeed.devResultValSaved = EDevResult.OverRange;
            devFeed.firstFlag = true;
        }

        SyncObject<bool> feedStop = new SyncObject<bool>(false);
        //		public Action<int> callbackExeForDevLever = (a_iInArg) =>
        private void CallbackExeForDevFeed()
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
                        devFeed.paramForInside[0] = devCmdPktLast.iParam[0];
                        devFeed.devStateVal = EDevState.FeedForwardInit;
                        break;
                    case EDevCmd.FeedReverse:
                        devFeed.paramForInside[0] = devCmdPktLast.iParam[0];
                        devFeed.devStateVal = EDevState.FeedReverseInit;
                        break;
                    case EDevCmd.FeedStop:
                        devFeed.devStateVal = EDevState.FeedStopInit;
                        break;
                    default:
                        break;
                }
            }
            switch (devFeed.devStateVal)
            {
                case EDevState.FeedInit:
                    devFeed.devResultVal = EDevResult.None;
                    devFeed.devStateVal = EDevState.FeedStopInit;
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
                    devFeed.devResultVal = EDevResult.FeedStop;
                    devFeed.devStateVal = EDevState.FeedStop;
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
                    devFeed.devResultVal = EDevResult.None;
                    //開始時IO送信しない
                    //funcRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.FeedStop);
                    //if (funcRet != true)
                    //{
                    //    Debug.WriteLine("Feed:State[FeedForwardInit]FeedStop時、IoBoardDevice.SetUpperStateOfDOut()エラー");
                    //}
                    // 指定時間==0の時、停止
                    if (devFeed.paramForInside[0] == 0 && (!preferencesDatOriginal.EnableConveyor || !ioBoardDevice.GetData(IoMicrochip.IoBoardDInStatusCode.ConveyorBusy, false)))
                    {
                        // Feed完了
                        devFeed.devResultVal = EDevResult.FeedStop;
                        devFeed.devStateVal = EDevState.FeedStop;
                        OpFlagFeedOn = true;
                        feedStop.Value = false;
                        break;
                    }
                    if (preferencesDatOriginal.EnableExtraFeeder)
                    {
                        ioBoardDevice.SetOutBit((byte)IoMicrochip.IoBoardOutPortNum.ExtraFeederStart, true, 0x4);
                        funcRet = true;
                    }
                    else if (preferencesDatOriginal.EnableConveyor)
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
                    if (devFeed.paramForInside[0] < 0)
                    {
                        // 連続動作時
                        devFeed.devResultVal = EDevResult.FeedForward;
                        devFeed.devStateVal = EDevState.FeedForward;
                    }
                    else
                    {
                        devFeed.stopwatch.Restart();
                        devFeed.devResultVal = EDevResult.FeedForward;
                        if (preferencesDatOriginal.EnableConveyor)
                        {
                            if (ioBoardDevice.GetData(IoMicrochip.IoBoardDInStatusCode.ConveyorBusy, false))
                            {
                                //Conveyor接点落とす
                                funcRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.FeedStop);
                                if (funcRet != true)
                                {
                                    Debug.WriteLine("Feed:State[FeedForwardWithTime]FeedStop時、IoBoardDevice.SetUpperStateOfDOut()エラー");
                                }
                                devFeed.devStateVal = EDevState.FeedConveyorWithTime;
                            }
                        }
                        else if (preferencesDatOriginal.EnableExtraFeeder)
                        {
                            devFeed.devStateVal = EDevState.FeedForwardWithTime;
                        }
                        else
                        {
                            devFeed.devStateVal = EDevState.FeedForwardWithTime;
                        }
                    }
                    break;
                case EDevState.FeedForwardWithTime:
                    {

                        if (preferencesDatOriginal.EnableExtraFeeder)
                        {
                            // Pulse時間 300ms固定
                            if (devFeed.stopwatch.ElapsedMilliseconds >= 300)
                            {
                                ioBoardDevice.SetOutBit((byte)IoMicrochip.IoBoardOutPortNum.ExtraFeederStart, false, 0x4);
                                if (ioBoardDevice.GetRecieveData((int)IoMicrochip.IoBoardInPortNum.ExtraFeederError))
                                {
                                    Feeding = false;
                                }

                                // Feed完了
                                OpFlagFeedOn = true;
                                devFeed.devResultVal = EDevResult.FeedStop;
                                devFeed.devStateVal = EDevState.FeedStop;
                                feedStop.Value = false;
                                break;
                            }
                        }
                        if (devFeed.stopwatch.ElapsedMilliseconds >= devFeed.paramForInside[0])
                        {
                            devFeed.stopwatch.Stop();
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
                            OpFlagFeedOn = true;
                            devFeed.devResultVal = EDevResult.FeedStop;
                            devFeed.devStateVal = EDevState.FeedStop;
                            feedStop.Value = false;
                        }
                        break;
                    }
                case EDevState.FeedConveyorWithTime:
                    //コンベアBusyじゃなくなったら
                    if (!ioBoardDevice.GetData(IoMicrochip.IoBoardDInStatusCode.ConveyorBusy, false))
                    {
                        devFeed.stopwatch.Stop();
                        funcRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.FeedStop);
                        if (funcRet != true)
                        {
                            Debug.WriteLine("Feed:State[FeedForwardWithTime]FeedStop時、IoBoardDevice.SetUpperStateOfDOut()エラー");
                        }

                        // FeedConveyor完了
                        OpFlagFeedOn = true;
                        devFeed.devResultVal = EDevResult.FeedStop;
                        devFeed.devStateVal = EDevState.FeedStop;
                        feedStop.Value = false;
                    }
                    //コンベアエラー時は給餌したことにして復帰 エラーメッセージのみ
                    if (ioBoardDevice.GetData(IoMicrochip.IoBoardDInStatusCode.ConveyorError, false))
                    {
                        devFeed.stopwatch.Stop();
                        Debug.WriteLine("Feedコンベアエラー");
                        opCollection.callbackMessageError("コンベアフィードエラー");

                        OpFlagFeedOn = true;
                        devFeed.devResultVal = EDevResult.FeedStop;
                        devFeed.devStateVal = EDevState.FeedStop;
                        feedStop.Value = false;
                    }
                    break;
                case EDevState.FeedForward:
                    if (preferencesDatOriginal.EnableExtraFeeder)
                    {
                        // Pulse時間 300ms固定
                        if (devFeed.stopwatch.ElapsedMilliseconds >= 300)
                        {
                            ioBoardDevice.SetOutBit((byte)IoMicrochip.IoBoardOutPortNum.ExtraFeederStart, false, 0x4);
                            break;
                        }
                    }
                    break;
                case EDevState.FeedReverseInit:
                    // 結果: クリア
                    devFeed.devResultVal = EDevResult.None;
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
                    if (devFeed.paramForInside[0] == 0)
                    {
                        // Reverse動作完了
                        devFeed.devResultVal = EDevResult.FeedStop;
                        devFeed.devStateVal = EDevState.FeedStop;
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
                    if (devFeed.paramForInside[0] < 0)
                    {
                        // 連続動作時
                        devFeed.devResultVal = EDevResult.FeedReverse;
                        devFeed.devStateVal = EDevState.FeedReverse;
                    }
                    else
                    {
                        devFeed.stopwatch.Restart();
                        devFeed.devResultVal = EDevResult.FeedReverse;
                        devFeed.devStateVal = EDevState.FeedRevserseWithTime;
                    }
                    break;
                case EDevState.FeedRevserseWithTime:
                    {
                        if (devFeed.stopwatch.ElapsedMilliseconds >= devFeed.paramForInside[0])
                        {
                            devFeed.stopwatch.Stop();
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
                            devFeed.devResultVal = EDevResult.FeedStop;
                            devFeed.devStateVal = EDevState.FeedStop;
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
                    devFeed.devResultVal = EDevResult.FeedStop;
                    devFeed.devStateVal = EDevState.FeedStop;
                    break;
                default:
                    break;
            }
        }
        private void CallbackInitForDevFeedInCheckDevice()
        {
            devFeedInCheckDevice.firstFlag = true;
        }
        private void CallbackExeForDevFeedInCheckDevice()
        {
            // 変化した時、表示更新
            if (devFeed.devStateVal != devFeed.devStateValSaved)
            {
                Invoke((MethodInvoker)(() =>
                {
                    userControlCheckDeviceOnFormMain.textBoxDOutFeedStateOnGpFeedOnUcCheckDevice.Text = devFeed.devStateVal.ToString();
                }));

                devFeed.devStateValSaved = devFeed.devStateVal;
            }
            if (devFeed.devResultVal != devFeed.devResultValSaved)
            {
                // devFeed.devResultVal.ToString()
                Invoke((MethodInvoker)(() =>
                {
                    userControlCheckDeviceOnFormMain.textBoxDOutFeedResultOnGpFeedOnUcCheckDevice.Text = devFeed.devResultVal.ToString();
                }));

                devFeed.devResultValSaved = devFeed.devResultVal;
            }
            // 最初の初期化は終了
            devFeedInCheckDevice.firstFlag = false;

        }
    }
#endif
}
