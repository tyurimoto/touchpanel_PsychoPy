using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Windows.Forms;

namespace Compartment
{
    public partial class FormMain : Form
    {
        // ActuatorPhase: relative state offsets (compile-time constants for switch/case)
        // These match the offset from baseState for both Door (base=1) and Lever (base=500)
        private const int AP_INIT = 0;
        private const int AP_STOP = 1;
        private const int AP_STOP_INIT_AFTER_WAIT = 2;
        private const int AP_STOP_INIT_AFTER_WAIT_PULSE_OFF = 3;
        private const int AP_STOP_INIT = 4;
        private const int AP_STOP_WITH_FWD_PULSE_ON1 = 5;
        private const int AP_STOP_WITH_FWD_PULSE_OFF1 = 6;
        private const int AP_STOP_WITH_FWD_PULSE_ON2 = 7;
        private const int AP_STOP_WITH_REV_PULSE_OFF2 = 8;
        private const int AP_STOP_WITH_REV_PULSE_ON = 9;
        private const int AP_FWD_INIT_AFTER_WAIT = 10;
        private const int AP_FWD_INIT_AFTER_WAIT_PULSE_OFF = 11;
        private const int AP_FWD_INIT = 12;
        private const int AP_FWD_PULSE_ON1 = 13;
        private const int AP_FWD_PULSE_OFF1 = 14;
        private const int AP_FWD_PULSE_ON2 = 15;
        private const int AP_FWD_DOING = 16;
        private const int AP_FWD_RECOVER_INIT = 17;
        private const int AP_FWD_RECOVER_INIT_RETRY = 18;
        private const int AP_FWD_RECOVER_WITH_REV_PULSE_ON1 = 19;
        private const int AP_FWD_RECOVER_WITH_REV_PULSE_OFF1 = 20;
        private const int AP_FWD_RECOVER_WITH_REV_PULSE_ON2 = 21;
        private const int AP_FWD_RECOVER_WITH_REV_DOING = 22;
        private const int AP_FWD_RECOVER_WITH_FWD_PULSE_ON1 = 23;
        private const int AP_FWD_RECOVER_WITH_FWD_PULSE_OFF1 = 24;
        private const int AP_FWD_RECOVER_WITH_FWD_PULSE_ON2 = 25;
        private const int AP_FWD_RECOVER_WITH_FWD_DOING = 26;
        private const int AP_FWD_RECOVER_TIMEOUT_INIT = 27;
        private const int AP_FWD_RECOVER_TIMEOUT_WITH_REV_PULSE_ON1 = 28;
        private const int AP_FWD_RECOVER_TIMEOUT_WITH_REV_PULSE_OFF1 = 29;
        private const int AP_FWD_RECOVER_TIMEOUT_WITH_REV_PULSE_ON2 = 30;
        private const int AP_FWD_RECOVER_TIMEOUT_WITH_REV_DOING = 31;
        private const int AP_REV_INIT_AFTER_WAIT = 32;
        private const int AP_REV_INIT_AFTER_WAIT_PULSE_OFF = 33;
        private const int AP_REV_INIT = 34;
        private const int AP_REV_PULSE_ON1 = 35;
        private const int AP_REV_PULSE_OFF1 = 36;
        private const int AP_REV_PULSE_ON2 = 37;
        private const int AP_REV_DOING = 38;
        private const int AP_REV_RECOVER_INIT = 39;
        private const int AP_REV_RECOVER_INIT_RETRY = 40;
        private const int AP_REV_RECOVER_WITH_FWD_PULSE_ON1 = 41;
        private const int AP_REV_RECOVER_WITH_FWD_PULSE_OFF1 = 42;
        private const int AP_REV_RECOVER_WITH_FWD_PULSE_ON2 = 43;
        private const int AP_REV_RECOVER_WITH_FWD_DOING = 44;
        private const int AP_REV_RECOVER_WITH_REV_PULSE_ON1 = 45;
        private const int AP_REV_RECOVER_WITH_REV_PULSE_OFF1 = 46;
        private const int AP_REV_RECOVER_WITH_REV_PULSE_ON2 = 47;
        private const int AP_REV_RECOVER_WITH_REV_DOING = 48;
        private const int AP_REV_RECOVER_TIMEOUT_INIT = 49;
        private const int AP_REV_RECOVER_TIMEOUT_WITH_FWD_PULSE_ON1 = 50;
        private const int AP_REV_RECOVER_TIMEOUT_WITH_FWD_PULSE_OFF1 = 51;
        private const int AP_REV_RECOVER_TIMEOUT_WITH_FWD_PULSE_ON2 = 52;
        private const int AP_REV_RECOVER_TIMEOUT_WITH_FWD_DOING = 53;

        /// <summary>
        /// Configuration for a bi-directional actuator (Door or Lever).
        /// Holds all device-specific parameters so the state machine can be shared.
        /// </summary>
        private class ActuatorConfig
        {
            public Dev dev;
            public ConcurrentQueue<DevCmdPkt> cmdQueue;
            public EDevState baseState;
            public EDevCmd cmdForward, cmdReverse, cmdStop;
            public IoBoardDOutLogicalName outForward, outReverse, outStop;
            public IoBoardDInLogicalName inForward, inReverse;
            public int activePulseWidth, inactivePulseWidth;
            public int timeoutForward, timeoutReverse;              // seconds
            public int reverseTimeInRecoverForward;                 // ms
            public int forwardTimeInRecoverReverse;                 // ms
            public int retryNum;
            public bool recoverEnabled;
            public Action<bool> setFlagForward, setFlagReverse, setFlagStop;
            public Action<EDeviceResult> setResultForward, setResultReverse, setResultStop;
            public Action clearAllFlagsAndResults;                  // for CheckDevice mode
            public string deviceName;                               // for Debug.WriteLine
        }

        private void ActuatorInit(ActuatorConfig cfg)
        {
            cfg.dev.devStateVal = cfg.baseState;
            cfg.dev.devStateValSaved = EDevState.OverRange;
            cfg.dev.devCmdVal = EDevCmd.None;
            cfg.dev.devResultVal = EDevResult.None;
            cfg.dev.devResultValSaved = EDevResult.OverRange;
            cfg.dev.firstFlag = true;
        }

