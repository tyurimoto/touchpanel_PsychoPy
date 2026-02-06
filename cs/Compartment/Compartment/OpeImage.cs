//#define DEBUG_OPEIMAGE

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Svg;

namespace Compartment
{
    /// <summary>
    /// <c>ShapeObject</c>
    /// 描画情報をまとめたクラス
    /// </summary>
    public class ShapeObject
    {
        /// <summary>
        /// 描画Point <seealso cref="System.Drawing.Point"/> 
        /// </summary>
        public Point Point { set; get; } = new Point(0, 0);
        /// <summary>
        /// 描画形状 <seealso cref="ECpShape"/> 
        /// </summary>
        public ECpShape Shape { set; get; } = ECpShape.Circle;
        /// <summary>
        /// 描画色 <seealso cref="Color"/> 
        /// </summary>
        public Color ShapeColor { set; get; } = Color.White;

        /// <summary>
        /// Image <seealso cref="Bitmap"/> 
        /// </summary>
        //public Bitmap Image { set; get; }
        /// <summary>
        /// Image Filename  <seealso cref="string"/> 
        /// </summary>
        public string ImageFilename { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        //今はAnyTouch時のみ利用
        public GraphicsPath ShapeObjectGraphicsPath { get; set; }

        public bool Touched { get; private set; }

        public void CheckTouch(Point point)
        {
            try
            {
                Touched = ShapeObjectGraphicsPath.IsVisible(point);
            }
            catch (Exception)
            {
                throw;
            }
        }

        //public GraphicsPath ShapeGraphicsPath { set; get; } = new GraphicsPath();
        public ShapeObject()
        {

        }
        // Pointジェネレート部を入れる？

    }

    public class OpeImage
    {
        readonly Form formSub;
        readonly PictureBox formSubPictureBox;
        PreferencesDat preferencesDatOriginal;

        const string targetFolder = @".\svg";

        const string svgfileCircle = "circle.svg";
        const string svgfileRectangle = "rectangle.svg";
        const string svgfileSquare = "square.svg";
        const string svgfileTriangle = "triangle.svg";
        const string svgfileScalene = "scalene.svg";
        const string svgfilePentagon = "pentagon.svg";
        const string svgfileRight = "right.svg";
        const string svgfileTrapeze = "trapeze.svg";
        const string svgfileKite = "kite.svg";
        const string svgfilePolygon = "polygon.svg";
        const string svgfileParallelogram = "parallelogram.svg";
        const string svgfileEllipse = "ellipse.svg";
        const string svgfileTrefoil = "trefoil.svg";
        const string svgfileStar = "star.svg";
        const string svgfileSemiCircle = "semicircle.svg";
        const string svgfileHexagon = "hexagon.svg";
        const string svgfileCresent = "cresent.svg";
        const string svgfileOctagon = "octagon.svg";
        const string svgfileCross = "cross.svg";
        const string svgfileRing = "ring.svg";
        const string svgfilePic = "pic.svg";
        const string svgfileHeart = "heart.svg";
        const string svgfileArrow = "arrow.svg";
        const string svgfileQuadrefoil = "quadrefoil.svg";
        const string svgfileRhombus = "rhombus.svg";

        /// <summary>
        /// OpeImageコンストラクタ
        /// </summary>
        /// <param name="subform">SubForm</param>
        /// <param name="pictureBox">親PictureBox</param>
        /// <param name="pd">PreferenceDat</param>
        public OpeImage(Form subform, PictureBox pictureBox, PreferencesDat pd)
        {
            formSub = subform;
            formSubPictureBox = pictureBox;
            preferencesDatOriginal = pd;
        }

        /// <summary>
        /// OpeImage初期値 タッチパネル側解像度 自動取得機能いれる？
        /// </summary>
        public enum OpeImageDefault : int
        {
            WidthOfWholeArea = 1920,
            HeightOfWholeArea = 1080,
            TimeoutOfTryToMakeWroingShapePoint = 350     // 350[ms]

        }
        public int WidthOfWholeArea { get; set; } = 800;
        public int HeightOfWholeArea { get; set; } = 600;


        private List<Point> _PointOpeImageCenterOfWrongShapeList;
        private List<ECpShape> _ShapeOpeImageWrongList;
        private List<Color> _ColorOpeImageWrongList;
        private string _CorrectImageFile;
        private readonly object syncPictureObject = new object();

        public string CorrectImageFile
        {
            private set { _CorrectImageFile = value; }
            get { return _CorrectImageFile; }
        }

        /// <summary>
        /// タッチ・パネルのピクチャー・ボックスの全表示領域
        /// </summary>
        public Rectangle RectOpeImageWholeArea { get; set; }

        /// <summary>
        /// タッチ・パネルの有効領域
        /// </summary>
        public Rectangle RectOpeImageValidArea { get; set; }

        /// <summary>
        /// タッチパネル描画領域
        /// </summary>
        public Rectangle RectOpeImageValidAreaToDrawShape { get; set; }

        /// <summary>
        /// 有効範囲内ピクセル数
        /// </summary>
        public int OpeImageSizeOfShapeInPixel { get; set; }

        /// <summary>
        /// 正答図形
        /// </summary>
        public ECpShape CorrectShape { get; set; }
        /// <summary>
        /// 不正解図形
        /// </summary>
        public ECpShape WrongShape { get; set; }

        // ランダム時保持用
        private ECpShape correctShape { get; set; }
        private ECpShape wrongShape { get; set; }

        /// <summary>
        /// 正答座標
        /// </summary>
        public Point PointOpeImageCenterOfCorrectShape { get; set; }

        /// <summary>
        /// 正答図形オブジェクト
        /// </summary>
        public ShapeObject ShapeOpeImageCorrectShape;

        /// <summary>
        /// 教示用オブジェクト
        /// </summary>
        public ShapeObject ShapeOpeImageTrainShape;

        /// <summary>
        /// 不正解用オブジェクト
        /// </summary>
        public ShapeObject[] IncorrectShapes;

        /// <summary>
        /// Episode用オブジェクト
        /// </summary>
        public ShapeObject[] EpisodeShapes;

        /// <summary>
        /// どれでもタッチ用オブジェクト
        /// </summary>
        public ShapeObject[] TouchAnyShapes;

        /// <summary>
        /// 不正解座標リスト
        /// </summary>
        public IReadOnlyList<Point> PointOpeImageCenterOfWrongShapeList
        {
            get { return _PointOpeImageCenterOfWrongShapeList; }
            set { }
        }

        /// <summary>
        /// 不正解形状リスト
        /// </summary>
        public IReadOnlyList<ECpShape> ShapeOpeImageWrongList
        {
            get { return _ShapeOpeImageWrongList; }
            set { }
        }

        /// <summary>
        /// 不正解カラーリスト
        /// </summary>
        public IReadOnlyList<Color> ColorOpeImageWrongList
        {
            get { return _ColorOpeImageWrongList; }
            set { }
        }


        public PictureBox mPictureBoxOpeImageFromMain = null;
        public PictureBox mPictureBoxOpeImageFromSub = null;

        public GraphicsPath mGraphicsPathOpeImageShape = null;
        public GraphicsPath mGraphicsPathOpeIncorrectImageShape = null;

        /// <summary>
        /// 正答図形色
        /// </summary>
        public Color CorrectShapeColor { get; set; } = Color.White;
        /// <summary>
        /// 不正解図形色 単一用未使用に
        /// </summary>
        public Color WrongShapeColor { get; set; } = Color.White;
        /// <summary>
        /// 背景色
        /// </summary>
        public Color ColorOpeImageBackColor { get; set; } = Color.Green;
        /// <summary>
        /// 遅延時背景色
        /// </summary>
        public Color ColorOpeImageDelayBackColor { get; set; } = Color.Green;

        /// <summary>
        /// 描画キャンバス
        /// </summary>
        public Bitmap mBitmapOpeImageCanvas;

        /// <summary>
        /// OpeImage共通StopWatch
        /// </summary>
        public Stopwatch mStopwatchOpeImage = new Stopwatch();

        /// <summary>
        /// OpeImage共通Random
        /// </summary>
        private readonly Random opeImageRandom = new Random();

        // サブ・ディスプレイが存在するか否かを表すフラグ
        public bool IsThereSubDisplay = false;

        /// <summary>
        /// Preferrenceを更新
        /// </summary>
        /// <param name="pd">PreferrenceDatインスタンス</param>
        public void UpdatePreferences(PreferencesDat pd)
        {
            preferencesDatOriginal = pd;
        }
        public void UpdateCanvas()
        {
            mPictureBoxOpeImageFromMain.Refresh();
            mPictureBoxOpeImageFromSub.Refresh();
        }

