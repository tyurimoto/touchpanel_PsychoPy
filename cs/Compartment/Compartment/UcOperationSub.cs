using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Compartment
{
    class UcOperationSub
    {
    }
    public partial class FormMain : Form
    {
        #region デバッグ・トレース用コード
        public bool TraceMessage2OutDisplayFlag { get; set; } = true;
        public bool TraceMessage2OutLogFileFlag { get; set; } = true;
        public void TraceMessage2(string message,
        [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
        [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
        [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)
        {
            string str = "";
            str += "[TraceMessage2]" + message + ",";
            //			str += "関数名=" + memberName + ",";
            //			str += "ファイル名=" + Path.GetFileName(sourceFilePath) + ",";
            //			str += "行=" + sourceLineNumber;

            WriteLineStatus(str);
        }
        #endregion

        public static bool PlaySoundEnded { private set; get; } = false;

        /// <summary>
        /// 音声ループ再生
        /// </summary>
        /// <param name="waveFile">再生ファイル</param>
        /// <param name="milliseconds">再生時間</param>
        public static void PlaySoundLooping(string waveFile, int milliseconds)
        {
            PlaySoundEnded = false;
            Stopwatch sw = new Stopwatch();
            if (!System.IO.File.Exists(waveFile))
            {
                return;
            }
            //var task = Task.Run(async () =>
            //{
            //    await 
            //Task.Run(() =>
            // {
            using (System.Media.SoundPlayer player = new System.Media.SoundPlayer())
            {
                try
                {
                    sw.Start();
                    player.SoundLocation = waveFile;
                    player.Load();
                    player.PlayLooping();
                    //player.PlaySync();
                    while (sw.ElapsedMilliseconds < milliseconds)
                    {
                        Thread.Sleep(1);
                    }
                }
                catch
                {
                    player.Stop();
                }
                finally
                {
                    player.Stop();
                    sw.Stop();
                }

                player.Stop();
                PlaySoundEnded = true;
            }
            //});
            //});
        }

        /// <summary>
        /// 音声再生
        /// </summary>
        /// <param name="waveFile">再生ファイル</param>
        public static void PlaySound(string waveFile)
        {
            PlaySoundEnded = false;
            if (!System.IO.File.Exists(waveFile))
            {
                return;
            }
            //var task = Task.Run(async () =>
            //{
            //    await
            //Task.Run(() =>
            //{
            using (System.Media.SoundPlayer player = new System.Media.SoundPlayer())
            {
                try
                {
                    player.SoundLocation = waveFile;
                    player.Load();
                    player.PlaySync();
                }
                catch
                {
                    player.Stop();
                }
                player.Stop();
                PlaySoundEnded = true;
            }
            //});
            //});
        }

        public static void PlayMedia(string path)
        {
            Debug.Assert(path != "");

            if (!System.IO.File.Exists(path))
            {
                Debug.WriteLine(string.Format("ファイルが存在しません,{0}", path));
                return;
            }
            AudioFileReader reader = new AudioFileReader(path);
            WaveOut waveOut = new WaveOut();
            try
            {

                waveOut?.Init(reader);
                waveOut?.Play();
            }
            catch (Exception)
            {

                throw;
            }
            //Stopwatch sw = new Stopwatch();
            //sw.Start();
            //while (waveOut?.PlaybackState != PlaybackState.Stopped)
            //{
            //    if (sw.ElapsedMilliseconds > 10000)
            //    {
            //        waveOut?.Stop();
            //        waveOut?.Dispose();
            //        waveOut = null;
            //        break;
            //    }
            //    Thread.Sleep(1);
            //}

        }
        /// <summary>
        /// PlayMedia
        /// WindowsMediaPlayerを使用して再生(非Task)
        /// </summary>
        /// <param name="path">音声ファイルパス</param>
        /// <param name="player">WindowsMediaPlayerインスタンス</param>
        public static void PlayMedia(string path, out WaveOut player)
        {
            Debug.Assert(path != "");

            if (!System.IO.File.Exists(path))
            {
                Debug.WriteLine(string.Format("ファイルが存在しません,{0}", path));
                player = null;
                return;
            }
            //player = new WMPLib.WindowsMediaPlayer();
            PlaySoundEnded = false;
            AudioFileReader reader = new AudioFileReader(path);
            player = new WaveOut();
            player.Init(reader);
            player.Play();
        }

        public static void PlayMedia(string path, int milliseconds)
        {
            try
            {
                if (!System.IO.File.Exists(path))
                {
                    //再生ファイルがなかった時
                    return;
                }
                AudioFileReader reader = new AudioFileReader(path);

                WaveOut waveOut = new WaveOut();
                waveOut?.Init(reader);
                waveOut?.Play();
                Stopwatch sw = new Stopwatch();
                sw.Start();
                while (true)
                {
                    if (sw.ElapsedMilliseconds > milliseconds)
                    {
                        waveOut?.Stop();
                        waveOut?.Dispose();
                        waveOut = null;
                        break;
                    }
                    if (waveOut?.PlaybackState == PlaybackState.Stopped)
                    {
                        reader.Position = 0;
                        waveOut.Play();
                    }
                    Thread.Sleep(1);

                }
            }
            catch (Exception)
            {
                throw;
            }

        }

        public void OpSubOpenDoor()
        {
            DevCmdPkt devCmdPktObj = new DevCmdPkt();

            devCmdPktObj.DevCmdVal = EDevCmd.DoorOpen;
            concurrentQueueDevCmdPktDoor.Enqueue(devCmdPktObj);
        }
        public void OpSubCloseDoor()
        {
            DevCmdPkt devCmdPktObj = new DevCmdPkt();

            devCmdPktObj.DevCmdVal = EDevCmd.DoorClose;
            concurrentQueueDevCmdPktDoor.Enqueue(devCmdPktObj);
        }
        public void OpSubStopDoor()
        {
            DevCmdPkt devCmdPktObj = new DevCmdPkt();

            devCmdPktObj.DevCmdVal = EDevCmd.DoorStop;
            concurrentQueueDevCmdPktDoor.Enqueue(devCmdPktObj);
        }
        /// <summary>
        /// FeedOnのサブ関数
        /// </summary>
        /// <param name="iTrialNoArg">
        /// 試行回数の何回目: 1回目:0
        /// </param>
        public void OpSubSetFeedOn(int iTrialNoArg, out bool boolDoesFeedArgOut, DevFeedMulti dFeed)
        {
            bool boolDoesFeed = true;
            int iRandValue;
            DevCmdPkt devCmdPktObj = new DevCmdPkt();
            Random randomValue = new System.Random();

            if (preferencesDatOriginal.OpeNumberOfTrial >= 1)
            {
                // 試行回数設定ある時
                // 現在の試行回目を使用してスケジューリング結果を引き、登録されていれば、Feedする
                if ((mDictionaryScheduleOfFeed != null) &&
                   (mDictionaryScheduleOfFeed.ContainsKey(iTrialNoArg) == true))
                {
                    boolDoesFeed = true;
                }
                else
                {
                    boolDoesFeed = false;
                }
                //				TraceMessage2(String.Format("FeedOn: TrialNo[{0}]/[{1}] {2} Time:{3}",
                //					iTrialNoArg,
                //					preferencesDatOriginal.OpeNumberOfTrial,
                //					boolDoesFeed == true ?"On":"Off",
                //					preferencesDatOriginal.OpeTimeToFeed));
            }
            else
            {
                // 試行回数設定ない時(無限回の時)
                if (preferencesDatOriginal.OpeFeedingRate <= 0)
                {
                    // Feed確率≦0の時、Feedなし
                    boolDoesFeed = false;
                }
                else
                {
                    // Feed確率＞0の時、乱数から決定
                    iRandValue = globalRandomValue.Next(1, 100 + 1);
                    if (iRandValue <= preferencesDatOriginal.OpeFeedingRate)
                    {
                        // 確率内の時、Feedする
                        boolDoesFeed = true;
                    }
                    else
                    {
                        // 確率外時、Feedしない
                        boolDoesFeed = false;
                    }
                }
                //				TraceMessage2(String.Format("FeedOn: NoLimit[{0}] {1} Time{2}",
                //					iTrialNoArg,
                //					boolDoesFeed == true ? "On" : "Off",
                //					preferencesDatOriginal.OpeTimeToFeed));
            }
            // Feed時間≦0の時
            if (preferencesDatOriginal.OpeTimeToFeed <= 0)
            {
                // Feedしない
                boolDoesFeed = false;
            }

            if (boolDoesFeed)
            {
                // Feed実行する時
                devCmdPktObj.DevCmdVal = EDevCmd.FeedForward;
                devCmdPktObj.iParam[0] = preferencesDatOriginal.OpeTimeToFeed;
                //concurrentQueueDevCmdPktFeed.Enqueue(devCmdPktObj);
                dFeed.CommandEnqueue(devCmdPktObj);

                //				{
                //					TraceMessage2(String.Format("ActFeed: TrialNo:{0} Time:{1}",
                //									iTrialNoArg +1,
                //									preferencesDatOriginal.OpeTimeToFeed));
                //				}
            }
            else
            {
                // Feed実行しない時
                // MainステートマシンへFeed完了を設定
                OpFlagFeedOn = true;
                //				{
                //					TraceMessage2(String.Format("NoActFeed: TrialNo:{0}",
                //									iTrialNoArg + 1));
                //				}
            }
            // Feedしたか否かを出力
            boolDoesFeedArgOut = boolDoesFeed;
        }
        public void OpSubSetFeedOn(int ms)
        {
            DevCmdPkt devCmdPktObj = new DevCmdPkt();

            devCmdPktObj.DevCmdVal = EDevCmd.FeedForward;
            devCmdPktObj.iParam[0] = ms;
            //concurrentQueueDevCmdPktFeed.Enqueue(devCmdPktObj);
            devFeed.CommandEnqueue(devCmdPktObj);
        }
        /// <summary>
        /// FeedOffのサブ関数
        /// </summary>
        public void OpSubSetFeedOff()
        {
            DevCmdPkt devCmdPktObj = new DevCmdPkt();

            devCmdPktObj.DevCmdVal = EDevCmd.FeedStop;
            //concurrentQueueDevCmdPktFeed.Enqueue(devCmdPktObj);
            devFeed.CommandEnqueue(devCmdPktObj);
        }
        public void OpSubSetFeed2On(int ms)
        {
            DevCmdPkt devCmdPktObj = new DevCmdPkt();

            devCmdPktObj.DevCmdVal = EDevCmd.FeedForward;
            devCmdPktObj.iParam[0] = ms;
            //concurrentQueueDevCmdPktFeed.Enqueue(devCmdPktObj);
            devFeed2.CommandEnqueue(devCmdPktObj);
        }
        /// <summary>
        /// FeedOffのサブ関数
        /// </summary>
        public void OpSubSetFeed2Off()
        {
            DevCmdPkt devCmdPktObj = new DevCmdPkt();

            devCmdPktObj.DevCmdVal = EDevCmd.FeedStop;
            //concurrentQueueDevCmdPktFeed.Enqueue(devCmdPktObj);
            devFeed2.CommandEnqueue(devCmdPktObj);
        }
        /// <summary>
        /// 試行回数指定ありの時、Feedスケジュールを格納する
        /// 
        /// </summary>
        public Dictionary<int, bool> mDictionaryScheduleOfFeed = null;
        public void OpSubMakeScheduleOfFeed()
        {
            Random randomValue = new System.Random();
            int iNumOfFeed;
            int iTrialNo;

            mDictionaryScheduleOfFeed = new Dictionary<int, bool>();
            mDictionaryScheduleOfFeed.Clear();

            // 試行回数設定≦0の時、スケジューリングは行わない(mDictionaryScheduleOfFeedの要素数==0のままとなる)
            if (preferencesDatOriginal.OpeNumberOfTrial <= 0)
            {
                return;
            }
            // 試行回数設定ある時
            // Feed確率≦0の時(mDictionaryScheduleOfFeedの要素数==0のままとなる)
            if (preferencesDatOriginal.OpeFeedingRate <= 0)
            {
                return;
            }
            // Feed確率＞0の時、割り当てるFeed回数算出(小数点以下は切り捨て)
            iNumOfFeed = (int)((Decimal)(preferencesDatOriginal.OpeNumberOfTrial * preferencesDatOriginal.OpeFeedingRate)
                                / ((Decimal)100));
            // 割り当てるFeed回数≦0の時(mDictionaryScheduleOfFeedの要素数==0のままとなる)
            if (iNumOfFeed <= 0)
            {
                return;
            }
            // 割り当てるFeed回数≧1の時、割り当てるFeed回数分を試行回目に割り当てる
            for (int iTryNo = 0; iTryNo < iNumOfFeed; iTryNo++)
            {
                // 
                do
                {
                    iTrialNo = globalRandomValue.Next(0, preferencesDatOriginal.OpeNumberOfTrial);
                } while (mDictionaryScheduleOfFeed.ContainsKey(iTrialNo) == true);
                mDictionaryScheduleOfFeed.Add(iTrialNo, true);
            }
        }
        public void OpSubMoveLeverOut()
        {
            DevCmdPkt devCmdPktObj = new DevCmdPkt();

            devCmdPktObj.DevCmdVal = EDevCmd.LeverOut;
            concurrentQueueDevCmdPktLever.Enqueue(devCmdPktObj);
        }
        public void OpSubMoveLeverIn()
        {
            DevCmdPkt devCmdPktObj = new DevCmdPkt();

            devCmdPktObj.DevCmdVal = EDevCmd.LeverIn;
            concurrentQueueDevCmdPktLever.Enqueue(devCmdPktObj);
        }
        public void OpSubStopLever()
        {
            DevCmdPkt devCmdPktObj = new DevCmdPkt();

            devCmdPktObj.DevCmdVal = EDevCmd.LeverStop;
            concurrentQueueDevCmdPktLever.Enqueue(devCmdPktObj);
        }

        public bool OpSubSetLeverLampOn()
        {
            bool boolRet = true;
            bool boolFuncRet;

            boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.LeverLampOn);
            if (boolFuncRet != true)
            {
                Debug.WriteLine("OpSubLeverLampOn(): IoBoardDevice.SetUpperStateOfDOut()エラー");
                boolRet = false;
            }
            return boolRet;
        }
        public bool OpSubSetLeverLampOff()
        {
            bool boolRet = true;
            bool boolFuncRet;

            boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.LeverLampOff);
            if (boolFuncRet != true)
            {
                Debug.WriteLine("OpSubLeverLampOff(): IoBoardDevice.SetUpperStateOfDOut()エラー");
                boolRet = false;
            }
            return boolRet;
        }
        public bool OpSubAirPuffOn()
        {
            bool boolRet = true;
            bool boolFuncRet;

            boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.AirPuffOn);
            if (boolFuncRet != true)
            {
                Debug.WriteLine("OpSubAirPuffOn(): IoBoardDevice.SetUpperStateOfDOut()エラー");
                boolRet = false;
            }
            return boolRet;
        }
        public bool OpSubAirPuffOff()
        {
            bool boolRet = true;
            bool boolFuncRet;

            boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.AirPuffOff);
            if (boolFuncRet != true)
            {
                Debug.WriteLine("OpSubAirPuffOff(): IoBoardDevice.SetUpperStateOfDOut()エラー");
                boolRet = false;
            }
            return boolRet;
        }
        public bool OpSubSetRoomLampOn()
        {
            bool boolRet = true;
            bool boolFuncRet;

            boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.RoomLampOn);
            if (boolFuncRet != true)
            {
                Debug.WriteLine("OpSubRoomLampOn(): IoBoardDevice.SetUpperStateOfDOut()エラー");
                boolRet = false;
            }
            return boolRet;
        }
        public bool OpSubSetRoomLampOff()
        {
            bool boolRet = true;
            bool boolFuncRet;

            boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.RoomLampOff);
            if (boolFuncRet != true)
            {
                Debug.WriteLine("OpSubRoomLampOff(): IoBoardDevice.SetUpperStateOfDOut()エラー");
                boolRet = false;
            }
            return boolRet;
        }
        public bool OpSubSetFeedLampOn()
        {
            bool boolRet = true;
            bool boolFuncRet;

            boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.FeedLampOn);
            if (boolFuncRet != true)
            {
                Debug.WriteLine("OpSubFeedLampOn(): IoBoardDevice.SetUpperStateOfDOut()エラー");
                boolRet = false;
            }
            return boolRet;
        }
        public bool OpSubSetFeedLampOff()
        {
            bool boolRet = true;
            bool boolFuncRet;

            boolFuncRet = ioBoardDevice.SetUpperStateOfDOut(IoBoardDOutLogicalName.FeedLampOff);
            if (boolFuncRet != true)
            {
                Debug.WriteLine("OpSubFeedLampOff(): IoBoardDevice.SetUpperStateOfDOut()エラー");
                boolRet = false;
            }
            return boolRet;
        }
        public int OpSubGetIntervalTime()
        {
            int iIntervalTime;
            int iMinTime;
            int iMaxTime;
            Random randomValue = new System.Random();

            iMinTime = preferencesDatOriginal.OpeIntervalTimeMinimum;
            if (preferencesDatOriginal.OpeIntervalTimeMinimum <= 0)
            {
                iMinTime = 0;
            }
            iMaxTime = preferencesDatOriginal.OpeIntervalTimeMaximum;
            if (preferencesDatOriginal.OpeIntervalTimeMaximum <= 0)
            {
                iMaxTime = 0;
            }
            // 最小値＞最大値の時、最大値=最小値とする
            if (iMinTime > iMaxTime)
            {
                iMaxTime = iMinTime;
            }
            iIntervalTime = globalRandomValue.Next(iMinTime, iMaxTime + 1);

            //			TraceMessage2(String.Format("DesidedInterval: {0}", iIntervalTime));
            return iIntervalTime;
        }
        public int OpSubGetAfterMatchRandomTime()
        {
            int ret;
            int minTime;
            int maxTime;
            Random randomValue = new System.Random();

            minTime = preferencesDatOriginal.OpeRandomTimeMinimum;
            if (preferencesDatOriginal.OpeRandomTimeMinimum <= 0)
            {
                minTime = 0;
            }
            maxTime = preferencesDatOriginal.OpeRandomTimeMaximum;
            if (preferencesDatOriginal.OpeRandomTimeMaximum <= 0)
            {
                maxTime = 0;
            }
            // 最小値＞最大値の時、最大値=最小値とする
            if (minTime > maxTime)
            {
                maxTime = minTime;
            }
            ret = globalRandomValue.Next(minTime, maxTime + 1);

            return ret;
        }

        public ECpTask OpeSubGetTypeOfTask()
        {
            ECpTask cpTaskRet;

            if (preferencesDatOriginal.OpeTypeOfTask == ECpTask.Training.ToString())
            {
                cpTaskRet = ECpTask.Training;
            }
            else if (preferencesDatOriginal.OpeTypeOfTask == ECpTask.DelayMatch.ToString())
            {
                cpTaskRet = ECpTask.DelayMatch;
            }
            else if (preferencesDatOriginal.OpeTypeOfTask == ECpTask.TrainingEasy.ToString())
            {
                cpTaskRet = ECpTask.TrainingEasy;
            }
            else if (preferencesDatOriginal.OpeTypeOfTask == ECpTask.UnConditionalFeeding.ToString())
            {
                cpTaskRet = ECpTask.UnConditionalFeeding;
            }
            else
            {
                cpTaskRet = ECpTask.Training;
            }
            return cpTaskRet;
        }
        public void OpSubStartToTouchCorrectOnTouchPanel()
        {
            DevCmdPkt devCmdPktObj = new DevCmdPkt();

            devCmdPktObj.DevCmdVal = EDevCmd.TouchPanelCorrectShape;
            concurrentQueueDevCmdPktTouchPanel.Enqueue(devCmdPktObj);
        }
        public void OpSubStartToTouchAnyCorrectOnTouchPanel()
        {
            DevCmdPkt devCmdPktObj = new DevCmdPkt();
            devCmdPktObj.DevCmdVal = EDevCmd.TouchPanelCorrectShapeAny;
            concurrentQueueDevCmdPktTouchPanel.Enqueue(devCmdPktObj);
        }
        public void OpSubEndToTouchCorrectOnTouchPanel()
        {
            DevCmdPkt devCmdPktObj = new DevCmdPkt();

            devCmdPktObj.DevCmdVal = EDevCmd.TouchPanelStop;
            concurrentQueueDevCmdPktTouchPanel.Enqueue(devCmdPktObj);
        }
        public void OpSubStartToTouchAnyOnTouchPanel()
        {
            DevCmdPkt devCmdPktObj = new DevCmdPkt();

            devCmdPktObj.DevCmdVal = EDevCmd.TouchPanelTouchAny;
            concurrentQueueDevCmdPktTouchPanel.Enqueue(devCmdPktObj);
        }
        public void OpSubEndToTouchAnyOnTouchPanel()
        {
            DevCmdPkt devCmdPktObj = new DevCmdPkt();

            devCmdPktObj.DevCmdVal = EDevCmd.TouchPanelStop;
            concurrentQueueDevCmdPktTouchPanel.Enqueue(devCmdPktObj);
        }
        /// <summary>
        /// 訓練正答をタッチパネルに描画
        /// </summary>
        /// <param name="pointCorrectShapeArgOut">描画位置</param>
        /// <returns>bool</returns>
        public bool OpSubDrawCorrectShapeOnTouchPanel(out Point pointCorrectShapeArgOut)
        {
            //Random BgColor用 パラメータ更新
            opImage.SetBgColor();
            opImage.SetDelayBgColor();
            //opImage.MakePointOfCorrectShapeOpeImage();
            //opImage.MakePathOfCorrectShapeOpeImage();
            opImage.MakeCorrectShape();
            opImage.DrawBackColor();
            _ = opImage.DrawShapeOpeImage(opImage.ShapeOpeImageTrainShape);

            pointCorrectShapeArgOut = opImage.ShapeOpeImageCorrectShape.Point;
            //			{
            //				TraceMessage2(String.Format("CorrectShape: Point(X:{0} Y:{1})",
            //								PointOpeImageCenterOfCorrectShape.X,
            //								PointOpeImageCenterOfCorrectShape.Y));
            //			}
            return true;
        }

        /// <summary>
        /// MatchDelay訓練正答をタッチパネルに描画
        /// </summary>
        /// <param name="pointCorrectShapeArgOut">描画位置</param>
        public void OpSubDrawCorrectShapeOnTouchPanel(out ShapeObject shapeObjectTrainShape)
        {
            try
            {
                //Random BgColor用 パラメータ更新
                opImage.SetBgColor();
                opImage.SetDelayBgColor();
                //opImage.MakePointOfCorrectShapeOpeImage();
                //opImage.MakePathOfCorrectShapeOpeImage();

                opImage.MakeCorrectShape();

                opImage.DrawBackColor(); //背景描画後描画すると初回描画されない治る？

                bool ret = opImage.DrawShapeOpeImage(opImage.ShapeOpeImageTrainShape);
                if (ret)
                {
                    //throw new Exception();
                }
            }
            catch (Exception ex)
            {
                Invoke((MethodInvoker)(() =>
                {
                    //画像ファイルが読み取れないExceptionとする
                    MessageBox.Show(ex.Message + "\n 画像ファイルが読み取れませんでした。");
                }));
            }
            finally
            {
                shapeObjectTrainShape = opImage.ShapeOpeImageTrainShape;
            }

        }

        /// <summary>
        /// 訓練時正答をタッチパネルに描画
        /// </summary>
        /// <param name="pointCorrectShapeArgOut">描画位置</param>
        public void OpSubDrawTrainShapeOnTouchPanel(out ShapeObject shapeObjectTrainShape)
        {
            try
            {
                //Random BgColor用 パラメータ更新
                opImage.SetBgColor();
                opImage.SetDelayBgColor();
                opImage.DrawBackColor();
                opImage.MakeCorrectShape();
                //訓練時正答用描画
                _ = opImage.DrawShapeOpeImage(opImage.ShapeOpeImageCorrectShape);
            }
            catch (Exception ex)
            {
                Invoke((MethodInvoker)(() =>
                {
                    //画像ファイルが読み取れないExceptionとする
                    MessageBox.Show(ex.Message + "\n 画像ファイルが読み取れませんでした。");
                }));
            }
            finally
            {
                //訓練時正答返し
                shapeObjectTrainShape = opImage.ShapeOpeImageCorrectShape;
            }

        }

        /// <summary>
        /// ブロック用訓練画像生成
        /// </summary>
        /// <param name="shapeObject"></param>
        public void OpSubDrawTrainShapeOnTouchPanelBlock(out ShapeObject shapeObject)
        {
            try
            {
                //Random BgColor用 パラメータ更新
                opImage.SetBgColor();
                opImage.SetDelayBgColor();

                opImage.MakeTrainShapeBlock();

                opImage.DrawBackColor(); //背景描画後描画すると初回描画されない治る？

                bool ret = opImage.DrawShapeOpeImage(opImage.ShapeOpeImageTrainShape);
                if (ret)
                {
                    //throw new Exception();
                }
            }
            catch (Exception ex)
            {
                Invoke((MethodInvoker)(() =>
                {
                    //画像ファイルが読み取れないExceptionとする
                    MessageBox.Show(ex.Message + "\n 画像ファイルが読み取れませんでした。");
                }));
            }
            finally
            {
                shapeObject = opImage.ShapeOpeImageTrainShape;
            }

        }
        /// <summary>
        /// ブロック用正解画像生成
        /// </summary>
        /// <param name="shapeObject"></param>
        public void OpSubDrawCorrectShapeOnTouchPanelBlock(out ShapeObject shapeObject)
        {
            try
            {
                //Random BgColor用 パラメータ更新
                opImage.SetBgColor();
                opImage.SetDelayBgColor();
                opImage.MakeCorrectShapeBlock();

                opImage.DrawBackColor(); //背景描画後描画すると初回描画されない治る？

                bool ret = opImage.DrawShapeOpeImage(opImage.ShapeOpeImageTrainShape);
                if (ret)
                {
                    //throw new Exception();
                }
            }
            catch (Exception ex)
            {
                Invoke((MethodInvoker)(() =>
                {
                    //画像ファイルが読み取れないExceptionとする
                    MessageBox.Show(ex.Message + "\n 画像ファイルが読み取れませんでした。");
                }));
            }
            finally
            {
                shapeObject = opImage.ShapeOpeImageTrainShape;
            }

        }
        /// <summary>
        /// EpisodeMemory用 正解画像生成
        /// </summary>
        /// <param name="shapeObject"></param>
        /// <param name="shapeObjectIncorrectList"></param>
        public void OpSubDrawCorrectShapeOnTouchPanelBlockEpisodeMemory(out ShapeObject shapeObject)
        {
            try
            {
                if (episodeMemory is null)
                {
                    throw new NullReferenceException();
                }
                if (idControlHelper is null)
                {
                    throw new NullReferenceException();
                }

                // ID有効期間確認
                if (idControlHelper.FindId(opCollection.idCode))
                {
                    idControlHelper.CheckExpire();
                    if (!idControlHelper.FindId(opCollection.idCode))
                    {
                        // なければエントリ削除？
                        episodeMemory.RemoveEntry(opCollection.idCode);
                    }
                    else
                    {
                        idControlHelper.UpdateEntry(opCollection.idCode);
                    }
                }
                else
                {
                    // なければエントリ削除
                    episodeMemory.RemoveEntry(opCollection.idCode);
                }

                // EpisodeMemoryとIdControlHelperの繋ぎ、ここで行っているのを別クラス内にまとめたほうが？
                // エントリがない場合と指定時間内入室の場合は再ジェネレート
                ShapeObject[] shapeObjects;
                if (episodeMemory.ContainsKey(opCollection.idCode) && opCollection.idCode != "")
                {
                    //if (!idControlHelper.CheckElaspedTime(opCollection.idCode, preferencesDatOriginal.EpisodeReEntryTime) && opCollection.trialCount == 0)
                    //{
                    //    shapeObjects = GenerateNewShapeObjects();
                    //    episodeMemory.AddOrUpdateShapObject(opCollection.idCode, shapeObjects);
                    //}
                    //else
                    //{
                    //    shapeObjects = episodeMemory.ReadShapeObject(opCollection.idCode);
                    //}
                    shapeObjects = episodeMemory.ReadShapeObject(opCollection.idCode);
                    opImage.ShapeOpeImageCorrectShape = shapeObjects[0];
                    opImage.ShapeOpeImageTrainShape = shapeObjects[0];

                    // タッチ座標をジェネレート
                    opImage.MakePathOfCorrectShapeOpeImage(opImage.ShapeOpeImageTrainShape.Point);
                    opImage.MakePathOfCorrectShapeOpeImage(opImage.ShapeOpeImageCorrectShape.Point);
                }
                else
                {
                    idControlHelper.AddEntry(opCollection.idCode);
                    //opImage.MakeTrainShapeBlock();
                    //opImage.MakeCorrectShapeBlock();
                    //opImage.MakeIncorrectShape();
                    //shapeObjects = new ShapeObject[opImage.IncorrectShapes.Length + 1];

                    //shapeObjects[0] = opImage?.ShapeOpeImageCorrectShape;

                    //for (int i = 0; i < opImage.IncorrectShapes.Length; i++)
                    //{
                    //    shapeObjects[i + 1] = opImage.IncorrectShapes[i];
                    //}
                    shapeObjects = GenerateNewShapeObjects();

                    if (!episodeMemory.ContainsKey(opCollection.idCode))
                    {
                        episodeMemory.AddOrUpdateShapObject(opCollection.idCode, shapeObjects);
                    }
                    else
                    {
                        episodeMemory.AddOrUpdateShapObject(opCollection.idCode, shapeObjects);
                    }
                }

                //shapeObjects = episodeMemory.ReadShapeObject(opCollection.idCode);

                _ = opImage.DrawShapeOpeImage(opImage?.ShapeOpeImageTrainShape);
                shapeObject = opImage?.ShapeOpeImageTrainShape;

                // 描画後保存
                // 入室時保存にする？インスタンス
                episodeMemory.SaveKeys("epsave.json");
                idControlHelper.SaveId(episodeBaseIdFileName);

            }
            catch (Exception)
            {

                throw;
            }
        }
        /// <summary>
        /// ShapeObject生成
        /// </summary>
        /// <returns></returns>
        private ShapeObject[] GenerateNewShapeObjects()
        {
            opImage.MakeTrainShapeBlock();
            opImage.MakeCorrectShapeBlock();
            opImage.MakeIncorrectShape();
            ShapeObject[] shapeObjects = new ShapeObject[opImage.IncorrectShapes.Length + 1];

            shapeObjects[0] = opImage?.ShapeOpeImageCorrectShape;

            for (int i = 0; i < opImage.IncorrectShapes.Length; i++)
            {
                shapeObjects[i + 1] = opImage.IncorrectShapes[i];
            }
            return shapeObjects;
        }

        public void OpSubDrawTrainShapeOnTouchPanelAfterIllegalExit(ShapeObject shapeObjectTrainShape)
        {
            try
            {
                //Random BgColor用 パラメータ更新
                //opImage.SetBgColor();
                //opImage.SetDelayBgColor();
                //opImage.DrawBackColor();;
                opImage.DrawShapeOpeImage(shapeObjectTrainShape);
            }
            catch (Exception ex)
            {
                Invoke((MethodInvoker)(() =>
                {
                    //画像ファイルが読み取れないExceptionとする
                    MessageBox.Show(ex.Message + "\n 画像ファイルが読み取れませんでした。");
                }));
            }
            finally
            {

            }

        }
        /// <summary>
        /// 正答誤答をタッチパネルに描画
        /// </summary>
        /// <param name="pointWrongShapeArgOut">描画位置</param>
        /// <returns>bool</returns>
        public bool OpSubDrawCorrectAndWrongShapeOnTouchPanel(out Point pointWrongShapeArgOut)
        {
            bool boolRet = true;

            pointWrongShapeArgOut = new Point(0, 0);
            //opImage.ResetWrongShapePoint();
            //for (int i = 0; i < preferencesDatOriginal.IncorrectNum; i++)
            //{
            //    if (opImage.MakePointOfWrongShapeOpeImage() != true)
            //    {
            //        opImage.DeletePathOfCorrectShapeOpeImage();
            //        boolRet = false;
            //        //				TraceMessage2("WrongShape: Draw error");
            //    }
            //}
            opImage.DrawBackColor();
            try
            {

                opImage.MakeIncorrectShape();

            }
            catch (Exception)
            {
                throw;
            }
            if (boolRet == true)
            {
                //opImage.DrawShapeOpeImage(opImage.PointOpeImageCenterOfCorrectShape, false);
                _ = opImage.DrawShapeOpeImage(opImage.ShapeOpeImageCorrectShape);

                foreach (ShapeObject n in opImage.IncorrectShapes)
                {
                    _ = opImage.DrawShapeOpeImage(n);
                    pointWrongShapeArgOut = n.Point;
                }

                //Point point;
                //Color color;
                //ECpShape shape;
                //for (int j = 0; j < opImage.PointOpeImageCenterOfWrongShapeList.Count; j++)
                //            {
                //	point = opImage.PointOpeImageCenterOfWrongShapeList[j];
                //	color = opImage.ColorOpeImageWrongList[j];
                //	shape = opImage.ShapeOpeImageWrongList[j];


                //	opImage.DrawWrongShapeOpeImage(point, false, shape, color);

                //}

                //				{
                //					TraceMessage2(String.Format("WrongShape: Point(X:{0} Y:{1})",
                //									PointOpeImageCenterOfWrongShape.X,
                //									PointOpeImageCenterOfWrongShape.Y));
                //				}
            }
            return boolRet;
        }

        /// <summary>
        /// 正答誤答をタッチパネルに描画
        /// </summary>
        /// <param name="shapeObjectIncorrect">描画ShapeObject</param>
        /// <returns>bool</returns>
        public void OpSubDrawCorrectAndWrongShapeOnTouchPanel(out ShapeObject shapeObjectCorrect, out List<ShapeObject> shapeObjectIncorrectList)
        {
            shapeObjectIncorrectList = new List<ShapeObject>();
            try
            {
                opImage.MakeIncorrectShape();

                _ = opImage.DrawShapeOpeImage(opImage?.ShapeOpeImageCorrectShape);
                shapeObjectCorrect = opImage?.ShapeOpeImageCorrectShape;

                foreach (ShapeObject n in opImage.IncorrectShapes)
                {
                    if (opImage.DrawShapeOpeImage(n))
                    {

                    }
                    else
                    {
                        new Exception("DrawingError");
                    }

                    shapeObjectIncorrectList.Add(n);
                }

            }
            catch (Exception)
            {

                throw;
            }
        }
        public void OpSubDrawCorrectAndWrongShapeOnTouchPanelBlock(out ShapeObject shapeObjectCorrect, out List<ShapeObject> shapeObjectIncorrectList)
        {
            shapeObjectIncorrectList = new List<ShapeObject>();
            try
            {
                opImage.MakeCorrectShapeBlock();
                opImage.MakeIncorrectShape();

                _ = opImage.DrawShapeOpeImage(opImage?.ShapeOpeImageCorrectShape);
                shapeObjectCorrect = opImage?.ShapeOpeImageCorrectShape;

                foreach (ShapeObject n in opImage.IncorrectShapes)
                {
                    if (opImage.DrawShapeOpeImage(n))
                    {

                    }
                    else
                    {
                        new Exception("DrawingError");
                    }

                    shapeObjectIncorrectList.Add(n);
                }

            }
            catch (Exception)
            {

                throw;
            }
        }
        /// <summary>
        /// EpisodeMemory用 正答不正答図形描画
        /// </summary>
        /// <param name="shapeObjectCorrect"></param>
        /// <param name="shapeObjectIncorrectList"></param>
        public void OpSubDrawCorrectAndWrongShapeOnTouchPanelBlockEpisodeMemory(out ShapeObject shapeObjectCorrect, out List<ShapeObject> shapeObjectIncorrectList)
        {
            shapeObjectIncorrectList = new List<ShapeObject>();

            Color color;
            _ = opImage.ConvertStringToColor(preferencesDatOriginal.EpisodeTestBackColor, out color);
            opImage.DrawBackColor(color);

            try
            {
                // Episode memory test
                // インスタンスはメイングローバル側で作成
                //episodeMemory = new EpisodeMemory("epsave.json");
                //idControlHelper = new IdControlHelper(episodeBaseIdFileName);

                // インスタンス作成直後にCheckExpireする エントリが多量にExpireしていた場合時間がかかるため
                if (episodeMemory is null)
                {
                    throw new NullReferenceException();
                }
                if (idControlHelper is null)
                {
                    throw new NullReferenceException();
                }

                ShapeObject[] shapeObjects;

                // EpisodeMemoryとIdControlHelperの繋ぎ、ここで行っているのを別クラス内にまとめたほうが？
                if (episodeMemory.ContainsKey(opCollection.idCode) && opCollection.idCode != "")
                {
                    shapeObjects = episodeMemory.ReadShapeObject(opCollection.idCode);

                    opImage.ShapeOpeImageCorrectShape = shapeObjects[0];
                    opImage.ShapeOpeImageTrainShape = shapeObjects[0];
                    opImage.EpisodeShapes = new ShapeObject[shapeObjects.Length - 1];

                    if (!preferencesDatOriginal.EnableEpisodeIncorrectRandom)
                    {
                        for (int i = 0; i < shapeObjects.Length - 1; i++)
                        {
                            opImage.EpisodeShapes[i] = shapeObjects[i + 1];
                        }
                    }
                    else
                    {
                        opImage.MakeEpisodeShape(shapeObjects[0]);
                        // MakeEpisodeShapeはPreferencesDat.EpisodeTargetNum分の数を生成 座標チェックもれで重なる
                        //opImage.EpisodeShapes[0] = shapeObjects[0];
                        for (int i = 1; i < opImage.EpisodeShapes.Length; i++)
                        {
                            shapeObjects[i] = opImage.EpisodeShapes[i];
                        }
                        episodeMemory.AddOrUpdateShapObject(opCollection.idCode, shapeObjects);
                    }
                    // タッチ座標をジェネレート
                    opImage.MakePathOfCorrectShapeOpeImage(opImage.ShapeOpeImageCorrectShape.Point);
                    opImage.MakePathOfIncorrectShapeOpeImage(shapeObjects);
                }
                else
                {
                    idControlHelper.AddEntry(opCollection.idCode);

                    //opImage.MakeCorrectShapeBlock();
                    opImage.MakeAnyShape();
                    shapeObjects = new ShapeObject[opImage.EpisodeShapes.Length + 1];

                    shapeObjects[0] = opImage?.ShapeOpeImageCorrectShape;

                    for (int i = 0; i < opImage.EpisodeShapes.Length; i++)
                    {
                        shapeObjects[i + 1] = opImage.EpisodeShapes[i];
                    }

                    if (!episodeMemory.ContainsKey(opCollection.idCode))
                    {
                        episodeMemory.AddOrUpdateShapObject(opCollection.idCode, shapeObjects);
                    }
                    else
                    {
                        episodeMemory.AddOrUpdateShapObject(opCollection.idCode, shapeObjects);
                    }
                }

                //shapeObjects = episodeMemory.ReadShapeObject(opCollection.idCode);

                //if (opImage.ShapeOpeImageCorrectShape.ImageFilename != null)
                //{
                //    opImage.ShapeOpeImageCorrectShape.Image = ImageLoader.LoadImage(opImage?.ShapeOpeImageCorrectShape.ImageFilename, new Size(opImage.OpeImageSizeOfShapeInPixel, opImage.OpeImageSizeOfShapeInPixel));
                //}
                _ = opImage.DrawShapeOpeImage(opImage?.ShapeOpeImageCorrectShape);
                shapeObjectCorrect = opImage?.ShapeOpeImageCorrectShape;

                foreach (ShapeObject n in shapeObjects)
                {
                    if (opImage.DrawShapeOpeImage(n))
                    {

                    }
                    else
                    {
                        new Exception("DrawingError");
                    }

                    shapeObjectIncorrectList.Add(n);
                }
                if (preferencesDatOriginal.EnableTestIndicator)
                {
                    opImage.DrawMarker(preferencesDatOriginal.IndicatorPosition);
                }
                TraceMessage("エピソード記憶 回答表示");
                // 描画後保存
                // 入室時保存にする？インスタンス
                episodeMemory.SaveKeys("epsave.json");
                idControlHelper.SaveId(episodeBaseIdFileName);

            }
            catch (Exception)
            {

                throw;
            }
        }

        public void OpSubDrawAnyShapeOnTouchPanelBlockEpisodeMemory(out ShapeObject shapeObjectCorrect, out List<ShapeObject> shapeObjectCorrectList)
        {
            shapeObjectCorrectList = new List<ShapeObject>();

            Color color;
            _ = opImage.ConvertStringToColor(preferencesDatOriginal.EpisodeFirstBackColor, out color);
            opImage.DrawBackColor(color);

            try
            {
                // インスタンス作成直後にCheckExpireする エントリが多量にExpireしていた場合時間がかかるため
                if (episodeMemory is null)
                {
                    throw new NullReferenceException();
                }
                if (idControlHelper is null)
                {
                    throw new NullReferenceException();
                }

                // EpisodeMemoryとIdControlHelperの繋ぎ、ここで行っているのを別クラス内にまとめたほうが？
                if (episodeMemory.ContainsKey(opCollection.idCode) && opCollection.idCode != "")
                {
                    opImage.TouchAnyShapes = episodeMemory.ReadShapeObject(opCollection.idCode);

                    // ShapeObjectGraphicsPathがnullかどうか確かめる
                    foreach (ShapeObject touchSo in opImage.TouchAnyShapes)
                    {
                        if (touchSo.ShapeObjectGraphicsPath is null)
                        {
                            touchSo.ShapeObjectGraphicsPath = opImage.MakePath(touchSo.Point);
                        }
                    }

                    opImage.ShapeOpeImageCorrectShape = opImage.TouchAnyShapes[0];
                    opImage.ShapeOpeImageTrainShape = opImage.TouchAnyShapes[0];
                    opImage.EpisodeShapes = new ShapeObject[opImage.TouchAnyShapes.Length - 1];

                    if (!preferencesDatOriginal.EnableEpisodeIncorrectRandom && CheckShapes(opImage.TouchAnyShapes))
                    {


                        for (int i = 0; i < opImage.TouchAnyShapes.Length - 1; i++)
                        {
                            opImage.EpisodeShapes[i] = opImage.TouchAnyShapes[i + 1];
                        }
                    }
                    else
                    {
                        opImage.MakeAnyShape();
                        episodeMemory.AddOrUpdateShapObject(opCollection.idCode, opImage.TouchAnyShapes);
                    }
                }
                else
                {
                    idControlHelper.AddEntry(opCollection.idCode);

                    opImage.MakeAnyShape();

                    if (!episodeMemory.ContainsKey(opCollection.idCode))
                    {
                        episodeMemory.AddOrUpdateShapObject(opCollection.idCode, opImage.TouchAnyShapes);
                    }
                    else
                    {
                        episodeMemory.AddOrUpdateShapObject(opCollection.idCode, opImage.TouchAnyShapes);
                    }
                }

                shapeObjectCorrect = opImage?.TouchAnyShapes[0];

                foreach (ShapeObject n in opImage.TouchAnyShapes)
                {
                    if (opImage.DrawShapeOpeImage(n))
                    {

                    }
                    else
                    {
                        new Exception("DrawingError");
                    }

                    shapeObjectCorrectList.Add(n);
                }
                TraceMessage("エピソード記憶 課題表示");
                // 描画後保存
                // 回答後保存にする タイムアウトは前回の回答からの時間とする
                episodeMemory.SaveKeys("epsave.json");
                idControlHelper.SaveId(episodeBaseIdFileName);

            }
            catch (Exception)
            {

                throw;
            }
        }

        public bool CheckShapes(ShapeObject[] so)
        {
            bool ret = false;
            foreach (ShapeObject n in so)
            {
                ret = OpeImage.CheckExistSvgShape(n.Shape);
                if (!ret)
                {
                    return ret;
                }

            }
            return ret;
        }

        public ECpTask OpSubGetTypeOfTask()
        {
            if (opImage.ConvertStringToCpTask(preferencesDatOriginal.OpeTypeOfTask, out ECpTask cpTaskValue) != true)
            {
                cpTaskValue = ECpTask.Training;
            }
            return cpTaskValue;
        }

        public void OpSubTouchClear()
        {
            formSub.pictureBoxOnFormSub.TouchClear();
        }

        public void OpSubCheckIdExpire()
        {
            if (idControlHelper is null)
            {
                idControlHelper = new IdControlHelper();
            }
            // ID有効期間確認
            if (idControlHelper.FindId(opCollection.idCode))
            {
                idControlHelper.CheckExpire();
                if (!idControlHelper.FindId(opCollection.idCode))
                {
                    // なければエントリ削除？
                    episodeMemory.RemoveEntry(opCollection.idCode);
                }
                else
                {
                    //入室時更新に統合
                    //idControlHelper.UpdateEntry(opCollection.idCode);
                }
            }
            else
            {
                // なければエントリ削除
                episodeMemory.RemoveEntry(opCollection.idCode);
            }
            EpisodeMode.Value = !idControlHelper.FindId(opCollection.idCode) || idControlHelper.GetCount(opCollection.idCode) == 0;
        }

        /// <summary>
        /// エピソード記憶実行有効
        /// </summary>
        public SyncObject<bool> EpisodeActive = new SyncObject<bool>(false);
        public SyncObject<DateTime> EpisodeTime = new SyncObject<DateTime>();

        /// <summary>
        /// エピソード記憶有効判別
        /// ・エピソード記憶設定有効
        /// ・タイムゾーン内
        /// ・カウント未達
        /// </summary>
        public void CheckEpisodeCount()
        {
            if (opCollection.idCode == "")
            {
                EpisodeActive.Value = false;
                return;
            }
            if (preferencesDatOriginal.ForceEpisodeMemory)
            {
                EpisodeActive.Value = true;
                return;
            }

            if (!idControlHelper.FindId(opCollection.idCode))
            {
                if (CheckSelectShapeTimezone())
                {
                    EpisodeActive.Value = true;
                }
                else
                {
                    EpisodeActive.Value = false;
                }
                return;
            }

            var count = idControlHelper.GetCount(opCollection.idCode);
            if (count == 0)
            {
                if (CheckSelectShapeTimezone())
                {
                    // エピソード記憶実行有効
                    EpisodeActive.Value = true;
                    EpisodeTime.Value = DateTime.Now;
                    return;
                }
                else
                {
                    EpisodeActive.Value = false;
                    return;
                }
            }
            if (preferencesDatOriginal.EpisodeCount > count)
            {
                if (CheckTimezone())
                {
                    if (CheckEpInterval(EpisodeTime.Value))
                    {
                        // エピソード記憶実行有効
                        EpisodeActive.Value = true;
                        EpisodeTime.Value = DateTime.Now;
                    }
                    else
                    {
                        EpisodeActive.Value = false;
                    }
                }
                else
                {
                    EpisodeActive.Value = false;

                    //タイムゾーン超えたらリセット
                    idControlHelper.ResetCount(opCollection.idCode);
                    count = idControlHelper.GetCount(opCollection.idCode);
                }
            }
            // 回数到達時処理
            if (preferencesDatOriginal.EpisodeCount == count)
            {
                EpisodeActive.Value = false;

                if (!CheckTimezone())
                {
                    //タイムゾーン超えたらリセット
                    idControlHelper.ResetCount(opCollection.idCode);
                }
                else
                {
                    //Timezone中は何もしない
                }
            }

        }
        public bool CheckTimezone()
        {
            return CheckTimezoneInternal((int)preferencesDatOriginal.EpisodeTimezoneStartTime,
                                    (int)preferencesDatOriginal.EpisodeTimezoneEndTime);
        }
        public bool CheckSelectShapeTimezone()
        {
            return CheckTimezoneInternal((int)preferencesDatOriginal.EpisodeSelectShapeTimezoneStartTime,
                                    (int)preferencesDatOriginal.EpisodeSelectShapeTimezoneEndTime);
        }

        private bool CheckTimezoneInternal(int startTime , int endTime)
        {
            TimeSpan timeOfDay = DateTime.Now.TimeOfDay;
            TimeSpan startTimeTick = new TimeSpan(hours: startTime, minutes: 0, seconds: 0);
            TimeSpan endTimeTick = new TimeSpan(hours: endTime, minutes: 0, seconds: 0);

            if (startTimeTick <= timeOfDay && timeOfDay <= endTimeTick)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool CheckEpInterval(DateTime time)
        {
            TimeSpan keikaTime = DateTime.Now - time;
            return keikaTime > new TimeSpan(0, preferencesDatOriginal.EpisodeIntervalTime, 0);
        }
    }
}
