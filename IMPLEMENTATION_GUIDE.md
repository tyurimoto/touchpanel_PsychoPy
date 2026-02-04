# PsychoPy統合API実装ガイド（段階的実装版）

**最終更新**: 2026-02-04
**目的**: 既存C#ハードウェア制御システムにAPIサーバーを統合し、PsychoPyから安全に制御できるようにする

---

## 📋 実装の全体フロー

### **Phase 1: 基盤修正（Critical） ← まずここから**
- 既存コードの矛盾修正（9個）
- ExternalControlモード実装（3個）
- API安全チェック追加（2個）
- **ビルド・テストポイント**: Phase 1完了後、APIサーバーが起動することを確認

### **Phase 2: コア機能実装（High）**
- 新規コントローラー作成（3個）
- HardwareServiceメソッド追加（5個）
- **ビルド・テストポイント**: Phase 2完了後、Postmanで各APIをテスト

### **Phase 3: 統合・完成（Medium）**
- モデルクラス作成（2個）
- FormMain統合（1個）
- csproj登録（1個）
- **ビルド・テストポイント**: Phase 3完了後、PsychoPyから実際に制御テスト

---

## 🔧 Phase 1: 基盤修正（Critical）

### ✅ 完了済み（3個）
1. ✅ FormMain.csのusingディレクティブ解除
2. ✅ _hardwareServiceフィールド解除
3. ✅ UcMain.csにrfidReaderHelperフィールド追加

---

### 📝 タスク4: RFIDReaderHelper.csにRFIDプロパティ追加

**ファイル**: `/Users/terumi/Downloads/compartment/cs/Compartment/Compartment/RFIDReaderHelper.cs`

**追加場所**: クラス内（CurrentIDCodeフィールドの近く、推定行50-100）

**追加コード**:
```csharp
/// <summary>
/// RFID値のプロパティ（API互換用）
/// </summary>
public string RFID
{
    get => CurrentIDCode?.Value ?? string.Empty;
    set
    {
        if (CurrentIDCode != null)
            CurrentIDCode.Value = value;
    }
}
```

**確認方法**:
```csharp
// CurrentIDCodeフィールドの定義を探す
public SyncObject<string> CurrentIDCode;

// その近くに上記プロパティを追加
```

---

### 📝 タスク5: RFIDReaderDummy.csにRFIDプロパティ追加

**ファイル**: `/Users/terumi/Downloads/compartment/cs/Compartment/Compartment/RFIDReaderDummy.cs`

**追加場所**: クラス内（CurrentIDCodeフィールドの近く）

**追加コード**:
```csharp
/// <summary>
/// RFID値のプロパティ（API互換用）
/// </summary>
public string RFID
{
    get => CurrentIDCode?.Value ?? string.Empty;
    set
    {
        if (CurrentIDCode != null)
            CurrentIDCode.Value = value;
    }
}
```

---

### 📝 タスク6: HardwareService.csのSetSensorState()修正

**ファイル**: `/Users/terumi/Downloads/compartment/cs/Compartment/Compartment/Services/HardwareService.cs`

**変更箇所**: 行350

**変更前**:
```csharp
dummyBoard.SetSensorState(sensor, state);
```

**変更後**:
```csharp
dummyBoard.SetManualSensorState(sensor, state);
```

---

### 📝 タスク7: HardwareService.csのCompartmentNo型変換修正

**ファイル**: `Services/HardwareService.cs`

**変更箇所**: 行25

**変更前**:
```csharp
return _formMain.preferencesDatOriginal?.CompartmentNo ?? "room1";
```

**変更後**:
```csharp
return _formMain.preferencesDatOriginal?.CompartmentNo.ToString() ?? "0";
```

---

### 📝 タスク8: IoMicrochipDummyEx.csにResetAllStates()追加

**ファイル**: `/Users/terumi/Downloads/compartment/cs/Compartment/Compartment/IoMicrochipDummyEx.cs`

**追加場所**: クラス内の最後（他のpublicメソッドの後）

