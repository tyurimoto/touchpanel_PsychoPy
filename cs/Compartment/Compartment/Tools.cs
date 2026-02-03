#define BG_WORKER       //サル二倍速用

using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Windows.Forms;


namespace Compartment
{
    class Tools
    {
    }
    public partial class FormMain : Form
    {
        #region タイマ・インターバルからカウント数を取得
        // タイマ・インターバルを保存
        int iTimerInterval { set; get; } = 1;
        void SaveTimerInterval(int a_iIntervalMs)
        {
            iTimerInterval = a_iIntervalMs;
        }
        int GetTimerCountFromInterval(int a_iTimeMs)
        {
            int l_iRetTimerCount;

            l_iRetTimerCount = a_iTimeMs / iTimerInterval;
            if (l_iRetTimerCount <= 0)
            {
                l_iRetTimerCount = 1;
            }
            return l_iRetTimerCount;
        }
        #endregion

        public enum EMainState : int
        {
            Init = 0,
            Min = 0,
            Exe = 1,
            Max = 100,
            OverRange = 101
        };
        public enum EOpeModeType : int
        {
            Operation = 0,
            Min = 0,
            CheckDevice = 1,
            OtherMode = 2,
            Max = 2,
            OverRange = 3
        };
        public enum EDevState : int
        {
            Init = 0,
            //			Min = 0,

#if true
            // 新版
            DoorInit = 1,
            DoorStop,
            DoorStopInitAfterWait,
            DoorStopInitAfterWaitPulseOff,
            DoorStopInit,
            DoorStopWithOpenPulseOn1,
            DoorStopWithOpenPulseOff1,
            DoorStopWithOpenPulseOn2,
            DoorStopWithClosePulseOff2,
            DoorStopWithClosePulseOn,

            DoorOpenInitAfterWait,
            DoorOpenInitAfterWaitPulseOff,
            DoorOpenInit,
            DoorOpenPulseOn1,
            DoorOpenPulseOff1,
            DoorOpenPulseOn2,
            DoorOpenDoing,
            DoorOpenRecoverInit,
            DoorOpenRecoverInitRetry,
            DoorOpenRecoverWithClosePulseOn1,
            DoorOpenRecoverWithClosePulseOff1,
            DoorOpenRecoverWithClosePulseOn2,   // Add
            DoorOpenRecoverWithClosing,
            DoorOpenRecoverWithOpenPulseOn1,
            DoorOpenRecoverWithOpenPulseOff1,
            DoorOpenRecoverWithOpenPulseOn2,    //Add
            DoorOpenRecoverWithOpenning,
            DoorOpenRecoverTimeoutInit,
            DoorOpenRecoverTimeoutWithClosePulseOn1,
            DoorOpenRecoverTimeoutWithClosePulseOff1,
            DoorOpenRecoverTimeoutWithClosePulseOn2,
            DoorOpenRecoverTimeoutWithClosing,

            DoorCloseInitAfterWait,
            DoorCloseInitAfterWaitPulseOff,
            DoorCloseInit,
            DoorClosePulseOn1,
            DoorClosePulseOff1,
            DoorClosePulseOn2,
            DoorCloseDoing,
            DoorCloseRecoverInit,
            DoorCloseRecoverInitRetry,
            DoorCloseRecoverWithOpenPulseOn1,
            DoorCloseRecoverWithOpenPulseOff1,
            DoorCloseRecoverWithOpenPulseOn2,   // Add
            DoorCloseRecoverWithOpenning,
            DoorCloseRecoverWithClosePulseOn1,
            DoorCloseRecoverWithClosePulseOff1,
            DoorCloseRecoverWithClosePulseOn2,   //Add
            DoorCloseRecoverWithClosing,
            DoorCloseRecoverTimeoutInit,
            DoorCloseRecoverTimeoutWithOpenPulseOn1,
            DoorCloseRecoverTimeoutWithOpenPulseOff1,
            DoorCloseRecoverTimeoutWithOpenPulseOn2,
            DoorCloseRecoverTimeoutWithOpenning,
#else
			// 旧版
			DoorInit =1,
			DoorStop = 2,
//			DoorMin = 3,
			DoorOpenInit = 4,
			DoorOpenPulseOn = 5,
			DoorOpenning = 6,
			DoorCloseInit=7,
			DoorClosePulseOn= 8,
			DoorClosing=9,
			DoorOpenTimeout = 10,
			DoorCloseTimeout = 11,
			DoorReset=12,
			DoorError = 13,
//			DoorMax = 14,
#endif

#if true

