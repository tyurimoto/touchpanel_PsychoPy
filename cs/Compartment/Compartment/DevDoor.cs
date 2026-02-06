using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace Compartment
{
    public class DevDoor
    {
    }

    public partial class FormMain : Form
    {
        /// <summary>
        /// ドアの状態表示用辞書 (CheckDevice UI)
        /// </summary>
        public Dictionary<int, String> dictionaryDoorState = new Dictionary<int, string>
        {
            { (int)EDevState.DoorInit,                                  "Init"},
            { (int)EDevState.DoorStop,                                  "Stop"},
            { (int)EDevState.DoorStopInitAfterWait,                     "StopInit1"},
            { (int)EDevState.DoorStopInitAfterWaitPulseOff,             "StopInit2"},
            { (int)EDevState.DoorStopInit,                              "StopInit"},
            { (int)EDevState.DoorStopWithOpenPulseOn1,                  "StopCmd1-1"},
            { (int)EDevState.DoorStopWithOpenPulseOff1,                 "StopCmd1-2"},
            { (int)EDevState.DoorStopWithOpenPulseOn2,                  "StopCmd1-3"},
            { (int)EDevState.DoorStopWithClosePulseOff2,                "StopCmd2-1"},
            { (int)EDevState.DoorStopWithClosePulseOn,                  "StopCmd2-2"},
            { (int)EDevState.DoorOpenInitAfterWait,                     "OpenInit1"},
            { (int)EDevState.DoorOpenInitAfterWaitPulseOff,             "OpenInit2"},
            { (int)EDevState.DoorOpenInit,                              "OpenInit"},
            { (int)EDevState.DoorOpenPulseOn1,                          "OpenCmd1-1"},
            { (int)EDevState.DoorOpenPulseOff1,                         "OpenCmd1-2"},
            { (int)EDevState.DoorOpenPulseOn2,                          "OpenCmd1-3"},
            { (int)EDevState.DoorOpenDoing,                             "Open"},
            { (int)EDevState.DoorOpenRecoverInit,                       "OpenRvInit"},
            { (int)EDevState.DoorOpenRecoverInitRetry,                  "OpenRvInit1"},
            { (int)EDevState.DoorOpenRecoverWithClosePulseOn1,          "OpenRvClose1"},
            { (int)EDevState.DoorOpenRecoverWithClosePulseOff1,         "OpenRvClose2"},
            { (int)EDevState.DoorOpenRecoverWithClosePulseOn2,          "OpenRvClose3"},
            { (int)EDevState.DoorOpenRecoverWithClosing,                "OpenRvClose"},
            { (int)EDevState.DoorOpenRecoverWithOpenPulseOn1,           "OpenRvOpen1"},
            { (int)EDevState.DoorOpenRecoverWithOpenPulseOff1,          "OpenRvOpen2"},
            { (int)EDevState.DoorOpenRecoverWithOpenPulseOn2,           "OpenRvOpen3"},
            { (int)EDevState.DoorOpenRecoverWithOpenning,               "OpenRvOpen"},
            { (int)EDevState.DoorOpenRecoverTimeoutInit,                "OpenRvTmInit"},
            { (int)EDevState.DoorOpenRecoverTimeoutWithClosePulseOn1,   "OpenRvTmClose1"},
            { (int)EDevState.DoorOpenRecoverTimeoutWithClosePulseOff1,  "OpenRvTmClose2"},
            { (int)EDevState.DoorOpenRecoverTimeoutWithClosePulseOn2,   "OpenRvTmClose3"},
            { (int)EDevState.DoorOpenRecoverTimeoutWithClosing,         "OpenRvTmClose"},

            { (int)EDevState.DoorCloseInitAfterWait,                    "CloseInit1"},
            { (int)EDevState.DoorCloseInitAfterWaitPulseOff,            "CloseInit2"},
            { (int)EDevState.DoorCloseInit,                             "CloseInit"},
            { (int)EDevState.DoorClosePulseOn1,                         "CloseCmd1-1"},
            { (int)EDevState.DoorClosePulseOff1,                        "CloseCmd1-2"},
            { (int)EDevState.DoorClosePulseOn2,                         "CloseCmd1-3"},
            { (int)EDevState.DoorCloseDoing,                            "Close"},
            { (int)EDevState.DoorCloseRecoverInit,                      "CloseRvInit"},
            { (int)EDevState.DoorCloseRecoverInitRetry,                 "CloseRvInit1"},
            { (int)EDevState.DoorCloseRecoverWithOpenPulseOn1,          "CloseRvOpen1"},
            { (int)EDevState.DoorCloseRecoverWithOpenPulseOff1,         "CloseRvOpen2"},
            { (int)EDevState.DoorCloseRecoverWithOpenPulseOn2,          "CloseRvOpen3"},
            { (int)EDevState.DoorCloseRecoverWithOpenning,              "CloseRvOpen"},
            { (int)EDevState.DoorCloseRecoverWithClosePulseOn1,         "CloseRvClose1"},
            { (int)EDevState.DoorCloseRecoverWithClosePulseOff1,        "CloseRvClose2"},
            { (int)EDevState.DoorCloseRecoverWithClosePulseOn2,         "CloseRvClose3"},
            { (int)EDevState.DoorCloseRecoverWithClosing,               "CloseRvClose"},
            { (int)EDevState.DoorCloseRecoverTimeoutInit,               "CloseRvTmInit"},
            { (int)EDevState.DoorCloseRecoverTimeoutWithOpenPulseOn1,   "CloseRvTmOpen1"},
            { (int)EDevState.DoorCloseRecoverTimeoutWithOpenPulseOff1,  "CloseRvTmOpen2"},
            { (int)EDevState.DoorCloseRecoverTimeoutWithOpenPulseOn2,   "CloseRvTmOpen3"},
            { (int)EDevState.DoorCloseRecoverTimeoutWithOpenning,       "CloseRvTmOpen"},
        };

        private void CallbackInitForDevDoor()
        {
            ActuatorInit(BuildDoorConfig());
        }

        private void CallbackExeForDevDoor()
        {
            ActuatorExe(BuildDoorConfig());
        }

        private ActuatorConfig BuildDoorConfig()
        {
            return new ActuatorConfig
            {
                dev = devDoor,
                cmdQueue = concurrentQueueDevCmdPktDoor,
                baseState = EDevState.DoorInit,
                cmdForward = EDevCmd.DoorOpen,
                cmdReverse = EDevCmd.DoorClose,
                cmdStop = EDevCmd.DoorStop,
                outForward = IoBoardDOutLogicalName.DoorOpen,
                outReverse = IoBoardDOutLogicalName.DoorClose,
                outStop = IoBoardDOutLogicalName.DoorStop,
                inForward = IoBoardDInLogicalName.DoorOpen,
                inReverse = IoBoardDInLogicalName.DoorClose,
                activePulseWidth = preferencesDatOriginal.ActivePulseWidthOfDoor,
                inactivePulseWidth = preferencesDatOriginal.InactivePulseWidthOfDoor,
                timeoutForward = preferencesDatOriginal.TimeoutOfDoorOpen,
                timeoutReverse = preferencesDatOriginal.TimeoutOfDoorClose,
                reverseTimeInRecoverForward = preferencesDatOriginal.DoorCloseTimeInRecoverOpen,
                forwardTimeInRecoverReverse = preferencesDatOriginal.DoorOpenTimeInRecoverClose,
                retryNum = preferencesDatOriginal.DoorRetryNumInRecover,
                recoverEnabled = Properties.Settings.Default.IS_DOOR_RECOVER_ENABLE,
                setFlagForward = (v) => { OpFlagOpenDoor = v; },
                setFlagReverse = (v) => { OpFlagCloseDoor = v; },
                setFlagStop = (v) => { OpFlagStopDoor = v; },
                setResultForward = (r) => { OpResultOpenDoor = r; },
                setResultReverse = (r) => { OpResultCloseDoor = r; },
                setResultStop = (r) => { OpResultStopDoor = r; },
                clearAllFlagsAndResults = () =>
                {
                    OpFlagStopDoor = false;
                    OpResultStopDoor = EDeviceResult.None;
                    OpFlagOpenDoor = false;
                    OpResultOpenDoor = EDeviceResult.None;
                    OpFlagCloseDoor = false;
                    OpResultCloseDoor = EDeviceResult.None;
                },
                deviceName = "Door",
            };
        }

        private void CallbackInitForDevDoorInCheckDevice()
        {
            devDoorInCheckDevice.firstFlag = true;
        }
        private void CallbackExeForDevDoorInCheckDevice()
        {
            // 変化した時、表示更新
            if ((devDoorInCheckDevice.firstFlag == true) ||
                (devInDoorOpen.dInCurrent != devDoorInCheckDevice.dIn00Saved))
            {
                if (devInDoorOpen.dInCurrent == true)
                {
                    Invoke((MethodInvoker)(() =>
                    {
                        userControlCheckDeviceOnFormMain.textBoxDInOpenOnGpDoorOnUcCheckDevice.Text = "ON";
                        userControlCheckDeviceOnFormMain.textBoxDInOpenOnGpDoorOnUcCheckDevice.BackColor = Color.Red;
                        userControlCheckDeviceOnFormMain.textBoxDInOpenOnGpDoorOnUcCheckDevice.ForeColor = Color.White;
                    }));
                }
                else
                {
                    Invoke((MethodInvoker)(() =>
                    {
                        userControlCheckDeviceOnFormMain.textBoxDInOpenOnGpDoorOnUcCheckDevice.Text = "OFF";
                        userControlCheckDeviceOnFormMain.textBoxDInOpenOnGpDoorOnUcCheckDevice.BackColor = Color.Black;
                        userControlCheckDeviceOnFormMain.textBoxDInOpenOnGpDoorOnUcCheckDevice.ForeColor = Color.White;
                    }));
                }
                devDoorInCheckDevice.dIn00Saved = devInDoorOpen.dInCurrent;
            }
            if ((devDoorInCheckDevice.firstFlag == true) ||
                (devInDoorClose.dInCurrent != devDoorInCheckDevice.dIn01Saved))
            {
                if (devInDoorClose.dInCurrent == true)
                {
                    Invoke((MethodInvoker)(() =>
                    {
                        userControlCheckDeviceOnFormMain.textBoxDInCloseOnGpDoorOnUcCheckDevice.Text = "ON";
                        userControlCheckDeviceOnFormMain.textBoxDInCloseOnGpDoorOnUcCheckDevice.BackColor = Color.Red;
                        userControlCheckDeviceOnFormMain.textBoxDInCloseOnGpDoorOnUcCheckDevice.ForeColor = Color.White;
                    }));
                }
                else
                {
                    Invoke((MethodInvoker)(() =>
                    {
                        userControlCheckDeviceOnFormMain.textBoxDInCloseOnGpDoorOnUcCheckDevice.Text = "OFF";
                        userControlCheckDeviceOnFormMain.textBoxDInCloseOnGpDoorOnUcCheckDevice.BackColor = Color.Black;
                        userControlCheckDeviceOnFormMain.textBoxDInCloseOnGpDoorOnUcCheckDevice.ForeColor = Color.White;
                    }));
                }
                devDoorInCheckDevice.dIn01Saved = devInDoorClose.dInCurrent;
            }
            // 変化した時、表示更新
            if (devDoor.devStateVal != devDoor.devStateValSaved)
            {
                String stringState;
                if (dictionaryDoorState.TryGetValue((int)devDoor.devStateVal, out stringState) != true)
                {
                    stringState = "none";
                }
                Invoke((MethodInvoker)(() =>
                {
                    userControlCheckDeviceOnFormMain.textBoxDoutDoorStateOnGpDoorOnUcCheckDevice.Text = stringState;
                }));
                devDoor.devStateValSaved = devDoor.devStateVal;
            }
            String stringDoorResult;

            if (devDoorInCheckDevice.firstFlag == true)
            {
                stringDoorResult = "none";
            }
            else
            {
                stringDoorResult = "";
            }
            if (OpFlagStopDoor == true)
            {
                OpFlagStopDoor = false;
                stringDoorResult = OpResultStopDoor.ToString();
            }
            else if (OpFlagOpenDoor == true)
            {
                OpFlagOpenDoor = false;
                stringDoorResult = OpResultOpenDoor.ToString();
            }
            else if (OpFlagCloseDoor == true)
            {
                OpFlagCloseDoor = false;
                stringDoorResult = OpResultCloseDoor.ToString();
            }
            if (stringDoorResult != "")
            {
                Invoke((MethodInvoker)(() =>
                {
                    userControlCheckDeviceOnFormMain.textBoxDoutDoorResultOnGpDoorOnUcCheckDevice.Text = stringDoorResult;
                }));
            }
            // 最初の初期化は終了
            devDoorInCheckDevice.firstFlag = false;

        }
        private void CallbackInitForDevInDoorOpen()
        {
            devInDoorOpen.firstFlag = true;
        }
        private void CallbackExeForDevInDoorOpen()
        {
            bool funcRet;
            bool logicalStateOfOpen;

            // DoorOpenセンサ
            funcRet = ioBoardDevice.GetUpperStateOfSaveDIn(IoBoardDInLogicalName.DoorOpen, out logicalStateOfOpen);
            if (funcRet != true)
            {
                Debug.WriteLine("ExeForDevInDoorOpen:IoBoardDevice.GetUpperStateOfSaveDIn(DoorOpen)エラー");
            }
            devInDoorOpen.dInCurrent = logicalStateOfOpen;
        }
        private void CallbackInitForDevInDoorClose()
        {
            devInDoorOpen.firstFlag = true;
        }
        private void CallbackExeForDevInDoorClose()
        {
            bool funcRet;
            bool logicalStateOfClose;

            // DoorCloseセンサ
            funcRet = ioBoardDevice.GetUpperStateOfSaveDIn(IoBoardDInLogicalName.DoorClose, out logicalStateOfClose);
            if (funcRet != true)
            {
                Debug.WriteLine("ExeForDevInDoorClose:IoBoardDevice.GetUpperStateOfSaveDIn(DoorClose)エラー");
            }
            devInDoorClose.dInCurrent = logicalStateOfClose;
        }
    }
}
