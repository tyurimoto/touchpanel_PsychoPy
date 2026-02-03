using System;
using System.Diagnostics;
using System.Drawing;
using System.Management;
using System.Windows.Forms;

namespace Compartment
{
    class UcCheckIo
    {
    }

    public partial class FormMain : Form
    {
        private CheckIoOutPort[] CheckIoOutPortTbl;
        private CheckIoInPort[] CheckIoInPortTbl;
        private void InitializeComponentOnUcCheckIo()
        {
            // ユーザ・コントロール: VisibleChanged
            userControlCheckIoOnFormMain.VisibleChanged += new System.EventHandler(userControlCheckIoOnFormMain_VisibleChanged);
            // ボタン:
            userControlCheckIoOnFormMain.buttonEnd.Click += (object sender, EventArgs e) =>
            {
                EndUcCheckIo();
                // UserControlCheckIo.buttonEnd: Click
                // userControlOperationOnFormMain: 表示
                VisibleUcMain();
            };
            userControlCheckIoOnFormMain.buttonClearIdCode.Click += (object sender, EventArgs e) =>
            {
                userControlCheckIoOnFormMain.textBoxIdCode.ResetText();
            };
            userControlCheckIoOnFormMain.buttonClearTouchPanel.Click += (object sender, EventArgs e) =>
            {
                userControlCheckIoOnFormMain.textBoxTouchPanel.ResetText();
            };
            CheckIoOutPortTbl = new CheckIoOutPort[]
            {
                new CheckIoOutPort(IoBoardPortNo.Port0,  0x01,   userControlCheckIoOnFormMain.checkBoxPortA_Out0),
                new CheckIoOutPort(IoBoardPortNo.Port0,  0x02,   userControlCheckIoOnFormMain.checkBoxPortA_Out1),
                new CheckIoOutPort(IoBoardPortNo.Port0,  0x04,   userControlCheckIoOnFormMain.checkBoxPortA_Out2),
                new CheckIoOutPort(IoBoardPortNo.Port0,  0x08,   userControlCheckIoOnFormMain.checkBoxPortA_Out3),
                new CheckIoOutPort(IoBoardPortNo.Port0,  0x10,   userControlCheckIoOnFormMain.checkBoxPortA_Out4),
                new CheckIoOutPort(IoBoardPortNo.Port0,  0x20,   userControlCheckIoOnFormMain.checkBoxPortA_Out5),
                new CheckIoOutPort(IoBoardPortNo.Port0,  0x40,   userControlCheckIoOnFormMain.checkBoxPortA_Out6),
                new CheckIoOutPort(IoBoardPortNo.Port0,  0x80,   userControlCheckIoOnFormMain.checkBoxPortA_Out7),
                new CheckIoOutPort(IoBoardPortNo.Port2,  0x01,   userControlCheckIoOnFormMain.checkBoxPortC_Out0),
                new CheckIoOutPort(IoBoardPortNo.Port2,  0x02,   userControlCheckIoOnFormMain.checkBoxPortC_Out1),
                new CheckIoOutPort(IoBoardPortNo.Port2,  0x04,   userControlCheckIoOnFormMain.checkBoxPortC_Out2),
                new CheckIoOutPort(IoBoardPortNo.Port2,  0x08,   userControlCheckIoOnFormMain.checkBoxPortC_Out3),

                new CheckIoOutPort(IoBoardPortNo.Port4,  0x01,   userControlCheckIoOnFormMain.checkBoxPortD_Out0),
                new CheckIoOutPort(IoBoardPortNo.Port4,  0x02,   userControlCheckIoOnFormMain.checkBoxPortD_Out1),
                new CheckIoOutPort(IoBoardPortNo.Port4,  0x04,   userControlCheckIoOnFormMain.checkBoxPortD_Out2),
                new CheckIoOutPort(IoBoardPortNo.Port4,  0x08,   userControlCheckIoOnFormMain.checkBoxPortD_Out3),
                new CheckIoOutPort(IoBoardPortNo.Port4,  0x10,   userControlCheckIoOnFormMain.checkBoxPortD_Out4),
                new CheckIoOutPort(IoBoardPortNo.Port4,  0x20,   userControlCheckIoOnFormMain.checkBoxPortD_Out5),
                new CheckIoOutPort(IoBoardPortNo.Port4,  0x40,   userControlCheckIoOnFormMain.checkBoxPortD_Out6),
                new CheckIoOutPort(IoBoardPortNo.Port4,  0x80,   userControlCheckIoOnFormMain.checkBoxPortD_Out7),
                new CheckIoOutPort(IoBoardPortNo.Port6,  0x01,   userControlCheckIoOnFormMain.checkBoxPortF_Out0),
                new CheckIoOutPort(IoBoardPortNo.Port6,  0x02,   userControlCheckIoOnFormMain.checkBoxPortF_Out1),
                new CheckIoOutPort(IoBoardPortNo.Port6,  0x04,   userControlCheckIoOnFormMain.checkBoxPortF_Out2),
                new CheckIoOutPort(IoBoardPortNo.Port6,  0x08,   userControlCheckIoOnFormMain.checkBoxPortF_Out3),
                new CheckIoOutPort(IoBoardPortNo.Port6,  0x10,   userControlCheckIoOnFormMain.checkBoxPortF_Out4),
                new CheckIoOutPort(IoBoardPortNo.Port6,  0x20,   userControlCheckIoOnFormMain.checkBoxPortF_Out5),
                new CheckIoOutPort(IoBoardPortNo.Port6,  0x40,   userControlCheckIoOnFormMain.checkBoxPortF_Out6),
                new CheckIoOutPort(IoBoardPortNo.Port6,  0x80,   userControlCheckIoOnFormMain.checkBoxPortF_Out7),
            };
            CheckIoInPortTbl = new CheckIoInPort[]
            {
                    new CheckIoInPort(IoBoardPortNo.Port1,  0x01,   userControlCheckIoOnFormMain.textBoxPortB_In0),
                    new CheckIoInPort(IoBoardPortNo.Port1,  0x02,   userControlCheckIoOnFormMain.textBoxPortB_In1),
                    new CheckIoInPort(IoBoardPortNo.Port1,  0x04,   userControlCheckIoOnFormMain.textBoxPortB_In2),
                    new CheckIoInPort(IoBoardPortNo.Port1,  0x08,   userControlCheckIoOnFormMain.textBoxPortB_In3),
                    new CheckIoInPort(IoBoardPortNo.Port1,  0x10,   userControlCheckIoOnFormMain.textBoxPortB_In4),
                    new CheckIoInPort(IoBoardPortNo.Port1,  0x20,   userControlCheckIoOnFormMain.textBoxPortB_In5),
                    new CheckIoInPort(IoBoardPortNo.Port1,  0x40,   userControlCheckIoOnFormMain.textBoxPortB_In6),
                    new CheckIoInPort(IoBoardPortNo.Port1,  0x80,   userControlCheckIoOnFormMain.textBoxPortB_In7),
                    new CheckIoInPort(IoBoardPortNo.Port2,  0x01,   userControlCheckIoOnFormMain.textBoxPortC_In4),
                    new CheckIoInPort(IoBoardPortNo.Port2,  0x02,   userControlCheckIoOnFormMain.textBoxPortC_In5),
                    new CheckIoInPort(IoBoardPortNo.Port2,  0x04,   userControlCheckIoOnFormMain.textBoxPortC_In6),
                    new CheckIoInPort(IoBoardPortNo.Port2,  0x08,   userControlCheckIoOnFormMain.textBoxPortC_In7),
                    new CheckIoInPort(IoBoardPortNo.Port3,  0x01,   userControlCheckIoOnFormMain.textBoxPortD_In0),
                    new CheckIoInPort(IoBoardPortNo.Port3,  0x02,   userControlCheckIoOnFormMain.textBoxPortD_In1),
                    new CheckIoInPort(IoBoardPortNo.Port3,  0x04,   userControlCheckIoOnFormMain.textBoxPortD_In2),
                    new CheckIoInPort(IoBoardPortNo.Port3,  0x08,   userControlCheckIoOnFormMain.textBoxPortD_In3),
                    new CheckIoInPort(IoBoardPortNo.Port3,  0x10,   userControlCheckIoOnFormMain.textBoxPortD_In4),
                    new CheckIoInPort(IoBoardPortNo.Port3,  0x20,   userControlCheckIoOnFormMain.textBoxPortD_In5),
                    new CheckIoInPort(IoBoardPortNo.Port3,  0x40,   userControlCheckIoOnFormMain.textBoxPortD_In6),
                    new CheckIoInPort(IoBoardPortNo.Port3,  0x80,   userControlCheckIoOnFormMain.textBoxPortD_In7),
                    new CheckIoInPort(IoBoardPortNo.Port4,  0x01,   userControlCheckIoOnFormMain.textBoxPortE_In0),
                    new CheckIoInPort(IoBoardPortNo.Port4,  0x02,   userControlCheckIoOnFormMain.textBoxPortE_In1),
                    new CheckIoInPort(IoBoardPortNo.Port4,  0x04,   userControlCheckIoOnFormMain.textBoxPortE_In2),
                    new CheckIoInPort(IoBoardPortNo.Port4,  0x08,   userControlCheckIoOnFormMain.textBoxPortE_In3),
                    new CheckIoInPort(IoBoardPortNo.Port4,  0x10,   userControlCheckIoOnFormMain.textBoxPortE_In4),
                    new CheckIoInPort(IoBoardPortNo.Port4,  0x20,   userControlCheckIoOnFormMain.textBoxPortE_In5),
                    new CheckIoInPort(IoBoardPortNo.Port4,  0x40,   userControlCheckIoOnFormMain.textBoxPortE_In6),
                    new CheckIoInPort(IoBoardPortNo.Port4,  0x80,   userControlCheckIoOnFormMain.textBoxPortE_In7),
            };
            // イベントハンドラに関連付け
            foreach (CheckIoOutPort l_CheckIoOutPortObj in CheckIoOutPortTbl)
            {
                l_CheckIoOutPortObj.CheckBoxPort.CheckedChanged += new EventHandler(CheckBoxPortOnCheckIoOutPort_CheckedChanged);
            }
        }

