
using System;

namespace Compartment
{
    public class IoBoardDummy : IoBoardBase
    {
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

        public IoBoardDummy()
        {
            errorMsg = "";
        }

        public override bool AcquireDevice()
        {
            return true;
        }

        public override bool ReleaseDevice()
        {
            bool l_boolRet = true;
            return l_boolRet;
        }

        private static readonly object IoOutSyncObject = new object();
        private static readonly object IoIntSyncObject = new object();
        public override bool DirectOut(IoBoardPortNo a_IoBoardPortNoObj, ushort a_ushortOutCode)
        {
            bool l_boolRet = true;

            return l_boolRet;
        }
        public override bool DirectIn(IoBoardPortNo a_IoBoardPortNoObj, out ushort a_ushortInCode)
        {
            a_ushortInCode = 0;
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
        public override bool SaveDIn()
        {
            bool l_boolRet = true;

            return l_boolRet;
        }
        /// <summary>
        /// 論理入力ポートのそのままの論理状態(Raw)を取得
        /// </summary>
        /// <param name="a_IoBoardDInLogicalNameObj"></param>
        /// <param name="a_boolRawState"></param>
        /// <returns></returns>
        public override bool GetRawStateOfSaveDIn(IoBoardDInLogicalName a_IoBoardDInLogicalNameObj, out bool a_boolRawState)
        {
            bool l_boolRet = true;
            a_boolRawState = true;
            return l_boolRet;
        }
        /// <summary>
        /// 論理入力ポートの上位層の論理状態を取得
        /// 注意: 今後このレベルで中間処理を入れる事が可能なように直接GetRawStateOfSaveDIn()はコールしない事
        /// </summary>
        /// <param name="a_IoBoardDInLogicalNameObj"></param>
        /// <param name="a_boolLogicalState"></param>
        /// <returns></returns>
        public override bool GetUpperStateOfSaveDIn(IoBoardDInLogicalName a_IoBoardDInLogicalNameObj, out bool a_boolLogicalState)
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

        public override bool SetUpperStateOfDOut(IoBoardDOutLogicalName a_IoBoardDOutLogicalNameObj)
        {
            return true;
        }

        public override bool GetData(IoMicrochip.IoBoardDInCode ioBoardDInCode) { return true; }
        public override bool GetData(IoMicrochip.IoBoardDInStatusCode ioBoardDInCode, bool n) { return true; }
    }
}
