"""
Training Task Example for PsychoPy
PsychoPyで動作する訓練課題のサンプル
"""

from psychopy import visual, core, event
from compartment_hardware import CompartmentHardware
import random
import sys


class TrainingTask:
    def __init__(self, debug_mode=False, fullscreen=True, screen=1):
        """
        訓練課題の初期化

        Args:
            debug_mode: デバッグモード（ハードウェアなしで動作）
            fullscreen: フルスクリーン表示
            screen: 表示するスクリーン番号（0=メイン、1=セカンダリ）
        """
        self.debug_mode = debug_mode
        self.hw = CompartmentHardware(debug_mode=debug_mode)

        # PsychoPyウィンドウ作成（セカンダリディスプレイに表示）
        self.win = visual.Window(
            size=[1920, 1080],
            color='black',
            fullscr=fullscreen,
            screen=screen,
            allowGUI=False,
            units='pix'
        )

        # マウス/タッチ入力
        self.mouse = event.Mouse(win=self.win, visible=False)

        # 刺激作成
        self.create_stimuli()

        # 課題パラメータ
        self.trial_count = 0
        self.correct_count = 0
        self.max_trials = 20
        self.current_rfid = None

    def create_stimuli(self):
        """刺激を作成"""
        # 背景
        self.background = visual.Rect(
            self.win,
            width=1920,
            height=1080,
            fillColor='gray',
            lineColor=None
        )

        # ターゲット刺激（円）
        self.target = visual.Circle(
            self.win,
            radius=100,
            fillColor='blue',
            lineColor='white',
            lineWidth=3
        )

        # フィードバックテキスト
        self.feedback_text = visual.TextStim(
            self.win,
            text='',
            height=80,
            color='white',
            bold=True
        )

        # 情報表示テキスト（画面上部）
        self.info_text = visual.TextStim(
            self.win,
            text='',
            pos=(0, 450),
            height=40,
            color='white'
        )

    def wait_for_entrance(self):
        """入室を待つ"""
        print("入室待ち...")
        self.info_text.text = "入室待ち..."
        self.info_text.draw()
        self.win.flip()

        if self.debug_mode:
            print("[デバッグ] Eキーで入室をシミュレート")

        while True:
            # 入室センサーチェック
            if self.hw.check_entrance():
                print("入室検出！")
                return True

            # デバッグモード: Eキーで入室シミュレート
            if self.debug_mode:
                keys = event.getKeys(['e', 'escape'])
                if 'e' in keys:
                    self.hw.debug_simulate_entrance()
                    print("[デバッグ] 入室シミュレート")
                    return True
                if 'escape' in keys:
                    return False

            core.wait(0.05)

    def wait_for_rfid(self):
        """RFID読み取りを待つ"""
        print("RFID読み取り待ち...")
        self.info_text.text = "RFID読み取り中..."
        self.info_text.draw()
        self.win.flip()

        # RFIDクリア
        self.hw.clear_rfid()

        if self.debug_mode:
            print("[デバッグ] Rキーでランダム RFID生成")

        timeout = 10.0  # 10秒タイムアウト
        start_time = core.getTime()

        while True:
            # RFID読み取り
            rfid = self.hw.read_rfid()
            if rfid:
                self.current_rfid = rfid
                print(f"RFID読み取り: {rfid}")
                return True

            # デバッグモード: Rキーでランダム RFID生成
            if self.debug_mode:
                keys = event.getKeys(['r', 'escape'])
                if 'r' in keys:
                    rfid = self.hw.debug_set_random_rfid()
                    self.current_rfid = rfid
                    print(f"[デバッグ] RFID生成: {rfid}")
                    return True
                if 'escape' in keys:
                    return False

            # タイムアウトチェック
            if core.getTime() - start_time > timeout:
                print("RFID読み取りタイムアウト")
                return False

            core.wait(0.1)

    def run_trial(self):
        """1試行を実行"""
        self.trial_count += 1
        print(f"\n=== 試行 {self.trial_count}/{self.max_trials} ===")

        # ターゲット位置をランダムに設定
        x = random.randint(-400, 400)
        y = random.randint(-300, 300)
        self.target.pos = (x, y)

        # 試行開始表示
        self.info_text.text = f"試行 {self.trial_count}/{self.max_trials}"
        self.background.draw()
        self.info_text.draw()
        self.win.flip()
        core.wait(0.5)

        # ターゲット表示
        self.background.draw()
        self.target.draw()
        self.info_text.draw()
        self.win.flip()

        # タッチ待ち
        self.mouse.clickReset()
        correct = False
        timeout = 10.0
        start_time = core.getTime()

        print(f"ターゲット位置: ({x}, {y})")

        while True:
            # タッチ検出
            if self.mouse.getPressed()[0]:
                mouse_pos = self.mouse.getPos()
                print(f"タッチ位置: {mouse_pos}")

                # ターゲットとの距離計算
                distance = ((mouse_pos[0] - x)**2 + (mouse_pos[1] - y)**2)**0.5

                if distance < 100:  # ターゲットの半径内
                    correct = True
                    print("正解！")
                    break
                else:
                    print("不正解（ターゲット外）")
                    break

            # タイムアウトチェック
            if core.getTime() - start_time > timeout:
                print("タイムアウト")
                break

            # ESCキーで中断
            if event.getKeys(['escape']):
                return False

            core.wait(0.01)

        # フィードバック表示
        if correct:
            self.correct_count += 1
            self.feedback_text.text = "正解！"
            self.feedback_text.color = 'green'

            # 給餌
            print("給餌実行")
            self.hw.dispense_feed(duration_ms=1000)
        else:
            self.feedback_text.text = "不正解"
            self.feedback_text.color = 'red'

        self.background.draw()
        self.feedback_text.draw()
        self.info_text.draw()
        self.win.flip()
        core.wait(1.0)

        return True

    def wait_for_exit(self):
        """退室を待つ"""
        print("退室待ち...")
        self.info_text.text = "退室待ち..."
        self.info_text.draw()
        self.win.flip()

        # ドアを開く
        self.hw.open_door()
        core.wait(0.5)

        if self.debug_mode:
            print("[デバッグ] Xキーで退室をシミュレート")

        timeout = 30.0
        start_time = core.getTime()

        while True:
            # 退室センサーチェック
            if self.hw.check_exit():
                print("退室検出！")
                # ドアを閉じる
                self.hw.close_door()
                return True

            # デバッグモード: Xキーで退室シミュレート
            if self.debug_mode:
                keys = event.getKeys(['x', 'escape'])
                if 'x' in keys:
                    self.hw.debug_simulate_exit()
                    print("[デバッグ] 退室シミュレート")
                    self.hw.close_door()
                    return True
                if 'escape' in keys:
                    self.hw.close_door()
                    return False

            # タイムアウトチェック
            if core.getTime() - start_time > timeout:
                print("退室タイムアウト")
                self.hw.close_door()
                return False

            core.wait(0.05)

    def show_results(self):
        """結果表示"""
        success_rate = (self.correct_count / self.trial_count * 100) if self.trial_count > 0 else 0

        results_text = f"""
セッション終了

総試行数: {self.trial_count}
正解数: {self.correct_count}
不正解数: {self.trial_count - self.correct_count}
成功率: {success_rate:.1f}%

RFID: {self.current_rfid}
        """

        self.background.draw()
        self.feedback_text.text = results_text
        self.feedback_text.color = 'white'
        self.feedback_text.height = 50
        self.feedback_text.draw()
        self.win.flip()

        print("\n" + "="*50)
        print(results_text)
        print("="*50)

        core.wait(3.0)

    def run(self):
        """課題を実行"""
        try:
            print("\n" + "="*50)
            print("訓練課題開始")
            if self.debug_mode:
                print("デバッグモード: Eキー=入室, Rキー=RFID, Xキー=退室")
            print("="*50)

            # 入室待ち
            if not self.wait_for_entrance():
                return

            core.wait(0.5)

            # RFID読み取り
            if not self.wait_for_rfid():
                return

            core.wait(0.5)

            # 試行ループ
            for i in range(self.max_trials):
                if not self.run_trial():
                    break

            # 結果表示
            self.show_results()

            # 退室待ち
            self.wait_for_exit()

            print("\n課題終了")

        except KeyboardInterrupt:
            print("\n課題が中断されました")

        finally:
            # クリーンアップ
            self.win.close()
            core.quit()


def main():
    # 起動引数でデバッグモードを指定可能
    debug_mode = '--debug' in sys.argv or '-d' in sys.argv
    windowed = '--window' in sys.argv or '-w' in sys.argv

    print(f"デバッグモード: {debug_mode}")
    print(f"ウィンドウモード: {windowed}")

    task = TrainingTask(
        debug_mode=debug_mode,
        fullscreen=not windowed,
        screen=1 if not windowed else 0
    )
    task.run()


if __name__ == "__main__":
    main()
