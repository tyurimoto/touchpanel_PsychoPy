using System;
using System.Windows.Forms;

namespace Compartment
{
    public class MultiPurpose
    {
        public Action callbackStart = () => { };
        public Action callbackStop = () => { };
        public void Setup()
        {
            callbackStart();
        }

        public void Start()
        {
            callbackStart();
        }

        public void Stop()
        {
            callbackStop();
        }
    }

    public partial class FormMain : Form
    {
        private MultiPurpose multiPurpose = new MultiPurpose();

        private void SetupPurpose()
        {
            multiPurpose.callbackStart = () =>
            {
                LoadSettings();
            };

            multiPurpose.callbackStop = () =>
            {
                SaveSettings();
            };

            multiPurpose.Setup();
        }

        public Action callbackLoadedSettings = () => { };

        public Action callbackSavingSettings = () => { };

        /// <summary>
        /// 設定の呼び出し
        /// </summary>
        private void LoadSettings()
        {
            // preferencesDatOriginal.ComPort = Properties.Settings.Default.COM_PORT;
            // preferencesDatOriginal.ComBaudRate = Properties.Settings.Default.COM_BAUDRATE;
            callbackLoadedSettings();
        }

        /// <summary>
        /// 設定の保存
        /// </summary>
        private void SaveSettings()
        {
            // Properties.Settings.Default.COM_PORT = preferencesDatOriginal.ComPort;
            // Properties.Settings.Default.COM_BAUDRATE = preferencesDatOriginal.ComBaudRate;

            callbackSavingSettings();

            // ここで設定を保存する
            Properties.Settings.Default.Save();
        }
    }

}
