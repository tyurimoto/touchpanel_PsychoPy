"""
Compartment Hardware API Client Library
PsychoPyから C# Compartment Hardware API を制御するためのライブラリ
"""

import requests
import time
from typing import Optional, Dict, Any
from datetime import datetime


class CompartmentHardware:
    """
    Compartment hardware control client for PsychoPy

    Usage:
        hw = CompartmentHardware(base_url="http://localhost:5000/api")

        # センサー読み取り
        if hw.check_entrance():
            print("入室検出")

        # RFID読み取り
        rfid = hw.read_rfid()

        # ドア制御
        hw.open_door()
        hw.close_door()

        # レバー制御
        hw.extend_lever()
        hw.retract_lever()

        # 給餌
        hw.dispense_feed(duration_ms=1000)
    """

    def __init__(self, base_url: str = "http://localhost:5000/api",
                 debug_mode: bool = False,
                 timeout: int = 5):
        """
        Initialize hardware client

        Args:
            base_url: API server base URL (default: http://localhost:5000/api)
            debug_mode: Enable debug mode features (default: False)
            timeout: Request timeout in seconds (default: 5)
        """
        self.base_url = base_url.rstrip('/')
        self.debug_mode = debug_mode
        self.timeout = timeout

    # ===== センサー読み取り =====

    def check_entrance(self) -> bool:
        """入室センサーの状態をチェック"""
        try:
            response = requests.get(f"{self.base_url}/sensor/entrance", timeout=self.timeout)
            response.raise_for_status()
            return response.json().get('active', False)
        except Exception as e:
            print(f"Error checking entrance sensor: {e}")
            return False

    def check_exit(self) -> bool:
        """退室センサーの状態をチェック"""
        try:
            response = requests.get(f"{self.base_url}/sensor/exit", timeout=self.timeout)
            response.raise_for_status()
            return response.json().get('active', False)
        except Exception as e:
            print(f"Error checking exit sensor: {e}")
            return False

    def check_stay(self) -> bool:
        """在室センサーの状態をチェック"""
        try:
            response = requests.get(f"{self.base_url}/sensor/stay", timeout=self.timeout)
            response.raise_for_status()
            return response.json().get('active', False)
        except Exception as e:
            print(f"Error checking stay sensor: {e}")
            return False

    def check_lever_switch(self) -> bool:
        """レバースイッチの状態をチェック"""
        try:
            response = requests.get(f"{self.base_url}/sensor/lever", timeout=self.timeout)
            response.raise_for_status()
            return response.json().get('pressed', False)
        except Exception as e:
            print(f"Error checking lever switch: {e}")
            return False

    # ===== RFID =====

    def read_rfid(self) -> Optional[str]:
        """RFID値を読み取る"""
        try:
            response = requests.get(f"{self.base_url}/rfid/read", timeout=self.timeout)
            response.raise_for_status()
            data = response.json()
            rfid_id = data.get('id', '')
            return rfid_id if rfid_id else None
        except Exception as e:
            print(f"Error reading RFID: {e}")
            return None

    def clear_rfid(self) -> bool:
        """RFID値をクリア"""
        try:
            response = requests.delete(f"{self.base_url}/rfid", timeout=self.timeout)
            response.raise_for_status()
            return response.json().get('success', False)
        except Exception as e:
            print(f"Error clearing RFID: {e}")
            return False

    # ===== ドア制御 =====

    def open_door(self) -> bool:
        """ドアを開く"""
        try:
            response = requests.post(f"{self.base_url}/door/open", timeout=self.timeout)
            response.raise_for_status()
            return response.json().get('success', False)
        except Exception as e:
            print(f"Error opening door: {e}")
            return False

    def close_door(self) -> bool:
        """ドアを閉じる"""
        try:
            response = requests.post(f"{self.base_url}/door/close", timeout=self.timeout)
            response.raise_for_status()
            return response.json().get('success', False)
        except Exception as e:
            print(f"Error closing door: {e}")
            return False

    def get_door_status(self) -> Dict[str, Any]:
        """ドアの状態を取得"""
        try:
            response = requests.get(f"{self.base_url}/door/status", timeout=self.timeout)
            response.raise_for_status()
            return response.json()
        except Exception as e:
            print(f"Error getting door status: {e}")
            return {'state': 'unknown', 'sensorOpen': False, 'sensorClose': False}

    # ===== レバー制御 =====

    def extend_lever(self) -> bool:
        """レバーを出す"""
        try:
            response = requests.post(f"{self.base_url}/lever/extend", timeout=self.timeout)
            response.raise_for_status()
            return response.json().get('success', False)
        except Exception as e:
            print(f"Error extending lever: {e}")
            return False

    def retract_lever(self) -> bool:
        """レバーを引っ込める"""
        try:
            response = requests.post(f"{self.base_url}/lever/retract", timeout=self.timeout)
            response.raise_for_status()
            return response.json().get('success', False)
        except Exception as e:
            print(f"Error retracting lever: {e}")
            return False

    # ===== 給餌 =====

    def dispense_feed(self, duration_ms: int = 1000) -> bool:
        """
        給餌を実行

        Args:
            duration_ms: 給餌時間（ミリ秒）
        """
        try:
            response = requests.post(
                f"{self.base_url}/feed/dispense",
                json={'durationMs': duration_ms},
                timeout=self.timeout
            )
            response.raise_for_status()
            return response.json().get('success', False)
        except Exception as e:
            print(f"Error dispensing feed: {e}")
            return False

    def is_feeding(self) -> bool:
        """給餌中かチェック"""
        try:
            response = requests.get(f"{self.base_url}/feed/status", timeout=self.timeout)
            response.raise_for_status()
            return response.json().get('feeding', False)
        except Exception as e:
            print(f"Error checking feed status: {e}")
            return False

    # ===== デバッグモード専用メソッド =====

    def debug_set_sensor(self, sensor_name: str, state: bool) -> bool:
        """
        デバッグ: センサー状態を設定

        Args:
            sensor_name: センサー名 ('entrance', 'exit', 'stay', 'lever')
            state: センサー状態 (True=ON, False=OFF)
        """
        if not self.debug_mode:
            print("Warning: debug_set_sensor called in non-debug mode")
            return False

        try:
            response = requests.post(
                f"{self.base_url}/debug/sensor/set",
                json={'sensor': sensor_name, 'state': state},
                timeout=self.timeout
            )
            response.raise_for_status()
            return response.json().get('success', False)
        except Exception as e:
            print(f"Error setting debug sensor: {e}")
            return False

    def debug_set_rfid(self, rfid_id: str) -> bool:
        """
        デバッグ: RFID値を設定

        Args:
            rfid_id: RFID文字列（16桁）
        """
        if not self.debug_mode:
            print("Warning: debug_set_rfid called in non-debug mode")
            return False

        try:
            response = requests.post(
                f"{self.base_url}/debug/rfid/set",
                json={'id': rfid_id},
                timeout=self.timeout
            )
            response.raise_for_status()
            return response.json().get('success', False)
        except Exception as e:
            print(f"Error setting debug RFID: {e}")
            return False

    def debug_set_random_rfid(self) -> Optional[str]:
        """デバッグ: ランダムなRFIDを生成・設定"""
        if not self.debug_mode:
            print("Warning: debug_set_random_rfid called in non-debug mode")
            return None

        try:
            response = requests.post(
                f"{self.base_url}/debug/rfid/random",
                timeout=self.timeout
            )
            response.raise_for_status()
            return response.json().get('id')
        except Exception as e:
            print(f"Error setting random RFID: {e}")
            return None

    def debug_get_all_sensors(self) -> Dict[str, bool]:
        """デバッグ: すべてのセンサー状態を取得"""
        if not self.debug_mode:
            print("Warning: debug_get_all_sensors called in non-debug mode")
            return {}

        try:
            response = requests.get(
                f"{self.base_url}/debug/sensors/all",
                timeout=self.timeout
            )
            response.raise_for_status()
            return response.json().get('sensors', {})
        except Exception as e:
            print(f"Error getting all sensors: {e}")
            return {}

    def debug_reset(self) -> bool:
        """デバッグ: すべての状態をリセット"""
        if not self.debug_mode:
            print("Warning: debug_reset called in non-debug mode")
            return False

        try:
            response = requests.post(
                f"{self.base_url}/debug/reset",
                timeout=self.timeout
            )
            response.raise_for_status()
            return response.json().get('success', False)
        except Exception as e:
            print(f"Error resetting debug state: {e}")
            return False

    def debug_get_task_status(self) -> Dict[str, Any]:
        """デバッグ: 課題の状態を取得"""
        try:
            response = requests.get(
                f"{self.base_url}/debug/task/status",
                timeout=self.timeout
            )
            response.raise_for_status()
            return response.json().get('task', {})
        except Exception as e:
            print(f"Error getting task status: {e}")
            return {}

    def debug_get_task_history(self) -> list:
        """デバッグ: 試行履歴を取得"""
        try:
            response = requests.get(
                f"{self.base_url}/debug/task/history",
                timeout=self.timeout
            )
            response.raise_for_status()
            data = response.json()
            return data.get('history', {}).get('trials', [])
        except Exception as e:
            print(f"Error getting task history: {e}")
            return []

    # ===== 結果報告 =====

    @staticmethod
    def report_result(correct: bool):
        """
        課題の結果をC#エンジンに報告する

        ハイブリッドモード用: stdoutに RESULT:CORRECT または RESULT:INCORRECT を出力する
        C#側がこの出力をパースして給餌判定を行う

        Args:
            correct: 正解の場合True、不正解の場合False
        """
        if correct:
            print("RESULT:CORRECT")
        else:
            print("RESULT:INCORRECT")

    # ===== ヘルパーメソッド =====

    def debug_simulate_entrance(self) -> bool:
        """デバッグ: 入室をシミュレート（500ms後に自動OFF）"""
        if not self.debug_mode:
            return False

        self.debug_set_sensor('entrance', True)
        time.sleep(0.5)
        self.debug_set_sensor('entrance', False)
        return True

    def debug_simulate_exit(self) -> bool:
        """デバッグ: 退室をシミュレート（500ms後に自動OFF）"""
        if not self.debug_mode:
            return False

        self.debug_set_sensor('exit', True)
        time.sleep(0.5)
        self.debug_set_sensor('exit', False)
        return True

    def wait_for_entrance(self, timeout_sec: float = None) -> bool:
        """
        入室を待つ

        Args:
            timeout_sec: タイムアウト時間（秒）。Noneの場合は無制限

        Returns:
            True: 入室検出, False: タイムアウト
        """
        start_time = time.time()
        while True:
            if self.check_entrance():
                return True

            if timeout_sec is not None:
                if time.time() - start_time > timeout_sec:
                    return False

            time.sleep(0.05)  # 50msごとにチェック

    def wait_for_rfid(self, timeout_sec: float = None) -> Optional[str]:
        """
        RFIDを待つ

        Args:
            timeout_sec: タイムアウト時間（秒）。Noneの場合は無制限

        Returns:
            RFID文字列、またはタイムアウトでNone
        """
        start_time = time.time()
        while True:
            rfid = self.read_rfid()
            if rfid:
                return rfid

            if timeout_sec is not None:
                if time.time() - start_time > timeout_sec:
                    return None

            time.sleep(0.1)  # 100msごとにチェック


# ===== 使用例 =====

if __name__ == "__main__":
    # 通常モード
    print("=== 通常モード ===")
    hw = CompartmentHardware()

    print("入室センサー:", hw.check_entrance())
    print("RFID:", hw.read_rfid())

    # デバッグモード
    print("\n=== デバッグモード ===")
    hw_debug = CompartmentHardware(debug_mode=True)

    # センサーシミュレーション
    print("入室をシミュレート...")
    hw_debug.debug_simulate_entrance()

    # ランダムRFID生成
    rfid = hw_debug.debug_set_random_rfid()
    print(f"ランダムRFID生成: {rfid}")

    # 課題状態取得
    task_status = hw_debug.debug_get_task_status()
    print(f"課題状態: {task_status}")

    # すべてのセンサー状態取得
    sensors = hw_debug.debug_get_all_sensors()
    print(f"センサー状態: {sensors}")
