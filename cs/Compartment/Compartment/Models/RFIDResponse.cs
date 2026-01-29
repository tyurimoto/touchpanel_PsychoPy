namespace Compartment.Models
{
    /// <summary>
    /// Response model for RFID reading
    /// </summary>
    public class RFIDResponse : ApiResponseBase
    {
        /// <summary>
        /// RFID ID string
        /// </summary>
        public string Id { get; set; }
    }
}
