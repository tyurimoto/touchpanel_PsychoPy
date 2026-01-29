using System;
using System.Collections.Generic;

namespace Compartment
{
    /// <summary>
    /// Extended dummy IOBoard that holds sensor states and allows external setting.
    /// Used for debug mode to simulate hardware without physical devices.
    /// </summary>
    public class IoMicrochipDummyEx : IoBoardBase
    {
        private readonly object _syncLock = new object();

        // Sensor states (true = active/on, false = inactive/off)
        private Dictionary<IoBoardDInLogicalName, bool> _sensorStates;

        // Output states for tracking device commands
        private Dictionary<IoBoardDOutLogicalName, bool> _outputStates;

        public IoMicrochipDummyEx()
        {
            errorMsg = "";
            InitializeSensorStates();
        }

        private void InitializeSensorStates()
        {
            _sensorStates = new Dictionary<IoBoardDInLogicalName, bool>
            {
                { IoBoardDInLogicalName.RoomEntrance, false },
                { IoBoardDInLogicalName.RoomExit, false },
                { IoBoardDInLogicalName.RoomStay, false },
                { IoBoardDInLogicalName.DoorOpen, false },
                { IoBoardDInLogicalName.DoorClose, true },  // Default: door is closed
                { IoBoardDInLogicalName.LeverIn, true },    // Default: lever is in
                { IoBoardDInLogicalName.LeverOut, false },
                { IoBoardDInLogicalName.LeverSw, false }
            };

            _outputStates = new Dictionary<IoBoardDOutLogicalName, bool>
            {
                { IoBoardDOutLogicalName.DoorOpen, false },
                { IoBoardDOutLogicalName.DoorClose, false },
                { IoBoardDOutLogicalName.DoorStop, false },
                { IoBoardDOutLogicalName.LeverExtend, false },
                { IoBoardDOutLogicalName.LeverRetract, false },
                { IoBoardDOutLogicalName.LeverStop, false },
                { IoBoardDOutLogicalName.LeverLamp, false },
                { IoBoardDOutLogicalName.FeedOn, false },
                { IoBoardDOutLogicalName.FeedLamp, false },
                { IoBoardDOutLogicalName.RoomLamp, false }
            };
        }

        /// <summary>
        /// Manually set sensor state (for debug/simulation)
        /// </summary>
        public void SetSensorState(IoBoardDInLogicalName sensor, bool state)
        {
            lock (_syncLock)
            {
                if (_sensorStates.ContainsKey(sensor))
                {
                    _sensorStates[sensor] = state;
                }
            }
        }

        /// <summary>
        /// Get current sensor state
        /// </summary>
        public bool GetSensorState(IoBoardDInLogicalName sensor)
        {
            lock (_syncLock)
            {
                return _sensorStates.ContainsKey(sensor) ? _sensorStates[sensor] : false;
            }
        }

        /// <summary>
        /// Get all sensor states (for debug UI)
        /// </summary>
        public Dictionary<IoBoardDInLogicalName, bool> GetAllSensorStates()
        {
            lock (_syncLock)
            {
                return new Dictionary<IoBoardDInLogicalName, bool>(_sensorStates);
            }
        }

        /// <summary>
        /// Reset all states to default
        /// </summary>
        public void ResetAllStates()
        {
            lock (_syncLock)
            {
                InitializeSensorStates();
            }
        }

        public override bool AcquireDevice()
        {
            return true;
        }

        public override bool ReleaseDevice()
        {
            return true;
        }

        public override bool DirectOut(IoBoardPortNo a_IoBoardPortNoObj, ushort a_ushortOutCode)
        {
            return true;
        }

        public override bool DirectIn(IoBoardPortNo a_IoBoardPortNoObj, out ushort a_ushortInCode)
        {
            a_ushortInCode = 0;
            return true;
        }

        public ushort SaveDInForPort1 { get; set; } = 0x00;
        public ushort SaveDInForPort3 { get; set; } = 0x00;

        public override bool SaveDIn()
        {
            // In dummy mode, we don't actually read from hardware
            return true;
        }

        public override bool GetRawStateOfSaveDIn(IoBoardDInLogicalName a_IoBoardDInLogicalNameObj, out bool a_boolRawState)
        {
            lock (_syncLock)
            {
                a_boolRawState = _sensorStates.ContainsKey(a_IoBoardDInLogicalNameObj)
                    ? _sensorStates[a_IoBoardDInLogicalNameObj]
                    : false;
            }
            return true;
        }

        public override bool GetUpperStateOfSaveDIn(IoBoardDInLogicalName a_IoBoardDInLogicalNameObj, out bool a_boolLogicalState)
        {
            return GetRawStateOfSaveDIn(a_IoBoardDInLogicalNameObj, out a_boolLogicalState);
        }

        public override bool SetUpperStateOfDOut(IoBoardDOutLogicalName a_IoBoardDOutLogicalNameObj)
        {
            lock (_syncLock)
            {
                _outputStates[a_IoBoardDOutLogicalNameObj] = true;

                // Automatically update sensor states based on commands
                SimulateDeviceResponse(a_IoBoardDOutLogicalNameObj);
            }
            return true;
        }

        /// <summary>
        /// Simulate how real hardware would respond to output commands
        /// </summary>
        private void SimulateDeviceResponse(IoBoardDOutLogicalName command)
        {
            switch (command)
            {
                case IoBoardDOutLogicalName.DoorOpen:
                    // Simulate door opening: after command, door open sensor becomes true, close becomes false
                    System.Threading.Tasks.Task.Delay(500).ContinueWith(_ =>
                    {
                        lock (_syncLock)
                        {
                            _sensorStates[IoBoardDInLogicalName.DoorOpen] = true;
                            _sensorStates[IoBoardDInLogicalName.DoorClose] = false;
                        }
                    });
                    break;

                case IoBoardDOutLogicalName.DoorClose:
                    // Simulate door closing
                    System.Threading.Tasks.Task.Delay(500).ContinueWith(_ =>
                    {
                        lock (_syncLock)
                        {
                            _sensorStates[IoBoardDInLogicalName.DoorOpen] = false;
                            _sensorStates[IoBoardDInLogicalName.DoorClose] = true;
                        }
                    });
                    break;

                case IoBoardDOutLogicalName.LeverExtend:
                    // Simulate lever extending
                    System.Threading.Tasks.Task.Delay(300).ContinueWith(_ =>
                    {
                        lock (_syncLock)
                        {
                            _sensorStates[IoBoardDInLogicalName.LeverOut] = true;
                            _sensorStates[IoBoardDInLogicalName.LeverIn] = false;
                        }
                    });
                    break;

                case IoBoardDOutLogicalName.LeverRetract:
                    // Simulate lever retracting
                    System.Threading.Tasks.Task.Delay(300).ContinueWith(_ =>
                    {
                        lock (_syncLock)
                        {
                            _sensorStates[IoBoardDInLogicalName.LeverOut] = false;
                            _sensorStates[IoBoardDInLogicalName.LeverIn] = true;
                        }
                    });
                    break;
            }
        }

        public override bool GetData(IoMicrochip.IoBoardDInCode ioBoardDInCode)
        {
            return true;
        }

        public override bool GetData(IoMicrochip.IoBoardDInStatusCode ioBoardDInCode, bool n)
        {
            return true;
        }
    }
}
