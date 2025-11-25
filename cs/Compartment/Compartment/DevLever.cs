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
            { (int)EDevState.LeverStopInitAfterWait,                        "StopInit1"},
            { (int)EDevState.LeverStopInitAfterWaitPulseOff,                "StopInit2"},
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
            { (int)EDevState.LeverInRecoverInit,                            "InRvInit"},
            { (int)EDevState.LeverInRecoverInitRetry,                   "InRvInit1"},
            { (int)EDevState.LeverInRecoverWithOutPulseOn1,             "InRvOut1"},
            { (int)EDevState.LeverInRecoverWithOutPulseOff1,                "InRvOut2"},
            { (int)EDevState.LeverInRecoverWithOutPulseOn2,             "InRvOut3"},
            { (int)EDevState.LeverInRecoverWithOutDoing,                    "InRvOut"},
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
            { (int)EDevState.LeverOutRecoverWithInPulseOff1,                "OutRvIn2"},
            { (int)EDevState.LeverOutRecoverWithInPulseOn2,             "OutRvIn3"},
            { (int)EDevState.LeverOutRecoverWithInDoing,                    "OutRvIn"},
            { (int)EDevState.LeverOutRecoverWithOutPulseOn1,                "OutRvOut1"},
            { (int)EDevState.LeverOutRecoverWithOutPulseOff1,           "OutRvOut2"},
            { (int)EDevState.LeverOutRecoverWithOutPulseOn2,                "OutRvOut3"},
            { (int)EDevState.LeverOutRecoverWithOutDoing,               "OutRvOut"},
            { (int)EDevState.LeverOutRecoverTimeoutInit,                    "OutRvTmInit"},
            { (int)EDevState.LeverOutRecoverTimeoutWithInPulseOn1,      "OutRvTmIn1"},
            { (int)EDevState.LeverOutRecoverTimeoutWithInPulseOff1,     "OutRvTmIn2"},
            { (int)EDevState.LeverOutRecoverTimeoutWithInPulseOn2,      "OutRvTmIn3"},
            { (int)EDevState.LeverOutRecoverTimeoutWithInDoing,         "OutRvTmIn"},
        };

