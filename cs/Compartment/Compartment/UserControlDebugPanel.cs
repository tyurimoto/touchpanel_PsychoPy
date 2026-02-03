using System;
using System.Drawing;
using System.Windows.Forms;

namespace Compartment
{
    public partial class UserControlDebugPanel : UserControl
    {
        private IoMicrochipDummyEx dummyIoBoard;
        private FormMain formMain;

        public UserControlDebugPanel(FormMain parent)
        {
            InitializeComponent();
            formMain = parent;

            // IOボードの参照を取得
            if (parent.ioBoardDevice is IoMicrochipDummyEx dummyEx)
            {
                dummyIoBoard = dummyEx;
            }
            else
            {
                // デバッグモード以外では無効化
                this.Enabled = false;
                AddLog("エラー: デバッグモードではありません");
                return;
            }

            // イベントハンドラ登録
            InitializeEventHandlers();

            // 状態更新タイマー（100ms周期）
            updateTimer = new Timer();
            updateTimer.Interval = 100;
            updateTimer.Tick += UpdateDisplay;
            updateTimer.Start();

            AddLog("デバッグパネルを初期化しました");
        }

        private void InitializeEventHandlers()
        {
            // センサーボタン
            buttonEntranceOn.Click += (s, e) => SetSensor(IoBoardDInLogicalName.RoomEntrance, true);
            buttonEntranceOff.Click += (s, e) => SetSensor(IoBoardDInLogicalName.RoomEntrance, false);
            buttonExitOn.Click += (s, e) => SetSensor(IoBoardDInLogicalName.RoomExit, true);
            buttonExitOff.Click += (s, e) => SetSensor(IoBoardDInLogicalName.RoomExit, false);

            // RFIDボタン
            buttonRFIDRandom.Click += ButtonRFIDRandom_Click;
            buttonRFIDSet.Click += ButtonRFIDSet_Click;
            buttonRFIDClear.Click += ButtonRFIDClear_Click;
        }

        private void SetSensor(IoBoardDInLogicalName sensor, bool state)
        {
            if (dummyIoBoard == null) return;

            try
            {
                dummyIoBoard.SetManualSensorState(sensor, state);
                string sensorName = GetSensorDisplayName(sensor);
                AddLog($"{sensorName} {(state ? "ON" : "OFF")}");
            }
            catch (Exception ex)
            {
                AddLog($"エラー: {ex.Message}");
            }
        }

        private void ButtonRFIDRandom_Click(object sender, EventArgs e)
        {
            var random = new Random();
            // 16桁のランダムな数値を生成
            long part1 = random.Next(100000000, 999999999);
            long part2 = random.Next(10000000, 99999999);
            string rfid = part1.ToString() + part2.ToString();
            textBoxRFID.Text = rfid;
            AddLog($"ランダムRFID生成: {rfid}");
        }

        private void ButtonRFIDSet_Click(object sender, EventArgs e)
        {
            string rfid = textBoxRFID.Text;
            if (string.IsNullOrWhiteSpace(rfid))
            {
                AddLog("エラー: RFID値を入力してください");
                return;
            }

            if (formMain.rfidReaderDummy != null)
            {
                formMain.rfidReaderDummy.SetRFID(rfid);
                AddLog($"RFID設定: {rfid}");
            }
            else
            {
                AddLog("エラー: RFIDリーダーが初期化されていません");
            }
        }

        private void ButtonRFIDClear_Click(object sender, EventArgs e)
        {
            if (formMain.rfidReaderDummy != null)
            {
                formMain.rfidReaderDummy.ClearRFID();
                textBoxRFID.Text = "";
                AddLog("RFIDクリア");
            }
            else
            {
                AddLog("エラー: RFIDリーダーが初期化されていません");
            }
        }

        private void UpdateDisplay(object sender, EventArgs e)
        {
            if (dummyIoBoard == null) return;

            try
            {
                // センサー状態更新
                UpdateSensorDisplay(IoBoardDInLogicalName.RoomEntrance, labelEntranceStatus);
                UpdateSensorDisplay(IoBoardDInLogicalName.RoomExit, labelExitStatus);

                // デバイス状態更新
                UpdateDeviceStatus();
            }
            catch (Exception ex)
            {
                // タイマー処理でエラーが出ても止めないようにする
                System.Diagnostics.Debug.WriteLine($"UpdateDisplay error: {ex.Message}");
            }
        }

        private void UpdateSensorDisplay(IoBoardDInLogicalName sensor, Label statusLabel)
        {
            if (dummyIoBoard.GetUpperStateOfSaveDIn(sensor, out bool state))
            {
                statusLabel.BackColor = state ? Color.Lime : Color.Gray;
                statusLabel.Text = state ? "ON" : "OFF";
            }
        }

        private void UpdateDeviceStatus()
        {
            // ドア状態
            bool doorOpen = false;
            bool doorClose = false;
            dummyIoBoard.GetUpperStateOfSaveDIn(IoBoardDInLogicalName.DoorOpen, out doorOpen);
            dummyIoBoard.GetUpperStateOfSaveDIn(IoBoardDInLogicalName.DoorClose, out doorClose);

            if (doorOpen)
                labelDoorStatus.Text = "● Open";
            else if (doorClose)
                labelDoorStatus.Text = "● Close";
            else
                labelDoorStatus.Text = "○ 不明";

            // レバー状態
            bool leverOut = false;
            bool leverIn = false;
            dummyIoBoard.GetUpperStateOfSaveDIn(IoBoardDInLogicalName.LeverOut, out leverOut);
            dummyIoBoard.GetUpperStateOfSaveDIn(IoBoardDInLogicalName.LeverIn, out leverIn);

            if (leverOut)
                labelLeverStatus.Text = "○ Out";
            else if (leverIn)
                labelLeverStatus.Text = "● In";
            else
                labelLeverStatus.Text = "○ 不明";

            // 給餌状態
            bool feeding = formMain.Feeding;
            labelFeedingStatus.Text = feeding ? "● 給餌中" : "○ 給餌中ではありません";
        }

        private void AddLog(string message)
        {
            // UIスレッドで実行されることを保証
            if (textBoxLog.InvokeRequired)
            {
                textBoxLog.Invoke(new Action(() => AddLog(message)));
                return;
            }

            string timestamp = DateTime.Now.ToString("HH:mm:ss");
            textBoxLog.AppendText($"{timestamp} - {message}\r\n");

            // 自動スクロール
            textBoxLog.SelectionStart = textBoxLog.Text.Length;
            textBoxLog.ScrollToCaret();
        }

        private string GetSensorDisplayName(IoBoardDInLogicalName sensor)
        {
            switch (sensor)
            {
                case IoBoardDInLogicalName.RoomEntrance:
                    return "入室センサー";
                case IoBoardDInLogicalName.RoomExit:
                    return "退室センサー";
                case IoBoardDInLogicalName.RoomStay:
                    return "在室センサー";
                case IoBoardDInLogicalName.DoorOpen:
                    return "ドア開センサー";
                case IoBoardDInLogicalName.DoorClose:
                    return "ドア閉センサー";
                case IoBoardDInLogicalName.LeverIn:
                    return "レバーInセンサー";
                case IoBoardDInLogicalName.LeverOut:
                    return "レバーOutセンサー";
                case IoBoardDInLogicalName.LeverSw:
                    return "レバーSW";
                default:
                    return sensor.ToString();
            }
        }
    }
}
