# 最終デバッグテスト手順

## 修正内容（確定）

1. ✅ 課題開始時にドアを開く処理を追加
2. ✅ 入室センサーシーケンス: ON → ON → OFF → OFF

---

## テスト手順

### Step 1: リビルド

```
Visual Studio メニューバー
→ ビルド → ソリューションのクリーン
→ ビルド → ソリューションのリビルド
```

**ビルド結果を確認**:
```
========== ビルド: 成功 1、失敗 0、最新の状態 0、スキップ 0 ==========
```

もしエラーがあれば、エラー内容を共有してください。

---

### Step 2: デバッグ実行

1. **F5 キーを押す**
2. エンジン選択画面で「**ブロックプログラミングエンジン**」を選択
3. **OK** をクリック

---

### Step 3: 出力ウィンドウを開く

```
Ctrl + Alt + O
```

---

### Step 4: FormScript タブで課題を確認

1. **FormScript タブ**を開く
2. ビジュアルプログラムが設定されているか確認
3. 「入室するだけで成功」のプログラムが表示されているか確認

---

### Step 5: Operation タブに切り替え

1. **Operation タブ**をクリック
2. **5秒間待つ**（アプリケーションが完全に起動するまで）

---

### Step 6: 起動ログを確認

出力ウィンドウで以下のログを探してください：

```
[OnOperationStateMachineProc] State=Idle, Command=None, IsBusy=False
[UcOperationInternal Idle()] Called. Command=None
```

**質問A: このログは表示されますか？**
- YES → 正常です、次に進んでください ✓
- NO → error.md.txt にログを保存して共有してください

---

### Step 7: Start ボタンをクリック

1. **Start ボタン**をクリック
2. **5秒間待つ**

---

### Step 8: Start後のログを確認

出力ウィンドウで以下のログを探してください：

```
=== START BUTTON CLICKED ===
Current State: Idle
Command set to: Start

[OnOperationStateMachineProc] State=Idle, Command=Start
[UcOperationInternal Idle()] Called. Command=Start
[UcOperationInternal Idle] Start command received. EnableDebugMode=True
[UcOperationInternal Idle] eDoor enabled
[UcOperationInternal Idle] Transitioned to PreEnterCageProc

[UcOperationInternal DeviceStandbyBegin] EnableDebugMode=True
[UcOperationInternal デバッグモード] デバイススタンバイをスキップ
[UcOperationInternal デバッグモード] デバイスを初期化（レバー、ランプ）
[UcOperationInternal デバッグモード] ドアを開きます

[IoMicrochipDummyEx] SetUpperStateOfDOut: DoorOpen
[IoMicrochipDummyEx] SimulateDoorOpen: DoorOpen=true, DoorClose=false

[UcOperationInternal デバッグモード] デバイススタンバイ完了
```

**質問B: これらのログは表示されますか？**
- YES → 完璧です！課題が開始されています ✓
- NO → どこまで表示されましたか？

---

### Step 9: デバッグコンソールで入室をシミュレート

画面右側の「デバッグコンソール」で、以下の順番でボタンをクリックしてください：

1. **「入室センサー ON」** をクリック
2. **「退出センサー ON」** をクリック
3. **「入室センサー OFF」** をクリック
4. **「退出センサー OFF」** をクリック

---

### Step 10: 入室検出のログを確認

出力ウィンドウで入室が検出されたか確認してください：

期待されるログ:
```
RoomState:RoomInOutEnterHead  （または類似のログ）
入室検知
課題成功
```

---

### Step 11: error.md.txt にログを保存

**全ての操作が完了したら**、出力ウィンドウの**全内容**を error.md.txt に保存してください。

---

## 重要な確認ポイント

### ✅ 成功の条件:

1. `[UcOperationInternal Idle] Start command received` が表示される
2. `[UcOperationInternal デバッグモード] ドアを開きます` が表示される
3. `[IoMicrochipDummyEx] SimulateDoorOpen` が表示される
4. 入室センサーをシミュレートすると、課題が成功する

### ❌ 失敗の場合:

どの時点でログが止まったか教えてください：
- Start ボタンをクリックしたが、何もログが出ない
- `[UcOperationInternal Idle]` のログが出ない
- ドアが開かない
- 入室センサーに反応しない

---

## トラブルシューティング

### 問題1: `[OnOperationStateMachineProc]` のログが出ない

→ backgroundWorker1 が動作していません。
→ ビルド構成が正しいか確認してください（DebugDummyIo）

### 問題2: `[UcOperationInternal Idle] Start command received` が出ない

→ Command が Start にならないか、State が Idle でない可能性があります。
→ 出力ウィンドウの全ログを共有してください。

### 問題3: ドアが開かない

→ `OpOpenDoor()` が実行されていないか、IoMicrochipDummyEx が使われていません。
→ ログで `[IoMicrochipDummyEx]` という文字列を検索してください。

---

## 次のステップ

テストを実行して、以下を報告してください：

1. **ビルドは成功しましたか？** (YES / NO / エラー内容)
2. **Step 6: 起動ログは表示されましたか？** (YES / NO)
3. **Step 8: Start後のログは表示されましたか？** (YES / NO / どこまで表示されたか)
4. **Step 10: 入室は検出されましたか？** (YES / NO)
5. **error.md.txt の内容**

これで問題を完全に診断できます！ 🚀