**追加コード**:
```csharp
/// <summary>
/// 全センサー状態をデフォルトにリセット
/// </summary>
public void ResetAllStates()
{
    lock (sensorStateLock)
    {
        // 全センサーをfalseにリセット
        foreach (IoBoardDInLogicalName sensor in Enum.GetValues(typeof(IoBoardDInLogicalName)))
        {
            if (sensor == IoBoardDInLogicalName.RangeOver) continue;
            sensorStates[sensor] = false;
        }

        // 初期状態: ドアは閉じている、レバーは引っ込んでいる
        sensorStates[IoBoardDInLogicalName.DoorClose] = true;
        sensorStates[IoBoardDInLogicalName.LeverIn] = true;
    }
}
```

---

### 📝 タスク9: RFIDReaderDummy.csにSetRandomRFID()追加

**ファイル**: `/Users/terumi/Downloads/compartment/cs/Compartment/Compartment/RFIDReaderDummy.cs`

**追加場所**: クラス内の最後（SetRFIDメソッドの後）

**追加コード**:
```csharp
/// <summary>
/// ランダムなRFID値を生成して設定
/// </summary>
/// <returns>生成されたRFID値</returns>
public string SetRandomRFID()
{
    var random = new Random();
    string randomRfid = "";
    for (int i = 0; i < 16; i++)
    {
        randomRfid += random.Next(0, 10).ToString();
    }
    SetRFID(randomRfid);
    return randomRfid;
}
```

---

### 📝 タスク10: PreferencesDat.csにExternalControl列挙型追加

**ファイル**: `/Users/terumi/Downloads/compartment/cs/Compartment/Compartment/PreferencesDat.cs`

**変更箇所**: 行63-70（ECpTask列挙型）

**変更前**:
```csharp
public enum ECpTask : int
{
    Training = 0,
    DelayMatch = 1,
    None = 2,
    TrainingEasy = 3,
    UnConditionalFeeding = 4
}
```

**変更後**:
```csharp
public enum ECpTask : int
{
    Training = 0,
    DelayMatch = 1,
    None = 2,
    TrainingEasy = 3,
    UnConditionalFeeding = 4,
    ExternalControl = 5  // ← 追加（PsychoPy制御モード）
}
```

---

### 📝 タスク11: UcOperationInternal.csでステートマシン無効化

**ファイル**: `/Users/terumi/Downloads/compartment/cs/Compartment/Compartment/UcOperationInternal.cs`

**変更箇所**: `OnOperationStateMachineProc()` メソッドの先頭

**探し方**:
```csharp
// この関数を探す
private void OnOperationStateMachineProc()
{
    // ここに追加
}
```

**追加コード**:
```csharp
private void OnOperationStateMachineProc()
{
    // ExternalControlモード時はステートマシンを停止
    if (PreferencesDatOriginal.OpeTypeOfTask == ECpTask.ExternalControl)
    {
        return;
    }

    // 既存のステートマシン処理
    // ...（元のコードはそのまま）
}
```

---

### 📝 タスク12: UcOperation.csでeDoor無効化

**ファイル**: `/Users/terumi/Downloads/compartment/cs/Compartment/Compartment/UcOperation.cs`

**変更箇所**: eDoor初期化箇所（FormMain_Load内、推定行500-600）

**探し方**:
```csharp
// eDoor = new EDoor(...) の近くを探す
```

**追加コード**:
```csharp
// eDoor初期化の後に追加
if (eDoor != null)
{
    // ExternalControlモード時はeDoorを無効化
    if (preferencesDatOriginal.OpeTypeOfTask == ECpTask.ExternalControl)
    {
        eDoor.Enable = false;
        Debug.WriteLine("[ExternalControl] eDoor disabled for PsychoPy control");
    }
    // デバッグモード時もeDoorを無効化（センサー互換性のため）
    else if (preferencesDatOriginal.EnableDebugMode)
    {
        eDoor.Enable = false;
        Debug.WriteLine("[Debug Mode] eDoor disabled (not compatible with dummy board)");
    }
    else
    {
        // 実機モード＋通常タスクの場合のみ有効化
        eDoor.Enable = true;
    }
}
```

---

### 📝 タスク13: HardwareService.OpenDoorAsync()に安全チェック追加

**ファイル**: `Services/HardwareService.cs`

**変更箇所**: 行123-140（OpenDoorAsyncメソッド全体を置き換え）

