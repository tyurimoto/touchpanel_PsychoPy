using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Compartment
{
    public class ImageLoader
    {
        public static readonly string[] SupportExtension = { ".jpg", ".png", ".bmp" };

        private string _fileDirectory;
        private Func<string, Bitmap> GetBitmap;
        private Func<List<string>> GetFileList;
        private static Dictionary<int, Size> sizeDict = new Dictionary<int, Size>()
        {
            {1, new Size(50,50)},
            {2, new Size(75,75)},
            {3, new Size(100,100)},
            {4, new Size(125,125)},
            {5, new Size(150,150)},
        };

        /// <summary>
        /// ImageLoaderコンストラクタ
        /// </summary>
        /// <param name="path">管理するディレクトリ</param>
        public ImageLoader(string path)
        {
            _fileDirectory = path;
            GetBitmap = (filename) => { return new Bitmap(System.IO.Path.Combine(_fileDirectory, filename)); };
            GetFileList = () => { return System.IO.Directory.GetFiles(_fileDirectory).Select(f => System.IO.Path.GetFileName(f)).ToList(); };
        }

        public ImageLoader(string path, Func<string, Bitmap> getbmp, Func<List<string>> getlist)
        {
            _fileDirectory = path;
            GetBitmap = getbmp;
            GetFileList = getlist;
        }

        /// <summary>
        /// 正解画像と指定された数のランダムな不正解画像のBitMapをリストとして返す
        /// </summary>
        /// <param name="correctImage">正解画像ファイル名</param>
        /// <param name="incorrectNum">不正解画像の枚数</param>
        /// <param name="incorrectVariety">不正解画像が最低何種類か</param>
        /// <param name="steps">指定サイズのリスト</param>
        public List<Bitmap> LoadImages(string correctImage, int incorrectNum, int incorrectVariety, List<int> steps)
        {
            var images = new List<Bitmap>();
            var incorrectList = GetFileList().Where(f => f != correctImage).ToList();

            images.Add(GetBitmap(correctImage));
            foreach (var f in RandomPick<string>.PickWithoutDuplicate(incorrectList, incorrectVariety))
            {
                images.Add(GetBitmap(f));
            };
            if (incorrectNum > incorrectVariety)
            {
                foreach (var f in RandomPick<string>.PickWithDuplicate(incorrectList, incorrectNum - incorrectVariety))
                {
                    images.Add(GetBitmap(f));
                };
            }

            var resizedImages = images.Zip(steps, (bmp, s) => Resize(bmp, sizeDict[s])).ToList();
            return resizedImages;
        }
        public static Bitmap Resize(Bitmap original, Size size)
        {
            var brush = new SolidBrush(Color.Black);
            var resized = new Bitmap(size.Width, size.Height);
            Graphics graph = Graphics.FromImage(resized);

            graph.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graph.Clear(Color.Transparent);
            graph.DrawImage(original, 0, 0, size.Width, size.Height);
            return resized;
        }

        public static Bitmap LoadImage(string filename, Size size)
        {
            //きっと遅い
            //return new Bitmap(new Bitmap(filename), size);
            return Resize(new Bitmap(filename), size);
        }
        public static Bitmap LoadImage(string filename)
        {
            return new Bitmap(filename);
        }
        public static List<Bitmap> LoadMultiImages(string correctImage, int incorrectNum, string folder, Random rnd, Size size)
        {
            var list = new List<Bitmap>();
            var tempList = LoadMultiImages(correctImage, incorrectNum, folder, rnd);
            //ParallelOptions options = new ParallelOptions();
            //Parallel.For(0, tempList.Count - 1, options, (i, loopkstate) =>
            //  {
            //      list.Add(new Bitmap(tempList.ElementAt(i), size));
            //  }
            // );
            for (int i = 0; i < tempList.Count; i++)
            {
                list.Add(new Bitmap(tempList.ElementAt(i), size));
            }
            return list;
        }
        public static List<Bitmap> LoadMultiImages(string correctImage, int incorrectNum, string folder, Random rnd, Size size, out List<string> fileList)
        {
            var list = new List<Bitmap>();
            var tempList = LoadMultiImages(correctImage, incorrectNum, folder, rnd, out fileList);
            for (int i = 0; i < tempList.Count; i++)
            {
                list.Add(new Bitmap(tempList.ElementAt(i), size));
            }
            return list;
        }
        public static List<Bitmap> LoadMultiImages(string correctImage, int incorrectNum, string folder, Random rnd)
        {
            //Image file fix extension
            string[] patterns = SupportExtension;
            var incorrectList = System.IO.Directory.GetFiles(folder).Where(f => ((f != correctImage) && patterns.Any(pattern => f.ToLower().EndsWith(pattern)))).ToList();
            var list = new List<Bitmap>();
            for (int i = 0; i < incorrectNum; i++)
            {
                var selectionNum = rnd.Next(incorrectList.Count);
                list.Add(new Bitmap(incorrectList.ElementAt(selectionNum)));
            }
            return list;
        }
        public static List<Bitmap> LoadMultiImages(string correctImage, int incorrectNum, string folder, Random rnd, out List<string> incorrectList)
        {
            //Image file fix extension
            string[] patterns = SupportExtension;
            incorrectList = System.IO.Directory.GetFiles(folder).Where(f => ((f != correctImage) && patterns.Any(pattern => f.ToLower().EndsWith(pattern)))).ToList();
            var list = new List<Bitmap>();
            for (int i = 0; i < incorrectNum; i++)
            {
                var selectionNum = rnd.Next(incorrectList.Count);
                list.Add(new Bitmap(incorrectList.ElementAt(selectionNum)));
            }
            return list;
        }

    }
    public class RandomPick<T>
    {
        static public List<T> PickWithoutDuplicate(List<T> list, int num)
        {
            return list.OrderBy(f => Guid.NewGuid()).Take(num).ToList();
        }
        static public List<T> PickWithDuplicate(List<T> list, int num)
        {
            var picked = new List<T>();
            var random = new Random();
            for (int i = 0; i < num; i++)
            {
                picked.Add(list[random.Next(list.Count)]);
            }
            return picked;
        }
    }
}