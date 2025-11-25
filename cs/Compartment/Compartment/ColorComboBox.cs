using System;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace Compartment
{
    /// <summary>
    /// カラーコンボボックス
    /// </summary>
    public class ColorComboBox : ComboBox
    {
        private Color selectedColor;

        public ColorComboBox() : base()
        {
            DrawMode = DrawMode.OwnerDrawFixed;
            DropDownStyle = ComboBoxStyle.DropDownList;
            if (!DesignMode && Items.Count == 0)
            {
                PropertyInfo[] info = typeof(Color).GetProperties(BindingFlags.Public | BindingFlags.Static);
                foreach (var n in info)
                {
                    object a = n.GetValue(this, null);
                    if (!Items.Contains((Color)a))
                    {
                        _ = Items.Add(a);
                    }
                }
            }
            // Default 黒色
            SelectedColor = Color.Black;
        }

        [DefaultValue(typeof(Color), "Black")]
        [Browsable(true)]
        public Color SelectedColor
        {
            get => selectedColor;
            set
            {
                selectedColor = value;

                for (int i = 0; i < Items.Count; ++i)
                {
                    if ((Color)Items[i] == value)
                    {
                        SelectedIndex = i;
                        break;
                    }
                }
            }
        }

        protected override void OnLayout(LayoutEventArgs levent)
        {
            base.OnLayout(levent);
            Refresh();
        }

        protected override void OnSelectedIndexChanged(EventArgs e)
        {
            selectedColor = (Color)SelectedItem;
            base.OnSelectedIndexChanged(e);
        }

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            if (e.Index == -1) return;

            e.DrawBackground();

            Rectangle rect = e.Bounds;
            Color color = (Color)this.Items[e.Index];
            rect.Offset(2, 2);
            rect.Width = 25;
            rect.Height -= 4;

            e.Graphics.FillRectangle(new SolidBrush(color), rect);
            e.Graphics.DrawRectangle(new Pen(e.ForeColor), rect);
            e.Graphics.DrawString(color.Name, Font, new SolidBrush(e.ForeColor), e.Bounds.X + 30, e.Bounds.Y + 1);
            base.OnDrawItem(e);
        }
    }
}