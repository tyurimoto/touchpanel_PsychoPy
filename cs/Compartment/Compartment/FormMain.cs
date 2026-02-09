#define BG_WORKER       //サル二倍速用
#define BG_WORKER2       //サル二倍速用

using BlockProgramming;
// TODO: APIサーバー実装時に有効化
// using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Owin.Hosting;
using Compartment.Services;
using Compartment.Controllers;

namespace Compartment
{
    public partial class FormMain : Form
    {
        public OpeImage opImage;
        public EpisodeMemory episodeMemory;
        public UcOperationDataStore ucOperationDataStore;
        public IdControlHelper idControlHelper;
        public IdControlHelper recentIdHelper;
        SyncObject<bool> runBg1 = new SyncObject<bool>(true);
        SyncObject<bool> runBg2 = new SyncObject<bool>(true);
        FormScript formScript;
        private Random globalRandomValue;
        public UcOperationBlock uob;
        private const string episodeBaseIdFileName = "baseid.json";
        private const string recentIdFileName = "recentid.ids";

        // OWIN Web API server
        private IDisposable _apiServer;
        internal HardwareService _hardwareService;

        public bool Feeding { get => devFeed.Feeding; }
        public FormMain()
        {
            InitializeComponent();

            this.Size = new Size(1920, 1080);

            SetupPurpose();
            this.Shown += (sender, e) => { multiPurpose.Start(); };

            InitializeComponentOnUcMain();
            InitializeComponentOnUcOperation();
            InitializeComponentOnUcCheckDevice();
            InitializeComponentOnUcCheckIo();
            InitializeComponentOnUcPreferencesTab();
#if DEBUG
            userControlOperationOnFormMain.buttonDebugEnter.Visible = true;
            userControlOperationOnFormMain.buttonDebugLeave.Visible = true;
#endif

            //SettingSearcher.SearchSetting();

            uob = new UcOperationBlock(this);

            //** ↓2025/01 多層化対応↓ **//
            ucOperationDataStore = new UcOperationDataStore();

            // userControlMainOnFormMain: 表示
            VisibleUcMain();
            // 注意：イベント定義とイベント登録を同時に行う例→ユーザ・コントロール:userControlMainOnFormMainを生成後でないとアクセスできない
            //			userControlMainOnFormMain.buttonEndOnUserControlMain.Click += (object sender, EventArgs e) =>
            //			{
            //				DialogResult l_dialogresultResult = MessageBox.Show("Do you end this program", "End Program", MessageBoxButtons.OKCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2);
            //				if (l_dialogresultResult == DialogResult.Yes)
            //				{
            //					this.Close();
            //				}
            //			};
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            // ビルド確認用ログ（このログが表示されたら、リビルドが正しく反映されています）
            System.Diagnostics.Debug.WriteLine("=================================================");
            System.Diagnostics.Debug.WriteLine($"[BUILD VERIFICATION] FormMain_Load - ビルド日時: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            System.Diagnostics.Debug.WriteLine("=================================================");

            // フォーム・テキストへ表示
            this.Text = GetTextOfFormMain();

            formSub = new FormSub();
            opImage = new OpeImage(formSub, formSub.pictureBoxOnFormSub, preferencesDatOriginal);
            globalRandomValue = new Random();

            // Progress
            formProgress = new FormProgress();
            formProgress.Show();
            formProgress.Activate();
            formProgress.Refresh();
            this.Enabled = false;
            this.Refresh();

            switch (Program.SelectedEngine)
            {
                case EEngineType.BlockProgramming:
                    try
                    {
                        formScript = new FormScript();
                        formScript.formParent = this;
                        userControlMainOnFormMain.buttonBlockProgramming.Visible = true;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    //formScript.Show();

                    //以前の状態復帰
                    try
                    {
                        //** ↓2025/01 多層化対応↓ **//
                        //ucOperationDataStore = new UcOperationDataStore();

                        // データの読み込み
                        List<string> ids = ucOperationDataStore.GetIDs();
                        foreach (string id in ids)
                        {
                            FileRelatedActionParam fileRelatedActionParams = ucOperationDataStore.GetEntry(id);
                            fileRelatedActionParams.UpdateActionParam();
                            if (fileRelatedActionParams != null)
                            {
                                uob.JsonToOperationProc(id, fileRelatedActionParams.ActionParams);
                                uob.OperationProcToJson(id, fileRelatedActionParams.FilePath);
                            }
                        }

                        //if (File.Exists("latestOperationProc.json"))
                        //{
                        //    string latestJsonFilePath = "latestOperationProc.json";
                        //    var json = File.ReadAllText(latestJsonFilePath);
                        //    if (json != "")
                        //    {
                        //        uob.JsonToOperationProc(json);
                        //    }

                        //}
                        //** ↑2025/01 多層化対応↑ **//
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }

                    HighlightScriptSetting(true);
                    break;

                case EEngineType.PsychoPy:
                    // PsychoPyエンジンはFormScriptやBlock Programmingボタン不要
                    userControlMainOnFormMain.buttonBlockProgramming.Visible = false;
                    System.Diagnostics.Debug.WriteLine("[PsychoPy] Engine selected - API server mode");
                    break;

                default:
                    // 旧エンジン削除済み
                    userControlMainOnFormMain.buttonBlockProgramming.Visible = false;
                    break;
            }

            foreach (var screen in System.Windows.Forms.Screen.AllScreens)
            {
                Debug.WriteLine("Device Name: {0}", screen.DeviceName);
                Debug.WriteLine("Bounds: {0}", screen.Bounds);
                Debug.WriteLine("Type: {0}", screen.GetType());
                Debug.WriteLine("Working Area: {0}", screen.WorkingArea);
                Debug.WriteLine("Primary Screen: {0}", screen.Primary);
                Debug.WriteLine("");
                //              if(screen.DeviceName == "\\\\.\\DISPLAY2")
                //              Windowsの設定のディスプレイ設定をプライマリ/セカンダリに変更しても
                //              上記のディスプレイ名は変わらず、固定的な名前が割り当てられるよう
                if (!screen.Primary)
                {
                    formSub.TopMost = true;
                    formSub.StartPosition = FormStartPosition.Manual;
                    //                    l_formFormSub.Left = screen.Bounds.Left;
                    //                    l_formFormSub.Top = screen.Bounds.Top;
                    formSub.Location = new Point(screen.Bounds.Left, screen.Bounds.Top);
                    formSub.ShowIcon = false;
                    formSub.ShowInTaskbar = false;
                    formSub.MaximizeBox = false;
                    formSub.MinimizeBox = false;
                    formSub.FormBorderStyle = FormBorderStyle.None;
                    formSub.BackColor = Color.Black;
                    Debug.WriteLine("Before: formSub.Left:{0} formSub.Top:{1} screen.Bounds.Left:{2} Top:{3}",
                                        formSub.Left, formSub.Top,
                                        screen.Bounds.Left, screen.Bounds.Top);
                    formSub.WindowState = FormWindowState.Maximized;
                    formSub.Show();
                    //                  l_formFormSub.WindowState = FormWindowState.Maximized;
                    Debug.WriteLine("After: formFormSub.Left:{0} formFormSub.Top:{1} screen.Bounds.Left:{2} Top:{3}",
                                        formSub.Left, formSub.Top,
                                        screen.Bounds.Left, screen.Bounds.Top);
                    // 解像度自動取得
                    opImage.WidthOfWholeArea = screen.Bounds.Width;
                    opImage.HeightOfWholeArea = screen.Bounds.Height;
                    Debug.WriteLine("formsubFormSub.pictureBoxOnFormSub.Width:{0} formsubFormSub.pictureBoxOnFormSub.Height:{1}",
                                        this.formSub.pictureBoxOnFormSub.Width,
                                        this.formSub.pictureBoxOnFormSub.Height);
                    bitmapCanvas = new Bitmap(this.formSub.pictureBoxOnFormSub.Width, this.formSub.pictureBoxOnFormSub.Height);

                    // サブディスプレイを発見できた時、サブ・ディスプレイ存在フラグをセット
                    opImage.IsThereSubDisplay = true;
                    break;
                }
            }

            // 設定を読み出す
            if (LoadPreference(out preferencesDatTemp) != true)
            {
                string oldSetting = SettingSearcher.SearchSetting();

                if (System.IO.File.Exists(oldSetting))
                {
                    if (DialogResult.Yes == MessageBox.Show("設定ファイルが見つかりません。前のバージョンを設定を引き継ぎますか？", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1))
                    {
                        LoadPreference(oldSetting, out preferencesDatTemp);

                        preferencesDatOriginal = preferencesDatTemp.Clone();
                    }
                }
                else
                {
                    // preferencesDatOriginalはデフォルト値のままとなる
                    MessageBox.Show("Preference file load error, so use default setting", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }


            }
            else
            {
                // 読み込んだ設定をオリジナルへコピー
                preferencesDatOriginal = preferencesDatTemp.Clone();
            }
            // 設定値反映クラス化弊害
            opImage.UpdatePreferences(preferencesDatOriginal);

            // eDoor有効時
            eDoor = new EDoor(ioBoardDevice, this);

            //var t = ShowSplashWindow(null);
            //t.Wait();
            //Invoke((MethodInvoker)(() => { fp.Activate(); }));

            if (!CheckRuntime("msvcp100.dll"))
            {
                this.Activate();
                MessageBox.Show("VC++2010RedistributionLibraryがインストールされていません", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
            if (!CheckRuntime("msvcp140.dll"))
            {
                this.Activate();
                MessageBox.Show("VC++2015-22RedistributionLibraryがインストールされていません", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            if (!OpeImage.CheckSvgImage())
            {
                this.Activate();
                isErrorHappened = true;
                MessageBox.Show("SVG画像の読み込みに失敗しました\nプログラムフォルダ内SVG画像フォルダを確認してください。", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }

            Stopwatch camSw = Stopwatch.StartNew();
            var camInitializeTask = new Task(() => { camImage = new CamImage(this); });
            camInitializeTask.Start();
            //camImage = new CamImage(this);
            camInitializeTask.Wait();
            camSw.Stop();
            Debug.WriteLine("caminitilize :" + camSw.ElapsedMilliseconds.ToString());

            // Preference読み込み後に
            InitializeIdContoroler();

            // デバッグモード初期化
            if (preferencesDatOriginal.EnableDebugMode)
            {
                InitializeIoBoardForDebugMode();
                InitializeRFIDReaderForDebugMode();
                // デバッグモードではシリアルポートとIOボードの実機初期化をスキップ
                serialPortOpenFlag = false;
                this.Activate();
            }
            else
            {
                // 通常モード: シリアル・ポート初期化
                serialHelperPort.ComPort = preferencesDatOriginal.ComPort;
                //serialHelperPort.BaudRate = preferencesDatOriginal.ComBaudRate;
                serialHelperPort.BaudRate = "9600";
                serialHelperPort.DataBits = preferencesDatOriginal.ComDataBitLength;
                serialHelperPort.StopBits = preferencesDatOriginal.ComStopBitLength;
                serialHelperPort.Handshake = System.IO.Ports.Handshake.None.ToString();
                serialHelperPort.Parity = preferencesDatOriginal.ComParity;
                InitSerialPort();
                // IO開く前にサブからこちらへ
                this.Activate();

                // IOボード: デバイス取得
                try
                {

                    if (!ioBoardDevice.AcquireDevice())
                    {
                        // IOビヘイビアモデル
                        ioBoardDevice = null;
                        this.Activate();
                        MessageBox.Show("IOオープンエラー ビヘイビアモデルで起動");
                        ioBoardDevice = new IoMicrochipDummy();
                    }
                    else
                    {
                        ioBoardDevice.SetMotorSpeed(preferencesDatOriginal.TimeToOutputSoundOfCorrect);
                        eDoor.Start();
                    }
                }
                catch (Exception)
                {
                    // IOビヘイビアモデル
                    ioBoardDevice = null;
                    MessageBox.Show("IOオープンエラー ビヘイビアモデルで起動");
                    ioBoardDevice = new IoMicrochipDummy();
                }

                try
                {
                    if (serialHelperPort.Open() != true)
                    {
                        serialPortOpenFlag = false;
                        MessageBox.Show("COM port open error", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        serialPortOpenFlag = true;
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("シリアルポートが見つかりません\\nアプリケーションを終了します。", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //アプリケーションを終了する
                    isErrorHappened = true;
                    Application.Exit();
                }
            }
            // サブ・ディスプレイが存在しない時
            if (opImage.IsThereSubDisplay != true)
            {
                // デバッグモードではサブディスプレイなしでも動作可能
                if (!preferencesDatOriginal.EnableDebugMode)
                {
                    MessageBox.Show("Sub display isn't found", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    Debug.WriteLine("[Debug] サブディスプレイが見つかりませんが、デバッグモードのため続行します");
                }
            }

            try
            {
                devFeed = new DevFeedMulti(ioBoardDevice, this, 1);
                devFeed.UpdateFeedStatus = x => UpdateFeedStatus(x);
                devFeed2 = new DevFeedMulti(ioBoardDevice, this, 2);
                devFeed2.UpdateFeedStatus = x => UpdateFeed2Status(x);
            }
            catch (Exception)
            {
                throw;
            }

            try
            {
#if !BG_WORKER
			timerOperationStateMachine.Enabled = true;
#else
                timerOperationStateMachine.Enabled = false;
                backgroundWorker1.RunWorkerAsync();
#endif

#if !BG_WORKER2
			timerOperation.Enabled = true;
#else
                timerOperation.Enabled = false;
                backgroundWorker2.RunWorkerAsync();
#endif
            }
            catch (Exception)
            {
                throw;
            }
            // 自分自身(FormMain)をアクティブにする(理由:FormaSubへフォーカスが行ってしまい、メッセージ・ボックス表示がFormSubに表示されてしまったそれを回避する為)
            this.Activate();
            formProgress?.Close();
            this.Enabled = true;

            // Start OWIN Web API server
            StartApiServer();

            // デバッグモードチェックボックスの状態を設定ファイルから反映
            userControlMainOnFormMain.checkBoxEnableDebugMode.Checked = preferencesDatOriginal.EnableDebugMode;

            // デバッグモード時のデバッグコントロールパネル表示
            if (preferencesDatOriginal.EnableDebugMode)
            {
                // デバッグコントロールパネルと動作画面を分割表示
                // メインの左右分割
                SplitContainer splitContainerMain = new SplitContainer();
                splitContainerMain.Dock = DockStyle.Fill;
                splitContainerMain.Orientation = Orientation.Vertical;
                splitContainerMain.SplitterDistance = this.ClientSize.Width - 500; // 右側500pxをデバッグパネルに
                splitContainerMain.FixedPanel = FixedPanel.Panel2; // 右パネルを固定サイズに
                splitContainerMain.IsSplitterFixed = false; // スプリッターを移動可能に

                // 左側パネルをさらに上下に分割
                SplitContainer splitContainerLeft = new SplitContainer();
                splitContainerLeft.Dock = DockStyle.Fill;
                splitContainerLeft.Orientation = Orientation.Horizontal;
                splitContainerLeft.SplitterDistance = this.ClientSize.Height - 150; // 下側150pxを動物シミュレーターに
                splitContainerLeft.FixedPanel = FixedPanel.Panel2; // 下パネルを固定サイズに
                splitContainerLeft.IsSplitterFixed = false;

                // 左上: 既存のpanelBaseを移動
                this.Controls.Remove(panelBase);
                splitContainerLeft.Panel1.Controls.Add(panelBase);
                panelBase.Dock = DockStyle.Fill;
                splitContainerLeft.Panel1.AutoScroll = true;

                // 左下: 動物位置シミュレーターを追加
                UserControlAnimalSimulator animalSimulator = new UserControlAnimalSimulator(this);
                animalSimulator.Dock = DockStyle.Fill;
                splitContainerLeft.Panel2.Controls.Add(animalSimulator);
                splitContainerLeft.Panel2.AutoScroll = true;

                // 左側の上下分割をメインの左側に配置
                splitContainerMain.Panel1.Controls.Add(splitContainerLeft);

                // 右側: デバッグコントロールパネルを追加
                userControlDebugPanel = new UserControlDebugPanel(this);
                userControlDebugPanel.Dock = DockStyle.Fill;
                splitContainerMain.Panel2.Controls.Add(userControlDebugPanel);
                splitContainerMain.Panel2.AutoScroll = true;

                // メインSplitContainerをフォームに追加
                this.Controls.Add(splitContainerMain);
                splitContainerMain.BringToFront();
            }

            // デバッグモード時は実機を必要とする機能を無効化
            if (preferencesDatOriginal.EnableDebugMode)
            {
                userControlMainOnFormMain.buttonCheckDeviceOnUserControlMain.Enabled = false;
                userControlMainOnFormMain.buttonCheckIoOnUserControlMain.Enabled = false;
                Debug.WriteLine("[Debug] Check device/Check IO ボタンを無効化しました（デバッグモード）");
            }

            opCollection.callbackMessageDebug("アプリ起動");
            return;
        }
        public void InitializeIdContoroler()
        {
            episodeMemory = new EpisodeMemory("epsave.json");
            idControlHelper = new IdControlHelper(episodeBaseIdFileName);
            recentIdHelper = new IdControlHelper(recentIdFileName);
            //IdControlHelper null時 強制作成
            if (idControlHelper is null)
            {
                idControlHelper = new IdControlHelper();
            }
            if (recentIdHelper is null)
            {
                recentIdHelper = new IdControlHelper();
            }
            // インスタンス作成直後にCheckExpireする エントリが多量にExpireしていた場合時間がかかるため

            // ID有効期間確認
            if (idControlHelper.FindId(opCollection.idCode))
            {
                idControlHelper.CheckExpire();
                if (!idControlHelper.FindId(opCollection.idCode))
                {
                    // なければエントリ削除？
                    episodeMemory.RemoveEntry(opCollection.idCode);
                }
            }
            else
            {
                // なければエントリ削除
                episodeMemory.RemoveEntry(opCollection.idCode);
            }

            idControlHelper.ExpireTime = preferencesDatOriginal.EpisodeExpireTime;

            // RecentID有効期間確認
            recentIdHelper.ExpireTime = 100;
            recentIdHelper.CheckExpire();
        }

        /// <summary>
        /// Start OWIN Web API server for PsychoPy integration
        /// </summary>
        private void StartApiServer()
        {
            try
            {
                // Get port number from preferences (default: 5000)
                int port = preferencesDatOriginal?.ApiServerPort ?? 5000;
                string baseAddress = $"http://localhost:{port}/";

                // Initialize hardware service
                _hardwareService = new HardwareService(this);

                // Initialize all controllers
                SensorController.Initialize(_hardwareService);
                DoorController.Initialize(_hardwareService);
                LeverController.Initialize(_hardwareService);
                FeedController.Initialize(_hardwareService);
                RFIDController.Initialize(_hardwareService);
                DebugController.Initialize(_hardwareService);
                EmergencyController.Initialize(_hardwareService);
                RoomController.Initialize(_hardwareService);
                LampController.Initialize(_hardwareService);
                SoundController.Initialize(_hardwareService);

                // Start OWIN server
                _apiServer = WebApp.Start<Startup>(baseAddress);

                Debug.WriteLine($"[API] Web API server started at {baseAddress}");
                opCollection?.callbackMessageDebug($"API起動: {baseAddress}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[API] Failed to start Web API server: {ex.Message}");
                MessageBox.Show($"API起動エラー: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Stop OWIN Web API server
        /// </summary>
        private void StopApiServer()
        {
            try
            {
                if (_apiServer != null)
                {
                    _apiServer.Dispose();
                    _apiServer = null;
                    Debug.WriteLine("[API] Web API server stopped");
                    opCollection?.callbackMessageDebug("API停止");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[API] Error stopping Web API server: {ex.Message}");
            }
        }

        // ID Code重複排除用にID Codeを保存
        public SyncObject<String> mIdCode0 = new SyncObject<String>("");
        private void InitSerialPort()
        {
            // TODO: APIサーバー実装時にデバッグモードを有効化
            // Skip normal RFID initialization if debug mode is enabled
            // if (!preferencesDatOriginal.EnableDebugMode)
            {
                RFIDReaderHelper rFIDReaderHelper = new RFIDReaderHelper();
                rFIDReaderHelper.callbackReceivedDataSub += (x) => { callbackReceivedDataSub(x); };
                // シリアル受信デリゲートを設定
                serialHelperPort.callbackReceivedDatagram = rFIDReaderHelper.GetUnivrsalIDAction();
                // rfidReaderHelperメンバ変数への代入
                rfidReaderHelper = rFIDReaderHelper;
            }

            //serialHelperPort.ComPort = "COM4";
            serialHelperPort.StopBits = "Two";
            serialHelperPort.BaudRate = "9600";
            serialHelperPort.Parity = "None";

            serialHelperPort.Init(serialPort1);
        }

        /// <summary>
        /// デバッグモード用IOボード初期化
        /// </summary>
        private void InitializeIoBoardForDebugMode()
        {
            // 既存のioBoardDeviceを破棄
            if (ioBoardDevice != null)
            {
                try
                {
                    ioBoardDevice.ReleaseDevice();
                }
                catch { }
            }

            // デバッグモードタイプに応じて初期化
            if (preferencesDatOriginal.DebugModeType == EDebugModeType.FullDummy)
            {
                // 完全ダミーモード
                ioBoardDevice = new IoMicrochipDummyEx();
                ioBoardDevice.AcquireDevice();
                Debug.WriteLine("デバッグモード（完全ダミー）で起動しました");
            }
            else if (preferencesDatOriginal.DebugModeType == EDebugModeType.Hybrid)
            {
                // ハイブリッドモード（将来実装）
                // TODO: IoHybridBoard実装後に有効化
                // ioBoardDevice = new IoHybridBoard(useRealHardware: true);
                ioBoardDevice = new IoMicrochipDummyEx();
                ioBoardDevice.AcquireDevice();
                Debug.WriteLine("デバッグモード（ハイブリッド - 未実装のため完全ダミー）で起動しました");
            }

            // eDoorを初期化
            eDoor = new EDoor(ioBoardDevice, this);
            eDoor.Start();
        }

        /// <summary>
        /// デバッグモード用RFIDリーダー初期化
        /// </summary>
        private void InitializeRFIDReaderForDebugMode()
        {
            // ダミーRFIDリーダーを作成（メンバー変数に保存）
            rfidReaderDummy = new RFIDReaderDummy();
            rfidReaderDummy.callbackReceivedDataSub += (x) => { callbackReceivedDataSub(x); };

            // シリアルポートのコールバックを設定（実際には呼ばれないが、互換性のため）
            serialHelperPort.callbackReceivedDatagram = rfidReaderDummy.GetUnivrsalIDAction();

            Debug.WriteLine("デバッグモード用RFIDリーダーを初期化しました");
        }

        /// <summary>
        /// 設定タブ タスク選択有効切り替え
        /// </summary>
        /// <param name="n"></param>
        public void EnableChangeTask(bool n)
        {
            userControlPreferencesTabOnFormMain.comboBoxTypeOfTask.Enabled = n;
        }
        public void HighlightScriptSetting(bool n)
        {
            if (n)
            {
                //userControlPreferencesTabOnFormMain.groupBoxTask.BackColor = Color.DarkGray;
                userControlPreferencesTabOnFormMain.groupBoxDisplay.BackColor = Color.DarkGray;
                userControlPreferencesTabOnFormMain.groupBoxInterval.BackColor = Color.DarkGray;
                userControlPreferencesTabOnFormMain.groupBoxTimingSetting.BackColor = Color.DarkGray;
                userControlPreferencesTabOnFormMain.groupBoxRandomTime.BackColor = Color.DarkGray;
                userControlPreferencesTabOnFormMain.groupBoxTrigger.BackColor = Color.DarkGray;
                //userControlPreferencesTabOnFormMain.groupBoxEndSound.BackColor = Color.DarkGray;
                //userControlPreferencesTabOnFormMain.tableLayoutPanelEndSound.CellPaint += new TableLayoutCellPaintEventHandler((sender, e) =>
                //{
                //    if (e.Row == 1)
                //    {
                //        e.Graphics.FillRectangle(Brushes.DarkGray, e.CellBounds);
                //    }
                //});
                userControlPreferencesTabOnFormMain.tableLayoutPanelRewardSound.CellPaint += new TableLayoutCellPaintEventHandler((sender, e) =>
                {
                    if (e.Row == 1)
                    {
                        e.Graphics.FillRectangle(Brushes.DarkGray, e.CellBounds);
                    }
                });
                //userControlPreferencesTabOnFormMain.tableLayoutPanelCorrectSound.CellPaint += new TableLayoutCellPaintEventHandler((sender, e) => 
                //{
                //    if (e.Row == 1)
                //    {
                //        e.Graphics.FillRectangle(Brushes.DarkGray, e.CellBounds);
                //    }
                //});
                userControlPreferencesTabOnFormMain.tableLayoutPanel7.CellPaint += new TableLayoutCellPaintEventHandler((sender, e) =>
                {
                    if (e.Row == 0)
                    {
                        e.Graphics.FillRectangle(Brushes.DarkGray, e.CellBounds);
                    }
                });

            }
            else
            {
                userControlPreferencesTabOnFormMain.groupBoxTask.BackColor = Color.Transparent;
                userControlPreferencesTabOnFormMain.groupBoxDisplay.BackColor = Color.Transparent;
                userControlPreferencesTabOnFormMain.groupBoxInterval.BackColor = Color.Transparent;
                userControlPreferencesTabOnFormMain.groupBoxTimingSetting.BackColor = Color.Transparent;
                userControlPreferencesTabOnFormMain.groupBoxRandomTime.BackColor = Color.Transparent;
            }

        }

        private void timerCheckIo_Tick(object sender, EventArgs e)
        {
            timerCheckIo_TickSub(sender, e);
        }

        private void timerOperation_Tick(object sender, EventArgs e)
        {
#if !BG_WORKER2
			timerOperation_TickSub(sender, e);
#endif

        }

        private void serialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            serialHelperPort.OnDataReceived2(500);
        }

        private void timerOperationStateMachine_Tick(object sender, EventArgs e)
        {
#if !BG_WORKER
            // BG_WORKER未定義の場合、タイマーから呼ばれる
            OnOperationStateMachineProc();
#endif
        }
        private void StopBgWorkerOperation()
        {
#if BG_WORKER
            runBg1.Value = false;
#endif

#if BG_WORKER2

            runBg2.Value = false;
#endif
            multiPurpose.Stop();

            // IOボード・デバイス開放
            ioBoardDevice.ReleaseDevice();
            // シリアル・ポート: クローズ
            serialHelperPort.Close();
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (isErrorHappened == true)
            {
                StopBgWorkerOperation();
            }
            else
            {
                if (MessageBox.Show("End this program ?", "Infomation", MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    StopBgWorkerOperation();
                }
                else
                {
                    e.Cancel = true;
                    return;
                }

                recentIdHelper.SaveId(recentIdFileName);
            }

            // eDoorの停止（BackgroundWorker停止後、他のDispose前に呼ぶ）
            eDoor?.Dispose();

            ucOperationDataStore?.Dispose();
            camImage?.Dispose();

            // Disable EventLogger if enabled
            _hardwareService?.EventLogger?.Disable();

            // Stop Web API server
            StopApiServer();

            opCollection.callbackMessageDebug("アプリ終了");

            if (Program.SelectedEngine == EEngineType.BlockProgramming)
            {
                try
                {
                    formScript?.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        static object syncObjectSplash = new object();
        private Task ShowSplashWindow(FormMain mainForm)
        {
            lock (syncObjectSplash)
            {
                if (mainForm != null)
                {
                }
                ManualResetEvent splashShownEvent = new System.Threading.ManualResetEvent(false);

                Task progresstask = new Task(() =>
                {
                    formProgress = new FormProgress();
                    formProgress.Show();
                });
                progresstask.Start();
                return progresstask;
            }
        }
        private void CloseSplash()
        {
            lock (syncObjectSplash)
            {
                formProgress.Close();
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("[backgroundWorker1_DoWork] Started!");

            Action M_OperationProc = () => { };
            UcOperationPsychoPy ucOperationPsychoPy = null;

            switch (Program.SelectedEngine)
            {
                case EEngineType.BlockProgramming:
                    UcOperationInternal ucOperationInternal = new UcOperationInternal(this);
                    M_OperationProc = () => { ucOperationInternal.OnOperationStateMachineProc(); };
                    System.Diagnostics.Debug.WriteLine("[backgroundWorker1_DoWork] Using Block Programming engine");
                    break;

                case EEngineType.PsychoPy:
                    ucOperationPsychoPy = new UcOperationPsychoPy(this);
                    M_OperationProc = () => { ucOperationPsychoPy.OnOperationStateMachineProc(); };
                    System.Diagnostics.Debug.WriteLine("[backgroundWorker1_DoWork] Using PsychoPy engine");
                    break;

                default:
                    // 旧エンジン削除済み: デフォルトはBlockを使用
                    goto case EEngineType.BlockProgramming;
            }
#if BG_WORKER
            System.Diagnostics.Debug.WriteLine("[backgroundWorker1_DoWork] BG_WORKER is DEFINED - using background worker loop");
#else
            System.Diagnostics.Debug.WriteLine("[backgroundWorker1_DoWork] BG_WORKER is NOT DEFINED - background worker will do nothing!");
#endif
#if BG_WORKER
            try
            {
                do
                {
                    M_OperationProc();
                    System.Threading.Thread.Sleep(1);
                } while (runBg1.Value);
            }
            catch (Exception ex)
            {
                opCollection.callbackMessageError(ex.Message);
                //MessageBox.Show(ex.Message);
            }
            finally
            {
                // PsychoPyエンジン: Pythonプロセスが残っている場合にKill
                ucOperationPsychoPy?.Cleanup();
            }
#endif

        }

        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
#if BG_WORKER2
            //var stopwatch = new Stopwatch();
            //var loopCount = 0;
            try
            {
                do
                {
                    //if(loopCount<20)
                    //	stopwatch.Start();

                    timerOperation_TickSub(sender, e);
                    System.Threading.Thread.Sleep(1);

                    //if (loopCount < 120)
                    //{
                    //	Debug.WriteLine("Loop speed : " + stopwatch.ElapsedMilliseconds + "ms");
                    //	stopwatch.Restart();
                    //	loopCount++;
                    //}



                } while (runBg2.Value);
            }
            catch (Exception ex)
            {
                opCollection.callbackMessageError(ex.Message);
                //MessageBox.Show(ex.Message);
            }
#endif
        }
        public static bool CheckRuntime(string systemFile)
        {
            string sysdir = System.Environment.GetFolderPath(System.Environment.SpecialFolder.System);
            string path = sysdir + "\\" + systemFile;
            bool ret = File.Exists(path);
            return ret;
        }
    }
}
