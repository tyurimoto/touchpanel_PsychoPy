"""
Debug Keyboard Control for Compartment Hardware
キーボードでセンサーをシミュレートするデバッグツール
"""

from psychopy import visual, core, event
from compartment_hardware import CompartmentHardware
import sys


def main():
    # ハードウェアクライアント初期化（デバッグモード）
    hw = CompartmentHardware(debug_mode=True)

    # ウィンドウ作成（情報表示用）
    win = visual.Window(
        size=[800, 600],
        color='black',
        fullscr=False,
        screen=0,
        allowGUI=True
    )

    # テキスト表示
    title_text = visual.TextStim(
        win,
        text="デバッグモード - キーボードコントロール",
        pos=(0, 0.8),
        height=0.08,
        color='white'
    )

    help_text = visual.TextStim(
        win,
        text="""
キー操作:
  E: 入室センサー ON (一時的)
  X: 退室センサー ON (一時的)
  S: 在室センサー トグル
  L: レバースイッチ トグル

  R: ランダムRFID生成
  C: RFID クリア

  O: ドアを開く
  P: ドアを閉じる

  [: レバーを出す
  ]: レバーを引っ込める

  F: 給餌 (1秒)

  T: 課題状態表示
  H: 試行履歴表示

  SPACE: すべてのセンサー状態表示
  BACKSPACE: 状態リセット

  ESC: 終了
        """,
        pos=(0, 0),
        height=0.05,
        color='white',
        alignText='left'
    )

    status_text = visual.TextStim(
        win,
        text="",
        pos=(0, -0.7),
        height=0.04,
        color='yellow'
    )

    # センサー状態（トグル用）
    sensor_states = {
        'stay': False,
        'lever': False
    }

    print("=== デバッグモード キーボードコントロール ===")
    print("Escキーで終了")

    # メインループ
    while True:
        # 画面描画
        title_text.draw()
        help_text.draw()
        status_text.draw()
        win.flip()

        # キーボード入力チェック
        keys = event.getKeys()

        if 'escape' in keys:
            print("終了します")
            break

        # 入室センサー (一時的)
        elif 'e' in keys:
            print("入室センサー ON")
            hw.debug_set_sensor("entrance", True)
            status_text.text = "入室センサー ON"
            core.wait(0.5)
            hw.debug_set_sensor("entrance", False)
            print("入室センサー OFF")

        # 退室センサー (一時的)
        elif 'x' in keys:
            print("退室センサー ON")
            hw.debug_set_sensor("exit", True)
            status_text.text = "退室センサー ON"
            core.wait(0.5)
            hw.debug_set_sensor("exit", False)
            print("退室センサー OFF")

        # 在室センサー (トグル)
        elif 's' in keys:
            sensor_states['stay'] = not sensor_states['stay']
            hw.debug_set_sensor("stay", sensor_states['stay'])
            state_str = "ON" if sensor_states['stay'] else "OFF"
            print(f"在室センサー {state_str}")
            status_text.text = f"在室センサー {state_str}"

        # レバースイッチ (トグル)
        elif 'l' in keys:
            sensor_states['lever'] = not sensor_states['lever']
            hw.debug_set_sensor("lever", sensor_states['lever'])
            state_str = "ON" if sensor_states['lever'] else "OFF"
            print(f"レバースイッチ {state_str}")
            status_text.text = f"レバースイッチ {state_str}"

        # ランダムRFID生成
        elif 'r' in keys:
            rfid = hw.debug_set_random_rfid()
            print(f"RFID生成: {rfid}")
            status_text.text = f"RFID: {rfid}"

        # RFID クリア
        elif 'c' in keys:
            hw.clear_rfid()
            print("RFID クリア")
            status_text.text = "RFID クリア"

        # ドアを開く
        elif 'o' in keys:
            hw.open_door()
            print("ドアを開く")
            status_text.text = "ドアを開く"

        # ドアを閉じる
        elif 'p' in keys:
            hw.close_door()
            print("ドアを閉じる")
            status_text.text = "ドアを閉じる"

        # レバーを出す
        elif 'bracketleft' in keys:
            hw.extend_lever()
            print("レバーを出す")
            status_text.text = "レバーを出す"

        # レバーを引っ込める
        elif 'bracketright' in keys:
            hw.retract_lever()
            print("レバーを引っ込める")
            status_text.text = "レバーを引っ込める"

        # 給餌
        elif 'f' in keys:
            hw.dispense_feed(duration_ms=1000)
            print("給餌 (1秒)")
            status_text.text = "給餌実行"

        # 課題状態表示
        elif 't' in keys:
            task_status = hw.debug_get_task_status()
            print("\n=== 課題状態 ===")
            print(f"  課題タイプ: {task_status.get('taskType', 'Unknown')}")
            print(f"  現在の状態: {task_status.get('currentState', 'Unknown')}")
            print(f"  実行中: {task_status.get('isRunning', False)}")
            print(f"  総試行数: {task_status.get('totalTrials', 0)}")
            print(f"  正解数: {task_status.get('correctCount', 0)}")
            print(f"  不正解数: {task_status.get('incorrectCount', 0)}")
            print(f"  成功率: {task_status.get('successRate', 0)}%")
            print(f"  RFID: {task_status.get('currentIdCode', '')}")

            status_text.text = f"課題: {task_status.get('taskType', 'Unknown')} | 試行: {task_status.get('totalTrials', 0)} | 成功率: {task_status.get('successRate', 0)}%"

        # 試行履歴表示
        elif 'h' in keys:
            history = hw.debug_get_task_history()
            print("\n=== 試行履歴 ===")
            if history:
                for i, trial in enumerate(history, 1):
                    print(f"  {i}. 試行#{trial.get('sessionNum', '?')} - {trial.get('result', '?')} - {trial.get('timestamp', '?')}")
            else:
                print("  履歴なし")

            status_text.text = f"試行履歴: {len(history)}件"

        # すべてのセンサー状態表示
        elif 'space' in keys:
            sensors = hw.debug_get_all_sensors()
            print("\n=== センサー状態 ===")
            for sensor, state in sensors.items():
                state_str = "ON " if state else "OFF"
                print(f"  {sensor}: {state_str}")

            status_text.text = "センサー状態を表示（コンソール確認）"

        # 状態リセット
        elif 'backspace' in keys:
            hw.debug_reset()
            sensor_states['stay'] = False
            sensor_states['lever'] = False
            print("状態リセット")
            status_text.text = "状態リセット"

        # CPU負荷軽減
        core.wait(0.01)

    # 終了処理
    win.close()
    core.quit()


if __name__ == "__main__":
    try:
        main()
    except KeyboardInterrupt:
        print("\n中断されました")
        sys.exit(0)
    except Exception as e:
        print(f"エラー: {e}")
        import traceback
        traceback.print_exc()
        sys.exit(1)
