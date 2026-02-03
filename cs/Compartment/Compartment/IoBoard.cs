using System;
using System.Diagnostics;
#if !DUMMY_IO
using MccDaq;
#endif

namespace Compartment
{
    public enum IoBoardPortNo : int
    {
        /// <summary>
        /// ポート番号
        /// </summary>
        /// 
        Port0 = 0,  // PortA	out
        Port1,  // PortB	in
        Port2,  // PortCL	out
        Port3,  // PortCH	in
        Port4,  // IoExt
        Port5,  // IoExt
        Port6,  // IoExt
        Port7,  // IoExt
        PortRangeOver = 255
    };
    public enum IOBoardInitOutDat : ushort
    {
        Port0 = 0xff,   // PortA
        Port2 = 0xf     // PortCL
    };
    public enum IOBoardDOutCode : ushort
    {
        DoorOpen = 0x01,   // PortA Port: Bit0
        DoorClose = 0x02,   // PortA Port: Bit1

        LeverIn = 0x10,   // PortA Port: Bit4
        LeverOut = 0x20,   // PortA Port: Bit5

        LeverLamp = 0x01,   // PortCL: Bit0

        AirPuff = 0x02,    // PortCL: Bit1

        RoomLamp = 0x40,   // PortA: Bit6

        FeedLamp = 0x80,        // PortA.Bit7

        FeedForward = 0x04,  // PortA.Bit2
        FeedReverse = 0x08, // PortA.Bit3
    };
    public enum IoBoardDInLogicalName : int
    {
        /// <summary>
        /// 入力論理名
        /// </summary>
        /// 
        Min = 0,    // Min
        RoomEntrance = 0,
        RoomExit = 1,
        RoomStay = 2,
        DoorOpen = 3,
        DoorClose = 4,
        LeverIn = 5,
        LeverOut = 6,
        LeverSw = 7,
        Max = 7,    // Max
        RangeOver = 8
    };
    public enum IoBoardDOutLogicalName : int
    {
        /// <summary>
        /// 入力論理名
        /// </summary>
        /// 
        Min = 0,    // Min
        DoorOpen = 1,
        DoorClose = 2,
        DoorStop = 3,

        LeverIn = 4,
        LeverOut = 5,
        LeverStop = 6,

        LeverLampOn = 7,
        LeverLampOff = 8,

        AirPuffOn = 9,
        AirPuffOff = 10,

        RoomLampOn = 11,
        RoomLampOff = 12,

        FeedLampOn = 13,
        FeedLampOff = 14,

        FeedForward = 15,
        FeedReverse = 16,
        FeedStop = 17,
        FeedConveyor = 18,
        Max = 100,    // Max
        RangeOver = 101
    };

    public enum IoBoardDInRawCode : ushort
    {
        /// <summary>
        /// 入力論理名
        /// </summary>
        /// 
        DoorOpen = 0x10,
        DoorClose = 0x08,

        LeverIn = 0x20,
        LeverOut = 0x40,
    };
#if !DUMMY_IO
    public class IoBoard
    {
        private MccDaq.MccBoard mMccDaqBoard = null;
        public String errorMsg { get; set; }
        private ErrorInfo mErrorInfoSave { get; set; }
        private int mErrorValue { get; set; } = (int)ErrorInfo.ErrorCode.NoErrors;

        class DInLogical
        {
            //	IoBoardDInLogicalName Name;
            public IoBoardPortNo IoBoardPortNoObj { set; get; }
            public ushort BitCode { set; get; }
            public bool ActiveType { set; get; }