**変更後の完全なメソッド**:
```csharp
/// <summary>
/// ドアを開く（安全チェック付き）
/// </summary>
public Task<bool> OpenDoorAsync()
{
    var tcs = new TaskCompletionSource<bool>();
    _formMain.Invoke((MethodInvoker)(() =>
    {
        try
        {
            // 安全チェック1: 在室センサー確認（デバッグモード時は無視）
            if (!IsDebugModeEnabled())
            {
                bool isInside = false;
                _formMain.ioBoardDevice?.GetUpperStateOfSaveDIn(
                    IoBoardDInLogicalName.RoomStay, out isInside);

                if (isInside)
                {
                    System.Diagnostics.Debug.WriteLine("[Safety] Cannot open door: animal inside");
                    tcs.SetResult(false);
                    return;
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("[Debug Mode] Safety check skipped");
            }

            // 安全チェック2: ドアが既に開いているか確認
            bool doorOpen = false;
            _formMain.ioBoardDevice?.GetUpperStateOfSaveDIn(
                IoBoardDInLogicalName.DoorOpen, out doorOpen);

            if (doorOpen)
            {
                System.Diagnostics.Debug.WriteLine("[Info] Door already open");
                tcs.SetResult(true);
                return;
            }

            // ドア開コマンド送信
            var cmdPkt = new DevCmdPkt { DevCmdVal = EDevCmd.DoorOpen };
            _formMain.concurrentQueueDevCmdPktDoor?.Enqueue(cmdPkt);
            tcs.SetResult(true);
        }
        catch (Exception ex)
        {
            tcs.SetException(ex);
        }
    }));
    return tcs.Task;
}
```

---

### 📝 タスク14: HardwareService.CloseDoorAsync()に安全チェック追加

**ファイル**: `Services/HardwareService.cs`

**変更箇所**: 行145-162（CloseDoorAsyncメソッド全体を置き換え）

**変更後の完全なメソッド**:
```csharp
/// <summary>
/// ドアを閉じる（安全チェック付き）
/// </summary>
public Task<bool> CloseDoorAsync()
{
    var tcs = new TaskCompletionSource<bool>();
    _formMain.Invoke((MethodInvoker)(() =>
    {
        try
        {
            // 安全チェック: ドアが既に閉じているか確認
            bool doorClose = false;
            _formMain.ioBoardDevice?.GetUpperStateOfSaveDIn(
                IoBoardDInLogicalName.DoorClose, out doorClose);

            if (doorClose)
            {
                System.Diagnostics.Debug.WriteLine("[Info] Door already closed");
                tcs.SetResult(true);
                return;
            }

            // ドア閉コマンド送信
            var cmdPkt = new DevCmdPkt { DevCmdVal = EDevCmd.DoorClose };
            _formMain.concurrentQueueDevCmdPktDoor?.Enqueue(cmdPkt);
            tcs.SetResult(true);
        }
        catch (Exception ex)
        {
            tcs.SetException(ex);
        }
    }));
    return tcs.Task;
}
```

---

## 🧪 Phase 1 完了後のビルド・テスト手順

### ビルド手順
1. Visual Studio でソリューションを開く
2. `ビルド` → `ソリューションのリビルド`
3. エラーがないことを確認

### エラーが出た場合の確認項目
- [ ] using Compartment.Services; が追加されているか
- [ ] using Compartment.Controllers; が追加されているか
- [ ] ECpTask.ExternalControl が定義されているか
- [ ] RFIDプロパティが正しく追加されているか

### 動作確認
1. アプリケーションを起動
2. Preferences画面で `OpeTypeOfTask` に `ExternalControl` が選択肢に表示されるか確認
3. デバッグモードチェックボックスが動作するか確認

**Phase 1完了の目安**: コンパイルエラーなし、アプリケーション起動可能

---

## 🚀 Phase 2: コア機能実装（High）

Phase 1が完了してから実施してください。

### 📝 タスク15: RoomController.cs作成

**新規ファイル**: `Controllers/RoomController.cs`

