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
            buttonStayOn.Click += (s, e) => SetSensor(IoBoardDInLogicalName.RoomStay, true);
            buttonStayOff.Click += (s, e) => SetSensor(IoBoardDInLogicalName.RoomStay, false);
            buttonLeverSwOn.Click += (s, e) => SetSensor(IoBoardDInLogicalName.LeverSw, true);
            buttonLeverSwOff.Click += (s, e) => SetSensor(IoBoardDInLogicalName.LeverSw, false);

            // デバイス制御ボタン
            buttonDoorOpen.Click += ButtonDoorOpen_Click;
            buttonDoorClose.Click += ButtonDoorClose_Click;
            buttonLeverExtend.Click += ButtonLeverExtend_Click;
            buttonLeverRetract.Click += ButtonLeverRetract_Click;
            buttonFeedDispense.Click += ButtonFeedDispense_Click;
            buttonRoomLampOn.Click += (s, e) => ControlRoomLamp(true);
            buttonRoomLampOff.Click += (s, e) => ControlRoomLamp(false);
            buttonLeverLampOn.Click += (s, e) => ControlLeverLamp(true);
            buttonLeverLampOff.Click += (s, e) => ControlLeverLamp(false);

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
                AddLog($"[手動] {sensorName} {(state ? "ON" : "OFF")}");
            }
            catch (Exception ex)
            {
                AddLog($"エラー: {ex.Message}");
            }
        }

        private void ButtonDoorOpen_Click(object sender, EventArgs e)
        {
            try
            {
                var cmdPkt = new FormMain.DevCmdPkt { DevCmdVal = FormMain.EDevCmd.DoorOpen };
                formMain.concurrentQueueDevCmdPktDoor.Enqueue(cmdPkt);
                AddLog("[制御] ドアを開く");
            }
            catch (Exception ex)
            {
                AddLog($"エラー: ドア開 - {ex.Message}");
            }
        }

        private void ButtonDoorClose_Click(object sender, EventArgs e)
        {
            try
            {
                var cmdPkt = new FormMain.DevCmdPkt { DevCmdVal = FormMain.EDevCmd.DoorClose };
                formMain.concurrentQueueDevCmdPktDoor.Enqueue(cmdPkt);
                AddLog("[制御] ドアを閉じる");
            }
            catch (Exception ex)
            {
                AddLog($"エラー: ドア閉 - {ex.Message}");
            }
        }

        private void ButtonLeverExtend_Click(object sender, EventArgs e)
        {
            try
            {
                var cmdPkt = new FormMain.DevCmdPkt { DevCmdVal = FormMain.EDevCmd.LeverOut };
                formMain.concurrentQueueDevCmdPktLever.Enqueue(cmdPkt);
                AddLog("[制御] レバーを出す");
            }
            catch (Exception ex)
            {
                AddLog($"エラー: レバー出 - {ex.Message}");
            }
        }

        private void ButtonLeverRetract_Click(object sender, EventArgs e)
        {
            try
            {
                var cmdPkt = new FormMain.DevCmdPkt { DevCmdVal = FormMain.EDevCmd.LeverIn };
                formMain.concurrentQueueDevCmdPktLever.Enqueue(cmdPkt);
                AddLog("[制御] レバーを引っ込める");
            }
            catch (Exception ex)
            {
                AddLog($"エラー: レバー引込 - {ex.Message}");
            }
        }

        private void ButtonFeedDispense_Click(object sender, EventArgs e)
        {
            try
            {
                var cmdPkt = new FormMain.DevCmdPkt { DevCmdVal = FormMain.EDevCmd.FeedForward };
                cmdPkt.iParam[0] = 1000; // 1秒間の給餌
                formMain.concurrentQueueDevCmdPktFeed.Enqueue(cmdPkt);
                AddLog("[制御] 給餌実行 (1秒)");
            }
            catch (Exception ex)
            {
                AddLog($"エラー: 給餌 - {ex.Message}");
            }
        }

        private void ControlRoomLamp(bool on)
        {
            try
            {
                bool result = formMain.ioBoardDevice.SetUpperStateOfDOut(
                    on ? IoBoardDOutLogicalName.RoomLampOn : IoBoardDOutLogicalName.RoomLampOff);

                if (result)
                {
                    AddLog($"[制御] ルームランプ {(on ? "ON" : "OFF")}");
                }
                else
                {
                    AddLog($"エラー: ルームランプ制御失敗");
                }
            }
            catch (Exception ex)
            {
                AddLog($"エラー: ルームランプ - {ex.Message}");
            }
        }

        private void ControlLeverLamp(bool on)
        {
            try
            {
                bool result = formMain.ioBoardDevice.SetUpperStateOfDOut(
                    on ? IoBoardDOutLogicalName.LeverLampOn : IoBoardDOutLogicalName.LeverLampOff);

                if (result)
                {
                    AddLog($"[制御] レバーランプ {(on ? "ON" : "OFF")}");
                }
                else
                {
                    AddLog($"エラー: レバーランプ制御失敗");
                }
            }
            catch (Exception ex)
            {
                AddLog($"エラー: レバーランプ - {ex.Message}");
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
                // 手動シミュレート可能なセンサー状態更新
                UpdateSensorDisplay(IoBoardDInLogicalName.RoomEntrance, labelEntranceStatus);
                UpdateSensorDisplay(IoBoardDInLogicalName.RoomExit, labelExitStatus);
                UpdateSensorDisplay(IoBoardDInLogicalName.RoomStay, labelStayStatus);
                UpdateSensorDisplay(IoBoardDInLogicalName.LeverSw, labelLeverSwStatus);

                // デバイスセンサー状態更新（リアルタイム表示）
                UpdateSensorDisplay(IoBoardDInLogicalName.DoorOpen, labelDoorOpenStatus);
                UpdateSensorDisplay(IoBoardDInLogicalName.DoorClose, labelDoorCloseStatus);
                UpdateSensorDisplay(IoBoardDInLogicalName.LeverIn, labelLeverInStatus);
                UpdateSensorDisplay(IoBoardDInLogicalName.LeverOut, labelLeverOutStatus);
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
