using System;

namespace Compartment.Models
{
    /// <summary>
    /// Base class for all API responses
    /// </summary>
    public class ApiResponseBase
    {
        /// <summary>
        /// Room/Compartment ID for multi-room orchestration
        /// </summary>
        public string RoomId { get; set; }

        /// <summary>
        /// Timestamp of the response
        /// </summary>
        public DateTime Timestamp { get; set; }

        public ApiResponseBase()
        {
            Timestamp = DateTime.Now;
        }
    }
}
