using System;
using System.Collections.Concurrent;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace Compartment
{
    class UcMain
    {
    }
    public partial class FormMain : Form
    {
        #region メンバ変数定義
        //		public PreferencesSet PreferencesSetOriginal = new PreferencesSet();
        //		private PreferencesSet PreferencesSetTemp= null;
        public PreferencesDat preferencesDatOriginal = new PreferencesDat();
        private PreferencesDat preferencesDatTemp = new PreferencesDat();
        private SerialHelper serialHelperPort = new SerialHelper();
        private FormSub formSub = null;
        public Bitmap bitmapCanvas;
#if DUMMY_IO
        // デバッグモードで実行時に初期化されます（FormMain_Load参照）
        public IoBoardBase ioBoardDevice = new IoMicrochipDummy();
#elif NORMAL
        public IoBoardBase ioBoardDevice = new IoBoard();
#else
        public IoBoardBase ioBoardDevice = new IoMicrochip();

#endif
        public EDoor eDoor;

        public CamImage camImage;

        FormProgress formProgress;

        private ConcurrentQueue<Point> concurrentQueueFromTouchPanel = new ConcurrentQueue<Point>();
        // シリアル受信時にコールされるデリゲートからコールされるサブ・デリゲート
        private Action<string> callbackReceivedDataSub = (str) => { };
        private String currentIdCode { get; set; } = "";
        private bool isErrorHappened = false;

        //シリアル・ポート・オープン・フラグ
        private bool serialPortOpenFlag = false;

        // デバッグモード用RFIDリーダー
        public RFIDReaderDummy rfidReaderDummy = null;

        // デバッグコントロールパネル
        private UserControlDebugPanel userControlDebugPanel = null;

        #endregion
        private void InitializeComponentOnUcMain()
        {
            #region ボタンClickイベント・ハンドラ設定
            userControlMainOnFormMain.buttonOperationOnUserControlMain.Click += (object sender, EventArgs e) =>
            {
                // UserControlMain.buttonOperationOnUserControlMain: Click
                // userControlOperationOnFormMain: 表示
                VisibleUcOperation();
            };
            userControlMainOnFormMain.buttonCheckDeviceOnUserControlMain.Click += (object sender, EventArgs e) =>
            {
                // UserControlMain.buttonCheckDeviceOnUserControlMain: Click
                // userControlCheckDeviceOnFormMain: 表示
                VisibleUcCheckDevice();
            };

            userControlMainOnFormMain.buttonCheckIoOnUserControlMain.Click += (object sender, EventArgs e) =>
            {
                // UserControlMain.buttonCheckIoOnUserControlMain: Click
                // userControlCheckIoOnFormMain: 表示
                VisibleUcCheckIo();
            };
            userControlMainOnFormMain.buttonPreferencesOnUserControlMain.Click += (object sender, EventArgs e) =>
            {
                // UserControlMain.buttonPreferencesOnUserControlMain: Click
                // userControlPreferencesOnFormMain: 表示
                VisibleUcPreferencesTab();
            };
            userControlMainOnFormMain.buttonEndOnUserControlMain.Click += (object sender, EventArgs e) =>
            {
                // 設定をファイルへ保存
                if (SavePreference(preferencesDatOriginal) != true)
                {
                    MessageBox.Show("Preference file save error", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                if (!formScript.Visible)
                    Close();
            };
            userControlMainOnFormMain.buttonBlockProgramming.Click += (object sender, EventArgs e) =>
            {
                formScript?.Show();
                formScript?.ReloadCurrentActionParam();
                formScript?.Activate();

            };

            // デバッグモードチェックボックスのイベントハンドラ
            userControlMainOnFormMain.checkBoxEnableDebugMode.CheckedChanged += (object sender, EventArgs e) =>
            {
                // チェックボックスの状態をpreferencesDatOriginalに反映
                preferencesDatOriginal.EnableDebugMode = userControlMainOnFormMain.checkBoxEnableDebugMode.Checked;

                // 設定を保存
                SavePreference(preferencesDatOriginal);

                // メッセージを表示
                if (preferencesDatOriginal.EnableDebugMode)
                {
                    MessageBox.Show("デバッグモードが有効になりました。\nアプリケーションを再起動してください。", "デバッグモード", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("デバッグモードが無効になりました。\nアプリケーションを再起動してください。", "デバッグモード", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            };
            #endregion
        }

        /// <summary>
        /// userControlMainOnFormMain: 表示
        /// </summary>
        private void VisibleUcMain()
        {
            // userControlMainOnFormMain: 表示
            userControlMainOnFormMain.Visible = true;
            userControlMainOnFormMain.Dock = DockStyle.Fill;
            userControlOperationOnFormMain.Visible = false;
            userControlCheckDeviceOnFormMain.Visible = false;
            userControlCheckIoOnFormMain.Visible = false;
            userControlPreferencesTabOnFormMain.Visible = false;
            userControlInputComOnFormMain.Visible = false;

            // デバッグモードチェックボックスの状態を設定
            userControlMainOnFormMain.checkBoxEnableDebugMode.Checked = preferencesDatOriginal.EnableDebugMode;

            // デバッグモード時は実機を必要とする機能を無効化
            if (preferencesDatOriginal.EnableDebugMode)
            {
                userControlMainOnFormMain.buttonCheckDeviceOnUserControlMain.Enabled = false;
                userControlMainOnFormMain.buttonCheckIoOnUserControlMain.Enabled = false;
            }
            else
            {
                userControlMainOnFormMain.buttonCheckDeviceOnUserControlMain.Enabled = true;
                userControlMainOnFormMain.buttonCheckIoOnUserControlMain.Enabled = true;
            }

            // Form.Text設定
            this.Text = GetTextOfFormMain();
        }
        public void ToggleCloseButton(bool n)
        {
            userControlMainOnFormMain.buttonEndOnUserControlMain.Enabled = n;
            userControlMainOnFormMain.buttonOperationOnUserControlMain.Enabled = n;
            userControlMainOnFormMain.buttonPreferencesOnUserControlMain.Enabled = n;

            // デバッグモード時はCheck Device/Check IOボタンを常に無効
            if (preferencesDatOriginal.EnableDebugMode)
            {
                userControlMainOnFormMain.buttonCheckDeviceOnUserControlMain.Enabled = false;
                userControlMainOnFormMain.buttonCheckIoOnUserControlMain.Enabled = false;
            }
            else
            {
                userControlMainOnFormMain.buttonCheckDeviceOnUserControlMain.Enabled = n;
                userControlMainOnFormMain.buttonCheckIoOnUserControlMain.Enabled = n;
            }
        }
        /// <summary>
        /// ForｍMainのTextへ表示する文字列を取得する
        /// </summary>
        /// <returns>
        /// ForｍMainのTextへ表示する文字列
        /// </returns>
        private String GetTextOfFormMain()
        {
            String stringText;

            //自分自身のAssemblyを取得
            System.Reflection.Assembly assembly = Assembly.GetExecutingAssembly();
            System.Reflection.AssemblyName asmName = assembly.GetName();
            System.Version version = asmName.Version;

            stringText = "Compartment" + version.ToString();

            return stringText;
        }
    }
}
