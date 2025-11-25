using Compartment;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Linq;
using Svg;
using System.Collections.Generic;
using ExecutionScope = Microsoft.VisualStudio.TestTools.UnitTesting.ExecutionScope;

[assembly: Parallelize(Workers = 4, Scope = ExecutionScope.ClassLevel)]
namespace Compartment.Tests
{
    [TestClass()]
    public class OpeImageTests
    {
        [TestMethod()]
        public void OpeImageTest()
        {
            var subForm = new Form();
            var picbox = new PictureBox();
            var preferenceDat = new PreferencesDat();
            var opeImage = new OpeImage(subForm, picbox, preferenceDat);
            var privateObject = new PrivateObject(opeImage);

            Assert.AreEqual(privateObject.GetFieldOrProperty("formSub"), subForm);
            Assert.AreEqual(privateObject.GetFieldOrProperty("formSubPictureBox"), picbox);
            Assert.AreEqual(privateObject.GetFieldOrProperty("preferencesDatOriginal"), preferenceDat);

        }

        [TestMethod()]
        public void InitOpeImageTest()
        {
            var subForm = new Form();
            var picbox = new PictureBox();
            var picboxSub = new PictureBox();
            var preferenceDat = new PreferencesDat();
            var opeImage = new OpeImage(subForm, picbox, preferenceDat);
            var privateObject = new PrivateObject(opeImage);

            opeImage.InitOpeImage(picbox, picboxSub);

            //mPictureBoxOpeImageFromMain
            Assert.AreEqual(privateObject.GetFieldOrProperty("mPictureBoxOpeImageFromMain"), picbox);
            Assert.AreEqual(privateObject.GetFieldOrProperty("mPictureBoxOpeImageFromSub"), picboxSub);
            Assert.IsNotNull(privateObject.GetFieldOrProperty("mBitmapOpeImageCanvas"));
            Assert.IsNotNull(privateObject.GetFieldOrProperty("RectOpeImageValidArea"));
            //var rect = new Rectangle(0, 0, 800, 600);

        }

        [TestMethod()]
        public void ConvertStringToCpStepTest()
        {
            var subForm = new Form();
            var picbox = new PictureBox();
            var picboxSub = new PictureBox();
            var preferenceDat = new PreferencesDat();
            var opeImage = new OpeImage(subForm, picbox, preferenceDat);
            var n = new ECpStep();

            opeImage.ConvertStringToCpStep("Step1", out n);
            Assert.AreEqual(n, ECpStep.Step1);

            opeImage.ConvertStringToCpStep("Step2", out n);
            Assert.AreEqual(n, ECpStep.Step2);

            opeImage.ConvertStringToCpStep("Step3", out n);
            Assert.AreEqual(n, ECpStep.Step3);

            opeImage.ConvertStringToCpStep("Step4", out n);
            Assert.AreEqual(n, ECpStep.Step4);

            opeImage.ConvertStringToCpStep("Step5", out n);
            Assert.AreEqual(n, ECpStep.Step5);

            opeImage.ConvertStringToCpStep("Step6", out n);
            Assert.AreEqual(n, ECpStep.Step1);

        }

        [TestMethod()]
        public void ConvertStringToCpShapeTest()
        {
            var subForm = new Form();
            var picbox = new PictureBox();
            var picboxSub = new PictureBox();
            var preferenceDat = new PreferencesDat();
            var opeImage = new OpeImage(subForm, picbox, preferenceDat);
            var n = new ECpShape();
            opeImage.CorrectShape = ECpShape.None;
            opeImage.ConvertStringToCpShape("Circle", out n);
            Assert.AreEqual(n, ECpShape.Circle);

            opeImage.ConvertStringToCpShape("Rectangle", out n);
            Assert.AreEqual(n, ECpShape.Rectangle);

            opeImage.ConvertStringToCpShape("Triangle", out n);
            Assert.AreEqual(n, ECpShape.Triangle);

            opeImage.ConvertStringToCpShape("Star", out n);
            Assert.AreEqual(n, ECpShape.Star);

            //ランダムテスト
            for (int i = 0; i < 100; i++)
            {
                opeImage.ConvertStringToCpShape("Random", out n);
                Assert.AreNotEqual(n, ECpShape.None);
            }
            opeImage.ConvertStringToCpShape("None", out n);
            Assert.AreNotEqual(n, ECpShape.None);

            // Correct Wrong別テスト
            opeImage.CorrectShape = ECpShape.Circle;
            //ランダムテスト
            for (int i = 0; i < 1000; i++)
            {
                opeImage.ConvertStringToCpShape("Random", out n);
                Assert.AreNotEqual(n, ECpShape.None);
                Assert.AreNotEqual(n, opeImage.CorrectShape);
            }
        }

        [TestMethod()]
        public void ConvertStringToColorTest()
        {
            var subForm = new Form();
            var picbox = new PictureBox();
            var picboxSub = new PictureBox();
            var preferenceDat = new PreferencesDat();
            var opeImage = new OpeImage(subForm, picbox, preferenceDat);
            var n = new Color();

            opeImage.ConvertStringToColor("Black", out n);
            Assert.AreEqual(n, Color.Black);
            opeImage.ConvertStringToColor("Blue", out n);
            Assert.AreEqual(n, Color.Blue);
            opeImage.ConvertStringToColor("Green", out n);
            Assert.AreEqual(n, Color.Green);
            opeImage.ConvertStringToColor("Red", out n);
            Assert.AreEqual(n, Color.Red);
            opeImage.ConvertStringToColor("White", out n);
            Assert.AreEqual(n, Color.White);
            opeImage.ConvertStringToColor("Yellow", out n);
            Assert.AreEqual(n, Color.Yellow);

        }

        [TestMethod()]
        public void ConvertStringToBgColorTest()
        {
            var subForm = new Form();
            var picbox = new PictureBox();
            var picboxSub = new PictureBox();
            var preferenceDat = new PreferencesDat();
            var opeImage = new OpeImage(subForm, picbox, preferenceDat);
            var n = new Color();

            opeImage.ConvertStringToBgColor("Black", out n);
            Assert.AreEqual(n, Color.Black);
            opeImage.ConvertStringToBgColor("Blue", out n);
            Assert.AreEqual(n, Color.Blue);
            opeImage.ConvertStringToBgColor("Green", out n);
            Assert.AreEqual(n, Color.Green);
            opeImage.ConvertStringToBgColor("Red", out n);
            Assert.AreEqual(n, Color.Red);
            opeImage.ConvertStringToBgColor("White", out n);
            Assert.AreEqual(n, Color.White);
            opeImage.ConvertStringToBgColor("Yellow", out n);
            Assert.AreEqual(n, Color.Yellow);

            //Add Test FromNamed
            opeImage.ConvertStringToBgColor("AliceBlue", out n);
            Assert.AreEqual(n, Color.AliceBlue);
            opeImage.ConvertStringToBgColor("DarkOrange", out n);
            Assert.AreEqual(n, Color.DarkOrange);


        }

        //[TestMethod()]
        //public void ConvertStringToCpColorTest()
        //{
        //    var subForm = new Form();
        //    var picbox = new PictureBox();
        //    var picboxSub = new PictureBox();
        //    var preferenceDat = new PreferencesDat();
        //    var opeImage = new OpeImage(subForm, picbox, preferenceDat);
        //    var n = new ECpColor();

        //    opeImage.ConvertStringToCpColor("Black", out n);
        //    Assert.AreEqual(n, ECpColor.Black);
        //    opeImage.ConvertStringToCpColor("Blue", out n);
        //    Assert.AreEqual(n, ECpColor.Blue);
        //    opeImage.ConvertStringToCpColor("Green", out n);
        //    Assert.AreEqual(n, ECpColor.Green);
        //    opeImage.ConvertStringToCpColor("Red", out n);
        //    Assert.AreEqual(n, ECpColor.Red);
        //    opeImage.ConvertStringToCpColor("White", out n);
        //    Assert.AreEqual(n, ECpColor.White);
        //    opeImage.ConvertStringToCpColor("Yellow", out n);
        //    Assert.AreEqual(n, ECpColor.Yellow);
        //}

        [TestMethod()]
        public void ConvertStringToCpTaskTest()
        {
            var subForm = new Form();
            var picbox = new PictureBox();
            var picboxSub = new PictureBox();
            var preferenceDat = new PreferencesDat();
            var opeImage = new OpeImage(subForm, picbox, preferenceDat);
            var n = new ECpTask();

            opeImage.ConvertStringToCpTask("DelayMatch", out n);
            Assert.AreEqual(n, ECpTask.DelayMatch);
            opeImage.ConvertStringToCpTask("Training", out n);
            Assert.AreEqual(n, ECpTask.Training);
            opeImage.ConvertStringToCpTask("TrainingEasy", out n);
            Assert.AreEqual(n, ECpTask.TrainingEasy);
            opeImage.ConvertStringToCpTask("UnConditionalFeeding", out n);
            Assert.AreEqual(n, ECpTask.UnConditionalFeeding);
            opeImage.ConvertStringToCpTask("hogehoge", out n);
            Assert.AreEqual(n, ECpTask.Training);
        }

