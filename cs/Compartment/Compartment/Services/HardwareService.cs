using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Compartment.Services
{
    /// <summary>
    /// Hardware service that provides thread-safe access to FormMain hardware controls
    /// </summary>
    public class HardwareService
    {
        private readonly FormMain _formMain;
        private readonly EventLogger _eventLogger;

        public HardwareService(FormMain formMain)
        {
            _formMain = formMain ?? throw new ArgumentNullException(nameof(formMain));
            _eventLogger = new EventLogger(GetCompartmentNo());
        }

        /// <summary>
        /// Get event logger instance
        /// </summary>
        public EventLogger EventLogger => _eventLogger;

        /// <summary>
        /// Get compartment number (room ID)
        /// </summary>
        public string GetCompartmentNo()
        {
            return _formMain.preferencesDatOriginal?.CompartmentNo.ToString() ?? "0";
        }

        /// <summary>
        /// Check entrance sensor status
        /// </summary>
        public Task<bool> GetEntranceSensorAsync()
        {
            var tcs = new TaskCompletionSource<bool>();
            _formMain.Invoke((MethodInvoker)(() =>
            {
                try
                {
                    bool state = false;
                    _formMain.ioBoardDevice?.GetUpperStateOfSaveDIn(
                        IoBoardDInLogicalName.RoomEntrance, out state);
                    tcs.SetResult(state);
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            }));
            return tcs.Task;
        }

        /// <summary>
        /// Check exit sensor status
        /// </summary>
        public Task<bool> GetExitSensorAsync()
        {
            var tcs = new TaskCompletionSource<bool>();
            _formMain.Invoke((MethodInvoker)(() =>
            {
                try
                {
                    bool state = false;
                    _formMain.ioBoardDevice?.GetUpperStateOfSaveDIn(
                        IoBoardDInLogicalName.RoomExit, out state);
                    tcs.SetResult(state);
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            }));
            return tcs.Task;
        }

        /// <summary>
        /// Check stay sensor status
        /// </summary>
        public Task<bool> GetStaySensorAsync()
        {
            var tcs = new TaskCompletionSource<bool>();
            _formMain.Invoke((MethodInvoker)(() =>
            {
                try
                {
                    bool state = false;
                    _formMain.ioBoardDevice?.GetUpperStateOfSaveDIn(
                        IoBoardDInLogicalName.RoomStay, out state);
                    tcs.SetResult(state);
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            }));
            return tcs.Task;
        }

        /// <summary>
        /// Check lever switch status
        /// </summary>
        public Task<bool> GetLeverSwitchAsync()
        {
            var tcs = new TaskCompletionSource<bool>();
            _formMain.Invoke((MethodInvoker)(() =>
            {
                try
                {
                    bool state = false;
                    _formMain.ioBoardDevice?.GetUpperStateOfSaveDIn(
                        IoBoardDInLogicalName.LeverSw, out state);
                    tcs.SetResult(state);
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            }));
            return tcs.Task;
        }

        /// <summary>
        /// Get door status from sensors
        /// </summary>
        public Task<(bool sensorOpen, bool sensorClose, string state)> GetDoorStatusAsync()
        {
            var tcs = new TaskCompletionSource<(bool, bool, string)>();
            _formMain.Invoke((MethodInvoker)(() =>
            {
                try
                {
                    bool sensorOpen = false;
                    bool sensorClose = false;

                    _formMain.ioBoardDevice?.GetUpperStateOfSaveDIn(
                        IoBoardDInLogicalName.DoorOpen, out sensorOpen);
                    _formMain.ioBoardDevice?.GetUpperStateOfSaveDIn(
                        IoBoardDInLogicalName.DoorClose, out sensorClose);

                    // Determine door state from sensors
                    string state;
                    if (sensorOpen && sensorClose)
                    {
                        state = "error"; // Both sensors ON is abnormal
                    }
                    else if (sensorOpen)
                    {
                        state = "open";
                    }
                    else if (sensorClose)
                    {
                        state = "closed";
                    }
                    else
                    {
                        state = "moving"; // Neither sensor ON = door is moving
                    }

                    tcs.SetResult((sensorOpen, sensorClose, state));
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            }));
            return tcs.Task;
        }

        /// <summary>
        /// Open door
        /// </summary>
        public Task<bool> OpenDoorAsync()
        {
            var tcs = new TaskCompletionSource<bool>();
            _formMain.Invoke((MethodInvoker)(() =>
            {
                try
                {
                    // デバッグモード時は実機操作をスキップ
                    if (_formMain.preferencesDatOriginal.EnableDebugMode)
                    {
                        tcs.SetResult(true);
                        return;
                    }

                    // 安全チェック：部屋に動物がいる場合はドアを開けない
                    bool animalInside = false;
                    _formMain.ioBoardDevice?.GetUpperStateOfSaveDIn(
                        IoBoardDInLogicalName.RoomStay, out animalInside);

                    if (animalInside)
                    {
                        System.Diagnostics.Debug.WriteLine("[OpenDoorAsync] Cannot open door: animal is inside");
                        tcs.SetResult(false);
                        return;
                    }

                    var cmdPkt = new FormMain.DevCmdPkt { DevCmdVal = FormMain.EDevCmd.DoorOpen };
                    _formMain.concurrentQueueDevCmdPktDoor?.Enqueue(cmdPkt);
                    tcs.SetResult(true);
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            }));
            return tcs.Task;
        }

        /// <summary>
        /// Close door
        /// </summary>
        public Task<bool> CloseDoorAsync()
        {
            var tcs = new TaskCompletionSource<bool>();
            _formMain.Invoke((MethodInvoker)(() =>
            {
                try
                {
                    // デバッグモード時は実機操作をスキップ
                    if (_formMain.preferencesDatOriginal.EnableDebugMode)
                    {
                        tcs.SetResult(true);
                        return;
                    }

                    var cmdPkt = new FormMain.DevCmdPkt { DevCmdVal = FormMain.EDevCmd.DoorClose };
                    _formMain.concurrentQueueDevCmdPktDoor?.Enqueue(cmdPkt);
                    tcs.SetResult(true);
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            }));
            return tcs.Task;
        }

        /// <summary>
        /// Extend lever
        /// </summary>
        public Task<bool> ExtendLeverAsync()
        {
            var tcs = new TaskCompletionSource<bool>();
            _formMain.Invoke((MethodInvoker)(() =>
            {
                try
                {
                    var cmdPkt = new FormMain.DevCmdPkt { DevCmdVal = FormMain.EDevCmd.LeverOut };
                    _formMain.concurrentQueueDevCmdPktLever?.Enqueue(cmdPkt);
                    tcs.SetResult(true);
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            }));
            return tcs.Task;
        }

        /// <summary>
        /// Retract lever
        /// </summary>
        public Task<bool> RetractLeverAsync()
        {
            var tcs = new TaskCompletionSource<bool>();
            _formMain.Invoke((MethodInvoker)(() =>
            {
                try
                {
                    var cmdPkt = new FormMain.DevCmdPkt { DevCmdVal = FormMain.EDevCmd.LeverIn };
                    _formMain.concurrentQueueDevCmdPktLever?.Enqueue(cmdPkt);
                    tcs.SetResult(true);
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            }));
            return tcs.Task;
        }

        /// <summary>
        /// Emergency stop all motors (door, lever, feed)
        /// Called when PsychoPy script crashes or encounters an error
        /// </summary>
        public Task<bool> EmergencyStopAllAsync()
        {
            var tcs = new TaskCompletionSource<bool>();
            _formMain.Invoke((MethodInvoker)(() =>
            {
                try
                {
                    System.Diagnostics.Debug.WriteLine("[EmergencyStop] Stopping all motors");

                    // Stop door motor
                    var doorStopCmd = new FormMain.DevCmdPkt { DevCmdVal = FormMain.EDevCmd.DoorStop };
                    _formMain.concurrentQueueDevCmdPktDoor?.Enqueue(doorStopCmd);

                    // Stop lever motor (retract to safe position)
                    var leverStopCmd = new FormMain.DevCmdPkt { DevCmdVal = FormMain.EDevCmd.LeverStop };
                    _formMain.concurrentQueueDevCmdPktLever?.Enqueue(leverStopCmd);

                    // Stop feed motor
                    var feedStopCmd = new FormMain.DevCmdPkt { DevCmdVal = FormMain.EDevCmd.FeedStop };
                    _formMain.concurrentQueueDevCmdPktFeed?.Enqueue(feedStopCmd);

                    tcs.SetResult(true);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[EmergencyStop] Error: {ex.Message}");
                    tcs.SetException(ex);
                }
            }));
            return tcs.Task;
        }

        /// <summary>
        /// Dispense feed
        /// </summary>
        public Task<bool> DispenseFeedAsync(int durationMs)
        {
            var tcs = new TaskCompletionSource<bool>();
            _formMain.Invoke((MethodInvoker)(() =>
            {
                try
                {
                    var cmdPkt = new FormMain.DevCmdPkt
                    {
                        DevCmdVal = FormMain.EDevCmd.FeedForward,
                        iParam = new int[] { durationMs }
                    };
                    _formMain.concurrentQueueDevCmdPktFeed?.Enqueue(cmdPkt);
                    tcs.SetResult(true);
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            }));
            return tcs.Task;
        }

        /// <summary>
        /// Check if currently feeding
        /// </summary>
        public Task<bool> IsFeedingAsync()
        {
            var tcs = new TaskCompletionSource<bool>();
            _formMain.Invoke((MethodInvoker)(() =>
            {
                try
                {
                    tcs.SetResult(_formMain.Feeding);
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            }));
            return tcs.Task;
        }

        /// <summary>
        /// Read RFID
        /// </summary>
        public Task<string> ReadRFIDAsync()
        {
            var tcs = new TaskCompletionSource<string>();
            _formMain.Invoke((MethodInvoker)(() =>
            {
                try
                {
                    string rfid = _formMain.rfidReaderHelper?.RFID ?? string.Empty;
                    tcs.SetResult(rfid);
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            }));
            return tcs.Task;
        }

        /// <summary>
        /// Clear RFID
        /// </summary>
        public Task<bool> ClearRFIDAsync()
        {
            var tcs = new TaskCompletionSource<bool>();
            _formMain.Invoke((MethodInvoker)(() =>
            {
                try
                {
                    if (_formMain.rfidReaderHelper != null)
                    {
                        _formMain.rfidReaderHelper.CurrentIDCode.Value = string.Empty;
                    }
                    tcs.SetResult(true);
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            }));
            return tcs.Task;
        }

        // ===== Debug Mode Methods =====

        /// <summary>
        /// Check if debug mode is enabled
        /// </summary>
        public bool IsDebugModeEnabled()
        {
            bool result = false;
            _formMain.Invoke((MethodInvoker)(() =>
            {
                result = _formMain.preferencesDatOriginal?.EnableDebugMode ?? false;
            }));
            return result;
        }

        /// <summary>
        /// Check if real hardware is connected (for hybrid mode)
        /// </summary>
        public bool IsRealHardwareConnected()
        {
            bool result = false;
            _formMain.Invoke((MethodInvoker)(() =>
            {
                if (_formMain.ioBoardDevice is IoHybridBoard hybridBoard)
                {
                    result = hybridBoard.IsRealHardwareConnected;
                }
                else if (_formMain.ioBoardDevice is IoMicrochip)
                {
                    result = true;
                }
                else
                {
                    result = false;
                }
            }));
            return result;
        }

        /// <summary>
        /// Set sensor state in debug mode
        /// </summary>
        public bool SetDebugSensorState(IoBoardDInLogicalName sensor, bool state)
        {
            bool success = false;
            _formMain.Invoke((MethodInvoker)(() =>
            {
                try
                {
                    if (_formMain.ioBoardDevice is IoMicrochipDummyEx dummyBoard)
                    {
                        dummyBoard.SetManualSensorState(sensor, state);
                        success = true;
                    }
                    else if (_formMain.ioBoardDevice is IoHybridBoard hybridBoard)
                    {
                        hybridBoard.SetManualSensorState(sensor, state);
                        success = true;
                    }
                }
                catch
                {
                    success = false;
                }
            }));
            return success;
        }

        /// <summary>
        /// Set RFID in debug mode
        /// </summary>
        public bool SetDebugRFID(string rfidValue)
        {
            bool success = false;
            _formMain.Invoke((MethodInvoker)(() =>
            {
                try
                {
                    if (_formMain.rfidReaderDummy != null)
                    {
                        _formMain.rfidReaderDummy.SetRFID(rfidValue);
                        success = true;
                    }
                }
                catch
                {
                    success = false;
                }
            }));
            return success;
        }

        /// <summary>
        /// Get all sensor states in debug mode
        /// </summary>
        public System.Collections.Generic.Dictionary<string, bool> GetAllDebugSensorStates()
        {
            var states = new System.Collections.Generic.Dictionary<string, bool>();
            _formMain.Invoke((MethodInvoker)(() =>
            {
                try
                {
                    bool state = false;
                    _formMain.ioBoardDevice?.GetUpperStateOfSaveDIn(IoBoardDInLogicalName.RoomEntrance, out state);
                    states["entrance"] = state;

                    _formMain.ioBoardDevice?.GetUpperStateOfSaveDIn(IoBoardDInLogicalName.RoomExit, out state);
                    states["exit"] = state;

                    _formMain.ioBoardDevice?.GetUpperStateOfSaveDIn(IoBoardDInLogicalName.RoomStay, out state);
                    states["stay"] = state;

                    _formMain.ioBoardDevice?.GetUpperStateOfSaveDIn(IoBoardDInLogicalName.DoorOpen, out state);
                    states["doorOpen"] = state;

                    _formMain.ioBoardDevice?.GetUpperStateOfSaveDIn(IoBoardDInLogicalName.DoorClose, out state);
                    states["doorClose"] = state;

                    _formMain.ioBoardDevice?.GetUpperStateOfSaveDIn(IoBoardDInLogicalName.LeverIn, out state);
                    states["leverIn"] = state;

                    _formMain.ioBoardDevice?.GetUpperStateOfSaveDIn(IoBoardDInLogicalName.LeverOut, out state);
                    states["leverOut"] = state;

                    _formMain.ioBoardDevice?.GetUpperStateOfSaveDIn(IoBoardDInLogicalName.LeverSw, out state);
                    states["leverSw"] = state;
                }
                catch
                {
                    // Return empty dictionary on error
                }
            }));
            return states;
        }

        /// <summary>
        /// Reset debug state to default
        /// </summary>
        public bool ResetDebugState()
        {
            bool success = false;
            _formMain.Invoke((MethodInvoker)(() =>
            {
                try
                {
                    if (_formMain.ioBoardDevice is IoMicrochipDummyEx dummyBoard)
                    {
                        dummyBoard.ResetAllStates();
                        success = true;
                    }

                    if (_formMain.rfidReaderDummy != null)
                    {
                        _formMain.rfidReaderDummy.ClearRFID();
                        success = true;
                    }
                }
                catch
                {
                    success = false;
                }
            }));
            return success;
        }

        /// <summary>
        /// Generate random RFID for debug mode
        /// </summary>
        public string GenerateRandomRFID()
        {
            string rfid = "";
            _formMain.Invoke((MethodInvoker)(() =>
            {
                try
                {
                    if (_formMain.rfidReaderDummy != null)
                    {
                        rfid = _formMain.rfidReaderDummy.SetRandomRFID();
                    }
                    else
                    {
                        // Fallback: generate random RFID
                        var random = new Random();
                        for (int i = 0; i < 16; i++)
                        {
                            rfid += random.Next(0, 10).ToString();
                        }
                    }
                }
                catch
                {
                    rfid = "";
                }
            }));
            return rfid;
        }

        // ===== Task Status Methods (for debug monitoring) =====

        /// <summary>
        /// Get current task status
        /// </summary>
        public object GetTaskStatus()
        {
            object status = null;
            _formMain.Invoke((MethodInvoker)(() =>
            {
                try
                {
                    var idController = _formMain.idControlHelper;
                    var prefs = _formMain.preferencesDatOriginal;
                    var opCollection = _formMain.opCollection;

                    int totalTrials = 0;
                    int correctCount = 0;
                    int incorrectCount = 0;
                    double successRate = 0.0;

                    // TODO: Implement proper session counting in IdControlHelper
                    // Currently IdControlHelper does not have GetTotalSessionNum() and GetCorrectNum() methods
                    if (idController != null)
                    {
                        totalTrials = idController.KeyPairsCount;
                        correctCount = 0; // Placeholder
                        incorrectCount = totalTrials - correctCount;
                        successRate = totalTrials > 0 ? (double)correctCount / totalTrials * 100 : 0;
                    }

                    string currentState = "Unknown";
                    bool isRunning = false;

                    if (opCollection != null && opCollection.sequencer != null)
                    {
                        currentState = opCollection.sequencer.State.ToString();
                        isRunning = opCollection.sequencer.State != OpCollection.Sequencer.EState.Idle;
                    }

                    status = new
                    {
                        taskType = prefs?.OpeTypeOfTask.ToString() ?? "None",
                        currentState = currentState,
                        isRunning = isRunning,
                        totalTrials = totalTrials,
                        correctCount = correctCount,
                        incorrectCount = incorrectCount,
                        successRate = Math.Round(successRate, 1),
                        currentIdCode = _formMain.rfidReaderHelper?.RFID ?? "",
                        compartmentNo = prefs?.CompartmentNo ?? 0
                    };
                }
                catch (Exception ex)
                {
                    status = new
                    {
                        error = ex.Message,
                        taskType = "Unknown",
                        currentState = "Error",
                        isRunning = false,
                        totalTrials = 0,
                        correctCount = 0,
                        incorrectCount = 0,
                        successRate = 0.0
                    };
                }
            }));
            return status;
        }

        /// <summary>
        /// Get recent trial history
        /// </summary>
        public object GetTaskHistory()
        {
            object history = null;
            _formMain.Invoke((MethodInvoker)(() =>
            {
                try
                {
                    var idController = _formMain.idControlHelper;
                    var recentList = new System.Collections.Generic.List<object>();

                    // TODO: Implement proper trial history in IdControlHelper
                    // Currently IdControlHelper does not have listOfIdDataSingle property
                    // This functionality is disabled until IdControlHelper exposes trial history

                    history = new
                    {
                        trials = recentList,
                        count = recentList.Count
                    };
                }
                catch (Exception ex)
                {
                    history = new
                    {
                        error = ex.Message,
                        trials = new System.Collections.Generic.List<object>(),
                        count = 0
                    };
                }
            }));
            return history;
        }

        /// <summary>
        /// Simulate a correct trial (for debug testing)
        /// </summary>
        public bool SimulateCorrectTrial()
        {
            bool success = false;
            _formMain.Invoke((MethodInvoker)(() =>
            {
                try
                {
                    // This would trigger the same logic as a real correct trial
                    // Implementation depends on task architecture
                    // For now, just log it
                    System.Diagnostics.Debug.WriteLine("[Debug] Simulating correct trial");
                    success = true;
                }
                catch
                {
                    success = false;
                }
            }));
            return success;
        }

        /// <summary>
        /// Simulate an incorrect trial (for debug testing)
        /// </summary>
        public bool SimulateIncorrectTrial()
        {
            bool success = false;
            _formMain.Invoke((MethodInvoker)(() =>
            {
                try
                {
                    // This would trigger the same logic as a real incorrect trial
                    // Implementation depends on task architecture
                    // For now, just log it
                    System.Diagnostics.Debug.WriteLine("[Debug] Simulating incorrect trial");
                    success = true;
                }
                catch
                {
                    success = false;
                }
            }));
            return success;
        }

        // ===== Room/Entry/Exit Methods =====

        /// <summary>
        /// Get room/cage status
        /// </summary>
        public object GetRoomStatus()
        {
            object status = null;
            _formMain.Invoke((MethodInvoker)(() =>
            {
                try
                {
                    bool isInside = false;
                    _formMain.ioBoardDevice?.GetUpperStateOfSaveDIn(
                        IoBoardDInLogicalName.RoomStay, out isInside);

                    status = new
                    {
                        roomId = _formMain.preferencesDatOriginal?.CompartmentNo ?? 0,
                        animalInside = isInside,
                        timestamp = DateTime.Now
                    };
                }
                catch (Exception ex)
                {
                    status = new { error = ex.Message };
                }
            }));
            return status;
        }

        /// <summary>
        /// Wait for animal entry (monitors OpFlagRoomIn)
        /// </summary>
        public Task<bool> WaitForEntryAsync(int timeoutMs)
        {
            var tcs = new TaskCompletionSource<bool>();
            Task.Run(() =>
            {
                var sw = System.Diagnostics.Stopwatch.StartNew();
                while (sw.ElapsedMilliseconds < timeoutMs)
                {
                    bool entered = false;
                    _formMain.Invoke((MethodInvoker)(() =>
                    {
                        entered = _formMain.OpFlagRoomIn;
                        if (entered)
                        {
                            _formMain.OpFlagRoomIn = false;
                        }
                    }));

                    if (entered)
                    {
                        tcs.SetResult(true);
                        return;
                    }

                    System.Threading.Thread.Sleep(10);
                }
                tcs.SetResult(false);
            });
            return tcs.Task;
        }

        /// <summary>
        /// Wait for animal exit (monitors OpFlagRoomOut)
        /// </summary>
        public Task<bool> WaitForExitAsync(int timeoutMs)
        {
            var tcs = new TaskCompletionSource<bool>();
            Task.Run(() =>
            {
                var sw = System.Diagnostics.Stopwatch.StartNew();
                while (sw.ElapsedMilliseconds < timeoutMs)
                {
                    bool exited = false;
                    _formMain.Invoke((MethodInvoker)(() =>
                    {
                        exited = _formMain.OpFlagRoomOut;
                        if (exited)
                        {
                            _formMain.OpFlagRoomOut = false;
                        }
                    }));

                    if (exited)
                    {
                        tcs.SetResult(true);
                        return;
                    }

                    System.Threading.Thread.Sleep(10);
                }
                tcs.SetResult(false);
            });
            return tcs.Task;
        }

        /// <summary>
        /// Wait for RFID read
        /// </summary>
        public Task<string> WaitForRFIDAsync(int timeoutMs)
        {
            var tcs = new TaskCompletionSource<string>();
            Task.Run(() =>
            {
                var sw = System.Diagnostics.Stopwatch.StartNew();
                while (sw.ElapsedMilliseconds < timeoutMs)
                {
                    string rfid = "";
                    _formMain.Invoke((MethodInvoker)(() =>
                    {
                        rfid = _formMain.rfidReaderHelper?.RFID ?? string.Empty;
                    }));

                    if (!string.IsNullOrEmpty(rfid))
                    {
                        tcs.SetResult(rfid);
                        return;
                    }

                    System.Threading.Thread.Sleep(10);
                }
                tcs.SetResult(string.Empty);
            });
            return tcs.Task;
        }

        // ===== Lamp Control Methods =====

        /// <summary>
        /// Room lamp ON/OFF
        /// </summary>
        public Task<bool> SetRoomLampAsync(bool on)
        {
            var tcs = new TaskCompletionSource<bool>();
            _formMain.Invoke((MethodInvoker)(() =>
            {
                try
                {
                    var logicalName = on
                        ? IoBoardDOutLogicalName.RoomLampOn
                        : IoBoardDOutLogicalName.RoomLampOff;
                    bool result = _formMain.ioBoardDevice?.SetUpperStateOfDOut(logicalName) ?? false;
                    tcs.SetResult(result);
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            }));
            return tcs.Task;
        }

        /// <summary>
        /// Lever lamp ON/OFF
        /// </summary>
        public Task<bool> SetLeverLampAsync(bool on)
        {
            var tcs = new TaskCompletionSource<bool>();
            _formMain.Invoke((MethodInvoker)(() =>
            {
                try
                {
                    var logicalName = on
                        ? IoBoardDOutLogicalName.LeverLampOn
                        : IoBoardDOutLogicalName.LeverLampOff;
                    bool result = _formMain.ioBoardDevice?.SetUpperStateOfDOut(logicalName) ?? false;
                    tcs.SetResult(result);
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            }));
            return tcs.Task;
        }

        /// <summary>
        /// Feed lamp ON/OFF
        /// </summary>
        public Task<bool> SetFeedLampAsync(bool on)
        {
            var tcs = new TaskCompletionSource<bool>();
            _formMain.Invoke((MethodInvoker)(() =>
            {
                try
                {
                    var logicalName = on
                        ? IoBoardDOutLogicalName.FeedLampOn
                        : IoBoardDOutLogicalName.FeedLampOff;
                    bool result = _formMain.ioBoardDevice?.SetUpperStateOfDOut(logicalName) ?? false;
                    tcs.SetResult(result);
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            }));
            return tcs.Task;
        }

        // ===== Sound Methods =====

        /// <summary>
        /// Play sound file
        /// </summary>
        public Task<bool> PlaySoundAsync(string filePath, int durationMs)
        {
            var tcs = new TaskCompletionSource<bool>();
            _formMain.Invoke((MethodInvoker)(() =>
            {
                try
                {
                    if (System.IO.File.Exists(filePath))
                    {
                        var player = new System.Media.SoundPlayer(filePath);
                        player.Play();
                        tcs.SetResult(true);
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"[Sound] File not found: {filePath}");
                        tcs.SetResult(false);
                    }
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            }));
            return tcs.Task;
        }
    }
}
