# Compartment Hardware Control API - Phase 1

PsychoPyからCompartmentハードウェアを制御するためのREST APIドキュメント

## 📋 概要

このAPIを使用すると、PsychoPyスクリプトからCompartmentの以下の機能を制御できます：

- **センサー読み取り**: 入口、出口、滞在、レバースイッチ
- **ドア制御**: 開閉
- **レバー制御**: 出し入れ
- **給餌制御**: 餌の供給
- **RFID読み取り**: 個体識別
- **緊急停止**: 全モーター即座停止
- **イベントログ**: ハードウェアコマンド実行履歴（ExternalControlモード時）
- **デバッグモード**: ハードウェアなしでのテスト

## 🚀 セットアップ

### 1. Compartmentアプリケーション側

1. **Compartment.exe を起動**
2. **デバッグモードを有効化**（テスト時）:
   - メイン画面の「デバッグモード」チェックボックスをON
   - アプリケーションを再起動
3. **APIサーバー起動確認**:
   - 起動時に「**API起動: http://localhost:5000/**」メッセージが表示されることを確認

### 2. PsychoPy側

```bash
pip install requests
```

サンプルコード `psychopy_api_example.py` をダウンロードして使用

## 📡 API エンドポイント一覧

### センサー API

| メソッド | エンドポイント | 説明 | レスポンス例 |
|---------|---------------|------|------------|
| GET | `/api/sensor/entrance` | 入口センサー状態 | `{"roomId":"0","state":true}` |
| GET | `/api/sensor/exit` | 出口センサー状態 | `{"roomId":"0","state":false}` |
| GET | `/api/sensor/stay` | 滞在センサー状態 | `{"roomId":"0","state":true}` |
| GET | `/api/sensor/lever` | レバースイッチ状態 | `{"roomId":"0","state":false}` |

### ドア制御 API

| メソッド | エンドポイント | 説明 | レスポンス例 |
|---------|---------------|------|------------|
| POST | `/api/door/open` | ドアを開ける（在室時は開けない） | `{"roomId":"0","success":true,"state":"opening"}` |
| POST | `/api/door/close` | ドアを閉じる | `{"roomId":"0","success":true,"state":"closing"}` |
| GET | `/api/door/status` | ドア状態を取得 | `{"roomId":"0","state":"open","sensorOpen":true,"sensorClose":false}` |

**ドアの状態 (state):**
- `"open"` - 完全に開いている
- `"closed"` - 完全に閉じている
- `"moving"` - 開閉中
- `"error"` - センサー異常

**安全機能:**
- `POST /api/door/open` は動物が在室中（stay sensor = ON）の場合、ドアを開けません
- ドア開閉はC#側のステートマシンで自動制御されます（開センサーONまで継続）

**センサー故障時の対応:**
- `GET /api/door/status` をタイムアウト付きで使用することを推奨（例1参照）
- タイムアウト時はフォールバック時間待機後に実験継続（センサー故障でも実験中断を防ぐ）

### レバー制御 API

| メソッド | エンドポイント | 説明 | レスポンス例 |
|---------|---------------|------|------------|
| POST | `/api/lever/extend` | レバーを出す | `{"roomId":"0","success":true,"command":"extend"}` |
| POST | `/api/lever/retract` | レバーを引っ込める | `{"roomId":"0","success":true,"command":"retract"}` |

### 給餌制御 API

| メソッド | エンドポイント | 説明 | リクエストボディ | レスポンス例 |
|---------|---------------|------|----------------|------------|
| POST | `/api/feed/dispense` | 餌を供給 | `{"durationMs":1000}` | `{"roomId":"0","success":true,"durationMs":1000}` |
| GET | `/api/feed/status` | 給餌中か確認 | - | `{"roomId":"0","isFeeding":false}` |

### RFID API

| メソッド | エンドポイント | 説明 | レスポンス例 |
|---------|---------------|------|------------|
| GET | `/api/rfid/read` | RFIDを読み取る | `{"roomId":"0","id":"1234567890123456"}` |
| POST | `/api/rfid/clear` | RFIDをクリア | `{"roomId":"0","success":true}` |

### 緊急停止 API