#if true

        private void CallbackInitForDevLever()
        {
            // devLever: 初期化
            devLever.devStateVal = EDevState.LeverInit;
            devLever.devStateValSaved = EDevState.OverRange;
            devLever.devCmdVal = EDevCmd.None;
            devLever.devResultVal = EDevResult.None;
            devLever.devResultValSaved = EDevResult.OverRange;
            devLever.firstFlag = true;
        }
        //		public Action<int> callbackExeForDevLever = (a_iInArg) =>
        private void CallbackExeForDevLever()
        {
            bool boolLogicalStateIn;
            bool boolLogicalStateOut;
            bool boolFuncRet;
            DevCmdPkt devCmdPktLast = null;
            DevCmdPkt devCmdPktCur;
            EDevCmd devCmdCur = EDevCmd.None;

            boolFuncRet = ioBoardDevice.GetUpperStateOfSaveDIn(IoBoardDInLogicalName.LeverIn, out boolLogicalStateIn);
            if (boolFuncRet != true)
            {
                Debug.WriteLine("Lever: IoBoardDevice.DirectIn(LeverIn)エラー");
            }
            boolFuncRet = ioBoardDevice.GetUpperStateOfSaveDIn(IoBoardDInLogicalName.LeverOut, out boolLogicalStateOut);
            if (boolFuncRet != true)
            {
                Debug.WriteLine("Lever: IoBoardDevice.DirectIn(LeverOut)エラー");
            }
            // コマンド受信: 最後のコマンドが有効
            while (concurrentQueueDevCmdPktLever.TryDequeue(out devCmdPktCur))
            {
                devCmdPktLast = devCmdPktCur;
            }
            if (devCmdPktLast != null)
            {
                switch (devCmdPktLast.DevCmdVal)
                {
                    case EDevCmd.LeverIn:
                    case EDevCmd.LeverOut:
                    case EDevCmd.LeverStop:
                        devCmdCur = devCmdPktLast.DevCmdVal;
                        break;
                    default:
                        break;
                }
            }
            switch (devLever.devStateVal)
            {
                case EDevState.LeverInit:
                case EDevState.LeverStop:
                    switch (devCmdCur)
                    {
                        case EDevCmd.LeverStop:
                            // Mainステート・マシンへ結果を出力: Stop正常完了
                            OpFlagStopLever = true;
                            OpResultStopLever = EDeviceResult.Done;
                            if (opeModeTypeVal == EOpeModeType.CheckDevice)
                            {
                                // CheckDevice時、Mainステート・マシンの代わりに結果クリア
                                OpFlagMoveLeverIn = false;
                                OpResultMoveLeverIn = EDeviceResult.None;
                                OpFlagMoveLeverOut = false;
                                OpResultMoveLeverOut = EDeviceResult.None;
                            }
                            break;
                        case EDevCmd.LeverIn:
                            boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.LeverStop);
                            if (boolFuncRet != true)
                            {
                                Debug.WriteLine("Cmd.LeverIn[InStop]、IoBoardDevice.SetUpperStateOfDOut()エラー");
                            }
                            devLever.devStateVal = EDevState.LeverInInit;
                            if (opeModeTypeVal == EOpeModeType.CheckDevice)
                            {
                                // CheckDevice時、Mainステート・マシンの代わりに結果クリア
                                OpFlagStopLever = false;
                                OpResultStopLever = EDeviceResult.None;
                                OpFlagMoveLeverIn = false;
                                OpResultMoveLeverIn = EDeviceResult.None;
                                OpFlagMoveLeverOut = false;
                                OpResultMoveLeverOut = EDeviceResult.None;
                            }
                            break;
                        case EDevCmd.LeverOut:
                            boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.LeverStop);
                            if (boolFuncRet != true)
                            {
                                Debug.WriteLine("Cmd.LeverOut[InStop]、IoBoardDevice.SetUpperStateOfDOut()エラー");
                            }
                            devLever.devStateVal = EDevState.LeverOutInit;
                            if (opeModeTypeVal == EOpeModeType.CheckDevice)
                            {
                                // CheckDevice時、Mainステート・マシンの代わりに結果クリア
                                OpFlagStopLever = false;
                                OpResultStopLever = EDeviceResult.None;
                                OpFlagMoveLeverIn = false;
                                OpResultMoveLeverIn = EDeviceResult.None;
                                OpFlagMoveLeverOut = false;
                                OpResultMoveLeverOut = EDeviceResult.None;
                            }
                            break;
                        default:
                            break;
                    }
                    break;
                case EDevState.LeverStopInitAfterWait:
                case EDevState.LeverStopInitAfterWaitPulseOff:
                case EDevState.LeverStopInit:
                case EDevState.LeverStopWithInPulseOn1:
                case EDevState.LeverStopWithInPulseOff1:
                case EDevState.LeverStopWithInPulseOn2:
                case EDevState.LeverStopWithOutPulseOff2:
                case EDevState.LeverStopWithOutPulseOn:
                    switch (devCmdCur)
                    {
                        case EDevCmd.LeverIn:
                            boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.LeverStop);
                            if (boolFuncRet != true)
                            {
                                Debug.WriteLine("Cmd.LeverIn[InStop]、IoBoardDevice.SetUpperStateOfDOut()エラー");
                            }
                            devLever.devStateVal = EDevState.LeverInInitAfterWait;
                            if (opeModeTypeVal == EOpeModeType.CheckDevice)
                            {
                                // CheckDevice時、Mainステート・マシンの代わりに結果クリア
                                OpFlagStopLever = false;
                                OpResultStopLever = EDeviceResult.None;
                                OpFlagMoveLeverIn = false;
                                OpResultMoveLeverIn = EDeviceResult.None;
                                OpFlagMoveLeverOut = false;
                                OpResultMoveLeverOut = EDeviceResult.None;
                            }
                            break;
                        case EDevCmd.LeverOut:
                            boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.LeverStop);
                            if (boolFuncRet != true)
                            {
                                Debug.WriteLine("Cmd.LeverOut[InStop]、IoBoardDevice.SetUpperStateOfDOut()エラー");
                            }
                            devLever.devStateVal = EDevState.LeverOutInitAfterWait;
                            if (opeModeTypeVal == EOpeModeType.CheckDevice)
                            {
                                // CheckDevice時、Mainステート・マシンの代わりに結果クリア
                                OpFlagStopLever = false;
                                OpResultStopLever = EDeviceResult.None;
                                OpFlagMoveLeverIn = false;
                                OpResultMoveLeverIn = EDeviceResult.None;
                                OpFlagMoveLeverOut = false;
                                OpResultMoveLeverOut = EDeviceResult.None;
                            }
                            break;
                        default:
                            break;
                    }
                    break;
                case EDevState.LeverInInitAfterWait:
                case EDevState.LeverInInitAfterWaitPulseOff:
                case EDevState.LeverInInit:
                case EDevState.LeverInPulseOn1:
                case EDevState.LeverInPulseOff1:
                case EDevState.LeverInPulseOn2:
                case EDevState.LeverInDoing:
                case EDevState.LeverInRecoverInit:
                case EDevState.LeverInRecoverInitRetry:
                case EDevState.LeverInRecoverWithOutPulseOn1:
                case EDevState.LeverInRecoverWithOutPulseOff1:
                case EDevState.LeverInRecoverWithOutPulseOn2:
                case EDevState.LeverInRecoverWithOutDoing:
                case EDevState.LeverInRecoverWithInPulseOn1:
                case EDevState.LeverInRecoverWithInPulseOff1:
                case EDevState.LeverInRecoverWithInPulseOn2:
                case EDevState.LeverInRecoverWithInDoing:
                case EDevState.LeverInRecoverTimeoutInit:
                case EDevState.LeverInRecoverTimeoutWithOutPulseOn1:
                case EDevState.LeverInRecoverTimeoutWithOutPulseOff1:
                case EDevState.LeverInRecoverTimeoutWithOutPulseOn2:
                case EDevState.LeverInRecoverTimeoutWithOutDoing:
                    switch (devCmdCur)
                    {
                        case EDevCmd.LeverOut:
                            boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.LeverStop);
                            if (boolFuncRet != true)
                            {
                                Debug.WriteLine("Cmd.LeverOut[InLeverIn]、IoBoardDevice.SetUpperStateOfDOut()エラー");
                            }
                            devLever.devStateVal = EDevState.LeverOutInitAfterWait;
                            if (opeModeTypeVal == EOpeModeType.CheckDevice)
                            {
                                // CheckDevice時、Mainステート・マシンの代わりに結果クリア
                                OpFlagStopLever = false;
                                OpResultStopLever = EDeviceResult.None;
                                OpFlagMoveLeverIn = false;
                                OpResultMoveLeverIn = EDeviceResult.None;
                                OpFlagMoveLeverOut = false;
                                OpResultMoveLeverOut = EDeviceResult.None;
                            }
                            break;
                        case EDevCmd.LeverStop:
                            boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.LeverStop);
                            if (boolFuncRet != true)
                            {
                                Debug.WriteLine("Cmd.LeverStop[InLeverIn]、IoBoardDevice.SetUpperStateOfDOut()エラー");
                            }
                            devLever.devStateVal = EDevState.LeverStopInitAfterWait;
                            if (opeModeTypeVal == EOpeModeType.CheckDevice)
                            {
                                // CheckDevice時、Mainステート・マシンの代わりに結果クリア
                                OpFlagStopLever = false;
                                OpResultStopLever = EDeviceResult.None;
                                OpFlagMoveLeverIn = false;
                                OpResultMoveLeverIn = EDeviceResult.None;
                                OpFlagMoveLeverOut = false;
                                OpResultMoveLeverOut = EDeviceResult.None;
                            }
                            break;
                        default:
                            break;
                    }
                    break;
                case EDevState.LeverOutInitAfterWait:
                case EDevState.LeverOutInitAfterWaitPulseOff:
                case EDevState.LeverOutInit:
                case EDevState.LeverOutPulseOn1:
                case EDevState.LeverOutPulseOff1:
                case EDevState.LeverOutPulseOn2:
                case EDevState.LeverOutDoing:
                case EDevState.LeverOutRecoverInit:
                case EDevState.LeverOutRecoverInitRetry:
                case EDevState.LeverOutRecoverWithInPulseOn1:
                case EDevState.LeverOutRecoverWithInPulseOff1:
                case EDevState.LeverOutRecoverWithInPulseOn2:
                case EDevState.LeverOutRecoverWithInDoing:
                case EDevState.LeverOutRecoverWithOutPulseOn1:
                case EDevState.LeverOutRecoverWithOutPulseOff1:
                case EDevState.LeverOutRecoverWithOutPulseOn2:
                case EDevState.LeverOutRecoverWithOutDoing:
                case EDevState.LeverOutRecoverTimeoutInit:
                case EDevState.LeverOutRecoverTimeoutWithInPulseOn1:
                case EDevState.LeverOutRecoverTimeoutWithInPulseOff1:
                case EDevState.LeverOutRecoverTimeoutWithInPulseOn2:
                case EDevState.LeverOutRecoverTimeoutWithInDoing:
                    switch (devCmdCur)
                    {
                        case EDevCmd.LeverIn:
                            boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.LeverStop);
                            if (boolFuncRet != true)
                            {
                                Debug.WriteLine("Cmd.LeverIn[InLeverOut]、IoBoardDevice.SetUpperStateOfDOut()エラー");
                            }
                            devLever.devStateVal = EDevState.LeverInInitAfterWait;
                            if (opeModeTypeVal == EOpeModeType.CheckDevice)
                            {
                                // CheckDevice時、Mainステート・マシンの代わりに結果クリア
                                OpFlagStopLever = false;
                                OpResultStopLever = EDeviceResult.None;
                                OpFlagMoveLeverIn = false;
                                OpResultMoveLeverIn = EDeviceResult.None;
                                OpFlagMoveLeverOut = false;
                                OpResultMoveLeverOut = EDeviceResult.None;
                            }
                            break;
                        case EDevCmd.LeverStop:
                            boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.LeverStop);
                            if (boolFuncRet != true)
                            {
                                Debug.WriteLine("Cmd.LeverStop[InLeverOut]、IoBoardDevice.SetUpperStateOfDOut()エラー");
                            }
                            devLever.devStateVal = EDevState.LeverStopInitAfterWait;
                            if (opeModeTypeVal == EOpeModeType.CheckDevice)
                            {
                                // CheckDevice時、Mainステート・マシンの代わりに結果クリア
                                OpFlagStopLever = false;
                                OpResultStopLever = EDeviceResult.None;
                                OpFlagMoveLeverIn = false;
                                OpResultMoveLeverIn = EDeviceResult.None;
                                OpFlagMoveLeverOut = false;
                                OpResultMoveLeverOut = EDeviceResult.None;
                            }
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }
            switch (devLever.devStateVal)
            {
                case EDevState.LeverInit:
                    devLever.devStateVal = EDevState.LeverStop;
                    break;
                #region LeverStop
                case EDevState.LeverStop:
                    break;
                case EDevState.LeverStopInitAfterWait:
                    boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.LeverStop);
                    if (boolFuncRet != true)
                    {
                        Debug.WriteLine("Lever:State[LeverStopInitAfterWait] IoBoardDevice.SetUpperStateOfDOut(LeverStop)エラー");
                    }
                    if ((boolLogicalStateIn == true) ||
                        (boolLogicalStateOut == true))
                    {
                        // Mainステート・マシンへ結果を出力: Stop正常完了
                        OpFlagStopLever = true;
                        OpResultStopLever = EDeviceResult.Done;
                        devLever.devStateVal = EDevState.LeverStop;
                    }
                    else
                    {
                        devLever.stopwatchPulse.Restart();
                        devLever.devStateVal = EDevState.LeverStopInitAfterWaitPulseOff;
                    }
                    break;
                case EDevState.LeverStopInitAfterWaitPulseOff:
                    if ((boolLogicalStateIn == true) ||
                        (boolLogicalStateOut == true))
                    {
                        // Mainステート・マシンへ結果を出力: Stop正常完了
                        OpFlagStopLever = true;
                        OpResultStopLever = EDeviceResult.Done;
                        devLever.devStateVal = EDevState.LeverStop;
                    }
                    else if (devLever.stopwatchPulse.ElapsedMilliseconds >= preferencesDatOriginal.InactivePulseWidthOfLever)
                    {
                        devLever.stopwatchPulse.Stop();
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.LeverIn);
                        if (boolFuncRet != true)
                        {
                            Debug.WriteLine("Lever:State[LeverStopInitAfterWaitPulseOff] IoBoardDevice.SetUpperStateOfDOut(LeverIn)エラー");
                        }
                        devLever.stopwatchPulse.Restart();
                        devLever.devStateVal = EDevState.LeverStopWithInPulseOn1;
                    }
                    break;
                case EDevState.LeverStopInit:
                    boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.LeverStop);
                    if (boolFuncRet != true)
                    {
                        Debug.WriteLine("Lever:State[LeverStopInit] IoBoardDevice.SetUpperStateOfDOut(LeverStop)エラー");
                    }
                    if ((boolLogicalStateIn == true) ||
                        (boolLogicalStateOut == true))
                    {
                        // Mainステート・マシンへ結果を出力: Stop正常完了
                        OpFlagStopLever = true;
                        OpResultStopLever = EDeviceResult.Done;
                        devLever.devStateVal = EDevState.LeverStop;
                    }
                    else
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.LeverIn);
                        if (boolFuncRet != true)
                        {
                            Debug.WriteLine("Lever:State[LeverStopInit] IoBoardDevice.SetUpperStateOfDOut(LeverIn)エラー");
                        }
                        devLever.stopwatchPulse.Restart();
                        devLever.devStateVal = EDevState.LeverStopWithInPulseOn1;
                    }
                    break;
                case EDevState.LeverStopWithInPulseOn1:
                    if ((boolLogicalStateIn == true) ||
                        (boolLogicalStateOut == true))
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.LeverStop);
                        if (boolFuncRet != true)
                        {
                            Debug.WriteLine("Lever:State[LeverStopWithInPulseOn1] IoBoardDevice.SetUpperStateOfDOut(LeverStop)エラー");
                        }
                        // Mainステート・マシンへ結果を出力: Stop正常完了
                        OpFlagStopLever = true;
                        OpResultStopLever = EDeviceResult.Done;
                        devLever.devStateVal = EDevState.LeverStop;
                    }
                    else if (devLever.stopwatchPulse.ElapsedMilliseconds >= preferencesDatOriginal.ActivePulseWidthOfLever)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.LeverStop);
                        if (boolFuncRet != true)
                        {
                            Debug.WriteLine("Lever:State[LeverStopWithInPulseOn1] IoBoardDevice.SetUpperStateOfDOut(LeverStop)エラー");
                        }
                        devLever.stopwatchPulse.Restart();
                        devLever.devStateVal = EDevState.LeverStopWithInPulseOff1;
                    }
                    break;

                case EDevState.LeverStopWithInPulseOff1:
                    if ((boolLogicalStateIn == true) ||
                        (boolLogicalStateOut == true))
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.LeverStop);
                        if (boolFuncRet != true)
                        {
                            Debug.WriteLine("Lever:State[LeverStopWithInPulseOff1] IoBoardDevice.SetUpperStateOfDOut(LeverStop)エラー");
                        }
                        // Mainステート・マシンへ結果を出力: Stop正常完了
                        OpFlagStopLever = true;
                        OpResultStopLever = EDeviceResult.Done;
                        devLever.devStateVal = EDevState.LeverStop;
                    }
                    else if (devLever.stopwatchPulse.ElapsedMilliseconds >= preferencesDatOriginal.InactivePulseWidthOfLever)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.LeverIn);
                        if (boolFuncRet != true)
                        {
                            Debug.WriteLine("Lever:State[LeverStopWithInPulseOff1] IoBoardDevice.SetUpperStateOfDOut(LeverIn)エラー");
                        }
                        devLever.stopwatchPulse.Restart();
                        devLever.devStateVal = EDevState.LeverStopWithInPulseOn2;
                    }
                    break;
                case EDevState.LeverStopWithInPulseOn2:
                    if ((boolLogicalStateIn == true) ||
                        (boolLogicalStateOut == true))
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.LeverStop);
                        if (boolFuncRet != true)
                        {
                            Debug.WriteLine("Lever:State[LeverStopWithInPulseOn2] IoBoardDevice.SetUpperStateOfDOut(LeverStop)エラー");
                        }
                        // Mainステート・マシンへ結果を出力: Stop正常完了
                        OpFlagStopLever = true;
                        OpResultStopLever = EDeviceResult.Done;
                        devLever.devStateVal = EDevState.LeverStop;
                    }
                    else if (devLever.stopwatchPulse.ElapsedMilliseconds >= preferencesDatOriginal.ActivePulseWidthOfLever)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.LeverStop);
                        if (boolFuncRet != true)
                        {
                            Debug.WriteLine("Lever:State[LeverStopWithInPulseOn2] IoBoardDevice.SetUpperStateOfDOut(LeverStop)エラー");
                        }
                        devLever.stopwatchPulse.Restart();
                        devLever.devStateVal = EDevState.LeverStopWithOutPulseOff2;
                    }
                    break;
                case EDevState.LeverStopWithOutPulseOff2:
                    if ((boolLogicalStateIn == true) ||
                        (boolLogicalStateOut == true))
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.LeverStop);
                        if (boolFuncRet != true)
                        {
                            Debug.WriteLine("Lever:State[LeverStopWithOutPulseOff2] IoBoardDevice.SetUpperStateOfDOut(LeverStop)エラー");
                        }
                        // Mainステート・マシンへ結果を出力: Stop正常完了
                        OpFlagStopLever = true;
                        OpResultStopLever = EDeviceResult.Done;
                        devLever.devStateVal = EDevState.LeverStop;
                    }
                    else if (devLever.stopwatchPulse.ElapsedMilliseconds >= preferencesDatOriginal.InactivePulseWidthOfLever)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.LeverOut);
                        if (boolFuncRet != true)
                        {
                            Debug.WriteLine("Lever:State[LeverStopWithOutPulseOff2] IoBoardDevice.SetUpperStateOfDOut(LeverOut)エラー");
                        }
                        devLever.stopwatchPulse.Restart();
                        devLever.devStateVal = EDevState.LeverStopWithOutPulseOn;
                    }
                    break;
                case EDevState.LeverStopWithOutPulseOn:
                    if ((boolLogicalStateIn == true) ||
                        (boolLogicalStateOut == true))
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.LeverStop);
                        if (boolFuncRet != true)
                        {
                            Debug.WriteLine("Lever:State[LeverStopWithOutPulseOn] IoBoardDevice.SetUpperStateOfDOut(LeverStop)エラー");
                        }
                        // Mainステート・マシンへ結果を出力: Stop正常完了
                        OpFlagStopLever = true;
                        OpResultStopLever = EDeviceResult.Done;
                        devLever.devStateVal = EDevState.LeverStop;
                    }
                    else if (devLever.stopwatchPulse.ElapsedMilliseconds >= preferencesDatOriginal.ActivePulseWidthOfLever)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.LeverStop);
                        if (boolFuncRet != true)
                        {
                            Debug.WriteLine("Lever:State[LeverStopWithOutPulseOn] IoBoardDevice.SetUpperStateOfDOut(LeverStop)エラー");
                        }
                        // Mainステート・マシンへ結果を出力: Stop正常完了
                        OpFlagStopLever = true;
                        OpResultStopLever = EDeviceResult.Done;
                        devLever.devStateVal = EDevState.LeverStop;
                        devLever.stopwatchPulse.Stop();
                    }
                    break;
                #endregion
                #region LeverIn

                case EDevState.LeverInInitAfterWait:
                    boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.LeverStop);
                    if (boolFuncRet != true)
                    {
                        Debug.WriteLine("Lever:State[LeverOpenInitAfterWait] IoBoardDevice.SetUpperStateOfDOut(LeverStop)エラー");
                    }
                    if (boolLogicalStateIn == true)
                    {
                        // Mainステート・マシンへ結果を出力: In完了
                        OpFlagMoveLeverIn = true;
                        OpResultMoveLeverIn = EDeviceResult.Done;
                        devLever.devStateVal = EDevState.LeverStop;
                    }
                    else
                    {
                        devLever.stopwatchPulse.Restart();
                        devLever.devStateVal = EDevState.LeverInInitAfterWaitPulseOff;
                    }
                    break;
                case EDevState.LeverInInitAfterWaitPulseOff:
                    if (boolLogicalStateIn == true)
                    {
                        // Mainステート・マシンへ結果を出力: In完了
                        OpFlagMoveLeverIn = true;
                        OpResultMoveLeverIn = EDeviceResult.Done;
                        devLever.devStateVal = EDevState.LeverStop;
                    }
                    else if (devLever.stopwatchPulse.ElapsedMilliseconds >= preferencesDatOriginal.InactivePulseWidthOfLever)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.LeverIn);
                        if (boolFuncRet != true)
                        {
                            Debug.WriteLine("Lever:State[LeverInInitAfterWaitPulseOff] IoBoardDevice.SetUpperStateOfDOut(LeverIn)エラー");
                        }
                        devLever.stopwatchPulse.Restart();
                        devLever.stopwatchTimeout.Restart();
                        devLever.devStateVal = EDevState.LeverInPulseOn1;
                    }
                    break;
                case EDevState.LeverInInit:
                    boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.LeverStop);
                    if (boolFuncRet != true)
                    {
                        Debug.WriteLine("Lever:State[LeverInInit] IoBoardDevice.SetUpperStateOfDOut(LeverStop)エラー");
                    }
                    if (boolLogicalStateIn == true)
                    {
                        // Mainステート・マシンへ結果を出力: In完了
                        OpFlagMoveLeverIn = true;
                        OpResultMoveLeverIn = EDeviceResult.Done;
                        devLever.stopwatchPulse.Stop();
                        devLever.devStateVal = EDevState.LeverStop;
                    }
                    else
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.LeverIn);
                        if (boolFuncRet != true)
                        {
                            Debug.WriteLine("Lever:State[LeverInInitAfterWaitPulseOff] IoBoardDevice.SetUpperStateOfDOut(LeverIn)エラー");
                        }
                        devLever.stopwatchPulse.Restart();
                        devLever.stopwatchTimeout.Restart();
                        devLever.devStateVal = EDevState.LeverInPulseOn1;
                    }
                    break;
                case EDevState.LeverInPulseOn1:
                    if (boolLogicalStateIn == true)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.LeverStop);
                        if (boolFuncRet != true)
                        {
                            Debug.WriteLine("Lever:State[LeverInPulseOn1] IoBoardDevice.SetUpperStateOfDOut(LeverStop)エラー");
                        }
                        // Mainステート・マシンへ結果を出力: In完了
                        OpFlagMoveLeverIn = true;
                        OpResultMoveLeverIn = EDeviceResult.Done;
                        devLever.devStateVal = EDevState.LeverStop;
                    }
                    else if (devLever.stopwatchPulse.ElapsedMilliseconds >= preferencesDatOriginal.ActivePulseWidthOfLever)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.LeverStop);
                        if (boolFuncRet != true)
                        {
                            Debug.WriteLine("Lever:State[LeverInPulseOn1] IoBoardDevice.SetUpperStateOfDOut(LeverStop)エラー");
                        }
                        devLever.stopwatchPulse.Restart();
                        devLever.devStateVal = EDevState.LeverInPulseOff1;
                    }
                    break;
                case EDevState.LeverInPulseOff1:
                    if (boolLogicalStateIn == true)
                    {
                        // Mainステート・マシンへ結果を出力: In完了
                        OpFlagMoveLeverIn = true;
                        OpResultMoveLeverIn = EDeviceResult.Done;
                        devLever.devStateVal = EDevState.LeverStop;
                    }
                    else if (devLever.stopwatchPulse.ElapsedMilliseconds >= preferencesDatOriginal.InactivePulseWidthOfLever)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.LeverIn);
                        if (boolFuncRet != true)
                        {
                            Debug.WriteLine("Lever:State[LeverInPulseOff1] IoBoardDevice.SetUpperStateOfDOut(LeverIn)エラー");
                        }
                        devLever.stopwatchPulse.Restart();
                        devLever.devStateVal = EDevState.LeverInPulseOn2;
                    }
                    break;
                case EDevState.LeverInPulseOn2:
                    if (boolLogicalStateIn == true)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.LeverStop);
                        if (boolFuncRet != true)
                        {
                            Debug.WriteLine("Lever:State[LeverInPulseOn1] IoBoardDevice.SetUpperStateOfDOut(LeverStop)エラー");
                        }
                        // Mainステート・マシンへ結果を出力: In完了
                        OpFlagMoveLeverIn = true;
                        OpResultMoveLeverIn = EDeviceResult.Done;
                        devLever.devStateVal = EDevState.LeverStop;
                    }
                    else if (devLever.stopwatchPulse.ElapsedMilliseconds >= preferencesDatOriginal.ActivePulseWidthOfLever)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.LeverStop);
                        if (boolFuncRet != true)
                        {
                            Debug.WriteLine("Lever:State[LeverInPulseOn2] IoBoardDevice.SetUpperStateOfDOut(LeverStop)エラー");
                        }
                        devLever.stopwatchPulse.Stop();
                        devLever.devStateVal = EDevState.LeverInDoing;
                    }
                    break;
                case EDevState.LeverInDoing:
                    if (boolLogicalStateIn == true)
                    {
                        // Mainステート・マシンへ結果を出力: Open完了
                        OpFlagMoveLeverIn = true;
                        OpResultMoveLeverIn = EDeviceResult.Done;
                        devLever.devStateVal = EDevState.LeverStop;
                    }
                    else if (devLever.stopwatchTimeout.ElapsedMilliseconds >= (preferencesDatOriginal.TimeoutOfLeverIn * 1000))
                    {
                        // Openタイムアウトの時
                        if (Properties.Settings.Default.IS_LEVER_RECOVER_ENABLE == true)
                        {
                            // レバー回復処理する時
                            boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.LeverOut);
                            if (boolFuncRet != true)
                            {
                                Debug.WriteLine("Lever:State[LeverInDoing] IoBoardDevice.SetUpperStateOfDOut(LeverOut)エラー");
                            }
                            // リトライ・カウント: 0クリア
                            devLever.retryCount = 0;
                            devLever.stopwatchPulse.Restart();
                            devLever.stopwatchTimeout.Restart();
                            devLever.devStateVal = EDevState.LeverInRecoverWithOutPulseOn1;
                        }
                        else
                        {
                            // レバー回復処理しない時
                            devLever.stopwatchPulse.Stop();
                            devLever.stopwatchTimeout.Stop();
                            // Mainステート・マシンへ結果を出力: Timeout
                            OpFlagMoveLeverIn = true;
                            OpResultMoveLeverIn = EDeviceResult.TimeOut;
                            devLever.devStateVal = EDevState.LeverStop;
                        }
                    }
                    break;
                case EDevState.LeverInRecoverInit:
                    if (boolLogicalStateIn == true)
                    {
                        // Mainステート・マシンへ結果を出力: In完了
                        OpFlagMoveLeverIn = true;
                        OpResultMoveLeverIn = EDeviceResult.Done;
                        devLever.devStateVal = EDevState.LeverStop;
                    }
                    else
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.LeverStop);
                        if (boolFuncRet != true)
                        {
                            Debug.WriteLine("Lever:State[LeverInRecoverInit] IoBoardDevice.SetUpperStateOfDOut(LeverStop)エラー");
                        }
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.LeverOut);
                        if (boolFuncRet != true)
                        {
                            Debug.WriteLine("Lever:State[LeverInRecoverInit] IoBoardDevice.SetUpperStateOfDOut(LeverOut)エラー");
                        }
                        devLever.retryCount = 0;
                        devLever.stopwatchPulse.Restart();
                        devLever.devStateVal = EDevState.LeverInRecoverWithOutPulseOn1;
                    }
                    break;
                case EDevState.LeverInRecoverInitRetry:
                    break;
                case EDevState.LeverInRecoverWithOutPulseOn1:
                    // 現在: Outリミット・センサは見ない
                    if (devLever.stopwatchPulse.ElapsedMilliseconds >= preferencesDatOriginal.ActivePulseWidthOfLever)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.LeverStop);
                        if (boolFuncRet != true)
                        {
                            Debug.WriteLine("Lever:State[LeverInRecoverWithOutPulseOn1] IoBoardDevice.SetUpperStateOfDOut(LeverStop)エラー");
                        }
                        devLever.stopwatchPulse.Restart();
                        devLever.devStateVal = EDevState.LeverInRecoverWithOutPulseOff1;
                    }
                    break;
                case EDevState.LeverInRecoverWithOutPulseOff1:
                    // 現在: Outリミット・センサは見ない
                    if (devLever.stopwatchPulse.ElapsedMilliseconds >= preferencesDatOriginal.InactivePulseWidthOfLever)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.LeverOut);
                        if (boolFuncRet != true)
                        {
                            Debug.WriteLine("Lever:State[LeverInRecoverWithOutPulseOff1] IoBoardDevice.SetUpperStateOfDOut(LeverOut)エラー");
                        }
                        devLever.stopwatchPulse.Restart();
                        devLever.devStateVal = EDevState.LeverInRecoverWithOutPulseOn2;
                    }
                    break;
                case EDevState.LeverInRecoverWithOutPulseOn2:
                    // 現在: Outリミット・センサは見ない
                    if (devLever.stopwatchPulse.ElapsedMilliseconds >= preferencesDatOriginal.ActivePulseWidthOfLever)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.LeverStop);
                        if (boolFuncRet != true)
                        {
                            Debug.WriteLine("Lever:State[LeverInRecoverWithOutPulseOn2] IoBoardDevice.SetUpperStateOfDOut(LeverStop)エラー");
                        }
                        devLever.stopwatchPulse.Stop();
                        // Close動作時間: 開始
                        devLever.stopwatchTimeout.Restart();
                        devLever.devStateVal = EDevState.LeverInRecoverWithOutDoing;
                    }
                    break;
                case EDevState.LeverInRecoverWithOutDoing:
                    if ((boolLogicalStateOut == true) ||
                        (devLever.stopwatchTimeout.ElapsedMilliseconds >= preferencesDatOriginal.LeverOutTimeInRecoverIn))
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.LeverIn);
                        if (boolFuncRet != true)
                        {
                            Debug.WriteLine("Lever:State[LeverInRecoverWithOutDoing] IoBoardDevice.SetUpperStateOfDOut(LeverIn)エラー");
                        }
                        devLever.stopwatchPulse.Restart();
                        // Openタイムアウト: 開始
                        devLever.stopwatchTimeout.Restart();
                        devLever.devStateVal = EDevState.LeverInRecoverWithInPulseOn1;
                    }
                    break;
                case EDevState.LeverInRecoverWithInPulseOn1:
                    if (boolLogicalStateIn == true)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.LeverStop);
                        if (boolFuncRet != true)
                        {
                            Debug.WriteLine("Lever:State[LeverInRecoverWithInPulseOn1] IoBoardDevice.SetUpperStateOfDOut(LeverStop)エラー");
                        }
                        // Mainステート・マシンへ結果を出力: In完了
                        OpFlagMoveLeverIn = true;
                        OpResultMoveLeverIn = EDeviceResult.Done;
                        devLever.stopwatchPulse.Stop();
                        devLever.stopwatchTimeout.Stop();
                        devLever.devStateVal = EDevState.LeverStop;
                    }
                    else if (devLever.stopwatchPulse.ElapsedMilliseconds >= preferencesDatOriginal.ActivePulseWidthOfLever)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.LeverStop);
                        if (boolFuncRet != true)
                        {
                            Debug.WriteLine("Lever:State[LeverInRecoverWithInPulseOn1] IoBoardDevice.SetUpperStateOfDOut(LeverStop)エラー");
                        }
                        devLever.stopwatchPulse.Restart();
                        devLever.devStateVal = EDevState.LeverInRecoverWithInPulseOff1;
                    }
                    break;
                case EDevState.LeverInRecoverWithInPulseOff1:
                    if (boolLogicalStateIn == true)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.LeverStop);
                        if (boolFuncRet != true)
                        {
                            Debug.WriteLine("Lever:State[LeverInRecoverWithInPulseOff1] IoBoardDevice.SetUpperStateOfDOut(LeverStop)エラー");
                        }
                        // Mainステート・マシンへ結果を出力: In完了
                        OpFlagMoveLeverIn = true;
                        OpResultMoveLeverIn = EDeviceResult.Done;
                        devLever.stopwatchPulse.Stop();
                        devLever.stopwatchTimeout.Stop();
                        devLever.devStateVal = EDevState.LeverStop;
                    }
                    else if (devLever.stopwatchPulse.ElapsedMilliseconds >= preferencesDatOriginal.InactivePulseWidthOfLever)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.LeverIn);
                        if (boolFuncRet != true)
                        {
                            Debug.WriteLine("Lever:State[LeverInRecoverWithInPulseOff1] IoBoardDevice.SetUpperStateOfDOut(LeverOpen)エラー");
                        }
                        devLever.stopwatchPulse.Restart();
                        devLever.devStateVal = EDevState.LeverInRecoverWithInPulseOn2;
                    }
                    break;
                case EDevState.LeverInRecoverWithInPulseOn2:
                    if (boolLogicalStateIn == true)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.LeverStop);
                        if (boolFuncRet != true)
                        {
                            Debug.WriteLine("Lever:State[LeverInRecoverWithInPulseOn2] IoBoardDevice.SetUpperStateOfDOut(LeverStop)エラー");
                        }
                        // Mainステート・マシンへ結果を出力: Open完了
                        OpFlagMoveLeverIn = true;
                        OpResultMoveLeverIn = EDeviceResult.Done;
                        devLever.stopwatchPulse.Stop();
                        devLever.stopwatchTimeout.Stop();
                        devLever.devStateVal = EDevState.LeverStop;
                    }
                    else if (devLever.stopwatchPulse.ElapsedMilliseconds >= preferencesDatOriginal.ActivePulseWidthOfLever)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.LeverStop);
                        if (boolFuncRet != true)
                        {
                            Debug.WriteLine("Lever:State[LeverInRecoverWithInPulseOn2] IoBoardDevice.SetUpperStateOfDOut(LeverStop)エラー");
                        }
                        devLever.stopwatchPulse.Stop();
                        devLever.devStateVal = EDevState.LeverInRecoverWithInDoing;
                    }
                    break;
                case EDevState.LeverInRecoverWithInDoing:
                    if (boolLogicalStateIn == true)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.LeverStop);
                        if (boolFuncRet != true)
                        {
                            Debug.WriteLine("Lever:State[LeverInRecoverWithInDoing] IoBoardDevice.SetUpperStateOfDOut(LeverStop)エラー");
                        }
                        // Mainステート・マシンへ結果を出力: In完了
                        OpFlagMoveLeverIn = true;
                        OpResultMoveLeverIn = EDeviceResult.Done;
                        devLever.stopwatchPulse.Stop();
                        devLever.stopwatchTimeout.Stop();
                        devLever.devStateVal = EDevState.LeverStop;
                    }
                    // タイムアウトはOut方向へ動かした2倍の時間
                    else if (devLever.stopwatchTimeout.ElapsedMilliseconds >= (preferencesDatOriginal.LeverOutTimeInRecoverIn * 2))
                    {
                        // タイムアウトの時
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.LeverStop);
                        if (boolFuncRet != true)
                        {
                            Debug.WriteLine("Lever:State[LeverInRecoverWithInDoing] IoBoardDevice.SetUpperStateOfDOut(LeverStop)エラー");
                        }
                        devLever.stopwatchTimeout.Stop();
                        // リトライ・カウントアップ 
                        devLever.retryCount++;
                        if (devLever.retryCount > preferencesDatOriginal.LeverRetryNumInRecover)
                        {
                            // リトライ回数に到達した時
                            boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.LeverOut);
                            if (boolFuncRet != true)
                            {
                                Debug.WriteLine("Lever:State[LeverInRecoverWithInDoing] IoBoardDevice.SetUpperStateOfDOut(LeverOut)エラー");
                            }
                            devLever.stopwatchPulse.Restart();
                            devLever.stopwatchTimeout.Restart();
                            devLever.devStateVal = EDevState.LeverInRecoverTimeoutWithOutPulseOn1;
                        }
                        else
                        {
                            // リトライ回数までいっていない時
                            boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.LeverOut);
                            if (boolFuncRet != true)
                            {
                                Debug.WriteLine("Lever:State[LeverInRecoverWithInDoing] IoBoardDevice.SetUpperStateOfDOut(LeverOut)エラー");
                            }
                            devLever.stopwatchPulse.Restart();
                            devLever.stopwatchTimeout.Restart();
                            devLever.devStateVal = EDevState.LeverInRecoverWithOutPulseOn1;

                        }
                    }
                    break;
                case EDevState.LeverInRecoverTimeoutInit:
                    break;
                case EDevState.LeverInRecoverTimeoutWithOutPulseOn1:
                    // 現在: Outリミット・センサは見ない
                    if (devLever.stopwatchPulse.ElapsedMilliseconds >= preferencesDatOriginal.ActivePulseWidthOfLever)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.LeverStop);
                        if (boolFuncRet != true)
                        {
                            Debug.WriteLine("Lever:State[LeverInRecoverTimeoutWithOutPulseOn1] IoBoardDevice.SetUpperStateOfDOut(LeverStop)エラー");
                        }
                        devLever.stopwatchPulse.Restart();
                        devLever.devStateVal = EDevState.LeverInRecoverTimeoutWithOutPulseOff1;
                    }
                    break;
                case EDevState.LeverInRecoverTimeoutWithOutPulseOff1:
                    // 現在: Outリミット・センサは見ない
                    if (devLever.stopwatchPulse.ElapsedMilliseconds >= preferencesDatOriginal.InactivePulseWidthOfLever)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.LeverOut);
                        if (boolFuncRet != true)
                        {
                            Debug.WriteLine("Lever:State[LeverInRecoverTimeoutWithOutPulseOff1] IoBoardDevice.SetUpperStateOfDOut(LeverOut)エラー");
                        }
                        devLever.stopwatchPulse.Restart();
                        devLever.devStateVal = EDevState.LeverInRecoverTimeoutWithOutPulseOn2;
                    }
                    break;
                case EDevState.LeverInRecoverTimeoutWithOutPulseOn2:
                    // 現在: Outリミット・センサは見ない
                    if (devLever.stopwatchPulse.ElapsedMilliseconds >= preferencesDatOriginal.ActivePulseWidthOfLever)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.LeverStop);
                        if (boolFuncRet != true)
                        {
                            Debug.WriteLine("Lever:State[LeverInRecoverTimeoutWithOutPulseOn2] IoBoardDevice.SetUpperStateOfDOut(LeverStop)エラー");
                        }
                        devLever.stopwatchPulse.Stop();
                        devLever.devStateVal = EDevState.LeverInRecoverTimeoutWithOutDoing;
                    }
                    break;
                case EDevState.LeverInRecoverTimeoutWithOutDoing:
                    // Close方向へClose時間引いて停止
                    if (devLever.stopwatchTimeout.ElapsedMilliseconds >= preferencesDatOriginal.LeverOutTimeInRecoverIn)
                    {
                        devLever.stopwatchPulse.Stop();
                        devLever.stopwatchTimeout.Stop();
                        // Mainステート・マシンへ結果を出力: Timeout
                        OpFlagMoveLeverIn = true;
                        OpResultMoveLeverIn = EDeviceResult.TimeOut;
                        devLever.devStateVal = EDevState.LeverStop;
                    }
                    break;
                #endregion
                #region LeverOut

                case EDevState.LeverOutInitAfterWait:
                    boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.LeverStop);
                    if (boolFuncRet != true)
                    {
                        Debug.WriteLine("Lever:State[LeverOutInitAfterWait] IoBoardDevice.SetUpperStateOfDOut(LeverStop)エラー");
                    }
                    if (boolLogicalStateOut == true)
                    {
                        // Mainステート・マシンへ結果を出力: Out完了
                        OpFlagMoveLeverOut = true;
                        OpResultMoveLeverOut = EDeviceResult.Done;
                        devLever.devStateVal = EDevState.LeverStop;
                    }
                    else
                    {
                        devLever.stopwatchPulse.Restart();
                        devLever.devStateVal = EDevState.LeverOutInitAfterWaitPulseOff;
                    }
                    break;
                case EDevState.LeverOutInitAfterWaitPulseOff:
                    if (boolLogicalStateOut == true)
                    {
                        // Mainステート・マシンへ結果を出力: Out完了
                        OpFlagMoveLeverOut = true;
                        OpResultMoveLeverOut = EDeviceResult.Done;
                        devLever.devStateVal = EDevState.LeverStop;
                    }
                    else if (devLever.stopwatchPulse.ElapsedMilliseconds >= preferencesDatOriginal.InactivePulseWidthOfLever)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.LeverOut);
                        if (boolFuncRet != true)
                        {
                            Debug.WriteLine("Lever:State[LeverOutInitAfterWaitPulseOff] IoBoardDevice.SetUpperStateOfDOut(LeverClose)エラー");
                        }
                        devLever.stopwatchPulse.Restart();
                        devLever.stopwatchTimeout.Restart();
                        devLever.devStateVal = EDevState.LeverOutPulseOn1;
                    }
                    break;
                case EDevState.LeverOutInit:
                    boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.LeverStop);
                    if (boolFuncRet != true)
                    {
                        Debug.WriteLine("Lever:State[LeverOutInit] IoBoardDevice.SetUpperStateOfDOut(LeverStop)エラー");
                    }
                    if (boolLogicalStateOut == true)
                    {
                        // Mainステート・マシンへ結果を出力: Out完了
                        OpFlagMoveLeverOut = true;
                        OpResultMoveLeverOut = EDeviceResult.Done;
                        devLever.stopwatchPulse.Stop();
                        devLever.devStateVal = EDevState.LeverStop;
                    }
                    else
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.LeverOut);
                        if (boolFuncRet != true)
                        {
                            Debug.WriteLine("Lever:State[LeverOutInitAfterWaitPulseOff] IoBoardDevice.SetUpperStateOfDOut(LeverOut)エラー");
                        }
                        devLever.stopwatchPulse.Restart();
                        devLever.stopwatchTimeout.Restart();
                        devLever.devStateVal = EDevState.LeverOutPulseOn1;
                    }
                    break;
                case EDevState.LeverOutPulseOn1:
                    if (boolLogicalStateOut == true)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.LeverStop);
                        if (boolFuncRet != true)
                        {
                            Debug.WriteLine("Lever:State[LeverOutPulseOn1] IoBoardDevice.SetUpperStateOfDOut(LeverStop)エラー");
                        }
                        // Mainステート・マシンへ結果を出力: Out完了
                        OpFlagMoveLeverOut = true;
                        OpResultMoveLeverOut = EDeviceResult.Done;
                        devLever.devStateVal = EDevState.LeverStop;
                    }
                    else if (devLever.stopwatchPulse.ElapsedMilliseconds >= preferencesDatOriginal.ActivePulseWidthOfLever)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.LeverStop);
                        if (boolFuncRet != true)
                        {
                            Debug.WriteLine("Lever:State[LeverOutPulseOn1] IoBoardDevice.SetUpperStateOfDOut(LeverStop)エラー");
                        }
                        devLever.stopwatchPulse.Restart();
                        devLever.devStateVal = EDevState.LeverOutPulseOff1;
                    }
                    break;
                case EDevState.LeverOutPulseOff1:
                    if (boolLogicalStateOut == true)
                    {
                        // Mainステート・マシンへ結果を出力: Out完了
                        OpFlagMoveLeverOut = true;
                        OpResultMoveLeverOut = EDeviceResult.Done;
                        devLever.devStateVal = EDevState.LeverStop;
                    }
                    else if (devLever.stopwatchPulse.ElapsedMilliseconds >= preferencesDatOriginal.InactivePulseWidthOfLever)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.LeverOut);
                        if (boolFuncRet != true)
                        {
                            Debug.WriteLine("Lever:State[LeverOutPulseOff1] IoBoardDevice.SetUpperStateOfDOut(LeverOut)エラー");
                        }
                        devLever.stopwatchPulse.Restart();
                        devLever.devStateVal = EDevState.LeverOutPulseOn2;
                    }
                    break;
                case EDevState.LeverOutPulseOn2:
                    if (boolLogicalStateOut == true)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.LeverStop);
                        if (boolFuncRet != true)
                        {
                            Debug.WriteLine("Lever:State[LeverOutPulseOn2] IoBoardDevice.SetUpperStateOfDOut(LeverStop)エラー");
                        }
                        // Mainステート・マシンへ結果を出力: Out完了
                        OpFlagMoveLeverOut = true;
                        OpResultMoveLeverOut = EDeviceResult.Done;
                        devLever.devStateVal = EDevState.LeverStop;
                    }
                    else if (devLever.stopwatchPulse.ElapsedMilliseconds >= preferencesDatOriginal.ActivePulseWidthOfLever)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.LeverStop);
                        if (boolFuncRet != true)
                        {
                            Debug.WriteLine("Lever:State[LeverOutPulseOn2] IoBoardDevice.SetUpperStateOfDOut(LeverStop)エラー");
                        }
                        devLever.stopwatchPulse.Stop();
                        devLever.devStateVal = EDevState.LeverOutDoing;
                    }
                    break;
                case EDevState.LeverOutDoing:
                    if (boolLogicalStateOut == true)
                    {
                        // Mainステート・マシンへ結果を出力: Out完了
                        OpFlagMoveLeverOut = true;
                        OpResultMoveLeverOut = EDeviceResult.Done;
                        devLever.devStateVal = EDevState.LeverStop;
                    }
                    else if (devLever.stopwatchTimeout.ElapsedMilliseconds >= (preferencesDatOriginal.TimeoutOfLeverOut * 1000))
                    {
                        // Openタイムアウトの時
                        if (Properties.Settings.Default.IS_LEVER_RECOVER_ENABLE == true)
                        {
                            // レバー回復処理する時
                            boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.LeverIn);
                            if (boolFuncRet != true)
                            {
                                Debug.WriteLine("Lever:State[LeverOutDoing] IoBoardDevice.SetUpperStateOfDOut(LeverInn)エラー");
                            }
                            // リトライ・カウント: 0クリア
                            devLever.retryCount = 0;
                            devLever.stopwatchPulse.Restart();
                            devLever.stopwatchTimeout.Restart();
                            devLever.devStateVal = EDevState.LeverOutRecoverWithInPulseOn1;
                        }
                        else
                        {
                            // レバー回復処理しない時
                            devLever.stopwatchPulse.Stop();
                            devLever.stopwatchTimeout.Stop();
                            // Mainステート・マシンへ結果を出力: Timeout
                            OpFlagMoveLeverOut = true;
                            OpResultMoveLeverOut = EDeviceResult.TimeOut;
                            devLever.devStateVal = EDevState.LeverStop;
                        }
                    }
                    break;
                case EDevState.LeverOutRecoverInit:
                    if (boolLogicalStateOut == true)
                    {
                        // Mainステート・マシンへ結果を出力: Out完了
                        OpFlagMoveLeverOut = true;
                        OpResultMoveLeverOut = EDeviceResult.Done;
                        devLever.devStateVal = EDevState.LeverStop;
                    }
                    else
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.LeverStop);
                        if (boolFuncRet != true)
                        {
                            Debug.WriteLine("Lever:State[LeverOutRecoverInit] IoBoardDevice.SetUpperStateOfDOut(LeverStop)エラー");
                        }
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.LeverIn);
                        if (boolFuncRet != true)
                        {
                            Debug.WriteLine("Lever:State[LeverOutRecoverInit] IoBoardDevice.SetUpperStateOfDOut(LeverIn)エラー");
                        }
                        devLever.retryCount = 0;
                        devLever.stopwatchPulse.Restart();
                        devLever.devStateVal = EDevState.LeverOutRecoverWithInPulseOn1;
                    }
                    break;
                case EDevState.LeverOutRecoverInitRetry:
                    break;
                case EDevState.LeverOutRecoverWithInPulseOn1:
                    // 現在: Inリミット・センサは見ない
                    if (devLever.stopwatchPulse.ElapsedMilliseconds >= preferencesDatOriginal.ActivePulseWidthOfLever)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.LeverStop);
                        if (boolFuncRet != true)
                        {
                            Debug.WriteLine("Lever:State[LeverOutRecoverWithInPulseOn1] IoBoardDevice.SetUpperStateOfDOut(LeverStop)エラー");
                        }
                        devLever.stopwatchPulse.Restart();
                        devLever.devStateVal = EDevState.LeverOutRecoverWithInPulseOff1;
                    }
                    break;
                case EDevState.LeverOutRecoverWithInPulseOff1:
                    // 現在: Inリミット・センサは見ない
                    if (devLever.stopwatchPulse.ElapsedMilliseconds >= preferencesDatOriginal.InactivePulseWidthOfLever)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.LeverIn);
                        if (boolFuncRet != true)
                        {
                            Debug.WriteLine("Lever:State[LeverOutRecoverWithInPulseOff1] IoBoardDevice.SetUpperStateOfDOut(LeverIn)エラー");
                        }
                        devLever.stopwatchPulse.Restart();
                        devLever.devStateVal = EDevState.LeverOutRecoverWithInPulseOn2;
                    }
                    break;
                case EDevState.LeverOutRecoverWithInPulseOn2:
                    // 現在: Inリミット・センサは見ない
                    if (devLever.stopwatchPulse.ElapsedMilliseconds >= preferencesDatOriginal.ActivePulseWidthOfLever)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.LeverStop);
                        if (boolFuncRet != true)
                        {
                            Debug.WriteLine("Lever:State[LeverOutRecoverWithInPulseOn2] IoBoardDevice.SetUpperStateOfDOut(LeverStop)エラー");
                        }
                        devLever.stopwatchPulse.Stop();
                        // Open動作時間: 開始
                        devLever.stopwatchTimeout.Restart();
                        devLever.devStateVal = EDevState.LeverOutRecoverWithInDoing;
                    }
                    break;
                case EDevState.LeverOutRecoverWithInDoing:
                    if ((boolLogicalStateIn == true) ||
                        (devLever.stopwatchTimeout.ElapsedMilliseconds >= preferencesDatOriginal.LeverInTimeInRecoverOut))
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.LeverOut);
                        if (boolFuncRet != true)
                        {
                            Debug.WriteLine("Lever:State[LeverOutRecoverWithInDoing] IoBoardDevice.SetUpperStateOfDOut(LeverClose)エラー");
                        }
                        devLever.stopwatchPulse.Restart();
                        // Openタイムアウト: 開始
                        devLever.stopwatchTimeout.Restart();
                        devLever.devStateVal = EDevState.LeverOutRecoverWithOutPulseOn1;
                    }
                    break;
                case EDevState.LeverOutRecoverWithOutPulseOn1:
                    if (boolLogicalStateOut == true)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.LeverStop);
                        if (boolFuncRet != true)
                        {
                            Debug.WriteLine("Lever:State[LeverOutRecoverWithOutPulseOn1] IoBoardDevice.SetUpperStateOfDOut(LeverStop)エラー");
                        }
                        // Mainステート・マシンへ結果を出力: Out完了
                        OpFlagMoveLeverOut = true;
                        OpResultMoveLeverOut = EDeviceResult.Done;
                        devLever.stopwatchPulse.Stop();
                        devLever.stopwatchTimeout.Stop();
                        devLever.devStateVal = EDevState.LeverStop;
                    }
                    else if (devLever.stopwatchPulse.ElapsedMilliseconds >= preferencesDatOriginal.ActivePulseWidthOfLever)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.LeverStop);
                        if (boolFuncRet != true)
                        {
                            Debug.WriteLine("Lever:State[LeverOutRecoverWithOutPulseOn1] IoBoardDevice.SetUpperStateOfDOut(LeverStop)エラー");
                        }
                        devLever.stopwatchPulse.Restart();
                        devLever.devStateVal = EDevState.LeverOutRecoverWithOutPulseOff1;
                    }
                    break;
                case EDevState.LeverOutRecoverWithOutPulseOff1:
                    if (boolLogicalStateOut == true)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.LeverStop);
                        if (boolFuncRet != true)
                        {
                            Debug.WriteLine("Lever:State[LeverOutRecoverWithOutPulseOff1] IoBoardDevice.SetUpperStateOfDOut(LeverStop)エラー");
                        }
                        // Mainステート・マシンへ結果を出力: Out完了
                        OpFlagMoveLeverOut = true;
                        OpResultMoveLeverOut = EDeviceResult.Done;
                        devLever.stopwatchPulse.Stop();
                        devLever.stopwatchTimeout.Stop();
                        devLever.devStateVal = EDevState.LeverStop;
                    }
                    else if (devLever.stopwatchPulse.ElapsedMilliseconds >= preferencesDatOriginal.InactivePulseWidthOfLever)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.LeverOut);
                        if (boolFuncRet != true)
                        {
                            Debug.WriteLine("Lever:State[LeverOutRecoverWithOutPulseOff1] IoBoardDevice.SetUpperStateOfDOut(LeverOut)エラー");
                        }
                        devLever.stopwatchPulse.Restart();
                        devLever.devStateVal = EDevState.LeverOutRecoverWithOutPulseOn2;
                    }
                    break;
                case EDevState.LeverOutRecoverWithOutPulseOn2:
                    if (boolLogicalStateOut == true)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.LeverStop);
                        if (boolFuncRet != true)
                        {
                            Debug.WriteLine("Lever:State[LeverOutRecoverWithOutPulseOn2] IoBoardDevice.SetUpperStateOfDOut(LeverStop)エラー");
                        }
                        // Mainステート・マシンへ結果を出力: Out完了
                        OpFlagMoveLeverOut = true;
                        OpResultMoveLeverOut = EDeviceResult.Done;
                        devLever.stopwatchPulse.Stop();
                        devLever.stopwatchTimeout.Stop();
                        devLever.devStateVal = EDevState.LeverStop;
                    }
                    else if (devLever.stopwatchPulse.ElapsedMilliseconds >= preferencesDatOriginal.ActivePulseWidthOfLever)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.LeverStop);
                        if (boolFuncRet != true)
                        {
                            Debug.WriteLine("Lever:State[LeverOutRecoverWithOutPulseOn2] IoBoardDevice.SetUpperStateOfDOut(LeverStop)エラー");
                        }
                        devLever.stopwatchPulse.Stop();
                        devLever.devStateVal = EDevState.LeverOutRecoverWithOutDoing;
                    }
                    break;
                case EDevState.LeverOutRecoverWithOutDoing:
                    if (boolLogicalStateOut == true)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.LeverStop);
                        if (boolFuncRet != true)
                        {
                            Debug.WriteLine("Lever:State[LeverOutRecoverWithOutDoing] IoBoardDevice.SetUpperStateOfDOut(LeverStop)エラー");
                        }
                        // Mainステート・マシンへ結果を出力: Out完了
                        OpFlagMoveLeverOut = true;
                        OpResultMoveLeverOut = EDeviceResult.Done;
                        devLever.stopwatchPulse.Stop();
                        devLever.stopwatchTimeout.Stop();
                        devLever.devStateVal = EDevState.LeverStop;
                    }
                    // タイムアウトはIn方向へ動かした2倍の時間
                    else if (devLever.stopwatchTimeout.ElapsedMilliseconds >= (preferencesDatOriginal.LeverInTimeInRecoverOut * 2))
                    {
                        // タイムアウトの時
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.LeverStop);
                        if (boolFuncRet != true)
                        {
                            Debug.WriteLine("Lever:State[LeverOutRecoverWithOutDoing] IoBoardDevice.SetUpperStateOfDOut(LeverStop)エラー");
                        }
                        devLever.stopwatchTimeout.Stop();
                        // リトライ・カウントアップ 
                        devLever.retryCount++;
                        if (devLever.retryCount > preferencesDatOriginal.LeverRetryNumInRecover)
                        {
                            // リトライ回数に到達した時
                            boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.LeverIn);
                            if (boolFuncRet != true)
                            {
                                Debug.WriteLine("Lever:State[LeverOutRecoverWithOutDoing] IoBoardDevice.SetUpperStateOfDOut(LeverIn)エラー");
                            }
                            devLever.stopwatchPulse.Restart();
                            devLever.stopwatchTimeout.Restart();
                            devLever.devStateVal = EDevState.LeverOutRecoverTimeoutWithInPulseOn1;
                        }
                        else
                        {
                            // リトライ回数までいっていない時
                            boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.LeverIn);
                            if (boolFuncRet != true)
                            {
                                Debug.WriteLine("Lever:State[LeverOutRecoverWithClosing] IoBoardDevice.SetUpperStateOfDOut(LeverIn)エラー");
                            }
                            devLever.stopwatchPulse.Restart();
                            devLever.stopwatchTimeout.Restart();
                            devLever.devStateVal = EDevState.LeverOutRecoverWithInPulseOn1;

                        }
                    }
                    break;
                case EDevState.LeverOutRecoverTimeoutInit:
                    break;
                case EDevState.LeverOutRecoverTimeoutWithInPulseOn1:
                    // 現在: Inリミット・センサは見ない
                    if (devLever.stopwatchPulse.ElapsedMilliseconds >= preferencesDatOriginal.ActivePulseWidthOfLever)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.LeverStop);
                        if (boolFuncRet != true)
                        {
                            Debug.WriteLine("Lever:State[LeverOutRecoverTimeoutWithInPulseOn1] IoBoardDevice.SetUpperStateOfDOut(LeverStop)エラー");
                        }
                        devLever.stopwatchPulse.Restart();
                        devLever.devStateVal = EDevState.LeverOutRecoverTimeoutWithInPulseOff1;
                    }
                    break;
                case EDevState.LeverOutRecoverTimeoutWithInPulseOff1:
                    // 現在: Inリミット・センサは見ない
                    if (devLever.stopwatchPulse.ElapsedMilliseconds >= preferencesDatOriginal.InactivePulseWidthOfLever)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.LeverIn);
                        if (boolFuncRet != true)
                        {
                            Debug.WriteLine("Lever:State[LeverOutRecoverTimeoutWithInPulseOff1] IoBoardDevice.SetUpperStateOfDOut(LeverIn)エラー");
                        }
                        devLever.stopwatchPulse.Restart();
                        devLever.devStateVal = EDevState.LeverOutRecoverTimeoutWithInPulseOn2;
                    }
                    break;
                case EDevState.LeverOutRecoverTimeoutWithInPulseOn2:
                    // 現在: Inリミット・センサは見ない
                    if (devLever.stopwatchPulse.ElapsedMilliseconds >= preferencesDatOriginal.ActivePulseWidthOfLever)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.LeverStop);
                        if (boolFuncRet != true)
                        {
                            Debug.WriteLine("Lever:State[LeverOutRecoverTimeoutWithInPulseOn2] IoBoardDevice.SetUpperStateOfDOut(LeverStop)エラー");
                        }
                        devLever.stopwatchPulse.Stop();
                        devLever.devStateVal = EDevState.LeverOutRecoverTimeoutWithInDoing;
                    }
                    break;
                case EDevState.LeverOutRecoverTimeoutWithInDoing:
                    // In方向へIn時間動かして停止
                    if (devLever.stopwatchTimeout.ElapsedMilliseconds >= preferencesDatOriginal.LeverInTimeInRecoverOut)
                    {
                        devLever.stopwatchPulse.Stop();
                        devLever.stopwatchTimeout.Stop();
                        // Mainステート・マシンへ結果を出力: Timeout
                        OpFlagMoveLeverOut = true;
                        OpResultMoveLeverOut = EDeviceResult.TimeOut;
                        devLever.devStateVal = EDevState.LeverStop;
                    }
                    break;
                #endregion
                default:
                    break;

            }
        }
