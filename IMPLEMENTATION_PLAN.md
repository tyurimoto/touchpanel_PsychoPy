# コンパートメント・ソフト PsychoPy統合 実装計画

## 概要

既存のC#コンパートメント・ソフトをハードウェアAPIサーバー化し、PsychoPyから課題制御を可能にする。ハードウェア制御ロジックはそのまま維持し、課題ロジックのみをPythonに移行する段階的アプローチ。

## ユーザー決定事項

- ✅ **制御範囲**: 課題ロジック全体（入室→課題実行→給餌→退室）をPsychoPyで制御
- ✅ **既存機能**: 段階的に置き換え（Training, DelayMatch等を徐々に移行）
- ✅ **実装方法**: REST API方式（C#をハードウェアAPIサーバー化）
- ✅ **タッチパネル表示**: PsychoPyウィンドウで直接表示
- ✅ **FormSub**: ExternalControlモード時は非表示にする
- ✅ **データ記録**: C#側で記録（既存CSVフォーマット維持）
- ✅ **デバッグモード**: ハードウェアなしでPC単体でデモ動作可能
- ✅ **システム規模**: 5-10ケージ、1ケージ=約4デバイス、1デバイス=1PC（C#+PsychoPy）
- ✅ **条件付きシーケンス制御**: 部屋Aクリア→部屋B利用可能→部屋C利用可能（任意順序設定可能）
- ✅ **通知機能**: 次の課題が利用可能になった部屋で音・光による通知
- ✅ **オペレーター機能**: 課題選択、パラメータ設定、開始/停止、状態監視、データ収集、条件付きシーケンス制御

## アーキテクチャ

```
┌─────────────────────────────────────┐
│  PsychoPy スクリプト (Python)        │
│  - 課題フロー制御                    │
│  - セカンダリディスプレイに表示       │
│  - タッチ入力検出                    │
│  - compartment_hardware.py 使用     │
└──────────────┬──────────────────────┘
               │ HTTP REST API
               │ (localhost:5000)
               ▼
┌─────────────────────────────────────┐
│  C# Web API サーバー                 │
│  - ASP.NET Core (新規プロジェクト)   │
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
│  (変更なし)                          │
└─────────────────────────────────────┘
```

## デバッグモード設計

### 概要

ハードウェアを接続せずにPC単体でデモ動作可能なデバッグモードを追加。既存の`IoMicrochipDummy`を拡張し、API経由でセンサー状態をシミュレート可能にする。

### デバッグモードの特徴

- ✅ **完全ダミーモード**: ハードウェア不要でPCのみで動作
- ✅ **ハイブリッドモード**: 実機使用＋制御困難な部分のみ手動シミュレート
- ✅ センサーごとに「実機/手動」を切り替え可能
- ✅ API経由でセンサー状態を設定可能
- ✅ PsychoPy側からキーボードでセンサーをシミュレート
- ✅ ダミーRFID値を生成・設定可能
- ✅ デバッグパネルで状態を可視化

### 実装方針

#### 1. ハイブリッドIOボード（IoHybridBoard）

実機とダミーを組み合わせ、センサーごとに切り替え可能なハイブリッドIOボード：

```csharp
public class IoHybridBoard : IoBoardBase
{
    private IoBoardBase realBoard;      // 実機（IoMicrochip）
    private IoBoardBase dummyBoard;     // ダミー（IoMicrochipDummyEx）

    // センサーごとのモード設定（true=手動シミュレート、false=実機）
    private Dictionary<IoBoardDInLogicalName, bool> sensorOverrides = new Dictionary<IoBoardDInLogicalName, bool>
    {
        { IoBoardDInLogicalName.RoomEntrance, true },  // デフォルト：手動
        { IoBoardDInLogicalName.RoomExit, true },      // デフォルト：手動
        { IoBoardDInLogicalName.RoomStay, false },     // デフォルト：実機
        { IoBoardDInLogicalName.DoorOpen, false },     // デフォルト：実機
        { IoBoardDInLogicalName.DoorClose, false },    // デフォルト：実機
        { IoBoardDInLogicalName.LeverIn, false },      // デフォルト：実機
        { IoBoardDInLogicalName.LeverOut, false },     // デフォルト：実機
        { IoBoardDInLogicalName.LeverSw, false }       // デフォルト：実機
    };

    // 手動シミュレート用のセンサー状態
    private Dictionary<IoBoardDInLogicalName, bool> manualSensorStates = new Dictionary<IoBoardDInLogicalName, bool>();

    public IoHybridBoard(bool useRealHardware)
    {
        // 実機を試行
        if (useRealHardware)
        {
            realBoard = new IoMicrochip();
            if (!realBoard.AcquireDevice())
            {
                realBoard = null;  // 接続失敗時はnull
            }
        }

        // ダミーは常に準備
        dummyBoard = new IoMicrochipDummyEx();
        dummyBoard.AcquireDevice();
    }

    // センサーのオーバーライド設定
    public void SetSensorOverride(IoBoardDInLogicalName sensor, bool useManual)
    {
        sensorOverrides[sensor] = useManual;
    }

    // 手動でセンサー状態を設定
    public void SetManualSensorState(IoBoardDInLogicalName sensor, bool state)
    {
        lock (manualSensorStates)
        {
            manualSensorStates[sensor] = state;
        }
    }

    public override bool GetUpperStateOfSaveDIn(IoBoardDInLogicalName logicalName, out bool logicalState)
    {
        // オーバーライド設定を確認
        if (sensorOverrides.ContainsKey(logicalName) && sensorOverrides[logicalName])
        {
            // 手動シミュレートモード
            lock (manualSensorStates)
            {
                logicalState = manualSensorStates.ContainsKey(logicalName)
                    ? manualSensorStates[logicalName]
                    : false;
            }
            return true;
        }
        else
        {
            // 実機モード
            if (realBoard != null)
            {
                return realBoard.GetUpperStateOfSaveDIn(logicalName, out logicalState);
            }
            else
            {
                // 実機なし時はダミーにフォールバック
                return dummyBoard.GetUpperStateOfSaveDIn(logicalName, out logicalState);
            }
        }
    }

    // 出力制御は常に実機優先（または全てダミー）
    public override bool SetUpperStateOfDOut(IoBoardDOutLogicalName logicalName)
    {
        if (realBoard != null)
        {
            return realBoard.SetUpperStateOfDOut(logicalName);
        }
        else
        {
            return dummyBoard.SetUpperStateOfDOut(logicalName);
        }
    }

    // 実機接続状態を取得
    public bool IsRealHardwareConnected => realBoard != null;
}
```

#### 1-2. 完全ダミーIOボード（IoMicrochipDummyEx）

実機なしで完全動作するダミー（既存のIoMicrochipDummyを拡張）：

```csharp
public class IoMicrochipDummyEx : IoBoardBase
{
    // （前述のダミー実装）
    // センサー状態を保持し、デバイスコマンドで自動更新
}
```

#### 2. デバッグモード設定

**Preferences画面に追加**:
- チェックボックス「Enable Debug Mode」
- ラジオボタン:
  - 「完全ダミー（実機不要）」
  - 「ハイブリッド（実機＋手動シミュレート）」← **推奨**

**FormMain.csの初期化**:
```csharp
private void InitializeIoBoard()
{
    if (preferencesDatOriginal.EnableDebugMode)
    {
        if (preferencesDatOriginal.DebugModeType == DebugModeType.FullDummy)
        {
            // 完全ダミー
            ioBoardDevice = new IoMicrochipDummyEx();
        }
        else if (preferencesDatOriginal.DebugModeType == DebugModeType.Hybrid)
        {
            // ハイブリッド（実機あれば使用、制御困難な部分のみ手動）
            ioBoardDevice = new IoHybridBoard(useRealHardware: true);
        }

        // RFID読み取りはダミー化（手動設定可能）
        rfidReader = new RFIDReaderDummy();
    }
    else
    {
        // 通常モード（実機のみ）
        ioBoardDevice = new IoMicrochip();
        rfidReader = new RFIDReaderHelper();
    }
}
```

#### 3. デバッグ専用API

**新しいコントローラー: `DebugController.cs`**