            // 新版
            LeverInit = 500,
            LeverStop,
            LeverStopInitAfterWait,
            LeverStopInitAfterWaitPulseOff,
            LeverStopInit,
            LeverStopWithInPulseOn1,
            LeverStopWithInPulseOff1,
            LeverStopWithInPulseOn2,
            LeverStopWithOutPulseOff2,
            LeverStopWithOutPulseOn,
            LeverInInitAfterWait,
            LeverInInitAfterWaitPulseOff,
            LeverInInit,
            LeverInPulseOn1,
            LeverInPulseOff1,
            LeverInPulseOn2,
            LeverInDoing,
            LeverInRecoverInit,
            LeverInRecoverInitRetry,
            LeverInRecoverWithOutPulseOn1,
            LeverInRecoverWithOutPulseOff1,
            LeverInRecoverWithOutPulseOn2,
            LeverInRecoverWithOutDoing,
            LeverInRecoverWithInPulseOn1,
            LeverInRecoverWithInPulseOff1,
            LeverInRecoverWithInPulseOn2,
            LeverInRecoverWithInDoing,
            LeverInRecoverTimeoutInit,
            LeverInRecoverTimeoutWithOutPulseOn1,
            LeverInRecoverTimeoutWithOutPulseOff1,
            LeverInRecoverTimeoutWithOutPulseOn2,
            LeverInRecoverTimeoutWithOutDoing,
            LeverOutInitAfterWait,
            LeverOutInitAfterWaitPulseOff,
            LeverOutInit,
            LeverOutPulseOn1,
            LeverOutPulseOff1,
            LeverOutPulseOn2,
            LeverOutDoing,
            LeverOutRecoverInit,
            LeverOutRecoverInitRetry,
            LeverOutRecoverWithInPulseOn1,
            LeverOutRecoverWithInPulseOff1,
            LeverOutRecoverWithInPulseOn2,
            LeverOutRecoverWithInDoing,
            LeverOutRecoverWithOutPulseOn1,
            LeverOutRecoverWithOutPulseOff1,
            LeverOutRecoverWithOutPulseOn2,
            LeverOutRecoverWithOutDoing,
            LeverOutRecoverTimeoutInit,
            LeverOutRecoverTimeoutWithInPulseOn1,
            LeverOutRecoverTimeoutWithInPulseOff1,
            LeverOutRecoverTimeoutWithInPulseOn2,
            LeverOutRecoverTimeoutWithInDoing,
#else
			LeverStop = 500,
			LeverOutInit,
			LeverOutPulseOn,
			LeverOut,
			LeverInInit,
			LeverInPulseOn,
			LeverIn,
			LeverOutTimeout,
			LeverInTimeout,
			LeverReset,
			LeverError,
#endif

            AirPuffInit = 600,
            AirPuffOn,
            AirPuffOff,

            FeedInit = 700,
            FeedStopInit,
            FeedStop,
            FeedForwardInit,
            FeedForward,
            FeedForwardWithTime,
            FeedConveyorWithTime,
            FeedReverseInit,
            FeedReverse,
            FeedRevserseWithTime,
            FeedError,

            TouchPanelInit = 800,
            TouchPanelStop,
            TouchPanelTouchAny,
            TouchPanelCorrectShape,
            TouchPanelCorrectShapeAny,

            RoomInOutInit = 900,
            RoomInOutIdle,
            RoomInOutEnterHead,
            RoomInOutEnterBody,
            RoomInOutEnterTail,
            RoomInOutLeaveHead,
            RoomInOutLeaveBody,
            RoomInOutLeaveTail,

            RoomSnsInit = 1000,
            RoomSnsIdle,
            RoomSnsWaitIn,
            RoomSnsWaitOut,

