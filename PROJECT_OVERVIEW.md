# Compartment PsychoPy統合プロジェクト

既存のC# Compartment（動物行動実験ソフト）をハードウェアAPIサーバー化し、PsychoPyから課題制御を可能にするプロジェクト。

## プロジェクト概要

### アーキテクチャ

```
┌─────────────────────────────────────┐
│  PsychoPy スクリプト (Python)        │
│  - 課題フロー制御                    │
│  - セカンダリディスプレイに表示       │
│  - タッチ入力検出                    │
└──────────────┬──────────────────────┘
               │ HTTP REST API
               │ (localhost:5000)
               ▼
┌─────────────────────────────────────┐
│  C# Web API サーバー                 │
│  - OWIN/Katana (自己ホスト)          │
│  - ハードウェア制御APIを公開          │
│  - FormMainと共存                   │
└──────────────┬──────────────────────┘
               │
               ▼
┌─────────────────────────────────────┐
│  既存ハードウェア制御                 │
│  - DevDoor, DevLever, DevFeed...    │
│  - IoBoardBase (IoMicrochip)        │
│  - センサー、RFID                    │
└─────────────────────────────────────┘
```

### 主要な特徴

- ✅ **ハードウェア制御**: C#で既存コードを維持
- ✅ **課題ロジック**: PsychoPyで柔軟に実装
- ✅ **REST API**: 標準的なHTTP通信で連携
- ✅ **デバッグモード**: ハードウェアなしでPC単体動作
- 🔄 **複数部屋管理**: 中央オーケストレーターで統合管理（将来実装）

### 実装状況

#### ✅ 完了（Phase 1-2）
- C# OWIN APIサーバー
- 基本APIエンドポイント（センサー、RFID、ドア、レバー、給餌）
- デバッグモード（完全ダミー、ハイブリッド）
- Python APIクライアントライブラリ
- PsychoPy訓練課題サンプル
- デバッグツール（キーボードコントロール）

#### ❌ 未実装
- C#ビルド・動作確認（Windows環境必要）
- UserControlDebugPanel（GUI）
- Preferences UI更新
- ExternalControlモード
- 既存課題のPython移行
- 中央オーケストレーター

詳細は [STATUS.md](./STATUS.md) 参照

## クイックスタート

### 必要な環境

**C#側:**
- Windows 10/11
- .NET Framework 4.8
- Visual Studio 2019/2022

**Python側:**
- Python 3.8以上
- PsychoPy 2023.x以上
- requests ライブラリ

### 初回セットアップ

#### 1. C#プロジェクトのビルド

```bash
# Visual Studioで開く
1. cs/Compartment/Compartment.sln を開く
2. NuGetパッケージ復元
3. ビルド → ソリューションのビルド
```

#### 2. デバッグモード設定

`Preferences.xml` を編集:
```xml
<EnableDebugMode>true</EnableDebugMode>
<DebugModeType>FullDummy</DebugModeType>
<ApiServerPort>5000</ApiServerPort>
```

#### 3. Compartmentソフト起動

```
Compartment.exe を実行
```

APIサーバーが自動起動: `http://localhost:5000/api`

#### 4. Python環境セットアップ

```bash
pip install psychopy requests
```

#### 5. デバッグツール実行

```bash
cd psychopy
python debug_keyboard_control.py
```

キーボードでセンサーをシミュレート:
- `E`: 入室
- `R`: RFID生成
- `F`: 給餌
- `X`: 退室

#### 6. 訓練課題実行

```bash
python training_task_example.py --debug --window
```

## ドキュメント

### 📚 主要ドキュメント

| ファイル | 内容 | 対象読者 |
|---------|------|---------|
| [PROJECT_OVERVIEW.md](./PROJECT_OVERVIEW.md) | このファイル（プロジェクト概要） | 全員 |
| [STATUS.md](./STATUS.md) | 実装状況サマリー | 開発者 |
| [USAGE.md](./USAGE.md) | 操作方法マニュアル | ユーザー・開発者 |
| [NEXT_STEPS.md](./NEXT_STEPS.md) | 次にやるべきこと | 開発者 |
| [IMPLEMENTATION_PLAN.md](./IMPLEMENTATION_PLAN.md) | 実装計画詳細 | 開発者 |
| [psychopy/README.md](./psychopy/README.md) | PsychoPy使い方ガイド | PsychoPyユーザー |

