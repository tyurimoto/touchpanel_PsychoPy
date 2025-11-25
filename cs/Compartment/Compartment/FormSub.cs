using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace Compartment
{
    public partial class FormSub : Form
    {
        public FormSub()
        {
            InitializeComponent();
        }

        private void pictureBoxOnFormSub_MouseClick(object sender, MouseEventArgs e)
        {
            Debug.WriteLine("FormSubPictureBox-MouseClick: X:{0} Y:{1} Locate:{2}",
                            e.X,
                            e.Y,
                            e.Location);
            if (boolEnableCallBackTouchPoint.Value == true && e.Button == MouseButtons.Left)
            {
                callbackTouchPoint(new Point(e.X, e.Y));
            }
        }
        //	public bool boolEnableCallBackTouchPoint { get; set; } = false;
        public SyncObject<bool> boolEnableCallBackTouchPoint = new SyncObject<bool>(false);

        public Action<Point> callbackTouchPoint = (point) => { };

        private void pictureBoxOnFormSub_MouseEnter(object sender, EventArgs e)
        {
            //PictureBox内でカーソルを非表示にする
            System.Windows.Forms.Cursor.Hide();
        }

        private void pictureBoxOnFormSub_MouseLeave(object sender, EventArgs e)
        {
            //PictureBoxから出たらカーソルを表示にする
            System.Windows.Forms.Cursor.Show();
        }
    }
}
