using System.Drawing;
using System.Windows.Forms;


namespace Compartment
{
    public class DevExt
    {
    }
    public partial class FormMain : Form
    {
        public void CallbackEDoorStatus()
        {
            // 変化した時、表示更新
            {
                if (!eDoor.CwLim)
                {
                    Invoke((MethodInvoker)(() =>
                    {
                        userControlCheckDeviceOnFormMain.textBoxEDoorCWOnCheckDevice.Text = "ON";
                        userControlCheckDeviceOnFormMain.textBoxEDoorCWOnCheckDevice.BackColor = Color.Red;
                        userControlCheckDeviceOnFormMain.textBoxEDoorCWOnCheckDevice.ForeColor = Color.White;
                    }));
                }
                else
                {
                    Invoke((MethodInvoker)(() =>
                    {
                        userControlCheckDeviceOnFormMain.textBoxEDoorCWOnCheckDevice.Text = "OFF";
                        userControlCheckDeviceOnFormMain.textBoxEDoorCWOnCheckDevice.BackColor = Color.Black;
                        userControlCheckDeviceOnFormMain.textBoxEDoorCWOnCheckDevice.ForeColor = Color.White;
                    }));
                }
            }

            {
                if (!eDoor.CCwLim)
                {
                    Invoke((MethodInvoker)(() =>
                    {
                        userControlCheckDeviceOnFormMain.textBoxEDoorCCWOnCheckDevice.Text = "ON";
                        userControlCheckDeviceOnFormMain.textBoxEDoorCCWOnCheckDevice.BackColor = Color.Red;
                        userControlCheckDeviceOnFormMain.textBoxEDoorCCWOnCheckDevice.ForeColor = Color.White;
                    }));
                }
                else
                {
                    Invoke((MethodInvoker)(() =>
                    {
                        userControlCheckDeviceOnFormMain.textBoxEDoorCCWOnCheckDevice.Text = "OFF";
                        userControlCheckDeviceOnFormMain.textBoxEDoorCCWOnCheckDevice.BackColor = Color.Black;
                        userControlCheckDeviceOnFormMain.textBoxEDoorCCWOnCheckDevice.ForeColor = Color.White;
                    }));
                }
            }

            {
                if (!eDoor.OutDirection)
                {
                    Invoke((MethodInvoker)(() =>
                    {
                        userControlCheckDeviceOnFormMain.textBoxEDoorOutDirection.Text = "Out DIR";
                        userControlCheckDeviceOnFormMain.textBoxEDoorOutDirection.BackColor = Color.Red;
                        userControlCheckDeviceOnFormMain.textBoxEDoorOutDirection.ForeColor = Color.White;
                    }));
                }
                else
                {
                    Invoke((MethodInvoker)(() =>
                    {
                        userControlCheckDeviceOnFormMain.textBoxEDoorOutDirection.Text = "Out DIR";
                        userControlCheckDeviceOnFormMain.textBoxEDoorOutDirection.BackColor = Color.Black;
                        userControlCheckDeviceOnFormMain.textBoxEDoorOutDirection.ForeColor = Color.White;
                    }));
                }

                {
                    if (!eDoor.InDirection)
                    {
                        Invoke((MethodInvoker)(() =>
                        {
                            userControlCheckDeviceOnFormMain.textBoxEDoorInDirection.Text = "In DIR";
                            userControlCheckDeviceOnFormMain.textBoxEDoorInDirection.BackColor = Color.Red;
                            userControlCheckDeviceOnFormMain.textBoxEDoorInDirection.ForeColor = Color.White;
                        }));
                    }
                    else
                    {
                        Invoke((MethodInvoker)(() =>
                        {
                            userControlCheckDeviceOnFormMain.textBoxEDoorInDirection.Text = "In DIR";
                            userControlCheckDeviceOnFormMain.textBoxEDoorInDirection.BackColor = Color.Black;
                            userControlCheckDeviceOnFormMain.textBoxEDoorInDirection.ForeColor = Color.White;
                        }));
                    }
                }

                {
                    if (!eDoor.InsideSensor)
                    {
                        Invoke((MethodInvoker)(() =>
                        {
                            userControlCheckDeviceOnFormMain.textBoxEDoorInsideSensor.Text = "In Sensor";
                            userControlCheckDeviceOnFormMain.textBoxEDoorInsideSensor.BackColor = Color.Red;
                            userControlCheckDeviceOnFormMain.textBoxEDoorInsideSensor.ForeColor = Color.White;
                        }));
                    }
                    else
                    {
                        Invoke((MethodInvoker)(() =>
                        {
                            userControlCheckDeviceOnFormMain.textBoxEDoorInsideSensor.Text = "In Sensor";
                            userControlCheckDeviceOnFormMain.textBoxEDoorInsideSensor.BackColor = Color.Black;
                            userControlCheckDeviceOnFormMain.textBoxEDoorInsideSensor.ForeColor = Color.White;
                        }));
                    }
                }
                {
                    if (!eDoor.OutsideSensor)
                    {
                        Invoke((MethodInvoker)(() =>
                        {
                            userControlCheckDeviceOnFormMain.textBoxEDoorOutsideSensor.Text = "Out Sensor";
                            userControlCheckDeviceOnFormMain.textBoxEDoorOutsideSensor.BackColor = Color.Red;
                            userControlCheckDeviceOnFormMain.textBoxEDoorOutsideSensor.ForeColor = Color.White;
                        }));
                    }
                    else
                    {
                        Invoke((MethodInvoker)(() =>
                        {
                            userControlCheckDeviceOnFormMain.textBoxEDoorOutsideSensor.Text = "Out Sensor";
                            userControlCheckDeviceOnFormMain.textBoxEDoorOutsideSensor.BackColor = Color.Black;
                            userControlCheckDeviceOnFormMain.textBoxEDoorOutsideSensor.ForeColor = Color.White;
                        }));
                    }
                }
            }
        }

    }
}