            Max = 5000,
            OverRange = 5001
        };
        public enum EDevCmd : int
        {
            None = 0,
            //			Min = 0,

            DoorStop = 1,
            //			DoorMin = 1,
            DoorOpen = 2,
            DoorClose = 3,
            DoorReset = 4,
            //			DoorMax = 4,

            LeverIn = 20,
            //			LeverMin = 20,
            LeverOut = 21,
            LeverStop = 22,
            LeverReset = 23,
            //			LeverMax = 23,

            AirPuffOn = 40,
            //			AirPuffMin= 40,
            AirPuffOff = 41,
            //			AirPuffMax = 42,

            FeedStop = 60,
            //			FeedMin=60,
            FeedForward = 61,
            FeedReverse = 62,
            //			FeedMax=62,

            TouchPanelStop = 80,
            //			TouchPanelMin = 80,
            TouchPanelTouchAny = 81,
            TouchPanelCorrectShape = 82,
            TouchPanelCorrectShapeAny = 83,
            //			TouchPaneMax= 82,

            Max = 100,
            OverRange = 101
        };
        public enum EDevResult : int
        {
            None = 0,
            //			Min = 0,

            DoorOpenRunning = 1,
            //			DoorMin = 1,
            DoorOpenned = 2,
            DoorOpenTimeout = 3,
            DoorCloseRunning = 4,
            DoorClosed = 5,
            DoorCloseTimeout = 6,
            DoorError = 7,
            //			DoorMax = 7,

            LeverInDoing = 20,
            //			LeverMin = 20,
            LeverInDone = 21,
            LeverInTimeout = 22,
            LeverOutDoing = 23,
            LeverOutDone = 24,
            LeverOutTimeout = 25,
            LeverError = 26,
            //			LeverMax = 26,

            FeedForward = 40,
            //			FeedMin = 40,
            FeedReverse = 41,
            FeedStop = 42,
            FeedError = 43,
            //			FeedMax = 43,

            Max = 100,
            OverRange = 101
        };
        public enum EDeviceResult : int
        {
            None = 0,
            Done = 1,
            TimeOut = 2
        };
        public class Dev
        {
            public Stopwatch stopwatch = new Stopwatch();
            public Stopwatch stopwatchPulse = new Stopwatch();
            public Stopwatch stopwatchTimeout = new Stopwatch();

            //			public Dev(Action<int> a_callbackInit, Action<int> a_callbackExe)
            //			{
            //				callbackInit = a_callbackInit;
            //				callbackExe = a_callbackExe;
            //			}
            public EDevState devStateVal { set; get; }
            public EDevState devStateValSaved { set; get; }
            public EDevCmd devCmdVal { set; get; }
            public EDevResult devResultVal { set; get; }
            public EDevResult devResultValSaved { set; get; }
            public int pulseTimeCount;
            public int timeoutCount;
            public int[] param = new int[10];
            public String[] stringParam = new string[10];
            public int[] paramForInside = new int[10];
            public String[] stringParamForInside = new string[10];

            // 1回のみ行う初期化の時に使用
            // trueに設定しておき、初期化を行った後、falseとする
            public bool firstFlag;
            // センサ等の変化時の表示更新に使用
            // 前回のセンサの状態を保存しておく
            public bool dIn00Saved;
            public bool dIn01Saved;
            public bool dIn02Saved;
            public uint intervalCount = 0;
            public int retryCount;

            public Action callbackInit = () => { };
            public Action callbackExe = () => { };
        }
        public class DevIn
        {
            public EDevState devStateVal { set; get; }
            public EDevState devStateValSaved { set; get; }
            public EDevCmd devCmdVal { set; get; }
            public EDevResult devResultVal { set; get; }
            public EDevResult devResultValSaved { set; get; }

            public bool[] devValSaved = new bool[5];
            public long timeoutCount;

            // 1回のみ行う初期化の時に使用
            // trueに設定しておき、初期化を行った後、falseとする
            public bool firstFlag;
            // センサ等の変化時の表示更新に使用
            // 前回のセンサの状態を保存しておく
            public bool dInOld;
            public bool dInCurrent;