        [TestMethod()]
        public void SetShapeColorTest()
        {
            var subForm = new Form();
            var picbox = new PictureBox();
            var picboxSub = new PictureBox();
            var preferenceDat = new PreferencesDat();

            preferenceDat.ShapeColor = "Black";
            var opeImage = new OpeImage(subForm, picbox, preferenceDat);
            var privateObject = new PrivateObject(opeImage);

            privateObject.Invoke("SetShapeColor");
            Assert.AreEqual(opeImage.CorrectShapeColor, Color.Black);

            preferenceDat.ShapeColor = "Blue";
            //opeImage = new OpeImage(subForm, picbox, preferenceDat);

            privateObject.Invoke("SetShapeColor");
            //opeImage.SetShapeColor();
            Assert.AreEqual(opeImage.CorrectShapeColor, Color.Blue);

            preferenceDat.ShapeColor = "Green";
            //opeImage = new OpeImage(subForm, picbox, preferenceDat);

            privateObject.Invoke("SetShapeColor");
            //opeImage.SetShapeColor();
            Assert.AreEqual(opeImage.CorrectShapeColor, Color.Green);

            preferenceDat.ShapeColor = "Red";
            //opeImage = new OpeImage(subForm, picbox, preferenceDat);

            privateObject.Invoke("SetShapeColor");
            //opeImage.SetShapeColor();
            Assert.AreEqual(opeImage.CorrectShapeColor, Color.Red);

            preferenceDat.ShapeColor = "White";
            //opeImage = new OpeImage(subForm, picbox, preferenceDat);

            privateObject.Invoke("SetShapeColor");
            //opeImage.SetShapeColor();
            Assert.AreEqual(opeImage.CorrectShapeColor, Color.White);

            preferenceDat.ShapeColor = "Yellow";
            //opeImage = new OpeImage(subForm, picbox, preferenceDat);
            privateObject.Invoke("SetShapeColor");
            //opeImage.SetShapeColor();
            Assert.AreEqual(opeImage.CorrectShapeColor, Color.Yellow);

        }

        [TestMethod()]
        public void SetBgColorTest()
        {
            var subForm = new Form();
            var picbox = new PictureBox();
            var picboxSub = new PictureBox();
            var preferenceDat = new PreferencesDat();

            preferenceDat.BackColor = "Black";
            var opeImage = new OpeImage(subForm, picbox, preferenceDat);
            var privateObject = new PrivateObject(opeImage);

            privateObject.Invoke("SetBgColor");
            //opeImage.SetBgColor();
            Assert.AreEqual(opeImage.ColorOpeImageBackColor, Color.Black);

            preferenceDat.BackColor = "Blue";
            //opeImage = new OpeImage(subForm, picbox, preferenceDat);
            privateObject.Invoke("SetBgColor");
            //opeImage.SetBgColor();
            Assert.AreEqual(opeImage.ColorOpeImageBackColor, Color.Blue);

            preferenceDat.BackColor = "Green";
            //opeImage = new OpeImage(subForm, picbox, preferenceDat);
            privateObject.Invoke("SetBgColor");
            //opeImage.SetBgColor();
            Assert.AreEqual(opeImage.ColorOpeImageBackColor, Color.Green);

            preferenceDat.BackColor = "Red";
            //opeImage = new OpeImage(subForm, picbox, preferenceDat);
            privateObject.Invoke("SetBgColor");
            //opeImage.SetBgColor();
            Assert.AreEqual(opeImage.ColorOpeImageBackColor, Color.Red);

            preferenceDat.BackColor = "White";
            //opeImage = new OpeImage(subForm, picbox, preferenceDat);
            privateObject.Invoke("SetBgColor");
            //opeImage.SetBgColor();
            Assert.AreEqual(opeImage.ColorOpeImageBackColor, Color.White);

            preferenceDat.BackColor = "Yellow";
            //opeImage = new OpeImage(subForm, picbox, preferenceDat);
            privateObject.Invoke("SetBgColor");
            //opeImage.SetBgColor();
            Assert.AreEqual(opeImage.ColorOpeImageBackColor, Color.Yellow);
        }

        [TestMethod()]
        public void SetDelayBgColorTest()
        {
            var subForm = new Form();
            var picbox = new PictureBox();
            var picboxSub = new PictureBox();
            var preferenceDat = new PreferencesDat();

            preferenceDat.DelayBackColor = "Black";
            var opeImage = new OpeImage(subForm, picbox, preferenceDat);
            var privateObject = new PrivateObject(opeImage);

            privateObject.Invoke("SetDelayBgColor");
            //opeImage.SetDelayBgColor();
            Assert.AreEqual(opeImage.ColorOpeImageDelayBackColor, Color.Black);

            preferenceDat.DelayBackColor = "Blue";
            //opeImage = new OpeImage(subForm, picbox, preferenceDat);
            privateObject.Invoke("SetDelayBgColor");
            //opeImage.SetDelayBgColor();
            Assert.AreEqual(opeImage.ColorOpeImageDelayBackColor, Color.Blue);

            preferenceDat.DelayBackColor = "Green";
            //opeImage = new OpeImage(subForm, picbox, preferenceDat);
            privateObject.Invoke("SetDelayBgColor");
            //opeImage.SetDelayBgColor();
            Assert.AreEqual(opeImage.ColorOpeImageDelayBackColor, Color.Green);

            preferenceDat.DelayBackColor = "Red";
            //opeImage = new OpeImage(subForm, picbox, preferenceDat);
            privateObject.Invoke("SetDelayBgColor");
            //opeImage.SetDelayBgColor();
            Assert.AreEqual(opeImage.ColorOpeImageDelayBackColor, Color.Red);

            preferenceDat.DelayBackColor = "White";
            //opeImage = new OpeImage(subForm, picbox, preferenceDat);
            privateObject.Invoke("SetDelayBgColor");
            //opeImage.SetDelayBgColor();
            Assert.AreEqual(opeImage.ColorOpeImageDelayBackColor, Color.White);

            preferenceDat.DelayBackColor = "Yellow";
            //opeImage = new OpeImage(subForm, picbox, preferenceDat);
            privateObject.Invoke("SetDelayBgColor");
            //opeImage.SetDelayBgColor();
            Assert.AreEqual(opeImage.ColorOpeImageDelayBackColor, Color.Yellow);
        }

        [TestMethod()]
        public void SetParamOfShapeOpeImageTest()
        {
            var subForm = new Form();
            var picbox = new PictureBox();
            var picboxSub = new PictureBox();
            var preferenceDat = new PreferencesDat();

            var opeImage = new OpeImage(subForm, picbox, preferenceDat);
            var privateObject = new PrivateObject(opeImage);

            opeImage.InitOpeImage(picbox, picboxSub);

            preferenceDat.ShapeColor = "Black";
            // ShapeObjectに変更したので判定式の書き換えが必要

            opeImage.SetParamOfShapeOpeImage("Circle", "Step1", 10);
            Assert.AreEqual(opeImage.CorrectShape, ECpShape.Circle);
            Assert.AreEqual(opeImage.CorrectShapeColor, Color.Black);
            //Assert.AreNotEqual(privateObject.GetFieldOrProperty("correctShape"), privateObject.GetFieldOrProperty("wrongShape"));
            Debug.WriteLine("WrongShape: " + privateObject.GetFieldOrProperty("wrongShape").ToString());

            preferenceDat.ShapeColor = "Blue";
            opeImage.SetParamOfShapeOpeImage("Rectangle", "Step1", 10);
            Assert.AreEqual(opeImage.CorrectShape, ECpShape.Rectangle);
            Assert.AreEqual(opeImage.CorrectShapeColor, Color.Blue);
            //Assert.AreNotEqual(privateObject.GetFieldOrProperty("correctShape"), privateObject.GetFieldOrProperty("wrongShape"));
            Debug.WriteLine("WrongShape: " + privateObject.GetFieldOrProperty("wrongShape").ToString());

            preferenceDat.ShapeColor = "Yellow";
            opeImage.SetParamOfShapeOpeImage("Triangle", "Step1", 10);
            Assert.AreEqual(opeImage.CorrectShape, ECpShape.Triangle);
            Assert.AreEqual(opeImage.CorrectShapeColor, Color.Yellow);
            //Assert.AreNotEqual(privateObject.GetFieldOrProperty("correctShape"), privateObject.GetFieldOrProperty("wrongShape"));
            Debug.WriteLine("WrongShape: " + privateObject.GetFieldOrProperty("wrongShape").ToString());

            preferenceDat.ShapeColor = "Red";
            opeImage.SetParamOfShapeOpeImage("Star", "Step1", 10);
            Assert.AreEqual(opeImage.CorrectShape, ECpShape.Star);
            Assert.AreEqual(opeImage.CorrectShapeColor, Color.Red);
            //Assert.AreNotEqual(privateObject.GetFieldOrProperty("correctShape"), privateObject.GetFieldOrProperty("wrongShape"));
            Debug.WriteLine("WrongShape: " + privateObject.GetFieldOrProperty("wrongShape").ToString());

        }

        [TestMethod()]
        public void DecideSizeOfShapeInPixelOpeImageTest()
        {
            var subForm = new Form();
            var picbox = new PictureBox();
            var picboxSub = new PictureBox();
            var preferenceDat = new PreferencesDat();

            var opeImage = new OpeImage(subForm, picbox, preferenceDat);
            var privateObject = new PrivateObject(opeImage);
            int outputPixel;
            int inputValue = 250;

            opeImage.InitOpeImage(picbox, picboxSub);
            opeImage.DecideSizeOfShapeInPixelOpeImage(ECpStep.Step1, 0, out outputPixel);
            Assert.AreEqual(outputPixel, preferenceDat.SizeOfShapeInPixelForStep1);
            opeImage.DecideSizeOfShapeInPixelOpeImage(ECpStep.Step2, 0, out outputPixel);
            Assert.AreEqual(outputPixel, preferenceDat.SizeOfShapeInPixelForStep2);
            opeImage.DecideSizeOfShapeInPixelOpeImage(ECpStep.Step3, 0, out outputPixel);
            Assert.AreEqual(outputPixel, preferenceDat.SizeOfShapeInPixelForStep3);
            opeImage.DecideSizeOfShapeInPixelOpeImage(ECpStep.Step4, 0, out outputPixel);
            Assert.AreEqual(outputPixel, preferenceDat.SizeOfShapeInPixelForStep4);
            opeImage.DecideSizeOfShapeInPixelOpeImage(ECpStep.Step5, 0, out outputPixel);
            Assert.AreEqual(outputPixel, preferenceDat.SizeOfShapeInPixelForStep5);

            opeImage.DecideSizeOfShapeInPixelOpeImage(0, inputValue, out outputPixel);
            Assert.AreEqual(outputPixel, inputValue);

        }