**完全なコード**:
```csharp
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Compartment.Services;

namespace Compartment.Controllers
{
    [RoutePrefix("api/room")]
    public class RoomController : ApiController
    {
        private static HardwareService _hardwareService;

        public static void Initialize(HardwareService hardwareService)
        {
            _hardwareService = hardwareService;
        }

        /// <summary>
        /// GET api/room/status
        /// ケージの状態を取得
        /// </summary>
        [HttpGet]
        [Route("status")]
        public IHttpActionResult GetStatus()
        {
            if (_hardwareService == null)
                return BadRequest("Hardware service not initialized");

            var status = _hardwareService.GetRoomStatus();
            return Ok(status);
        }

        /// <summary>
        /// GET api/room/wait-entry?timeout=60000
        /// 入室を待つ（ブロッキング）
        /// </summary>
        [HttpGet]
        [Route("wait-entry")]
        public async Task<IHttpActionResult> WaitEntry(int timeout = 60000)
        {
            if (_hardwareService == null)
                return BadRequest("Hardware service not initialized");

            bool success = await _hardwareService.WaitForEntryAsync(timeout);

            return Ok(new
            {
                roomId = _hardwareService.GetCompartmentNo(),
                success = success,
                message = success ? "Animal entered" : "Timeout",
                timestamp = DateTime.Now
            });
        }

        /// <summary>
        /// GET api/room/wait-exit?timeout=60000
        /// 退室を待つ（ブロッキング）
        /// </summary>
        [HttpGet]
        [Route("wait-exit")]
        public async Task<IHttpActionResult> WaitExit(int timeout = 60000)
        {
            if (_hardwareService == null)
                return BadRequest("Hardware service not initialized");

            bool success = await _hardwareService.WaitForExitAsync(timeout);

            return Ok(new
            {
                roomId = _hardwareService.GetCompartmentNo(),
                success = success,
                message = success ? "Animal exited" : "Timeout",
                timestamp = DateTime.Now
            });
        }
    }
}
```

---

### 📝 タスク16: LampController.cs作成

**新規ファイル**: `Controllers/LampController.cs`

**完全なコード**:
```csharp
using System.Threading.Tasks;
using System.Web.Http;
using Compartment.Models;
using Compartment.Services;

namespace Compartment.Controllers
{
    [RoutePrefix("api/lamp")]
    public class LampController : ApiController
    {
        private static HardwareService _hardwareService;

        public static void Initialize(HardwareService hardwareService)
        {
            _hardwareService = hardwareService;
        }

        /// <summary>
        /// POST api/lamp/room
        /// ルームランプON/OFF
        /// </summary>
        [HttpPost]
        [Route("room")]
        public async Task<IHttpActionResult> RoomLamp([FromBody] LampRequest request)
        {
            if (_hardwareService == null)
                return BadRequest("Hardware service not initialized");

            if (request == null)
                return BadRequest("Invalid request");

            bool success = await _hardwareService.SetRoomLampAsync(request.On);

            return Ok(new DeviceCommandResponse
            {
                RoomId = _hardwareService.GetCompartmentNo(),
                Success = success,
                State = request.On ? "on" : "off"
            });
        }

        /// <summary>
        /// POST api/lamp/lever
        /// レバーランプON/OFF
        /// </summary>
        [HttpPost]
        [Route("lever")]
        public async Task<IHttpActionResult> LeverLamp([FromBody] LampRequest request)
        {
            if (_hardwareService == null)
                return BadRequest("Hardware service not initialized");

            if (request == null)
                return BadRequest("Invalid request");

            bool success = await _hardwareService.SetLeverLampAsync(request.On);

            return Ok(new DeviceCommandResponse
            {
                RoomId = _hardwareService.GetCompartmentNo(),
                Success = success,
                State = request.On ? "on" : "off"
            });
        }

        /// <summary>
        /// POST api/lamp/feed
        /// 給餌ランプON/OFF
        /// </summary>
        [HttpPost]
        [Route("feed")]
        public async Task<IHttpActionResult> FeedLamp([FromBody] LampRequest request)
        {
            if (_hardwareService == null)
                return BadRequest("Hardware service not initialized");

            if (request == null)
                return BadRequest("Invalid request");

            bool success = await _hardwareService.SetFeedLampAsync(request.On);

            return Ok(new DeviceCommandResponse
            {
                RoomId = _hardwareService.GetCompartmentNo(),
                Success = success,
                State = request.On ? "on" : "off"
            });
        }
    }
}
```

---

### 📝 タスク17: SoundController.cs作成

**新規ファイル**: `Controllers/SoundController.cs`

