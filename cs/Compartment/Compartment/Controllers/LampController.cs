using System.Threading.Tasks;
using System.Web.Http;
using Compartment.Models;
using Compartment.Services;

namespace Compartment.Controllers
{
    /// <summary>
    /// API Controller for lamp control (room, lever, feed)
    /// </summary>
    [RoutePrefix("api/lamp")]
    public class LampController : ApiController
    {
        private static HardwareService _hardwareService;

        public static void Initialize(HardwareService hardwareService)
        {
            _hardwareService = hardwareService;
        }

        /// <summary>
        /// POST api/lamp/room
        /// </summary>
        [HttpPost]
        [Route("room")]
        public async Task<IHttpActionResult> RoomLamp([FromBody] LampRequest request)
        {
            if (_hardwareService == null)
                return BadRequest("Hardware service not initialized");

            if (request == null)
                return BadRequest("Invalid request");

            bool success = await _hardwareService.SetRoomLampAsync(request.On);

            _hardwareService.EventLogger.LogEvent(
                request.On ? "RoomLampOn" : "RoomLampOff", "Lamp", "", success);

            return Ok(new DeviceCommandResponse
            {
                RoomId = _hardwareService.GetCompartmentNo(),
                Success = success,
                State = request.On ? "on" : "off"
            });
        }

        /// <summary>
        /// POST api/lamp/lever
        /// </summary>
        [HttpPost]
        [Route("lever")]
        public async Task<IHttpActionResult> LeverLamp([FromBody] LampRequest request)
        {
            if (_hardwareService == null)
                return BadRequest("Hardware service not initialized");

            if (request == null)
                return BadRequest("Invalid request");

            bool success = await _hardwareService.SetLeverLampAsync(request.On);

            _hardwareService.EventLogger.LogEvent(
                request.On ? "LeverLampOn" : "LeverLampOff", "Lamp", "", success);

            return Ok(new DeviceCommandResponse
            {
                RoomId = _hardwareService.GetCompartmentNo(),
                Success = success,
                State = request.On ? "on" : "off"
            });
        }

        /// <summary>
        /// POST api/lamp/feed
        /// </summary>
        [HttpPost]
        [Route("feed")]
        public async Task<IHttpActionResult> FeedLamp([FromBody] LampRequest request)
        {
            if (_hardwareService == null)
                return BadRequest("Hardware service not initialized");

            if (request == null)
                return BadRequest("Invalid request");

            bool success = await _hardwareService.SetFeedLampAsync(request.On);

            _hardwareService.EventLogger.LogEvent(
                request.On ? "FeedLampOn" : "FeedLampOff", "Lamp", "", success);

            return Ok(new DeviceCommandResponse
            {
                RoomId = _hardwareService.GetCompartmentNo(),
                Success = success,
                State = request.On ? "on" : "off"
            });
        }
    }
}
