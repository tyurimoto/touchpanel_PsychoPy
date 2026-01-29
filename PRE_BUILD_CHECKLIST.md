# ビルド前チェックリスト

Windows環境でビルドする前に、このチェックリストを確認してください。

## ✅ 完了済み（macOSで実施）

### コード実装
- [x] C# APIサーバー実装（OWIN/Katana）
- [x] 基本APIコントローラー（Sensor, Door, Lever, Feed, RFID）
- [x] デバッグモードクラス（IoMicrochipDummyEx, IoHybridBoard, RFIDReaderDummy）
- [x] DebugController実装
- [x] HardwareService実装
- [x] FormMain統合
- [x] PreferencesDat拡張

### Python実装
- [x] compartment_hardware.py（APIクライアント）
- [x] debug_keyboard_control.py（デバッグツール）
- [x] training_task_example.py（訓練課題サンプル）
- [x] test_integration.py（統合テスト）

### ドキュメント
- [x] PROJECT_OVERVIEW.md
- [x] STATUS.md
- [x] USAGE.md
- [x] NEXT_STEPS.md
- [x] IMPLEMENTATION_PLAN.md
- [x] psychopy/README.md

### 事前修正（コンパイルエラー対策）
- [x] FormMain.cs - using Compartment.Controllers; 追加済み
- [x] HardwareService.cs - using System.Linq; 追加済み

### 設定ファイル
- [x] Preferences.template.xml（テンプレート）
- [x] Preferences.debug.xml（デバッグモード用）

### テストスクリプト
- [x] test_api.sh（macOS/Linux用）
- [x] test_api.bat（Windows用）
- [x] test_integration.py（Python統合テスト）

## 📦 Windows環境への転送

### 転送方法の選択

#### オプション1: GitHub（推奨）
```bash
# macOSで
cd /Users/terumi/Downloads/compartment
git init
git add .
git commit -m "PsychoPy integration implementation"
git remote add origin https://github.com/yourusername/compartment.git
git push -u origin main

# Windowsで
git clone https://github.com/yourusername/compartment.git
```

#### オプション2: USBメモリ
```bash
# macOSで
cp -r /Users/terumi/Downloads/compartment /Volumes/USB/

# Windowsで USBからコピー
```

#### オプション3: クラウドストレージ
- Google Drive / OneDrive / Dropbox にアップロード
- Windows でダウンロード

## 🔧 Windows環境でのセットアップ

### 1. 必要なソフトウェア

- [ ] Windows 10/11
- [ ] Visual Studio 2019 または 2022
  - [ ] ワークロード: `.NET デスクトップ開発`
  - [ ] ワークロード: `ASP.NET と Web 開発`
- [ ] Git（オプション、GitHub使用時）

### 2. プロジェクトを開く

- [ ] Visual Studio 2022 起動
- [ ] `cs/Compartment/Compartment.sln` を開く

### 3. NuGetパッケージ確認

#### 自動復元（推奨）
- [ ] ソリューションを開くと自動的に復元開始
- [ ] 完了まで待つ（1～2分）

#### 手動復元
- [ ] ソリューションエクスプローラー → ソリューション右クリック
- [ ] 「NuGetパッケージの復元」

#### 必要なパッケージ（自動でインストールされる）
- [ ] Microsoft.AspNet.WebApi.OwinSelfHost (5.2.9)
- [ ] Microsoft.Owin.Host.HttpListener (4.2.2)
- [ ] Microsoft.Owin.Hosting (4.2.2)
- [ ] Microsoft.Owin.Cors (4.2.2)
- [ ] Newtonsoft.Json (13.x)

### 4. ビルド前の最終確認

#### コードチェック
- [ ] ソリューションエクスプローラーで新規ファイルが表示されているか
  - [ ] Startup.cs
  - [ ] IoMicrochipDummyEx.cs
  - [ ] IoHybridBoard.cs
  - [ ] RFIDReaderDummy.cs
  - [ ] Controllers/ フォルダ
  - [ ] Services/ フォルダ
  - [ ] Models/ フォルダ

