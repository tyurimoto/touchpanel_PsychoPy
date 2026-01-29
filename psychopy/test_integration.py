"""
Compartment Hardware API Integration Test
統合テストスクリプト - デバッグモードで完全な課題フローをテスト
"""

import sys
import time
from compartment_hardware import CompartmentHardware


class Colors:
    """ターミナル色"""
    GREEN = '\033[92m'
    RED = '\033[91m'
    YELLOW = '\033[93m'
    BLUE = '\033[94m'
    RESET = '\033[0m'
    BOLD = '\033[1m'


def print_test(name):
    """テスト名を表示"""
    print(f"\n{Colors.YELLOW}▶ Testing:{Colors.RESET} {name}")


def print_success(message):
    """成功メッセージ"""
    print(f"{Colors.GREEN}✓{Colors.RESET} {message}")


def print_error(message):
    """エラーメッセージ"""
    print(f"{Colors.RED}✗{Colors.RESET} {message}")


def print_info(message):
    """情報メッセージ"""
    print(f"{Colors.BLUE}ℹ{Colors.RESET} {message}")


def test_connection(hw):
    """接続テスト"""
    print_test("API接続確認")
    try:
        sensors = hw.debug_get_all_sensors()
        if sensors:
            print_success(f"接続成功: {len(sensors)}個のセンサー検出")
            return True
        else:
            print_error("接続失敗: センサー情報が取得できません")
            return False
    except Exception as e:
        print_error(f"接続エラー: {e}")
        return False


def test_sensors(hw):
    """センサーテスト"""
    print_test("センサー読み取り")

    # 入室センサー
    hw.debug_set_sensor("entrance", True)
    if hw.check_entrance():
        print_success("入室センサー ON 確認")
    else:
        print_error("入室センサー ON 失敗")

    hw.debug_set_sensor("entrance", False)
    if not hw.check_entrance():
        print_success("入室センサー OFF 確認")
    else:
        print_error("入室センサー OFF 失敗")

    # 退室センサー
    hw.debug_set_sensor("exit", True)
    if hw.check_exit():
        print_success("退室センサー ON 確認")
    else:
        print_error("退室センサー ON 失敗")

    hw.debug_set_sensor("exit", False)
    if not hw.check_exit():
        print_success("退室センサー OFF 確認")
    else:
        print_error("退室センサー OFF 失敗")


def test_rfid(hw):
    """RFIDテスト"""
    print_test("RFID機能")

    # RFIDクリア
    hw.clear_rfid()
    rfid = hw.read_rfid()
    if not rfid:
        print_success("RFIDクリア確認")
    else:
        print_error(f"RFIDクリア失敗: {rfid}")

    # RFID設定
    test_rfid = "1234567890123456"
    hw.debug_set_rfid(test_rfid)
    time.sleep(0.1)
    rfid = hw.read_rfid()
    if rfid == test_rfid:
        print_success(f"RFID設定確認: {rfid}")
    else:
        print_error(f"RFID設定失敗: 期待={test_rfid}, 実際={rfid}")

    # ランダムRFID生成
    random_rfid = hw.debug_set_random_rfid()
    if random_rfid and len(random_rfid) == 16:
        print_success(f"ランダムRFID生成: {random_rfid}")
    else:
        print_error(f"ランダムRFID生成失敗: {random_rfid}")


def test_devices(hw):
    """デバイス制御テスト"""
    print_test("デバイス制御")

    # ドア
    if hw.open_door():
        print_success("ドア開く")
    else:
        print_error("ドア開く失敗")

    time.sleep(0.5)

    if hw.close_door():
        print_success("ドア閉じる")
    else:
        print_error("ドア閉じる失敗")

    # レバー
    if hw.extend_lever():
        print_success("レバー出す")
    else:
        print_error("レバー出す失敗")

    time.sleep(0.3)

    if hw.retract_lever():
        print_success("レバー引っ込める")
    else:
        print_error("レバー引っ込める失敗")

    # 給餌
    if hw.dispense_feed(duration_ms=500):
        print_success("給餌実行")
    else:
        print_error("給餌失敗")


