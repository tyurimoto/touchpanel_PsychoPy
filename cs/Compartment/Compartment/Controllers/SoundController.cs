using System.Threading.Tasks;
using System.Web.Http;
using Compartment.Models;
using Compartment.Services;

namespace Compartment.Controllers
{
    /// <summary>
    /// API Controller for sound playback
    /// </summary>
    [RoutePrefix("api/sound")]
    public class SoundController : ApiController
    {
        private static HardwareService _hardwareService;

        public static void Initialize(HardwareService hardwareService)
        {
            _hardwareService = hardwareService;
        }

        /// <summary>
        /// POST api/sound/play
        /// </summary>
        [HttpPost]
        [Route("play")]
        public async Task<IHttpActionResult> Play([FromBody] SoundRequest request)
        {
            if (_hardwareService == null)
                return BadRequest("Hardware service not initialized");

            if (request == null || string.IsNullOrEmpty(request.File))
                return BadRequest("Invalid request: file path required");

            bool success = await _hardwareService.PlaySoundAsync(request.File, request.DurationMs);

            _hardwareService.EventLogger.LogEvent(
                "SoundPlay", "Sound", request.File, success);

            return Ok(new
            {
                roomId = _hardwareService.GetCompartmentNo(),
                success = success,
                file = request.File,
                durationMs = request.DurationMs,
                timestamp = System.DateTime.Now
            });
        }
    }
}
