namespace Compartment.Models
{
    /// <summary>
    /// Request model for lamp control
    /// </summary>
    public class LampRequest
    {
        /// <summary>
        /// true = ON, false = OFF
        /// </summary>
        public bool On { get; set; }
    }
}
