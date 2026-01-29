using System.Threading.Tasks;
using System.Web.Http;
using Compartment.Models;
using Compartment.Services;

namespace Compartment.Controllers
{
    /// <summary>
    /// API Controller for RFID reading
    /// </summary>
    [RoutePrefix("api/rfid")]
    public class RFIDController : ApiController
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
        /// GET api/rfid/read
        /// </summary>
        [HttpGet]
        [Route("read")]
        public async Task<IHttpActionResult> Read()
        {
            if (_hardwareService == null)
                return BadRequest("Hardware service not initialized");

            string rfid = await _hardwareService.ReadRFIDAsync();
            return Ok(new RFIDResponse
            {
                RoomId = _hardwareService.GetCompartmentNo(),
                Id = rfid
            });
        }

        /// <summary>
        /// DELETE api/rfid
        /// </summary>
        [HttpDelete]
        public async Task<IHttpActionResult> Clear()
        {
            if (_hardwareService == null)
                return BadRequest("Hardware service not initialized");

            bool success = await _hardwareService.ClearRFIDAsync();
            return Ok(new
            {
                roomId = _hardwareService.GetCompartmentNo(),
                success = success,
                timestamp = System.DateTime.Now
            });
        }
    }
}
