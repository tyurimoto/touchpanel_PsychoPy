namespace Compartment.Models
{
    /// <summary>
    /// Request model for sound playback
    /// </summary>
    public class SoundRequest
    {
        /// <summary>
        /// Path to sound file
        /// </summary>
        public string File { get; set; }

        /// <summary>
        /// Duration in milliseconds (default: 1000)
        /// </summary>
        public int DurationMs { get; set; } = 1000;
    }
}
