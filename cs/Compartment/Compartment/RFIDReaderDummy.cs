using System;

namespace Compartment
{
    /// <summary>
    /// Dummy RFID reader for debug mode.
    /// Allows manual setting of RFID values without physical hardware.
    /// </summary>
    public class RFIDReaderDummy
    {
        private readonly object _syncLock = new object();

        /// <summary>
        /// Current ID Code (compatible with RFIDReaderHelper interface)
        /// </summary>
        public SyncObject<string> CurrentIDCode { get; set; }

        /// <summary>
        /// Callback for received data (compatible with RFIDReaderHelper interface)
        /// </summary>
        public Action<string> callbackReceivedDataSub { get; set; }

        public RFIDReaderDummy()
        {
            CurrentIDCode = new SyncObject<string>("");
            callbackReceivedDataSub = (str) => { };
        }

        /// <summary>
        /// Manually set RFID value (for debug/simulation)
        /// </summary>
        /// <param name="rfidValue">RFID string (e.g., "3920145000567278")</param>
        public void SetRFID(string rfidValue)
        {
            lock (_syncLock)
            {
                CurrentIDCode.Value = rfidValue;
                // Trigger callback like real hardware would
                callbackReceivedDataSub?.Invoke(rfidValue);
            }
        }

        /// <summary>
        /// Clear current RFID value
        /// </summary>
        public void ClearRFID()
        {
            lock (_syncLock)
            {
                CurrentIDCode.Value = "";
            }
        }

        /// <summary>
        /// Generate a random RFID for testing
        /// </summary>
        /// <returns>Random 16-digit RFID string</returns>
        public string GenerateRandomRFID()
        {
            var random = new Random();
            string rfid = "";

            // Generate 16 random digits
            for (int i = 0; i < 16; i++)
            {
                rfid += random.Next(0, 10).ToString();
            }

            return rfid;
        }

        /// <summary>
        /// Set a random RFID and return it
        /// </summary>
        public string SetRandomRFID()
        {
            string rfid = GenerateRandomRFID();
            SetRFID(rfid);
            return rfid;
        }

        /// <summary>
        /// Get current RFID value
        /// </summary>
        public string GetCurrentRFID()
        {
            lock (_syncLock)
            {
                return CurrentIDCode.Value;
            }
        }

        /// <summary>
        /// Check if RFID is present (non-empty)
        /// </summary>
        public bool HasRFID()
        {
            lock (_syncLock)
            {
                return !string.IsNullOrEmpty(CurrentIDCode.Value);
            }
        }

        // Compatibility methods with RFIDReaderHelper interface
        public Action<byte[]> GetDosetIDAction()
        {
            // Dummy implementation - not used in debug mode
            return (datagram) => { };
        }

        public Action<byte[]> GetUnivrsalIDAction()
        {
            // Dummy implementation - not used in debug mode
            return (datagram) => { };
        }
    }
}