```csharp
[ApiController]
[Route("api/debug")]
public class DebugController : ControllerBase
{
    private readonly HardwareService _hardwareService;

    // センサー状態を設定
    [HttpPost("sensor/set")]
    public IActionResult SetSensor([FromBody] SetSensorRequest req)
    {
        // req.sensor = "entrance", req.state = true
        _hardwareService.SetDebugSensorState(req.sensor, req.state);
        return Ok(new { success = true });
    }

    // RFID値を設定
    [HttpPost("rfid/set")]
    public IActionResult SetRFID([FromBody] SetRFIDRequest req)
    {
        // req.id = "1234567890123456"
        _hardwareService.SetDebugRFID(req.id);
        return Ok(new { success = true });
    }

    // すべてのセンサー状態を取得
    [HttpGet("sensors/all")]
    public IActionResult GetAllSensors()
    {
        var states = _hardwareService.GetAllDebugSensorStates();
        return Ok(states);
    }

    // デバイス状態をリセット
    [HttpPost("reset")]
    public IActionResult Reset()
    {
        _hardwareService.ResetDebugState();
        return Ok(new { success = true });
    }
}
```

#### 4. PsychoPy側のシミュレーション機能

**キーボードシミュレーション（compartment_hardware.py拡張）**:

```python
class CompartmentHardware:
    def __init__(self, base_url="http://localhost:5000/api", debug_mode=False):
        self.base_url = base_url
        self.debug_mode = debug_mode

    # デバッグモード専用メソッド
    def debug_set_sensor(self, sensor_name: str, state: bool) -> bool:
        """デバッグ: センサー状態を設定"""
        if not self.debug_mode:
            print("Warning: debug_set_sensor called in non-debug mode")
            return False

        try:
            response = requests.post(
                f"{self.base_url}/debug/sensor/set",
                json={"sensor": sensor_name, "state": state},
                timeout=self.timeout
            )
            response.raise_for_status()
            return response.json()["success"]
        except Exception as e:
            print(f"Error setting debug sensor: {e}")
            return False

    def debug_set_rfid(self, rfid_id: str) -> bool:
        """デバッグ: RFID値を設定"""
        if not self.debug_mode:
            return False

        try:
            response = requests.post(
                f"{self.base_url}/debug/rfid/set",
                json={"id": rfid_id},
                timeout=self.timeout
            )
            response.raise_for_status()
            return response.json()["success"]
        except Exception as e:
            print(f"Error setting debug RFID: {e}")
            return False

    def debug_simulate_entrance(self) -> bool:
        """デバッグ: 入室をシミュレート"""
        return self.debug_set_sensor("entrance", True)

    def debug_simulate_exit(self) -> bool:
        """デバッグ: 退室をシミュレート"""
        return self.debug_set_sensor("exit", True)
```

**キーボードインタラクティブモード**:

```python
# debug_keyboard_control.py
from psychopy import event, core
from compartment_hardware import CompartmentHardware

hw = CompartmentHardware(debug_mode=True)

print("=== デバッグモード ===")
print("キー操作:")
print("  E: 入室センサーON")
print("  X: 退室センサーON")
print("  R: RFID読み取りシミュレート")
print("  D: ドア開く")
print("  C: ドア閉じる")
print("  ESC: 終了")

while True:
    keys = event.getKeys()

    if 'escape' in keys:
        break
    elif 'e' in keys:
        print("入室センサー ON")
        hw.debug_set_sensor("entrance", True)
        core.wait(0.5)
        hw.debug_set_sensor("entrance", False)
    elif 'x' in keys:
        print("退室センサー ON")
        hw.debug_set_sensor("exit", True)
        core.wait(0.5)
        hw.debug_set_sensor("exit", False)
    elif 'r' in keys:
        import random
        rfid = f"{random.randint(1000000000000000, 9999999999999999)}"
        print(f"RFID: {rfid}")
        hw.debug_set_rfid(rfid)
    elif 'd' in keys:
        print("ドアを開く")
        hw.open_door()
    elif 'c' in keys:
        print("ドアを閉じる")
        hw.close_door()

    core.wait(0.01)
```

#### 5. デバッグコントロールパネル（C# GUI）

**デバッグモード時のディスプレイ構成**:
- **セカンダリディスプレイ**: PsychoPyの課題画面（タッチパネル代わり）
- **メインディスプレイ（ノートPC）**: FormMainにデバッグコントロールパネル表示

**新しいUserControl: `UserControlDebugPanel.cs`**

FormMainに追加するデバッグパネル。デバッグモード時のみ表示。

```csharp
public partial class UserControlDebugPanel : UserControl
{
    private IoMicrochipDummyEx dummyIoBoard;

    // UI要素:
    // - センサー状態表示（LED風）
    // - センサーON/OFFボタン
    // - RFID入力フィールド
    // - デバイス状態表示
    // - ログ出力エリア
}
```

**パネルレイアウト（ハイブリッドモード対応）**:

```
┌─────────────────────────────────────────────────┐
│ デバッグコントロールパネル  [ハイブリッドモード] │
│ 実機接続: ● 接続中                              │
├─────────────────────────────────────────────────┤
│ [センサー - 手動シミュレート]                   │
│  ● 入室センサー [ON] [OFF] ○ OFF  [☑手動]    │
│  ● 退室センサー [ON] [OFF] ○ OFF  [☑手動]    │
│  ● 在室センサー              ● OFF  [☐実機]    │
│  ● レバーSW                  ○ OFF  [☐実機]    │
│                                                 │
│ [デバイス状態 - 実機から取得]                   │
│  ドア:    ○ Open  ● Close  (実機センサー)      │
│  レバー:  ● In    ○ Out    (実機センサー)      │
│  給餌中:  ○ 給餌中ではありません                │
│                                                 │
│ [RFID - 手動設定]                               │
│  ID: [1234567890123456] [ランダム生成]         │
│      [設定] [クリア]                            │
│                                                 │
│ [ログ]                                          │
│  12:34:56 - [手動] 入室センサー ON             │
│  12:34:57 - [手動] RFID設定: 1234...           │
│  12:34:58 - [実機] ドア閉じる                   │
└─────────────────────────────────────────────────┘
```

**チェックボックスの意味**:
- ☑手動: このセンサーは手動シミュレート（ボタンでON/OFF）
- ☐実機: このセンサーは実機から読み取り

**実装詳細**:

