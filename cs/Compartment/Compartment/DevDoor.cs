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
        /// ドアの回復処理を行うか否かの設定
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

#if true
        private void CallbackInitForDevDoor()
        {
            // devDoor: 初期化
            devDoor.devStateVal = EDevState.DoorInit;
            devDoor.devStateValSaved = EDevState.OverRange;
            devDoor.devCmdVal = EDevCmd.None;
            devDoor.devResultVal = EDevResult.None;
            devDoor.devResultValSaved = EDevResult.OverRange;
            devDoor.firstFlag = true;
        }
        private void CallbackExeForDevDoor()
        {
            bool boolLogicalStateOpen;
            bool boolLogicalStateClose;
            bool boolFuncRet;
            DevCmdPkt devCmdPktLast = null;
            DevCmdPkt devCmdPktCur;
            EDevCmd devCmdCur = EDevCmd.None;

            boolFuncRet = ioBoardDevice.GetUpperStateOfSaveDIn(IoBoardDInLogicalName.DoorOpen, out boolLogicalStateOpen);
            if (boolFuncRet != true)
            {
                Debug.WriteLine("Door: IoBoardDevice.DirectIn(DoorOpen)エラー");
            }
            boolFuncRet = ioBoardDevice.GetUpperStateOfSaveDIn(IoBoardDInLogicalName.DoorClose, out boolLogicalStateClose);
            if (boolFuncRet != true)
            {
                Debug.WriteLine("Door: IoBoardDevice.DirectIn(DoorClose)エラー");
            }
            // コマンド受信: 最後のコマンドが有効
            while (concurrentQueueDevCmdPktDoor.TryDequeue(out devCmdPktCur))
            {
                devCmdPktLast = devCmdPktCur;
            }
            if (devCmdPktLast != null)
            {
                switch (devCmdPktLast.DevCmdVal)
                {
                    case EDevCmd.DoorOpen:
                    case EDevCmd.DoorClose:
                    case EDevCmd.DoorStop:
                        devCmdCur = devCmdPktLast.DevCmdVal;
                        break;
                    default:
                        break;
                }
            }
            switch (devDoor.devStateVal)
            {
                case EDevState.DoorInit:
                case EDevState.DoorStop:
                    switch (devCmdCur)
                    {
                        case EDevCmd.DoorStop:
                            // Mainステート・マシンへ結果を出力: Stop正常完了
                            OpFlagStopDoor = true;
                            OpResultStopDoor = EDeviceResult.Done;
                            if (opeModeTypeVal == EOpeModeType.CheckDevice)
                            {
                                // CheckDevice時、Mainステート・マシンの代わりに結果クリア
                                OpFlagOpenDoor = false;
                                OpResultOpenDoor = EDeviceResult.None;
                                OpFlagCloseDoor = false;
                                OpResultCloseDoor = EDeviceResult.None;
                            }
                            break;
                        case EDevCmd.DoorOpen:
                            boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.DoorStop);
                            if (boolFuncRet != true)
                            {
                                Debug.WriteLine("Cmd.DoorOpen[InStop]、IoBoardDevice.SetUpperStateOfDOut()エラー");
                            }
                            devDoor.devStateVal = EDevState.DoorOpenInit;
                            if (opeModeTypeVal == EOpeModeType.CheckDevice)
                            {
                                // CheckDevice時、Mainステート・マシンの代わりに結果クリア
                                OpFlagStopDoor = false;
                                OpResultStopDoor = EDeviceResult.None;
                                OpFlagOpenDoor = false;
                                OpResultOpenDoor = EDeviceResult.None;
                                OpFlagCloseDoor = false;
                                OpResultCloseDoor = EDeviceResult.None;
                            }
                            break;
                        case EDevCmd.DoorClose:
                            boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.DoorStop);
                            if (boolFuncRet != true)
                            {
                                Debug.WriteLine("Cmd.DoorClose[InStop]、IoBoardDevice.SetUpperStateOfDOut()エラー");
                            }
                            devDoor.devStateVal = EDevState.DoorCloseInit;
                            if (opeModeTypeVal == EOpeModeType.CheckDevice)
                            {
                                // CheckDevice時、Mainステート・マシンの代わりに結果クリア
                                OpFlagStopDoor = false;
                                OpResultStopDoor = EDeviceResult.None;
                                OpFlagOpenDoor = false;
                                OpResultOpenDoor = EDeviceResult.None;
                                OpFlagCloseDoor = false;
                                OpResultCloseDoor = EDeviceResult.None;
                            }
                            break;
                        default:
                            break;
                    }
                    break;
                case EDevState.DoorStopInitAfterWait:
                case EDevState.DoorStopInitAfterWaitPulseOff:
                case EDevState.DoorStopInit:
                case EDevState.DoorStopWithOpenPulseOn1:
                case EDevState.DoorStopWithOpenPulseOff1:
                case EDevState.DoorStopWithOpenPulseOn2:
                case EDevState.DoorStopWithClosePulseOff2:
                case EDevState.DoorStopWithClosePulseOn:
                    switch (devCmdCur)
                    {
                        case EDevCmd.DoorOpen:
                            boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.DoorStop);
                            if (boolFuncRet != true)
                            {
                                Debug.WriteLine("Cmd.DoorOpen[InStop]、IoBoardDevice.SetUpperStateOfDOut()エラー");
                            }
                            devDoor.devStateVal = EDevState.DoorOpenInitAfterWait;
                            if (opeModeTypeVal == EOpeModeType.CheckDevice)
                            {
                                // CheckDevice時、Mainステート・マシンの代わりに結果クリア
                                OpFlagStopDoor = false;
                                OpResultStopDoor = EDeviceResult.None;
                                OpFlagOpenDoor = false;
                                OpResultOpenDoor = EDeviceResult.None;
                                OpFlagCloseDoor = false;
                                OpResultCloseDoor = EDeviceResult.None;
                            }
                            break;
                        case EDevCmd.DoorClose:
                            boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.DoorStop);
                            if (boolFuncRet != true)
                            {
                                Debug.WriteLine("Cmd.DoorClose[InStop]、IoBoardDevice.SetUpperStateOfDOut()エラー");
                            }
                            devDoor.devStateVal = EDevState.DoorCloseInitAfterWait;
                            if (opeModeTypeVal == EOpeModeType.CheckDevice)
                            {
                                // CheckDevice時、Mainステート・マシンの代わりに結果クリア
                                OpFlagStopDoor = false;
                                OpResultStopDoor = EDeviceResult.None;
                                OpFlagOpenDoor = false;
                                OpResultOpenDoor = EDeviceResult.None;
                                OpFlagCloseDoor = false;
                                OpResultCloseDoor = EDeviceResult.None;
                            }
                            break;
                        default:
                            break;
                    }
                    break;
                case EDevState.DoorOpenInitAfterWait:
                case EDevState.DoorOpenInitAfterWaitPulseOff:
                case EDevState.DoorOpenInit:
                case EDevState.DoorOpenPulseOn1:
                case EDevState.DoorOpenPulseOff1:
                case EDevState.DoorOpenPulseOn2:
                case EDevState.DoorOpenDoing:
                case EDevState.DoorOpenRecoverInit:
                case EDevState.DoorOpenRecoverInitRetry:
                case EDevState.DoorOpenRecoverWithClosePulseOn1:
                case EDevState.DoorOpenRecoverWithClosePulseOff1:
                case EDevState.DoorOpenRecoverWithClosePulseOn2:
                case EDevState.DoorOpenRecoverWithClosing:
                case EDevState.DoorOpenRecoverWithOpenPulseOn1:
                case EDevState.DoorOpenRecoverWithOpenPulseOff1:
                case EDevState.DoorOpenRecoverWithOpenPulseOn2:
                case EDevState.DoorOpenRecoverWithOpenning:
                case EDevState.DoorOpenRecoverTimeoutInit:
                case EDevState.DoorOpenRecoverTimeoutWithClosePulseOn1:
                case EDevState.DoorOpenRecoverTimeoutWithClosePulseOff1:
                case EDevState.DoorOpenRecoverTimeoutWithClosePulseOn2:
                case EDevState.DoorOpenRecoverTimeoutWithClosing:
                    switch (devCmdCur)
                    {
                        case EDevCmd.DoorClose:
                            boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.DoorStop);
                            if (boolFuncRet != true)
                            {
                                Debug.WriteLine("Cmd.DoorClose[InStop]、IoBoardDevice.SetUpperStateOfDOut()エラー");
                            }
                            devDoor.devStateVal = EDevState.DoorCloseInitAfterWait;
                            if (opeModeTypeVal == EOpeModeType.CheckDevice)
                            {
                                // CheckDevice時、Mainステート・マシンの代わりに結果クリア
                                OpFlagStopDoor = false;
                                OpResultStopDoor = EDeviceResult.None;
                                OpFlagOpenDoor = false;
                                OpResultOpenDoor = EDeviceResult.None;
                                OpFlagCloseDoor = false;
                                OpResultCloseDoor = EDeviceResult.None;
                            }
                            break;
                        case EDevCmd.DoorStop:
                            boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.DoorStop);
                            if (boolFuncRet != true)
                            {
                                Debug.WriteLine("Cmd.DoorClose[InStop]、IoBoardDevice.SetUpperStateOfDOut()エラー");
                            }
                            devDoor.devStateVal = EDevState.DoorStopInitAfterWait;
                            if (opeModeTypeVal == EOpeModeType.CheckDevice)
                            {
                                // CheckDevice時、Mainステート・マシンの代わりに結果クリア
                                OpFlagStopDoor = false;
                                OpResultStopDoor = EDeviceResult.None;
                                OpFlagOpenDoor = false;
                                OpResultOpenDoor = EDeviceResult.None;
                                OpFlagCloseDoor = false;
                                OpResultCloseDoor = EDeviceResult.None;
                            }
                            break;
                        default:
                            break;
                    }
                    break;
                case EDevState.DoorCloseInitAfterWait:
                case EDevState.DoorCloseInitAfterWaitPulseOff:
                case EDevState.DoorCloseInit:
                case EDevState.DoorClosePulseOn1:
                case EDevState.DoorClosePulseOff1:
                case EDevState.DoorClosePulseOn2:
                case EDevState.DoorCloseDoing:
                case EDevState.DoorCloseRecoverInit:
                case EDevState.DoorCloseRecoverInitRetry:
                case EDevState.DoorCloseRecoverWithOpenPulseOn1:
                case EDevState.DoorCloseRecoverWithOpenPulseOff1:
                case EDevState.DoorCloseRecoverWithOpenPulseOn2:
                case EDevState.DoorCloseRecoverWithOpenning:
                case EDevState.DoorCloseRecoverWithClosePulseOn1:
                case EDevState.DoorCloseRecoverWithClosePulseOff1:
                case EDevState.DoorCloseRecoverWithClosePulseOn2:
                case EDevState.DoorCloseRecoverWithClosing:
                case EDevState.DoorCloseRecoverTimeoutInit:
                case EDevState.DoorCloseRecoverTimeoutWithOpenPulseOn1:
                case EDevState.DoorCloseRecoverTimeoutWithOpenPulseOff1:
                case EDevState.DoorCloseRecoverTimeoutWithOpenPulseOn2:
                case EDevState.DoorCloseRecoverTimeoutWithOpenning:
                    switch (devCmdCur)
                    {
                        case EDevCmd.DoorOpen:
                            boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.DoorStop);
                            if (boolFuncRet != true)
                            {
                                Debug.WriteLine("Cmd.DoorOpen[InStop]、IoBoardDevice.SetUpperStateOfDOut()エラー");
                            }
                            devDoor.devStateVal = EDevState.DoorOpenInitAfterWait;
                            if (opeModeTypeVal == EOpeModeType.CheckDevice)
                            {
                                // CheckDevice時、Mainステート・マシンの代わりに結果クリア
                                OpFlagStopDoor = false;
                                OpResultStopDoor = EDeviceResult.None;
                                OpFlagOpenDoor = false;
                                OpResultOpenDoor = EDeviceResult.None;
                                OpFlagCloseDoor = false;
                                OpResultCloseDoor = EDeviceResult.None;
                            }
                            break;
                        case EDevCmd.DoorStop:
                            boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.DoorStop);
                            if (boolFuncRet != true)
                            {
                                Debug.WriteLine("Cmd.DoorClose[InStop]、IoBoardDevice.SetUpperStateOfDOut()エラー");
                            }
                            devDoor.devStateVal = EDevState.DoorStopInitAfterWait;
                            if (opeModeTypeVal == EOpeModeType.CheckDevice)
                            {
                                // CheckDevice時、Mainステート・マシンの代わりに結果クリア
                                OpFlagStopDoor = false;
                                OpResultStopDoor = EDeviceResult.None;
                                OpFlagOpenDoor = false;
                                OpResultOpenDoor = EDeviceResult.None;
                                OpFlagCloseDoor = false;
                                OpResultCloseDoor = EDeviceResult.None;
                            }
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }
            switch (devDoor.devStateVal)
            {
                case EDevState.DoorInit:
                    devDoor.devStateVal = EDevState.DoorStop;
                    break;
                case EDevState.DoorStop:
                    break;
                case EDevState.DoorStopInitAfterWait:
                    boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.DoorStop);
                    if (boolFuncRet != true)
                    {
                        Debug.WriteLine("Door:State[DoorStopInitAfterWait] IoBoardDevice.SetUpperStateOfDOut(DoorStop)エラー");
                    }
                    if ((boolLogicalStateOpen == true) ||
                        (boolLogicalStateClose == true))
                    {
                        // Mainステート・マシンへ結果を出力: Stop正常完了
                        OpFlagStopDoor = true;
                        OpResultStopDoor = EDeviceResult.Done;
                        devDoor.devStateVal = EDevState.DoorStop;
                    }
                    else
                    {
                        devDoor.stopwatchPulse.Restart();
                        devDoor.devStateVal = EDevState.DoorStopInitAfterWaitPulseOff;
                    }
                    break;
                case EDevState.DoorStopInitAfterWaitPulseOff:
                    if ((boolLogicalStateOpen == true) ||
                        (boolLogicalStateClose == true))
                    {
                        // Mainステート・マシンへ結果を出力: Stop正常完了
                        OpFlagStopDoor = true;
                        OpResultStopDoor = EDeviceResult.Done;
                        devDoor.devStateVal = EDevState.DoorStop;
                    }
                    else if (devDoor.stopwatchPulse.ElapsedMilliseconds >= preferencesDatOriginal.InactivePulseWidthOfDoor)
                    {
                        devDoor.stopwatchPulse.Stop();
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.DoorOpen);
                        if (boolFuncRet != true)
                        {
                            Debug.WriteLine("Door:State[DoorStopInitAfterWaitPulseOff] IoBoardDevice.SetUpperStateOfDOut(DoorOpen)エラー");
                        }
                        devDoor.stopwatchPulse.Restart();
                        devDoor.devStateVal = EDevState.DoorStopWithOpenPulseOn1;
                    }
                    break;
                case EDevState.DoorStopInit:
                    boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.DoorStop);
                    if (boolFuncRet != true)
                    {
                        Debug.WriteLine("Door:State[DoorStopInit] IoBoardDevice.SetUpperStateOfDOut(DoorStop)エラー");
                    }
                    if ((boolLogicalStateOpen == true) ||
                        (boolLogicalStateClose == true))
                    {
                        // Mainステート・マシンへ結果を出力: Stop正常完了
                        OpFlagStopDoor = true;
                        OpResultStopDoor = EDeviceResult.Done;
                        devDoor.devStateVal = EDevState.DoorStop;
                    }
                    else
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.DoorOpen);
                        if (boolFuncRet != true)
                        {
                            Debug.WriteLine("Door:State[DoorStopInit] IoBoardDevice.SetUpperStateOfDOut(DoorOpen)エラー");
                        }
                        devDoor.stopwatchPulse.Restart();
                        devDoor.devStateVal = EDevState.DoorStopWithOpenPulseOn1;
                    }
                    break;
                case EDevState.DoorStopWithOpenPulseOn1:
                    if ((boolLogicalStateOpen == true) ||
                        (boolLogicalStateClose == true))
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.DoorStop);
                        if (boolFuncRet != true)
                        {
                            Debug.WriteLine("Door:State[DoorStopWithOpenPulseOn1] IoBoardDevice.SetUpperStateOfDOut(DoorStop)エラー");
                        }
                        // Mainステート・マシンへ結果を出力: Stop正常完了
                        OpFlagStopDoor = true;
                        OpResultStopDoor = EDeviceResult.Done;
                        devDoor.devStateVal = EDevState.DoorStop;
                    }
                    else if (devDoor.stopwatchPulse.ElapsedMilliseconds >= preferencesDatOriginal.ActivePulseWidthOfDoor)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.DoorStop);
                        if (boolFuncRet != true)
                        {
                            Debug.WriteLine("Door:State[DoorStopWithOpenPulseOn1] IoBoardDevice.SetUpperStateOfDOut(DoorStop)エラー");
                        }
                        devDoor.stopwatchPulse.Restart();
                        devDoor.devStateVal = EDevState.DoorStopWithOpenPulseOff1;
                    }
                    break;
                case EDevState.DoorStopWithOpenPulseOff1:
                    if ((boolLogicalStateOpen == true) ||
                        (boolLogicalStateClose == true))
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.DoorStop);
                        if (boolFuncRet != true)
                        {
                            Debug.WriteLine("Door:State[DoorStopWithOpenPulseOff1] IoBoardDevice.SetUpperStateOfDOut(DoorStop)エラー");
                        }
                        // Mainステート・マシンへ結果を出力: Stop正常完了
                        OpFlagStopDoor = true;
                        OpResultStopDoor = EDeviceResult.Done;
                        devDoor.devStateVal = EDevState.DoorStop;
                    }
                    else if (devDoor.stopwatchPulse.ElapsedMilliseconds >= preferencesDatOriginal.InactivePulseWidthOfDoor)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.DoorOpen);
                        if (boolFuncRet != true)
                        {
                            Debug.WriteLine("Door:State[DoorStopWithOpenPulseOff1] IoBoardDevice.SetUpperStateOfDOut(DoorOpen)エラー");
                        }
                        devDoor.stopwatchPulse.Restart();
                        devDoor.devStateVal = EDevState.DoorStopWithOpenPulseOn2;
                    }
                    break;
                case EDevState.DoorStopWithOpenPulseOn2:
                    if ((boolLogicalStateOpen == true) ||
                        (boolLogicalStateClose == true))
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.DoorStop);
                        if (boolFuncRet != true)
                        {
                            Debug.WriteLine("Door:State[DoorStopWithOpenPulseOn2] IoBoardDevice.SetUpperStateOfDOut(DoorStop)エラー");
                        }
                        // Mainステート・マシンへ結果を出力: Stop正常完了
                        OpFlagStopDoor = true;
                        OpResultStopDoor = EDeviceResult.Done;
                        devDoor.devStateVal = EDevState.DoorStop;
                    }
                    else if (devDoor.stopwatchPulse.ElapsedMilliseconds >= preferencesDatOriginal.ActivePulseWidthOfDoor)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.DoorStop);
                        if (boolFuncRet != true)
                        {
                            Debug.WriteLine("Door:State[DoorStopWithOpenPulseOn2] IoBoardDevice.SetUpperStateOfDOut(DoorStop)エラー");
                        }
                        devDoor.stopwatchPulse.Restart();
                        devDoor.devStateVal = EDevState.DoorStopWithClosePulseOff2;
                    }
                    break;
                case EDevState.DoorStopWithClosePulseOff2:
                    if ((boolLogicalStateOpen == true) ||
                        (boolLogicalStateClose == true))
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.DoorStop);
                        if (boolFuncRet != true)
                        {
                            Debug.WriteLine("Door:State[DoorStopWithClosePulseOff2] IoBoardDevice.SetUpperStateOfDOut(DoorStop)エラー");
                        }
                        // Mainステート・マシンへ結果を出力: Stop正常完了
                        OpFlagStopDoor = true;
                        OpResultStopDoor = EDeviceResult.Done;
                        devDoor.devStateVal = EDevState.DoorStop;
                    }
                    else if (devDoor.stopwatchPulse.ElapsedMilliseconds >= preferencesDatOriginal.InactivePulseWidthOfDoor)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.DoorClose);
                        if (boolFuncRet != true)
                        {
                            Debug.WriteLine("Door:State[DoorStopWithClosePulseOff2] IoBoardDevice.SetUpperStateOfDOut(DoorClose)エラー");
                        }
                        devDoor.stopwatchPulse.Restart();
                        devDoor.devStateVal = EDevState.DoorStopWithClosePulseOn;
                    }
                    break;
                case EDevState.DoorStopWithClosePulseOn:
                    if ((boolLogicalStateOpen == true) ||
                        (boolLogicalStateClose == true))
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.DoorStop);
                        if (boolFuncRet != true)
                        {
                            Debug.WriteLine("Door:State[DoorStopWithClosePulseOn] IoBoardDevice.SetUpperStateOfDOut(DoorStop)エラー");
                        }
                        // Mainステート・マシンへ結果を出力: Stop正常完了
                        OpFlagStopDoor = true;
                        OpResultStopDoor = EDeviceResult.Done;
                        devDoor.devStateVal = EDevState.DoorStop;
                    }
                    else if (devDoor.stopwatchPulse.ElapsedMilliseconds >= preferencesDatOriginal.ActivePulseWidthOfDoor)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.DoorStop);
                        if (boolFuncRet != true)
                        {
                            Debug.WriteLine("Door:State[DoorStopWithClosePulseOn] IoBoardDevice.SetUpperStateOfDOut(DoorStop)エラー");
                        }
                        // Mainステート・マシンへ結果を出力: Stop正常完了
                        OpFlagStopDoor = true;
                        OpResultStopDoor = EDeviceResult.Done;
                        devDoor.devStateVal = EDevState.DoorStop;
                        devDoor.stopwatchPulse.Stop();
                    }
                    break;
                #region DoorOpen
                case EDevState.DoorOpenInitAfterWait:
                    boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.DoorStop);
                    if (boolFuncRet != true)
                    {
                        Debug.WriteLine("Door:State[DoorOpenInitAfterWait] IoBoardDevice.SetUpperStateOfDOut(DoorStop)エラー");
                    }
                    if (boolLogicalStateOpen == true)
                    {
                        // Mainステート・マシンへ結果を出力: Open完了
                        OpFlagOpenDoor = true;
                        OpResultOpenDoor = EDeviceResult.Done;
                        devDoor.devStateVal = EDevState.DoorStop;
                    }
                    else
                    {
                        devDoor.stopwatchPulse.Restart();
                        devDoor.devStateVal = EDevState.DoorOpenInitAfterWaitPulseOff;
                    }
                    break;
                case EDevState.DoorOpenInitAfterWaitPulseOff:
                    if (boolLogicalStateOpen == true)
                    {
                        // Mainステート・マシンへ結果を出力: Open完了
                        OpFlagOpenDoor = true;
                        OpResultOpenDoor = EDeviceResult.Done;
                        devDoor.devStateVal = EDevState.DoorStop;
                    }
                    else if (devDoor.stopwatchPulse.ElapsedMilliseconds >= preferencesDatOriginal.InactivePulseWidthOfDoor)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.DoorOpen);
                        if (boolFuncRet != true)
                        {
                            Debug.WriteLine("Door:State[DoorOpenInitAfterWaitPulseOff] IoBoardDevice.SetUpperStateOfDOut(DoorOpen)エラー");
                        }
                        devDoor.stopwatchPulse.Restart();
                        devDoor.stopwatchTimeout.Restart();
                        devDoor.devStateVal = EDevState.DoorOpenPulseOn1;
                    }
                    break;
                case EDevState.DoorOpenInit:
                    boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.DoorStop);
                    if (boolFuncRet != true)
                    {
                        Debug.WriteLine("Door:State[DoorOpenInit] IoBoardDevice.SetUpperStateOfDOut(DoorStop)エラー");
                    }
                    if (boolLogicalStateOpen == true)
                    {
                        // Mainステート・マシンへ結果を出力: Open完了
                        OpFlagOpenDoor = true;
                        OpResultOpenDoor = EDeviceResult.Done;
                        devDoor.stopwatchPulse.Stop();
                        devDoor.devStateVal = EDevState.DoorStop;
                    }
                    else
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.DoorOpen);
                        if (boolFuncRet != true)
                        {
                            Debug.WriteLine("Door:State[DoorOpenInitAfterWaitPulseOff] IoBoardDevice.SetUpperStateOfDOut(DoorOpen)エラー");
                        }
                        devDoor.stopwatchPulse.Restart();
                        devDoor.stopwatchTimeout.Restart();
                        devDoor.devStateVal = EDevState.DoorOpenPulseOn1;
                    }
                    break;
                case EDevState.DoorOpenPulseOn1:
                    if (boolLogicalStateOpen == true)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.DoorStop);
                        if (boolFuncRet != true)
                        {
                            Debug.WriteLine("Door:State[DoorOpenPulseOn1] IoBoardDevice.SetUpperStateOfDOut(DoorStop)エラー");
                        }
                        // Mainステート・マシンへ結果を出力: Open完了
                        OpFlagOpenDoor = true;
                        OpResultOpenDoor = EDeviceResult.Done;
                        devDoor.devStateVal = EDevState.DoorStop;
                    }
                    else if (devDoor.stopwatchPulse.ElapsedMilliseconds >= preferencesDatOriginal.ActivePulseWidthOfDoor)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.DoorStop);
                        if (boolFuncRet != true)
                        {
                            Debug.WriteLine("Door:State[DoorOpenPulseOn1] IoBoardDevice.SetUpperStateOfDOut(DoorStop)エラー");
                        }
                        devDoor.stopwatchPulse.Restart();
                        devDoor.devStateVal = EDevState.DoorOpenPulseOff1;
                    }
                    break;
                case EDevState.DoorOpenPulseOff1:
                    if (boolLogicalStateOpen == true)
                    {
                        // Mainステート・マシンへ結果を出力: Open完了
                        OpFlagOpenDoor = true;
                        OpResultOpenDoor = EDeviceResult.Done;
                        devDoor.devStateVal = EDevState.DoorStop;
                    }
                    else if (devDoor.stopwatchPulse.ElapsedMilliseconds >= preferencesDatOriginal.InactivePulseWidthOfDoor)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.DoorOpen);
                        if (boolFuncRet != true)
                        {
                            Debug.WriteLine("Door:State[DoorOpenPulseOff1] IoBoardDevice.SetUpperStateOfDOut(DoorOpen)エラー");
                        }
                        devDoor.stopwatchPulse.Restart();
                        devDoor.devStateVal = EDevState.DoorOpenPulseOn2;
                    }
                    break;
                case EDevState.DoorOpenPulseOn2:
                    if (boolLogicalStateOpen == true)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.DoorStop);
                        if (boolFuncRet != true)
                        {
                            Debug.WriteLine("Door:State[DoorOpenPulseOn2] IoBoardDevice.SetUpperStateOfDOut(DoorStop)エラー");
                        }
                        // Mainステート・マシンへ結果を出力: Open完了
                        OpFlagOpenDoor = true;
                        OpResultOpenDoor = EDeviceResult.Done;
                        devDoor.devStateVal = EDevState.DoorStop;
                    }
                    else if (devDoor.stopwatchPulse.ElapsedMilliseconds >= preferencesDatOriginal.ActivePulseWidthOfDoor)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.DoorStop);
                        if (boolFuncRet != true)
                        {
                            Debug.WriteLine("Door:State[DoorOpenPulseOn2] IoBoardDevice.SetUpperStateOfDOut(DoorStop)エラー");
                        }
                        devDoor.stopwatchPulse.Stop();
                        devDoor.devStateVal = EDevState.DoorOpenDoing;
                    }
                    break;
                case EDevState.DoorOpenDoing:
                    if (boolLogicalStateOpen == true)
                    {
                        // Mainステート・マシンへ結果を出力: Open完了
                        OpFlagOpenDoor = true;
                        OpResultOpenDoor = EDeviceResult.Done;
                        devDoor.devStateVal = EDevState.DoorStop;
                    }
                    else if (devDoor.stopwatchTimeout.ElapsedMilliseconds >= (preferencesDatOriginal.TimeoutOfDoorOpen * 1000))
                    {
                        // Openタイムアウトの時
                        if (Properties.Settings.Default.IS_DOOR_RECOVER_ENABLE == true)
                        {
                            // ドア回復処理する時
                            boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.DoorClose);
                            if (boolFuncRet != true)
                            {
                                Debug.WriteLine("Door:State[DoorOpenDoing] IoBoardDevice.SetUpperStateOfDOut(DoorClose)エラー");
                            }
                            // リトライ・カウント: 0クリア
                            devDoor.retryCount = 0;
                            devDoor.stopwatchPulse.Restart();
                            devDoor.stopwatchTimeout.Restart();
                            devDoor.devStateVal = EDevState.DoorOpenRecoverWithClosePulseOn1;
                        }
                        else
                        {
                            // ドア回復処理しない時
                            devDoor.stopwatchPulse.Stop();
                            devDoor.stopwatchTimeout.Stop();
                            // Mainステート・マシンへ結果を出力: Timeout
                            OpFlagOpenDoor = true;
                            OpResultOpenDoor = EDeviceResult.TimeOut;
                            devDoor.devStateVal = EDevState.DoorStop;
                        }
                    }
                    break;
                case EDevState.DoorOpenRecoverInit:
                    if (boolLogicalStateOpen == true)
                    {
                        // Mainステート・マシンへ結果を出力: Open完了
                        OpFlagOpenDoor = true;
                        OpResultOpenDoor = EDeviceResult.Done;
                        devDoor.devStateVal = EDevState.DoorStop;
                    }
                    else
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.DoorStop);
                        if (boolFuncRet != true)
                        {
                            Debug.WriteLine("Door:State[DoorOpenRecoverInit] IoBoardDevice.SetUpperStateOfDOut(DoorStop)エラー");
                        }
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.DoorClose);
                        if (boolFuncRet != true)
                        {
                            Debug.WriteLine("Door:State[DoorOpenRecoverInit] IoBoardDevice.SetUpperStateOfDOut(DoorClose)エラー");
                        }
                        devDoor.retryCount = 0;
                        devDoor.stopwatchPulse.Restart();
                        devDoor.devStateVal = EDevState.DoorOpenRecoverWithClosePulseOn1;
                    }
                    break;
                case EDevState.DoorOpenRecoverInitRetry:
                    break;
                case EDevState.DoorOpenRecoverWithClosePulseOn1:
                    // 現在: Closeリミット・センサは見ない
                    if (devDoor.stopwatchPulse.ElapsedMilliseconds >= preferencesDatOriginal.ActivePulseWidthOfDoor)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.DoorStop);
                        if (boolFuncRet != true)
                        {
                            Debug.WriteLine("Door:State[DoorOpenRecoverWithClosePulseOn1] IoBoardDevice.SetUpperStateOfDOut(DoorStop)エラー");
                        }
                        devDoor.stopwatchPulse.Restart();
                        devDoor.devStateVal = EDevState.DoorOpenRecoverWithClosePulseOff1;
                    }
                    break;
                case EDevState.DoorOpenRecoverWithClosePulseOff1:
                    // 現在: Closeリミット・センサは見ない
                    if (devDoor.stopwatchPulse.ElapsedMilliseconds >= preferencesDatOriginal.InactivePulseWidthOfDoor)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.DoorClose);
                        if (boolFuncRet != true)
                        {
                            Debug.WriteLine("Door:State[DoorOpenRecoverWithClosePulseOff1] IoBoardDevice.SetUpperStateOfDOut(DoorClose)エラー");
                        }
                        devDoor.stopwatchPulse.Restart();
                        devDoor.devStateVal = EDevState.DoorOpenRecoverWithClosePulseOn2;
                    }
                    break;
                case EDevState.DoorOpenRecoverWithClosePulseOn2:
                    // 現在: Closeリミット・センサは見ない
                    if (devDoor.stopwatchPulse.ElapsedMilliseconds >= preferencesDatOriginal.ActivePulseWidthOfDoor)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.DoorStop);
                        if (boolFuncRet != true)
                        {
                            Debug.WriteLine("Door:State[DoorOpenRecoverWithClosePulseOn2] IoBoardDevice.SetUpperStateOfDOut(DoorStop)エラー");
                        }
                        devDoor.stopwatchPulse.Stop();
                        // Close動作時間: 開始
                        devDoor.stopwatchTimeout.Restart();
                        devDoor.devStateVal = EDevState.DoorOpenRecoverWithClosing;
                    }
                    break;
                case EDevState.DoorOpenRecoverWithClosing:
                    if ((boolLogicalStateClose == true) ||
                        (devDoor.stopwatchTimeout.ElapsedMilliseconds >= preferencesDatOriginal.DoorCloseTimeInRecoverOpen))
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.DoorOpen);
                        if (boolFuncRet != true)
                        {
                            Debug.WriteLine("Door:State[DoorOpenRecoverWithClosing] IoBoardDevice.SetUpperStateOfDOut(DoorOpen)エラー");
                        }
                        devDoor.stopwatchPulse.Restart();
                        // Openタイムアウト: 開始
                        devDoor.stopwatchTimeout.Restart();
                        devDoor.devStateVal = EDevState.DoorOpenRecoverWithOpenPulseOn1;
                    }
                    break;
                case EDevState.DoorOpenRecoverWithOpenPulseOn1:
                    if (boolLogicalStateOpen == true)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.DoorStop);
                        if (boolFuncRet != true)
                        {
                            Debug.WriteLine("Door:State[DoorOpenRecoverWithOpenPulseOn1] IoBoardDevice.SetUpperStateOfDOut(DoorStop)エラー");
                        }
                        // Mainステート・マシンへ結果を出力: Open完了
                        OpFlagOpenDoor = true;
                        OpResultOpenDoor = EDeviceResult.Done;
                        devDoor.stopwatchPulse.Stop();
                        devDoor.stopwatchTimeout.Stop();
                        devDoor.devStateVal = EDevState.DoorStop;
                    }
                    else if (devDoor.stopwatchPulse.ElapsedMilliseconds >= preferencesDatOriginal.ActivePulseWidthOfDoor)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.DoorStop);
                        if (boolFuncRet != true)
                        {
                            Debug.WriteLine("Door:State[DoorOpenRecoverWithOpenPulseOn1] IoBoardDevice.SetUpperStateOfDOut(DoorStop)エラー");
                        }
                        devDoor.stopwatchPulse.Restart();
                        devDoor.devStateVal = EDevState.DoorOpenRecoverWithOpenPulseOff1;
                    }
                    break;
                case EDevState.DoorOpenRecoverWithOpenPulseOff1:
                    if (boolLogicalStateOpen == true)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.DoorStop);
                        if (boolFuncRet != true)
                        {
                            Debug.WriteLine("Door:State[DoorOpenRecoverWithOpenPulseOff1] IoBoardDevice.SetUpperStateOfDOut(DoorStop)エラー");
                        }
                        // Mainステート・マシンへ結果を出力: Open完了
                        OpFlagOpenDoor = true;
                        OpResultOpenDoor = EDeviceResult.Done;
                        devDoor.stopwatchPulse.Stop();
                        devDoor.stopwatchTimeout.Stop();
                        devDoor.devStateVal = EDevState.DoorStop;
                    }
                    else if (devDoor.stopwatchPulse.ElapsedMilliseconds >= preferencesDatOriginal.InactivePulseWidthOfDoor)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.DoorOpen);
                        if (boolFuncRet != true)
                        {
                            Debug.WriteLine("Door:State[DoorOpenRecoverWithOpenPulseOff1] IoBoardDevice.SetUpperStateOfDOut(DoorOpen)エラー");
                        }
                        devDoor.stopwatchPulse.Restart();
                        devDoor.devStateVal = EDevState.DoorOpenRecoverWithOpenPulseOn2;
                    }
                    break;
                case EDevState.DoorOpenRecoverWithOpenPulseOn2:
                    if (boolLogicalStateOpen == true)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.DoorStop);
                        if (boolFuncRet != true)
                        {
                            Debug.WriteLine("Door:State[DoorOpenRecoverWithOpenPulseOn2] IoBoardDevice.SetUpperStateOfDOut(DoorStop)エラー");
                        }
                        // Mainステート・マシンへ結果を出力: Open完了
                        OpFlagOpenDoor = true;
                        OpResultOpenDoor = EDeviceResult.Done;
                        devDoor.stopwatchPulse.Stop();
                        devDoor.stopwatchTimeout.Stop();
                        devDoor.devStateVal = EDevState.DoorStop;
                    }
                    else if (devDoor.stopwatchPulse.ElapsedMilliseconds >= preferencesDatOriginal.ActivePulseWidthOfDoor)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.DoorStop);
                        if (boolFuncRet != true)
                        {
                            Debug.WriteLine("Door:State[DoorOpenRecoverWithOpenPulseOn2] IoBoardDevice.SetUpperStateOfDOut(DoorStop)エラー");
                        }
                        devDoor.stopwatchPulse.Stop();
                        devDoor.devStateVal = EDevState.DoorOpenRecoverWithOpenning;
                    }
                    break;
                case EDevState.DoorOpenRecoverWithOpenning:
                    if (boolLogicalStateOpen == true)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.DoorStop);
                        if (boolFuncRet != true)
                        {
                            Debug.WriteLine("Door:State[DoorOpenRecoverWithOpenning] IoBoardDevice.SetUpperStateOfDOut(DoorStop)エラー");
                        }
                        // Mainステート・マシンへ結果を出力: Open完了
                        OpFlagOpenDoor = true;
                        OpResultOpenDoor = EDeviceResult.Done;
                        devDoor.stopwatchPulse.Stop();
                        devDoor.stopwatchTimeout.Stop();
                        devDoor.devStateVal = EDevState.DoorStop;
                    }
                    // タイムアウトはClose方向へ引いた2倍の時間
                    else if (devDoor.stopwatchTimeout.ElapsedMilliseconds >= (preferencesDatOriginal.DoorCloseTimeInRecoverOpen * 2))
                    {
                        // タイムアウトの時
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.DoorStop);
                        if (boolFuncRet != true)
                        {
                            Debug.WriteLine("Door:State[DoorOpenRecoverWithOpenning] IoBoardDevice.SetUpperStateOfDOut(DoorStop)エラー");
                        }
                        devDoor.stopwatchTimeout.Stop();
                        // リトライ・カウントアップ 
                        devDoor.retryCount++;
                        if (devDoor.retryCount > preferencesDatOriginal.DoorRetryNumInRecover)
                        {
                            // リトライ回数に到達した時
                            boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.DoorClose);
                            if (boolFuncRet != true)
                            {
                                Debug.WriteLine("Door:State[DoorOpenRecoverWithOpenning] IoBoardDevice.SetUpperStateOfDOut(DoorClose)エラー");
                            }
                            devDoor.stopwatchPulse.Restart();
                            devDoor.stopwatchTimeout.Restart();
                            devDoor.devStateVal = EDevState.DoorOpenRecoverTimeoutWithClosePulseOn1;
                        }
                        else
                        {
                            // リトライ回数までいっていない時
                            boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.DoorClose);
                            if (boolFuncRet != true)
                            {
                                Debug.WriteLine("Door:State[DoorOpenRecoverWithOpenning] IoBoardDevice.SetUpperStateOfDOut(DoorClose)エラー");
                            }
                            devDoor.stopwatchPulse.Restart();
                            devDoor.stopwatchTimeout.Restart();
                            devDoor.devStateVal = EDevState.DoorOpenRecoverWithClosePulseOn1;

                        }
                    }
                    break;
                case EDevState.DoorOpenRecoverTimeoutInit:
                    break;
                case EDevState.DoorOpenRecoverTimeoutWithClosePulseOn1:
                    // 現在: Closeリミット・センサは見ない
                    if (devDoor.stopwatchPulse.ElapsedMilliseconds >= preferencesDatOriginal.ActivePulseWidthOfDoor)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.DoorStop);
                        if (boolFuncRet != true)
                        {
                            Debug.WriteLine("Door:State[DoorOpenRecoverTimeoutWithClosePulseOn1] IoBoardDevice.SetUpperStateOfDOut(DoorStop)エラー");
                        }
                        devDoor.stopwatchPulse.Restart();
                        devDoor.devStateVal = EDevState.DoorOpenRecoverTimeoutWithClosePulseOff1;
                    }
                    break;
                case EDevState.DoorOpenRecoverTimeoutWithClosePulseOff1:
                    // 現在: Closeリミット・センサは見ない
                    if (devDoor.stopwatchPulse.ElapsedMilliseconds >= preferencesDatOriginal.InactivePulseWidthOfDoor)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.DoorClose);
                        if (boolFuncRet != true)
                        {
                            Debug.WriteLine("Door:State[DoorOpenRecoverTimeoutWithClosePulseOff1] IoBoardDevice.SetUpperStateOfDOut(DoorClose)エラー");
                        }
                        devDoor.stopwatchPulse.Restart();
                        devDoor.devStateVal = EDevState.DoorOpenRecoverTimeoutWithClosePulseOn2;
                    }
                    break;
                case EDevState.DoorOpenRecoverTimeoutWithClosePulseOn2:
                    // 現在: Closeリミット・センサは見ない
                    if (devDoor.stopwatchPulse.ElapsedMilliseconds >= preferencesDatOriginal.ActivePulseWidthOfDoor)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.DoorStop);
                        if (boolFuncRet != true)
                        {
                            Debug.WriteLine("Door:State[DoorOpenRecoverTimeoutWithClosePulseOn2] IoBoardDevice.SetUpperStateOfDOut(DoorStop)エラー");
                        }
                        devDoor.stopwatchPulse.Stop();
                        devDoor.devStateVal = EDevState.DoorOpenRecoverTimeoutWithClosing;
                    }
                    break;
                case EDevState.DoorOpenRecoverTimeoutWithClosing:
                    // Close方向へClose時間引いて停止
                    if (devDoor.stopwatchTimeout.ElapsedMilliseconds >= preferencesDatOriginal.DoorCloseTimeInRecoverOpen)
                    {
                        devDoor.stopwatchPulse.Stop();
                        devDoor.stopwatchTimeout.Stop();
                        // Mainステート・マシンへ結果を出力: Timeout
                        OpFlagOpenDoor = true;
                        OpResultOpenDoor = EDeviceResult.TimeOut;
                        devDoor.devStateVal = EDevState.DoorStop;
                    }
                    break;

                #endregion

                #region DoorClose
                case EDevState.DoorCloseInitAfterWait:
                    boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.DoorStop);
                    if (boolFuncRet != true)
                    {
                        Debug.WriteLine("Door:State[DoorCloseInitAfterWait] IoBoardDevice.SetUpperStateOfDOut(DoorStop)エラー");
                    }
                    if (boolLogicalStateClose == true)
                    {
                        // Mainステート・マシンへ結果を出力: Close完了
                        OpFlagCloseDoor = true;
                        OpResultCloseDoor = EDeviceResult.Done;
                        devDoor.devStateVal = EDevState.DoorStop;
                    }
                    else
                    {
                        devDoor.stopwatchPulse.Restart();
                        devDoor.devStateVal = EDevState.DoorCloseInitAfterWaitPulseOff;
                    }
                    break;
                case EDevState.DoorCloseInitAfterWaitPulseOff:
                    if (boolLogicalStateClose == true)
                    {
                        // Mainステート・マシンへ結果を出力: Close完了
                        OpFlagCloseDoor = true;
                        OpResultCloseDoor = EDeviceResult.Done;
                        devDoor.devStateVal = EDevState.DoorStop;
                    }
                    else if (devDoor.stopwatchPulse.ElapsedMilliseconds >= preferencesDatOriginal.InactivePulseWidthOfDoor)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.DoorClose);
                        if (boolFuncRet != true)
                        {
                            Debug.WriteLine("Door:State[DoorCloseInitAfterWaitPulseOff] IoBoardDevice.SetUpperStateOfDOut(DoorClose)エラー");
                        }
                        devDoor.stopwatchPulse.Restart();
                        devDoor.stopwatchTimeout.Restart();
                        devDoor.devStateVal = EDevState.DoorClosePulseOn1;
                    }
                    break;
                case EDevState.DoorCloseInit:
                    boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.DoorStop);
                    if (boolFuncRet != true)
                    {
                        Debug.WriteLine("Door:State[DoorCloseInit] IoBoardDevice.SetUpperStateOfDOut(DoorStop)エラー");
                    }
                    if (boolLogicalStateClose == true)
                    {
                        // Mainステート・マシンへ結果を出力: Close完了
                        OpFlagCloseDoor = true;
                        OpResultCloseDoor = EDeviceResult.Done;
                        devDoor.stopwatchPulse.Stop();
                        devDoor.devStateVal = EDevState.DoorStop;
                    }
                    else
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.DoorClose);
                        if (boolFuncRet != true)
                        {
                            Debug.WriteLine("Door:State[DoorCloseInitAfterWaitPulseOff] IoBoardDevice.SetUpperStateOfDOut(DoorClose)エラー");
                        }
                        devDoor.stopwatchPulse.Restart();
                        devDoor.stopwatchTimeout.Restart();
                        devDoor.devStateVal = EDevState.DoorClosePulseOn1;
                    }
                    break;
                case EDevState.DoorClosePulseOn1:
                    if (boolLogicalStateClose == true)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.DoorStop);
                        if (boolFuncRet != true)
                        {
                            Debug.WriteLine("Door:State[DoorClosePulseOn1] IoBoardDevice.SetUpperStateOfDOut(DoorStop)エラー");
                        }
                        // Mainステート・マシンへ結果を出力: Close完了
                        OpFlagCloseDoor = true;
                        OpResultCloseDoor = EDeviceResult.Done;
                        devDoor.devStateVal = EDevState.DoorStop;
                    }
                    else if (devDoor.stopwatchPulse.ElapsedMilliseconds >= preferencesDatOriginal.ActivePulseWidthOfDoor)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.DoorStop);
                        if (boolFuncRet != true)
                        {
                            Debug.WriteLine("Door:State[DoorClosePulseOn1] IoBoardDevice.SetUpperStateOfDOut(DoorStop)エラー");
                        }
                        devDoor.stopwatchPulse.Restart();
                        devDoor.devStateVal = EDevState.DoorClosePulseOff1;
                    }
                    break;
                case EDevState.DoorClosePulseOff1:
                    if (boolLogicalStateClose == true)
                    {
                        // Mainステート・マシンへ結果を出力: Close完了
                        OpFlagCloseDoor = true;
                        OpResultCloseDoor = EDeviceResult.Done;
                        devDoor.devStateVal = EDevState.DoorStop;
                    }
                    else if (devDoor.stopwatchPulse.ElapsedMilliseconds >= preferencesDatOriginal.InactivePulseWidthOfDoor)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.DoorClose);
                        if (boolFuncRet != true)
                        {
                            Debug.WriteLine("Door:State[DoorClosePulseOff1] IoBoardDevice.SetUpperStateOfDOut(DoorClose)エラー");
                        }
                        devDoor.stopwatchPulse.Restart();
                        devDoor.devStateVal = EDevState.DoorClosePulseOn2;
                    }
                    break;
                case EDevState.DoorClosePulseOn2:
                    if (boolLogicalStateClose == true)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.DoorStop);
                        if (boolFuncRet != true)
                        {
                            Debug.WriteLine("Door:State[DoorClosePulseOn2] IoBoardDevice.SetUpperStateOfDOut(DoorStop)エラー");
                        }
                        // Mainステート・マシンへ結果を出力: Close完了
                        OpFlagCloseDoor = true;
                        OpResultCloseDoor = EDeviceResult.Done;
                        devDoor.devStateVal = EDevState.DoorStop;
                    }
                    else if (devDoor.stopwatchPulse.ElapsedMilliseconds >= preferencesDatOriginal.ActivePulseWidthOfDoor)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.DoorStop);
                        if (boolFuncRet != true)
                        {
                            Debug.WriteLine("Door:State[DoorClosePulseOn2] IoBoardDevice.SetUpperStateOfDOut(DoorStop)エラー");
                        }
                        devDoor.stopwatchPulse.Stop();
                        devDoor.devStateVal = EDevState.DoorCloseDoing;
                    }
                    break;
                case EDevState.DoorCloseDoing:
                    if (boolLogicalStateClose == true)
                    {
                        // Mainステート・マシンへ結果を出力: Close完了
                        OpFlagCloseDoor = true;
                        OpResultCloseDoor = EDeviceResult.Done;
                        devDoor.devStateVal = EDevState.DoorStop;
                    }
                    else if (devDoor.stopwatchTimeout.ElapsedMilliseconds >= (preferencesDatOriginal.TimeoutOfDoorClose * 1000))
                    {
                        // Openタイムアウトの時
                        if (Properties.Settings.Default.IS_DOOR_RECOVER_ENABLE == true)
                        {
                            // ドア回復処理する時
                            boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.DoorOpen);
                            if (boolFuncRet != true)
                            {
                                Debug.WriteLine("Door:State[DoorCloseDoing] IoBoardDevice.SetUpperStateOfDOut(DoorOpen)エラー");
                            }
                            // リトライ・カウント: 0クリア
                            devDoor.retryCount = 0;
                            devDoor.stopwatchPulse.Restart();
                            devDoor.stopwatchTimeout.Restart();
                            devDoor.devStateVal = EDevState.DoorCloseRecoverWithOpenPulseOn1;
                        }
                        else
                        {
                            // ドア回復処理しない時
                            devDoor.stopwatchPulse.Stop();
                            devDoor.stopwatchTimeout.Stop();
                            // Mainステート・マシンへ結果を出力: Timeout
                            OpFlagCloseDoor = true;
                            OpResultCloseDoor = EDeviceResult.TimeOut;
                            devDoor.devStateVal = EDevState.DoorStop;
                        }
                    }
                    break;
                case EDevState.DoorCloseRecoverInit:
                    if (boolLogicalStateClose == true)
                    {
                        // Mainステート・マシンへ結果を出力: Close完了
                        OpFlagCloseDoor = true;
                        OpResultCloseDoor = EDeviceResult.Done;
                        devDoor.devStateVal = EDevState.DoorStop;
                    }
                    else
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.DoorStop);
                        if (boolFuncRet != true)
                        {
                            Debug.WriteLine("Door:State[DoorCloseRecoverInit] IoBoardDevice.SetUpperStateOfDOut(DoorStop)エラー");
                        }
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.DoorOpen);
                        if (boolFuncRet != true)
                        {
                            Debug.WriteLine("Door:State[DoorCloseRecoverInit] IoBoardDevice.SetUpperStateOfDOut(DoorOpen)エラー");
                        }
                        devDoor.retryCount = 0;
                        devDoor.stopwatchPulse.Restart();
                        devDoor.devStateVal = EDevState.DoorCloseRecoverWithOpenPulseOn1;
                    }
                    break;
                case EDevState.DoorCloseRecoverInitRetry:
                    break;
                case EDevState.DoorCloseRecoverWithOpenPulseOn1:
                    // 現在: Openリミット・センサは見ない
                    if (devDoor.stopwatchPulse.ElapsedMilliseconds >= preferencesDatOriginal.ActivePulseWidthOfDoor)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.DoorStop);
                        if (boolFuncRet != true)
                        {
                            Debug.WriteLine("Door:State[DoorCloseRecoverWithOpenPulseOn1] IoBoardDevice.SetUpperStateOfDOut(DoorStop)エラー");
                        }
                        devDoor.stopwatchPulse.Restart();
                        devDoor.devStateVal = EDevState.DoorCloseRecoverWithOpenPulseOff1;
                    }
                    break;
                case EDevState.DoorCloseRecoverWithOpenPulseOff1:
                    // 現在: Openリミット・センサは見ない
                    if (devDoor.stopwatchPulse.ElapsedMilliseconds >= preferencesDatOriginal.InactivePulseWidthOfDoor)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.DoorOpen);
                        if (boolFuncRet != true)
                        {
                            Debug.WriteLine("Door:State[DoorCloseRecoverWithOpenPulseOff1] IoBoardDevice.SetUpperStateOfDOut(DoorOpen)エラー");
                        }
                        devDoor.stopwatchPulse.Restart();
                        devDoor.devStateVal = EDevState.DoorCloseRecoverWithOpenPulseOn2;
                    }
                    break;
                case EDevState.DoorCloseRecoverWithOpenPulseOn2:
                    // 現在: Openリミット・センサは見ない
                    if (devDoor.stopwatchPulse.ElapsedMilliseconds >= preferencesDatOriginal.ActivePulseWidthOfDoor)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.DoorStop);
                        if (boolFuncRet != true)
                        {
                            Debug.WriteLine("Door:State[DoorCloseRecoverWithOpenPulseOn2] IoBoardDevice.SetUpperStateOfDOut(DoorStop)エラー");
                        }
                        devDoor.stopwatchPulse.Stop();
                        // Open動作時間: 開始
                        devDoor.stopwatchTimeout.Restart();
                        devDoor.devStateVal = EDevState.DoorCloseRecoverWithOpenning;
                    }
                    break;
                case EDevState.DoorCloseRecoverWithOpenning:
                    if ((boolLogicalStateOpen == true) ||
                        (devDoor.stopwatchTimeout.ElapsedMilliseconds >= preferencesDatOriginal.DoorOpenTimeInRecoverClose))
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.DoorClose);
                        if (boolFuncRet != true)
                        {
                            Debug.WriteLine("Door:State[DoorCloseRecoverWithOpenning] IoBoardDevice.SetUpperStateOfDOut(DoorClose)エラー");
                        }
                        devDoor.stopwatchPulse.Restart();
                        // Openタイムアウト: 開始
                        devDoor.stopwatchTimeout.Restart();
                        devDoor.devStateVal = EDevState.DoorCloseRecoverWithClosePulseOn1;
                    }
                    break;
                case EDevState.DoorCloseRecoverWithClosePulseOn1:
                    if (boolLogicalStateClose == true)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.DoorStop);
                        if (boolFuncRet != true)
                        {
                            Debug.WriteLine("Door:State[DoorCloseRecoverWithClosePulseOn1] IoBoardDevice.SetUpperStateOfDOut(DoorStop)エラー");
                        }
                        // Mainステート・マシンへ結果を出力: Close完了
                        OpFlagCloseDoor = true;
                        OpResultCloseDoor = EDeviceResult.Done;
                        devDoor.stopwatchPulse.Stop();
                        devDoor.stopwatchTimeout.Stop();
                        devDoor.devStateVal = EDevState.DoorStop;
                    }
                    else if (devDoor.stopwatchPulse.ElapsedMilliseconds >= preferencesDatOriginal.ActivePulseWidthOfDoor)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.DoorStop);
                        if (boolFuncRet != true)
                        {
                            Debug.WriteLine("Door:State[DoorCloseRecoverWithClosePulseOn1] IoBoardDevice.SetUpperStateOfDOut(DoorStop)エラー");
                        }
                        devDoor.stopwatchPulse.Restart();
                        devDoor.devStateVal = EDevState.DoorCloseRecoverWithClosePulseOff1;
                    }
                    break;
                case EDevState.DoorCloseRecoverWithClosePulseOff1:
                    if (boolLogicalStateClose == true)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.DoorStop);
                        if (boolFuncRet != true)
                        {
                            Debug.WriteLine("Door:State[DoorCloseRecoverWithClosePulseOff1] IoBoardDevice.SetUpperStateOfDOut(DoorStop)エラー");
                        }
                        // Mainステート・マシンへ結果を出力: Close完了
                        OpFlagCloseDoor = true;
                        OpResultCloseDoor = EDeviceResult.Done;
                        devDoor.stopwatchPulse.Stop();
                        devDoor.stopwatchTimeout.Stop();
                        devDoor.devStateVal = EDevState.DoorStop;
                    }
                    else if (devDoor.stopwatchPulse.ElapsedMilliseconds >= preferencesDatOriginal.InactivePulseWidthOfDoor)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.DoorClose);
                        if (boolFuncRet != true)
                        {
                            Debug.WriteLine("Door:State[DoorCloseRecoverWithClosePulseOff1] IoBoardDevice.SetUpperStateOfDOut(DoorClose)エラー");
                        }
                        devDoor.stopwatchPulse.Restart();
                        devDoor.devStateVal = EDevState.DoorCloseRecoverWithClosePulseOn2;
                    }
                    break;
                case EDevState.DoorCloseRecoverWithClosePulseOn2:
                    if (boolLogicalStateClose == true)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.DoorStop);
                        if (boolFuncRet != true)
                        {
                            Debug.WriteLine("Door:State[DoorCloseRecoverWithClosePulseOn2] IoBoardDevice.SetUpperStateOfDOut(DoorStop)エラー");
                        }
                        // Mainステート・マシンへ結果を出力: Close完了
                        OpFlagCloseDoor = true;
                        OpResultCloseDoor = EDeviceResult.Done;
                        devDoor.stopwatchPulse.Stop();
                        devDoor.stopwatchTimeout.Stop();
                        devDoor.devStateVal = EDevState.DoorStop;
                    }
                    else if (devDoor.stopwatchPulse.ElapsedMilliseconds >= preferencesDatOriginal.ActivePulseWidthOfDoor)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.DoorStop);
                        if (boolFuncRet != true)
                        {
                            Debug.WriteLine("Door:State[DoorCloseRecoverWithClosePulseOn2] IoBoardDevice.SetUpperStateOfDOut(DoorStop)エラー");
                        }
                        devDoor.stopwatchPulse.Stop();
                        devDoor.devStateVal = EDevState.DoorCloseRecoverWithClosing;
                    }
                    break;
                case EDevState.DoorCloseRecoverWithClosing:
                    if (boolLogicalStateClose == true)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.DoorStop);
                        if (boolFuncRet != true)
                        {
                            Debug.WriteLine("Door:State[DoorCloseRecoverWithClosing] IoBoardDevice.SetUpperStateOfDOut(DoorStop)エラー");
                        }
                        // Mainステート・マシンへ結果を出力: Close完了
                        OpFlagCloseDoor = true;
                        OpResultCloseDoor = EDeviceResult.Done;
                        devDoor.stopwatchPulse.Stop();
                        devDoor.stopwatchTimeout.Stop();
                        devDoor.devStateVal = EDevState.DoorStop;
                    }
                    // タイムアウトはOpen方向へ引いた2倍の時間
                    else if (devDoor.stopwatchTimeout.ElapsedMilliseconds >= (preferencesDatOriginal.DoorOpenTimeInRecoverClose * 2))
                    {
                        // タイムアウトの時
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.DoorStop);
                        if (boolFuncRet != true)
                        {
                            Debug.WriteLine("Door:State[DoorCloseRecoverWithClosing] IoBoardDevice.SetUpperStateOfDOut(DoorStop)エラー");
                        }
                        devDoor.stopwatchTimeout.Stop();
                        // リトライ・カウントアップ 
                        devDoor.retryCount++;
                        if (devDoor.retryCount > preferencesDatOriginal.DoorRetryNumInRecover)
                        {
                            // リトライ回数に到達した時
                            boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.DoorOpen);
                            if (boolFuncRet != true)
                            {
                                Debug.WriteLine("Door:State[DoorCloseRecoverWithClosing] IoBoardDevice.SetUpperStateOfDOut(DoorOpen)エラー");
                            }
                            devDoor.stopwatchPulse.Restart();
                            devDoor.stopwatchTimeout.Restart();
                            devDoor.devStateVal = EDevState.DoorCloseRecoverTimeoutWithOpenPulseOn1;
                        }
                        else
                        {
                            // リトライ回数までいっていない時
                            boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.DoorOpen);
                            if (boolFuncRet != true)
                            {
                                Debug.WriteLine("Door:State[DoorCloseRecoverWithClosing] IoBoardDevice.SetUpperStateOfDOut(DoorOpen)エラー");
                            }
                            devDoor.stopwatchPulse.Restart();
                            devDoor.stopwatchTimeout.Restart();
                            devDoor.devStateVal = EDevState.DoorCloseRecoverWithOpenPulseOn1;

                        }
                    }
                    break;
                case EDevState.DoorCloseRecoverTimeoutInit:
                    break;
                case EDevState.DoorCloseRecoverTimeoutWithOpenPulseOn1:
                    // 現在: Openリミット・センサは見ない
                    if (devDoor.stopwatchPulse.ElapsedMilliseconds >= preferencesDatOriginal.ActivePulseWidthOfDoor)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.DoorStop);
                        if (boolFuncRet != true)
                        {
                            Debug.WriteLine("Door:State[DoorCloseRecoverTimeoutWithOpenPulseOn1] IoBoardDevice.SetUpperStateOfDOut(DoorStop)エラー");
                        }
                        devDoor.stopwatchPulse.Restart();
                        devDoor.devStateVal = EDevState.DoorCloseRecoverTimeoutWithOpenPulseOff1;
                    }
                    break;
                case EDevState.DoorCloseRecoverTimeoutWithOpenPulseOff1:
                    // 現在: Openリミット・センサは見ない
                    if (devDoor.stopwatchPulse.ElapsedMilliseconds >= preferencesDatOriginal.InactivePulseWidthOfDoor)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.DoorOpen);
                        if (boolFuncRet != true)
                        {
                            Debug.WriteLine("Door:State[DoorCloseRecoverTimeoutWithOpenPulseOff1] IoBoardDevice.SetUpperStateOfDOut(DoorOpen)エラー");
                        }
                        devDoor.stopwatchPulse.Restart();
                        devDoor.devStateVal = EDevState.DoorCloseRecoverTimeoutWithOpenPulseOn2;
                    }
                    break;
                case EDevState.DoorCloseRecoverTimeoutWithOpenPulseOn2:
                    // 現在: Openリミット・センサは見ない
                    if (devDoor.stopwatchPulse.ElapsedMilliseconds >= preferencesDatOriginal.ActivePulseWidthOfDoor)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.DoorStop);
                        if (boolFuncRet != true)
                        {
                            Debug.WriteLine("Door:State[DoorCloseRecoverTimeoutWithOpenPulseOn2] IoBoardDevice.SetUpperStateOfDOut(DoorStop)エラー");
                        }
                        devDoor.stopwatchPulse.Stop();
                        devDoor.devStateVal = EDevState.DoorCloseRecoverTimeoutWithOpenning;
                    }
                    break;
                case EDevState.DoorCloseRecoverTimeoutWithOpenning:
                    // Open方向へOpen時間引いて停止
                    if (devDoor.stopwatchTimeout.ElapsedMilliseconds >= preferencesDatOriginal.DoorOpenTimeInRecoverClose)
                    {
                        devDoor.stopwatchPulse.Stop();
                        devDoor.stopwatchTimeout.Stop();
                        // Mainステート・マシンへ結果を出力: Timeout
                        OpFlagCloseDoor = true;
                        OpResultCloseDoor = EDeviceResult.TimeOut;
                        devDoor.devStateVal = EDevState.DoorStop;
                    }
                    break;
                #endregion
                default:
                    break;
            }
        }

