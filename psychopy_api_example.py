"""
Compartment Hardware Control API - PsychoPy Sample Code
========================================================

This script demonstrates how to control Compartment hardware from PsychoPy
using the REST API.

Requirements:
    pip install requests

Usage:
    1. Start Compartment application with Debug Mode enabled
    2. Run this script in PsychoPy or standalone Python
"""

import requests
import json
import logging
from typing import Dict, Any, Optional
from datetime import datetime

# API Configuration
API_BASE_URL = "http://localhost:5000"


class CompartmentAPI:
    """Client for Compartment Hardware Control API"""

    def __init__(self, base_url: str = API_BASE_URL, log_file: str = None):
        self.base_url = base_url.rstrip('/')
        self._setup_logger(log_file)

    def _setup_logger(self, log_file: Optional[str]):
        """Setup error logger"""
        self.logger = logging.getLogger('CompartmentAPI')
        self.logger.setLevel(logging.ERROR)

        if log_file:
            handler = logging.FileHandler(log_file)
        else:
            # Default log file with timestamp
            timestamp = datetime.now().strftime("%Y%m%d_%H%M%S")
            handler = logging.FileHandler(f"compartment_api_errors_{timestamp}.log")

        formatter = logging.Formatter('%(asctime)s - %(levelname)s - %(message)s')
        handler.setFormatter(formatter)
        self.logger.addHandler(handler)

    def _request(self, method: str, endpoint: str, data: Optional[Dict] = None) -> Dict[str, Any]:
        """Make HTTP request to API with error handling"""
        url = f"{self.base_url}{endpoint}"
        try:
            if method == "GET":
                response = requests.get(url, timeout=5)
            elif method == "POST":
                response = requests.post(url, json=data, timeout=5)
            else:
                raise ValueError(f"Unsupported method: {method}")

            response.raise_for_status()
            return response.json()
        except requests.exceptions.ConnectionError as e:
            error_msg = f"Connection error: Cannot connect to Compartment API at {url}"
            self.logger.error(error_msg)
            self.logger.error(f"Details: {str(e)}")
            print(f"❌ NETWORK ERROR: {error_msg}")
            raise CompartmentAPIError(error_msg) from e
        except requests.exceptions.Timeout as e:
            error_msg = f"Timeout error: API request to {url} timed out after 5 seconds"
            self.logger.error(error_msg)
            self.logger.error(f"Details: {str(e)}")
            print(f"❌ NETWORK ERROR: {error_msg}")
            raise CompartmentAPIError(error_msg) from e
        except requests.exceptions.HTTPError as e:
            error_msg = f"HTTP error {e.response.status_code}: {url}"
            self.logger.error(error_msg)
            self.logger.error(f"Response: {e.response.text}")
            print(f"❌ API ERROR: {error_msg}")
            raise CompartmentAPIError(error_msg) from e
        except requests.exceptions.RequestException as e:
            error_msg = f"Request error: {url}"
            self.logger.error(error_msg)
            self.logger.error(f"Details: {str(e)}")
            print(f"❌ NETWORK ERROR: {error_msg}")
            raise CompartmentAPIError(error_msg) from e