            public DInLogical(IoBoardPortNo a_ioBoardPortNoObj, ushort a_ushortBitCode, bool a_boolActiveType)
            {
                IoBoardPortNoObj = a_ioBoardPortNoObj;
                BitCode = a_ushortBitCode;
                ActiveType = a_boolActiveType;
            }
        }
        private DInLogical[] mDInLogicalTbl = new DInLogical[]
        {
            new DInLogical(IoBoardPortNo.Port1, 0x04,   false),	// IoBoardDInLogicalName.RoomEntrance	PortB:Bit2→Port1:0x04
			new DInLogical(IoBoardPortNo.Port1, 0x02,   false),	// IoBoardDInLogicalName.RoomExit		PortB:Bit1→Port1:0x02
			new DInLogical(IoBoardPortNo.Port1, 0x01,   false),	// IoBoardDInLogicalName.RoomStay		PortB:Bit0→Port1:0x01
			new DInLogical(IoBoardPortNo.Port1, 0x10,   false),	// IoBoardDInLogicalName.DoorOpen		PortB:Bit4→Port1:0x10
			new DInLogical(IoBoardPortNo.Port1, 0x08,   false),	// IoBoardDInLogicalName.DoorClose		PortB:Bit3→Port1:0x08
			new DInLogical(IoBoardPortNo.Port1, 0x20,   false),	// IoBoardDInLogicalName.LeverIn		PortB:Bit5→Port1:0x20
			new DInLogical(IoBoardPortNo.Port1, 0x40,   false),	// IoBoardDInLogicalName.LeverOut		PortB:Bit6→Port1:0x40
			new DInLogical(IoBoardPortNo.Port1, 0x80,   false)	// IoBoardDInLogicalName.LeverSw		PortB:Bit7→Port1:0x80
		};

        public IoBoard()
        {
            errorMsg = "";
            mErrorInfoSave = new ErrorInfo((int)ErrorInfo.ErrorCode.NoErrors);
        }