```csharp
// UserControlDebugPanel.Designer.cs
partial class UserControlDebugPanel
{
    // センサーボタン
    private Button buttonEntranceOn;
    private Button buttonEntranceOff;
    private Label labelEntranceStatus;  // LED表示

    private Button buttonExitOn;
    private Button buttonExitOff;
    private Label labelExitStatus;

    // RFID
    private TextBox textBoxRFID;
    private Button buttonRFIDSet;
    private Button buttonRFIDRandom;

    // デバイス状態表示
    private Label labelDoorStatus;
    private Label labelLeverStatus;
    private Label labelFeedingStatus;

    // ログ
    private TextBox textBoxLog;
}

// UserControlDebugPanel.cs
public partial class UserControlDebugPanel : UserControl
{
    private IoMicrochipDummyEx dummyIoBoard;
    private FormMain formMain;

    public UserControlDebugPanel(FormMain parent, IoMicrochipDummyEx dummyBoard)
    {
        InitializeComponent();
        formMain = parent;
        dummyIoBoard = dummyBoard;

        // イベントハンドラ登録
        buttonEntranceOn.Click += (s, e) => SetSensor(IoBoardDInLogicalName.RoomEntrance, true);
        buttonEntranceOff.Click += (s, e) => SetSensor(IoBoardDInLogicalName.RoomEntrance, false);
        // ... 他のセンサーも同様

        buttonRFIDSet.Click += ButtonRFIDSet_Click;
        buttonRFIDRandom.Click += ButtonRFIDRandom_Click;

        // 状態更新タイマー（100ms周期）
        var timer = new Timer { Interval = 100 };
        timer.Tick += UpdateDisplay;
        timer.Start();
    }

    private void SetSensor(IoBoardDInLogicalName sensor, bool state)
    {
        dummyIoBoard.SetSensorState(sensor, state);
        AddLog($"{sensor.GetEnumDisplayName()} {(state ? "ON" : "OFF")}");
    }

    private void ButtonRFIDSet_Click(object sender, EventArgs e)
    {
        string rfid = textBoxRFID.Text;
        if (formMain.RFIDReaderHelper is RFIDReaderDummy dummy)
        {
            dummy.SetRFID(rfid);
            AddLog($"RFID設定: {rfid}");
        }
    }

    private void ButtonRFIDRandom_Click(object sender, EventArgs e)
    {
        var random = new Random();
        string rfid = random.Next(1000000000, int.MaxValue).ToString() +
                      random.Next(1000000, 9999999).ToString();
        textBoxRFID.Text = rfid;
    }

    private void UpdateDisplay(object sender, EventArgs e)
    {
        // センサー状態更新（実機 or 手動）
        ioBoard.GetUpperStateOfSaveDIn(IoBoardDInLogicalName.RoomEntrance, out bool entrance);
        labelEntranceStatus.BackColor = entrance ? Color.Green : Color.Gray;

        ioBoard.GetUpperStateOfSaveDIn(IoBoardDInLogicalName.RoomExit, out bool exit);
        labelExitStatus.BackColor = exit ? Color.Green : Color.Gray;

        // デバイス状態更新（実機から取得）
        ioBoard.GetUpperStateOfSaveDIn(IoBoardDInLogicalName.DoorOpen, out bool doorOpen);
        ioBoard.GetUpperStateOfSaveDIn(IoBoardDInLogicalName.DoorClose, out bool doorClose);
        if (doorOpen)
            labelDoorStatus.Text = "● Open (実機)";
        else if (doorClose)
            labelDoorStatus.Text = "● Close (実機)";
        else
            labelDoorStatus.Text = "○ 不明";

        ioBoard.GetUpperStateOfSaveDIn(IoBoardDInLogicalName.LeverOut, out bool leverOut);
        ioBoard.GetUpperStateOfSaveDIn(IoBoardDInLogicalName.LeverIn, out bool leverIn);
        if (leverOut)
            labelLeverStatus.Text = "○ Out (実機)";
        else if (leverIn)
            labelLeverStatus.Text = "● In (実機)";
        else
            labelLeverStatus.Text = "○ 不明";

        labelFeedingStatus.Text = formMain.Feeding ? "● 給餌中" : "○ 給餌中ではありません";
    }

    private void AddLog(string message)
    {
        textBoxLog.AppendText($"{DateTime.Now:HH:mm:ss} - {message}\r\n");
    }
}
```

**FormMainへの統合**:

```csharp
// FormMain.cs
private UserControlDebugPanel userControlDebugPanel;

private void FormMain_Load(object sender, EventArgs e)
{
    // ... 既存の初期化

    // IOボード初期化
    InitializeIoBoard();

    // デバッグモード時のみパネル表示
    if (preferencesDatOriginal.EnableDebugMode)
    {
        userControlDebugPanel = new UserControlDebugPanel(this, ioBoardDevice);
        userControlDebugPanel.Dock = DockStyle.Right;  // 右側に配置
        userControlDebugPanel.Width = 450;
        this.Controls.Add(userControlDebugPanel);
        userControlDebugPanel.BringToFront();
    }
}
```

**キーボードショートカット追加**:

デバッグパネルにキーボードショートカットを追加（オプション）:
- `E`: 入室センサーON（500ms後に自動OFF）
- `X`: 退室センサーON（500ms後に自動OFF）
- `R`: ランダムRFID生成
- `D`: ドア開く/閉じるトグル
- `L`: レバー出す/引っ込めるトグル

```csharp
protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
{
    if (!preferencesDatOriginal.EnableDebugMode)
        return base.ProcessCmdKey(ref msg, keyData);

    switch (keyData)
    {
        case Keys.E:
            userControlDebugPanel.SimulateEntrancePulse();
            return true;
        case Keys.X:
            userControlDebugPanel.SimulateExitPulse();
            return true;
        // ... 他のショートカット
    }
    return base.ProcessCmdKey(ref msg, keyData);
}
```

### デバッグモードのメリット

1. **開発効率**: ハードウェアなしでコード開発・テスト可能
2. **デモ**: プレゼンや説明時に実機不要
3. **教育**: 学生や新規メンバーのトレーニングに使用
4. **CI/CD**: 自動テスト環境で使用可能
5. **リモート開発**: 在宅勤務時でも開発継続
6. **可視化**: メインディスプレイで機器状態をリアルタイム確認
7. **簡易操作**: ボタンクリックで擬似的に動物の行動をシミュレート

## 条件付きシーケンス制御（多部屋連携）

### 概要

複数の部屋で連鎖的な課題を実施する機能。部屋Aの課題が完了したら部屋Bが利用可能になり、部屋Bが完了したら部屋Cが利用可能になる、といった条件付きシーケンスを設定可能。

### 主要機能

#### 1. シーケンス定義

オペレーターが部屋の順序を任意に設定：

```json
{
  "sequenceId": "seq_001",
  "name": "3部屋連携課題",
  "rooms": [
    {
      "order": 1,
      "roomId": "room1",
      "taskType": "training",
      "requiredTrials": 10,
      "notifyOnAvailable": true
    },
    {
      "order": 2,
      "roomId": "room3",
      "taskType": "delaymatch",
      "requiredTrials": 20,
      "notifyOnAvailable": true
    },
    {
      "order": 3,
      "roomId": "room2",
      "taskType": "reversal",
      "requiredTrials": 15,
      "notifyOnAvailable": true
    }
  ]
}
```

**特徴**:
- 部屋の順序は任意（room1→room3→room2など）
- 各ステップで完了条件を設定（試行回数、正答率など）
- 次の部屋で音・光による通知の有無を設定

#### 2. 状態管理

中央オーケストレーターがシーケンス全体の状態を管理：

```python
# central_orchestrator/sequence_manager.py
from enum import Enum
from dataclasses import dataclass
from typing import List, Dict, Optional

class RoomStatus(Enum):
    LOCKED = "locked"           # まだ利用不可
    AVAILABLE = "available"     # 利用可能（動物が入室可能）
    IN_PROGRESS = "in_progress" # 課題実行中
    COMPLETED = "completed"     # 完了

@dataclass
class RoomStep:
    order: int
    room_id: str
    task_type: str
    required_trials: int
    notify_on_available: bool
    status: RoomStatus = RoomStatus.LOCKED
    completed_trials: int = 0

@dataclass
class Sequence:
    sequence_id: str
    name: str
    rooms: List[RoomStep]
    current_step: int = 0

    def get_current_room(self) -> Optional[RoomStep]:
        if self.current_step < len(self.rooms):
            return self.rooms[self.current_step]
        return None

    def get_next_room(self) -> Optional[RoomStep]:
        if self.current_step + 1 < len(self.rooms):
            return self.rooms[self.current_step + 1]
        return None

class SequenceManager:
    def __init__(self):
        self.sequences: Dict[str, Sequence] = {}

    async def start_sequence(self, sequence: Sequence):
        """シーケンス開始 - 最初の部屋をAVAILABLEに"""
        sequence.rooms[0].status = RoomStatus.AVAILABLE
        if sequence.rooms[0].notify_on_available:
            await self.notify_room(sequence.rooms[0].room_id)
        self.sequences[sequence.sequence_id] = sequence

    async def on_trial_completed(self, room_id: str, sequence_id: str):
        """試行完了時の処理"""
        sequence = self.sequences.get(sequence_id)
        if not sequence:
            return

        current_room = sequence.get_current_room()
        if current_room.room_id != room_id:
            return

        current_room.completed_trials += 1

        # 必要試行数に到達したらステップ完了
        if current_room.completed_trials >= current_room.required_trials:
            current_room.status = RoomStatus.COMPLETED
            await self.advance_to_next_room(sequence)

    async def advance_to_next_room(self, sequence: Sequence):
        """次の部屋に進む"""
        sequence.current_step += 1
        next_room = sequence.get_current_room()

        if next_room:
            # 次の部屋を利用可能に
            next_room.status = RoomStatus.AVAILABLE

            # 通知が有効なら音・光を鳴らす
            if next_room.notify_on_available:
                await self.notify_room(next_room.room_id)
        else:
            # シーケンス完了
            print(f"Sequence {sequence.sequence_id} completed!")

    async def notify_room(self, room_id: str):
        """部屋で音・光による通知"""
        room_url = ROOMS[room_id]["url"]
        async with httpx.AsyncClient() as client:
            # ルームランプON
            await client.post(f"{room_url}/room/lamp", json={"on": True})
            # 音を鳴らす
            await client.post(f"{room_url}/sound/play", json={
                "file": "/sounds/notification.wav",
                "durationMs": 1000
            })
            # 3秒後にランプOFF
            await asyncio.sleep(3)
            await client.post(f"{room_url}/room/lamp", json={"on": False})
```