class CompartmentAPIError(Exception):
    """Exception raised when Compartment API encounters a network error"""
    pass

    # ========== Sensor APIs ==========

    def get_entrance_sensor(self) -> bool:
        """Check if animal is at entrance"""
        result = self._request("GET", "/api/sensor/entrance")
        return result.get("state", False)

    def get_exit_sensor(self) -> bool:
        """Check if animal is at exit"""
        result = self._request("GET", "/api/sensor/exit")
        return result.get("state", False)

    def get_stay_sensor(self) -> bool:
        """Check if animal is staying inside"""
        result = self._request("GET", "/api/sensor/stay")
        return result.get("state", False)

    def get_lever_switch(self) -> bool:
        """Check if lever switch is pressed"""
        result = self._request("GET", "/api/sensor/lever")
        return result.get("state", False)

    # ========== Door Control APIs ==========

    def open_door(self) -> bool:
        """Open the door"""
        result = self._request("POST", "/api/door/open")
        return result.get("success", False)

    def close_door(self) -> bool:
        """Close the door"""
        result = self._request("POST", "/api/door/close")
        return result.get("success", False)

    def get_door_status(self) -> Dict[str, Any]:
        """Get door status from sensors

        Returns:
            dict with keys:
                - state: "open", "closed", "moving", or "error"
                - sensorOpen: bool
                - sensorClose: bool
        """
        return self._request("GET", "/api/door/status")

    # ========== Lever Control APIs ==========

    def extend_lever(self) -> bool:
        """Extend the lever"""
        result = self._request("POST", "/api/lever/extend")
        return result.get("success", False)

    def retract_lever(self) -> bool:
        """Retract the lever"""
        result = self._request("POST", "/api/lever/retract")
        return result.get("success", False)

    # ========== Emergency Control APIs ==========

    def emergency_stop(self) -> bool:
        """Emergency stop all motors (door, lever, feed)

        IMPORTANT: Call this in finally block to ensure motors stop
        even if script crashes or encounters an error
        """
        result = self._request("POST", "/api/emergency/stop")
        return result.get("success", False)

    # ========== Feed Control APIs ==========

    def dispense_feed(self, duration_ms: int = 1000) -> bool:
        """Dispense feed for specified duration"""
        result = self._request("POST", "/api/feed/dispense", {"durationMs": duration_ms})
        return result.get("success", False)

    def is_feeding(self) -> bool:
        """Check if currently feeding"""
        result = self._request("GET", "/api/feed/status")
        return result.get("isFeeding", False)

    # ========== RFID APIs ==========

    def read_rfid(self) -> str:
        """Read current RFID"""
        result = self._request("GET", "/api/rfid/read")
        return result.get("id", "")

    def clear_rfid(self) -> bool:
        """Clear RFID value"""
        result = self._request("POST", "/api/rfid/clear")
        return result.get("success", False)

    # ========== Debug Mode APIs (for testing without hardware) ==========

    def debug_get_status(self) -> Dict[str, Any]:
        """Get debug mode status"""
        return self._request("GET", "/api/debug/status")

    def debug_set_sensor(self, sensor: str, state: bool) -> bool:
        """Set sensor state in debug mode

        Args:
            sensor: "entrance", "exit", "stay", "dooropen", "doorclose",
                   "leverin", "leverout", "leversw"
            state: True (ON) or False (OFF)
        """
        result = self._request("POST", "/api/debug/sensor/set",
                              {"sensor": sensor, "state": state})
        return result.get("success", False)

    def debug_set_rfid(self, rfid_id: str) -> bool:
        """Set RFID value in debug mode"""
        result = self._request("POST", "/api/debug/rfid/set", {"id": rfid_id})
        return result.get("success", False)

    def debug_set_random_rfid(self) -> str:
        """Generate and set random RFID"""
        result = self._request("POST", "/api/debug/rfid/random")
        return result.get("id", "")

    def debug_get_all_sensors(self) -> Dict[str, bool]:
        """Get all sensor states"""
        result = self._request("GET", "/api/debug/sensors/all")
        return result.get("sensors", {})

    def debug_reset_state(self) -> bool:
        """Reset all debug states to default"""
        result = self._request("POST", "/api/debug/reset")
        return result.get("success", False)


# ========== Helper Functions ==========

def wait_for_door_state(api: CompartmentAPI, expected_state: str,
                       timeout: float = 5.0, fallback_time: float = 2.0) -> bool:
    """
    Wait for door to reach expected state with timeout and fallback

    Args:
        api: CompartmentAPI instance
        expected_state: "open" or "closed"
        timeout: Sensor check timeout in seconds (default: 5.0)
        fallback_time: Fallback wait time if sensor doesn't respond (default: 2.0)

    Returns:
        True if sensor confirmed, False if timed out (but continues with fallback)
    """
    import time

    start = time.time()
    sensor_working = False

    while time.time() - start < timeout:
        status = api.get_door_status()
        if status.get("state") == expected_state:
            print(f"✓ Door {expected_state} confirmed by sensor")
            sensor_working = True
            break
        time.sleep(0.1)

    if not sensor_working:
        print(f"⚠ Warning: Door sensor timeout ({timeout}s)")
        print(f"   → Waiting {fallback_time}s and continuing (sensor may be faulty)")
        time.sleep(fallback_time)

    return sensor_working


def wait_for_sensor(api: CompartmentAPI, sensor_getter, sensor_name: str,
                   timeout: float = None) -> bool:
    """
    Wait for sensor to activate (with optional timeout)

    Args:
        api: CompartmentAPI instance
        sensor_getter: Function to get sensor state (e.g., api.get_entrance_sensor)
        sensor_name: Sensor name for logging
        timeout: Optional timeout in seconds (None = wait forever)

    Returns:
        True if sensor activated, False if timed out
    """
    import time

    start = time.time()

    while True:
        if sensor_getter():
            return True

        if timeout and (time.time() - start > timeout):
            print(f"⚠ Warning: {sensor_name} sensor timeout ({timeout}s)")
            return False

        time.sleep(0.1)


# ========== Example Usage ==========

