using System.Collections.Concurrent;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Converters;

namespace Compartment
{
    /// <summary>
    /// 出題記録保存クラス
    /// </summary>
    public class EpisodeMemory
    {
        private readonly ConcurrentDictionary<string, ShapeObject[]> keyValuePairs;
        public EpisodeMemory()
        {
            keyValuePairs = new ConcurrentDictionary<string, ShapeObject[]>();
        }

        public EpisodeMemory(string fileName)
        {
            try
            {
                if (File.Exists(fileName))
                {
                    var json = File.ReadAllText(fileName);

                    if (json != "")
                    {
                        keyValuePairs = JsonConvert.DeserializeObject<ConcurrentDictionary<string, ShapeObject[]>>(json);
                    }
                }
                else
                {
                    keyValuePairs = new ConcurrentDictionary<string, ShapeObject[]>();
                }
            }
            catch (System.ArgumentException)
            {
                File.Delete(fileName);
            }
        }
        public void AddOrUpdateShapObject(string id, ShapeObject[] sos)
        {
            _ = keyValuePairs.AddOrUpdate(id, sos, (_, value) => sos);
        }
        public bool ContainsKey(string id)
        {
            return keyValuePairs.ContainsKey(id);
        }

        public void SaveKeys(string fileName)
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
                {
                    serializer.Serialize(writer, keyValuePairs);
                }
            }
        }

        public ShapeObject[] ReadShapeObject(string id)
        {
            if (keyValuePairs.ContainsKey(id))
            {
                return keyValuePairs[id];
            }
            else { return null; }
        }
        public void AddEntry(string id, ShapeObject[] so)
        {
            _ = keyValuePairs.TryAdd(id, so);
        }
        public void RemoveEntry(string id)
        {
            _ = keyValuePairs.TryRemove(id, out _);
        }

        public void ClearEntry()
        {
            keyValuePairs.Clear();
        }

        public int GetCount()
        {
            return keyValuePairs?.Count ?? 0;
        }
        public ShapeObject ReadCorrectShapeObject(string id)
        {
            if (keyValuePairs.ContainsKey(id))
            {
                ShapeObject[] shapeObjects;
                _ = keyValuePairs.TryGetValue(id, out shapeObjects);
                return shapeObjects[0];
            }
            else
            {
                return null;
            }
        }
        public ShapeObject[] ReadIncorrectShapeObjects(string id)
        {
            // IncorrectShapeObjectサイズに合わせてリサイズ
            // して格納

            if (keyValuePairs.ContainsKey(id))
            {
                ShapeObject[] shapeObjects;
                _ = keyValuePairs.TryGetValue(id, out shapeObjects);
                ShapeObject[] retShapeObjects = new ShapeObject[shapeObjects.Length - 1];
                for (int i = 1; i < shapeObjects.Length; i++)
                {
                    retShapeObjects[i - 1] = shapeObjects[i];
                }
                return retShapeObjects;
            }
            else
            {
                return null;
            }
        }
    }


}