### 📖 読む順序（初めての方）

1. **PROJECT_OVERVIEW.md**（このファイル）- プロジェクト全体像
2. **STATUS.md** - 現在の実装状況を把握
3. **USAGE.md** - 実際の使い方を学ぶ
4. **psychopy/README.md** - PsychoPy課題の作り方

### 📝 継続作業時

1. **STATUS.md** - どこまで完了したか確認
2. **NEXT_STEPS.md** - 次にやることを確認
3. **USAGE.md** - 操作方法を参照

## ディレクトリ構成

```
compartment/
├── PROJECT_OVERVIEW.md              # プロジェクト概要（このファイル）
├── STATUS.md                        # 実装状況サマリー
├── USAGE.md                         # 操作方法マニュアル
├── NEXT_STEPS.md                    # 次のステップ
├── IMPLEMENTATION_PLAN.md           # 実装計画詳細
│
├── README.md                        # 既存の説明書（日本語）
├── Schedule.md                      # 既存のスケジュール
│
├── cs/Compartment/                  # C#プロジェクト
│   └── Compartment/
│       ├── Compartment.sln          # Visual Studioソリューション
│       ├── Startup.cs               # OWIN設定
│       ├── FormMain.cs              # メインフォーム（API起動含む）
│       ├── PreferencesDat.cs        # 設定クラス
│       ├── IoMicrochipDummyEx.cs    # 完全ダミーIOボード
│       ├── IoHybridBoard.cs         # ハイブリッドIOボード
│       ├── RFIDReaderDummy.cs       # ダミーRFIDリーダー
│       ├── Controllers/             # APIコントローラー
│       │   ├── SensorController.cs
│       │   ├── DoorController.cs
│       │   ├── LeverController.cs
│       │   ├── FeedController.cs
│       │   ├── RFIDController.cs
│       │   └── DebugController.cs
│       ├── Services/
│       │   └── HardwareService.cs   # FormMain連携サービス
│       └── Models/                  # APIレスポンスモデル
│
└── psychopy/                        # PsychoPy課題スクリプト
    ├── README.md                    # PsychoPy使い方ガイド
    ├── compartment_hardware.py      # APIクライアントライブラリ
    ├── debug_keyboard_control.py    # デバッグツール
    └── training_task_example.py     # 訓練課題サンプル
```

## API仕様

### 基本エンドポイント

**センサー読み取り:**
```
GET /api/sensor/entrance   - 入室センサー
GET /api/sensor/exit       - 退室センサー
GET /api/sensor/stay       - 在室センサー
GET /api/sensor/lever      - レバースイッチ
```

**RFID:**
```
GET    /api/rfid/read      - RFID読み取り
DELETE /api/rfid           - RFIDクリア
```

**ドア制御:**
```
POST /api/door/open        - ドアを開く
POST /api/door/close       - ドアを閉じる
GET  /api/door/status      - ドア状態取得
```

**レバー制御:**
```
POST /api/lever/extend     - レバーを出す
POST /api/lever/retract    - レバーを引っ込める
```

**給餌:**
```
POST /api/feed/dispense    - 給餌実行
GET  /api/feed/status      - 給餌状態取得
```

### デバッグモード専用API

```
POST /api/debug/sensor/set         - センサー状態設定
POST /api/debug/rfid/set           - RFID値設定
POST /api/debug/rfid/random        - ランダムRFID生成
GET  /api/debug/sensors/all        - 全センサー状態取得
POST /api/debug/reset              - 状態リセット
GET  /api/debug/status             - デバッグモード状態取得
GET  /api/debug/task/status        - 課題状態取得
GET  /api/debug/task/history       - 試行履歴取得
```

詳細は [USAGE.md](./USAGE.md) 参照

## 使用例

### Python（PsychoPy）から使う