def example_basic_trial():
    """Example: Basic trial flow with network error handling and emergency stop"""
    import time
    api = CompartmentAPI()

    print("=== Basic Trial Example ===")

    try:
        # 1. Wait for animal to enter
        print("Waiting for animal at entrance...")
        wait_for_sensor(api, api.get_entrance_sensor, "entrance")
        print("Animal detected at entrance!")

        # 2. Open door (with safety check)
        print("Opening door...")
        if not api.open_door():
            print("Error: Cannot open door (animal may be inside)")
            return

        # 3. Wait for door to fully open (with timeout and fallback)
        print("Waiting for door to open...")
        wait_for_door_state(api, "open", timeout=5.0, fallback_time=2.0)

        # 4. Wait for animal to enter room
        print("Waiting for animal to enter...")
        wait_for_sensor(api, api.get_stay_sensor, "stay")
        print("Animal entered room!")

        # 5. Close door
        print("Closing door...")
        api.close_door()

        # 6. Wait for door to fully close (with timeout and fallback)
        print("Waiting for door to close...")
        wait_for_door_state(api, "closed", timeout=5.0, fallback_time=2.0)

        # 7. Extend lever
        print("Extending lever...")
        api.extend_lever()

        # 8. Wait for lever press
        print("Waiting for lever press...")
        wait_for_sensor(api, api.get_lever_switch, "lever switch")
        print("Lever pressed!")

        # 9. Dispense reward
        print("Dispensing reward...")
        api.dispense_feed(duration_ms=500)

        # 10. Retract lever
        print("Retracting lever...")
        api.retract_lever()

        print("Trial complete!")

    except CompartmentAPIError as e:
        # Network error occurred - stop experiment immediately
        print(f"\n❌ EXPERIMENT STOPPED DUE TO NETWORK ERROR")
        print(f"Error details: {e}")
        print(f"Check log file: compartment_api_errors_*.log")

        # Try emergency stop (may also fail if network is down)
        try:
            print("Attempting emergency stop...")
            api.emergency_stop()
            print("Emergency stop successful")
        except:
            print("⚠ Warning: Emergency stop failed (network may be down)")
            print("⚠ Please manually stop motors using Compartment application")

        # Re-raise to stop experiment
        raise

    finally:
        # CRITICAL: Always stop all motors in case of error or interruption
        # This may fail if network error occurred, but we try anyway
        try:
            print("Stopping all motors (safety cleanup)...")
            api.emergency_stop()
        except:
            pass  # Already handled in except block


def example_debug_mode():
    """Example: Testing with debug mode (no real hardware)"""
    api = CompartmentAPI()

    print("=== Debug Mode Example ===")

    # Check if debug mode is enabled
    status = api.debug_get_status()
    print(f"Debug mode enabled: {status.get('debugModeEnabled', False)}")

    # Simulate animal at entrance
    print("\nSimulating animal at entrance...")
    api.debug_set_sensor("entrance", True)

    # Check sensor state
    entrance_state = api.get_entrance_sensor()
    print(f"Entrance sensor: {entrance_state}")

    # Set RFID
    print("\nSetting RFID...")
    api.debug_set_rfid("1234567890123456")
    rfid = api.read_rfid()
    print(f"Current RFID: {rfid}")

    # Get all sensor states
    print("\nAll sensor states:")
    sensors = api.debug_get_all_sensors()
    for sensor_name, state in sensors.items():
        print(f"  {sensor_name}: {state}")

    # Reset all states
    print("\nResetting all states...")
    api.debug_reset_state()


def example_with_psychopy():
    """Example: Integration with PsychoPy experiment with network error handling"""
    from psychopy import core, event

    api = CompartmentAPI()

    print("=== PsychoPy Integration Example ===")

    try:
        # Trial loop
        for trial_num in range(1, 4):
            print(f"\n--- Trial {trial_num} ---")

            # Present visual stimulus (PsychoPy code here)
            # ...

            # Control hardware
            api.extend_lever()

            # Wait for response with timeout
            timer = core.Clock()
            timeout = 10.0  # 10 seconds
            responded = False

            while timer.getTime() < timeout:
                if api.get_lever_switch():
                    print("Correct response!")
                    api.dispense_feed(duration_ms=500)
                    responded = True
                    break

                # Check for keyboard abort
                if event.getKeys(['escape']):
                    print("Experiment aborted")
                    return

                core.wait(0.01)

            if not responded:
                print("No response (timeout)")

            # Retract lever and prepare for next trial
            api.retract_lever()
            core.wait(2.0)  # ITI

    except CompartmentAPIError as e:
        # Network error occurred - stop experiment
        print(f"\n❌ EXPERIMENT STOPPED DUE TO NETWORK ERROR")
        print(f"Error: {e}")

        try:
            api.emergency_stop()
        except:
            print("⚠ Emergency stop failed - manually stop motors")

        raise

    finally:
        # Always try to stop motors
        try:
            api.emergency_stop()
        except:
            pass


if __name__ == "__main__":
    # Run examples
    print("Compartment API Examples\n")

    # Test API connection
    api = CompartmentAPI()
    status = api.debug_get_status()

    if "error" in status:
        print("ERROR: Cannot connect to Compartment API")
        print("Please make sure:")
        print("  1. Compartment application is running")
        print("  2. API server is started (check for 'API起動' message)")
        print("  3. Port 5000 is not blocked by firewall")
    else:
        print(f"✓ Connected to Compartment API")
        print(f"  Room ID: {status.get('roomId', 'N/A')}")
        print(f"  Debug Mode: {status.get('debugModeEnabled', False)}")
        print(f"  Hardware Connected: {status.get('hardwareConnected', False)}")
        print()

        # Run debug mode example
        example_debug_mode()

        # Uncomment to run other examples:
        # example_basic_trial()
        # example_with_psychopy()