        private void ActuatorExe(ActuatorConfig cfg)
        {
            bool boolLogicalStateForward;
            bool boolLogicalStateReverse;
            bool boolFuncRet;
            DevCmdPkt devCmdPktLast = null;
            DevCmdPkt devCmdPktCur;
            EDevCmd devCmdCur = EDevCmd.None;
            string name = cfg.deviceName;

            // Read sensors
            boolFuncRet = ioBoardDevice.GetUpperStateOfSaveDIn(cfg.inForward, out boolLogicalStateForward);
            if (boolFuncRet != true)
            {
                Debug.WriteLine(name + ": IoBoardDevice.DirectIn(" + cfg.inForward + ")エラー");
            }
            boolFuncRet = ioBoardDevice.GetUpperStateOfSaveDIn(cfg.inReverse, out boolLogicalStateReverse);
            if (boolFuncRet != true)
            {
                Debug.WriteLine(name + ": IoBoardDevice.DirectIn(" + cfg.inReverse + ")エラー");
            }

            // Dequeue commands: last command wins
            while (cfg.cmdQueue.TryDequeue(out devCmdPktCur))
            {
                devCmdPktLast = devCmdPktCur;
            }
            if (devCmdPktLast != null)
            {
                EDevCmd cmd = devCmdPktLast.DevCmdVal;
                if (cmd == cfg.cmdForward || cmd == cfg.cmdReverse || cmd == cfg.cmdStop)
                {
                    devCmdCur = cmd;
                }
            }

            // Helper lambdas for state manipulation
            int phase = (int)cfg.dev.devStateVal - (int)cfg.baseState;
            Action<int> setPhase = (p) => { cfg.dev.devStateVal = (EDevState)((int)cfg.baseState + p); };

            // --- First switch: command dispatch (preemption) ---
            if (phase == AP_INIT || phase == AP_STOP)
            {
                if (devCmdCur == cfg.cmdStop)
                {
                    cfg.setFlagStop(true);
                    cfg.setResultStop(EDeviceResult.Done);
                    if (opeModeTypeVal == EOpeModeType.CheckDevice)
                    {
                        cfg.clearAllFlagsAndResults();
                    }
                }
                else if (devCmdCur == cfg.cmdForward)
                {
                    boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(cfg.outStop);
                    if (boolFuncRet != true) Debug.WriteLine("Cmd.Forward[InStop] SetUpperStateOfDOut error");
                    setPhase(AP_FWD_INIT);
                    if (opeModeTypeVal == EOpeModeType.CheckDevice) cfg.clearAllFlagsAndResults();
                }
                else if (devCmdCur == cfg.cmdReverse)
                {
                    boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(cfg.outStop);
                    if (boolFuncRet != true) Debug.WriteLine("Cmd.Reverse[InStop] SetUpperStateOfDOut error");
                    setPhase(AP_REV_INIT);
                    if (opeModeTypeVal == EOpeModeType.CheckDevice) cfg.clearAllFlagsAndResults();
                }
            }
            else if (phase >= AP_STOP_INIT_AFTER_WAIT && phase <= AP_STOP_WITH_REV_PULSE_ON)
            {
                // In stop sequence: accept Forward or Reverse
                if (devCmdCur == cfg.cmdForward)
                {
                    boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(cfg.outStop);
                    if (boolFuncRet != true) Debug.WriteLine("Cmd.Forward[InStopSeq] SetUpperStateOfDOut error");
                    setPhase(AP_FWD_INIT_AFTER_WAIT);
                    if (opeModeTypeVal == EOpeModeType.CheckDevice) cfg.clearAllFlagsAndResults();
                }
                else if (devCmdCur == cfg.cmdReverse)
                {
                    boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(cfg.outStop);
                    if (boolFuncRet != true) Debug.WriteLine("Cmd.Reverse[InStopSeq] SetUpperStateOfDOut error");
                    setPhase(AP_REV_INIT_AFTER_WAIT);
                    if (opeModeTypeVal == EOpeModeType.CheckDevice) cfg.clearAllFlagsAndResults();
                }
            }
            else if (phase >= AP_FWD_INIT_AFTER_WAIT && phase <= AP_FWD_RECOVER_TIMEOUT_WITH_REV_DOING)
            {
                // In forward sequence: accept Reverse or Stop
                if (devCmdCur == cfg.cmdReverse)
                {
                    boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(cfg.outStop);
                    if (boolFuncRet != true) Debug.WriteLine("Cmd.Reverse[InFwd] SetUpperStateOfDOut error");
                    setPhase(AP_REV_INIT_AFTER_WAIT);
                    if (opeModeTypeVal == EOpeModeType.CheckDevice) cfg.clearAllFlagsAndResults();
                }
                else if (devCmdCur == cfg.cmdStop)
                {
                    boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(cfg.outStop);
                    if (boolFuncRet != true) Debug.WriteLine("Cmd.Stop[InFwd] SetUpperStateOfDOut error");
                    setPhase(AP_STOP_INIT_AFTER_WAIT);
                    if (opeModeTypeVal == EOpeModeType.CheckDevice) cfg.clearAllFlagsAndResults();
                }
            }
            else if (phase >= AP_REV_INIT_AFTER_WAIT && phase <= AP_REV_RECOVER_TIMEOUT_WITH_FWD_DOING)
            {
                // In reverse sequence: accept Forward or Stop
                if (devCmdCur == cfg.cmdForward)
                {
                    boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(cfg.outStop);
                    if (boolFuncRet != true) Debug.WriteLine("Cmd.Forward[InRev] SetUpperStateOfDOut error");
                    setPhase(AP_FWD_INIT_AFTER_WAIT);
                    if (opeModeTypeVal == EOpeModeType.CheckDevice) cfg.clearAllFlagsAndResults();
                }
                else if (devCmdCur == cfg.cmdStop)
                {
                    boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(cfg.outStop);
                    if (boolFuncRet != true) Debug.WriteLine("Cmd.Stop[InRev] SetUpperStateOfDOut error");
                    setPhase(AP_STOP_INIT_AFTER_WAIT);
                    if (opeModeTypeVal == EOpeModeType.CheckDevice) cfg.clearAllFlagsAndResults();
                }
            }

            // Recompute phase after possible command preemption
            phase = (int)cfg.dev.devStateVal - (int)cfg.baseState;

            // --- Second switch: state execution ---
            switch (phase)
            {
                case AP_INIT:
                    setPhase(AP_STOP);
                    break;
                case AP_STOP:
                    break;

                #region Stop sequence
                case AP_STOP_INIT_AFTER_WAIT:
                    boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(cfg.outStop);
                    if (boolFuncRet != true) Debug.WriteLine(name + ":State[StopInitAfterWait] SetUpperStateOfDOut(Stop)エラー");
                    if (boolLogicalStateForward || boolLogicalStateReverse)
                    {
                        cfg.setFlagStop(true);
                        cfg.setResultStop(EDeviceResult.Done);
                        setPhase(AP_STOP);
                    }
                    else
                    {
                        cfg.dev.stopwatchPulse.Restart();
                        setPhase(AP_STOP_INIT_AFTER_WAIT_PULSE_OFF);
                    }
                    break;
                case AP_STOP_INIT_AFTER_WAIT_PULSE_OFF:
                    if (boolLogicalStateForward || boolLogicalStateReverse)
                    {
                        cfg.setFlagStop(true);
                        cfg.setResultStop(EDeviceResult.Done);
                        setPhase(AP_STOP);
                    }
                    else if (cfg.dev.stopwatchPulse.ElapsedMilliseconds >= cfg.inactivePulseWidth)
                    {
                        cfg.dev.stopwatchPulse.Stop();
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(cfg.outForward);
                        if (boolFuncRet != true) Debug.WriteLine(name + ":State[StopInitAfterWaitPulseOff] SetUpperStateOfDOut(Fwd)エラー");
                        cfg.dev.stopwatchPulse.Restart();
                        setPhase(AP_STOP_WITH_FWD_PULSE_ON1);
                    }
                    break;
                case AP_STOP_INIT:
                    boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(cfg.outStop);
                    if (boolFuncRet != true) Debug.WriteLine(name + ":State[StopInit] SetUpperStateOfDOut(Stop)エラー");
                    if (boolLogicalStateForward || boolLogicalStateReverse)
                    {
                        cfg.setFlagStop(true);
                        cfg.setResultStop(EDeviceResult.Done);
                        setPhase(AP_STOP);
                    }
                    else
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(cfg.outForward);
                        if (boolFuncRet != true) Debug.WriteLine(name + ":State[StopInit] SetUpperStateOfDOut(Fwd)エラー");
                        cfg.dev.stopwatchPulse.Restart();
                        setPhase(AP_STOP_WITH_FWD_PULSE_ON1);
                    }
                    break;
                case AP_STOP_WITH_FWD_PULSE_ON1:
                    if (boolLogicalStateForward || boolLogicalStateReverse)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(cfg.outStop);
                        if (boolFuncRet != true) Debug.WriteLine(name + ":State[StopWithFwdPulseOn1] SetUpperStateOfDOut(Stop)エラー");
                        cfg.setFlagStop(true);
                        cfg.setResultStop(EDeviceResult.Done);
                        setPhase(AP_STOP);
                    }
                    else if (cfg.dev.stopwatchPulse.ElapsedMilliseconds >= cfg.activePulseWidth)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(cfg.outStop);
                        if (boolFuncRet != true) Debug.WriteLine(name + ":State[StopWithFwdPulseOn1] SetUpperStateOfDOut(Stop)エラー");
                        cfg.dev.stopwatchPulse.Restart();
                        setPhase(AP_STOP_WITH_FWD_PULSE_OFF1);
                    }
                    break;
                case AP_STOP_WITH_FWD_PULSE_OFF1:
                    if (boolLogicalStateForward || boolLogicalStateReverse)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(cfg.outStop);
                        if (boolFuncRet != true) Debug.WriteLine(name + ":State[StopWithFwdPulseOff1] SetUpperStateOfDOut(Stop)エラー");
                        cfg.setFlagStop(true);
                        cfg.setResultStop(EDeviceResult.Done);
                        setPhase(AP_STOP);
                    }
                    else if (cfg.dev.stopwatchPulse.ElapsedMilliseconds >= cfg.inactivePulseWidth)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(cfg.outForward);
                        if (boolFuncRet != true) Debug.WriteLine(name + ":State[StopWithFwdPulseOff1] SetUpperStateOfDOut(Fwd)エラー");
                        cfg.dev.stopwatchPulse.Restart();
                        setPhase(AP_STOP_WITH_FWD_PULSE_ON2);
                    }
                    break;
                case AP_STOP_WITH_FWD_PULSE_ON2:
                    if (boolLogicalStateForward || boolLogicalStateReverse)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(cfg.outStop);
                        if (boolFuncRet != true) Debug.WriteLine(name + ":State[StopWithFwdPulseOn2] SetUpperStateOfDOut(Stop)エラー");
                        cfg.setFlagStop(true);
                        cfg.setResultStop(EDeviceResult.Done);
                        setPhase(AP_STOP);
                    }
                    else if (cfg.dev.stopwatchPulse.ElapsedMilliseconds >= cfg.activePulseWidth)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(cfg.outStop);
                        if (boolFuncRet != true) Debug.WriteLine(name + ":State[StopWithFwdPulseOn2] SetUpperStateOfDOut(Stop)エラー");
                        cfg.dev.stopwatchPulse.Restart();
                        setPhase(AP_STOP_WITH_REV_PULSE_OFF2);
                    }
                    break;
                case AP_STOP_WITH_REV_PULSE_OFF2:
                    if (boolLogicalStateForward || boolLogicalStateReverse)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(cfg.outStop);
                        if (boolFuncRet != true) Debug.WriteLine(name + ":State[StopWithRevPulseOff2] SetUpperStateOfDOut(Stop)エラー");
                        cfg.setFlagStop(true);
                        cfg.setResultStop(EDeviceResult.Done);
                        setPhase(AP_STOP);
                    }
                    else if (cfg.dev.stopwatchPulse.ElapsedMilliseconds >= cfg.inactivePulseWidth)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(cfg.outReverse);
                        if (boolFuncRet != true) Debug.WriteLine(name + ":State[StopWithRevPulseOff2] SetUpperStateOfDOut(Rev)エラー");
                        cfg.dev.stopwatchPulse.Restart();
                        setPhase(AP_STOP_WITH_REV_PULSE_ON);
                    }
                    break;
                case AP_STOP_WITH_REV_PULSE_ON:
                    if (boolLogicalStateForward || boolLogicalStateReverse)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(cfg.outStop);
                        if (boolFuncRet != true) Debug.WriteLine(name + ":State[StopWithRevPulseOn] SetUpperStateOfDOut(Stop)エラー");
                        cfg.setFlagStop(true);
                        cfg.setResultStop(EDeviceResult.Done);
                        setPhase(AP_STOP);
                    }
                    else if (cfg.dev.stopwatchPulse.ElapsedMilliseconds >= cfg.activePulseWidth)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(cfg.outStop);
                        if (boolFuncRet != true) Debug.WriteLine(name + ":State[StopWithRevPulseOn] SetUpperStateOfDOut(Stop)エラー");
                        cfg.setFlagStop(true);
                        cfg.setResultStop(EDeviceResult.Done);
                        setPhase(AP_STOP);
                        cfg.dev.stopwatchPulse.Stop();
                    }
                    break;
                #endregion

                #region Forward sequence
                case AP_FWD_INIT_AFTER_WAIT:
                    boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(cfg.outStop);
                    if (boolFuncRet != true) Debug.WriteLine(name + ":State[FwdInitAfterWait] SetUpperStateOfDOut(Stop)エラー");
                    if (boolLogicalStateForward)
                    {
                        cfg.setFlagForward(true);
                        cfg.setResultForward(EDeviceResult.Done);
                        setPhase(AP_STOP);
                    }
                    else
                    {
                        cfg.dev.stopwatchPulse.Restart();
                        setPhase(AP_FWD_INIT_AFTER_WAIT_PULSE_OFF);
                    }
                    break;
                case AP_FWD_INIT_AFTER_WAIT_PULSE_OFF:
                    if (boolLogicalStateForward)
                    {
                        cfg.setFlagForward(true);
                        cfg.setResultForward(EDeviceResult.Done);
                        setPhase(AP_STOP);
                    }
                    else if (cfg.dev.stopwatchPulse.ElapsedMilliseconds >= cfg.inactivePulseWidth)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(cfg.outForward);
                        if (boolFuncRet != true) Debug.WriteLine(name + ":State[FwdInitAfterWaitPulseOff] SetUpperStateOfDOut(Fwd)エラー");
                        cfg.dev.stopwatchPulse.Restart();
                        cfg.dev.stopwatchTimeout.Restart();
                        setPhase(AP_FWD_PULSE_ON1);
                    }
                    break;
                case AP_FWD_INIT:
                    boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(cfg.outStop);
                    if (boolFuncRet != true) Debug.WriteLine(name + ":State[FwdInit] SetUpperStateOfDOut(Stop)エラー");
                    if (boolLogicalStateForward)
                    {
                        cfg.setFlagForward(true);
                        cfg.setResultForward(EDeviceResult.Done);
                        cfg.dev.stopwatchPulse.Stop();
                        setPhase(AP_STOP);
                    }
                    else
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(cfg.outForward);
                        if (boolFuncRet != true) Debug.WriteLine(name + ":State[FwdInit] SetUpperStateOfDOut(Fwd)エラー");
                        cfg.dev.stopwatchPulse.Restart();
                        cfg.dev.stopwatchTimeout.Restart();
                        setPhase(AP_FWD_PULSE_ON1);
                    }
                    break;
                case AP_FWD_PULSE_ON1:
                    if (boolLogicalStateForward)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(cfg.outStop);
                        if (boolFuncRet != true) Debug.WriteLine(name + ":State[FwdPulseOn1] SetUpperStateOfDOut(Stop)エラー");
                        cfg.setFlagForward(true);
                        cfg.setResultForward(EDeviceResult.Done);
                        setPhase(AP_STOP);
                    }
                    else if (cfg.dev.stopwatchPulse.ElapsedMilliseconds >= cfg.activePulseWidth)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(cfg.outStop);
                        if (boolFuncRet != true) Debug.WriteLine(name + ":State[FwdPulseOn1] SetUpperStateOfDOut(Stop)エラー");
                        cfg.dev.stopwatchPulse.Restart();
                        setPhase(AP_FWD_PULSE_OFF1);
                    }
                    break;
                case AP_FWD_PULSE_OFF1:
                    if (boolLogicalStateForward)
                    {
                        cfg.setFlagForward(true);
                        cfg.setResultForward(EDeviceResult.Done);
                        setPhase(AP_STOP);
                    }
                    else if (cfg.dev.stopwatchPulse.ElapsedMilliseconds >= cfg.inactivePulseWidth)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(cfg.outForward);
                        if (boolFuncRet != true) Debug.WriteLine(name + ":State[FwdPulseOff1] SetUpperStateOfDOut(Fwd)エラー");
                        cfg.dev.stopwatchPulse.Restart();
                        setPhase(AP_FWD_PULSE_ON2);
                    }
                    break;
                case AP_FWD_PULSE_ON2:
                    if (boolLogicalStateForward)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(cfg.outStop);
                        if (boolFuncRet != true) Debug.WriteLine(name + ":State[FwdPulseOn2] SetUpperStateOfDOut(Stop)エラー");
                        cfg.setFlagForward(true);
                        cfg.setResultForward(EDeviceResult.Done);
                        setPhase(AP_STOP);
                    }
                    else if (cfg.dev.stopwatchPulse.ElapsedMilliseconds >= cfg.activePulseWidth)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(cfg.outStop);
                        if (boolFuncRet != true) Debug.WriteLine(name + ":State[FwdPulseOn2] SetUpperStateOfDOut(Stop)エラー");
                        cfg.dev.stopwatchPulse.Stop();
                        setPhase(AP_FWD_DOING);
                    }
                    break;
                case AP_FWD_DOING:
                    if (boolLogicalStateForward)
                    {
                        cfg.setFlagForward(true);
                        cfg.setResultForward(EDeviceResult.Done);
                        setPhase(AP_STOP);
                    }
                    else if (cfg.dev.stopwatchTimeout.ElapsedMilliseconds >= (cfg.timeoutForward * 1000))
                    {
                        if (cfg.recoverEnabled)
                        {
                            boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(cfg.outReverse);
                            if (boolFuncRet != true) Debug.WriteLine(name + ":State[FwdDoing] SetUpperStateOfDOut(Rev)エラー");
                            cfg.dev.retryCount = 0;
                            cfg.dev.stopwatchPulse.Restart();
                            cfg.dev.stopwatchTimeout.Restart();
                            setPhase(AP_FWD_RECOVER_WITH_REV_PULSE_ON1);
                        }
                        else
                        {
                            cfg.dev.stopwatchPulse.Stop();
                            cfg.dev.stopwatchTimeout.Stop();
                            cfg.setFlagForward(true);
                            cfg.setResultForward(EDeviceResult.TimeOut);
                            setPhase(AP_STOP);
                        }
                    }
                    break;
                case AP_FWD_RECOVER_INIT:
                    if (boolLogicalStateForward)
                    {
                        cfg.setFlagForward(true);
                        cfg.setResultForward(EDeviceResult.Done);
                        setPhase(AP_STOP);
                    }
                    else
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(cfg.outStop);
                        if (boolFuncRet != true) Debug.WriteLine(name + ":State[FwdRecoverInit] SetUpperStateOfDOut(Stop)エラー");
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(cfg.outReverse);
                        if (boolFuncRet != true) Debug.WriteLine(name + ":State[FwdRecoverInit] SetUpperStateOfDOut(Rev)エラー");
                        cfg.dev.retryCount = 0;
                        cfg.dev.stopwatchPulse.Restart();
                        setPhase(AP_FWD_RECOVER_WITH_REV_PULSE_ON1);
                    }
                    break;
                case AP_FWD_RECOVER_INIT_RETRY:
                    break;
                case AP_FWD_RECOVER_WITH_REV_PULSE_ON1:
                    if (cfg.dev.stopwatchPulse.ElapsedMilliseconds >= cfg.activePulseWidth)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(cfg.outStop);
                        if (boolFuncRet != true) Debug.WriteLine(name + ":State[FwdRecoverWithRevPulseOn1] SetUpperStateOfDOut(Stop)エラー");
                        cfg.dev.stopwatchPulse.Restart();
                        setPhase(AP_FWD_RECOVER_WITH_REV_PULSE_OFF1);
                    }
                    break;
                case AP_FWD_RECOVER_WITH_REV_PULSE_OFF1:
                    if (cfg.dev.stopwatchPulse.ElapsedMilliseconds >= cfg.inactivePulseWidth)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(cfg.outReverse);
                        if (boolFuncRet != true) Debug.WriteLine(name + ":State[FwdRecoverWithRevPulseOff1] SetUpperStateOfDOut(Rev)エラー");
                        cfg.dev.stopwatchPulse.Restart();
                        setPhase(AP_FWD_RECOVER_WITH_REV_PULSE_ON2);
                    }
                    break;
                case AP_FWD_RECOVER_WITH_REV_PULSE_ON2:
                    if (cfg.dev.stopwatchPulse.ElapsedMilliseconds >= cfg.activePulseWidth)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(cfg.outStop);
                        if (boolFuncRet != true) Debug.WriteLine(name + ":State[FwdRecoverWithRevPulseOn2] SetUpperStateOfDOut(Stop)エラー");
                        cfg.dev.stopwatchPulse.Stop();
                        cfg.dev.stopwatchTimeout.Restart();
                        setPhase(AP_FWD_RECOVER_WITH_REV_DOING);
                    }
                    break;
                case AP_FWD_RECOVER_WITH_REV_DOING:
                    if (boolLogicalStateReverse ||
                        cfg.dev.stopwatchTimeout.ElapsedMilliseconds >= cfg.reverseTimeInRecoverForward)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(cfg.outForward);
                        if (boolFuncRet != true) Debug.WriteLine(name + ":State[FwdRecoverWithRevDoing] SetUpperStateOfDOut(Fwd)エラー");
                        cfg.dev.stopwatchPulse.Restart();
                        cfg.dev.stopwatchTimeout.Restart();
                        setPhase(AP_FWD_RECOVER_WITH_FWD_PULSE_ON1);
                    }
                    break;
                case AP_FWD_RECOVER_WITH_FWD_PULSE_ON1:
                    if (boolLogicalStateForward)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(cfg.outStop);
                        if (boolFuncRet != true) Debug.WriteLine(name + ":State[FwdRecoverWithFwdPulseOn1] SetUpperStateOfDOut(Stop)エラー");
                        cfg.setFlagForward(true);
                        cfg.setResultForward(EDeviceResult.Done);
                        cfg.dev.stopwatchPulse.Stop();
                        cfg.dev.stopwatchTimeout.Stop();
                        setPhase(AP_STOP);
                    }
                    else if (cfg.dev.stopwatchPulse.ElapsedMilliseconds >= cfg.activePulseWidth)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(cfg.outStop);
                        if (boolFuncRet != true) Debug.WriteLine(name + ":State[FwdRecoverWithFwdPulseOn1] SetUpperStateOfDOut(Stop)エラー");
                        cfg.dev.stopwatchPulse.Restart();
                        setPhase(AP_FWD_RECOVER_WITH_FWD_PULSE_OFF1);
                    }
                    break;
                case AP_FWD_RECOVER_WITH_FWD_PULSE_OFF1:
                    if (boolLogicalStateForward)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(cfg.outStop);
                        if (boolFuncRet != true) Debug.WriteLine(name + ":State[FwdRecoverWithFwdPulseOff1] SetUpperStateOfDOut(Stop)エラー");
                        cfg.setFlagForward(true);
                        cfg.setResultForward(EDeviceResult.Done);
                        cfg.dev.stopwatchPulse.Stop();
                        cfg.dev.stopwatchTimeout.Stop();
                        setPhase(AP_STOP);
                    }
                    else if (cfg.dev.stopwatchPulse.ElapsedMilliseconds >= cfg.inactivePulseWidth)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(cfg.outForward);
                        if (boolFuncRet != true) Debug.WriteLine(name + ":State[FwdRecoverWithFwdPulseOff1] SetUpperStateOfDOut(Fwd)エラー");
                        cfg.dev.stopwatchPulse.Restart();
                        setPhase(AP_FWD_RECOVER_WITH_FWD_PULSE_ON2);
                    }
                    break;
                case AP_FWD_RECOVER_WITH_FWD_PULSE_ON2:
                    if (boolLogicalStateForward)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(cfg.outStop);
                        if (boolFuncRet != true) Debug.WriteLine(name + ":State[FwdRecoverWithFwdPulseOn2] SetUpperStateOfDOut(Stop)エラー");
                        cfg.setFlagForward(true);
                        cfg.setResultForward(EDeviceResult.Done);
                        cfg.dev.stopwatchPulse.Stop();
                        cfg.dev.stopwatchTimeout.Stop();
                        setPhase(AP_STOP);
                    }
                    else if (cfg.dev.stopwatchPulse.ElapsedMilliseconds >= cfg.activePulseWidth)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(cfg.outStop);
                        if (boolFuncRet != true) Debug.WriteLine(name + ":State[FwdRecoverWithFwdPulseOn2] SetUpperStateOfDOut(Stop)エラー");
                        cfg.dev.stopwatchPulse.Stop();
                        setPhase(AP_FWD_RECOVER_WITH_FWD_DOING);
                    }
                    break;
                case AP_FWD_RECOVER_WITH_FWD_DOING:
                    if (boolLogicalStateForward)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(cfg.outStop);
                        if (boolFuncRet != true) Debug.WriteLine(name + ":State[FwdRecoverWithFwdDoing] SetUpperStateOfDOut(Stop)エラー");
                        cfg.setFlagForward(true);
                        cfg.setResultForward(EDeviceResult.Done);
                        cfg.dev.stopwatchPulse.Stop();
                        cfg.dev.stopwatchTimeout.Stop();
                        setPhase(AP_STOP);
                    }
                    else if (cfg.dev.stopwatchTimeout.ElapsedMilliseconds >= (cfg.reverseTimeInRecoverForward * 2))
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(cfg.outStop);
                        if (boolFuncRet != true) Debug.WriteLine(name + ":State[FwdRecoverWithFwdDoing] SetUpperStateOfDOut(Stop)エラー");
                        cfg.dev.stopwatchTimeout.Stop();
                        cfg.dev.retryCount++;
                        if (cfg.dev.retryCount > cfg.retryNum)
                        {
                            boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(cfg.outReverse);
                            if (boolFuncRet != true) Debug.WriteLine(name + ":State[FwdRecoverWithFwdDoing] SetUpperStateOfDOut(Rev)エラー");
                            cfg.dev.stopwatchPulse.Restart();
                            cfg.dev.stopwatchTimeout.Restart();
                            setPhase(AP_FWD_RECOVER_TIMEOUT_WITH_REV_PULSE_ON1);
                        }
                        else
                        {
                            boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(cfg.outReverse);
                            if (boolFuncRet != true) Debug.WriteLine(name + ":State[FwdRecoverWithFwdDoing] SetUpperStateOfDOut(Rev)エラー");
                            cfg.dev.stopwatchPulse.Restart();
                            cfg.dev.stopwatchTimeout.Restart();
                            setPhase(AP_FWD_RECOVER_WITH_REV_PULSE_ON1);
                        }
                    }
                    break;
                case AP_FWD_RECOVER_TIMEOUT_INIT:
                    break;
                case AP_FWD_RECOVER_TIMEOUT_WITH_REV_PULSE_ON1:
                    if (cfg.dev.stopwatchPulse.ElapsedMilliseconds >= cfg.activePulseWidth)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(cfg.outStop);
                        if (boolFuncRet != true) Debug.WriteLine(name + ":State[FwdRecoverTimeoutWithRevPulseOn1] SetUpperStateOfDOut(Stop)エラー");
                        cfg.dev.stopwatchPulse.Restart();
                        setPhase(AP_FWD_RECOVER_TIMEOUT_WITH_REV_PULSE_OFF1);
                    }
                    break;
                case AP_FWD_RECOVER_TIMEOUT_WITH_REV_PULSE_OFF1:
                    if (cfg.dev.stopwatchPulse.ElapsedMilliseconds >= cfg.inactivePulseWidth)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(cfg.outReverse);
                        if (boolFuncRet != true) Debug.WriteLine(name + ":State[FwdRecoverTimeoutWithRevPulseOff1] SetUpperStateOfDOut(Rev)エラー");
                        cfg.dev.stopwatchPulse.Restart();
                        setPhase(AP_FWD_RECOVER_TIMEOUT_WITH_REV_PULSE_ON2);
                    }
                    break;
                case AP_FWD_RECOVER_TIMEOUT_WITH_REV_PULSE_ON2:
                    if (cfg.dev.stopwatchPulse.ElapsedMilliseconds >= cfg.activePulseWidth)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(cfg.outStop);
                        if (boolFuncRet != true) Debug.WriteLine(name + ":State[FwdRecoverTimeoutWithRevPulseOn2] SetUpperStateOfDOut(Stop)エラー");
                        cfg.dev.stopwatchPulse.Stop();
                        setPhase(AP_FWD_RECOVER_TIMEOUT_WITH_REV_DOING);
                    }
                    break;
                case AP_FWD_RECOVER_TIMEOUT_WITH_REV_DOING:
                    if (cfg.dev.stopwatchTimeout.ElapsedMilliseconds >= cfg.reverseTimeInRecoverForward)
                    {
                        cfg.dev.stopwatchPulse.Stop();
                        cfg.dev.stopwatchTimeout.Stop();
                        cfg.setFlagForward(true);
                        cfg.setResultForward(EDeviceResult.TimeOut);
                        setPhase(AP_STOP);
                    }
                    break;
                #endregion

                #region Reverse sequence
                case AP_REV_INIT_AFTER_WAIT:
                    boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(cfg.outStop);
                    if (boolFuncRet != true) Debug.WriteLine(name + ":State[RevInitAfterWait] SetUpperStateOfDOut(Stop)エラー");
                    if (boolLogicalStateReverse)
                    {
                        cfg.setFlagReverse(true);
                        cfg.setResultReverse(EDeviceResult.Done);
                        setPhase(AP_STOP);
                    }
                    else
                    {
                        cfg.dev.stopwatchPulse.Restart();
                        setPhase(AP_REV_INIT_AFTER_WAIT_PULSE_OFF);
                    }
                    break;
                case AP_REV_INIT_AFTER_WAIT_PULSE_OFF:
                    if (boolLogicalStateReverse)
                    {
                        cfg.setFlagReverse(true);
                        cfg.setResultReverse(EDeviceResult.Done);
                        setPhase(AP_STOP);
                    }
                    else if (cfg.dev.stopwatchPulse.ElapsedMilliseconds >= cfg.inactivePulseWidth)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(cfg.outReverse);
                        if (boolFuncRet != true) Debug.WriteLine(name + ":State[RevInitAfterWaitPulseOff] SetUpperStateOfDOut(Rev)エラー");
                        cfg.dev.stopwatchPulse.Restart();
                        cfg.dev.stopwatchTimeout.Restart();
                        setPhase(AP_REV_PULSE_ON1);
                    }
                    break;
                case AP_REV_INIT:
                    boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(cfg.outStop);
                    if (boolFuncRet != true) Debug.WriteLine(name + ":State[RevInit] SetUpperStateOfDOut(Stop)エラー");
                    if (boolLogicalStateReverse)
                    {
                        cfg.setFlagReverse(true);
                        cfg.setResultReverse(EDeviceResult.Done);
                        cfg.dev.stopwatchPulse.Stop();
                        setPhase(AP_STOP);
                    }
                    else
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(cfg.outReverse);
                        if (boolFuncRet != true) Debug.WriteLine(name + ":State[RevInit] SetUpperStateOfDOut(Rev)エラー");
                        cfg.dev.stopwatchPulse.Restart();
                        cfg.dev.stopwatchTimeout.Restart();
                        setPhase(AP_REV_PULSE_ON1);
                    }
                    break;
                case AP_REV_PULSE_ON1:
                    if (boolLogicalStateReverse)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(cfg.outStop);
                        if (boolFuncRet != true) Debug.WriteLine(name + ":State[RevPulseOn1] SetUpperStateOfDOut(Stop)エラー");
                        cfg.setFlagReverse(true);
                        cfg.setResultReverse(EDeviceResult.Done);
                        setPhase(AP_STOP);
                    }
                    else if (cfg.dev.stopwatchPulse.ElapsedMilliseconds >= cfg.activePulseWidth)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(cfg.outStop);
                        if (boolFuncRet != true) Debug.WriteLine(name + ":State[RevPulseOn1] SetUpperStateOfDOut(Stop)エラー");
                        cfg.dev.stopwatchPulse.Restart();
                        setPhase(AP_REV_PULSE_OFF1);
                    }
                    break;
                case AP_REV_PULSE_OFF1:
                    if (boolLogicalStateReverse)
                    {
                        cfg.setFlagReverse(true);
                        cfg.setResultReverse(EDeviceResult.Done);
                        setPhase(AP_STOP);
                    }
                    else if (cfg.dev.stopwatchPulse.ElapsedMilliseconds >= cfg.inactivePulseWidth)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(cfg.outReverse);
                        if (boolFuncRet != true) Debug.WriteLine(name + ":State[RevPulseOff1] SetUpperStateOfDOut(Rev)エラー");
                        cfg.dev.stopwatchPulse.Restart();
                        setPhase(AP_REV_PULSE_ON2);
                    }
                    break;
                case AP_REV_PULSE_ON2:
                    if (boolLogicalStateReverse)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(cfg.outStop);
                        if (boolFuncRet != true) Debug.WriteLine(name + ":State[RevPulseOn2] SetUpperStateOfDOut(Stop)エラー");
                        cfg.setFlagReverse(true);
                        cfg.setResultReverse(EDeviceResult.Done);
                        setPhase(AP_STOP);
                    }
                    else if (cfg.dev.stopwatchPulse.ElapsedMilliseconds >= cfg.activePulseWidth)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(cfg.outStop);
                        if (boolFuncRet != true) Debug.WriteLine(name + ":State[RevPulseOn2] SetUpperStateOfDOut(Stop)エラー");
                        cfg.dev.stopwatchPulse.Stop();
                        setPhase(AP_REV_DOING);
                    }
                    break;
                case AP_REV_DOING:
                    if (boolLogicalStateReverse)
                    {
                        cfg.setFlagReverse(true);
                        cfg.setResultReverse(EDeviceResult.Done);
                        setPhase(AP_STOP);
                    }
                    else if (cfg.dev.stopwatchTimeout.ElapsedMilliseconds >= (cfg.timeoutReverse * 1000))
                    {
                        if (cfg.recoverEnabled)
                        {
                            boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(cfg.outForward);
                            if (boolFuncRet != true) Debug.WriteLine(name + ":State[RevDoing] SetUpperStateOfDOut(Fwd)エラー");
                            cfg.dev.retryCount = 0;
                            cfg.dev.stopwatchPulse.Restart();
                            cfg.dev.stopwatchTimeout.Restart();
                            setPhase(AP_REV_RECOVER_WITH_FWD_PULSE_ON1);
                        }
                        else
                        {
                            cfg.dev.stopwatchPulse.Stop();
                            cfg.dev.stopwatchTimeout.Stop();
                            cfg.setFlagReverse(true);
                            cfg.setResultReverse(EDeviceResult.TimeOut);
                            setPhase(AP_STOP);
                        }
                    }
                    break;
                case AP_REV_RECOVER_INIT:
                    if (boolLogicalStateReverse)
                    {
                        cfg.setFlagReverse(true);
                        cfg.setResultReverse(EDeviceResult.Done);
                        setPhase(AP_STOP);
                    }
                    else
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(cfg.outStop);
                        if (boolFuncRet != true) Debug.WriteLine(name + ":State[RevRecoverInit] SetUpperStateOfDOut(Stop)エラー");
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(cfg.outForward);
                        if (boolFuncRet != true) Debug.WriteLine(name + ":State[RevRecoverInit] SetUpperStateOfDOut(Fwd)エラー");
                        cfg.dev.retryCount = 0;
                        cfg.dev.stopwatchPulse.Restart();
                        setPhase(AP_REV_RECOVER_WITH_FWD_PULSE_ON1);
                    }
                    break;
                case AP_REV_RECOVER_INIT_RETRY:
                    break;
                case AP_REV_RECOVER_WITH_FWD_PULSE_ON1:
                    if (cfg.dev.stopwatchPulse.ElapsedMilliseconds >= cfg.activePulseWidth)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(cfg.outStop);
                        if (boolFuncRet != true) Debug.WriteLine(name + ":State[RevRecoverWithFwdPulseOn1] SetUpperStateOfDOut(Stop)エラー");
                        cfg.dev.stopwatchPulse.Restart();
                        setPhase(AP_REV_RECOVER_WITH_FWD_PULSE_OFF1);
                    }
                    break;
                case AP_REV_RECOVER_WITH_FWD_PULSE_OFF1:
                    if (cfg.dev.stopwatchPulse.ElapsedMilliseconds >= cfg.inactivePulseWidth)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(cfg.outForward);
                        if (boolFuncRet != true) Debug.WriteLine(name + ":State[RevRecoverWithFwdPulseOff1] SetUpperStateOfDOut(Fwd)エラー");
                        cfg.dev.stopwatchPulse.Restart();
                        setPhase(AP_REV_RECOVER_WITH_FWD_PULSE_ON2);
                    }
                    break;
                case AP_REV_RECOVER_WITH_FWD_PULSE_ON2:
                    if (cfg.dev.stopwatchPulse.ElapsedMilliseconds >= cfg.activePulseWidth)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(cfg.outStop);
                        if (boolFuncRet != true) Debug.WriteLine(name + ":State[RevRecoverWithFwdPulseOn2] SetUpperStateOfDOut(Stop)エラー");
                        cfg.dev.stopwatchPulse.Stop();
                        cfg.dev.stopwatchTimeout.Restart();
                        setPhase(AP_REV_RECOVER_WITH_FWD_DOING);
                    }
                    break;
                case AP_REV_RECOVER_WITH_FWD_DOING:
                    if (boolLogicalStateForward ||
                        cfg.dev.stopwatchTimeout.ElapsedMilliseconds >= cfg.forwardTimeInRecoverReverse)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(cfg.outReverse);
                        if (boolFuncRet != true) Debug.WriteLine(name + ":State[RevRecoverWithFwdDoing] SetUpperStateOfDOut(Rev)エラー");
                        cfg.dev.stopwatchPulse.Restart();
                        cfg.dev.stopwatchTimeout.Restart();
                        setPhase(AP_REV_RECOVER_WITH_REV_PULSE_ON1);
                    }
                    break;
                case AP_REV_RECOVER_WITH_REV_PULSE_ON1:
                    if (boolLogicalStateReverse)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(cfg.outStop);
                        if (boolFuncRet != true) Debug.WriteLine(name + ":State[RevRecoverWithRevPulseOn1] SetUpperStateOfDOut(Stop)エラー");
                        cfg.setFlagReverse(true);
                        cfg.setResultReverse(EDeviceResult.Done);
                        cfg.dev.stopwatchPulse.Stop();
                        cfg.dev.stopwatchTimeout.Stop();
                        setPhase(AP_STOP);
                    }
                    else if (cfg.dev.stopwatchPulse.ElapsedMilliseconds >= cfg.activePulseWidth)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(cfg.outStop);
                        if (boolFuncRet != true) Debug.WriteLine(name + ":State[RevRecoverWithRevPulseOn1] SetUpperStateOfDOut(Stop)エラー");
                        cfg.dev.stopwatchPulse.Restart();
                        setPhase(AP_REV_RECOVER_WITH_REV_PULSE_OFF1);
                    }
                    break;
                case AP_REV_RECOVER_WITH_REV_PULSE_OFF1:
                    if (boolLogicalStateReverse)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(cfg.outStop);
                        if (boolFuncRet != true) Debug.WriteLine(name + ":State[RevRecoverWithRevPulseOff1] SetUpperStateOfDOut(Stop)エラー");
                        cfg.setFlagReverse(true);
                        cfg.setResultReverse(EDeviceResult.Done);
                        cfg.dev.stopwatchPulse.Stop();
                        cfg.dev.stopwatchTimeout.Stop();
                        setPhase(AP_STOP);
                    }
                    else if (cfg.dev.stopwatchPulse.ElapsedMilliseconds >= cfg.inactivePulseWidth)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(cfg.outReverse);
                        if (boolFuncRet != true) Debug.WriteLine(name + ":State[RevRecoverWithRevPulseOff1] SetUpperStateOfDOut(Rev)エラー");
                        cfg.dev.stopwatchPulse.Restart();
                        setPhase(AP_REV_RECOVER_WITH_REV_PULSE_ON2);
                    }
                    break;
                case AP_REV_RECOVER_WITH_REV_PULSE_ON2:
                    if (boolLogicalStateReverse)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(cfg.outStop);
                        if (boolFuncRet != true) Debug.WriteLine(name + ":State[RevRecoverWithRevPulseOn2] SetUpperStateOfDOut(Stop)エラー");
                        cfg.setFlagReverse(true);
                        cfg.setResultReverse(EDeviceResult.Done);
                        cfg.dev.stopwatchPulse.Stop();
                        cfg.dev.stopwatchTimeout.Stop();
                        setPhase(AP_STOP);
                    }
                    else if (cfg.dev.stopwatchPulse.ElapsedMilliseconds >= cfg.activePulseWidth)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(cfg.outStop);
                        if (boolFuncRet != true) Debug.WriteLine(name + ":State[RevRecoverWithRevPulseOn2] SetUpperStateOfDOut(Stop)エラー");
                        cfg.dev.stopwatchPulse.Stop();
                        setPhase(AP_REV_RECOVER_WITH_REV_DOING);
                    }
                    break;
                case AP_REV_RECOVER_WITH_REV_DOING:
                    if (boolLogicalStateReverse)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(cfg.outStop);
                        if (boolFuncRet != true) Debug.WriteLine(name + ":State[RevRecoverWithRevDoing] SetUpperStateOfDOut(Stop)エラー");
                        cfg.setFlagReverse(true);
                        cfg.setResultReverse(EDeviceResult.Done);
                        cfg.dev.stopwatchPulse.Stop();
                        cfg.dev.stopwatchTimeout.Stop();
                        setPhase(AP_STOP);
                    }
                    else if (cfg.dev.stopwatchTimeout.ElapsedMilliseconds >= (cfg.forwardTimeInRecoverReverse * 2))
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(cfg.outStop);
                        if (boolFuncRet != true) Debug.WriteLine(name + ":State[RevRecoverWithRevDoing] SetUpperStateOfDOut(Stop)エラー");
                        cfg.dev.stopwatchTimeout.Stop();
                        cfg.dev.retryCount++;
                        if (cfg.dev.retryCount > cfg.retryNum)
                        {
                            boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(cfg.outForward);
                            if (boolFuncRet != true) Debug.WriteLine(name + ":State[RevRecoverWithRevDoing] SetUpperStateOfDOut(Fwd)エラー");
                            cfg.dev.stopwatchPulse.Restart();
                            cfg.dev.stopwatchTimeout.Restart();
                            setPhase(AP_REV_RECOVER_TIMEOUT_WITH_FWD_PULSE_ON1);
                        }
                        else
                        {
                            boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(cfg.outForward);
                            if (boolFuncRet != true) Debug.WriteLine(name + ":State[RevRecoverWithRevDoing] SetUpperStateOfDOut(Fwd)エラー");
                            cfg.dev.stopwatchPulse.Restart();
                            cfg.dev.stopwatchTimeout.Restart();
                            setPhase(AP_REV_RECOVER_WITH_FWD_PULSE_ON1);
                        }
                    }
                    break;
                case AP_REV_RECOVER_TIMEOUT_INIT:
                    break;
                case AP_REV_RECOVER_TIMEOUT_WITH_FWD_PULSE_ON1:
                    if (cfg.dev.stopwatchPulse.ElapsedMilliseconds >= cfg.activePulseWidth)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(cfg.outStop);
                        if (boolFuncRet != true) Debug.WriteLine(name + ":State[RevRecoverTimeoutWithFwdPulseOn1] SetUpperStateOfDOut(Stop)エラー");
                        cfg.dev.stopwatchPulse.Restart();
                        setPhase(AP_REV_RECOVER_TIMEOUT_WITH_FWD_PULSE_OFF1);
                    }
                    break;
                case AP_REV_RECOVER_TIMEOUT_WITH_FWD_PULSE_OFF1:
                    if (cfg.dev.stopwatchPulse.ElapsedMilliseconds >= cfg.inactivePulseWidth)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(cfg.outForward);
                        if (boolFuncRet != true) Debug.WriteLine(name + ":State[RevRecoverTimeoutWithFwdPulseOff1] SetUpperStateOfDOut(Fwd)エラー");
                        cfg.dev.stopwatchPulse.Restart();
                        setPhase(AP_REV_RECOVER_TIMEOUT_WITH_FWD_PULSE_ON2);
                    }
                    break;
                case AP_REV_RECOVER_TIMEOUT_WITH_FWD_PULSE_ON2:
                    if (cfg.dev.stopwatchPulse.ElapsedMilliseconds >= cfg.activePulseWidth)
                    {
                        boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(cfg.outStop);
                        if (boolFuncRet != true) Debug.WriteLine(name + ":State[RevRecoverTimeoutWithFwdPulseOn2] SetUpperStateOfDOut(Stop)エラー");
                        cfg.dev.stopwatchPulse.Stop();
                        setPhase(AP_REV_RECOVER_TIMEOUT_WITH_FWD_DOING);
                    }
                    break;
                case AP_REV_RECOVER_TIMEOUT_WITH_FWD_DOING:
                    if (cfg.dev.stopwatchTimeout.ElapsedMilliseconds >= cfg.forwardTimeInRecoverReverse)
                    {
                        cfg.dev.stopwatchPulse.Stop();
                        cfg.dev.stopwatchTimeout.Stop();
                        cfg.setFlagReverse(true);
                        cfg.setResultReverse(EDeviceResult.TimeOut);
                        setPhase(AP_STOP);
                    }
                    break;
                #endregion

                default:
                    break;
            }
        }
    }
}
