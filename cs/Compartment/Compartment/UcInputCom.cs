using System;
using System.Windows.Forms;

namespace Compartment
{
    class UcInputCom
    {
    }
    public partial class FormMain : Form
    {
        private void InitializeComponentOnUcInputCom()
        {
            userControlInputComOnFormMain.buttonCOMSense.Click += (object sender, EventArgs e) =>
            {
                // COM portコンボ・ボックス選択候補設定
                // 項目クリア
                userControlInputComOnFormMain.comboBoxComPort.Items.Clear();
                String[] stringComPort = serialHelperPort.GetSerialDeviceNames();

                if (stringComPort != null && stringComPort[0] != String.Empty)
                {
                    foreach (String l_stringComPort in stringComPort)
                    {
                        userControlInputComOnFormMain.comboBoxComPort.Items.Add(GetSerialPortName(l_stringComPort));
                    }
                }
                else
                {
                    MessageBox.Show("There is no COM port", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                // 初期値:未設定状態
                userControlInputComOnFormMain.comboBoxComPort.SelectedIndex = -1;
            };
            userControlInputComOnFormMain.buttonCOMSense.Click += (object sender, EventArgs e) =>
            {
                // UserControlCheckIo.buttonEnd: Click
                // userControlOperationOnFormMain: 表示
                VisibleUcMain();
            };
        }

        /// <summary>
        /// userControlMainOnFormMain: 表示
        /// </summary>
        private void VisibleUcInputCom()
        {
            // userControlMainOnFormMain: 表示
            userControlMainOnFormMain.Visible = false;
            userControlOperationOnFormMain.Visible = false;
            userControlCheckDeviceOnFormMain.Visible = false;
            userControlCheckIoOnFormMain.Visible = false;
            userControlPreferencesTabOnFormMain.Visible = false;
            userControlInputComOnFormMain.Visible = true;
            userControlInputComOnFormMain.Dock = DockStyle.Fill;
            // Form.Text設定
            this.Text = "COM input";
        }
    }

}
