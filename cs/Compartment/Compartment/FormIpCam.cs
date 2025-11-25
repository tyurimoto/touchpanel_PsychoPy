using Compartment.media1;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Discovery;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Media;
using LibVLCSharp.Shared;
using System.Reflection;
using System.Windows.Media.Imaging;
using System.Net.Http;
using System.Windows;
using System.Diagnostics;

namespace Compartment
{
    public partial class FormIpCam : Form
    {

        private static uint Pitch;
        private static uint Lines;
        private const uint BytePerPixel = 4;
        private const uint width = 1280;
        private const uint height = 720;

        // vlcコントロール用
        DirectoryInfo vlcPath = new DirectoryInfo(Path.Combine(Directory.GetCurrentDirectory(), "libvlc", IntPtr.Size == 4 ? "win-x86" : "win-x64"));

        private static readonly ConcurrentQueue<(MemoryMappedFile file, MemoryMappedViewAccessor accessor)> FilesToProcess = new ConcurrentQueue<(MemoryMappedFile file, MemoryMappedViewAccessor accessor)>();

        private const int splitterOffset = 10;

        private const int discoveryTimeout = 1000000;

        readonly ParentHelper<FormMain> mainForm = new ParentHelper<FormMain>();
        public FormIpCam(FormMain baseForm)
        {
            mainForm.SetParent(baseForm);

            InitializeComponent();
            splitContainer1.SplitterDistance = comboBoxMedia.Right + splitterOffset;
            pictureBox1.Visible = false;
            Stopwatch sw = Stopwatch.StartNew();
            // DiscoveryClientでCam発見されるまで待つ
            while (!mainForm.Parent.camImage.DevicesDiscovered)
            {
                Thread.Sleep(1);
                if (sw.ElapsedMilliseconds > discoveryTimeout)
                {
                    System.Windows.Forms.MessageBox.Show("カメラ検索エラー");
                    return;
                }
            }
            // デバイスの追加・削除・変更時
            mainForm.Parent.camImage.DeviceUris.CollectionChanged += (object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) =>
            {
                comboBoxDevices.Items.Clear();
                foreach (var uri in mainForm.Parent.camImage.DeviceUris)
                {
                    comboBoxDevices.Items.Add("CAM" + comboBoxDevices.Items.Count);
                }
            };
            // 初期デバイス追加
            if (mainForm.Parent.camImage.DeviceUris.Count > 0)
            {
                foreach (var uri in mainForm.Parent.camImage.DeviceUris)
                {
                    comboBoxDevices.Items.Add("CAM" + comboBoxDevices.Items.Count);
                }
            }

            // プロファイルの追加・削除・変更時
            mainForm.Parent.camImage.Profiles.CollectionChanged += (object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) =>
            {
                comboBoxMedia.Items.Clear();
                foreach (var profile in mainForm.Parent.camImage.Profiles)
                {
                    comboBoxMedia.Items.Add(profile.Name);
                }
                if (comboBoxMedia.Items.Count > 0)
                {
                    comboBoxMedia.SelectedIndex = 0;
                }
            };

            vlcControl1.VlcMediaPlayer.Log += VlcMediaPlayer_Log;
            vlcControl1.VlcMediaPlayer.Stopped += VlcMediaPlayer_Stopped;
            vlcControl1.EndInit();

        }

        ~FormIpCam()
        {
            Console.WriteLine("FormIpCam Disposed.");
            vlcControl1.Dispose();
        }
        /// <summary>
        /// 領域確保32byte揃え用
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        uint Align(uint size)
        {
            if (size % 32 == 0)
            {
                return size;
            }

            return ((size / 32) + 1) * 32;// Align on the next multiple of 32
        }

        private void EnableButton(object sender, bool buttonEnable)
        {
            if (sender == buttonCaptureImage)
            {
                //buttonCaptureImage.Enabled = n;
                buttonPlay.Enabled = buttonEnable;
                buttonDetectionTest.Enabled = buttonEnable;
            }
            else if (sender == buttonDetectionTest)
            {
                buttonCaptureImage.Enabled = buttonEnable;
                buttonPlay.Enabled = buttonEnable;
                //buttonDetectionTest.Enabled = n;
            }
            else if (sender == buttonPlay)
            {
                buttonCaptureImage.Enabled = buttonEnable;
                //buttonPlay.Enabled = n;
                buttonDetectionTest.Enabled = buttonEnable;
            }
        }

        private void FormIpCam_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (mainForm.Parent.camImage.CaptureStarted)
                mainForm.Parent.camImage.StopSavingFromCamImage();
            if (mainForm.Parent.camImage.DetectionStarted)
                mainForm.Parent.camImage.StopDetectFromCamImage();
            Debug.WriteLine("FormIpCam closing.");
            vlcControl1.VlcMediaPlayer.Stop();
        }

