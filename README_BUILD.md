# ビルド準備完了 - Windows環境での次のステップ

## 🎉 macOSでの準備が完了しました！

すべてのコード実装、ドキュメント作成、テストスクリプト準備が完了しています。
Windows環境でビルドする準備が整いました。

## 📦 プロジェクトをWindowsに転送

### 推奨方法: GitHub

```bash
# 1. macOSでGitリポジトリ作成
cd /Users/terumi/Downloads/compartment
git init
git add .
git commit -m "Compartment PsychoPy integration - ready for build"

# 2. GitHubにプッシュ（リポジトリは事前作成）
git remote add origin https://github.com/yourusername/compartment.git
git push -u origin main

# 3. Windowsでクローン
git clone https://github.com/yourusername/compartment.git
cd compartment
```

### 代替方法: USBメモリ、クラウドストレージ

詳細は **PRE_BUILD_CHECKLIST.md** 参照

## 🚀 Windows環境での作業手順（3ステップ）

### ステップ1: Visual Studio でビルド（30分）

```
1. Visual Studio 2022 を起動
2. cs/Compartment/Compartment.sln を開く
3. NuGetパッケージが自動復元されるまで待つ
4. ビルド → ソリューションのビルド (Ctrl+Shift+B)
```

**期待される結果:**
```
========== ビルド: 1 正常終了、0 失敗、0 スキップ ==========
```

エラーが出た場合 → **PRE_BUILD_CHECKLIST.md** の「想定されるエラーと対処」参照

### ステップ2: デバッグモードで起動（10分）

```
1. Preferences.debug.xml を Preferences.xml にコピー
2. Compartment.exe と同じフォルダに配置
3. Compartment.exe を実行
```

**期待される出力:**
```
[Debug] Initialized IoMicrochipDummyEx (Full Dummy Mode)
[Debug] Initialized RFIDReaderDummy
[API] Web API server started at http://localhost:5000/
```

### ステップ3: APIテスト（10分）

```
# PowerShellで
cd compartment
.\test_api.bat

# または手動で
curl http://localhost:5000/api/sensor/entrance
```

**期待されるレスポンス:**
```json
{
  "roomId": "1",
  "active": false,
  "timestamp": "2026-01-29T12:34:56Z"
}
```

## 📋 完了済みの作業（macOSで実施）

### ✅ C#実装
- OWIN/Katana APIサーバー
- 基本APIコントローラー（Sensor, Door, Lever, Feed, RFID）
- デバッグモードクラス（IoMicrochipDummyEx, IoHybridBoard, RFIDReaderDummy）
- DebugController（課題状態取得API含む）
- HardwareService（FormMain連携）
- PreferencesDat拡張（デバッグモード設定）

### ✅ Python実装
- compartment_hardware.py（APIクライアントライブラリ）
- debug_keyboard_control.py（デバッグツール）
- training_task_example.py（訓練課題サンプル）
- test_integration.py（統合テスト）

### ✅ ドキュメント
- PROJECT_OVERVIEW.md（プロジェクト概要）
- STATUS.md（実装状況）
- USAGE.md（操作マニュアル）
- NEXT_STEPS.md（次のステップ）
- IMPLEMENTATION_PLAN.md（実装計画詳細）
- PRE_BUILD_CHECKLIST.md（ビルド前チェックリスト）
- psychopy/README.md（PsychoPy使い方）

### ✅ 設定・テスト
- Preferences.template.xml（設定テンプレート）
- Preferences.debug.xml（デバッグモード用設定）
- test_api.sh / test_api.bat（APIテストスクリプト）
- test_integration.py（Python統合テスト）

### ✅ コード修正
- FormMain.cs: using Compartment.Controllers; 追加済み
- HardwareService.cs: using System.Linq; 追加済み

## 📚 重要なドキュメント

| ファイル | 用途 | いつ読む |
|---------|------|---------|
| **PRE_BUILD_CHECKLIST.md** | ビルド前の確認事項 | **最初に読む** |
| NEXT_STEPS.md | ビルド後の作業手順 | ビルド成功後 |
| USAGE.md | 使い方・トラブル対処 | 動作確認時 |
| PROJECT_OVERVIEW.md | プロジェクト概要 | 全体像把握 |

## ⚠️ よくある質問

### Q1. ビルドエラーが出たら？

**A:** **PRE_BUILD_CHECKLIST.md** の「想定されるエラーと対処」を確認してください。
よくあるエラー:
- using ディレクティブ不足 → 追加済みですが、念のため確認
- NuGetパッケージ未復元 → 自動復元を待つ
- .NET Framework バージョン → 4.8に設定

### Q2. macOSでビルドできないの？

**A:** Windows Forms は macOS 非対応のため、Windows環境が必須です。
ただし、Python側（PsychoPy課題）は macOS でも動作します。

### Q3. どのくらい時間がかかる？

**A:** Windows環境で初回ビルド～動作確認まで約1時間：
- ビルド: 30分
- デバッグモード起動確認: 10分
- APIテスト: 10分
- PsychoPy統合テスト: 10分

### Q4. 次のステップは？

**A:** ビルド成功後は **NEXT_STEPS.md** のタスク2以降を実施：
1. ✅ タスク1: C#ビルド
2. → タスク2: API起動テスト
3. → タスク3: デバッグモード確認
4. → タスク4: PsychoPyテスト
5. → タスク5: UserControlDebugPanel作成（GUI）

## 🎯 成功の指標

以下が確認できれば成功です：

### ビルド成功
- [ ] エラー 0件でビルド完了
- [ ] Compartment.exe が生成される

### 起動成功
- [ ] Compartment.exe が起動する
- [ ] API起動メッセージが表示される

### API動作確認
- [ ] curl で APIにアクセスできる
- [ ] JSONレスポンスが返る

### デバッグモード確認
- [ ] センサー状態設定APIが動作する
- [ ] RFID設定APIが動作する
- [ ] 課題状態取得APIが動作する

### Python統合
- [ ] test_integration.py が成功する
- [ ] debug_keyboard_control.py が動作する
- [ ] training_task_example.py が実行できる

## 📞 問題が発生したら

1. **PRE_BUILD_CHECKLIST.md** を確認
2. **USAGE.md** のトラブルシューティングを確認
3. エラーメッセージで検索
4. それでも解決しない場合は Issues 報告

## 🚀 準備完了！

すべての準備が整いました。Windows環境で以下を実行してください：

```
1. プロジェクトをWindowsに転送
2. PRE_BUILD_CHECKLIST.md を確認しながらビルド
3. NEXT_STEPS.md に従って動作確認
```

頑張ってください！🎉
