"""
Compartment Hardware API 簡易テストスクリプト
PsychoPyなしで動作する最小限のテスト

C#アプリケーションからStartボタンで起動され、
compartment_hardware.pyを使ってAPI接続確認と基本操作テストを行う
"""

import sys
import os
import time

# 同じディレクトリのcompartment_hardware.pyをインポート
sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))
from compartment_hardware import CompartmentHardware


def test_connection(hw):
    """API接続テスト"""
    print("[テスト1] API接続確認...")
    try:
        result = hw.check_entrance()
        print(f"  入室センサー読み取り: {result} -> OK")
        return True
    except Exception as e:
        print(f"  接続失敗: {e}")
        return False


def test_sensors(hw):
    """センサー読み取りテスト"""
    print("[テスト2] センサー読み取り...")
    try:
        entrance = hw.check_entrance()
        print(f"  入室センサー: {entrance}")

        exit_sensor = hw.check_exit()
        print(f"  退室センサー: {exit_sensor}")

        stay = hw.check_stay()
        print(f"  在室センサー: {stay}")

        lever = hw.check_lever_switch()
        print(f"  レバースイッチ: {lever}")

        print("  -> OK")
        return True
    except Exception as e:
        print(f"  センサー読み取り失敗: {e}")
        return False


def test_rfid(hw):
    """RFID読み取りテスト"""
    print("[テスト3] RFID読み取り...")
    try:
        rfid = hw.read_rfid()
        print(f"  RFID値: {rfid}")
        print("  -> OK")
        return True
    except Exception as e:
        print(f"  RFID読み取り失敗: {e}")
        return False


def test_door(hw):
    """ドア状態確認テスト"""
    print("[テスト4] ドア状態確認...")
    try:
        status = hw.get_door_status()
        print(f"  ドア状態: {status}")

        print("  ドアを開く...")
        hw.open_door()
        time.sleep(0.5)

        print("  ドアを閉じる...")
        hw.close_door()
        time.sleep(0.5)

        print("  -> OK")
        return True
    except Exception as e:
        print(f"  ドア操作失敗: {e}")
        return False


def test_feed(hw):
    """給餌テスト"""
    print("[テスト5] 給餌テスト...")
    try:
        feeding = hw.is_feeding()
        print(f"  給餌状態: {feeding}")

        print("  給餌実行 (500ms)...")
        result = hw.dispense_feed(duration_ms=500)
        print(f"  給餌結果: {result}")

        print("  -> OK")
        return True
    except Exception as e:
        print(f"  給餌失敗: {e}")
        return False


def main():
    print("=" * 50)
    print("Compartment Hardware API 簡易テスト")
    print("=" * 50)
    print()

    hw = CompartmentHardware(base_url="http://localhost:5000/api")

    tests = [
        ("API接続", test_connection),
        ("センサー読み取り", test_sensors),
        ("RFID読み取り", test_rfid),
        ("ドア操作", test_door),
        ("給餌", test_feed),
    ]

    passed = 0
    failed = 0

    for name, test_func in tests:
        try:
            if test_func(hw):
                passed += 1
            else:
                failed += 1
        except Exception as e:
            print(f"  予期しないエラー: {e}")
            failed += 1
        print()

    print("=" * 50)
    print(f"結果: {passed} 成功 / {failed} 失敗 (全{passed + failed}テスト)")
    print("=" * 50)

    # テスト完了後、少し待機してからスクリプト終了
    print("\nテスト完了。5秒後に終了します...")
    time.sleep(5)


if __name__ == "__main__":
    main()