#else
		private void callbackInitForDevLever()
		{
			// devLever: 初期化
			devLever.devStateVal = DevState.Init;
			devLever.devStateValSaved = DevState.OverRange;
			devLever.devCmdVal = DevCmd.None;
			devLever.devResultVal = DevResult.None;
			devLever.devResultValSaved = DevResult.OverRange;
			devLever.boolFirstFlag = true;
		}
		//		public Action<int> callbackExeForDevLever = (a_iInArg) =>
		private void callbackExeForDevLever()
		{
			bool logicalState;
			bool funcRet;
			DevCmdPkt devCmdPktLast = null;
			DevCmdPkt devCmdPktCur;

			// コマンド受信: 最後のコマンドが有効
			while (concurrentQueueDevCmdPktLever.TryDequeue(out devCmdPktCur))
			{
				devCmdPktLast = devCmdPktCur;
			}
			if (devCmdPktLast != null)
			{
				switch (devCmdPktLast.DevCmdVal)
				{
					case DevCmd.LeverOut:
						devLever.devStateVal = DevState.LeverOutInit;
						break;
					case DevCmd.LeverIn:
						devLever.devStateVal = DevState.LeverInInit;
						break;
					default:
						break;
				}
			}

			switch (devLever.devStateVal)
			{
				case DevState.Init:
					devLever.devStateVal = DevState.LeverStop;
					break;
				case DevState.LeverStop:
					break;
				case DevState.LeverOutInit:
					// 結果: クリア
					devLever.devResultVal = DevResult.None;
					funcRet = IoBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.LeverStop);
					if (funcRet != true)
					{
						Debug.WriteLine("Lever:State[LeverOutInit]LeverStop時、IoBoardDevice.SetUpperStateOfDOut()エラー");
					}
					//					Debug.Assert(funcRet != true, "Lever:State[LeverOutInit]LeverStop時、IoBoardDevice.SetUpperStateOfDOut()エラー");
					funcRet = IoBoardDevice.GetUpperStateOfSaveDIn(IoBoardDInLogicalName.LeverOut, out logicalState);

					if (funcRet != true)
					{
						Debug.WriteLine("Lever:State[LeverOutInit]LeverOutCmd受信時、IoBoardDevice.DirectIn()エラー");
					}
					//					Debug.Assert(funcRet != true, "Lever:State[LeverOutInit]LeverOutCmd受信時、IoBoardDevice.DirectIn()エラー");
#if false
					if (funcRet != true)
					{
						devLever.devStateVal = DevState.LeverError;
						devLever.devResultVal = DevResult.LeverError;
						break;
					}
#endif
					if (logicalState == true)
					{
						// Outセンサ: onの時
						devLever.devStateVal = DevState.LeverStop;
						devLever.devResultVal = DevResult.LeverOutDone;
						// Mainステート・マシンへ結果を出力: Out正常完了
						OpFlagMoveLeverOut = true;
						OpResultMoveLeverOut = DeviceResult.Done;
					}
					else
					{
						// Outセンサ: offの時
						funcRet = IoBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.LeverOut);
						if (funcRet != true)
						{
							Debug.WriteLine("Lever:State[LeverOutInit]LeverOutCmd受信時、IoBoardDevice.DirectIn()エラー");
						}
						//						Debug.Assert(funcRet != true, "Lever:State[LeverOutInit]LeverOutCmd受信時、IoBoardDevice.DirectIn()エラー");
#if false
								if (funcRet != true)
								{
									devLever.devStateVal = DevState.LeverError;
									devLever.devResultVal = DevResult.LeverError;
									break;
								}
#endif
						devLever.iPulseTimeCount = GetTimerCountFromInterval(PreferencesDatOriginal.iOutPulseOfLeverOut);   // 200[ms]
						devLever.iTimeoutCount = GetTimerCountFromInterval(PreferencesDatOriginal.iTimeoutOfLeverOut * 1000);   // 6[s]
						devLever.devStateVal = DevState.LeverOutPulseOn;
					}
					break;
				case DevState.LeverOutPulseOn:
					funcRet = IoBoardDevice.GetUpperStateOfSaveDIn(IoBoardDInLogicalName.LeverOut, out logicalState);
					if (funcRet != true)
					{
						Debug.WriteLine("Lever:State[LeverOutPulseOn] IoBoardDevice.GetUpperStateOfSaveDIn()エラー");
					}
					//					Debug.Assert(funcRet != true, "Lever:State[LeverOutPulseOn] IoBoardDevice.GetUpperStateOfSaveDIn()エラー");