        private void userControlCheckIoOnFormMain_VisibleChanged(object sender, EventArgs e)
        {
            ushort l_ushortInValueOfPort0 = 0;
            ushort l_ushortInValueOfPort2 = 0;
            ushort l_ushortInValueOfPort3 = 0;
            ushort l_ushortInValueOfPort4 = 0;


            if (userControlCheckIoOnFormMain.Visible == true)
            {
                if (!ioBoardDevice.DirectIn(IoBoardPortNo.Port0, out l_ushortInValueOfPort0))
                {
                    Debug.WriteLine(ioBoardDevice.errorMsg);
                    // userControlOperationOnFormMain: 表示
                    if (Properties.Settings.Default.IS_CHECK_SERIAL_OPENED == true)
                    {
                        EndUcCheckIo();
                        VisibleUcMain();
                        return;
                    }
                }
                if (!ioBoardDevice.DirectIn(IoBoardPortNo.Port2, out l_ushortInValueOfPort2))
                {
                    Debug.WriteLine(ioBoardDevice.errorMsg);
                    // userControlOperationOnFormMain: 表示
                    if (Properties.Settings.Default.IS_CHECK_SERIAL_OPENED == true)
                    {
                        EndUcCheckIo();
                        VisibleUcMain();
                        return;
                    }
                }
                if (!ioBoardDevice.DirectIn(IoBoardPortNo.Port3, out l_ushortInValueOfPort3))
                {
                    Debug.WriteLine(ioBoardDevice.errorMsg);
                    // userControlOperationOnFormMain: 表示
                    if (Properties.Settings.Default.IS_CHECK_SERIAL_OPENED == true)
                    {
                        EndUcCheckIo();
                        VisibleUcMain();
                        return;
                    }
                }
                if (!ioBoardDevice.DirectIn(IoBoardPortNo.Port4, out l_ushortInValueOfPort4))
                {
                    Debug.WriteLine(ioBoardDevice.errorMsg);
                    // userControlOperationOnFormMain: 表示
                    if (Properties.Settings.Default.IS_CHECK_SERIAL_OPENED == true)
                    {
                        EndUcCheckIo();
                        VisibleUcMain();
                        return;
                    }
                }
                foreach (CheckIoOutPort l_CheckIoOutPortObj in CheckIoOutPortTbl)
                {
                    switch (l_CheckIoOutPortObj.IoBoardPortNoPort)
                    {
                        case IoBoardPortNo.Port0:
                            if ((ioBoardDevice.GetSendData()[0] & l_CheckIoOutPortObj.ushortBitCode) == 0x0)
                            {
                                l_CheckIoOutPortObj.CheckBoxPort.Checked = false;
                            }
                            else
                            {
                                l_CheckIoOutPortObj.CheckBoxPort.Checked = true;
                            }
                            SetDiaplayStateOfCheckBoxPortOut(l_CheckIoOutPortObj.CheckBoxPort);
                            break;
                        case IoBoardPortNo.Port2:
                            if ((ioBoardDevice.GetSendData()[2] & l_CheckIoOutPortObj.ushortBitCode) == 0x0)
                            {
                                l_CheckIoOutPortObj.CheckBoxPort.Checked = false;
                            }
                            else
                            {
                                l_CheckIoOutPortObj.CheckBoxPort.Checked = true;
                            }
                            SetDiaplayStateOfCheckBoxPortOut(l_CheckIoOutPortObj.CheckBoxPort);
                            break;
                        case IoBoardPortNo.Port3:
                            if ((ioBoardDevice.GetSendData()[3] & l_CheckIoOutPortObj.ushortBitCode) == 0x0)
                            {
                                l_CheckIoOutPortObj.CheckBoxPort.Checked = false;
                            }
                            else
                            {
                                l_CheckIoOutPortObj.CheckBoxPort.Checked = true;
                            }
                            SetDiaplayStateOfCheckBoxPortOut(l_CheckIoOutPortObj.CheckBoxPort);
                            break;
                        case IoBoardPortNo.Port4:
                            if ((ioBoardDevice.GetSendData()[4] & l_CheckIoOutPortObj.ushortBitCode) == 0x0)
                            {
                                l_CheckIoOutPortObj.CheckBoxPort.Checked = false;
                            }
                            else
                            {
                                l_CheckIoOutPortObj.CheckBoxPort.Checked = true;
                            }
                            SetDiaplayStateOfCheckBoxPortOut(l_CheckIoOutPortObj.CheckBoxPort);
                            break;
                        case IoBoardPortNo.Port5:
                            if ((ioBoardDevice.GetSendData()[5] & l_CheckIoOutPortObj.ushortBitCode) == 0x0)
                            {
                                l_CheckIoOutPortObj.CheckBoxPort.Checked = false;
                            }
                            else
                            {
                                l_CheckIoOutPortObj.CheckBoxPort.Checked = true;
                            }
                            SetDiaplayStateOfCheckBoxPortOut(l_CheckIoOutPortObj.CheckBoxPort);
                            break;
                        case IoBoardPortNo.Port6:
                            if ((ioBoardDevice.GetSendData()[6] & l_CheckIoOutPortObj.ushortBitCode) == 0x0)
                            {
                                l_CheckIoOutPortObj.CheckBoxPort.Checked = false;
                            }
                            else
                            {
                                l_CheckIoOutPortObj.CheckBoxPort.Checked = true;
                            }
                            SetDiaplayStateOfCheckBoxPortOut(l_CheckIoOutPortObj.CheckBoxPort);
                            break;

                        default:
                            break;
                    }
                    // TextBoxをクリアする
                    userControlCheckIoOnFormMain.textBoxIdCode.ResetText();
                    userControlCheckIoOnFormMain.textBoxTouchPanel.ResetText();

                    // FormSubのMouseClickイベントを無効とする
                    formSub.boolEnableCallBackTouchPoint.Value = false;
                    formSub.callbackTouchPoint = (point) =>
                    {
                        Invoke((MethodInvoker)(() =>
                        {
                            // フォーカス移動
                            userControlCheckIoOnFormMain.textBoxTouchPanel.Focus();
                            // 末尾に文字列を加し、スクロール
                            userControlCheckIoOnFormMain.textBoxTouchPanel.AppendText(
                                String.Format("{0} X:{1} Y:{2} {3}",
                                    DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"),
                                    point.X, point.Y,
                                    Environment.NewLine));
                            //					// 最後尾へカーソル移動
                            //					textBoxTouchPanel.Select(textBoxTouchPanel.Text.Length, 0);
                        }));
                    };

                    // FormSubのMouseClickイベントを有効とする
                    formSub.boolEnableCallBackTouchPoint.Value = true;
                    // シリアルの受信デリゲートを無効にし、CheckIo画面用のデリゲートを設定
                    serialHelperPort.isEnableCallBackReceivedData.Value = false;
                    callbackReceivedDataSub = (str) =>
                    {
                        Invoke((MethodInvoker)(() =>
                        {
                            // ID code表示
                            //							 userControlCheckIoOnFormMain.textBoxIdCode.Text = str;
                            userControlCheckIoOnFormMain.textBoxIdCode.AppendText(str + Environment.NewLine);
                            userControlCheckIoOnFormMain.textBoxIdCode.Focus();
                        }));
                        // ID Code: 重複検査クリア
                        mIdCode0.Value = "";
                    };
                    // // シリアルの受信デリゲートを有効にする
                    serialHelperPort.isEnableCallBackReceivedData.Value = true;
                }
                // タイマ開始
                timerCheckIo.Start();
            }
            return;
        }

