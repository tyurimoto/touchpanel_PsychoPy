# 診断テスト手順

## 目的
なぜ課題が開始されないか診断します。

## 手順

### 1. ソリューションをクリーンしてリビルド
```
Visual Studio メニューバー → ビルド → ソリューションのクリーン
→ ビルド → ソリューションのリビルド
```

**ビルドが成功したことを確認してください。**

### 2. F5 でデバッグ実行

### 3. 出力ウィンドウを開く
```
Ctrl + Alt + O
```

### 4. Operation タブを開く

### 5. Start ボタンをクリック

### 6. 出力ウィンドウで以下のログを確認

#### 期待されるログ:

```
=== START BUTTON CLICKED ===
Current State: Idle
Current IsBusy: False
EnableDebugMode: True
backgroundWorker1.IsBusy: False
Starting backgroundWorker1...
Command set to: Start
IsBusy after setCommand: False (または True)

[backgroundWorker1_DoWork] Started!
[backgroundWorker1_DoWork] Using OLD engine (UcOperation)
[backgroundWorker1_DoWork] BG_WORKER is NOT DEFINED - background worker will do nothing!

[OnOperationStateMachineProc] In Idle state. Command=Start
[Idle] Start command received. EnableDebugMode=True
```

### 7. error.md.txt にログを保存

**重要**: Start ボタンをクリックしてから、少なくとも5秒間待ってから、出力ウィンドウの内容を error.md.txt に保存してください。

---

## 報告してください

以下の情報を教えてください：

1. **「BG_WORKER is NOT DEFINED」というログは表示されましたか？**
2. **「[OnOperationStateMachineProc] In Idle state」というログは表示されましたか？**
3. **「[Idle] Start command received」というログは表示されましたか？**
4. **error.md.txt の内容**

これらのログにより、問題の原因を特定できます。

---

## 期待される結果

- `BG_WORKER is NOT DEFINED` が表示される
  → backgroundWorker は使われていない、タイマーが使われる

- `[OnOperationStateMachineProc] In Idle state` が**大量に**表示される
  → タイマーが正常に動作している（1秒間に数回呼ばれる）

- `[Idle] Start command received` が表示される
  → 課題が開始された

もしこれらのログが表示されない場合、原因を特定できます。