#if false
					if (funcRet != true)
					{
						devLever.devStateVal = DevState.LeverError;
						devLever.devResultVal = DevResult.LeverError;
						break;
					}
#endif
					if (logicalState == true)
					{
						// Outセンサ: onの時
						funcRet = IoBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.LeverOut);
						if (funcRet != true)
						{
							Debug.WriteLine("Lever:State[LeverOutPulseOn]Outセンサ==on時、IoBoardDevice.SetUpperStateOfDOut()エラー");
						}
						//						Debug.Assert(funcRet != true, "Lever:State[LeverOutPulseOn]Outセンサ==on時、IoBoardDevice.SetUpperStateOfDOut()エラー");
#if false
						if (funcRet != true)
						{
							devLever.devStateVal = DevState.LeverError;
							devLever.devResultVal = DevResult.LeverError;
							break;
						}
#endif
						devLever.devStateVal = DevState.LeverStop;
						devLever.devResultVal = DevResult.LeverOutDone;
						// Mainステート・マシンへ結果を出力: Out正常完了
						OpFlagMoveLeverOut = true;
						OpResultMoveLeverOut = DeviceResult.Done;
					}
					else
					{
						// Outセンサ: offの時
						devLever.iTimeoutCount--;
						if (devLever.iTimeoutCount <= 0)
						{
							funcRet = IoBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.LeverStop);
							if (funcRet != true)
							{
								Debug.WriteLine("Lever:State[LeverOutPulseOn]Outセンサ==off→タイムアウト時、IoBoardDevice.SetUpperStateOfDOut()エラー");
							}
							//							Debug.Assert(funcRet != true, "Lever:State[LeverOutPulseOn]Outセンサ==off→タイムアウト時、IoBoardDevice.SetUpperStateOfDOut()エラー");
#if false
							if (funcRet != true)
							{
								devLever.devStateVal = DevState.LeverError;
								devLever.devResultVal = DevResult.LeverError;
								break;
							}
#endif
							devLever.devStateVal = DevState.LeverStop;
							devLever.devResultVal = DevResult.LeverOutTimeout;
							// Mainステート・マシンへ結果を出力: Outタイムアウト
							OpFlagMoveLeverOut = true;
							OpResultMoveLeverOut = DeviceResult.TimeOut;
							break;
						}
						devLever.iPulseTimeCount--;
						if (devLever.iPulseTimeCount <= 0)
						{
							funcRet = IoBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.LeverStop);
							if (funcRet != true)
							{
								Debug.WriteLine("Lever:State[LeverOutPulseOn]Outセンサ==off→パルスカウント満時、IoBoardDevice.SetUpperStateOfDOut()エラー");
							}
							//							Debug.Assert(funcRet != true, "Lever:State[LeverOutPulseOn]Outセンサ==off→パルスカウント満時、IoBoardDevice.SetUpperStateOfDOut()エラー");