#else
		// 旧版
		//		public Action<int> callbackExeForDevDoor = (a_iInArg) =>
		private void callbackInitForDevDoor()
		{
			// devDoor: 初期化
			devDoor.devStateVal = DevState.Init;
			devDoor.devStateValSaved = DevState.OverRange;
			devDoor.devCmdVal = DevCmd.None;
			devDoor.devResultVal = DevResult.None;
			devDoor.devResultValSaved = DevResult.OverRange;
			devDoor.boolFirstFlag = true;
		}
		private void callbackExeForDevDoor()
		{
			bool logicalState;
			bool funcRet;
			DevCmdPkt devCmdPktLast = null;
			DevCmdPkt devCmdPktCur;

			// コマンド受信: 最後のコマンドが有効
			while (concurrentQueueDevCmdPktDoor.TryDequeue(out devCmdPktCur))
			{
				devCmdPktLast = devCmdPktCur;
			}
			if (devCmdPktLast != null)
			{
				switch (devCmdPktLast.DevCmdVal)
				{
					case DevCmd.DoorOpen:
						devDoor.devStateVal = DevState.DoorOpenInit;
						break;
					case DevCmd.DoorClose:
						devDoor.devStateVal = DevState.DoorCloseInit;
						break;
					default:
						break;
				}
			}
			switch ((int)devDoor.devStateVal)
			{
				case (int)DevState.Init:
					devDoor.devStateVal = DevState.DoorStop;
					break;
				case (int)DevState.DoorStop:
					break;
				case (int)DevState.DoorOpenInit:
					// 結果: クリア
					devDoor.devResultVal = DevResult.None;
					funcRet = IoBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.DoorStop);
					if (funcRet != true)
					{
						Debug.WriteLine("Door:State[DoorOpenInit]DoorStop時、IoBoardDevice.SetUpperStateOfDOut()エラー");
					}
					//					Debug.Assert(funcRet != true, "Door:State[DoorOpenInit]DoorStop時、IoBoardDevice.SetUpperStateOfDOut()エラー");
					funcRet = IoBoardDevice.GetUpperStateOfSaveDIn(IoBoardDInLogicalName.DoorOpen, out logicalState);

					if (funcRet != true)
					{
						Debug.WriteLine("Door:State[DoorOpenInit]DoorOpenCmd受信時、IoBoardDevice.DirectIn()エラー");
					}
					//					Debug.Assert(funcRet != true, "Door:State[DoorOpenInit]DoorOpenCmd受信時、IoBoardDevice.DirectIn()エラー");
#if false
							if (funcRet != true)
							{
								devDoor.devStateVal = DevState.DoorError;
								devDoor.devResultVal = DevResult.DoorError;
								break;
							}
#endif
					//							if ((l_ushortInCode & (ushort)IoBoardDInRawCode.DoorOpen) == 0) // OpenセンサON
					if (logicalState == true)
					{
						// Openセンサ: onの時
						devDoor.devStateVal = DevState.DoorStop;
						devDoor.devResultVal = DevResult.DoorOpenned;
						// Mainステート・マシンへ結果を出力: Open正常完了
						OpFlagOpenDoor = true;
						OpResultOpenDoor = DeviceResult.Done;
					}
					else
					{
						// Openセンサ: offの時
						funcRet = IoBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.DoorOpen);
						if (funcRet != true)
						{
							Debug.WriteLine("Door:State[DoorOpenInit]DoorOpenCmd受信時、IoBoardDevice.DirectIn()エラー");
						}
						//						Debug.Assert(funcRet != true, "Door:State[DoorOpenInit]Openセンサ==off時、IoBoardDevice.SetUpperStateOfDOut()エラー");
#if false
								if (funcRet != true)
								{
									devDoor.devStateVal = DevState.DoorError;
									devDoor.devResultVal = DevResult.DoorError;
									break;
								}
#endif
						devDoor.iPulseTimeCount = GetTimerCountFromInterval(PreferencesDatOriginal.iOutPulseOfDoorOpen);   // 200[ms]
						devDoor.iTimeoutCount = GetTimerCountFromInterval(PreferencesDatOriginal.iTimeoutOfDoorOpen * 1000);   // 6[s]
						devDoor.devStateVal = DevState.DoorOpenPulseOn;
					}
					break;
				case (int)DevState.DoorOpenPulseOn:
					//					IoBoardDevice.DirectIn(IoBoardPortNo.Port1, out l_ushortInCode);
					funcRet = IoBoardDevice.GetUpperStateOfSaveDIn(IoBoardDInLogicalName.DoorOpen, out logicalState);
					if (funcRet != true)
					{
						Debug.WriteLine("Door:State[DoorOpenPulseOn] IoBoardDevice.GetUpperStateOfSaveDIn()エラー");
					}
					//					Debug.Assert(funcRet != true, "Door:State[DoorOpenPulseOn] IoBoardDevice.GetUpperStateOfSaveDIn()エラー");