#### 設定確認
- [ ] ビルド構成: Debug または Release
- [ ] プラットフォーム: Any CPU

### 5. 初回ビルド

- [ ] `ビルド` → `ソリューションのビルド` (Ctrl+Shift+B)
- [ ] 出力ウィンドウで結果確認

#### 成功の場合
```
========== ビルド: 1 正常終了、0 失敗、0 スキップ ==========
```

#### エラーの場合
→ 「想定されるエラーと対処」セクション参照

## 🐛 想定されるエラーと対処

### エラー1: using が見つからない

```
エラー CS0246: 型または名前空間の名前 'DebugController' が見つかりませんでした
```

**確認:**
- [ ] FormMain.cs の先頭に `using Compartment.Controllers;` があるか

**対処:**
```csharp
// FormMain.cs の先頭に追加
using Compartment.Controllers;
```

### エラー2: System.Linq が見つからない

```
エラー CS1061: 'IEnumerable<T>' に 'OrderByDescending' の定義が含まれていません
```

**確認:**
- [ ] HardwareService.cs の先頭に `using System.Linq;` があるか

**対処:**
```csharp
// HardwareService.cs の先頭に追加
using System.Linq;
```

### エラー3: NuGetパッケージエラー

```
エラー: パッケージ 'Microsoft.Owin' を復元できません
```

**対処:**
1. パッケージマネージャーコンソールを開く
   - `ツール` → `NuGetパッケージマネージャー` → `パッケージマネージャーコンソール`
2. 以下を実行:
   ```
   Update-Package -reinstall
   ```

### エラー4: .NET Framework バージョン

```
エラー: プロジェクトは .NET Framework 4.8 を対象としています
```

**対処:**
1. プロジェクトを右クリック → `プロパティ`
2. `アプリケーション` タブ
3. `ターゲットフレームワーク` を `.NET Framework 4.8` に変更
4. Visual Studio 再起動

### エラー5: ファイルが見つからない

```
エラー: 'IoMicrochipDummyEx.cs' が見つかりません
```

**対処:**
1. ソリューションエクスプローラーで確認
2. ファイルが存在するか確認
3. 存在する場合: プロジェクトに追加
   - プロジェクト右クリック → `追加` → `既存の項目`
   - ファイルを選択

## ✅ ビルド成功後の確認

### 実行ファイル確認
- [ ] `cs/Compartment/Compartment/bin/Debug/Compartment.exe` が生成されている

### 設定ファイル配置
- [ ] `Preferences.debug.xml` を `Preferences.xml` にコピー
- [ ] `Compartment.exe` と同じフォルダに配置

### 初回起動
- [ ] Compartment.exe をダブルクリック
- [ ] FormMain が起動する
- [ ] デバッグ出力に以下が表示される:
  ```
  [Debug] Initialized IoMicrochipDummyEx (Full Dummy Mode)
  [Debug] Initialized RFIDReaderDummy
  [API] Web API server started at http://localhost:5000/
  ```

### APIテスト
- [ ] PowerShell または コマンドプロンプトで:
  ```
  curl http://localhost:5000/api/sensor/entrance
  ```
- [ ] または `test_api.bat` を実行

### Python統合テスト
- [ ] Python環境セットアップ:
  ```
  pip install psychopy requests
  ```
- [ ] 統合テスト実行:
  ```
  cd psychopy
  python test_integration.py
  ```

## 📋 次のステップ

ビルド成功後は **NEXT_STEPS.md** のタスク2以降を実施してください：

1. ✅ タスク1: C#ビルド（完了）
2. → タスク2: API起動テスト
3. → タスク3: デバッグモード確認
4. → タスク4: PsychoPyテスト

## 📞 サポート

問題が発生した場合:
1. エラーメッセージを確認
2. 「想定されるエラーと対処」を参照
3. USAGE.md のトラブルシューティングを参照
4. それでも解決しない場合は Issues を報告

---

**チェックリスト完了日:** _______________

**ビルド実施者:** _______________

**備考:**