            public Action callbackInit = () => { };
            public Action callbackExe = () => { };
        }
        public class DevCmdPkt
        {
            public EDevCmd DevCmdVal { set; get; }
            public int[] iParam = new int[10];
        }
        public ConcurrentQueue<DevCmdPkt> concurrentQueueDevCmdPktTouchPanel = new ConcurrentQueue<DevCmdPkt>();
        public ConcurrentQueue<DevCmdPkt> concurrentQueueDevCmdPktFeed = new ConcurrentQueue<DevCmdPkt>();
        public ConcurrentQueue<DevCmdPkt> concurrentQueueDevCmdPktDoor = new ConcurrentQueue<DevCmdPkt>();
        public ConcurrentQueue<DevCmdPkt> concurrentQueueDevCmdPktLever = new ConcurrentQueue<DevCmdPkt>();

        //		public Dev	devDoor= new Dev((Action<int>)callbackInitForDevDoor, (Action<int>)callbackExeForDevDoor);
        public Dev devDoor = new Dev();
        public Dev devDoorInCheckDevice = new Dev();
        public Dev devLever = new Dev();
        public Dev devLeverInCheckDevice = new Dev();
        public Dev devRoomInCheckDevice = new Dev();
        //public Dev devFeed = new Dev();
        public DevFeedMulti devFeed;
        public DevFeedMulti devFeed2;
        public Dev devFeedInCheckDevice = new Dev();
        public Dev devTouchPanel = new Dev();
        public DevIn devInDoorOpen = new DevIn();
        public DevIn devInDoorClose = new DevIn();
        public DevIn devInLeverIn = new DevIn();
        public DevIn devInLeverOut = new DevIn();
        public DevIn devInLeverSw = new DevIn();
        public DevIn devInRoomEntrance = new DevIn();
        public DevIn devInRoomExit = new DevIn();
        public DevIn devInRoomStay = new DevIn();
        public DevIn devInRoomInOut = new DevIn();
        public DevIn devInRoomSns = new DevIn();
        public EMainState mainStateVal = new EMainState();
        public EOpeModeType opeModeTypeVal = new EOpeModeType();
        /// <summary>
        /// Operationの時、受信したID codeを保存する
        /// </summary>
        public SyncObject<String> IdCodeInOperation = new SyncObject<String>("");