```python
from compartment_hardware import CompartmentHardware

# 初期化
hw = CompartmentHardware(debug_mode=True)

# 入室待ち
while not hw.check_entrance():
    core.wait(0.05)

# RFID読み取り
rfid = hw.wait_for_rfid(timeout_sec=10.0)
print(f"RFID: {rfid}")

# 課題実行後、給餌
if correct:
    hw.dispense_feed(duration_ms=1000)

# 退室待ち
hw.open_door()
while not hw.check_exit():
    core.wait(0.05)
hw.close_door()
```

### curlコマンドで使う

```bash
# センサー読み取り
curl http://localhost:5000/api/sensor/entrance

# ドアを開く
curl -X POST http://localhost:5000/api/door/open

# 給餌
curl -X POST http://localhost:5000/api/feed/dispense \
  -H "Content-Type: application/json" \
  -d '{"durationMs":1000}'

# デバッグ: センサー設定
curl -X POST http://localhost:5000/api/debug/sensor/set \
  -H "Content-Type: application/json" \
  -d '{"sensor":"entrance","state":true}'
```

## よくある質問（FAQ）

### Q1. どうやって始めればいいですか？

**A:** Windows環境で以下の順序で進めてください：

1. [NEXT_STEPS.md](./NEXT_STEPS.md) のタスク1～4を実施
2. デバッグモードで動作確認
3. PsychoPy課題を実行

### Q2. macOSでも動きますか？

**A:** Python側（PsychoPy課題）はmacOSでも動作しますが、C#側はWindows専用です。

### Q3. ハードウェアなしでテストできますか？

**A:** はい、デバッグモードで完全にPC単体で動作します。
- C#側: `EnableDebugMode=true`
- Python側: `debug_mode=True`

### Q4. 既存の課題は動きますか？

**A:** はい、影響ありません。ExternalControlモードを選択しない限り、既存課題は従来通り動作します。

### Q5. 複数のCompartmentを同時に使えますか？

**A:** はい、ポート番号を変えることで可能です。詳細は [USAGE.md](./USAGE.md) 参照。

## トラブルシューティング

### API起動エラー

**症状:** `AddressAlreadyInUseException`

**対処:** Preferences.xmlでApiServerPortを変更（5001等）

### 接続エラー（Python側）

**症状:** `ConnectionError`

**対処:**
1. C# Compartmentソフトが起動しているか確認
2. URLが正しいか確認（`/api`が必要）
3. ファイアウォール設定確認

詳細は [USAGE.md - トラブルシューティング](./USAGE.md#トラブルシューティング) 参照

## 開発ロードマップ

### ✅ Phase 1-2: 基本機能（完了）
- OWIN APIサーバー
- 基本エンドポイント
- デバッグモード
- Python クライアント

### 🔄 Phase 3: GUI・ExternalControl（次）
- UserControlDebugPanel
- Preferences UI更新
- ExternalControlモード

### 🔄 Phase 4-5: 課題移行
- DelayMatch課題
- データ記録API
- 既存課題の順次移行

### 🔮 Phase 6: 中央オーケストレーター（将来）
- 複数部屋統合管理
- 条件付きシーケンス制御
- Webダッシュボード
- データ集約・分析

詳細は [IMPLEMENTATION_PLAN.md](./IMPLEMENTATION_PLAN.md) 参照

## 貢献・フィードバック

### バグ報告

Issues または直接連絡してください。

### 機能リクエスト

[NEXT_STEPS.md](./NEXT_STEPS.md) に追加してください。

## ライセンス

（ライセンス情報を記載）

## 作成者

- C#実装: （既存のCompartmentソフト開発者）
- PsychoPy統合: Claude Code (Anthropic)
- 実装日: 2026-01-29

## 参考資料

- [PsychoPy公式ドキュメント](https://www.psychopy.org/)
- [OWIN/Katana](https://github.com/aspnet/AspNetKatana/wiki)
- [ASP.NET Web API](https://learn.microsoft.com/aspnet/web-api/)

---

**次のステップ:** [NEXT_STEPS.md](./NEXT_STEPS.md) を確認してください。
