using System.Threading.Tasks;
using System.Web.Http;
using Compartment.Models;
using Compartment.Services;

namespace Compartment.Controllers
{
    /// <summary>
    /// API Controller for lever control
    /// </summary>
    [RoutePrefix("api/lever")]
    public class LeverController : ApiController
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
        /// POST api/lever/extend
        /// </summary>
        [HttpPost]
        [Route("extend")]
        public async Task<IHttpActionResult> Extend()
        {
            if (_hardwareService == null)
                return BadRequest("Hardware service not initialized");

            bool success = await _hardwareService.ExtendLeverAsync();

            // Log event
            _hardwareService.EventLogger.LogEvent("LeverExtend", "Lever", "", success);

            return Ok(new DeviceCommandResponse
            {
                RoomId = _hardwareService.GetCompartmentNo(),
                Success = success,
                State = "extending"
            });
        }

        /// <summary>
        /// POST api/lever/retract
        /// </summary>
        [HttpPost]
        [Route("retract")]
        public async Task<IHttpActionResult> Retract()
        {
            if (_hardwareService == null)
                return BadRequest("Hardware service not initialized");

            bool success = await _hardwareService.RetractLeverAsync();

            // Log event
            _hardwareService.EventLogger.LogEvent("LeverRetract", "Lever", "", success);

            return Ok(new DeviceCommandResponse
            {
                RoomId = _hardwareService.GetCompartmentNo(),
                Success = success,
                State = "retracting"
            });
        }
    }
}
