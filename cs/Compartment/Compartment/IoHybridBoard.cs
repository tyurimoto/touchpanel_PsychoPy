using System;
using System.Collections.Generic;

namespace Compartment
{
    /// <summary>
    /// Hybrid IOBoard that combines real hardware and manual simulation.
    /// Allows per-sensor configuration to choose between real hardware and manual override.
    /// Recommended for debugging when some sensors are difficult to control manually.
    /// </summary>
    public class IoHybridBoard : IoBoardBase
    {
        private readonly object _syncLock = new object();

        private IoBoardBase _realBoard;      // Real hardware (IoMicrochip)
        private IoBoardBase _dummyBoard;     // Dummy board (IoMicrochipDummyEx)

        // Sensor override settings (true = manual simulation, false = real hardware)
        private Dictionary<IoBoardDInLogicalName, bool> _sensorOverrides;

        // Manual sensor states (only used when override is true)
        private Dictionary<IoBoardDInLogicalName, bool> _manualSensorStates;

        /// <summary>
        /// Initialize hybrid board
        /// </summary>
        /// <param name="useRealHardware">Try to connect to real hardware</param>
        public IoHybridBoard(bool useRealHardware)
        {
            errorMsg = "";
            InitializeOverrideSettings();

            // Try to acquire real hardware
            if (useRealHardware)
            {
                _realBoard = new IoMicrochip();
                if (!_realBoard.AcquireDevice())
                {
                    _realBoard = null;  // Failed to connect
                }
            }

            // Always prepare dummy board as fallback
            _dummyBoard = new IoMicrochipDummyEx();
            _dummyBoard.AcquireDevice();
        }

        private void InitializeOverrideSettings()
        {
            // Default: entrance and exit sensors use manual simulation (hard to control)
            // Other sensors use real hardware if available
            _sensorOverrides = new Dictionary<IoBoardDInLogicalName, bool>
            {
                { IoBoardDInLogicalName.RoomEntrance, true },   // Default: manual
                { IoBoardDInLogicalName.RoomExit, true },       // Default: manual
                { IoBoardDInLogicalName.RoomStay, false },      // Default: real hardware
                { IoBoardDInLogicalName.DoorOpen, false },      // Default: real hardware
                { IoBoardDInLogicalName.DoorClose, false },     // Default: real hardware
                { IoBoardDInLogicalName.LeverIn, false },       // Default: real hardware
                { IoBoardDInLogicalName.LeverOut, false },      // Default: real hardware
                { IoBoardDInLogicalName.LeverSw, false }        // Default: real hardware
            };

            _manualSensorStates = new Dictionary<IoBoardDInLogicalName, bool>
            {
                { IoBoardDInLogicalName.RoomEntrance, false },
                { IoBoardDInLogicalName.RoomExit, false },
                { IoBoardDInLogicalName.RoomStay, false },
                { IoBoardDInLogicalName.DoorOpen, false },
                { IoBoardDInLogicalName.DoorClose, true },
                { IoBoardDInLogicalName.LeverIn, true },
                { IoBoardDInLogicalName.LeverOut, false },
                { IoBoardDInLogicalName.LeverSw, false }
            };
        }

        /// <summary>
        /// Check if real hardware is connected
        /// </summary>
        public bool IsRealHardwareConnected => _realBoard != null;

        /// <summary>
        /// Set sensor override (true = manual simulation, false = real hardware)
        /// </summary>
        public void SetSensorOverride(IoBoardDInLogicalName sensor, bool useManual)
        {
            lock (_syncLock)
            {
                if (_sensorOverrides.ContainsKey(sensor))
                {
                    _sensorOverrides[sensor] = useManual;
                }
            }
        }

        /// <summary>
        /// Get sensor override setting
        /// </summary>
        public bool GetSensorOverride(IoBoardDInLogicalName sensor)
        {
            lock (_syncLock)
            {
                return _sensorOverrides.ContainsKey(sensor) ? _sensorOverrides[sensor] : false;
            }
        }

        /// <summary>
        /// Manually set sensor state (only affects sensors with manual override enabled)
        /// </summary>
        public void SetManualSensorState(IoBoardDInLogicalName sensor, bool state)
        {
            lock (_syncLock)
            {
                if (_manualSensorStates.ContainsKey(sensor))
                {
                    _manualSensorStates[sensor] = state;
                }
            }
        }

        /// <summary>
        /// Get manual sensor state
        /// </summary>
        public bool GetManualSensorState(IoBoardDInLogicalName sensor)
        {
            lock (_syncLock)
            {
                return _manualSensorStates.ContainsKey(sensor) ? _manualSensorStates[sensor] : false;
            }
        }

        /// <summary>
        /// Get all sensor override settings
        /// </summary>
        public Dictionary<IoBoardDInLogicalName, bool> GetAllOverrideSettings()
        {
            lock (_syncLock)
            {
                return new Dictionary<IoBoardDInLogicalName, bool>(_sensorOverrides);
            }
        }

