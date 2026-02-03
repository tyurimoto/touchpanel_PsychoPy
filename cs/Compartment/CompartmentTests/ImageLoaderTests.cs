using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Drawing;
using System.Collections.Generic;
using System.Diagnostics;
using System;
using Compartment;

namespace Compartment.Tests
{
    [TestClass]
    public class ImageLoaderTest
    {
        private readonly ImageLoader _imageLoader;

        public ImageLoaderTest()
        {
            string path = @"C:\Windows\Web\Screen";

            Func<string, Bitmap> bitmapDummy = (filename) => {
                var dict = new Dictionary<string, Color>()
                {
                    {"correct", Color.Green},
                    {"incorrect0", Color.Red},
                    {"incorrect1", Color.Blue},
                    {"incorrect2", Color.Purple},
                    {"incorrect3", Color.Cyan}
                };
                var b = new Bitmap(128,128);
                var g = Graphics.FromImage(b);
                g.Clear(dict[filename]);
                return b;
            };

            Func<List<string>> fileListDummy = () => { return new List<string>(){"correct", "incorrect0", "incorrect1", "incorrect2","incorrect3"};};

            _imageLoader = new ImageLoader(path, bitmapDummy, fileListDummy);
        }

        [TestMethod]
        public void ResizeTest()
        {
            var bmp = new Bitmap(5,5);
            var expectedSize = new Size(100,100);
            var resizedImage = ImageLoader.Resize(bmp, expectedSize);
            Assert.AreEqual(typeof(Bitmap), resizedImage.GetType());
            Assert.AreEqual(expectedSize, resizedImage.Size);
        }

        [TestMethod]
        public void LoadImagesTest(){
            string correct = "correct";
            var steps = new List<int>(){1,2,3,4,5,3,1};
            var bitmaps = _imageLoader.LoadImages(correct, steps.Count - 1, 3, steps);
            Assert.AreEqual(new Size(50, 50), bitmaps[0].Size);
            Assert.AreEqual(7, bitmaps.Count);
            //bitmaps[0].Save(@"testimages/correct50.png");
            //bitmaps[1].Save(@"testimages/incorrect75.png");
            //bitmaps[2].Save(@"testimages/incorrect100.png");
            //bitmaps[3].Save(@"testimages/incorrect125.png");
        }
    }
}