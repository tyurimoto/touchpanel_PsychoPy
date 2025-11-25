using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Compartment
{
    public class ParentHelper<T>
    {
        private T _Parent = default;

        public T Parent => _Parent;

        public void SetParent(T parent)
        {
            _Parent = parent;
        }
    }

    public class DictAction
    {
        readonly Dictionary<string, object> mainDict = new Dictionary<string, object>();

        public object GetAction(string funcName)
        {
            try
            {
                if (mainDict.Count(f => f.Key == funcName) == 0)
                {
                    throw new KeyNotFoundException("Key not found.");
                }
                if (mainDict.TryGetValue(funcName, out var currentAction))
                {
                    return currentAction;
                }
                throw new KeyNotFoundException("Key not found.");
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void SetAction(Action<int> act, string methodName)
        {
            if (mainDict.Count(f => f.Key == methodName) == 0)
            {
                mainDict.Add(methodName, act);
            }
        }
        public void SetAction(Action<int, int> act, string methodName)
        {
            if (mainDict.Count(f => f.Key == methodName) == 0)
            {
                mainDict.Add(methodName, act);
            }
        }
        public void SetAction(Action<int, int, string> act, string methodName)
        {
            if (mainDict.Count(f => f.Key == methodName) == 0)
            {
                mainDict.Add(methodName, act);
            }
        }
        public void RemoveAction(Action<int> act)
        {
            mainDict.Remove(act.Method.Name);
        }
    }

    //Suboptionお試し用
    //未使用
    public class DictActionSuboptions
    {
        readonly Dictionary<string, List<object>> mainDict = new Dictionary<string, List<object>>();

        List<string> KeyTable
        {
            get => keyTable;
            set => keyTable = value;
        }
        List<string> keyTable = new List<string>();

        public Action GetAction(string funcName, params int[] optionParam)
        {
            if (mainDict.Count(f => f.Key == funcName) == 0)
            {
                throw new Exception("Key not found.");
            }
            var entry = mainDict[funcName];

            if (entry.Count == 2)
            {
                var func = entry[0] as Action<int>;
                return () => { func(optionParam[0]); };
            }
            else if (entry.Count == 3)
            {
                var func = entry[0] as Action<int, int>;
                return () => { func(optionParam[0], optionParam[1]); };

            }
            else
            {
                var func = entry[0] as Action<int>;
                return () => { func(0); };
            }
        }

        public void DoAction(string funcName, params int[] optionParam)
        {
            GetAction(funcName)();
        }
        public void SetAction(Action<int> act, string methodName, params int[] arg)
        {
            List<object> addFuncData = new List<object>();

            KeyTable.Add(methodName);
            addFuncData.Add(act);

            foreach (int s in arg)
            {
                addFuncData.Add(s);
            }

            if (mainDict.Count(f => f.Key == methodName) == 0)
            {
                if (addFuncData?.Count > 0)
                {
                    mainDict.Add(methodName, addFuncData);
                }
            }
        }
        public void RemoveAction(Action act)
        {
            mainDict.Remove(act.Method.Name);
        }
    }

    /// <summary>
    /// OperationActionHelperクラス
    /// </summary>
    public class OperationActionHelper
    {
        private Action _RunAction = () => { };

        public bool TaskBusy { get { InterruptFlag = false; return _TaskBusy.Value; } private set => _TaskBusy.Value = value; }

        private SyncObject<bool> _TaskBusy = new SyncObject<bool>(false);

        private ConcurrentQueue<Action> internalStartingActionQueue = new ConcurrentQueue<Action>();
        private ConcurrentQueue<Action> internalEndingActionQueue = new ConcurrentQueue<Action>();
        private ConcurrentQueue<Tuple<Action, bool, bool>> internalActionQueueAsync = new ConcurrentQueue<Tuple<Action, bool, bool>>();
        private ConcurrentQueue<Tuple<Action, bool, bool>> resumeActionQueueAsync = new ConcurrentQueue<Tuple<Action, bool, bool>>();
        private List<Task> listTask = new List<Task>();
        private CancellationTokenSource cancellationTokenSource;
        public int RepeatCount { get; set; } = 1;

        public bool Infinity { get; set; } = false;

        /// <summary>
        /// 割り込みフラグ
        /// </summary>
        public bool InterruptFlag { get => _InterruptFlag.Value; set => _InterruptFlag.Value = value; }

        private readonly SyncObject<bool> _InterruptFlag = new SyncObject<bool>(false);

        /// <summary>
        /// 不正解中断フラグ
        /// </summary>
        public bool IncorrectFlag { get => _IncorrectFlag.Value; set => _IncorrectFlag.Value = value; }

        private readonly SyncObject<bool> _IncorrectFlag = new SyncObject<bool>(false);
        /// <summary>
        /// 再入フラグ
        /// </summary>
        public bool ReEntryFlag { get => _ReEntryFlag.Value; set => _ReEntryFlag.Value = value; }
        private readonly SyncObject<bool> _ReEntryFlag = new SyncObject<bool>(false);

        public int CurrentActionCount => internalActionQueueAsync.Count;

        /// <summary>
        /// ConcurrentQueueセット
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="baseQueue"></param>
        /// <param name="setQueue"></param>
        public static void SetConcurrentQueue<T>(ref ConcurrentQueue<T> baseQueue, ConcurrentQueue<T> setQueue)
        {
            var newQueue = new ConcurrentQueue<T>(setQueue);
            _ = Interlocked.Exchange(ref baseQueue, newQueue);
        }

        /// <summary>
        /// ConcurrentQueueクリア
        /// Clearメソッドはスレッドセーフでないので空のQueueを置き換える
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queue"></param>
        public static void ClearConcurrentQueue<T>(ref ConcurrentQueue<T> queue)
        {
            var newQueue = new ConcurrentQueue<T>();
            _ = Interlocked.Exchange(ref queue, newQueue);
        }

        public OperationActionHelper()
        {
        }
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="repeatCount">繰り返し回数 0:Infinity</param>
        public OperationActionHelper(int repeatCount)
        {
            RepeatCount = repeatCount;
        }

        /// <summary>
        /// 通常実行
        /// </summary>
        public void DoAction()
        {
            _RunAction();
            ClearAction();
        }
        /// <summary>
        /// Taskで実行
        /// </summary>
        public async void DoTask()
        {
            await Task.Run(_RunAction);
        }
        /// <summary>
        /// 登録ActionをTaskで全実行
        /// RepeatCount分繰り返す 0の場合は1回実行
        /// 全実行が終わったらActionをクリア 毎回SetActionで登録する必要がある
        /// </summary>
        public async Task DoAllTask()
        {
            cancellationTokenSource = new CancellationTokenSource();
            if (!TaskBusy)
            {
                TaskBusy = true;
            }

            ConcurrentQueue<Tuple<Action, bool, bool>> queue = new ConcurrentQueue<Tuple<Action, bool, bool>>();
            ConcurrentQueue<Action> startingQueue = new ConcurrentQueue<Action>();
            ConcurrentQueue<Action> endingQueue = new ConcurrentQueue<Action>();
            
            //無限回
            if (Infinity)
            {

                while (!InterruptFlag)
                {
                    SetConcurrentQueue(ref queue, internalActionQueueAsync);
                    SetConcurrentQueue(ref startingQueue, internalStartingActionQueue);
                    SetConcurrentQueue(ref endingQueue, internalEndingActionQueue);
                    DoStartingTaskProc(startingQueue);
                    await internalDoTaskProc(startingQueue, queue, endingQueue);
                    DoEndingTaskProc(endingQueue);
                }
            }
            //回数指定
            else
            {
                for (int i = 0; i < RepeatCount; i++)
                {
                    SetConcurrentQueue(ref queue, internalActionQueueAsync);
                    SetConcurrentQueue(ref startingQueue, internalStartingActionQueue);
                    SetConcurrentQueue(ref endingQueue, internalEndingActionQueue);
                    DoStartingTaskProc(startingQueue);
                    await internalDoTaskProc(startingQueue, queue, endingQueue);
                    DoEndingTaskProc(endingQueue);
                }
            }
            //TaskBusy = false;
            // 前回のTaskを消す　ClearAction内でTaskBusyをfalseにしているので重複させない
            ClearAction();
        }
        /// <summary>
        /// DoTask内分解
        /// </summary>
        /// <param name="queue"></param>
        /// <param name="endingQueue"></param>
        /// <returns></returns>
        private async Task internalDoTaskProc(ConcurrentQueue<Action> startingQueue, ConcurrentQueue<Tuple<Action, bool, bool>> queue, ConcurrentQueue<Action> endingQueue)
        {
            Tuple<Action, bool, bool> action;

            while (0 < queue.Count)
            {
                if (ReEntryFlag)
                {
                    ReEntryFlag = false;
                    action = GetReplayAction(queue, resumeActionQueueAsync);
                }
                else
                {
                    _ = queue.TryDequeue(out action);
                }
                if (action is null)
                    return;
                _RunAction = action.Item1;
                var asyncFlag = action.Item2;

                if (!asyncFlag)
                {
                    await Task.Run(_RunAction, cancellationTokenSource.Token);
                }
                else
                {
                    //非同期側積まれたらすぐ実行されるので非同期ディレイがない
                    Task t = Task.Run(_RunAction, cancellationTokenSource.Token);
                    listTask.Add(t);
                }

                if (InterruptFlag)
                {
                    // レジューム用ActionQueueコピーしておく
                    SetConcurrentQueue(ref resumeActionQueueAsync, queue);
                    ClearConcurrentQueue(ref queue);
                    // 本来はこう
                    ClearConcurrentQueue(ref internalActionQueueAsync);
                    break;
                }
                if (IncorrectFlag)
                {
                    ClearConcurrentQueue(ref queue);
                    // 本来はこう
                    ClearConcurrentQueue(ref internalActionQueueAsync);
                    IncorrectFlag = false;
                    break;
                }
            }
            // ここで全体タスク完了を見る
            _ = Task.WaitAll(listTask?.ToArray(), 100000, cancellationTokenSource.Token);
        }
        /// <summary>
        /// StartingTask
        /// </summary>
        private void DoStartingTaskProc(ConcurrentQueue<Action> startQueue)
        {
            while (0 < startQueue.Count)
            {
                if (startQueue.Count > 3)
                {
                    Debug.WriteLine("hoge");
                }
                if (!InterruptFlag)
                {
                    _ = startQueue.TryDequeue(out Action startingAct);
                    startingAct();
                }
                else
                {
                    ClearConcurrentQueue(ref startQueue);
                    break;
                }
            }
        }
        /// <summary>
        /// EndingTask
        /// </summary>
        private void DoEndingTaskProc(ConcurrentQueue<Action> endingQueue)
        {
            while (0 < endingQueue.Count)
            {
                if (!InterruptFlag)
                {
                    _ = endingQueue.TryDequeue(out Action endingAct);
                    endingAct();
                }
                else
                {
                    ClearConcurrentQueue(ref endingQueue);
                    break;
                }
            }
        }

        /// <summary>
        /// リプレイ用アクションを取得する
        /// ReEntryFlagがtrueの時のみ動作
        /// mainActionQueueの中でReplayEnableなActionまで巻き戻す
        /// </summary>
        /// <param name="mainActionQueue"></param>
        /// <param name="redumeActionQueue"></param>
        /// <returns></returns>
        private Tuple<Action, bool, bool> GetReplayAction(ConcurrentQueue<Tuple<Action, bool, bool>> mainActionQueue, ConcurrentQueue<Tuple<Action, bool, bool>> redumeActionQueue)
        {
            Tuple<Action, bool, bool> action;
            Tuple<Action, bool, bool> tempAction;
            int reEntryOffsetCount = 2; //検索数 手前個数分

            // コスト高・・・
            ConcurrentQueue<Tuple<Action, bool, bool>> inputQueueBackup = new ConcurrentQueue<Tuple<Action, bool, bool>>(mainActionQueue);

            if (mainActionQueue.Count == 0)
            {
                return null;
            }

            while (true)
            {
                //関数内で外部Queueをいじるあまりよくない実装
                _ = mainActionQueue.TryDequeue(out tempAction);

                // redumeActionQueueが0条件 reEntryOffetQueueカウント以下条件 以降にresume可能アクションがない場合

                if (redumeActionQueue.Count == 0)
                {
                    action = tempAction;
                    break;
                }
                if (tempAction.Item3 && redumeActionQueue.Count + reEntryOffsetCount >= mainActionQueue.Count)
                {
                    action = tempAction;
                    break;
                }
                // resume可能アクションがない場合最後までDequeueされるのでmainActionQueueを書き戻す
                if (mainActionQueue.Count == 0)
                {
                    //throw new Exception("Not found replay action.");
                    mainActionQueue = inputQueueBackup;
                    _ = mainActionQueue.TryDequeue(out action);
                    break;
                }
            }
            return action;
        }
        /// <summary>
        /// 登録Actionをクリア
        /// </summary>
        public void ClearAction()
        {
            if (TaskBusy)
            {
                //InterruptFlag = true;
            }
            TaskBusy = false;
            cancellationTokenSource?.Cancel();
            _RunAction = () => { };

            //両方のQueue内をクリア
            ClearConcurrentQueue(ref internalActionQueueAsync);
            ClearConcurrentQueue(ref internalStartingActionQueue);
            ClearConcurrentQueue(ref internalEndingActionQueue);
        }

        /// <summary>
        /// Actionを登録
        /// </summary>
        /// <param name="act">Action</param>
        public void SetAction(Action act)
        {
            SetAction(act, false, false);
        }

        /// <summary>
        /// 非同期Actionを登録
        /// </summary>
        /// <param name="act"></param>
        /// <param name="asyncAction">Action</param>
        public void SetActionAsync(Action act)
        {
            SetAction(act, true, false);
        }
        /// <summary>
        /// 再入時再開可能Actionを登録
        /// </summary>
        /// <param name="act"></param>
        public void SetActionEnableReplay(Action act)
        {
            SetAction(act, false, true);
        }

        /// <summary>
        /// Action登録内部
        /// </summary>
        /// <param name="act">Action</param>
        /// <param name="asyncFlag">非同期フラグ</param>
        private void SetAction(Action act, bool asyncFlag, bool enableReplay)
        {
            if (TaskBusy)
            {
                TaskBusy = false;
                cancellationTokenSource?.Cancel();
            }
            // Asyncフラグ
            var tuple = new Tuple<Action, bool, bool>(act, asyncFlag, enableReplay);
            internalActionQueueAsync.Enqueue(tuple);
        }
        public void SetStartingAction(Action act)
        {
            internalStartingActionQueue.Enqueue(act);
        }
        public void SetEndingAction(Action act)
        {
            internalEndingActionQueue.Enqueue(act);
        }
    }
    internal static class ConcurrentQueueExtensions
    {
        public static void Clear<T>(this ConcurrentQueue<T> queue)
        {
            T item;
            while (queue.TryDequeue(out item))
            {
            }
        }
    }
}
