using Compartment.media1;
using LibVLCSharp.Shared;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.ServiceModel.Channels;
using System.ServiceModel;
using System.ServiceModel.Discovery;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Darknet;
using System.Windows.Forms;
using System.Windows.Controls;
using System.Diagnostics;
using System.Drawing;

namespace Compartment
{
    public class CamImage : IDisposable
    {
        private static uint Pitch;
        private static uint Lines;
        private uint width = 1280;
        private uint height = 720;

        static DiscoveryEndpoint endpoint;
        DiscoveryClient discoveryClient;
        FindCriteria findCriteria;

        public readonly ObservableCollection<UriBuilder> DeviceUris = new ObservableCollection<UriBuilder>();
        public readonly ObservableCollection<Profile> Profiles = new ObservableCollection<Profile>();
        public MediaClient Media;

        public bool DevicesDiscovered { get; private set; } = false;


        LibVLC libVlc = new LibVLC(enableDebugLogs: false);
        public readonly LibVLCSharp.Shared.MediaPlayer MediaPlayer;

        private static MemoryMappedFile CurrentMappedFile;
        private static MemoryMappedViewAccessor CurrentMappedViewAccessor;
        private static readonly ConcurrentQueue<(MemoryMappedFile file, MemoryMappedViewAccessor accessor)> FilesToProcess = new ConcurrentQueue<(MemoryMappedFile file, MemoryMappedViewAccessor accessor)>();
        private static readonly ConcurrentQueue<BitmapSource> bitmapSources = new ConcurrentQueue<BitmapSource>();
        private static readonly ConcurrentQueue<YoloWrapper.bbox_t> detectedBBoxs = new ConcurrentQueue<YoloWrapper.bbox_t>();
        private static long FrameCounter = 0;

        private CancellationTokenSource ProcessCancelationToken;

        readonly ParentHelper<FormMain> mainForm = new ParentHelper<FormMain>();

        static SyncObject<int> _CameraIntervalFrame = new SyncObject<int>(); // Frame

        public int ResolutionX { get; private set; }
        public int ResolutionY { get; private set; }
        public int FramerateLimit { get; private set; }


        static int CameraIntervalFrame { get => _CameraIntervalFrame.Value; set => _CameraIntervalFrame.Value = value; }

        private EventHandler<LogEventArgs> libVlcLogEventHandler = new EventHandler<LogEventArgs>((s, eventArgs) =>
        {
            // Vlc Log出力
            //Debug.WriteLine(eventArgs.Message);
        });
        /// <summary>
        /// 画像保存用インターバル
        /// </summary>
        private static int ShotCamInterval; // milisecond

        private static SyncObject<int> _detectNum = new SyncObject<int>();

        /// <summary>
        /// 検知匹数
        /// </summary>
        public static int DetectNum { get => _detectNum.Value; private set => _detectNum.Value = value; }

        private static SyncObject<int> _lastDetectNum = new SyncObject<int>(0);

        public static int LastDetectNum { get => _lastDetectNum.Value; private set => _lastDetectNum.Value = value; }

        private SyncObject<bool> _CamOpened = new SyncObject<bool>(false);
        /// <summary>
        /// カメラオープンフラグ
        /// </summary>
        public bool CamOpened { get => _CamOpened.Value; private set => _CamOpened.Value = value; }

        private SyncObject<bool> _DetectionStarted = new SyncObject<bool>(false);

        private Task taskDiscovery;
        /// <summary>
        /// Detectionスタートフラグ
        /// </summary>
        public bool DetectionStarted { get => _DetectionStarted.Value; private set => _DetectionStarted.Value = value; }

        private SyncObject<bool> _CaptureStarted = new SyncObject<bool>(false);

        public static bool CaptureEnable { get => _CaptureEnable.Value; set => _CaptureEnable.Value = value; }
        private static SyncObject<bool> _CaptureEnable = new SyncObject<bool>(false);
        /// <summary>
        /// Captureスタートフラグ
        /// </summary>
        public bool CaptureStarted { get => _CaptureStarted.Value; private set => _CaptureStarted.Value = value; }

        private YoloWrapper Yolo;

        private ref PreferencesDat PreferencesDatOriginal
        {
            get
            {
                return ref mainForm.Parent.preferencesDatOriginal;
            }
        }

        public event EventHandler CaptureStopped;

        private readonly object YoloDetectSyncObject = new object();
        SyncObject<bool> yoloDetectRunningFlag = new SyncObject<bool>(true);