        [TestMethod()]
        public void IsDistanceInRangeOpeImageTest()
        {
            var pointA = new Point(247, 350);
            var pointB = new Point(360, 360);
            var pointC = new Point(566, 403);
            var pointD = new Point(279, 355);
            var pointE = new Point(434, 439);
            var pointF = new Point(392, 346);
            var pointG = new Point(566, 403);
            int distance = 100;

            var subForm = new Form();
            var picbox = new PictureBox();
            var picboxSub = new PictureBox();
            var preferenceDat = new PreferencesDat();

            var opeImage = new OpeImage(subForm, picbox, preferenceDat);
            var privateObject = new PrivateObject(opeImage);

            var vct_size = Math.Sqrt(Math.Pow((pointA.X - pointB.X), 2) + Math.Pow((pointA.Y - pointB.Y), 2));


            Assert.IsFalse(opeImage.IsDistanceInRangeOpeImage(pointA, pointB, distance));

            distance = 122;
            Assert.IsTrue(opeImage.IsDistanceInRangeOpeImage(pointA, pointB, distance));
            Assert.IsTrue(opeImage.IsDistanceInRangeOpeImage(pointA, pointB, (int)vct_size));


            distance = 250;
            Assert.IsFalse(opeImage.IsDistanceInRangeOpeImage(pointC, pointD, distance));
            Assert.IsTrue(opeImage.IsDistanceInRangeOpeImage(pointC, pointE, distance));
            Assert.IsTrue(opeImage.IsDistanceInRangeOpeImage(pointC, pointF, distance));
            Assert.IsTrue(opeImage.IsDistanceInRangeOpeImage(pointC, pointG, distance));
            Assert.IsTrue(opeImage.IsDistanceInRangeOpeImage(pointD, pointE, distance));
            Assert.IsTrue(opeImage.IsDistanceInRangeOpeImage(pointD, pointF, distance));
            Assert.IsFalse(opeImage.IsDistanceInRangeOpeImage(pointD, pointG, distance));
            Assert.IsTrue(opeImage.IsDistanceInRangeOpeImage(pointE, pointF, distance));
            Assert.IsTrue(opeImage.IsDistanceInRangeOpeImage(pointE, pointG, distance));
            Assert.IsTrue(opeImage.IsDistanceInRangeOpeImage(pointF, pointG, distance));

        }

        [TestMethod()]
        public void ConvertFromAngleToRadianOpeImageTest()
        {
            var subForm = new Form();
            var picbox = new PictureBox();
            var picboxSub = new PictureBox();
            var preferenceDat = new PreferencesDat();

            var opeImage = new OpeImage(subForm, picbox, preferenceDat);
            var privateObject = new PrivateObject(opeImage);

            double angleValue = 100.12;
            double targetValue = (angleValue * System.Math.PI) / 180;

            Assert.AreEqual((opeImage.ConvertFromAngleToRadianOpeImage(angleValue)), targetValue);

        }

        [TestMethod(), TestCategory("MakePoint")]
        public void MakePointOfCorrectShapeOpeImageTest()
        {
            var subForm = new Form();
            var picbox = new PictureBox();
            picbox.Width = 1920;
            picbox.Height = 1080;
            var picboxSub = new PictureBox();
            picboxSub.Width = 800;
            picboxSub.Height = 600;
            var preferenceDat = new PreferencesDat();

            var opeImage = new OpeImage(subForm, picbox, preferenceDat);
            var privateObject = new PrivateObject(opeImage);
            opeImage.InitOpeImage(picbox, picboxSub);
            opeImage.SetParamOfShapeOpeImage("Circle", "Step5", 10);
            opeImage.MakePointOfCorrectShapeOpeImage();


            Debug.WriteLine((opeImage.PointOpeImageCenterOfCorrectShape.X >= 0).ToString());
            Debug.WriteLine((opeImage.PointOpeImageCenterOfCorrectShape.X < opeImage.RectOpeImageValidArea.Width).ToString());

            Debug.WriteLine((opeImage.PointOpeImageCenterOfCorrectShape.Y >= 0).ToString());
            Debug.WriteLine((opeImage.PointOpeImageCenterOfCorrectShape.Y < opeImage.RectOpeImageValidArea.Height).ToString());

            Assert.IsTrue(opeImage.PointOpeImageCenterOfCorrectShape.X >= 0 && opeImage.PointOpeImageCenterOfCorrectShape.X < opeImage.RectOpeImageValidArea.Width);
            Assert.IsTrue(opeImage.PointOpeImageCenterOfCorrectShape.Y >= 0 && opeImage.PointOpeImageCenterOfCorrectShape.Y < opeImage.RectOpeImageValidArea.Height);

        }

        [TestMethod(), TestCategory("MakePoint")]
        public void MakePointOfWrongShapeOpeImageTest()
        {
            var subForm = new Form();
            var picbox = new PictureBox();
            picbox.Width = 1920;
            picbox.Height = 1080;
            var picboxSub = new PictureBox();
            picboxSub.Width = 800;
            picboxSub.Height = 600;
            var preferenceDat = new PreferencesDat();

            var opeImage = new OpeImage(subForm, picbox, preferenceDat);
            var privateObject = new PrivateObject(opeImage);
            opeImage.InitOpeImage(picbox, picboxSub);
            opeImage.SetParamOfShapeOpeImage("Circle", "Step1", 10);

            opeImage.MakePointOfCorrectShapeOpeImage();
            opeImage.MakePointOfWrongShapeOpeImage();

            Assert.IsTrue(opeImage.PointOpeImageCenterOfWrongShapeList[0].X >= 0 && opeImage.PointOpeImageCenterOfWrongShapeList[0].X < opeImage.RectOpeImageValidArea.Width);
            Assert.IsTrue(opeImage.PointOpeImageCenterOfWrongShapeList[0].Y >= 0 && opeImage.PointOpeImageCenterOfWrongShapeList[0].Y < opeImage.RectOpeImageValidArea.Height);

        }
        [TestMethod(), TestCategory("MakePoint")]
        public void MakePointOfWrongShapeMultiPointTest()
        {
            var subForm = new Form();
            var picbox = new PictureBox();
            picbox.Width = 1920;
            picbox.Height = 1080;
            var picboxSub = new PictureBox();
            picboxSub.Width = 800;
            picboxSub.Height = 600;
            var preferenceDat = new PreferencesDat();

            var opeImage = new OpeImage(subForm, picbox, preferenceDat);
            var privateObject = new PrivateObject(opeImage);
            opeImage.InitOpeImage(picbox, picboxSub);
            opeImage.SetParamOfShapeOpeImage("Circle", "Step1", 10);
            opeImage.ResetWrongShapePoint();

            opeImage.MakePointOfCorrectShapeOpeImage();
            opeImage.MakePointOfWrongShapeOpeImage();
            opeImage.MakePointOfWrongShapeOpeImage();
            opeImage.MakePointOfWrongShapeOpeImage();
            opeImage.MakePointOfWrongShapeOpeImage();
            Debug.WriteLine("Point Count:" + opeImage.PointOpeImageCenterOfWrongShapeList.Count);
            Debug.WriteLine(opeImage.PointOpeImageCenterOfWrongShapeList[0]);
            Debug.WriteLine(opeImage.PointOpeImageCenterOfWrongShapeList[1]);
            Debug.WriteLine(opeImage.PointOpeImageCenterOfWrongShapeList[2]);
            Debug.WriteLine(opeImage.PointOpeImageCenterOfWrongShapeList[3]);
            //座標確認する

            Assert.IsTrue(opeImage.PointOpeImageCenterOfWrongShapeList[0].X >= 0 && opeImage.PointOpeImageCenterOfWrongShapeList[0].X < opeImage.RectOpeImageValidArea.Width);
            Assert.IsTrue(opeImage.PointOpeImageCenterOfWrongShapeList[0].Y >= 0 && opeImage.PointOpeImageCenterOfWrongShapeList[0].Y < opeImage.RectOpeImageValidArea.Height);

        }

        [TestMethod()]
        public void MakePathOfCorrectShapeOpeImageTest()
        {
            var subForm = new Form();
            var picbox = new PictureBox();
            picbox.Width = 1920;
            picbox.Height = 1080;
            var picboxSub = new PictureBox();
            picboxSub.Width = 800;
            picboxSub.Height = 600;
            var preferenceDat = new PreferencesDat();

            var opeImage = new OpeImage(subForm, picbox, preferenceDat);
            var privateObject = new PrivateObject(opeImage);
            opeImage.InitOpeImage(picbox, picboxSub);
            //opeImage.SetParamOfShapeOpeImage("Circle", "Step1", 10);
            opeImage.MakePathOfCorrectShapeOpeImage();
            System.Drawing.Drawing2D.PathData pathData = opeImage.mGraphicsPathOpeImageShape.PathData;
            Assert.IsNotNull(pathData);
        }