        private void timerOperation_TickSub(object sender, EventArgs e)
        {
#if !BG_WORKER
			Timer timerObj = (Timer)sender;
#endif

            // 入力ポートを読み保存
            ioBoardDevice.SaveDIn();
            switch (mainStateVal)
            {
                case EMainState.Init:
#if !BG_WORKER
					// タイマ: インターバル値を保存
					SaveTimerInterval(timerObj.Interval);
#endif
                    devDoor.callbackInit = CallbackInitForDevDoor;
                    devDoor.callbackExe = CallbackExeForDevDoor;

                    devInDoorOpen.callbackInit = CallbackInitForDevInDoorOpen;
                    devInDoorOpen.callbackExe = CallbackExeForDevInDoorOpen;
                    devInDoorClose.callbackInit = CallbackInitForDevInDoorClose;
                    devInDoorClose.callbackExe = CallbackExeForDevInDoorClose;

                    devLever.callbackInit = CallbackInitForDevLever;
                    devLever.callbackExe = CallbackExeForDevLever;

                    devInLeverIn.callbackInit = CallbackInitForDevInLeverIn;
                    devInLeverIn.callbackExe = CallbackExeForDevInLeverIn;
                    devInLeverOut.callbackInit = CallbackInitForDevInLeverOut;
                    devInLeverOut.callbackExe = CallbackExeForDevInLeverOut;
                    devInLeverSw.callbackInit = CallbackInitForDevInLeverSw;
                    devInLeverSw.callbackExe = CallbackExeForDevInLeverSw;

                    devInRoomEntrance.callbackInit = CallbackInitForDevInRoomEntrance;
                    devInRoomEntrance.callbackExe = CallbackExeForDevInRoomEntrance;
                    devInRoomExit.callbackInit = CallbackInitForDevInRoomExit;
                    devInRoomExit.callbackExe = CallbackExeForDevInRoomExit;
                    devInRoomStay.callbackInit = CallbackInitForDevInRoomStay;
                    devInRoomStay.callbackExe = CallbackExeForDevInRoomStay;
                    devInRoomInOut.callbackInit = CallbackInitForDevInRoomInOut;
                    devInRoomInOut.callbackExe = CallbackExeForDevInRoomInOut;
                    devInRoomSns.callbackInit = CallbackInitForDevInRoomSns;
                    devInRoomSns.callbackExe = CallbackExeForDevInRoomSns;

                    //devFeed.callbackInit = CallbackInitForDevFeed;
                    //devFeed.callbackExe = CallbackExeForDevFeed;

                    devTouchPanel.callbackInit = CallbackInitFordevTouchPanel;
                    devTouchPanel.callbackExe = CallbackExeFordevTouchPanel;

                    switch (opeModeTypeVal)
                    {
                        case EOpeModeType.Operation:
                            devDoor.callbackInit();

                            devInDoorOpen.callbackInit();
                            devInDoorClose.callbackInit();

                            devLever.callbackInit();
                            devInLeverIn.callbackInit();
                            devInLeverOut.callbackInit();
                            devInLeverSw.callbackInit();

                            devInRoomEntrance.callbackInit();
                            devInRoomExit.callbackInit();
                            devInRoomStay.callbackInit();
                            devInRoomInOut.callbackInit();
                            devInRoomSns.callbackInit();

                            //devFeed.CallbackInitForDevFeed();

                            devTouchPanel.callbackInit();
                            break;
                        case EOpeModeType.CheckDevice:
                            devDoorInCheckDevice.callbackInit = CallbackInitForDevDoorInCheckDevice;
                            devDoorInCheckDevice.callbackExe = CallbackExeForDevDoorInCheckDevice;

                            devLeverInCheckDevice.callbackInit = CallbackInitForDevLeverInCheckDevice;
                            devLeverInCheckDevice.callbackExe = CallbackExeForDevLeverInCheckDevice;

                            devRoomInCheckDevice.callbackInit = CallbackInitForDevRoomInCheckDevice;
                            devRoomInCheckDevice.callbackExe = CallbackExeForDevRoomInCheckDevice;

                            //devFeedInCheckDevice.callbackInit = CallbackInitForDevFeedInCheckDevice;
                            //devFeedInCheckDevice.callbackExe = CallbackExeForDevFeedInCheckDevice;

                            devDoor.callbackInit();

                            devInDoorOpen.callbackInit();
                            devInDoorClose.callbackInit();

                            devLever.callbackInit();
                            devInLeverIn.callbackInit();
                            devInLeverOut.callbackInit();
                            devInLeverSw.callbackInit();

                            devInRoomEntrance.callbackInit();
                            devInRoomExit.callbackInit();
                            devInRoomStay.callbackInit();
                            devInRoomInOut.callbackInit();
                            devInRoomSns.callbackInit();

                            //devFeed.callbackInit();
                            //devFeed.CallbackInitForDevFeedInCheckDevice();

                            devTouchPanel.callbackInit();

                            devLeverInCheckDevice.callbackInit();
                            devDoorInCheckDevice.callbackInit();
                            devRoomInCheckDevice.callbackInit();
                            devFeedInCheckDevice.callbackInit();


                            CallbackEDoorStatus();

                            break;
                        default:
                            break;
                    }
                    mainStateVal = EMainState.Exe;
                    break;
                case EMainState.Exe:
                    switch (opeModeTypeVal)
                    {
                        case EOpeModeType.Operation:
                            devDoor.callbackExe();
                            devInDoorOpen.callbackExe();
                            devInDoorClose.callbackExe();

                            devLever.callbackExe();
                            devInLeverIn.callbackExe();
                            devInLeverOut.callbackExe();
                            devInLeverSw.callbackExe();

                            devInRoomEntrance.callbackExe();
                            devInRoomExit.callbackExe();
                            devInRoomStay.callbackExe();
                            devInRoomInOut.callbackExe();
                            devInRoomSns.callbackExe();

                            //devFeed.callbackExe();
                            devFeed.CallbackExeForDevFeed();
                            devFeed2.CallbackExeForDevFeed();

                            devTouchPanel.callbackExe();
                            break;
                        case EOpeModeType.CheckDevice:
                            devDoor.callbackExe();
                            devInDoorOpen.callbackExe();
                            devInDoorClose.callbackExe();

                            devLever.callbackExe();
                            devInLeverIn.callbackExe();
                            devInLeverOut.callbackExe();
                            devInLeverSw.callbackExe();

                            devInRoomEntrance.callbackExe();
                            devInRoomExit.callbackExe();
                            devInRoomStay.callbackExe();
                            devInRoomInOut.callbackExe();
                            devInRoomSns.callbackExe();

                            //devFeed.callbackExe();

                            devFeed.CallbackExeForDevFeed();
                            devFeed.CallbackExeForDevFeedInCheckDevice();
                            devFeed2.CallbackExeForDevFeed();
                            devFeed2.CallbackExeForDevFeedInCheckDevice();

                            devTouchPanel.callbackExe();

                            devFeedInCheckDevice.callbackExe();
                            devRoomInCheckDevice.callbackExe();
                            devLeverInCheckDevice.callbackExe();
                            devDoorInCheckDevice.callbackExe();


                            CallbackEDoorStatus();
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }
            if ((mainStateVal == EMainState.Exe) &&
                (opeModeTypeVal == EOpeModeType.Operation))
            {
                // OnOperationStateMachineProc();
            }

            #region デバッグ動作確認用コード
#if false

			{
				if (OpFlagOpenDoor == true)
				{
					OpFlagOpenDoor = false;
					String l_stringText = String.Format("FlagOpendDoor==true" + Environment.NewLine + "Result:{0}", OpResultOpenDoor.ToString());
					Invoke((MethodInvoker)(() =>
					{
						userControlCheckDeviceOnFormMain.textBoxIdCodeOnGpIdCodeOnUcCheckDevice.AppendText(l_stringText);
					}));
				}
				if (OpFlagCloseDoor == true)
				{
					OpFlagCloseDoor = false;
					String l_stringText = String.Format("FlagCloseDoor==true" + Environment.NewLine + "Result:{0}", OpResultCloseDoor.ToString());
					Invoke((MethodInvoker)(() =>
					{
						userControlCheckDeviceOnFormMain.textBoxIdCodeOnGpIdCodeOnUcCheckDevice.AppendText(l_stringText);
					}));
				}
				if (OpFlagMoveLeverIn == true)
				{
					OpFlagMoveLeverIn = false;
					String l_stringText = String.Format("FlagMoveLeverIn==true" + Environment.NewLine + "Result:{0}", OpResultMoveLeverIn.ToString());
					Invoke((MethodInvoker)(() =>
					{
						userControlCheckDeviceOnFormMain.textBoxIdCodeOnGpIdCodeOnUcCheckDevice.AppendText(l_stringText);
					}));
				}
				if (OpFlagMoveLeverOut== true)
				{
					OpFlagMoveLeverOut= false;
					String l_stringText = String.Format("FlagMoveLeverOut==true" + Environment.NewLine + "Result:{0}", OpResultMoveLeverOut.ToString());
					Invoke((MethodInvoker)(() =>
					{
						userControlCheckDeviceOnFormMain.textBoxIdCodeOnGpIdCodeOnUcCheckDevice.AppendText(l_stringText);
					}));
				}
				if (OpFlagFeedOn == true)
				{
					OpFlagFeedOn = false;
					String l_stringText = "OpFlagFeedOn==true" + Environment.NewLine;
					Invoke((MethodInvoker)(() =>
					{
						userControlCheckDeviceOnFormMain.textBoxIdCodeOnGpIdCodeOnUcCheckDevice.AppendText(l_stringText);
					}));
				}
			}
#endif
            #endregion
        }
        //		public Action<int> callbackInitForDevDoor = (a_iInArg) =>
    }

}
