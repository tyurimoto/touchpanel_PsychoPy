namespace Compartment.Models
{
    /// <summary>
    /// Response model for device commands (door, lever, etc.)
    /// </summary>
    public class DeviceCommandResponse : ApiResponseBase
    {
        /// <summary>
        /// Command execution success
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Current device state
        /// </summary>
        public string State { get; set; }
    }
}