#if false
							if (funcRet != true)
							{
								devLever.devStateVal = DevState.LeverError;
								devLever.devResultVal = DevResult.LeverError;
								break;
							}
#endif
							devLever.devStateVal = DevState.LeverOut;
						}
					}
					break;
				case DevState.LeverOut:
					funcRet = IoBoardDevice.GetUpperStateOfSaveDIn(IoBoardDInLogicalName.LeverOut, out logicalState);
					if (funcRet != true)
					{
						Debug.WriteLine("Lever:State[LeverOpenning]IoBoardDevice.GetUpperStateOfSaveDIn()エラー");
					}
#if false
					if (funcRet != true)
					{
						devLever.devStateVal = DevState.LeverError;
						devLever.devResultVal = DevResult.LeverError;
						break;
					}
#endif
					if (logicalState == true)
					{
						// Openセンサ: onの時
						funcRet = IoBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.LeverStop);
						if (funcRet != true)
						{
							Debug.WriteLine("Lever:State[LeverOut]Outセンサ==on時、IoBoardDevice.SetUpperStateOfDOut()エラー");
						}
#if false
						if (funcRet != true)
						{
							devLever.devStateVal = DevState.LeverError;
							devLever.devResultVal = DevResult.LeverrError;
							break;
						}
#endif
						devLever.devStateVal = DevState.LeverStop;
						devLever.devResultVal = DevResult.LeverOutDone;
						// Mainステート・マシンへ結果を出力: Out正常完了
						OpFlagMoveLeverOut = true;
						OpResultMoveLeverOut = DeviceResult.Done;
					}
					else
					{
						// Outセンサ: offの時
						devLever.iTimeoutCount--;
						if (devLever.iTimeoutCount <= 0)
						{
							funcRet = IoBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.LeverStop);
							if (funcRet != true)
							{
								Debug.WriteLine("Lever:State[LeverOut]Outセンサ==off時、IoBoardDevice.SetUpperStateOfDOut()エラー");
							}
#if false
							if (funcRet != true)
							{
								devLever.devStateVal = DevState.LeverError;
								devLever.devResultVal = DevResult.LeverError;
								break;
							}
#endif

							devLever.devStateVal = DevState.LeverStop;
							devLever.devResultVal = DevResult.LeverOutTimeout;
							// Mainステート・マシンへ結果を出力: Outタイムアウト
							OpFlagMoveLeverOut = true;
							OpResultMoveLeverOut = DeviceResult.TimeOut;
							break;
						}
					}
					break;
				case DevState.LeverInInit:
					// 結果: クリア
					devLever.devResultVal = DevResult.None;
					funcRet = IoBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.LeverStop);
					if (funcRet != true)
					{
						Debug.WriteLine("Lever:State[DevLever.LeverInInit]LeverStop時、IoBoardDevice.SetUpperStateOfDOut()エラー");
					}
					//					Debug.Assert(funcRet != true, "Lever:State[DevLever.LeverInInit]LeverStop時、IoBoardDevice.SetUpperStateOfDOut()エラー");
					funcRet = IoBoardDevice.GetUpperStateOfSaveDIn(IoBoardDInLogicalName.LeverIn, out logicalState);
					if (funcRet != true)
					{
						Debug.WriteLine("Lever:State[LeverInInit] IoBoardDevice.DirectIn()エラー");
					}
					//					Debug.Assert(funcRet != true, "Lever:State[LeverInInit] IoBoardDevice.DirectIn()エラー");