        [TestMethod()]
        public void DeletePathOfCorrectShapeOpeImageTest()
        {
            var subForm = new Form();
            var picbox = new PictureBox();
            picbox.Width = 1920;
            picbox.Height = 1080;
            var picboxSub = new PictureBox();
            picboxSub.Width = 800;
            picboxSub.Height = 600;
            var preferenceDat = new PreferencesDat();

            var opeImage = new OpeImage(subForm, picbox, preferenceDat);
            var privateObject = new PrivateObject(opeImage);
            opeImage.InitOpeImage(picbox, picboxSub);

            opeImage.MakePathOfCorrectShapeOpeImage();
            System.Drawing.Drawing2D.PathData pathData = opeImage.mGraphicsPathOpeImageShape.PathData;
            Assert.IsNotNull(pathData);

            opeImage.DeletePathOfCorrectShapeOpeImage();
            Assert.IsNull(opeImage.mGraphicsPathOpeImageShape);
        }

        [TestMethod()]
        public void DrawShapeOpeImageTest()
        {
            var subForm = new Form();
            var picbox = new PictureBox();
            picbox.Width = 1920;
            picbox.Height = 1080;
            var picboxSub = new PictureBox();
            picboxSub.Width = 800;
            picboxSub.Height = 600;
            var preferenceDat = new PreferencesDat();

            var opeImage = new OpeImage(subForm, picbox, preferenceDat);
            var privateObject = new PrivateObject(opeImage);
            opeImage.InitOpeImage(picbox, picboxSub);
            opeImage.DrawShapeOpeImage(opeImage.PointOpeImageCenterOfCorrectShape, false);

            Assert.IsNotNull(opeImage.mPictureBoxOpeImageFromMain.Image);
            Assert.IsNotNull(opeImage.mPictureBoxOpeImageFromSub.Image);

        }

        [TestMethod()]
        public void drawCircleTest()
        {
            var subForm = new Form();
            var picbox = new PictureBox();
            var picboxSub = new PictureBox();
            var preferenceDat = new PreferencesDat();

            var opeImage = new OpeImage(subForm, picbox, preferenceDat);
            var graphics = picbox.CreateGraphics();
            var privateObject = new PrivateObject(opeImage);

            privateObject.Invoke("drawCircle", new Point(0, 0), graphics, Color.White);

            //opeImage.drawCircle(new Point(0, 0), graphics, Color.White);
            Assert.IsNotNull(picbox);

        }

        [TestMethod()]
        public void drawRectangleTest()
        {
            var subForm = new Form();
            var picbox = new PictureBox();
            var picboxSub = new PictureBox();
            var preferenceDat = new PreferencesDat();

            var opeImage = new OpeImage(subForm, picbox, preferenceDat);
            var graphics = picbox.CreateGraphics();

            var privateObject = new PrivateObject(opeImage);

            privateObject.Invoke("drawRectangle", new Point(0, 0), graphics, Color.White);
            //opeImage.drawRectangle(new Point(0, 0), graphics, Color.White);
            Assert.IsNotNull(picbox);
        }

        [TestMethod()]
        public void drawTriangleTest()
        {
            var subForm = new Form();
            var picbox = new PictureBox();
            var picboxSub = new PictureBox();
            var preferenceDat = new PreferencesDat();

            var opeImage = new OpeImage(subForm, picbox, preferenceDat);
            var graphics = picbox.CreateGraphics();
            var privateObject = new PrivateObject(opeImage);

            privateObject.Invoke("drawTriangle", new Point(0, 0), graphics, Color.White);
            //opeImage.drawTriangle(new Point(0, 0), graphics, Color.White);
            Assert.IsNotNull(picbox);
        }

        [TestMethod()]
        public void drawStarTest()
        {
            var subForm = new Form();
            var picbox = new PictureBox();
            var picboxSub = new PictureBox();
            var preferenceDat = new PreferencesDat();

            var opeImage = new OpeImage(subForm, picbox, preferenceDat);
            var graphics = picbox.CreateGraphics();
            var privateObject = new PrivateObject(opeImage);

            privateObject.Invoke("drawStar", new Point(0, 0), graphics, Color.White);
            //opeImage.drawStar(new Point(0, 0), graphics, Color.White);
            Assert.IsNotNull(picbox);
        }

        [TestMethod()]
        public void DrawBackColorTest()
        {
            var subForm = new Form();
            var picbox = new PictureBox();
            picbox.Width = 1920;
            picbox.Height = 1080;
            var picboxSub = new PictureBox();
            picboxSub.Width = 800;
            picboxSub.Height = 600;
            var preferenceDat = new PreferencesDat();
            var opeImage = new OpeImage(subForm, picbox, preferenceDat);
            var privateObject = new PrivateObject(opeImage);
            opeImage.InitOpeImage(picbox, picboxSub);

            opeImage.DrawBackColor();
            Assert.IsNotNull(opeImage.mPictureBoxOpeImageFromMain.Image);
            Assert.IsNotNull(opeImage.mPictureBoxOpeImageFromSub.Image);
        }

        [TestMethod()]
        public void DrawBackColorTest1()
        {
            var subForm = new Form();
            var picbox = new PictureBox();
            picbox.Width = 1920;
            picbox.Height = 1080;
            var picboxSub = new PictureBox();
            picboxSub.Width = 800;
            picboxSub.Height = 600;
            var preferenceDat = new PreferencesDat();

            var opeImage = new OpeImage(subForm, picbox, preferenceDat);
            var privateObject = new PrivateObject(opeImage);
            opeImage.InitOpeImage(picbox, picboxSub);

            opeImage.DrawBackColor(Color.Black);
            // Pixel pickupして色が変わってるか確認する？
            Assert.IsNotNull(opeImage.mPictureBoxOpeImageFromMain.Image);
            Assert.IsNotNull(opeImage.mPictureBoxOpeImageFromSub.Image);
        }

        [TestMethod()]
        public void DrawImageTest()
        {

            var subForm = new Form();
            var picbox = new PictureBox();
            picbox.Width = 1920;
            picbox.Height = 1080;
            var picboxSub = new PictureBox();
            picboxSub.Width = 800;
            picboxSub.Height = 600;
            var preferenceDat = new PreferencesDat();

            var opeImage = new OpeImage(subForm, picbox, preferenceDat);
            var privateObject = new PrivateObject(opeImage);
            opeImage.InitOpeImage(picbox, picboxSub);

            var path = @"C:\Windows\Web\Screen";
            string jpgFile;
            if (System.IO.Directory.Exists(path))
            {
                System.Random rnd = new System.Random();
                string[] fileList = System.IO.Directory.GetFiles(path, "*.jpg");
                if (fileList.Length != 0)
                {
                    jpgFile = fileList[rnd.Next(0, fileList.Length - 1)];
                    opeImage.DrawImage(jpgFile);
                }
                else
                    Assert.Fail();
            }
            else
            {
                Assert.Fail();
            }

            Assert.IsNotNull(opeImage.mPictureBoxOpeImageFromMain.Image);
            Assert.IsNotNull(opeImage.mPictureBoxOpeImageFromSub.Image);
        }

        [TestMethod()]
        public void DrawImageTest1()
        {
            var bmp = OpeImage.LoadImageFromFolder(@"C:\Windows\Web\Screen", "*.jpg");

            var subForm = new Form();
            var picbox = new PictureBox();
            picbox.Width = 1920;
            picbox.Height = 1080;
            var picboxSub = new PictureBox();
            picboxSub.Width = 800;
            picboxSub.Height = 600;
            var preferenceDat = new PreferencesDat();
            var opeImage = new OpeImage(subForm, picbox, preferenceDat);
            var privateObject = new PrivateObject(opeImage);
            opeImage.InitOpeImage(picbox, picboxSub);

            opeImage.DrawImage(bmp);
            Assert.IsNotNull(opeImage.mPictureBoxOpeImageFromMain.Image);
            Assert.IsNotNull(opeImage.mPictureBoxOpeImageFromSub.Image);

        }

        [TestMethod()]
        public void LoadImageFromFolderTest()
        {
            var bmp = OpeImage.LoadImageFromFolder(@"C:\Windows\Web\Screen", "*.jpg");
            Assert.IsTrue(bmp.GetType() == typeof(Bitmap));
        }

