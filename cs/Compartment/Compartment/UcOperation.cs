//#define NO_TRIGGER_TOUCH

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Compartment
{
    class UcOperation
    {
    }

    public partial class FormMain : Form
    {
        public SyncObject<bool> IsOperationBackButtonBusy = new SyncObject<bool>(false);


        private void InitializeComponentOnUcOperation()
        {
            #region コレクションのイベント
            opCollection.callbackMessageNormal = (str) => { WriteLineStatus("[メッセージ]," + str); };

            opCollection.callbackMessageDebug = (str) => { WriteLineStatus("[デバッグ]," + str); };

            opCollection.callbackMessageError = (str) => { WriteLineStatus("[エラー]," + str); };

            opCollection.callbackSetUiCurentNumberOfTrial = (value) =>
            {
                Invoke((MethodInvoker)(() =>
                {
                    if (preferencesDatOriginal.OpeNumberOfTrial == 0)
                    {
                        userControlOperationOnFormMain.textBoxCurentNumberOfTrial.Text = value.ToString() + "/" + "∞";
                    }
                    else
                    {
                        userControlOperationOnFormMain.textBoxCurentNumberOfTrial.Text = value.ToString() + "/" + preferencesDatOriginal.OpeNumberOfTrial.ToString();
                    }
                }));
            };

            opCollection.callbackSetUiCurrentIdCode = (str) =>
            {
                Invoke((MethodInvoker)(() =>
                {
                    userControlOperationOnFormMain.textBoxCurrentIdCode.Text = str;
                }));
            };

            // ステートに設定された際のイベント
            opCollection.sequencer.setStateEvent = (state) => { WriteLineStatus("[setState]," + state.GetEnumDisplayName()); };


            logFile.Open(preferencesDatOriginal.OutputLogFile);
            // logFile.Open(Application.LocalUserAppDataPath, false);
            this.FormClosing += (sender, e) => 
            {
                if(e.Cancel == false)
                {
                    logFile.Close();
                }
            };
            #endregion

            #region 出力ファイル
            // 出力ファイルのヘッダ
            opCollection.file.callbackOpenNewFile = () =>
            {
                string line = "";
                line += "コンパートメント番号";
                line += ",IDコード";
                line += ",結果";
                line += ",入室時間";
                line += ",トリガ・タッチ時刻タイムスタンプ";
                line += ",正解時刻タイムスタンプ";
                line += ",タイムアウト時刻タイムスタンプ";
                line += ",現在の試行回数";
                line += ",タッチ座標X";
                line += ",タッチ座標Y";
                line += ",訓練画像表示座標X";
                line += ",訓練画像表示座標Y";
                line += ",訓練画像形状";
                line += ",訓練画像色";
                line += ",正解画像表示座標X";
                line += ",正解画像表示座標Y";
                line += ",正解画像形状";
                line += ",正解画像色";
                line += ",正解画像File";
                line += ",不正解画像表示座標X";
                line += ",不正解画像表示座標Y";
                line += ",不正解画像形状";
                line += ",不正解画像色";
                line += ",不正解画像表示座標X2";
                line += ",不正解画像表示座標Y2";
                line += ",不正解画像形状2";
                line += ",不正解画像色2";
                line += ",不正解画像表示座標X3";
                line += ",不正解画像表示座標Y3";
                line += ",不正解画像形状3";
                line += ",不正解画像色3";
                line += ",不正解画像表示座標X4";
                line += ",不正解画像表示座標Y4";
                line += ",不正解画像形状4";
                line += ",不正解画像色4";
                line += ",給餌したか否か";
                line += ",総インターバル時間";
                line += ",インターバル時間実行回数";
                line += ",ランダム待機時間";
                #region 設定に関するコンテンツ
                line += ",[設定]TypeOfTask(課題タイプ)";
                line += ",[設定]CorrectCondition(正解条件)";
                line += ",[設定]NumberOfTrial(試行回数)";
                line += ",[設定]TimeToDisplayCorrectImage(正解画像表示時間)";
                line += ",[設定]TimeToDisplayNoImage(画像消去時間)";
                line += ",[設定]IntevalTimeMinimum(インターバル時間最小値)";
                line += ",[設定]IntevalTimeMaximum(インターバル時間最大値)";
                line += ",[設定]FeedingRate(給餌比率)";
                line += ",[設定]TimeToFeed(給餌時間)";
                line += ",[設定]TimeoutOfStart(開始タイムアウト時間)";
                line += ",[設定]TimeoutOfTrial(課題タイムアウト時間)";
                line += ",[設定]TriggerImage(トリガ画像)";
                line += ",[設定]BackColor(背景色)";
                line += ",[設定]DelayBackColor(ディレイ時背景色)";
                //line += ",[設定]ShapeColor(図形色)";
                //line += ",[設定]TypeOfShape(図形タイプ)";
                line += ",[設定]SizeOfShapeInStep(段階指定の 図形サイズ)";
                line += ",[設定]SizeOfShapeInPixelForStep1(Step1の図形サイズ)";
                line += ",[設定]SizeOfShapeInPixelForStep2(Step2の図形サイズ)";
                line += ",[設定]SizeOfShapeInPixelForStep3(Step3の図形サイズ)";
                line += ",[設定]SizeOfShapeInPixelForStep4(Step4の図形サイズ)";
                line += ",[設定]SizeOfShapeInPixelForStep5(Step5の図形サイズ)";
                line += ",[設定]SizeOfShapeInPixel(ピクセル指定の図形サイズ)";
                line += ",[設定]SoundFileOfEnd(終了音ファイル)";
                line += ",[設定]TimeToOutputSoundOfEnd(終了音出力時間)";
                line += ",[設定]SoundFileOfReward(報酬音ファイル)";
                line += ",[設定]TimeToOutputSoundOfReward(報酬音出力時間)";
                line += ",[設定]SoundFileOfCorrect(正答音ファイル)";
                line += ",[設定]TimeToOutputSoundOfCorrect(正答音出力時間)";
                line += ",[Block用]フィード時間";
                line += ",[Block用]フィードサウンド時間";
                line += ",[Block用]フィードランプ時間";
                #endregion 設定に関するコンテンツ
                line += ",[Detect]検出個体数";

                line += ",[MultiID]実行ファイル名";

                opCollection.file.WriteLineAsync(line);
            };

            // 出力ファイルのコンテンツ
            opCollection.sequencer.callbackOutputFile = (state) =>
            {
                string timeTriggerTouch;
                string timeCorrectTouch;
                string timeTimeout;
                string timeEnterCage;
                string trialCount;
                //String correctShapeX;
                //String correctShapeY;
                //String wrongShapeX;
                //String wrongShapeY;
                ShapeObject trainingShape = new ShapeObject();
                ShapeObject correctShape = new ShapeObject();
                string correctImageFile = "";
                ShapeObject incorrectShape1 = new ShapeObject();
                ShapeObject incorrectShape2 = new ShapeObject();
                ShapeObject incorrectShape3 = new ShapeObject();
                ShapeObject incorrectShape4 = new ShapeObject();
                string flagFeed;
                string intervalTimeTotal;
                string intervalNum;
                string dateTimeDefaultValue = "0000/00/00 00:00:00";
                string beforeDelayMatchRandomTime;

                timeEnterCage = opCollection.TimeEnterCage.ToString("yyyy/MM/dd HH:mm:ss");

                switch (state)
                {
                    case OpCollection.Sequencer.EState.WaitingForTouchTriggerScreen:
                        {
                            // トリガ画面タッチ待ちタイムアウト
                            timeTriggerTouch = dateTimeDefaultValue; // トリガ・タッチ時刻タイムスタンプ
                            timeCorrectTouch = dateTimeDefaultValue; // 正解時刻タイムスタンプ
                            timeTimeout = opCollection.dateTimeout.ToString("yyyy/MM/dd HH:mm:ss"); // タイムアウト時刻タイムスタンプ
                            trialCount = (opCollection.trialCount + 1).ToString(); // 現在の試行回数(インクリメントしていない為、+1として使用)
                            //correctShapeX = "0"; // 正解画像表示座標X
                            //correctShapeY = "0"; // 正解画像表示座標Y
                            //wrongShapeX = "0"; // 不正解画像表示座標X
                            //wrongShapeY = "0"; // 不正解画像表示座標Y
                            flagFeed = "OFF"; // 給餌したか否か
                            intervalTimeTotal = "0"; // 総インターバル時間
                            intervalNum = "0"; // インターバル時間実行回数
                            beforeDelayMatchRandomTime = "0"; //ランダム待機時間
                            break;
                        }
                    case OpCollection.Sequencer.EState.Training:
                        {
                            // 訓練タイムアウト
                            timeTriggerTouch = opCollection.dateTimeTriggerTouch.ToString("yyyy/MM/dd HH:mm:ss"); // トリガ・タッチ時刻タイムスタンプ
                            timeCorrectTouch = dateTimeDefaultValue; // 正解時刻タイムスタンプ
                            timeTimeout = opCollection.dateTimeout.ToString("yyyy/MM/dd HH:mm:ss"); // タイムアウト時刻タイムスタンプ
                            trialCount = (opCollection.trialCount + 1).ToString(); // 現在の試行回数(インクリメントしていない為、+1として使用)
                            trainingShape = opCollection.trainingShape;
                            //correctShapeX = "0"; // 正解画像表示座標X
                            //correctShapeY = "0"; // 正解画像表示座標Y
                            //wrongShapeX = "0"; // 不正解画像表示座標X
                            //wrongShapeY = "0"; // 不正解画像表示座標Y
                            flagFeed = "OFF"; // 給餌したか否か
                            intervalTimeTotal = "0"; // 総インターバル時間
                            intervalNum = "0"; // インターバル時間実行回数
                            beforeDelayMatchRandomTime = "0"; //ランダム待機時間
                            break;
                        }
                    case OpCollection.Sequencer.EState.WaitingForFeedComplete:
                        {
                            // 設定試行回数により給餌完了(正常)
                            timeTriggerTouch = opCollection.dateTimeTriggerTouch.ToString("yyyy/MM/dd HH:mm:ss"); // トリガ・タッチ時刻タイムスタンプ
                            timeCorrectTouch = opCollection.dateTimeCorrectTouch.ToString("yyyy/MM/dd HH:mm:ss"); // 正解時刻タイムスタンプ
                            timeTimeout = dateTimeDefaultValue; // タイムアウト時刻タイムスタンプ
                            trialCount = opCollection.trialCount.ToString(); // 現在の試行回数(既にインクリメントしてある為、そのままの値を使用)
                            trainingShape = opCollection.trainingShape;
                            correctShape = opCollection.correctShape;
                            correctImageFile = opCollection.correctImageFile; //ImageFile時正答ファイル名
                            //correctShapeX = opCollection.correctShape.Point.X.ToString(); // 正解画像表示座標X
                            //correctShapeY = opCollection.correctShape.Point.Y.ToString(); // 正解画像表示座標Y
                            beforeDelayMatchRandomTime = opCollection.beforeDelayMatchRandomTime.ToString(); //ランダム待機時間
                            if (OpeGetTypeOfTask() == ECpTask.DelayMatch)
                            {
                                if (opCollection?.incorrectShapeList?.Count > 0)
                                {
                                    incorrectShape1 = opCollection?.incorrectShapeList?.ElementAt(0);
                                }

                                if (opCollection?.incorrectShapeList?.Count > 1)
                                {
                                    incorrectShape2 = opCollection?.incorrectShapeList?.ElementAt(1);
                                }

                                if (opCollection?.incorrectShapeList?.Count > 2)
                                {
                                    incorrectShape3 = opCollection?.incorrectShapeList?.ElementAt(2);
                                }

                                if (opCollection?.incorrectShapeList?.Count > 3)
                                {
                                    incorrectShape4 = opCollection?.incorrectShapeList?.ElementAt(3);
                                }

                                //wrongShapeX = opCollection.incorrectShapeList.ElementAt(0).Point.X.ToString(); // 不正解画像表示座標X
                                //wrongShapeY = opCollection.incorrectShapeList.ElementAt(0).Point.Y.ToString(); // 不正解画像表示座標Y
                            }
                            else
                            {
                                //wrongShapeX = "0"; // 不正解画像表示座標X
                                //wrongShapeY = "0"; // 不正解画像表示座標Y
                            }
                            flagFeed = (opCollection.flagFeed == true ? "ON" : "OFF"); // 給餌したか否か
                            intervalTimeTotal = "0"; // 総インターバル時間
                            intervalNum = "0"; // インターバル時間実行回数
                            break;
                        }
                    case OpCollection.Sequencer.EState.IntervalProc:
                        {
                            // インターバル処理中画面非タッチにより試行継続(正常)
                            timeTriggerTouch = opCollection.dateTimeTriggerTouch.ToString("yyyy/MM/dd HH:mm:ss"); // トリガ・タッチ時刻タイムスタンプ
                            timeCorrectTouch = opCollection.dateTimeCorrectTouch.ToString("yyyy/MM/dd HH:mm:ss"); // 正解時刻タイムスタンプ
                            timeTimeout = dateTimeDefaultValue; // タイムアウト時刻タイムスタンプ
                            trialCount = opCollection.trialCount.ToString(); // 現在の試行回数(既にインクリメントしてある為、そのままの値を使用)
                            trainingShape = opCollection.trainingShape;
                            correctShape = opCollection.correctShape;
                            correctImageFile = opCollection.correctImageFile; //ImageFile時正答ファイル名
                            //correctShapeX = opCollection.pointCorrectShape.X.ToString(); // 正解画像表示座標X
                            //correctShapeY = opCollection.pointCorrectShape.Y.ToString(); // 正解画像表示座標Y

                            if (OpeGetTypeOfTask() == ECpTask.DelayMatch)
                            {
                                if (opCollection?.incorrectShapeList?.Count > 0)
                                {
                                    incorrectShape1 = opCollection?.incorrectShapeList?.ElementAt(0);
                                }

                                if (opCollection?.incorrectShapeList?.Count > 1)
                                {
                                    incorrectShape2 = opCollection?.incorrectShapeList?.ElementAt(1);
                                }

                                if (opCollection?.incorrectShapeList?.Count > 2)
                                {
                                    incorrectShape3 = opCollection?.incorrectShapeList?.ElementAt(2);
                                }

                                if (opCollection?.incorrectShapeList?.Count > 3)
                                {
                                    incorrectShape4 = opCollection?.incorrectShapeList?.ElementAt(3);
                                }

                                //wrongShapeX = opCollection.pointWrongShape.X.ToString(); // 不正解画像表示座標X
                                //wrongShapeY = opCollection.pointWrongShape.Y.ToString(); // 不正解画像表示座標Y
                            }
                            else
                            {
                                //wrongShapeX = "0"; // 不正解画像表示座標X
                                //wrongShapeY = "0"; // 不正解画像表示座標Y
                            }
                            flagFeed = opCollection.flagFeed == true ? "ON" : "OFF"; // 給餌したか否か
                            intervalTimeTotal = opCollection.intervalTimeTotal.ToString(); // 総インターバル時間
                            intervalNum = opCollection.intervalNum.ToString(); // インターバル時間実行回数
                            beforeDelayMatchRandomTime = opCollection.beforeDelayMatchRandomTime.ToString(); //ランダム待機時間
                            break;
                        }
                    case OpCollection.Sequencer.EState.IllegalExitDetection:
                        timeTriggerTouch = opCollection.dateTimeTriggerTouch.ToString("yyyy/MM/dd HH:mm:ss"); // トリガ・タッチ時刻タイムスタンプ
                        timeCorrectTouch = dateTimeDefaultValue; // 正解時刻タイムスタンプ
                        timeTimeout = dateTimeDefaultValue; // タイムアウト時刻タイムスタンプ
                        trialCount = (opCollection.trialCount + 1).ToString(); //次の回で途中退出したので
                        //タッチポイントリセット
                        opCollection.TouchPoint.X = 0;
                        opCollection.TouchPoint.Y = 0;
                        //イリーガルExit確定なので結果固定？
                        opCollection.taskResultVal = OpCollection.ETaskResult.IllegalExit;
                        //内容不正かも
                        //trainingShape = opCollection.trainingShape;
                        //correctShape = opCollection.correctShape;
                        beforeDelayMatchRandomTime = opCollection.beforeDelayMatchRandomTime.ToString(); //ランダム待機時間
                        flagFeed = (opCollection.flagFeed == true ? "ON" : "OFF"); // 給餌したか否か
                        intervalTimeTotal = "0"; // 総インターバル時間
                        intervalNum = "0"; // インターバル時間実行回数
                        opCollection.DetectCount = CamImage.DetectNum;  // 検出個体数
                        if (OpeGetTypeOfTask() == ECpTask.DelayMatch)
                        {
                            if (opCollection?.incorrectShapeList?.Count > 0)
                            {
                                incorrectShape1 = opCollection?.incorrectShapeList?.ElementAt(0);
                            }

                            if (opCollection?.incorrectShapeList?.Count > 1)
                            {
                                incorrectShape2 = opCollection?.incorrectShapeList?.ElementAt(1);
                            }

                            if (opCollection?.incorrectShapeList?.Count > 2)
                            {
                                incorrectShape3 = opCollection?.incorrectShapeList?.ElementAt(2);
                            }

                            if (opCollection?.incorrectShapeList?.Count > 3)
                            {
                                incorrectShape4 = opCollection?.incorrectShapeList?.ElementAt(3);
                            }

                            //wrongShapeX = opCollection.incorrectShapeList.ElementAt(0).Point.X.ToString(); // 不正解画像表示座標X
                            //wrongShapeY = opCollection.incorrectShapeList.ElementAt(0).Point.Y.ToString(); // 不正解画像表示座標Y
                        }

                        break;
                    case OpCollection.Sequencer.EState.BlockOutput:
                        // ブロック出力用 基本全項目記述
                        timeTriggerTouch = opCollection.dateTimeTriggerTouch.ToString("yyyy/MM/dd HH:mm:ss"); // トリガ・タッチ時刻タイムスタンプ
                        timeCorrectTouch = opCollection.dateTimeCorrectTouch.ToString("yyyy/MM/dd HH:mm:ss"); // 正解時刻タイムスタンプ
                        timeTimeout = dateTimeDefaultValue; // タイムアウト時刻タイムスタンプ
                        trialCount = opCollection.trialCount.ToString(); // 現在の試行回数(既にインクリメントしてある為、そのままの値を使用)
                        trainingShape = opCollection.trainingShape;
                        correctShape = opCollection.correctShape;
                        correctImageFile = opCollection.correctImageFile; //ImageFile時正答ファイル名
                        if (opCollection.taskResultVal == OpCollection.ETaskResult.OkEpisode || opCollection.taskResultVal == OpCollection.ETaskResult.OkEpisodeIssue)
                        {
                            var shape = new ShapeObject();
                            shape.Shape = ECpShape.None;
                            trainingShape = shape;
                            opCollection.trainingShape = shape;
                        }
                        if (opCollection?.incorrectShapeList?.Count > 0)
                        {
                            incorrectShape1 = opCollection?.incorrectShapeList?.ElementAt(0);
                        }

                        if (opCollection?.incorrectShapeList?.Count > 1)
                        {
                            incorrectShape2 = opCollection?.incorrectShapeList?.ElementAt(1);
                        }

                        if (opCollection?.incorrectShapeList?.Count > 2)
                        {
                            incorrectShape3 = opCollection?.incorrectShapeList?.ElementAt(2);
                        }

                        if (opCollection?.incorrectShapeList?.Count > 3)
                        {
                            incorrectShape4 = opCollection?.incorrectShapeList?.ElementAt(3);
                        }
                        // incorrectCount = opCollection.IncorrectCount.ToString();
                        flagFeed = opCollection.flagFeed == true ? "ON" : "OFF"; // 給餌したか否か
                        intervalTimeTotal = opCollection.intervalTimeTotal.ToString(); // 総インターバル時間
                        intervalNum = opCollection.intervalNum.ToString(); // インターバル時間実行回数
                        beforeDelayMatchRandomTime = opCollection.beforeDelayMatchRandomTime.ToString(); //ランダム待機時間

                        opCollection.DetectCount = CamImage.DetectNum;  // 検出個体数

                        break;
                    case OpCollection.Sequencer.EState.ExitAfterFeedingDetection:
                        // 給餌直後 退室出力用 基本全項目記述
                        timeTriggerTouch = opCollection.dateTimeTriggerTouch.ToString("yyyy/MM/dd HH:mm:ss"); // トリガ・タッチ時刻タイムスタンプ
                        timeCorrectTouch = opCollection.dateTimeCorrectTouch.ToString("yyyy/MM/dd HH:mm:ss"); // 正解時刻タイムスタンプ
                        timeTimeout = dateTimeDefaultValue; // タイムアウト時刻タイムスタンプ
                        trialCount = opCollection.trialCount.ToString(); // 現在の試行回数(既にインクリメントしてある為、そのままの値を使用)
                        trainingShape = opCollection.trainingShape;
                        correctShape = opCollection.correctShape;
                        correctImageFile = opCollection.correctImageFile; //ImageFile時正答ファイル名
                                                                          //opCollection.taskResultVal = OpCollection.ETaskResult.FeedEndExit;

                        if (opCollection?.incorrectShapeList?.Count > 0)
                        {
                            incorrectShape1 = opCollection?.incorrectShapeList?.ElementAt(0);
                        }

                        if (opCollection?.incorrectShapeList?.Count > 1)
                        {
                            incorrectShape2 = opCollection?.incorrectShapeList?.ElementAt(1);
                        }

                        if (opCollection?.incorrectShapeList?.Count > 2)
                        {
                            incorrectShape3 = opCollection?.incorrectShapeList?.ElementAt(2);
                        }

                        if (opCollection?.incorrectShapeList?.Count > 3)
                        {
                            incorrectShape4 = opCollection?.incorrectShapeList?.ElementAt(3);
                        }
                        // incorrectCount = opCollection.IncorrectCount.ToString();
                        flagFeed = opCollection.flagFeed == true ? "ON" : "OFF"; // 給餌したか否か
                        intervalTimeTotal = opCollection.intervalTimeTotal.ToString(); // 総インターバル時間
                        intervalNum = opCollection.intervalNum.ToString(); // インターバル時間実行回数
                        beforeDelayMatchRandomTime = opCollection.beforeDelayMatchRandomTime.ToString(); //ランダム待機時間

                        opCollection.DetectCount = CamImage.DetectNum;  // 検出個体数

                        break;
                    default:
                        {
                            // ここに来る予定はない
                            return;
                        }
                }
                string line = "";
                line += preferencesDatOriginal.CompartmentNo.ToString(); // コンパートメント番号
                line += "," + opCollection.idCode; // IDコード
                line += "," + opCollection.taskResultVal.ToString(); // 結果
                line += "," + timeEnterCage;    //入室時間
                line += "," + timeTriggerTouch; // トリガ・タッチ時刻タイムスタンプ
                line += "," + timeCorrectTouch; // 正解時刻タイムスタンプ
                line += "," + timeTimeout; // タイムアウト時刻タイムスタンプ
                line += "," + trialCount; // 現在の試行回数
                line += "," + opCollection.TouchPoint.X.ToString(); // タッチ座標X
                line += "," + opCollection.TouchPoint.Y.ToString(); // タッチ座標Y
                line += "," + trainingShape.Point.X.ToString();
                line += "," + trainingShape.Point.Y.ToString();
                line += "," + trainingShape.Shape.ToString();
                line += "," + trainingShape.ShapeColor.ToString();
                line += "," + correctShape.Point.X.ToString();
                line += "," + correctShape.Point.Y.ToString();
                line += "," + correctShape.Shape.ToString();
                if (!preferencesDatOriginal.EnableImageShape) line += "," + correctShape.ShapeColor.ToString();
                else line += "," + "";
                line += "," + correctImageFile;
                //line += "," + correctShapeX; // 正解画像表示座標X
                //line += "," + correctShapeY; // 正解画像表示座標Y
                line += "," + incorrectShape1.Point.X.ToString();   // 不正解画像表示座標X
                line += "," + incorrectShape1.Point.Y.ToString();   // 不正解画像表示座標Y
                line += "," + incorrectShape1.Shape.ToString();     // 不正解画像形状
                if (!preferencesDatOriginal.EnableImageShape) line += "," + incorrectShape1.ShapeColor.ToString();    // 不正解画像色
                else line += "," + "";
                line += "," + incorrectShape2.Point.X.ToString();   // 不正解画像表示座標X
                line += "," + incorrectShape2.Point.Y.ToString();   // 不正解画像表示座標Y
                line += "," + incorrectShape2.Shape.ToString();     // 不正解画像形状
                if (!preferencesDatOriginal.EnableImageShape) line += "," + incorrectShape2.ShapeColor.ToString();    // 不正解画像色
                else line += "," + "";
                line += "," + incorrectShape3.Point.X.ToString();   // 不正解画像表示座標X
                line += "," + incorrectShape3.Point.Y.ToString();   // 不正解画像表示座標Y
                line += "," + incorrectShape3.Shape.ToString();     // 不正解画像形状
                if (!preferencesDatOriginal.EnableImageShape) line += "," + incorrectShape3.ShapeColor.ToString();    // 不正解画像色
                else line += "," + "";
                line += "," + incorrectShape4.Point.X.ToString();   // 不正解画像表示座標X
                line += "," + incorrectShape4.Point.Y.ToString();   // 不正解画像表示座標Y
                line += "," + incorrectShape4.Shape.ToString();     // 不正解画像形状
                if (!preferencesDatOriginal.EnableImageShape) line += "," + incorrectShape4.ShapeColor.ToString();    // 不正解画像色
                else line += "," + "";
                line += "," + flagFeed; // 給餌したか否か
                line += "," + intervalTimeTotal; // 総インターバル時間
                line += "," + intervalNum; // インターバル時間実行回数
                line += "," + beforeDelayMatchRandomTime; // ランダムディレイ時間
                #region 設定に関するコンテンツ
                line += "," + preferencesDatOriginal.OpeTypeOfTask; // ",[設定]TypeOfTask(課題タイプ)
                line += "," + preferencesDatOriginal.CorrectCondition.ToString(); // [設定]CorrectCondition(正解条件)
                line += "," + preferencesDatOriginal.OpeNumberOfTrial.ToString(); // [設定]NumberOfTrial(試行回数)
                line += "," + preferencesDatOriginal.OpeTimeToDisplayCorrectImage.ToString(); // [設定]TimeToDisplayCorrectImage(正解画像表示時間)
                line += "," + preferencesDatOriginal.OpeTimeToDisplayNoImage.ToString(); // [設定]TimeToDisplayNoImage(画像消去時間)
                line += "," + preferencesDatOriginal.OpeIntervalTimeMinimum.ToString(); // [設定]IntevalTimeMinimum(インターバル時間最小値)
                line += "," + preferencesDatOriginal.OpeIntervalTimeMaximum.ToString(); // [設定]IntevalTimeMaximum(インターバル時間最大値)
                line += "," + preferencesDatOriginal.OpeFeedingRate.ToString(); // [設定]FeedingRate(給餌比率)
                line += "," + preferencesDatOriginal.OpeTimeToFeed.ToString(); // [設定]TimeToFeed(給餌時間)  ->実際の給餌時間
                line += "," + preferencesDatOriginal.OpeTimeoutOfStart.ToString(); // [設定]TimeoutOfStart(開始タイムアウト時間)
                line += "," + preferencesDatOriginal.OpeTimeoutOfTrial.ToString(); // [設定]TimeoutOfTrial(課題タイムアウト時間)
                line += "," + preferencesDatOriginal.TriggerImageFile; // [設定]TriggerImage(トリガ画像)
                line += "," + preferencesDatOriginal.BackColor; // [設定]BackColor(背景色)
                line += "," + preferencesDatOriginal.DelayBackColor; // [設定]DelayBackColor(ディレイ時背景色)
                //line += "," + preferencesDatOriginal.ShapeColor; // [設定]ShapeColor(図形色)
                //line += "," + preferencesDatOriginal.TypeOfShape; // [設定]TypeOfShape(図形タイプ)
                line += "," + preferencesDatOriginal.SizeOfShapeInStep; // [設定]SizeOfShapeInStep(段階指定の 図形サイズ)
                line += "," + preferencesDatOriginal.SizeOfShapeInPixelForStep1.ToString(); // [設定]SizeOfShapeInPixelForStep1(Step1の図形サイズ)";
                line += "," + preferencesDatOriginal.SizeOfShapeInPixelForStep2.ToString(); // [設定]SizeOfShapeInPixelForStep2(Step2の図形サイズ)";
                line += "," + preferencesDatOriginal.SizeOfShapeInPixelForStep3.ToString(); // [設定]SizeOfShapeInPixelForStep3(Step3の図形サイズ)";
                line += "," + preferencesDatOriginal.SizeOfShapeInPixelForStep4.ToString(); // [設定]SizeOfShapeInPixelForStep4(Step4の図形サイズ)";
                line += "," + preferencesDatOriginal.SizeOfShapeInPixelForStep5.ToString(); // [設定]SizeOfShapeInPixelForStep5(Step5の図形サイズ)";
                line += "," + preferencesDatOriginal.SizeOfShapeInPixel.ToString(); // [設定]SizeOfShapeInPixel(ピクセル指定の図形サイズ)
                line += "," + preferencesDatOriginal.SoundFileOfEnd; // [設定]SoundFileOfEnd(終了音ファイル)
                line += "," + preferencesDatOriginal.TimeToOutputSoundOfEnd.ToString(); // [設定]TimeToOutputSoundOfEnd(終了音出力時間)
                line += "," + preferencesDatOriginal.SoundFileOfReward; // [設定]SoundFileOfReward(報酬音ファイル)
                line += "," + preferencesDatOriginal.TimeToOutputSoundOfReward.ToString(); // [設定]TimeToOutputSoundOfReward(報酬音出力時間)
                line += "," + preferencesDatOriginal.SoundFileOfCorrect; // [設定]SoundFileOfReward(正答音ファイル)
                line += "," + preferencesDatOriginal.TimeToOutputSoundOfCorrect.ToString(); // [設定]TimeToOutputSoundOfReward(正答音出力時間)
                line += "," + opCollection.FeedTime.ToString();
                line += "," + opCollection.FeedSoundTime.ToString();
                line += "," + opCollection.FeedLampTime.ToString();
                #endregion 設定に関するコンテンツ

                line += "," + opCollection.DetectCount; // 検出個体数

                line += "," + opCollection.MultiIdProgName; // マルチID実行ファイル名                

                opCollection.file.WriteLineAsync(line);
            };
            #endregion 出力ファイル

            #region ユーザコントロールのイベント
            /// <summary>
            /// コマンドを設定
            /// </summary>
            /// <param name="cmd"></param>
            void setCommand(OpCollection.ECommand cmd)
            {
                WriteLineStatus("[コマンド]," + cmd.GetEnumDisplayName() + opCollection.IsBusy.Value.ToString() + " [" + opCollection.sequencer.State.GetEnumDisplayName() + "]");

                switch (cmd)
                {
                    case OpCollection.ECommand.Start:
                        {
                            if (IsOperationBackButtonBusy.Value == true)
                            {
                                WriteLineStatus("戻るボタン動作中");
                                return;
                            }

                            if (opCollection.IsBusy.Value == true)
                            {
                                WriteLineStatus("動作中");
                                return;
                            }
                            break;
                        }
                    case OpCollection.ECommand.Stop:
                    case OpCollection.ECommand.EmergencyStop:
                        {
                            if (opCollection.IsBusy.Value == false)
                            {
                                WriteLineStatus("停止中");
                                return;
                            }
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
                opCollection.Command = cmd;
            }

            // 開始ボタン
            userControlOperationOnFormMain.buttonStart.Click += (sender, e) =>
            {
                if (!backgroundWorker1.IsBusy)
                    backgroundWorker1.RunWorkerAsync();
                setCommand(OpCollection.ECommand.Start);
            };

            // 停止ボタン
            userControlOperationOnFormMain.buttonStop.Click += (sender, e) =>
            {
                setCommand(OpCollection.ECommand.Stop);
            };

            // 緊急停止ボタン
            userControlOperationOnFormMain.buttonEmergencyStop.Click += (sender, e) =>
            {
                setCommand(OpCollection.ECommand.EmergencyStop);
            };

            userControlOperationOnFormMain.buttonDebugEnter.Click += (sender, e) =>
            {
                OpFlagRoomIn = true;
            };
            userControlOperationOnFormMain.buttonDebugLeave.Click += (sender, e) =>
            {
                OpFlagRoomOut = true;
            };

            userControlOperationOnFormMain.buttonDebugSetID.Click += (sender, e) =>
            {
                opCollection.idCode = userControlOperationOnFormMain.textBoxDebugID.Text;
                IdCodeInOperation.Value = opCollection.idCode;
            };

            // 戻るボタン
            userControlOperationOnFormMain.buttonBack.Click += (sender, e) =>
            {
                var task = Task.Run(async () =>
                {
                    if (IsOperationBackButtonBusy.Value == true)
                    {
                        WriteLineStatus("戻るボタン動作中");
                        return;
                    }

                    await Task.Run(() =>
                    {
                        IsOperationBackButtonBusy.Value = true;

                        setCommand(OpCollection.ECommand.Stop);
                        while (opCollection.IsBusy.Value == true) { Thread.Sleep(1); };

                        // userControlMainOnFormMain表示
                        {
                            EndUcOperation();

                            Invoke((MethodInvoker)(() =>
                            {
                                VisibleUcMain();
                            }));
                        }

                        IsOperationBackButtonBusy.Value = false;
                    });
                });

            };

            // 表示が変わったとき
            userControlOperationOnFormMain.VisibleChanged += (sender, e) =>
            {
                if (userControlOperationOnFormMain.Visible == true)
                {
                    // シリアル・ポート・オープンしていない時（デバッグモードではスキップ）
                    if (serialPortOpenFlag != true && !preferencesDatOriginal.EnableDebugMode)
                    {
                        MessageBox.Show("COM port isn't openned", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        if (Properties.Settings.Default.IS_CHECK_SERIAL_OPENED == true)
                        {
                            EndUcOperation();
                            VisibleUcMain();
                            return;
                        }
                    }
                    // TextBoxをクリアする
                    //					userControlOperationOnFormMain.textBoxStatusOnUcOperation.ResetText();

                    // メイン・ステート: 初期化
                    mainStateVal = EMainState.Init;
                    // 動作モード: Operation
                    opeModeTypeVal = EOpeModeType.Operation;
                    // タイマ開始

#if !BG_WORKER2
                    //                    timerOperation.Start();
#else

#endif
                    // タッチ・パネル初期化
                    // FormSubのMouseClickデリゲートを無効とする
                    formSub.boolEnableCallBackTouchPoint.Value = false;
                    formSub.callbackTouchPoint = (point) =>
                    {
                        // タッチ座標をキューへ入力
                        concurrentQueueFromTouchPanel.Enqueue(point);
                    };
                    // FormSubのMouseClickデリゲートを有効とする
                    formSub.boolEnableCallBackTouchPoint.Value = true;
                    // シリアルの受信デリゲートを無効にする
                    serialHelperPort.isEnableCallBackReceivedData.Value = false;
                    callbackReceivedDataSub = (str) =>
                    {
                        IdCodeInOperation.Value = str;
                    };
                    // シリアルの受信デリゲートを有効にする
                    serialHelperPort.isEnableCallBackReceivedData.Value = true;

                    // OpeImage初期化
                    if ((opImage.InitOpeImage(
                            userControlOperationOnFormMain.pictureBoxTouchPanelOnUcOperation,
                            formSub.pictureBoxOnFormSub) != true) ||
                        (opImage.SetParamOfShapeOpeImage(
                            preferencesDatOriginal.TypeOfShape,
                            preferencesDatOriginal.SizeOfShapeInStep,
                            preferencesDatOriginal.SizeOfShapeInPixel) != true))
                    {
                        MessageBox.Show("Error in initialize image", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        EndUcOperation();
                        VisibleUcMain();
                        return;
                    }
                    // 画面:黒
                    opImage.DrawBackColor(Color.Black);

                    // 開始を促すメッセージボックス
                    const bool isShowStartOperationMessageBox = false;
                    if (isShowStartOperationMessageBox == true)
                    {
                        if (MessageBox.Show("Start operation ?", "Infomation", MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                        {
                            setCommand(OpCollection.ECommand.Start);
                        }
                        else
                        {
                            return;
                        }
                    }
                    else
                    {
                        // setCommand(OpCollection.ECommand.Start);
                    }
                    userControlOperationOnFormMain.labelIllegalExitIndicator.Visible = preferencesDatOriginal.OpeEnableReEntry;


                }
            };

            // 表示が変わったとき
            userControlOperationOnFormMain.VisibleChanged += (sender, e) =>
            {
                int width = userControlOperationOnFormMain.splitContainer2.Size.Width;
                int multiPlier = 1000;
                int aspectWideX = 16;
                int aspectWideY = 9;
                float aspectLegacyX = 4f;
                float aspectLegacyY = 3f;

                if ((opImage.WidthOfWholeArea * multiPlier / opImage.HeightOfWholeArea * multiPlier) == (aspectWideX * multiPlier / aspectWideY * multiPlier))
                {
                    userControlOperationOnFormMain.splitContainer2.SplitterDistance = (int)(width * (float)aspectWideY / (float)aspectWideX);
                }
                else
                {
                    userControlOperationOnFormMain.splitContainer2.SplitterDistance = (int)(width * aspectLegacyY / aspectLegacyX);
                }

            };
            #endregion ユーザコントロールのイベント

            return;
        }

        /// <summary>
        /// userControlOperationOnFormMain: 表示
        /// </summary>
        private void VisibleUcOperation()
        {
            // シリアル・ポート・オープンしていない時（デバッグモードではスキップ）
            if (serialPortOpenFlag != true && !preferencesDatOriginal.EnableDebugMode)
            {
                MessageBox.Show("COM port isn't openned", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (Properties.Settings.Default.IS_CHECK_SERIAL_OPENED == true)
                {
                    EndUcOperation();
                    VisibleUcMain();
                    return;
                }
            }
            // datastore.datのデータ初期性の確認
            {
                if (ucOperationDataStore != null)
                {
                    var ids = ucOperationDataStore.GetIDs();
                    if (ids.Count == 0)
                    {
                        MessageBox.Show("BlockProgramが設定されていません。", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        EndUcOperation();
                        VisibleUcMain();
                        return;
                    }
                }
            }


            // UIの更新
            {
                // TextBoxをクリアする
                userControlOperationOnFormMain.textBoxStatusOnUcOperation.ResetText();

                userControlOperationOnFormMain.textBoxSettingCompartmentNoOnUcOperation.Text = preferencesDatOriginal.CompartmentNo.ToString();
                //Blockモードの時はそのように表示
                if (Program.EnableNewEngine)
                {
                    userControlOperationOnFormMain.textBoxSettingTypeOfTaskOnUcOperation.Text = "Block Programming";
                }
                else
                {
                    userControlOperationOnFormMain.textBoxSettingTypeOfTaskOnUcOperation.Text = preferencesDatOriginal.OpeTypeOfTask;
                }
                userControlOperationOnFormMain.textBoxSettingNumberOfTrialOnUcOperation.Text = preferencesDatOriginal.OpeNumberOfTrial.ToString();

                /*
				if (preferencesDatOriginal.OpeNumberOfTrial == 0)
				{
					userControlOperationOnFormMain.textBoxCurentNumberOfTrial.Text = "Infinite";
				}
				else
				{
					userControlOperationOnFormMain.textBoxCurentNumberOfTrial.Text = "0/" + preferencesDatOriginal.OpeNumberOfTrial.ToString();
				}
				*/
                userControlOperationOnFormMain.textBoxCurentNumberOfTrial.Text = "-";
                userControlOperationOnFormMain.textBoxCurrentIdCode.Text = "-";
            }

            // userControlOperationOnFormMain: 表示
            userControlMainOnFormMain.Visible = false;
            userControlOperationOnFormMain.Visible = true;
            userControlOperationOnFormMain.Dock = DockStyle.Fill;
            userControlCheckDeviceOnFormMain.Visible = false;
            userControlCheckIoOnFormMain.Visible = false;
            userControlPreferencesTabOnFormMain.Visible = false;
            userControlInputComOnFormMain.Visible = false;

            // Form.Text設定
            this.Text = "Operation";
        }

        public void EndUcOperation()
        {
            // タイマ停止
            timerOperation.Stop();
            // FormSubのMouseClickイベントを無効とする
            formSub.boolEnableCallBackTouchPoint.Value = false;
            formSub.callbackTouchPoint = (point) => { };
            // シリアルの受信デリゲートを無効にする
            serialHelperPort.isEnableCallBackReceivedData.Value = false;
            callbackReceivedDataSub = (str) => { };
            OpSubEndToTouchCorrectOnTouchPanel();
        }

        public void UpdateOptionalStatus()
        {
            UpdateIllegalStatus();
            UpdateEpisodeStatus();
        }

        public void UpdateIllegalStatus()
        {
            Invoke((MethodInvoker)(() =>
            {
                userControlOperationOnFormMain.labelWaitIligalExit.Visible = (opCollection.sequencer.State == OpCollection.Sequencer.EState.IllegalExitDetection_WaitReEnter);
            }));
        }
        public void UpdateEpisodeStatus()
        {
            // UcOperationIfでExipreチェック直後でないと意味がないのでそこでフラグ状態を入れる
            OpSubCheckIdExpire();
            Invoke((MethodInvoker)(() =>
            {
                userControlOperationOnFormMain.labelEpisodeStatus.Visible = EpisodeActive.Value;
                if (EpisodeMode.Value)
                {
                    userControlOperationOnFormMain.labelEpisodeStatus.Text = "エピソード記憶出題中";
                    //エピソード記憶出題中
                }
                else
                {
                    userControlOperationOnFormMain.labelEpisodeStatus.Text = "エピソード記憶回答中";
                }
            }));
        }
        public void ClearEpisodeStatus()
        {
            userControlOperationOnFormMain.labelEpisodeStatus.Visible = false;
        }
    }
}

#region オペレーションコレクション
namespace Compartment
{
    public partial class FormMain : Form
    {
        /// <summary>
        /// オペレーションステータスに追加
        /// </summary>
        /// <param name="str"></param>
        public void AppendOperationStatus(string str)
        {
            //			str = System.DateTime.Now.ToString("yyyy年MM月dd日HH時mm分ss秒,") + str;
            str = System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss,") + str;
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)(() =>
                {
                    userControlOperationOnFormMain.textBoxStatusOnUcOperation.AppendText(str);
                }));
            }
            else
            {
                userControlOperationOnFormMain.textBoxStatusOnUcOperation.AppendText(str);
            }
            logFile.WriteAsync(str);
        }

        public void WriteLineStatus(string str)
        {
            str += Environment.NewLine;
            AppendOperationStatus(str);
        }

        /// <summary>
        /// トレース機能
        /// </summary>
        /// <param name="message"></param>
        /// <param name="memberName"></param>
        /// <param name="sourceFilePath"></param>
        /// <param name="sourceLineNumber"></param>
        public void TraceMessage(string message, [System.Runtime.CompilerServices.CallerMemberName] string memberName = "", [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "", [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)
        {
            string str = "";
            str += "[TraceMessage]" + message + ",";
            //			str += "関数名=" + memberName + ",";
            //			str += "ファイル名=" + Path.GetFileName(sourceFilePath) + ",";
            //			str += "行=" + sourceLineNumber;
            WriteLineStatus(str);
        }

        #region ログファイル
        /// <summary>
        /// ログファイル
        /// </summary>
        public class LogFile
        {
            ~LogFile()
            {
                Close();
            }

            public void WriteLineAsync(string value, bool isAddDateTime = false)
            {
                if (writer is null) { return; }

                if (isAddDateTime == true) { value = DateTime.Now.ToString("yyyy年MM月dd日,HH時mm分ss秒") + "," + value; }
                var task = Task.Run(async () =>
                {
                    await writer.WriteLineAsync(value);
                    writer.Flush();
                });
            }

            public void WriteAsync(string value, bool isAddDateTime = false)
            {
                if (writer is null) { return; }

                if (isAddDateTime == true) { value = DateTime.Now.ToString("yyyy年MM月dd日,HH時mm分ss秒") + "," + value; }
                var task = Task.Run(async () =>
                {
                    await writer.WriteAsync(value);
                    writer.Flush();
                });
            }

            private StreamWriter writer = null;

            private string mFolder = Application.LocalUserAppDataPath;
            private readonly string mFilePath = System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase) + ".log";
            private string mFileName = "";

            public void Open(string path, bool isFullPath = true)
            {
                if (writer != null) { Close(); }

                if (isFullPath == true)
                {
                    mFileName = path;
                }
                else
                {
                    mFolder = path;
                    mFileName = System.IO.Path.Combine(mFolder, mFilePath);
                }

                writer = new StreamWriter(mFileName, true, Encoding.UTF8);
            }

            public void Start()
            {
                System.Diagnostics.Process p = System.Diagnostics.Process.Start(mFileName);
            }

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

        private readonly LogFile logFile = new LogFile();
        #endregion ログファイル

        public OpCollection opCollection = new OpCollection();
        private DateTime dt = new DateTime();
        private OpCollection.Sequencer.EState lastState = OpCollection.Sequencer.EState.Init;


        /// <summary>
        /// オペレーションステートマシン処理
        /// </summary>
        private void OnOperationStateMachineProc()
        {

            switch (opCollection.sequencer.State)
            {
                case OpCollection.Sequencer.EState.Init:
                    // 初期化
                    {
                        opCollection.sequencer.State = OpCollection.Sequencer.EState.Idle;
                        break;
                    }
                case OpCollection.Sequencer.EState.Idle:
                    // アイドル
                    {
                        // 開始
                        if (opCollection.Command == OpCollection.ECommand.Start)
                        {
                            //eDoor自動開始
                            eDoor.Enable = true;

                            opCollection.IsBusy.Value = true;
                            opCollection.file.Open(preferencesDatOriginal.OutputResultFile);
                            // opCollection.file.Open(Application.LocalUserAppDataPath, "OpCollection.csv");

                            //初期回数表示
                            opCollection.trialCount = 0; // 試行回数カウンタをクリア
                            opCollection.callbackSetUiCurentNumberOfTrial(opCollection.trialCount);

                            opCollection.sequencer.State = OpCollection.Sequencer.EState.PreEnterCageProc;
                        }
                        break;
                    }
                #region デバイススタンバイ
                case OpCollection.Sequencer.EState.DeviceStandbyBegin:
                    // デバイススタンバイ開始
                    {
                        opCollection.callbackMessageNormal("デバイススタンバイ開始");
                        // 処理内容
                        {
                            OpMoveLeverIn(); // レバーをIN

                            OpSetRoomLampOff(); // 天井ランプをOFF
                            OpDrawBackColorBlackOnTouchPanel(); // タッチパネル画面を黒
                            OpSetFeedLampOff(); // フィードランプをOFF
                            OpSetLeverLampOff(); // レバーランプをOFF
                            OpAirPuffOff(); // エアパフをOFF
                        }
                        if (preferencesDatOriginal.DisableLever)
                        {
                            // DisableDoor時操作しない
                            if (!preferencesDatOriginal.DisableDoor)
                            {
                                OpOpenDoor(); // ドアをOPEN
                            }
                            else
                            {
                                opCollection.callbackMessageNormal("ドア無効ユーザー入室");
                            }
                            opCollection.sequencer.State = OpCollection.Sequencer.EState.DeviceStandbyEnd;
                        }
                        else
                        {
                            opCollection.sequencer.State = OpCollection.Sequencer.EState.DeviceStandby2;
                        }

                        break;
                    }
                case OpCollection.Sequencer.EState.DeviceStandby2:
                    // デバイススタンバイ中
                    {
                        // レバーIN待ち
                        if (OpFlagMoveLeverIn == true)
                        {
                            if (!preferencesDatOriginal.DisableLever)
                            {
                                opCollection.callbackMessageNormal("レバーIN完了");
                            }

                            if (!preferencesDatOriginal.DisableDoor)
                            {
                                OpOpenDoor(); // ドアをOPEN
                            }
                            else
                            {
                                opCollection.callbackMessageNormal("[Debug] ドア無効のためスキップ");
                            }

                            opCollection.sequencer.State = OpCollection.Sequencer.EState.DeviceStandbyEnd;
                        }
                        else
                        {
                            // デバッグ: レバーIN待ち中
                            System.Diagnostics.Debug.WriteLine("[DeviceStandby2] レバーIN待ち中... OpFlagMoveLeverIn=" + OpFlagMoveLeverIn);
                        }
                        break;
                    }
                case OpCollection.Sequencer.EState.DeviceStandbyEnd:
                    // デバイススタンバイ完了
                    {
                        if (OpFlagOpenDoor == true)
                        {
                            opCollection.callbackMessageNormal("ドアOPEN完了");
                            opCollection.sequencer.LoadState();

                            // Stop状態からDeviceStandbyに遷移した場合、LoadStateでStopに戻ってしまうので
                            // その場合はIsBusyをfalseにしてIdleに遷移する
                            if (opCollection.sequencer.State == OpCollection.Sequencer.EState.Stop)
                            {
                                opCollection.callbackMessageNormal("停止完了");
                                opCollection.file.Close();
                                opCollection.IsBusy.Value = false;
                                opCollection.sequencer.State = OpCollection.Sequencer.EState.Idle;
                            }
                        }
                        else if (preferencesDatOriginal.DisableDoor)
                        {
                            opCollection.callbackMessageNormal("ドア無効待機完了");
                            opCollection.sequencer.LoadState();

                            // Stop状態からDeviceStandbyに遷移した場合、LoadStateでStopに戻ってしまうので
                            // その場合はIsBusyをfalseにしてIdleに遷移する
                            if (opCollection.sequencer.State == OpCollection.Sequencer.EState.Stop)
                            {
                                opCollection.callbackMessageNormal("停止完了");
                                opCollection.file.Close();
                                opCollection.IsBusy.Value = false;
                                opCollection.sequencer.State = OpCollection.Sequencer.EState.Idle;
                            }
                        }
                        else
                        {
                            // デバッグ: ドアOPEN待ち中
                            System.Diagnostics.Debug.WriteLine("[DeviceStandbyEnd] ドアOPEN待ち中... OpFlagOpenDoor=" + OpFlagOpenDoor);
                        }
                        break;
                    }
                #endregion デバイススタンバイ

                #region 停止系
                case OpCollection.Sequencer.EState.Stop:
                    // 停止
                    {
                        eDoor.Enable = false;
                        UpdateIllegalStatus();
                        if (opCollection.sequencer.BeforeState == OpCollection.Sequencer.EState.DeviceStandbyEnd)
                        {
                            opCollection.file.Close();
                            opCollection.IsBusy.Value = false;
                            opCollection.sequencer.State = OpCollection.Sequencer.EState.Idle;
                        }
                        else
                        {
                            opCollection.sequencer.StoreStateStack();
                            opCollection.sequencer.State = OpCollection.Sequencer.EState.DeviceStandbyBegin;
                        }
                        break;
                    }
                case OpCollection.Sequencer.EState.EmergencyStop:
                    // 緊急停止
                    {

                        UpdateIllegalStatus();

                        opCollection.file.Close();
                        opCollection.IsBusy.Value = false;
                        opCollection.sequencer.State = OpCollection.Sequencer.EState.Idle;
                        break;
                    }
                #endregion 停止系

                #region 入室
                case OpCollection.Sequencer.EState.PreEnterCageProc:
                    // 入室前処理
                    {
                        if (opCollection.sequencer.BeforeState == OpCollection.Sequencer.EState.DeviceStandbyEnd)
                        {
                            OpeClearIdCode();
                            dt = DateTime.Now;
                            opCollection.sequencer.State = OpCollection.Sequencer.EState.WaitingForEnterCage;
                            OpFlagRoomIn = false;
                        }
                        else
                        {
                            opCollection.sequencer.StoreStateStack();
                            opCollection.sequencer.State = OpCollection.Sequencer.EState.DeviceStandbyBegin;
                        }
                        break;
                    }
                case OpCollection.Sequencer.EState.WaitingForEnterCage:
                    // 入室待ち
                    {
                        // 停止系
                        {
                            OpCollection.ECommand command = opCollection.Command;
                            if (command == OpCollection.ECommand.EmergencyStop) { opCollection.sequencer.State = OpCollection.Sequencer.EState.EmergencyStop; break; } // 緊急停止
                            if (command == OpCollection.ECommand.Stop) { opCollection.sequencer.State = OpCollection.Sequencer.EState.Stop; break; } // 中止
                        }
                        // モニタースタンバイ
                        if ((DateTime.Now - dt).TotalMinutes > preferencesDatOriginal.MonitorSaveTime && preferencesDatOriginal.EnableMonitorSave)
                        {
                            opCollection.callbackMessageNormal(string.Format("待機時間{0}[秒]経過 モニターオフ)", (DateTime.Now - dt).TotalMinutes));
                            dt = DateTime.Now;
                            MonitorPower.Monitor.PowerOff();
                        }
#if NO_TRIGGER_TOUCH
                        //debug
                        opCollection.sequencer.State = OpCollection.Sequencer.EState.WaitingForOutLever;
#endif
                        // 入室検知 (ドア無効時 ドア手動閉追加)
                        if (OpFlagRoomIn == true || (preferencesDatOriginal.DisableDoor && (devInDoorClose.dInCurrent == true || devInDoorOpen.dInCurrent == false)))
                        {
                            MonitorPower.Monitor.PowerOn();
                            opCollection.callbackMessageNormal("入室検知");
                            opCollection.TimeEnterCage = DateTime.Now;

                            // 入室検知時に 回数カウント表示
                            // opCollection.callbackSetUiCurentNumberOfTrial(opCollection.trialCount);

                            opCollection.idCode = OpeGetIdCode();
                            opCollection.callbackSetUiCurrentIdCode(opCollection.idCode);

                            if (!preferencesDatOriginal.DisableLever)
                            {
                                OpMoveLeverOut(); // レバーをOUT
                            }

                            OpFlagRoomOut = false;


                            // IlegalExit時ステートを再開
                            if (lastState != OpCollection.Sequencer.EState.Init)
                            {
                                // lastState初期化
                                lastState = OpCollection.Sequencer.EState.Init;
                                // タッチ直前まで復帰
                                opCollection.sequencer.State = OpCollection.Sequencer.EState.PreTouchTriggerScreenProc;
                            }
                            else if (lastState == OpCollection.Sequencer.EState.WaitingForTouchTriggerScreen)
                            {
                                opCollection.sequencer.State = OpCollection.Sequencer.EState.PreTouchTriggerScreenProc;
                            }
                            else
                            {
                                opCollection.sequencer.State = OpCollection.Sequencer.EState.WaitingForOutLever;
                            }
                        }
                        break;
                    }
                case OpCollection.Sequencer.EState.WaitingForOutLever:
                    // レバーOUT待ち
                    {
                        // 停止系
                        {
                            OpCollection.ECommand command = opCollection.Command;
                            if (command == OpCollection.ECommand.EmergencyStop) { opCollection.sequencer.State = OpCollection.Sequencer.EState.EmergencyStop; break; } // 緊急停止
                            if (command == OpCollection.ECommand.Stop) { opCollection.sequencer.State = OpCollection.Sequencer.EState.Stop; break; } // 中止
                        }

                        if (OpFlagRoomOut)
                        {
                            lastState = opCollection.sequencer.State;
                            opCollection.sequencer.State = OpCollection.Sequencer.EState.IllegalExitDetection;
                            break;
                        } // イリーガル退室検知

                        if (preferencesDatOriginal.DisableLever)
                        {
                            opCollection.stopwatch.Restart(); // ストップウォッチをリスタート
                            OpFlagLeverSw = false;
                            opCollection.sequencer.State = OpCollection.Sequencer.EState.WaitingForDownLever;
                        }

                        // 動作完了
                        if (OpFlagMoveLeverOut == true)
                        {
                            // 結果正常
                            if (OpResultMoveLeverOut == EDeviceResult.Done)
                            {
                                if (!preferencesDatOriginal.DisableLever)
                                {
                                    opCollection.callbackMessageNormal("レバーOUT正常");
                                    opCollection.callbackMessageNormal(string.Format("レバーDOWN開始(タイムアウト時間={0}[分])", preferencesDatOriginal.OpeTimeoutOfStart));
                                }
                                opCollection.stopwatch.Restart(); // ストップウォッチをリスタート
                                OpFlagLeverSw = false;
                                opCollection.sequencer.State = OpCollection.Sequencer.EState.WaitingForDownLever;
                            }

                            // 結果異常
                            else
                            {
                                opCollection.callbackMessageNormal("レバーOUT異常");
                                opCollection.sequencer.State = OpCollection.Sequencer.EState.PreLeaveCageProc;
                            }
                        }
                        break;
                    }
                case OpCollection.Sequencer.EState.WaitingForDownLever:
                    // レバーDOWN待ち
                    {
                        // 停止系
                        {
                            OpCollection.ECommand command = opCollection.Command;
                            if (command == OpCollection.ECommand.EmergencyStop) { opCollection.sequencer.State = OpCollection.Sequencer.EState.EmergencyStop; break; } // 緊急停止
                            if (command == OpCollection.ECommand.Stop) { opCollection.sequencer.State = OpCollection.Sequencer.EState.Stop; break; } // 中止
                        }

                        if (OpFlagRoomOut)
                        {
                            lastState = opCollection.sequencer.State;
                            opCollection.sequencer.State = OpCollection.Sequencer.EState.IllegalExitDetection;
                            break;
                        } // イリーガル退室検知

                        // タイムアウト
                        if (opCollection.stopwatch.Elapsed.TotalMinutes >= preferencesDatOriginal.OpeTimeoutOfStart)
                        {
                            opCollection.callbackMessageNormal("レバーDOWNタイムアウト");
                            opCollection.stopwatch.Stop(); // ストップウォッチを停止
                            if (!preferencesDatOriginal.DisableLever)
                            {
                                OpMoveLeverIn(); // レバーをIN
                            }

                            opCollection.sequencer.State = OpCollection.Sequencer.EState.PreLeaveCageProc;
                            break;
                        }

                        if (preferencesDatOriginal.DisableLever)
                        {
                            opCollection.sequencer.State = OpCollection.Sequencer.EState.WaitingForInLever;
                        }

                        // レバーDOWN
                        if (OpFlagLeverSw == true)
                        {
                            if (!preferencesDatOriginal.DisableLever)
                            {
                                opCollection.callbackMessageNormal("レバーDOWN正常");
                            }

                            OpMoveLeverIn(); // レバーをIN
                            opCollection.sequencer.State = OpCollection.Sequencer.EState.WaitingForInLever;
                        }
                        break;
                    }
                case OpCollection.Sequencer.EState.WaitingForInLever:
                    // レバーIN待ち
                    {
                        // 停止系
                        {
                            OpCollection.ECommand command = opCollection.Command;
                            if (command == OpCollection.ECommand.EmergencyStop) { opCollection.sequencer.State = OpCollection.Sequencer.EState.EmergencyStop; break; } // 緊急停止
                            if (command == OpCollection.ECommand.Stop) { opCollection.sequencer.State = OpCollection.Sequencer.EState.Stop; break; } // 中止
                        }

                        if (OpFlagRoomOut)
                        {
                            lastState = opCollection.sequencer.State;
                            opCollection.sequencer.State = OpCollection.Sequencer.EState.IllegalExitDetection;
                            break;
                        } // イリーガル退室検知

                        if (preferencesDatOriginal.DisableLever)
                        {
                            opCollection.stopwatch.Restart(); // ストップウォッチをリスタート
                            if (!preferencesDatOriginal.DisableDoor)
                            {
                                OpCloseDoor(); // ドアをCLOSE
                                opCollection.sequencer.State = OpCollection.Sequencer.EState.WaitingForCloseDoor;
                            }
                            else
                            {
                                OpMakeScheduleOfFeed(); // フィード・スケジュールを行う
                                opCollection.trialCount = 0; // 試行回数カウンタをクリア
                                opCollection.sequencer.State = OpCollection.Sequencer.EState.PreTouchTriggerScreenProc;
                            }
                            break;
                        }

                        // レバーIN動作完了
                        if (OpFlagMoveLeverIn == true)
                        {
                            // レバーIN異常
                            if (OpResultMoveLeverIn != EDeviceResult.Done)
                            {
                                opCollection.callbackMessageNormal("レバーIN異常");
                                opCollection.sequencer.State = OpCollection.Sequencer.EState.PreLeaveCageProc;
                                break;
                            }

                            // レバーIN正常
                            {
                                if (!preferencesDatOriginal.DisableLever)
                                {
                                    opCollection.callbackMessageNormal("レバーIN正常");
                                }

                                opCollection.stopwatch.Restart(); // ストップウォッチをリスタート
                                if (!preferencesDatOriginal.DisableDoor)
                                {
                                    OpCloseDoor(); // ドアをCLOSE
                                    opCollection.sequencer.State = OpCollection.Sequencer.EState.WaitingForCloseDoor;
                                }
                                else
                                {
                                    OpMakeScheduleOfFeed(); // フィード・スケジュールを行う
                                    opCollection.trialCount = 0; // 試行回数カウンタをクリア
                                    opCollection.sequencer.State = OpCollection.Sequencer.EState.PreTouchTriggerScreenProc;
                                }
                            }
                        }
                        break;
                    }
                case OpCollection.Sequencer.EState.WaitingForCloseDoor:
                    // ドアCLOSE待ち
                    {
                        // 停止系
                        {
                            OpCollection.ECommand command = opCollection.Command;
                            if (command == OpCollection.ECommand.EmergencyStop) { opCollection.sequencer.State = OpCollection.Sequencer.EState.EmergencyStop; break; } // 緊急停止
                            if (command == OpCollection.ECommand.Stop) { opCollection.sequencer.State = OpCollection.Sequencer.EState.Stop; break; } // 中止
                        }

                        if (OpFlagRoomOut)
                        {
                            lastState = opCollection.sequencer.State;
                            opCollection.sequencer.State = OpCollection.Sequencer.EState.IllegalExitDetection;
                            break;
                        } // イリーガル退室検知

                        // CLOSE動作完了
                        if (OpFlagCloseDoor == true)
                        {
                            // ドアCLOSE異常
                            if (OpResultCloseDoor != EDeviceResult.Done)
                            {
                                opCollection.callbackMessageNormal("ドアCLOSE異常");
                                if (!preferencesDatOriginal.IgnoreDoorError)
                                {
                                    opCollection.sequencer.State = OpCollection.Sequencer.EState.PreLeaveCageProc;
                                }
                                else
                                {
                                    OpMakeScheduleOfFeed(); // フィード・スケジュールを行う
                                    opCollection.trialCount = 0; // 試行回数カウンタをクリア
                                    opCollection.sequencer.State = OpCollection.Sequencer.EState.PreTouchTriggerScreenProc;
                                }
                                break;
                            }

                            // ドアCLOSE正常
                            {
                                opCollection.callbackMessageNormal("ドアCLOSE正常処理");
                                OpMakeScheduleOfFeed(); // フィード・スケジュールを行う
                                opCollection.trialCount = 0; // 試行回数カウンタをクリア
                                opCollection.sequencer.State = OpCollection.Sequencer.EState.PreTouchTriggerScreenProc;
                            }
                        }
                        break;
                    }
                #endregion 入室

                #region 試行
                case OpCollection.Sequencer.EState.PreTouchTriggerScreenProc:
                    // トリガ画面タッチ前処理
                    {
                        opCollection.callbackMessageNormal(string.Format("トリガ画面タッチ待ち(タイムアウト時間={0}[分])", preferencesDatOriginal.OpeTimeoutOfTrial));
                        opCollection.flagFeed = false; // 給餌したか否かのフラグ:初期化(結果ファイル出力用)
                        opCollection.intervalTimeTotal = 0; // インターバル値合計:初期化(結果ファイル出力用)
                        opCollection.intervalNum = 0; ; // インターバル回数:初期化(結果ファイル出力用)
                        opCollection.dateTimeTriggerTouch = new DateTime();      // トリガ画面タッチ時刻:初期化(結果ファイル出力用)
                        opCollection.dateTimeCorrectTouch = new DateTime();      // 正解図形タッチ時刻:初期化(結果ファイル出力用)
                        opCollection.dateTimeout = new DateTime();               // タイムアウト時刻:初期化(結果ファイル出力用))
                        opCollection.pointCorrectShape = new Point(0, 0); // 正解図形の座標: 初期化(結果ファイル出力用)
                        opCollection.pointWrongShape = new Point(0, 0); // 不正解図形の座標: 初期化(結果ファイル出力用)

                        //描画画像初期化
                        {
                            opImage.SetParamOfShapeOpeImage(
                                preferencesDatOriginal.TypeOfShape,
                                preferencesDatOriginal.SizeOfShapeInStep,
                                preferencesDatOriginal.SizeOfShapeInPixel);
                        }
                        if (OpeGetTypeOfTask() == ECpTask.UnConditionalFeeding)
                        {
                            opCollection.callbackMessageNormal("入室報酬");
                            opCollection.callbackMessageNormal(string.Format("訓練(タイムアウト時間={0}[分])", preferencesDatOriginal.OpeTimeoutOfTrial));
                            opCollection.taskResultVal = OpCollection.ETaskResult.Ok;       // 結果: 正解を保存
                            opCollection.dateTimeCorrectTouch = DateTime.Now; // 正解時刻時刻を保存
                            opCollection.sequencer.State = OpCollection.Sequencer.EState.CorrectAnswer;
                        }
                        else
                        {
                            OpSetRoomLampOff(); // 天井ランプをOFF
                            OpDrawTriggerImageOnTouchPanel(); // トリガ画面表示
                            OpStartToTouchAnyOnTouchPanel(); // どこでもタッチ検知を開始
                            opCollection.stopwatch.Restart(); // ストップウォッチをリスタート
                            opCollection.sequencer.State = OpCollection.Sequencer.EState.WaitingForTouchTriggerScreen;
                        }

                        break;
                    }
                case OpCollection.Sequencer.EState.WaitingForTouchTriggerScreen:
                    // トリガ画面タッチ待ち
                    {
                        // 停止系
                        {
                            OpCollection.ECommand command = opCollection.Command;
                            if (command == OpCollection.ECommand.EmergencyStop) { opCollection.sequencer.State = OpCollection.Sequencer.EState.EmergencyStop; break; } // 緊急停止
                            if (command == OpCollection.ECommand.Stop) { opCollection.sequencer.State = OpCollection.Sequencer.EState.Stop; break; } // 中止
                        }

                        if (OpFlagRoomOut)
                        {
                            lastState = opCollection.sequencer.State;
                            opCollection.sequencer.State = OpCollection.Sequencer.EState.IllegalExitDetection;
                            break;
                        } // イリーガル退室検知

                        // タイムアウト
                        if (opCollection.stopwatch.Elapsed.TotalMinutes >= preferencesDatOriginal.OpeTimeoutOfTrial)
                        {
                            opCollection.callbackMessageNormal("トリガ画面タッチタイムアウト");
                            opCollection.stopwatch.Stop(); // ストップウォッチを停止
                            OpEndToTouchAnyOnTouchPanel(); // どこでもタッチを停止
                            opCollection.taskResultVal = OpCollection.ETaskResult.StartTimeout;       // 結果: 開始タイムアウトを保存(結果ファイル出力用)
                            opCollection.dateTimeout = DateTime.Now;                             // タイムアウト時刻を保存(結果ファイル出力用)
                            opCollection.sequencer.callbackOutputFile(opCollection.sequencer.State); // ファイル出力
                            opCollection.sequencer.State = OpCollection.Sequencer.EState.PreOpenDoorProc;
                            break;
                        }

                        // 画面タッチ
#if NO_TRIGGER_TOUCH
                        if (OpFlagTouchAnyOnTouchPanel != true)
#else
                        if (OpFlagTouchAnyOnTouchPanel == true)
#endif
                        {
                            // 開始時表示だけインクリメント 完了時にカウント
                            opCollection.callbackSetUiCurentNumberOfTrial(opCollection.trialCount + 1);

                            opCollection.dateTimeTriggerTouch = DateTime.Now; // トリガ・タッチ時刻を保存
                            opCollection.callbackMessageNormal("画面タッチ");
                            opCollection.stopwatch.Stop(); // ストップウォッチを停止
                            OpEndToTouchAnyOnTouchPanel(); // どこでもタッチを停止
                            switch (OpeGetTypeOfTask())
                            {
                                case ECpTask.DelayMatch:

                                    if (!preferencesDatOriginal.EnableRandomTime)
                                    {
                                        OpDrawCorrectShapeOnTouchPanel(out opCollection.trainingShape); // 訓練正解図形表示
                                    }
                                    // ディレイマッチ
                                    opCollection.callbackMessageNormal(string.Format("ディレイマッチ(正解図形表示時間={0}[秒])", preferencesDatOriginal.OpeTimeToDisplayCorrectImage));
                                    opCollection.stopwatch.Restart(); // ストップウォッチをリスタート
                                    if (preferencesDatOriginal.EnableRandomTime)
                                    {
                                        OpDrawDelayBackColorOnTouchPanel(); // タッチパネル画面をDelay指定色でクリア
                                        opCollection.beforeDelayMatchRandomTime = OpSubGetAfterMatchRandomTime();
                                        opCollection.callbackMessageNormal(string.Format("ディレイ(ランダム遅延時間={0}[秒])", opCollection.beforeDelayMatchRandomTime));
                                        opCollection.sequencer.State = OpCollection.Sequencer.EState.DelayBeforeMatch;
                                    }
                                    else
                                    {
                                        opCollection.sequencer.State = OpCollection.Sequencer.EState.DelayMatchImage;
                                    }
                                    break;
                                case ECpTask.Training:
                                    // 訓練
                                    opCollection.callbackMessageNormal("画面タッチ(訓練)");
                                    opCollection.callbackMessageNormal(string.Format("訓練(タイムアウト時間={0}[分])", preferencesDatOriginal.OpeTimeoutOfTrial));
                                    OpStartToTouchCorrectOnTouchPanel(); // タッチコレクトを開始
                                    opCollection.stopwatch.Restart(); // ストップウォッチをリスタート
                                    if (preferencesDatOriginal.EnableRandomTime)
                                    {
                                        OpDrawDelayBackColorOnTouchPanel(); // タッチパネル画面をDelay指定色でクリア
                                        opCollection.beforeDelayMatchRandomTime = OpSubGetAfterMatchRandomTime();
                                        opCollection.callbackMessageNormal(string.Format("ディレイ(ランダム遅延時間={0}[秒])", opCollection.beforeDelayMatchRandomTime));
                                        opCollection.sequencer.State = OpCollection.Sequencer.EState.DelayAfterTraning;
                                    }
                                    else
                                    {
                                        OpDrawTrainShapeOnTouchPanel(out opCollection.trainingShape); // 訓練図形表示
                                        opCollection.sequencer.State = OpCollection.Sequencer.EState.Training;
                                    }
                                    break;
                                case ECpTask.TrainingEasy:
                                    // かんたん訓練
                                    opCollection.callbackMessageNormal("画面タッチ(かんたん訓練)");
                                    opCollection.callbackMessageNormal(string.Format("訓練(タイムアウト時間={0}[分])", preferencesDatOriginal.OpeTimeoutOfTrial));
                                    opCollection.taskResultVal = OpCollection.ETaskResult.Ok;       // 結果: 正解を保存
                                    opCollection.dateTimeCorrectTouch = DateTime.Now; // 正解時刻時刻を保存
                                    opCollection.sequencer.State = OpCollection.Sequencer.EState.CorrectAnswer;
                                    break;

                                default:
                                    opCollection.callbackMessageError("存在しないタスク");
                                    opCollection.sequencer.State = OpCollection.Sequencer.EState.Error;
                                    break;
                            }
                        }
                        break;
                    }
                case OpCollection.Sequencer.EState.DelayBeforeMatch:
                    {
                        // 停止系
                        {
                            OpCollection.ECommand command = opCollection.Command;
                            if (command == OpCollection.ECommand.EmergencyStop) { opCollection.sequencer.State = OpCollection.Sequencer.EState.EmergencyStop; break; } // 緊急停止
                            if (command == OpCollection.ECommand.Stop) { opCollection.sequencer.State = OpCollection.Sequencer.EState.Stop; break; } // 中止
                        }

                        if (OpFlagRoomOut)
                        {
                            lastState = opCollection.sequencer.State;
                            opCollection.sequencer.State = OpCollection.Sequencer.EState.IllegalExitDetection;
                            break;
                        } // イリーガル退室検知

                        // タイムアウト
                        if (opCollection.stopwatch.Elapsed.TotalSeconds >= opCollection.beforeDelayMatchRandomTime)
                        {
                            opCollection.stopwatch.Stop(); // ストップウォッチを停止
                            opCollection.stopwatch.Restart(); // ストップウォッチをリスタート
                            OpDrawCorrectShapeOnTouchPanel(out opCollection.trainingShape); // 訓練正解図形表示
                            opCollection.sequencer.State = OpCollection.Sequencer.EState.DelayMatchImage;

                        }
                        break;
                    }
                case OpCollection.Sequencer.EState.DelayMatchImage:
                    // ディレイマッチ(正解図形表示)
                    {
                        // 停止系
                        {
                            OpCollection.ECommand command = opCollection.Command;
                            if (command == OpCollection.ECommand.EmergencyStop) { opCollection.sequencer.State = OpCollection.Sequencer.EState.EmergencyStop; break; } // 緊急停止
                            if (command == OpCollection.ECommand.Stop) { opCollection.sequencer.State = OpCollection.Sequencer.EState.Stop; break; } // 中止
                        }

                        if (OpFlagRoomOut)
                        {
                            lastState = opCollection.sequencer.State;
                            opCollection.sequencer.State = OpCollection.Sequencer.EState.IllegalExitDetection;
                            break;
                        } // イリーガル退室検知

                        // タイムアウト
                        if (opCollection.stopwatch.Elapsed.TotalSeconds >= preferencesDatOriginal.OpeTimeToDisplayCorrectImage)
                        {
                            opCollection.callbackMessageNormal("ディレイマッチ(正解図形表示)正常完了");
                            opCollection.callbackMessageNormal(string.Format("ディレイマッチ(タッチパネル画面を黒にする時間={0}[秒])", preferencesDatOriginal.OpeTimeToDisplayNoImage));
                            opCollection.stopwatch.Stop(); // ストップウォッチを停止
                            opCollection.stopwatch.Restart(); // ストップウォッチをリスタート
                            OpDrawBackColorOnTouchPanel(); // タッチパネル画面を背景色でクリア

                            opCollection.sequencer.State = OpCollection.Sequencer.EState.DelayMatchBackColor;
                        }
                        break;
                    }
                case OpCollection.Sequencer.EState.AfterIllegalExit_DelayMatchImage:
                    {
                        opCollection.stopwatch.Restart(); // ストップウォッチをリスタート
                        OpDrawBackColorOnTouchPanel(); // タッチパネル画面を背景色でクリア

                        // 背景クリアと描画遅延なしで行う
                        try
                        {
                            OpDrawCorrectAndWrongShapeOnTouchPanel(out opCollection.correctShape, out opCollection.incorrectShapeList); // 正解図形・不正解図形表示
                        }
                        catch (Exception ex)
                        {
                            opCollection.callbackMessageError(ex.Message);
                        }
                        // ImageFile 有効時ファイル名ログ伝達
                        if (preferencesDatOriginal.EnableImageShape)
                        {
                            if (preferencesDatOriginal.RandomCorrectImage)
                            {
                                opCollection.correctImageFile = System.IO.Path.GetFileName(opImage.CorrectImageFile);
                            }
                            else
                            {
                                opCollection.correctImageFile = System.IO.Path.GetFileName(preferencesDatOriginal.CorrectImageFile);
                            }
                        }

                        OpStartToTouchCorrectOnTouchPanel(); // タッチコレクトを開始
                        opCollection.sequencer.State = OpCollection.Sequencer.EState.Training;
                        break;
                    }
                case OpCollection.Sequencer.EState.DelayAfterTraning:
                    {
                        // 停止系
                        {
                            OpCollection.ECommand command = opCollection.Command;
                            if (command == OpCollection.ECommand.EmergencyStop) { opCollection.sequencer.State = OpCollection.Sequencer.EState.EmergencyStop; break; } // 緊急停止
                            if (command == OpCollection.ECommand.Stop) { opCollection.sequencer.State = OpCollection.Sequencer.EState.Stop; break; } // 中止
                        }

                        if (OpFlagRoomOut)
                        {
                            lastState = opCollection.sequencer.State;
                            opCollection.sequencer.State = OpCollection.Sequencer.EState.IllegalExitDetection;
                            break;
                        } // イリーガル退室検知

                        // タイムアウト
                        if (opCollection.stopwatch.Elapsed.TotalSeconds >= opCollection.beforeDelayMatchRandomTime)
                        {
                            opCollection.stopwatch.Stop(); // ストップウォッチを停止
                            opCollection.stopwatch.Restart(); // ストップウォッチをリスタート
                            OpDrawTrainShapeOnTouchPanel(out opCollection.trainingShape); // 訓練図形表示
                            opCollection.sequencer.State = OpCollection.Sequencer.EState.Training;

                        }
                        break;
                    }
                case OpCollection.Sequencer.EState.AfterIllegalExit_Training:
                    {
                        // 停止系
                        {
                            OpCollection.ECommand command = opCollection.Command;
                            if (command == OpCollection.ECommand.EmergencyStop) { opCollection.sequencer.State = OpCollection.Sequencer.EState.EmergencyStop; break; } // 緊急停止
                            if (command == OpCollection.ECommand.Stop) { opCollection.sequencer.State = OpCollection.Sequencer.EState.Stop; break; } // 中止
                        }

                        if (OpFlagRoomOut)
                        {
                            lastState = opCollection.sequencer.State;
                            opCollection.sequencer.State = OpCollection.Sequencer.EState.IllegalExitDetection;
                            break;
                        } // イリーガル退室検知


                        opCollection.stopwatch.Restart(); // ストップウォッチをリスタート
                        OpDrawTrainShapeOnTouchPanelAfterIllegalExit(opCollection.trainingShape); // 訓練図形表示
                        opCollection.sequencer.State = OpCollection.Sequencer.EState.Training;

                        break;
                    }
                case OpCollection.Sequencer.EState.DelayMatchBackColor:
                    // ディレイマッチ(タッチパネル画面を背景色でクリア)
                    {
                        // 停止系
                        {
                            OpCollection.ECommand command = opCollection.Command;
                            if (command == OpCollection.ECommand.EmergencyStop) { opCollection.sequencer.State = OpCollection.Sequencer.EState.EmergencyStop; break; } // 緊急停止
                            if (command == OpCollection.ECommand.Stop) { opCollection.sequencer.State = OpCollection.Sequencer.EState.Stop; break; } // 中止
                        }
                        if (OpFlagRoomOut)
                        {
                            lastState = opCollection.sequencer.State;
                            opCollection.sequencer.State = OpCollection.Sequencer.EState.IllegalExitDetection;
                            break;
                        } // イリーガル退室検知

                        // タイムアウト
                        if (opCollection.stopwatch.Elapsed.TotalSeconds >= preferencesDatOriginal.OpeTimeToDisplayNoImage)
                        {
                            opCollection.callbackMessageNormal("ディレイマッチ(タッチパネル画面を黒)正常完了");
                            opCollection.callbackMessageNormal(string.Format("訓練(タイムアウト時間={0}[分])", preferencesDatOriginal.OpeTimeoutOfTrial));
                            opCollection.stopwatch.Stop(); // ストップウォッチを停止
                            opCollection.stopwatch.Restart(); // ストップウォッチをリスタート

                            try
                            {
                                OpDrawCorrectAndWrongShapeOnTouchPanel(out opCollection.correctShape, out opCollection.incorrectShapeList); // 正解図形・不正解図形表示
                            }
                            catch (Exception ex)
                            {
                                opCollection.callbackMessageError(ex.Message);
                            }
                            // ImageFile 有効時ファイル名ログ伝達
                            if (preferencesDatOriginal.EnableImageShape)
                            {
                                if (preferencesDatOriginal.RandomCorrectImage)
                                {
                                    opCollection.correctImageFile = System.IO.Path.GetFileName(opImage.CorrectImageFile);
                                }
                                else
                                {
                                    opCollection.correctImageFile = System.IO.Path.GetFileName(preferencesDatOriginal.CorrectImageFile);
                                }
                            }

                            OpStartToTouchCorrectOnTouchPanel(); // タッチコレクトを開始
                            opCollection.sequencer.State = OpCollection.Sequencer.EState.Training;
                        }
                        break;
                    }
                case OpCollection.Sequencer.EState.Training:
                    // 訓練
                    {
                        // 停止系
                        {
                            OpCollection.ECommand command = opCollection.Command;
                            if (command == OpCollection.ECommand.EmergencyStop) { opCollection.sequencer.State = OpCollection.Sequencer.EState.EmergencyStop; break; } // 緊急停止
                            if (command == OpCollection.ECommand.Stop) { opCollection.sequencer.State = OpCollection.Sequencer.EState.Stop; break; } // 中止
                        }
                        if (OpFlagRoomOut)
                        {
                            lastState = opCollection.sequencer.State;
                            opCollection.sequencer.State = OpCollection.Sequencer.EState.IllegalExitDetection;
                            break;
                        } // イリーガル退室検知

                        // タイムアウト
                        if (opCollection.stopwatch.Elapsed.TotalMinutes >= preferencesDatOriginal.OpeTimeoutOfTrial)
                        {
                            opCollection.callbackMessageNormal("訓練タイムアウト");
                            opCollection.stopwatch.Stop(); // ストップウォッチを停止
                            OpEndToTouchCorrectOnTouchPanel(); // タッチコレクトを停止
                            opCollection.taskResultVal = OpCollection.ETaskResult.TrialTimeout;       // 結果: 試行タイムアウトを保存(結果ファイル出力用)
                            opCollection.dateTimeout = DateTime.Now;                             // タイムアウト時刻を保存(結果ファイル出力用)
                            opCollection.sequencer.callbackOutputFile(opCollection.sequencer.State); // ファイル出力
                            opCollection.sequencer.State = OpCollection.Sequencer.EState.PreOpenDoorProc;
                            break;
                        }

                        // 正解図形タッチ
                        if (OpFlagTouchCorrectOnTouchPanel == true)
                        {
                            opCollection.taskResultVal = OpCollection.ETaskResult.Ok;       // 結果: 正解を保存
                            opCollection.dateTimeCorrectTouch = DateTime.Now; // 正解時刻時刻を保存
                            opCollection.callbackMessageNormal("訓練正解図形タッチ");
                            opCollection.stopwatch.Stop(); // ストップウォッチを停止
                            OpEndToTouchCorrectOnTouchPanel(); // タッチコレクトを停止
                            opCollection.sequencer.State = OpCollection.Sequencer.EState.CorrectAnswer;
                        }
                        break;
                    }
                case OpCollection.Sequencer.EState.CorrectAnswer:
                    // 正解
                    {
                        // 停止系
                        {
                            OpCollection.ECommand command = opCollection.Command;
                            if (command == OpCollection.ECommand.EmergencyStop) { opCollection.sequencer.State = OpCollection.Sequencer.EState.EmergencyStop; break; } // 緊急停止
                            if (command == OpCollection.ECommand.Stop) { opCollection.sequencer.State = OpCollection.Sequencer.EState.Stop; break; } // 中止
                        }
                        if (OpFlagRoomOut)
                        {
                            lastState = opCollection.sequencer.State;
                            opCollection.sequencer.State = OpCollection.Sequencer.EState.IllegalExitDetection;
                            break;
                        } // イリーガル退室検知

                        {
                            OpDrawBackColorBlackOnTouchPanel(); // タッチパネル画面を黒
                            OpSetRoomLampOff(); // 天井ランプをOFF
                            OpPlaySoundOfCorrect(); //正解音ならす
                        }

                        {
                            opCollection.stopwatch.Restart(); // ストップウォッチをリスタート
                            opCollection.sequencer.State = OpCollection.Sequencer.EState.DelayFeed;
                        }
                        break;
                    }

                case OpCollection.Sequencer.EState.DelayFeed:
                    //給餌ディレイ
                    {
                        // 停止系
                        {
                            OpCollection.ECommand command = opCollection.Command;
                            if (command == OpCollection.ECommand.EmergencyStop) { opCollection.sequencer.State = OpCollection.Sequencer.EState.EmergencyStop; break; } // 緊急停止
                            if (command == OpCollection.ECommand.Stop) { opCollection.sequencer.State = OpCollection.Sequencer.EState.Stop; break; } // 中止
                        }

                        if (OpFlagRoomOut)
                        {
                            lastState = opCollection.sequencer.State;
                            opCollection.sequencer.State = OpCollection.Sequencer.EState.IllegalExitDetection;
                            break;
                        } // イリーガル退室検知

                        if (opCollection.stopwatch.Elapsed.TotalMilliseconds >= preferencesDatOriginal.OpeDelayFeedLamp)
                        {
                            if (preferencesDatOriginal.EnableFeedLamp)
                            {
                                if (!OpFlagFeedLampON)
                                {
                                    OpSetFeedLampOn(); // フィードランプをON
                                    OpFlagFeedLampON = true;
                                }
                            }
                        }
                        // ディレイ
                        if (opCollection.stopwatch.Elapsed.TotalMilliseconds >= preferencesDatOriginal.OpeDelayFeed && PlaySoundEnded)
                        {
                            opCollection.stopwatch.Stop(); // ストップウォッチを停止

                            OpPlaySoundOfReward(); // 報酬音を再生

                            opCollection.stopwatch.Restart(); // ストップウォッチをリスタート
                            opCollection.sequencer.State = OpCollection.Sequencer.EState.WaitingForReward;
                        }
                        break;
                    }
                case OpCollection.Sequencer.EState.WaitingForReward:
                    //Feed待ち
                    {
                        // 停止系
                        {
                            OpCollection.ECommand command = opCollection.Command;
                            if (command == OpCollection.ECommand.EmergencyStop) { opCollection.sequencer.State = OpCollection.Sequencer.EState.EmergencyStop; break; } // 緊急停止
                            if (command == OpCollection.ECommand.Stop) { opCollection.sequencer.State = OpCollection.Sequencer.EState.Stop; break; } // 中止
                        }
                        if (OpFlagRoomOut)
                        {
                            lastState = opCollection.sequencer.State;
                            opCollection.sequencer.State = OpCollection.Sequencer.EState.IllegalExitDetection;
                            break;
                        } // イリーガル退室検知

                        // 時間経過後給餌を実行
                        if (opCollection.stopwatch.Elapsed.TotalMilliseconds >= preferencesDatOriginal.OpeDelayFeed)
                        {
                            opCollection.stopwatch.Stop(); // ストップウォッチを停止

                            OpSetFeedOn(opCollection.trialCount, out opCollection.flagFeed); // 給餌を実行 

                            // イリーガル再入有効時はフィード直後に試行回数インクリメント後表示
                            {
                                if (preferencesDatOriginal.OpeEnableReEntry)
                                {
                                    opCollection.trialCount++; // 試行回数カウンタをインクリメント
                                    opCollection.callbackSetUiCurentNumberOfTrial(opCollection.trialCount);
                                }
                            }
                            opCollection.stopwatch.Restart(); // ストップウォッチをリスタート
                            opCollection.sequencer.State = OpCollection.Sequencer.EState.WaitingDelayForRoomLump;
                        }
                    }
                    break;

                case OpCollection.Sequencer.EState.WaitingDelayForRoomLump:
                    //フィード完了待ち
                    {
                        // 停止系
                        {
                            OpCollection.ECommand command = opCollection.Command;
                            if (command == OpCollection.ECommand.EmergencyStop) { opCollection.sequencer.State = OpCollection.Sequencer.EState.EmergencyStop; break; } // 緊急停止
                            if (command == OpCollection.ECommand.Stop) { opCollection.sequencer.State = OpCollection.Sequencer.EState.Stop; break; } // 中止
                        }
                        if (OpFlagRoomOut)
                        {
                            lastState = opCollection.sequencer.State;
                            opCollection.sequencer.State = OpCollection.Sequencer.EState.IllegalExitDetection;
                            break;
                        } // イリーガル退室検知

                        // 給餌が終わったらFeedランプ消灯
                        if (OpFlagFeedOn == true)
                        {
                            opCollection.stopwatch.Stop(); // ストップウォッチをリスタート
                            OpSetFeedLampOff(); // フィードランプをOFF
                            OpFlagFeedLampON = false;

                            opCollection.stopwatch.Restart(); // ストップウォッチをリスタート
                            opCollection.sequencer.State = OpCollection.Sequencer.EState.TurnOnRoomLump;
                        }
                    }
                    break;

                case OpCollection.Sequencer.EState.TurnOnRoomLump:
                    //ルームランプ点灯
                    {
                        // 停止系
                        {
                            OpCollection.ECommand command = opCollection.Command;
                            if (command == OpCollection.ECommand.EmergencyStop) { opCollection.sequencer.State = OpCollection.Sequencer.EState.EmergencyStop; break; } // 緊急停止
                            if (command == OpCollection.ECommand.Stop) { opCollection.sequencer.State = OpCollection.Sequencer.EState.Stop; break; } // 中止
                        }
                        if (OpFlagRoomOut)
                        {
                            lastState = opCollection.sequencer.State;
                            opCollection.sequencer.State = OpCollection.Sequencer.EState.IllegalExitDetection;
                            break;
                        } // イリーガル退室検知

                        // 給餌完了 給餌ランプOFF 報酬音声停止→ 時間経過
                        if (OpFlagFeedOn && !OpFlagFeedLampON && PlaySoundEnded && opCollection.stopwatch.Elapsed.TotalMilliseconds > preferencesDatOriginal.OpeDelayRoomLampOnTime)
                        {
                            opCollection.stopwatch.Stop(); // ストップウォッチをリスタート

                            OpSetRoomLampOn(); // 天井ランプをON

                            opCollection.stopwatch.Restart(); // ストップウォッチをリスタート
                            opCollection.sequencer.State = OpCollection.Sequencer.EState.WaitingForFeedComplete;
                        }
                    }
                    break;

                case OpCollection.Sequencer.EState.WaitingForFeedComplete:
                    // 給餌完了 ルームランプ点灯時間待ち
                    {
                        // 停止系
                        {
                            OpCollection.ECommand command = opCollection.Command;
                            if (command == OpCollection.ECommand.EmergencyStop) { opCollection.sequencer.State = OpCollection.Sequencer.EState.EmergencyStop; break; } // 緊急停止
                            if (command == OpCollection.ECommand.Stop) { opCollection.sequencer.State = OpCollection.Sequencer.EState.Stop; break; } // 中止
                        }
                        // 給餌後イリーガル検知させない
                        //if (OpFlagRoomOut)
                        //{
                        //    lastState = opCollection.sequencer.State;
                        //    opCollection.sequencer.State = OpCollection.Sequencer.EState.IlegalExitDetection;
                        //    break;
                        //} // イリーガル退室検知

                        // 給餌完了 ルームランプ点灯時間経過
                        if (OpFlagFeedOn == true && opCollection.stopwatch.Elapsed.TotalMilliseconds > 100)
                        {
                            if (!preferencesDatOriginal.OpeEnableReEntry)
                            {
                                opCollection.trialCount++; // 試行回数カウンタをインクリメント
                                opCollection.callbackSetUiCurentNumberOfTrial(opCollection.trialCount);
                            }

                            // 設定試行回数により終了
                            if (opCollection.trialCount >= preferencesDatOriginal.OpeNumberOfTrial && preferencesDatOriginal.OpeNumberOfTrial != 0)
                            {
                                opCollection.callbackMessageNormal(string.Format("設定試行回数({0}/{1})により給餌終了", opCollection.trialCount, preferencesDatOriginal.OpeNumberOfTrial));
                                opCollection.sequencer.callbackOutputFile(opCollection.sequencer.State); // ファイル出力
                                opCollection.sequencer.State = OpCollection.Sequencer.EState.PreOpenDoorProc;
                            }
                            // 入室報酬用
                            else if (OpeGetTypeOfTask() == ECpTask.UnConditionalFeeding)
                            {
                                opCollection.callbackMessageNormal(string.Format("入室給餌終了"));
                                opCollection.sequencer.callbackOutputFile(opCollection.sequencer.State); // ファイル出力
                                opCollection.sequencer.State = OpCollection.Sequencer.EState.PreOpenDoorProc;
                            }
                            // 試行継続
                            else
                            {
                                opCollection.callbackMessageNormal(string.Format("給餌完了試行継続({0}/{1})", opCollection.trialCount, preferencesDatOriginal.OpeNumberOfTrial));
                                opCollection.sequencer.State = OpCollection.Sequencer.EState.InitInterval;
                            }

                        }
                        break;
                    }

                case OpCollection.Sequencer.EState.InitInterval:
                    // インターバル初期化
                    {
                        // 停止系
                        {
                            OpCollection.ECommand command = opCollection.Command;
                            if (command == OpCollection.ECommand.EmergencyStop) { opCollection.sequencer.State = OpCollection.Sequencer.EState.EmergencyStop; break; } // 緊急停止
                            if (command == OpCollection.ECommand.Stop) { opCollection.sequencer.State = OpCollection.Sequencer.EState.Stop; break; } // 中止
                        }

                        {
                            opCollection.intervalTime = OpSubGetIntervalTime(); // インターバル値を取得
                            opCollection.beforeDelayMatchRandomTime = OpSubGetAfterMatchRandomTime();
                            opCollection.intervalTimeTotal += opCollection.intervalTime; // インターバル値合計を保存(結果ファイル出力用)
                            opCollection.intervalNum++; // インターバル回数をカウントし、保存(結果ファイル出力用)
                            opCollection.callbackMessageNormal(string.Format("インターバル開始(時間={0}[秒])", opCollection.intervalTime));
                            opCollection.stopwatch.Restart(); // ストップウォッチをリスタート
                            OpStartToTouchAnyOnTouchPanel(); // どこでもタッチ検知を開始
                            opCollection.sequencer.State = OpCollection.Sequencer.EState.IntervalProc;
                        }
                        break;
                    }
                case OpCollection.Sequencer.EState.IntervalProc:
                    // インターバル期間
                    {
                        // 停止系
                        {
                            OpCollection.ECommand command = opCollection.Command;
                            if (command == OpCollection.ECommand.EmergencyStop) { opCollection.sequencer.State = OpCollection.Sequencer.EState.EmergencyStop; break; } // 緊急停止
                            if (command == OpCollection.ECommand.Stop) { opCollection.sequencer.State = OpCollection.Sequencer.EState.Stop; break; } // 中止
                        }
                        if (OpFlagRoomOut)
                        {
                            lastState = opCollection.sequencer.State;
                            opCollection.sequencer.State = OpCollection.Sequencer.EState.IllegalExitDetection;
                            break;
                        } // イリーガル退室検知

                        // タイムアウト
                        if (opCollection.stopwatch.Elapsed.TotalSeconds >= opCollection.intervalTime)
                        {
                            // 画面タッチ
                            if (OpFlagTouchAnyOnTouchPanel == true)
                            {
                                opCollection.callbackMessageNormal("画面をタッチしたのでインターバル期間継続");
                                opCollection.sequencer.State = OpCollection.Sequencer.EState.InitInterval;
                            }
                            // 画面非タッチ
                            else
                            {
                                opCollection.callbackMessageNormal("画面タッチしなかったのでインターバル期間完了");
                                opCollection.sequencer.callbackOutputFile(opCollection.sequencer.State); // ファイル出力
                                OpEndToTouchAnyOnTouchPanel();
                                opCollection.sequencer.State = OpCollection.Sequencer.EState.PreTouchTriggerScreenProc;
                            }
                        }
                        break;
                    }
                #endregion 試行

                #region 退室
                case OpCollection.Sequencer.EState.PreOpenDoorProc:
                    // ドアOPEN前処理
                    {
                        OpDrawBackColorBlackOnTouchPanel(); // タッチパネル画面を黒
                        if (!preferencesDatOriginal.DisableDoor)
                        {
                            OpOpenDoor(); // ドアをOPEN
                        }

                        opCollection.sequencer.State = OpCollection.Sequencer.EState.WaitingForOpenDoor;
                        break;
                    }
                case OpCollection.Sequencer.EState.WaitingForOpenDoor:
                    // ドアOPEN待ち
                    {
                        // 停止系
                        {
                            OpCollection.ECommand command = opCollection.Command;
                            if (command == OpCollection.ECommand.EmergencyStop) { opCollection.sequencer.State = OpCollection.Sequencer.EState.EmergencyStop; break; } // 緊急停止
                            if (command == OpCollection.ECommand.Stop) { opCollection.sequencer.State = OpCollection.Sequencer.EState.Stop; break; } // 中止
                        }

                        if (OpFlagRoomOut)
                        {
                            lastState = opCollection.sequencer.State;
                            opCollection.sequencer.State = OpCollection.Sequencer.EState.IllegalExitDetection;
                            break;
                        } // イリーガル退室検知

                        // ドアOPEN動作完了
                        if (devInDoorOpen.dInCurrent == true)
                        {
                            // 結果正常
                            if (OpResultOpenDoor == EDeviceResult.Done)
                            {
                                opCollection.callbackMessageNormal("ドアOPEN正常");
                                opCollection.sequencer.State = OpCollection.Sequencer.EState.PreLeaveCageProc;
                            }
                            // 結果異常
                            else
                            {
                                opCollection.callbackMessageNormal("ドアOPEN異常");
                                opCollection.sequencer.State = OpCollection.Sequencer.EState.PreLeaveCageProc;
                            }
                        }

                        if (preferencesDatOriginal.DisableDoor && devInDoorOpen.dInCurrent == false)
                        {
                            opCollection.callbackMessageNormal("ドア無効停止待機");
                            opCollection.sequencer.State = OpCollection.Sequencer.EState.WaitingForLeaveCage;
                        }

                        break;
                    }
                case OpCollection.Sequencer.EState.PreLeaveCageProc:
                    // 退室前処理
                    {
                        // 停止系
                        {
                            OpCollection.ECommand command = opCollection.Command;
                            if (command == OpCollection.ECommand.EmergencyStop) { opCollection.sequencer.State = OpCollection.Sequencer.EState.EmergencyStop; break; } // 緊急停止
                            if (command == OpCollection.ECommand.Stop) { opCollection.sequencer.State = OpCollection.Sequencer.EState.Stop; break; } // 中止
                        }

                        {
                            OpSetRoomLampOff(); // 天井ランプをOFF
                            OpPlaySoundOfEnd(); // 終了音を再生
                            opCollection.stopwatch.Restart(); // ストップウォッチをリスタート
                            OpFlagRoomOut = false;
                            if (preferencesDatOriginal.DisableDoor)
                            {
                                opCollection.callbackMessageNormal(string.Format("退室タイムアウト有効(時間={0}[分])", preferencesDatOriginal.OpeTimeoutOfLeaveCage));
                            }

                            opCollection.sequencer.State = OpCollection.Sequencer.EState.WaitingForLeaveCage;
                        }
                        break;
                    }
                case OpCollection.Sequencer.EState.WaitingForLeaveCage:
                    // 退室待ち
                    {
                        // 停止系
                        {
                            OpCollection.ECommand command = opCollection.Command;
                            if (command == OpCollection.ECommand.EmergencyStop) { opCollection.sequencer.State = OpCollection.Sequencer.EState.EmergencyStop; break; } // 緊急停止
                            if (command == OpCollection.ECommand.Stop) { opCollection.sequencer.State = OpCollection.Sequencer.EState.Stop; break; } // 中止
                        }
                        // タイムアウト
                        if (opCollection.stopwatch.Elapsed.TotalMinutes >= preferencesDatOriginal.OpeTimeoutOfLeaveCage && preferencesDatOriginal.DisableDoor)
                        {
                            opCollection.callbackMessageNormal("退室タイムアウト");
                            opCollection.stopwatch.Stop(); // ストップウォッチを停止

                            opCollection.sequencer.State = OpCollection.Sequencer.EState.PreEnterCageProc;
                            break;
                        }

                        // 退室検知
                        if (OpFlagRoomOut == true)
                        {
                            opCollection.callbackMessageNormal("退室検知");
                            opCollection.stopwatch.Stop(); // ストップウォッチを停止
                            opCollection.sequencer.State = OpCollection.Sequencer.EState.PreEnterCageProc;
                        }

                        // ドア制御無効時 手動停止待ち
                        if (preferencesDatOriginal.DisableDoor)
                        {

                        }

                        break;
                    }
                case OpCollection.Sequencer.EState.IllegalExitDetection:
                    // イリーガル退室検知
                    {
                        opCollection.callbackMessageNormal("イリーガル退室検知: " + lastState.ToString());
                        opCollection.taskResultVal = OpCollection.ETaskResult.IllegalExit;
                        opCollection.dateTimeout = DateTime.Now;                             // タイムアウト？退出時刻を保存(結果ファイル出力用)
                        opCollection.sequencer.callbackOutputFile(opCollection.sequencer.State); // ファイル出力

                        OpFlagRoomIn = false;

                        OpDrawBackColorBlackOnTouchPanel(); // タッチパネル画面を黒
                                                            // 設定試行回数により終了

                        //イリーガル再入機能 切り替え
                        if (preferencesDatOriginal.OpeEnableReEntry)
                        {
                            // 直前ステートで戻る位置判断
                            // DelayMatchImage Target描画済み
                            // DelayBeforeMatch Target描画済み
                            // DelayMatchBackColor Target描画済み
                            // Training Target描画済み
                            // 描画済みの場合にのみ再開とする
                            //if (lastState == OpCollection.Sequencer.EState.DelayMatchImage ||
                            //    lastState == OpCollection.Sequencer.EState.DelayBeforeMatch ||
                            //    lastState == OpCollection.Sequencer.EState.DelayMatchBackColor ||
                            //    lastState == OpCollection.Sequencer.EState.Training
                            //    )

                            opCollection.stopwatch.Restart(); // ストップウォッチをリスタート
                            opCollection.sequencer.State = OpCollection.Sequencer.EState.IllegalExitDetection_WaitReEnter;

                            UpdateIllegalStatus();
                        }
                        else
                        {
                            opCollection.sequencer.State = OpCollection.Sequencer.EState.PreEnterCageProc;
                        }

                        // 退出時 設定試行回数より多ければ終了
                        // トライアル回数リセット
                        if (opCollection.trialCount >= preferencesDatOriginal.OpeNumberOfTrial && preferencesDatOriginal.OpeNumberOfTrial != 0)
                        {
                            opCollection.callbackMessageNormal(string.Format("設定試行回数({0}/{1})により給餌終了", opCollection.trialCount, preferencesDatOriginal.OpeNumberOfTrial));
                            opCollection.trialCount = 0;
                        }
                        break;
                    }
                #endregion 退室

                #region イリーガル退室後待ち
                case OpCollection.Sequencer.EState.IllegalExitDetection_WaitReEnter:
                    {
                        // 停止系
                        {
                            OpCollection.ECommand command = opCollection.Command;
                            if (command == OpCollection.ECommand.EmergencyStop) { opCollection.sequencer.State = OpCollection.Sequencer.EState.EmergencyStop; break; } // 緊急停止
                            if (command == OpCollection.ECommand.Stop) { opCollection.sequencer.State = OpCollection.Sequencer.EState.Stop; break; } // 中止
                        }

                        // 再入タイムアウト
                        if (opCollection.stopwatch.Elapsed.TotalSeconds >= preferencesDatOriginal.OpeReEntryTimeout)
                        {
                            opCollection.callbackMessageNormal("再入タイムアウト");
                            opCollection.stopwatch.Stop(); // ストップウォッチを停止
                            opCollection.sequencer.State = OpCollection.Sequencer.EState.PreEnterCageProc;

                            UpdateIllegalStatus();
                        }

                        // 入室検知 (ドア無効時 ドア手動閉追加)
                        if (OpFlagRoomIn == true || (preferencesDatOriginal.DisableDoor && (devInDoorClose.dInCurrent == true || devInDoorOpen.dInCurrent == false)))
                        {
                            MonitorPower.Monitor.PowerOn();
                            opCollection.callbackMessageNormal("入室検知");
                            opCollection.TimeEnterCage = DateTime.Now;
                            //opCollection.callbackSetUiCurentNumberOfTrial(opCollection.trialCount);


                            opCollection.idCode = OpeGetIdCode();
                            opCollection.callbackSetUiCurrentIdCode(opCollection.idCode);

                            //イリーガル再入室時はレバー制御しない（レバーINのまま）

                            OpFlagRoomOut = false;

                            opCollection.sequencer.State = OpCollection.Sequencer.EState.IllegalExitDetection_TrainingWait;

                            UpdateIllegalStatus();
                        }
                    }
                    break;

                case OpCollection.Sequencer.EState.IllegalExitDetection_TrainingWait:
                    {
                        opCollection.stopwatch.Restart(); // ストップウォッチをリスタート
                        //ライト類消灯
                        OpSetRoomLampOff();
                        OpSetFeedLampOff();
                        //フィード中であれば停止
                        OpSetFeedOff();
                        OpAirPuffOff();

                        // イリーガル再開分岐条件 正答表示から再開
                        // Trainingのみ課題表示から
                        switch (lastState)
                        {
                            case OpCollection.Sequencer.EState.DelayMatchImage:
                                OpStartToTouchCorrectOnTouchPanel(); // タッチコレクトを開始
                                opCollection.sequencer.State = OpCollection.Sequencer.EState.DelayBeforeMatch;
                                break;
                            case OpCollection.Sequencer.EState.DelayBeforeMatch:
                                //OpStartToTouchCorrectOnTouchPanel(); // タッチコレクトを開始
                                opCollection.sequencer.State = OpCollection.Sequencer.EState.DelayBeforeMatch;
                                break;
                            case OpCollection.Sequencer.EState.DelayMatchBackColor:
                                OpStartToTouchCorrectOnTouchPanel(); // タッチコレクトを開始
                                opCollection.sequencer.State = OpCollection.Sequencer.EState.DelayBeforeMatch;
                                break;
                            case OpCollection.Sequencer.EState.Training:
                                OpEndToTouchAnyOnTouchPanel(); // どこでもタッチを停止

                                switch (OpeGetTypeOfTask())
                                {
                                    case ECpTask.DelayMatch:
                                        OpStartToTouchCorrectOnTouchPanel(); // タッチコレクトを開始
                                        opCollection.sequencer.State = OpCollection.Sequencer.EState.AfterIllegalExit_DelayMatchImage;
                                        break;
                                    case ECpTask.Training:
                                        OpStartToTouchCorrectOnTouchPanel(); // タッチコレクトを開始
                                        OpDrawBackColorOnTouchPanel(); // タッチパネル画面を背景色でクリア
                                        opCollection.sequencer.State = OpCollection.Sequencer.EState.AfterIllegalExit_Training;
                                        break;
                                }
                                break;
                            case OpCollection.Sequencer.EState.WaitingForReward:
                                opCollection.sequencer.State = OpCollection.Sequencer.EState.DelayFeed;
                                break;
                            case OpCollection.Sequencer.EState.WaitingForTouchTriggerScreen:
                                opCollection.sequencer.State = OpCollection.Sequencer.EState.PreTouchTriggerScreenProc;
                                break;
                            default:
                                opCollection.sequencer.State = OpCollection.Sequencer.EState.PreTouchTriggerScreenProc;
                                break;
                        }
                        opCollection.callbackMessageNormal("イリーガル後再入室検知: " + opCollection.sequencer.State.ToString());
                    }
                    break;
                #endregion

                case OpCollection.Sequencer.EState.Error:
                    {
                        opCollection.callbackMessageError("エラーステート");
                        break;
                    }
                default:
                    {
                        opCollection.callbackMessageError("デフォルトステート");
                        break;
                    }
            }
        }
    }
}
#endregion