#if false
							if (funcRet != true)
							{
								devLever.devStateVal = DevState.LeverError;
								devLever.devResultVal = DevResult.LeverError;
								break;
							}
#endif
					if (logicalState == true)
					{
						// Inセンサ: onの時
						devLever.devStateVal = DevState.LeverStop;
						devLever.devResultVal = DevResult.LeverInDone;
						// Mainステート・マシンへ結果を出力: In正常完了
						OpFlagMoveLeverIn = true;
						OpResultMoveLeverIn = DeviceResult.Done;
					}
					else
					{
						// Inセンサ: offの時
						funcRet = IoBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.LeverIn);
						if (funcRet != true)
						{
							Debug.WriteLine("Lever:State[LeverInInit]Inセンサ==off時、IoBoardDevice.SetUpperStateOfDOut()エラー");
						}
						//						Debug.Assert(funcRet != true, "Lever:State[LeverInInit]Inセンサ==off時、IoBoardDevice.SetUpperStateOfDOut()エラー");
#if false
								if (funcRet != true)
								{
									devLever.devStateVal = DevState.LeverError;
									devLever.devResultVal = DevResult.LeverError;
									break;
								}
#endif

						devLever.iPulseTimeCount = GetTimerCountFromInterval(PreferencesDatOriginal.iOutPulseOfLeverIn);   // 200[ms]
						devLever.iTimeoutCount = GetTimerCountFromInterval(PreferencesDatOriginal.iTimeoutOfLeverIn * 1000);   // 6[s]
						devLever.devStateVal = DevState.LeverInPulseOn;
					}
					break;
				case DevState.LeverInPulseOn:
					funcRet = IoBoardDevice.GetUpperStateOfSaveDIn(IoBoardDInLogicalName.LeverIn, out logicalState);
					if (funcRet != true)
					{
						Debug.WriteLine("Lever:State[LeverInPulseOn] IoBoardDevice.GetUpperStateOfSaveDIn()エラー");
					}
					//					Debug.Assert(funcRet != true, "Lever:State[LeverInPulseOn] IoBoardDevice.GetUpperStateOfSaveDIn()エラー");
					if (logicalState == true)
					{
						// Inセンサ: onの時
						funcRet = IoBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.LeverStop);
						if (funcRet != true)
						{
							Debug.WriteLine("Lever:State[LeverInPulseOn]Openセンサ==on時、IoBoardDevice.SetUpperStateOfDOut()エラー");
						}
						//						Debug.Assert(funcRet != true, "Lever:State[LeverInPulseOn]Openセンサ==on時、IoBoardDevice.SetUpperStateOfDOut()エラー");
						devLever.devStateVal = DevState.LeverStop;
						devLever.devResultVal = DevResult.LeverInDone;
						// Mainステート・マシンへ結果を出力: In正常完了
						OpFlagMoveLeverIn = true;
						OpResultMoveLeverIn = DeviceResult.Done;
					}
					else
					{
						// Inセンサ: offの時
						devLever.iTimeoutCount--;
						if (devLever.iTimeoutCount <= 0)
						{
							funcRet = IoBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.LeverStop);
							if (funcRet != true)
							{
								Debug.WriteLine("Lever:State[LeverInPulseOn]Closeセンサ==off→タイムアウト時、IoBoardDevice.SetUpperStateOfDOut()エラー");
							}
							//							Debug.Assert(funcRet != true, "Lever:State[LeverInPulseOn]Closeセンサ==off→タイムアウト時、IoBoardDevice.SetUpperStateOfDOut()エラー");
							devLever.devStateVal = DevState.LeverStop;
							devLever.devResultVal = DevResult.LeverInTimeout;
							// Mainステート・マシンへ結果を出力: Inタイムアウト
							OpFlagMoveLeverIn = true;
							OpResultMoveLeverIn = DeviceResult.TimeOut;
							break;
						}
						devLever.iPulseTimeCount--;
						if (devLever.iPulseTimeCount <= 0)
						{
							funcRet = IoBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.LeverStop);
							if (funcRet != true)
							{
								Debug.WriteLine("lever:State[LeverInPulseOn]Inセンサ==off→パルスカウント・フル時、IoBoardDevice.SetUpperStateOfDOut()エラー");
							}
							//							Debug.Assert(funcRet != true, "lever:State[LeverInPulseOn]Inセンサ==off→パルスカウント・フル時、IoBoardDevice.SetUpperStateOfDOut()エラー");
							devLever.devStateVal = DevState.LeverIn;
						}
					}
					break;
				case DevState.LeverIn:
					funcRet = IoBoardDevice.GetUpperStateOfSaveDIn(IoBoardDInLogicalName.LeverIn, out logicalState);
					if (funcRet != true)
					{
						Debug.WriteLine("Lever:State[LeverIn]IoBoardDevice.GetUpperStateOfSaveDIn()エラー");
					}
					if (logicalState == true)
					{
						// Inセンサ: onの時
						funcRet = IoBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.LeverStop);
						if (funcRet != true)
						{
							Debug.WriteLine("Lever:State[LeverIn]Inセンサ==on時、IoBoardDevice.SetUpperStateOfDOut()エラー");
						}
						devLever.devStateVal = DevState.LeverStop;
						devLever.devResultVal = DevResult.LeverInDone;
						// Mainステート・マシンへ結果を出力: In正常完了
						OpFlagMoveLeverIn = true;
						OpResultMoveLeverIn = DeviceResult.Done;
					}
					else
					{
						// Inセンサ: offの時
						devLever.iTimeoutCount--;
						if (devLever.iTimeoutCount <= 0)
						{
							funcRet = IoBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.LeverStop);
							if (funcRet != true)
							{
								Debug.WriteLine("Lever:State[LeverIn]Inセンサ==off時、IoBoardDevice.SetUpperStateOfDOut()エラー");
							}
							devLever.devStateVal = DevState.LeverStop;
							devLever.devResultVal = DevResult.LeverInTimeout;
							// Mainステート・マシンへ結果を出力: Inタイムアウト
							OpFlagMoveLeverIn = true;
							OpResultMoveLeverIn = DeviceResult.TimeOut;

							break;
						}
					}
					break;
				case DevState.LeverInTimeout:
				case DevState.LeverOutTimeout:
				case DevState.LeverError:
					funcRet = IoBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.LeverStop);
					if (funcRet != true)
					{
						Debug.WriteLine("Lever:State[LeverTimeout/LeverError] IoBoardDevice.SetUpperStateOfDOut()エラー");
					}
					devLever.devStateVal = DevState.LeverStop;
					break;
				default:
					break;
			}
		}
#endif

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
                    //				userControlCheckDeviceOnFormMain.textBoxDoutLeverStateOnGpLeverOnUcCheckDevice.Text = devLever.devStateVal.ToString();
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