def test_task_status(hw):
    """課題状態取得テスト"""
    print_test("課題状態取得")

    status = hw.debug_get_task_status()
    if status:
        print_success("課題状態取得成功")
        print_info(f"  課題タイプ: {status.get('taskType', 'Unknown')}")
        print_info(f"  試行数: {status.get('totalTrials', 0)}")
        print_info(f"  成功率: {status.get('successRate', 0)}%")
    else:
        print_error("課題状態取得失敗")

    history = hw.debug_get_task_history()
    if history is not None:
        print_success(f"試行履歴取得成功: {len(history)}件")
    else:
        print_error("試行履歴取得失敗")


def test_complete_flow(hw):
    """完全な課題フローテスト"""
    print_test("完全フローシミュレーション")

    # 1. 初期化
    hw.debug_reset()
    print_info("1. 状態リセット")

    # 2. 入室シミュレート
    hw.debug_simulate_entrance()
    print_info("2. 入室シミュレート")
    time.sleep(0.5)

    # 3. RFID読み取り
    rfid = hw.debug_set_random_rfid()
    print_info(f"3. RFID読み取り: {rfid}")
    time.sleep(0.5)

    # 4. 課題実行（3試行）
    print_info("4. 課題実行開始")
    for trial in range(3):
        print_info(f"   試行 {trial + 1}/3")
        time.sleep(0.3)

        # 正解のシミュレート
        correct = (trial % 2 == 0)  # 交互に正解・不正解
        if correct:
            hw.dispense_feed(duration_ms=500)
            print_info(f"   → 正解 - 給餌")
        else:
            print_info(f"   → 不正解")

    # 5. 退室シミュレート
    hw.open_door()
    print_info("5. ドア開放")
    time.sleep(0.5)

    hw.debug_simulate_exit()
    print_info("6. 退室シミュレート")
    time.sleep(0.5)

    hw.close_door()
    print_info("7. ドア閉鎖")

    print_success("完全フロー完了")


def main():
    print(f"\n{Colors.BOLD}{'='*60}{Colors.RESET}")
    print(f"{Colors.BOLD}  Compartment Hardware API Integration Test{Colors.RESET}")
    print(f"{Colors.BOLD}{'='*60}{Colors.RESET}\n")

    # ハードウェアクライアント初期化（デバッグモード）
    hw = CompartmentHardware(
        base_url="http://localhost:5000/api",
        debug_mode=True,
        timeout=5
    )

    # テスト実行
    tests = [
        ("接続", lambda: test_connection(hw)),
        ("センサー", lambda: test_sensors(hw)),
        ("RFID", lambda: test_rfid(hw)),
        ("デバイス制御", lambda: test_devices(hw)),
        ("課題状態", lambda: test_task_status(hw)),
        ("完全フロー", lambda: test_complete_flow(hw)),
    ]

    passed = 0
    failed = 0

    for test_name, test_func in tests:
        try:
            test_func()
            passed += 1
        except Exception as e:
            print_error(f"{test_name}テスト中にエラー: {e}")
            failed += 1
            import traceback
            traceback.print_exc()

    # 結果サマリー
    print(f"\n{Colors.BOLD}{'='*60}{Colors.RESET}")
    print(f"{Colors.BOLD}  Test Summary{Colors.RESET}")
    print(f"{Colors.BOLD}{'='*60}{Colors.RESET}")
    print(f"{Colors.GREEN}Passed:{Colors.RESET} {passed}/{len(tests)}")
    if failed > 0:
        print(f"{Colors.RED}Failed:{Colors.RESET} {failed}/{len(tests)}")
    print()

    return 0 if failed == 0 else 1


if __name__ == "__main__":
    try:
        sys.exit(main())
    except KeyboardInterrupt:
        print("\n\nテスト中断")
        sys.exit(1)
    except Exception as e:
        print(f"\n{Colors.RED}予期しないエラー:{Colors.RESET} {e}")
        import traceback
        traceback.print_exc()
        sys.exit(1)