        public override bool AcquireDevice()
        {
            // Already acquired in constructor
            return true;
        }

        public override bool ReleaseDevice()
        {
            if (_realBoard != null)
            {
                _realBoard.ReleaseDevice();
            }
            if (_dummyBoard != null)
            {
                _dummyBoard.ReleaseDevice();
            }
            return true;
        }

        public override bool DirectOut(IoBoardPortNo a_IoBoardPortNoObj, ushort a_ushortOutCode)
        {
            if (_realBoard != null)
            {
                return _realBoard.DirectOut(a_IoBoardPortNoObj, a_ushortOutCode);
            }
            else
            {
                return _dummyBoard.DirectOut(a_IoBoardPortNoObj, a_ushortOutCode);
            }
        }

        public override bool DirectIn(IoBoardPortNo a_IoBoardPortNoObj, out ushort a_ushortInCode)
        {
            if (_realBoard != null)
            {
                return _realBoard.DirectIn(a_IoBoardPortNoObj, out a_ushortInCode);
            }
            else
            {
                return _dummyBoard.DirectIn(a_IoBoardPortNoObj, out a_ushortInCode);
            }
        }

        public ushort SaveDInForPort1
        {
            get
            {
                if (_realBoard != null)
                {
                    return ((IoMicrochip)_realBoard).SaveDInForPort1;
                }
                else
                {
                    return ((IoMicrochipDummyEx)_dummyBoard).SaveDInForPort1;
                }
            }
            set
            {
                if (_realBoard != null)
                {
                    ((IoMicrochip)_realBoard).SaveDInForPort1 = value;
                }
                else
                {
                    ((IoMicrochipDummyEx)_dummyBoard).SaveDInForPort1 = value;
                }
            }
        }

        public ushort SaveDInForPort3
        {
            get
            {
                if (_realBoard != null)
                {
                    return ((IoMicrochip)_realBoard).SaveDInForPort3;
                }
                else
                {
                    return ((IoMicrochipDummyEx)_dummyBoard).SaveDInForPort3;
                }
            }
            set
            {
                if (_realBoard != null)
                {
                    ((IoMicrochip)_realBoard).SaveDInForPort3 = value;
                }
                else
                {
                    ((IoMicrochipDummyEx)_dummyBoard).SaveDInForPort3 = value;
                }
            }
        }

        public override bool SaveDIn()
        {
            if (_realBoard != null)
            {
                return _realBoard.SaveDIn();
            }
            else
            {
                return _dummyBoard.SaveDIn();
            }
        }

        public override bool GetRawStateOfSaveDIn(IoBoardDInLogicalName a_IoBoardDInLogicalNameObj, out bool a_boolRawState)
        {
            lock (_syncLock)
            {
                // Check if this sensor has manual override enabled
                if (_sensorOverrides.ContainsKey(a_IoBoardDInLogicalNameObj) && _sensorOverrides[a_IoBoardDInLogicalNameObj])
                {
                    // Use manual simulation
                    a_boolRawState = _manualSensorStates.ContainsKey(a_IoBoardDInLogicalNameObj)
                        ? _manualSensorStates[a_IoBoardDInLogicalNameObj]
                        : false;
                    return true;
                }
                else
                {
                    // Use real hardware (or dummy if not connected)
                    if (_realBoard != null)
                    {
                        return _realBoard.GetRawStateOfSaveDIn(a_IoBoardDInLogicalNameObj, out a_boolRawState);
                    }
                    else
                    {
                        return _dummyBoard.GetRawStateOfSaveDIn(a_IoBoardDInLogicalNameObj, out a_boolRawState);
                    }
                }
            }
        }

        public override bool GetUpperStateOfSaveDIn(IoBoardDInLogicalName a_IoBoardDInLogicalNameObj, out bool a_boolLogicalState)
        {
            return GetRawStateOfSaveDIn(a_IoBoardDInLogicalNameObj, out a_boolLogicalState);
        }

        public override bool SetUpperStateOfDOut(IoBoardDOutLogicalName a_IoBoardDOutLogicalNameObj)
        {
            // Output commands always go to real hardware (or dummy if not available)
            if (_realBoard != null)
            {
                return _realBoard.SetUpperStateOfDOut(a_IoBoardDOutLogicalNameObj);
            }
            else
            {
                return _dummyBoard.SetUpperStateOfDOut(a_IoBoardDOutLogicalNameObj);
            }
        }

        public override bool GetData(IoMicrochip.IoBoardDInCode ioBoardDInCode)
        {
            if (_realBoard != null)
            {
                return _realBoard.GetData(ioBoardDInCode);
            }
            else
            {
                return _dummyBoard.GetData(ioBoardDInCode);
            }
        }

        public override bool GetData(IoMicrochip.IoBoardDInStatusCode ioBoardDInCode, bool n)
        {
            if (_realBoard != null)
            {
                return _realBoard.GetData(ioBoardDInCode, n);
            }
            else
            {
                return _dummyBoard.GetData(ioBoardDInCode, n);
            }
        }
    }
}
