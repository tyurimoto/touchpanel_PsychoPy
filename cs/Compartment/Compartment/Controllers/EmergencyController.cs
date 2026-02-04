using System.Threading.Tasks;
using System.Web.Http;
using Compartment.Services;

namespace Compartment.Controllers
{
    /// <summary>
    /// Emergency control API for stopping all motors
    /// </summary>
    [RoutePrefix("api/emergency")]
    public class EmergencyController : ApiController
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
        /// POST api/emergency/stop
        /// Stop all motors immediately (door, lever, feed)
        /// Should be called in PsychoPy script's finally block
        /// </summary>
        [HttpPost]
        [Route("stop")]
        public async Task<IHttpActionResult> StopAll()
        {
            if (_hardwareService == null)
                return BadRequest("Hardware service not initialized");

            bool success = await _hardwareService.EmergencyStopAllAsync();

            // Log event
            _hardwareService.EventLogger.LogEvent("EmergencyStop", "AllMotors", "", success,
                "Door, Lever, Feed motors stopped");

            return Ok(new
            {
                roomId = _hardwareService.GetCompartmentNo(),
                success = success,
                message = "All motors stopped",
                timestamp = System.DateTime.Now
            });
        }
    }
}
