using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Compartment
{
    class UcOperationIf
    {
    }
    public partial class FormMain : Form
    {
        /// <summary>
        /// ルーム・ランプON
        /// ステータス検査用フラグ: なし
        /// 
        /// 実装ステータス: 済
        /// </summary>
        public void OpSetRoomLampOn()
        {
            TraceMessage("ルーム・ランプON");
            OpSubSetRoomLampOn();
        }
        /// <summary>
        /// ルーム・ランプOFF
        /// ステータス検査用フラグ: なし
        /// 
        /// 実装ステータス: 済
        /// </summary>
        public void OpSetRoomLampOff()
        {
            TraceMessage("ルーム・ランプOFF");
            OpSubSetRoomLampOff();
        }

        /// <summary>
        /// 入室検知ステータス・フラグ
        /// </summary>
        /// <remarks>
        /// 実装ステータス: 済
        /// </remarks>
        public bool OpFlagRoomIn { get => _OpFlagRoomIn.Value; set => _OpFlagRoomIn.Value = value; }

        public SyncObject<bool> _OpFlagRoomIn = new SyncObject<bool>(false);
        /// <summary>
        /// 退出検知ステータス・フラグ
        /// スレッドセーフ済み
        /// </summary>
        /// <remarks>
        /// 実装ステータス: 済
        /// </remarks>
        public bool OpFlagRoomOut
        {
            get
            {
                return _OpFlagRoomOut.Value;
            }
            set
            {
                _OpFlagRoomOut.Value = value;
            }
        }
        public SyncObject<bool> _OpFlagRoomOut = new SyncObject<bool>(false);

        public SyncObject<bool> EpisodeMode = new SyncObject<bool>(true); // 出題時True
#if false
		/// <summary>
		/// フィードON
		/// 内部で設定時間分Forwardで出力
		/// ステータス検査用フラグ: true/false
		/// 
		/// 実装ステータス: 済
		/// </summary>
		/// <param name="iTrialNoArg">
		/// 試行回数の何回目: 1回目:0
		/// </param>
		/// 
#endif


        /// <summary>
        /// フィードON
        /// 内部で設定時間分Forwardで出力
        /// ステータス検査用フラグ: true/false
        /// 
        /// 実装ステータス: 済
        /// </summary>
        /// <param name="iTrialNoArg">
        /// 試行回数の何回目: 1回目:0
        /// </param>
        /// <param name="boolDoesFeedArgOut">
        /// 実際にFeedしたか否かのフラグを出力
        /// </param>
        public void OpSetFeedOn(int iTrialNoArg, out bool boolDoesFeedArgOut)
        {
            OpFlagFeedOn = false;
            TraceMessage("フィードON");
            OpSubSetFeedOn(iTrialNoArg, out boolDoesFeedArgOut, devFeed);
        }
        public void OpSetFeedOn(int ms)
        {
            OpFlagFeedOn = false;
            TraceMessage("フィードON");
            OpSubSetFeedOn(ms);
        }
        public void OpSetFeed2On(int iTrialNoArg, out bool boolDoesFeedArgOut)
        {
            OpFlagFeedOn = false;
            TraceMessage("フィードON");
            OpSubSetFeedOn(iTrialNoArg, out boolDoesFeedArgOut, devFeed2);
        }
        public void OpSetFeed2On(int ms)
        {
            OpFlagFeedOn = false;
            TraceMessage("フィードON");
            OpSubSetFeed2On(ms);
        }
        /// <summary>
        /// フィードON用ステータス・フラグ
        /// ステータス検査用フラグ: true/false
        /// 
        /// 実装ステータス: 済
        /// </summary>
        public bool OpFlagFeedOn
        {
            get
            {
                return _OpFlagFeedOn.Value;
            }
            set
            {
                _OpFlagFeedOn.Value = value;
            }

        }
        public SyncObject<bool> _OpFlagFeedOn = new SyncObject<bool>(false);

        /// <summary>
        /// フィードOFF
        /// ステータス検査用フラグ: なし
        /// </summary>
        public void OpSetFeedOff()
        {
            TraceMessage("フィードOFF");
            OpSubSetFeedOff();
        }
        /// <summary>
        /// フィードOFF
        /// ステータス検査用フラグ: なし
        /// </summary>
        public void OpSetFeed2Off()
        {
            TraceMessage("フィード2OFF");
            OpSubSetFeed2Off();
        }
        /// <summary>
        /// フィード・スケジュールを行う
        /// </summary>
        /// <remarks>
        /// 実装ステータス: 済
        /// </remarks>
        public void OpMakeScheduleOfFeed()
        {
            TraceMessage("フィード・スケジュールを行う");
            OpSubMakeScheduleOfFeed();
        }
        /// <summary>
        /// FeedランプON
        /// ステータス検査用フラグ: なし
        /// 
        /// 実装ステータス: 済
        /// </summary>
        public void OpSetFeedLampOn()
        {
            TraceMessage("FeedランプON");
            OpSubSetFeedLampOn();
        }
        /// <summary>
        /// FeedランプOFF
        /// ステータス検査用フラグ: なし
        /// 
        /// 実装ステータス: 済
        /// </summary>
        public void OpSetFeedLampOff()
        {
            TraceMessage("FeedランプOFF");
            OpSubSetFeedLampOff();
        }

        /// <summary>
        /// Feedランプフラグ
        /// </summary>
        public bool OpFlagFeedLampON { get; set; } = false;

        /// <summary>
        /// ドア・オープン
        /// 内部でタイムアウト検知
        /// ステータス検査用フラグ: true/false
        /// 結果検査用プロパティ: int
        /// 
        /// 実装ステータス: 済
        /// </summary>
        public void OpOpenDoor()
        {
            OpFlagOpenDoor = false;
            TraceMessage("ドア・オープン");
            OpSubOpenDoor();
        }
        /// <summary>
        /// ドア・オープン用ステータス・フラグ
        /// ステータス検査用フラグ: true/false
        /// </summary>
        public bool OpFlagOpenDoor { get; set; } = false;

        /// <summary>
        /// ドア・オープン用結果
        /// </summary>
        public EDeviceResult OpResultOpenDoor { get; set; } = EDeviceResult.None;
        /// <summary>
        /// ドア・クローズ
        /// 内部でタイムアウト検知
        /// ステータス検査用フラグ: true/false
        /// 結果検査用プロパティ: int
        /// 
        /// 実装ステータス: 済
        /// </summary>
        public void OpCloseDoor()
        {
            OpFlagCloseDoor = false;
            TraceMessage("ドア・クローズ");
            OpSubCloseDoor();
        }
        /// <summary>
        /// ドア・クローズ用ステータス・フラグ
        /// ステータス検査用フラグ: true/false
        /// </summary>
        public bool OpFlagCloseDoor { get; set; } = false;

        /// <summary>
        /// ドア・クローズ用結果
        /// </summary>
        public EDeviceResult OpResultCloseDoor { get; set; } = EDeviceResult.None;

        /// <summary>
        /// ドア・ストップ
        /// 内部でタイムアウト検知
        /// ステータス検査用フラグ: true/false
        /// 結果検査用プロパティ: int
        /// 
        /// 実装ステータス: 済
        /// </summary>
        public void OpStopDoor()
        {
            TraceMessage("ドア・ストップ");
            OpSubStopDoor();
        }
        /// <summary>
        /// ドア・ストップ用ステータス・フラグ
        /// ステータス検査用フラグ: true/false
        /// </summary>
        public bool OpFlagStopDoor { get; set; } = false;

        /// <summary>
        /// ドア・クローズ用結果
        /// </summary>
        public EDeviceResult OpResultStopDoor { get; set; } = EDeviceResult.None;
        /// <summary>
        /// レバーOut(ケージ内にレバーが出現する)
        /// 内部でタイムアウト検知
        /// ステータス検査用フラグ: true/false
        /// 結果検査用プロパティ: int
        /// 
        /// 実装ステータス: 済
        /// </summary>
        public void OpMoveLeverOut()
        {
            OpFlagMoveLeverOut = false;
            TraceMessage("レバーOUT");
            OpSubMoveLeverOut();
        }
        /// <summary>
        /// レバーOut用ステータス・フラグ
        /// ステータス検査用フラグ: true/false
        /// </summary>
        public bool OpFlagMoveLeverOut { get; set; } = false;

        /// <summary>
        /// レバーOut用結果
        /// </summary>
        public EDeviceResult OpResultMoveLeverOut { get; set; } = EDeviceResult.None;
        /// <summary>
        /// レバーIn(レバーをケージから引き、ケージからなくなる)
        /// 内部でタイムアウト検知
        /// ステータス検査用フラグ: true/false
        /// 結果検査用プロパティ: int
        /// 
        /// 実装ステータス: 済
        /// </summary>
        public void OpMoveLeverIn()
        {
            OpFlagMoveLeverIn = false;
            TraceMessage("レバーIN");
            OpSubMoveLeverIn();
        }
        /// <summary>
        /// レバーIn用ステータス・フラグ
        /// ステータス検査用フラグ: true/false
        /// </summary>
        public bool OpFlagMoveLeverIn { get; set; } = false;

        /// <summary>
        /// レバーIn用結果
        /// </summary>
        public EDeviceResult OpResultMoveLeverIn { get; set; } = EDeviceResult.None;
        /// <summary>
        /// レバーStop
        /// 内部でタイムアウト検知
        /// ステータス検査用フラグ: true/false
        /// 結果検査用プロパティ: int
        /// 
        /// 実装ステータス: 済
        /// </summary>
        public void OpStopLever()
        {
            TraceMessage("レバーSTOP");
            OpSubStopLever();
        }
        /// <summary>
        /// レバーOut用ステータス・フラグ
        /// ステータス検査用フラグ: true/false
        /// </summary>
        public bool OpFlagStopLever { get; set; } = false;

        /// <summary>
        /// レバーAtop用結果
        /// </summary>
        public EDeviceResult OpResultStopLever { get; set; } = EDeviceResult.None;
        /// <summary>
        /// レバーSw用ステータス・フラグ
        /// ステータス検査用フラグ: true/false
        /// 
        /// 実装ステータス: 済
        /// </summary>
        public bool OpFlagLeverSw { get; set; } = false;

        /// <summary>
        /// レバー・ランプON
        /// ステータス検査用フラグ: なし
        /// 
        /// 実装ステータス: 済
        /// </summary>
        public void OpSetLeverLampOn()
        {
            TraceMessage("レバー・ランプON");
            OpSubSetLeverLampOn();
        }
        /// <summary>
        /// レバー・ランプOFF
        /// ステータス検査用フラグ: なし
        /// 
        /// 実装ステータス: 済
        /// </summary>
        public void OpSetLeverLampOff()
        {
            TraceMessage("レバー・ランプOFF");
            OpSubSetLeverLampOff();
        }

        /// <summary>
        /// 終了音を鳴らす
        /// ステータス検査用フラグ: なし
        /// 
        /// 実装ステータス: 済
        /// </summary>
        public void OpPlaySoundOfEnd()
        {
            TraceMessage(string.Format("終了音を鳴らす {0}ms", preferencesDatOriginal.TimeToOutputSoundOfEnd.ToString()));
            //PlaySoundLooping(preferencesDatOriginal.SoundFileOfEnd, preferencesDatOriginal.TimeToOutputSoundOfEnd);
            var task = System.Threading.Tasks.Task.Run(() =>
            {
                PlayMedia(preferencesDatOriginal.SoundFileOfEnd, preferencesDatOriginal.TimeToOutputSoundOfEnd);
            });

        }
        /// <summary>
        /// 報酬音を鳴らす
        /// ステータス検査用フラグ: なし
        /// 
        /// 実装ステータス: 済
        /// </summary>
        public void OpPlaySoundOfReward()
        {
            TraceMessage("報酬音を鳴らす");
            //PlaySoundLooping(preferencesDatOriginal.SoundFileOfReward, preferencesDatOriginal.TimeToOutputSoundOfReward);
            if (System.IO.File.Exists(preferencesDatOriginal.SoundFileOfReward))
            {
                PlaySound(preferencesDatOriginal.SoundFileOfReward);
            }
            else
            {
                TraceMessage("報酬音: " + preferencesDatOriginal.SoundFileOfReward + "が見つからない");
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public void OpPlaySoundOfIncorrectReward()
        {
            TraceMessage("不正解給餌音を鳴らす");
            //PlaySoundLooping(preferencesDatOriginal.SoundFileOfReward, preferencesDatOriginal.TimeToOutputSoundOfReward);
            if (System.IO.File.Exists(preferencesDatOriginal.SoundFileOfIncorrectReward))
            {
                PlaySound(preferencesDatOriginal.SoundFileOfIncorrectReward);
            }
            else
            {
                TraceMessage("不正解給餌音: " + preferencesDatOriginal.SoundFileOfIncorrectReward + "が見つからない");
            }
        }
        /// <summary>
        /// 正解音を鳴らす
        /// </summary>
        public void OpPlaySoundOfCorrect()
        {
            TraceMessage("正解音を鳴らす");
            if (System.IO.File.Exists(preferencesDatOriginal.SoundFileOfCorrect))
            {
                PlaySound(preferencesDatOriginal.SoundFileOfCorrect);
            }
            else
            {
                TraceMessage("正解音: " + preferencesDatOriginal.SoundFileOfCorrect + "が見つからない");
            }
        }
        /// <summary>
        /// 不正解給餌音を鳴らす
        /// </summary>
        public void OpPlaySoundOfIncorrect()
        {
            TraceMessage("不正解音を鳴らす");
            if (System.IO.File.Exists(preferencesDatOriginal.SoundFileOfIncorrect))
            {
                PlaySound(preferencesDatOriginal.SoundFileOfIncorrect);
            }
            else
            {
                TraceMessage("不正解音: " + preferencesDatOriginal.SoundFileOfIncorrect + "が見つからない");
            }
        }
        /// <summary>
        /// タッチ・パネルを背景色でクリアする
        /// ステータス検査用フラグ: なし
        /// 
        /// 実装ステータス: 済
        /// </summary>
        public void OpDrawBackColorOnTouchPanel()
        {
            TraceMessage("タッチ・パネルを背景色" + opImage.ColorOpeImageBackColor.ToString() + "でクリアする");
            opImage.DrawBackColor();
        }
        /// <summary>
        /// タッチ・パネルを黒でクリアする
        /// ステータス検査用フラグ: なし
        /// 
        /// 実装ステータス: 済
        /// </summary>
        public void OpDrawBackColorBlackOnTouchPanel()
        {
            TraceMessage("タッチ・パネルを黒でクリアする");
            opImage.DrawBackColor(Color.Black);
        }
        /// <summary>
        /// タッチ・パネルをディレイ時色でクリアする
        /// ステータス検査用フラグ: なし
        /// 
        /// 実装ステータス: 済
        /// </summary>
        public void OpDrawDelayBackColorOnTouchPanel()
        {
            TraceMessage("タッチ・パネルをディレイ時背景色" + opImage.ColorOpeImageDelayBackColor.ToString() + "でクリアする");
            opImage.DrawBackColor(opImage.ColorOpeImageDelayBackColor);
        }
        /// <summary>
        /// タッチ・パネルへトリガ画像を表示する
        /// ステータス検査用フラグ: なし
        /// 
        /// 実装ステータス: 済
        /// </summary>
        public void OpDrawTriggerImageOnTouchPanel()
        {
            if (EpisodeActive.Value)
            {
                if (System.IO.File.Exists(preferencesDatOriginal.EpisodeTriggerImageFile))
                {
                    TraceMessage("タッチ・パネルへトリガ画像を表示する" + System.IO.Path.GetFileName(preferencesDatOriginal.EpisodeTriggerImageFile));
                    opImage.DrawImage(preferencesDatOriginal.EpisodeTriggerImageFile);
                }
                else
                {
                    TraceMessage("タッチ・パネルへトリガ画像エラー ファイル無し" + System.IO.Path.GetFileName(preferencesDatOriginal.EpisodeTriggerImageFile));
                }
            }
            else
            {
                string imageFilePath = preferencesDatOriginal.TriggerImageFile;
                TraceMessage("タッチ・パネルへトリガ画像を表示する" + System.IO.Path.GetFullPath(imageFilePath));
                opImage.DrawImage(imageFilePath);
            }
        }
        /// <summary>
        /// タッチ・パネルへトリガ画像を表示する ファイル引数
        /// ステータス検査用フラグ: なし
        /// 
        /// 実装ステータス: 済
        /// </summary>
        public void OpDrawTriggerImageOnTouchPanel(string imageFilePath)
        {
            if (EpisodeActive.Value)
            {
                if (System.IO.File.Exists(preferencesDatOriginal.EpisodeTriggerImageFile))
                {
                    TraceMessage("タッチ・パネルへトリガ画像を表示する" + System.IO.Path.GetFileName(preferencesDatOriginal.EpisodeTriggerImageFile));
                    opImage.DrawImage(preferencesDatOriginal.EpisodeTriggerImageFile);
                }
                else
                {
                    TraceMessage("タッチ・パネルへトリガ画像エラー ファイル無し" + System.IO.Path.GetFileName(preferencesDatOriginal.EpisodeTriggerImageFile));
                }
            }
            else
            {
                if (System.IO.File.Exists(imageFilePath))
                {
                    TraceMessage("タッチ・パネルへトリガ画像を表示する" + System.IO.Path.GetFileName(imageFilePath));
                    opImage.DrawImage(imageFilePath);
                }
                else
                {
                    TraceMessage("タッチ・パネルへトリガ画像エラー ファイル無し" + System.IO.Path.GetFileName(imageFilePath));
                }
            }
        }
        /// <summary>
        /// タッチ・パネルへ正解図形を表示する 未使用
        /// ステータス検査用フラグ: なし
        /// </summary>
        /// <remarks>
        /// 実装ステータス: 済
        /// </remarks>
        public void OpDrawCorrectShapeOnTouchPanel(out Point pointCorrectShapeArgOut)
        {
            OpSubDrawCorrectShapeOnTouchPanel(out pointCorrectShapeArgOut);
        }
        /// <summary>
        /// タッチ・パネルへ訓練図形を表示する ShapeObject
        /// ステータス検査用フラグ: なし
        /// </summary>
        /// <remarks>
        /// 実装ステータス: 済
        /// </remarks>
        public void OpDrawTrainShapeOnTouchPanel(out ShapeObject shapeObject)
        {
            OpSubDrawTrainShapeOnTouchPanel(out shapeObject);
        }

        public void OpDrawTrainShapeOnTouchPanelAfterIllegalExit(ShapeObject shapeObject)
        {
            OpSubDrawTrainShapeOnTouchPanelAfterIllegalExit(shapeObject);
        }
        /// <summary>
        /// タッチ・パネルへ正解図形を表示する ShapeObject
        /// ステータス検査用フラグ: なし
        /// </summary>
        /// <remarks>
        /// 実装ステータス: 済
        /// </remarks>
        public void OpDrawCorrectShapeOnTouchPanel(out ShapeObject shapeObject)
        {
            OpSubDrawCorrectShapeOnTouchPanel(out shapeObject);
        }

        /// <summary>
        /// タッチパネル訓練図形Block用
        /// </summary>
        /// <param name="shapeObject"></param>
        public void OpDrawTrainShapeOnTouchPanelBlock(out ShapeObject shapeObject)
        {
            if (!EpisodeActive.Value)
            {
                OpSubDrawTrainShapeOnTouchPanelBlock(out shapeObject);
            }
            else
            {
                shapeObject = new ShapeObject();
            }
        }
        /// <summary>
        /// タッチパネル正解図形Block用
        /// </summary>
        /// <param name="shapeObject"></param>
        public void OpDrawCorrectShapeOnTouchPanelBlock(out ShapeObject shapeObject)
        {
            OpSubDrawCorrectShapeOnTouchPanelBlock(out shapeObject);
        }

        public void OpDrawCorrectAndWrongShapeOnTouchPanelBlock(out ShapeObject correctShapeObject, out List<ShapeObject> incorrectShapeObjects)
        {
            try
            {
                // 実行時に判断
                if (!EpisodeActive.Value)
                {
                    OpSubDrawCorrectAndWrongShapeOnTouchPanelBlock(out correctShapeObject, out incorrectShapeObjects);
                }
                else
                {
                    OpSubCheckIdExpire();

                    if (EpisodeMode.Value)
                    {
                        OpSubDrawAnyShapeOnTouchPanelBlockEpisodeMemory(out correctShapeObject, out incorrectShapeObjects);
                    }
                    else
                    {
                        OpSubDrawCorrectAndWrongShapeOnTouchPanelBlockEpisodeMemory(out correctShapeObject, out incorrectShapeObjects);
                    }

                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
            }

        }
        /// <summary>
        /// タッチ・パネルへ正解図形+不正解図形を表示する
        /// ステータス検査用フラグ: なし
        /// </summary>
        /// <remarks>
        /// 実装ステータス: 済
        /// </remarks>
        public void OpDrawCorrectAndWrongShapeOnTouchPanel(out Point pointWrongShapeArgOut)
        {
            try
            {
                OpSubDrawCorrectAndWrongShapeOnTouchPanel(out pointWrongShapeArgOut);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {

            }
        }

        /// <summary>
        /// タッチ・パネルへ正解図形+不正解図形を表示する ShapeObject
        /// ステータス検査用フラグ: なし
        /// </summary>
        /// <remarks>
        /// 実装ステータス: 済
        /// </remarks>
        public void OpDrawCorrectAndWrongShapeOnTouchPanel(out ShapeObject correctShapeObject, out List<ShapeObject> incorrectShapeObjects)
        {
            try
            {
                OpSubDrawCorrectAndWrongShapeOnTouchPanel(out correctShapeObject, out incorrectShapeObjects);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
            }
        }

        /// <summary>
        /// タッチ・パネルで課題開始
        /// ステータス検査用フラグ: true/false
        /// </summary>
        /// <remarks>
        /// 実装ステータス: 済
        /// </remarks>
        public void OpStartToTouchCorrectOnTouchPanel()
        {
            OpFlagTouchCorrectOnTouchPanel = false;
            OpFlagTouchIncorrectOnTouchPanel = false;
            OpSubStartToTouchCorrectOnTouchPanel();
            OpSubTouchClear();
        }
        public void OpStartToTouchAnyCorrectOnTouchPanel()
        {
            OpFlagTouchCorrectOnTouchPanel = false;
            OpFlagTouchIncorrectOnTouchPanel = false;
            OpSubStartToTouchAnyCorrectOnTouchPanel();
            OpSubTouchClear();
        }
        /// <summary>
        /// タッチ・パネルで課題終了
        /// ステータス検査用フラグ: なし
        /// </summary>
        /// <remarks>
        /// 実装ステータス: 済
        /// </remarks>
        public void OpEndToTouchCorrectOnTouchPanel()
        {
            OpSubEndToTouchCorrectOnTouchPanel();
        }
        /// <summary>
        /// 課題用ステータス・フラグ
        /// ステータス検査用フラグ: true/false
        /// 
        /// 実装ステータス: 済(Device側で、セットした)
        /// スレッドセーフ
        /// </summary>
        public bool OpFlagTouchCorrectOnTouchPanel
        {
            get => _OpFlagTouchCorrectOnTouchPanel.Value;
            set => _OpFlagTouchCorrectOnTouchPanel.Value = value;
        }

        private readonly SyncObject<bool> _OpFlagTouchCorrectOnTouchPanel = new SyncObject<bool>(false);

        /// <summary>
        /// 課題用ステータス 不正解タッチされたよフラグ
        /// スレッドセーフ
        /// </summary>
        public bool OpFlagTouchIncorrectOnTouchPanel
        {
            get => _OpFlagTouchIncorrectOnTouchPanel.Value;
            set => _OpFlagTouchIncorrectOnTouchPanel.Value = value;
        }
        private readonly SyncObject<bool> _OpFlagTouchIncorrectOnTouchPanel = new SyncObject<bool>(false);

        /// <summary>
        /// タッチ・パネルでどこでもタッチ検知開始
        /// ステータス検査用フラグ: true/false
        /// </summary>
        /// <remarks>
        /// 実装ステータス: 済
        /// </remarks>
        public void OpStartToTouchAnyOnTouchPanel()
        {
            OpFlagTouchAnyOnTouchPanel = false;
            OpSubStartToTouchAnyOnTouchPanel();
            OpSubTouchClear();
        }
        /// <summary>
        /// タッチ・パネルでどこでもタッチ検知終了
        /// ステータス検査用フラグ: なし
        /// </summary>
        /// <remarks>
        /// 実装ステータス: 済
        /// </remarks>
        public void OpEndToTouchAnyOnTouchPanel()
        {
            OpSubEndToTouchAnyOnTouchPanel();
        }
        /// <summary>
        /// タッチ・パネルでどこでもタッチ検知用ステータス・フラグ
        /// ステータス検査用フラグ: true/false
        /// スレッドセーフ
        /// </summary>
        public bool OpFlagTouchAnyOnTouchPanel
        {
            get => _OpFlagTouchAnyOnTouchPanel.Value;
            set => _OpFlagTouchAnyOnTouchPanel.Value = value;
        }
        private readonly SyncObject<bool> _OpFlagTouchAnyOnTouchPanel = new SyncObject<bool>(false);

        /// <summary>
        /// インターバル時間[s]を取得する
        /// </summary>
        /// <remarks>
        /// 実装ステータス: 済
        /// </remarks>
        /// <returns>
        /// int: 決定したインターバル時間[s]
        /// </returns>
        public int OpGetIntervalTime()
        {
            return OpSubGetIntervalTime();
        }
        /// <summary>
        /// タスク・タイプを取得する
        /// </summary>
        /// <returns>
        /// CpTask: タスク・タイプ
        /// </returns>
        /// <remarks>
        /// 実装ステータス: 済
        /// </remarks>
        public ECpTask OpeGetTypeOfTask()
        {
            return OpSubGetTypeOfTask();
        }

        public String OpeGetIdCode()
        {
            return IdCodeInOperation.Value;
        }
        public void OpeClearIdCode()
        {
            mIdCode0.Value = "";
            IdCodeInOperation.Value = "";
        }
        /// <summary>
        /// エアパフをONする
        /// </summary>
        /// <remarks>
        /// 実装ステータス: 済
        /// </remarks>
        public void OpAirPuffOn()
        {
            OpSubAirPuffOn();
        }
        /// <summary>
        /// エアパフをOFFする
        /// </summary>
        /// <remarks>
        /// 実装ステータス: 済
        /// </remarks>
        public void OpAirPuffOff()
        {
            _ = OpSubAirPuffOff();
        }
        /// <summary>
        /// エピソード記憶状態チェック
        /// </summary>
        public void OpCheckEpisode()
        {
            CheckEpisodeCount();
        }
    }
}
