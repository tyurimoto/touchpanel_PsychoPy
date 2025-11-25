using System;
using System.Drawing;
using System.Windows.Forms;

namespace Compartment
{
    class DevTouchPanel
    {
    }
    public partial class FormMain : Form
    {
        private void CallbackInitFordevTouchPanel()
        {
            // devTouchPanel: 初期化
            devTouchPanel.devStateVal = EDevState.TouchPanelInit;
            devTouchPanel.devStateValSaved = EDevState.OverRange;
            devTouchPanel.devCmdVal = EDevCmd.None;
            devTouchPanel.devResultVal = EDevResult.None;
            devTouchPanel.devResultValSaved = EDevResult.OverRange;
            devTouchPanel.firstFlag = true;
        }
        //		public Action<int> callbackExeForDevLever = (a_iInArg) =>
        private void CallbackExeFordevTouchPanel()
        {
            bool boolResult;
            bool boolClearQueue = false;
            bool touchIncorrectResult = false;
            Point pointTouchPoint;
            String stringMsg;
            DevCmdPkt devCmdPktLast = null;
            DevCmdPkt devCmdPktCur;
            try
            {

                // コマンド受信: 最後のコマンドが有効
                while (concurrentQueueDevCmdPktTouchPanel.TryDequeue(out devCmdPktCur))
                {
                    devCmdPktLast = devCmdPktCur;
                }

                if (devCmdPktLast != null)
                {
                    switch (devCmdPktLast.DevCmdVal)
                    {
                        case EDevCmd.TouchPanelStop:
                            devTouchPanel.devStateVal = EDevState.TouchPanelStop;
                            break;
                        case EDevCmd.TouchPanelTouchAny:
                            devTouchPanel.devStateVal = EDevState.TouchPanelTouchAny;
                            // キュー: クリア
                            boolClearQueue = true;
                            break;
                        case EDevCmd.TouchPanelCorrectShape:
                            devTouchPanel.devStateVal = EDevState.TouchPanelCorrectShape;
                            // キュー: クリア
                            boolClearQueue = true;
                            break;
                        case EDevCmd.TouchPanelCorrectShapeAny:
                            devTouchPanel.devStateVal = EDevState.TouchPanelCorrectShapeAny;
                            // キュー: クリア
                            boolClearQueue = true;
                            break;
                        default:
                            break;
                    }
                }
                // キュー: クリア
                if (boolClearQueue == true)
                {
                    while (concurrentQueueFromTouchPanel.TryDequeue(out pointTouchPoint))
                    {
                    }
                }
                switch (devTouchPanel.devStateVal)
                {
                    case EDevState.TouchPanelInit:
                        devTouchPanel.devStateVal = EDevState.TouchPanelStop;
                        break;
                    case EDevState.TouchPanelStop:
                        // キュー: クリア
                        while (concurrentQueueFromTouchPanel.TryDequeue(out pointTouchPoint))
                        {
                        }
                        break;
                    case EDevState.TouchPanelTouchAny:
                        while (concurrentQueueFromTouchPanel.TryDequeue(out pointTouchPoint))
                        {
                            if ((pointTouchPoint.X >= opImage.RectOpeImageValidArea.X) &&
                                (pointTouchPoint.X < (opImage.RectOpeImageValidArea.X + opImage.RectOpeImageValidArea.Width)) &&
                                (pointTouchPoint.Y >= opImage.RectOpeImageValidArea.Y) &&
                                (pointTouchPoint.Y < opImage.RectOpeImageValidArea.Y + opImage.RectOpeImageValidArea.Height))
                            {
                                boolResult = true;
                                // Operationステート・マシンへ結果を出力
                                OpFlagTouchAnyOnTouchPanel = true;
                            }
                            else
                            {
                                boolResult = false;
                            }
                            if (opeModeTypeVal == EOpeModeType.CheckDevice)
                            {
                                stringMsg = string.Format("X:{0} Y:{1} Result:{2} {3}" + Environment.NewLine,
                                            pointTouchPoint.X, pointTouchPoint.Y,
                                            boolResult == true ? "Ok" : "Ng",
                                            touchIncorrectResult == true ? "不正解タッチ" : "");
                                Invoke((MethodInvoker)(() =>
                                {
                                    userControlCheckDeviceOnFormMain.textBoxResultOnGpTouchPanelOnUcCheckDevice.AppendText(stringMsg);
                                }));
                            }
                        }
                        break;
                    case EDevState.TouchPanelCorrectShape:
                        while (concurrentQueueFromTouchPanel.TryDequeue(out pointTouchPoint))
                        {
                            opCollection.TouchPoint = pointTouchPoint;
                            if ((opImage.mGraphicsPathOpeImageShape != null) &&
                               (opImage.mGraphicsPathOpeImageShape.IsVisible(pointTouchPoint.X, pointTouchPoint.Y) == true))
                            {
                                boolResult = true;
                                // Operationステート・マシンへ結果を出力
                                OpFlagTouchCorrectOnTouchPanel = true;
                            }
                            else if ((opImage.mGraphicsPathOpeIncorrectImageShape != null) &&
                                 (opImage.mGraphicsPathOpeIncorrectImageShape.IsVisible(pointTouchPoint.X, pointTouchPoint.Y) == true))
                            {
                                // NGオブジェクトが明示的に押された
                                boolResult = false;
                                touchIncorrectResult = true;
                                // Operationステート・マシンへ結果を出力
                                OpFlagTouchIncorrectOnTouchPanel = true;
                            }
                            else
                            {
                                boolResult = false;
                            }
                            if (opeModeTypeVal == EOpeModeType.CheckDevice)
                            {
                                stringMsg = string.Format("X:{0} Y:{1} Result:{2} {3}" + Environment.NewLine,
                                            pointTouchPoint.X, pointTouchPoint.Y,
                                            boolResult == true ? "Ok" : "Ng",
                                            touchIncorrectResult == true ? "不正解タッチ" : "");
                                Invoke((MethodInvoker)(() =>
                                {
                                    userControlCheckDeviceOnFormMain.textBoxResultOnGpTouchPanelOnUcCheckDevice.AppendText(stringMsg);
                                }));
                            }
                        }
                        break;
                    case EDevState.TouchPanelCorrectShapeAny:
                        boolResult = false;
                        string ShapeObjecetName = "";
                        while (concurrentQueueFromTouchPanel.TryDequeue(out pointTouchPoint))
                        {
                            opCollection.TouchPoint = pointTouchPoint;

                            if (opImage.TouchAnyShapes != null)
                            {
                                foreach (var so in opImage.TouchAnyShapes)
                                {
                                    so.CheckTouch(pointTouchPoint);
                                    if (so.Touched)
                                    {
                                        //該当図形座標
                                        boolResult = true;
                                        // Operationステート・マシンへ結果を出力
                                        OpFlagTouchCorrectOnTouchPanel = true;
                                        ShapeObjecetName = so.Shape.ToString();
                                    }
                                }
                            }
                            if (opeModeTypeVal == EOpeModeType.CheckDevice)
                            {
                                stringMsg = string.Format("X:{0} Y:{1} Result:{2} {3}" + Environment.NewLine,
                                            pointTouchPoint.X, pointTouchPoint.Y,
                                            boolResult == true ? "Ok" : "Ng",
                                            ": " + ShapeObjecetName);
                                Invoke((MethodInvoker)(() =>
                                {
                                    userControlCheckDeviceOnFormMain.textBoxResultOnGpTouchPanelOnUcCheckDevice.AppendText(stringMsg);
                                }));
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