#### 3. 中央オーケストレーターAPIエンドポイント

**シーケンス管理API**:

```python
# central_orchestrator/main.py

sequence_manager = SequenceManager()

@app.post("/sequences/create")
async def create_sequence(sequence: Sequence):
    """新しいシーケンスを作成"""
    await sequence_manager.start_sequence(sequence)
    return {"success": True, "sequenceId": sequence.sequence_id}

@app.get("/sequences/{sequence_id}/status")
async def get_sequence_status(sequence_id: str):
    """シーケンスの現在状態を取得"""
    sequence = sequence_manager.sequences.get(sequence_id)
    if not sequence:
        return {"error": "Sequence not found"}

    return {
        "sequenceId": sequence.sequence_id,
        "name": sequence.name,
        "currentStep": sequence.current_step,
        "totalSteps": len(sequence.rooms),
        "rooms": [
            {
                "order": room.order,
                "roomId": room.room_id,
                "taskType": room.task_type,
                "status": room.status.value,
                "completedTrials": room.completed_trials,
                "requiredTrials": room.required_trials
            }
            for room in sequence.rooms
        ]
    }

@app.post("/sequences/{sequence_id}/trial_completed")
async def trial_completed(sequence_id: str, room_id: str):
    """試行完了を通知（各部屋のC# APIから呼ばれる）"""
    await sequence_manager.on_trial_completed(room_id, sequence_id)
    return {"success": True}

@app.get("/rooms/{room_id}/sequence_status")
async def get_room_sequence_status(room_id: str):
    """特定部屋がどのシーケンスに含まれているか確認"""
    for sequence in sequence_manager.sequences.values():
        for room in sequence.rooms:
            if room.room_id == room_id:
                return {
                    "sequenceId": sequence.sequence_id,
                    "status": room.status.value,
                    "isAvailable": room.status == RoomStatus.AVAILABLE,
                    "completedTrials": room.completed_trials,
                    "requiredTrials": room.required_trials
                }
    return {"error": "Room not in any active sequence"}
```

#### 4. PsychoPy側の統合

各部屋のPsychoPyスクリプトがシーケンス状態をチェック：

```python
# training_task_with_sequence.py
from compartment_hardware import CompartmentHardware
import requests

# 中央オーケストレーターに接続
ORCHESTRATOR_URL = "http://server-pc:8000"
ROOM_ID = "room1"  # この部屋のID

hw = CompartmentHardware()

def check_if_available():
    """この部屋が利用可能か確認"""
    response = requests.get(f"{ORCHESTRATOR_URL}/rooms/{ROOM_ID}/sequence_status")
    data = response.json()
    return data.get("isAvailable", False)

def notify_trial_completed(sequence_id):
    """試行完了を中央に通知"""
    requests.post(
        f"{ORCHESTRATOR_URL}/sequences/{sequence_id}/trial_completed",
        params={"room_id": ROOM_ID}
    )

# メインループ
while True:
    # シーケンス状態を確認
    if not check_if_available():
        print("この部屋はまだ利用できません。待機中...")
        core.wait(1.0)
        continue

    # 入室待ち
    if hw.check_entrance():
        rfid = hw.read_rfid()

        # 課題実行
        for trial in range(10):
            # ... 課題ロジック ...

            # 試行完了を通知
            notify_trial_completed(sequence_id="seq_001")
```

#### 5. Webダッシュボードでのシーケンス管理

オペレーター用のWebダッシュボードに以下の機能を追加：

```
┌─────────────────────────────────────────────────┐
│   シーケンス制御パネル                          │
├─────────────────────────────────────────────────┤
│ [新規シーケンス作成]                            │
│                                                 │
│ アクティブシーケンス: seq_001                   │
│ 名前: 3部屋連携課題                             │
│                                                 │
│ ステップ 1/3                                    │
│ ┌─────────────────────────────────────────┐   │
│ │ ✅ 部屋1 (Training) - 完了 10/10試行    │   │
│ └─────────────────────────────────────────┘   │
│      ↓                                         │
│ ┌─────────────────────────────────────────┐   │
│ │ ▶ 部屋3 (DelayMatch) - 進行中 5/20試行  │   │
│ │    [🔔 通知済み]                        │   │
│ └─────────────────────────────────────────┘   │
│      ↓                                         │
│ ┌─────────────────────────────────────────┐   │
│ │ 🔒 部屋2 (Reversal) - ロック中 0/15試行 │   │
│ └─────────────────────────────────────────┘   │
│                                                 │
│ [シーケンス停止] [シーケンス編集]               │
└─────────────────────────────────────────────────┘
```

**シーケンス作成UI**:

```
┌─────────────────────────────────────────────────┐
│   新規シーケンス作成                            │
├─────────────────────────────────────────────────┤
│ シーケンス名: [3部屋連携課題              ]     │
│                                                 │
│ ステップ設定:                                   │
│ ┌───────────────────────────────────────┐       │
│ │ ステップ 1                             │       │
│ │ 部屋: [room1 ▼]  課題: [Training ▼]   │       │
│ │ 必要試行数: [10]                       │       │
│ │ ☑ 利用可能時に通知                    │       │
│ └───────────────────────────────────────┘       │
│ ┌───────────────────────────────────────┐       │
│ │ ステップ 2                             │       │
│ │ 部屋: [room3 ▼]  課題: [DelayMatch ▼] │       │
│ │ 必要試行数: [20]                       │       │
│ │ ☑ 利用可能時に通知                    │       │
│ └───────────────────────────────────────┘       │
│ ┌───────────────────────────────────────┐       │
│ │ ステップ 3                             │       │
│ │ 部屋: [room2 ▼]  課題: [Reversal ▼]   │       │
│ │ 必要試行数: [15]                       │       │
│ │ ☑ 利用可能時に通知                    │       │
│ └───────────────────────────────────────┘       │
│                                                 │
│ [+ ステップ追加]                                │
│                                                 │
│ [キャンセル] [シーケンス開始]                   │
└─────────────────────────────────────────────────┘
```

### システム規模

#### ケージ・デバイス構成

- **ケージ数**: 5-10ケージ
- **1ケージあたり**: 約4デバイス
- **1デバイスあたり**: 1PC（C# Compartment.exe + PsychoPy課題スクリプト）
- **合計PC数**: 20-40台（5ケージ×4デバイス ～ 10ケージ×4デバイス）

#### ネットワーク構成

```
┌──────────────────────────┐
│   中央サーバーPC          │
│   (FastAPI Orchestrator) │
│   192.168.1.100:8000     │
└─────────┬────────────────┘
          │ LAN
    ┌─────┴─────┬─────┬─────┬─────┐
    ▼           ▼     ▼     ▼     ▼
┌─────────┐ ┌─────┐ ...  ┌─────┐ ┌─────┐
│Cage1-PC1│ │C1-P2│      │C5-P3│ │C5-P4│
│:5001    │ │:5002│      │:5019│ │:5020│
└─────────┘ └─────┘      └─────┘ └─────┘
  C#+Psy     C#+Psy        C#+Psy  C#+Psy
```

**ポート番号割り当て例**:
- Cage1-PC1: `http://192.168.1.101:5001/api`
- Cage1-PC2: `http://192.168.1.102:5002/api`
- Cage1-PC3: `http://192.168.1.103:5003/api`
- Cage1-PC4: `http://192.168.1.104:5004/api`
- Cage2-PC1: `http://192.168.1.105:5005/api`
- ...