        public string ImageSavePath { get => PreferencesDatOriginal.LearningSaveImageFolder; }
        /// <summary>
        /// CameraSave or Detect タイミング設定 Preferenceから
        /// </summary>
        public void SetCamIntervalToFrame()
        {
            SetCamIntervalToFrame(PreferencesDatOriginal.CameraFrameRate);
        }
        /// <summary>
        /// CameraSave or Detectタイミング設定
        /// </summary>
        /// <param name="frame"></param>
        public void SetCamIntervalToFrame(int frame)
        {
            ShotCamInterval = PreferencesDatOriginal.ShotCamInterval;
            var framePerSecond = frame * (ShotCamInterval / 1000.0);
            CameraIntervalFrame = (int)framePerSecond;
        }
        /// <summary>
        /// CamImage コンストラクタ
        /// </summary>
        /// <param name="baseForm">base Form</param>
        public CamImage(FormMain baseForm, bool enableYolo = true)
        {
            mainForm.SetParent(baseForm);

            width = (uint)PreferencesDatOriginal.CameraResolutionX;
            height = (uint)PreferencesDatOriginal.CameraResolutionY;

            Pitch = AlignSize(width * (uint)PixelFormats.Bgra32.BitsPerPixel / 8);
            Lines = AlignSize(height);
            ProcessCancelationToken = new CancellationTokenSource();
            try
            {
                MediaPlayer = new LibVLCSharp.Shared.MediaPlayer(libVlc);
                MediaPlayer.Stopped += MediaPlayerStopped;
#if DEBUG
                //libVlc.Log += libVlcLogEventHandler;
#endif
            }
            catch (Exception)
            {
                throw;
            }
            //Task task = new Task(() => { InitializeDiscoverClient(); });
            taskDiscovery = new Task(() => { InitializeDiscoverClient(); });
            taskDiscovery.Start();
            SetCamIntervalToFrame();

            if (enableYolo)
            {
                // Yolo初期化
                YoloInit();
            }
        }
        public void StopMediaPlay()
        {
            if (MediaPlayer.IsPlaying)
            {
                MediaPlayer.Stop();
            }
        }
        /// <summary>
        /// ディスカバリクライアント初期化
        /// OnvifDevice探索を行う
        /// ディスカバリーで探すの時間かかるのでTask化してほっとく
        /// </summary>
        private void InitializeDiscoverClient()
        {
            // WS-Discoveryの機能を利用してOnvif対応カメラを探す
            endpoint = new UdpDiscoveryEndpoint(DiscoveryVersion.WSDiscoveryApril2005);
            discoveryClient = new DiscoveryClient(endpoint);
            discoveryClient.FindProgressChanged += (object sender, FindProgressChangedEventArgs e) =>
            {
                Debug.WriteLine(e.EndpointDiscoveryMetadata.ListenUris.FirstOrDefault());
                DeviceUris.Add(new UriBuilder(e.EndpointDiscoveryMetadata.ListenUris.FirstOrDefault()));
            };

            findCriteria = new FindCriteria();
            findCriteria.Duration = TimeSpan.MaxValue;
            findCriteria.MaxResults = 20;
            findCriteria.ContractTypeNames.Add(new System.Xml.XmlQualifiedName("NetworkVideoTransmitter", "http://www.onvif.org/ver10/network/wsdl"));

            discoveryClient.FindAsync(findCriteria);
            DevicesDiscovered = true;
        }
        /// <summary>
        /// Yolo 初期化
        /// </summary>
        public void YoloInit()
        {
            try
            {
                string cfg = System.IO.Path.Combine(Application.StartupPath, "darknet", "yolov7-tiny_marmo_test.cfg");
                string weights;
                if (File.Exists(PreferencesDatOriginal.LearningModelPath))
                {
                    weights = PreferencesDatOriginal.LearningModelPath;
                }
                else
                {
                    weights = System.IO.Path.Combine(Application.StartupPath, "darknet", "yolov7-tiny_marmo_training_1400000.weights");
                }
                Yolo = new YoloWrapper(cfg, weights, 0);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void YoloInit(string config, string weights, int maxResults = 0)
        {
            Yolo = new YoloWrapper(config, weights, maxResults);
        }
        /// <summary>
        /// ディスカバリ待ちタスク
        /// </summary>
        public void WaitDiscovery()
        {
            if (taskDiscovery != null)
            {
                taskDiscovery.Wait();

                Task task = new Task(() =>
                {
                    while (DeviceUris.Count == 0)
                    {
                        Thread.Sleep(100);
                    }
                });
                task.Start();
            }
        }
        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            CamOpened = false;
            if (MediaPlayer != null)
            {
                if (MediaPlayer.IsPlaying)
                    MediaPlayer.Stop();
                //MediaPlayer.Dispose();
            }
            if (libVlc != null)
            {
#if DEBUG
                //libVlc.Log -= libVlcLogEventHandler;
#endif
                libVlc.Dispose();
            }
            //GC.Collect();
            //GC.WaitForPendingFinalizers();
            Yolo?.Dispose();
        }
        /// <summary>
        /// カメラ接続
        /// 
        /// Onvif device_serviceよりストリーミングのプロファイルを取得する
        /// 接続時にPC側と時計を合わせる
        /// </summary>
        /// <param name="deviceUri">deviceURI</param>
        public void ConnectCam(UriBuilder deviceUri)
        {
            System.ServiceModel.Channels.Binding binding;
            HttpTransportBindingElement httpTransport = new HttpTransportBindingElement();
            httpTransport.AuthenticationScheme = System.Net.AuthenticationSchemes.Digest;
            binding = new CustomBinding(new TextMessageEncodingBindingElement(MessageVersion.Soap12WSAddressing10, Encoding.UTF8), httpTransport);
            Stopwatch stopwatch = Stopwatch.StartNew();
            try
            {
                Devices.DeviceClient device = new Devices.DeviceClient(binding, new EndpointAddress(deviceUri.ToString()));
                stopwatch.Stop();
                mainForm.Parent.opCollection.callbackMessageDebug("Device: " + stopwatch.ElapsedMilliseconds.ToString() + "ms");
                stopwatch.Restart();
                Devices.Service[] services = device.GetServices(false);
                stopwatch.Stop();
                mainForm.Parent.opCollection.callbackMessageDebug("Service: " + stopwatch.ElapsedMilliseconds.ToString() + "ms");
                stopwatch.Restart();
                if (services.Any(s => s.Namespace == "http://www.onvif.org/ver10/media/wsdl"))
                {
                    Media = new MediaClient(binding, new EndpointAddress(deviceUri.ToString()));
                    Profiles.Clear();

                    foreach (media1.Profile profile in Media.GetProfiles())
                    {
                        Profiles.Add(profile);
                    }
                    stopwatch.Stop();
                    mainForm.Parent.opCollection.callbackMessageDebug("Profile: " + stopwatch.ElapsedMilliseconds.ToString() + "ms");

                }
                // 時刻合わせ
                SetCamDateTime(device);

                // Camera情報取得
                GetCameraInfo(Profiles[0]);
                SetCamIntervalToFrame(FramerateLimit);
                CamOpened = true;
            }

            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                //camera 未接続時ここにくる

                throw;
            }


        }
        private void GetCameraInfo(Profile profile)
        {
            ResolutionX = profile.VideoEncoderConfiguration.Resolution.Width;
            ResolutionY = profile.VideoEncoderConfiguration.Resolution.Height;
            FramerateLimit = GetFramerateLimit(profile);
        }
        private int GetFramerateLimit(Profile profile)
        {
            return profile.VideoEncoderConfiguration.RateControl.FrameRateLimit;
        }
        /// <summary>
        /// ONVIF経由でカメラに時間を設定
        /// </summary>
        /// <param name="device">Devices.DeviceClient</param>
        private void SetCamDateTime(Devices.DeviceClient device)
        {
            Devices.TimeZone tz = new Devices.TimeZone
            {
                TZ = "JST-9"
            };
            device.SetSystemDateAndTime(Devices.SetDateTimeType.Manual, false, tz, GetDeviceDateTimeNow());

        }

