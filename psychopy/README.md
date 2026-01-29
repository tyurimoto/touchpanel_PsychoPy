# Compartment PsychoPy Tasks

PsychoPyからCompartmentハードウェアを制御するための課題スクリプト集

## 必要な環境

- Python 3.8以上
- PsychoPy 2023.x以上
- requests ライブラリ

```bash
pip install psychopy requests
```

## ファイル構成

```
psychopy/
├── compartment_hardware.py         # ハードウェアAPIクライアントライブラリ
├── debug_keyboard_control.py       # キーボードデバッグツール
├── training_task_example.py        # 訓練課題サンプル
└── README.md                        # このファイル
```

## 使い方

### 1. C#側のAPI設定

1. Compartmentソフトを起動
2. Preferences → "Enable Debug Mode" にチェック
3. "Debug Mode Type" を選択:
   - **Full Dummy**: ハードウェアなしで完全動作（PC単体テスト用）
   - **Hybrid**: 実機＋手動シミュレート（推奨）
4. API Server Port を確認（デフォルト: 5000）
5. 設定を保存

### 2. デバッグツールでハードウェアテスト

キーボードでセンサーをシミュレートできるツール：

```bash
python debug_keyboard_control.py
```

**キー操作:**
- `E`: 入室センサー ON (一時的)
- `X`: 退室センサー ON (一時的)
- `S`: 在室センサー トグル
- `L`: レバースイッチ トグル
- `R`: ランダムRFID生成
- `C`: RFID クリア
- `O`: ドアを開く
- `P`: ドアを閉じる
- `[`: レバーを出す
- `]`: レバーを引っ込める
- `F`: 給餌 (1秒)
- `T`: 課題状態表示
- `H`: 試行履歴表示
- `SPACE`: すべてのセンサー状態表示
- `BACKSPACE`: 状態リセット
- `ESC`: 終了

### 3. 訓練課題の実行

#### デバッグモードで実行（ハードウェアなし）

```bash
python training_task_example.py --debug --window
```

**操作方法:**
- `E`キー: 入室をシミュレート
- `R`キー: ランダムRFIDを生成
- 画面をクリック: ターゲットにタッチ
- `X`キー: 退室をシミュレート
- `ESC`キー: 中断

#### 実機モードで実行

```bash
python training_task_example.py
```

- セカンダリディスプレイにフルスクリーン表示
- 実際のセンサー、RFID、タッチパネルを使用

## API クライアントライブラリの使い方

### 基本的な使い方

```python
from compartment_hardware import CompartmentHardware

# 通常モード（実機）
hw = CompartmentHardware(base_url="http://localhost:5000/api")

# デバッグモード（シミュレーション）
hw = CompartmentHardware(
    base_url="http://localhost:5000/api",
    debug_mode=True
)
```

### センサー読み取り

```python
# 入室センサー
if hw.check_entrance():
    print("入室検出！")

# 退室センサー
if hw.check_exit():
    print("退室検出！")

# 在室センサー
if hw.check_stay():
    print("在室中")

# レバースイッチ
if hw.check_lever_switch():
    print("レバー押下！")
```

### RFID読み取り

```python
# RFID読み取り
rfid = hw.read_rfid()
if rfid:
    print(f"RFID: {rfid}")

# RFID待機（タイムアウト付き）
rfid = hw.wait_for_rfid(timeout_sec=10.0)
if rfid:
    print(f"RFID読み取り成功: {rfid}")
else:
    print("タイムアウト")

# RFIDクリア
hw.clear_rfid()
```

### ドア制御

```python
# ドアを開く
hw.open_door()

# ドアを閉じる
hw.close_door()

# ドア状態取得
status = hw.get_door_status()
print(f"ドア状態: {status['state']}")
```

### レバー制御

```python
# レバーを出す
hw.extend_lever()

# レバーを引っ込める
hw.retract_lever()
```

### 給餌

```python
# 1秒間給餌
hw.dispense_feed(duration_ms=1000)

# 給餌中かチェック
if hw.is_feeding():
    print("給餌中")
```

### デバッグモード専用機能