### 実装上の考慮点

1. **状態同期**: 中央オーケストレーターがシーケンス状態を永続化（ファイルまたはDB）
2. **障害時の復旧**: サーバー再起動時に状態を復元
3. **並列シーケンス**: 複数のシーケンスを同時実行可能（異なる個体が異なるシーケンスを実施）
4. **個体管理**: RFID→個体ID→シーケンスIDのマッピング
5. **通知のカスタマイズ**: 音ファイル、ランプ点滅パターンを設定可能

## 実装フェーズ

### フェーズ1: APIサーバー構築（1-2週間）

**目的**: C#にREST API機能を追加し、基本的なハードウェア制御を公開

**作業内容**:
1. 新規ASP.NET Core Web APIプロジェクト `Compartment.HardwareAPI` を追加
2. FormMainにKestrelサーバー起動ロジックを追加
3. 基本的なコントローラーを実装（Sensor, Door, Lever, Feed, RFID）
4. HardwareServiceクラスでFormMainとの連携
5. **デバッグモード実装**:
   - `IoMicrochipDummyEx` クラス作成（センサー状態を保持）
   - `RFIDReaderDummy` クラス作成（ダミーRFID）
   - Preferences画面に「Enable Debug Mode」追加
   - `DebugController` 実装
6. Postmanで動作確認

**成果物**:
- 動作するAPIサーバー（localhost:5000）
- 基本的なエンドポイント（センサー読み取り、ドア制御、レバー制御、給餌、RFID）
- デバッグモード機能

**検証方法**:
- デバッグモードOFF: 実機で `POST http://localhost:5000/api/door/open` でドアが開く
- デバッグモードON: `POST http://localhost:5000/api/debug/sensor/set` でセンサー状態変更

### フェーズ2: Pythonクライアントライブラリ（1週間）

**目的**: PsychoPyからハードウェアを制御するためのPythonライブラリ作成

**作業内容**:
1. `compartment_hardware.py` を作成
2. 各APIエンドポイントに対応するメソッドを実装
3. エラーハンドリング、タイムアウト処理を追加
4. **デバッグモード対応**:
   - `debug_mode` パラメータ追加
   - `debug_set_sensor()`, `debug_set_rfid()` メソッド実装
   - `debug_keyboard_control.py` 作成（キーボードシミュレーション）
5. 簡単なテストスクリプトで動作確認

**成果物**:
- `CompartmentHardware` Pythonクラス
- デバッグモード機能付き
- キーボードコントロールスクリプト
- テストスクリプト

**検証方法**:
- デバッグモードOFF: 実機で `hw.check_entrance()` でセンサー読み取り
- デバッグモードON: `hw.debug_simulate_entrance()` で入室シミュレート
- キーボード操作でセンサーをシミュレート

### フェーズ3: ExternalControlモード追加（1週間）

**目的**: C#側の課題制御を無効化し、外部制御を可能にするモード追加

**作業内容**:
1. `ECpTask` 列挙型に `ExternalControl = 5` を追加
2. OpCollection/UcOperationのステートマシンにスキップロジック追加
3. ExternalControlモード時にFormSubを非表示
4. UI設定画面に「ExternalControl」を追加

**成果物**:
- ExternalControlモードが選択可能
- C#側のステートマシンが無効化される

**検証方法**:
- Preferences画面でExternalControlを選択可能
- Startボタンを押しても課題ステートマシンが動作しない

### フェーズ4: 簡単な課題実装（1-2週間）

**目的**: PsychoPyで完全な課題フローを実装し、エンドツーエンド検証

**作業内容**:
1. `training_task_example.py` を実装
2. PsychoPyウィンドウをセカンダリディスプレイに設定
3. 入室検出→RFID→課題→給餌→退室の完全フロー実装
4. データ記録APIエンドポイント `POST /api/log/write` を追加
5. **デバッグモードでの動作確認**:
   - キーボードで入室/退室をシミュレート
   - ダミーRFIDを自動生成
   - デモモードで完全フロー実行
6. 実機で動作確認

**成果物**:
- PsychoPyで動作する訓練課題
- データ記録機能
- デバッグモード対応

**検証方法**:
- デバッグモード: PC単体で課題フローが動作
- 実機モード: 実際のハードウェアで課題が実行できる
- 既存CSVフォーマットでデータが記録される

### フェーズ5: 既存課題の移行（2-4週間）

**目的**: Training、DelayMatch等の既存課題をPython実装に移行

**作業内容**:
1. Training課題をPython実装 (`training_task.py`)
2. DelayMatch課題をPython実装 (`delaymatch_task.py`)
3. データフォーマットの互換性確認
4. 既存データとの比較検証

**成果物**:
- 既存課題と同等のPython実装
- 検証レポート

### フェーズ6: 中央オーケストレーター実装（3-5週間） ⭐重要

**目的**: 複数部屋を統合管理し、条件付きシーケンス制御を実現

**作業内容**:
1. **中央オーケストレーター（FastAPI）実装**:
   - `CentralOrchestrator` Pythonプロジェクト作成
   - 部屋管理API実装（rooms/, rooms/{room_id}/）
   - シーケンス管理API実装（sequences/）
   - 状態永続化（JSON/SQLite）

2. **C# API拡張**:
   - APIレスポンスに `compartmentNo` / `roomId` 追加
   - Preferences に `ApiServerPort` プロパティ追加
   - ポート番号を部屋ごとに設定可能に

3. **シーケンスマネージャー実装**:
   - `SequenceManager` クラス（状態管理）
   - 条件付き進行ロジック（A完了→B利用可能）
   - 音・光による通知機能

4. **Webダッシュボード実装**:
   - FastAPI + React/Vue.js
   - シーケンス作成UI
   - リアルタイム状態監視
   - データ可視化

5. **PsychoPy側の統合**:
   - `compartment_hardware.py` に中央オーケストレーター連携機能追加
   - シーケンス状態チェック
   - 試行完了通知

6. **ネットワーク設定・検証**:
   - 複数PC（20-40台）のネットワーク設定
   - ポート番号割り当て
   - 中央サーバーとの通信確認

**成果物**:
- 動作する中央オーケストレーター（FastAPI）
- Webダッシュボード
- 条件付きシーケンス制御機能
- 複数部屋での動作検証レポート

**検証方法**:
- 3部屋でシーケンス実行（room1→room3→room2）
- 各部屋での音・光通知確認
- ダッシュボードでリアルタイム状態監視
- 20-40台規模でのスケーラビリティ検証

## 重要なファイル

### 変更が必要な既存ファイル

1. **`/Users/terumi/Downloads/compartment/cs/Compartment/Compartment.sln`**
   - HardwareAPIプロジェクトを追加

2. **`/Users/terumi/Downloads/compartment/cs/Compartment/Compartment/FormMain.cs`**
   - Kestrelサーバー起動ロジック追加
   - FormSubの表示/非表示制御

3. **`/Users/terumi/Downloads/compartment/cs/Compartment/Compartment/PreferencesDat.cs`**
   - `ECpTask` 列挙型に `ExternalControl = 5` を追加

4. **`/Users/terumi/Downloads/compartment/cs/Compartment/Compartment/OpCollection.cs`**
   - `Sequencer.Execute()` にExternalControlモードのスキップロジック追加

5. **`/Users/terumi/Downloads/compartment/cs/Compartment/Compartment/UcOperation.cs`**
   - `OnOperationStateMachineProc()` にExternalControlモードのスキップロジック追加

6. **`/Users/terumi/Downloads/compartment/cs/Compartment/Compartment/UserControlPreferencesTab.Designer.cs`**
   - `comboBoxTypeOfTask` に「ExternalControl」選択肢を追加
   - **チェックボックス「Enable Debug Mode」を追加** ⭐NEW

7. **`/Users/terumi/Downloads/compartment/cs/Compartment/Compartment/PreferencesDat.cs`** ⭐NEW
   - `EnableDebugMode` プロパティを追加

