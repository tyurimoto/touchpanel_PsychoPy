# Compartment PsychoPy統合 実装状況

最終更新: 2026-01-29

## プロジェクト概要

既存のC# Compartment（動物行動実験ソフト）をハードウェアAPIサーバー化し、PsychoPyから課題制御を可能にするプロジェクト。

**アーキテクチャ:**
```
PsychoPy (Python) ─── HTTP REST API ───→ C# API Server ───→ ハードウェア
   課題制御                              OWIN/Katana          IoBoard, RFID
```

**主要な目的:**
- ✅ ハードウェア制御はC#で維持（既存コード活用）
- ✅ 課題ロジックはPsychoPyで柔軟に実装
- ✅ デバッグモード: ハードウェアなしでPC単体で開発・テスト可能
- 🔄 複数部屋統合管理（中央オーケストレーター）- 将来実装

## 実装完了状況

### ✅ Phase 1: APIサーバー構築（完了）

#### 1-1. OWIN/Katana統合

**完了項目:**
- ✅ NuGetパッケージ追加（Microsoft.AspNet.WebApi.OwinSelfHost）
- ✅ Startup.cs作成（OWIN設定）
- ✅ FormMainにOWINサーバー起動/停止ロジック追加
- ✅ ポート番号設定（PreferencesDat.ApiServerPort、デフォルト5000）

**変更ファイル:**
- `/Compartment/Startup.cs` (新規)
- `/Compartment/FormMain.cs` (StartApiServer/StopApiServer追加)
- `/Compartment/packages.config` (OWIN関連パッケージ追加)
- `/Compartment/PreferencesDat.cs` (ApiServerPortプロパティ追加)

#### 1-2. 基本APIコントローラー

**完了項目:**
- ✅ SensorController（センサー読み取りAPI）
- ✅ DoorController（ドア制御API）
- ✅ LeverController（レバー制御API）
- ✅ FeedController（給餌制御API）
- ✅ RFIDController（RFID読み取りAPI）

**APIエンドポイント一覧:**
```
センサー読み取り:
  GET  /api/sensor/entrance   - 入室センサー
  GET  /api/sensor/exit       - 退室センサー
  GET  /api/sensor/stay       - 在室センサー
  GET  /api/sensor/lever      - レバースイッチ

RFID:
  GET    /api/rfid/read      - RFID読み取り
  DELETE /api/rfid           - RFIDクリア

ドア制御:
  POST /api/door/open        - ドアを開く
  POST /api/door/close       - ドアを閉じる
  GET  /api/door/status      - ドア状態取得

レバー制御:
  POST /api/lever/extend     - レバーを出す
  POST /api/lever/retract    - レバーを引っ込める

給餌:
  POST /api/feed/dispense    - 給餌実行
  GET  /api/feed/status      - 給餌状態取得
```

**変更ファイル:**
- `/Compartment/Controllers/SensorController.cs` (新規)
- `/Compartment/Controllers/DoorController.cs` (新規)
- `/Compartment/Controllers/LeverController.cs` (新規)
- `/Compartment/Controllers/FeedController.cs` (新規)
- `/Compartment/Controllers/RFIDController.cs` (新規)
- `/Compartment/Models/` (各種レスポンスモデル)

#### 1-3. HardwareService（FormMain連携）

**完了項目:**
- ✅ スレッドセーフなFormMain連携（Invoke + TaskCompletionSource）
- ✅ 全コントローラーで静的初期化パターン使用
- ✅ CompartmentNo（RoomID）をすべてのレスポンスに含める

**変更ファイル:**
- `/Compartment/Services/HardwareService.cs` (新規)

### ✅ Phase 1+: デバッグモード実装（完了）

#### 1-4. デバッグモード用IOボード

**完了項目:**
- ✅ IoMicrochipDummyEx（完全ダミーIOボード）
  - センサー状態をメモリに保持
  - デバイスコマンドで自動的にセンサー状態更新
  - API経由で状態設定可能
- ✅ IoHybridBoard（ハイブリッドIOボード）
  - 実機＋手動シミュレート切替
  - センサーごとに「実機/手動」を選択可能
  - デフォルト: entrance/exit=手動、他=実機
- ✅ RFIDReaderDummy（ダミーRFIDリーダー）
  - 手動RFID設定
  - ランダムRFID生成

**変更ファイル:**
- `/Compartment/IoMicrochipDummyEx.cs` (新規)
- `/Compartment/IoHybridBoard.cs` (新規)
- `/Compartment/RFIDReaderDummy.cs` (新規)

#### 1-5. デバッグモードAPI

**完了項目:**
- ✅ DebugController（デバッグ専用API）
- ✅ センサー状態設定API
- ✅ RFID設定API
- ✅ 課題状態取得API（タスク情報、試行履歴）

