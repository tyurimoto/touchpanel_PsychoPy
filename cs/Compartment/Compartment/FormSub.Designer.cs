
namespace Compartment
{
	partial class FormSub
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.pictureBoxOnFormSub = new Compartment.WMTouchPictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxOnFormSub)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBoxOnFormSub
            // 
            this.pictureBoxOnFormSub.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBoxOnFormSub.Location = new System.Drawing.Point(0, 0);
            this.pictureBoxOnFormSub.Name = "pictureBoxOnFormSub";
            this.pictureBoxOnFormSub.Size = new System.Drawing.Size(800, 450);
            this.pictureBoxOnFormSub.TabIndex = 1;
            this.pictureBoxOnFormSub.TabStop = false;
            this.pictureBoxOnFormSub.TappedOnlyOnePoint += new System.EventHandler<System.Windows.Forms.MouseEventArgs>(this.pictureBoxOnFormSub_MouseClick);
            this.pictureBoxOnFormSub.MouseEnter += new System.EventHandler(this.pictureBoxOnFormSub_MouseEnter);
            this.pictureBoxOnFormSub.MouseLeave += new System.EventHandler(this.pictureBoxOnFormSub_MouseLeave);
            // 
            // FormSub
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.pictureBoxOnFormSub);
            this.Cursor = System.Windows.Forms.Cursors.No;
            this.DoubleBuffered = true;
            this.Name = "FormSub";
            this.Text = "FormSub";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxOnFormSub)).EndInit();
            this.ResumeLayout(false);

		}

        #endregion

        public WMTouchPictureBox pictureBoxOnFormSub;
    }
}