8. **`/Users/terumi/Downloads/compartment/cs/Compartment/Compartment/FormMain.cs`** ⭐拡張
   - デバッグパネル表示ロジック追加
   - デバッグモード時のキーボードショートカット追加（オプション）

### 新規作成ファイル

#### C#側（Compartment.HardwareAPIプロジェクト）

1. **`Compartment.HardwareAPI/Program.cs`**
   - ASP.NET Core起動設定

2. **`Compartment.HardwareAPI/Controllers/SensorController.cs`**
   - センサー読み取りAPI

3. **`Compartment.HardwareAPI/Controllers/RFIDController.cs`**
   - RFID読み取りAPI

4. **`Compartment.HardwareAPI/Controllers/DoorController.cs`**
   - ドア制御API

5. **`Compartment.HardwareAPI/Controllers/LeverController.cs`**
   - レバー制御API

6. **`Compartment.HardwareAPI/Controllers/FeedController.cs`**
   - 給餌制御API

7. **`Compartment.HardwareAPI/Controllers/RoomController.cs`**
   - ルームランプ制御API

8. **`Compartment.HardwareAPI/Controllers/SoundController.cs`**
   - 音声再生API

9. **`Compartment.HardwareAPI/Controllers/LogController.cs`**
   - データ記録API（フェーズ4で追加）

10. **`Compartment.HardwareAPI/Controllers/DebugController.cs`** ⭐NEW
    - デバッグモード専用API

11. **`Compartment.HardwareAPI/Services/IHardwareService.cs`**
    - ハードウェアサービスインターフェース

12. **`Compartment.HardwareAPI/Services/HardwareService.cs`**
    - FormMainとの連携ロジック

13. **`Compartment.HardwareAPI/Models/SensorStatus.cs`**
    - APIレスポンスモデル

14. **`Compartment.HardwareAPI/Models/DeviceCommand.cs`**
    - APIリクエストモデル

#### C#側（Compartmentプロジェクト - デバッグモード）

15. **`Compartment/IoMicrochipDummyEx.cs`** ⭐NEW
    - 完全ダミーIOボード（センサー状態を保持）

16. **`Compartment/IoHybridBoard.cs`** ⭐NEW
    - ハイブリッドIOボード（実機＋手動シミュレート切替）

17. **`Compartment/RFIDReaderDummy.cs`** ⭐NEW
    - ダミーRFIDリーダー

18. **`Compartment/UserControlDebugPanel.cs`** ⭐NEW
    - デバッグコントロールパネル（メインディスプレイ表示、ハイブリッドモード対応）

19. **`Compartment/UserControlDebugPanel.Designer.cs`** ⭐NEW
    - デバッグパネルUI定義

20. **`Compartment/PreferencesDat.cs`** ⭐拡張
    - `DebugModeType` 列挙型追加（FullDummy / Hybrid）

#### Python側（PsychoPy課題スクリプト）

1. **`compartment_hardware.py`**
   - ハードウェアAPIクライアントライブラリ（デバッグモード対応、シーケンス連携機能追加）

2. **`debug_keyboard_control.py`** ⭐NEW
   - キーボードでセンサーをシミュレート

3. **`training_task_example.py`**
   - 訓練課題サンプル（フェーズ4）

4. **`training_task_debug.py`** ⭐NEW
   - 訓練課題（デバッグモード版、キーボード操作）

5. **`training_task_with_sequence.py`** ⭐NEW
   - 訓練課題（シーケンス連携版、中央オーケストレーターと連携）

6. **`delaymatch_task_example.py`**
   - DelayMatch課題サンプル（フェーズ5）

#### Python側（中央オーケストレーター） ⭐NEW

7. **`CentralOrchestrator/main.py`**
   - FastAPIメインアプリケーション
   - 部屋管理API、シーケンス管理API

8. **`CentralOrchestrator/sequence_manager.py`**
   - SequenceManagerクラス（シーケンス状態管理）
   - 条件付き進行ロジック
   - 通知機能

9. **`CentralOrchestrator/models.py`**
   - データモデル（Sequence, RoomStep, RoomStatus）

10. **`CentralOrchestrator/database.py`**
    - 状態永続化（SQLite/JSON）
    - データ集約機能

11. **`CentralOrchestrator/dashboard/`**
    - Webダッシュボード（React/Vue.js）
    - シーケンス作成UI、リアルタイム監視UI

## APIエンドポイント仕様

### センサー読み取り

- **`GET /api/sensor/entrance`** - 入室センサー
  - レスポンス: `{ "active": true, "timestamp": "2026-01-29T12:34:56Z" }`

- **`GET /api/sensor/exit`** - 退室センサー
  - レスポンス: `{ "active": false, "timestamp": "..." }`

- **`GET /api/sensor/stay`** - 在室センサー
  - レスポンス: `{ "active": true, "timestamp": "..." }`

- **`GET /api/sensor/lever`** - レバーSW
  - レスポンス: `{ "pressed": false, "timestamp": "..." }`

### RFID

- **`GET /api/rfid/read`** - RFID読み取り
  - レスポンス: `{ "id": "3920145000567278", "timestamp": "..." }`

- **`DELETE /api/rfid`** - RFID値クリア
  - レスポンス: `{ "success": true }`

### ドア制御

- **`POST /api/door/open`** - ドアを開く
  - レスポンス: `{ "success": true, "state": "opening" }`

- **`POST /api/door/close`** - ドアを閉じる
  - レスポンス: `{ "success": true, "state": "closing" }`

- **`GET /api/door/status`** - ドア状態取得
  - レスポンス: `{ "state": "opened", "sensorOpen": true, "sensorClose": false }`

### レバー制御

- **`POST /api/lever/extend`** - レバーを出す
  - レスポンス: `{ "success": true, "state": "extending" }`

- **`POST /api/lever/retract`** - レバーを引っ込める
  - レスポンス: `{ "success": true, "state": "retracting" }`

- **`POST /api/lever/lamp`** - レバーランプ制御
  - リクエスト: `{ "on": true }`
  - レスポンス: `{ "success": true }`

### 給餌

- **`POST /api/feed/dispense`** - 給餌実行
  - リクエスト: `{ "durationMs": 1000 }`
  - レスポンス: `{ "success": true, "feeding": true }`

- **`POST /api/feed/lamp`** - 給餌ランプ制御
  - リクエスト: `{ "on": true }`
  - レスポンス: `{ "success": true }`

- **`GET /api/feed/status`** - 給餌状態取得
  - レスポンス: `{ "feeding": false }`

### ルーム

- **`POST /api/room/lamp`** - ルームランプ制御
  - リクエスト: `{ "on": true }`
  - レスポンス: `{ "success": true }`

### 音声

- **`POST /api/sound/play`** - 音声再生
  - リクエスト: `{ "file": "/path/to/sound.wav", "durationMs": 500 }`
  - レスポンス: `{ "success": true, "playing": true }`

### データ記録（フェーズ4で追加）

- **`POST /api/log/write`** - データ記録
  - リクエスト:
    ```json
    {
      "idCode": "3920145000567278",
      "result": "correct",
      "trialNumber": 5,
      "touchX": 123,
      "touchY": 456,
      "timestamp": "2026-01-29T12:34:56Z"
    }
    ```
  - レスポンス: `{ "success": true }`

### デバッグモード専用API ⭐NEW

- **`POST /api/debug/sensor/set`** - センサー状態を設定
  - リクエスト: `{ "sensor": "entrance", "state": true }`
  - レスポンス: `{ "success": true }`
  - 使用可能なセンサー名: `"entrance"`, `"exit"`, `"stay"`, `"lever"`

- **`POST /api/debug/rfid/set`** - RFID値を設定
  - リクエスト: `{ "id": "1234567890123456" }`
  - レスポンス: `{ "success": true }`

- **`GET /api/debug/sensors/all`** - すべてのセンサー状態を取得
  - レスポンス:
    ```json
    {
      "entrance": false,
      "exit": false,
      "stay": false,
      "doorOpen": false,
      "doorClose": true,
      "leverIn": true,
      "leverOut": false,
      "leverSw": false
    }
    ```

- **`POST /api/debug/reset`** - デバイス状態をリセット
  - レスポンス: `{ "success": true }`