        [TestMethod()]
        public void ConvertStringToColorTest1()
        {
            var subForm = new Form();
            var picbox = new PictureBox();
            var picboxSub = new PictureBox();
            var preferenceDat = new PreferencesDat();
            var opeImage = new OpeImage(subForm, picbox, preferenceDat);
            var n = new Color();

            opeImage.ConvertStringToColor("Black", out n);
            Assert.AreEqual(n, Color.Black);
            opeImage.ConvertStringToColor("Blue", out n);
            Assert.AreEqual(n, Color.Blue);
            opeImage.ConvertStringToColor("Green", out n);
            Assert.AreEqual(n, Color.Green);
            opeImage.ConvertStringToColor("Red", out n);
            Assert.AreEqual(n, Color.Red);
            opeImage.ConvertStringToColor("White", out n);
            Assert.AreEqual(n, Color.White);
            opeImage.ConvertStringToColor("Yellow", out n);
            Assert.AreEqual(n, Color.Yellow);
            object syncObject = new object();
            ParallelOptions options = new ParallelOptions();
            Parallel.For(0, 1000, options, (i, loopkstate) =>
               {
                   opeImage.ConvertStringToColor("Random", Color.White, Color.Green, out n);
                   lock (syncObject)
                   {
                       Assert.AreNotEqual(n, Color.White);
                       if (n == Color.Yellow)
                       {
                           Debug.WriteLine("Yellow");
                       }
                   }
               }
            );

            // Blackだけ引っかかる スレッドセーフではない？
            Parallel.For(0, 5, options, (i, loopkstate) =>
            {
                opeImage.ConvertStringToColor("Random", Color.Black, Color.Green, out n);
                lock (syncObject)
                {
                    Assert.AreNotEqual(n, Color.Black);
                    if (n == Color.Yellow)
                    {
                        Debug.WriteLine("Yellow");
                    }
                }
            }
            );
            try
            {
                for (int i = 0; i < 1000; i++)
                {
                    opeImage.ConvertStringToColor("Random", Color.Black, Color.Green, out n);
                    lock (syncObject)
                    {
                        Assert.AreNotEqual(n, Color.Black);
                        if (n == Color.Yellow)
                        {
                            Debug.WriteLine("Yellow");
                        }
                    }
                }

            }
            catch (Exception)
            {

                throw;
            }

            Parallel.For(0, 1000, options, (i, loopkstate) =>
            {
                opeImage.ConvertStringToColor("Random", Color.Blue, Color.Green, out n);
                lock (syncObject)
                {
                    Assert.AreNotEqual(n, Color.Blue);
                    if (n == Color.Yellow)
                    {
                        Debug.WriteLine("Yellow");
                    }
                }


            }
            );


            Parallel.For(0, 1000, options, (i, loopkstate) =>
            {
                opeImage.ConvertStringToColor("Random", Color.Green, Color.Blue, out n);
                lock (syncObject)
                {
                    Assert.AreNotEqual(n, Color.Green);
                    if (n == Color.Yellow)
                    {
                        Debug.WriteLine("Yellow");
                    }
                }
            }
            );

            Parallel.For(0, 1000, options, (i, loopkstate) =>
            {
                opeImage.ConvertStringToColor("Random", Color.Red, Color.Green, out n);
                lock (syncObject)
                {
                    Assert.AreNotEqual(n, Color.Red);
                    if (n == Color.Yellow)
                    {
                        Debug.WriteLine("Yellow");
                    }
                }
            }
            );

            Parallel.For(0, 1000, options, (i, loopkstate) =>
            {
                opeImage.ConvertStringToColor("Random", Color.Yellow, Color.Green, out n);
                lock (syncObject)
                {
                    Assert.AreNotEqual(n, Color.Yellow);
                }
            }
            );

        }

        [TestMethod()]
        public void ConvertStringToCpCorrectCondtionTest()
        {
            var subForm = new Form();
            var picbox = new PictureBox();
            picbox.Width = 1920;
            picbox.Height = 1080;
            var picboxSub = new PictureBox();
            picboxSub.Width = 800;
            picboxSub.Height = 600;
            var preferenceDat = new PreferencesDat();
            var opeImage = new OpeImage(subForm, picbox, preferenceDat);

            ECpCorrectCondition eccc;
            opeImage.ConvertStringToCpCorrectCondtion(ECpCorrectCondition.Coordinate.ToString(), out eccc);
            Assert.AreEqual(eccc, ECpCorrectCondition.Coordinate);
            opeImage.ConvertStringToCpCorrectCondtion(ECpCorrectCondition.Color.ToString(), out eccc);
            Assert.AreEqual(eccc, ECpCorrectCondition.Color);
            opeImage.ConvertStringToCpCorrectCondtion(ECpCorrectCondition.Shape.ToString(), out eccc);
            Assert.AreEqual(eccc, ECpCorrectCondition.Shape);

            Assert.ThrowsException<Exception>(() =>
            {
                opeImage.ConvertStringToCpCorrectCondtion("hogehoge".ToString(), out eccc);
            });
        }

        [TestMethod()]
        public void ConvertEcpColorToColorTest()
        {
            var subForm = new Form();
            var picbox = new PictureBox();
            var picboxSub = new PictureBox();
            var preferenceDat = new PreferencesDat();
            var opeImage = new OpeImage(subForm, picbox, preferenceDat);
            var n = new Color();

            Assert.AreEqual(opeImage.ConvertEcpColorToColor(ECpColor.Black, Color.Green), Color.Black);
            Assert.AreEqual(opeImage.ConvertEcpColorToColor(ECpColor.Blue, Color.Green), Color.Blue);
            Assert.AreEqual(opeImage.ConvertEcpColorToColor(ECpColor.Green, Color.Blue), Color.Green);

            Assert.AreEqual(opeImage.ConvertEcpColorToColor(ECpColor.Green, Color.Green), Color.Green);

            Assert.AreEqual(opeImage.ConvertEcpColorToColor(ECpColor.Red, Color.Green), Color.Red);
            Assert.AreEqual(opeImage.ConvertEcpColorToColor(ECpColor.White, Color.Green), Color.White);
            Assert.AreEqual(opeImage.ConvertEcpColorToColor(ECpColor.Yellow, Color.Green), Color.Yellow);

            ParallelOptions options = new ParallelOptions();
            object syncObject = new object();

            Parallel.For(0, 1000, options, (i, loopkstate) =>
            {
                lock (syncObject)
                {
                    n = opeImage.ConvertEcpColorToColor(ECpColor.Random, Color.Green);
                    Assert.AreNotEqual(n, Color.Green);
                }
            });
            Debug.WriteLine(n.ToString());

            Parallel.For(0, 1000, options, (i, loopkstate) =>
            {
                lock (syncObject)
                {
                    n = opeImage.ConvertEcpColorToColor(ECpColor.Random, Color.Black);
                    Assert.AreNotEqual(n, Color.Black);
                }
            });
            Debug.WriteLine(n.ToString());

            Parallel.For(0, 1000, options, (i, loopkstate) =>
            {
                lock (syncObject)
                {
                    n = opeImage.ConvertEcpColorToColor(ECpColor.Random, Color.Blue);
                    Assert.AreNotEqual(n, Color.Blue);
                }
            });
            Debug.WriteLine(n.ToString());

            Parallel.For(0, 1000, options, (i, loopkstate) =>
            {
                lock (syncObject)
                {
                    n = opeImage.ConvertEcpColorToColor(ECpColor.Random, Color.Red);
                    Assert.AreNotEqual(n, Color.Red);
                }
            });
            Debug.WriteLine(n.ToString());

            Parallel.For(0, 1000, options, (i, loopkstate) =>
            {
                lock (syncObject)
                {
                    n = opeImage.ConvertEcpColorToColor(ECpColor.Random, Color.Yellow);
                    Assert.AreNotEqual(n, actual: Color.Yellow);
                }
            });
            Debug.WriteLine(n.ToString());
        }
        [TestMethod(), TestCategory("MakePoint")]
        public void GeneratePointTest()
        {
            var subForm = new Form();
            var picbox = new PictureBox();
            picbox.Width = 1920;
            picbox.Height = 1080;
            var picboxSub = new PictureBox();
            picboxSub.Width = 800;
            picboxSub.Height = 600;
            var preferenceDat = new PreferencesDat();
            var opeImage = new OpeImage(subForm, picbox, preferenceDat);
            var privateObject = new PrivateObject(opeImage);

            preferenceDat.SizeOfShapeInPixelForStep4 = 200;
            preferenceDat.SizeOfShapeInPixelForStep5 = 250;
            preferenceDat.TypeOfShape = "Circle";
            preferenceDat.SizeOfShapeInStep = "Step5";
            preferenceDat.IncorrectNum = 1;
            opeImage.InitOpeImage(picbox, picboxSub);
            opeImage.SetParamOfShapeOpeImage(preferenceDat.TypeOfShape, preferenceDat.SizeOfShapeInStep, preferenceDat.SizeOfShapeInPixel);

            var minDistance = opeImage.OpeImageSizeOfShapeInPixel + preferenceDat.MinDistanceBetweenCorrectAndWrongShape;

            //Point[] points = { new Point(60, 60), new Point(180, 180), new Point(240, 240), new Point(360, 360) };
            Point[] points = new Point[5];
            Point dstPoint = new Point();

            //privateObject.Invoke("GeneratePoint", points, dstPoint, 20);
            //opeImage.GeneratePoint(points, out dstPoint, 20);


            ParallelOptions options = new ParallelOptions();
            object syncObject = new object();
            int count = 0;

            Point correctPoint;
            opeImage.GeneratePoint(null, out correctPoint, minDistance);
            points[4] = correctPoint;
            //try
            //{
            //    Parallel.For(0, 10000, options, (i, loopstate) =>
            //    {
            //        lock (syncObject)
            //        {
            //            Assert.IsTrue(opeImage.GeneratePoint(points, out dstPoint, minDistance));
            //        }
            //    });
            //}
            //catch (Exception)
            //{
            //    Debug.WriteLine(dstPoint.ToString());
            //    Assert.Fail();
            //}



            for (int i = 0; i < 1000000; i++)
            {
                // Test毎に初期化
                points = new Point[5];
                points[4] = correctPoint;

                for (int j = 0; j < preferenceDat.IncorrectNum; j++)
                {

                    if (!opeImage.GeneratePoint(points, out dstPoint, minDistance))
                    {
                        points = new Point[5];
                        points[4] = correctPoint;
                        Debug.WriteLine("Not found points. Reset points!:" + count.ToString());
                    }

                    count++;
                    points[j] = dstPoint;
                }
                //foreach (Point n in points)
                //{
                //    if (n.X == 0 || n.Y == 0)
                //    {
                //        //Assert.Fail();
                //    }

                //}
                //Debug.WriteLine(points[0].ToString());
                //Debug.WriteLine(points[1].ToString());
                //Debug.WriteLine(points[2].ToString());
            }

            Debug.WriteLine(points[0].ToString());
            Debug.WriteLine(points[1].ToString());
            Debug.WriteLine(points[2].ToString());
            Debug.WriteLine(points[3].ToString());
            Debug.WriteLine(points[4].ToString());

        }

