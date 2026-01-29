namespace Compartment.Models
{
    /// <summary>
    /// Response model for sensor status
    /// </summary>
    public class SensorStatusResponse : ApiResponseBase
    {
        /// <summary>
        /// Sensor active state
        /// </summary>
        public bool Active { get; set; }
    }
}