**完全なコード**:
```csharp
using System.Threading.Tasks;
using System.Web.Http;
using Compartment.Models;
using Compartment.Services;

namespace Compartment.Controllers
{
    [RoutePrefix("api/sound")]
    public class SoundController : ApiController
    {
        private static HardwareService _hardwareService;

        public static void Initialize(HardwareService hardwareService)
        {
            _hardwareService = hardwareService;
        }

        /// <summary>
        /// POST api/sound/play
        /// 音声ファイルを再生
        /// </summary>
        [HttpPost]
        [Route("play")]
        public async Task<IHttpActionResult> Play([FromBody] SoundRequest request)
        {
            if (_hardwareService == null)
                return BadRequest("Hardware service not initialized");

            if (request == null || string.IsNullOrEmpty(request.File))
                return BadRequest("Invalid request: file path required");

            bool success = await _hardwareService.PlaySoundAsync(request.File, request.DurationMs);

            return Ok(new
            {
                roomId = _hardwareService.GetCompartmentNo(),
                success = success,
                file = request.File,
                durationMs = request.DurationMs,
                timestamp = System.DateTime.Now
            });
        }
    }
}
```

---

### 📝 タスク18-22: HardwareServiceに大量のメソッド追加

**ファイル**: `Services/HardwareService.cs`

**追加場所**: クラスの最後（行663の前）

**追加する全メソッド（約400行）**:

```csharp
        // ===== 以下を HardwareService クラスの最後に追加 =====

        /// <summary>
        /// 入室を待つ（OpFlagRoomInを監視）
        /// </summary>
        public Task<bool> WaitForEntryAsync(int timeoutMs)
        {
            var tcs = new TaskCompletionSource<bool>();
            Task.Run(() =>
            {
                var sw = System.Diagnostics.Stopwatch.StartNew();
                while (sw.ElapsedMilliseconds < timeoutMs)
                {
                    bool entered = false;
                    _formMain.Invoke((MethodInvoker)(() =>
                    {
                        entered = _formMain.OpFlagRoomIn;
                        if (entered)
                        {
                            _formMain.OpFlagRoomIn = false; // フラグクリア
                        }
                    }));

                    if (entered)
                    {
                        tcs.SetResult(true);
                        return;
                    }

                    System.Threading.Thread.Sleep(10);
                }
                tcs.SetResult(false);
            });
            return tcs.Task;
        }

        /// <summary>
        /// 退室を待つ（OpFlagRoomOutを監視）
        /// </summary>
        public Task<bool> WaitForExitAsync(int timeoutMs)
        {
            var tcs = new TaskCompletionSource<bool>();
            Task.Run(() =>
            {
                var sw = System.Diagnostics.Stopwatch.StartNew();
                while (sw.ElapsedMilliseconds < timeoutMs)
                {
                    bool exited = false;
                    _formMain.Invoke((MethodInvoker)(() =>
                    {
                        exited = _formMain.OpFlagRoomOut;
                        if (exited)
                        {
                            _formMain.OpFlagRoomOut = false; // フラグクリア
                        }
                    }));

                    if (exited)
                    {
                        tcs.SetResult(true);
                        return;
                    }

                    System.Threading.Thread.Sleep(10);
                }
                tcs.SetResult(false);
            });
            return tcs.Task;
        }

        /// <summary>
        /// RFID読み取りを待つ
        /// </summary>
        public Task<string> WaitForRFIDAsync(int timeoutMs)
        {
            var tcs = new TaskCompletionSource<string>();
            Task.Run(() =>
            {
                var sw = System.Diagnostics.Stopwatch.StartNew();
                while (sw.ElapsedMilliseconds < timeoutMs)
                {
                    string rfid = "";
                    _formMain.Invoke((MethodInvoker)(() =>
                    {
                        rfid = _formMain.rfidReaderHelper?.RFID ?? string.Empty;
                    }));

                    if (!string.IsNullOrEmpty(rfid))
                    {
                        tcs.SetResult(rfid);
                        return;
                    }

                    System.Threading.Thread.Sleep(10);
                }
                tcs.SetResult(string.Empty);
            });
            return tcs.Task;
        }

        /// <summary>
        /// ケージの状態を取得
        /// </summary>
        public object GetRoomStatus()
        {
            object status = null;
            _formMain.Invoke((MethodInvoker)(() =>
            {
                bool isInside = false;
                _formMain.ioBoardDevice?.GetUpperStateOfSaveDIn(
                    IoBoardDInLogicalName.RoomStay, out isInside);

                status = new
                {
                    roomId = _formMain.preferencesDatOriginal?.CompartmentNo ?? 0,
                    animalInside = isInside,
                    timestamp = System.DateTime.Now
                };
            }));
            return status;
        }

        /// <summary>
        /// ルームランプON/OFF
        /// </summary>
        public Task<bool> SetRoomLampAsync(bool on)
        {
            var tcs = new TaskCompletionSource<bool>();
            _formMain.Invoke((MethodInvoker)(() =>
            {
                try
                {
                    // TODO: 正しいキューを確認
                    // RoomLamp用の専用キューがない場合は、Door用キューを使用
                    var cmdPkt = new DevCmdPkt
                    {
                        DevCmdVal = on ? EDevCmd.RoomLampOn : EDevCmd.RoomLampOff
                    };
                    _formMain.concurrentQueueDevCmdPktDoor?.Enqueue(cmdPkt);
                    tcs.SetResult(true);
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            }));
            return tcs.Task;
        }

        /// <summary>
        /// レバーランプON/OFF
        /// </summary>
        public Task<bool> SetLeverLampAsync(bool on)
        {
            var tcs = new TaskCompletionSource<bool>();
            _formMain.Invoke((MethodInvoker)(() =>
            {
                try
                {
                    var cmdPkt = new DevCmdPkt
                    {
                        DevCmdVal = on ? EDevCmd.LeverLampOn : EDevCmd.LeverLampOff
                    };
                    _formMain.concurrentQueueDevCmdPktLever?.Enqueue(cmdPkt);
                    tcs.SetResult(true);
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            }));
            return tcs.Task;
        }

        /// <summary>
        /// 給餌ランプON/OFF
        /// </summary>
        public Task<bool> SetFeedLampAsync(bool on)
        {
            var tcs = new TaskCompletionSource<bool>();
            _formMain.Invoke((MethodInvoker)(() =>
            {
                try
                {
                    var cmdPkt = new DevCmdPkt
                    {
                        DevCmdVal = on ? EDevCmd.FeedLampOn : EDevCmd.FeedLampOff
                    };
                    _formMain.concurrentQueueDevCmdPktFeed?.Enqueue(cmdPkt);
                    tcs.SetResult(true);
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            }));
            return tcs.Task;
        }

        /// <summary>
        /// 音声ファイルを再生
        /// </summary>
        public Task<bool> PlaySoundAsync(string filePath, int durationMs)
        {
            var tcs = new TaskCompletionSource<bool>();
            _formMain.Invoke((MethodInvoker)(() =>
            {
                try
                {
                    // 実装方法は既存のサウンド再生機能に依存
                    // 仮実装: System.Media.SoundPlayerを使用
                    if (System.IO.File.Exists(filePath))
                    {
                        var player = new System.Media.SoundPlayer(filePath);
                        player.Play();
                        tcs.SetResult(true);
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"[Sound] File not found: {filePath}");
                        tcs.SetResult(false);
                    }
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            }));
            return tcs.Task;
        }

        /// <summary>
        /// ドアの状態を取得
        /// </summary>
        public object GetDoorStatus()
        {
            object status = null;
            _formMain.Invoke((MethodInvoker)(() =>
            {
                bool sensorOpen = false;
                bool sensorClose = false;

                _formMain.ioBoardDevice?.GetUpperStateOfSaveDIn(
                    IoBoardDInLogicalName.DoorOpen, out sensorOpen);
                _formMain.ioBoardDevice?.GetUpperStateOfSaveDIn(
                    IoBoardDInLogicalName.DoorClose, out sensorClose);

                string state = "unknown";
                if (sensorOpen && !sensorClose)
                    state = "opened";
                else if (!sensorOpen && sensorClose)
                    state = "closed";
                else if (!sensorOpen && !sensorClose)
                    state = "moving";

                status = new
                {
                    roomId = _formMain.preferencesDatOriginal?.CompartmentNo ?? 0,
                    state = state,
                    sensorOpen = sensorOpen,
                    sensorClose = sensorClose,
                    timestamp = System.DateTime.Now
                };
            }));
            return status;
        }

        /// <summary>
        /// 現在のモードが実機モードかどうか
        /// </summary>
        public bool IsRealHardwareMode()
        {
            bool result = false;
            _formMain.Invoke((MethodInvoker)(() =>
            {
                result = !(_formMain.preferencesDatOriginal?.EnableDebugMode ?? false);
            }));
            return result;
        }
```

---

## 🎨 Phase 3: 統合・完成（Medium）

