using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Compartment
{
    public class UcOperationDataStore : IDisposable
    {
        /// <summary>
        /// レシピデータ(String型のID<Key>, String型のActionParam<Value>)
        /// </summary>
        private readonly ConcurrentDictionary<string, FileRelatedActionParam> operationKeyValuePairs;

        /// <summary>
        /// 固定のディレクトリパス
        /// </summary>
        private readonly string directory = ".\\";
        // ファイル無い場合は作るがこのデフォルトはやめる
        private readonly string defaultFileName = "_latestOperationProc.json";
        // データストアファイル名
        private readonly string dataStoreFileName = "datastore.dat";

        /// <summary>
        /// インスタンス作成
        /// </summary>
        public UcOperationDataStore()
        {
            operationKeyValuePairs = new ConcurrentDictionary<string, FileRelatedActionParam>();
            if (File.Exists(dataStoreFileName))
            {
                string readStrings = File.ReadAllText(dataStoreFileName);
                try
                {
                    operationKeyValuePairs = JsonConvert.DeserializeObject<ConcurrentDictionary<string, FileRelatedActionParam>>(readStrings);

                    if (!CheckPath())
                    {
                        operationKeyValuePairs = new ConcurrentDictionary<string, FileRelatedActionParam>();
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }

        }

        public string ReadDefaultPath(string id)
        {
            return directory + "/" + id + defaultFileName;
        }
        /// <summary>
        /// データストアの保存処理
        /// デストラクタで実行するのは暫定
        /// </summary>
        public void Dispose()
        {
            var json = JsonConvert.SerializeObject(operationKeyValuePairs, Formatting.Indented);
            try
            {
                File.WriteAllText(dataStoreFileName, json);
            }
            catch (Exception)
            {
                MessageBox.Show("データストアの保存に失敗しました。", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        /// <summary>
        /// 最終実行データのレシピデータ読み込み処理（共通:0, 固有:1以上）
        /// datastore.datに変更したので、この処理は廃止
        /// </summary>
        /// <param name="id"></param>
        public void ReadUniqueAddOrUpdateOperation(string id)
        {
            try
            {
                // 共通:0, 固有:1以上
                FileRelatedActionParam fileRelatedActionParam = new FileRelatedActionParam();
                if (File.Exists(directory + "\\" + id + defaultFileName))
                {
                    // file listは別で持つのでこの導線は廃止
                    string latestJsonFilePath = directory + "\\" + id + defaultFileName;
                    var jsonOperation = File.ReadAllText(latestJsonFilePath);
                    // FileRelatedActionParam->Jsonを返す
                    var dser = JsonConvert.DeserializeObject<FileRelatedActionParam>(jsonOperation);
                    fileRelatedActionParam = dser;

                    operationKeyValuePairs.AddOrUpdate(id, fileRelatedActionParam, (key, oldValue) => fileRelatedActionParam);
                }
                else
                {
                    var actionParams = UcOperationBlock.GetDefaultActionParamJson();
                    fileRelatedActionParam.FilePath = defaultFileName;
                    fileRelatedActionParam.ActionParams = actionParams;
                    operationKeyValuePairs.AddOrUpdate(id, fileRelatedActionParam, (key, oldValue) => fileRelatedActionParam);
                }
            }
            catch (System.IO.IOException)
            {
                // ファイルが破損している場合の処理
                MessageBox.Show("Block Programingの保存データで読み込めないファイルを確認しました。この場合、読み込めないファイルを削除後に新しくデータを再作成する必要があります。OKボタンを押下後、対象の読み込めないファイルの削除を試みます。", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                try
                {
                    if (File.Exists(directory + "\\" + id + defaultFileName))
                    {
                        File.Delete(directory + "\\" + id + defaultFileName);
                        MessageBox.Show("読み込めないファイルを削除するのに成功しました。対象ID:" + id + "。このまま操作を続け、Block Programingで対象IDデータを再作成してください。削除ファイル: BlockProgramingData / " + id + "_latestOperationProc.json", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("読み込めないファイルを削除するのに失敗しました。対象ID:" + id + "。手動でファイルの削除を試み、その後にBlock Programingで対象IDデータを再作成してください。対象ファイル:BlockProgramingData/" + id + "_latestOperationProc.json", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        /// <summary>
        /// レシピデータファイルのID取得
        /// </summary>
        /// <returns></returns>
        public List<string> GetFileIDs()
        {
            List<string> ids = new List<string>();
            // エントリ一元化する
            Regex pattern = new Regex(@"(\d+)_latestOperationProc\.json");

            foreach (string file in Directory.GetFiles(directory))
            {
                string filename = Path.GetFileName(file);
                Match match = pattern.Match(filename);
                if (match.Success)
                {
                    ids.Add(match.Groups[1].Value);
                }
            }

            return ids;
        }

        public List<string> GetIDs()
        {
            List<string> ids = new List<string>(operationKeyValuePairs.Keys);
            return ids;
        }

        public List<string> GetIDsFromDictionary()
        {
            List<string> ids = new List<string>(operationKeyValuePairs.Keys);
            return ids;
        }

        /// <summary>
        /// レシピデータを追加
        /// </summary>
        /// <param name="id"></param>
        /// <param name="operation"></param>
        public void AddOrUpdateOperation(string id, FileRelatedActionParam operation)
        {
            _ = operationKeyValuePairs.AddOrUpdate(id, operation, (_, value) => operation);
        }

        /// <summary>
        /// 格納データに対象のIDデータが有るかを確認
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool CheckContainKeys(string id)
        {
            return operationKeyValuePairs.ContainsKey(id);
        }

        /// <summary>
        /// レシピデータを取得（共通:0, 固有:1以上）
        /// </summary>
        /// <param name="id"></param>
        /// <param name="operation"></param>
        /// <returns>string</returns>
        public string ReadStringOperation(string id)
        {
            if (operationKeyValuePairs.ContainsKey(id))
            {
                return operationKeyValuePairs[id].ToString();
            }
            else { return ""; }
        }
        public FileRelatedActionParam GetEntry(string id)
        {
            if (operationKeyValuePairs.ContainsKey(id))
            {
                return operationKeyValuePairs[id];
            }
            else { return null; }
        }
        /// <summary>
        /// 対象IDのレシピデータを取り除く
        /// </summary>
        /// <param name="id"></param>
        public void RemoveEntry(string id)
        {
            _ = operationKeyValuePairs.TryRemove(id, out _);
        }

        /// <summary>
        /// レシピデータをクリア
        /// </summary>
        public void ClearEntry()
        {
            operationKeyValuePairs.Clear();
        }
        /// <summary>
        /// レシピデータのファイルパスが空白かを確認
        /// </summary>
        /// <returns> List<Tuple<bool, string>> </returns>
        public List<Tuple<bool, string>> CheckEmptyPath()
        {
            List<Tuple<bool, string>> emptyPathList = new List<Tuple<bool, string>>();
            foreach (var entry in operationKeyValuePairs)
            {
                if (entry.Value.FilePath == "")
                {
                    emptyPathList.Add(new Tuple<bool, string>(true, entry.Key.ToString()));
                }
            }
            return emptyPathList;
        }

        public bool CheckPath()
        {
            foreach (var entry in operationKeyValuePairs)
            {
                if (!File.Exists(entry.Value.FilePath))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
