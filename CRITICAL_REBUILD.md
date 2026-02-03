# 【緊急】リビルド手順

## 問題
古いビルドが実行されています。最新の修正が反映されていません。

---

## 必須手順

### Step 1: Visual Studio でソリューションを開く

`Compartment.sln` をダブルクリックして Visual Studio で開いてください。

---

### Step 2: 出力ウィンドウを開く

```
メニューバー → 表示 → 出力
または Ctrl + Alt + O
```

**出力ウィンドウを常に表示したままにしてください。**

---

### Step 3: ソリューションをクリーン

```
メニューバー → ビルド → ソリューションのクリーン
```

出力ウィンドウで以下が表示されるまで待ってください：
```
========== クリーン: 成功 1、失敗 0、スキップ 0 ==========
```

---

### Step 4: ソリューションをリビルド

```
メニューバー → ビルド → ソリューションのリビルド
```

**出力ウィンドウを注意深く見てください。**

---

### Step 5: ビルド結果を確認

#### ✅ 成功の場合:

```
========== ビルド: 成功 1、失敗 0、最新の状態 0、スキップ 0 ==========
========== 経過時間 00:00:XX.XX ==========
```

**「成功 1」と表示されていることを確認してください。**

→ Step 6 に進んでください。

---

#### ❌ 失敗の場合:

```
========== ビルド: 成功 0、失敗 1、最新の状態 0、スキップ 0 ==========
```

エラー一覧ウィンドウを開いてください：
```
メニューバー → 表示 → エラー一覧
または Ctrl + \, E
```

**エラーの内容をすべて error.md.txt にコピーしてください。**

よくあるエラー：
1. `debugLogCounter` が宣言されていない
2. 構文エラー
3. セミコロンが無い

→ エラー内容を共有してください。

---

### Step 6: F5 でデバッグ実行

リビルドが成功したら：

1. **F5 キーを押す**
2. ブロックプログラミングエンジンを選択
3. OK をクリック

---

### Step 7: BUILD VERIFICATION を確認

出力ウィンドウで以下のログを探してください：

```
=================================================
[BUILD VERIFICATION] FormMain_Load - ビルド日時: 2026-02-04 XX:XX:XX
=================================================
```

**ビルド日時が最新（今の時刻）になっていることを確認してください。**

もし古い日時（23:19:25）のままなら、リビルドが失敗しています。

---

### Step 8: 診断ログを確認

**Start ボタンをクリックせずに**、5秒間待ってから出力ウィンドウを確認してください。

以下のログが **1秒ごとに** 表示されるはずです：

```
[OnOperationStateMachineProc] State=Idle, Command=None, IsBusy=False
[UcOperationInternal Idle()] Called. Command=None
```

**質問A: このログは表示されますか？**
- YES → 正常です！Start ボタンをクリックしてください
- NO → error.md.txt に出力ウィンドウの内容を保存して共有してください

---

### Step 9: Start ボタンをクリック

1. Operation タブで **Start ボタン**をクリック
2. **5秒間待つ**
3. 出力ウィンドウを確認

以下のログが表示されるはずです：

```
=== START BUTTON CLICKED ===
[OnOperationStateMachineProc] State=Idle, Command=Start
[UcOperationInternal Idle()] Called. Command=Start
[UcOperationInternal Idle] Start command received. EnableDebugMode=True
[UcOperationInternal Idle] eDoor enabled
[UcOperationInternal DeviceStandbyBegin] EnableDebugMode=True
[UcOperationInternal デバッグモード] ドアを開きます
[IoMicrochipDummyEx] SimulateDoorOpen: DoorOpen=true, DoorClose=false
```

**質問B: これらのログは表示されますか？**
- YES → 完璧です！課題が開始されています
- NO → error.md.txt に出力ウィンドウの内容を保存して共有してください

---

## 報告してください

以下の情報を教えてください：

1. **Step 4: リビルドは成功しましたか？** (成功 / 失敗 / エラー内容)
2. **Step 7: BUILD VERIFICATION の日時は？** (例: 2026-02-04 XX:XX:XX)
3. **Step 8: 診断ログは表示されますか？** (YES / NO)
4. **Step 9: Start 後のログは表示されますか？** (YES / NO / どこまで表示されたか)
5. **error.md.txt の最新の内容**

これで問題を解決できます！