        public bool AcquireDevice()
        {
            int targetBoardNo = -1;
            ErrorInfo errorInfoInfo;

            // デバイス取得している時、そのままOkとする
            if (mMccDaqBoard != null)
            {
                return true;
            }

            errorInfoInfo = MccDaq.MccService.ErrHandling(
                                MccDaq.ErrorReporting.DontPrint,
                                MccDaq.ErrorHandling.DontStop);
            if (errorInfoInfo.Value != ErrorInfo.ErrorCode.NoErrors)
            {
                mErrorInfoSave = errorInfoInfo;
                mErrorValue = (int)errorInfoInfo.Value;
                errorMsg = errorInfoInfo.Message + "[in ErrHandling()]";
                Debug.WriteLine(errorMsg);
                return false;
            }

            // InstaCalの情報を無効にする
            MccDaq.DaqDeviceManager.IgnoreInstaCal();

            MccDaq.DaqDeviceDescriptor[] daq_device_deccriptorInventory = MccDaq.DaqDeviceManager.GetDaqDeviceInventory(MccDaq.DaqDeviceInterface.Any);
            if (daq_device_deccriptorInventory.Length == 0)
            {
                mErrorValue = (int)ErrorInfo.ErrorCode.BoardNotExist;
                errorMsg = "IoBoard: There is no board[in Inventory]";
                Debug.WriteLine(errorMsg);
                return false;
            }


            for (int iBoardNo = 0; iBoardNo < daq_device_deccriptorInventory.Length; iBoardNo++)
            {
                Debug.WriteLine("No:{0} ProductName:{1} ProductID]{2} DevString:{3} UniqueID:{4} NUID:{5} InterfaceType:{6}",
                    iBoardNo,
                    daq_device_deccriptorInventory[iBoardNo].ProductName,
                    daq_device_deccriptorInventory[iBoardNo].ProductID,
                    daq_device_deccriptorInventory[iBoardNo].DevString,
                    daq_device_deccriptorInventory[iBoardNo].UniqueID,
                    daq_device_deccriptorInventory[iBoardNo].NUID,
                    daq_device_deccriptorInventory[iBoardNo].InterfaceType);
                // 出力結果: No:0 ProductName:USB-DIO24/37 ProductID]147 DevString:USB-DIO24/37 UniqueID:1F511D7 NUID:32838103 InterfaceType:Usb
                if (daq_device_deccriptorInventory[iBoardNo].ProductName == "USB-DIO24/37")
                {
                    if (targetBoardNo < 0)
                    {
                        // 最初に発見された対象ボードを記録
                        targetBoardNo = iBoardNo;
                    }
                    else
                    {
                        // 対象ボードが複数発見された時
                        mErrorValue = (int)ErrorInfo.ErrorCode.BadBoard;
                        errorMsg = "IoBoard: Multiple boards are found]";
                        Debug.WriteLine(errorMsg);
                        return false;
                    }
                }
            }
            if (targetBoardNo < 0)
            {
                mErrorValue = (int)ErrorInfo.ErrorCode.BoardNotExist;
                errorMsg = "IoBoard: There is no I/O target board[in type]";
                Debug.WriteLine(errorMsg);
                return false;
            }
            // 最初に発見されたUSB DIO24/37ボードを使用する
            try
            {
                // ボードNo:0を割り当て、デバイス生成
                mMccDaqBoard = MccDaq.DaqDeviceManager.CreateDaqDevice(0, daq_device_deccriptorInventory[targetBoardNo]);
            }
            catch (ULException ule)
            {
                mErrorValue = (int)ErrorInfo.ErrorCode.BoardNotExist;
                errorMsg = ule.Message + "[in create]";
                Debug.WriteLine(errorMsg);
                return false;
            }
            errorInfoInfo = mMccDaqBoard.DConfigPort(DigitalPortType.FirstPortA, DigitalPortDirection.DigitalOut);
            if (errorInfoInfo.Value != ErrorInfo.ErrorCode.NoErrors)
            {
                mErrorInfoSave = errorInfoInfo;
                mErrorValue = (int)errorInfoInfo.Value;
                errorMsg = errorInfoInfo.Message + "[in DConfigPort(PortA)]";
                Debug.WriteLine(errorMsg);
                return false;
            }
            errorInfoInfo = mMccDaqBoard.DConfigPort(DigitalPortType.FirstPortB, DigitalPortDirection.DigitalIn);
            if (errorInfoInfo.Value != ErrorInfo.ErrorCode.NoErrors)
            {
                mErrorInfoSave = errorInfoInfo;
                mErrorValue = (int)errorInfoInfo.Value;
                errorMsg = errorInfoInfo.Message + "[in DConfigPort(PortB)]";
                Debug.WriteLine(errorMsg);
                return false;
            }
            errorInfoInfo = mMccDaqBoard.DConfigPort(DigitalPortType.FirstPortCH, DigitalPortDirection.DigitalIn);
            if (errorInfoInfo.Value != ErrorInfo.ErrorCode.NoErrors)
            {
                mErrorInfoSave = errorInfoInfo;
                mErrorValue = (int)errorInfoInfo.Value;
                errorMsg = errorInfoInfo.Message + "[in DConfigPort(PortCH)]";
                Debug.WriteLine(errorMsg);
                return false;
            }
            errorInfoInfo = mMccDaqBoard.DConfigPort(DigitalPortType.FirstPortCL, DigitalPortDirection.DigitalOut);
            if (errorInfoInfo.Value != ErrorInfo.ErrorCode.NoErrors)
            {
                mErrorInfoSave = errorInfoInfo;
                mErrorValue = (int)errorInfoInfo.Value;
                errorMsg = errorInfoInfo.Message + "[in DConfigPort(PortCL)]";
                Debug.WriteLine(errorMsg);
                return false;
            }
            // 出力ポートA:初期値設定
            errorInfoInfo = mMccDaqBoard.DOut(DigitalPortType.FirstPortA, (ushort)IOBoardInitOutDat.Port0);
            if (errorInfoInfo.Value != ErrorInfo.ErrorCode.NoErrors)
            {
                mErrorInfoSave = errorInfoInfo;
                mErrorValue = (int)errorInfoInfo.Value;
                errorMsg = errorInfoInfo.Message + "[in DOut(PortA)]";
                Debug.WriteLine(errorMsg);
                return false;
            }
            // 出力ポートCL:初期値設定
            errorInfoInfo = mMccDaqBoard.DOut(DigitalPortType.FirstPortCL, (ushort)IOBoardInitOutDat.Port2);
            if (errorInfoInfo.Value != ErrorInfo.ErrorCode.NoErrors)
            {
                mErrorInfoSave = errorInfoInfo;
                mErrorValue = (int)errorInfoInfo.Value;
                errorMsg = errorInfoInfo.Message + "[in DOut(PortCL)]";
                Debug.WriteLine(errorMsg);
                return false;
            }
            return true;
        }

