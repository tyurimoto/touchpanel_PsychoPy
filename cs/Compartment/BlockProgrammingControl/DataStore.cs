using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace BlockProgramming
{
    internal class DataStore
    {
        public DataStore()
        {
            Items = new ObservableCollection<FunctionItem>();
            AddItems(
                "DrawScreenReset",
                new List<FuncArg>() {
                    //new FuncArg("param1", 0),
                    //new FuncArg("param2", 0)
                },
                "画面を黒色に書き換えます"
                );
            AddItems(
                "DrawScreenColor",
                new List<FuncArg>() {
                    //new FuncArg("param1", 0),
                    //new FuncArg("param2", 0)
                },
                "画面を指定色に書き換えます"
                );
            AddItems(
                "DrawScreenDelayColor",
                new List<FuncArg>() {
                    //new FuncArg("param1", 0),
                    //new FuncArg("param2", 0)
                },
                "画面をDelay指定色に書き換えます"
                );
            AddItems(
                "ViewTriggerImage",
                new List<FuncArg>() {
                    new PathFuncArg("画像ファイル", "", "Image Files(*.jpg;*.png;*.bmp)|*.jpg;*.png;*.bmp|JPG(*.jpg)|*.jpg|PNG(*.png)|*.png|BMP(*.bmp)|*.bmp", @"C:\"),
                    //new FuncArg("param2", 0)
                },
                "トリガータッチ画像を表示します"
                );
            AddItems(
                "WaitTouchTrigger",
                 new List<FuncArg>() {
                     //new ValueTypeFuncArg("待機時間", 0),
                     //new FuncArg("param2", 0)
                 },
                 "トリガータッチを待機します"
                 );
            AddItems(
                "Delay",
                 new List<FuncArg>() {
                     new ValueTypeFuncArg("最小値", 1000.0, 0, 10000),
                     new ValueTypeFuncArg("最大値", 1000, 0, 10000)
                 },
                 "Delayを設定します。最小-最大が異なる場合はその範囲でランダム値でDelayします"
                 );
            AddItems(
                "DelayEpisode",
                 new List<FuncArg>() {
                                 new ValueTypeFuncArg("最小値", 1000.0, 0, 10000),
                                 new ValueTypeFuncArg("最大値", 1000, 0, 10000)
                 },
                 "Episode時のDelayを設定します。最小-最大が異なる場合はその範囲でランダム値でDelayします"
                 );
            AddItems(
                "ViewCorrectImage",
                 new List<FuncArg>() {
                    new PathFuncArg("画像ファイル", "", "Image Files(*.jpg;*.png;*.bmp)|*.jpg;*.png;*.bmp|JPG(*.jpg)|*.jpg|PNG(*.png)|*.png|BMP(*.bmp)|*.bmp", @"C:\"),
                     //new FuncArg("param2", 0)
                 },
                 "正解画像を表示"
                 );
            AddItems(
                "ViewCorrectWrongImage",
                 new List<FuncArg>() {
                     //new PathFuncArg("画像ファイル", "","Image Files(*.jpg;*.png;*.bmp)|*.jpg;*.png;*.bmp|JPG(*.jpg)|*.jpg|PNG(*.png)|*.png|BMP(*.bmp)|*.bmp", @"C:\"),
                     //new FuncArg("param1", 0),
                     //new FuncArg("param2", 0)
                 },
                 "正解画像と不正解画像を表示。不正解画像は正解画像フォルダから選ばれます"
                 );
            AddItems(
                "WaitCorrectTouchTrigger",
                 new List<FuncArg>() {
                     //new FuncArg("param1", 0),
                     //new FuncArg("param2", 0)
                 },
                 "正解画像タッチ待ち待機"
                 );
            AddItems(
                "PlaySound",
                 new List<FuncArg>() {
                     //new PathFuncArg("音声ファイル", "", "Audio Files(*.wav;*.mp3)|*.wav;*.mp3|WAV(*.wav)|*.wav|MP3(*.mp3)|*.mp3", @"C:\"),
                     //new FuncArg("param2", 0)
                 },
                 "正解音を再生"
                 , true
                 );
            AddItems(
                "UserPlaySound",
                 new List<FuncArg>() {
                     new PathFuncArg("音声ファイル", "", "Audio Files(*.wav;*.mp3)|*.wav;*.mp3|WAV(*.wav)|*.wav|MP3(*.mp3)|*.mp3", @"C:\"),
                     //new FuncArg("param2", 0)
                 },
                 "任意の音声を再生"
                 , true
                 );
            AddItems(
                "FeedLamp",
                 new List<FuncArg>() {
                     new ValueTypeFuncArg("最小値ms", 1000, 1000, 10000),
                     new ValueTypeFuncArg("最大値ms", 1000, 1000, 10000)
                 },
                 "FeedLamp点灯。点灯時間を設定します"
                 , true
                 );
            AddItems(
                "Feed",
                 new List<FuncArg>() {
                     new ValueTypeFuncArg("最小値ms", 2000),
                     new ValueTypeFuncArg("最大値ms", 2000)
                 },
                 "Feedをします。Feed時間を設定します"
                 , true
                 );
            AddItems(
                "FeedSound",
                 new List<FuncArg>() {
                     new ValueTypeFuncArg("最小値ms", 1000),
                     new ValueTypeFuncArg("最大値ms", 1000),
                     //new PathFuncArg("音声ファイル", "", "Audio Files(*.wav;*.mp3)|*.wav;*.mp3|WAV(*.wav)|*.wav|MP3(*.mp3)|*.mp3", @"C:\"),
                 },
                 "FeedSoundを再生します"
                 , true
                 );
            AddItems(
                "OutputResult",
                 new List<FuncArg>() {
                     //new FuncArg("param1", 0),
                     //new FuncArg("param2", 0)
                 },
                 "結果出力を行います。一連の動作が終了したら設定してください"
                 );
            AddItems(
                "TouchDelay",
                 new List<FuncArg>() {
                     new ValueTypeFuncArg("最小時間ms", 1000),
                     new ValueTypeFuncArg("最大時間ms", 1000)
                 },
                 "TouchDelayを設定します。タッチされた場合後段の処理を遅延します。"
                 );
            AddItems(
                "TouchDelayWithLamp",
                 new List<FuncArg>() {
                                 new ValueTypeFuncArg("最小時間ms", 1000),
                                 new ValueTypeFuncArg("最大時間ms", 1000)
                 },
                 "TouchDelayを設定します。同時にDelay中RoomLampを点灯します。"
                 );
            //AddItems(
            //    "PathTest",
            //    new List<FuncArg>()
            //    {
            //        new PathFuncArg("画像Path", "PathString"),
            //    },
            //    "画像・音声ファイル設定"
            //    );
        }

        public void AddItems(string name, List<FuncArg> args, string description = "", bool isAsync = false)
        {
            if (Items is null) Items = new ObservableCollection<FunctionItem>();
            var id = Items.Count();
            var item = new FunctionItem(id)
            {
                Description = description,
                FuncName = name,
                IsAsync = isAsync
            };
            foreach (var arg in args)
            {
                item.FuncArgs.Add(arg);
            }
            Items.Add(item);
        }

        public void AddItems(string name, string description = "")
        {
            AddItems(name, new List<FuncArg>(), description);
        }

        #region Property
        public ObservableCollection<FunctionItem> Queue { get; set; }
        public ObservableCollection<FunctionItem> Items { get; set; }
        #endregion
    }


    [JsonObject]
    internal class Message
    {
        [JsonProperty("ActionName")]
        public string Name { get; set; }

        [JsonProperty("param1")]
        public int? Param1 { get; set; }

        [JsonProperty("param2")]
        public int? Param2 { get; set; }

        [JsonProperty("param3")]
        public string Param3 { get; set; } = null;

        public static int? GetIntValue(object value)
        {
            if (value == null) return 0;

            if(value is System.ValueType)
            {
                return System.Convert.ToInt32(value);
            }
            else if(value is string)
            {
                double v;
                if (double.TryParse((string)value, out v)) return System.Convert.ToInt32(v);
                return 0;
            }

            return 0;
        }
    }
}