| メソッド | エンドポイント | 説明 | レスポンス例 |
|---------|---------------|------|------------|
| POST | `/api/emergency/stop` | 全モーター緊急停止（ドア、レバー、給餌） | `{"roomId":"0","success":true,"message":"All motors stopped"}` |

**重要:**
- PsychoPyスクリプトのfinallyブロックで必ず呼び出してください
- スクリプトがクラッシュしても全モーターが確実に停止します
- ドア、レバー、給餌モーターすべてを即座に停止します

### デバッグ API（ハードウェアなしテスト用）

| メソッド | エンドポイント | 説明 | リクエストボディ | レスポンス例 |
|---------|---------------|------|----------------|------------|
| GET | `/api/debug/status` | デバッグモード状態 | - | `{"roomId":"0","debugModeEnabled":true,"hardwareConnected":false}` |
| POST | `/api/debug/sensor/set` | センサー状態を設定 | `{"sensor":"entrance","state":true}` | `{"success":true,"sensor":"entrance","state":true}` |
| POST | `/api/debug/rfid/set` | RFID値を設定 | `{"id":"1234567890123456"}` | `{"roomId":"0","success":true,"id":"1234567890123456"}` |
| POST | `/api/debug/rfid/random` | ランダムRFIDを生成 | - | `{"roomId":"0","success":true,"id":"9876543210987654"}` |
| GET | `/api/debug/sensors/all` | 全センサー状態取得 | - | `{"roomId":"0","sensors":{...}}` |
| POST | `/api/debug/reset` | 全状態をリセット | - | `{"roomId":"0","success":true}` |

## 💻 使用例

### 例1: 基本的なトライアルフロー（推奨：緊急停止機能付き）

```python
from psychopy import core
import requests
import time

API_URL = "http://localhost:5000"

def wait_for_door(state, timeout=5.0, fallback_time=2.0):
    """ドアの状態を待つ（タイムアウト＋フォールバック）"""
    start = time.time()
    sensor_ok = False

    while time.time() - start < timeout:
        response = requests.get(f"{API_URL}/api/door/status")
        if response.json()["state"] == state:
            print(f"✓ Door {state} confirmed!")
            sensor_ok = True
            break
        core.wait(0.1)

    if not sensor_ok:
        print(f"⚠ Warning: Door sensor timeout ({timeout}s)")
        print(f"   Waiting {fallback_time}s and continuing...")
        time.sleep(fallback_time)

    return sensor_ok

try:
    # 1. 動物が入口に来るまで待つ
    while True:
        response = requests.get(f"{API_URL}/api/sensor/entrance")
        if response.json()["state"]:
            print("Animal detected!")
            break
        core.wait(0.1)

    # 2. ドアを開ける（安全チェック付き）
    result = requests.post(f"{API_URL}/api/door/open")
    if not result.json()["success"]:
        print("Error: Cannot open door (animal may be inside)")
        exit()

    # 3. ドアが完全に開くまで待つ（タイムアウト付き）
    wait_for_door("open", timeout=5.0, fallback_time=2.0)

    # 4. 動物が入室するまで待つ
    while True:
        response = requests.get(f"{API_URL}/api/sensor/stay")
        if response.json()["state"]:
            print("Animal entered!")
            break
        core.wait(0.1)

    # 5. ドアを閉じる
    requests.post(f"{API_URL}/api/door/close")

    # 6. ドアが完全に閉じるまで待つ（タイムアウト付き）
    wait_for_door("closed", timeout=5.0, fallback_time=2.0)

    # 7. レバーを出す
    requests.post(f"{API_URL}/api/lever/extend")

    # 8. レバー押しを待つ
    while True:
        response = requests.get(f"{API_URL}/api/sensor/lever")
        if response.json()["state"]:
            print("Lever pressed!")
            break
        core.wait(0.1)

    # 9. 報酬を与える
    requests.post(f"{API_URL}/api/feed/dispense", json={"durationMs": 500})

    # 10. レバーを引っ込める
    requests.post(f"{API_URL}/api/lever/retract")

finally:
    # **重要**: スクリプトがクラッシュしても必ずモーターを停止
    print("Stopping all motors (safety cleanup)...")
    requests.post(f"{API_URL}/api/emergency/stop")
```