**APIエンドポイント一覧:**
```
デバッグモード専用:
  POST /api/debug/sensor/set         - センサー状態設定
  POST /api/debug/rfid/set           - RFID値設定
  POST /api/debug/rfid/random        - ランダムRFID生成
  GET  /api/debug/sensors/all        - 全センサー状態取得
  POST /api/debug/reset              - 状態リセット
  GET  /api/debug/status             - デバッグモード状態取得
  GET  /api/debug/task/status        - 課題状態取得
  GET  /api/debug/task/history       - 試行履歴取得
  POST /api/debug/task/simulate-correct   - 正解試行シミュレート
  POST /api/debug/task/simulate-incorrect - 不正解試行シミュレート
```

**変更ファイル:**
- `/Compartment/Controllers/DebugController.cs` (新規)
- `/Compartment/Services/HardwareService.cs` (デバッグメソッド追加)

#### 1-6. デバッグモード設定

**完了項目:**
- ✅ PreferencesDatにデバッグモードプロパティ追加
- ✅ EDebugModeType列挙型（FullDummy / Hybrid）
- ✅ FormMainでデバッグモード判定・初期化
- ✅ IOボード/RFID自動切り替え

**変更ファイル:**
- `/Compartment/PreferencesDat.cs` (EnableDebugMode, DebugModeType追加)
- `/Compartment/FormMain.cs` (InitializeIoBoardForDebugMode等追加)

### ✅ Phase 2: Pythonクライアントライブラリ（完了）

#### 2-1. compartment_hardware.py

**完了項目:**
- ✅ 全APIエンドポイントに対応するメソッド
- ✅ センサー読み取り（check_entrance, check_exit, check_stay, check_lever_switch）
- ✅ RFID制御（read_rfid, clear_rfid, wait_for_rfid）
- ✅ デバイス制御（open_door, close_door, extend_lever, retract_lever, dispense_feed）
- ✅ デバッグモード機能（debug_set_sensor, debug_set_rfid, debug_get_task_status等）
- ✅ ヘルパーメソッド（wait_for_entrance, wait_for_rfid, debug_simulate_entrance等）

**変更ファイル:**
- `/psychopy/compartment_hardware.py` (新規)

#### 2-2. デバッグツール

**完了項目:**
- ✅ debug_keyboard_control.py（キーボードでセンサーシミュレート）
- ✅ PsychoPyウィンドウに操作ガイド表示
- ✅ リアルタイムステータス表示
- ✅ 課題状態・試行履歴表示機能

**変更ファイル:**
- `/psychopy/debug_keyboard_control.py` (新規)

#### 2-3. 訓練課題サンプル

**完了項目:**
- ✅ training_task_example.py（完全な課題フロー実装）
- ✅ 入室→RFID→課題実行→給餌→退室の完全フロー
- ✅ デバッグモード対応（キーボード操作）
- ✅ タッチパネル入力対応

**変更ファイル:**
- `/psychopy/training_task_example.py` (新規)

#### 2-4. ドキュメント

**完了項目:**
- ✅ PsychoPy README（使い方、API仕様、サンプルコード）

**変更ファイル:**
- `/psychopy/README.md` (新規)

## 🔄 未実装項目（Phase 2+）

### Phase 2-1: ExternalControlモード（未実装）

**必要な作業:**
- ❌ ECpTask列挙型にExternalControl追加
- ❌ OpCollection/UcOperationにスキップロジック追加
- ❌ FormSub表示制御（ExternalControl時は非表示）
- ❌ Preferences UIに「ExternalControl」選択肢追加

**対象ファイル:**
- `/Compartment/PreferencesDat.cs`
- `/Compartment/OpCollection.cs`
- `/Compartment/UcOperation.cs`
- `/Compartment/FormMain.cs`
- `/Compartment/UserControlPreferencesTab.Designer.cs`

### Phase 3: UserControlDebugPanel（未実装）

**必要な作業:**
- ❌ デバッグコントロールパネルGUI作成
- ❌ センサー状態表示（LED風）
- ❌ センサーON/OFFボタン
- ❌ RFID入力フィールド
- ❌ デバイス状態表示
- ❌ ログ出力エリア
- ❌ キーボードショートカット（オプション）

**対象ファイル:**
- `/Compartment/UserControlDebugPanel.cs` (新規)
- `/Compartment/UserControlDebugPanel.Designer.cs` (新規)
- `/Compartment/FormMain.cs` (パネル表示ロジック追加)

**理由:** Windows Forms Designerが必要（macOSでは実装不可）

### Phase 4: Preferences UI更新（未実装）

**必要な作業:**
- ❌ チェックボックス「Enable Debug Mode」追加
- ❌ ラジオボタン「Full Dummy / Hybrid」追加
- ❌ イベントハンドラ追加

**対象ファイル:**
- `/Compartment/UserControlPreferencesTab.Designer.cs`
- `/Compartment/UserControlPreferencesTab.cs`

