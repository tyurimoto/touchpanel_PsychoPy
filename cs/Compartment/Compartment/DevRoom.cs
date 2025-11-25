using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace Compartment
{
    class DevRoom
    {
    }

    public enum RoomInTimeout : int
    {
        // [ms]
        ExitToEntrance = 3000,
        EntranceToStay = 3000
    };
    public enum RoomOutTimeout : int
    {
        // [ms]
        EntranceToExit = 3000,
        ExitToStayOff = 3000
    };

    public partial class FormMain : Form
    {
        public EDevState DevStateSave { get; set; } = EDevState.Init;
        private readonly Stopwatch devRoomStopwatch = new Stopwatch();

        private void CallbackInitForDevInRoomEntrance()
        {
            bool funcRet;
            bool logicalStateOfEnt;

            // RoomEntranceセンサ
            funcRet = ioBoardDevice.GetUpperStateOfSaveDIn(IoBoardDInLogicalName.RoomEntrance, out logicalStateOfEnt);
            if (funcRet != true)
            {
                Debug.WriteLine("ExeForDevInRoomEntrance:IoBoardDevice.GetUpperStateOfSaveDIn(RoomEntrance)エラー");
            }
            devInRoomEntrance.dInCurrent = logicalStateOfEnt;

            devInRoomEntrance.firstFlag = true;
        }
        private void CallbackExeForDevInRoomEntrance()
        {
            bool funcRet;
            bool logicalStateOfEnt;

            // LeverInセンサ
            funcRet = ioBoardDevice.GetUpperStateOfSaveDIn(IoBoardDInLogicalName.RoomEntrance, out logicalStateOfEnt);
            if (funcRet != true)
            {
                Debug.WriteLine("ExeForDevInRoomEntrance:IoBoardDevice.GetUpperStateOfSaveDIn(RoomEntrance)エラー");
            }
            devInRoomEntrance.dInCurrent = logicalStateOfEnt;
        }
        private void CallbackInitForDevInRoomExit()
        {
            bool funcRet;
            bool logicalStateOfExit;

            // RoomExitセンサ
            funcRet = ioBoardDevice.GetUpperStateOfSaveDIn(IoBoardDInLogicalName.RoomExit, out logicalStateOfExit);
            if (funcRet != true)
            {
                Debug.WriteLine("ExeForDevInRoomExit:IoBoardDevice.GetUpperStateOfSaveDIn(RoomExit)エラー");
            }
            devInRoomExit.dInCurrent = logicalStateOfExit;

            devInRoomExit.firstFlag = true;
        }
        private void CallbackExeForDevInRoomExit()
        {
            bool funcRet;
            bool logicalStateOfExit;

            // RoomExitセンサ
            funcRet = ioBoardDevice.GetUpperStateOfSaveDIn(IoBoardDInLogicalName.RoomExit, out logicalStateOfExit);
            if (funcRet != true)
            {
                Debug.WriteLine("ExeForDevInRoomExit:IoBoardDevice.GetUpperStateOfSaveDIn(RoomExit)エラー");
            }
            devInRoomExit.dInCurrent = logicalStateOfExit;
        }
        private void CallbackInitForDevInRoomStay()
        {
            bool funcRet;
            bool logicalStateOfStay;

            // RoomStayセンサ
            funcRet = ioBoardDevice.GetUpperStateOfSaveDIn(IoBoardDInLogicalName.RoomStay, out logicalStateOfStay);
            if (funcRet != true)
            {
                Debug.WriteLine("ExeForDevInRoomStay:IoBoardDevice.GetUpperStateOfSaveDIn(RoomStay)エラー");
            }
            devInRoomStay.dInCurrent = logicalStateOfStay;

            devInRoomStay.firstFlag = true;
        }
        private void CallbackExeForDevInRoomStay()
        {
            bool funcRet;
            bool logicalStateOfStay;

            // LeverInセンサ
            funcRet = ioBoardDevice.GetUpperStateOfSaveDIn(IoBoardDInLogicalName.RoomStay, out logicalStateOfStay);
            if (funcRet != true)
            {
                Debug.WriteLine("ExeForDevInRoomStay:IoBoardDevice.GetUpperStateOfSaveDIn(RoomStay)エラー");
            }
            devInRoomStay.dInCurrent = logicalStateOfStay;
        }
        private void CallbackInitForDevRoomInCheckDevice()
        {
            devRoomInCheckDevice.firstFlag = true;
        }
        private void CallbackExeForDevRoomInCheckDevice()
        {
            // 変化した時、表示更新
            if ((devRoomInCheckDevice.firstFlag == true) ||
                (devInRoomEntrance.dInCurrent != devRoomInCheckDevice.dIn00Saved))
            {
                if (devInRoomEntrance.dInCurrent == true)
                {
                    Invoke((MethodInvoker)(() =>
                    {
                        userControlCheckDeviceOnFormMain.textBoxEntranceOnGpRoomOnUcCheckDevice.Text = "ON";
                        userControlCheckDeviceOnFormMain.textBoxEntranceOnGpRoomOnUcCheckDevice.BackColor = Color.Red;
                        userControlCheckDeviceOnFormMain.textBoxEntranceOnGpRoomOnUcCheckDevice.ForeColor = Color.White;
                    }));
                }
                else
                {
                    Invoke((MethodInvoker)(() =>
                    {
                        userControlCheckDeviceOnFormMain.textBoxEntranceOnGpRoomOnUcCheckDevice.Text = "OFF";
                        userControlCheckDeviceOnFormMain.textBoxEntranceOnGpRoomOnUcCheckDevice.BackColor = Color.Black;
                        userControlCheckDeviceOnFormMain.textBoxEntranceOnGpRoomOnUcCheckDevice.ForeColor = Color.White;
                    }));
                }
                devRoomInCheckDevice.dIn00Saved = devInRoomEntrance.dInCurrent;
            }
            if ((devRoomInCheckDevice.firstFlag == true) ||
                (devInRoomExit.dInCurrent != devRoomInCheckDevice.dIn01Saved))
            {
                if (devInRoomExit.dInCurrent == true)
                {
                    Invoke((MethodInvoker)(() =>
                    {
                        userControlCheckDeviceOnFormMain.textBoxExitOnGpRoomOnUcCheckDevice.Text = "ON";
                        userControlCheckDeviceOnFormMain.textBoxExitOnGpRoomOnUcCheckDevice.BackColor = Color.Red;
                        userControlCheckDeviceOnFormMain.textBoxExitOnGpRoomOnUcCheckDevice.ForeColor = Color.White;
                    }));
                }
                else
                {
                    Invoke((MethodInvoker)(() =>
                    {
                        userControlCheckDeviceOnFormMain.textBoxExitOnGpRoomOnUcCheckDevice.Text = "OFF";
                        userControlCheckDeviceOnFormMain.textBoxExitOnGpRoomOnUcCheckDevice.BackColor = Color.Black;
                        userControlCheckDeviceOnFormMain.textBoxExitOnGpRoomOnUcCheckDevice.ForeColor = Color.White;
                    }));
                }
                devRoomInCheckDevice.dIn01Saved = devInRoomExit.dInCurrent;
            }
            if ((devRoomInCheckDevice.firstFlag == true) ||
                (devInRoomStay.dInCurrent != devRoomInCheckDevice.dIn02Saved))
            {
                if (devInRoomStay.dInCurrent == true)
                {
                    Invoke((MethodInvoker)(() =>
                    {
                        userControlCheckDeviceOnFormMain.textBoxStayOnGpRoomOnCheckDevice.Text = "ON";
                        userControlCheckDeviceOnFormMain.textBoxStayOnGpRoomOnCheckDevice.BackColor = Color.Red;
                        userControlCheckDeviceOnFormMain.textBoxStayOnGpRoomOnCheckDevice.ForeColor = Color.White;
                    }));
                }
                else
                {
                    Invoke((MethodInvoker)(() =>
                    {
                        userControlCheckDeviceOnFormMain.textBoxStayOnGpRoomOnCheckDevice.Text = "OFF";
                        userControlCheckDeviceOnFormMain.textBoxStayOnGpRoomOnCheckDevice.BackColor = Color.Black;
                        userControlCheckDeviceOnFormMain.textBoxStayOnGpRoomOnCheckDevice.ForeColor = Color.White;
                    }));
                }
                devRoomInCheckDevice.dIn02Saved = devInRoomStay.dInCurrent;
            }

            // 最初の初期化は終了
            devRoomInCheckDevice.firstFlag = false;

        }
        private void CallbackInitForDevInRoomInOut()
        {
            bool funcRet;
            bool logicalStateOfExit;
            bool logicalStateOfEnt;

            devInRoomInOut.devStateVal = EDevState.RoomInOutInit;

            // フラグ初期化: ケージに入ったフラグ、ケージから出たフラグ
            FlagDevInRoomInOut_Inside = false;
            FlagDevInRoomInOut_Outside = false;
            //			devInRoomInOut.boolFirstFlag = true;

            // RoomExitセンサ(ケージから遠い側)
            funcRet = ioBoardDevice.GetUpperStateOfSaveDIn(IoBoardDInLogicalName.RoomExit, out logicalStateOfExit);
            if (funcRet != true)
            {
                Debug.WriteLine("ExeForDevInRoomIn:IoBoardDevice.GetUpperStateOfSaveDIn(RoomExit)エラー");
            }
            // RoomEntranceセンサ(ケージに近い側)
            funcRet = ioBoardDevice.GetUpperStateOfSaveDIn(IoBoardDInLogicalName.RoomEntrance, out logicalStateOfEnt);
            if (funcRet != true)
            {
                Debug.WriteLine("ExeForDevInRoomIn:IoBoardDevice.GetUpperStateOfSaveDIn(RoomEntrance)エラー");
            }
            devInRoomInOut.devValSaved[0] = logicalStateOfExit;
            devInRoomInOut.devValSaved[1] = logicalStateOfEnt;
        }

        /// <summary>
        /// ケージの中に入った事を表すフラグ
        /// ハンドシェイク用のフラグ
        /// </summary>
        private bool FlagDevInRoomInOut_Inside { get; set; } = false;
        /// <summary>
        /// ケージの外に出た事を表すフラグ
        /// ハンドシェイク用のフラグ
        /// </summary>
        private bool FlagDevInRoomInOut_Outside { get; set; } = false;

        private void CallbackExeForDevInRoomInOut()
        {
            bool funcRet;
            bool logicalStateOfExit;
            bool logicalStateOfEnt;
            bool logicalStateOfExitOld = devInRoomInOut.devValSaved[0];
            bool logicalStateOfEntOld = devInRoomInOut.devValSaved[1];

            //devRoomStopwatch.Restart();

            // RoomExitセンサ(ケージから遠い側)
            funcRet = ioBoardDevice.GetUpperStateOfSaveDIn(IoBoardDInLogicalName.RoomExit, out logicalStateOfExit);
            if (funcRet != true)
            {
                Debug.WriteLine("ExeForDevInRoomIn:IoBoardDevice.GetUpperStateOfSaveDIn(RoomExit)エラー");
            }
            // RoomEntranceセンサ(ケージに近い側)
            funcRet = ioBoardDevice.GetUpperStateOfSaveDIn(IoBoardDInLogicalName.RoomEntrance, out logicalStateOfEnt);
            if (funcRet != true)
            {
                Debug.WriteLine("ExeForDevInRoomIn:IoBoardDevice.GetUpperStateOfSaveDIn(RoomEntrance)エラー");
            }
            //			bool logicalStateOfExit;
            //			bool logicalStateOfEnt;

            //			bool logicalStateOfExitOld
            //			bool logicalStateOfEntOld
            // デバッグ出力: ステートが変化した時、出力
            if (devInRoomInOut.devStateVal != DevStateSave)
            {
                Debug.WriteLine(String.Format("RoomState:{0}", devInRoomInOut.devStateVal.ToString()));
                DevStateSave = devInRoomInOut.devStateVal;
            }

            switch (devInRoomInOut.devStateVal)
            {
                case EDevState.RoomInOutInit:
                    devInRoomInOut.devStateVal = EDevState.RoomInOutIdle;
                    break;
                case EDevState.RoomInOutIdle:
                    // Exitの立ち上がり時
                    if ((logicalStateOfExitOld == false) && (logicalStateOfExit == true))
                    {
                        Debug.WriteLine("  Exit+++");
                        devRoomStopwatch.Restart();
                        // ステート:入室Headへ移動
                        devInRoomInOut.devStateVal = EDevState.RoomInOutEnterHead;
                    }
                    // Entの立ち上がり時
                    else if ((logicalStateOfEntOld == false) && (logicalStateOfEnt == true))
                    {
                        Debug.WriteLine("  Enter+++");
                        // ステート:退室Bodyへ移動
                        devInRoomInOut.devStateVal = EDevState.RoomInOutLeaveHead;
                    }
                    //else if ((logicalStateOfEnt == false && logicalStateOfEntOld == true) && (logicalStateOfExit == false && logicalStateOfExitOld == true))
                    //{
                    //	Debug.WriteLine("exit");
                    //	OpFlagRoomOut = true;

                    //	// ステート:Idelへ移動
                    //	devInRoomInOut.devStateVal = EDevState.RoomInOutIdle;
                    //	Debug.WriteLine(String.Format("AckLeave:-----------------------------"));
                    //}
                    break;
                case EDevState.RoomInOutEnterHead:
                    // Exitの立ち下がり時
                    if ((logicalStateOfExitOld == true) && (logicalStateOfExit == false))
                    {
                        Debug.WriteLine("  Exit---");
                        // ステート:Idelへ移動
                        devInRoomInOut.devStateVal = EDevState.RoomInOutIdle;
                    }
                    // Entの立ち上がり時
                    else if ((logicalStateOfEntOld == false) && (logicalStateOfEnt == true))
                    {
                        Debug.WriteLine("  Enter+++");
                        // ステート:入室Bodyへ移動
                        devInRoomInOut.devStateVal = EDevState.RoomInOutEnterBody;
                    }
                    else if (logicalStateOfEnt == true && logicalStateOfExit == true)
                    {

                        Debug.WriteLine("Sensor on Time");
                    }
                    break;
                case EDevState.RoomInOutEnterBody:
                    // Exitの立ち下がり時
                    if ((logicalStateOfExitOld == true) && (logicalStateOfExit == false))
                    {
                        Debug.WriteLine("  Exit---");
                        // ステート:入室Tailへ移動
                        devInRoomInOut.devStateVal = EDevState.RoomInOutEnterTail;
                    }
                    // Entの立ち下がり時
                    else if ((logicalStateOfEntOld == true) && (logicalStateOfEnt == false))
                    {
                        Debug.WriteLine("  Enter---");
                        // ステート:入室Headへ移動
                        devInRoomInOut.devStateVal = EDevState.RoomInOutEnterHead;
                    }
                    else if (logicalStateOfEnt == true && logicalStateOfExit == true)
                    {
                        //尻尾がセンサー両方反応してたら時間で入室判定
                        if (devRoomStopwatch.Elapsed.TotalSeconds >= 10)
                        {
                            devRoomStopwatch.Stop();
                            devRoomStopwatch.Reset();
                            Debug.WriteLine("Sensor on Time");
                            OpFlagRoomIn = true;
                            devInRoomInOut.devStateVal = EDevState.RoomInOutIdle;
                        }
                    }
                    break;
                case EDevState.RoomInOutEnterTail:
                    // Exitの立ち上がり時
                    if ((logicalStateOfExitOld == false) && (logicalStateOfExit == true))
                    {
                        Debug.WriteLine("  Exit+++");
                        // ステート:入室Bodyへ移動
                        devInRoomInOut.devStateVal = EDevState.RoomInOutEnterBody;
                    }
                    // Entの立ち下がり時
                    else if ((logicalStateOfEntOld == true) && (logicalStateOfEnt == false))
                    {
                        Debug.WriteLine("  Enter---");
                        // ケージに入ったフラグを立てる
                        //						FlagDevInRoomInOut_Inside = true;
                        // Mainステート・マシン用にフラグON
                        devRoomStopwatch.Stop();
                        Debug.WriteLine(devRoomStopwatch.Elapsed.TotalSeconds.ToString() + "秒経過");
                        OpFlagRoomIn = true;
                        // ステート:Idelへ移動
                        devInRoomInOut.devStateVal = EDevState.RoomInOutIdle;
                        Debug.WriteLine(String.Format("AckEnter:-----------------------------"));
                    }
                    break;
                case EDevState.RoomInOutLeaveHead:
                    // Exitの立ち上がり時
                    if ((logicalStateOfExitOld == false) && (logicalStateOfExit == true))
                    {
                        Debug.WriteLine("  Exit+++");
                        // ステート:退室Bodyへ移動
                        devInRoomInOut.devStateVal = EDevState.RoomInOutLeaveBody;
                    }
                    // Entの立ち下がり時
                    else if ((logicalStateOfEntOld == true) && (logicalStateOfEnt == false))
                    {
                        Debug.WriteLine("  Enter---");
                        // ステート:Idelへ移動
                        devInRoomInOut.devStateVal = EDevState.RoomInOutIdle;
                    }
                    break;
                case EDevState.RoomInOutLeaveBody:
                    // Exitの立ち下がり時
                    if ((logicalStateOfExitOld == true) && (logicalStateOfExit == false))
                    {
                        Debug.WriteLine("  Exit---");
                        // ステート:退室Headへ移動
                        devInRoomInOut.devStateVal = EDevState.RoomInOutLeaveHead;
                    }
                    // Entの立ち下がり時
                    else if ((logicalStateOfEntOld == true) && (logicalStateOfEnt == false))
                    {
                        Debug.WriteLine("  Enter---");
                        // ステート:Idelへ移動
                        devInRoomInOut.devStateVal = EDevState.RoomInOutLeaveTail;
                    }

                    break;
                case EDevState.RoomInOutLeaveTail:
                    // Exitの立ち下がり時
                    if ((logicalStateOfExitOld == true) && (logicalStateOfExit == false))
                    {
                        Debug.WriteLine("  Exit---");
                        // ケージを出たフラグを立てる
                        //						FlagDevInRoomInOut_Outside = true;
                        // Mainステート・マシン用にフラグON
                        OpFlagRoomOut = true;

                        // ステート:Idelへ移動
                        devInRoomInOut.devStateVal = EDevState.RoomInOutIdle;
                        Debug.WriteLine(String.Format("AckLeave:-----------------------------"));
                    }
                    break;
                default:
                    break;
            }
            // 現在値をセーブ
            devInRoomInOut.devValSaved[0] = logicalStateOfExit;
            devInRoomInOut.devValSaved[1] = logicalStateOfEnt;
        }
        private void CallbackInitForDevInRoomSns()
        {
            //			bool funcRet;
            //			bool logicalStateOfStay;

            devInRoomSns.devStateVal = EDevState.RoomSnsInit;
            //			devInRoomOut.boolFirstFlag = true;
            // RoomStayセンサ(室内センサ)
            //			funcRet = IoBoardDevice.GetUpperStateOfSaveDIn(IoBoardDInLogicalName.RoomStay, out logicalStateOfStay);
            //			if (funcRet != true)
            //			{
            //				Debug.WriteLine("ExeForDevInRoomStay:IoBoardDevice.GetUpperStateOfSaveDIn(RoomStay)エラー");
            //			}
            //			devInRoomSns.devValSaved[0] = logicalStateOfStay;
        }
        private void CallbackExeForDevInRoomSns()
        {
            bool funcRet;
            bool logicalStateOfStay;        // 在室

            // RoomStayセンサ(室内センサ
            funcRet = ioBoardDevice.GetUpperStateOfSaveDIn(IoBoardDInLogicalName.RoomStay, out logicalStateOfStay);
            if (funcRet != true)
            {
                Debug.WriteLine("ExeForDevInRoomStay:IoBoardDevice.GetUpperStateOfSaveDIn(RoomStay)エラー");
            }
            switch (devInRoomSns.devStateVal)
            {
                case EDevState.RoomSnsInit:
                case EDevState.RoomSnsIdle:
                    // ケージに入ったフラグONの時
                    if (FlagDevInRoomInOut_Inside == true)
                    {
                        // 入室確認待ちへ移動
                        devInRoomSns.devStateVal = EDevState.RoomSnsWaitIn;
                    }
                    // ケージを出たフラグONの時
                    else if (FlagDevInRoomInOut_Outside == true)
                    {
                        // 退室確認待ちへ移動
                        devInRoomSns.devStateVal = EDevState.RoomSnsWaitOut;
                    }
                    break;
                case EDevState.RoomSnsWaitIn:
                    // 在室センサONの時
                    if (logicalStateOfStay == true)
                    {
                        // ケージに入ったフラグ: クリア
                        FlagDevInRoomInOut_Inside = false;
                        // Mainステート・マシン用にフラグON
                        OpFlagRoomIn = true;
                    }
                    break;
                case EDevState.RoomSnsWaitOut:
                    // 在室センサOFFの時
                    if (logicalStateOfStay == false)
                    {
                        // ケージを出たフラグ: クリア
                        FlagDevInRoomInOut_Outside = false;
                        // Mainステート・マシン用にフラグON
                        OpFlagRoomOut = true;
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