#if false
					if (funcRet != true)
					{
						devDoor.devStateVal = DevState.DoorError;
						devDoor.devResultVal = DevResult.DoorError;
						break;
					}
#endif
					//					if ((l_ushortInCode & (ushort)IoBoardDInRawCode.DoorOpen) == 0) // OpenセンサON
					if (logicalState == true)
					{
						// Openセンサ: onの時
						funcRet = IoBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.DoorStop);
						if (funcRet != true)
						{
							Debug.WriteLine("Door:State[DoorOpenPulseOn]Openセンサ==on時、IoBoardDevice.SetUpperStateOfDOut()エラー");
						}
						//						Debug.Assert(funcRet != true, "Door:State[DoorOpenPulseOn]Openセンサ==on時、IoBoardDevice.SetUpperStateOfDOut()エラー");
#if false
						if (funcRet != true)
						{
							devDoor.devStateVal = DevState.DoorError;
							devDoor.devResultVal = DevResult.DoorError;
							break;
						}
#endif
						devDoor.devStateVal = DevState.DoorStop;
						devDoor.devResultVal = DevResult.DoorOpenned;
						// Mainステート・マシンへ結果を出力: Open正常完了
						OpFlagOpenDoor = true;
						OpResultOpenDoor = DeviceResult.Done;
					}
					else
					{
						// Openセンサ: offの時
						devDoor.iTimeoutCount--;
						if (devDoor.iTimeoutCount <= 0)
						{
							funcRet = IoBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.DoorStop);
							if (funcRet != true)
							{
								Debug.WriteLine("Door:State[DoorOpenPulseOn]Openセンサ==off→タイムアウト時、IoBoardDevice.SetUpperStateOfDOut()エラー");
							}
							//							Debug.Assert(funcRet != true, "Door:State[DoorOpenPulseOn]Openセンサ==off→タイムアウト時、IoBoardDevice.SetUpperStateOfDOut()エラー");
#if false
							if (funcRet != true)
							{
								devDoor.devStateVal = DevState.DoorError;
								devDoor.devResultVal = DevResult.DoorError;
								break;
							}
#endif
							devDoor.devStateVal = DevState.DoorStop;
							devDoor.devResultVal = DevResult.DoorOpenTimeout;
							// Mainステート・マシンへ結果を出力: Openタイムアウト
							OpFlagOpenDoor = true;
							OpResultOpenDoor = DeviceResult.TimeOut;
							break;
						}
						devDoor.iPulseTimeCount--;
						if (devDoor.iPulseTimeCount <= 0)
						{
							funcRet = IoBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.DoorStop);
							if (funcRet != true)
							{
								Debug.WriteLine("Door:State[DoorOpenPulseOn]Openセンサ==off→パルスカウント満時、IoBoardDevice.SetUpperStateOfDOut()エラー");
							}
							//							Debug.Assert(funcRet != true, "Door:State[DoorOpenPulseOn]Openセンサ==off→パルスカウント満時、IoBoardDevice.SetUpperStateOfDOut()エラー");
