using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

namespace Compartment
{
    public enum ECpColor
    {
        Black = 0,
        White = 1,
        Red = 2,
        Green = 3,
        Blue = 4,
        Yellow = 5,
        None = 6,
        Random = 7
    }
    public enum ECpShape
    {
        Circle = 0,
        Rectangle = 1,
        Triangle = 2,
        Star = 3,
        None = 4,
        Random = 5,
        Image = 6,
        SvgCircle,
        SvgRectangle,
        SvgTriangle,
        SvgStar,
        SvgScalene,
        SvgPentagon,
        SvgSquare,
        SvgRight,
        SvgTrapeze,
        SvgKite,
        SvgPolygon,
        SvgParallelogram,
        SvgEllipse,
        SvgTrefoil,
        SvgSemiCircle,
        SvgHexagon,
        SvgCresent,
        SvgOctagon,
        SvgCross,
        SvgRing,
        SvgPic,
        SvgHeart,
        SvgArrow,
        SvgQuadrefoil,
        SvgRhombus,
    }
    public enum ECpStep
    {
        Step1 = 0,
        Step2 = 1,
        Step3 = 2,
        Step4 = 3,
        Step5 = 4,
        None = 5
    }
    public enum ECpTask
    {
        Training = 0,
        DelayMatch = 1,
        None = 2,
        TrainingEasy = 3,
        UnConditionalFeeding = 4
    }

    public enum EDebugModeType
    {
        FullDummy = 0,    // 完全ダミー（実機不要）
        Hybrid = 1        // ハイブリッド（実機＋手動シミュレート）
    }

    public enum ECpCommand
    {
        ClearScreen = 0,
        DrawBackColor = 1,
        DrawImage = 2,
        TouchAny = 3,
        TouchOne = 4,
        TouchTwo = 5,
        TouchEnd = 6,
        None = 7,
        TouchAnyShape,
        DrawIndicator,
    }
    public enum ECpCorrectCondition
    {
        Coordinate = 0,
        Shape = 1,
        Color = 2
    }

    public enum ECpMarkerPosion
    {
        RightBottom = 0,
        RightTop,
        LeftBottom,
        LeftTop,
    }

    public partial class FormMain : Form
    {
        string preferenceFilename = System.IO.Path.Combine(Application.UserAppDataPath, "preference.xml");