        [TestMethod(), TestCategory("MakePoint")]
        public void GenerateIncorrectPointTest()
        {
            var subForm = new Form();
            var picbox = new PictureBox();
            picbox.Width = 800;
            picbox.Height = 600;
            var picboxSub = new PictureBox();
            picboxSub.Width = 800;
            picboxSub.Height = 600;
            var preferenceDat = new PreferencesDat();
            var opeImage = new OpeImage(subForm, picboxSub, preferenceDat);
            var privateObject = new PrivateObject(opeImage);

            preferenceDat.SizeOfShapeInPixelForStep4 = 200;
            preferenceDat.SizeOfShapeInPixelForStep5 = 250;
            preferenceDat.TypeOfShape = "Circle";
            preferenceDat.SizeOfShapeInStep = "Step5";
            preferenceDat.IncorrectNum = 1;

            opeImage.IsThereSubDisplay = true;
            opeImage.InitOpeImage(picbox, picboxSub);
            opeImage.SetParamOfShapeOpeImage(preferenceDat.TypeOfShape, preferenceDat.SizeOfShapeInStep, preferenceDat.SizeOfShapeInPixel);
            var minDistance = opeImage.OpeImageSizeOfShapeInPixel + preferenceDat.MinDistanceBetweenCorrectAndWrongShape;

            Point[] points;
            ParallelOptions options = new ParallelOptions();
            object syncObject = new object();

            Point correctPoint = new Point(100, 100);
            points = opeImage.GenerateIncorrectPoint(correctPoint, minDistance);


            Debug.WriteLine(points[0].ToString());
            Debug.WriteLine(points[1].ToString());
            Debug.WriteLine(points[2].ToString());
            Debug.WriteLine(points[3].ToString());

            Assert.IsTrue(points[0].X < picboxSub.Width);
            Assert.IsTrue(points[0].Y < picboxSub.Height);
            Assert.IsTrue(points[1].X < picboxSub.Width);
            Assert.IsTrue(points[1].Y < picboxSub.Height);
            Assert.IsTrue(points[2].X < picboxSub.Width);
            Assert.IsTrue(points[2].Y < picboxSub.Height);
            Assert.IsTrue(points[3].X < picboxSub.Width);
            Assert.IsTrue(points[3].Y < picboxSub.Height);

        }
        [TestMethod()]
        public void MakeCorrectShapeTest()
        {
            var subForm = new Form();
            var picbox = new PictureBox();
            picbox.Width = 1920;
            picbox.Height = 1080;
            var picboxSub = new PictureBox();
            picboxSub.Width = 800;
            picboxSub.Height = 600;
            var preferenceDat = new PreferencesDat();
            var opeImage = new OpeImage(subForm, picbox, preferenceDat);
            var privateObject = new PrivateObject(opeImage);
            opeImage.InitOpeImage(picbox, picboxSub);

            opeImage.SetParamOfShapeOpeImage("Circle", "Step1", 10);
            opeImage.ResetWrongShapePoint();
            opeImage.GetRandomShape(ECpShape.None);

            object syncObject = new object();
            preferenceDat.MinDistanceBetweenCorrectAndWrongShape = 20;

            preferenceDat.CoordinateFixX = 320;
            preferenceDat.CoordinateFixY = 320;
            preferenceDat.ShapeFix = "Circle";
            preferenceDat.ColorFix = "Red";




            // 座標同じ
            preferenceDat.CorrectCondition = ECpCorrectCondition.Coordinate.ToString();
            preferenceDat.RandomCoordinate = true;
            preferenceDat.IncorrectCoordinateRandom = true;
            preferenceDat.OpeTypeOfTask = ECpTask.DelayMatch.ToString();
            preferenceDat.RandomShape = true;
            preferenceDat.RandomColor = true;
            preferenceDat.EnableImageShape = false;
            //Coordinae Random
            {
                opeImage.UpdatePreferences(preferenceDat);
                opeImage.MakeCorrectShape();

                Assert.IsNotNull(opeImage.ShapeOpeImageCorrectShape);
                Assert.IsNotNull(opeImage.ShapeOpeImageTrainShape);
                //内部条件による結果を判断
                Assert.AreNotEqual(opeImage.ShapeOpeImageCorrectShape.Shape, opeImage.ShapeOpeImageTrainShape.Shape);
                Assert.AreNotEqual(opeImage.ShapeOpeImageCorrectShape.ShapeColor, opeImage.ShapeOpeImageTrainShape.ShapeColor);
                Assert.AreEqual(opeImage.ShapeOpeImageCorrectShape.Point, opeImage.ShapeOpeImageTrainShape.Point);
                if (preferenceDat.EnableImageShape)
                {
                    Assert.IsNotNull(opeImage.ShapeOpeImageCorrectShape.ImageFilename);
                    Assert.IsNotNull(opeImage.ShapeOpeImageTrainShape.ImageFilename);
                }
            }

            preferenceDat.RandomCoordinate = false;

            //Coordinae Fix
            {
                opeImage.UpdatePreferences(preferenceDat);
                opeImage.MakeCorrectShape();

                Assert.IsNotNull(opeImage.ShapeOpeImageCorrectShape);
                Assert.IsNotNull(opeImage.ShapeOpeImageTrainShape);

                //内部条件による結果を判断
                Assert.AreNotEqual(opeImage.ShapeOpeImageCorrectShape.Shape, opeImage.ShapeOpeImageTrainShape.Shape);
                Assert.AreNotEqual(opeImage.ShapeOpeImageCorrectShape.ShapeColor, opeImage.ShapeOpeImageTrainShape.ShapeColor);
                Assert.AreEqual(opeImage.ShapeOpeImageCorrectShape.Point, opeImage.ShapeOpeImageTrainShape.Point);
            }
            // 形状同じ
            preferenceDat.CorrectCondition = ECpCorrectCondition.Shape.ToString();
            preferenceDat.RandomCoordinate = true;
            {
                opeImage.UpdatePreferences(preferenceDat);
                opeImage.MakeCorrectShape();

                Assert.IsNotNull(opeImage.ShapeOpeImageCorrectShape);
                Assert.IsNotNull(opeImage.ShapeOpeImageTrainShape);

                //内部条件による結果を判断
                Assert.AreEqual(opeImage.ShapeOpeImageCorrectShape.Shape, opeImage.ShapeOpeImageTrainShape.Shape);
                Assert.AreNotEqual(opeImage.ShapeOpeImageCorrectShape.ShapeColor, opeImage.ShapeOpeImageTrainShape.ShapeColor);
                Assert.AreNotEqual(opeImage.ShapeOpeImageCorrectShape.Point, opeImage.ShapeOpeImageTrainShape.Point);
            }
            preferenceDat.RandomShape = false;
            {
                opeImage.UpdatePreferences(preferenceDat);
                opeImage.MakeCorrectShape();

                Assert.IsNotNull(opeImage.ShapeOpeImageCorrectShape);
                Assert.IsNotNull(opeImage.ShapeOpeImageTrainShape);

                //内部条件による結果を判断
                Assert.AreEqual(opeImage.ShapeOpeImageCorrectShape.Shape, opeImage.ShapeOpeImageTrainShape.Shape);
                Assert.AreNotEqual(opeImage.ShapeOpeImageCorrectShape.ShapeColor, opeImage.ShapeOpeImageTrainShape.ShapeColor);
                Assert.AreNotEqual(opeImage.ShapeOpeImageCorrectShape.Point, opeImage.ShapeOpeImageTrainShape.Point);
            }
            // 色同じ
            preferenceDat.CorrectCondition = ECpCorrectCondition.Color.ToString();
            preferenceDat.RandomShape = true;
            {
                opeImage.UpdatePreferences(preferenceDat);
                opeImage.MakeCorrectShape();

                Assert.IsNotNull(opeImage.ShapeOpeImageCorrectShape);
                Assert.IsNotNull(opeImage.ShapeOpeImageTrainShape);

                //内部条件による結果を判断する
                Assert.AreNotEqual(opeImage.ShapeOpeImageCorrectShape.Shape, opeImage.ShapeOpeImageTrainShape.Shape);
                Assert.AreEqual(opeImage.ShapeOpeImageCorrectShape.ShapeColor, opeImage.ShapeOpeImageTrainShape.ShapeColor);
                Assert.AreNotEqual(opeImage.ShapeOpeImageCorrectShape.Point, opeImage.ShapeOpeImageTrainShape.Point);
            }
            preferenceDat.RandomColor = false;
            {
                opeImage.UpdatePreferences(preferenceDat);
                opeImage.MakeCorrectShape();

                Assert.IsNotNull(opeImage.ShapeOpeImageCorrectShape);
                Assert.IsNotNull(opeImage.ShapeOpeImageTrainShape);

                //内部条件による結果を判断する
                Assert.AreNotEqual(opeImage.ShapeOpeImageCorrectShape.Shape, opeImage.ShapeOpeImageTrainShape.Shape);
                Assert.AreEqual(opeImage.ShapeOpeImageCorrectShape.ShapeColor, opeImage.ShapeOpeImageTrainShape.ShapeColor);
                Assert.AreNotEqual(opeImage.ShapeOpeImageCorrectShape.Point, opeImage.ShapeOpeImageTrainShape.Point);
            }
        }
        [TestMethod()]
        public void GetRandomColorTest()
        {
            var subForm = new Form();
            var picbox = new PictureBox();
            picbox.Width = 1920;
            picbox.Height = 1080;
            var picboxSub = new PictureBox();
            picboxSub.Width = 800;
            picboxSub.Height = 600;
            var preferenceDat = new PreferencesDat();
            var opeImage = new OpeImage(subForm, picbox, preferenceDat);
            var privateObject = new PrivateObject(opeImage);
            opeImage.InitOpeImage(picbox, picboxSub);

            opeImage.SetParamOfShapeOpeImage("Circle", "Step1", 10);
            opeImage.ResetWrongShapePoint();
            opeImage.GetRandomShape(ECpShape.None);

            object syncObject = new object();

            var n = new Color();

            ParallelOptions options = new ParallelOptions();
            Parallel.For(0, 1000, options, (i, loopkstate) =>
            {
                n = opeImage.GetRandomColor(Color.Empty, Color.Green);
                lock (syncObject)
                {
                    Assert.AreNotEqual(n, Color.Green);
                    Assert.AreNotEqual(n, Color.Empty);
                }
            }
            );
            Parallel.For(0, 1000, options, (i, loopkstate) =>
            {
                n = opeImage.GetRandomColor(Color.Red, Color.Green);
                lock (syncObject)
                {
                    Assert.AreNotEqual(n, Color.Green);
                    Assert.AreNotEqual(n, Color.Red);
                }
            }
            );
            Parallel.For(0, 1000, options, (i, loopkstate) =>
            {
                n = opeImage.GetRandomColor(Color.White, Color.Green);
                lock (syncObject)
                {
                    Assert.AreNotEqual(n, Color.Green);
                    Assert.AreNotEqual(n, Color.White);
                }
            }
            );
        }

