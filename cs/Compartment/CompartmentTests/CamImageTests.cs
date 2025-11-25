using Microsoft.VisualStudio.TestTools.UnitTesting;
using Compartment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fizzler;
using System.Threading;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Darknet;

namespace Compartment.Tests
{
    [TestClass()]
    public class CamImageTests
    {
        [TestMethod()]
        public void ConvertBgrToRgbTest()
        {
            Assert.IsTrue(File.Exists("0000698.bmp"));
            Image im = Image.FromFile(@"0000698.bmp");
            byte[] aa = CamImage.ImageToByte(im);
            Stopwatch sw = new Stopwatch();

            sw.Start();

            byte[] ab = CamImage.ConvertBgraToRgb(aa, (uint)im.Height, (uint)im.Width);
            sw.Stop();
            Debug.WriteLine("No parallel convert:" + sw.ElapsedMilliseconds + "ms");
            sw.Restart();
            byte[] ac = CamImage.ConvertBgraToRgbParallel(aa, (uint)im.Height, (uint)im.Width);
            sw.Stop();
            Debug.WriteLine("Parallel convert:" + sw.ElapsedMilliseconds + "ms");

            for (int i = 0; i < ab.Length; i++)
            {
                Assert.AreEqual(ab[i], ac[i]);
            }
        }

        [TestMethod()]
        public void StartDetectFromCamImageTest()
        {
            FormMain mainForm = new FormMain();
            using (CamImage camImage = new CamImage(mainForm, false))
            {
                camImage.WaitDiscovery();
                if (camImage.DevicesDiscovered)
                {
                    Thread.Sleep(TimeSpan.FromSeconds(5));
                    try
                    {
                        if (camImage.DeviceUris.Count > 0)
                        {
                            camImage.ConnectCam(camImage.DeviceUris[0]);
                            if (camImage.CamOpened)
                            {
                                camImage.StartDetectFromCamImage(camImage.DeviceUris[0].ToString());

                                camImage.StopDetectFromCamImage();
                                Debug.WriteLine("Detect num: " + CamImage.DetectNum.ToString());
                            }
                            else
                            {
                                Assert.Fail("カメラ開けない");
                            }
                        }
                        else
                        {
                            Assert.Fail("DeviceUris count error.");
                        }
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                }
                else
                {
                    Assert.Fail("Discovery error.");
                }
            }
        }

        [TestMethod()]
        public void StartDetectImageContinuous()
        {
            const string fileName = "0000698.bmp";
            Image im = Image.FromFile(@"0000698.bmp");
            byte[] aa = CamImage.ImageToByte(im);
            FormMain mainForm = new FormMain();
            Assert.IsTrue(File.Exists(fileName));
            var uri = new Uri(Path.GetFullPath(fileName));
            //Image im = Image.FromFile(@"0000698.bmp");

            Stopwatch sw = new Stopwatch();

            List<long> detectTime = new List<long>();

            using (CamImage camImage = new CamImage(mainForm, false))
            {
                //var privateObject = new PrivateObject(camImage);
                //var yolo = privateObject.GetFieldOrProperty("Yolo");
                
                try
                {
                    camImage.YoloInit("darknet\\yolov7-tiny_marmo_test.cfg", "darknet\\yolov7-tiny_marmo_training_1400000.weights");
                    for (int i = 0; i < 1000; i++)
                    {
                        sw.Restart();
                        camImage.Detect(aa, 1280, 720, 0.8);
                        detectTime.Add(sw.ElapsedMilliseconds);
                        Thread.Sleep(TimeSpan.FromMilliseconds(1));
                    }
                    Debug.WriteLine(detectTime.Average().ToString() + "ms average.");
                    //camImage.StartDetectFromCamImage(uri.AbsoluteUri);
                    //camImage.StopDetectFromCamImage();
                    //Assert.Fail();
                }
                catch (Exception)
                {

                    throw;
                }
            }
        }

        [TestMethod()]
        public void ImageToByteTest()
        {
            FormMain mainForm = new FormMain();
            Assert.IsTrue(File.Exists("0000698.bmp"));
            Image im = Image.FromFile(@"0000698.bmp");
            byte[] aa = CamImage.ImageToByte(im);
            byte[] ab = CamImage.ImageToByte(im);

            for (int i = 0; i < aa.Length; i++)
            {
                Assert.AreEqual(aa[i], ab[i]);
            }
        }
    }
}