Phase 2が完了してから実施してください。

### 📝 タスク23: LampRequest.cs作成

**新規ファイル**: `Models/LampRequest.cs`

```csharp
namespace Compartment.Models
{
    public class LampRequest
    {
        public bool On { get; set; }
    }
}
```

---

### 📝 タスク24: SoundRequest.cs作成

**新規ファイル**: `Models/SoundRequest.cs`

```csharp
namespace Compartment.Models
{
    public class SoundRequest
    {
        public string File { get; set; }
        public int DurationMs { get; set; } = 1000;
    }
}
```

---

### 📝 タスク25: FormMain.csのStartApiServer()修正

**ファイル**: `FormMain.cs`

**変更箇所**: 行505-540（StartApiServerメソッド）

**現在コメントアウトされている部分を以下に置き換え**:

```csharp
private void StartApiServer()
{
    try
    {
        _hardwareService = new HardwareService(this);

        // 全コントローラーの初期化
        SensorController.Initialize(_hardwareService);
        DoorController.Initialize(_hardwareService);
        LeverController.Initialize(_hardwareService);
        FeedController.Initialize(_hardwareService);
        RFIDController.Initialize(_hardwareService);
        DebugController.Initialize(_hardwareService);
        RoomController.Initialize(_hardwareService);
        LampController.Initialize(_hardwareService);
        SoundController.Initialize(_hardwareService);

        string url = $"http://localhost:{preferencesDatOriginal.ApiServerPort}/";

        _apiServer = WebApp.Start<Startup>(url);

        Debug.WriteLine($"API Server started at {url}");
    }
    catch (Exception ex)
    {
        Debug.WriteLine($"Failed to start API server: {ex.Message}");
    }
}
```

**そして FormMain_Load で呼び出す**:

```csharp
// FormMain_Load メソッド内に追加
private void FormMain_Load(object sender, EventArgs e)
{
    // 既存の初期化処理...

    // APIサーバー起動（デバッグモードでも起動）
    StartApiServer();
}
```

---

### 📝 タスク26: Compartment.csprojにファイル登録

**ファイル**: `Compartment.csproj`

**追加場所**: `<ItemGroup>` セクション内

**追加するXML**:

```xml
<ItemGroup>
  <!-- 既存の <Compile Include="..." /> の後に追加 -->

  <!-- Controllers -->
  <Compile Include="Controllers\SensorController.cs" />
  <Compile Include="Controllers\DoorController.cs" />
  <Compile Include="Controllers\LeverController.cs" />
  <Compile Include="Controllers\FeedController.cs" />
  <Compile Include="Controllers\RFIDController.cs" />
  <Compile Include="Controllers\DebugController.cs" />
  <Compile Include="Controllers\RoomController.cs" />
  <Compile Include="Controllers\LampController.cs" />
  <Compile Include="Controllers\SoundController.cs" />

  <!-- Services -->
  <Compile Include="Services\HardwareService.cs" />

  <!-- Models -->
  <Compile Include="Models\ApiResponseBase.cs" />
  <Compile Include="Models\SensorStatusResponse.cs" />
  <Compile Include="Models\DeviceCommandResponse.cs" />
  <Compile Include="Models\FeedRequest.cs" />
  <Compile Include="Models\RFIDResponse.cs" />
  <Compile Include="Models\LampRequest.cs" />
  <Compile Include="Models\SoundRequest.cs" />
</ItemGroup>
```

**注意**: 既存のコントローラー・モデルファイルがすでに登録されている場合は、新規追加分（RoomController, LampController, SoundController, LampRequest, SoundRequest）のみ追加してください。

---

## 🧪 各Phase完了後のテスト手順

### Phase 1完了後のテスト

**目標**: コンパイルエラーなし、アプリケーション起動

```
1. ビルド → ソリューションのリビルド
2. エラー0件を確認
3. アプリケーション起動
4. Preferences画面でOpeTypeOfTaskに「ExternalControl」が表示されることを確認
5. デバッグモードチェックボックスが動作することを確認
```

**成功の基準**: アプリケーションが起動し、ExternalControlモードが選択できる

---

### Phase 2完了後のテスト

**目標**: APIサーバーが起動し、エンドポイントにアクセスできる

**テスト1: APIサーバー起動確認**
```
1. アプリケーション起動
2. デバッグ出力に "API Server started at http://localhost:5000/" が表示されることを確認
```