        private Graphics graphicsObjGlobal;
        /// <summary>
        /// 画像関連処理の初期設定を行う
        /// </summary>
        /// <param name="pictureBoxFormMainArg"></param>
        /// <param name="pictureBoxFormSubArg"></param>
        /// <returns></returns>
        public bool InitOpeImage(PictureBox pictureBoxFormMainArg, PictureBox pictureBoxFormSubArg)
        {
            bool boolRet = true;
            Color colorBackVal = new Color();
            Color colorDelayBackVal = new Color();
            int iWidthOfWholeArea;
            int iHeightOfWholeArea;
            int iXOfValidArea;
            int iYOfValidArea;
            int iWidthOfValidArea;
            int iHeightOfValidArea;

            // 表示対象ピクチャー・ボックスを保存
            mPictureBoxOpeImageFromMain = pictureBoxFormMainArg;
            mPictureBoxOpeImageFromSub = pictureBoxFormSubArg;

            if (ConvertStringToBgColor(preferencesDatOriginal.BackColor, out colorBackVal) != true)
            {
                // 背景色設定エラーの時、緑とする
                ColorOpeImageBackColor = Color.Green;
                throw new Exception(String.Format("Setting error in {0}: Back color:{1} is over range",
                        MethodBase.GetCurrentMethod().Name,
                        preferencesDatOriginal.BackColor));
            }
            ColorOpeImageBackColor = colorBackVal;

            if (ConvertStringToBgColor(preferencesDatOriginal.DelayBackColor, out colorDelayBackVal) != true)
            {
                // 背景色設定エラーの時、黒とする
                ColorOpeImageDelayBackColor = Color.Black;
                throw new Exception(String.Format("Setting error in {0}: Back color:{1} is over range",
                        MethodBase.GetCurrentMethod().Name,
                        preferencesDatOriginal.DelayBackColor));
            }
            ColorOpeImageDelayBackColor = colorDelayBackVal;

            try
            {
                // サブ・ディスプレイが存在する時
                if (IsThereSubDisplay == true)
                {
                    iWidthOfWholeArea = formSubPictureBox.Width;
                    iHeightOfWholeArea = formSubPictureBox.Height;
                }
                else
                {
                    // サブ・ディスプレイが存在しない時
                    iWidthOfWholeArea = (int)OpeImageDefault.WidthOfWholeArea;
                    iHeightOfWholeArea = (int)OpeImageDefault.HeightOfWholeArea;
                }
                // 全体領域を設定
                RectOpeImageWholeArea = new Rectangle
                {
                    X = 0,
                    Y = 0,
                    Width = iWidthOfWholeArea,
                    Height = iHeightOfWholeArea
                };
                // BITMAPキャンバスを生成
                mBitmapOpeImageCanvas = new Bitmap(iWidthOfWholeArea, iHeightOfWholeArea);
                graphicsObjGlobal = Graphics.FromImage(mBitmapOpeImageCanvas);
                // 範囲検査
                iWidthOfValidArea = iWidthOfWholeArea - preferencesDatOriginal.TouchPanelInvalidLeftInPixel - preferencesDatOriginal.TouchPanelInvalidRightInPixel;
                iHeightOfValidArea = iHeightOfWholeArea - preferencesDatOriginal.TouchPanelInvalidTopInPixel - preferencesDatOriginal.TouchPanelInvalidBottomInPixel;

                if ((preferencesDatOriginal.TouchPanelInvalidLeftInPixel >= iWidthOfWholeArea) ||
                   (preferencesDatOriginal.TouchPanelInvalidRightInPixel >= iWidthOfWholeArea) ||
                   (preferencesDatOriginal.TouchPanelInvalidTopInPixel >= iHeightOfWholeArea) ||
                   (preferencesDatOriginal.TouchPanelInvalidBottomInPixel >= iHeightOfWholeArea) ||
                   (iWidthOfValidArea <= 1) ||
                   (iHeightOfValidArea <= 1))
                {
                    // 範囲外の時、有効領域を全領域とする
                    //					iXOfValidArea = RectOpeImageWholeArea.X;
                    //					iYOfValidArea = RectOpeImageWholeArea.Y;
                    //					iWidthOfValidArea = RectOpeImageWholeArea.Width;
                    //					iHeightOfValidArea = RectOpeImageWholeArea.Height;
                    throw new Exception(String.Format("Setting error in {0}: WholeArea[Width:{1} Height{2}] InvalidArea[Left:{3} Rigth:{4} Top:{5} Bottom:{6}",
                        MethodBase.GetCurrentMethod().Name,
                        iWidthOfWholeArea, iHeightOfWholeArea,
                        preferencesDatOriginal.TouchPanelInvalidLeftInPixel, preferencesDatOriginal.TouchPanelInvalidRightInPixel,
                        preferencesDatOriginal.TouchPanelInvalidTopInPixel, preferencesDatOriginal.TouchPanelInvalidBottomInPixel));
                }
                else
                {
                    // 範囲内の時
                    iXOfValidArea = preferencesDatOriginal.TouchPanelInvalidLeftInPixel;
                    iYOfValidArea = preferencesDatOriginal.TouchPanelInvalidTopInPixel;
                }
                // 有効領域を設定
                RectOpeImageValidArea = new Rectangle
                {
                    X = iXOfValidArea,
                    Y = iYOfValidArea,
                    Width = iWidthOfValidArea,
                    Height = iHeightOfValidArea
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
            {
                Debug.WriteLine("InitOpeImage().Start:----------");
                // 全体領域を出力
                Debug.WriteLine("  WholeArea:[X:{0} Y:{1} Width:{2} Height:{3}]",
                                    RectOpeImageWholeArea.X,
                                    RectOpeImageWholeArea.Y,
                                    RectOpeImageWholeArea.Width,
                                    RectOpeImageWholeArea.Height);
                // 有効領域を出力
                Debug.WriteLine("  ValidArea:[X:{0} Y:{1} Width:{2} Height:{3}]",
                                    RectOpeImageValidArea.X,
                                    RectOpeImageValidArea.Y,
                                    RectOpeImageValidArea.Width,
                                    RectOpeImageValidArea.Height);
                Debug.WriteLine("InitOpeImage().End:----------");
            }
            return boolRet;
        }

        public bool ConvertStringToCpStep(string stringStepArg, out ECpStep CpStepArgOut)
        {
            bool boolRet = true;


            if (stringStepArg == ECpStep.Step1.ToString())
            {
                CpStepArgOut = ECpStep.Step1;
            }
            else if (stringStepArg == ECpStep.Step2.ToString())
            {
                CpStepArgOut = ECpStep.Step2;
            }
            else if (stringStepArg == ECpStep.Step3.ToString())
            {
                CpStepArgOut = ECpStep.Step3;
            }
            else if (stringStepArg == ECpStep.Step4.ToString())
            {
                CpStepArgOut = ECpStep.Step4;
            }
            else if (stringStepArg == ECpStep.Step5.ToString())
            {
                CpStepArgOut = ECpStep.Step5;
            }
            else
            {
                CpStepArgOut = ECpStep.Step1;
                return false;
            }
            return boolRet;
        }

        /// <summary>
        /// ECpShapeからランダムに取得
        /// </summary>
        /// <param name="exclusionShape">除外形状</param>
        /// <returns>ECpShape</returns>
        public ECpShape GetRandomShape(ECpShape exclusionShape)
        {
            // 無効図形3つ
            var n = Enum.GetValues(typeof(ECpShape)).Length - 3;
            var targetNum = opeImageRandom.Next(0, n);

            IEnumerable<ECpShape> enumerableECpShape = Enum.GetValues(typeof(ECpShape)).Cast<ECpShape>();
            // 存在しないSVG形状を除外
            List<ECpShape> nonExistShapeList = CheckUnExistSvgShape();
            IEnumerable<ECpShape> targetShapeList = enumerableECpShape.Where(ees => !nonExistShapeList.Contains(ees) && ees != exclusionShape && ees != ECpShape.None && ees != ECpShape.Image && ees != ECpShape.Random);
            n = targetShapeList.Count();// - 2;
            targetNum = opeImageRandom.Next(0, n);
            return targetShapeList.ElementAt(targetNum);
        }

        public ECpShape GetRandomShape(ECpShape[] existShapes, ECpShape[] exclusionShapes)
        {
            // 無効図形3つ
            var n = Enum.GetValues(typeof(ECpShape)).Length - 3;
            var targetNum = opeImageRandom.Next(0, n);
            IEnumerable<ECpShape> enumerableECpShape = Enum.GetValues(typeof(ECpShape)).Cast<ECpShape>();
            // 存在しないSVG形状を除外
            List<ECpShape> nonExistShapeList = CheckUnExistSvgShape();

            if (existShapes.Contains(ECpShape.SvgCircle) || exclusionShapes.Contains(ECpShape.SvgCircle))
            {
                nonExistShapeList.Add(ECpShape.Circle);
                nonExistShapeList.Add(ECpShape.SvgCircle);
            }
            if (existShapes.Contains(ECpShape.SvgSquare) || exclusionShapes.Contains(ECpShape.SvgSquare))
            {
                nonExistShapeList.Add(ECpShape.Rectangle);
                nonExistShapeList.Add(ECpShape.SvgSquare);
            }
            if (existShapes.Contains(ECpShape.SvgStar) || exclusionShapes.Contains(ECpShape.SvgStar))
            {
                nonExistShapeList.Add(ECpShape.Star);
                nonExistShapeList.Add(ECpShape.SvgStar);
            }
            if (existShapes.Contains(ECpShape.SvgTriangle) || exclusionShapes.Contains(ECpShape.SvgTriangle))
            {
                nonExistShapeList.Add(ECpShape.Triangle);
                nonExistShapeList.Add(ECpShape.SvgTriangle);
            }
            // existShapes Default Circleになってる
            if (existShapes.Contains(ECpShape.Circle) || exclusionShapes.Contains(ECpShape.Circle))
            {
                nonExistShapeList.Add(ECpShape.Circle);
                nonExistShapeList.Add(ECpShape.SvgCircle);
            }
            if (existShapes.Contains(ECpShape.Rectangle) || exclusionShapes.Contains(ECpShape.Rectangle))
            {
                nonExistShapeList.Add(ECpShape.SvgSquare);
                nonExistShapeList.Add(ECpShape.Rectangle);
            }
            if (existShapes.Contains(ECpShape.Star) || exclusionShapes.Contains(ECpShape.Star))
            {
                nonExistShapeList.Add(ECpShape.SvgStar);
                nonExistShapeList.Add(ECpShape.Star);
            }
            if (existShapes.Contains(ECpShape.Triangle) || exclusionShapes.Contains(ECpShape.Triangle))
            {
                nonExistShapeList.Add(ECpShape.Triangle);
                nonExistShapeList.Add(ECpShape.SvgTriangle);
            }
            IEnumerable<ECpShape> targetShapeList = enumerableECpShape.Where(ees => !nonExistShapeList.Contains(ees) && !existShapes.Contains(ees) && !exclusionShapes.Contains(ees) && ees != ECpShape.None && ees != ECpShape.Image && ees != ECpShape.Random);
            n = targetShapeList.Count();
            targetNum = opeImageRandom.Next(0, n);
            
            if (n == 0)
                throw new Exception("除外数がECpShape項目を超えました");
            Debug.WriteLine(targetShapeList.ElementAt(targetNum));
            return targetShapeList.ElementAt(targetNum);
        }

        /// <summary>
        /// StringからEcpShapeを返す
        /// Random時、targetShapeとは違うものを返す機能付き
        /// </summary>
        /// <param name="stringTypeOfShapeArg">string</param>
        /// <param name="targetShape">ターゲット形状とは違うものを返す</param>
        /// <param name="CpShapeArgOut">ECpShape</param>
        /// <returns></returns>
        public bool ConvertStringToCpShape(string stringTypeOfShapeArg, ECpShape targetShape, out ECpShape CpShapeArgOut)
        {
            bool boolRet = true;

            if (stringTypeOfShapeArg == ECpShape.Circle.ToString())
            {
                CpShapeArgOut = ECpShape.Circle;
            }
            else if (stringTypeOfShapeArg == ECpShape.Rectangle.ToString())
            {
                CpShapeArgOut = ECpShape.Rectangle;
            }
            else if (stringTypeOfShapeArg == ECpShape.Triangle.ToString())
            {
                CpShapeArgOut = ECpShape.Triangle;
            }
            else if (stringTypeOfShapeArg == ECpShape.Star.ToString())
            {
                CpShapeArgOut = ECpShape.Star;
            }
            else if (stringTypeOfShapeArg == ECpShape.Random.ToString())
            {
                CpShapeArgOut = GetRandomShape(targetShape);
            }
            else if (stringTypeOfShapeArg == ECpShape.SvgCircle.ToString())
            {
                CpShapeArgOut = ECpShape.SvgCircle;
            }
            else if (stringTypeOfShapeArg == ECpShape.SvgRectangle.ToString())
            {
                CpShapeArgOut = ECpShape.SvgRectangle;
            }
            else if (stringTypeOfShapeArg == ECpShape.SvgTriangle.ToString())
            {
                CpShapeArgOut = ECpShape.SvgTriangle;
            }
            else if (stringTypeOfShapeArg == ECpShape.SvgStar.ToString())
            {
                CpShapeArgOut = ECpShape.SvgStar;
            }
            else if (stringTypeOfShapeArg == ECpShape.SvgScalene.ToString())
            {
                CpShapeArgOut = ECpShape.SvgScalene;
            }
            else if (stringTypeOfShapeArg == ECpShape.SvgPentagon.ToString())
            {
                CpShapeArgOut = ECpShape.SvgPentagon;
            }
            else if (stringTypeOfShapeArg == ECpShape.SvgSquare.ToString())
            {
                CpShapeArgOut = ECpShape.SvgSquare;
            }
            else if (stringTypeOfShapeArg == ECpShape.SvgRight.ToString())
            {
                CpShapeArgOut = ECpShape.SvgRight;
            }
            else if (stringTypeOfShapeArg == ECpShape.SvgTrapeze.ToString())
            {
                CpShapeArgOut = ECpShape.SvgTrapeze;
            }
            else if (stringTypeOfShapeArg == ECpShape.SvgKite.ToString())
            {
                CpShapeArgOut = ECpShape.SvgKite;
            }
            else if (stringTypeOfShapeArg == ECpShape.SvgPolygon.ToString())
            {
                CpShapeArgOut = ECpShape.SvgPolygon;
            }
            else if (stringTypeOfShapeArg == ECpShape.SvgParallelogram.ToString())
            {
                CpShapeArgOut = ECpShape.SvgParallelogram;
            }
            else if (stringTypeOfShapeArg == ECpShape.SvgEllipse.ToString())
            {
                CpShapeArgOut = ECpShape.SvgEllipse;
            }
            else if (stringTypeOfShapeArg == ECpShape.SvgTrefoil.ToString())
            {
                CpShapeArgOut = ECpShape.SvgTrefoil;
            }
            else if (stringTypeOfShapeArg == ECpShape.SvgSemiCircle.ToString())
            {
                CpShapeArgOut = ECpShape.SvgSemiCircle;
            }
            else if (stringTypeOfShapeArg == ECpShape.SvgHexagon.ToString())
            {
                CpShapeArgOut = ECpShape.SvgHexagon;
            }
            else if (stringTypeOfShapeArg == ECpShape.SvgCresent.ToString())
            {
                CpShapeArgOut = ECpShape.SvgCresent;
            }
            else if (stringTypeOfShapeArg == ECpShape.SvgOctagon.ToString())
            {
                CpShapeArgOut = ECpShape.SvgOctagon;
            }
            else if (stringTypeOfShapeArg == ECpShape.SvgCross.ToString())
            {
                CpShapeArgOut = ECpShape.SvgCross;
            }
            else if (stringTypeOfShapeArg == ECpShape.SvgRing.ToString())
            {
                CpShapeArgOut = ECpShape.SvgRing;
            }
            else if (stringTypeOfShapeArg == ECpShape.SvgPic.ToString())
            {
                CpShapeArgOut = ECpShape.SvgPic;
            }
            else if (stringTypeOfShapeArg == ECpShape.SvgHeart.ToString())
            {
                CpShapeArgOut = ECpShape.SvgHeart;
            }
            else if (stringTypeOfShapeArg == ECpShape.SvgArrow.ToString())
            {
                CpShapeArgOut = ECpShape.SvgArrow;
            }
            else if (stringTypeOfShapeArg == ECpShape.SvgQuadrefoil.ToString())
            {
                CpShapeArgOut = ECpShape.SvgQuadrefoil;
            }
            else if (stringTypeOfShapeArg == ECpShape.SvgRhombus.ToString())
            {
                CpShapeArgOut = ECpShape.SvgRhombus;
            }
            else
            {
                CpShapeArgOut = ECpShape.Circle;
                return false;
            }
            return boolRet;
        }

        /// <summary>
        /// StringからEcpShapeを返す
        /// Random時、正答とは違うものを返す機能付き
        /// </summary>
        /// <param name="stringTypeOfShapeArg">string</param>
        /// <param name="CpShapeArgOut">ECpShape</param>
        /// <returns>bool</returns>
        public bool ConvertStringToCpShape(string stringTypeOfShapeArg, out ECpShape CpShapeArgOut)
        {
            return ConvertStringToCpShape(stringTypeOfShapeArg, CorrectShape, out CpShapeArgOut);
        }

        /// <summary>
        /// ランダム色取得
        /// </summary>
        /// <param name="exclusionColor">除外色</param>
        /// <param name="bgColor">背景色</param>
        /// <returns>Color</returns>
        public Color GetRandomColor(Color exclusionColor, Color bgColor)
        {
            List<Color> colList = new List<Color> { Color.Black, Color.Blue, Color.Green, Color.Red, Color.White, Color.Yellow };

            // Natural Color
            //Type type = typeof(Color);
            //PropertyInfo[] propertyInfos = type.GetProperties(BindingFlags.Public | BindingFlags.Static);
            //IEnumerable<Color> colors1 = propertyInfos.Select(propertyInfo => Color.FromName(propertyInfo.Name));

            IEnumerable<Color> colors;
            if (exclusionColor != null || exclusionColor == Color.Transparent)
            {
                // target colList
                colors = colList.Where(col => (col != exclusionColor) && (col != bgColor));
            }
            else
            {
                colors = colList.Where(col => col != bgColor);
            }

            // Selected color random
            var targetNum = opeImageRandom.Next(colors.Count());
            return colors.ElementAt<Color>(targetNum);
        }

        /// <summary>
        /// StringからColorを取得 
        /// ランダム時対象色 背景色と被らないようColorを選択
        /// </summary>
        /// <param name="stringCpColorArg">入力string</param>
        /// <param name="targetColor">対象色</param>
        /// <param name="bgColor">背景色</param>
        /// <param name="ColorArgOut">出力Color</param>
        /// <returns>bool</returns>
        public bool ConvertStringToColor(string stringCpColorArg, Color targetColor, Color bgColor, out Color ColorArgOut)
        {
            bool boolRet = true;

            if (stringCpColorArg == ECpColor.Black.ToString())
            {
                ColorArgOut = Color.Black;
            }
            else if (stringCpColorArg == ECpColor.Blue.ToString())
            {
                ColorArgOut = Color.Blue;
            }
            else if (stringCpColorArg == ECpColor.Green.ToString())
            {
                ColorArgOut = Color.Green;
            }
            else if (stringCpColorArg == ECpColor.Red.ToString())
            {
                ColorArgOut = Color.Red;
            }
            else if (stringCpColorArg == ECpColor.White.ToString())
            {
                ColorArgOut = Color.White;
            }
            else if (stringCpColorArg == ECpColor.Yellow.ToString())
            {
                ColorArgOut = Color.Yellow;
            }
            else if (stringCpColorArg == ECpColor.Random.ToString())
            {
                ColorArgOut = Color.Black;

                List<Color> colList = new List<Color> { Color.Black, Color.Blue, Color.Green, Color.Red, Color.White, Color.Yellow };
                Type type = typeof(Color);
                // Natural Color
                PropertyInfo[] propertyInfos = type.GetProperties(BindingFlags.Public | BindingFlags.Static);
                IEnumerable<Color> colors1 = propertyInfos.Select(propertyInfo => Color.FromName(propertyInfo.Name));

                IEnumerable<Color> colors;
                if (targetColor != null)
                {
                    // target colList
                    colors = colors1.Where(col => (col != targetColor) && (col != bgColor));
                }
                else
                {
                    colors = colList;
                }
                // Full color random
                //ColorArgOut = Color.FromArgb(opeImageRandom.Next(256), opeImageRandom.Next(256), opeImageRandom.Next(256));

                // Selected color random
                var targetNum = opeImageRandom.Next(colors.Count());
                ColorArgOut = colors.ElementAt<Color>(targetNum);

                //Debug.WriteLine(ColorArgOut.R.ToString() + ":" + ColorArgOut.G.ToString() + ":" + ColorArgOut.B.ToString());
            }
            else if (Color.FromName(stringCpColorArg).IsNamedColor)
            {
                ColorArgOut = Color.FromName(stringCpColorArg);
            }
            else
            {
                ColorArgOut = Color.Black;
                return false;
            }
            return boolRet;
        }

        /// <summary>
        /// StringからColorを取得
        /// ランダム時除外なし
        /// </summary>
        /// <param name="stringCpColorArg">入力string</param>
        /// <param name="ColorArgOut">出力Color</param>
        /// <returns>bool</returns>
        public bool ConvertStringToColor(string stringCpColorArg, out Color ColorArgOut)
        {
            bool boolRet = true;
            Color retColor = new Color();
            boolRet = ConvertStringToColor(stringCpColorArg, Color.Empty, ColorOpeImageBackColor, out retColor);
            ColorArgOut = retColor;
            return boolRet;
        }

        /// <summary>
        /// ECpColorからColorを取得
        /// </summary>
        /// <param name="eCpColor">入力ECpColor</param>
        /// <param name="targetColor">出力Color</param>
        /// <returns>bool</returns>
        public Color ConvertEcpColorToColor(ECpColor eCpColor, Color targetColor)
        {
            Color retColor = new Color();
            ConvertStringToColor(eCpColor.ToString(), targetColor, ColorOpeImageBackColor, out retColor);
            return retColor;
        }

        /// <summary>
        /// Stringから背景色を取得
        /// </summary>
        /// <param name="stringCpColorArg">入力String</param>
        /// <param name="ColorArgOut">出力Color</param>
        /// <returns>bool</returns>
        public bool ConvertStringToBgColor(string stringCpColorArg, out Color ColorArgOut)
        {
            bool boolRet = true;

            if (stringCpColorArg == ECpColor.Black.ToString())
            {
                ColorArgOut = Color.Black;
            }
            else if (stringCpColorArg == ECpColor.Blue.ToString())
            {
                ColorArgOut = Color.Blue;
            }
            else if (stringCpColorArg == ECpColor.Green.ToString())
            {
                ColorArgOut = Color.Green;
            }
            else if (stringCpColorArg == ECpColor.Red.ToString())
            {
                ColorArgOut = Color.Red;
            }
            else if (stringCpColorArg == ECpColor.White.ToString())
            {
                ColorArgOut = Color.White;
            }
            else if (stringCpColorArg == ECpColor.Yellow.ToString())
            {
                ColorArgOut = Color.Yellow;
            }
            else if (stringCpColorArg == ECpColor.Random.ToString())
            {
                ColorArgOut = Color.Black;

                List<Color> colList = new List<Color> { Color.Black, Color.Blue, Color.Green, Color.Red, Color.White, Color.Yellow };
                IEnumerable<Color> colors = colList.Where(col => col != ColorOpeImageBackColor);
                // Full color random
                //ColorArgOut = Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));

                // Selected color random
                var targetNum = opeImageRandom.Next(colList.Count);
                ColorArgOut = colors.ElementAt<Color>(targetNum);
            }
            else if (Color.FromName(stringCpColorArg).IsNamedColor)
            {
                ColorArgOut = Color.FromName(stringCpColorArg);
            }
            else
            {
                ColorArgOut = Color.Black;
                return false;
            }
            return boolRet;

        }

        ///// <summary>
        ///// StringからECpColorを取得
        ///// </summary>
        ///// <param name="stringCpColorArg">入力String</param>
        ///// <param name="cpColorArgOut">出力ECpColor</param>
        ///// <returns>bool</returns>
        //public bool ConvertStringToCpColor(string stringCpColorArg, out ECpColor cpColorArgOut)
        //{
        //    bool boolRet = true;

        //    if (stringCpColorArg == ECpColor.Black.ToString())
        //    {
        //        cpColorArgOut = ECpColor.Black;
        //    }
        //    else if (stringCpColorArg == ECpColor.Blue.ToString())
        //    {
        //        cpColorArgOut = ECpColor.Blue;
        //    }
        //    else if (stringCpColorArg == ECpColor.Green.ToString())
        //    {
        //        cpColorArgOut = ECpColor.Green;
        //    }
        //    else if (stringCpColorArg == ECpColor.Red.ToString())
        //    {
        //        cpColorArgOut = ECpColor.Red;
        //    }
        //    else if (stringCpColorArg == ECpColor.White.ToString())
        //    {
        //        cpColorArgOut = ECpColor.White;
        //    }
        //    else if (stringCpColorArg == ECpColor.Yellow.ToString())
        //    {
        //        cpColorArgOut = ECpColor.Yellow;
        //    }
        //    else if (stringCpColorArg == ECpColor.Random.ToString())
        //    {
        //        cpColorArgOut = ECpColor.Random;
        //    }
        //    else
        //    {
        //        cpColorArgOut = ECpColor.Black;
        //        return false;
        //    }
        //    return boolRet;
        //}

        /// <summary>
        /// StringからECpTask
        /// </summary>
        /// <param name="stringCpTaskArg">入力String</param>
        /// <param name="cpTaskArgOut">出力ECpTask</param>
        /// <returns>bool</returns>
        public bool ConvertStringToCpTask(string stringCpTaskArg, out ECpTask cpTaskArgOut)
        {
            bool boolRet = true;

            if (stringCpTaskArg == ECpTask.Training.ToString())
            {
                cpTaskArgOut = ECpTask.Training;
            }
            else if (stringCpTaskArg == ECpTask.DelayMatch.ToString())
            {
                cpTaskArgOut = ECpTask.DelayMatch;
            }
            else if (stringCpTaskArg == ECpTask.TrainingEasy.ToString())
            {
                cpTaskArgOut = ECpTask.TrainingEasy;
            }
            else if (stringCpTaskArg == ECpTask.UnConditionalFeeding.ToString())
            {
                cpTaskArgOut = ECpTask.UnConditionalFeeding;
            }
            else
            {
                cpTaskArgOut = ECpTask.Training;
                return false;
            }
            return boolRet;
        }

        /// <summary>
        /// stringからECpCorrectConditionを取得
        /// </summary>
        /// <param name="stringCpCorrectCondion">入力String</param>
        /// <param name="cpCorrectArgOut">出力ECpCorrectCondition</param>
        public void ConvertStringToCpCorrectCondtion(string stringCpCorrectCondion, out ECpCorrectCondition cpCorrectArgOut)
        {
            if (stringCpCorrectCondion == ECpCorrectCondition.Coordinate.ToString())
            {
                cpCorrectArgOut = ECpCorrectCondition.Coordinate;
            }
            else if (stringCpCorrectCondion == ECpCorrectCondition.Color.ToString())
            {
                cpCorrectArgOut = ECpCorrectCondition.Color;
            }
            else if (stringCpCorrectCondion == ECpCorrectCondition.Shape.ToString())
            {
                cpCorrectArgOut = ECpCorrectCondition.Shape;
            }
            else
            {
                cpCorrectArgOut = ECpCorrectCondition.Coordinate;
                throw new Exception("Input EcpCorrectCondition error");
            }
        }

        /// <summary>
        /// Shape色設定
        /// </summary>
        private void SetShapeColor()
        {
            Color colorShapeVal;
            if (ConvertStringToColor(preferencesDatOriginal.ShapeColor, out colorShapeVal) != true)
            {
                // 図形色設定エラーの時、白とする
                colorShapeVal = Color.White;
                throw new Exception(String.Format("Setting error in {0}: Shape color:{1} is over range",
                    MethodBase.GetCurrentMethod().Name,
                    preferencesDatOriginal.ShapeColor));
            }
            CorrectShapeColor = colorShapeVal;
        }

        /// <summary>
        /// 背景色設定
        /// </summary>
        public void SetBgColor()
        {
            Color colorBackVal;
            if (ConvertStringToBgColor(preferencesDatOriginal.BackColor, out colorBackVal) != true)
            {
                // 背景色設定エラーの時、緑とする
                ColorOpeImageBackColor = Color.Green;
                throw new Exception(String.Format("Setting error in {0}: Back color:{1} is over range",
                        MethodBase.GetCurrentMethod().Name,
                        preferencesDatOriginal.BackColor));
            }
            ColorOpeImageBackColor = colorBackVal;
        }

        /// <summary>
        /// 遅延時間背景色設定
        /// </summary>
        public void SetDelayBgColor()
        {
            Color colorBackVal;
            if (ConvertStringToBgColor(preferencesDatOriginal.DelayBackColor, out colorBackVal) != true)
            {
                // Delay背景色設定エラーの時、黒とする
                ColorOpeImageDelayBackColor = Color.Black;
                throw new Exception(String.Format("Setting error in {0}: Back color:{1} is over range",
                        MethodBase.GetCurrentMethod().Name,
                        preferencesDatOriginal.DelayBackColor));
            }
            ColorOpeImageDelayBackColor = colorBackVal;
        }

        /// <summary>
        /// OpeImage初期化
        /// 描画ターゲットは単一用
        /// </summary>
        /// <param name="stringTypeOfShapeArg"></param>
        /// <param name="stringStepArg"></param>
        /// <param name="iSizeOfShapeInPixelArg"></param>
        /// <returns></returns>
        public bool SetParamOfShapeOpeImage(string stringTypeOfShapeArg, string stringStepArg, int iSizeOfShapeInPixelArg)
        {
            bool boolRet = true;
            bool boolIsFunfOk;
            int iSizeOfShapeInPixel;
            int iXOfValidAreaToDrawShape;
            int iYOfValidAreaToDrawShape;
            int iWidthOfValidAreaToDrawShape;
            int iHeightOfValidAreaToDrawShape;
            ECpStep cpStepVal;
            ECpShape cpShapeVal;
            Color colorShapeVal;
            int Distance = preferencesDatOriginal.MinDistanceBetweenCorrectAndWrongShape;

            // 既存設定ファイルから大きい値が設定されていた場合小さくする
            if (preferencesDatOriginal.MinDistanceBetweenCorrectAndWrongShape > 20)
            {
                preferencesDatOriginal.MinDistanceBetweenCorrectAndWrongShape = 5;
            }

            if (Distance <= 0)
            {
                Distance = 0;
            }

            try
            {
                if (ConvertStringToCpStep(stringStepArg, out cpStepVal) != true)
                {
                    // Step設定エラーの時、Step1とする
                    cpStepVal = ECpStep.Step1;
                    throw new Exception(string.Format("Setting error in {0}: Step:{1} is over range",
                        MethodBase.GetCurrentMethod().Name,
                        stringStepArg));
                }
                if (ConvertStringToCpShape(stringTypeOfShapeArg, out cpShapeVal) != true)
                {
                    // 図形タイプ設定エラーの時、Circleとする
                    cpShapeVal = ECpShape.Circle;
                    throw new Exception(string.Format("Setting error in {0}: Type of shape:{1} is over range",
                        MethodBase.GetCurrentMethod().Name,
                        stringTypeOfShapeArg));
                }
                CorrectShape = cpShapeVal;
                correctShape = cpShapeVal;

                //複数化でこの辺が無意味
                if (preferencesDatOriginal.IncorrectShapeRandom)
                {
                    if (ConvertStringToCpShape(ECpShape.Random.ToString(), out cpShapeVal) != true)
                    {
                        // 図形タイプ設定エラーの時、Randomとする
                        cpShapeVal = ECpShape.Random;
                        throw new Exception(string.Format("Setting error in {0}: Type of shape:{1} is over range",
                            MethodBase.GetCurrentMethod().Name,
                            stringTypeOfShapeArg));
                    }
                }
                else
                {
                    if (ConvertStringToCpShape(stringTypeOfShapeArg, out cpShapeVal) != true)
                    {
                        // 図形タイプ設定エラーの時、Randomとする
                        cpShapeVal = ECpShape.Random;
                        throw new Exception(string.Format("Setting error in {0}: Type of shape:{1} is over range",
                            MethodBase.GetCurrentMethod().Name,
                            stringTypeOfShapeArg));
                    }
                }
                wrongShape = cpShapeVal;
                WrongShape = cpShapeVal;

                if (ConvertStringToColor(preferencesDatOriginal.ShapeColor, out colorShapeVal) != true)
                {
                    // 図形色設定エラーの時、白とする
                    colorShapeVal = Color.White;
                    throw new Exception(string.Format("Setting error in {0}: Shape color:{1} is over range",
                        MethodBase.GetCurrentMethod().Name,
                        preferencesDatOriginal.ShapeColor));
                }
                CorrectShapeColor = colorShapeVal;

                // 図形サイズ(ピクセル数)を決定
                boolIsFunfOk = DecideSizeOfShapeInPixelOpeImage(
                                    cpStepVal, iSizeOfShapeInPixelArg,
                                    out iSizeOfShapeInPixel);
                if (boolIsFunfOk != true)
                {
                    throw new Exception(String.Format("Setting error in {0}: Step:{1} Size of shape in pixel:{2}",
                        MethodBase.GetCurrentMethod().Name,
                        cpStepVal.ToString(), iSizeOfShapeInPixelArg));
                }
                // 図形中心描画可能領域を算出
                iXOfValidAreaToDrawShape = RectOpeImageValidArea.X + (iSizeOfShapeInPixel / 2);
                iYOfValidAreaToDrawShape = RectOpeImageValidArea.Y + (iSizeOfShapeInPixel / 2);
                iWidthOfValidAreaToDrawShape = RectOpeImageValidArea.Width - iSizeOfShapeInPixel;
                iHeightOfValidAreaToDrawShape = RectOpeImageValidArea.Height - iSizeOfShapeInPixel;
                if ((iXOfValidAreaToDrawShape >= (RectOpeImageValidArea.X + RectOpeImageValidArea.Width)) ||
                    (iYOfValidAreaToDrawShape >= RectOpeImageValidArea.Y + RectOpeImageValidArea.Height) ||
                    ((iWidthOfValidAreaToDrawShape < (iSizeOfShapeInPixel + Distance)) &&
                     (iHeightOfValidAreaToDrawShape < (iSizeOfShapeInPixel + Distance))))
                {
                    // 範囲外の時
                    throw new Exception(String.Format("Setting error in {0}: Step:{1} ValidAreaToDrawShape is over range "
                                                    + "(ValidArea[X:{2} Y:{3} W:{4}, H:[5]] SizeOfShapeInPixel:{6} Distance]{7})",
                        MethodBase.GetCurrentMethod().Name,
                        cpStepVal.ToString(),
                        RectOpeImageValidArea.X, RectOpeImageValidArea.Y,
                        RectOpeImageValidArea.Width, RectOpeImageValidArea.Height,
                        iSizeOfShapeInPixel,
                        preferencesDatOriginal.MinDistanceBetweenCorrectAndWrongShape));
                }
                RectOpeImageValidAreaToDrawShape = new Rectangle
                {
                    X = iXOfValidAreaToDrawShape,
                    Y = iYOfValidAreaToDrawShape,
                    Width = iWidthOfValidAreaToDrawShape,
                    Height = iHeightOfValidAreaToDrawShape
                };
                // 図形サイズ(ピクセル数)を保存
                OpeImageSizeOfShapeInPixel = iSizeOfShapeInPixel;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
            {
                Debug.WriteLine("SetParamOfShapeOpeImage().Start:----------");
                // 図形描画有効領域を出力
                Debug.WriteLine("  WholeArea:[X:{0} Y:{1} Width:{2} Height:{3}]",
                                    RectOpeImageValidAreaToDrawShape.X,
                                    RectOpeImageValidAreaToDrawShape.Y,
                                    RectOpeImageValidAreaToDrawShape.Width,
                                    RectOpeImageValidAreaToDrawShape.Height);
                Debug.WriteLine("SetParamOfShapeOpeImage().End:----------");
            }
            return boolRet;
        }

        /// <summary>
        /// 図形サイズのピクセル数を決定する
        /// </summary>
        /// <param name="cpStepVal"></param>
        /// <param name="iSizeOfShapeInPixelVal"></param>
        /// <param name="iDesidedSizeOfShapeInPixel"></param>
        /// <returns></returns>
        public bool DecideSizeOfShapeInPixelOpeImage(ECpStep cpStepArg, int iSizeOfShapeInPixelArg, out int iDesidedSizeOfShapeInPixelArgOut)
        {
            bool boolRet = true;
            int iSizeOfShapeInPixelTemp;

            iDesidedSizeOfShapeInPixelArgOut = 1;
            //			try
            //			{
            if (iSizeOfShapeInPixelArg <= 0)
            {
                // Stepでの指定の時
                switch (cpStepArg)
                {
                    case ECpStep.Step1:
                        iSizeOfShapeInPixelTemp = preferencesDatOriginal.SizeOfShapeInPixelForStep1;
                        break;
                    case ECpStep.Step2:
                        iSizeOfShapeInPixelTemp = preferencesDatOriginal.SizeOfShapeInPixelForStep2;
                        break;
                    case ECpStep.Step3:
                        iSizeOfShapeInPixelTemp = preferencesDatOriginal.SizeOfShapeInPixelForStep3;
                        break;
                    case ECpStep.Step4:
                        iSizeOfShapeInPixelTemp = preferencesDatOriginal.SizeOfShapeInPixelForStep4;
                        break;
                    case ECpStep.Step5:
                        iSizeOfShapeInPixelTemp = preferencesDatOriginal.SizeOfShapeInPixelForStep5;
                        break;
                    // Step指定異常値の場合、Step1を使用する
                    default:
                        iSizeOfShapeInPixelTemp = preferencesDatOriginal.SizeOfShapeInPixelForStep1;
                        //							throw new Exception(String.Format("Setting error in {0}: Step of shape[{1}] is over range",
                        //								MethodBase.GetCurrentMethod().Name,
                        //								cpStepArg.ToString()));
                        break;
                }
                if (iSizeOfShapeInPixelTemp <= 0)
                {
                    iSizeOfShapeInPixelTemp = 1;
                    //						throw new Exception(String.Format("Setting error in {0}: Size of shape in pixel[{1}] for step[{2}] is over range",
                    //							MethodBase.GetCurrentMethod().Name,
                    //							iSizeOfShapeInPixelTemp,
                    //							cpStepArg.ToString()));
                }
            }
            else
            {
                // Pixelでの指定の時
                iSizeOfShapeInPixelTemp = iSizeOfShapeInPixelArg;
                if (iSizeOfShapeInPixelTemp <= 0)
                {
                    iSizeOfShapeInPixelTemp = 1;
                    //						throw new Exception(String.Format("Setting error in {0}: Size of shape in pixel[{1}] is over range",
                    //							MethodBase.GetCurrentMethod().Name,
                    //							iSizeOfShapeInPixelTemp));
                }
            }

            //			}
            //			catch (Exception ex)
            //			{
            //				Debug.WriteLine(ex.Message);
            //				return false;
            //			}
            iDesidedSizeOfShapeInPixelArgOut = iSizeOfShapeInPixelTemp;
            return boolRet;
        }
        /// <summary>
        /// 点間の距離が指定距離内か検査する
        /// </summary>
        /// <param name="pointSrc"></param>
        /// <param name="pointDst"></param>
        /// <param name="distanceToCheck"></param>
        /// <returns></returns>
        public bool IsDistanceInRangeOpeImage(
            Point pointSrcArg,
            Point pointDstArg,
            int distanceToCheckArg)
        {
            bool boolRet;
            int distanceBetweenSrcAndDst;

            distanceBetweenSrcAndDst = (int)Math.Sqrt(
                Math.Pow((double)(pointSrcArg.X - pointDstArg.X), 2) +
                Math.Pow((double)(pointSrcArg.Y - pointDstArg.Y), 2));
            if (distanceBetweenSrcAndDst <= distanceToCheckArg)
            {
                // 指定距離内の時
                boolRet = true;
            }
            else
            {
                // 指定距離外の時
                boolRet = false;
            }
            return boolRet;
        }
        /// <summary>
        /// 角度をラジアンへ変換する
        /// </summary>
        /// <param name="angleValue"></param>
        /// <returns></returns>
        public double ConvertFromAngleToRadianOpeImage(double angleValueArg)
        {
            return (angleValueArg * Math.PI) / 180.0;
        }

        /// <summary>
        /// 正解図形表示座標を生成
        /// 単一条件用
        /// </summary>
        /// <remarks>
        /// 有効領域のWidth、Height＜(図形サイズ/2)の時、ノーケア
        /// </remarks>
        public void MakePointOfCorrectShapeOpeImage()
        {
            Point pointShape = new Point();

            // 不正解ランダム時は正答を先にジェネレート
            // 固定の場合は固定値を参照

            if (preferencesDatOriginal.IncorrectCoordinateRandom)
            {
                pointShape.X = opeImageRandom.Next(RectOpeImageValidAreaToDrawShape.X,
                                                    RectOpeImageValidAreaToDrawShape.X + RectOpeImageValidAreaToDrawShape.Width);
                pointShape.Y = opeImageRandom.Next(RectOpeImageValidAreaToDrawShape.Y,
                                                    RectOpeImageValidAreaToDrawShape.Y + RectOpeImageValidAreaToDrawShape.Height);

            }
            else
            {
                Point[] points = { new Point(preferencesDatOriginal.IncorrectCoordinateFixX1,preferencesDatOriginal.IncorrectCoordinateFixY1),
                new Point(preferencesDatOriginal.IncorrectCoordinateFixX2,preferencesDatOriginal.IncorrectCoordinateFixY2),
                new Point(preferencesDatOriginal.IncorrectCoordinateFixX3,preferencesDatOriginal.IncorrectCoordinateFixY3),
                new Point(preferencesDatOriginal.IncorrectCoordinateFixX4,preferencesDatOriginal.IncorrectCoordinateFixY4) };

                var minDistance = OpeImageSizeOfShapeInPixel + preferencesDatOriginal.MinDistanceBetweenCorrectAndWrongShape;

                GeneratePoint(points, out pointShape, minDistance);
            }
            PointOpeImageCenterOfCorrectShape = pointShape;
            if (ShapeOpeImageCorrectShape != null)
                ShapeOpeImageCorrectShape.Point = pointShape;

            return;
        }

        /// <summary>
        /// 不正解図形リストリセット
        /// </summary>
        public void ResetWrongShapePoint()
        {
            _PointOpeImageCenterOfWrongShapeList?.Clear();
            _ShapeOpeImageWrongList?.Clear();
            _ColorOpeImageWrongList?.Clear();
        }

        /// <summary>
        /// 不正解ポイントを作成
        /// 廃止
        /// </summary>
        /// <returns>bool</returns>
        public bool MakePointOfWrongShapeOpeImage()
        {
            bool boolRet = true;
            Point searchPoint = new Point();
            int iMinDistance;
            long lCurrentTime;

            bool pointListMatch = false;

            if (_PointOpeImageCenterOfWrongShapeList != null)
            {
                //PointOpeImageCenterOfWrongShapeList.Where<Point>(item=>item.X>0).Select(x =>x /* 何かの処理 */).ToList();

                // ResetWronShapePoint() でリセット
                //_PointOpeImageCenterOfWrongShapeList.Clear();
            }
            else
            {
                _PointOpeImageCenterOfWrongShapeList = new List<Point>();
                _ShapeOpeImageWrongList = new List<ECpShape>();
                _ColorOpeImageWrongList = new List<Color>();
            }

            // 計測開始
            mStopwatchOpeImage.Reset();
            mStopwatchOpeImage.Start();
            try
            {
                iMinDistance = OpeImageSizeOfShapeInPixel + preferencesDatOriginal.MinDistanceBetweenCorrectAndWrongShape;
                if (preferencesDatOriginal.IncorrectCoordinateRandom)
                {
                    while (true)
                    {
                        pointListMatch = false;

                        searchPoint.X = opeImageRandom.Next(RectOpeImageValidAreaToDrawShape.X,
                                                            RectOpeImageValidAreaToDrawShape.X + RectOpeImageValidAreaToDrawShape.Width);
                        searchPoint.Y = opeImageRandom.Next(RectOpeImageValidAreaToDrawShape.Y,
                                                            RectOpeImageValidAreaToDrawShape.Y + RectOpeImageValidAreaToDrawShape.Height);
                        // 正解図形と不正解図形の距離>最小距離の時、終了

                        // CorrectとTrainにする
                        if (IsDistanceInRangeOpeImage(ShapeOpeImageCorrectShape.Point, searchPoint, iMinDistance) == false)
                        {
                            // 不正解要素同士の距離比較
                            //_PointOpeImageCenterOfWrongShapeList.Where<Point>(item => item.X > 0).Select(x => IsDistanceInRangeOpeImage(x, pointShape, iMinDistance));
                            if (_PointOpeImageCenterOfWrongShapeList?.Count > 0)
                            {
                                foreach (Point p in _PointOpeImageCenterOfWrongShapeList)
                                {
                                    Debug.WriteLine(searchPoint.ToString() + " : " + p.ToString());
                                    if (IsDistanceInRangeOpeImage(p, searchPoint, iMinDistance))
                                    {
                                        pointListMatch = true;
                                    }
                                }
                            }
                            if (!pointListMatch)
                                break;
                        }
                        lCurrentTime = mStopwatchOpeImage.ElapsedMilliseconds;
                        if (lCurrentTime >= (long)OpeImageDefault.TimeoutOfTryToMakeWroingShapePoint)
                        {
                            throw new Exception(String.Format("Error in {0}: Timeout to make pointa of wrong shape",
                                MethodBase.GetCurrentMethod().Name));
                        }
                    }
                }
                else
                {
                    var pointCount = _PointOpeImageCenterOfWrongShapeList?.Count();
                    Point makePoint;
                    if (pointCount == 0)
                    {
                        makePoint = new Point(preferencesDatOriginal.IncorrectCoordinateFixX1, preferencesDatOriginal.IncorrectCoordinateFixY1);
                    }
                    else if (pointCount == 1)
                    {
                        makePoint = new Point(preferencesDatOriginal.IncorrectCoordinateFixX2, preferencesDatOriginal.IncorrectCoordinateFixY2);
                    }
                    else if (pointCount == 2)
                    {
                        makePoint = new Point(preferencesDatOriginal.IncorrectCoordinateFixX3, preferencesDatOriginal.IncorrectCoordinateFixY3);
                    }
                    else if (pointCount == 3)
                    {
                        makePoint = new Point(preferencesDatOriginal.IncorrectCoordinateFixX4, preferencesDatOriginal.IncorrectCoordinateFixY4);
                    }
                    else
                    {
                        // とりあえずセンターに描画
                        makePoint = new Point(RectOpeImageValidAreaToDrawShape.Width / 2, RectOpeImageValidAreaToDrawShape.Height / 2);
                    }

                    searchPoint = makePoint;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
            finally
            {
                // 計測停止
                mStopwatchOpeImage.Stop();
            }
            // 不正解図形のポイントを保存

            _PointOpeImageCenterOfWrongShapeList?.Add(searchPoint);

            // 他の情報も作成
            ECpShape ecpShape;
            if (preferencesDatOriginal.IncorrectShapeRandom)
            {
                ConvertStringToCpShape(ECpShape.Random.ToString(), out ecpShape);
                _ShapeOpeImageWrongList?.Add(ecpShape);
            }
            else
            {
                /// リスト内個数で固定値判断
                var shapeCount = _ShapeOpeImageWrongList?.Count();
                if (shapeCount == 0)
                {
                    ConvertStringToCpShape(preferencesDatOriginal.IncorrectShapeFix1.ToString(), out ecpShape);
                }
                else if (shapeCount == 1)
                {
                    ConvertStringToCpShape(preferencesDatOriginal.IncorrectShapeFix2.ToString(), out ecpShape);
                }
                else if (shapeCount == 2)
                {
                    ConvertStringToCpShape(preferencesDatOriginal.IncorrectShapeFix3.ToString(), out ecpShape);
                }
                else if (shapeCount == 3)
                {
                    ConvertStringToCpShape(preferencesDatOriginal.IncorrectShapeFix4.ToString(), out ecpShape);
                }
                else
                {
                    ecpShape = new ECpShape();
                }
                _ShapeOpeImageWrongList?.Add(ecpShape);
            }

            if (preferencesDatOriginal.IncorrectColorRandom)
            {
                _ColorOpeImageWrongList?.Add(ConvertEcpColorToColor(ECpColor.Random, CorrectShapeColor));
            }
            else
            {
                Color color;
                var colorCount = _ColorOpeImageWrongList?.Count();
                if (colorCount == 0)
                {
                    ConvertStringToColor(preferencesDatOriginal.IncorrectColorFix1.ToString(), out color);
                }
                else if (colorCount == 1)
                {
                    ConvertStringToColor(preferencesDatOriginal.IncorrectColorFix2.ToString(), out color);
                }
                else if (colorCount == 2)
                {
                    ConvertStringToColor(preferencesDatOriginal.IncorrectColorFix3.ToString(), out color);
                }
                else if (colorCount == 3)
                {
                    ConvertStringToColor(preferencesDatOriginal.IncorrectColorFix4.ToString(), out color);
                }
                else
                {
                    color = new Color();
                }
                _ColorOpeImageWrongList?.Add(color);
            }
            return boolRet;
        }

        /// <summary>
        /// 不正解図形作成
        /// </summary>
        /// <returns></returns>
        public bool MakeIncorrectShape()
        {
            Point[] fixedIncorrectPoints ={ new Point(preferencesDatOriginal.IncorrectCoordinateFixX1,preferencesDatOriginal.IncorrectCoordinateFixY1),
                new Point(preferencesDatOriginal.IncorrectCoordinateFixX2,preferencesDatOriginal.IncorrectCoordinateFixY2),
                new Point(preferencesDatOriginal.IncorrectCoordinateFixX3,preferencesDatOriginal.IncorrectCoordinateFixY3),
                new Point(preferencesDatOriginal.IncorrectCoordinateFixX4,preferencesDatOriginal.IncorrectCoordinateFixY4) };

            var minDistance = OpeImageSizeOfShapeInPixel + preferencesDatOriginal.MinDistanceBetweenCorrectAndWrongShape;

            IncorrectShapes = new ShapeObject[preferencesDatOriginal.IncorrectNum];
            Point targetPoint;

            // CorrectのみTrainを含まない
            Point[] allPoints = new Point[preferencesDatOriginal.IncorrectNum + 1];

            if (ShapeOpeImageCorrectShape is null)
            {
                ShapeOpeImageCorrectShape = new ShapeObject();
            }
            allPoints[0] = ShapeOpeImageCorrectShape.Point;
            // Trainを使わないように変更
            //allPoints[1] = ShapeOpeImageTrainShape.Point;
            if (!preferencesDatOriginal.IncorrectCoordinateRandom)
            {
                allPoints[1] = fixedIncorrectPoints[0];
                if (IncorrectShapes.Length >= 2)
                {
                    allPoints[2] = fixedIncorrectPoints[1];
                }
                if (IncorrectShapes.Length >= 3)
                {
                    allPoints[3] = fixedIncorrectPoints[2];
                }
                if (IncorrectShapes.Length >= 4)
                {
                    allPoints[4] = fixedIncorrectPoints[3];
                }
            }

            // Point
            // 探索時指定時間オーバーしたら 繰り返し検索済み座標を破棄して再探索
            try
            {

                for (int i = 0; i < IncorrectShapes.Length; i++)
                {
                    IncorrectShapes[i] = new ShapeObject();
                    IncorrectShapes[i].Shape = ECpShape.None;
                    if (!preferencesDatOriginal.IncorrectCoordinateRandom)
                    {
                        IncorrectShapes[i].Point = fixedIncorrectPoints[i];
                    }
                    else
                    {
                        _ = GeneratePoint(allPoints, out targetPoint, minDistance);
                        IncorrectShapes[i].Point = targetPoint;
                        allPoints[i + 1] = targetPoint;
                    }
                }

            }
            catch (Exception)
            {
                throw;
            }
            MakePathOfIncorrectShapeOpeImage(allPoints);
            // Shape

            var correctShapes = new ECpShape[1] { ShapeOpeImageCorrectShape.Shape };

            if (preferencesDatOriginal.IncorrectShapeRandom)
            {
                ECpShape[] existShapes;
                foreach (ShapeObject n in IncorrectShapes)
                {
                    // exitShapes毎回更新 コスト高
                    existShapes = IncorrectShapes.Select(x => x.Shape).ToArray();
                    n.Shape = GetRandomShape(existShapes, correctShapes);
                }
            }
            else
            {
                ECpShape eCpShape;

                _ = ConvertStringToCpShape(preferencesDatOriginal.IncorrectShapeFix1, out eCpShape);
                IncorrectShapes[0].Shape = eCpShape;

                if (IncorrectShapes.Length >= 2)

                {
                    _ = ConvertStringToCpShape(preferencesDatOriginal.IncorrectShapeFix2, out eCpShape);
                    IncorrectShapes[1].Shape = eCpShape;
                }

                if (IncorrectShapes.Length >= 3)

                {
                    _ = ConvertStringToCpShape(preferencesDatOriginal.IncorrectShapeFix3, out eCpShape);
                    IncorrectShapes[2].Shape = eCpShape;

                }
                if (IncorrectShapes.Length >= 4)

                {

                    _ = ConvertStringToCpShape(preferencesDatOriginal.IncorrectShapeFix4, out eCpShape);
                    IncorrectShapes[3].Shape = eCpShape;
                }
            }

            // Color
            if (preferencesDatOriginal.IncorrectColorRandom)
            {
                foreach (ShapeObject n in IncorrectShapes)
                {
                    n.ShapeColor = GetRandomColor(ShapeOpeImageCorrectShape.ShapeColor, ColorOpeImageBackColor);
                }
            }
            else
            {
                Color color;
                _ = ConvertStringToColor(preferencesDatOriginal.IncorrectColorFix1, out color);
                IncorrectShapes[0].ShapeColor = color;

                if (IncorrectShapes.Length >= 2)
                {
                    _ = ConvertStringToColor(preferencesDatOriginal.IncorrectColorFix2, out color);
                    IncorrectShapes[1].ShapeColor = color;
                }
                if (IncorrectShapes.Length >= 3)
                {
                    _ = ConvertStringToColor(preferencesDatOriginal.IncorrectColorFix3, out color);
                    IncorrectShapes[2].ShapeColor = color;
                }
                if (IncorrectShapes.Length >= 4)

                {
                    _ = ConvertStringToColor(preferencesDatOriginal.IncorrectColorFix4, out color);
                    IncorrectShapes[3].ShapeColor = color;
                }

            }

            if (preferencesDatOriginal.EnableImageShape)
            {

                List<string> fileList = GetIncorrectImageObject(preferencesDatOriginal.ImageFileFolder, preferencesDatOriginal.IncorrectNum);
                for (int i = 0; i < IncorrectShapes.Length; i++)
                {
                    IncorrectShapes[i].Shape = ECpShape.Image;
                    IncorrectShapes[i].ImageFilename = fileList[i];
                }
            }
            return false;
        }

        public void MakeEpisodeShape()
        {
            Point[] fixedEpisodePoints ={ new Point(preferencesDatOriginal.EpTarget1FixX,preferencesDatOriginal.EpTarget1FixY),
                new Point(preferencesDatOriginal.EpTarget2FixX,preferencesDatOriginal.EpTarget2FixY),
                new Point(preferencesDatOriginal.EpTarget3FixX,preferencesDatOriginal.EpTarget3FixY),
                new Point(preferencesDatOriginal.EpTarget4FixX,preferencesDatOriginal.EpTarget4FixY),
                new Point(preferencesDatOriginal.EpTarget5FixX,preferencesDatOriginal.EpTarget5FixY)};

            var minDistance = OpeImageSizeOfShapeInPixel + preferencesDatOriginal.MinDistanceBetweenCorrectAndWrongShape;

            EpisodeShapes = new ShapeObject[preferencesDatOriginal.EpisodeTargetNum];
            for (int i = 0; i < EpisodeShapes.Length; i++)
            {
                EpisodeShapes[i] = new ShapeObject();
            }

            Point targetPoint;

            Point[] allPoints = new Point[preferencesDatOriginal.EpisodeTargetNum];
            if (ShapeOpeImageCorrectShape is null)
            {
                ShapeOpeImageCorrectShape = new ShapeObject();
            }

            if (!preferencesDatOriginal.EpisodeRandomCoordinate)
            {
                for (int i = 0; i < EpisodeShapes.Length; i++)
                {
                    EpisodeShapes[i].Point = fixedEpisodePoints[i];
                    allPoints[i] = fixedEpisodePoints[i];
                }
            }

            // Point
            // 探索時指定時間オーバーしたら 繰り返し検索済み座標を破棄して再探索
            try
            {

                for (int i = 0; i < EpisodeShapes.Length; i++)
                {
                    EpisodeShapes[i] = new ShapeObject();
                    EpisodeShapes[i].Shape = ECpShape.None;
                    if (!preferencesDatOriginal.EpisodeRandomCoordinate)
                    {
                        EpisodeShapes[i].Point = fixedEpisodePoints[i];
                    }
                    else
                    {
                        _ = GeneratePoint(allPoints, out targetPoint, minDistance);
                        EpisodeShapes[i].Point = targetPoint;
                        allPoints[i] = targetPoint;
                    }
                }

            }
            catch (Exception)
            {
                //MessageBox.Show(ex.Message);
                throw;
            }
            MakePathOfIncorrectShapeOpeImage(allPoints);
            // Shape

            var correctShapes = new ECpShape[1] { ShapeOpeImageCorrectShape.Shape };

            if (preferencesDatOriginal.EpisodeRandomShape)
            {
                ECpShape[] existShapes;
                foreach (ShapeObject n in EpisodeShapes)
                {
                    // exitShapes毎回更新 コスト高
                    existShapes = EpisodeShapes.Select(x => x.Shape).ToArray();
                    n.Shape = GetRandomShape(existShapes, correctShapes);
                }
            }
            else
            {
                ECpShape eCpShape;


                _ = ConvertStringToCpShape(preferencesDatOriginal.EpShapeFix1, out eCpShape);
                EpisodeShapes[0].Shape = eCpShape;

                if (EpisodeShapes.Length >= 2)
                {
                    _ = ConvertStringToCpShape(preferencesDatOriginal.EpShapeFix2, out eCpShape);
                    EpisodeShapes[1].Shape = eCpShape;
                }

                if (EpisodeShapes.Length >= 3)
                {
                    _ = ConvertStringToCpShape(preferencesDatOriginal.EpShapeFix3, out eCpShape);
                    EpisodeShapes[2].Shape = eCpShape;

                }

                if (EpisodeShapes.Length >= 4)
                {

                    _ = ConvertStringToCpShape(preferencesDatOriginal.EpShapeFix4, out eCpShape);
                    EpisodeShapes[3].Shape = eCpShape;
                }

                if (EpisodeShapes.Length >= 5)
                {

                    _ = ConvertStringToCpShape(preferencesDatOriginal.EpShapeFix5, out eCpShape);
                    EpisodeShapes[4].Shape = eCpShape;
                }
            }

            // Color
            if (preferencesDatOriginal.EpisodeRandomColor)
            {
                foreach (ShapeObject n in EpisodeShapes)
                {
                    n.ShapeColor = GetRandomColor(Color.Transparent, ColorOpeImageBackColor);
                }
            }
            else
            {
                _ = ConvertStringToColor(preferencesDatOriginal.EpColorFix1, out Color color);
                EpisodeShapes[0].ShapeColor = color;

                if (EpisodeShapes.Length >= 2)
                {
                    _ = ConvertStringToColor(preferencesDatOriginal.EpColorFix2, out color);
                    EpisodeShapes[1].ShapeColor = color;
                }

                if (EpisodeShapes.Length >= 3)
                {
                    _ = ConvertStringToColor(preferencesDatOriginal.EpColorFix3, out color);
                    EpisodeShapes[2].ShapeColor = color;
                }

                if (EpisodeShapes.Length >= 4)
                {
                    _ = ConvertStringToColor(preferencesDatOriginal.EpColorFix4, out color);
                    EpisodeShapes[3].ShapeColor = color;
                }

                if (EpisodeShapes.Length >= 5)
                {
                    _ = ConvertStringToColor(preferencesDatOriginal.EpColorFix5, out color);
                    EpisodeShapes[4].ShapeColor = color;
                }
            }

            if (preferencesDatOriginal.EnableEpImageShape)
            {

                List<string> fileList = GetIncorrectImageObject(preferencesDatOriginal.EpImageFileFolder, preferencesDatOriginal.EpisodeTargetNum);
                if(fileList.Count< preferencesDatOriginal.EpisodeTargetNum)
                {
                    throw new Exception("イメージファイルがTargetNumより少ないです");
                }
                for (int i = 0; i < EpisodeShapes.Length; i++)
                {
                    EpisodeShapes[i].Shape = ECpShape.Image;
                    EpisodeShapes[i].ImageFilename = fileList[i];
                }
            }
        }

        private void CheckFixEpisodePoint(Point correctPoint, Point targetPoint)
        {
            var minDistance = OpeImageSizeOfShapeInPixel + preferencesDatOriginal.MinDistanceBetweenCorrectAndWrongShape;
            _ = IsDistanceInRangeOpeImage(correctPoint, targetPoint, minDistance);

        }
        public void MakeEpisodeShape(ShapeObject so)
        {
            Point[] fixedEpisodePoints ={ new Point(preferencesDatOriginal.EpTarget1FixX,preferencesDatOriginal.EpTarget1FixY),
                new Point(preferencesDatOriginal.EpTarget2FixX,preferencesDatOriginal.EpTarget2FixY),
                new Point(preferencesDatOriginal.EpTarget3FixX,preferencesDatOriginal.EpTarget3FixY),
                new Point(preferencesDatOriginal.EpTarget4FixX,preferencesDatOriginal.EpTarget4FixY),
                new Point(preferencesDatOriginal.EpTarget5FixX,preferencesDatOriginal.EpTarget5FixY)};

            var correctPoint = fixedEpisodePoints.Where(x => x == so.Point).ToList();
            var ca = fixedEpisodePoints.Where(x => x != so.Point).ToList();

            Point[] fixedEpisodePointsSorted = new Point[ca.Count + 1];
            fixedEpisodePointsSorted[0] = correctPoint.First();
            for (int i = 1; i < fixedEpisodePointsSorted.Length; i++)
            {
                fixedEpisodePointsSorted[i] = ca[i - 1];
            }

            var minDistance = OpeImageSizeOfShapeInPixel + preferencesDatOriginal.MinDistanceBetweenCorrectAndWrongShape;

            EpisodeShapes = new ShapeObject[preferencesDatOriginal.EpisodeTargetNum];
            for (int i = 0; i < EpisodeShapes.Length; i++)
            {
                EpisodeShapes[i] = new ShapeObject();
            }
            EpisodeShapes[0] = so;

            Point targetPoint;
            Point[] allPoints = new Point[preferencesDatOriginal.EpisodeTargetNum];

            if (ShapeOpeImageCorrectShape is null)
            {
                ShapeOpeImageCorrectShape = new ShapeObject();
            }

            if (!preferencesDatOriginal.EpisodeRandomCoordinate)
            {
                for (int i = 0; i < EpisodeShapes.Length; i++)
                {
                    EpisodeShapes[i].Point = fixedEpisodePointsSorted[i];
                    allPoints[i] = fixedEpisodePointsSorted[i];
                }
            }

            // Point
            // 探索時指定時間オーバーしたら 繰り返し検索済み座標を破棄して再探索
            try
            {

                for (int i = 1; i < EpisodeShapes.Length; i++)
                {
                    EpisodeShapes[i] = new ShapeObject();
                    EpisodeShapes[i].Shape = ECpShape.None;
                    if (!preferencesDatOriginal.EpisodeRandomCoordinate)
                    {
                        EpisodeShapes[i].Point = fixedEpisodePointsSorted[i];
                    }
                    else
                    {
                        _ = GeneratePoint(allPoints, out targetPoint, minDistance);
                        EpisodeShapes[i].Point = targetPoint;
                        allPoints[i] = targetPoint;
                    }
                }

            }
            catch (Exception)
            {
                //MessageBox.Show(ex.Message);
                throw;
            }
            MakePathOfIncorrectShapeOpeImage(allPoints);
            // Shape

            var correctShapes = new ECpShape[1] { ShapeOpeImageCorrectShape.Shape };

            if (preferencesDatOriginal.EpisodeRandomShape)
            {
                ECpShape[] existShapes;
                for (int i = 1; i < EpisodeShapes.Length; i++)
                {
                    // exitShapes毎回更新 コスト高
                    existShapes = EpisodeShapes.Select(x => x.Shape).ToArray();
                    EpisodeShapes[i].Shape = GetRandomShape(existShapes, correctShapes);
                }
            }
            else
            {
                ECpShape eCpShape;
                _ = ConvertStringToCpShape(preferencesDatOriginal.EpShapeFix1, out eCpShape);
                EpisodeShapes[0].Shape = eCpShape;

                if (EpisodeShapes.Length >= 2)
                {
                    _ = ConvertStringToCpShape(preferencesDatOriginal.EpShapeFix2, out eCpShape);
                    EpisodeShapes[1].Shape = eCpShape;
                }

                if (EpisodeShapes.Length >= 3)
                {
                    _ = ConvertStringToCpShape(preferencesDatOriginal.EpShapeFix3, out eCpShape);
                    EpisodeShapes[2].Shape = eCpShape;

                }

                if (EpisodeShapes.Length >= 4)
                {

                    _ = ConvertStringToCpShape(preferencesDatOriginal.EpShapeFix4, out eCpShape);
                    EpisodeShapes[3].Shape = eCpShape;
                }

                if (EpisodeShapes.Length >= 5)
                {

                    _ = ConvertStringToCpShape(preferencesDatOriginal.EpShapeFix5, out eCpShape);
                    EpisodeShapes[4].Shape = eCpShape;
                }
            }

            // Color
            if (preferencesDatOriginal.EpisodeRandomColor)
            {
                foreach (ShapeObject n in EpisodeShapes)
                {
                    n.ShapeColor = GetRandomColor(Color.Transparent, ColorOpeImageBackColor);
                }
            }
            else
            {
                _ = ConvertStringToColor(preferencesDatOriginal.EpColorFix1, out Color color);
                EpisodeShapes[0].ShapeColor = color;

                if (EpisodeShapes.Length >= 2)
                {
                    _ = ConvertStringToColor(preferencesDatOriginal.EpColorFix2, out color);
                    EpisodeShapes[1].ShapeColor = color;
                }

                if (EpisodeShapes.Length >= 3)
                {
                    _ = ConvertStringToColor(preferencesDatOriginal.EpColorFix3, out color);
                    EpisodeShapes[2].ShapeColor = color;
                }

                if (EpisodeShapes.Length >= 4)
                {
                    _ = ConvertStringToColor(preferencesDatOriginal.EpColorFix4, out color);
                    EpisodeShapes[3].ShapeColor = color;
                }

                if (EpisodeShapes.Length >= 5)
                {
                    _ = ConvertStringToColor(preferencesDatOriginal.EpColorFix5, out color);
                    EpisodeShapes[4].ShapeColor = color;
                }
            }

            if (preferencesDatOriginal.EnableEpImageShape)
            {

                List<string> fileList = GetIncorrectImageObject(preferencesDatOriginal.EpImageFileFolder, preferencesDatOriginal.EpisodeTargetNum);
                for (int i = 1; i < EpisodeShapes.Length; i++)
                {
                    EpisodeShapes[i].Shape = ECpShape.Image;
                    EpisodeShapes[i].ImageFilename = fileList[i];
                }
            }
        }
        /// <summary>
        /// ShapeObject用ポイント生成
        /// 正答条件と訓練条件生成
        /// タッチパスも生成
        /// </summary>
        public void MakeCorrectShape()
        {
            // 固定条件以外はランダムに各パラメータを決定
            ConvertStringToCpCorrectCondtion(preferencesDatOriginal.CorrectCondition, out ECpCorrectCondition eccc);

            //ShapeObject条件作成
            //MakeCorrectShapeInternalFunc(eccc);
            ShapeOpeImageTrainShape = MakeTrainingShape();
            ShapeOpeImageCorrectShape = new ShapeObject();

            //訓練時は正答座標同じ固定 block時は別のを使う
            if (preferencesDatOriginal.OpeTypeOfTask != ECpTask.Training.ToString())
            {
                ShapeOpeImageCorrectShape.Point = MakeCorrectShapeCoordinate(ShapeOpeImageTrainShape).Point;
            }
            else
            {
                ShapeOpeImageCorrectShape.Point = ShapeOpeImageTrainShape.Point;
            }

            ShapeOpeImageCorrectShape.Shape = MakeCorrectShapeShape(ShapeOpeImageTrainShape).Shape;
            ShapeOpeImageCorrectShape.ShapeColor = MakeCorrectShapeColor(ShapeOpeImageTrainShape).ShapeColor;

            // 正答タッチ領域 ShapeOpeImageCorrectShapeの座標を指定
            MakePathOfCorrectShapeOpeImage(ShapeOpeImageCorrectShape.Point);

            //Image
            // randomize mo
            if (preferencesDatOriginal.EnableImageShape)
            {
                ShapeOpeImageCorrectShape.Shape = ECpShape.Image;
                ShapeOpeImageTrainShape.Shape = ECpShape.Image;
                ShapeOpeImageCorrectShape.ImageFilename = GetCorrectImageObject();
                ShapeOpeImageTrainShape.ImageFilename = ShapeOpeImageCorrectShape.ImageFilename;
            }
            return;
        }
        /// <summary>
        /// Episode用 どれでもタッチ
        /// </summary>
        public void MakeAnyShape()
        {
            // 固定条件以外はランダムに各パラメータを決定
            ConvertStringToCpCorrectCondtion(preferencesDatOriginal.CorrectCondition, out ECpCorrectCondition eccc);

            TouchAnyShapes = new ShapeObject[preferencesDatOriginal.EpisodeTargetNum];

            MakeEpisodeShape();
            for (int i = 0; i < EpisodeShapes.Length; i++)
            {
                TouchAnyShapes[i] = EpisodeShapes[i];
            }
            foreach (ShapeObject touchSo in TouchAnyShapes)
            {
                touchSo.ShapeObjectGraphicsPath = MakePath(touchSo.Point);
            }
        }

        public void MakeAnyShape(string path)
        {
            // 固定条件以外はランダムに各パラメータを決定
            ConvertStringToCpCorrectCondtion(preferencesDatOriginal.CorrectCondition, out ECpCorrectCondition eccc);

            TouchAnyShapes = new ShapeObject[preferencesDatOriginal.EpisodeTargetNum];

            //ShapeOpeImageCorrectShape = MakeTrainingShape();

            //if (ShapeOpeImageTrainShape is null)
            //{
            //    ShapeOpeImageTrainShape = new ShapeObject();
            //}

            //// Image時
            //if (preferencesDatOriginal.EnableEpImageShape)
            //{
            //    ShapeOpeImageCorrectShape.Shape = ECpShape.Image;
            //    ShapeOpeImageTrainShape.Shape = ECpShape.Image;
            //    ShapeOpeImageCorrectShape.ImageFilename = GetCorrectImageObject(preferencesDatOriginal.EpImageFileFolder);
            //    ShapeOpeImageTrainShape.ImageFilename = ShapeOpeImageCorrectShape.ImageFilename;
            //}
            //TouchAnyShapes[0] = ShapeOpeImageCorrectShape;

            MakeEpisodeShape();
            for (int i = 0; i < EpisodeShapes.Length; i++)
            {
                TouchAnyShapes[i] = EpisodeShapes[i];
            }
            foreach (ShapeObject touchSo in TouchAnyShapes)
            {
                touchSo.ShapeObjectGraphicsPath = MakePath(touchSo.Point);
            }
        }

        public void SortCorrectShape()
        {
            ShapeObject[] shapeObjects = new ShapeObject[TouchAnyShapes.Length];
            shapeObjects[0] = TouchAnyShapes.Where(x => x.Touched).First();

            ShapeOpeImageCorrectShape = shapeObjects[0];

            IEnumerable<ShapeObject> query = TouchAnyShapes.Where(x => x.Touched == false);


            int i = 1;
            foreach (var n in query)
            {
                if (i > shapeObjects.Length)
                {
                    break;
                }

                EpisodeShapes[i - 1] = n;
                shapeObjects[i] = n;
                i++;
            }
            TouchAnyShapes = shapeObjects;
        }

        public void MakeTrainShapeBlock()
        {
            // 固定条件以外はランダムに各パラメータを決定
            ConvertStringToCpCorrectCondtion(preferencesDatOriginal.CorrectCondition, out ECpCorrectCondition eccc);

            //ShapeObject条件作成
            ShapeOpeImageTrainShape = MakeTrainingShape();

            // Trainを正解に
            ShapeOpeImageCorrectShape = ShapeOpeImageTrainShape;

            // 正答タッチ領域 ShapeOpeImageCorrectShapeの座標を指定
            MakePathOfCorrectShapeOpeImage(ShapeOpeImageTrainShape.Point);

            //Image
            // randomize mo
            if (preferencesDatOriginal.EnableImageShape)
            {
                ShapeOpeImageTrainShape.Shape = ECpShape.Image;
                ShapeOpeImageTrainShape.ImageFilename = GetCorrectImageObject();
            }
            return;
        }

        public void MakeCorrectShapeBlock()
        {
            //ShapeOpeImageTrainShapeが有効か確認チェック もっと厳密にやる
            if (ShapeOpeImageTrainShape is null)
            {
                // nullの場合は2択想定
                // ViewCorrectImageされる前にCorrectWrongImageされていると発生
                MakeTrainShapeBlock();
                //throw new Exception("正解画像が生成されていません");
                //return;
            }
            ShapeOpeImageCorrectShape = new ShapeObject();
            ShapeOpeImageCorrectShape.Point = MakeCorrectShapeCoordinate(ShapeOpeImageTrainShape).Point;
            ShapeOpeImageCorrectShape.Shape = MakeCorrectShapeShape(ShapeOpeImageTrainShape).Shape;
            ShapeOpeImageCorrectShape.ShapeColor = MakeCorrectShapeColor(ShapeOpeImageTrainShape).ShapeColor;

            // 正答タッチ領域 ShapeOpeImageCorrectShapeの座標を指定
            MakePathOfCorrectShapeOpeImage(ShapeOpeImageCorrectShape.Point);

            // randomize mo
            if (preferencesDatOriginal.EnableImageShape)
            {
                ShapeOpeImageCorrectShape.Shape = ECpShape.Image;
                // 訓練ShapeObjectから情報取得
                ShapeOpeImageCorrectShape.ImageFilename = ShapeOpeImageTrainShape.ImageFilename;
            }
        }

        /// <summary>
        /// ShapeObject作成用内部関数 ばらしたので未使用
        /// </summary>
        /// <param name="eCpCorrectCondition"></param>
        private void MakeCorrectShapeInternalFunc(ECpCorrectCondition eCpCorrectCondition)
        {
            ShapeOpeImageCorrectShape = new ShapeObject();
            ShapeOpeImageTrainShape = new ShapeObject();

            ECpShape settingECpShape;
            ECpTask settingECpTask;
            Color settingColor;
            Point point;
            Point[] incorrectPoints ={ new Point(preferencesDatOriginal.IncorrectCoordinateFixX1,preferencesDatOriginal.IncorrectCoordinateFixY1),
                new Point(preferencesDatOriginal.IncorrectCoordinateFixX2,preferencesDatOriginal.IncorrectCoordinateFixY2),
                new Point(preferencesDatOriginal.IncorrectCoordinateFixX3,preferencesDatOriginal.IncorrectCoordinateFixY3),
                new Point(preferencesDatOriginal.IncorrectCoordinateFixX4,preferencesDatOriginal.IncorrectCoordinateFixY4) };

            var minDistance = OpeImageSizeOfShapeInPixel + preferencesDatOriginal.MinDistanceBetweenCorrectAndWrongShape;

            ConvertStringToCpTask(preferencesDatOriginal.OpeTypeOfTask, out settingECpTask);

            // Train + FixPoints
            var existsPoints = new Point[preferencesDatOriginal.IncorrectNum + 1];
            if (!preferencesDatOriginal.IncorrectCoordinateRandom)
            {
                // existsPoints[0] train用
                existsPoints[1] = incorrectPoints[0];
                if (preferencesDatOriginal.IncorrectNum >= 2)
                {
                    existsPoints[2] = incorrectPoints[1];
                }
                if (preferencesDatOriginal.IncorrectNum >= 3)
                {
                    existsPoints[3] = incorrectPoints[2];
                }
                if (preferencesDatOriginal.IncorrectNum >= 4)
                {
                    existsPoints[4] = incorrectPoints[3];
                }
            }

            // 座標決定
            {
                if (preferencesDatOriginal.RandomCoordinate)
                {
                    // 判定条件によらない
                    if (!preferencesDatOriginal.IncorrectCoordinateRandom && settingECpTask != ECpTask.Training)
                    {
                        _ = GeneratePoint(existsPoints, out point, minDistance);
                    }
                    else
                    {
                        _ = GeneratePoint(null, out point, minDistance);
                    }
                    //---------------------
                    ShapeOpeImageTrainShape.Point = point;

                    if (eCpCorrectCondition == ECpCorrectCondition.Coordinate)
                    {
                        //同じ座標
                        ShapeOpeImageCorrectShape.Point = point;
                    }
                    else
                    {
                        existsPoints[0] = ShapeOpeImageTrainShape.Point;
                        // 訓練時以外のみ違う座標をジェネレート この部分をブロックプログラムで呼ばれているか判定する
                        if (settingECpTask != ECpTask.Training)
                        {
                            _ = GeneratePoint(null, out point, minDistance);
                        }
                        ShapeOpeImageCorrectShape.Point = point;
                    }
                    //---------------------
                }
                else
                {
                    Point fixedPoint = new Point(preferencesDatOriginal.CoordinateFixX, preferencesDatOriginal.CoordinateFixY);
                    ShapeOpeImageCorrectShape.Point = fixedPoint;
                    ShapeOpeImageTrainShape.Point = fixedPoint;
                }


            }
            // 形状決定
            {
                if (eCpCorrectCondition == ECpCorrectCondition.Shape)
                {
                    //固定形状
                    if (!preferencesDatOriginal.RandomShape)
                    {
                        _ = ConvertStringToCpShape(preferencesDatOriginal.ShapeFix, out settingECpShape);
                    }
                    else
                    {
                        if (!preferencesDatOriginal.IncorrectShapeRandom && settingECpTask != ECpTask.Training)
                        {
                            ECpShape incorrectShape;
                            _ = ConvertStringToCpShape(preferencesDatOriginal.IncorrectShapeFix1, out incorrectShape);
                            settingECpShape = GetRandomShape(incorrectShape);
                        }
                        else
                        {
                            settingECpShape = GetRandomShape(ECpShape.None);

                        }
                    }
                    ShapeOpeImageCorrectShape.Shape = settingECpShape;
                    ShapeOpeImageTrainShape.Shape = settingECpShape;
                }
                else
                {
                    if (preferencesDatOriginal.RandomShape)
                    {
                        settingECpShape = GetRandomShape(ECpShape.None);
                        ShapeOpeImageTrainShape.Shape = settingECpShape;
                        // 違う形状を選択
                        if (settingECpTask != ECpTask.Training)
                            settingECpShape = GetRandomShape(ShapeOpeImageTrainShape.Shape);
                        ShapeOpeImageCorrectShape.Shape = settingECpShape;
                    }
                    else
                    {
                        //固定形状
                        _ = ConvertStringToCpShape(preferencesDatOriginal.ShapeFix, out settingECpShape);
                        ShapeOpeImageCorrectShape.Shape = settingECpShape;
                        ShapeOpeImageTrainShape.Shape = settingECpShape;
                    }
                }
            }

            //色決定
            {
                if (eCpCorrectCondition == ECpCorrectCondition.Color)
                {
                    if (!preferencesDatOriginal.RandomColor)
                    {
                        _ = ConvertStringToColor(preferencesDatOriginal.ColorFix, out settingColor);
                    }
                    else
                    {
                        if (!preferencesDatOriginal.IncorrectColorRandom && settingECpTask != ECpTask.Training)
                        {
                            Color incorrectColor;
                            _ = ConvertStringToColor(preferencesDatOriginal.IncorrectColorFix1, out incorrectColor);
                            settingColor = GetRandomColor(incorrectColor, ColorOpeImageBackColor);
                        }
                        else
                        {
                            settingColor = GetRandomColor(Color.Empty, ColorOpeImageBackColor);

                        }
                    }
                    ShapeOpeImageCorrectShape.ShapeColor = settingColor;
                    ShapeOpeImageTrainShape.ShapeColor = settingColor;

                }
                else // (eCpCorrectCondition != ECpCorrectCondition.Color && preferencesDatOriginal.RandomColor)
                {
                    // ランダムカラー条件
                    if (preferencesDatOriginal.RandomColor)
                    {
                        settingColor = GetRandomColor(Color.Empty, ColorOpeImageBackColor);
                        ShapeOpeImageTrainShape.ShapeColor = settingColor;
                        // 違う色を選択
                        if (settingECpTask != ECpTask.Training)
                            settingColor = GetRandomColor(settingColor, ColorOpeImageBackColor);
                        ShapeOpeImageCorrectShape.ShapeColor = settingColor;
                    }
                    else
                    {
                        //固定色
                        _ = ConvertStringToColor(preferencesDatOriginal.ColorFix, out settingColor);
                        ShapeOpeImageTrainShape.ShapeColor = settingColor;
                        ShapeOpeImageCorrectShape.ShapeColor = settingColor;
                    }

                }
            }
        }

        /// <summary>
        /// 訓練用ShapeObject取得
        /// </summary>
        /// <returns></returns>
        private ShapeObject MakeTrainingShape()
        {
            ShapeObject retShapeObject = new ShapeObject();
            Point[] p = {
                new Point(preferencesDatOriginal.IncorrectCoordinateFixX1, preferencesDatOriginal.IncorrectCoordinateFixY1),
                new Point(preferencesDatOriginal.IncorrectCoordinateFixX2, preferencesDatOriginal.IncorrectCoordinateFixY2),
                new Point(preferencesDatOriginal.IncorrectCoordinateFixX3, preferencesDatOriginal.IncorrectCoordinateFixY3),
                new Point(preferencesDatOriginal.IncorrectCoordinateFixX4, preferencesDatOriginal.IncorrectCoordinateFixY4)
            };
            Point[] incorrectPoints = p;
            var minDistance = OpeImageSizeOfShapeInPixel + preferencesDatOriginal.MinDistanceBetweenCorrectAndWrongShape;

            // Train + FixPoints
            var existsPoints = new Point[preferencesDatOriginal.IncorrectNum + 1];

            if (preferencesDatOriginal.IncorrectCoordinateRandom)
            {
            }
            else
            {
                existsPoints[1] = incorrectPoints[0];
                if (preferencesDatOriginal.IncorrectNum >= 2)
                {
                    existsPoints[2] = incorrectPoints[1];
                }
                if (preferencesDatOriginal.IncorrectNum >= 3)
                {
                    existsPoints[3] = incorrectPoints[2];
                }
                if (preferencesDatOriginal.IncorrectNum >= 4)
                {
                    existsPoints[4] = incorrectPoints[3];
                }
            }
            // Point
            {
                Point settingPoint;
                if (preferencesDatOriginal.RandomCoordinate)
                {
                    //ランダム
                    _ = GeneratePoint(existsPoints, out settingPoint, minDistance);
                    retShapeObject.Point = settingPoint;
                }
                else
                {
                    //固定座標
                    settingPoint = new Point(preferencesDatOriginal.CoordinateFixX, preferencesDatOriginal.CoordinateFixY);
                    retShapeObject.Point = settingPoint;
                }
            }
            // Shape
            {
                ECpShape settingECpShape;
                if (preferencesDatOriginal.RandomShape)
                {
                    //完全ランダム 重複あり
                    settingECpShape = GetRandomShape(ECpShape.None);
                    retShapeObject.Shape = settingECpShape;
                }
                else
                {
                    //固定形状
                    _ = ConvertStringToCpShape(preferencesDatOriginal.ShapeFix, out settingECpShape);
                    retShapeObject.Shape = settingECpShape;
                }
            }
            // Color
            {
                Color settingColor;
                // ランダムカラー条件
                if (preferencesDatOriginal.RandomColor)
                {
                    //完全ランダム 重複あり
                    settingColor = GetRandomColor(Color.Empty, ColorOpeImageBackColor);
                    retShapeObject.ShapeColor = settingColor;
                }
                else
                {
                    //固定色
                    _ = ConvertStringToColor(preferencesDatOriginal.ColorFix, out settingColor);
                    retShapeObject.ShapeColor = settingColor;
                }
            }
            return retShapeObject;
        }

        /// <summary>
        /// 正答座標 ShapeObject経由
        /// </summary>
        /// <param name="trainingShapeObject"></param>
        /// <returns></returns>
        private ShapeObject MakeCorrectShapeCoordinate(ShapeObject trainingShapeObject)
        {
            ShapeObject retShapeObject = new ShapeObject();
            ConvertStringToCpCorrectCondtion(preferencesDatOriginal.CorrectCondition, out ECpCorrectCondition correctCondition);
            Point[] p = {
                new Point(preferencesDatOriginal.IncorrectCoordinateFixX1, preferencesDatOriginal.IncorrectCoordinateFixY1),
                new Point(preferencesDatOriginal.IncorrectCoordinateFixX2, preferencesDatOriginal.IncorrectCoordinateFixY2),
                new Point(preferencesDatOriginal.IncorrectCoordinateFixX3, preferencesDatOriginal.IncorrectCoordinateFixY3),
                new Point(preferencesDatOriginal.IncorrectCoordinateFixX4, preferencesDatOriginal.IncorrectCoordinateFixY4)
            };
            Point[] incorrectPoints = p;
            var minDistance = OpeImageSizeOfShapeInPixel + preferencesDatOriginal.MinDistanceBetweenCorrectAndWrongShape;

            // Train + FixPoints
            var existsPoints = new Point[preferencesDatOriginal.IncorrectNum + 1];
            //Trainingポイント
            //2択時想定
            if (trainingShapeObject is null)
            {
                trainingShapeObject = new ShapeObject();
            }
            else
            {
                existsPoints[0] = trainingShapeObject.Point;
            }
            if (!preferencesDatOriginal.IncorrectCoordinateRandom)
            {
                existsPoints[1] = incorrectPoints[0];
                if (preferencesDatOriginal.IncorrectNum >= 2)
                {
                    existsPoints[2] = incorrectPoints[1];
                }
                if (preferencesDatOriginal.IncorrectNum >= 3)
                {
                    existsPoints[3] = incorrectPoints[2];
                }
                if (preferencesDatOriginal.IncorrectNum >= 4)
                {
                    existsPoints[4] = incorrectPoints[3];
                }
            }
            if (correctCondition == ECpCorrectCondition.Coordinate)
            {
                if (trainingShapeObject.Point != new Point(0, 0))
                {
                    retShapeObject.Point = trainingShapeObject.Point;
                }
                else
                {
                    retShapeObject.Point = GenerateCorrectPoint(existsPoints);
                }
            }
            else
            // Coordinate条件ではない時
            {
                retShapeObject.Point = GenerateCorrectPoint(existsPoints);
            }
            return retShapeObject;
        }
        private Point GenerateCorrectPoint(Point[] existsPoints)
        {
            Point settingPoint;
            int minDistance = OpeImageSizeOfShapeInPixel + preferencesDatOriginal.MinDistanceBetweenCorrectAndWrongShape;
            if (preferencesDatOriginal.RandomCoordinate)
            {
                //ランダム
                _ = GeneratePoint(existsPoints, out settingPoint, minDistance);
            }
            else
            {
                //固定座標
                settingPoint = new Point(preferencesDatOriginal.CoordinateFixX, preferencesDatOriginal.CoordinateFixY);
            }
            return settingPoint;
        }
        /// <summary>
        /// 与えられたShapeObjectに対してのCorrectObject Shapeを返す
        /// </summary>
        /// <param name="trainingShapeObject"></param>
        /// <returns></returns>
        private ShapeObject MakeCorrectShapeShape(ShapeObject trainingShapeObject)
        {
            ShapeObject retShapeObject = new ShapeObject();
            ConvertStringToCpCorrectCondtion(preferencesDatOriginal.CorrectCondition, out ECpCorrectCondition correctCondition);
            if (correctCondition == ECpCorrectCondition.Shape)
            {
                retShapeObject.Shape = trainingShapeObject.Shape;
            }
            else
            // Shape条件ではない時
            {
                ECpShape settingECpShape;
                if (preferencesDatOriginal.RandomShape)
                {
                    //完全ランダム 重複あり
                    settingECpShape = GetRandomShape(trainingShapeObject.Shape);
                    retShapeObject.Shape = settingECpShape;
                }
                else
                {
                    //固定形状
                    _ = ConvertStringToCpShape(preferencesDatOriginal.ShapeFix, out settingECpShape);
                    retShapeObject.Shape = settingECpShape;
                }
            }
            return retShapeObject;
        }

        /// <summary>
        /// 与えられたShapeObjectに対してのCorrectObjectを作成する
        /// </summary>
        /// <param name="trainingShapeObject"></param>
        private ShapeObject MakeCorrectShapeColor(ShapeObject trainingShapeObject)
        {
            ShapeObject retShapeObject = new ShapeObject();
            ConvertStringToCpCorrectCondtion(preferencesDatOriginal.CorrectCondition, out ECpCorrectCondition correctCondition);

            if (correctCondition == ECpCorrectCondition.Color)
            {
                retShapeObject.ShapeColor = trainingShapeObject.ShapeColor;
            }
            else
            // Color条件ではない時
            {
                Color settingColor;
                // ランダムカラー条件
                if (preferencesDatOriginal.RandomColor)
                {
                    //完全ランダム 重複あり
                    settingColor = GetRandomColor(trainingShapeObject.ShapeColor, ColorOpeImageBackColor);
                    retShapeObject.ShapeColor = settingColor;
                }
                else
                {
                    //固定色
                    _ = ConvertStringToColor(preferencesDatOriginal.ColorFix, out settingColor);
                    retShapeObject.ShapeColor = settingColor;
                }
            }
            return retShapeObject;
        }


        private Bitmap GetCorrectImageObject(out string fileName)
        {
            if (preferencesDatOriginal.RandomCorrectImage)
            {
                string[] patterns = { ".jpg", ".png", ".bmp" };
                var folder = preferencesDatOriginal.ImageFileFolder;
                var fileList = System.IO.Directory.GetFiles(folder).Where(f => (patterns.Any(pattern => f.ToLower().EndsWith(pattern)))).ToList();
                var file = fileList.ElementAt(opeImageRandom.Next(fileList.Count));
                CorrectImageFile = file;

                if (!System.IO.File.Exists(file))
                {
                    throw new System.IO.FileNotFoundException("File not found.");
                }
                fileName = _CorrectImageFile; // フルパスで取得
                return ImageLoader.LoadImage(_CorrectImageFile, new Size(OpeImageSizeOfShapeInPixel, OpeImageSizeOfShapeInPixel));
            }
            else
            {
                if (!System.IO.File.Exists(preferencesDatOriginal.CorrectImageFile))
                {
                    throw new System.IO.FileNotFoundException("File not found.");
                }
                CorrectImageFile = preferencesDatOriginal.CorrectImageFile;
                fileName = _CorrectImageFile;
                return ImageLoader.LoadImage(preferencesDatOriginal.CorrectImageFile, new Size(OpeImageSizeOfShapeInPixel, OpeImageSizeOfShapeInPixel));
            }

        }
        /// <summary>
        /// 正答画像ファイル取得
        /// </summary>
        /// <returns>フルパス</returns>
        /// <exception cref="FileNotFoundException"></exception>
        private string GetCorrectImageObject()
        {
            string fileName;
            if (preferencesDatOriginal.RandomCorrectImage)
            {
                string[] patterns = { ".jpg", ".png", ".bmp" };
                var folder = preferencesDatOriginal.ImageFileFolder;
                var fileList = Directory.GetFiles(folder).Where(f => (patterns.Any(pattern => f.ToLower().EndsWith(pattern)))).ToList();
                var file = fileList.ElementAt(opeImageRandom.Next(fileList.Count));
                CorrectImageFile = file;
                if (!File.Exists(file))
                {
                    throw new FileNotFoundException("File not found.");
                }
                fileName = file;
            }
            else
            {
                if (!File.Exists(preferencesDatOriginal.CorrectImageFile))
                {
                    throw new FileNotFoundException("File not found.");
                }
                CorrectImageFile = preferencesDatOriginal.CorrectImageFile;
                fileName = _CorrectImageFile;
            }
            return fileName;
        }

        private string GetCorrectImageObject(string path)
        {
            string fileName;
            if (preferencesDatOriginal.RandomCorrectImage)
            {
                string[] patterns = { ".jpg", ".png", ".bmp" };
                var folder = path;
                if (!Directory.Exists(folder))
                {
                    throw new DirectoryNotFoundException("Folder not found.");
                }
                var fileList = Directory.GetFiles(folder).Where(f => (patterns.Any(pattern => f.ToLower().EndsWith(pattern)))).ToList();
                var file = fileList.ElementAt(opeImageRandom.Next(fileList.Count));
                CorrectImageFile = file;
                if (!File.Exists(file))
                {
                    throw new FileNotFoundException("File not found.");
                }
                fileName = file;
            }
            else
            {
                if (!File.Exists(preferencesDatOriginal.CorrectImageFile))
                {
                    throw new FileNotFoundException("File not found.");
                }
                CorrectImageFile = preferencesDatOriginal.CorrectImageFile;
                fileName = _CorrectImageFile;
            }
            return fileName;
        }

        private List<Bitmap> GetIncorrectImageObject(out List<string> fileList)
        {
            if (preferencesDatOriginal.RandomCorrectImage)
            {
                return ImageLoader.LoadMultiImages(_CorrectImageFile, preferencesDatOriginal.IncorrectNum, preferencesDatOriginal.ImageFileFolder, opeImageRandom, new Size(OpeImageSizeOfShapeInPixel, OpeImageSizeOfShapeInPixel), out fileList);
            }
            else
            {
                return ImageLoader.LoadMultiImages(preferencesDatOriginal.CorrectImageFile, preferencesDatOriginal.IncorrectNum, preferencesDatOriginal.ImageFileFolder, opeImageRandom, new Size(OpeImageSizeOfShapeInPixel, OpeImageSizeOfShapeInPixel), out fileList);
            }

        }

        private List<string> GetIncorrectImageObject(string folder, int incorrectNum)
        {
            if (preferencesDatOriginal.RandomCorrectImage)
            {

            }
            string[] patterns = { ".jpg", ".png", ".bmp" };
            List<string> imageList = Directory.GetFiles(folder).Where(f => ((f != _CorrectImageFile) && patterns.Any(pattern => f.ToLower().EndsWith(pattern)))).ToList();
            var list = new List<Bitmap>();
            for (int i = 0; i < incorrectNum; i++)
            {
                var selectionNum = opeImageRandom.Next(imageList.Count);
                list.Add(new Bitmap(imageList.ElementAt(selectionNum)));
            }
            return imageList;
        }

        /// <summary>
        /// タッチ領域用パス作成
        /// </summary>
        /// <param name="point">正答タッチ領域中心</param>
        public void MakePathOfCorrectShapeOpeImage(Point point)
        {
            mGraphicsPathOpeImageShape = new System.Drawing.Drawing2D.GraphicsPath();
            mGraphicsPathOpeImageShape.AddEllipse(
                                point.X - OpeImageSizeOfShapeInPixel / 2,
                                point.Y - OpeImageSizeOfShapeInPixel / 2,
                                OpeImageSizeOfShapeInPixel,
                                OpeImageSizeOfShapeInPixel);
            return;
        }

        public GraphicsPath MakePath(Point point)
        {
            var gp = new GraphicsPath();
            gp.AddEllipse(
                                point.X - OpeImageSizeOfShapeInPixel / 2,
                                point.Y - OpeImageSizeOfShapeInPixel / 2,
                                OpeImageSizeOfShapeInPixel,
                                OpeImageSizeOfShapeInPixel);
            return gp;
        }
        /// <summary>
        /// SVGインスタンスからパス作成
        /// </summary>
        public void MakePathOfSvg(ShapeObject so)
        {
            //mGraphicsPathOpeImageShape = so.ShapeGraphicsPath;
            mGraphicsPathOpeImageShape = GetGraphicsPath(so.Shape, so.Point);
        }
        public GraphicsPath GetGraphicsPath(ECpShape eCpShape, Point centerPoint)
        {
            return GetGraphicsPath(GetBitmapFromEcpShape(eCpShape), centerPoint);
        }

        public GraphicsPath GetGraphicsPath(Bitmap bitmap, Point centerPoint)
        {
            GraphicsPath graphicsPath;
            List<Point> points;
            points = GetBitmapContour(bitmap, Color.Black, centerPoint);

            byte[] n = new byte[points.Count];
            for (int i = 0; i < points.Count; i++)
            {
                if (i == 0)
                {
                    n[i] = 0x0;
                }
                else if (i == points.Count - 1)
                {
                    n[i] = 128;
                }
                else
                {
                    n[i] = 0x3;
                }
            }
            graphicsPath = new GraphicsPath();
            graphicsPath.AddPolygon(points.ToArray());


            return graphicsPath;
        }
        List<Point> GetBitmapContour(Bitmap bitmap, Color color, Point point)
        {
            List<Point> nPoints = new List<Point>();
            for (int x = 0; x < bitmap.Width; x++)
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    Color pixelColor = bitmap.GetPixel(x, y);
                    if (pixelColor.ToArgb() == color.ToArgb())
                    {
                        bool b1 = bitmap.GetPixel(x + 1, y).ToArgb() != color.ToArgb();
                        bool b2 = bitmap.GetPixel(x - 1, y).ToArgb() != color.ToArgb();
                        bool b3 = bitmap.GetPixel(x, y + 1).ToArgb() != color.ToArgb();
                        bool b4 = bitmap.GetPixel(x, y - 1).ToArgb() != color.ToArgb();

                        if (b1 || b2 || b3 || b4)
                            nPoints.Add(new Point(x + (point.X - OpeImageSizeOfShapeInPixel / 2), y + (point.Y - OpeImageSizeOfShapeInPixel / 2)));
                    }
                }
            }
            return nPoints;
        }
        Point GetPointFromColor(Bitmap bitmap, Color color)
        {
            for (int x = 0; x < bitmap.Width; x++)
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    Color color1 = bitmap.GetPixel(x, y);
                    if (color1.ToArgb() == color.ToArgb())
                    {
                        return new Point(x, y);
                    }
                }
            }
            return Point.Empty;
        }
        /// <summary>
        /// 正答タッチ領域パス作成
        /// </summary>
        public void MakePathOfCorrectShapeOpeImage()
        {
            MakePathOfCorrectShapeOpeImage(PointOpeImageCenterOfCorrectShape);
        }

        /// <summary>
        /// 正答ポイント削除
        /// </summary>
        public void DeletePathOfCorrectShapeOpeImage()
        {
            mGraphicsPathOpeImageShape = null;
        }
        /// <summary>
        /// タッチ領域用パス作成 不正解図形用
        /// </summary>
        /// <param name="point"></param>
        public void MakePathOfIncorrectShapeOpeImage(IReadOnlyList<Point> points)
        {
            mGraphicsPathOpeIncorrectImageShape = new GraphicsPath();

            foreach (Point p in points)
            {
                mGraphicsPathOpeIncorrectImageShape.AddEllipse(
                                    p.X - OpeImageSizeOfShapeInPixel / 2,
                                    p.Y - OpeImageSizeOfShapeInPixel / 2,
                                    OpeImageSizeOfShapeInPixel,
                                    OpeImageSizeOfShapeInPixel);
            }
            return;
        }

        /// <summary>
        /// 不正解タッチ領域パス作成
        /// </summary>
        public void MakePathOfIncorrectShapeOpeImage()
        {
            MakePathOfIncorrectShapeOpeImage(PointOpeImageCenterOfWrongShapeList);
        }
        /// <summary>
        /// 不正解タッチ領域パス作成 ShapeObject[] 先頭CorrectShapeなので読みとばす
        /// </summary>
        /// <param name="so"></param>
        public void MakePathOfIncorrectShapeOpeImage(ShapeObject[] so)
        {
            List<Point> points = new List<Point>();

            for (int i = 1; i < so.Length; i++)
            {
                points.Add(so[i].Point);
            }
            MakePathOfIncorrectShapeOpeImage(points);
        }
        /// <summary>
        /// 不正解ポイント削除
        /// </summary>
        public void DeletePathOfIncorrectShapeOpeImage()
        {
            mGraphicsPathOpeIncorrectImageShape = null;
        }
        /// <summary>
        /// 既存ポイントから許容距離以上離れた <seealso cref="System.Drawing.Point"/> を生成する
        /// </summary>
        /// <param name="inputPoints">既存ポイントリスト</param>
        /// <param name="targetPoint">生成結果Point</param>
        /// <param name="distance">許容距離</param>
        public bool GeneratePoint(Point[] inputPoints, out Point targetPoint, int distance)
        {
            Point point = new Point();
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            List<bool> resultTestList = new List<bool>();
            //int count = 0;
            bool ret = false;

            if ((RectOpeImageValidAreaToDrawShape.X == 0) || (RectOpeImageValidAreaToDrawShape.Y == 0))
            {
                throw new Exception("Not initialize OpeImage!");
            }

            while (true)
            {
                point.X = opeImageRandom.Next(RectOpeImageValidAreaToDrawShape.X,
                    RectOpeImageValidAreaToDrawShape.X + RectOpeImageValidAreaToDrawShape.Width + 1);

                point.Y = opeImageRandom.Next(RectOpeImageValidAreaToDrawShape.Y,
                    RectOpeImageValidAreaToDrawShape.Y + RectOpeImageValidAreaToDrawShape.Height + 1);

                resultTestList.Clear();

                // inputPointsが渡されなかったらチェックせずに終了
                // 初期ジェネレート用
                if (inputPoints == null)
                {
                    stopwatch.Stop();
                    break;
                }

                foreach (Point point1 in inputPoints)
                {
                    if (IsDistanceInRangeOpeImage(point1, point, distance) == false)
                    {
                        resultTestList.Add(true);
                    }
                    else
                    {
                        resultTestList.Add(false);
                        //count++;
                        //Debug.WriteLine("Point false.: " + count.ToString());
                    }
                }
                //Parallel.ForEach(inputPoints, points =>
                // {
                //     if (IsDistanceInRangeOpeImage(points, point, distance) == false)
                //     {
                //         resultTestList.Add(true);
                //     }
                //     else
                //     {
                //         resultTestList.Add(false);
                //         //Debug.WriteLine("Point false.");
                //     }
                // });

                //if (IsDistanceInRangeOpeImage(inputPoint, point, distance) == false)
                if (!resultTestList.Any(result => result == false))
                {
                    ret = true;
                    stopwatch.Stop();
                    break;
                }

                if (stopwatch.ElapsedMilliseconds >= (long)OpeImageDefault.TimeoutOfTryToMakeWroingShapePoint)
                {
                    //break;
                    throw new Exception(String.Format("Error in {0}: Timeout to make point", MethodBase.GetCurrentMethod().Name));
                }
            }

            targetPoint = point;
            return ret;
        }
        public Point[] GenerateIncorrectPoint(Point correctPoint, int distance)
        {
            List<Point> point = new List<Point>();
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            List<bool> resultTestList = new List<bool>();
            //int count = 0;

            var path = new GraphicsPath();
            path.AddEllipse(correctPoint.X, correctPoint.Y, (float)distance, distance);


            if ((RectOpeImageValidAreaToDrawShape.X == 0) || (RectOpeImageValidAreaToDrawShape.Y == 0))
            {
                throw new Exception("Not initialize OpeImage!");
            }
            Point tempPoint = new Point();

            if (correctPoint == null)
            {
                stopwatch.Stop();
            }
            while (true)
            {

                tempPoint.X = opeImageRandom.Next(RectOpeImageValidAreaToDrawShape.X,
                    RectOpeImageValidAreaToDrawShape.X + RectOpeImageValidAreaToDrawShape.Width + 1);

                tempPoint.Y = opeImageRandom.Next(RectOpeImageValidAreaToDrawShape.Y,
                    RectOpeImageValidAreaToDrawShape.Y + RectOpeImageValidAreaToDrawShape.Height + 1);

                //resultTestList.Clear();

                if (!path.IsVisible(tempPoint))
                {
                    point.Add(tempPoint);
                }
                path.AddEllipse(tempPoint.X, tempPoint.Y, distance, distance);

                if (point.Count > 3)
                {
                    break;
                }

                if (stopwatch.ElapsedMilliseconds >= (long)OpeImageDefault.TimeoutOfTryToMakeWroingShapePoint)
                {
                    //break;
                    throw new Exception(String.Format("Error in {0}: Timeout to make point", MethodBase.GetCurrentMethod().Name));
                }
            }



            return point.ToArray();
        }

        /// <summary>
        /// ShapeObjectを描画
        /// </summary>
        /// <param name="so"></param>
        /// <param name="addCenterDot"></param>
        private void DrawShape(ShapeObject so, bool addCenterDot)
        {
            try
            {
                lock (syncPictureObject)
                {
                    //ImageオブジェクトのGraphicsオブジェクトを作成する
                    //using (Graphics graphicsObj = Graphics.FromImage(mBitmapOpeImageCanvas))
                    Graphics graphicsObj = graphicsObjGlobal;
                    {

                        switch (so.Shape)
                        {
                            case ECpShape.Circle:
                                if (CheckExistSvgShape(ECpShape.SvgCircle))
                                {
                                    so.Shape = ECpShape.SvgCircle;
                                    drawSvg(so);
                                }
                                else
                                {
                                    drawCircle(so.Point, graphicsObj, so.ShapeColor);
                                }
                                break;
                            case ECpShape.Rectangle:
                                if (CheckExistSvgShape(ECpShape.SvgSquare))
                                {
                                    so.Shape = ECpShape.SvgSquare;
                                    drawSvg(so);
                                }
                                else
                                {
                                    drawRectangle(so.Point, graphicsObj, so.ShapeColor);
                                }
                                break;
                            case ECpShape.Triangle:
                                if (CheckExistSvgShape(ECpShape.SvgTriangle))
                                {
                                    so.Shape = ECpShape.SvgTriangle;
                                    drawSvg(so);
                                }
                                else
                                {
                                    drawTriangle(so.Point, graphicsObj, so.ShapeColor);
                                }
                                break;
                            case ECpShape.Star:
                                if (CheckExistSvgShape(ECpShape.SvgStar))
                                {
                                    so.Shape = ECpShape.SvgStar;
                                    drawSvg(so);
                                }
                                else
                                {
                                    drawStar(so.Point, graphicsObj, so.ShapeColor);
                                }
                                break;
                            case ECpShape.Image:
                                DrawBitmap(so);
                                break;
                            case ECpShape.SvgCircle:
                                drawSvg(so);
                                break;
                            case ECpShape.SvgRectangle:
                                drawSvg(so);
                                break;
                            case ECpShape.SvgSquare:
                                drawSvg(so);
                                break;
                            case ECpShape.SvgScalene:
                                drawSvg(so);
                                break;
                            case ECpShape.SvgTriangle:
                                drawSvg(so);
                                break;
                            case ECpShape.SvgStar:
                                drawSvg(so);
                                break;

                            case ECpShape.SvgRight:
                                drawSvg(so);
                                break;
                            case ECpShape.SvgPentagon:
                                drawSvg(so);
                                break;
                            case ECpShape.SvgTrapeze:
                                drawSvg(so);
                                break;
                            case ECpShape.SvgKite:
                                drawSvg(so);
                                break;
                            case ECpShape.SvgPolygon:
                                drawSvg(so);
                                break;
                            case ECpShape.SvgParallelogram:
                                drawSvg(so);
                                break;
                            case ECpShape.SvgEllipse:
                                drawSvg(so);
                                break;
                            case ECpShape.SvgTrefoil:
                                drawSvg(so);
                                break;
                            case ECpShape.SvgSemiCircle:
                                drawSvg(so);
                                break;
                            case ECpShape.SvgHexagon:
                                drawSvg(so);
                                break;
                            case ECpShape.SvgCresent:
                                drawSvg(so);
                                break;
                            case ECpShape.SvgOctagon:
                                drawSvg(so);
                                break;
                            case ECpShape.SvgCross:
                                drawSvg(so);
                                break;
                            case ECpShape.SvgRing:
                                drawSvg(so);
                                break;
                            case ECpShape.SvgPic:
                                drawSvg(so);
                                break;
                            case ECpShape.SvgHeart:
                                drawSvg(so);
                                break;
                            case ECpShape.SvgArrow:
                                drawSvg(so);
                                break;
                            case ECpShape.SvgQuadrefoil:
                                drawSvg(so);
                                break;
                            case ECpShape.SvgRhombus:
                                drawSvg(so);
                                break;
                            default:
                                throw new Exception(String.Format("Error in {0}: Type of shape:{1}",
                                    MethodBase.GetCurrentMethod().Name,
                                    (int)so.Shape));
                        }
                        if (addCenterDot == true)
                        {
                            SolidBrush solidBrushShapeAdd = new SolidBrush(Color.Gray);
                            int iAddSize = 10;
                            graphicsObj.FillEllipse(solidBrushShapeAdd,
                                                    (int)(so.Point.X - (iAddSize / 2.0) + 0.5),
                                                    (int)(so.Point.Y - (iAddSize / 2.0) + 0.5),
                                                    iAddSize, iAddSize);
                        }
                        mPictureBoxOpeImageFromMain.SizeMode = PictureBoxSizeMode.StretchImage;
                        mPictureBoxOpeImageFromSub.SizeMode = PictureBoxSizeMode.Normal; // 元の設定
                                                                                         // mPictureBoxOpeImageFromSub.SizeMode = PictureBoxSizeMode.StretchImage;
                                                                                         // PictureBoxSizeMode.StretchImage: PictureBox 内のイメージのサイズは、PictureBox のサイズに合うように調整されます。
                                                                                         // PictureBoxSizeMode.Zoom: イメージのサイズは、サイズ比率を維持したままで拡大または縮小します。
                        mPictureBoxOpeImageFromMain.Image = mBitmapOpeImageCanvas;
                        mPictureBoxOpeImageFromSub.Image = mBitmapOpeImageCanvas;
                        mPictureBoxOpeImageFromMain.Update();
                        mPictureBoxOpeImageFromSub.Update();
                    }
                }

            }
            catch (Exception ex) { Debug.WriteLine(ex.Message); }
        }
        /// <summary>
        /// Shapeを描画
        /// </summary>
        /// <param name="point">描画位置Point</param>
        /// <param name="addCenterDot">センターにマーキング用ドットを描画するか</param>
        /// <param name="shape">形状</param>
        /// <param name="drawColor">描画色</param>
        private void DrawShape(Point point, bool addCenterDot, ECpShape shape, Color drawColor)
        {
            try
            {
                lock (syncPictureObject)
                {
                    //ImageオブジェクトのGraphicsオブジェクトを作成する
                    //using (Graphics graphicsObj = Graphics.FromImage(mBitmapOpeImageCanvas))
                    Graphics graphicsObj = graphicsObjGlobal;
                    {

                        switch (shape)
                        {
                            case ECpShape.Circle:
                                drawCircle(point, graphicsObj, drawColor);
                                break;
                            case ECpShape.Rectangle:
                                drawRectangle(point, graphicsObj, drawColor);
                                break;
                            case ECpShape.Triangle:
                                drawTriangle(point, graphicsObj, drawColor);
                                break;
                            case ECpShape.Star:
                                drawStar(point, graphicsObj, drawColor);
                                break;
                            case ECpShape.SvgCircle:
                                Debug.WriteLine("Draw Svg.");
                                break;
                            case ECpShape.SvgRectangle:
                                break;
                            default:
                                throw new Exception(String.Format("Error in {0}: Type of shape:{1}",
                                    MethodBase.GetCurrentMethod().Name,
                                    (int)shape));
                        }
                        if (addCenterDot == true)
                        {
                            SolidBrush solidBrushShapeAdd = new SolidBrush(Color.Gray);
                            int iAddSize = 10;
                            graphicsObj.FillEllipse(solidBrushShapeAdd,
                                                    (int)(point.X - (iAddSize / 2.0) + 0.5),
                                                    (int)(point.Y - (iAddSize / 2.0) + 0.5),
                                                    iAddSize, iAddSize);
                        }
                        mPictureBoxOpeImageFromMain.SizeMode = PictureBoxSizeMode.StretchImage;
                        mPictureBoxOpeImageFromSub.SizeMode = PictureBoxSizeMode.Normal; // 元の設定
                                                                                         // mPictureBoxOpeImageFromSub.SizeMode = PictureBoxSizeMode.StretchImage;
                                                                                         // PictureBoxSizeMode.StretchImage: PictureBox 内のイメージのサイズは、PictureBox のサイズに合うように調整されます。
                                                                                         // PictureBoxSizeMode.Zoom: イメージのサイズは、サイズ比率を維持したままで拡大または縮小します。
                        mPictureBoxOpeImageFromMain.Image = mBitmapOpeImageCanvas;
                        mPictureBoxOpeImageFromSub.Image = mBitmapOpeImageCanvas;
                        mPictureBoxOpeImageFromMain.Update();
                        mPictureBoxOpeImageFromSub.Update();
                    }
                }

            }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// 正答point描画
        /// </summary>
        /// <param name="pointToDrawArg"></param>
        /// <param name="boolAddCenterOfDot">中央ドットマーカー</param>
        /// <returns>bool</returns>
        public bool DrawShapeOpeImage(Point pointToDrawArg, bool boolAddCenterOfDot)
        {
            bool boolRet = true;

            try
            {
                DrawShape(pointToDrawArg, boolAddCenterOfDot, CorrectShape, CorrectShapeColor);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
            return boolRet;
        }

        /// <summary>
        /// 正答Point描画 ShapeObject用
        /// </summary>
        /// <param name="so">ShapeObject</param>
        /// <returns>bool</returns>
        public bool DrawShapeOpeImage(ShapeObject so)
        {
            bool boolRet = true;
            try
            {
                DrawShape(so, false);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
            return boolRet;
        }
        /// <summary>
        /// 正答Point描画 ShapeObject用
        /// </summary>
        /// <param name="so">ShapeObject</param>
        /// <returns>bool</returns>
        public bool DrawShapeOpeImage(ShapeObject so, bool makerDot)
        {
            bool boolRet = true;
            try
            {
                DrawShape(so, makerDot);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
            return boolRet;
        }
        /// <summary>
        /// <seealso cref="ShapeObject"/> を描画 
        /// </summary>
        /// <param name="so">ShapeObject</param>
        public void DrawBitmap(ShapeObject so)
        {
            // 取得Bitmapをスケール？ NEWサイズをStep由来にする
            //Bitmap drawBitmap = new Bitmap(LoadImageFromFolder(@"C:\Windows\Web\Screen", "*.jpg"), new Size(OpeImageSizeOfShapeInPixel, OpeImageSizeOfShapeInPixel));
            lock (syncPictureObject)
            {
                //ImageオブジェクトのGraphicsオブジェクトを作成する
                //using (Graphics graphicsObj = Graphics.FromImage(mBitmapOpeImageCanvas))
                Graphics graphicsObj = graphicsObjGlobal;
                {
                    //graphicsObj.DrawImage(drawBitmap, GetBitmapDrawPoint(drawBitmap, so.Point));
                    Bitmap bitmap = ImageLoader.LoadImage(so.ImageFilename, new Size(OpeImageSizeOfShapeInPixel, OpeImageSizeOfShapeInPixel));
                    graphicsObj.DrawImage(bitmap, GetBitmapDrawPoint(bitmap, so.Point));
                }
            }
        }

        /// <summary>
        /// <seealso cref="System.Drawing.Bitmap"/> イメージサイズを考慮した描画座標を取得
        /// </summary>
        /// <param name="bitmap">対象Bitmap</param>
        /// <param name="centerPoint">CenterPoint</param>
        /// <returns>描画 <seealso cref="System.Drawing.Point"/> を返します</returns>
        private Point GetBitmapDrawPoint(Bitmap bitmap, Point centerPoint)
        {
            return (new Point(centerPoint.X - (bitmap.Width / 2), centerPoint.Y - (bitmap.Height / 2)));
        }

        /// <summary>
        /// 不正解形状を描画
        /// </summary>
        /// <param name="pointToDrawArg"></param>
        /// <param name="boolAddCenterOfDot"></param>
        /// <returns></returns>
        public bool DrawWrongShapeOpeImage(Point pointToDrawArg, bool boolAddCenterOfDot)
        {
            bool ret = true;

            try
            {
                DrawShape(pointToDrawArg, boolAddCenterOfDot, WrongShape, WrongShapeColor);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
            return ret;
        }

        /// <summary>
        /// 不正解ポイント描画
        /// </summary>
        /// <param name="pointToDrawArg">描画Point</param>
        /// <param name="boolAddCenterOfDot">中央ドットマーカー</param>
        /// <param name="ecpShape">ECpShape</param>
        /// <param name="color">Color</param>
        /// <returns><seealso cref="bool"/></returns>
        public bool DrawWrongShapeOpeImage(Point pointToDrawArg, bool boolAddCenterOfDot, ECpShape ecpShape, Color color)
        {
            bool ret = true;

            try
            {
                DrawShape(pointToDrawArg, boolAddCenterOfDot, ecpShape, color);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
            return ret;
        }

        /// <summary>
        /// 円を描画
        /// </summary>
        /// <param name="pointToDrawArg">Point</param>
        /// <param name="g">Graphics</param>
        private void drawCircle(Point pointToDrawArg, Graphics graphics, Color color)
        {
            SolidBrush solidBrushShape = new SolidBrush(color);

            graphics.FillEllipse(solidBrushShape,
                        (int)(pointToDrawArg.X - (OpeImageSizeOfShapeInPixel / 2.0) + 0.5),
                        (int)(pointToDrawArg.Y - (OpeImageSizeOfShapeInPixel / 2.0) + 0.5),
                        OpeImageSizeOfShapeInPixel, OpeImageSizeOfShapeInPixel);
        }
        /// <summary>
        /// 四角を描画
        /// </summary>
        /// <param name="pointToDrawArg">Point</param>
        /// <param name="g">Graphics</param>
        private void drawRectangle(Point pointToDrawArg, Graphics graphics, Color color)
        {
            int iHalfOfSide;
            SolidBrush solidBrushShape = new SolidBrush(color);

            iHalfOfSide = (int)(OpeImageSizeOfShapeInPixel * Math.Sin(Math.PI / 4.0) / 2.0 + 0.5);
            graphics.FillRectangle(solidBrushShape,
                                    pointToDrawArg.X - iHalfOfSide,
                                    pointToDrawArg.Y - iHalfOfSide,
                                    2 * iHalfOfSide,
                                    2 * iHalfOfSide);
        }
        /// <summary>
        /// 三角を描画
        /// </summary>
        /// <param name="pointToDrawArg">Point</param>
        /// <param name="g">Graphics</param>
        private void drawTriangle(Point pointToDrawArg, Graphics graphics, Color color)
        {
            int offsetX;
            int offsetY;
            SolidBrush solidBrushShape = new SolidBrush(color);

            offsetX = (int)((OpeImageSizeOfShapeInPixel * Math.Cos(Math.PI / 6.0)) / 2.0 + 0.5);
            offsetY = (int)((OpeImageSizeOfShapeInPixel * Math.Sin(Math.PI / 6.0)) / 2.0 + 0.5);
            Point[] pointTriangle =
            {
                                    new Point(pointToDrawArg.X, pointToDrawArg.Y -(int)(OpeImageSizeOfShapeInPixel/2.0 +0.5)),
                                    new Point(pointToDrawArg.X + offsetX, pointToDrawArg.Y+offsetY),
                                    new Point(pointToDrawArg.X - offsetX, pointToDrawArg.Y+offsetY)
                                };
            graphics.FillPolygon(solidBrushShape,
                                    pointTriangle);
        }
        /// <summary>
        /// 星を描画
        /// </summary>
        /// <param name="pointToDrawArg">Point</param>
        /// <param name="g">Graphics</param>
        private void drawStar(Point pointToDrawArg, Graphics graphics, Color color)
        {
            SolidBrush solidBrushShape = new SolidBrush(color);
            double inRate = 0.39;
            double cos18 = Math.Cos(ConvertFromAngleToRadianOpeImage(18));
            double sin18 = Math.Sin(ConvertFromAngleToRadianOpeImage(18));
            double cos54 = Math.Cos(ConvertFromAngleToRadianOpeImage(54));
            double sin54 = Math.Sin(ConvertFromAngleToRadianOpeImage(54));
            double outHalfSizeOfSahpe = OpeImageSizeOfShapeInPixel / 2.0;
            double inHalfSizeOfSahpe = (OpeImageSizeOfShapeInPixel * inRate) / 2.0;
            double outCos18 = outHalfSizeOfSahpe * cos18;
            double outCos54 = outHalfSizeOfSahpe * cos54;
            double outSin18 = outHalfSizeOfSahpe * sin18;
            double outSin54 = outHalfSizeOfSahpe * sin54;
            double inCos18 = inHalfSizeOfSahpe * cos18;
            double inCos54 = inHalfSizeOfSahpe * cos54;
            double inSin18 = inHalfSizeOfSahpe * sin18;
            double inSin54 = inHalfSizeOfSahpe * sin54;

            Point[] pointStar =
            {
                new Point(pointToDrawArg.X, (int)(pointToDrawArg.Y -outHalfSizeOfSahpe +0.5)),				// Out.1
                new Point((int)(pointToDrawArg.X+ inCos54 +0.5), (int)(pointToDrawArg.Y -inSin54 +0.5)),	// In.1
                new Point((int)(pointToDrawArg.X+ outCos18 +0.5), (int)(pointToDrawArg.Y -outSin18 +0.5)),	// Out.2
                new Point((int)(pointToDrawArg.X+ inCos18 +0.5), (int)(pointToDrawArg.Y +inSin18 +0.5)),	// In.2
                new Point((int)(pointToDrawArg.X+ outCos54 +0.5), (int)(pointToDrawArg.Y +outSin54 +0.5)),	// Out.3
                new Point(pointToDrawArg.X, (int)(pointToDrawArg.Y +inHalfSizeOfSahpe +0.5)),				// In.3
                new Point((int)(pointToDrawArg.X- outCos54 +0.5), (int)(pointToDrawArg.Y +outSin54 +0.5)),	// Out.4
                new Point((int)(pointToDrawArg.X- inCos18 +0.5), (int)(pointToDrawArg.Y +inSin18 +0.5)),	// In.4
                new Point((int)(pointToDrawArg.X- outCos18 +0.5), (int)(pointToDrawArg.Y -outSin18 +0.5)),	// Out.5
                new Point((int)(pointToDrawArg.X- inCos54 +0.5), (int)(pointToDrawArg.Y -inSin54 +0.5))	// In.5
            };
            graphics.FillPolygon(solidBrushShape,
                                    pointStar);
        }

        /// <summary>
        /// 背景色描画
        /// </summary>
        /// <returns>bool</returns>
        public bool DrawBackColor()
        {
            bool boolRet = true;
            try
            {
                lock (syncPictureObject)
                {

                    //ImageオブジェクトのGraphicsオブジェクトを作成する
                    //using (Graphics graphicsObj = Graphics.FromImage(mBitmapOpeImageCanvas))
                    Graphics graphicsObj = graphicsObjGlobal;
                    {
                        graphicsObj.Clear(ColorOpeImageBackColor);
                        mPictureBoxOpeImageFromMain.SizeMode = PictureBoxSizeMode.StretchImage;
                        mPictureBoxOpeImageFromSub.SizeMode = PictureBoxSizeMode.StretchImage;
                        // PictureBoxSizeMode.StretchImage: PictureBox 内のイメージのサイズは、PictureBox のサイズに合うように調整されます。
                        // PictureBoxSizeMode.Zoom: イメージのサイズは、サイズ比率を維持したままで拡大または縮小します。
                        mPictureBoxOpeImageFromMain.Image = mBitmapOpeImageCanvas;
                        mPictureBoxOpeImageFromSub.Image = mBitmapOpeImageCanvas;
                        mPictureBoxOpeImageFromMain.Update();
                        mPictureBoxOpeImageFromSub.Update();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
            return boolRet;
        }

        /// <summary>
        /// 背景色描画
        /// </summary>
        /// <param name="colorBackColorArg">Color</param>
        /// <returns>bool</returns>
        public bool DrawBackColor(Color colorBackColorArg)
        {
            bool boolRet = true;

            try
            {
                lock (syncPictureObject)
                {

                    //ImageオブジェクトのGraphicsオブジェクトを作成する
                    //using (Graphics graphicsObj = Graphics.FromImage(mBitmapOpeImageCanvas))
                    Graphics graphicsObj = graphicsObjGlobal;
                    {
                        graphicsObj.Clear(colorBackColorArg);
                        mPictureBoxOpeImageFromMain.SizeMode = PictureBoxSizeMode.StretchImage;
                        mPictureBoxOpeImageFromSub.SizeMode = PictureBoxSizeMode.StretchImage;  // 追加
                                                                                                // PictureBoxSizeMode.StretchImage: PictureBox 内のイメージのサイズは、PictureBox のサイズに合うように調整されます。
                                                                                                // PictureBoxSizeMode.Zoom: イメージのサイズは、サイズ比率を維持したままで拡大または縮小します。
                        mPictureBoxOpeImageFromMain.Image = mBitmapOpeImageCanvas;
                        mPictureBoxOpeImageFromSub.Image = mBitmapOpeImageCanvas;
                        mPictureBoxOpeImageFromMain.Update();
                        mPictureBoxOpeImageFromSub.Update();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
            return boolRet;
        }

        /// <summary>
        /// 画像描画
        /// </summary>
        /// <param name="stringFileNameArg">ファイル名</param>
        /// <returns>bool</returns>
        public bool DrawImage(string stringFileNameArg)
        {
            bool boolRet = true;
            Bitmap bitmapObj;

            if (!System.IO.File.Exists(stringFileNameArg))
            {
                return false;
            }
            try
            {
                bitmapObj = new Bitmap(System.Drawing.Image.FromFile(stringFileNameArg));
                {
                    // ファイルを読み込んで、Bitmap型にキャスト
                    mPictureBoxOpeImageFromMain.SizeMode = PictureBoxSizeMode.StretchImage;
                    mPictureBoxOpeImageFromSub.SizeMode = PictureBoxSizeMode.StretchImage;
                    mPictureBoxOpeImageFromMain.Image = bitmapObj;
                    mPictureBoxOpeImageFromSub.Image = bitmapObj;
                    mPictureBoxOpeImageFromMain.Update();
                    mPictureBoxOpeImageFromSub.Update();
                }
            }
            catch (OutOfMemoryException ex)
            {
                // ファイル形式無効な場合
                Debug.WriteLine(ex.Message);
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
            return boolRet;
        }

        /// <summary>
        /// 画像描画 <seealso cref="Bitmap"/>
        /// </summary>
        /// <param name="bmp">ビットマップ</param>
        /// <returns><seealso cref="bool"/></returns>
        public bool DrawImage(Bitmap bmp)
        {
            bool boolRet = true;
            try
            {
                {
                    mPictureBoxOpeImageFromMain.SizeMode = PictureBoxSizeMode.StretchImage;
                    mPictureBoxOpeImageFromSub.SizeMode = PictureBoxSizeMode.StretchImage;
                    mPictureBoxOpeImageFromMain.Image = bmp;
                    mPictureBoxOpeImageFromSub.Image = bmp;
                    mPictureBoxOpeImageFromMain.Update();
                    mPictureBoxOpeImageFromSub.Update();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
            return boolRet;
        }
        public void DrawImage(Bitmap bmp, Point point)
        {
            graphicsObjGlobal.DrawImage(bmp, point);
        }
        /// <summary>
        /// 指定フォルダからランダム画像抽出
        /// </summary>
        /// <param name="path">パス</param>
        /// <param name="extension">拡張子</param>
        /// <returns><seealso cref="System.Drawing.Bitmap"/></returns>
        public static Bitmap LoadImageFromFolder(string path, string extension)
        {
            if (System.IO.Directory.Exists(path))
            {
                Random rnd = new Random();
                string[] fileList = System.IO.Directory.GetFiles(path, extension);
                if (fileList.Length != 0)
                    return new Bitmap(fileList[rnd.Next(0, fileList.Length - 1)]);
                else
                    return null;
            }
            else
                return null;
        }
        private Bitmap GetBitmapFromEcpShape(ECpShape eCpShape)
        {
            string targetFolder = @".\svg\";
            string imagePath = "";
            Graphics graphicsObj = graphicsObjGlobal;
            switch (eCpShape)
            {
                case ECpShape.SvgSquare:
                    imagePath = targetFolder + svgfileSquare;
                    break;
                case ECpShape.SvgCircle:
                    imagePath = targetFolder + svgfileCircle;
                    break;
                case ECpShape.SvgTriangle:
                    imagePath = targetFolder + svgfileTriangle;
                    break;
                case ECpShape.SvgRectangle:
                    imagePath = targetFolder + svgfileRectangle;
                    break;
                case ECpShape.SvgScalene:
                    imagePath = targetFolder + svgfileScalene;
                    break;
                case ECpShape.SvgPentagon:
                    imagePath = targetFolder + svgfilePentagon;
                    break;
                case ECpShape.SvgStar:
                    imagePath = targetFolder + svgfileStar;
                    break;
                case ECpShape.SvgRight:
                    imagePath = targetFolder + svgfileRight;
                    break;
                case ECpShape.SvgHeart:
                    imagePath = targetFolder + svgfileHeart;
                    break;
                case ECpShape.SvgHexagon:
                    imagePath = targetFolder + svgfileHexagon;
                    break;
                case ECpShape.SvgOctagon:
                    imagePath = targetFolder + svgfileOctagon;
                    break;
                case ECpShape.SvgTrefoil:
                    imagePath = targetFolder + svgfileTrefoil;
                    break;
                case ECpShape.SvgKite:
                    imagePath = targetFolder + svgfileKite;
                    break;
                case ECpShape.SvgPic:
                    imagePath = targetFolder + svgfilePic;
                    break;
                case ECpShape.SvgEllipse:
                    imagePath = targetFolder + svgfileEllipse;
                    break;
                case ECpShape.SvgCresent:
                    imagePath = targetFolder + svgfileCresent;
                    break;
                case ECpShape.SvgTrapeze:
                    imagePath = targetFolder + svgfileTrapeze;
                    break;
                case ECpShape.SvgPolygon:
                    imagePath = targetFolder + svgfilePolygon;
                    break;
                case ECpShape.SvgParallelogram:
                    imagePath = targetFolder + svgfileParallelogram;
                    break;
                case ECpShape.SvgSemiCircle:
                    imagePath = targetFolder + svgfileSemiCircle;
                    break;
                case ECpShape.SvgCross:
                    imagePath = targetFolder + svgfileCross;
                    break;
                case ECpShape.SvgRing:
                    imagePath = targetFolder + svgfileRing;
                    break;
                case ECpShape.SvgArrow:
                    imagePath = targetFolder + svgfileArrow;
                    break;
                case ECpShape.SvgQuadrefoil:
                    imagePath = targetFolder + svgfileQuadrefoil;
                    break;
                case ECpShape.SvgRhombus:
                    imagePath = targetFolder + svgfileRhombus;
                    break;
                default:
                    imagePath = targetFolder + svgfileCircle;
                    break;
            }
            Bitmap bitmap;
            ShapeObject so = new ShapeObject();
            if (Path.GetExtension(imagePath) == ".svg")
            {
                bitmap = LoadSvgImage(imagePath, OpeImageSizeOfShapeInPixel, OpeImageSizeOfShapeInPixel, Color.Black, ref so);
            }
            else
            {
                bitmap = ImageLoader.LoadImage(imagePath, new Size(OpeImageSizeOfShapeInPixel, OpeImageSizeOfShapeInPixel));
            }
            return bitmap;
        }

        private void drawSvg(ShapeObject so)
        {
            SolidBrush solidBrushShape = new SolidBrush(so.ShapeColor);

            string targetFolder = @".\svg\";

            Graphics graphicsObj = graphicsObjGlobal;
            switch (so.Shape)
            {
                case ECpShape.SvgSquare:
                    so.ImageFilename = targetFolder + svgfileSquare;
                    break;
                case ECpShape.SvgCircle:
                    so.ImageFilename = targetFolder + svgfileCircle;
                    break;
                case ECpShape.SvgTriangle:
                    so.ImageFilename = targetFolder + svgfileTriangle;
                    break;
                case ECpShape.SvgRectangle:
                    so.ImageFilename = targetFolder + svgfileRectangle;
                    break;
                case ECpShape.SvgScalene:
                    so.ImageFilename = targetFolder + svgfileScalene;
                    break;
                case ECpShape.SvgPentagon:
                    so.ImageFilename = targetFolder + svgfilePentagon;
                    break;
                case ECpShape.SvgStar:
                    so.ImageFilename = targetFolder + svgfileStar;
                    break;
                case ECpShape.SvgRight:
                    so.ImageFilename = targetFolder + svgfileRight;
                    break;
                case ECpShape.SvgHeart:
                    so.ImageFilename = targetFolder + svgfileHeart;
                    break;
                case ECpShape.SvgHexagon:
                    so.ImageFilename = targetFolder + svgfileHexagon;
                    break;
                case ECpShape.SvgOctagon:
                    so.ImageFilename = targetFolder + svgfileOctagon;
                    break;
                case ECpShape.SvgTrefoil:
                    so.ImageFilename = targetFolder + svgfileTrefoil;
                    break;
                case ECpShape.SvgKite:
                    so.ImageFilename = targetFolder + svgfileKite;
                    break;
                case ECpShape.SvgPic:
                    so.ImageFilename = targetFolder + svgfilePic;
                    break;
                case ECpShape.SvgEllipse:
                    so.ImageFilename = targetFolder + svgfileEllipse;
                    break;
                case ECpShape.SvgCresent:
                    so.ImageFilename = targetFolder + svgfileCresent;
                    break;
                case ECpShape.SvgTrapeze:
                    so.ImageFilename = targetFolder + svgfileTrapeze;
                    break;
                case ECpShape.SvgPolygon:
                    so.ImageFilename = targetFolder + svgfilePolygon;
                    break;
                case ECpShape.SvgParallelogram:
                    so.ImageFilename = targetFolder + svgfileParallelogram;
                    break;
                case ECpShape.SvgSemiCircle:
                    so.ImageFilename = targetFolder + svgfileSemiCircle;
                    break;
                case ECpShape.SvgCross:
                    so.ImageFilename = targetFolder + svgfileCross;
                    break;
                case ECpShape.SvgRing:
                    so.ImageFilename = targetFolder + svgfileRing;
                    break;
                case ECpShape.SvgArrow:
                    so.ImageFilename = targetFolder + svgfileArrow;
                    break;
                case ECpShape.SvgQuadrefoil:
                    so.ImageFilename = targetFolder + svgfileQuadrefoil;
                    break;
                case ECpShape.SvgRhombus:
                    so.ImageFilename = targetFolder + svgfileRhombus;
                    break;
                default:
                    so.ImageFilename = targetFolder + svgfileCircle;
                    break;
            }

            {
                //graphicsObj.DrawImage(drawBitmap, GetBitmapDrawPoint(drawBitmap, so.Point));
                Bitmap bitmap;
                if (Path.GetExtension(so.ImageFilename) == ".svg")
                {
                    bitmap = LoadSvgImage(so.ImageFilename, OpeImageSizeOfShapeInPixel, OpeImageSizeOfShapeInPixel, so.ShapeColor, ref so);
                }
                else
                {
                    bitmap = ImageLoader.LoadImage(so.ImageFilename, new Size(OpeImageSizeOfShapeInPixel, OpeImageSizeOfShapeInPixel));
                }
                graphicsObj.DrawImage(bitmap, GetBitmapDrawPoint(bitmap, so.Point));
                //if (so.ShapeGraphicsPath != null)
                {
                    // SVG形状タッチ有効Path
                    //MakePathOfSvg(ShapeOpeImageCorrectShape);

                    //ShapeOpeImageCorrectShape nullのことがある
                    //タッチパス
                    if (ShapeOpeImageCorrectShape is null)
                    {
                        ShapeOpeImageCorrectShape = new ShapeObject();
                        ShapeOpeImageCorrectShape.Point = so.Point;
                    }

                    // Train時は違う座標参照してしまうのでMake側でCorrectにコピーしている
                    MakePathOfCorrectShapeOpeImage(ShapeOpeImageCorrectShape.Point);

                    //Matrix translateMatrix = new Matrix();
                    //translateMatrix.Translate(ShapeOpeImageCorrectShape.Point.X - 30, ShapeOpeImageCorrectShape.Point.Y - 30);
                    //translateMatrix.Scale(2.1f, 2.1f);
                    //ShapeOpeImageCorrectShape.ShapeGraphicsPath.Transform(translateMatrix);
                    //graphicsObj.DrawPath(Pens.Yellow, ShapeOpeImageCorrectShape.ShapeGraphicsPath);
#if DEBUG_OPEIMAGE
                    graphicsObj.DrawPath(new Pen(Color.Yellow,1.0f), mGraphicsPathOpeImageShape);
#endif

                }
            }
        }
        /// <summary>
        /// SVGイメージ
        /// </summary>
        /// <param name="path"></param>
        /// <param name="renderX"></param>
        /// <param name="renderY"></param>
        /// <returns></returns>
        public Bitmap LoadSvgImage(string path, int renderX, int renderY, Color color, ref ShapeObject so)
        {
            if (System.IO.File.Exists(path))
            {
                var svgDoc = SvgDocument.Open(path);
                if (svgDoc != null)
                {

                    processNodes(svgDoc.Descendants(), new SvgColourServer(color));
                    //so.ShapeGraphicsPath = new GraphicsPath();
                    //so.ShapeGraphicsPath = (GraphicsPath)svgDoc.Path.Clone();

                    return new Bitmap(svgDoc.Draw(renderX, renderY));
                }
                else
                {
                    //so.ShapeGraphicsPath = new GraphicsPath();
                    return null;
                }
            }
            else
            {
                //so.ShapeGraphicsPath = new GraphicsPath();
                return null;
            }
        }

        public Bitmap LoadSvgImage(string path, int renderX, int renderY, Color color)
        {
            ShapeObject so = new ShapeObject();
            return LoadSvgImage(path, renderX, renderY, color, ref so);
        }

        private void processNodes(IEnumerable<SvgElement> nodes, SvgPaintServer colorServer)
        {
            foreach (var node in nodes)
            {
                if (node.Fill != SvgPaintServer.None) node.Fill = colorServer;
                if (node.Color != SvgPaintServer.None) node.Color = colorServer;
                if (node.Stroke != SvgPaintServer.None) node.Stroke = colorServer;
                processNodes(node.Descendants(), colorServer);
            }
        }
        public static ECpShape GetStringToEcpShape(string str)
        {
            return Enum.GetValues(typeof(ECpShape)).Cast<ECpShape>().First(e => e.ToString().Contains(str));
        }
        public static List<ECpShape> CheckUnExistSvgShape()
        {
            List<ECpShape> result = new List<ECpShape>();
            //string targetFolder = @".\svg\";


            if (Directory.Exists(targetFolder))
            {
                if (!File.Exists(targetFolder + "\\" + svgfileCircle))
                {
                    result.Add(GetStringToEcpShape("SvgCircle"));
                }
                if (!File.Exists(targetFolder + "\\" + svgfileRectangle))
                {
                    result.Add(GetStringToEcpShape("SvgRectangle"));
                }
                if (!File.Exists(targetFolder + "\\" + svgfileSquare))
                {
                    result.Add(GetStringToEcpShape("SvgSquare"));
                }
                if (!File.Exists(targetFolder + "\\" + svgfileTriangle))
                {
                    result.Add(GetStringToEcpShape("SvgTriangle"));
                }
                if (!File.Exists(targetFolder + "\\" + svgfileScalene))
                {
                    result.Add(GetStringToEcpShape("SvgScalene"));
                }
                if (!File.Exists(targetFolder + "\\" + svgfilePentagon))
                {
                    result.Add(GetStringToEcpShape("SvgPentagon"));
                }
                if (!File.Exists(targetFolder + "\\" + svgfileRight))
                {
                    result.Add(GetStringToEcpShape("SvgRight"));
                }
                if (!File.Exists(targetFolder + "\\" + svgfileTrapeze))
                {
                    result.Add(GetStringToEcpShape("SvgTrapeze"));
                }
                if (!File.Exists(targetFolder + "\\" + svgfileKite))
                {
                    result.Add(GetStringToEcpShape("SvgKite"));
                }
                if (!File.Exists(targetFolder + "\\" + svgfilePolygon))
                {
                    result.Add(GetStringToEcpShape("SvgPolygon"));
                }
                if (!File.Exists(targetFolder + "\\" + svgfileParallelogram))
                {
                    result.Add(GetStringToEcpShape("SvgParallelogram"));
                }
                if (!File.Exists(targetFolder + "\\" + svgfileEllipse))
                {
                    result.Add(GetStringToEcpShape("SvgEllipse"));
                }
                if (!File.Exists(targetFolder + "\\" + svgfileTrefoil))
                {
                    result.Add(GetStringToEcpShape("SvgTrefoil"));
                }
                if (!File.Exists(targetFolder + "\\" + svgfileStar))
                {
                    result.Add(GetStringToEcpShape("SvgStar"));
                }
                if (!File.Exists(targetFolder + "\\" + svgfileSemiCircle))
                {
                    result.Add(GetStringToEcpShape("SvgSemiCircle"));
                }
                if (!File.Exists(targetFolder + "\\" + svgfileHexagon))
                {
                    result.Add(GetStringToEcpShape("SvgHexagon"));
                }
                if (!File.Exists(targetFolder + "\\" + svgfileCresent))
                {
                    result.Add(GetStringToEcpShape("SvgCresent"));
                }
                if (!File.Exists(targetFolder + "\\" + svgfileOctagon))
                {
                    result.Add(GetStringToEcpShape("SvgOctagon"));
                }
                if (!File.Exists(targetFolder + "\\" + svgfileCross))
                {
                    result.Add(GetStringToEcpShape("SvgCross"));
                }
                if (!File.Exists(targetFolder + "\\" + svgfileRing))
                {
                    result.Add(GetStringToEcpShape("SvgRing"));
                }
                if (!File.Exists(targetFolder + "\\" + svgfilePic))
                {
                    result.Add(GetStringToEcpShape("SvgPic"));
                }
                if (!File.Exists(targetFolder + "\\" + svgfileHeart))
                {
                    result.Add(GetStringToEcpShape("SvgHeart"));
                }
                if (!File.Exists(targetFolder + "\\" + svgfileArrow))
                {
                    result.Add(GetStringToEcpShape("SvgArrow"));
                }
                if (!File.Exists(targetFolder + "\\" + svgfileQuadrefoil))
                {
                    result.Add(GetStringToEcpShape("SvgQuadrefoil"));
                }
                if (!File.Exists(targetFolder + "\\" + svgfileRhombus))
                {
                    result.Add(GetStringToEcpShape("SvgRhombus"));
                }
            }
            else
            {
                result = Enum.GetValues(typeof(ECpShape)).Cast<ECpShape>().Where(e => e.ToString().Contains("Svg")).ToList();
            }
            return result;
        }

        public static List<ECpShape> CheckExistSvgShape()
        {
            List<ECpShape> result = new List<ECpShape>();
            result = Enum.GetValues(typeof(ECpShape)).Cast<ECpShape>().Where(e => e.ToString().Contains("Svg")).ToList();
            List<ECpShape> unresult = CheckUnExistSvgShape();

            result = result.Where(e => !unresult.Contains(e)).ToList();
            return result;
        }
        public static bool CheckExistSvgShape(ECpShape eCpShape)
        {
            return CheckExistSvgShape().Exists(x => x == eCpShape);
        }
        public static bool CheckSvgImage()
        {
            //string targetFolder = @".\svg\";

            //string svgfileCircle = "circle.svg";
            //string svgfileRectangle = "rectangle.svg";
            //string svgfileSquare = "square.svg";
            //string svgfileTriangle = "triangle.svg";
            //string svgfileScalene = "scalene.svg";
            //string svgfilePentagon = "pentagon.svg";
            //string svgfileRight = "right.svg";
            //string svgfileTrapeze = "trapeze.svg";
            //string svgfileKite = "kite.svg";
            //string svgfilePolygon = "polygon.svg";
            //string svgfileParallelogram = "parallelogram.svg";
            //string svgfileEllipse = "ellipse.svg";
            //string svgfileTrefoil = "trefoil.svg";
            //string svgfileStar = "star.svg";
            //string svgfileSemicircle = "semicircle.svg";
            //string svgfileHexagon = "hexagon.svg";
            //string svgfileCresent = "cresent.svg";
            //string svgfileOctagon = "octagon.svg";
            //string svgfileCross = "cross.svg";
            //string svgfileRing = "ring.svg";
            //string svgfilePic = "pic.svg";
            //string svgfileHeart = "heart.svg";
            //string svgfileArrow = "arrow.svg";
            //string svgfileQuatrefoil = "quadrefoil.svg";
            //string svgfileRhombus = "rhombus.svg";

            bool ret = true;
            if (Directory.Exists(targetFolder))
            {
                if (!File.Exists(targetFolder + "\\" + svgfileCircle))
                {
                    ret = false;
                }
                if (!File.Exists(targetFolder + "\\" + svgfileRectangle))
                {
                    ret = false;
                }
                if (!File.Exists(targetFolder + "\\" + svgfileSquare))
                {
                    ret = false;
                }
                if (!File.Exists(targetFolder + "\\" + svgfileTriangle))
                {
                    ret = false;
                }
                if (!File.Exists(targetFolder + "\\" + svgfileScalene))
                {
                    ret = false;
                }
                if (!File.Exists(targetFolder + "\\" + svgfilePentagon))
                {
                    ret = false;
                }
                if (!File.Exists(targetFolder + "\\" + svgfileRight))
                {
                    ret = false;
                }
                if (!File.Exists(targetFolder + "\\" + svgfileTrapeze))
                {
                    ret = false;
                }
                if (!File.Exists(targetFolder + "\\" + svgfileKite))
                {
                    ret = false;
                }
                if (!File.Exists(targetFolder + "\\" + svgfilePolygon))
                {
                    ret = false;
                }
                if (!File.Exists(targetFolder + "\\" + svgfileParallelogram))
                {
                    ret = false;
                }
                if (!File.Exists(targetFolder + "\\" + svgfileEllipse))
                {
                    ret = false;
                }
                if (!File.Exists(targetFolder + "\\" + svgfileTrefoil))
                {
                    ret = false;
                }
                if (!File.Exists(targetFolder + "\\" + svgfileStar))
                {
                    ret = false;
                }
                if (!File.Exists(targetFolder + "\\" + svgfileSemiCircle))
                {
                    ret = false;
                }
                if (!File.Exists(targetFolder + "\\" + svgfileHexagon))
                {
                    ret = false;
                }
                if (!File.Exists(targetFolder + "\\" + svgfileCresent))
                {
                    ret = false;
                }
                if (!File.Exists(targetFolder + "\\" + svgfileOctagon))
                {
                    ret = false;
                }
                if (!File.Exists(targetFolder + "\\" + svgfileCross))
                {
                    ret = false;
                }
                if (!File.Exists(targetFolder + "\\" + svgfileRing))
                {
                    ret = false;
                }
                if (!File.Exists(targetFolder + "\\" + svgfilePic))
                {
                    ret = false;
                }
                if (!File.Exists(targetFolder + "\\" + svgfileHeart))
                {
                    ret = false;
                }
                if (!File.Exists(targetFolder + "\\" + svgfileArrow))
                {
                    ret = false;
                }
                if (!File.Exists(targetFolder + "\\" + svgfileQuadrefoil))
                {
                    ret = false;
                }
                if (!File.Exists(targetFolder + "\\" + svgfileRhombus))
                {
                    ret = false;
                }
            }
            else
            {
                Directory.CreateDirectory(targetFolder);

                ret = false;
            }

            return ret;
        }
        public void DrawMarker(int position)
        {
            const int shapeOffset = 10;
            // ShapeSizeの1/3
            int drawWidth = OpeImageSizeOfShapeInPixel / 3;
            int drawHeight = OpeImageSizeOfShapeInPixel / 3;
            if (!System.IO.File.Exists(preferencesDatOriginal.TestIndicatorSvgPath))
            {
                preferencesDatOriginal.TestIndicatorSvgPath = targetFolder + "\\" + svgfileTrefoil;
            }
            Bitmap bmp = LoadSvgImage(preferencesDatOriginal.TestIndicatorSvgPath, drawWidth, drawHeight, Color.YellowGreen);

            switch (position)
            {
                // 右下
                case 0:
                    DrawImage(bmp, new Point(WidthOfWholeArea - drawWidth - shapeOffset, HeightOfWholeArea - drawHeight - shapeOffset));
                    break;
                // 右上
                case 1:
                    DrawImage(bmp, new Point(WidthOfWholeArea - drawWidth - shapeOffset, shapeOffset));
                    break;
                // 左下
                case 2:
                    DrawImage(bmp, new Point(shapeOffset, HeightOfWholeArea - drawHeight - shapeOffset));
                    break;
                // 左上
                case 3:
                    DrawImage(bmp, new Point(shapeOffset, shapeOffset));
                    break;
                default:
                    DrawImage(bmp, new Point(WidthOfWholeArea - drawWidth - shapeOffset, HeightOfWholeArea - drawHeight - shapeOffset));
                    break;

            }
            //描画更新
            UpdateCanvas();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool CheckDrawSpace()
        {
            var ret = false;

            var drawAllSpace = RectOpeImageValidArea.Width * RectOpeImageValidArea.Height;
            var requierDrawSphapeSpace = Math.Pow((OpeImageSizeOfShapeInPixel) / 2, 2) * Math.PI * (preferencesDatOriginal.IncorrectNum + 1);

            if (drawAllSpace > requierDrawSphapeSpace)
            {
                ret = true;
            }
            Debug.WriteLine(drawAllSpace.ToString());
            Debug.WriteLine(requierDrawSphapeSpace.ToString());
            return ret;
        }
    }
}