#if false
							if (funcRet != true)
							{
								devDoor.devStateVal = DevState.DoorError;
								devDoor.devResultVal = DevResult.DoorError;
								break;
							}
#endif
							devDoor.devStateVal = DevState.DoorOpenning;
						}
					}
					break;
				case (int)DevState.DoorOpenning:
					//					IoBoardDevice.DirectIn(IoBoardPortNo.Port1, out l_ushortInCode);
					funcRet = IoBoardDevice.GetUpperStateOfSaveDIn(IoBoardDInLogicalName.DoorOpen, out logicalState);
					if (funcRet != true)
					{
						Debug.WriteLine("Door:State[DoorOpenning]IoBoardDevice.GetUpperStateOfSaveDIn()エラー");
					}
					//					Debug.Assert(funcRet != true, "Door:State[DoorOpenning]IoBoardDevice.GetUpperStateOfSaveDIn()エラー");
#if false
					if (funcRet != true)
					{
						devDoor.devStateVal = DevState.DoorError;
						devDoor.devResultVal = DevResult.DoorError;
						break;
					}
#endif
					//					if ((l_ushortInCode & (ushort)IoBoardDInRawCode.DoorOpen) == 0) // OpenセンサON
					if (logicalState == true)
					{
						// Openセンサ: onの時
						funcRet = IoBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.DoorStop);
						if (funcRet != true)
						{
							Debug.WriteLine("Door:State[DoorOpenning]Openセンサ==on時、IoBoardDevice.SetUpperStateOfDOut()エラー");
						}
						//						Debug.Assert(funcRet != true, "Door:State[DoorOpenning]Openセンサ==on時、IoBoardDevice.SetUpperStateOfDOut()エラー");