        [TestMethod()]
        public void GetRandomShapeTest()
        {
            var subForm = new Form();
            var picbox = new PictureBox();
            picbox.Width = 1920;
            picbox.Height = 1080;
            var picboxSub = new PictureBox();
            picboxSub.Width = 800;
            picboxSub.Height = 600;
            var preferenceDat = new PreferencesDat();
            var opeImage = new OpeImage(subForm, picbox, preferenceDat);
            var privateObject = new PrivateObject(opeImage);
            opeImage.InitOpeImage(picbox, picboxSub);
            opeImage.SetParamOfShapeOpeImage("Circle", "Step1", 10);
            opeImage.ResetWrongShapePoint();
            opeImage.GetRandomShape(ECpShape.None);

            object syncObject = new object();

            var n = new ECpShape();

            ParallelOptions options = new ParallelOptions();
            Parallel.For(0, 1000, options, (i, loopkstate) =>
            {
                n = opeImage.GetRandomShape(ECpShape.Circle);
                lock (syncObject)
                {
                    Assert.AreNotEqual(n, ECpShape.Circle);
                    if (n == ECpShape.Circle)
                    {
                        Debug.WriteLine("Circle");
                    }
                }
            }
            );

            Parallel.For(0, 1000, options, (i, loopkstate) =>
            {
                n = opeImage.GetRandomShape(ECpShape.Triangle);
                lock (syncObject)
                {
                    Assert.AreNotEqual(n, ECpShape.Triangle);
                    if (n == ECpShape.Triangle)
                    {
                        Debug.WriteLine("Circle");
                    }
                }
            }
            );

            Parallel.For(0, 1000, options, (i, loopkstate) =>
            {
                n = opeImage.GetRandomShape(ECpShape.Rectangle);
                lock (syncObject)
                {
                    Assert.AreNotEqual(n, ECpShape.Rectangle);
                    if (n == ECpShape.Rectangle)
                    {
                        Debug.WriteLine("Circle");
                    }
                }
            }
            );

            Parallel.For(0, 1000, options, (i, loopkstate) =>
            {
                n = opeImage.GetRandomShape(ECpShape.Star);
                lock (syncObject)
                {
                    Assert.AreNotEqual(n, ECpShape.Star);
                    if (n == ECpShape.Star)
                    {
                        Debug.WriteLine("Star");
                    }
                }
            }
            );

        }
        [TestMethod()]
        public void GetRandomShapesTest()
        {
            var subForm = new Form();
            var picbox = new PictureBox();
            picbox.Width = 1920;
            picbox.Height = 1080;
            var picboxSub = new PictureBox();
            picboxSub.Width = 800;
            picboxSub.Height = 600;
            var preferenceDat = new PreferencesDat();
            var opeImage = new OpeImage(subForm, picbox, preferenceDat);
            var privateObject = new PrivateObject(opeImage);
            opeImage.InitOpeImage(picbox, picboxSub);
            opeImage.SetParamOfShapeOpeImage("Circle", "Step1", 10);
            opeImage.ResetWrongShapePoint();


            object syncObject = new object();
            ECpShape[] existsShapes;

            var n = new List<ECpShape>();
            n.Add(opeImage.GetRandomShape(ECpShape.None));

            var ssdfa = new ECpShape[3] { ECpShape.None, ECpShape.Random, ECpShape.Image };
            ParallelOptions options = new ParallelOptions();
            for (int i = 0; i < 20; i++)
            {
                n.Add(opeImage.GetRandomShape(n.ToArray(), ssdfa));
                lock (syncObject)
                {
                    Assert.IsFalse(n.GroupBy(x => x).Any(s => s.Skip(1).Any()));
                }
            }

            //Parallel.For(0, 1000, options, (i, loopkstate) =>
            //{
            //    n = opeImage.GetRandomShape(ECpShape.Triangle);
            //    lock (syncObject)
            //    {
            //        Assert.AreNotEqual(n, ECpShape.Triangle);
            //        if (n == ECpShape.Triangle)
            //        {
            //            Debug.WriteLine("Circle");
            //        }
            //    }
            //}
            //);

            //Parallel.For(0, 1000, options, (i, loopkstate) =>
            //{
            //    n = opeImage.GetRandomShape(ECpShape.Rectangle);
            //    lock (syncObject)
            //    {
            //        Assert.AreNotEqual(n, ECpShape.Rectangle);
            //        if (n == ECpShape.Rectangle)
            //        {
            //            Debug.WriteLine("Circle");
            //        }
            //    }
            //}
            //);

            //Parallel.For(0, 1000, options, (i, loopkstate) =>
            //{
            //    n = opeImage.GetRandomShape(ECpShape.Star);
            //    lock (syncObject)
            //    {
            //        Assert.AreNotEqual(n, ECpShape.Star);
            //        if (n == ECpShape.Star)
            //        {
            //            Debug.WriteLine("Star");
            //        }
            //    }
            //}
            //);

        }
        [TestMethod()]
        public void MakeEpisodeShapeTest()
        {
            var subForm = new Form();
            var picbox = new PictureBox();
            picbox.Width = 1920;
            picbox.Height = 1080;
            var picboxSub = new PictureBox();
            picboxSub.Width = 800;
            picboxSub.Height = 600;
            var preferenceDat = new PreferencesDat();
            var opeImage = new OpeImage(subForm, picbox, preferenceDat);
            var privateObject = new PrivateObject(opeImage);
            opeImage.InitOpeImage(picbox, picboxSub);

            opeImage.SetParamOfShapeOpeImage("Circle", "Step1", 10);
            opeImage.ResetWrongShapePoint();
            opeImage.GetRandomShape(ECpShape.None);

            object syncObject = new object();
            opeImage.MakeEpisodeShape();
            preferenceDat.IncorrectColorRandom = false;
            _ = opeImage.MakeIncorrectShape();

            Assert.IsNotNull(opeImage.EpisodeShapes);

            preferenceDat.IncorrectColorRandom = true;
            opeImage.MakeEpisodeShape();

            Assert.IsNotNull(opeImage.EpisodeShapes);

            preferenceDat.IncorrectColorRandom = true;
            preferenceDat.IncorrectNum = 4;
            opeImage.MakeEpisodeShape();

            Assert.IsNotNull(opeImage.EpisodeShapes);
        }

        [TestMethod()]
        public void GetBitmapDrawPointTest()
        {
            var subForm = new Form();
            var picbox = new PictureBox();
            picbox.Width = 1920;
            picbox.Height = 1080;
            var picboxSub = new PictureBox();
            picboxSub.Width = 800;
            picboxSub.Height = 600;
            var preferenceDat = new PreferencesDat();
            var opeImage = new OpeImage(subForm, picbox, preferenceDat);
            var privateObject = new PrivateObject(opeImage);
            opeImage.InitOpeImage(picbox, picboxSub);

            Point drawPoint = new Point(100, 100);
            var sizeW = 50;
            var sizeH = 50;

            var ret = privateObject.Invoke("GetBitmapDrawPoint", new Bitmap(sizeW, sizeH), drawPoint);
            Assert.IsNotNull(ret);

            Assert.AreEqual(drawPoint.X - sizeW / 2, ((Point)ret).X);
            Assert.AreEqual(drawPoint.Y - sizeH / 2, ((Point)ret).Y);

        }
        //[TestMethod()]
        public void GetCorrectImageObjectTest()
        {
            var subForm = new Form();
            var picbox = new PictureBox();
            var picboxSub = new PictureBox();
            var preferenceDat = new PreferencesDat();

            preferenceDat.ImageFileFolder = @"C:\Users\kodama\Desktop\hage";
            preferenceDat.RandomCorrectImage = true;
            var opeImage = new OpeImage(subForm, picbox, preferenceDat);
            opeImage.OpeImageSizeOfShapeInPixel = 50;
            var privateObject = new PrivateObject(opeImage);

            //var ret = privateObject.Invoke("GetCorrectImageObject");
            string path = "";
            ParallelOptions options = new ParallelOptions();
            Parallel.For(0, 1000, options, (i, loopkstate) =>
            {
                var ret = privateObject.Invoke("GetCorrectImageObject", path);
                Assert.IsNotNull(ret);
            }
            );
        }