        public bool ReleaseDevice()
        {
            bool l_boolRet = true;

            if (mMccDaqBoard != null)
            {
                MccDaq.DaqDeviceManager.ReleaseDaqDevice(mMccDaqBoard);
                mMccDaqBoard = null;
                goto out_func;
            }
        out_func:
            return l_boolRet;
        }

        private static readonly object IoOutSyncObject = new object();
        private static readonly object IoIntSyncObject = new object();
        public bool DirectOut(IoBoardPortNo a_IoBoardPortNoObj, ushort a_ushortOutCode)
        {
            bool l_boolRet = true;
            DigitalPortType l_DigitalPortTypePort;
            ErrorInfo errorInfoInfo;

            switch (a_IoBoardPortNoObj)
            {
                case IoBoardPortNo.Port0: l_DigitalPortTypePort = DigitalPortType.FirstPortA; break;
                //	case IoBoardPortNo.Port1: l_DigitalPortTypePort = DigitalPortType.FirstPortB; break;
                case IoBoardPortNo.Port2: l_DigitalPortTypePort = DigitalPortType.FirstPortCL; break;
                //	case IoBoardPortNo.Port3: l_DigitalPortTypePort = DigitalPortType.FirstPortCH; break;
                default:
                    mErrorValue = (int)ErrorInfo.ErrorCode.BadPortNum;
                    errorMsg = "Bad port num[in DirectOut]";
                    Debug.WriteLine(errorMsg);
                    l_boolRet = false;
                    goto out_func;
            }
            lock (IoOutSyncObject)
            {
                errorInfoInfo = mMccDaqBoard.DOut(l_DigitalPortTypePort, a_ushortOutCode);
            }
            if (errorInfoInfo.Value != ErrorInfo.ErrorCode.NoErrors)
            {
                mErrorInfoSave = errorInfoInfo;
                mErrorValue = (int)errorInfoInfo.Value;
                errorMsg = errorInfoInfo.Message + "[in DOut()]";
                Debug.WriteLine(errorMsg);
                l_boolRet = false;
                goto out_func;
            }
        out_func:
            return l_boolRet;
        }
        public bool DirectIn(IoBoardPortNo a_IoBoardPortNoObj, out ushort a_ushortInCode)
        {
            DigitalPortType l_DigitalPortTypePort;
            ErrorInfo errorInfoInfo;

            // 初期値設定
            a_ushortInCode = 0;
            switch (a_IoBoardPortNoObj)
            {
                case IoBoardPortNo.Port0: l_DigitalPortTypePort = DigitalPortType.FirstPortA; break;
                case IoBoardPortNo.Port1: l_DigitalPortTypePort = DigitalPortType.FirstPortB; break;
                case IoBoardPortNo.Port2: l_DigitalPortTypePort = DigitalPortType.FirstPortCL; break;
                case IoBoardPortNo.Port3: l_DigitalPortTypePort = DigitalPortType.FirstPortCH; break;
                default:
                    mErrorValue = (int)ErrorInfo.ErrorCode.BadPortNum;
                    errorMsg = "Bad port num[in DirectIn]";
                    Debug.WriteLine(errorMsg);
                    return false;
            }

            if (mMccDaqBoard is null)
            {
                return false;
            }
            lock (IoIntSyncObject)
            {
                errorInfoInfo = mMccDaqBoard.DIn(l_DigitalPortTypePort, out a_ushortInCode);
            }
            if (errorInfoInfo.Value != ErrorInfo.ErrorCode.NoErrors)
            {
                mErrorInfoSave = errorInfoInfo;
                mErrorValue = (int)errorInfoInfo.Value;
                errorMsg = errorInfoInfo.Message + "[in DIn()]";
                Debug.WriteLine(errorMsg);
                return false;
            }

            return true;
        }
        /// <summary>
        /// SaveDIn()にて入力ポートを保存するプロパティ
        /// </summary>
        public ushort SaveDInForPort1 { get; set; } = 0x00;
        public ushort SaveDInForPort3 { get; set; } = 0x00;