#if false
						if (funcRet != true)
						{
							devDoor.devStateVal = DevState.DoorError;
							devDoor.devResultVal = DevResult.DoorError;
							break;
						}
#endif
						devDoor.devStateVal = DevState.DoorStop;
						devDoor.devResultVal = DevResult.DoorOpenned;
						// Mainステート・マシンへ結果を出力: Open正常完了
						OpFlagOpenDoor = true;
						OpResultOpenDoor = DeviceResult.Done;
					}
					else
					{
						// Openセンサ: offの時
						devDoor.iTimeoutCount--;
						if (devDoor.iTimeoutCount <= 0)
						{
							funcRet = IoBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.DoorStop);
							if (funcRet != true)
							{
								Debug.WriteLine("Door:State[DoorOpenning]Openセンサ==off時、IoBoardDevice.SetUpperStateOfDOut()エラー");
							}
							//							Debug.Assert(funcRet != true, "Door:State[DoorOpenning]Openセンサ==off時、IoBoardDevice.SetUpperStateOfDOut()エラー");
#if false
							if (funcRet != true)
							{
								devDoor.devStateVal = DevState.DoorError;
								devDoor.devResultVal = DevResult.DoorError;
								break;
							}
#endif

							devDoor.devStateVal = DevState.DoorStop;
							devDoor.devResultVal = DevResult.DoorOpenTimeout;
							// Mainステート・マシンへ結果を出力: Openタイムアウト
							OpFlagOpenDoor = true;
							OpResultOpenDoor = DeviceResult.TimeOut;

							break;
						}
					}
					break;
				case (int)DevState.DoorCloseInit:
					// 結果: クリア
					devDoor.devResultVal = DevResult.None;
					funcRet = IoBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.DoorStop);
					if (funcRet != true)
					{
						Debug.WriteLine("Door:State[DevState.DoorCloseInit]DoorStop時、IoBoardDevice.SetUpperStateOfDOut()エラー");
					}
					//					Debug.Assert(funcRet != true, "Door:State[DevState.DoorCloseInit]DoorStop時、IoBoardDevice.SetUpperStateOfDOut()エラー");
					funcRet = IoBoardDevice.GetUpperStateOfSaveDIn(IoBoardDInLogicalName.DoorClose, out logicalState);
					if (funcRet != true)
					{
						Debug.WriteLine("Door:State[DoorCloseInit] IoBoardDevice.DirectIn()エラー");
					}
					//					Debug.Assert(funcRet != true, "Door:State[DoorCloseInit] IoBoardDevice.DirectIn()エラー");
