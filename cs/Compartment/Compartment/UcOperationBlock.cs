using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Compartment
{

    public class UcOperationBlock : IDisposable
    {
        readonly ParentHelper<FormMain> mainForm = new ParentHelper<FormMain>();

        readonly Random random;
        private ref PreferencesDat PreferencesDatOriginal
        {
            get
            {
                return ref mainForm.Parent.preferencesDatOriginal;
            }
        }
        private ref OpCollection opCollection => ref mainForm.Parent.opCollection;

        // Unused field - commented out to remove warning
        // private readonly OpCollection.Sequencer.EState lastState = OpCollection.Sequencer.EState.Init;
        private ConcurrentQueue<Action> ProcQueue = new ConcurrentQueue<Action>();
        private Action TimeoutProc = () => { };
        /// <summary>
        /// 積まれたActionをLoopにて自動実行
        /// </summary>
        private Action BlockLoopProc = () =>
        {

        };
        public Action OperationProc = () =>
        {

        };

        public ConcurrentDictionary<string, Action> OperationProcIds = new ConcurrentDictionary<string, Action>();

        private ActionParam[] RegistertActionParam;

        public struct ActionParam
        {
            public string ActionName { get; set; }
            public int param1 { get; set; }
            public int param2 { get; set; }
            public object param3 { get; set; }
        }

        private void InitProcLoop()
        {
            BlockLoopProc = () =>
            {
                while (!ProcQueue.IsEmpty && ProcQueue.Count > 0)
                {
                    if (!ProcQueue.IsEmpty)
                    {
                        ProcQueue.TryDequeue(out Action act);
                        act();
                    }
                }
            };
        }
        public void Dispose()
        {

        }

        public UcOperationBlock(FormMain baseForm)
        {
            mainForm.SetParent(baseForm);
            random = new Random();
            //opCollection = mainForm.Parent.opCollection;

            // Default Proc
            // 終了時JSONあればロード
            OperationProc = () =>
            {
                DrawScreenReset();

                ViewTriggerImage();
                WaitTouchTrigger();

                DrawScreenReset();
                Delay(100);
                PlaySound();
                Delay(500);
                FeedLamp(2000, 0);
                //Delay(1500);
                //FeedLampAndSound(1500, 1500);
                Delay(500);
                FeedSound(1500, 0);
                Feed(2000, 0);

                OutputResult();
                TouchDelay(1000, 0);
                Start();
            };
            RegisterBlockFunc();
        }

        static Action<int, int> FeedAction;
        static Action<int, int> FeedLampAction;
        static Action<int, int> FeedLampAndSoundAction;
        static Action<int, int> DrawScreenResetAction;
        static Action<int, int> DrawScreenBackColorAction;
        static Action<int, int> DrawScreenDelayColorAction;
        static Action<int, int> WaitTouchTriggerAction;
        static Action<int, int, string> ViewTriggerImageAction;
        static Action<int, int, string> ViewCorrectImageAction;
        static Action<int, int> WaitCorrectTouchTriggerAction;
        static Action<int, int> ViewCorrectWrongImageAction;
        static Action<int, int, string> FeedSoundAction;
        static Action<int, int, string> UserPlaySoundAction;
        static Action<int, int> PlaySoundAction;
        static Action<int, int> PlayEndSoundAction;
        static Action<int, int> OutputResultAction;
        static Action<int, int> DelayAction;
        static Action<int, int> DelayEpisodeAction;
        static Action<int, int> TouchDelayAction;
        static Action<int, int> TouchDelayWithLampAction;
        static Action<int, int> TouchDelayEndingAction;
        static Action<int, int> InitStartingAction;
        static Action<int, int> MatchBeforeDelayAction;

        DictAction ProgramableActionList = new DictAction();
        public void RegisterBlockFunc()
        {
            //登録時に引数2個の処理を行う？
            FeedAction = (x, y) =>
            {
                this.Feed(x, y);
            };

            FeedLampAction = (x, y) =>
            {
                this.FeedLamp(x, y);
            };
            FeedLampAndSoundAction = (x, y) =>
            {
                this.FeedLampAndSound(x, y);
            };
            Action<int, int> InitializeAction = (x, y) => { Initialize(); };
            Action<int, int> StartAction = (x, y) => { Start(); };

            DrawScreenResetAction = (x, y) =>
            {
                this.DrawScreenReset();
            };

            DrawScreenBackColorAction = (x, y) =>
            {
                this.DrawScreenBackColor();
            };

            DrawScreenDelayColorAction = (x, y) =>
            {
                this.DrawScreenDelayBackColor();
            };

            WaitTouchTriggerAction = (x, y) =>
            {
                this.WaitTouchTrigger();
            };

            ViewTriggerImageAction = (x, y, path) =>
            {
                this.ViewTriggerImage(path);
            };

            WaitCorrectTouchTriggerAction = (x, y) =>
            {
                this.WaitCorrectTouchTrigger();
            };

            ViewCorrectImageAction = (x, y, path) =>
            {
                if (path is null)
                {
                    this.ViewCorrectImage();
                }
                else
                {
                    this.ViewCorrectImage(path);
                }
            };

            ViewCorrectWrongImageAction = (x, y) =>
            {
                this.ViewCorrectWrongImage();
            };

            FeedSoundAction = (x, y, path) =>
            {
                this.FeedSound(x, y);
            };

            UserPlaySoundAction = (x, y, path) =>
            {
                this.PlaySound(path);
            };

            PlaySoundAction = (x, y) =>
            {
                this.PlaySound();
            };

            PlayEndSoundAction = (x, y) =>
            {
                this.PlayEndSound();
            };

            OutputResultAction = (x, y) =>
            {
                this.OutputResult();
            };

            DelayAction = (x, y) =>
            {
                this.Delay(x, y);
            };

            DelayEpisodeAction = (x, y) =>
            {
                this.DelayEpisode(x, y);
            };

            TouchDelayAction = (x, y) =>
            {
                this.TouchDelay(x, y);
            };
            TouchDelayWithLampAction = (x, y) =>
            {
                this.TouchDelayWithLamp(x, y);
            };

            TouchDelayEndingAction = (x, y) =>
            {
                this.TouchDelayEnding(x, y);
            };

            InitStartingAction = (x, y) =>
            {
                this.InitFeedingFlag();
                this.InitEpisodeStatus();
                this.ViewEpisodeStatus();
            };

            MatchBeforeDelayAction = (x, y) =>
            {
                this.MatchBeforeDelay(x, y);
            };

            //DictActionに登録 Initializeで登録するようにする
            ProgramableActionList.SetAction(FeedAction, "Feed");
            ProgramableActionList.SetAction(FeedLampAction, "FeedLamp");
            //ランプサウンド時ランプ消灯しないことがあるのでいったん廃止
            //ProgramableActionList.SetAction(FeedLampAndSoundAction, "FeedLampAndSound");
            ProgramableActionList.SetAction(DrawScreenResetAction, "DrawScreenReset");
            ProgramableActionList.SetAction(DrawScreenBackColorAction, "DrawScreenColor");
            ProgramableActionList.SetAction(DrawScreenDelayColorAction, "DrawScreenDelayColor");
            ProgramableActionList.SetAction(WaitTouchTriggerAction, "WaitTouchTrigger");
            ProgramableActionList.SetAction(WaitCorrectTouchTriggerAction, "WaitCorrectTouchTrigger");
            ProgramableActionList.SetAction(ViewTriggerImageAction, "ViewTriggerImage");
            ProgramableActionList.SetAction(ViewCorrectWrongImageAction, "ViewCorrectWrongImage");
            ProgramableActionList.SetAction(ViewCorrectImageAction, "ViewCorrectImage");
            ProgramableActionList.SetAction(FeedSoundAction, "FeedSound");
            ProgramableActionList.SetAction(PlaySoundAction, "PlaySound");
            ProgramableActionList.SetAction(UserPlaySoundAction, "UserPlaySound");
            ProgramableActionList.SetAction(PlayEndSoundAction, "PlayEndSound");
            ProgramableActionList.SetAction(InitializeAction, "Initialize");
            ProgramableActionList.SetAction(StartAction, "Start");
            ProgramableActionList.SetAction(OutputResultAction, "OutputResult");
            ProgramableActionList.SetAction(DelayAction, "Delay");
            ProgramableActionList.SetAction(DelayEpisodeAction, "DelayEpisode");
            ProgramableActionList.SetAction(TouchDelayAction, "TouchDelay");
            ProgramableActionList.SetAction(TouchDelayWithLampAction, "TouchDelayWithLamp");
            ProgramableActionList.SetAction(TouchDelayEndingAction, "TouchDelayEnding");
            ProgramableActionList.SetAction(InitStartingAction, "InitStarting");
            ProgramableActionList.SetAction(MatchBeforeDelayAction, "MatchBeforeDelay");

            // 登録ActionをInternal側で初期化で流し込む
            //SetStateFunction(a);
            //OperationProc = () =>
            //{
            //    //ProgramableActionList.GetAction("ViewTriggerImage")();
            //    //ProgramableActionList.GetAction("WaitTouchTrigger")();
            //    //ProgramableActionList.GetAction("FeedAction", 5000)();
            //    //ProgramableActionList.GetAction("DrawScreenResetAction", 0)();
            //    //ProgramableActionList.GetAction("FeedLampAction", 5000)();
            //    //ProgramableActionList.GetAction("PlaySound")();
            //    //ProgramableActionList.GetAction("OutputResult")();
            //    //ProgramableActionList.GetAction("Start", 1)();
            //};
        }

        public static ActionParam[] GenerateDefaultActionParams()
        {
            ActionParam[] actionParams = new ActionParam[18];
            actionParams[0] = new ActionParam { ActionName = "DrawScreenReset" };
            actionParams[1] = new ActionParam { ActionName = "ViewTriggerImage" };
            actionParams[2] = new ActionParam { ActionName = "WaitTouchTrigger" };
            actionParams[3] = new ActionParam { ActionName = "DrawScreenReset" };
            actionParams[4] = new ActionParam { ActionName = "Delay" };
            actionParams[5] = new ActionParam { ActionName = "ViewCorrectImage" };
            actionParams[6] = new ActionParam { ActionName = "Delay" };
            actionParams[7] = new ActionParam { ActionName = "DrawScreenReset" };
            actionParams[8] = new ActionParam { ActionName = "Delay" };
            actionParams[9] = new ActionParam { ActionName = "ViewCorrectWrongImage" };
            actionParams[10] = new ActionParam { ActionName = "WaitCorrectTouchTrigger" };
            actionParams[11] = new ActionParam { ActionName = "DrawScreenReset" };
            actionParams[12] = new ActionParam { ActionName = "PlaySound" };
            actionParams[13] = new ActionParam { ActionName = "FeedSound" };
            actionParams[14] = new ActionParam { ActionName = "FeedLamp" };
            actionParams[15] = new ActionParam { ActionName = "Feed" };
            actionParams[16] = new ActionParam { ActionName = "OutputResult" };
            actionParams[17] = new ActionParam { ActionName = "TouchDelay" };

            return actionParams;

            //FunctionAdd("DrawScreenReset");
            //FunctionAdd("ViewTriggerImage");
            //FunctionAdd("WaitTouchTrigger");
            //FunctionAdd("DrawScreenReset");
            //FunctionAdd("Delay");
            //FunctionAdd("ViewCorrectImage");
            //FunctionAdd("Delay");
            //FunctionAdd("DrawScreenReset");
            //FunctionAdd("Delay");
            //FunctionAdd("ViewCorrectWrongImage");
            //FunctionAdd("WaitCorrectTouchTrigger");
            //FunctionAdd("DrawScreenReset");
            //FunctionAdd("PlaySound");
            //FunctionAdd("FeedSound");
            //FunctionAdd("FeedLamp");
            //FunctionAdd("Feed");
            //FunctionAdd("OutputResult");
            //FunctionAdd("TouchDelay");
        }
        public static string GetDefaultActionParamJson()
        {
            var actionParams = GenerateDefaultActionParams();
            string json = JsonConvert.SerializeObject(actionParams);
            return json;
        }

        public bool CheckRegisterAction(ActionParam[] actionParams)
        {
            bool ret = false;
            if (actionParams is null)
            {
                return ret;
            }
            try
            {
                ret = true;
                for (int i = 0; i < actionParams?.Length; i++)
                {
                    _ = ProgramableActionList.GetAction(actionParams[i].ActionName);
                    CheckPathParameter(ref actionParams[i]);
                }
            }
            catch (KeyNotFoundException)
            {
                ret = false;
            }
            catch (Exception)
            {
                throw;
            }
            return ret;
        }

        public bool CheckRegisterActionWithoutException(ActionParam[] actionParams)
        {
            bool ret = false;
            try
            {
                ret = CheckRegisterAction(actionParams);
                return ret;
            }
            catch (Exception)
            {
                return ret;
            }
        }

        private Action OldOperationProc;

        public void RegisterOperationProc(string id, ActionParam[] actionParams)
        {
            if (actionParams is null)
            {
                return;
            }

            try
            {
                OperationProcIds.TryGetValue(id, out var action);
                //直前保存
                if (action != null)
                {
                    OldOperationProc = new Action(action);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }


            //文法チェック用読み込みLineカウンタ
            int lineCount = 0;

            try
            {
                RegistertActionParam = actionParams;

                // check用に一回読み出し
                // エラー行数抽出
                foreach (ActionParam n in actionParams)
                {
                    lineCount++;
                    var a = ProgramableActionList.GetAction(n.ActionName);

                }
                // パスチェック
                if (CheckRegisterAction(RegistertActionParam))
                {

                }
                else
                {
                    //throw new Exception("file path error");
                }
                Action operationProc = new Action(() =>
                    {
                        InitProcLoop();
                        BlockLoopProc();
                    }
                );
                //最初にInitializeを追加
                operationProc += () => { (ProgramableActionList.GetAction("Initialize") as Action<int, int>)?.Invoke(0, 0); };
                operationProc += () => { (ProgramableActionList.GetAction("InitStarting") as Action<int, int>)?.Invoke(0, 0); };

                foreach (ActionParam n in actionParams)
                {
                    //ID毎にパス指定できないので廃止
                    //SetPathParameter(n);
                    operationProc += () =>
                    {
                        var act = ProgramableActionList.GetAction(n.ActionName);
                        if (act is Action<int, int> action)
                        {
                            action(n.param1, n.param2);
                        }
                        else if (act is Action<int, int, string> action2)
                        {
                            action2(n.param1, n.param2, (string)n.param3);
                        }
                    };
                }
                //最後にStartを追加
                operationProc += () => { (ProgramableActionList.GetAction("Start") as Action<int, int>)?.Invoke(0, 0); };
                OperationProcIds.AddOrUpdate(id, operationProc, (_, value) => operationProc);
            }
            catch (KeyNotFoundException ex)
            {
                //文法チェック
                Debug.WriteLine(ex.Message);

                //operationProc = OldOperationProc;
                throw new Exception(string.Format("Line:{0} error.", lineCount));
            }
            catch (Exception ex)
            {
                //operationProc = OldOperationProc;
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public void RegisterOperationProc(ActionParam[] actionParams)
        {
            if (actionParams is null)
            {
                return;
            }

            //直前保存
            OldOperationProc = new Action(OperationProc);

            //OperationProc初期化
            OperationProc = () => { };

            //文法チェック用読み込みLineカウンタ
            int lineCount = 0;

            try
            {
                RegistertActionParam = actionParams;

                OperationProc = () => { };

                // check用に一回読み出し
                // エラー行数抽出
                foreach (ActionParam n in actionParams)
                {
                    lineCount++;
                    var a = ProgramableActionList.GetAction(n.ActionName);

                }
                // パスチェック
                if (CheckRegisterAction(RegistertActionParam))
                {

                }
                else
                {
                    //throw new Exception("file path error");
                }
                OperationProc += () =>
                {
                    InitProcLoop();
                    BlockLoopProc();
                };
                //最初にInitializeを追加
                OperationProc += () => { (ProgramableActionList.GetAction("Initialize") as Action<int, int>)?.Invoke(0, 0); };
                OperationProc += () => { (ProgramableActionList.GetAction("InitStarting") as Action<int, int>)?.Invoke(0, 0); };

                foreach (ActionParam n in actionParams)
                {
                    SetPathParameter(n);
                    OperationProc += () =>
                    {
                        var act = ProgramableActionList.GetAction(n.ActionName);
                        if (act is Action<int, int> action)
                        {
                            action(n.param1, n.param2);
                        }
                        else if (act is Action<int, int, string> action2)
                        {
                            action2(n.param1, n.param2, (string)n.param3);
                        }
                    };
                }
                //最後にStartを追加
                OperationProc += () => { (ProgramableActionList.GetAction("Start") as Action<int, int>)?.Invoke(0, 0); };
            }
            catch (KeyNotFoundException ex)
            {
                //文法チェック
                Debug.WriteLine(ex.Message);

                OperationProc = OldOperationProc;
                throw new Exception(string.Format("Line:{0} error.", lineCount));
            }
            catch (Exception ex)
            {
                OperationProc = OldOperationProc;
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        private void CheckPathParameter(ref ActionParam actionParam)
        {
            // ファイルパス有効性確認する
            switch (actionParam.ActionName)
            {
                case "ViewTriggerImage":
                    if (actionParam.param3 is null || (string)actionParam.param3 == "")
                    {
                        actionParam.param3 = PreferencesDatOriginal.TriggerImageFile;
                    }
                    else
                    {
                        if (!System.IO.File.Exists((string)actionParam.param3))
                        {
                            //path file無効時
                            throw new System.IO.FileNotFoundException((string)actionParam.param3 + "が存在しません");
                        }
                    }
                    break;
                case "ViewCorrectImage":
                    if (actionParam.param3 is null || (string)actionParam.param3 == "")
                    {
                        actionParam.param3 = PreferencesDatOriginal.CorrectImageFile;
                    }
                    else
                    {
                        if (!System.IO.File.Exists((string)actionParam.param3))
                        {
                            //path file無効時
                            throw new System.IO.FileNotFoundException((string)actionParam.param3 + "が存在しません");
                        }
                    }
                    break;
                case "ViewCorrectWrongImage":
                    break;
                case "PlaySound":
                    if (actionParam.param3 is null || (string)actionParam.param3 == "")
                    {
                        actionParam.param3 = PreferencesDatOriginal.SoundFileOfCorrect;
                    }
                    else
                    {
                        if (!System.IO.File.Exists((string)actionParam.param3))
                        {
                            //path file無効時
                            throw new System.IO.FileNotFoundException((string)actionParam.param3 + "が存在しません");
                        }
                    }
                    break;
                case "FeedSound":
                    if (actionParam.param3 is null || (string)actionParam.param3 == "")
                    {
                        actionParam.param3 = PreferencesDatOriginal.SoundFileOfReward;
                    }
                    else
                    {
                        if (!System.IO.File.Exists((string)actionParam.param3))
                        {
                            //path file無効時
                            throw new System.IO.FileNotFoundException((string)actionParam.param3 + "が存在しません");
                        }
                    }
                    break;
            }
            if (System.IO.File.Exists((string)actionParam.param3))
            {
                //throw new Exception("Sound file not found.");
            }
        }

        private void SetPathParameter(ActionParam actionParam)
        {
            if (actionParam.param3 is string param3)
            {
                Debug.WriteLine(param3);


                switch (actionParam.ActionName)
                {
                    case "ViewTriggerImage":
                        PreferencesDatOriginal.TriggerImageFile = param3;
                        break;
                    case "ViewCorrectImage":
                        PreferencesDatOriginal.CorrectImageFile = param3;
                        PreferencesDatOriginal.ImageFileFolder = System.IO.Path.GetDirectoryName(param3);
                        break;
                    case "ViewCorrectWrongImage":
                        break;
                    case "PlaySound":
                        PreferencesDatOriginal.SoundFileOfCorrect = param3;
                        break;
                    case "FeedSound":
                        PreferencesDatOriginal.SoundFileOfReward = param3;
                        break;
                }
            }
        }

        public void UndoOperationProc()
        {
            OperationProc = OldOperationProc;
        }
        /// <summary>
        /// index指定プロトタイプ
        /// </summary>
        /// <param name="id"></param>
        /// <param name="actionParamsStrings"></param>
        public void JsonToOperationProc(string id, string actionParamsStrings)
        {
            if (string.IsNullOrEmpty(actionParamsStrings))
            {
                throw new ArgumentException($"'{nameof(actionParamsStrings)}' を NULL または空にすることはできません。", nameof(actionParamsStrings));
            }

            ActionParam[] deser = JsonConvert.DeserializeObject<ActionParam[]>(actionParamsStrings);
            // パスチェック
            if (CheckRegisterAction(deser))
            {

            }
            else
            {
                //throw new Exception("File path error");
            }
            RegisterOperationProc(id, deser);
        }
        public void JsonToOperationProc(string str)
        {
            ActionParam[] deser = JsonConvert.DeserializeObject<ActionParam[]>(str);
            // パスチェック
            if (CheckRegisterAction(deser))
            {

            }
            else
            {
                //throw new Exception("File path error");
            }
            RegisterOperationProc(deser);
        }

        public string JsonToActionParams(string json)
        {
            string ret = "";
            try
            {
                ActionParam[] deser = JsonConvert.DeserializeObject<ActionParam[]>(json);
                ret = JsonConvert.SerializeObject(deser);
            }
            catch (KeyNotFoundException ex)
            {
                //文法チェック
                Debug.WriteLine(ex.Message);
            }
            catch (System.IO.IOException ex)
            {
                //文法チェック
                Debug.WriteLine(ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
            return ret;
        }
        /// <summary>
        /// JSONからActionParamを取得
        /// </summary>
        /// <returns></returns>
        public string GetJsonRegisterActionParam()
        {
            if (RegistertActionParam != null)
            {
                string output = JsonConvert.SerializeObject(RegistertActionParam);
                if (CheckRegisterActionWithoutException(RegistertActionParam))
                    return output;
                else
                    return "";
            }
            return "";
        }
        /// <summary>
        /// JSONからActionParam取得 ID指定
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string GetJsonRegisterActionParam(string id)
        {
            if (RegistertActionParam != null)
            {
                string output = JsonConvert.SerializeObject(RegistertActionParam);
                if (CheckRegisterActionWithoutException(RegistertActionParam))
                {
                    return output;
                }
                else
                    return "";
            }
            return "";
        }

        public void OpenPreferenceJson(string path)
        {
            var json = System.IO.File.ReadAllText(path);
            try
            {

                if (json != "")
                {
                    PreferencesDat deser = JsonConvert.DeserializeObject<PreferencesDat>(json);
                    mainForm.Parent.preferencesDatOriginal = deser;
                }
            }
            catch (Exception)
            {

                throw;
            }

        }
        //public void OpenPreferenceAndProcJson(string path)
        //{
        //    var json = System.IO.File.ReadAllText(path); try
        //    {

        //        if (json != "")
        //        {
        //            (PreferencesDat, ActionParam[]) deser = JsonConvert.DeserializeObject<(PreferencesDat, ActionParam[])>(json);
        //            mainForm.Parent.preferencesDatOriginal = deser.Item1;
        //            RegisterOperationProc(deser.Item2);
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }

        //}
        //public void SavePreferenceAndProcJson(string path)
        //{
        //    string savePath = path;
        //    try
        //    {
        //        JsonSerializer serializer = new JsonSerializer();
        //        if (!CheckRegisterAction(RegistertActionParam))
        //            return;
        //        //serializer.TypeNameHandling = TypeNameHandling.Arrays;
        //        serializer.NullValueHandling = NullValueHandling.Ignore;

        //        //上書き設定
        //        using (System.IO.StreamWriter sw = new System.IO.StreamWriter(savePath, false))
        //        using (JsonWriter writer = new JsonTextWriter(sw))
        //        {
        //            //初期状態OperationProcは同じ その状態のRegisterActionParamはNULLなので無視
        //            if (RegistertActionParam != null)
        //                serializer.Serialize(writer, (mainForm.Parent.preferencesDatOriginal, RegistertActionParam));
        //        }
        //    }
        //    catch (KeyNotFoundException ex)
        //    {
        //        //文法チェック
        //        Debug.WriteLine(ex.Message);
        //    }
        //    catch (System.IO.IOException ex)
        //    {
        //        //文法チェック
        //        Debug.WriteLine(ex.Message);
        //        throw;
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine(ex.Message);
        //        throw;
        //    }

        //}

        public void SavePreferenceJson(string path)
        {
            string savePreferencePath = path;
            try
            {
                JsonSerializer serializer = new JsonSerializer();

                //serializer.TypeNameHandling = TypeNameHandling.Arrays;
                serializer.NullValueHandling = NullValueHandling.Ignore;

                //上書き設定
                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(savePreferencePath, false))
                using (JsonWriter writer = new JsonTextWriter(sw))
                {
                    //初期状態OperationProcは同じ その状態のRegisterActionParamはNULLなので無視
                    if (RegistertActionParam != null)
                        serializer.Serialize(writer, mainForm.Parent.preferencesDatOriginal);
                }
            }
            catch (KeyNotFoundException ex)
            {
                //文法チェック
                Debug.WriteLine(ex.Message);
            }
            catch (System.IO.IOException ex)
            {
                //文法チェック
                Debug.WriteLine(ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        /// <summary>
        /// ユーザ操作におけるReference用のレシピデータを保存
        /// </summary>
        /// <param name="path"></param>
        public void OperationProcToJson(string path)
        {
            string latestJsonFilePath = path;
            try
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.NullValueHandling = NullValueHandling.Ignore;

                if (!CheckRegisterAction(RegistertActionParam))
                    return;
                //上書き
                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(latestJsonFilePath, false))
                using (JsonWriter writer = new JsonTextWriter(sw))
                {
                    //初期状態OperationProcは同じ その状態のRegisterActionParamはNULLなので無視
                    if (RegistertActionParam != null)
                        serializer.Serialize(writer, RegistertActionParam);
                }
            }
            catch (KeyNotFoundException ex)
            {
                //文法チェック
                Debug.WriteLine(ex.Message);
            }
            catch (System.IO.IOException ex)
            {
                //文法チェック
                Debug.WriteLine(ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        //** ↓2025/01 多層化対応↓ **//
        /// <summary>
        /// エンジン停止時に使用するidに紐づいたレシピデータを保存
        /// </summary>
        /// <param name="id"></param>
        /// <param name="path"></param>
        public void OperationProcToJson(string id, string path)
        {
            string latestJsonFilePath = path;
            try
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.NullValueHandling = NullValueHandling.Ignore;

                var fileRelatedActionParam = mainForm.Parent.ucOperationDataStore.GetEntry(id);

                ActionParam[] deser = JsonConvert.DeserializeObject<ActionParam[]>(fileRelatedActionParam.ActionParams);
                RegistertActionParam = deser;

                if (!CheckRegisterAction(RegistertActionParam))
                    return;
                //上書き
                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(latestJsonFilePath, false))
                using (JsonWriter writer = new JsonTextWriter(sw))
                {
                    //初期状態OperationProcは同じ その状態のRegisterActionParamはNULLなので無視
                    if (RegistertActionParam != null)
                        serializer.Serialize(writer, RegistertActionParam);
                }
            }
            catch (KeyNotFoundException ex)
            {
                //文法チェック
                Debug.WriteLine(ex.Message);
            }
            catch (System.IO.IOException ex)
            {
                //文法チェック
                Debug.WriteLine(ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }
        //** ↑2025/01 多層化対応↑ **//

        public static string JsonToScript(string json)
        {
            ActionParam[] deser = JsonConvert.DeserializeObject<ActionParam[]>(json);
            string ret = "";
            if (deser is null)
            {
                return "";
            }
            foreach (ActionParam action in deser)
            {
                ret += action.ActionName;

                if (action.param1 != 0)
                {
                    ret += "(";
                    ret += action.param1.ToString();
                    if (action.param2 != 0)
                    {
                        ret += "-";
                        ret += action.param2.ToString();
                    }
                    if (action.param3 is null)
                    {
                        ret += "";
                    }
                    else
                    {
                        ret += ",";
                        ret += action.param3.ToString();
                    }

                    ret += ")";
                }
                else
                {
                }


                ret += "\r\n";
            }

            return ret;
        }

        public static string ScriptToJson(string str)
        {
            string[] strList = str.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            // 空白除外
            var list = strList.Where(o => o != "").ToArray();

            List<string> stringActions = new List<string>();
            List<Tuple<string, string>> stringParams = new List<Tuple<string, string>>();

            // パラメータ分割部
            foreach (string s in list)
            {
                if (Regex.IsMatch(s, @"\(\d+\)"))
                {
                    string[] ss = Regex.Split(s, @"\(");
                    //先頭側の文字列を採用
                    stringActions.Add(ss[0]);
                    stringParams.Add(new Tuple<string, string>(Regex.Match(s, @"\d+").ToString(), "0"));
                }
                else if (Regex.IsMatch(s, @"\(\d+-\d+\)"))
                {
                    string[] ss = Regex.Split(s, @"\(");
                    //先頭側の文字列を採用
                    stringActions.Add(ss[0]);
                    var convinedParam = Regex.Match(s, @"\d+-\d+").ToString();
                    string[] minmaxParam = Regex.Split(convinedParam, "-");
                    stringParams.Add(new Tuple<string, string>(minmaxParam[0], minmaxParam[1]));

                }
                else
                {
                    string CommandString = Regex.Match(s, @"[a-z]*", RegexOptions.IgnoreCase).ToString();
                    stringActions.Add(CommandString);
                    stringParams.Add(new Tuple<string, string>("0", "0"));
                }
                // , 以降path情報ならパス情報入れるようにする
            }

            ActionParam[] actionParam = new ActionParam[stringActions.Count];
            try
            {
                for (int i = 0; i < stringActions.Count; i++)
                {
                    actionParam[i] = new ActionParam();
                    actionParam[i].ActionName = stringActions[i];
                    //Parseエラー判定追加する
                    actionParam[i].param1 = int.Parse(stringParams.ElementAt(i).Item1);
                    actionParam[i].param2 = int.Parse(stringParams.ElementAt(i).Item2);

                    //path情報追加部
                }
            }
            catch (Exception)
            {
                throw;
            }

            string jsonScript = JsonConvert.SerializeObject(actionParam);

            return jsonScript;
        }

        /// <summary>
        /// プログラム可能部ステートマシン
        /// 内部にCodeBuilderを展開する
        /// </summary>
        public Action OnOperationStateProgrammableProc = () => { };

        public void SetStateFunction(Action act)
        {
            OnOperationStateProgrammableProc += () =>
            {
                act();
            };
        }

        private void InvokeMethod(Action act)
        {
            if (mainForm.Parent.InvokeRequired)
                mainForm.Parent.Invoke(new System.Windows.Forms.MethodInvoker(act));
            else
                act();
        }

        // block用カテゴリ

        #region For Block Function

        internal static int GetRandomTime(int min, int max)
        {
            int minTime;
            int maxTime;
            //globalじゃないから同じTick時同じ数値
            Random randomValue = new System.Random();

            minTime = min;
            if (min <= 0)
            {
                minTime = 0;
            }
            maxTime = max;
            if (max <= 0)
            {
                maxTime = 0;
            }
            // ださい
            if (minTime > maxTime)
            {
                maxTime = minTime;
            }
            int ret = randomValue.Next(minTime, maxTime + 1);
            return ret;
        }

        public bool Feeded { get => _Feeded.Value; set => _Feeded.Value = value; }

        private readonly SyncObject<bool> _Feeded = new SyncObject<bool>(false);

        private void IllegalExitDetection()
        {
            InvokeMethod(() =>
            {
                //ライト類消灯
                mainForm.Parent.OpSetRoomLampOff();
                mainForm.Parent.OpSetFeedLampOff();
                //フィード中であれば停止
                mainForm.Parent.OpSetFeedOff();
                mainForm.Parent.OpAirPuffOff();
            });
        }
        private void ExitDetection()
        {
            InvokeMethod(() =>
            {
                //ライト類消灯
                mainForm.Parent.OpSetRoomLampOff();
                mainForm.Parent.OpSetFeedLampOff();
                //フィード中であれば停止
                mainForm.Parent.OpSetFeedOff();
                mainForm.Parent.OpAirPuffOff();
                //画面も消す
                DrawScreenReset();
            });
        }
        #endregion


        #region Programable Section
        /// <summary>
        /// タッチ前初期化 TrigerTouchにより使用
        /// </summary>
        private void PreTouchTriggerProc()
        {
            //opCollection.callbackMessageNormal(string.Format("トリガ画面タッチ待ち(タイムアウト時間={0}[分])", PreferencesDatOriginal.OpeTimeoutOfTrial));

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
                mainForm.Parent.opImage.SetParamOfShapeOpeImage(
                    PreferencesDatOriginal.TypeOfShape,
                    PreferencesDatOriginal.SizeOfShapeInStep,
                    PreferencesDatOriginal.SizeOfShapeInPixel);
            }
        }

        /// <summary>
        /// スクリーントリガー完了
        /// mainForm.Parent.OpFlagTouchAnyOnTouchPanel == true
        /// 完了カウント
        /// </summary>
        private void CompleteTouchTriggerScreen()
        {
            opCollection.taskResultVal = OpCollection.ETaskResult.TriggerTouch;
            opCollection.dateTimeTriggerTouch = DateTime.Now; // トリガ・タッチ時刻を保存
            opCollection.stopwatch.Stop(); // ストップウォッチを停止
        }
        /// <summary>
        /// 正解完了
        /// </summary>
        private void CompleteTouchCorrectScreen()
        {
            opCollection.taskResultVal = OpCollection.ETaskResult.Ok;       // 結果: 正解を保存
            opCollection.dateTimeCorrectTouch = DateTime.Now; // 正解時刻時刻を保存
            opCollection.stopwatch.Stop(); // ストップウォッチを停止
        }
        /// <summary>
        /// 不正解完了
        /// </summary>
        private void CompleteTouchIncorrectScreen()
        {
            opCollection.taskResultVal = OpCollection.ETaskResult.IncorrectTouchNG;       // 結果: 不正解を保存 不正解を追加
            opCollection.dateTimeCorrectTouch = DateTime.Now; // 不正解時刻時刻を保存
            opCollection.stopwatch.Stop(); // ストップウォッチを停止
        }
        /// <summary>
        /// タッチトリガータイムアウト InternalWaitで使用
        /// </summary>
        private void TimeoutTouchTrigger()
        {
            opCollection.callbackMessageNormal("トリガ画面タッチタイムアウト");
            opCollection.stopwatch.Stop(); // ストップウォッチを停止
            InvokeMethod(() =>
            {
                mainForm.Parent.OpEndToTouchAnyOnTouchPanel(); // どこでもタッチを停止
            });
            opCollection.taskResultVal = OpCollection.ETaskResult.StartTimeout;       // 結果: 開始タイムアウトを保存(結果ファイル出力用)
            opCollection.dateTimeout = DateTime.Now;                                // タイムアウト時刻を保存(結果ファイル出力用)

            // Blockプログラム用ステート入力作ったほうがいいかも
            opCollection.sequencer.callbackOutputFile(OpCollection.Sequencer.EState.IntervalProc); // ファイル出力
            return;
        }


        /// <summary>
        /// Trainingタイムアウト処理
        /// </summary>
        private void TrainingTimeout()
        {
            // タイムアウト
            TimeoutProc = () =>
            {
                opCollection.stopwatch.Stop(); // ストップウォッチを停止
                InvokeMethod(() =>
                {
                    opCollection.callbackMessageNormal("訓練タイムアウト");
                    mainForm.Parent.OpEndToTouchCorrectOnTouchPanel(); // タッチコレクトを停止

                    //タイムアウト後処理 選択できるように？
                    {
                        //とりあえず黒
                        mainForm.Parent.OpDrawBackColorBlackOnTouchPanel();
                        //退出促す Operationステートマシン側で
                        //mainForm.Parent.OpPlaySoundOfEnd();
                    }
                });
                //PlaySound();
                opCollection.stopwatch.Restart();
                opCollection.sequencer.State = OpCollection.Sequencer.EState.PreLeaveCageProc;

                opCollection.taskResultVal = OpCollection.ETaskResult.TrialTimeout;       // 結果: 試行タイムアウトを保存(結果ファイル出力用)
                opCollection.dateTimeout = DateTime.Now;                             // タイムアウト時刻を保存(結果ファイル出力用)
                opCollection.sequencer.callbackOutputFile(OpCollection.Sequencer.EState.WaitingForTouchTriggerScreen); // ファイル出力

                BlockAction.ClearAction();
            };

        }

        public void DetectIllegalExit()
        {
            BlockAction.InterruptFlag = true;
        }

        /// <summary>
        /// インターバル処理 ブロックで使用
        /// </summary>
        //インターバル動作 タッチ状態分岐 ----------------------
        private void IntervalProc()
        {
            // タイムアウト
            TimeoutProc = () =>
            {
                // 画面タッチ
                if (mainForm.Parent.OpFlagTouchAnyOnTouchPanel == true)
                {
                    opCollection.callbackMessageNormal("画面をタッチしたのでインターバル期間継続");
                }
                // 画面非タッチ
                else
                {
                    opCollection.callbackMessageNormal("画面タッチしなかったのでインターバル期間完了");
                    //opCollection.sequencer.callbackOutputFile(OpCollection.Sequencer.EState.IntervalProc); // ファイル出力
                    mainForm.Parent.OpEndToTouchAnyOnTouchPanel();
                }
            };
        }
        #endregion

        #region ブロックシーケンス用

        Stopwatch sw = new Stopwatch();
        ShapeObject shapeObject = new ShapeObject();

        readonly OperationActionHelper BlockAction = new Compartment.OperationActionHelper();
        public bool OperationStatus { get => BlockAction.TaskBusy; }

        /// <summary>
        /// 継続フラグ
        /// BlockActionを繰り返し行う
        /// </summary>
        public bool Infinity { get => BlockAction.Infinity; set => BlockAction.Infinity = value; }

        /// <summary>
        /// 再入フラグ
        /// </summary>
        public bool ReEntry { get => BlockAction.ReEntryFlag; set => BlockAction.ReEntryFlag = value; }

        /// <summary>
        /// 背景リセット
        /// </summary>
        public void DrawScreenReset()
        {
            BlockAction.SetAction(
            () =>
            {
                InvokeMethod(() =>
                {
                    //黒画面リセット
                    mainForm.Parent.OpDrawBackColorBlackOnTouchPanel();
                });
                // 描画向け緩衝用
                Thread.Sleep(1);
            });

        }
        /// <summary>
        /// 背景指定背景色に
        /// </summary>
        public void DrawScreenBackColor()
        {
            BlockAction.SetAction(
            () =>
            {
                InvokeMethod(() =>
                {
                    // エピソード時は背景色無視とする
                    if (!mainForm.Parent.EpisodeActive.Value)
                    {
                        //背景色画面リセット
                        mainForm.Parent.OpDrawBackColorOnTouchPanel();
                    }
                });
            });
        }
        /// <summary>
        /// 背景色指定ディレイ時指定色
        /// </summary>
        public void DrawScreenDelayBackColor()
        {
            BlockAction.SetAction(
            () =>
            {
                InvokeMethod(() =>
                {
                    //ディレイ時指定色画面リセット
                    mainForm.Parent.OpDrawDelayBackColorOnTouchPanel();
                });
            });
        }
        /// <summary>
        /// タッチされたら次の処理を遅延させる
        /// </summary>
        ///<param name="ms">タッチ待ち時間</param>
        public void TouchDelay(int min, int max)
        {
            // Touch された場合遅延させる機能
            // 正答タッチ待ちと競合
            BlockAction.SetAction(
            () =>
            {
                TouchDelayInternal(min, max);
            });
        }
        /// <summary>
        /// タッチされたら次の処理を遅延させる ルームランプ点灯付き
        /// </summary>
        ///<param name="ms">タッチ待ち時間</param>
        public void TouchDelayWithLamp(int min, int max)
        {
            // Touch された場合遅延させる機能
            // 正答タッチ待ちと競合
            BlockAction.SetAction(
            () =>
            {
                mainForm.Parent.OpSetRoomLampOn();
                TouchDelayInternal(min, max);
                mainForm.Parent.OpSetRoomLampOff();
            });
        }
        /// <summary>
        /// タッチされたら遅延させる エンディング時
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        public void TouchDelayEnding(int min, int max)
        {
            // Touch された場合遅延させる機能
            // 正答タッチ待ちと競合
            BlockAction.SetEndingAction(
            () =>
            {
                TouchDelayInternal(min, max);
            });
        }
        /// <summary>
        /// Feedフラグ初期化
        /// </summary>
        public void InitFeedingFlag()
        {
            BlockAction.SetStartingAction(
               () =>
               {
                   mainForm.Parent.OpFlagFeedOn = false;
               });
        }
        public void InitEpisodeStatus()
        {
            BlockAction.SetStartingAction(
                () =>
                {
                    if (PreferencesDatOriginal.EnableEpisodeMemory)
                    {
                        mainForm.Parent.OpCheckEpisode();
                    }
                });
        }

        public void ViewEpisodeStatus()
        {
            BlockAction.SetStartingAction(
            () =>
            {
                if (PreferencesDatOriginal.EnableEpisodeMemory)
                {
                    mainForm.Parent.UpdateEpisodeStatus();
                }
            });
        }
        /// <summary>
        /// タッチ 遅延 内部動作
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        private void TouchDelayInternal(int min, int max)
        {
            int ms = 0;
            if (max == 0)
            {
                ms = min;
            }
            else
            {
                ms = GenerateRandom(min, max);
            }
            // 試行回数最終時TouchDelay行わない かつ初回を除く
            if (opCollection.trialCount != PreferencesDatOriginal.OpeNumberOfTrial || opCollection.trialCount == 0)
            {
                //Touch動作Init
                InvokeMethod(() =>
                {
                    mainForm.Parent.OpStartToTouchAnyOnTouchPanel();
                });

                // Touch監視
                InternalWaitForTouchDelay(mainForm.Parent.OpFlagTouchAnyOnTouchPanel, ms);
                //

                // Touch終了
                InvokeMethod(() =>
                {
                    mainForm.Parent.OpEndToTouchAnyOnTouchPanel();
                });
                // 指定時間経過後終了
            }
        }
        /// <summary>
        /// 画面点滅 Block内用
        /// </summary>
        private void ViewFlashInternal()
        {
            mainForm.Parent.TraceMessage("画面点滅");
            for (int i = 0; i < 3; i++)
            {
                InvokeMethod(() =>
                {
                    //黒画面リセット
                    mainForm.Parent.opImage.DrawBackColor(Color.Black);
                });
                InternalWait(100, 10000).Wait();
                InvokeMethod(() =>
                {
                    //白画面リセット
                    mainForm.Parent.opImage.DrawBackColor(Color.DimGray);
                });
                InternalWait(100, 10000).Wait();
            }
            InvokeMethod(() =>
            {
                //黒画面リセット
                mainForm.Parent.OpDrawBackColorBlackOnTouchPanel();
            });
        }

        /// <summary>
        /// タッチトリガー待ち
        /// </summary>
        public void WaitTouchTrigger()
        {
            // どこでタッチトリガーの初期リセットを行う？
            PreTouchTriggerProc();
            BlockAction.SetAction(
            () =>
            {
                WaitTriggerInternal();
            });
        }
        /// <summary>
        /// タッチトリガー待ちエンディング時
        /// </summary>
        public void WaitTouchTriggerEnding()
        {
            // どこでタッチトリガーの初期リセットを行う？
            PreTouchTriggerProc();
            BlockAction.SetEndingAction(
            () =>
            {
                WaitTriggerInternal();
            });
        }
        /// <summary>
        /// タッチトリガー待ちアクション内部
        /// </summary>
        private void WaitTriggerInternal()
        {
            // Touch監視 押されたら終了
            sw.Restart();
            InvokeMethod(() =>
            {
                //タッチパネル監視開始
                mainForm.Parent.OpStartToTouchAnyOnTouchPanel();
            });
            //タイムアウト 非常停止監視付きウェイトタスク
            while (PreferencesDatOriginal.OpeTimeoutOfTrial > sw.Elapsed.TotalMinutes)
            {
                //if (opCollection.Command == OpCollection.ECommand.EmergencyStop)
                if (opCollection.sequencer.State == OpCollection.Sequencer.EState.EmergencyStop)
                {

                    BlockAction.InterruptFlag = true;
                    opCollection.taskResultVal = OpCollection.ETaskResult.None;
                    opCollection.sequencer.callbackOutputFile(OpCollection.Sequencer.EState.EmergencyStop);
                    break;
                }

                //if (opCollection.Command == OpCollection.ECommand.Stop)
                if (opCollection.sequencer.State == OpCollection.Sequencer.EState.Stop)
                {

                    BlockAction.InterruptFlag = true;
                    opCollection.sequencer.callbackOutputFile(OpCollection.Sequencer.EState.Stop);
                    break;
                }
                if (opCollection.sequencer.State == OpCollection.Sequencer.EState.IllegalExitDetection)
                {
                    // イリーガル退出時動作
                    IllegalExitDetection();
                    opCollection.taskResultVal = OpCollection.ETaskResult.IllegalExit;
                    BlockAction.InterruptFlag = true;
                    break;
                }
                if (mainForm.Parent.OpFlagTouchAnyOnTouchPanel == true)
                {
                    // Triggerタッチ処理

                    CompleteTouchTriggerScreen();
                    break;
                }

                //緩衝用
                Thread.Sleep(1);
            }

            TrainingTimeout();
            //タイムアウト時処理
            if (PreferencesDatOriginal.OpeTimeoutOfTrial < sw.Elapsed.TotalMinutes)
            {
                BlockAction.InterruptFlag = true;
                TimeoutProc();
            }
            InvokeMethod(() =>
            {
                //タッチパネル監視終了
                mainForm.Parent.OpEndToTouchAnyOnTouchPanel();
            });
        }

        /// <summary>
        /// タッチトリガー待ち
        /// </summary>
        public void WaitCorrectTouchTrigger()
        {
            BlockAction.SetAction(
            () =>
            {
                // Touch監視 押されたら終了
                sw.Restart();
                InvokeMethod(() =>
                {
                    //タッチパネル監視開始
                    // 実行時に判断
                    if (!mainForm.Parent.EpisodeActive.Value)
                    {
                        mainForm.Parent.OpStartToTouchCorrectOnTouchPanel();
                    }
                    else
                    {
                        // IDカウント0だったら、Expireしたら
                        if (!mainForm.Parent.idControlHelper.FindId(opCollection.idCode) || mainForm.Parent.idControlHelper.GetCount(opCollection.idCode) == 0)
                        {
                            mainForm.Parent.OpStartToTouchAnyCorrectOnTouchPanel();
                        }
                        else
                        {
                            mainForm.Parent.OpStartToTouchCorrectOnTouchPanel();
                        }
                    }
                });

                if (PreferencesDatOriginal.EnableMultiFeeder)
                {
                    IncorrectMultiWait();
                }
                else
                {
                    //元のタッチ待ちWait
                    CorrectWait();
                }

                //タイムアウト処理追加
                TrainingTimeout();

                //タイムアウト時処理 + ペナルティ時間(secからminへ)
                if (PreferencesDatOriginal.OpeTimeoutOfTrial + (PreferencesDatOriginal.IncorrectPenaltyTime / 60) < sw.Elapsed.TotalMinutes)
                {
                    // タッチ待ち時タイムアウトなので
                    //opCollection.sequencer.callbackOutputFile(OpCollection.Sequencer.EState.WaitingForTouchTriggerScreen);
                    TimeoutProc();

                    //後続処理をキャンセルして初期へ戻るように
                    BlockAction.InterruptFlag = true;
                }
                else
                {
                    InvokeMethod(() =>
                    {
                        //タッチパネル監視終了
                        opCollection.callbackMessageNormal("画面タッチ後処理");
                        mainForm.Parent.OpEndToTouchCorrectOnTouchPanel();
                    });
                }
            });
        }

        private void CorrectWait()
        {
            //タイムアウト 非常停止監視付きウェイトタスク + ペナルティ時間
            while (PreferencesDatOriginal.OpeTimeoutOfTrial + (PreferencesDatOriginal.IncorrectPenaltyTime / 60) > sw.Elapsed.TotalMinutes)
            {
                // 中断処理を入れる
                //if (opCollection.Command == OpCollection.ECommand.EmergencyStop)
                if (opCollection.sequencer.State == OpCollection.Sequencer.EState.EmergencyStop)
                {

                    BlockAction.InterruptFlag = true;
                    opCollection.sequencer.callbackOutputFile(OpCollection.Sequencer.EState.EmergencyStop);
                    break;
                }
                //if (opCollection.Command == OpCollection.ECommand.Stop)
                if (opCollection.sequencer.State == OpCollection.Sequencer.EState.Stop)
                {

                    BlockAction.InterruptFlag = true;
                    opCollection.sequencer.callbackOutputFile(OpCollection.Sequencer.EState.Stop);
                    break;
                }
                if (opCollection.sequencer.State == OpCollection.Sequencer.EState.IllegalExitDetection)
                {
                    // イリーガル退出時動作
                    IllegalExitDetection();
                    BlockAction.InterruptFlag = true;
                    break;
                }
                if (mainForm.Parent.OpFlagTouchCorrectOnTouchPanel)
                {
                    //正答後処理
                    CompleteTouchCorrectScreen();

                    //エピソード記憶時初回選択後処理
                    if (mainForm.Parent.EpisodeActive.Value && (!mainForm.Parent.idControlHelper.FindId(opCollection.idCode) || mainForm.Parent.idControlHelper.GetCount(opCollection.idCode) == 0))
                    {
                        mainForm.Parent.opImage.SortCorrectShape();
                        mainForm.Parent.episodeMemory.AddOrUpdateShapObject(opCollection.idCode, mainForm.Parent.opImage.TouchAnyShapes);
                    }
                    //正答時にIncorrectCountリセット
                    opCollection.IncorrectCount = 0;
                    break;
                }
                else if (mainForm.Parent.OpFlagTouchIncorrectOnTouchPanel && PreferencesDatOriginal.EnableIncorrectCancel)
                {
                    opCollection.IncorrectCount++;
                    mainForm.Parent.OpFlagTouchIncorrectOnTouchPanel = false;

                    if (opCollection.IncorrectCount >= PreferencesDatOriginal.IncorrectCount) //IncorrectCount設定判別
                    {
                        //Feedフラグ強制リセット
                        Feeded = false;
                        opCollection.flagFeed = false;

                        CompleteTouchIncorrectScreen();
                        BlockAction.IncorrectFlag = true;
                        opCollection.sequencer.callbackOutputFile(OpCollection.Sequencer.EState.BlockOutput); //不正解時結果書き込み
                        opCollection.callbackMessageNormal(string.Format("不正解画像タッチ回数({0}/{1})により試行再開", opCollection.IncorrectCount, PreferencesDatOriginal.IncorrectCount));
                        opCollection.IncorrectCount = 0;

                        // 復帰前動作
                        InvokeMethod(() =>
                        {
                            //黒画面リセット
                            mainForm.Parent.OpDrawBackColorBlackOnTouchPanel();
                        });
                        if (PreferencesDatOriginal.IncorrectTouchAction)
                        {
                            // 退室促進
                            opCollection.sequencer.State = OpCollection.Sequencer.EState.IncorrectTouchExit;
                        }
                        else
                        {
                            // 点滅
                            ViewFlashInternal();
                        }

                        //不正解時ペナルティ追加
                        if (PreferencesDatOriginal.IncorrectPenaltyTime > 0)
                        {
                            opCollection.callbackMessageNormal(string.Format("ペナルティによるディレイ{0}ms", PreferencesDatOriginal.IncorrectPenaltyTime * 1000));

                            while (PreferencesDatOriginal.IncorrectPenaltyTime * 1000 > sw.ElapsedMilliseconds)
                            {
                                //残り時間表示
                                if (sw.ElapsedMilliseconds > 0 && sw.ElapsedMilliseconds % 1000 == 0)
                                {
                                    // 残り時間通知
                                    opCollection.callbackMessageNormal(string.Format("ペナルティ経過{0}ms", sw.ElapsedMilliseconds));
                                }

                                //if (opCollection.Command == OpCollection.ECommand.EmergencyStop)
                                if (opCollection.sequencer.State == OpCollection.Sequencer.EState.EmergencyStop)
                                {
                                    BlockAction.InterruptFlag = true;
                                    opCollection.sequencer.callbackOutputFile(OpCollection.Sequencer.EState.EmergencyStop);
                                    break;
                                }
                                //if (opCollection.Command == OpCollection.ECommand.Stop)
                                if (opCollection.sequencer.State == OpCollection.Sequencer.EState.Stop)
                                {
                                    BlockAction.InterruptFlag = true;
                                    opCollection.sequencer.callbackOutputFile(OpCollection.Sequencer.EState.Stop);
                                    break;
                                }
                                if (opCollection.sequencer.State == OpCollection.Sequencer.EState.IllegalExitDetection)
                                {
                                    // イリーガル退出時動作
                                    IllegalExitDetection();
                                    BlockAction.InterruptFlag = true;
                                    break;
                                }
                                Thread.Sleep(1);
                            }
                        }
                        else
                        {
                            // タッチディレイ 基本1秒
                            TouchDelayInternal(1000, 1000);
                        }
                        opCollection.sequencer.State = OpCollection.Sequencer.EState.IncorrectTouchExit;
                        break;

                    }
                    //不正解タッチ処理
                    //break;
                }
                //else
                //{
                //    //正解するまで待ち
                //}

                //緩衝用
                Thread.Sleep(1);
            }

        }
        /// <summary>
        /// 不正解時FeedWait
        /// </summary>
        private void IncorrectMultiWait()
        {
            //タイムアウト 非常停止監視付きウェイトタスク + ペナルティ時間
            while (PreferencesDatOriginal.OpeTimeoutOfTrial + (PreferencesDatOriginal.IncorrectPenaltyTime / 60) > sw.Elapsed.TotalMinutes)
            {
                // 中断処理を入れる
                if (opCollection.sequencer.State == OpCollection.Sequencer.EState.EmergencyStop)
                {
                    BlockAction.InterruptFlag = true;
                    opCollection.sequencer.callbackOutputFile(OpCollection.Sequencer.EState.EmergencyStop);
                    break;
                }
                if (opCollection.sequencer.State == OpCollection.Sequencer.EState.Stop)
                {
                    BlockAction.InterruptFlag = true;
                    opCollection.sequencer.callbackOutputFile(OpCollection.Sequencer.EState.Stop);
                    break;
                }
                if (opCollection.sequencer.State == OpCollection.Sequencer.EState.IllegalExitDetection)
                {
                    // イリーガル退出時動作
                    IllegalExitDetection();
                    BlockAction.InterruptFlag = true;
                    break;
                }
                if (mainForm.Parent.OpFlagTouchCorrectOnTouchPanel)
                {
                    //正答後処理
                    CompleteTouchCorrectScreen();

                    //エピソード記憶時初回選択後処理
                    if (mainForm.Parent.EpisodeActive.Value && (!mainForm.Parent.idControlHelper.FindId(opCollection.idCode) || mainForm.Parent.idControlHelper.GetCount(opCollection.idCode) == 0))
                    {
                        mainForm.Parent.opImage.SortCorrectShape();
                        mainForm.Parent.episodeMemory.AddOrUpdateShapObject(opCollection.idCode, mainForm.Parent.opImage.TouchAnyShapes);
                    }
                    //正答時にIncorrectCountリセット
                    opCollection.IncorrectCount = 0;
                    break;
                }
                // Incorrect時給餌の場合は違う条件
                else if (mainForm.Parent.OpFlagTouchIncorrectOnTouchPanel)
                {
                    CompleteTouchIncorrectScreen();
                    // 復帰前動作
                    InvokeMethod(() =>
                    {
                        //黒画面リセット
                        mainForm.Parent.OpDrawBackColorBlackOnTouchPanel();
                    });
                    if (PreferencesDatOriginal.IncorrectTouchAction)
                    {
                        // 退室促進
                        opCollection.sequencer.State = OpCollection.Sequencer.EState.IncorrectTouchExit;
                    }
                    else
                    {
                        // 点滅
                        ViewFlashInternal();
                    }
                    break;
                }

                //緩衝用
                Thread.Sleep(1);
            }

        }

        /// <summary>
        /// トリガ画像表示 
        /// </summary>
        public void ViewTriggerImage()
        {
            BlockAction.SetActionEnableReplay(
               () =>
               {
                   InvokeMethod(() =>
                   {
                       mainForm.Parent.OpDrawTriggerImageOnTouchPanel();
                   });
               });

        }
        /// <summary>
        /// トリガ画像表示 ファイル引数  
        /// </summary>
        /// <param name="imageFilePath"></param>
        public void ViewTriggerImage(string imageFilePath)
        {
            BlockAction.SetActionEnableReplay(
               () =>
               {
                   InvokeMethod(() =>
                   {
                       mainForm.Parent.OpDrawTriggerImageOnTouchPanel(imageFilePath);
                   });
               });

        }
        /// <summary>
        /// 正解図形表示 訓練 Preference画像利用
        /// </summary>
        public void ViewCorrectImage()
        {
            BlockAction.SetActionEnableReplay(
            () =>
            {
                InvokeMethod(() =>
                {
                    if (!mainForm.Parent.EpisodeActive.Value)
                    {
                        mainForm.Parent.OpDrawBackColorOnTouchPanel();
                        //表示前予備処理入れる →タイムスタンプ記録など
                        mainForm.Parent.OpDrawTrainShapeOnTouchPanelBlock(out shapeObject); // 訓練正解図形表示
                                                                                            // ↑このタイミングで選択ファイル確定
                        opCollection.trainingShape = shapeObject;

                        // ImageFile 有効時ファイル名ログ伝達
                        if (PreferencesDatOriginal.EnableImageShape)
                        {
                            opCollection.correctImageFile = System.IO.Path.GetFileName(shapeObject.ImageFilename);
                        }
                    }
                });
            });
        }
        /// <summary>
        /// 正解図形表示
        /// </summary>
        /// <param name="imageFilePath">正解画像ファイル</param>
        public void ViewCorrectImage(string imageFilePath)
        {
            BlockAction.SetActionEnableReplay(
            () =>
            {
                InvokeMethod(() =>
                {
                    mainForm.Parent.OpDrawBackColorOnTouchPanel();
                    //表示前予備処理入れる →タイムスタンプ記録など
                    if (!mainForm.Parent.EpisodeActive.Value)
                    {
                        mainForm.Parent.OpDrawTrainShapeOnTouchPanelBlock(out shapeObject); // 訓練正解図形表示
                        opCollection.trainingShape = shapeObject;
                    }
                    // ImageFile 有効時ファイル名ログ伝達
                    if (PreferencesDatOriginal.EnableImageShape)
                    {
                        opCollection.correctImageFile = System.IO.Path.GetFileName(imageFilePath);
                    }
                });
            });
        }
        /// <summary>
        /// 正解図形・不正解図形表示 課題
        /// 訓練表示前に呼ばれるとエラー
        /// </summary>
        public void ViewCorrectWrongImage()
        {
            BlockAction.SetActionEnableReplay(
            () =>
            {
                mainForm.Parent.Invoke((System.Windows.Forms.MethodInvoker)(() =>
                {
                    mainForm.Parent.OpDrawBackColorOnTouchPanel();
                    //表示前予備処理入れる →タイムスタンプ記録など
                    mainForm.Parent.OpDrawCorrectAndWrongShapeOnTouchPanelBlock(out shapeObject, out opCollection.incorrectShapeList); //正解図形・不正解図形表示
                    //opCollection.trainingShape = shapeObject;
                    opCollection.correctShape = shapeObject;
                    // ImageFile 有効時ファイル名ログ伝達
                    if (PreferencesDatOriginal.EnableImageShape)
                    {
                        opCollection.correctImageFile = System.IO.Path.GetFileName(shapeObject.ImageFilename);
                    }

                }));
            });
        }

        public void MatchBeforeDelay(int min, int max)
        {
            BlockAction.SetAction(
            () =>
            {
                int ms = 0;
                if (max == 0)
                {
                    ms = min;
                }
                else
                {
                    ms = GenerateRandom(min, max);
                }
                genCount++;

                InvokeMethod(() =>
                {
                    opCollection.callbackMessageNormal(string.Format("ランダムディレイ:{0}ms", ms));
                });
                sw.Restart();
                //タイムアウト 非常停止監視付きウェイトタスク
                while (ms > sw.ElapsedMilliseconds)
                {
                    //if (opCollection.Command == OpCollection.ECommand.EmergencyStop)
                    if (opCollection.sequencer.State == OpCollection.Sequencer.EState.EmergencyStop)
                    {

                        BlockAction.InterruptFlag = true;
                        opCollection.sequencer.callbackOutputFile(OpCollection.Sequencer.EState.EmergencyStop);
                        opCollection.beforeDelayMatchRandomTime = (int)sw.ElapsedMilliseconds;
                        break;
                    }
                    //if (opCollection.Command == OpCollection.ECommand.Stop)
                    if (opCollection.sequencer.State == OpCollection.Sequencer.EState.Stop)
                    {

                        BlockAction.InterruptFlag = true;
                        opCollection.sequencer.callbackOutputFile(OpCollection.Sequencer.EState.Stop);
                        opCollection.beforeDelayMatchRandomTime = (int)sw.ElapsedMilliseconds;
                        break;
                    }
                    if (opCollection.sequencer.State == OpCollection.Sequencer.EState.IllegalExitDetection)
                    {
                        // イリーガル退出時動作
                        IllegalExitDetection();
                        opCollection.beforeDelayMatchRandomTime = (int)sw.ElapsedMilliseconds;
                        BlockAction.InterruptFlag = true;
                        break;
                    }
                    Thread.Sleep(1);
                }

                opCollection.beforeDelayMatchRandomTime = (int)sw.ElapsedMilliseconds;
                // タイムアウト処理
                // ディレイ時のタイムアウトは無し
                if (opCollection.stopwatch.ElapsedMilliseconds >= ms)
                {
                    opCollection.stopwatch.Restart(); // ストップウォッチをリスタート
                }
            });
        }
        /// <summary>
        /// ディレイ
        /// </summary>
        /// <param name="ms">時間</param>
        /// <returns>結果</returns>
        public void Delay(int ms)
        {
            BlockAction.SetAction(
            () =>
            {
                //InvokeMethod(() =>
                //{
                sw.Restart();
                opCollection.callbackMessageNormal(string.Format("ディレイ:{0}ms", ms));
                //タイムアウト 非常停止監視付きウェイトタスク
                while (ms > sw.ElapsedMilliseconds)
                {
                    //if (opCollection.Command == OpCollection.ECommand.EmergencyStop)
                    if (opCollection.sequencer.State == OpCollection.Sequencer.EState.EmergencyStop)
                    {

                        BlockAction.InterruptFlag = true;
                        opCollection.sequencer.callbackOutputFile(OpCollection.Sequencer.EState.EmergencyStop);
                        break;
                    }
                    //if (opCollection.Command == OpCollection.ECommand.Stop)
                    if (opCollection.sequencer.State == OpCollection.Sequencer.EState.Stop)
                    {

                        BlockAction.InterruptFlag = true;
                        opCollection.sequencer.callbackOutputFile(OpCollection.Sequencer.EState.Stop);
                        break;
                    }
                    if (opCollection.sequencer.State == OpCollection.Sequencer.EState.IllegalExitDetection)
                    {
                        // イリーガル退出時動作
                        IllegalExitDetection();
                        BlockAction.InterruptFlag = true;
                        break;
                    }
                    Thread.Sleep(1);
                }
                //});
                // タイムアウト処理

                // ディレイ時のタイムアウトは無し
                if (opCollection.stopwatch.ElapsedMilliseconds >= ms)
                {
                    // 初期化せず
                    //TimeoutProc();
                    opCollection.stopwatch.Stop(); // ストップウォッチを停止
                    opCollection.stopwatch.Restart(); // ストップウォッチをリスタート
                }
            });
        }
        public int GenerateRandom(int min, int max)
        {
            var seed = Convert.ToInt32(Regex.Match(Guid.NewGuid().ToString(), @"\d+").Value);
            if (max == 0 | min == max)
            {
                return min;
            }
            else
            {
                return new Random(seed).Next(min, maxValue: max - 1);
            }
        }
        int genCount = 0;
        public void Delay(int min, int max)
        {
            BlockAction.SetAction(
            () =>
            {
                if (!mainForm.Parent.EpisodeActive.Value)
                {
                    int ms = 0;
                    if (max == 0)
                    {
                        ms = min;
                    }
                    else
                    {
                        ms = GenerateRandom(min, max);
                    }
                    genCount++;

                    InvokeMethod(() =>
                    {
                        //opCollection.callbackMessageNormal(string.Format("{0}回目Delay", genCount));
                        opCollection.callbackMessageNormal(string.Format("ディレイ:{0}ms", ms));
                    });
                    sw.Restart();
                    //タイムアウト 非常停止監視付きウェイトタスク
                    while (ms > sw.ElapsedMilliseconds)
                    {
                        //if (opCollection.Command == OpCollection.ECommand.EmergencyStop)
                        if (opCollection.sequencer.State == OpCollection.Sequencer.EState.EmergencyStop)
                        {

                            BlockAction.InterruptFlag = true;
                            opCollection.sequencer.callbackOutputFile(OpCollection.Sequencer.EState.EmergencyStop);
                            break;
                        }
                        //if (opCollection.Command == OpCollection.ECommand.Stop)
                        if (opCollection.sequencer.State == OpCollection.Sequencer.EState.Stop)
                        {

                            BlockAction.InterruptFlag = true;
                            opCollection.sequencer.callbackOutputFile(OpCollection.Sequencer.EState.Stop);
                            break;
                        }
                        if (opCollection.sequencer.State == OpCollection.Sequencer.EState.IllegalExitDetection)
                        {
                            // イリーガル退出時動作
                            IllegalExitDetection();
                            BlockAction.InterruptFlag = true;
                            break;
                        }
                        Thread.Sleep(1);
                    }
                    //});
                    // タイムアウト処理

                    // ディレイ時のタイムアウトは無し
                    if (opCollection.stopwatch.ElapsedMilliseconds >= ms)
                    {
                        // 初期化せず
                        //TimeoutProc();
                        opCollection.stopwatch.Stop(); // ストップウォッチを停止
                        opCollection.stopwatch.Restart(); // ストップウォッチをリスタート
                    }
                }
            });
        }
        public void DelayEpisode(int min, int max)
        {
            BlockAction.SetAction(
            () =>
            {
                if (mainForm.Parent.EpisodeActive.Value)
                {
                    int ms = 0;
                    if (max == 0)
                    {
                        ms = min;
                    }
                    else
                    {
                        ms = GenerateRandom(min, max);
                    }
                    genCount++;

                    InvokeMethod(() =>
                    {
                        //opCollection.callbackMessageNormal(string.Format("{0}回目Delay", genCount));
                        opCollection.callbackMessageNormal(string.Format("ディレイ:{0}ms", ms));
                    });
                    sw.Restart();
                    //タイムアウト 非常停止監視付きウェイトタスク
                    while (ms > sw.ElapsedMilliseconds)
                    {
                        //if (opCollection.Command == OpCollection.ECommand.EmergencyStop)
                        if (opCollection.sequencer.State == OpCollection.Sequencer.EState.EmergencyStop)
                        {

                            BlockAction.InterruptFlag = true;
                            opCollection.sequencer.callbackOutputFile(OpCollection.Sequencer.EState.EmergencyStop);
                            break;
                        }
                        //if (opCollection.Command == OpCollection.ECommand.Stop)
                        if (opCollection.sequencer.State == OpCollection.Sequencer.EState.Stop)
                        {

                            BlockAction.InterruptFlag = true;
                            opCollection.sequencer.callbackOutputFile(OpCollection.Sequencer.EState.Stop);
                            break;
                        }
                        if (opCollection.sequencer.State == OpCollection.Sequencer.EState.IllegalExitDetection)
                        {
                            // イリーガル退出時動作
                            IllegalExitDetection();
                            BlockAction.InterruptFlag = true;
                            break;
                        }
                        Thread.Sleep(1);
                    }
                    //});
                    // タイムアウト処理

                    // ディレイ時のタイムアウトは無し
                    if (opCollection.stopwatch.ElapsedMilliseconds >= ms)
                    {
                        // 初期化せず
                        //TimeoutProc();
                        opCollection.stopwatch.Stop(); // ストップウォッチを停止
                        opCollection.stopwatch.Restart(); // ストップウォッチをリスタート
                    }
                }
            });
        }
        /// <summary>
        /// 給餌
        /// </summary>
        /// <param name="ms">給餌時間</param>
        public void Feed(int min, int max)
        {
            //指定トリガーTrue後指定時間ON
            BlockAction.SetActionAsync(
                () =>
                {
                    int ms = 0;
                    if (max == 0)
                    {
                        ms = min;
                    }
                    else
                    {
                        ms = GenerateRandom(min, max);
                    }
                    //InvokeMethod(() =>
                    //{
                    //mainForm.Parent.OpSetFeedOn(opCollection.trialCount, out opCollection.flagFeed);

                    //実行時判断
                    if (!mainForm.Parent.EpisodeActive.Value)
                    {
                        if (PreferencesDatOriginal.EnableMultiFeeder)
                        {
                            FeedInternal(ms, !mainForm.Parent.OpFlagTouchIncorrectOnTouchPanel);
                        }
                        else
                        {
                            FeedInternal(ms);
                        }
                    }
                    else
                    {
                        if (mainForm.Parent.idControlHelper.GetCount(opCollection.idCode.ToString()) > 0)
                        {
                            ms = PreferencesDatOriginal.OpeTimeToFeedReward;
                        }
                        else
                        {
                            //FirstTime
                            ms = PreferencesDatOriginal.OpeTimeToFeedRewardFirstTime;
                        }
                        // 給餌後カウントアップ
                        mainForm.Parent.idControlHelper.AddCount(opCollection.idCode.ToString());
                        if (PreferencesDatOriginal.EnableMultiFeeder)
                        {
                            FeedInternal(ms, mainForm.Parent.OpFlagTouchIncorrectOnTouchPanel);
                        }
                        else
                        {
                            FeedInternal(ms);
                        }
                    }

                    opCollection.FeedTime = ms;
                    //});

                });
            //Waitは同期側で
            BlockAction.SetAction(
                () =>
                {
                    //Feed終了まで待つ

                    //タイムアウト値は設定値から
                    InternalFeedWait(1000000).Wait();

                    if (mainForm.Parent.OpFlagFeedOn)
                    {
                        if (mainForm.Parent.EpisodeActive.Value)
                        {
                            if (mainForm.Parent.EpisodeMode.Value)
                            {
                                if (!PreferencesDatOriginal.EnableMultiFeeder)
                                {
                                    opCollection.taskResultVal = OpCollection.ETaskResult.OkEpisodeIssue;
                                }
                                else
                                {
                                    if (mainForm.Parent.OpFlagTouchIncorrectOnTouchPanel)
                                    {
                                        mainForm.Parent.OpFlagTouchIncorrectOnTouchPanel = false;
                                        opCollection.taskResultVal = OpCollection.ETaskResult.NgEpisodeIssue;
                                    }
                                    else
                                    {
                                        opCollection.taskResultVal = OpCollection.ETaskResult.OkEpisodeIssue;
                                    }
                                }
                            }
                            else
                            {
                                opCollection.taskResultVal = OpCollection.ETaskResult.OkEpisode;
                            }
                        }
                        else
                        {
                            opCollection.taskResultVal = OpCollection.ETaskResult.Ok;
                            if (mainForm.Parent.OpFlagTouchIncorrectOnTouchPanel)
                            {
                                opCollection.taskResultVal = OpCollection.ETaskResult.Ng;
                            }
                        }
                        Feeded = true;
                    }
                    InvokeMethod(() =>
                    {
                        opCollection.callbackMessageNormal(string.Format("給餌終了"));
                    });
                });
        }
        private void FeedInternal(int feedTime)
        {
            FeedInternal(feedTime, true);
        }
        private void FeedInternal(int feedTime, bool result)
        {
            // IncorrectFeeder2台目固定
            // Feed条件で切り替える
            if (result)
            {
                mainForm.Parent.OpSetFeedOn(feedTime);
            }
            else
            {
                mainForm.Parent.OpSetFeed2On(feedTime);
            }
        }
        /// <summary>
        /// 給餌向けランプ点灯
        /// </summary>
        /// <param name="ms">時間</param>
        public void FeedLamp(int min, int max)
        {
            //指定トリガーTrue後
            BlockAction.SetActionAsync(
                () =>
                {
                    int ms = 0;
                    if (max == 0)
                    {
                        ms = min;
                    }
                    else
                    {
                        ms = GenerateRandom(min, max);
                    }
                    opCollection.FeedLampTime = ms;
                    //待たない動作は非同期化
                    // 全体Waitより長いと次の処理中に点灯してしまう
                    //var t = Task.Run(() =>
                    //{

                    //InvokeMethod(() =>
                    //{
                    mainForm.Parent.OpSetFeedLampOn();
                    //});
                    {
                        //タイムアウト値は設定値から
                        InternalWait(ms, 1000000).Wait();
                    }

                    //InvokeMethod(() =>
                    //{
                    mainForm.Parent.OpSetFeedLampOff();
                    //});
                    //消えるまで10msほど待ってみる よさそう
                    InternalWait(10, 100000).Wait();
                });
        }
        /// <summary>
        /// 給餌向けランプ点灯サウンド再生 使わないけど一応セット
        /// </summary>
        /// <param name="ms">時間</param>
        public void FeedLampAndSound(int min, int max)
        {
            //指定トリガーTrue後
            BlockAction.SetActionAsync(() =>
            {
                int ms = 0;
                if (max == 0)
                {
                    ms = min;
                }
                else
                {
                    ms = GenerateRandom(min, max);
                }
                opCollection.FeedSoundTime = ms;

                //InvokeMethod(() =>
                //{
                mainForm.Parent.OpSetFeedLampOn();
                //FormMain.PlaySoundLooping(PreferencesDatOriginal.SoundFileOfReward, ms);
                //});
                // Naudio化によりInvoke外
                FormMain.PlayMedia(PreferencesDatOriginal.SoundFileOfReward, ms);

                {
                    //タイムアウト値は設定値から
                    InternalWait(ms, 1000000).Wait();
                }

                //InvokeMethod(() =>
                //{
                mainForm.Parent.OpSetFeedLampOff();
                //});
                //消えるまで10msほど待ってみる よさそう
                InternalWait(10, 100000).Wait();
            });
        }
        /// <summary>
        /// 給餌向け音再生 非同期再生なので再生終了待たない
        /// </summary>
        /// <param name="ms">時間</param>
        public void FeedSound(int min, int max)
        {
            if (!System.IO.File.Exists(PreferencesDatOriginal.SoundFileOfReward))
            {
                //throw new System.IO.FileNotFoundException("FeedSound音声ファイルが存在しません");
            }
            //指定トリガーTrue後
            BlockAction.SetActionAsync(
                () =>
                {
                    int ms = 0;
                    if (max == 0)
                    {
                        ms = min;
                    }
                    else
                    {
                        ms = GenerateRandom(min, max);
                    }
                    opCollection.FeedSoundTime = ms;


                    if (PreferencesDatOriginal.EnableMultiFeeder)
                    {
                        if (!mainForm.Parent.OpFlagTouchIncorrectOnTouchPanel)
                        {
                            InvokeMethod(() =>
                            {
                                //FormMain.PlaySoundLooping(PreferencesDatOriginal.SoundFileOfReward, ms);
                                opCollection.callbackMessageNormal("報酬音を鳴らす");
                            });
                            FormMain.PlayMedia(PreferencesDatOriginal.SoundFileOfReward, ms);
                        }
                        else
                        {
                            InvokeMethod(() =>
                            {
                                //FormMain.PlaySoundLooping(PreferencesDatOriginal.SoundFileOfReward, ms);
                                opCollection.callbackMessageNormal("不正解報酬音を鳴らす");
                            });
                            FormMain.PlayMedia(PreferencesDatOriginal.SoundFileOfIncorrectReward, ms);
                        }
                    }
                    else
                    {
                        InvokeMethod(() =>
                        {
                            //FormMain.PlaySoundLooping(PreferencesDatOriginal.SoundFileOfReward, ms);
                            opCollection.callbackMessageNormal("報酬音を鳴らす");
                        });
                        FormMain.PlayMedia(PreferencesDatOriginal.SoundFileOfReward, ms);
                    }
                    //});

                });
        }
        /// <summary>
        /// 音再生 事前指定ファイル Correct
        /// </summary>
        public void PlaySound()
        {

            BlockAction.SetActionAsync(
                () =>
                {
                    //Naudio化したことによりInvokeせずに呼べるように
                    //InvokeMethod(() =>
                    //{
                    //事前指定ファイル再生
                    //FormMain.PlaySound(PreferencesDatOriginal.SoundFileOfCorrect);
                    //FormMain.PlayMedia(PreferencesDatOriginal.SoundFileOfCorrect);
                    {
                        if (PreferencesDatOriginal.EnableMultiFeeder)
                        {
                            if (!mainForm.Parent.OpFlagTouchIncorrectOnTouchPanel)
                            {
                                mainForm.Parent.OpPlaySoundOfCorrect();
                            }
                            else
                            {
                                mainForm.Parent.OpPlaySoundOfIncorrect();
                            }
                        }
                        else
                        {
                            mainForm.Parent.OpPlaySoundOfCorrect();
                        }
                    }

                    //});

                });
        }
        /// <summary>
        /// 音再生 事前指定ファイル End
        /// </summary>
        public void PlayEndSound()
        {
            PlaySound(PreferencesDatOriginal.SoundFileOfEnd);
        }
        /// <summary>
        /// 音再生 指定ファイル
        /// </summary>
        private void PlaySound(string path)
        {
            BlockAction.SetActionAsync(
                () =>
                {
                    //InvokeMethod(() =>
                    //{
                    if (System.IO.File.Exists(path))
                        FormMain.PlayMedia(path);

                    //});
                });
        }
        /// <summary>
        /// 結果出力
        /// </summary>
        public void OutputResult()
        {
            BlockAction.SetEndingAction(
                () =>
                {
                    OutputResultOperation();
                });
        }
        private void OutputResultOperation()
        {
            if (PreferencesDatOriginal.EnableMamosetDetection) { OutputMarmosetDetectResult(); }
            // 開始時表示だけインクリメント 完了時にカウント
            opCollection.trialCount++; // 試行回数カウンタをインクリメント
            opCollection.callbackSetUiCurentNumberOfTrial(opCollection.trialCount);
            opCollection.flagFeed = Feeded;
            //結果出力 正常系
            InvokeMethod(() =>
            {
                if (Feeded)
                    opCollection.callbackMessageNormal(string.Format("設定試行回数({0}/{1})により給餌終了", opCollection.trialCount, PreferencesDatOriginal.OpeNumberOfTrial));
                else
                    opCollection.callbackMessageNormal(string.Format("設定試行回数({0}/{1})により終了", opCollection.trialCount, PreferencesDatOriginal.OpeNumberOfTrial));
            });

            opCollection.sequencer.callbackOutputFile(OpCollection.Sequencer.EState.BlockOutput); // ファイル出力時ステートはBlock時は特定のものに

            //Feedフラグリセット
            Feeded = false;

            opCollection.intervalTimeTotal = 0; // インターバル値合計:初期化(結果ファイル出力用)
            opCollection.intervalNum = 0;  // インターバル回数:初期化(結果ファイル出力用)
            opCollection.FeedTime = 0;
            opCollection.FeedSoundTime = 0;
            opCollection.DeleyTime = 0;
            opCollection.FeedLampTime = 0;

        }
        /// <summary>
        /// マーモセット検出結果処理
        /// </summary>
        private void OutputMarmosetDetectResult()
        {
            // マーモセット検出結果を出力
            //throw new NotImplementedException();
        }

        /// <summary>
        /// 試験終了用
        /// 条件満たしたら終了
        /// </summary>
        public void End()
        {
            // 終了処理->再利用したCloneインスタンスを破棄
            BlockAction.SetAction(() =>
                {

                });
        }
        /// <summary>
        /// BlockAction開始時に初期化動作
        /// </summary>
        public void Initialize()
        {
            BlockAction.SetAction(() =>
            {
                opCollection.intervalTime = 0;
                opCollection.beforeDelayMatchRandomTime = 0;
                // DetectNum初期化
                mainForm.Parent.camImage.InitializeDetectNum();
            });
        }
        public async void Start()
        {
            try
            {
                opCollection.intervalTime = 0;
                opCollection.beforeDelayMatchRandomTime = 0;
                await BlockAction.DoAllTask();

            }
            catch (TaskCanceledException ex)
            {
                Debug.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                //throw;
            }

            Debug.WriteLine("DoAllTask終了");
        }

        public void Stop()
        {
            BlockAction.InterruptFlag = true;
            BlockAction.ClearAction();

        }
        /// <summary>
        /// 試験繰り返し用
        /// 全体を指定回数繰り返す
        /// </summary>
        /// <param name="repeatCount">繰り返し回数</param>
        public void Repeat(int repeatCount)
        {
            //OperationActionHelper側に機能を持たせた

        }
        /// <summary>
        /// 指定時間Wait 非同期対応
        /// </summary>
        /// <param name="waitTime"></param>
        private async Task InternalWait(int waitTime, int timeout)
        {
            Stopwatch localSw = new Stopwatch();
            localSw.Start();
            await Task.Run(() =>
            {
                //与えられた条件Copyでかわらないので参照じゃないと
                while (true)
                {
                    //sw.Stop();
                    if (localSw.ElapsedMilliseconds > waitTime)
                    {
                        localSw.Stop();
                        //InvokeMethod(() =>
                        //{
                        //    opCollection.callbackMessageNormal(string.Format("InternalWait {0}ms", localSw.ElapsedMilliseconds));
                        //});
                        break;
                    }

                    //Emergency Stopも確認する
                    if (opCollection.sequencer.State == OpCollection.Sequencer.EState.EmergencyStop)
                    {

                        BlockAction.InterruptFlag = true;
                        opCollection.sequencer.callbackOutputFile(OpCollection.Sequencer.EState.EmergencyStop);
                        break;
                    }
                    //if (opCollection.Command == OpCollection.ECommand.Stop)
                    if (opCollection.sequencer.State == OpCollection.Sequencer.EState.Stop)
                    {

                        BlockAction.InterruptFlag = true;
                        opCollection.sequencer.callbackOutputFile(OpCollection.Sequencer.EState.Stop);
                        break;
                    }
                    if (opCollection.sequencer.State == OpCollection.Sequencer.EState.IllegalExitDetection)
                    {
                        // イリーガル退出時動作
                        IllegalExitDetection();
                        BlockAction.InterruptFlag = true;
                        break;
                    }
                    if (localSw.ElapsedMilliseconds > timeout)
                    {
                        BlockAction.InterruptFlag = true;
                        //TimeoutProc();
                        TimeoutTouchTrigger();
                        break;
                    }
                    Thread.Sleep(1);
                }
                localSw.Stop();
            });
        }
        /// <summary>
        /// タッチディレイ用内部ウェイト部 ブロックで使用
        /// </summary>
        /// <param name="time"></param>
        private bool InternalWaitForTouchDelay(in bool trg, int time)
        {
            //Stopwatchローカル化
            Stopwatch localSw = new Stopwatch();

            IntervalProc();
            InvokeMethod(() =>
            {
                opCollection.callbackMessageNormal("Touchディレイ開始");
            });

            localSw.Restart();
            while (localSw.ElapsedMilliseconds < time)
            {
                //Emergency Stopも確認する
                //if (opCollection.Command == OpCollection.ECommand.EmergencyStop)
                if (opCollection.sequencer.State == OpCollection.Sequencer.EState.EmergencyStop)
                {
                    BlockAction.InterruptFlag = true;
                    opCollection.sequencer.callbackOutputFile(OpCollection.Sequencer.EState.EmergencyStop);
                    break;
                }
                //if (opCollection.Command == OpCollection.ECommand.Stop)
                if (opCollection.sequencer.State == OpCollection.Sequencer.EState.Stop)
                {
                    BlockAction.InterruptFlag = true;
                    opCollection.sequencer.callbackOutputFile(OpCollection.Sequencer.EState.Stop);
                    break;
                }
                if (opCollection.sequencer.State == OpCollection.Sequencer.EState.IllegalExitDetection)
                {
                    // イリーガル退出時動作
                    IllegalExitDetection();
                    BlockAction.InterruptFlag = true;
                    break;
                }
                if (opCollection.sequencer.State == OpCollection.Sequencer.EState.ExitAfterFeedingDetection)
                {
                    ExitDetection();
                    BlockAction.InterruptFlag = true;
                    break;
                }
                //直接参照しないとだめ
                if (mainForm.Parent.OpFlagTouchAnyOnTouchPanel)
                {
                    //止めないとInvoke側時間参照できないよ
                    localSw.Stop();
                    InvokeMethod(() =>
                    {
                        opCollection.callbackMessageNormal(string.Format("画面をタッチしたのでインターバル期間継続 {0}ms", localSw.ElapsedMilliseconds));
                        opCollection.intervalTimeTotal += (int)localSw.ElapsedMilliseconds;
                        opCollection.intervalNum++;
                    });
                    localSw.Restart();
                    mainForm.Parent.OpFlagTouchAnyOnTouchPanel = false;
                }
                Thread.Sleep(1);
            }

            localSw.Stop();
            opCollection.callbackMessageNormal(string.Format("画面タッチしなかったのでインターバル期間完了 {0}ms", localSw.ElapsedMilliseconds));
            opCollection.intervalTimeTotal += (int)localSw.ElapsedMilliseconds;
            //opCollection.sequencer.callbackOutputFile(OpCollection.Sequencer.EState.IntervalProc); // ファイル出力

            //いらない
            return false;
        }
        private async Task InternalFeedWait(int timeout)
        {
            //Stopwatchローカル化
            Stopwatch localSw = new Stopwatch();
            await Task.Run(() =>
            {
                localSw.Restart();

                while (true)
                {
                    //Emergency Stopも確認する
                    if (opCollection.sequencer.State == OpCollection.Sequencer.EState.EmergencyStop)
                    {
                        BlockAction.InterruptFlag = true;
                        opCollection.sequencer.callbackOutputFile(OpCollection.Sequencer.EState.EmergencyStop);
                        break;
                    }
                    if (opCollection.sequencer.State == OpCollection.Sequencer.EState.Stop)
                    {
                        BlockAction.InterruptFlag = true;
                        opCollection.sequencer.callbackOutputFile(OpCollection.Sequencer.EState.Stop);
                        break;
                    }
                    if (opCollection.sequencer.State == OpCollection.Sequencer.EState.IllegalExitDetection)
                    {
                        // イリーガル退出時動作
                        IllegalExitDetection();
                        BlockAction.InterruptFlag = true;
                        break;
                    }
                    if (opCollection.sequencer.State == OpCollection.Sequencer.EState.ExitAfterFeedingDetection)
                    {
                        ExitDetection();
                        BlockAction.InterruptFlag = true;
                        break;
                    }
                    // Feed待ち急がない
                    Thread.Sleep(100);

                    // Task内でみたいので直参照のみ・・・
                    if (mainForm.Parent.OpFlagFeedOn)
                    {
                        break;
                    }
                    if (localSw.ElapsedMilliseconds > timeout)
                    {
                        BlockAction.InterruptFlag = true;
                        //タイムアウト処理
                        TimeoutTouchTrigger();
                        break;
                    }
                }

                localSw.Stop();
            });
        }
        #endregion
    }
}