**理由:** Windows Forms Designerが必要（macOSでは実装不可）

### Phase 5: 既存課題の移行（未実装）

**必要な作業:**
- ❌ DelayMatch課題のPython実装
- ❌ データフォーマット互換性確認
- ❌ 検証レポート作成

**対象ファイル:**
- `/psychopy/delaymatch_task.py` (新規)

### Phase 6: 中央オーケストレーター（未実装）

**必要な作業:**
- ❌ FastAPI中央サーバー実装
- ❌ 複数部屋管理API
- ❌ 条件付きシーケンス制御
- ❌ Webダッシュボード
- ❌ データ集約機能

**対象ファイル:**
- `/CentralOrchestrator/main.py` (新規)
- `/CentralOrchestrator/sequence_manager.py` (新規)
- 他多数

## ファイル構成

```
compartment/
├── cs/Compartment/                      # C#プロジェクト
│   └── Compartment/
│       ├── Startup.cs                   # ✅ OWIN設定
│       ├── FormMain.cs                  # ✅ API起動ロジック追加
│       ├── PreferencesDat.cs            # ✅ デバッグモード設定追加
│       ├── IoMicrochipDummyEx.cs        # ✅ 完全ダミーIOボード
│       ├── IoHybridBoard.cs             # ✅ ハイブリッドIOボード
│       ├── RFIDReaderDummy.cs           # ✅ ダミーRFIDリーダー
│       ├── Controllers/
│       │   ├── SensorController.cs      # ✅ センサーAPI
│       │   ├── DoorController.cs        # ✅ ドアAPI
│       │   ├── LeverController.cs       # ✅ レバーAPI
│       │   ├── FeedController.cs        # ✅ 給餌API
│       │   ├── RFIDController.cs        # ✅ RFID API
│       │   └── DebugController.cs       # ✅ デバッグAPI
│       ├── Services/
│       │   └── HardwareService.cs       # ✅ FormMain連携
│       └── Models/
│           └── (各種レスポンスモデル)   # ✅
│
├── psychopy/                            # PsychoPy課題スクリプト
│   ├── compartment_hardware.py          # ✅ APIクライアントライブラリ
│   ├── debug_keyboard_control.py        # ✅ デバッグツール
│   ├── training_task_example.py         # ✅ 訓練課題サンプル
│   └── README.md                        # ✅ PsychoPy使い方ガイド
│
├── IMPLEMENTATION_PLAN.md               # ✅ 実装計画（詳細版）
├── STATUS.md                            # ✅ このファイル
├── USAGE.md                             # 🔄 操作方法マニュアル（次に作成）
└── NEXT_STEPS.md                        # 🔄 次のステップ（次に作成）
```

## 動作環境

### C#側
- .NET Framework 4.8
- Windows 10/11
- OWIN/Katana (Microsoft.AspNet.WebApi.OwinSelfHost 5.2.9)

### Python側
- Python 3.8以上
- PsychoPy 2023.x以上
- requests ライブラリ

### ネットワーク
- C# APIサーバー: http://localhost:5000 (デフォルト)
- PythonクライアントとC#サーバーは同一PC、またはLAN内で通信

## テスト状況

### ✅ 実施済み
- APIエンドポイント構文チェック（コンパイル確認）
- Pythonコード構文チェック
- デバッグモード初期化ロジック確認

### ❌ 未実施（Windows環境が必要）
- C#ビルド・実行テスト
- 実機でのAPI動作確認
- PsychoPyからのAPI呼び出しテスト
- デバッグモード動作確認
- 課題フロー完全テスト

## 既知の問題

### macOS環境での制限
- C#ビルド不可（Windowsが必要）
- Windows Forms Designer不可（GUIパーツ追加できない）

### 次回実装時の注意点
1. **DebugController初期化**: FormMain.csで`DebugController.Initialize(_hardwareService);`を呼んでいるため、コンパイル時にusingディレクティブ追加が必要かも
2. **HardwareServiceのUsing**: `System.Linq`が必要（GetTaskHistory内でOrderByDescending使用）
3. **PreferencesDat**: XML シリアライズ属性が正しく動作するか要確認

## 次のステップ

詳細は `NEXT_STEPS.md` を参照。

**Windows環境で最優先:**
1. C#プロジェクトビルド
2. APIサーバー起動テスト
3. Postmanでエンドポイント動作確認
4. デバッグモード動作確認
5. PsychoPy課題実行

**GUI実装（Windows Forms Designer必要）:**
1. UserControlDebugPanel作成
2. Preferences UIにデバッグモード設定追加

## 参考資料

- [実装計画詳細](./IMPLEMENTATION_PLAN.md)
- [PsychoPy使い方](./psychopy/README.md)
- [操作方法マニュアル](./USAGE.md) ← 次に作成
- [次のステップ](./NEXT_STEPS.md) ← 次に作成
