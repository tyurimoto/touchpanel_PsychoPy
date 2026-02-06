using System;
using System.Threading.Tasks;
using System.Web.Http;
using Compartment.Services;

namespace Compartment.Controllers
{
    /// <summary>
    /// API Controller for room/cage status and entry/exit waiting
    /// </summary>
    [RoutePrefix("api/room")]
    public class RoomController : ApiController
    {
        private static HardwareService _hardwareService;

        public static void Initialize(HardwareService hardwareService)
        {
            _hardwareService = hardwareService;
        }

        /// <summary>
        /// GET api/room/status
        /// </summary>
        [HttpGet]
        [Route("status")]
        public IHttpActionResult GetStatus()
        {
            if (_hardwareService == null)
                return BadRequest("Hardware service not initialized");

            var status = _hardwareService.GetRoomStatus();
            return Ok(status);
        }

        /// <summary>
        /// GET api/room/wait-entry?timeout=60000
        /// </summary>
        [HttpGet]
        [Route("wait-entry")]
        public async Task<IHttpActionResult> WaitEntry(int timeout = 60000)
        {
            if (_hardwareService == null)
                return BadRequest("Hardware service not initialized");

            bool success = await _hardwareService.WaitForEntryAsync(timeout);

            _hardwareService.EventLogger.LogEvent(
                success ? "EntryDetected" : "EntryTimeout", "Room", "", success);

            return Ok(new
            {
                roomId = _hardwareService.GetCompartmentNo(),
                success = success,
                message = success ? "Animal entered" : "Timeout",
                timestamp = DateTime.Now
            });
        }

        /// <summary>
        /// GET api/room/wait-exit?timeout=60000
        /// </summary>
        [HttpGet]
        [Route("wait-exit")]
        public async Task<IHttpActionResult> WaitExit(int timeout = 60000)
        {
            if (_hardwareService == null)
                return BadRequest("Hardware service not initialized");

            bool success = await _hardwareService.WaitForExitAsync(timeout);

            _hardwareService.EventLogger.LogEvent(
                success ? "ExitDetected" : "ExitTimeout", "Room", "", success);

            return Ok(new
            {
                roomId = _hardwareService.GetCompartmentNo(),
                success = success,
                message = success ? "Animal exited" : "Timeout",
                timestamp = DateTime.Now
            });
        }
    }
}
