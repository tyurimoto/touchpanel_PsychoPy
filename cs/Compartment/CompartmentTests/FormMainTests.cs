using Compartment;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Compartment.Tests
{
    [TestClass()]
    public class FormMainTests
    {
        [TestMethod()]
        public void LoadImageFromFolderTest()
        {
            Bitmap bmp = OpeImage.LoadImageFromFolder(@"C:\Windows\Web\Screen", "*.jpg");
            if (bmp != null)
            {
                if (bmp.Size == new Size(0, 0))
                    Assert.Fail();
            }
            else
                Assert.Fail();
        }
        [TestMethod()]
        public void InitializeComponentOnUcOperationTest()
        {
            var fm = new FormMain();
            var privateObject = new PrivateObject(fm);
            var ret = privateObject.Invoke("InitializeComponentOnUcOperation");
            Assert.AreEqual(ret, null);
            fm.Dispose();
        }
        [TestMethod()]
        public void DevRoomCallbackExeForDevInRoomInOutTest()
        {
            var fm = new FormMain();
            var privateObject = new PrivateObject(fm);
            var ret = privateObject.Invoke("CallbackExeForDevInRoomInOut");
            var devin = privateObject.GetFieldOrProperty("devInRoomInOut");
            bool n = ((FormMain.DevIn)devin).devValSaved[0];
            bool n2 = ((FormMain.DevIn)devin).devValSaved[1];

            Assert.IsTrue(n);
            Assert.IsTrue(n2);
            Assert.AreEqual(ret, null);
            fm.Dispose();
        }
        [TestCategory("Sound")]
        [TestMethod()]
        public void PlaySoundTest()
        {
            try
            {
                FormMain.PlaySound(@"C:\Windows\Media\Windows Foreground.wav");
            }
            catch (Exception)
            {
                Assert.Fail();
            }
        }
        [TestCategory("Sound")]
        [TestMethod()]
        public void PlayMediaTest()
        {
            try
            {
                //FormMain.PlayMedia(@"a.mp3", 5000);
                FormMain.PlayMedia(@"C:\Windows\Media\Windows Foreground.wav", 10000);
            }
            catch (Exception)
            {
                Debug.WriteLine("File not fuond");
                Assert.Fail();
                //throw;
            }
            //Assert.IsTrue(FormMain.PlaySoundEnded);
        }
        [TestCategory("Sound")]
        [TestMethod()]
        public void PlaySoundMediaTest()
        {
            try
            {
                var task = Task.Run(() =>
                {
                    FormMain.PlayMedia(@"owin31.wav");
                });
                task.Wait();

            }
            catch (Exception)
            {
                Assert.Fail();
            }
        }

        [TestCategory("EpisodeTest")]
        [TestMethod()]
        public void CheckTimeZoneStartTest()
        {
            var fm = new FormMain();
            //var privateObject = new PrivateObject(fm);
            //var ret = privateObject.Invoke("CheckTimeZone");
            Assert.AreEqual(fm.CheckTimeZoneStart(0, 2), 0);
            Assert.AreEqual(fm.CheckTimeZoneStart(1, 2), 1);
            Assert.AreEqual(fm.CheckTimeZoneStart(2, 2), 1);
            Assert.AreEqual(fm.CheckTimeZoneStart(3, 2), 3);
            Assert.AreEqual(fm.CheckTimeZoneStart(4, 2), 4);
            Assert.AreEqual(fm.CheckTimeZoneStart(5, 2), 5);
            Assert.AreEqual(fm.CheckTimeZoneStart(6, 2), 6);
            Assert.AreEqual(fm.CheckTimeZoneStart(7, 2), 7);
            Assert.AreEqual(fm.CheckTimeZoneStart(8, 2), 8);
            Assert.AreEqual(fm.CheckTimeZoneStart(9, 2), 9);
            Assert.AreEqual(fm.CheckTimeZoneStart(10, 2), 10);
            Assert.AreEqual(fm.CheckTimeZoneStart(10, 23), 10);
            Assert.AreEqual(fm.CheckTimeZoneStart(23, 0), 23);

        }
        [TestCategory("EpisodeTest")]
        [TestMethod()]
        public void CheckTimeZoneEndTest()
        {
            var fm = new FormMain();
            Assert.AreEqual(fm.CheckTimeZoneEnd(0, 2), 2);
            Assert.AreEqual(fm.CheckTimeZoneEnd(1, 2), 2);
            Assert.AreEqual(fm.CheckTimeZoneEnd(2, 2), 3);
            Assert.AreEqual(fm.CheckTimeZoneEnd(3, 2), 2);
            Assert.AreEqual(fm.CheckTimeZoneEnd(4, 2), 2);
            Assert.AreEqual(fm.CheckTimeZoneEnd(5, 2), 2);
            Assert.AreEqual(fm.CheckTimeZoneEnd(6, 2), 2);
            Assert.AreEqual(fm.CheckTimeZoneEnd(7, 2), 2);
            Assert.AreEqual(fm.CheckTimeZoneEnd(8, 2), 2);
            Assert.AreEqual(fm.CheckTimeZoneEnd(9, 2), 2);
            Assert.AreEqual(fm.CheckTimeZoneEnd(10, 2), 2);
            Assert.AreEqual(fm.CheckTimeZoneEnd(10, 23), 23);
            Assert.AreEqual(fm.CheckTimeZoneEnd(23, 0), 0);
            Assert.AreEqual(fm.CheckTimeZoneEnd(23, 23), 0);
            Assert.AreEqual(fm.CheckTimeZoneEnd(23, 22), 22);

        }
        [TestCategory("EpisodeTest")]
        [TestMethod()]
        public void CheckTimezoneTest()
        {
            var timeOfDay = DateTime.Now.TimeOfDay;
            var startTime = (timeOfDay - new TimeSpan(1, 0, 0)).Hours; //-1h
            var endTime = (timeOfDay + new TimeSpan(1, 0, 0)).Hours; //+1h
            var fm = new FormMain();
            fm.preferencesDatOriginal.EpisodeTimezoneStartTime = startTime;
            fm.preferencesDatOriginal.EpisodeTimezoneEndTime = endTime;

            Assert.IsTrue(fm.CheckTimezone());

            fm.preferencesDatOriginal.EpisodeTimezoneStartTime = startTime + 2;
            fm.preferencesDatOriginal.EpisodeTimezoneEndTime = endTime + 2;

            Assert.IsFalse(fm.CheckTimezone());

            fm.preferencesDatOriginal.EpisodeTimezoneStartTime = startTime + 10;
            fm.preferencesDatOriginal.EpisodeTimezoneEndTime = endTime + 10;

            Assert.IsFalse(fm.CheckTimezone());
            fm.Dispose();
        }

        [TestMethod()]
        public void CheckSelectShapeTimezoneTest()
        {
            var timeOfDay = DateTime.Now.TimeOfDay;
            var startTime = (timeOfDay - new TimeSpan(1, 0, 0)).Hours;
            var endTime = (timeOfDay + new TimeSpan(1, 0, 0)).Hours;
            var fm = new FormMain();
            fm.preferencesDatOriginal.EpisodeSelectShapeTimezoneStartTime = startTime;
            fm.preferencesDatOriginal.EpisodeSelectShapeTimezoneEndTime = endTime;

            Assert.IsTrue(fm.CheckSelectShapeTimezone());

            fm.preferencesDatOriginal.EpisodeSelectShapeTimezoneStartTime = startTime + 2;
            fm.preferencesDatOriginal.EpisodeSelectShapeTimezoneEndTime = endTime + 2;

            Assert.IsFalse(fm.CheckSelectShapeTimezone());

            fm.preferencesDatOriginal.EpisodeSelectShapeTimezoneStartTime = startTime + 10;
            fm.preferencesDatOriginal.EpisodeSelectShapeTimezoneEndTime = endTime + 10;

            Assert.IsFalse(fm.CheckSelectShapeTimezone());
            fm.Dispose();
        }

        [TestMethod()]
        public void CheckEpisodeCountTest()
        {
            var timeOfDay = DateTime.Now.TimeOfDay;
            var startTime = (timeOfDay - new TimeSpan(1, 0, 0)).Hours;
            var endTime = (timeOfDay + new TimeSpan(1, 0, 0)).Hours;
            var fm = new FormMain();
            const string id = "123456789";
            fm.preferencesDatOriginal.EpisodeIntervalTime = 0;
            // SelectShape TimeZone count = 0
            fm.preferencesDatOriginal.EpisodeTimezoneStartTime = startTime;
            fm.preferencesDatOriginal.EpisodeTimezoneEndTime = endTime;

            fm.preferencesDatOriginal.EpisodeSelectShapeTimezoneStartTime = startTime;
            fm.preferencesDatOriginal.EpisodeSelectShapeTimezoneEndTime = endTime;
            fm.idControlHelper = new IdControlHelper();

            fm.idControlHelper.AddEntry(id);
            fm.opCollection.idCode = id;
            fm.CheckEpisodeCount();
            Assert.IsTrue(fm.EpisodeActive.Value);

            // TimeZone count > 0
            fm.preferencesDatOriginal.EpisodeTimezoneStartTime = startTime;
            fm.preferencesDatOriginal.EpisodeTimezoneEndTime = endTime;

            fm.preferencesDatOriginal.EpisodeSelectShapeTimezoneStartTime = startTime + 2;
            fm.preferencesDatOriginal.EpisodeSelectShapeTimezoneEndTime = endTime + 2;

            fm.idControlHelper.ResetCount(id);
            fm.CheckEpisodeCount();
            Assert.IsFalse(fm.EpisodeActive.Value);

            // SelectShape TimeZone False count <> 0
            fm.preferencesDatOriginal.EpisodeTimezoneStartTime = startTime;
            fm.preferencesDatOriginal.EpisodeTimezoneEndTime = endTime;

            //fm.preferencesDatOriginal.EpisodeSelectShapeTimezoneStartTime = startTime + 2;
            //fm.preferencesDatOriginal.EpisodeSelectShapeTimezoneEndTime = endTime + 2;

            fm.idControlHelper.AddCount(id);
            fm.CheckEpisodeCount();
            Assert.IsTrue(fm.EpisodeActive.Value);

            // TimeZone False count <> 0
            fm.preferencesDatOriginal.EpisodeTimezoneStartTime = startTime + 2;
            fm.preferencesDatOriginal.EpisodeTimezoneEndTime = endTime + 2;

            //fm.preferencesDatOriginal.EpisodeSelectShapeTimezoneStartTime = startTime + 2;
            //fm.preferencesDatOriginal.EpisodeSelectShapeTimezoneEndTime = endTime + 2;

            fm.CheckEpisodeCount();
            Assert.IsFalse(fm.EpisodeActive.Value);
            fm.Dispose();

        }

        [TestMethod()]
        public void CheckEpIntervalTest()
        {
            var now = DateTime.Now;
            var expireTime = (now - new TimeSpan(0, 1, 0));
            var endTime = (now - new TimeSpan(0, 0, 50));
            var fm = new FormMain();

            Assert.IsTrue(fm.CheckEpInterval(expireTime));
            Assert.IsFalse(fm.CheckEpInterval(endTime));
            fm.Dispose();

        }

        [TestMethod()]
        public void CheckRuntimeTest()
        {
            Assert.IsTrue(FormMain.CheckRuntime("msvcp100.dll"));
            Assert.IsTrue(FormMain.CheckRuntime("msvcp140.dll"));
        }
    }
}