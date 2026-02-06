using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace Compartment
{
    class DevLever
    {
    }

    public partial class FormMain : Form
    {
        public Dictionary<int, String> mDictionaryLeverState = new Dictionary<int, string>
        {
            { (int)EDevState.LeverInit,                                 "Init"},
            { (int)EDevState.LeverStop,                                 "Stop"},
            { (int)EDevState.LeverStopInitAfterWait,                    "StopInit1"},
            { (int)EDevState.LeverStopInitAfterWaitPulseOff,            "StopInit2"},
            { (int)EDevState.LeverStopInit,                             "StopInit"},
            { (int)EDevState.LeverStopWithInPulseOn1,                   "StopCmd1-1"},
            { (int)EDevState.LeverStopWithInPulseOff1,                  "StopCmd1-2"},
            { (int)EDevState.LeverStopWithInPulseOn2,                   "StopCmd1-3"},
            { (int)EDevState.LeverStopWithOutPulseOff2,                 "StopCmd2-1"},
            { (int)EDevState.LeverStopWithOutPulseOn,                   "StopCmd2-2"},
            { (int)EDevState.LeverInInitAfterWait,                      "InInit1"},
            { (int)EDevState.LeverInInitAfterWaitPulseOff,              "InInit2"},
            { (int)EDevState.LeverInInit,                               "InInit"},
            { (int)EDevState.LeverInPulseOn1,                           "InCmd1-1"},
            { (int)EDevState.LeverInPulseOff1,                          "InCmd1-2"},
            { (int)EDevState.LeverInPulseOn2,                           "InCmd1-3"},
            { (int)EDevState.LeverInDoing,                              "In"},
            { (int)EDevState.LeverInRecoverInit,                        "InRvInit"},
            { (int)EDevState.LeverInRecoverInitRetry,                   "InRvInit1"},
            { (int)EDevState.LeverInRecoverWithOutPulseOn1,             "InRvOut1"},
            { (int)EDevState.LeverInRecoverWithOutPulseOff1,            "InRvOut2"},
            { (int)EDevState.LeverInRecoverWithOutPulseOn2,             "InRvOut3"},
            { (int)EDevState.LeverInRecoverWithOutDoing,                "InRvOut"},
            { (int)EDevState.LeverInRecoverWithInPulseOn1,              "InRvIn1"},
            { (int)EDevState.LeverInRecoverWithInPulseOff1,             "InRvIn2"},
            { (int)EDevState.LeverInRecoverWithInPulseOn2,              "InRvIn3"},
            { (int)EDevState.LeverInRecoverWithInDoing,                 "InRvInDoing"},
            { (int)EDevState.LeverInRecoverTimeoutInit,                 "InRvTmInit"},
            { (int)EDevState.LeverInRecoverTimeoutWithOutPulseOn1,      "InRvTmOut1"},
            { (int)EDevState.LeverInRecoverTimeoutWithOutPulseOff1,     "InRvTmOut2"},
            { (int)EDevState.LeverInRecoverTimeoutWithOutPulseOn2,      "InRvTmOut3"},
            { (int)EDevState.LeverInRecoverTimeoutWithOutDoing,         "InRvTmOut"},
            { (int)EDevState.LeverOutInitAfterWait,                     "OutInit1"},
            { (int)EDevState.LeverOutInitAfterWaitPulseOff,             "OutInit2"},
            { (int)EDevState.LeverOutInit,                              "OutInit"},
            { (int)EDevState.LeverOutPulseOn1,                          "OutCmd1-1"},
            { (int)EDevState.LeverOutPulseOff1,                         "OutCmd1-2"},
            { (int)EDevState.LeverOutPulseOn2,                          "OutCmd1-3"},
            { (int)EDevState.LeverOutDoing,                             "Out"},
            { (int)EDevState.LeverOutRecoverInit,                       "OutRvInit"},
            { (int)EDevState.LeverOutRecoverInitRetry,                  "OutRvInit1"},
            { (int)EDevState.LeverOutRecoverWithInPulseOn1,             "OutRvIn1"},
            { (int)EDevState.LeverOutRecoverWithInPulseOff1,            "OutRvIn2"},
            { (int)EDevState.LeverOutRecoverWithInPulseOn2,             "OutRvIn3"},
            { (int)EDevState.LeverOutRecoverWithInDoing,                "OutRvIn"},
            { (int)EDevState.LeverOutRecoverWithOutPulseOn1,            "OutRvOut1"},
            { (int)EDevState.LeverOutRecoverWithOutPulseOff1,           "OutRvOut2"},
            { (int)EDevState.LeverOutRecoverWithOutPulseOn2,            "OutRvOut3"},
            { (int)EDevState.LeverOutRecoverWithOutDoing,               "OutRvOut"},
            { (int)EDevState.LeverOutRecoverTimeoutInit,                "OutRvTmInit"},
            { (int)EDevState.LeverOutRecoverTimeoutWithInPulseOn1,      "OutRvTmIn1"},
            { (int)EDevState.LeverOutRecoverTimeoutWithInPulseOff1,     "OutRvTmIn2"},
            { (int)EDevState.LeverOutRecoverTimeoutWithInPulseOn2,      "OutRvTmIn3"},
            { (int)EDevState.LeverOutRecoverTimeoutWithInDoing,         "OutRvTmIn"},
        };

        private void CallbackInitForDevLever()
        {
            ActuatorInit(BuildLeverConfig());
        }

        private void CallbackExeForDevLever()
        {
            ActuatorExe(BuildLeverConfig());
        }

        private ActuatorConfig BuildLeverConfig()
        {
            return new ActuatorConfig
            {
                dev = devLever,
                cmdQueue = concurrentQueueDevCmdPktLever,
                baseState = EDevState.LeverInit,
                cmdForward = EDevCmd.LeverIn,
                cmdReverse = EDevCmd.LeverOut,
                cmdStop = EDevCmd.LeverStop,
                outForward = IoBoardDOutLogicalName.LeverIn,
                outReverse = IoBoardDOutLogicalName.LeverOut,
                outStop = IoBoardDOutLogicalName.LeverStop,
                inForward = IoBoardDInLogicalName.LeverIn,
                inReverse = IoBoardDInLogicalName.LeverOut,
                activePulseWidth = preferencesDatOriginal.ActivePulseWidthOfLever,
                inactivePulseWidth = preferencesDatOriginal.InactivePulseWidthOfLever,
                timeoutForward = preferencesDatOriginal.TimeoutOfLeverIn,
                timeoutReverse = preferencesDatOriginal.TimeoutOfLeverOut,
                reverseTimeInRecoverForward = preferencesDatOriginal.LeverOutTimeInRecoverIn,
                forwardTimeInRecoverReverse = preferencesDatOriginal.LeverInTimeInRecoverOut,
                retryNum = preferencesDatOriginal.LeverRetryNumInRecover,
                recoverEnabled = Properties.Settings.Default.IS_LEVER_RECOVER_ENABLE,
                setFlagForward = (v) => { OpFlagMoveLeverIn = v; },
                setFlagReverse = (v) => { OpFlagMoveLeverOut = v; },
                setFlagStop = (v) => { OpFlagStopLever = v; },
                setResultForward = (r) => { OpResultMoveLeverIn = r; },
                setResultReverse = (r) => { OpResultMoveLeverOut = r; },
                setResultStop = (r) => { OpResultStopLever = r; },
                clearAllFlagsAndResults = () =>
                {
                    OpFlagStopLever = false;
                    OpResultStopLever = EDeviceResult.None;
                    OpFlagMoveLeverIn = false;
                    OpResultMoveLeverIn = EDeviceResult.None;
                    OpFlagMoveLeverOut = false;
                    OpResultMoveLeverOut = EDeviceResult.None;
                },
                deviceName = "Lever",
            };
        }

        private void CallbackInitForDevLeverInCheckDevice()
        {
            devLeverInCheckDevice.firstFlag = true;
        }
        private void CallbackExeForDevLeverInCheckDevice()
        {
            // 変化した時、表示更新
            if ((devLeverInCheckDevice.firstFlag == true) ||
                (devInLeverOut.dInCurrent != devLeverInCheckDevice.dIn00Saved))
            {
                if (devInLeverOut.dInCurrent == true)
                {
                    Invoke((MethodInvoker)(() =>
                    {
                        userControlCheckDeviceOnFormMain.textBoxDInLeverOutOnGpLeverOnUcCheckDevice.Text = "ON";
                        userControlCheckDeviceOnFormMain.textBoxDInLeverOutOnGpLeverOnUcCheckDevice.BackColor = Color.Red;
                        userControlCheckDeviceOnFormMain.textBoxDInLeverOutOnGpLeverOnUcCheckDevice.ForeColor = Color.White;
                    }));
                }
                else
                {
                    Invoke((MethodInvoker)(() =>
                    {
                        userControlCheckDeviceOnFormMain.textBoxDInLeverOutOnGpLeverOnUcCheckDevice.Text = "OFF";
                        userControlCheckDeviceOnFormMain.textBoxDInLeverOutOnGpLeverOnUcCheckDevice.BackColor = Color.Black;
                        userControlCheckDeviceOnFormMain.textBoxDInLeverOutOnGpLeverOnUcCheckDevice.ForeColor = Color.White;
                    }));
                }
                devLeverInCheckDevice.dIn00Saved = devInLeverOut.dInCurrent;
            }
            if ((devLeverInCheckDevice.firstFlag == true) ||
                (devInLeverIn.dInCurrent != devLeverInCheckDevice.dIn01Saved))
            {
                if (devInLeverIn.dInCurrent == true)
                {
                    Invoke((MethodInvoker)(() =>
                    {
                        userControlCheckDeviceOnFormMain.textBoxDInLeverInOnGpLeverOnUcCheckDevice.Text = "ON";
                        userControlCheckDeviceOnFormMain.textBoxDInLeverInOnGpLeverOnUcCheckDevice.BackColor = Color.Red;
                        userControlCheckDeviceOnFormMain.textBoxDInLeverInOnGpLeverOnUcCheckDevice.ForeColor = Color.White;
                    }));
                }
                else
                {
                    Invoke((MethodInvoker)(() =>
                    {
                        userControlCheckDeviceOnFormMain.textBoxDInLeverInOnGpLeverOnUcCheckDevice.Text = "OFF";
                        userControlCheckDeviceOnFormMain.textBoxDInLeverInOnGpLeverOnUcCheckDevice.BackColor = Color.Black;
                        userControlCheckDeviceOnFormMain.textBoxDInLeverInOnGpLeverOnUcCheckDevice.ForeColor = Color.White;
                    }));
                }
                devLeverInCheckDevice.dIn01Saved = devInLeverIn.dInCurrent;
            }
            if ((devLeverInCheckDevice.firstFlag == true) ||
                (devInLeverSw.dInCurrent != devLeverInCheckDevice.dIn02Saved))
            {
                if (devInLeverSw.dInCurrent == true)
                {
                    Invoke((MethodInvoker)(() =>
                    {
                        userControlCheckDeviceOnFormMain.textBoxDInLeverSwOnGpLeverOnUcCheckDevice.Text = "ON";
                        userControlCheckDeviceOnFormMain.textBoxDInLeverSwOnGpLeverOnUcCheckDevice.BackColor = Color.Red;
                        userControlCheckDeviceOnFormMain.textBoxDInLeverSwOnGpLeverOnUcCheckDevice.ForeColor = Color.White;
                    }));
                }
                else
                {
                    Invoke((MethodInvoker)(() =>
                    {
                        userControlCheckDeviceOnFormMain.textBoxDInLeverSwOnGpLeverOnUcCheckDevice.Text = "OFF";
                        userControlCheckDeviceOnFormMain.textBoxDInLeverSwOnGpLeverOnUcCheckDevice.BackColor = Color.Black;
                        userControlCheckDeviceOnFormMain.textBoxDInLeverSwOnGpLeverOnUcCheckDevice.ForeColor = Color.White;
                    }));
                }
                devLeverInCheckDevice.dIn02Saved = devInLeverSw.dInCurrent;
            }

            // 変化した時、表示更新
            if (devLever.devStateVal != devLever.devStateValSaved)
            {
                String stringState;
                if (mDictionaryLeverState.TryGetValue((int)devLever.devStateVal, out stringState) != true)
                {
                    stringState = "none";
                }
                Invoke((MethodInvoker)(() =>
                {
                    userControlCheckDeviceOnFormMain.textBoxDoutLeverStateOnGpLeverOnUcCheckDevice.Text = stringState;
                }));

                devLever.devStateValSaved = devLever.devStateVal;
            }
            String stringLeverResult;

            if (devLeverInCheckDevice.firstFlag == true)
            {
                stringLeverResult = "none";
            }
            else
            {
                stringLeverResult = "";
            }
            if (OpFlagStopLever == true)
            {
                OpFlagStopLever = false;
                stringLeverResult = OpResultStopLever.ToString();
            }
            else if (OpFlagMoveLeverIn == true)
            {
                OpFlagMoveLeverOut = false;
                stringLeverResult = OpResultMoveLeverIn.ToString();
            }
            else if (OpFlagMoveLeverOut == true)
            {
                OpFlagMoveLeverOut = false;
                stringLeverResult = OpResultMoveLeverOut.ToString();
            }
            if (stringLeverResult != "")
            {
                Invoke((MethodInvoker)(() =>
                {
                    userControlCheckDeviceOnFormMain.textBoxDoutLeverResultOnGpLeverOnUcCheckDevice.Text = stringLeverResult;
                }));
            }
            // 最初の初期化は終了
            devLeverInCheckDevice.firstFlag = false;

        }
        private void CallbackInitForDevInLeverIn()
        {
            bool funcRet;
            bool logicalStateOfIn;

            // LeverInセンサ
            funcRet = ioBoardDevice.GetUpperStateOfSaveDIn(IoBoardDInLogicalName.LeverIn, out logicalStateOfIn);
            if (funcRet != true)
            {
                Debug.WriteLine("ExeForDevInLeverIn:IoBoardDevice.GetUpperStateOfSaveDIn(LeverIn)エラー");
            }
            devInLeverIn.dInCurrent = logicalStateOfIn;

            devInLeverIn.firstFlag = true;
        }
        private void CallbackExeForDevInLeverIn()
        {
            bool funcRet;
            bool logicalStateOfIn;

            // LeverInセンサ
            funcRet = ioBoardDevice.GetUpperStateOfSaveDIn(IoBoardDInLogicalName.LeverIn, out logicalStateOfIn);
            if (funcRet != true)
            {
                Debug.WriteLine("ExeForDevInLeverIn:IoBoardDevice.GetUpperStateOfSaveDIn(LeverIn)エラー");
            }
            devInLeverIn.dInCurrent = logicalStateOfIn;
        }
        private void CallbackInitForDevInLeverOut()
        {
            bool funcRet;
            bool logicalStateOfOut;

            // LeverOutセンサ
            funcRet = ioBoardDevice.GetUpperStateOfSaveDIn(IoBoardDInLogicalName.LeverOut, out logicalStateOfOut);
            if (funcRet != true)
            {
                Debug.WriteLine("ExeForDevInLeverOut:IoBoardDevice.GetUpperStateOfSaveDIn(LeverOut)エラー");
            }
            devInLeverOut.dInCurrent = logicalStateOfOut;

            devInLeverOut.firstFlag = true;
        }
        private void CallbackExeForDevInLeverOut()
        {
            bool funcRet;
            bool logicalStateOfOut;

            // LeverOutセンサ
            funcRet = ioBoardDevice.GetUpperStateOfSaveDIn(IoBoardDInLogicalName.LeverOut, out logicalStateOfOut);
            if (funcRet != true)
            {
                Debug.WriteLine("ExeForDevInLeverOut:IoBoardDevice.GetUpperStateOfSaveDIn(LeverOut)エラー");
            }
            devInLeverOut.dInCurrent = logicalStateOfOut;
        }
        private void CallbackInitForDevInLeverSw()
        {
            bool funcRet;
            bool logicalStateOfSw;

            // LeverSwセンサ
            funcRet = ioBoardDevice.GetUpperStateOfSaveDIn(IoBoardDInLogicalName.LeverSw, out logicalStateOfSw);
            if (funcRet != true)
            {
                Debug.WriteLine("ExeForDevInLeverSw:IoBoardDevice.GetUpperStateOfSaveDIn(LeverSw)エラー");
            }
            devInLeverSw.dInCurrent = logicalStateOfSw;

            devInLeverSw.firstFlag = true;
        }
        private void CallbackExeForDevInLeverSw()
        {
            bool funcRet;
            bool logicalStateOfSw;

            // LeverSwセンサ
            funcRet = ioBoardDevice.GetUpperStateOfSaveDIn(IoBoardDInLogicalName.LeverSw, out logicalStateOfSw);
            if (funcRet != true)
            {
                Debug.WriteLine("ExeForDevInSw:IoBoardDevice.GetUpperStateOfSaveDIn(LeverSw)エラー");
            }
            devInLeverSw.dInCurrent = logicalStateOfSw;
            // SW.ONの時、Mainステートマシン用インターフェース・フラグをセット
            if (logicalStateOfSw == true)
            {
                OpFlagLeverSw = true;
            }
        }
    }
}
