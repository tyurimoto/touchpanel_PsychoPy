"""
Delayed Matching to Sample (DMTS) Task for PsychoPy Hybrid Engine

遅延見本合わせ課題:
  1. サンプル刺激を提示（見本）
  2. 遅延期間（画面ブランク）
  3. 選択刺激を提示（サンプルと同じ＋異なるもの）
  4. 被験体がサンプルと同じ刺激をタッチすれば正解

C#ハイブリッドエンジン用: 入退室・ドア・給餌はC#が制御。
本スクリプトは各trial毎に TRIAL_RESULT:CORRECT/INCORRECT/TIMEOUT を出力し、
C#からの CONTINUE シグナル（stdin）を待ってから次のtrialへ進む。
全trial完了後に SESSION_END を出力して終了する。
"""

from psychopy import visual, core, event
import random
import sys
import math
import time


class DMTSTask:
    """Delayed Matching to Sample 課題"""

    # 使用する刺激セット（色と形の組み合わせ）
    STIMULI = [
        {"shape": "circle",   "color": "red"},
        {"shape": "circle",   "color": "blue"},
        {"shape": "circle",   "color": "green"},
        {"shape": "circle",   "color": "yellow"},
        {"shape": "square",   "color": "red"},
        {"shape": "square",   "color": "blue"},
        {"shape": "square",   "color": "green"},
        {"shape": "square",   "color": "yellow"},
        {"shape": "triangle", "color": "red"},
        {"shape": "triangle", "color": "blue"},
        {"shape": "triangle", "color": "green"},
        {"shape": "triangle", "color": "yellow"},
    ]

    def __init__(self, rfid="UNKNOWN", debug_mode=False, fullscreen=True, screen=1):
        self.rfid = rfid
        self.debug_mode = debug_mode

        # 課題パラメータ
        self.max_trials = 5
        self.sample_duration = 3.0       # サンプル提示時間（秒）
        self.delay_duration = 2.0        # 遅延時間（秒）
        self.choice_timeout = 10.0       # 選択タイムアウト（秒）
        self.num_choices = 3             # 選択肢の数（サンプル含む）
        self.stimulus_size = 150         # 刺激サイズ（ピクセル）
        self.feedback_duration = 0.3     # フィードバック表示時間（秒）
        self.iti_duration = 0.0          # 試行間インターバル（秒）（都度給餌の待ち時間で代替）

        # 結果カウンター
        self.trial_count = 0
        self.correct_count = 0

        # PsychoPyウィンドウ
        self.win = visual.Window(
            size=[1920, 1080],
            color='black',
            fullscr=fullscreen,
            screen=screen,
            allowGUI=False,
            units='pix'
        )

        self.mouse = event.Mouse(win=self.win, visible=False)
        self.create_stimuli()

    def create_stimuli(self):
        """固定の刺激オブジェクトを作成"""
        self.background = visual.Rect(
            self.win, width=1920, height=1080,
            fillColor='gray', lineColor=None
        )

        self.feedback_text = visual.TextStim(
            self.win, text='', height=80,
            color='white', bold=True
        )

        self.info_text = visual.TextStim(
            self.win, text='', pos=(0, 450),
            height=40, color='white'
        )

        # 遅延期間用の画面
        self.delay_text = visual.TextStim(
            self.win, text='+', height=120,
            color='white', bold=True
        )

    def _make_shape(self, stim_def, pos, size=None):
        """刺激定義から PsychoPy 視覚オブジェクトを生成"""
        if size is None:
            size = self.stimulus_size
        shape_type = stim_def["shape"]
        color = stim_def["color"]

        if shape_type == "circle":
            return visual.Circle(
                self.win, radius=size / 2,
                fillColor=color, lineColor='white', lineWidth=3,
                pos=pos
            )
        elif shape_type == "square":
            return visual.Rect(
                self.win, width=size, height=size,
                fillColor=color, lineColor='white', lineWidth=3,
                pos=pos
            )
        elif shape_type == "triangle":
            # 正三角形の頂点を計算
            r = size / 2
            vertices = []
            for i in range(3):
                angle = math.radians(90 + 120 * i)
                vertices.append((r * math.cos(angle), r * math.sin(angle)))
            return visual.ShapeStim(
                self.win, vertices=vertices,
                fillColor=color, lineColor='white', lineWidth=3,
                pos=pos
            )
        else:
            # フォールバック: 円
            return visual.Circle(
                self.win, radius=size / 2,
                fillColor=color, lineColor='white', lineWidth=3,
                pos=pos
            )

    def _get_choice_positions(self, n):
        """n個の選択肢を横に等間隔で配置する座標を返す"""
        spacing = 400
        total_width = spacing * (n - 1)
        start_x = -total_width / 2
        return [(start_x + spacing * i, 0) for i in range(n)]

    def _check_touch(self, pos, target_pos, radius):
        """タッチ位置がターゲット範囲内か判定"""
        dx = pos[0] - target_pos[0]
        dy = pos[1] - target_pos[1]
        return (dx * dx + dy * dy) <= radius * radius

    def _wait_for_ready_touch(self):
        """Ready画面を表示し、タッチを待つ"""
        t0 = time.perf_counter()
        print(f"[TIMING] ready_draw_start: {t0:.3f}", flush=True)

        # Ready画面描画
        self.background.draw()
        ready_circle = visual.Circle(
            self.win, radius=200,
            fillColor='white', lineColor=None,
            pos=(0, 0)
        )
        ready_text = visual.TextStim(
            self.win, text='Touch to Start',
            height=40, color='black', pos=(0, 0), bold=True
        )
        ready_circle.draw()
        ready_text.draw()
        self.win.flip()
        t0f = time.perf_counter()
        print(f"[TIMING] ready_flip: {t0f:.3f} (draw+flip={t0f-t0:.3f}s)", flush=True)

        # マウス状態リセット（前のtrialの残りタッチを消去）
        self.mouse.clickReset()
        # 既にボタンが押されている場合はリリースを待つ
        while self.mouse.getPressed()[0]:
            core.wait(0.01)

        # タッチ待ち（画面のどこでもOK）
        while True:
            if self.mouse.getPressed()[0]:
                break
            if event.getKeys(['escape']):
                return False  # 中断
            core.wait(0.01)

        # タッチ後、リリースを待ってからtrial開始
        while self.mouse.getPressed()[0]:
            core.wait(0.01)

        t1 = time.perf_counter()
        print(f"[TIMING] ready_touch_done: {t1:.3f} (waited {t1-t0:.3f}s)", flush=True)

        return True  # OK

    def run_trial(self):
        """1試行を実行"""
        # Ready画面
        if not self._wait_for_ready_touch():
            return None  # ESC中断

        self.trial_count += 1
        print(f"\n=== Trial {self.trial_count}/{self.max_trials} ===")

        # サンプル刺激と不正解刺激を選択
        all_stimuli = list(self.STIMULI)
        random.shuffle(all_stimuli)
        sample_def = all_stimuli[0]
        distractor_defs = all_stimuli[1:self.num_choices]

        print(f"Sample: {sample_def['shape']} ({sample_def['color']})")

        # --- Phase 1: サンプル提示 ---
        self.info_text.text = f"Trial {self.trial_count}/{self.max_trials} - Sample"
        sample_shape = self._make_shape(sample_def, (0, 0), self.stimulus_size * 1.2)

        self.background.draw()
        sample_shape.draw()
        self.info_text.draw()
        self.win.flip()
        t2 = time.perf_counter()
        print(f"[TIMING] sample_flip: {t2:.3f}", flush=True)

        # サンプル提示時間
        start = core.getTime()
        while core.getTime() - start < self.sample_duration:
            if event.getKeys(['escape']):
                return None  # 中断
            core.wait(0.01)

        # --- Phase 2: 遅延期間 ---
        self.background.draw()
        self.delay_text.draw()
        self.info_text.text = "..."
        self.info_text.draw()
        self.win.flip()
        t_delay = time.perf_counter()
        print(f"[TIMING] delay_flip: {t_delay:.3f}", flush=True)

        start = core.getTime()
        while core.getTime() - start < self.delay_duration:
            if event.getKeys(['escape']):
                return None
            core.wait(0.01)

        # --- Phase 3: 選択肢提示 ---
        # 正解位置をランダムに決定
        choice_defs = [sample_def] + distractor_defs
        random.shuffle(choice_defs)
        correct_index = choice_defs.index(sample_def)

        positions = self._get_choice_positions(len(choice_defs))
        choice_shapes = []
        for i, (cdef, pos) in enumerate(zip(choice_defs, positions)):
            shape = self._make_shape(cdef, pos)
            choice_shapes.append(shape)

        # 選択肢を描画
        self.info_text.text = f"Trial {self.trial_count}/{self.max_trials} - Choose"
        self.background.draw()
        for s in choice_shapes:
            s.draw()
        self.info_text.draw()
        self.win.flip()
        t3 = time.perf_counter()
        print(f"[TIMING] choice_flip: {t3:.3f}", flush=True)

        # タッチ待ち
        self.mouse.clickReset()
        correct = False
        responded = False
        start = core.getTime()

        while core.getTime() - start < self.choice_timeout:
            if self.mouse.getPressed()[0]:
                mouse_pos = self.mouse.getPos()
                hit_radius = self.stimulus_size / 2 + 80  # タッチ判定を広めに

                for i, pos in enumerate(positions):
                    if self._check_touch(mouse_pos, pos, hit_radius):
                        responded = True
                        if i == correct_index:
                            correct = True
                            print(f"Correct! Touched {choice_defs[i]['shape']} ({choice_defs[i]['color']})")
                        else:
                            print(f"Incorrect. Touched {choice_defs[i]['shape']} ({choice_defs[i]['color']})")
                        break

                if responded:
                    t4 = time.perf_counter()
                    print(f"[TIMING] touch_detected: {t4:.3f} (response_time={t4-t3:.3f}s)", flush=True)
                    break

                # タッチしたがどの刺激にも当たらなかった場合はリリースを待つ
                while self.mouse.getPressed()[0]:
                    core.wait(0.01)

            if event.getKeys(['escape']):
                return None

            core.wait(0.01)

        if not responded:
            print("Timeout - no response")

        # --- Phase 4: フィードバック ---
        if correct:
            self.correct_count += 1
            self.feedback_text.text = "Correct!"
            self.feedback_text.color = 'lime'
        elif responded:
            self.feedback_text.text = "Incorrect"
            self.feedback_text.color = 'red'
        else:
            self.feedback_text.text = "Timeout"
            self.feedback_text.color = 'orange'

        self.background.draw()
        self.feedback_text.draw()
        self.win.flip()
        t_fb = time.perf_counter()
        print(f"[TIMING] feedback_flip: {t_fb:.3f}", flush=True)
        core.wait(self.feedback_duration)

        # trial結果をC#に送信
        t_send = time.perf_counter()
        if correct:
            print("TRIAL_RESULT:CORRECT", flush=True)
        elif responded:
            print("TRIAL_RESULT:INCORRECT", flush=True)
        else:
            print("TRIAL_RESULT:TIMEOUT", flush=True)
        print(f"[TIMING] trial_result_sent: {t_send:.3f}", flush=True)

        # C#の給餌完了シグナルを待つ（ITIの代わり）
        t_wait = time.perf_counter()
        print(f"[TIMING] waiting_for_continue: {t_wait:.3f}", flush=True)
        sys.stdin.readline()  # "CONTINUE\n" を受信するまでブロック
        t_cont = time.perf_counter()
        print(f"[TIMING] continue_received: {t_cont:.3f} (waited {t_cont-t_wait:.3f}s)", flush=True)

        return correct

    def run(self):
        """課題メインループ"""
        try:
            print("\n" + "=" * 50)
            print("Delayed Matching to Sample (DMTS)")
            print(f"RFID: {self.rfid}")
            print(f"Trials: {self.max_trials}")
            print(f"Sample duration: {self.sample_duration}s")
            print(f"Delay duration: {self.delay_duration}s")
            print(f"Choices: {self.num_choices}")
            print("=" * 50)

            for i in range(self.max_trials):
                result = self.run_trial()
                if result is None:
                    # ESCで中断
                    print("Task aborted by user")
                    break

            # 結果サマリー
            rate = (self.correct_count / self.trial_count * 100) if self.trial_count > 0 else 0
            print(f"\n{'=' * 50}")
            print(f"Results: {self.correct_count}/{self.trial_count} correct ({rate:.1f}%)")
            print(f"{'=' * 50}")

            # 結果画面
            self.background.draw()
            self.feedback_text.text = (
                f"Session Complete\n\n"
                f"Correct: {self.correct_count}/{self.trial_count}\n"
                f"Rate: {rate:.1f}%"
            )
            self.feedback_text.color = 'white'
            self.feedback_text.height = 60
            self.feedback_text.draw()
            self.win.flip()
            core.wait(1.0)

            # 全trial完了をC#に通知
            print("SESSION_END", flush=True)

        except KeyboardInterrupt:
            print("\nTask interrupted", flush=True)
            print("SESSION_END", flush=True)

        finally:
            self.win.close()
            core.quit()


def main():
    t_start = time.perf_counter()
    print(f"[TIMING] python_start: {t_start:.3f}", flush=True)

    # RFID: C#エンジンから第1引数で渡される
    rfid = sys.argv[1] if len(sys.argv) >= 2 else "UNKNOWN"

    debug_mode = '--debug' in sys.argv or '-d' in sys.argv
    windowed = '--window' in sys.argv or '-w' in sys.argv

    print(f"RFID: {rfid}")
    print(f"Debug: {debug_mode}, Windowed: {windowed}")

    task = DMTSTask(
        rfid=rfid,
        debug_mode=debug_mode,
        fullscreen=not windowed,
        screen=1 if not windowed else 0
    )

    t_init = time.perf_counter()
    print(f"[TIMING] init_complete: {t_init:.3f} (init_time={t_init-t_start:.3f}s)", flush=True)

    task.run()


if __name__ == "__main__":
    main()