**重要:**
- ドアセンサーが故障した場合でも実験は継続されます（タイムアウト後にフォールバック時間待機）
- finallyブロックで必ず緊急停止APIを呼び出し、スクリプトがクラッシュしてもモーターが停止することを保証

### 例2: デバッグモードでのテスト

```python
import requests

API_URL = "http://localhost:5000"

# センサー状態を手動で設定
requests.post(f"{API_URL}/api/debug/sensor/set",
             json={"sensor": "entrance", "state": True})

# RFIDを設定
requests.post(f"{API_URL}/api/debug/rfid/set",
             json={"id": "1234567890123456"})

# 全センサー状態を確認
response = requests.get(f"{API_URL}/api/debug/sensors/all")
print(response.json())
```

### 例3: PsychoPy Builderとの統合

Code Componentに以下を記述：

```python
# Begin Experiment
import requests
API_URL = "http://localhost:5000"

# Begin Routine
requests.post(f"{API_URL}/api/lever/extend")

# Each Frame
response = requests.get(f"{API_URL}/api/sensor/lever")
if response.json()["state"]:
    # Lever pressed - give reward
    requests.post(f"{API_URL}/api/feed/dispense", json={"durationMs": 500})
    continueRoutine = False

# End Routine
requests.post(f"{API_URL}/api/lever/retract")
```

## ⚠️ ネットワークエラー処理

PsychoPyスクリプト実行中にネットワークエラー（API接続失敗）が発生した場合：

### エラー処理の動作

1. **エラーログ自動出力**
   - ファイル名: `compartment_api_errors_{timestamp}.log`
   - 保存場所: スクリプト実行ディレクトリ
   - 内容: エラー発生時刻、URL、エラー詳細

2. **実験即時停止**
   - `CompartmentAPIError` 例外が発生
   - 実験スクリプトが中断
   - 緊急停止APIを呼び出し（可能な場合）

3. **手動対応が必要な場合**
   - ネットワーク断絶時は緊急停止APIも失敗します
   - 画面に「⚠ Please manually stop motors using Compartment application」と表示
   - Compartmentアプリの緊急停止ボタンで手動停止してください

### エラーログ例

```
2025-01-15 10:30:45,123 - ERROR - Connection error: Cannot connect to Compartment API at http://localhost:5000/api/door/open
2025-01-15 10:30:45,125 - ERROR - Details: [Errno 111] Connection refused
```

### リトライについて

**ネットワークエラー時はリトライを行いません。**
- 理由: 動物実験中の予期しないハードウェア動作を防ぐため
- 対応: エラーログを確認し、原因を特定してから実験を再開してください

## 🛠️ トラブルシューティング

### APIに接続できない

1. **Compartmentアプリが起動しているか確認**
   - 起動時に「API起動: http://localhost:5000/」メッセージを確認

2. **ファイアウォール設定を確認**
   - Windows Defenderでポート5000が許可されているか確認

3. **ポートが使用中でないか確認**
   ```bash
   netstat -ano | findstr :5000
   ```

4. **エラーログを確認**
   - `compartment_api_errors_*.log` ファイルを開いて詳細を確認

### デバッグモードが動作しない

1. **デバッグモードが有効か確認**
   ```bash
   curl http://localhost:5000/api/debug/status
   ```
   レスポンスの `debugModeEnabled` が `true` であることを確認

2. **アプリケーションを再起動**
   - デバッグモードチェックボックスをONにした後、必ず再起動

### センサー値が変わらない

- デバッグモード以外では、実際のハードウェアの状態を反映
- デバッグモードでは、`/api/debug/sensor/set` で手動設定が必要

### ドアが開かない

1. **動物が在室中**
   - `POST /api/door/open` は、在室センサーがONの場合は開けません（安全機能）
   - レスポンスの `success` が `false` の場合、動物が在室中です

2. **ドアの状態を確認**
   ```bash
   curl http://localhost:5000/api/door/status
   ```
   - `state` が `"error"` の場合、センサー異常の可能性があります