        private void VlcMediaPlayer_Stopped(object sender, Vlc.DotNet.Core.VlcMediaPlayerStoppedEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)(() => { buttonPlay.Text = "Play"; }));
            }
            else
            {
                buttonPlay.Text = "Play";
            }
        }

        private void VlcMediaPlayer_Log(object sender, Vlc.DotNet.Core.VlcMediaPlayerLogEventArgs e)
        {
            Debug.WriteLine(string.Format("libVlc : {0} {1} @ {2}", e.Level, e.Message, e.Module));
        }

        // 選択されているプロファイルでストリーミング
        private void StreamVideoOnVLC(string[] recordParams)
        {
            if (mainForm.Parent.camImage.Media != null)
            {
                var setup = new media1.StreamSetup();
                setup.Stream = media1.StreamType.RTPMulticast;
                setup.Transport = new media1.Transport();
                setup.Transport.Protocol = media1.TransportProtocol.RTSP;
                UriBuilder uri = null;
                if (comboBoxMedia.Items.Count > 0)
                {
                    uri = new UriBuilder(mainForm.Parent.camImage.Media.GetStreamUri(setup, mainForm.Parent.camImage.Profiles.ElementAt(comboBoxMedia.SelectedIndex).token).Uri);

                    vlcControl1.VlcMediaPlayer.Play(uri.Uri, recordParams);
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("カメラが見つかりません");
                }
            }
        }

        /// <summary>
        /// 単純再生テスト
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonPlay_Click(object sender, EventArgs e)
        {
            var option = new List<string>();
            option.Add("--demux h264");
            if (mainForm.Parent.camImage.Media == null || comboBoxMedia.Items.Count == 0)
            {
                System.Windows.Forms.MessageBox.Show("カメラが見つかりません");
                return;
            }
            if (!vlcControl1.IsPlaying)
            {
                StreamVideoOnVLC(option.ToArray());
                ((Button)sender).Text = "Stop";

                EnableButton(sender, false);
            }
            else
            {
                vlcControl1.Stop();
                EnableButton(sender, true);
            }
        }
        private void OnVlcControlNeedsLibDirectory(object sender, Vlc.DotNet.Forms.VlcLibDirectoryNeededEventArgs e)
        {
            var currentAssembly = System.Reflection.Assembly.GetEntryAssembly();
            var currentDirectory = new FileInfo(currentAssembly.Location).DirectoryName;
            if (currentDirectory == null)
                return;
            e.VlcLibDirectory = vlcPath;
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            mainForm.Parent.camImage.ConnectCam(mainForm.Parent.camImage.DeviceUris.ElementAt(comboBoxDevices.SelectedIndex));
        }
        private bool EnableCaptureImage = false;
        /// <summary>
        /// 画像切り出し保存テスト
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonCaptureImage_Click(object sender, EventArgs e)
        {
            var setup = new media1.StreamSetup();
            setup.Stream = media1.StreamType.RTPMulticast;
            setup.Transport = new media1.Transport();
            setup.Transport.Protocol = media1.TransportProtocol.RTSP;
            if (!(mainForm.Parent.camImage.Profiles.Count > 0)) return;
            //UriBuilder uri = new UriBuilder(mainForm.Parent.camImage.Media.GetStreamUri(setup, mainForm.Parent.camImage.Profiles.ElementAt(comboBoxMedia.SelectedIndex).token).Uri);
            UriBuilder uri = null;
            if (comboBoxMedia.Items.Count > 0)
            {
                uri = new UriBuilder(mainForm.Parent.camImage.Media.GetStreamUri(setup, mainForm.Parent.camImage.Profiles.ElementAt(comboBoxMedia.SelectedIndex).token).Uri);
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Camが見つかりません。");
                return;
            }

            mainForm.Parent.camImage.CaptureStopped += CamImageCapure_Stopped;
            if (!EnableCaptureImage)
            {
                mainForm.Parent.camImage.StartSavingFromCamImage(uri.ToString(), false);
                EnableCaptureImage = true;
                buttonCaptureImage.Text = "Stop";

                EnableButton(sender, false);
            }
            else
            {
                mainForm.Parent.camImage.StopSavingFromCamImage();
                EnableCaptureImage = false;

                EnableButton(sender, true);
            }

        }
        private void CamImageCapure_Stopped(object sender, EventArgs e)
        {
            buttonCaptureImage.Text = "Cam to Bmp";
        }
        /// <summary>
        /// BitmapSource領域確保テスト
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BitmapSoruceTest()
        {
            List<System.Windows.Media.Color> colors = new List<System.Windows.Media.Color>();
            int pixelsSize = (int)(width * height * 4);
            byte[] pixels = new byte[pixelsSize];

            // バイト列に色情報を入れる
            byte value = 0;
            for (int x = 0; x < width * height * 4; x += 4)
            {
                byte blue = value;
                byte green = value;
                byte red = value;
                byte alpha = 255;
                pixels[x] = blue;
                pixels[x + 1] = green;
                pixels[x + 2] = red;
                pixels[x + 3] = alpha;
                if (value < 255)
                {
                    value++;
                }
                else
                {
                    value = 0;
                }
            }

            // バイト列をBitmapSourceに変換する
            int stride = ((int)width * PixelFormats.Pbgra32.BitsPerPixel + 7) / 8;
            colors.Add(System.Windows.Media.Colors.Blue);
            colors.Add(System.Windows.Media.Colors.Green);
            colors.Add(System.Windows.Media.Colors.Red);
            BitmapPalette myPalette = new BitmapPalette(colors);
            BitmapSource bitmaps = BitmapSource.Create((int)width, (int)height, 96, 96, PixelFormats.Pbgra32, null, pixels.ToArray(), stride);
        }

        private async void JsonTimeSetRequestTest()
        {
            string url = "http://admin@192.168.0.142/cgi-bin/jvsweb.cgi";
            string timeStr = DateTime.Now.ToString("yyyy-MM-dd+HH:mm:ss");
            //"2015-01-01+09:09:44\
            string jsonData = "{\"type\":\"H8EV200-N4\",\"product\":\"JVS-HI3518ESIV200\",\"version\":\"V2.5.613\",\"acDevName\":\"HD+IPC\",\"nickName\":\"\",\"sn\":1122326,\"ystID\":795808659,\"nLanguage\":1,\"date\":\"2015-01-01+09:09:44\",\"bSntp\":0,\"sntpInterval\":24,\"ntpServer\":\"ntp.nict.jp\",\"enableStreamWatchDog\":1,\"tz\":9,\"bDST\":0,\"bIPSelfAdapt\":0,\"bEnablePrivateProto\":0,\"rebootDay\":0,\"rebootHour\":1,\"bRestriction\":1,\"portUsed\":\"\",\"osdText\":[\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\"],\"osdX\":0,\"osdY\":0,\"osdSize\":32,\"lcmsServer\":\"\",\"bEnableCalc\":0,\"bHomeIpc\":0,\"streamCnt\":2,\"bviWidth\":1920,\"bviHeight\":1080,\"versionTime\":\"V2.5.613+-+20180426+14:51:57\",\"ystGroupID\":\"SY795808659\",\"timeFormatInt\":1,\"nPort\":9101,\"nWebPort\":80,\"bShowOSD\":1,\"timeFormatStr\":\"" + timeStr + "\",\"position\":1,\"timePos\":2,\"channelName\":\"HD+IPC\",\"osdbInvColEn\":0,\"bLargeOSD\":0,\"IVPFuncSupport\":0,\"PTZBsupport\":0,\"bCropViX\":0,\"bCropViY\":0,\"bCropViW\":1920,\"bCropViH\":1080,\"bEightPrivacy\":0,\"bFaceDectSupport\":0}";
            string action = "websettime&";
            string cmd = "webipcinfo&param=";
            using (var client = new HttpClient())
            {

                var geturi1 = url + "?" + "action=" + "list" + "&cmd=" + "webipcinfo";

                var request = new HttpRequestMessage(HttpMethod.Get, geturi1);
                request.Headers.Add(@"Authorization", @"Basic ");
                request.Headers.Add("ContentType", "application/json");
                var response = await client.SendAsync(request);


                var geturi = url + "?" + "action=" + action + "cmd=" + cmd + jsonData;
                var requestTimeSet = new HttpRequestMessage(HttpMethod.Get, geturi);
                requestTimeSet.Headers.Add(@"Authorization", @"Basic ");
                requestTimeSet.Headers.Add("ContentType", "application/json");
                requestTimeSet.Headers.Add("Accept-Encoding", "gzip, deflate");
                requestTimeSet.Headers.Add("Cookie", "lang=un-us; curAccount=admin; curPwd=; status=1");
                requestTimeSet.Headers.Add("Referer", "http://192.168.0.142/");

                response = await client.SendAsync(requestTimeSet);
                Debug.WriteLine(response.Content.ToString());

                string _response = await response.Content.ReadAsStringAsync();
                //POSTリクエスト
                //var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                //var res = await client.PostAsync(url, content);

                //取得
                //var _response = await res.Content.ReadAsStringAsync();
                //Console.WriteLine(_response.ToString());
            }
        }

        private void buttonOpenSaveImageFolder_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer", mainForm.Parent.camImage.ImageSavePath);
        }

        private void FormIpCam_SizeChanged(object sender, EventArgs e)
        {
            splitContainer1.SplitterDistance = comboBoxMedia.Right + splitterOffset;
        }

        private bool EnableDetectMovie = false;
        private void buttonDetectionTest_Click(object sender, EventArgs e)
        {
            var filePath = string.Empty;
            if (!EnableDetectMovie)
            {
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.InitialDirectory = System.Windows.Forms.Application.StartupPath;
                    openFileDialog.Filter = "mp4 files(*.mp4) | *.mp4";
                    openFileDialog.FilterIndex = 1;
                    openFileDialog.RestoreDirectory = true;

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        //Get the path of specified file
                        filePath = openFileDialog.FileName;
                        Task task = new Task(() => { mainForm.Parent.camImage.StartSavingFromCamImage(filePath, true, true); });
                        task.Start();
                        EnableDetectMovie = true;

                        buttonDetectionTest.Text = "Stop";

                        EnableButton(sender, false);
                    }
                }
            }
            else
            {
                mainForm.Parent.camImage.StopSavingFromCamImage();
                buttonDetectionTest.Text = "detect on local video";
                EnableDetectMovie = false;

                EnableButton(sender, true);
            }
        }
    }
}
