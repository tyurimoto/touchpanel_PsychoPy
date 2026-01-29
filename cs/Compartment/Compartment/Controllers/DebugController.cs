using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using Compartment.Services;

namespace Compartment.Controllers
{
    /// <summary>
    /// Debug mode API controller for simulating hardware without physical devices
    /// </summary>
    [RoutePrefix("api/debug")]
    public class DebugController : ApiController
    {
        private static HardwareService _hardwareService;

        /// <summary>
        /// Initialize hardware service (called from FormMain)
        /// </summary>
        public static void Initialize(HardwareService hardwareService)
        {
            _hardwareService = hardwareService;
        }

        /// <summary>
        /// POST api/debug/sensor/set
        /// Set a sensor state manually (debug mode only)
        /// </summary>
        [HttpPost]
        [Route("sensor/set")]
        public IHttpActionResult SetSensor([FromBody] SetSensorRequest request)
        {
            if (_hardwareService == null)
                return BadRequest("Hardware service not initialized");

            if (!_hardwareService.IsDebugModeEnabled())
                return BadRequest("Debug mode is not enabled");

            if (request == null || string.IsNullOrEmpty(request.Sensor))
                return BadRequest("Invalid request: sensor name is required");

            // Map sensor name to IoBoardDInLogicalName
            IoBoardDInLogicalName? sensorEnum = MapSensorName(request.Sensor);
            if (!sensorEnum.HasValue)
                return BadRequest($"Unknown sensor: {request.Sensor}");

            bool success = _hardwareService.SetDebugSensorState(sensorEnum.Value, request.State);

            return Ok(new
            {
                success = success,
                sensor = request.Sensor,
                state = request.State,
                timestamp = DateTime.Now
            });
        }

        /// <summary>
        /// POST api/debug/rfid/set
        /// Set RFID value manually (debug mode only)
        /// </summary>
        [HttpPost]
        [Route("rfid/set")]
        public IHttpActionResult SetRFID([FromBody] SetRFIDRequest request)
        {
            if (_hardwareService == null)
                return BadRequest("Hardware service not initialized");

            if (!_hardwareService.IsDebugModeEnabled())
                return BadRequest("Debug mode is not enabled");

            if (request == null || string.IsNullOrEmpty(request.Id))
                return BadRequest("Invalid request: id is required");

            bool success = _hardwareService.SetDebugRFID(request.Id);

            return Ok(new
            {
                roomId = _hardwareService.GetCompartmentNo(),
                success = success,
                id = request.Id,
                timestamp = DateTime.Now
            });
        }

        /// <summary>
        /// GET api/debug/sensors/all
        /// Get all sensor states (debug mode only)
        /// </summary>
        [HttpGet]
        [Route("sensors/all")]
        public IHttpActionResult GetAllSensors()
        {
            if (_hardwareService == null)
                return BadRequest("Hardware service not initialized");

            if (!_hardwareService.IsDebugModeEnabled())
                return BadRequest("Debug mode is not enabled");

            var states = _hardwareService.GetAllDebugSensorStates();

            return Ok(new
            {
                roomId = _hardwareService.GetCompartmentNo(),
                sensors = states,
                timestamp = DateTime.Now
            });
        }

        /// <summary>
        /// POST api/debug/reset
        /// Reset all debug states to default (debug mode only)
        /// </summary>
        [HttpPost]
        [Route("reset")]
        public IHttpActionResult Reset()
        {
            if (_hardwareService == null)
                return BadRequest("Hardware service not initialized");

            if (!_hardwareService.IsDebugModeEnabled())
                return BadRequest("Debug mode is not enabled");

            bool success = _hardwareService.ResetDebugState();

            return Ok(new
            {
                roomId = _hardwareService.GetCompartmentNo(),
                success = success,
                message = "Debug state reset to default",
                timestamp = DateTime.Now
            });
        }

        /// <summary>
        /// GET api/debug/status
        /// Get debug mode status and configuration
        /// </summary>
        [HttpGet]
        [Route("status")]
        public IHttpActionResult GetDebugStatus()
        {
            if (_hardwareService == null)
                return BadRequest("Hardware service not initialized");

            return Ok(new
            {
                roomId = _hardwareService.GetCompartmentNo(),
                debugModeEnabled = _hardwareService.IsDebugModeEnabled(),
                hardwareConnected = _hardwareService.IsRealHardwareConnected(),
                timestamp = DateTime.Now
            });
        }

