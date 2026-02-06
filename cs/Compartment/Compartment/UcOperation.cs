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
                System.Diagnostics.Debug.WriteLine("=== START BUTTON CLICKED ===");
                System.Diagnostics.Debug.WriteLine($"Current State: {opCollection.sequencer.State}");
                System.Diagnostics.Debug.WriteLine($"Current IsBusy: {opCollection.IsBusy.Value}");
                System.Diagnostics.Debug.WriteLine($"EnableDebugMode: {preferencesDatOriginal.EnableDebugMode}");
                System.Diagnostics.Debug.WriteLine($"backgroundWorker1.IsBusy: {backgroundWorker1.IsBusy}");

                // PsychoPyエンジン時はスクリプト選択必須
                if (Program.SelectedEngine == EEngineType.PsychoPy
                    && string.IsNullOrEmpty(Program.PsychoPyScriptPath))
                {
                    System.Windows.Forms.MessageBox.Show(
                        "Pythonスクリプトが選択されていません。\nScript...ボタンからスクリプトを選択してください。",
                        "エラー", System.Windows.Forms.MessageBoxButtons.OK,
                        System.Windows.Forms.MessageBoxIcon.Warning);
                    return;
                }

                if (!backgroundWorker1.IsBusy)
                {
                    System.Diagnostics.Debug.WriteLine("Starting backgroundWorker1...");
                    backgroundWorker1.RunWorkerAsync();
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("backgroundWorker1 is already running!");
                }

                setCommand(OpCollection.ECommand.Start);

                // 注意：opCollection.Command は読み取ると Nop にリセットされるため、診断出力で読み取らない
                System.Diagnostics.Debug.WriteLine($"IsBusy after setCommand: {opCollection.IsBusy.Value}");
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

                    // 開始を促すメッセージボックス（無効化）
                    // const bool isShowStartOperationMessageBox = false;
                    // if (isShowStartOperationMessageBox == true)
                    // {
                    //     if (MessageBox.Show("Start operation ?", "Infomation", MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                    //     {
                    //         setCommand(OpCollection.ECommand.Start);
                    //     }
                    //     else
                    //     {
                    //         return;
                    //     }
                    // }
                    // else
                    // {
                    //     // setCommand(OpCollection.ECommand.Start);
                    // }
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
                //エンジンモードに応じて表示
                switch (Program.SelectedEngine)
                {
                    case EEngineType.BlockProgramming:
                        userControlOperationOnFormMain.textBoxSettingTypeOfTaskOnUcOperation.Text = "Block Programming";
                        break;
                    case EEngineType.PsychoPy:
                        userControlOperationOnFormMain.textBoxSettingTypeOfTaskOnUcOperation.Text = "PsychoPy";
                        break;
                    default:
                        userControlOperationOnFormMain.textBoxSettingTypeOfTaskOnUcOperation.Text = preferencesDatOriginal.OpeTypeOfTask;
                        break;
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

        // 旧エンジンの状態機械 OnOperationStateMachineProc() は削除済み
        // Block: UcOperationInternal.cs, PsychoPy: UcOperationPsychoPy.cs を使用
    }
}
#endregion