### ドアセンサーがタイムアウトする

**推奨実装（例1参照）では、センサー故障時も実験は継続します:**

1. **センサー確認を5秒間試行**
   - ドアセンサーが正常なら、開閉完了を確認

2. **タイムアウト時はフォールバックモード**
   - 警告メッセージを表示
   - 固定時間（2秒）待機後に続行
   - 実験は中断されません

3. **センサー故障が疑われる場合**
   - 実験終了後にセンサー配線を確認
   - デバッグモードで `GET /api/debug/sensors/all` を確認

## 📊 イベントログ機能（ExternalControlモード）

ExternalControlモードで実験を開始すると、C#側が自動的に全ハードウェアコマンドをCSVファイルに記録します。

### ログファイル

**ファイル名**: `ExternalControl_Room{N}_{YYYYMMDD}.csv`
- 例: `ExternalControl_Room0_20250115.csv`
- 保存場所: `%LOCALAPPDATA%\Compartment\{version}\`
- 日付別に自動作成（同日は追記モード）

**CSV形式**:
```csv
Timestamp,EventType,Device,Parameter,Success,Message
2025-01-15 10:30:45.123,DoorOpen,Door,-,True,
2025-01-15 10:30:50.456,FeedDispense,Feeder,1000ms,True,
2025-01-15 10:31:00.789,LeverExtend,Lever,-,True,
2025-01-15 10:31:05.012,EmergencyStop,AllMotors,-,True,Door, Lever, Feed motors stopped
```

### ログ対象イベント

| EventType | Device | Parameter | 説明 |
|-----------|--------|-----------|------|
| DoorOpen | Door | - | ドア開き実行 |
| DoorClose | Door | - | ドア閉じ実行 |
| LeverExtend | Lever | - | レバー出し実行 |
| LeverRetract | Lever | - | レバー引っ込め実行 |
| FeedDispense | Feeder | {duration}ms | 給餌実行（時間付き） |
| RFIDRead | RFID | {rfid_id} | RFID読み取り |
| RFIDClear | RFID | - | RFIDクリア |
| EmergencyStop | AllMotors | - | 緊急停止実行 |

### PsychoPy側のログと組み合わせる

**推奨**: PsychoPy側でもログを取り、C#側ログと突き合わせて完全な記録を作成

```python
import csv
from datetime import datetime

# PsychoPy側のログ
psychopy_log = open("psychopy_log.csv", "w", newline="")
writer = csv.writer(psychopy_log)
writer.writerow(["Timestamp", "Event", "Detail"])

# センサー読み取りをログ
response = api.get_entrance_sensor()
writer.writerow([datetime.now().isoformat(), "SensorRead", f"entrance={response['state']}"])

# コマンド実行をログ
api.open_door()
writer.writerow([datetime.now().isoformat(), "CommandSent", "DoorOpen"])
```

**2つのログの違い**:
- **C#側ログ**: ハードウェアコマンドが実際に実行された時刻
- **PsychoPy側ログ**: API呼び出し時刻、センサー読み取り結果、実験タイミング

### ログの確認方法

1. Compartmentアプリで `%LOCALAPPDATA%` フォルダを開く
2. `Compartment\{version}\ExternalControl_Room{N}_{date}.csv` を確認
3. Excelやテキストエディタで開く

## 📚 次のステップ

Phase 1では基本的なハードウェア制御APIとイベントログを実装しました。

**Phase 2** (今後の拡張):
- センサー状態変化の自動記録（現在はコマンド実行のみ）
- タスク開始/停止API
- トライアルデータ収集API
- WebSocket によるリアルタイム通知

## 🔗 関連ファイル

- `psychopy_api_example.py` - PsychoPy サンプルコード
- `Startup.cs` - API サーバー設定
- `Controllers/` - API エンドポイント実装
- `Services/HardwareService.cs` - ハードウェア制御ロジック

## ❓ サポート

問題が発生した場合は、以下を確認してください：

1. Visual StudioのOutput Windowで「[API]」プレフィックスのログ
2. Compartmentアプリのデバッグメッセージエリア
3. HTTP ステータスコード（200=成功、400=エラー、500=サーバーエラー）