        /// <summary>
        /// POST api/debug/rfid/random
        /// Generate and set a random RFID (debug mode only)
        /// </summary>
        [HttpPost]
        [Route("rfid/random")]
        public IHttpActionResult SetRandomRFID()
        {
            if (_hardwareService == null)
                return BadRequest("Hardware service not initialized");

            if (!_hardwareService.IsDebugModeEnabled())
                return BadRequest("Debug mode is not enabled");

            string randomRFID = _hardwareService.GenerateRandomRFID();

            return Ok(new
            {
                roomId = _hardwareService.GetCompartmentNo(),
                success = true,
                id = randomRFID,
                timestamp = DateTime.Now
            });
        }

        /// <summary>
        /// GET api/debug/task/status
        /// Get current task status (trial count, state, results, etc.)
        /// </summary>
        [HttpGet]
        [Route("task/status")]
        public IHttpActionResult GetTaskStatus()
        {
            if (_hardwareService == null)
                return BadRequest("Hardware service not initialized");

            var taskStatus = _hardwareService.GetTaskStatus();

            return Ok(new
            {
                roomId = _hardwareService.GetCompartmentNo(),
                task = taskStatus,
                timestamp = DateTime.Now
            });
        }

        /// <summary>
        /// GET api/debug/task/history
        /// Get recent trial history
        /// </summary>
        [HttpGet]
        [Route("task/history")]
        public IHttpActionResult GetTaskHistory()
        {
            if (_hardwareService == null)
                return BadRequest("Hardware service not initialized");

            var history = _hardwareService.GetTaskHistory();

            return Ok(new
            {
                roomId = _hardwareService.GetCompartmentNo(),
                history = history,
                timestamp = DateTime.Now
            });
        }

        /// <summary>
        /// POST api/debug/task/simulate-correct
        /// Simulate a correct trial (debug mode only)
        /// </summary>
        [HttpPost]
        [Route("task/simulate-correct")]
        public IHttpActionResult SimulateCorrectTrial()
        {
            if (_hardwareService == null)
                return BadRequest("Hardware service not initialized");

            if (!_hardwareService.IsDebugModeEnabled())
                return BadRequest("Debug mode is not enabled");

            bool success = _hardwareService.SimulateCorrectTrial();

            return Ok(new
            {
                roomId = _hardwareService.GetCompartmentNo(),
                success = success,
                message = "Correct trial simulated",
                timestamp = DateTime.Now
            });
        }

        /// <summary>
        /// POST api/debug/task/simulate-incorrect
        /// Simulate an incorrect trial (debug mode only)
        /// </summary>
        [HttpPost]
        [Route("task/simulate-incorrect")]
        public IHttpActionResult SimulateIncorrectTrial()
        {
            if (_hardwareService == null)
                return BadRequest("Hardware service not initialized");

            if (!_hardwareService.IsDebugModeEnabled())
                return BadRequest("Debug mode is not enabled");

            bool success = _hardwareService.SimulateIncorrectTrial();

            return Ok(new
            {
                roomId = _hardwareService.GetCompartmentNo(),
                success = success,
                message = "Incorrect trial simulated",
                timestamp = DateTime.Now
            });
        }

        /// <summary>
        /// Map sensor name string to enum
        /// </summary>
        private IoBoardDInLogicalName? MapSensorName(string sensorName)
        {
            switch (sensorName.ToLower())
            {
                case "entrance":
                    return IoBoardDInLogicalName.RoomEntrance;
                case "exit":
                    return IoBoardDInLogicalName.RoomExit;
                case "stay":
                    return IoBoardDInLogicalName.RoomStay;
                case "dooropen":
                    return IoBoardDInLogicalName.DoorOpen;
                case "doorclose":
                    return IoBoardDInLogicalName.DoorClose;
                case "leverin":
                    return IoBoardDInLogicalName.LeverIn;
                case "leverout":
                    return IoBoardDInLogicalName.LeverOut;
                case "lever":
                case "leversw":
                    return IoBoardDInLogicalName.LeverSw;
                default:
                    return null;
            }
        }
    }

    /// <summary>
    /// Request model for setting sensor state
    /// </summary>
    public class SetSensorRequest
    {
        public string Sensor { get; set; }
        public bool State { get; set; }
    }

    /// <summary>
    /// Request model for setting RFID
    /// </summary>
    public class SetRFIDRequest
    {
        public string Id { get; set; }
    }
}
