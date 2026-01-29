# Compartment PsychoPy統合 操作方法マニュアル

## 目次

1. [C#側の設定と起動](#c側の設定と起動)
2. [デバッグモードの使い方](#デバッグモードの使い方)
3. [PsychoPy課題の実行](#psychopy課題の実行)
4. [APIの使い方](#apiの使い方)
5. [トラブルシューティング](#トラブルシューティング)

---

## C#側の設定と起動

### 初回セットアップ

#### 1. プロジェクトのビルド

```bash
# Visual Studioでビルド
1. Compartment.slnを開く
2. ビルド → ソリューションのビルド
3. エラーがないことを確認
```

**必要なNuGetパッケージ（自動復元）:**
- Microsoft.AspNet.WebApi.OwinSelfHost (5.2.9)
- Microsoft.Owin.Host.HttpListener (4.2.2)
- Microsoft.Owin.Hosting (4.2.2)
- Microsoft.Owin.Cors (4.2.2)
- Newtonsoft.Json (13.x)

#### 2. 設定ファイルの確認

Preferences（環境設定）ファイル: `Preferences.xml`

**重要な設定項目:**
```xml
<CompartmentNo>1</CompartmentNo>           <!-- 部屋番号 -->
<ApiServerPort>5000</ApiServerPort>        <!-- APIポート番号 -->
<EnableDebugMode>false</EnableDebugMode>   <!-- デバッグモード -->
<DebugModeType>FullDummy</DebugModeType>   <!-- FullDummy or Hybrid -->
```

### 通常モード（実機使用）

#### 起動手順

1. **ハードウェア接続確認**
   - IOボード（マイクロチップ）がUSB接続されている
   - RFIDリーダーがCOMポート接続されている

2. **Compartmentソフト起動**
   ```
   Compartment.exe を実行
   ```

3. **API自動起動確認**
   - FormMain起動時に自動的にOWINサーバー起動
   - デバッグ出力に `[API] Web API server started at http://localhost:5000/` 表示

4. **動作確認**
   ```bash
   # ブラウザまたはcurlで確認
   curl http://localhost:5000/api/sensor/entrance
   ```

   **期待される応答:**
   ```json
   {
     "roomId": "1",
     "active": false,
     "timestamp": "2026-01-29T12:34:56Z"
   }
   ```

### デバッグモード（ハードウェア不要）

#### 設定方法

1. **Preferences画面を開く**
   - ※現在はXMLを直接編集（GUI未実装）

2. **Preferences.xml を編集**
   ```xml
   <EnableDebugMode>true</EnableDebugMode>
   <DebugModeType>FullDummy</DebugModeType>
   ```

   **DebugModeType の選択:**
   - `FullDummy`: 完全ダミー（ハードウェア一切不要）
   - `Hybrid`: ハイブリッド（実機＋手動シミュレート）

3. **Compartmentソフト起動**
   - IOボード・RFIDリーダーなしで起動可能
   - デバッグ出力に `[Debug] Initialized IoMicrochipDummyEx (Full Dummy Mode)` 表示

4. **動作確認**
   ```bash
   # デバッグモード確認
   curl http://localhost:5000/api/debug/status

   # センサー状態取得
   curl http://localhost:5000/api/debug/sensors/all
   ```

### API設定のカスタマイズ

#### ポート番号の変更

複数のCompartmentを同一PC/ネットワークで動かす場合:

```xml
<!-- 部屋1 -->
<CompartmentNo>1</CompartmentNo>
<ApiServerPort>5001</ApiServerPort>

<!-- 部屋2 -->
<CompartmentNo>2</CompartmentNo>
<ApiServerPort>5002</ApiServerPort>
```

Python側:
```python
# 部屋1に接続
hw1 = CompartmentHardware(base_url="http://localhost:5001/api")

# 部屋2に接続
hw2 = CompartmentHardware(base_url="http://localhost:5002/api")
```

---

## デバッグモードの使い方

### キーボードデバッグツール

#### 起動方法

```bash
cd /Users/terumi/Downloads/compartment/psychopy
python debug_keyboard_control.py
```

#### キー操作

**センサーシミュレート:**
- `E`: 入室センサー ON（500ms後に自動OFF）
- `X`: 退室センサー ON（500ms後に自動OFF）
- `S`: 在室センサー トグル（ON/OFF切替）
- `L`: レバースイッチ トグル

**RFID操作:**
- `R`: ランダムRFID生成
- `C`: RFIDクリア

**デバイス制御:**
- `O`: ドアを開く
- `P`: ドアを閉じる
- `[`: レバーを出す
- `]`: レバーを引っ込める
- `F`: 給餌（1秒）

**情報表示:**
- `T`: 課題状態表示（コンソール）
- `H`: 試行履歴表示（コンソール）
- `SPACE`: すべてのセンサー状態表示（コンソール）

**その他:**
- `BACKSPACE`: 状態リセット
- `ESC`: 終了

#### 使用例: 課題フローをシミュレート

```
1. Eキー → 入室シミュレート
2. Rキー → ランダムRFID生成
3. Fキー → 給餌（課題成功時）
4. Xキー → 退室シミュレート
```

### API経由での直接制御（curl）

#### センサー状態を設定

```bash
# 入室センサーON
curl -X POST http://localhost:5000/api/debug/sensor/set \
  -H "Content-Type: application/json" \
  -d '{"sensor":"entrance","state":true}'

# 入室センサーOFF
curl -X POST http://localhost:5000/api/debug/sensor/set \
  -H "Content-Type: application/json" \
  -d '{"sensor":"entrance","state":false}'
```

**利用可能なセンサー名:**
- `entrance` - 入室センサー
- `exit` - 退室センサー
- `stay` - 在室センサー
- `lever` - レバースイッチ
- `dooropen` - ドア開センサー
- `doorclose` - ドア閉センサー
- `leverin` - レバー引込センサー
- `leverout` - レバー出張センサー

#### RFID設定

```bash
# 特定のRFIDを設定
curl -X POST http://localhost:5000/api/debug/rfid/set \
  -H "Content-Type: application/json" \
  -d '{"id":"1234567890123456"}'

# ランダムRFID生成
curl -X POST http://localhost:5000/api/debug/rfid/random

# RFIDクリア
curl -X DELETE http://localhost:5000/api/rfid
```

#### 課題状態の確認

```bash
# 課題状態取得
curl http://localhost:5000/api/debug/task/status

# 出力例:
{
  "roomId": "1",
  "task": {
    "taskType": "Training",
    "currentState": "WaitingForEnterCage",
    "isRunning": true,
    "totalTrials": 15,
    "correctCount": 12,
    "incorrectCount": 3,
    "successRate": 80.0,
    "currentIdCode": "3920145000567278",
    "compartmentNo": 1
  },
  "timestamp": "2026-01-29T12:34:56Z"
}

# 試行履歴取得
curl http://localhost:5000/api/debug/task/history

# 出力例:
{
  "roomId": "1",
  "history": {
    "trials": [
      {
        "sessionNum": 15,
        "result": "Correct",
        "timestamp": "2026-01-29 12:30:00",
        "idCode": "3920145000567278"
      },
      ...
    ],
    "count": 10
  },
  "timestamp": "2026-01-29T12:34:56Z"
}
```

#### 状態リセット

```bash
# すべてのデバッグ状態をリセット
curl -X POST http://localhost:5000/api/debug/reset
```

---

## PsychoPy課題の実行

### 環境セットアップ

#### Pythonライブラリのインストール

```bash
pip install psychopy requests
```

#### ファイル配置確認

```
compartment/psychopy/
├── compartment_hardware.py          # APIクライアント
├── debug_keyboard_control.py        # デバッグツール
├── training_task_example.py         # 訓練課題サンプル
└── README.md
```

### 訓練課題の実行

#### デバッグモード（ハードウェアなし）

```bash
cd /Users/terumi/Downloads/compartment/psychopy

# ウィンドウモード・デバッグモードで起動
python training_task_example.py --debug --window
```

**操作方法:**
1. `E`キー: 入室をシミュレート
2. `R`キー: ランダムRFIDを生成
3. マウスクリック: ターゲット（青い円）をクリック
4. 正解すると「正解！」表示 + 給餌実行
5. 20試行完了後、`X`キーで退室シミュレート
6. `ESC`キーで中断可能

#### 実機モード（ハードウェアあり）

```bash
# フルスクリーン・実機モードで起動
python training_task_example.py
```

**動作:**
- セカンダリディスプレイ（screen=1）にフルスクリーン表示
- 実際の入室センサー、RFIDリーダー、タッチパネルを使用
- 給餌装置が実際に動作

#### 起動オプション

```bash
# --debug または -d: デバッグモード
python training_task_example.py --debug

# --window または -w: ウィンドウモード（フルスクリーンにしない）
python training_task_example.py --window

# 組み合わせ
python training_task_example.py --debug --window
```

### 独自課題の作成

#### 基本テンプレート

```python
from psychopy import visual, core, event
from compartment_hardware import CompartmentHardware

class MyTask:
    def __init__(self, debug_mode=False):
        # ハードウェアクライアント初期化
        self.hw = CompartmentHardware(debug_mode=debug_mode)

        # PsychoPyウィンドウ作成
        self.win = visual.Window(
            size=[1920, 1080],
            fullscr=True,
            screen=1  # セカンダリディスプレイ
        )

        # 刺激作成
        self.create_stimuli()

    def create_stimuli(self):
        # 課題で使う刺激を作成
        self.target = visual.Circle(
            self.win,
            radius=100,
            fillColor='blue'
        )

    def run(self):
        # 1. 入室待ち
        print("入室待ち...")
        while not self.hw.check_entrance():
            core.wait(0.05)

        # 2. RFID読み取り
        print("RFID待ち...")
        rfid = self.hw.wait_for_rfid(timeout_sec=10.0)
        if not rfid:
            print("RFIDタイムアウト")
            return

        print(f"RFID: {rfid}")

        # 3. 課題実行
        for trial in range(20):
            correct = self.run_trial(trial)
            if correct:
                self.hw.dispense_feed(duration_ms=1000)

        # 4. 退室待ち
        self.hw.open_door()
        while not self.hw.check_exit():
            core.wait(0.05)
        self.hw.close_door()

        self.win.close()

    def run_trial(self, trial_num):
        # 課題ロジックをここに実装
        print(f"試行 {trial_num + 1}/20")

        # 刺激提示
        self.target.draw()
        self.win.flip()

        # タッチ待ち
        # ... 課題ロジック ...

        return True  # 正解ならTrue

# 実行
if __name__ == "__main__":
    import sys
    debug = '--debug' in sys.argv
    task = MyTask(debug_mode=debug)
    task.run()
```

---

## APIの使い方

### Pythonクライアントライブラリ

#### 基本的な初期化

```python
from compartment_hardware import CompartmentHardware

# 通常モード（実機）
hw = CompartmentHardware(base_url="http://localhost:5000/api")

# デバッグモード
hw = CompartmentHardware(
    base_url="http://localhost:5000/api",
    debug_mode=True
)

# タイムアウト設定（デフォルト5秒）
hw = CompartmentHardware(timeout=10)
```

#### センサー読み取り

```python
# 入室センサーチェック（瞬時）
if hw.check_entrance():
    print("入室検出")

# 入室を待つ（ブロッキング、タイムアウトあり）
if hw.wait_for_entrance(timeout_sec=30.0):
    print("入室検出")
else:
    print("タイムアウト")

# 他のセンサー
hw.check_exit()         # 退室センサー
hw.check_stay()         # 在室センサー
hw.check_lever_switch() # レバースイッチ
```

#### RFID制御

```python
# RFID読み取り（瞬時）
rfid = hw.read_rfid()
if rfid:
    print(f"RFID: {rfid}")
else:
    print("RFID未検出")

# RFIDを待つ（ブロッキング、タイムアウトあり）
rfid = hw.wait_for_rfid(timeout_sec=10.0)
if rfid:
    print(f"RFID読み取り成功: {rfid}")
else:
    print("タイムアウト")

# RFIDクリア
hw.clear_rfid()
```

#### デバイス制御

```python
# ドア制御
hw.open_door()
hw.close_door()

# ドア状態取得
status = hw.get_door_status()
print(f"ドア状態: {status['state']}")  # 'opened', 'closed', 'unknown'

# レバー制御
hw.extend_lever()   # レバーを出す
hw.retract_lever()  # レバーを引っ込める

# 給餌
hw.dispense_feed(duration_ms=1000)  # 1秒間給餌

# 給餌中かチェック
if hw.is_feeding():
    print("給餌中")
```

#### デバッグモード専用機能

```python
# デバッグモードで初期化必須
hw = CompartmentHardware(debug_mode=True)

# センサーシミュレート
hw.debug_set_sensor("entrance", True)   # 入室センサーON
hw.debug_set_sensor("entrance", False)  # 入室センサーOFF

# 入室を500ms間シミュレート
hw.debug_simulate_entrance()

# RFID設定
hw.debug_set_rfid("1234567890123456")

# ランダムRFID生成
rfid = hw.debug_set_random_rfid()
print(f"生成: {rfid}")

# 全センサー状態取得
sensors = hw.debug_get_all_sensors()
for sensor, state in sensors.items():
    print(f"{sensor}: {'ON' if state else 'OFF'}")

# 課題状態取得
task_status = hw.debug_get_task_status()
print(f"課題: {task_status['taskType']}")
print(f"試行数: {task_status['totalTrials']}")
print(f"成功率: {task_status['successRate']}%")

# 試行履歴取得
history = hw.debug_get_task_history()
for trial in history:
    print(f"試行#{trial['sessionNum']}: {trial['result']}")

# 状態リセット
hw.debug_reset()
```

### REST API（直接呼び出し）

#### Python requests

```python
import requests

# センサー読み取り
response = requests.get("http://localhost:5000/api/sensor/entrance")
data = response.json()
print(f"入室センサー: {data['active']}")

# ドアを開く
response = requests.post("http://localhost:5000/api/door/open")
print(response.json())

# 給餌
response = requests.post(
    "http://localhost:5000/api/feed/dispense",
    json={'durationMs': 1000}
)
print(response.json())

# デバッグ: センサー設定
response = requests.post(
    "http://localhost:5000/api/debug/sensor/set",
    json={'sensor': 'entrance', 'state': True}
)
print(response.json())
```

#### JavaScript (Webアプリから)

```javascript
// センサー読み取り
fetch('http://localhost:5000/api/sensor/entrance')
  .then(res => res.json())
  .then(data => console.log('入室センサー:', data.active));

// ドアを開く
fetch('http://localhost:5000/api/door/open', { method: 'POST' })
  .then(res => res.json())
  .then(data => console.log('ドア:', data.success));

// 給餌
fetch('http://localhost:5000/api/feed/dispense', {
  method: 'POST',
  headers: { 'Content-Type': 'application/json' },
  body: JSON.stringify({ durationMs: 1000 })
})
  .then(res => res.json())
  .then(data => console.log('給餌:', data.success));
```

---

## トラブルシューティング

### C#側の問題

#### APIサーバーが起動しない

**症状:**
- FormMain起動時にエラー
- デバッグ出力に `[API] Web API server started` が出ない

**原因と対処:**

1. **ポートが既に使用されている**
   ```
   エラー: System.ServiceModel.AddressAlreadyInUseException
   ```
   対処: Preferences.xmlでApiServerPortを別の番号に変更（例: 5001）

2. **ファイアウォールがブロック**
   対処: Windows Defender ファイアウォールでCompartment.exeを許可

3. **NuGetパッケージ不足**
   対処: NuGet パッケージマネージャーで復元
   ```
   ツール → NuGet パッケージ マネージャー → ソリューションのNuGetパッケージの復元
   ```

#### デバッグモードが動作しない

**症状:**
- デバッグAPI（/api/debug/*）が400エラー

**原因と対処:**
1. Preferences.xmlで `EnableDebugMode` がfalse
   → trueに変更してソフト再起動

2. Python側で`debug_mode=True`を指定していない
   → `CompartmentHardware(debug_mode=True)`

### Python側の問題

#### APIに接続できない

**症状:**
```
requests.exceptions.ConnectionError: ('Connection aborted.', ...)
```

**原因と対処:**

1. **C# Compartmentソフトが起動していない**
   → C#ソフトを起動

2. **URLが間違っている**
   ```python
   # 正しい
   hw = CompartmentHardware(base_url="http://localhost:5000/api")

   # 間違い（/apiが必要）
   hw = CompartmentHardware(base_url="http://localhost:5000")
   ```

3. **ポート番号が違う**
   → Preferences.xmlのApiServerPortを確認

4. **ファイアウォール**
   → C#側でファイアウォール許可を確認

#### センサーが常にFalse

**原因と対処:**

1. **デバッグモードが無効**
   ```python
   # デバッグモード有効化
   hw = CompartmentHardware(debug_mode=True)
   ```

2. **SaveDIn()が呼ばれていない**
   C#側の問題。BackgroundWorkerで定期的にSaveDIn()が呼ばれているか確認

#### PsychoPyウィンドウが表示されない

**原因と対処:**

1. **セカンダリディスプレイがない**
   ```python
   # プライマリディスプレイに表示
   win = visual.Window(screen=0, ...)
   ```

2. **フルスクリーンエラー**
   ```python
   # ウィンドウモードで起動
   win = visual.Window(fullscr=False, ...)
   ```

### デバッグ手順

#### API接続確認

```bash
# 1. C# API起動確認
curl http://localhost:5000/api/sensor/entrance

# 2. デバッグモード確認
curl http://localhost:5000/api/debug/status

# 3. センサー状態確認
curl http://localhost:5000/api/debug/sensors/all
```

#### Python側デバッグ

```python
# 詳細なエラー表示
import traceback

try:
    hw = CompartmentHardware()
    sensors = hw.debug_get_all_sensors()
    print("接続成功:", sensors)
except Exception as e:
    print("エラー:", e)
    traceback.print_exc()
```

#### ログ確認

C#側:
- Visual Studio出力ウィンドウ（Debugビルド時）
- `Debug.WriteLine()` の出力を確認

Python側:
- コンソール出力
- `print()` でデバッグ情報表示

---

## よくある質問（FAQ）

### Q1. 複数のCompartmentを同時に動かせますか？

**A:** はい、ポート番号を変えれば可能です。

```xml
<!-- 部屋1: Preferences.xml -->
<CompartmentNo>1</CompartmentNo>
<ApiServerPort>5001</ApiServerPort>

<!-- 部屋2: Preferences.xml -->
<CompartmentNo>2</CompartmentNo>
<ApiServerPort>5002</ApiServerPort>
```

Python側:
```python
hw1 = CompartmentHardware(base_url="http://localhost:5001/api")
hw2 = CompartmentHardware(base_url="http://localhost:5002/api")
```

### Q2. デバッグモードでも課題データは記録されますか？

**A:** はい、C#側で通常通り記録されます。デバッグモードはハードウェアのシミュレートのみで、データ記録ロジックは影響を受けません。

### Q3. PsychoPyなしでAPI経由制御できますか？

**A:** はい、任意の言語（Python、JavaScript、C#等）からREST APIを呼び出せます。curlコマンドでも制御可能です。

### Q4. リモートPCから制御できますか？

**A:** 現在はlocalhostのみですが、Startup.csを修正すれば可能です：

```csharp
// Startup.cs
// 変更前: localhost のみ
app.UseWebApi(config);

// 変更後: すべてのインターフェースでリッスン
// FormMain.cs の StartApiServer()
string baseAddress = $"http://*:{port}/";  // localhost → *
```

ただしセキュリティリスクがあるため、LAN内のみで使用推奨。

### Q5. 既存の課題（Training等）は動きますか？

**A:** はい、影響ありません。ExternalControlモード（未実装）を選択しない限り、既存課題は従来通り動作します。

---

## 参考資料

- [実装状況](./STATUS.md)
- [実装計画詳細](./IMPLEMENTATION_PLAN.md)
- [PsychoPy README](./psychopy/README.md)
- [次のステップ](./NEXT_STEPS.md)
