using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Compartment.Services
{
    /// <summary>
    /// Event logger for ExternalControl mode
    /// Logs all hardware commands with timestamps (command execution only, no sensor monitoring)
    /// </summary>
    public class EventLogger
    {
        private readonly object _lock = new object();
        private StreamWriter _writer = null;
        private string _currentLogFile = null;
        private readonly string _roomId;
        private bool _isEnabled = false;

        public EventLogger(string roomId)
        {
            _roomId = roomId;
        }

        /// <summary>
        /// Enable logging for ExternalControl mode
        /// Creates daily log files: ExternalControl_Room{N}_{YYYYMMDD}.csv
        /// </summary>
        public void Enable(string logFolder = null)
        {
            lock (_lock)
            {
                if (_isEnabled) return;

                try
                {
                    // Use user app data folder if not specified
                    if (string.IsNullOrEmpty(logFolder))
                    {
                        logFolder = Application.UserAppDataPath;
                    }

                    // Create log file name with date
                    string dateStr = DateTime.Now.ToString("yyyyMMdd");
                    string fileName = $"ExternalControl_Room{_roomId}_{dateStr}.csv";
                    _currentLogFile = Path.Combine(logFolder, fileName);

                    bool fileExists = File.Exists(_currentLogFile);

                    // Open file in append mode
                    _writer = new StreamWriter(_currentLogFile, true, Encoding.UTF8);
                    _writer.AutoFlush = true; // Ensure data is written immediately

                    // Write header if new file
                    if (!fileExists)
                    {
                        _writer.WriteLine("Timestamp,EventType,Device,Parameter,Success,Message");
                    }

                    _isEnabled = true;
                    System.Diagnostics.Debug.WriteLine($"[EventLogger] Enabled: {_currentLogFile}");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[EventLogger] Failed to enable: {ex.Message}");
                    _isEnabled = false;
                }
            }
        }

        /// <summary>
        /// Disable logging and close file
        /// </summary>
        public void Disable()
        {
            lock (_lock)
            {
                if (!_isEnabled) return;

                try
                {
                    _writer?.Close();
                    _writer?.Dispose();
                    _writer = null;
                    _isEnabled = false;
                    System.Diagnostics.Debug.WriteLine("[EventLogger] Disabled");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[EventLogger] Error during disable: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Log hardware command execution
        /// </summary>
        /// <param name="eventType">Event type (e.g., "DoorOpen", "FeedDispense")</param>
        /// <param name="device">Device name (e.g., "Door", "Lever", "Feeder")</param>
        /// <param name="parameter">Optional parameter (e.g., "1000ms" for feed duration)</param>
        /// <param name="success">Whether command succeeded</param>
        /// <param name="message">Optional message</param>
        public void LogEvent(string eventType, string device, string parameter, bool success, string message = "")
        {
            if (!_isEnabled) return;

            Task.Run(() =>
            {
                try
                {
                    lock (_lock)
                    {
                        if (_writer == null) return;

                        string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                        string paramStr = string.IsNullOrEmpty(parameter) ? "-" : parameter;
                        string msgStr = string.IsNullOrEmpty(message) ? "" : message;

                        string line = $"{timestamp},{eventType},{device},{paramStr},{success},{msgStr}";
                        _writer.WriteLine(line);
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[EventLogger] Failed to write log: {ex.Message}");
                }
            });
        }

        /// <summary>
        /// Check if logger is enabled
        /// </summary>
        public bool IsEnabled => _isEnabled;

        /// <summary>
        /// Get current log file path
        /// </summary>
        public string CurrentLogFile => _currentLogFile;
    }
}
