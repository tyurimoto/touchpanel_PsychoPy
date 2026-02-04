using System.Threading.Tasks;
using System.Web.Http;
using Compartment.Models;
using Compartment.Services;

namespace Compartment.Controllers
{
    /// <summary>
    /// API Controller for door control
    /// </summary>
    [RoutePrefix("api/door")]
    public class DoorController : ApiController
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
        /// POST api/door/open
        /// </summary>
        [HttpPost]
        [Route("open")]
        public async Task<IHttpActionResult> Open()
        {
            if (_hardwareService == null)
                return BadRequest("Hardware service not initialized");

            bool success = await _hardwareService.OpenDoorAsync();

            // Log event
            _hardwareService.EventLogger.LogEvent("DoorOpen", "Door", "", success,
                success ? "" : "Failed (animal may be inside)");

            return Ok(new DeviceCommandResponse
            {
                RoomId = _hardwareService.GetCompartmentNo(),
                Success = success,
                State = "opening"
            });
        }

        /// <summary>
        /// POST api/door/close
        /// </summary>
        [HttpPost]
        [Route("close")]
        public async Task<IHttpActionResult> Close()
        {
            if (_hardwareService == null)
                return BadRequest("Hardware service not initialized");

            bool success = await _hardwareService.CloseDoorAsync();

            // Log event
            _hardwareService.EventLogger.LogEvent("DoorClose", "Door", "", success);

            return Ok(new DeviceCommandResponse
            {
                RoomId = _hardwareService.GetCompartmentNo(),
                Success = success,
                State = "closing"
            });
        }

        /// <summary>
        /// GET api/door/status
        /// Get door status from sensors
        /// </summary>
        [HttpGet]
        [Route("status")]
        public async Task<IHttpActionResult> GetStatus()
        {
            if (_hardwareService == null)
                return BadRequest("Hardware service not initialized");

            var (sensorOpen, sensorClose, state) = await _hardwareService.GetDoorStatusAsync();

            return Ok(new
            {
                roomId = _hardwareService.GetCompartmentNo(),
                state = state,
                sensorOpen = sensorOpen,
                sensorClose = sensorClose,
                timestamp = System.DateTime.Now
            });
        }
    }
}