        /// <summary>
        /// 入力ポートの内容をメンバ変数へセーブする
        /// </summary>
        /// <returns></returns>
        public bool SaveDIn()
        {
            bool l_boolRet = true;
            bool funcRet;
            ushort l_ushortDIn;

            funcRet = DirectIn(IoBoardPortNo.Port1, out l_ushortDIn);
            SaveDInForPort1 = l_ushortDIn;
            if (funcRet != true)
            {
                l_boolRet = false;
                goto out_func;
            }

            funcRet = DirectIn(IoBoardPortNo.Port3, out l_ushortDIn);
            SaveDInForPort3 = l_ushortDIn;
            if (funcRet != true)
            {
                l_boolRet = false;
                goto out_func;
            }
        out_func:
            return l_boolRet;
        }
        /// <summary>
        /// 論理入力ポートのそのままの論理状態(Raw)を取得
        /// </summary>
        /// <param name="a_IoBoardDInLogicalNameObj"></param>
        /// <param name="a_boolRawState"></param>
        /// <returns></returns>
        public bool GetRawStateOfSaveDIn(IoBoardDInLogicalName a_IoBoardDInLogicalNameObj, out bool a_boolRawState)
        {
            bool l_boolRet = true;
            DInLogical l_dInLogicalObj;
            ushort l_ushortDIn = 0x00;

            // 出力変数: 初期化
            a_boolRawState = false;
            if (a_IoBoardDInLogicalNameObj < IoBoardDInLogicalName.Min ||
                a_IoBoardDInLogicalNameObj > IoBoardDInLogicalName.Max)
            {
                mErrorValue = (int)ErrorInfo.ErrorCode.BadPortNum;
                errorMsg = "Bad port num[in GetRawStateOfSaveDIn()]";
                Debug.WriteLine(errorMsg);
                l_boolRet = false;
                goto out_func;
            }
            l_dInLogicalObj = mDInLogicalTbl[(int)a_IoBoardDInLogicalNameObj];
            switch (l_dInLogicalObj.IoBoardPortNoObj)
            {
                case IoBoardPortNo.Port1:
                    l_ushortDIn = SaveDInForPort1;
                    break;
                case IoBoardPortNo.Port3:
                    l_ushortDIn = SaveDInForPort3;
                    break;
                default:
                    mErrorValue = (int)ErrorInfo.ErrorCode.BadPortNum;
                    errorMsg = "Bad port num[in GetRawStateOfSaveDIn()]";
                    Debug.WriteLine(errorMsg);
                    l_boolRet = false;
                    goto out_func;
            }
            if ((l_ushortDIn & l_dInLogicalObj.BitCode) == 0)
            {
                // Bit==Lowの時
                if (l_dInLogicalObj.ActiveType)
                {
                    // Highアクティブの時
                    a_boolRawState = false;
                }
                else
                {
                    // Lowアクティブの時
                    a_boolRawState = true;
                }
            }
            else
            {
                // Bit==Highの時
                if (l_dInLogicalObj.ActiveType)
                {
                    // Highアクティブの時
                    a_boolRawState = true;
                }
                else
                {
                    // Lowアクティブの時
                    a_boolRawState = false;
                }
            }
        out_func:
            return l_boolRet;
        }
        /// <summary>
        /// 論理入力ポートの上位層の論理状態を取得
        /// 注意: 今後このレベルで中間処理を入れる事が可能なように直接GetRawStateOfSaveDIn()はコールしない事
        /// </summary>
        /// <param name="a_IoBoardDInLogicalNameObj"></param>
        /// <param name="a_boolLogicalState"></param>
        /// <returns></returns>
        public bool GetUpperStateOfSaveDIn(IoBoardDInLogicalName a_IoBoardDInLogicalNameObj, out bool a_boolLogicalState)
        {
            return GetRawStateOfSaveDIn(a_IoBoardDInLogicalNameObj, out a_boolLogicalState);
        }
        public enum EIoBoardDoutLogicalName : int
        {
            /// <summary>
            /// 入力論理名
            /// </summary>
            /// 
            Min = 0,    // Min
            DoorOpen = 1,
            DoorClose = 2,
            DoorStop = 3,
            Max = 3,    // Max
            RangeOver = 4
        };