        /// <summary>
        /// Deivces.DateTimeをDateTimeから取得
        /// </summary>
        /// <returns></returns>
        private Devices.DateTime GetDeviceDateTimeNow()
        {
            DateTime _now = DateTime.UtcNow;
            Devices.DateTime dt = new Devices.DateTime();
            dt.Date = new Devices.Date();
            dt.Time = new Devices.Time();
            dt.Date.Year = _now.Year;
            dt.Date.Month = _now.Month;
            dt.Date.Day = _now.Day;
            dt.Time.Hour = _now.Hour;
            dt.Time.Minute = _now.Minute;
            dt.Time.Second = _now.Second;

            return dt;
        }
        /// <summary>
        /// 取得データ用Size揃え
        /// </summary>
        /// <param name="size">size</param>
        /// <returns>aligned size</returns>
        private uint AlignSize(uint size)
        {
            if (size % 32 == 0)
            {
                return size;
            }
            return ((size / 32) + 1) * 32;// Align on the next multiple of 32
        }
        /// <summary>
        /// Image save用
        /// Bgra->Rgbしてからバウンディングボックスを描画保存
        /// </summary>
        /// <param name="destination"></param>
        /// <param name="token"></param>
        /// <param name="frameNum"></param>
        /// <returns></returns>
        private async Task ProcessSaveImageAsync(string destination, CancellationToken token, int frameNum, YoloWrapper yolo, bool enableBoundingBox)
        {
            var frameNumber = frameNum;
            byte[] pixels = new byte[height * width * PixelFormats.Bgra32.BitsPerPixel / 8];
            byte[] rgb;
            int detectNum = 0;

            Stopwatch sw = new Stopwatch();

            List<System.Windows.Media.Color> colors = new List<System.Windows.Media.Color>
            {
                Colors.Red,
                Colors.Green,
                Colors.Blue
            };
            BitmapPalette imagePalette = new BitmapPalette(colors);
            bitmapSources.Clear();
            while (!token.IsCancellationRequested)
            {
                try
                {
                    if (FilesToProcess.TryDequeue(out var file))
                    {
                        using (var sourceStream = file.file.CreateViewStream())
                        {
                            // UnmanagedSourceSteamから読み出し
                            _ = sourceStream.Read(pixels, 0, pixels.Length);

                            sw.Restart();
                            rgb = ConvertBgraToRgb(pixels);
                            sw.Stop();
                            Debug.WriteLine("Byte trans: " + sw.ElapsedMilliseconds + "ms");
                            // 検出してBBox描画
                            sw.Restart();
                            YoloWrapper.bbox_t[] detectedBoundingBox = null;
                            if (yolo != null)
                            {
                                lock (YoloDetectSyncObject)
                                {
                                    detectedBoundingBox = yolo.Detect(rgb, (int)width, (int)height, PreferencesDatOriginal.DetectThreshold);
                                }
                            }
                            else
                            {

                            }

                            sw.Stop();
                            Debug.WriteLine("Detect time: " + sw.ElapsedMilliseconds + "ms");


                            int stride = ((int)width * PixelFormats.Bgra32.BitsPerPixel / 8);
                            BitmapSource bitmaps = BitmapSource.Create((int)width, (int)height, 96, 96, PixelFormats.Bgr32, imagePalette, pixels.ToArray(), stride);
                            if (yolo != null && enableBoundingBox)
                            {
                                SaveDetectImageFile(bitmaps, detectedBoundingBox, destination, frameNumber);
                            }
                            else if (yolo != null)
                            {
                                // Detectionするけど画像保存
                                detectNum = yolo.DetectCount;

                                //Detect状況Log画面出力
                                if (LastDetectNum != detectNum)
                                {
                                    mainForm.Parent.opCollection.callbackMessageNormal("Detect : " + yolo.DetectCount.ToString() + " marmosets   DetectTime: " + sw.ElapsedMilliseconds + "ms");
                                    LastDetectNum = detectNum;
                                }
                                sw.Restart();
                                //簡単ピークホールド
                                if (DetectNum < detectNum && detectNum > 0)
                                {
                                    DetectNum = detectNum;
                                }
                                SaveDetectImageFile(bitmaps, null, destination, frameNumber);
                            }
                            else
                            {
                                SaveDetectImageFile(bitmaps, null, destination, frameNumber);
                            }
                        }
                        file.accessor.Dispose();
                        file.file.Dispose();
                        frameNumber++;
                    }
                    else
                    {
                        await Task.Delay(TimeSpan.FromMilliseconds(1), token);
                    }
                    Thread.Sleep(1);
                }
                catch (TaskCanceledException)
                {

                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        /// <summary>
        /// Image Save部抽出
        /// </summary>
        /// <param name="bitmapSource">bitmapSoruce</param>
        /// <param name="detectedBoundingBox">検出バウンディングボックス</param>
        /// <param name="destination">出力パス</param>
        /// <param name="frameNumber">フレーム番号</param>
        private void SaveDetectImageFile(BitmapSource bitmapSource, YoloWrapper.bbox_t[] detectedBoundingBox, string destination, int frameNumber)
        {
            //画像保存
            Console.WriteLine($"Writing {frameNumber:0000000}.jpg");
            var fileName = Path.Combine(destination, $"{frameNumber:0000000}.jpg");
            RenderTargetBitmap target;
            if (detectedBoundingBox != null)
            {
                WriteableBitmap writeableBitmap = new WriteableBitmap(bitmapSource);
                DrawingVisual drawingVisual = new DrawingVisual();
                using (var content = drawingVisual.RenderOpen())
                {
                    content.DrawImage(writeableBitmap, new System.Windows.Rect(0, 0, writeableBitmap.PixelWidth, writeableBitmap.PixelHeight));
                    var p = new System.Windows.Media.Pen(new SolidColorBrush(Colors.Cyan), 3);
                    for (int i = 0; i < Yolo.DetectCount; i++)
                    {
                        content.DrawRectangle(new SolidColorBrush(), p, new System.Windows.Rect(detectedBoundingBox[i].x, detectedBoundingBox[i].y, detectedBoundingBox[i].w, detectedBoundingBox[i].h));
                    }
                }
                target = new RenderTargetBitmap((int)width, (int)height, 96, 96, PixelFormats.Pbgra32);
                target.Render(drawingVisual);
            }
            else
            {
                target = null;
            }
            using (var outputFile = File.Open(fileName, FileMode.Create))
            {
                var encoder = new WmpBitmapEncoder();

                if (detectedBoundingBox != null)
                {
                    encoder.Frames.Add(BitmapFrame.Create(target));
                }
                else
                {
                    encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
                }
                encoder.Save(outputFile);
            }
        }
        /// <summary>
        /// Bgra -> RGB byte[] 720pくらいならforのが早い
        /// </summary>
        /// <param name="inputArray"></param>
        /// <param name="heightSize"></param>
        /// <param name="widthSize"></param>
        /// <returns></returns>
        public static byte[] ConvertBgraToRgb(byte[] inputArray, uint heightSize, uint widthSize)
        {
            byte[] rgb = new byte[heightSize * widthSize * PixelFormats.Rgb24.BitsPerPixel / 8];
            for (int p = 0; p < heightSize * widthSize; p++)
            {
                rgb[p * 3] = inputArray[p * 4 + 2];
                rgb[p * 3 + 1] = inputArray[p * 4 + 1];
                rgb[p * 3 + 2] = inputArray[p * 4];
            }
            return rgb;

        }
        /// <summary>
        /// Bgra-> RGB byte[] Parallel変換
        /// 画像サイズがすごくでかいと早いかも
        /// </summary>
        /// <param name="inputArray"></param>
        /// <param name="heightSize"></param>
        /// <param name="widthSize"></param>
        /// <returns></returns>
        public static byte[] ConvertBgraToRgbParallel(byte[] inputArray, uint heightSize, uint widthSize)
        {
            byte[] rgb = new byte[heightSize * widthSize * PixelFormats.Rgb24.BitsPerPixel / 8];

            var rangePartitioner = Partitioner.Create(0, heightSize * widthSize);
            Parallel.ForEach(rangePartitioner, (range, loopstate) =>
            {
                for (var p = range.Item1; p < range.Item2; p++)
                {
                    rgb[p * 3] = inputArray[p * 4 + 2];
                    rgb[p * 3 + 1] = inputArray[p * 4 + 1];
                    rgb[p * 3 + 2] = inputArray[p * 4];
                }
            });

            return rgb;

        }

        /// <summary>
        /// Bgra -> RGB byte[]変換 forのほうが速い
        /// </summary>
        /// <param name="inputArray">BGRA byte[]</param>
        /// <returns>RGB byte[]</returns>
        public byte[] ConvertBgraToRgb(byte[] inputArray)
        {
            byte[] rgb = ConvertBgraToRgb(inputArray, height, width);
            return rgb;

        }
        /// <summary>
        /// Bgra -> RGB byte[]変換 Parallel版
        /// 画像サイズがすごくでかいと早いかも
        /// </summary>
        /// <param name="inputArray">BGRA byte[]</param>
        /// <returns>RGB byte[]</returns>
        public byte[] ConvertBgraToRgbParallel(byte[] inputArray)
        {
            byte[] rgb = ConvertBgraToRgbParallel(inputArray, height, width);
            return rgb;
        }

        public void InitializeDetectNum()
        {
            DetectNum = 0;
        }
        /// <summary>
        /// Detection用 複数実行されないようにする
        /// </summary>
        /// <param name="token">CancellationToken</param>
        /// <returns>Task</returns>
        private Task ProcessDetectImage(CancellationToken token, YoloWrapper yolo)
        {
            byte[] pixels = new byte[height * width * PixelFormats.Bgra32.BitsPerPixel / 8];
            byte[] rgb;
            int detectNum = 0;

            Stopwatch sw = Stopwatch.StartNew();
            //while (!token.IsCancellationRequested)
            while (yoloDetectRunningFlag.Value)
            {
                try
                {
                    if (FilesToProcess.TryDequeue(out var file))
                    {
                        using (var sourceStream = file.file.CreateViewStream())
                        {
                            // UnmanagedSourceSteamから読み出し
                            _ = sourceStream.Read(pixels, 0, pixels.Length);

                            rgb = ConvertBgraToRgb(pixels);
                            // 検出してBBox描画
                            lock (YoloDetectSyncObject)
                            {
                                var detectedBoundingBox = yolo.Detect(rgb, (int)width, (int)height, PreferencesDatOriginal.DetectThreshold);
                            }
                            detectNum = yolo.DetectCount;
                            //Detect状況Log画面出力
                            if (LastDetectNum != detectNum)
                            {
                                mainForm.Parent.opCollection.callbackMessageNormal("Detect : " + yolo.DetectCount.ToString() + " marmosets   DetectTime: " + sw.ElapsedMilliseconds + "ms");
                                LastDetectNum = detectNum;
                            }
                            sw.Restart();

                            //大きい値だったら更新？移動平均的なものを入れるか検討
                            if (DetectNum < detectNum && detectNum > 0)
                            {
                                DetectNum = detectNum;
                            }
                        }
                        file.accessor.Dispose();
                        file.file.Dispose();
                    }
                    else
                    {
                        // awaitするとCancelation発生するのでダメ
                        _ = Task.Delay(TimeSpan.FromMilliseconds(1), token);
                    }
                    Thread.Sleep(1);
                }
                catch (TaskCanceledException)
                {

                }
                catch (Exception)
                {
                    throw;
                }
            }

            return Task.CompletedTask;
        }
        /// <summary>
        /// 画像保存用フレームカウント
        /// 保存Pathにある既存ファイル名から次のフレームカウントを取得する
        /// </summary>
        /// <param name="path">画像保存Path</param>
        /// <returns>現在フレームカウント</returns>
        private static int GetNextFrameCount(string path)
        {
            string lastFileName;
            int ret = 0;
            DirectoryInfo di = new DirectoryInfo(path);
            FileInfo[] fi = di.GetFiles();
            if (fi.Length > 0)
            {
                lastFileName = Path.GetFileNameWithoutExtension(fi.Last().Name);
                ret = int.Parse(lastFileName) + 1;
            }
            else
            {
            }

            return ret;
        }
        /// <summary>
        /// MediaPlayer Callback関連キャプチャフレームLock
        /// </summary>
        /// <param name="opaque"></param>
        /// <param name="planes"></param>
        /// <returns></returns>
        private static IntPtr LockCaptureFrame(IntPtr opaque, IntPtr planes)
        {
            CurrentMappedFile = MemoryMappedFile.CreateNew(null, Pitch * Lines);
            CurrentMappedViewAccessor = CurrentMappedFile.CreateViewAccessor();
            Marshal.WriteIntPtr(planes, CurrentMappedViewAccessor.SafeMemoryMappedViewHandle.DangerousGetHandle());
            return IntPtr.Zero;
        }
        /// <summary>
        /// MediaPlayer キャプチャ関連 MemoryMappedFileにEnqueue
        /// </summary>
        /// <param name="opaque"></param>
        /// <param name="picture"></param>
        private static void EnqeueCaptureFrame(IntPtr opaque, IntPtr picture)
        {
            if (FrameCounter % CameraIntervalFrame == 0 && CaptureEnable)
            {
                FilesToProcess.Enqueue((CurrentMappedFile, CurrentMappedViewAccessor));
                CurrentMappedFile = null;
                CurrentMappedViewAccessor = null;
            }
            else
            {
                CurrentMappedViewAccessor.Dispose();
                CurrentMappedFile.Dispose();
                CurrentMappedFile = null;
                CurrentMappedViewAccessor = null;
            }
            FrameCounter++;
        }
        /// <summary>
        /// カメライメージセーブ開始
        /// </summary>
        /// <param name="camUri">CamURI</param>
        /// <param name="enableDetection">検出有効</param>
        /// <param name="offlineDetection">オフライン検出</param>
        /// <exception cref="Exception"></exception>
        public async void StartSavingFromCamImage(string camUri, bool enableDetection, bool offlineDetection = false)
        {
            CaptureEnable = true;
            //string imageSaveFolder = PreferencesDatOriginal.LearningSaveImageFileFolder; // Preferrence.LearningSaveImageFileFolder
            var currentDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            //var destination = Path.Combine(currentDirectory, imageSaveFolder);
            var destination = PreferencesDatOriginal.LearningSaveImageFolder;
            if (destination == "")
            {
                destination = Path.Combine(currentDirectory, "saveImage");
                PreferencesDatOriginal.LearningSaveImageFolder = destination;
            }
            Directory.CreateDirectory(destination);

            int frameNumber = GetNextFrameCount(destination);

            if (!MediaPlayer.IsPlaying)
            {
                MediaPlayer.Stopped += (s, eventArgs) => ProcessCancelationToken.CancelAfter(1);
            }
            // Create new media
            var setup = new StreamSetup();
            setup.Stream = StreamType.RTPMulticast;
            setup.Transport = new Transport();
            setup.Transport.Protocol = TransportProtocol.RTSP;

            // Camera URIを入れる
            UriBuilder uri;
            if (camUri != "")
            {
                if (!File.Exists(camUri))
                {
                    // SteamURIをとらないといけない
                    uri = new UriBuilder(Media.GetStreamUri(setup, Profiles.ElementAt(0).token).Uri);
                }
                else
                {
                    uri = new UriBuilder(camUri);
                    offlineDetection = true;
                }
            }
            else
            {
                //URI空白ならエラー戻り
                throw new Exception("URI error");
            }

            var option = new List<string>
                {
                    "--demux h264"
                };

            //using (LibVLCSharp.Shared.Media media = new LibVLCSharp.Shared.Media(libvlc, new Uri("rtsp://192.168.0.142:5544/live0.264")))
            using (LibVLCSharp.Shared.Media media = new LibVLCSharp.Shared.Media(libVlc, uri.Uri, option.ToArray()))
            {
                media.AddOption(":no-audio");
                media.AddOption("--quiet");
                // ビデオサイズ指定
                MediaPlayer.SetVideoFormat("RV32", width, height, Pitch);
                MediaPlayer.SetVideoCallbacks(LockCaptureFrame, null, EnqeueCaptureFrame);

                //MediaPlayer.EncounteredError += (s, eventArgs) =>
                //{

                //    Debug.WriteLine(eventArgs.ToString());
                //    throw new Exception("VLC encountered error.");
                //};

                try
                {
                    // Start recording
                    if (!MediaPlayer.IsPlaying)
                    {
                        MediaPlayer.Play(media);
                    }
                    else
                    {
                        Debug.WriteLine("VLC再生中エラー");
                        throw new Exception("VLC play error");
                    }
                    if (MediaPlayer.State == VLCState.Error)
                    {
                        Debug.WriteLine("VLC再生エラー");
                        throw new Exception("VLC play error");
                    }
                    else
                    {
                        CaptureStarted = true;
                        if (enableDetection && offlineDetection && Yolo != null)
                        {
                            // Waits for the processing to stop
                            await ProcessSaveImageAsync(destination, ProcessCancelationToken.Token, frameNumber, Yolo, true);
                        }
                        else if (enableDetection && !offlineDetection && Yolo != null)
                        {
                            await ProcessSaveImageAsync(destination, ProcessCancelationToken.Token, frameNumber, Yolo, false);
                        }
                        else
                        {
                            await ProcessSaveImageAsync(destination, ProcessCancelationToken.Token, frameNumber, null, false);
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    //MediaPlayer.Stop();
                }
            }
        }
        /// <summary>
        /// カメライメージセーブ停止
        /// </summary>
        public void StopSavingFromCamImage()
        {
            CaptureEnable = false;
            if (!ProcessCancelationToken.IsCancellationRequested)
            {
                ProcessCancelationToken.Cancel();
            }
            CaptureStarted = false;
            //MediaPlayer.Stop();
            //MediaPlayer.Stopped -= (s, eventArgs) => SaveImageCancelationToken.CancelAfter(1);
        }

        /// <summary>
        /// Detection開始
        /// </summary>
        /// <param name="camUri">camera uri</param>
        /// <exception cref="Exception"></exception>
        public async void StartDetectFromCamImage(string camUri)
        {
            CaptureEnable = true;
            if (!MediaPlayer.IsPlaying)
            {
                MediaPlayer.Stopped += (s, eventArgs) => ProcessCancelationToken.CancelAfter(1);
            }
            // Create new media
            var setup = new StreamSetup();
            setup.Stream = StreamType.RTPMulticast;
            setup.Transport = new Transport();
            setup.Transport.Protocol = TransportProtocol.RTSP;

            // Camera URIを入れる
            UriBuilder uri;
            if (camUri != "")
            {
                var uriLocal = new Uri(camUri);
                if (uriLocal.IsFile)
                {
                    if (File.Exists(uriLocal.AbsolutePath))
                    {

                    }
                    uri = new UriBuilder(uriLocal);
                }
                else if (!File.Exists(camUri))
                {
                    // SteamURIをとらないといけない
                    uri = new UriBuilder(Media.GetStreamUri(setup, Profiles.ElementAt(0).token).Uri);
                }
                else
                {
                    uri = new UriBuilder(camUri);
                }
            }
            else
            {
                //URI空白ならエラー戻り
                throw new Exception("URI error");
            }

            var option = new List<string>
            {
                "--demux h264"
            };

            using (LibVLCSharp.Shared.Media media = new LibVLCSharp.Shared.Media(libVlc, uri.Uri, option.ToArray()))
            {
                media.AddOption(":no-audio");
                media.AddOption("--quiet");
                // ビデオサイズ指定
                MediaPlayer.SetVideoFormat("RV32", width, height, Pitch);
                MediaPlayer.SetVideoCallbacks(LockCaptureFrame, null, EnqeueCaptureFrame);

                //MediaPlayer.EncounteredError += (s, eventArgs) =>
                //{
                //    //Debug.WriteLine(eventArgs.ToString());
                //    throw new Exception("VLC encountered error.");
                //};

                try
                {
                    // Start recording
                    if (!MediaPlayer.IsPlaying)
                    {
                        MediaPlayer.Play(media);
                    }
                    else
                    {
                        Debug.WriteLine("VLC再生中エラー");
                        throw new Exception("VLC play error");
                    }
                    if (MediaPlayer.State == VLCState.Error)
                    {
                        Debug.WriteLine("VLC再生エラー");
                        throw new Exception("VLC play error");
                    }
                    else
                    {
                        Stopwatch sw = Stopwatch.StartNew();
                        // detect start flag
                        DetectionStarted = true;
                        yoloDetectRunningFlag.Value = true;
                        // Waits for the processing to stop
                        await ProcessDetectImage(ProcessCancelationToken.Token, Yolo);
                        Debug.WriteLine("Detect end " + sw.ElapsedMilliseconds.ToString());
                    }
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    //MediaPlayer.Stop();
                }
            }
        }

        /// <summary>
        /// Detection終了
        /// </summary>
        public void StopDetectFromCamImage()
        {
            CaptureEnable = false;
            if (!ProcessCancelationToken.IsCancellationRequested)
            {
                ProcessCancelationToken.Cancel();
                yoloDetectRunningFlag.Value = false;
            }
            //MediaPlayer.Stop();
            //MediaPlayer.Stopped -= (s, eventArgs) => DetectImageCancelationToken.CancelAfter(1);
        }
        /// <summary>
        /// StartCapture キュー積み開始
        /// </summary>
        public void StartCapture()
        {
            CaptureEnable = true;
        }
        /// <summary>
        /// StopCapture キュー積み停止
        /// </summary>
        public void StopCapture()
        {
            CaptureEnable = false;
        }

        /// <summary>
        /// Mediaplayer stop イベント用
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">EventArgs</param>
        private void MediaPlayerStopped(object sender, EventArgs e)
        {
            CaptureStopped?.Invoke(sender, e);

            //再生止まったらDetectionStarted終わり
            DetectionStarted = false;

        }
        /// <summary>
        /// BitmapSourceキューから取得
        /// </summary>
        /// <returns>BitmapSource</returns>
        public BitmapSource GetProcessBitmapSource()
        {
            BitmapSource bitmapSource;
            try
            {
                bitmapSources.TryDequeue(out bitmapSource);
            }
            catch (Exception)
            {
                throw;
            }

            return bitmapSource;
        }
        /// <summary>
        /// Detectテスト
        /// </summary>
        public void Detect()
        {
            var bb = Yolo.Detect(Path.Combine(Directory.GetCurrentDirectory(), "SACX-1080L.jpg"));
            Console.WriteLine(bb.Length);
        }
        public void Detect(byte[] rgbPixelData, int width, int height, double threshold)
        {
            byte[] aa;
            var bb = Yolo.Detect(rgbPixelData, width, height, threshold);
        }
        /// <summary>
        /// Imageからbyte[]取得 image_t向け
        /// </summary>
        /// <param name="img">System.Drawing.Image</param>
        /// <returns>byte[]</returns>
        public static byte[] ImageToByte(System.Drawing.Image img)
        {
            ImageConverter imageConverter = new ImageConverter();
            return (byte[])imageConverter.ConvertTo(img, typeof(byte[]));
        }
    }
}