### 中央オーケストレーターAPI（フェーズ6で追加） ⭐NEW

これらのAPIは中央サーバー（FastAPI）が提供します。

#### 部屋管理

- **`GET /rooms`** - 登録されている部屋一覧
  - レスポンス: `{ "room1": {"url": "http://...", "name": "実験室1"}, ... }`

- **`GET /rooms/{room_id}/sensors/all`** - 特定部屋の全センサー状態
  - レスポンス: `{ "roomId": "room1", "entrance": {...}, "exit": {...}, ... }`

- **`GET /rooms/{room_id}/sequence_status`** - 部屋のシーケンス状態
  - レスポンス:
    ```json
    {
      "sequenceId": "seq_001",
      "status": "available",
      "isAvailable": true,
      "completedTrials": 5,
      "requiredTrials": 10
    }
    ```

#### シーケンス制御

- **`POST /sequences/create`** - 新規シーケンス作成
  - リクエスト:
    ```json
    {
      "sequenceId": "seq_001",
      "name": "3部屋連携課題",
      "rooms": [
        {
          "order": 1,
          "roomId": "room1",
          "taskType": "training",
          "requiredTrials": 10,
          "notifyOnAvailable": true
        },
        ...
      ]
    }
    ```
  - レスポンス: `{ "success": true, "sequenceId": "seq_001" }`

- **`GET /sequences/{sequence_id}/status`** - シーケンス状態取得
  - レスポンス:
    ```json
    {
      "sequenceId": "seq_001",
      "name": "3部屋連携課題",
      "currentStep": 1,
      "totalSteps": 3,
      "rooms": [
        {
          "order": 1,
          "roomId": "room1",
          "taskType": "training",
          "status": "completed",
          "completedTrials": 10,
          "requiredTrials": 10
        },
        {
          "order": 2,
          "roomId": "room3",
          "taskType": "delaymatch",
          "status": "in_progress",
          "completedTrials": 5,
          "requiredTrials": 20
        },
        {
          "order": 3,
          "roomId": "room2",
          "taskType": "reversal",
          "status": "locked",
          "completedTrials": 0,
          "requiredTrials": 15
        }
      ]
    }
    ```

- **`POST /sequences/{sequence_id}/trial_completed`** - 試行完了通知
  - リクエストパラメータ: `room_id`
  - レスポンス: `{ "success": true }`
  - 説明: 各部屋のPsychoPyスクリプトが試行完了時に呼び出す

- **`DELETE /sequences/{sequence_id}`** - シーケンス停止
  - レスポンス: `{ "success": true }`

#### データ集約

- **`GET /data/summary`** - 全部屋のデータサマリー
  - レスポンス:
    ```json
    {
      "room1": {"totalTrials": 150, "successRate": 0.85, ...},
      "room2": {"totalTrials": 200, "successRate": 0.78, ...},
      ...
    }
    ```

- **`GET /data/by_rfid/{rfid_id}`** - 個体別データ取得
  - レスポンス: 特定個体の全試行履歴

## 重要な実装パターン

### FormMainとの統合（Kestrelサーバー起動）

```csharp
// FormMain.cs
private IHost _apiHost;

private void StartApiServer()
{
    _apiHost = Host.CreateDefaultBuilder()
        .ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.UseStartup<ApiStartup>();
            webBuilder.UseUrls("http://localhost:5000");
        })
        .Build();

    Task.Run(() => _apiHost.RunAsync());
}

// FormMain_Loadイベントで呼び出し
private void FormMain_Load(object sender, EventArgs e)
{
    // 既存の初期化処理...

    // APIサーバー起動
    StartApiServer();
}
```

### HardwareServiceでのスレッドセーフな制御

```csharp
// HardwareService.cs
public class HardwareService : IHardwareService
{
    private readonly FormMain _formMain;

    public HardwareService(FormMain formMain)
    {
        _formMain = formMain;
    }

    public async Task<bool> OpenDoorAsync()
    {
        var tcs = new TaskCompletionSource<bool>();
        _formMain.Invoke((MethodInvoker)(() =>
        {
            var cmdPkt = new DevCmdPkt { DevCmdVal = EDevCmd.DoorOpen };
            _formMain.concurrentQueueDevCmdPktDoor.Enqueue(cmdPkt);
            tcs.SetResult(true);
        }));
        return await tcs.Task;
    }
}
```

### ExternalControlモードのスキップロジック

```csharp
// OpCollection.cs - Sequencerクラス内
public void Execute()
{
    // ExternalControlモード時はステートマシンをスキップ
    if (_preferences.OpeTypeOfTask == ECpTask.ExternalControl)
    {
        // 何もしない（PsychoPyが制御）
        return;
    }

    // 既存のステートマシン処理
    switch (State)
    {
        case EState.WaitingForEnterCage:
            // ...
    }
}
```

### FormSubの表示制御

```csharp
// FormMain.cs
private void UpdateFormSubVisibility()
{
    if (preferencesDatOriginal.OpeTypeOfTask == ECpTask.ExternalControl)
    {
        formSub.Hide();
    }
    else
    {
        formSub.Show();
    }
}
```

## リスクと対策

### リスク1: REST APIのレイテンシ

**問題**: HTTP通信のオーバーヘッド（数ms~数十ms）がリアルタイム性に影響

**対策**:
- センサーポーリング間隔を100ms程度に設定
- 重要なコマンド（ドア、レバー）は即座にキューに追加
- 必要に応じてWebSocket導入を検討（リアルタイム通知）

### リスク2: スレッドセーフ

**問題**: Web APIスレッドとBackgroundWorkerスレッドの競合

**対策**:
- `Invoke` を使ってUIスレッドで実行
- `ConcurrentQueue` は既にスレッドセーフ
- センサー状態の読み取りは `SaveDIn()` 後の値を使用

### リスク3: 後方互換性

**問題**: 既存の課題が動作しなくなる

**対策**:
- ExternalControlモードは完全に独立
- 既存のTraining、DelayMatchモードは一切変更しない
- データ記録はC#側で既存フォーマットを維持

### リスク4: エラーハンドリング

**問題**: APIサーバーダウン、ハードウェアエラー、タイムアウト

**対策**:
- Python側でリトライロジック実装
- タイムアウト設定（5秒）
- エラーログ記録
- フェイルセーフ（エラー時はドアを開けて終了）

### リスク5: PsychoPyウィンドウ設定

**問題**: セカンダリディスプレイへの表示、タッチ入力検出

**対策**:
- PsychoPyの `screen=1` パラメータでセカンダリディスプレイ指定
- `fullscr=True` でフルスクリーン表示
- `event.Mouse(win=win)` でタッチ入力取得
- 事前にディスプレイ設定を確認

## 複数実験室の統合管理（フェーズ6で実装）

### アーキテクチャ

複数の実験室（5-10ケージ、20-40台PC）を中央のオーケストレーターから統合管理するシステム：

```
        ┌──────────────────────────┐
        │   中央オーケストレーター   │
        │     (Python/FastAPI)     │
        │  - 複数部屋の統合管理      │
        │  - 実験スケジューリング    │
        │  - データ集約・分析        │
        │  - Web UI ダッシュボード   │
        └─────────┬────────────────┘
                  │ HTTP REST API
    ┌─────────────┼─────────────┬─────────────┐
    ▼             ▼             ▼             ▼
┌──────────┐ ┌──────────┐ ┌──────────┐ ┌──────────┐
│ 部屋1 API│ │ 部屋2 API│ │ 部屋3 API│ │ 部屋N API│
│(C# 5001) │ │(C# 5002) │ │(C# 5003) │ │(C# 500N) │
└────┬─────┘ └────┬─────┘ └────┬─────┘ └────┬─────┘
     │            │            │            │
     ▼            ▼            ▼            ▼
┌──────────┐ ┌──────────┐ ┌──────────┐ ┌──────────┐
│PsychoPy 1│ │PsychoPy 2│ │PsychoPy 3│ │PsychoPy N│
│課題制御   │ │課題制御   │ │課題制御   │ │課題制御   │
└──────────┘ └──────────┘ └──────────┘ └──────────┘
```