#if false
							if (funcRet != true)
							{
								devDoor.devStateVal = DevState.DoorError;
								devDoor.devResultVal = DevResult.DoorError;
								break;
							}
#endif
					if (logicalState == true)
					{
						// Closeセンサ: onの時
						devDoor.devStateVal = DevState.DoorStop;
						devDoor.devResultVal = DevResult.DoorClosed;
						// Mainステート・マシンへ結果を出力: Close正常完了
						OpFlagCloseDoor = true;
						OpResultCloseDoor = DeviceResult.Done;
					}
					else
					{
						// Closeセンサ: offの時
						funcRet = IoBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.DoorClose);
						if (funcRet != true)
						{
							Debug.WriteLine("Door:State[DoorCloseInit]Closeセンサ==off時、IoBoardDevice.SetUpperStateOfDOut()エラー");
						}
						//						Debug.Assert(funcRet != true, "Door:State[DoorCloseInit]Closeセンサ==off時、IoBoardDevice.SetUpperStateOfDOut()エラー");
#if false
								if (funcRet != true)
								{
									devDoor.devStateVal = DevState.DoorError;
									devDoor.devResultVal = DevResult.DoorError;
									break;
								}
#endif

						devDoor.iPulseTimeCount = GetTimerCountFromInterval(PreferencesDatOriginal.iOutPulseOfDoorClose);   // 200[ms]
						devDoor.iTimeoutCount = GetTimerCountFromInterval(PreferencesDatOriginal.iTimeoutOfDoorClose * 1000);   // 6[s]
						devDoor.devStateVal = DevState.DoorClosePulseOn;
					}
					break;
				case (int)DevState.DoorClosePulseOn:
					funcRet = IoBoardDevice.GetUpperStateOfSaveDIn(IoBoardDInLogicalName.DoorClose, out logicalState);
					if (funcRet != true)
					{
						Debug.WriteLine("Door:State[DoorClosePulseOn] IoBoardDevice.GetUpperStateOfSaveDIn()エラー");
					}
					//					Debug.Assert(funcRet != true, "Door:State[DoorClosePulseOn] IoBoardDevice.GetUpperStateOfSaveDIn()エラー");
					if (logicalState == true)
					{
						// Closeセンサ: onの時
						funcRet = IoBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.DoorStop);
						if (funcRet != true)
						{
							Debug.WriteLine("Door:State[DoorClosePulseOn]Openセンサ==on時、IoBoardDevice.SetUpperStateOfDOut()エラー");
						}
						//						Debug.Assert(funcRet != true, "Door:State[DoorClosePulseOn]Openセンサ==on時、IoBoardDevice.SetUpperStateOfDOut()エラー");
						devDoor.devStateVal = DevState.DoorStop;
						devDoor.devResultVal = DevResult.DoorClosed;
						// Mainステート・マシンへ結果を出力: Close正常完了
						OpFlagCloseDoor = true;
						OpResultCloseDoor = DeviceResult.Done;
					}
					else
					{
						// Closeセンサ: offの時
						devDoor.iTimeoutCount--;
						if (devDoor.iTimeoutCount <= 0)
						{
							funcRet = IoBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.DoorStop);
							if (funcRet != true)
							{
								Debug.WriteLine("Door:State[DoorClosePulseOn]Closeセンサ==off→タイムアウト時、IoBoardDevice.SetUpperStateOfDOut()エラー");
							}
							//							Debug.Assert(funcRet != true, "Door:State[DoorClosePulseOn]Closeセンサ==off→タイムアウト時、IoBoardDevice.SetUpperStateOfDOut()エラー");
							devDoor.devStateVal = DevState.DoorStop;
							devDoor.devResultVal = DevResult.DoorCloseTimeout;
							// Mainステート・マシンへ結果を出力: Closeタイムアウト
							OpFlagCloseDoor = true;
							OpResultCloseDoor = DeviceResult.TimeOut;
							break;
						}
						devDoor.iPulseTimeCount--;
						if (devDoor.iPulseTimeCount <= 0)
						{
							funcRet = IoBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.DoorStop);
							if (funcRet != true)
							{
								Debug.WriteLine("Door:State[DoorClosePulseOn]Openセンサ==off→パルスカウント・フル時、IoBoardDevice.SetUpperStateOfDOut()エラー");
							}
							//							Debug.Assert(funcRet != true, "Door:State[DoorClosePulseOn]Openセンサ==off→パルスカウント・フル時、IoBoardDevice.SetUpperStateOfDOut()エラー");
							devDoor.devStateVal = DevState.DoorClosing;
						}
					}
					break;
				case (int)DevState.DoorClosing:
					funcRet = IoBoardDevice.GetUpperStateOfSaveDIn(IoBoardDInLogicalName.DoorClose, out logicalState);
					if (funcRet != true)
					{
						Debug.WriteLine("Door:State[DoorClosing]IoBoardDevice.GetUpperStateOfSaveDIn()エラー");
					}
					//					Debug.Assert(funcRet != true, "Door:State[DoorClosing]IoBoardDevice.GetUpperStateOfSaveDIn()エラー");
					if (logicalState == true)
					{
						// Closeセンサ: onの時
						funcRet = IoBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.DoorStop);
						if (funcRet != true)
						{
							Debug.WriteLine("Door:State[DoorClosing]Openセンサ==on時、IoBoardDevice.SetUpperStateOfDOut()エラー");
						}
						//						Debug.Assert(funcRet != true, "Door:State[DoorClosing]Openセンサ==on時、IoBoardDevice.SetUpperStateOfDOut()エラー");
						devDoor.devStateVal = DevState.DoorStop;
						devDoor.devResultVal = DevResult.DoorClosed;
						// Mainステート・マシンへ結果を出力: Close正常完了
						OpFlagCloseDoor = true;
						OpResultCloseDoor = DeviceResult.Done;
					}
					else
					{
						// Closeセンサ: offの時
						devDoor.iTimeoutCount--;
						if (devDoor.iTimeoutCount <= 0)
						{
							funcRet = IoBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.DoorStop);
							if (funcRet != true)
							{
								Debug.WriteLine("Door:State[DoorCloseing]Openセンサ==off時、IoBoardDevice.SetUpperStateOfDOut()エラー");
							}
							//							Debug.Assert(funcRet != true, "Door:State[DoorCloseing]Openセンサ==off時、IoBoardDevice.SetUpperStateOfDOut()エラー");
							devDoor.devStateVal = DevState.DoorStop;
							devDoor.devResultVal = DevResult.DoorCloseTimeout;
							// Mainステート・マシンへ結果を出力: Closeタイムアウト
							OpFlagCloseDoor = true;
							OpResultCloseDoor = DeviceResult.TimeOut;
							break;
						}
					}
					break;
				case (int)DevState.DoorOpenTimeout:
				case (int)DevState.DoorError:
					funcRet = IoBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.DoorStop);
					if (funcRet != true)
					{
						Debug.WriteLine("Door:State[DoorOpenTimeout/DoorError] IoBoardDevice.SetUpperStateOfDOut()エラー");
					}
					//					Debug.Assert(funcRet != true, "Door:State[DoorOpenTimeout/DoorError] IoBoardDevice.SetUpperStateOfDOut()エラー");
					devDoor.devStateVal = DevState.DoorStop;
					break;
				default:
					break;
			}
		}

#endif
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
                    //					userControlCheckDeviceOnFormMain.textBoxDoutDoorStateOnGpDoorOnUcCheckDevice.Text = devDoor.devStateVal.ToString();
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