        /// <summary>
        /// 設定をファイルへ書き込む
        /// </summary>
        /// <param name="preferenceDarArg"></param>
        /// <returns></returns>
        private bool SavePreference(PreferencesDat preferenceDarArg)
        {
            try
            {
                // XMLファイルに書き込む
                System.Xml.Serialization.XmlSerializer serializerFile = new System.Xml.Serialization.XmlSerializer(typeof(PreferencesDat));
                // UTF-9/BOMなし
                using (var streamWriterFile = new System.IO.StreamWriter(
                    preferenceFilename, false, new System.Text.UTF8Encoding(false)))
                {
                    serializerFile.Serialize(streamWriterFile, preferenceDarArg);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("LoadPreference(): {0}", ex.Message);
                return false;
            }
            return true;
        }
        /// <summary>
        /// 設定をファイルをファイルから読み込む
        /// 
        /// </summary>
        /// <param name="preferenceDarArg"></param>
        /// <returns></returns>
        private bool LoadPreference(out PreferencesDat preferenceDarArgOut)
        {
            // デフォルト設定を設定しておく
            preferenceDarArgOut = new PreferencesDat();
            try
            {
                // XMLファイルから読み込む
                System.Xml.Serialization.XmlSerializer serializerFile =
                    new System.Xml.Serialization.XmlSerializer(typeof(PreferencesDat));
                using (var streamReaderFile = new System.IO.StreamReader(
                    preferenceFilename, new System.Text.UTF8Encoding(false)))
                {
                    preferenceDarArgOut = (PreferencesDat)serializerFile.Deserialize(streamReaderFile);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("LoadPreference(): {0}", ex.Message);
                return false;
            }
            return true;
        }
        private bool LoadPreference(string fileName, out PreferencesDat preferencesDat)
        {
            preferencesDat = new PreferencesDat();
            try
            {
                // XMLファイルから読み込む
                System.Xml.Serialization.XmlSerializer serializerFile =
                    new System.Xml.Serialization.XmlSerializer(typeof(PreferencesDat));
                using (var streamReaderFile = new System.IO.StreamReader(
                    fileName, new System.Text.UTF8Encoding(false)))
                {
                    preferencesDat = (PreferencesDat)serializerFile.Deserialize(streamReaderFile);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("LoadPreference(): {0}", ex.Message);
                return false;
            }
            return true;
        }
    }

    public class PreferencesDat : ICloneable
    {
        #region メソッド:クローン
        // 複製を作成するメソッド
        public PreferencesDat Clone()
        {
            return (PreferencesDat)MemberwiseClone();
        }

        // ICloneable.Cloneの明示的な実装
        object ICloneable.Clone()
        {
            return Clone();
        }
        #endregion
        #region プロパティ
        //----------------------------------------------------------------------------
        // Compartmentカテゴリ
        [System.Xml.Serialization.XmlElement("CompartmentNo")]
        public int CompartmentNo { get; set; } = 0;

        [System.Xml.Serialization.XmlElement("ApiServerPort")]
        public int ApiServerPort { get; set; } = 5000;

        [System.Xml.Serialization.XmlElement("EnableDebugMode")]
        public bool EnableDebugMode { get; set; } = true;  // 一時的にtrueに設定（テスト用）

        [System.Xml.Serialization.XmlElement("DebugModeType")]
        public EDebugModeType DebugModeType { get; set; } = EDebugModeType.FullDummy;

        //----------------------------------------------------------------------------
        // ID codeカテゴリ
        public string ComPort { get; set; } = "COM1"; //= (System.IO.Ports.SerialPort.GetPortNames()).First();

        public string ComBaudRate { get; set; } = "19200";

        public string ComDataBitLength { get; set; } = "8";

        public string ComStopBitLength { get; set; } = System.IO.Ports.StopBits.One.ToString();

        //      [Category("ID code")]
        //      [DisplayName("フロー制御")]
        //      [Description("データ交換のフロー制御のためのハンドシェイキングプロトコルです。")]
        //      [Editor(typeof(ComboBoxHandshakeEditor), typeof(UITypeEditor))]
        //      public string Handshake { get; set; } = System.IO.Ports.Handshake.None.ToString();

        public string ComParity { get; set; } = System.IO.Ports.Parity.None.ToString();

        #region Operationカテゴリ

        //----------------------------------------------------------------------------
        // Operationカテゴリ

        [System.Xml.Serialization.XmlElement("TypeOfTask")]
        public String OpeTypeOfTask { get; set; } = ECpTask.Training.ToString();   //CpTask.Training.ToString();

        [System.Xml.Serialization.XmlElement("NumberOfTrial")]
        public int OpeNumberOfTrial { get; set; } = 3;

        [System.Xml.Serialization.XmlElement("TimeToDisplayCorrectImage")]
        public int OpeTimeToDisplayCorrectImage { get; set; } = 3;  // [s]

        [System.Xml.Serialization.XmlElement("TimeToDisplayNoImage")]
        public int OpeTimeToDisplayNoImage { get; set; } = 2;   // [s]

        [System.Xml.Serialization.XmlElement("IntervalTimeMinimum")]
        public int OpeIntervalTimeMinimum { get; set; } = 1; // [s]

        [System.Xml.Serialization.XmlElement("IntervalTimeMaximum")]
        public int OpeIntervalTimeMaximum { get; set; } = 2; // [s]

        [System.Xml.Serialization.XmlElement("EnableRandomTime")]
        public bool EnableRandomTime { get; set; } = false;

        [System.Xml.Serialization.XmlElement("RandomTimeMinimum")]
        public int OpeRandomTimeMinimum { get; set; } = 1; // [s]

        [System.Xml.Serialization.XmlElement("RandomTimeMaximum")]
        public int OpeRandomTimeMaximum { get; set; } = 2; // [s]

        [System.Xml.Serialization.XmlElement("FeedingRate")]
        public int OpeFeedingRate { get; set; } = 100; // [%]

        [System.Xml.Serialization.XmlElement("TimeToFeed")]
        public int OpeTimeToFeed { get; set; } = 2000; // [ms]

        [System.Xml.Serialization.XmlElement("TimeToFeedRewardFirstTime")]
        public int OpeTimeToFeedRewardFirstTime { get; set; } = 2000; // [ms]

        [System.Xml.Serialization.XmlElement("TimeToFeedReward")]
        public int OpeTimeToFeedReward { get; set; } = 4000; // [ms]

        [System.Xml.Serialization.XmlElement("EnableMultiFeeder")]
        public bool EnableMultiFeeder { get; set; } = false;

        [System.Xml.Serialization.XmlElement("RewardFeeder")]
        public int RewardFeeder { get; set; } = 1;

        [System.Xml.Serialization.XmlElement("DelayFeed")]
        public int OpeDelayFeed { get; set; } = 1000; // [ms]

        [System.Xml.Serialization.XmlElement("EnableFeedLamp")]
        public bool EnableFeedLamp { get; set; } = true;

        [System.Xml.Serialization.XmlElement("DelayFeedLamp")]
        public int OpeDelayFeedLamp { get; set; } = 500; // [ms]

        [System.Xml.Serialization.XmlElement("TimeoutOfStart")]
        public int OpeTimeoutOfStart { get; set; } = 1;        // 10; [m]

        [System.Xml.Serialization.XmlElement("TimeoutOfTrial")]
        public int OpeTimeoutOfTrial { get; set; } = 2;        // 10; [m]

        [System.Xml.Serialization.XmlElement("TimeoutOfLeaveCage")]
        public int OpeTimeoutOfLeaveCage { get; set; } = 1;        // 10; [m]

        [System.Xml.Serialization.XmlElement("EnableMonitorSave")]
        public bool EnableMonitorSave { get; set; } = false;

        [System.Xml.Serialization.XmlElement("MonitorSaveTime")]
        public int MonitorSaveTime { get; set; } = 10;        // 10; [m]

        [System.Xml.Serialization.XmlElement("DelayRoomLampOnTime")]
        public int OpeDelayRoomLampOnTime { get; set; } = 1000;        // 1000; [ms]

        [System.Xml.Serialization.XmlElement("EnableReEnter")]
        public bool OpeEnableReEntry { get; set; } = false;

        [System.Xml.Serialization.XmlElement("OpeReEntryTimeout")]
        public int OpeReEntryTimeout { get; set; } = 180; // [s]

        [System.Xml.Serialization.XmlElement("IncorrectTouchAction")]
        public bool IncorrectTouchAction { get; set; } = false;

        // Episode記憶

        [System.Xml.Serialization.XmlElement("EnableEpisodeMemory")]
        public bool EnableEpisodeMemory { get; set; } = false;

        [System.Xml.Serialization.XmlElement("ForceEpisodeMemory")]
        public bool ForceEpisodeMemory { get; set; } = false;

        [System.Xml.Serialization.XmlElement("EpisodeExpireTime")]
        public int EpisodeExpireTime { get; set; } = 3600; // [min]

        [System.Xml.Serialization.XmlElement("EnableEpisodeIncorrectRandom")]
        public bool EnableEpisodeIncorrectRandom { get; set; } = false;

        [System.Xml.Serialization.XmlElement("EpisodeCount")]
        public int EpisodeCount { get; set; } = 180; // [s]

        [System.Xml.Serialization.XmlElement("EpisodeStartTime")]
        public decimal EpisodeTimezoneStartTime { get; set; } = 0; // [時] 

        [System.Xml.Serialization.XmlElement("EpisodeEndTime")]
        public decimal EpisodeTimezoneEndTime { get; set; } = 1; // [時]

        [System.Xml.Serialization.XmlElement("EpisodeSelectShapeTimezoneStartTime")]
        public decimal EpisodeSelectShapeTimezoneStartTime { get; set; } = 0; // [時] 

        [System.Xml.Serialization.XmlElement("EpisodeSelectShapeTimezoneEndTime")]
        public decimal EpisodeSelectShapeTimezoneEndTime { get; set; } = 1; // [時]

        [System.Xml.Serialization.XmlElement("EpisodeIntervalTime")]
        public int EpisodeIntervalTime { get; set; } = 1; // [min]

        [System.Xml.Serialization.XmlElement("EnableTestIndicator")]
        public bool EnableTestIndicator { get; set; } = false;

        [System.Xml.Serialization.XmlElement("TestIndicatorSvgPath")]
        public string TestIndicatorSvgPath { get; set; } = @".\svg\trefoil.svg";

        [System.Xml.Serialization.XmlElement("IndicatorPosition")]
        public int IndicatorPosition { get; set; } = 0;

        // ID controll
        public bool EnableNoIDOperation { get; set; } = false;
        public bool EnableNoEntryIDOperation { get; set; } = false;

        #endregion

        #region Imageカテゴリ

        //----------------------------------------------------------------------------
        // Imageカテゴリ
        [System.Xml.Serialization.XmlElement("TriggerImageFile")]
        public string TriggerImageFile { get; set; } = System.IO.Path.Combine(Application.StartupPath, "TriggerImage.jpg");
        [System.Xml.Serialization.XmlElement("BackColor")]
        public String BackColor { get; set; } = ECpColor.Green.ToString();

        [System.Xml.Serialization.XmlElement("DelayBackColor")]
        public String DelayBackColor { get; set; } = ECpColor.Black.ToString();

        [System.Xml.Serialization.XmlElement("ShapeColor")]
        public String ShapeColor { get; set; } = ECpColor.White.ToString();

        [System.Xml.Serialization.XmlElement("TypeOfShape")]
        public String TypeOfShape { get; set; } = ECpShape.Circle.ToString();

        [System.Xml.Serialization.XmlElement("SizeOfShapeInStep")]
        public String SizeOfShapeInStep { get; set; } = ECpStep.Step1.ToString();

        [System.Xml.Serialization.XmlElement("SizeOfShapeInPixelForStep1")]
        public int SizeOfShapeInPixelForStep1 { get; set; } = 80; // [pixel]
        [System.Xml.Serialization.XmlElement("SizeOfShapeInPixelForStep2")]
        public int SizeOfShapeInPixelForStep2 { get; set; } = 120; // [pixel]
        [System.Xml.Serialization.XmlElement("SizeOfShapeInPixelForStep3")]
        public int SizeOfShapeInPixelForStep3 { get; set; } = 160; // [pixel]
        [System.Xml.Serialization.XmlElement("XmlElement")]
        public int SizeOfShapeInPixelForStep4 { get; set; } = 200; // [pixel]
        [System.Xml.Serialization.XmlElement("SizeOfShapeInPixelForStep5")]
        public int SizeOfShapeInPixelForStep5 { get; set; } = 250; // [pixel]

        [System.Xml.Serialization.XmlElement("SizeOfShapeInPixel")]
        public int SizeOfShapeInPixel { get; set; } = 0; // [pixel]

        [System.Xml.Serialization.XmlElement("EpisodeTriggerImageFile")]
        public string EpisodeTriggerImageFile { get; set; } = System.IO.Path.Combine(Application.StartupPath, "TriggerImage.jpg");
        [System.Xml.Serialization.XmlElement("EpisodeBackColor")]
        public string EpisodeFirstBackColor { get; set; } = ECpColor.Green.ToString();

        [System.Xml.Serialization.XmlElement("EpisodeTestBackColor")]
        public string EpisodeTestBackColor { get; set; } = ECpColor.Green.ToString();

        #endregion

        #region  CorrectConditionカテゴリ

        //----------------------------------------------------------------------------
        // CorrectConditionカテゴリ
        [System.Xml.Serialization.XmlElement("CorrectCondion")]
        public String CorrectCondition { get; set; } = ECpCorrectCondition.Coordinate.ToString();

        [System.Xml.Serialization.XmlElement("RandomCoordinate")]
        public bool RandomCoordinate { get; set; } = false;

        [System.Xml.Serialization.XmlElement("RandomShape")]
        public bool RandomShape { get; set; } = false;

        [System.Xml.Serialization.XmlElement("RandomColor")]
        public bool RandomColor { get; set; } = false;

        [System.Xml.Serialization.XmlElement("CoordinateFixX")]
        public int CoordinateFixX { get; set; } = 400;

        [System.Xml.Serialization.XmlElement("CoordinateFixY")]
        public int CoordinateFixY { get; set; } = 360;

        [System.Xml.Serialization.XmlElement("ShapeFix")]
        public String ShapeFix { get; set; } = ECpShape.Circle.ToString();

        [System.Xml.Serialization.XmlElement("ColorFix")]
        public String ColorFix { get; set; } = ECpColor.White.ToString();

        [System.Xml.Serialization.XmlElement("IncorrectNum")]
        public int IncorrectNum { get; set; } = 1;

        [System.Xml.Serialization.XmlElement("IncorrectRandomCoordinate")]
        public bool IncorrectCoordinateRandom { get; set; } = false;

        [System.Xml.Serialization.XmlElement("IncorrectRandomColor")]
        public bool IncorrectColorRandom { get; set; } = false;

        [System.Xml.Serialization.XmlElement("IncorrectRandomShape")]
        public bool IncorrectShapeRandom { get; set; } = false;

        [System.Xml.Serialization.XmlElement("IncorrectCoordinateFixX1")]
        public int IncorrectCoordinateFixX1 { get; set; } = 300;

        [System.Xml.Serialization.XmlElement("IncorrectCoordinateFixX2")]
        public int IncorrectCoordinateFixX2 { get; set; } = 310;

        [System.Xml.Serialization.XmlElement("IncorrectCoordinateFixX3")]
        public int IncorrectCoordinateFixX3 { get; set; } = 320;

        [System.Xml.Serialization.XmlElement("IncorrectCoordinateFixX4")]
        public int IncorrectCoordinateFixX4 { get; set; } = 330;

        [System.Xml.Serialization.XmlElement("IncorrectCoordinateFixY1")]
        public int IncorrectCoordinateFixY1 { get; set; } = 250;

        [System.Xml.Serialization.XmlElement("IncorrectCoordinateFixY2")]
        public int IncorrectCoordinateFixY2 { get; set; } = 260;

        [System.Xml.Serialization.XmlElement("IncorrectCoordinateFixY3")]
        public int IncorrectCoordinateFixY3 { get; set; } = 270;

        [System.Xml.Serialization.XmlElement("IncorrectCoordinateFixY4")]
        public int IncorrectCoordinateFixY4 { get; set; } = 280;

        [System.Xml.Serialization.XmlElement("IncorrectColorFix1")]
        public String IncorrectColorFix1 { get; set; } = ECpColor.White.ToString();

        [System.Xml.Serialization.XmlElement("IncorrectColorFix2")]
        public String IncorrectColorFix2 { get; set; } = ECpColor.White.ToString();

        [System.Xml.Serialization.XmlElement("IncorrectColorFix3")]
        public String IncorrectColorFix3 { get; set; } = ECpColor.White.ToString();

        [System.Xml.Serialization.XmlElement("IncorrectColorFix4")]
        public String IncorrectColorFix4 { get; set; } = ECpColor.White.ToString();

        [System.Xml.Serialization.XmlElement("IncorrectShapeFix1")]
        public String IncorrectShapeFix1 { get; set; } = ECpShape.Circle.ToString();

        [System.Xml.Serialization.XmlElement("IncorrectShapeFix2")]
        public String IncorrectShapeFix2 { get; set; } = ECpShape.Circle.ToString();

        [System.Xml.Serialization.XmlElement("IncorrectShapeFix3")]
        public String IncorrectShapeFix3 { get; set; } = ECpShape.Circle.ToString();

        [System.Xml.Serialization.XmlElement("IncorrectShapeFix4")]
        public String IncorrectShapeFix4 { get; set; } = ECpShape.Circle.ToString();

        [System.Xml.Serialization.XmlElement("CorrectImageFile")]
        public string CorrectImageFile { get; set; } = System.IO.Path.Combine(Application.StartupPath, "CorrectImage.jpg");

        [System.Xml.Serialization.XmlElement("ImageFileFolder")]
        public string ImageFileFolder { get; set; } = System.IO.Path.Combine(Application.StartupPath);

        [System.Xml.Serialization.XmlElement("EnableImageShape")]
        public bool EnableImageShape { get; set; } = false;

        [System.Xml.Serialization.XmlElement("RandomCorrectImage")]
        public bool RandomCorrectImage { get; set; } = false;


        [System.Xml.Serialization.XmlElement("EnableIncorrectCancel")]
        public bool EnableIncorrectCancel { get; set; } = false;

        [System.Xml.Serialization.XmlElement("IncorrectCount")]
        public int IncorrectCount { get; set; } = 1;

        [System.Xml.Serialization.XmlElement("IncorrectPenaltyTime")]
        public int IncorrectPenaltyTime { get; set; } = 0;

        #endregion

        #region Soundカテゴリ

        //----------------------------------------------------------------------------
        // Soundカテゴリ
        [System.Xml.Serialization.XmlElement("SoundFileOfEnd")]
        public String SoundFileOfEnd { get; set; } = System.IO.Path.Combine(Application.StartupPath, "EndSound.wav");

        [System.Xml.Serialization.XmlElement("TimeToOutputSoundOfEnd")]
        public int TimeToOutputSoundOfEnd { get; set; } = 3000; // [ms]

        [System.Xml.Serialization.XmlElement("SoundFileOfCorrect")]
        public String SoundFileOfCorrect { get; set; } = System.IO.Path.Combine(Application.StartupPath, "CorrectSound.wav");

        public string SoundFileOfIncorrect { get; set; } = System.IO.Path.Combine(Application.StartupPath, "IncorrectSound.wav");

        [System.Xml.Serialization.XmlElement("TimeToOutputSoundOfCorrect")]
        public int TimeToOutputSoundOfCorrect { get; set; } = 500; // [ms]

        [System.Xml.Serialization.XmlElement("SoundFileOfReward")]
        public String SoundFileOfReward { get; set; } = System.IO.Path.Combine(Application.StartupPath, "FeedSound.wav");

        [System.Xml.Serialization.XmlElement("SoundFileOfIncorrectReward")]
        public String SoundFileOfIncorrectReward { get; set; } = System.IO.Path.Combine(Application.StartupPath, "IncorrectFeedSound.wav");

        [System.Xml.Serialization.XmlElement("TimeToOutputSoundOfReward")]
        public int TimeToOutputSoundOfReward { get; set; } = 3000; // [ms]

        #endregion

        #region Mechanical thingカテゴリ

        //----------------------------------------------------------------------------
        // Mechanical thingカテゴリ
        [System.Xml.Serialization.XmlElement("TimeoutOfLeverIn")]
        public int TimeoutOfLeverIn { get; set; } = 4;

        [System.Xml.Serialization.XmlElement("TimeoutOfLeverOut")]
        public int TimeoutOfLeverOut { get; set; } = 4;

        [System.Xml.Serialization.XmlElement("TimeoutOfDoorOpen")]
        public int TimeoutOfDoorOpen { get; set; } = 6;

        [System.Xml.Serialization.XmlElement("TimeoutOfDoorClose")]
        public int TimeoutOfDoorClose { get; set; } = 6;

        [System.Xml.Serialization.XmlElement("DisableDoor")]
        public bool DisableDoor { get; set; } = true;

        [System.Xml.Serialization.XmlElement("IgnoreDoorError")]
        public bool IgnoreDoorError { get; set; } = false;

        [System.Xml.Serialization.XmlElement("DisableLever")]
        public bool DisableLever { get; set; } = true;

        [System.Xml.Serialization.XmlElement("CageEntryTime")]
        public int CageEntryTime { get; set; } = 5;

        [System.Xml.Serialization.XmlElement("EnableConveyor")]
        public bool EnableConveyor { get; set; } = false;

        [System.Xml.Serialization.XmlElement("EnableExtraFeeder")]
        public bool EnableExtraFeeder { get; set; } = false;

        [System.Xml.Serialization.XmlElement("EDoorOpenSpeed")]
        public int EDoorOpenSpeed { get; set; } = 235;
        [System.Xml.Serialization.XmlElement("EDoorCloseSpeed")]
        public int EDoorCloseSpeed { get; set; } = 235;
        [System.Xml.Serialization.XmlElement("EDoorMiddleSpeed")]
        public int EDoorMiddleSpeed { get; set; } = 235;
        [System.Xml.Serialization.XmlElement("EDoorOpenToMiddleTime")]
        public int EDoorOpenToMiddleTime { get; set; } = 300;
        [System.Xml.Serialization.XmlElement("EDoorCloseToMiddleTime")]
        public int EDoorCloseToMiddleTime { get; set; } = 300;
        [System.Xml.Serialization.XmlElement("EDoorCloseIntervalTime")]
        public int EDoorCloseIntervalTime { get; set; } = 1000;
        /// <summary>
        /// 開き監視インターバル時間 経過後条件監視開始
        /// </summary>
        [System.Xml.Serialization.XmlElement("EDoorOpenIntervalTime")]
        public int EDoorOpenIntervalTime { get; set; } = 1000;
        /// <summary>
        /// 閉め監視インターバル時間 経過後条件監視開始
        /// </summary>
        [System.Xml.Serialization.XmlElement("EDoorMiddleIntervalTime")]
        public int EDoorMiddleIntervalTime { get; set; } = 100;

        [System.Xml.Serialization.XmlElement("EDoorCloseTimeout")]
        public int EDoorCloseTimeout { get; set; } = 600000;
        /// <summary>
        /// 中間扉状態タイムアウト
        /// </summary>
        [System.Xml.Serialization.XmlElement("EDoorMiddleTimeout")]
        public int EDoorMiddleTimeout { get; set; } = 60000;
        [System.Xml.Serialization.XmlElement("EDoorMonitorStartTime")]
        public int EDoorMonitorStartTime { get; set; } = 800;
        [System.Xml.Serialization.XmlElement("EDoorReMonitorTime")]
        public int EDoorReMonitorTime { get; set; } = 1000;
        /// <summary>
        /// 人感センサーで閉めドアモニター開始時間
        /// </summary>
        [System.Xml.Serialization.XmlElement("EDoorPresenceMonitorStartTime")]
        public int EDoorCloseDoorMonitorStartTime { get; set; } = 100;
        /// <summary>
        /// 閉モニター時間
        /// </summary>
        [System.Xml.Serialization.XmlElement("EDoorCloseMonitorTime")]
        public int EDoorCloseMonitorTime { get; set; } = 5000;
        /// <summary>
        /// 内側センサー検知後ドア閉め時間
        /// </summary>
        [System.Xml.Serialization.XmlElement("EDoorInsideDetectCloseTime")]
        public int EDoorInsideDetectCloseTime { get; set; } = 3000;
        /// <summary>
        /// 中間扉待機時閉じ時間
        /// </summary>
        [System.Xml.Serialization.XmlElement("EDoorMiddleCloseTimeout")]
        public int EDoorMiddleCloseTimeout { get; set; } = 30000;
        /// <summary>
        /// 開き時内センサー検知再入時間
        /// </summary>
        [System.Xml.Serialization.XmlElement("EDoorReEntryTime")]
        public int EDoorReEntryTime { get; set; } = 5000;
        /// <summary>
        /// 入室センサー開き時間
        /// </summary>
        [System.Xml.Serialization.XmlElement("EDoorPresenceOpenTime")]
        public int EDoorPresenceOpenTime { get; set; } = 120000;

        public int EDoorPresenceMiddleDetectTime { get; set; } = 10000;
        /// <summary>
        /// 中間扉キャンセル閉め時間
        /// </summary>
        [System.Xml.Serialization.XmlElement("EDoorMiddleCancelCloseTime")]
        public int EDoorMiddleCancelCloseTime { get; set; } = 1000;
        #endregion

        #region Outputカテゴリ

        //----------------------------------------------------------------------------
        // Outputカテゴリ
        [System.Xml.Serialization.XmlElement("OutputResultFile")]
        public string OutputResultFile { get; set; } = System.IO.Path.Combine(Application.UserAppDataPath, "result.csv");

        #endregion

        #region EpisodeMemoryカテゴリ

        [System.Xml.Serialization.XmlElement("EpisodeRandomCoordinate")]
        public bool EpisodeRandomCoordinate { get; set; } = false;
        [System.Xml.Serialization.XmlElement("EpisodeRandomShape")]
        public bool EpisodeRandomShape { get; set; } = false;
        [System.Xml.Serialization.XmlElement("EpisodeRandomColor")]
        public bool EpisodeRandomColor { get; set; } = false;

        [System.Xml.Serialization.XmlElement("EpisodeTargetNum")]
        public int EpisodeTargetNum { get; set; } = 1;
        [System.Xml.Serialization.XmlElement("EpTarget1FixX")]
        public int EpTarget1FixX { get; set; } = 300;
        [System.Xml.Serialization.XmlElement("EpTarget1FixY")]
        public int EpTarget1FixY { get; set; } = 250;
        [System.Xml.Serialization.XmlElement("EpTarget2FixX")]
        public int EpTarget2FixX { get; set; } = 310;
        [System.Xml.Serialization.XmlElement("EpTarget2FixY")]
        public int EpTarget2FixY { get; set; } = 260;
        [System.Xml.Serialization.XmlElement("EpTarget3FixX")]
        public int EpTarget3FixX { get; set; } = 320;
        [System.Xml.Serialization.XmlElement("EpTarget3FixY")]
        public int EpTarget3FixY { get; set; } = 270;
        [System.Xml.Serialization.XmlElement("EpTarget4FixX")]
        public int EpTarget4FixX { get; set; } = 330;
        [System.Xml.Serialization.XmlElement("EpTarget4FixY")]
        public int EpTarget4FixY { get; set; } = 280;
        [System.Xml.Serialization.XmlElement("EpTarget5FixX")]
        public int EpTarget5FixX { get; set; } = 340;
        [System.Xml.Serialization.XmlElement("EpTarget5FixY")]
        public int EpTarget5FixY { get; set; } = 290;

        [System.Xml.Serialization.XmlElement("EpShapeFix1")]
        public string EpShapeFix1 { get; set; } = ECpShape.Circle.ToString();
        [System.Xml.Serialization.XmlElement("EpShapeFix2")]
        public string EpShapeFix2 { get; set; } = ECpShape.Circle.ToString();
        [System.Xml.Serialization.XmlElement("EpShapeFix3")]
        public string EpShapeFix3 { get; set; } = ECpShape.Circle.ToString();
        [System.Xml.Serialization.XmlElement("EpShapeFix4")]
        public string EpShapeFix4 { get; set; } = ECpShape.Circle.ToString();
        [System.Xml.Serialization.XmlElement("EpShapeFix5")]
        public string EpShapeFix5 { get; set; } = ECpShape.Circle.ToString();

        [System.Xml.Serialization.XmlElement("EpColorFix1")]
        public string EpColorFix1 { get; set; } = ECpColor.White.ToString();
        [System.Xml.Serialization.XmlElement("EpColorFix2")]
        public string EpColorFix2 { get; set; } = ECpColor.White.ToString();
        [System.Xml.Serialization.XmlElement("EpColorFix3")]
        public string EpColorFix3 { get; set; } = ECpColor.White.ToString();
        [System.Xml.Serialization.XmlElement("EpColorFix4")]
        public string EpColorFix4 { get; set; } = ECpColor.White.ToString();
        [System.Xml.Serialization.XmlElement("EpColorFix5")]
        public string EpColorFix5 { get; set; } = ECpColor.White.ToString();

        [System.Xml.Serialization.XmlElement("EnableEpImageShape")]
        public bool EnableEpImageShape { get; set; } = false;
        [System.Xml.Serialization.XmlElement("EpImageFileFolder")]
        public string EpImageFileFolder { get; set; } = "";

        #endregion

        #region MamosetDetectionCamカテゴリ

        //----------------------------------------------------------------------------
        // MamosetDetectionCamカテゴリ

        [System.Xml.Serialization.XmlElement("EnableMamosetDetection")]
        public bool EnableMamosetDetection { get; set; } = false;
        
        [System.Xml.Serialization.XmlElement("EnableMamosetDetectionTestMode")]
        public bool EnableMamosetDetectionSaveImageMode { get; set; } = false;
        
        [System.Xml.Serialization.XmlElement("EnableMamosetDetectionLearningMode")]
        public bool EnableMamosetDetectionSaveOnlyMode { get; set; } = false;

        [System.Xml.Serialization.XmlElement("LearningSaveImageFileFolder")]
        public string LearningSaveImageFolder { get; set; } = System.IO.Path.Combine(Application.StartupPath, "SaveImages");

        [System.Xml.Serialization.XmlElement("LearningModelPath")]
        public string LearningModelPath { get; set; } = System.IO.Path.Combine(Application.StartupPath, "modelFile");

        [System.Xml.Serialization.XmlElement("SelectCamUri")]
        public string SelectCamUri { get; set; }

        [System.Xml.Serialization.XmlElement("ShotCamIntervel")]
        public int ShotCamInterval { get; set; } = 500; // [ms]

        [System.Xml.Serialization.XmlElement("DetectThreshold")]
        public double DetectThreshold { get; set; } = 0.85;

        #endregion

        //============================================================================
        //----------------------------------------------------------------------------
        // 内部使用
        //      新版
        [System.Xml.Serialization.XmlElement("OutputLogFile")]
        public string OutputLogFile { get; set; } = System.IO.Path.Combine(Application.UserAppDataPath, "logFile.txt");


        // タッチパネル端の無効領域: 左端のピクセル数
        [System.Xml.Serialization.XmlElement("TouchPanelInvalidLeftInPixel")]
        public int TouchPanelInvalidLeftInPixel { get; set; } = 20; // [pixel]
        // タッチパネル端の無効領域: 右端のピクセル数
        [System.Xml.Serialization.XmlElement("TouchPanelInvalidRightInPixel")]
        public int TouchPanelInvalidRightInPixel { get; set; } = 20; // [pixel]
        // タッチパネル端の無効領域: 上端のピクセル数
        [System.Xml.Serialization.XmlElement("TouchPanelInvalidTopInPixel")]
        public int TouchPanelInvalidTopInPixel { get; set; } = 20; // [pixel]
        // タッチパネル端の無効領域: 下端のピクセル数
        [System.Xml.Serialization.XmlElement("TouchPanelInvalidBottomInPixel")]
        public int TouchPanelInvalidBottomInPixel { get; set; } = 20; // [pixel]
        // 正解図形と不正解図形の最小距離[pixel]
        [System.Xml.Serialization.XmlElement("MinDistanceBetweenCorrectAndWrongShape")]
        public int MinDistanceBetweenCorrectAndWrongShape { get; set; } = 5; // [pixel]

        // Door用
        [System.Xml.Serialization.XmlElement("ActivePulseWidthOfDoor")]
        public int ActivePulseWidthOfDoor { get; set; } = 150;      // [ms]
        [System.Xml.Serialization.XmlElement("InactivePulseWidthOfDoor")]
        public int InactivePulseWidthOfDoor { get; set; } = 150;    // [ms]

        [System.Xml.Serialization.XmlElement("DoorCloseTimeInRecoverOpen")]
        public int DoorCloseTimeInRecoverOpen { get; set; } = 2500; // [ms]
        [System.Xml.Serialization.XmlElement("DoorOpenTimeInRecoverClose")]
        public int DoorOpenTimeInRecoverClose { get; set; } = 2500; // [ms]
        [System.Xml.Serialization.XmlElement("DoorRetryNumInRecover")]
        public int DoorRetryNumInRecover { get; set; } = 2; // [回]

        // Lever用
        [System.Xml.Serialization.XmlElement("ActivePulseWidthOfLever")]
        public int ActivePulseWidthOfLever { get; set; } = 150; // [ms]
        [System.Xml.Serialization.XmlElement("InactivePulseWidthOfLever")]
        public int InactivePulseWidthOfLever { get; set; } = 150; // [ms]
        [System.Xml.Serialization.XmlElement("LeverInTimeInRecoverOut")]
        public int LeverInTimeInRecoverOut { get; set; } = 1500; // [ms]
        [System.Xml.Serialization.XmlElement("LeverOutTimeInRecoverIn")]
        public int LeverOutTimeInRecoverIn { get; set; } = 1500; // [ms]
        [System.Xml.Serialization.XmlElement("LeverRetryNumInRecover")]
        public int LeverRetryNumInRecover { get; set; } = 2; // [回]

        // MarmoDetection
        [System.Xml.Serialization.XmlElement("CameraResolutionX")]
        public int CameraResolutionX { get; set; } = 1280; // [pixel]
        [System.Xml.Serialization.XmlElement("CameraResolutionY")]
        public int CameraResolutionY { get; set; } = 720; // [pixel]
        [System.Xml.Serialization.XmlElement("CameraFrameRate")]
        public int CameraFrameRate { get; set; } = 20; // [fps]
        #endregion
    }
}