```python
# デバッグモードで初期化
hw = CompartmentHardware(debug_mode=True)

# センサーをシミュレート
hw.debug_set_sensor("entrance", True)   # 入室センサーON
hw.debug_set_sensor("exit", False)      # 退室センサーOFF

# 入室をシミュレート（500ms後に自動OFF）
hw.debug_simulate_entrance()

# ランダムRFID生成
rfid = hw.debug_set_random_rfid()
print(f"生成されたRFID: {rfid}")

# 特定のRFIDを設定
hw.debug_set_rfid("1234567890123456")

# すべてのセンサー状態を取得
sensors = hw.debug_get_all_sensors()
print(f"センサー状態: {sensors}")

# 課題状態を取得
task_status = hw.debug_get_task_status()
print(f"課題タイプ: {task_status['taskType']}")
print(f"試行数: {task_status['totalTrials']}")
print(f"成功率: {task_status['successRate']}%")

# 試行履歴を取得
history = hw.debug_get_task_history()
for trial in history:
    print(f"試行#{trial['sessionNum']}: {trial['result']}")

# 状態リセット
hw.debug_reset()
```

## 課題の作成方法

### 基本的な課題フロー

```python
from psychopy import visual, core, event
from compartment_hardware import CompartmentHardware

class MyTask:
    def __init__(self, debug_mode=False):
        self.hw = CompartmentHardware(debug_mode=debug_mode)
        self.win = visual.Window(
            size=[1920, 1080],
            fullscr=True,
            screen=1  # セカンダリディスプレイ
        )

    def run(self):
        # 1. 入室待ち
        while not self.hw.check_entrance():
            core.wait(0.05)

        # 2. RFID読み取り
        rfid = self.hw.wait_for_rfid(timeout_sec=10.0)
        if not rfid:
            return

        # 3. 課題実行
        for trial in range(20):
            correct = self.run_trial()
            if correct:
                self.hw.dispense_feed(duration_ms=1000)

        # 4. 退室待ち
        self.hw.open_door()
        while not self.hw.check_exit():
            core.wait(0.05)
        self.hw.close_door()

    def run_trial(self):
        # 課題ロジックをここに実装
        # PsychoPyの刺激提示、タッチ検出など
        pass
```

### デバッグモードでのテスト

デバッグモードでは、キーボードでセンサーをシミュレートできます：

```python
# 入室待ち（デバッグモード対応）
while True:
    if self.hw.check_entrance():
        break

    # デバッグモード: Eキーで入室
    if debug_mode and 'e' in event.getKeys():
        self.hw.debug_simulate_entrance()
        break

    core.wait(0.05)
```

## トラブルシューティング

### APIサーバーに接続できない

1. C# Compartmentソフトが起動しているか確認
2. ポート番号が正しいか確認（デフォルト: 5000）
3. ファイアウォールの設定を確認

```python
# 接続テスト
hw = CompartmentHardware()
try:
    status = hw.debug_get_all_sensors()
    print("接続成功:", status)
except Exception as e:
    print("接続失敗:", e)
```

### デバッグモードが動作しない

1. C#側でデバッグモードが有効になっているか確認
2. Python側でも `debug_mode=True` を指定

```python
hw = CompartmentHardware(debug_mode=True)

# デバッグモード確認
sensors = hw.debug_get_all_sensors()
if sensors:
    print("デバッグモード動作中")
else:
    print("デバッグモードが無効")
```

### センサーが反応しない

デバッグモードでセンサー状態を確認：

```python
hw = CompartmentHardware(debug_mode=True)

# すべてのセンサー状態を表示
sensors = hw.debug_get_all_sensors()
for sensor, state in sensors.items():
    print(f"{sensor}: {'ON' if state else 'OFF'}")
```

## 参考資料

- [PsychoPy公式ドキュメント](https://www.psychopy.org/)
- [Requests ドキュメント](https://requests.readthedocs.io/)
- 実装計画: `/Users/terumi/Downloads/compartment/IMPLEMENTATION_PLAN.md`

## ライセンス

このプロジェクトはMITライセンスの下で公開されています。