**テスト2: Postmanでエンドポイントテスト**

```bash
# ケージ状態取得
GET http://localhost:5000/api/room/status

# 期待されるレスポンス:
{
  "roomId": 0,
  "animalInside": false,
  "timestamp": "2026-02-04T12:34:56"
}

# ドア開く（デバッグモード推奨）
POST http://localhost:5000/api/door/open

# レバーランプON
POST http://localhost:5000/api/lamp/lever
Content-Type: application/json

{
  "on": true
}
```

**成功の基準**: 全てのエンドポイントが200 OKを返す

---

### Phase 3完了後のテスト

**目標**: PsychoPyから実際に制御できる

**テスト1: PsychoPyスクリプト実行**

```python
import requests

base_url = "http://localhost:5000/api"

# ドアを開く
response = requests.post(f"{base_url}/door/open")
print(response.json())

# ケージ状態確認
response = requests.get(f"{base_url}/room/status")
print(response.json())
```

**成功の基準**: PsychoPyからAPIを呼び出してハードウェアが制御できる

---

## ⚠️ よくあるエラーと対処法

### エラー1: "The type or namespace name 'Services' does not exist"

**原因**: using Compartment.Services; が追加されていない

**対処**: FormMain.cs の行17-18のコメントを解除

---

### エラー2: "'ECpTask' does not contain a definition for 'ExternalControl'"

**原因**: PreferencesDat.cs に ExternalControl が追加されていない

**対処**: タスク10を実施

---

### エラー3: "HardwareService does not contain a definition for 'WaitForEntryAsync'"

**原因**: HardwareService.cs にメソッドが追加されていない

**対処**: タスク18-22を実施

---

### エラー4: APIサーバーが起動しない

**原因**: StartApiServer()がコメントアウトされているか、FormMain_Loadで呼ばれていない

**対処**: タスク25を実施

---

### エラー5: "Port 5000 is already in use"

**原因**: 他のアプリケーションがポート5000を使用中

**対処**: PreferencesDat の ApiServerPort を 5001などに変更

---

## 📊 実装進捗チェックリスト

コピーして使用してください：

```
Phase 1: 基盤修正（Critical）
[ ] タスク1-3: FormMain/UcMain修正（完了済み）
[ ] タスク4: RFIDReaderHelper.csにRFIDプロパティ追加
[ ] タスク5: RFIDReaderDummy.csにRFIDプロパティ追加
[ ] タスク6: HardwareService.csのSetSensorState()修正
[ ] タスク7: HardwareService.csのCompartmentNo型変換修正
[ ] タスク8: IoMicrochipDummyEx.csにResetAllStates()追加
[ ] タスク9: RFIDReaderDummy.csにSetRandomRFID()追加
[ ] タスク10: PreferencesDat.csにExternalControl追加
[ ] タスク11: UcOperationInternal.csでステートマシン無効化
[ ] タスク12: UcOperation.csでeDoor無効化
[ ] タスク13: OpenDoorAsync()安全チェック
[ ] タスク14: CloseDoorAsync()安全チェック
[ ] Phase 1 ビルド・テスト完了

Phase 2: コア機能実装（High）
[ ] タスク15: RoomController.cs作成
[ ] タスク16: LampController.cs作成
[ ] タスク17: SoundController.cs作成
[ ] タスク18-22: HardwareServiceメソッド追加
[ ] Phase 2 ビルド・テスト完了

Phase 3: 統合・完成（Medium）
[ ] タスク23: LampRequest.cs作成
[ ] タスク24: SoundRequest.cs作成
[ ] タスク25: StartApiServer()修正
[ ] タスク26: Compartment.csproj登録
[ ] Phase 3 ビルド・テスト完了
[ ] PsychoPy統合テスト完了
```

---

## 🎯 次のステップ

1. **Phase 1を完了**（タスク4-14）
2. **Windows環境でビルド・テスト**
3. **問題なければPhase 2へ進む**
4. **Phase 2完了後、Postmanでテスト**
5. **Phase 3完了後、PsychoPy統合テスト**

各Phaseの完了ごとに動作確認を行うことで、問題を早期に発見できます。

---

**作成日**: 2026-02-04
**更新履歴**:
- 2026-02-04: 初版作成（タスク1-26の詳細手順）