        public bool SetUpperStateOfDOut(IoBoardDOutLogicalName a_IoBoardDOutLogicalNameObj)
        {
            bool ret = true;
            bool funcRet;
            ushort inCode;

            switch ((int)a_IoBoardDOutLogicalNameObj)
            {
                case (int)IoBoardDOutLogicalName.DoorOpen:
                    funcRet = DirectIn(IoBoardPortNo.Port0, out inCode);
                    if (funcRet != true)
                    {
                        ret = false;
                        goto out_func;
                    }
                    inCode = (ushort)((inCode & (~((ushort)IOBoardDOutCode.DoorOpen))) | ((ushort)IOBoardDOutCode.DoorClose));
                    funcRet = DirectOut(IoBoardPortNo.Port0, inCode);
                    if (funcRet != true)
                    {
                        ret = false;
                        goto out_func;
                    }
                    break;
                case (int)IoBoardDOutLogicalName.DoorClose:
                    funcRet = DirectIn(IoBoardPortNo.Port0, out inCode);
                    if (funcRet != true)
                    {
                        ret = false;
                        goto out_func;
                    }
                    inCode = (ushort)((inCode & (~((ushort)IOBoardDOutCode.DoorClose))) | ((ushort)IOBoardDOutCode.DoorOpen));
                    funcRet = DirectOut(IoBoardPortNo.Port0, inCode);
                    if (funcRet != true)
                    {
                        ret = false;
                        goto out_func;
                    }
                    break;
                case (int)IoBoardDOutLogicalName.DoorStop:
                    funcRet = DirectIn(IoBoardPortNo.Port0, out inCode);
                    if (funcRet != true)
                    {
                        ret = false;
                        goto out_func;
                    }
                    inCode = (ushort)(inCode | (((ushort)IOBoardDOutCode.DoorOpen) | ((ushort)IOBoardDOutCode.DoorClose)));
                    funcRet = DirectOut(IoBoardPortNo.Port0, inCode);
                    if (funcRet != true)
                    {
                        ret = false;
                        goto out_func;
                    }
                    break;
                case (int)IoBoardDOutLogicalName.LeverIn:
                    funcRet = DirectIn(IoBoardPortNo.Port0, out inCode);
                    if (funcRet != true)
                    {
                        ret = false;
                        goto out_func;
                    }
                    inCode = (ushort)((inCode & (~((ushort)IOBoardDOutCode.LeverIn))) | ((ushort)IOBoardDOutCode.LeverOut));
                    funcRet = DirectOut(IoBoardPortNo.Port0, inCode);
                    if (funcRet != true)
                    {
                        ret = false;
                        goto out_func;
                    }
                    break;
                case (int)IoBoardDOutLogicalName.LeverOut:
                    funcRet = DirectIn(IoBoardPortNo.Port0, out inCode);
                    if (funcRet != true)
                    {
                        ret = false;
                        goto out_func;
                    }
                    inCode = (ushort)((inCode & (~((ushort)IOBoardDOutCode.LeverOut))) | ((ushort)IOBoardDOutCode.LeverIn));
                    funcRet = DirectOut(IoBoardPortNo.Port0, inCode);
                    if (funcRet != true)
                    {
                        ret = false;
                        goto out_func;
                    }
                    break;
                case (int)IoBoardDOutLogicalName.LeverStop:
                    funcRet = DirectIn(IoBoardPortNo.Port0, out inCode);
                    if (funcRet != true)
                    {
                        ret = false;
                        goto out_func;
                    }
                    inCode = (ushort)(inCode | (((ushort)IOBoardDOutCode.LeverIn) | ((ushort)IOBoardDOutCode.LeverOut)));
                    funcRet = DirectOut(IoBoardPortNo.Port0, inCode);
                    if (funcRet != true)
                    {
                        ret = false;
                        goto out_func;
                    }
                    break;
                case (int)IoBoardDOutLogicalName.LeverLampOn:
                    funcRet = DirectIn(IoBoardPortNo.Port2, out inCode);
                    if (funcRet != true)
                    {
                        ret = false;
                        goto out_func;
                    }
                    inCode = (ushort)((inCode & (~((ushort)IOBoardDOutCode.LeverLamp))));
                    funcRet = DirectOut(IoBoardPortNo.Port2, inCode);
                    if (funcRet != true)
                    {
                        ret = false;
                        goto out_func;
                    }
                    break;
                case (int)IoBoardDOutLogicalName.LeverLampOff:
                    funcRet = DirectIn(IoBoardPortNo.Port2, out inCode);
                    if (funcRet != true)
                    {
                        ret = false;
                        goto out_func;
                    }
                    inCode = (ushort)((inCode | ((ushort)IOBoardDOutCode.LeverLamp)));
                    funcRet = DirectOut(IoBoardPortNo.Port2, inCode);
                    if (funcRet != true)
                    {
                        ret = false;
                        goto out_func;
                    }
                    break;
                case (int)IoBoardDOutLogicalName.AirPuffOn:
                    funcRet = DirectIn(IoBoardPortNo.Port2, out inCode);
                    if (funcRet != true)
                    {
                        ret = false;
                        goto out_func;
                    }
                    inCode = (ushort)((inCode & (~((ushort)IOBoardDOutCode.AirPuff))));
                    funcRet = DirectOut(IoBoardPortNo.Port2, inCode);
                    if (funcRet != true)
                    {
                        ret = false;
                        goto out_func;
                    }
                    break;
                case (int)IoBoardDOutLogicalName.AirPuffOff:
                    funcRet = DirectIn(IoBoardPortNo.Port2, out inCode);
                    if (funcRet != true)
                    {
                        ret = false;
                        goto out_func;
                    }
                    inCode = (ushort)((inCode | ((ushort)IOBoardDOutCode.AirPuff)));
                    funcRet = DirectOut(IoBoardPortNo.Port2, inCode);
                    if (funcRet != true)
                    {
                        ret = false;
                        goto out_func;
                    }
                    break;
                case (int)IoBoardDOutLogicalName.RoomLampOn:
                    funcRet = DirectIn(IoBoardPortNo.Port0, out inCode);
                    if (funcRet != true)
                    {
                        ret = false;
                        goto out_func;
                    }
                    inCode = (ushort)((inCode & (~((ushort)IOBoardDOutCode.RoomLamp))));
                    funcRet = DirectOut(IoBoardPortNo.Port0, inCode);
                    if (funcRet != true)
                    {
                        ret = false;
                        goto out_func;
                    }
                    break;
                case (int)IoBoardDOutLogicalName.RoomLampOff:
                    funcRet = DirectIn(IoBoardPortNo.Port0, out inCode);
                    if (funcRet != true)
                    {
                        ret = false;
                        goto out_func;
                    }
                    inCode = (ushort)((inCode | ((ushort)IOBoardDOutCode.RoomLamp)));
                    funcRet = DirectOut(IoBoardPortNo.Port0, inCode);
                    if (funcRet != true)
                    {
                        ret = false;
                        goto out_func;
                    }
                    break;
                case (int)IoBoardDOutLogicalName.FeedForward:
                    funcRet = DirectIn(IoBoardPortNo.Port0, out inCode);
                    if (funcRet != true)
                    {
                        ret = false;
                        goto out_func;
                    }
                    inCode = (ushort)((inCode & (~((ushort)IOBoardDOutCode.FeedForward))) | ((ushort)IOBoardDOutCode.FeedReverse));
                    funcRet = DirectOut(IoBoardPortNo.Port0, inCode);
                    if (funcRet != true)
                    {
                        ret = false;
                        goto out_func;
                    }
                    break;
                case (int)IoBoardDOutLogicalName.FeedReverse:
                    funcRet = DirectIn(IoBoardPortNo.Port0, out inCode);
                    if (funcRet != true)
                    {
                        ret = false;
                        goto out_func;
                    }
                    inCode = (ushort)((inCode & (~((ushort)IOBoardDOutCode.FeedReverse))) | ((ushort)IOBoardDOutCode.FeedForward));
                    funcRet = DirectOut(IoBoardPortNo.Port0, inCode);
                    if (funcRet != true)
                    {
                        ret = false;
                        goto out_func;
                    }
                    break;
                case (int)IoBoardDOutLogicalName.FeedStop:
                    funcRet = DirectIn(IoBoardPortNo.Port0, out inCode);
                    if (funcRet != true)
                    {
                        ret = false;
                        goto out_func;
                    }
                    inCode = (ushort)(inCode | (((ushort)IOBoardDOutCode.FeedForward) | ((ushort)IOBoardDOutCode.FeedReverse)));
                    funcRet = DirectOut(IoBoardPortNo.Port0, inCode);
                    if (funcRet != true)
                    {
                        ret = false;
                        goto out_func;
                    }
                    break;
                case (int)IoBoardDOutLogicalName.FeedLampOn:
                    funcRet = DirectIn(IoBoardPortNo.Port0, out inCode);
                    if (funcRet != true)
                    {
                        ret = false;
                        goto out_func;
                    }
                    inCode = (ushort)((inCode & (~((ushort)IOBoardDOutCode.FeedLamp))));
                    funcRet = DirectOut(IoBoardPortNo.Port0, inCode);
                    if (funcRet != true)
                    {
                        ret = false;
                        goto out_func;
                    }
                    break;
                case (int)IoBoardDOutLogicalName.FeedLampOff:
                    funcRet = DirectIn(IoBoardPortNo.Port0, out inCode);
                    if (funcRet != true)
                    {
                        ret = false;
                        goto out_func;
                    }
                    inCode = (ushort)((inCode | ((ushort)IOBoardDOutCode.FeedLamp)));
                    funcRet = DirectOut(IoBoardPortNo.Port0, inCode);
                    if (funcRet != true)
                    {
                        ret = false;
                        goto out_func;
                    }
                    break;
                default:
                    mErrorValue = (int)ErrorInfo.ErrorCode.BadPortNum;
                    errorMsg = "Bad port num[in GetRawStateOfSaveDIn()]";
                    Debug.WriteLine(errorMsg);
                    ret = false;
                    goto out_func;
            }
        out_func:
            return ret;
        }
    }
#endif
}
