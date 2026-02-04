using System.Threading.Tasks;
using System.Web.Http;
using Compartment.Models;
using Compartment.Services;

namespace Compartment.Controllers
{
    /// <summary>
    /// API Controller for feeding control
    /// </summary>
    [RoutePrefix("api/feed")]
    public class FeedController : ApiController
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
        /// POST api/feed/dispense
        /// </summary>
        [HttpPost]
        [Route("dispense")]
        public async Task<IHttpActionResult> Dispense([FromBody] FeedRequest request)
        {
            if (_hardwareService == null)
                return BadRequest("Hardware service not initialized");

            if (request == null || request.DurationMs <= 0)
                return BadRequest("Invalid duration");

            bool success = await _hardwareService.DispenseFeedAsync(request.DurationMs);
            bool feeding = await _hardwareService.IsFeedingAsync();

            // Log event
            _hardwareService.EventLogger.LogEvent("FeedDispense", "Feeder",
                $"{request.DurationMs}ms", success);

            return Ok(new
            {
                roomId = _hardwareService.GetCompartmentNo(),
                success = success,
                feeding = feeding,
                timestamp = System.DateTime.Now
            });
        }

        /// <summary>
        /// GET api/feed/status
        /// </summary>
        [HttpGet]
        [Route("status")]
        public async Task<IHttpActionResult> GetStatus()
        {
            if (_hardwareService == null)
                return BadRequest("Hardware service not initialized");

            bool feeding = await _hardwareService.IsFeedingAsync();
            return Ok(new
            {
                roomId = _hardwareService.GetCompartmentNo(),
                feeding = feeding,
                timestamp = System.DateTime.Now
            });
        }
    }
}