### スケーラブル設計のポイント

#### 1. ポート番号の体系化

各部屋のC# APIサーバーは異なるポートで起動：
- 部屋1: `http://localhost:5001/api`
- 部屋2: `http://localhost:5002/api`
- 部屋3: `http://localhost:5003/api`
- ...

**Preferences設定に追加**:
```csharp
public int ApiServerPort { get; set; } = 5001;  // 部屋ごとに設定
```

#### 2. 部屋識別子の追加

すべてのAPIレスポンスに部屋IDを含める：

```json
{
  "roomId": "room1",
  "sensor": "entrance",
  "active": true,
  "timestamp": "2026-01-29T12:34:56Z"
}
```

**APIレスポンス拡張**:
```csharp
public class ApiResponseBase
{
    public string RoomId { get; set; }  // CompartmentNoから取得
    public DateTime Timestamp { get; set; }
}
```

#### 3. 中央オーケストレーター（Python/FastAPI）

**新規プロジェクト**: `CentralOrchestrator` (Python)

```python
# central_orchestrator/main.py
from fastapi import FastAPI
from typing import List, Dict
import httpx

app = FastAPI()

# 部屋の設定
ROOMS = {
    "room1": {"url": "http://localhost:5001/api", "name": "実験室1"},
    "room2": {"url": "http://localhost:5002/api", "name": "実験室2"},
    "room3": {"url": "http://localhost:5003/api", "name": "実験室3"},
}

@app.get("/rooms")
async def list_rooms():
    """登録されている部屋一覧"""
    return ROOMS

@app.get("/rooms/{room_id}/sensors/all")
async def get_room_sensors(room_id: str):
    """特定部屋の全センサー状態取得"""
    room = ROOMS.get(room_id)
    if not room:
        return {"error": "Room not found"}

    async with httpx.AsyncClient() as client:
        # 各センサーを並列取得
        entrance = await client.get(f"{room['url']}/sensor/entrance")
        exit = await client.get(f"{room['url']}/sensor/exit")
        # ...

    return {
        "roomId": room_id,
        "entrance": entrance.json(),
        "exit": exit.json(),
        # ...
    }

@app.get("/sensors/entrance/all")
async def get_all_entrance_sensors():
    """全部屋の入室センサー状態を取得"""
    results = {}
    async with httpx.AsyncClient() as client:
        for room_id, room in ROOMS.items():
            try:
                response = await client.get(f"{room['url']}/sensor/entrance")
                results[room_id] = response.json()
            except Exception as e:
                results[room_id] = {"error": str(e)}
    return results

@app.post("/rooms/{room_id}/feed/dispense")
async def dispense_feed(room_id: str, duration_ms: int):
    """特定部屋の給餌"""
    room = ROOMS.get(room_id)
    if not room:
        return {"error": "Room not found"}

    async with httpx.AsyncClient() as client:
        response = await client.post(
            f"{room['url']}/feed/dispense",
            json={"durationMs": duration_ms}
        )
        return response.json()

@app.post("/feed/dispense/all")
async def dispense_feed_all(duration_ms: int):
    """全部屋で同時給餌（同期実験用）"""
    results = {}
    async with httpx.AsyncClient() as client:
        tasks = []
        for room_id, room in ROOMS.items():
            task = client.post(
                f"{room['url']}/feed/dispense",
                json={"durationMs": duration_ms}
            )
            tasks.append((room_id, task))

        # 並列実行
        for room_id, task in tasks:
            results[room_id] = (await task).json()

    return results

@app.get("/data/summary")
async def get_data_summary():
    """全部屋のデータサマリー"""
    # 各部屋のログファイルを集約
    # データベースから統計情報を取得
    pass
```

#### 4. Webダッシュボード（オプション）

**FastAPI + React/Vue.js**:
- リアルタイムで全部屋の状態を監視
- グラフィカルな実験管理UI
- データ可視化（成功率、試行回数等）

```
┌─────────────────────────────────────┐
│   実験室統合管理ダッシュボード        │
├─────────────────────────────────────┤
│ 部屋1: ● 実験中 (試行 5/20)          │
│   入室: ○  退室: ○  給餌: ○         │
│   RFID: 3920145000567278           │
│                                     │
│ 部屋2: ○ 待機中                     │
│   入室: ○  退室: ○  給餌: ○         │
│                                     │
│ 部屋3: ● 実験中 (試行 12/20)         │
│   入室: ●  退室: ○  給餌: ○         │
│   RFID: 1234567890123456           │
│                                     │
│ [全部屋で給餌] [全部屋リセット]      │
└─────────────────────────────────────┘
```

#### 5. データ集約・分析

**中央データベース（オプション）**:
- PostgreSQL / MongoDB で全部屋のデータを集約
- 部屋間比較分析
- 個体追跡（RFID）

```python
# central_orchestrator/database.py
from sqlalchemy import create_engine, Column, Integer, String, DateTime, Boolean
from sqlalchemy.ext.declarative import declarative_base

Base = declarative_base()

class Trial(Base):
    __tablename__ = 'trials'

    id = Column(Integer, primary_key=True)
    room_id = Column(String)
    rfid = Column(String)
    result = Column(String)  # correct, timeout, etc.
    trial_number = Column(Integer)
    timestamp = Column(DateTime)
    # ...

# 各部屋のC# APIから POST /api/log/write を受けて集約
```

#### 6. 実装タイミング

**フェーズ6で実装** - 上記セクション「フェーズ6: 中央オーケストレーター実装」を参照

この機能は本計画の一部として実装します（将来の拡張ではなく、初期リリースに含む）。

### 現計画への影響

**今回の実装（フェーズ1-5）で考慮すべき点**:

1. ✅ **ポート番号を設定可能に**
   - Preferences に `ApiServerPort` 追加
   - デフォルト: 5000（将来は部屋番号に応じて変更）

2. ✅ **RoomID/CompartmentNoをレスポンスに含める**
   - 全APIレスポンスに `compartmentNo` または `roomId` を追加
   - 将来の統合管理に備える

3. ✅ **APIの独立性を保つ**
   - 各部屋のC# APIは完全に独立して動作
   - 中央オーケストレーターなしでも単体動作可能

4. ✅ **ログフォーマットの統一**
   - 部屋間でデータフォーマットを統一
   - 将来の集約を容易に

### スケーラビリティのメリット

1. **段階的拡張**: 1部屋から開始、必要に応じて拡張
2. **並列実験**: 複数部屋で同時に異なる課題を実施
3. **同期実験**: 全部屋で同じタイミングで制御（群実験）
4. **データ統合**: 部屋間比較、個体追跡
5. **効率化**: 中央から一括管理、実験設定の共有

## 成功の指標

- ✅ PsychoPyから全ハードウェアを制御可能
- ✅ 既存課題（Training, DelayMatch）が引き続き動作
- ✅ データが既存CSVフォーマットで記録される
- ✅ API経由の制御レイテンシが50ms以下
- ✅ 既存ユーザーが ExternalControl モードを選択可能
- ✅ **将来の複数実験室統合に対応可能な設計** ⭐NEW

## 実装の優先順位

### 推奨実装順序

1. **フェーズ1-3**: まず単一部屋でのPsychoPy統合を完成させる
   - APIサーバー構築
   - Pythonクライアントライブラリ
   - ExternalControlモード

2. **フェーズ4**: 簡単な課題で動作確認
   - 完全なフロー実装
   - デバッグモード検証

3. **フェーズ5**: 既存課題の移行
   - Training, DelayMatch等のPython実装

4. **フェーズ6**: 複数部屋統合（最後に実装）
   - 中央オーケストレーター
   - 条件付きシーケンス制御

### 次のステップ（フェーズ1開始）

1. フェーズ1の実装開始
2. `Compartment.HardwareAPI` プロジェクト作成
3. 基本的なコントローラー実装（Sensor, Door）
4. FormMainにKestrel起動ロジック追加
5. **APIレスポンスにCompartmentNo追加**
6. **Preferences にApiServerPort追加**
7. デバッグモード実装（IoMicrochipDummyEx, IoHybridBoard）
8. デバッグコントロールパネル実装
9. Postmanで動作確認