        /// <summary>
        /// CheckIoタイマ: ティック・ハンドラ
        /// </summary>
        private void timerCheckIo_TickSub(object sender, EventArgs e)
        {
            ushort l_ushortInValue = 0x0;
            bool l_boolBitState;
            IoBoardPortNo IoBoardPortNoTarget = IoBoardPortNo.PortRangeOver;

            foreach (CheckIoInPort l_CheckIoInPortObj in CheckIoInPortTbl)
            {
                // ポート番号が変化した時、ポートを読み出す
                if (l_CheckIoInPortObj.IoBoardPortNoPort != IoBoardPortNoTarget)
                {
                    if (!ioBoardDevice.DirectIn(l_CheckIoInPortObj.IoBoardPortNoPort, out l_ushortInValue))
                    {
                        Debug.WriteLine(ioBoardDevice.errorMsg);
                        // userControlOperationOnFormMain: 表示
                        if (Properties.Settings.Default.IS_CHECK_SERIAL_OPENED == true)
                        {
                            EndUcCheckIo();
                            VisibleUcMain();
                            return;
                        }
                    }
                }
                if ((l_ushortInValue & l_CheckIoInPortObj.ushortBitCode) == 0x0)
                {
                    l_boolBitState = false;
                }
                else
                {
                    l_boolBitState = true;
                }
                SetDiaplayStateOfTextBoxPortIn(l_CheckIoInPortObj.TextBoxPort, l_boolBitState);
            }
        }