        [TestMethod()]
        public void CheckDrawSpaceTest()
        {
            var subForm = new Form();
            var picbox = new PictureBox();
            picbox.Width = 1920;
            picbox.Height = 1080;
            var picboxSub = new PictureBox();
            picboxSub.Width = 800;
            picboxSub.Height = 600;
            var preferenceDat = new PreferencesDat();
            var opeImage = new OpeImage(subForm, picbox, preferenceDat);
            var privateObject = new PrivateObject(opeImage);

            preferenceDat.SizeOfShapeInPixelForStep4 = 200;
            preferenceDat.SizeOfShapeInPixelForStep5 = 250;
            preferenceDat.TypeOfShape = "Circle";
            preferenceDat.SizeOfShapeInStep = "Step5";
            preferenceDat.IncorrectNum = 4;
            opeImage.InitOpeImage(picbox, picboxSub);
            opeImage.SetParamOfShapeOpeImage(preferenceDat.TypeOfShape, preferenceDat.SizeOfShapeInStep, preferenceDat.SizeOfShapeInPixel);

            var minDistance = opeImage.OpeImageSizeOfShapeInPixel + preferenceDat.MinDistanceBetweenCorrectAndWrongShape;


            Assert.IsTrue(opeImage.CheckDrawSpace());
        }

        [TestMethod()]
        public void CheckSvgShapeTest()
        {
            var a = OpeImage.CheckUnExistSvgShape();
            var b = Enum.GetValues(typeof(ECpShape)).Cast<ECpShape>().Where(e => e.ToString().Contains("Svg")).ToList();
            for (int i = 0; i < a.Count; i++)
            {
                Assert.AreEqual(a[i], b[i]);
            }
            //Assert.Fail();
        }
        [TestMethod()]
        public void CheckSvgShapeCircleTest()
        {
            var a = OpeImage.CheckExistSvgShape(ECpShape.SvgCircle);
            Assert.AreEqual(true, a);
            a = OpeImage.CheckExistSvgShape(ECpShape.SvgSquare);
            Assert.AreEqual(true, a);
            a = OpeImage.CheckExistSvgShape(ECpShape.SvgTriangle);
            Assert.AreEqual(true, a);
            a = OpeImage.CheckExistSvgShape(ECpShape.SvgStar);
            Assert.AreEqual(true, a);
        }

        [TestMethod()]
        public void GetStringToEcpShapeTest()
        {
            Assert.AreEqual(OpeImage.GetStringToEcpShape("SvgCircle"), ECpShape.SvgCircle);
            Assert.AreEqual(OpeImage.GetStringToEcpShape("SvgRectangle"), ECpShape.SvgRectangle);
            Assert.AreEqual(OpeImage.GetStringToEcpShape("SvgTriangle"), ECpShape.SvgTriangle);
            Assert.AreEqual(OpeImage.GetStringToEcpShape("SvgArrow"), ECpShape.SvgArrow);
            Assert.AreEqual(OpeImage.GetStringToEcpShape("SvgCresent"), ECpShape.SvgCresent);
            Assert.AreEqual(OpeImage.GetStringToEcpShape("SvgCross"), ECpShape.SvgCross);
            Assert.AreEqual(OpeImage.GetStringToEcpShape("SvgEllipse"), ECpShape.SvgEllipse);
            Assert.AreEqual(OpeImage.GetStringToEcpShape("SvgHeart"), ECpShape.SvgHeart);
            Assert.AreEqual(OpeImage.GetStringToEcpShape("SvgHexagon"), ECpShape.SvgHexagon);
            Assert.AreEqual(OpeImage.GetStringToEcpShape("SvgKite"), ECpShape.SvgKite);
            Assert.AreEqual(OpeImage.GetStringToEcpShape("SvgOctagon"), ECpShape.SvgOctagon);
            Assert.AreEqual(OpeImage.GetStringToEcpShape("SvgParallelogram"), ECpShape.SvgParallelogram);
            Assert.AreEqual(OpeImage.GetStringToEcpShape("SvgPentagon"), ECpShape.SvgPentagon);
            Assert.AreEqual(OpeImage.GetStringToEcpShape("SvgPic"), ECpShape.SvgPic);
            Assert.AreEqual(OpeImage.GetStringToEcpShape("SvgPolygon"), ECpShape.SvgPolygon);
            Assert.AreEqual(OpeImage.GetStringToEcpShape("SvgQuadrefoil"), ECpShape.SvgQuadrefoil);
            Assert.AreEqual(OpeImage.GetStringToEcpShape("SvgRhombus"), ECpShape.SvgRhombus);
            Assert.AreEqual(OpeImage.GetStringToEcpShape("SvgRight"), ECpShape.SvgRight);
            Assert.AreEqual(OpeImage.GetStringToEcpShape("SvgRing"), ECpShape.SvgRing);
            Assert.AreEqual(OpeImage.GetStringToEcpShape("SvgScalene"), ECpShape.SvgScalene);
            Assert.AreEqual(OpeImage.GetStringToEcpShape("SvgSemiCircle"), ECpShape.SvgSemiCircle);
            Assert.AreEqual(OpeImage.GetStringToEcpShape("SvgSquare"), ECpShape.SvgSquare);
            Assert.AreEqual(OpeImage.GetStringToEcpShape("SvgStar"), ECpShape.SvgStar);
            Assert.AreEqual(OpeImage.GetStringToEcpShape("SvgTrapeze"), ECpShape.SvgTrapeze);
            Assert.AreEqual(OpeImage.GetStringToEcpShape("SvgTrefoil"), ECpShape.SvgTrefoil);

        }

        [TestMethod()]
        public void CheckExistSvgShapeTest()
        {
            Debug.WriteLine(System.IO.Directory.GetCurrentDirectory());
            var a = OpeImage.CheckExistSvgShape();
            var b = Enum.GetValues(typeof(ECpShape)).Cast<ECpShape>().Where(e => e.ToString().Contains("Svg")).ToList();

            Assert.AreEqual(a.Count, 25);
            //for (int i = 0; i < a.Count; i++)
            //{
            //    Assert.AreEqual(a[i], b[i]);
            //}
        }

        [TestMethod()]
        public void GetGraphicsPathTest()
        {
            var subForm = new Form();
            var picbox = new PictureBox();
            picbox.Width = 1920;
            picbox.Height = 1080;
            var picboxSub = new PictureBox();
            picboxSub.Width = 800;
            picboxSub.Height = 600;
            var preferenceDat = new PreferencesDat();
            var opeImage = new OpeImage(subForm, picbox, preferenceDat);
            var privateObject = new PrivateObject(opeImage);

            preferenceDat.SizeOfShapeInPixelForStep4 = 200;
            preferenceDat.SizeOfShapeInPixelForStep5 = 250;
            preferenceDat.TypeOfShape = "Circle";
            preferenceDat.SizeOfShapeInStep = "Step5";
            preferenceDat.IncorrectNum = 1;
            opeImage.InitOpeImage(picbox, picboxSub);
            opeImage.SetParamOfShapeOpeImage(preferenceDat.TypeOfShape, preferenceDat.SizeOfShapeInStep, preferenceDat.SizeOfShapeInPixel);
            ShapeObject so = new ShapeObject();
            var minDistance = opeImage.OpeImageSizeOfShapeInPixel + preferenceDat.MinDistanceBetweenCorrectAndWrongShape;
            so.ShapeColor = Color.Black;


            string ImageFilename = @".\svg\circle.svg";
            Point correctPoint;
            Bitmap bitmap = opeImage.LoadSvgImage(ImageFilename, opeImage.OpeImageSizeOfShapeInPixel, opeImage.OpeImageSizeOfShapeInPixel, so.ShapeColor, ref so);
            _ = opeImage.GetGraphicsPath(bitmap, so.Point);


        }

        [TestMethod()]
        public void MakeAnyShapeTest()
        {
            var subForm = new Form();
            var picbox = new PictureBox();
            picbox.Width = 1920;
            picbox.Height = 1080;
            var picboxSub = new PictureBox();
            picboxSub.Width = 800;
            picboxSub.Height = 600;
            var preferenceDat = new PreferencesDat();
            var opeImage = new OpeImage(subForm, picbox, preferenceDat);
            var privateObject = new PrivateObject(opeImage);

            preferenceDat.SizeOfShapeInPixelForStep4 = 200;
            preferenceDat.SizeOfShapeInPixelForStep5 = 250;
            preferenceDat.TypeOfShape = "Circle";
            preferenceDat.SizeOfShapeInStep = "Step5";
            preferenceDat.IncorrectNum = 1;
            preferenceDat.EpisodeTargetNum = 5;
            preferenceDat.EpisodeRandomShape = true;
            preferenceDat.EpisodeRandomColor = true;
            opeImage.InitOpeImage(picbox, picboxSub);
            opeImage.SetParamOfShapeOpeImage(preferenceDat.TypeOfShape, preferenceDat.SizeOfShapeInStep, preferenceDat.SizeOfShapeInPixel);
            preferenceDat.EpisodeTargetNum = 5;
            opeImage.MakeAnyShape();
            preferenceDat.EpisodeTargetNum = 4;
            opeImage.MakeAnyShape();
            preferenceDat.EpisodeTargetNum = 3;
            opeImage.MakeAnyShape();
            preferenceDat.EpisodeTargetNum = 2;
            opeImage.MakeAnyShape();
            preferenceDat.EpisodeTargetNum = 1;
            opeImage.MakeAnyShape();
        }
    }
}