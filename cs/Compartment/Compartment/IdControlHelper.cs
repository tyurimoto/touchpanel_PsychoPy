using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Compartment
{
    // FindKey -> ContainsKey
    // ReleaseKey 時間による
    // AddData
    // 他有効ID閲覧などを実装する

    /// <summary>
    /// ID管理
    /// </summary>
    public class IdControlHelper
    {
        private readonly ConcurrentDictionary<string, Tuple<long, int>> keyValuePairs;
        private long _ExpireTicks = TimeSpan.TicksPerMinute * 10;
        /// <summary>
        /// Expire 単位:分
        /// </summary>
        public long ExpireTime
        {
            get => _ExpireTicks / TimeSpan.TicksPerMinute;
            set => _ExpireTicks = TimeSpan.TicksPerMinute * value;
        }
        /// <summary>
        /// 登録されているID数返す
        /// </summary>
        public int KeyPairsCount
        {
            get => keyValuePairs.Count;
        }
        public IEnumerable<string> Keys
        {
            get => keyValuePairs.Keys;
        }
        /// <summary>
        /// ファイルにIDを書き出す
        /// </summary>
        /// <param name="fileName">ファイル名</param>
        public void SaveId(string fileName)
        {
            JsonSerializer serializer = new JsonSerializer
            {
                NullValueHandling = NullValueHandling.Ignore
            };

            //上書き設定
            using (StreamWriter sw = new StreamWriter(fileName, false))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                if (keyValuePairs.Count > 0)
                    serializer.Serialize(writer, keyValuePairs);
            }
        }

        public IdControlHelper()
        {
            keyValuePairs = CreateSourceDictionary();
        }
        public IdControlHelper(string fileName)
        {
            //keyValuePairs.Clear();
            keyValuePairs = LoadDictionary(fileName);
            if (keyValuePairs is null)
            {
                keyValuePairs = CreateSourceDictionary();
            }
        }
        /// <summary>
        /// ID エピソード記憶実行回数を取得
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int GetCount(string id)
        {
            if (keyValuePairs.ContainsKey(id))
            {
                return keyValuePairs[id].Item2;
            }
            return 0;
        }
        /// <summary>
        /// 前回の入室から経過時間経っているか返す
        /// </summary>
        /// <param name="id">ID</param>
        /// <param name="checkSeconds">second</param>
        /// <returns></returns>
        public bool CheckElaspedTime(string id, int checkSeconds)
        {
            //Entry時間をどこで更新するかが問題
            long expireTime = TimeSpan.FromSeconds(checkSeconds).Ticks;
            long entryTime = GetEntry(id).Item1;
            long elaspedTime = DateTime.Now.Ticks - entryTime;
            TimeSpan ts = new TimeSpan(elaspedTime);
            Debug.WriteLine("Entry:" + ts.TotalMilliseconds);
            return expireTime < elaspedTime;

        }
        /// <summary>
        /// IDからentry取得
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns>long,int</returns>
        public Tuple<long, int> GetEntry(string id)
        {
            _ = keyValuePairs.TryGetValue(id, out Tuple<long, int> entry);
            return entry;
        }
        /// <summary>
        /// IDの試行カウントを追加
        /// </summary>
        /// <param name="id">ID</param>
        public void AddCount(string id)
        {
            if (keyValuePairs.ContainsKey(id))
            {
                _ = keyValuePairs.TryUpdate(id, new Tuple<long, int>(keyValuePairs[id].Item1, keyValuePairs[id].Item2 + 1), new Tuple<long, int>(keyValuePairs[id].Item1, keyValuePairs[id].Item2));
            }
        }
        /// <summary>
        /// IDの試行カウントを0リセット
        /// </summary>
        /// <param name="id"></param>
        public void ResetCount(string id)
        {
            if (keyValuePairs.ContainsKey(id))
            {
                _ = keyValuePairs.TryUpdate(id, new Tuple<long, int>(keyValuePairs[id].Item1, 0), new Tuple<long, int>(keyValuePairs[id].Item1, keyValuePairs[id].Item2));
            }
        }
        /// <summary>
        /// ExpireしたIDを削除
        /// </summary>
        public void CheckExpire()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            long nowTick = DateTime.Now.Ticks;
            foreach (KeyValuePair<string, Tuple<long, int>> pair in keyValuePairs)
            {
                if (nowTick - pair.Value.Item1 > _ExpireTicks)
                {
                    keyValuePairs.TryRemove(pair.Key, out var n);
                    //Debug.WriteLine(n.ToString() + @"\n");
                }
            }
            //Parallel.ForEach(keyValuePairs, pair =>
            //{
            //    if (nowTick - pair.Value.Item1 > _ExpireTicks)
            //    {
            //        keyValuePairs.TryRemove(pair.Key, out var n);
            //        //Debug.WriteLine(n.ToString() + @"\n");
            //    }
            //});
            stopwatch.Stop();
            Debug.WriteLine(stopwatch.ElapsedMilliseconds.ToString() + "ms");
        }
        /// <summary>
        /// IDを追加
        /// </summary>
        /// <param name="id"></param>
        public void AddEntry(string id)
        {
            _ = keyValuePairs.AddOrUpdate(id, new Tuple<long, int>(DateTime.Now.Ticks, 0), (_, value) => new Tuple<long, int>(DateTime.Now.Ticks, 0));
        }

        public void UpdateEntry(string id)
        {
            if (keyValuePairs.ContainsKey(id))
            {
                _ = keyValuePairs.TryUpdate(id, new Tuple<long, int>(DateTime.Now.Ticks, keyValuePairs[id].Item2), new Tuple<long, int>(keyValuePairs[id].Item1, keyValuePairs[id].Item2));
            }
        }
        /// <summary>
        /// Entryを除去
        /// </summary>
        /// <param name="id">ID</param>
        public void RemoveEntry(string id)
        {
            _ = keyValuePairs.TryRemove(id, out _);
        }
        /// <summary>
        /// Entryを全消去
        /// </summary>
        public void ClearEntry()
        {
            keyValuePairs.Clear();
        }
        /// <summary>
        /// IDが存在するか確認
        /// </summary>
        /// <param name="key">ID</param>
        /// <returns>bool</returns>
        public bool FindId(string key)
        {
            return keyValuePairs.ContainsKey(key);
        }
        /// <summary>
        /// 辞書初期化
        /// </summary>
        /// <returns>ConcurrentDictionary<string, Tuple<long, int>></returns>
        public ConcurrentDictionary<string, Tuple<long, int>> CreateSourceDictionary()
        => new ConcurrentDictionary<string, Tuple<long, int>>
        {

            ["392144000336485"] = new Tuple<long, int>(10, 10),
            ["392144000347682"] = new Tuple<long, int>(5, 0),
        };
        /// <summary>
        /// ファイルからConcurrentDictionaryを読む
        /// </summary>
        /// <param name="fileName">ファイル名</param>
        /// <returns>ConcurrentDictionary<string, Tuple<long, int>></returns>
        public ConcurrentDictionary<string, Tuple<long, int>> LoadDictionary(string fileName)
        {
            if (File.Exists(fileName))
            {
                var json = File.ReadAllText(fileName);
                if (json != "")
                {
                    return JsonConvert.DeserializeObject<ConcurrentDictionary<string, Tuple<long, int>>>(json);
                }
            }
            return null;
        }


        /*
        /// <summary>
        /// 取得時Value無しならDefault値を返す
        /// 基本これでID確認
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dict"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key)
            => dict.TryGetValue(key, out TValue result) ? result : default(TValue);
        /// <summary>
        /// TValue 追加
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dict"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool TryAddNew<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key) where TValue : new()
            => dict.TryAdd(key, _ => new TValue());

        public static bool TryAddDefault<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key)
            => dict.TryAdd(key, default(TValue));

        public static bool TryAdd<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue addValue)
        {
            bool canAdd = !dict.ContainsKey(key);

            if (canAdd)
                dict.Add(key, addValue);

            return canAdd;
        }

        public static bool TryAdd<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, Func<TKey, TValue> addValueFactory)
        {
            bool canAdd = !dict.ContainsKey(key);

            if (canAdd)
                dict.Add(key, addValueFactory(key));

            return canAdd;
        }

        public static TValue GetOrAddNew<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key) where TValue : new()
        {
            dict.TryAddNew(key);
            return dict[key];
        }

        public static TValue GetOrAddDefault<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key)
        {
            dict.TryAddDefault(key);
            return dict[key];
        }

        public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue addValue)
        {
            dict.TryAdd(key, addValue);
            return dict[key];
        }

        public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, Func<TKey, TValue> valueFactory)
        {
            dict.TryAdd(key, valueFactory);
            return dict[key];
        }
        public static void Add<TKey, TValue>(this IDictionary<TKey, TValue> source, KeyValuePair<TKey, TValue> addPair)
            => source.Add(addPair.Key, addPair.Value);
        public static Dictionary<Tkey, TValue> ToDictionary<Tkey, TValue>(this IEnumerable<KeyValuePair<Tkey, TValue>> source)
            => source.ToDictionary(
                keySelector: kv => kv.Key,
                elementSelector: kv => kv.Value);
        */
    }
}