        /// <summary>
        /// userControlCheckIoOnFormMain: 表示
        /// </summary>
        private void VisibleUcCheckIo()
        {
            // シリアル・ポート・オープンしていない時
            if (serialPortOpenFlag != true)
            {
                MessageBox.Show("COM port isn't openned", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (Properties.Settings.Default.IS_CHECK_SERIAL_OPENED == true)
                {
                    EndUcCheckIo();
                    VisibleUcMain();
                    return;
                }
            }

            // userControlCheckIoOnFormMain: 表示
            userControlMainOnFormMain.Visible = false;
            userControlOperationOnFormMain.Visible = false;
            userControlCheckDeviceOnFormMain.Visible = false;
            userControlCheckIoOnFormMain.Visible = true;
            userControlCheckIoOnFormMain.Dock = DockStyle.Fill;
            userControlPreferencesTabOnFormMain.Visible = false;
            userControlInputComOnFormMain.Visible = false;
            // Form.Text設定
            this.Text = "Check I/O";
        }

        private void EndUcCheckIo()
        {
            // タイマ停止
            timerCheckIo.Stop();
            // FormSubのMouseClickイベントを無効とする
            formSub.boolEnableCallBackTouchPoint.Value = false;
            formSub.callbackTouchPoint = (point) => { };
            // シリアルの受信デリゲートを無効にする
            serialHelperPort.isEnableCallBackReceivedData.Value = false;
            callbackReceivedDataSub = (str) => { };
        }

        private string GetSerialPortName(String stringSerialDeviceName)
        {
            var check = new System.Text.RegularExpressions.Regex("(COM[1-9][0-9]?[0-9]?)");
            if (check.IsMatch(stringSerialDeviceName))
            {
                return check.Match(stringSerialDeviceName).ToString();
            }
            return "";
        }

        public string[] GetSerialDeviceNames()
        {
            var deviceNameList = new System.Collections.ArrayList();
            var check = new System.Text.RegularExpressions.Regex("(COM[1-9][0-9]?[0-9]?)");

            ManagementClass mcPnPEntity = new ManagementClass("Win32_PnPEntity");
            ManagementObjectCollection manageObjCol = mcPnPEntity.GetInstances();

            //全てのPnPデバイスを探索しシリアル通信が行われるデバイスを随時追加する
            foreach (ManagementObject manageObj in manageObjCol)
            {
                //Nameプロパティを取得
                var namePropertyValue = manageObj.GetPropertyValue("Name");
                if (namePropertyValue == null)
                {
                    continue;
                }

                //Nameプロパティ文字列の一部が"(COM1)～(COM999)"と一致するときリストに追加"
                string name = namePropertyValue.ToString();
                if (check.IsMatch(name))
                {
                    deviceNameList.Add(name);
                }
            }

            //戻り値作成
            if (deviceNameList.Count > 0)
            {
                string[] deviceNames = new string[deviceNameList.Count];
                int index = 0;
                foreach (var name in deviceNameList)
                {
                    deviceNames[index++] = name.ToString();
                }
                return deviceNames;
            }
            else
            {
                return null;
            }
        }

        private void CheckBoxPortOnCheckIoOutPort_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox l_CheckBoxSender = (CheckBox)sender;
            CheckIoOutPort l_CheckIoOutPortTarget = null;
            ushort l_ushortOutValueForTarget;
            ushort l_ushortInValue;
            ushort l_ushortOutValue;
            bool l_boolFoundFlag = false;

            // イベント発信したチェック・ボックスを検索
            foreach (CheckIoOutPort l_CheckIoOutPortObj in CheckIoOutPortTbl)
            {
                if (l_CheckBoxSender == l_CheckIoOutPortObj.CheckBoxPort)
                {
                    l_CheckIoOutPortTarget = l_CheckIoOutPortObj;
                    l_boolFoundFlag = true;
                    break;
                }
            }
            // 登録ポートに対するチェック・ボックスでなかった時
            if (!l_boolFoundFlag)
            {
                return;
            }
            l_ushortOutValueForTarget = l_CheckIoOutPortTarget.ushortBitCode;
            //if (!ioBoardDevice.DirectIn(l_CheckIoOutPortTarget.IoBoardPortNoPort, out l_ushortInValue))
            //{
            //    MessageBox.Show(ioBoardDevice.errorMsg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    return;
            //}
            l_ushortInValue = ioBoardDevice.GetSendData(l_CheckIoOutPortTarget.IoBoardPortNoPort);

            if (l_CheckIoOutPortTarget.CheckBoxPort.Checked)
            {
                l_ushortOutValue = (ushort)(l_ushortInValue | l_ushortOutValueForTarget);
            }
            else
            {
                l_ushortOutValue = (ushort)(l_ushortInValue & (~l_ushortOutValueForTarget));
            }

            if (!ioBoardDevice.DirectOut(l_CheckIoOutPortTarget.IoBoardPortNoPort, l_ushortOutValue))
            {
                MessageBox.Show(ioBoardDevice.errorMsg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            SetDiaplayStateOfCheckBoxPortOut(l_CheckIoOutPortTarget.CheckBoxPort);
        }

        private void SetDiaplayStateOfCheckBoxPortOut(CheckBox CheckBoxPortOut)
        {
            if (CheckBoxPortOut.Checked)
            {
                CheckBoxPortOut.Text = "High";
                CheckBoxPortOut.ForeColor = Color.White;
                CheckBoxPortOut.BackColor = Color.Red;
            }
            else
            {
                CheckBoxPortOut.Text = "Low";
                CheckBoxPortOut.ForeColor = Color.White;
                CheckBoxPortOut.BackColor = Color.Black;
            }
        }

        private void SetDiaplayStateOfTextBoxPortIn(TextBox TextBoxPortIn, bool boolBitState)
        {
            if (boolBitState)   // Highの時
            {
                TextBoxPortIn.Text = "High";
                TextBoxPortIn.ForeColor = Color.White;
                TextBoxPortIn.BackColor = Color.Red;
            }
            else                // Lowの時
            {
                TextBoxPortIn.Text = "Low";
                TextBoxPortIn.ForeColor = Color.White;
                TextBoxPortIn.BackColor = Color.Black;
            }
        }
    }
}
