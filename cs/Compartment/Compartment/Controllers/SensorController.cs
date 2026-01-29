using System.Threading.Tasks;
using System.Web.Http;
using Compartment.Models;
using Compartment.Services;

namespace Compartment.Controllers
{
    /// <summary>
    /// API Controller for sensor status
    /// </summary>
    [RoutePrefix("api/sensor")]
    public class SensorController : ApiController
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
        /// GET api/sensor/entrance
        /// </summary>
        [HttpGet]
        [Route("entrance")]
        public async Task<IHttpActionResult> GetEntrance()
        {
            if (_hardwareService == null)
                return BadRequest("Hardware service not initialized");

            bool active = await _hardwareService.GetEntranceSensorAsync();
            return Ok(new SensorStatusResponse
            {
                RoomId = _hardwareService.GetCompartmentNo(),
                Active = active
            });
        }

        /// <summary>
        /// GET api/sensor/exit
        /// </summary>
        [HttpGet]
        [Route("exit")]
        public async Task<IHttpActionResult> GetExit()
        {
            if (_hardwareService == null)
                return BadRequest("Hardware service not initialized");

            bool active = await _hardwareService.GetExitSensorAsync();
            return Ok(new SensorStatusResponse
            {
                RoomId = _hardwareService.GetCompartmentNo(),
                Active = active
            });
        }

        /// <summary>
        /// GET api/sensor/stay
        /// </summary>
        [HttpGet]
        [Route("stay")]
        public async Task<IHttpActionResult> GetStay()
        {
            if (_hardwareService == null)
                return BadRequest("Hardware service not initialized");

            bool active = await _hardwareService.GetStaySensorAsync();
            return Ok(new SensorStatusResponse
            {
                RoomId = _hardwareService.GetCompartmentNo(),
                Active = active
            });
        }

        /// <summary>
        /// GET api/sensor/lever
        /// </summary>
        [HttpGet]
        [Route("lever")]
        public async Task<IHttpActionResult> GetLever()
        {
            if (_hardwareService == null)
                return BadRequest("Hardware service not initialized");

            bool pressed = await _hardwareService.GetLeverSwitchAsync();
            return Ok(new
            {
                roomId = _hardwareService.GetCompartmentNo(),
                pressed = pressed,
                timestamp = System.DateTime.Now
            });
        }
    }
}
