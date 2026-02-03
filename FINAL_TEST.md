# 最終テスト手順

## 目的
新エンジン (UcOperationInternal.cs) のデバッグモード対応を確認

## 手順

### 1. ソリューションをクリーンしてリビルド
```
Visual Studio メニューバー → ビルド → ソリューションのクリーン
→ ビルド → ソリューションのリビルド
```

**ビルドが成功したことを必ず確認してください。**

### 2. F5 でデバッグ実行

### 3. 出力ウィンドウを開く
```
Ctrl + Alt + O
```

### 4. エンジン選択画面で「ブロックプログラミングエンジン」を選択
（デフォルトで選択されているはずです）

### 5. Operation タブを開く

### 6. Start ボタンをクリック

### 7. 出力ウィンドウで期待されるログを確認

#### ✅ 期待されるログ:

```
=== START BUTTON CLICKED ===
Current State: Idle
Current IsBusy: False
EnableDebugMode: True
backgroundWorker1.IsBusy: True
backgroundWorker1 is already running!
Command set to: Start
IsBusy after setCommand: False

[backgroundWorker1_DoWork] Started!
[backgroundWorker1_DoWork] Using NEW engine (UcOperationInternal)
[backgroundWorker1_DoWork] BG_WORKER is DEFINED - using background worker loop

[UcOperationInternal Idle] Start command received. EnableDebugMode=True
[UcOperationInternal Idle] eDoor enabled
[UcOperationInternal Idle] File opened: (ファイルパス) または File open error
[UcOperationInternal Idle] Transitioned to PreEnterCageProc

[UcOperationInternal DeviceStandbyBegin] EnableDebugMode=True
[UcOperationInternal デバッグモード] デバイススタンバイをスキップ
[UcOperationInternal デバッグモード] センサー状態を初期化: LeverIn=true, DoorOpen=true
[UcOperationInternal デバッグモード] DeviceStandbyEnd に遷移

[UcOperationInternal デバッグモード] デバイススタンバイ完了
```

### 8. eDoor の動作を確認

**期待される動作:**
- ドアが自動で開く
- センサー状態: DoorOpen=true, DoorClose=false
- ドアが閉じる
- 課題が開始される

### 9. 入室センサー・退出センサーをON/OFFして課題が進むか確認

### 10. error.md.txt にログを保存

**重要**: 全ての動作を確認してから、出力ウィンドウの全内容を error.md.txt に保存してください。

---

## 期待される結果

1. ✅ **Start ボタンをクリックすると課題が開始される**
2. ✅ **eDoor が自動で開く（ログに表示される）**
3. ✅ **センサー状態が正しく更新される**
4. ✅ **入室センサー・退出センサーに反応する**
5. ✅ **Stop ボタンで正常に停止できる**

---

## もし問題が発生したら

以下の情報を教えてください：
1. どの時点で問題が発生したか
2. 出力ウィンドウのログ (error.md.txt)
3. 画面の状態（スクリーンショットがあれば最適）

これで、ブロックプログラミングモードでデバッグモードが動作するはずです！ 🚀
