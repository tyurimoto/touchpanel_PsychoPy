# Debug Mode Fixes Summary

## 修正内容 (Changes Made)

### 1. Stop Button Infinite Loop Fix (停止ボタン無限ループ修正)

**File**: `UcOperation.cs`
**Location**: Line 1136-1167 (DeviceStandbyEnd state)

**Problem**:
- Stopコマンド実行時、DeviceStandbyBegin → DeviceStandbyEnd → LoadState() → Stop の無限ループが発生していた
- LoadState()がStop状態を復元してしまい、再度DeviceStandbyBeginに遷移するサイクルが繰り返されていた

**Solution**:
- DeviceStandbyEndでLoadState()を呼んだ後、状態がStopに戻った場合を検出
- その場合は停止処理を完了させる:
  - opCollection.IsBusy.Value = false に設定
  - opCollection.file.Close() を実行
  - Idle状態に遷移

**Code Added**:
```csharp
// Stop状態からDeviceStandbyに遷移した場合、LoadStateでStopに戻ってしまうので
// その場合はIsBusyをfalseにしてIdleに遷移する
if (opCollection.sequencer.State == OpCollection.Sequencer.EState.Stop)
{
    opCollection.callbackMessageNormal("停止完了");
    opCollection.file.Close();
    opCollection.IsBusy.Value = false;
    opCollection.sequencer.State = OpCollection.Sequencer.EState.Idle;
}
```

---

## テスト手順 (Testing Procedure)

### Test 1: Stop Button Loop Fix (停止ボタン無限ループ修正のテスト)

1. デバッグモードでアプリケーションを起動
2. Operationタブで「Start」ボタンを押す
3. デバッグコントロールパネルでドアやレバーを操作
4. 「Stop」ボタンを押す

**Expected Result (期待される結果)**:
- 停止処理が完了する
- "停止完了" メッセージが表示される
- "戻るボタン動作中" や "デバイススタンバイ開始" の無限ループが発生しない
- IsBusyがfalseになる

---

## 既知の問題 (Known Issues)

### Issue: Room Entry/Exit Sensor Infinite Loop (入室/退室センサー無限ループ)

**User Report**: "動物が入室した途端（入室センサーON OFF →退出センサー ON OFF）が起きると無限ループとなった"

**Possible Causes**:
1. eDoorが入室センサーに反応して自動でドアを閉じようとする
2. 不正退出(IllegalExit)検出ロジックが誤動作している
3. DevRoom状態機械がセンサー変化に対して予期しない動作をしている

**Recommended Investigation**:
- eDoor.cs の入室センサー検出ロジック (line 198-200)
- UcOperationInternal.cs の IllegalExit 検出ロジック
- DevRoom.cs のセンサー監視ロジック

**Potential Solution**:
- デバッグモード時は eDoor の自動制御を無効化する
- または、UserControlAnimalSimulatorで位置変更時により自然なセンサー遷移シーケンスを実装する

---

## その他の推奨事項 (Additional Recommendations)

### 1. eDoor Debug Mode Handling
デバッグモード時は eDoor の自動ドア制御を無効化することを検討:
- eDoor.Enable を false に設定
- ドア操作は UserControlDebugPanel から手動で制御

### 2. Animal Simulator Improvements
UserControlAnimalSimulator で位置変更時、より自然なセンサーシーケンスを実装:
- Outside → Entering: RoomEntrance ON
- Entering → Inside: RoomEntrance OFF → RoomStay ON (順次実行、間に遅延を入れる)
- Inside → Exiting: RoomStay OFF → RoomExit ON
- Exiting → Outside: RoomExit OFF

### 3. Sensor State Logging
デバッグ時にセンサー状態変化をログ出力して、無限ループの原因を特定しやすくする:
```csharp
public void SetManualSensorState(IoBoardDInLogicalName sensor, bool state)
{
    lock (sensorStateLock)
    {
        bool oldState = sensorStates.ContainsKey(sensor) ? sensorStates[sensor] : false;
        sensorStates[sensor] = state;
        if (oldState != state)
        {
            Debug.WriteLine($"[Sensor Change] {sensor}: {oldState} → {state}");
        }
    }
}
```

---

## ビルドとテストの注意事項 (Build and Test Notes)

- このプロジェクトは .NET Framework 4.8 が必要です
- macOS ではビルドできません（Windows専用）
- Visual Studio でビルドしてテストしてください
- デバッグモードを有効にするには、設定ファイルで `EnableDebugMode = true` にしてください

---

## 変更ファイル一覧 (Modified Files)

1. `UcOperation.cs` - DeviceStandbyEnd state での Stop ループ修正

---

## 質問・問題報告 (Questions/Issues)

変更後も問題が発生する場合は、以下の情報を提供してください:
1. エラーログまたは無限ループのログ
2. 再現手順
3. どのセンサー/ボタンを操作したか
