using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Compartment
{
    public class OpCollection
    {
        #region コールバック
        public Action<string> callbackMessageNormal = (str) => { };
        public Action<string> callbackMessageDebug = (str) => { };
        public Action<string> callbackMessageError = (str) => { };

        public Action<int> callbackSetUiCurentNumberOfTrial = (value) => { };
        public Action<string> callbackSetUiCurrentIdCode = (str) => { };
        #endregion

        #region ファイルユーティリティ
        /// <summary>
        /// ファイルユーティリティ
        /// </summary>
        public class FileUtil
        {
            /// <summary>
            /// 新規オープンのコールバック
            /// </summary>
            public Action callbackOpenNewFile = () => { };

            /// <summary>
            /// デストラクタ
            /// </summary>
            ~FileUtil()
            {
                Close();
            }

            /// <summary>
            /// 文字の書き込み
            /// </summary>
            /// <param name="value"></param>
            public void WriteLineAsync(string value)
            {
                if (writer is null) { return; }
                var task = Task.Run(async () =>
                {
                    await writer.WriteLineAsync(value);
                    writer.Flush();
                });
            }

            /// <summary>
            /// 文字列の書き込み
            /// </summary>
            /// <param name="value"></param>
            public void WriteAsync(string value)
            {
                if (writer is null) { return; }
                var task = Task.Run(async () =>
                {
                    await writer.WriteAsync(value);
                    writer.Flush();
                });
            }

            private StreamWriter writer = null;

            private string mFolder = Application.LocalUserAppDataPath;
            private string mFilePath = System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase) + ".csv";
            private string mFileName = "";

            /// <summary>
            /// オープン
            /// </summary>
            /// <param name="folder"></param>
            /// <param name="filePath"></param>
            public void Open(string folder, string filePath)
            {
                if (writer != null) { Close(); }

                if (folder != "") { mFolder = folder; }
                if (filePath != "") { mFilePath = filePath; }

                mFileName = System.IO.Path.Combine(mFolder, mFilePath);

                bool fileExist = System.IO.File.Exists(mFileName);

                writer = new StreamWriter(mFileName, true, Encoding.UTF8);

                if (fileExist == false) { callbackOpenNewFile(); }
            }

            public void Open(string fileName)
            {
                try
                {
                    if (writer != null) { Close(); }

                    bool fileExist = System.IO.File.Exists(fileName);

                    writer = new StreamWriter(fileName, true, Encoding.UTF8);

                    if (fileExist == false) { callbackOpenNewFile(); }
                }
                catch (Exception)
                {

                    throw;
                }
            }

            /// <summary>
            /// オープン
            /// </summary>
            public void Open()
            {
                writer = new StreamWriter(mFileName, true, Encoding.UTF8);
            }

            /// <summary>
            /// 関連付けられたアプリケーションでファイルを開く
            /// </summary>
            public void Start()
            {
                System.Diagnostics.Process p = System.Diagnostics.Process.Start(mFileName);
            }

            /// <summary>
            /// クローズ
            /// </summary>
            public void Close()
            {
                if (writer is null) { return; }
                try
                {
                    writer.Close();
                }
                catch
                {
                }
                writer = null;
            }
        }

        public FileUtil file = new FileUtil();
        #endregion ファイルユーティリティ

        #region コマンド
        /// <summary>
        /// コマンド定義
        /// </summary>
        public enum ECommand
        {
            /// <summary>
            /// ノーオペレーション
            /// </summary>
            [EnumDisplayName("ノーオペレーション")]
            Nop,

            /// <summary>
            /// 開始
            /// </summary>
            [EnumDisplayName("開始")]
            Start,

            /// <summary>
            /// 中止
            /// </summary>
            [EnumDisplayName("中止")]
            Stop,

            /// <summary>
            /// 緊急停止
            /// </summary>
            [EnumDisplayName("緊急停止")]
            EmergencyStop
        }

        /// <summary>
        /// コマンド
        /// </summary>
        private ECommand mCommand = ECommand.Nop;

        /// <summary>
        /// コマンド Threadセーフ化済み １回読み取るとリセットされるので考慮した実装へ
        /// </summary>
        public ECommand Command
        {
            get
            {
                ECommand _value = _Command.Value;
                _Command.Value = ECommand.Nop;
                return _value;
            }
            set
            {
                _Command.Value = value;
            }
        }
        SyncObject<ECommand> _Command = new SyncObject<ECommand>(ECommand.Nop);
        #endregion コマンド

        #region シーケンサ
        /// <summary>
        /// シーケンサ
        /// </summary>
        public class Sequencer
        {
            /// <summary>
            /// ステート定義
            /// </summary>
            public enum EState
            {
                /// <summary>
                /// 初期化
                /// </summary>
                [EnumDisplayName("初期化")]
                Init,

                /// <summary>
                /// アイドル
                /// </summary>
                [EnumDisplayName("アイドル")]
                Idle,

                #region デバイススタンバイ
                /// <summary>
                /// デバイススタンバイ1
                /// </summary>
                [EnumDisplayName("デバイススタンバイ開始")]
                DeviceStandbyBegin,

                /// <summary>
                /// デバイススタンバイ2
                /// </summary>
                [EnumDisplayName("デバイススタンバイ中")]
                DeviceStandby2,

                /// <summary>
                /// デバイススタンバイ3
                /// </summary>
                [EnumDisplayName("デバイススタンバイ完了")]
                DeviceStandbyEnd,
                #endregion デバイススタンバイ

                #region 停止系
                /// <summary>
                /// 停止
                /// </summary>
                [EnumDisplayName("停止")]
                Stop,

                /// <summary>
                /// 緊急停止
                /// </summary>
                [EnumDisplayName("緊急停止")]
                EmergencyStop,
                #endregion 停止系

                #region 入室
                /// <summary>
                /// 入室前処理
                /// </summary>
                [EnumDisplayName("入室前処理")]
                PreEnterCageProc,

                /// <summary>
                /// 入室待ち
                /// </summary>
                [EnumDisplayName("入室待ち")]
                WaitingForEnterCage,

                /// <summary>
                /// レバーOUT待ち
                /// </summary>
                [EnumDisplayName("レバーOUT待ち")]
                WaitingForOutLever,

                /// <summary>
                /// レバーDOWN待ち
                /// </summary>
                [EnumDisplayName("レバーDOWN待ち")]
                WaitingForDownLever,

                #region ドア、レバーの同時動作回避用の実装
                // ドアCLOSE -> レバーINの順番でもダメ

                /// <summary>
                /// レバーIN待ち
                /// </summary>
                [EnumDisplayName("レバーIN")]
                WaitingForInLever,

                /// <summary>
                /// ドアCLOSE待ち
                /// </summary>
                [EnumDisplayName("ドアCLOSE")]
                WaitingForCloseDoor,
                #endregion ドア、レバーの同時動作回避用の実装

                #endregion 入室

                #region 試行
                /// <summary>
                /// トリガ画面タッチ前処理
                /// </summary>
                [EnumDisplayName("トリガ画面タッチ前処理")]
                PreTouchTriggerScreenProc,

                /// <summary>
                /// トリガ画面タッチ待ち
                /// </summary>
                [EnumDisplayName("トリガ画面タッチ待ち")]
                WaitingForTouchTriggerScreen,
                /// <summary>
                /// マッチ前遅延
                /// </summary>
                [EnumDisplayName("マッチ前ディレイ")]
                DelayBeforeMatch,
                /// <summary>
                /// マッチ後遅延
                /// </summary>
                [EnumDisplayName("訓練前ディレイ")]
                DelayAfterTraning,

                /// <summary>
                /// 訓練
                /// </summary>
                [EnumDisplayName("訓練")]
                Training,

                /// <summary>
                /// ディレイマッチ(正解図形表示)
                /// </summary>
                [EnumDisplayName("ディレイマッチ(正解図形表示)")]
                DelayMatchImage,

                /// <summary>
                /// ディレイマッチ(タッチパネル画面を背景色)
                /// </summary>
                [EnumDisplayName("ディレイマッチ(タッチパネル画面を背景色)")]
                DelayMatchBackColor,

                /// <summary>
                /// 正解
                /// </summary>正解
                [EnumDisplayName("")]
                CorrectAnswer,

                /// <summary>
                /// 給餌完了待ち
                /// </summary>
                [EnumDisplayName("給餌アナウンス待ち")]
                WaitingForReward,

                /// <summary>
                /// 給餌ディレイ
                /// </summary>
                [EnumDisplayName("給餌ディレイ")]
                DelayFeed,

                /// <summary>
                /// 給餌完了待ち
                /// </summary>
                [EnumDisplayName("給餌完了待ち")]
                WaitingForFeedComplete,

                /// <summary>
                /// 給餌完了待ち
                /// </summary>
                [EnumDisplayName("給餌完了後Delay")]
                WaitingDelayForRoomLump,

                /// <summary>
                /// 給餌完了待ち
                /// </summary>
                [EnumDisplayName("退室促進ルームランプ点灯")]
                TurnOnRoomLump,

                /// <summary>
                /// インターバル初期化
                /// </summary>
                [EnumDisplayName("インターバル初期化")]
                InitInterval,

                /// <summary>
                /// インターバル期間
                /// </summary>
                [EnumDisplayName("インターバル期間")]
                IntervalProc,

                #region 退室
                /// <summary>
                /// ドアOPEN前処理
                /// </summary>
                [EnumDisplayName("ドアOPEN前処理")]
                PreOpenDoorProc,

                /// <summary>
                /// ドアOPEN待ち
                /// </summary>
                [EnumDisplayName("ドアOPEN待ち")]
                WaitingForOpenDoor,
                #endregion 試行

                /// <summary>
                /// 退室前処理
                /// </summary>
                [EnumDisplayName("退室前処理")]
                PreLeaveCageProc,

                /// <summary>
                /// 退室待ち
                /// </summary>
                [EnumDisplayName("退室待ち")]
                WaitingForLeaveCage,

                /// <summary>
                /// イリーガル退出検知
                /// </summary>
                [EnumDisplayName("イリーガル退室検知")]
                IllegalExitDetection,

                /// <summary>
                /// 給餌後退出検知
                /// </summary>
                [EnumDisplayName("給餌後退室検知")]
                ExitAfterFeedingDetection,

                #endregion 退室

                #region イリーガル後復帰処理

                /// <summary>
                /// 
                /// </summary>
                [EnumDisplayName("イリーガル後訓練待機")]
                IllegalExitDetection_TrainingWait,

                /// <summary>
                /// 
                /// </summary>
                [EnumDisplayName("イリーガル後ディレイマッチ待機")]
                IllegalExitDetection_WaitReEnter,
                /// <summary>
                /// 
                /// </summary>
                [EnumDisplayName("イリーガル後Training")]
                AfterIllegalExit_Training,
                /// <summary>
                /// 
                /// </summary>
                [EnumDisplayName("イリーガル後DelayMatchImage")]
                AfterIllegalExit_DelayMatchImage,

                #endregion

                /// <summary>
                /// エラー
                /// </summary>
                [EnumDisplayName("エラー")]
                Error,

                /// <summary>
                /// タイムアウト退室待ち
                /// </summary>
                [EnumDisplayName("タイムアウト退室待ち")]
                WaitingForTimeoutLeaveCage,
                /// <summary>
                /// ブロック出力用
                /// </summary>
                [EnumDisplayName("ブロック出力用")]
                BlockOutput,
                /// <summary>
                /// ブロック出力用
                /// </summary>
                [EnumDisplayName("不正解中断")]
                IncorrectTouchExit,
                /// <summary>
                /// ブロック出力用
                /// </summary>
                [EnumDisplayName("不正解後待機")]
                IncorrectTouchAfterWait,
            }

            /// <summary>
            /// ステート設定時のイベント
            /// </summary>
            public Action<EState> setStateEvent = (state) => { };

            /// <summary>
            /// ステート
            /// </summary>
            private EState mState = EState.Init;

            private EState StateStack = EState.Init;

            public void StoreStateStack()
            {
                StateStack = State;
            }

            public void LoadState()
            {
                State = StateStack;
            }

            public EState BeforeState { get; set; } = EState.Init;

            /// <summary>
            /// ステート
            /// </summary>
            public EState State
            {
                get => mState;
                set
                {
                    BeforeState = mState;
                    mState = value;
                    setStateEvent(mState);
                }
            }
            /// <summary>
            /// ファイル出力のコールバック
            /// </summary>
            public Action<EState> callbackOutputFile = (state) => { };
        }

        /// <summary>
        /// シーケンサ
        /// </summary>
        public Sequencer sequencer = new Sequencer();

        /// <summary>
        /// ストップウォッチ
        /// </summary>
        public Stopwatch stopwatch = new Stopwatch();

        /// <summary>
        /// インターバル時間[秒]
        /// </summary>
        public int intervalTime = 0;

        /// <summary>
        /// ディレイマッチ前ランダム時間[秒]
        /// </summary>
        public int beforeDelayMatchRandomTime = 0;

        /// <summary>
        /// 試行回数カウンタ
        /// </summary>
        public int trialCount = 0;

        public string idCode = "";

        /// <summary>
        /// 動作中
        /// </summary>
        public SyncObject<bool> IsBusy = new SyncObject<bool>(false);
        #endregion シーケンサ

        #region 結果ファイル出力で使用
        /// <summary>
        /// 課題結果
        /// </summary>
        public enum ETaskResult
        {
            /// <summary>
            /// なし(初期値)
            /// </summary>
            [EnumDisplayName("None")]
            None,
            /// <summary>
            /// 正解
            /// </summary>
            [EnumDisplayName("Ok")]
            Ok,
            [EnumDisplayName("Ng")]
            Ng,
            /// <summary>
            /// 開始タイムアウト
            /// </summary>
            [EnumDisplayName("StartTimeout")]
            StartTimeout,
            /// <summary>
            /// 試行タイムアウト
            /// </summary>
            [EnumDisplayName("TrialTimeout")]
            TrialTimeout,
            /// <summary>
            /// イリーガル退出
            /// </summary>
            [EnumDisplayName("IllegalExit")]
            IllegalExit,
            /// <summary>
            /// トリガータッチ Block用
            /// </summary>
            [EnumDisplayName("TriggerTouch")]
            TriggerTouch,
            /// <summary>
            /// 不正解タッチ
            /// </summary>
            [EnumDisplayName("IncorrectTouchNG")]
            IncorrectTouchNG,
            /// <summary>
            /// 給餌後退出
            /// </summary>
            [EnumDisplayName("ExitAfterFeeding")]
            ExitAfterFeeding,
            /// <summary>
            /// エピソードOK
            /// </summary>
            [EnumDisplayName("OkEpisode")]
            OkEpisode,
            /// <summary>
            /// エピソード出題
            /// </summary>
            [EnumDisplayName("OkEpisodeIssue")]
            OkEpisodeIssue,
            [EnumDisplayName("NgEpisodeIssue")]
            NgEpisodeIssue,
        };
        public ETaskResult taskResultVal = ETaskResult.None;
        /// <summary>
        /// 給餌したか否かのフラグ
        /// </summary>
        /// <remarks>
        /// 初期化タイミング: トリガ画面タッチ前処理
        /// 保存タイミング: Feed時
        /// </remarks>
        public bool flagFeed = false;

        /// <summary>
        /// インターバル実行回数[回]
        /// </summary>
        /// <remarks>
        /// 初期化タイミング: レバーSW押した後
        /// 保存タイミング: インターバルを実行する時
        /// </remarks>
        public int intervalNum = 0;
        /// <summary>
        /// インターバル合計時間[s]
        /// </summary>
        /// <remarks>
        /// 初期化タイミング: レバーSW押した後
        /// 保存タイミング: インターバルを実行する時
        /// </remarks>
        public int intervalTimeTotal = 0;
        /// <summary>
        /// 正解図形の座標
        /// </summary>
        /// <remarks>
        /// 初期化タイミング: トリガ画面タッチ前処理
        /// 保存タイミング: 正解画像を表示する時
        /// </remarks>
        public Point pointCorrectShape = new Point(0, 0);

        /// <summary>
        /// 訓練正解図形情報
        /// </summary>
        public ShapeObject trainingShape = new ShapeObject();

        /// <summary>
        /// 正解図形情報
        /// </summary>
        public ShapeObject correctShape = new ShapeObject();

        /// <summary>
        /// 正解画像ファイル名
        /// </summary>
        public string correctImageFile = "";

        /// <summary>
        /// 不正解図形の座標
        /// </summary>
        /// <remarks>
        /// 初期化タイミング: トリガ画面タッチ前処理
        /// 保存タイミング: 正解/不正解画像を表示する時
        /// </remarks>
        public Point pointWrongShape = new Point(0, 0);

        /// <summary>
        /// 不正解図形情報
        /// </summary>
        public List<ShapeObject> incorrectShapeList = new List<ShapeObject>();

        public DateTime TimeEnterCage = new DateTime();

        /// <summary>
        /// トリガ画面タッチ時刻
        /// </summary>
        /// <remarks>
        /// 初期化タイミング: トリガ画面タッチ前処理
        /// 保存タイミング: トリガ画面タッチ時
        /// </remarks>
        public DateTime dateTimeTriggerTouch = new DateTime();
        /// <summary>
        /// 正解図形タッチ時刻
        /// </summary>
        /// <remarks>
        /// 初期化タイミング: トリガ画面タッチ前処理
        /// 保存タイミング: 正解タッチ時
        /// </remarks>
        public DateTime dateTimeCorrectTouch = new DateTime();
        /// <summary>
        /// タイムアウト時刻
        /// </summary>
        /// <remarks>
        /// 初期化タイミング: トリガ画面タッチ前処理
        /// 保存タイミング: タイムアウト発生時
        /// </remarks>
        public DateTime dateTimeout = new DateTime();

        //BlockProgramランダム用追加
        public int FeedTime = 0;
        public int FeedSoundTime = 0;
        public int DeleyTime = 0;
        public int FeedLampTime = 0;

        /// <summary>
        ///  不正解タッチ回数
        /// </summary>
        public int IncorrectCount = 0;

        /// <summary>
        /// タッチ座標
        /// </summary>
        public Point TouchPoint = new Point();

        /// <summary>
        /// 検出個体数
        /// </summary>
        public int DetectCount = 0;

        /// <summary>
        /// カメラ有効
        /// </summary>
        public bool EnableCamera = true;
        /// <summary>
        /// MultiID プログラム名
        /// ID or 0 default
        /// </summary>
        public string MultiIdProgName = "";
        #endregion
    }

}
