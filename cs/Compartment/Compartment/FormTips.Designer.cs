
namespace Compartment
{
    partial class FormTips
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
            this.buttonClose = new System.Windows.Forms.Button();
            this.listBoxTips = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // buttonClose
            // 
            this.buttonClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonClose.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonClose.Location = new System.Drawing.Point(322, 354);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(75, 23);
            this.buttonClose.TabIndex = 0;
            this.buttonClose.Text = "Close";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // listBoxTips
            // 
            this.listBoxTips.BackColor = System.Drawing.SystemColors.ControlLight;
            this.listBoxTips.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listBoxTips.FormattingEnabled = true;
            this.listBoxTips.ItemHeight = 12;
            this.listBoxTips.Items.AddRange(new object[] {
            "DrawScreenReset",
            "ViewTriggerImage",
            "WaitTouchTrigger",
            "DrawScreenReset",
            "Delay(100-1000)",
            "ViewCorrectImage",
            "Delay(3000)",
            "DrawScreenReset",
            "MatchBeforeDelay(1000-2000)",
            "ViewCorrectWrongImage",
            "Delay(100-500)",
            "WaitCorrectTouchTrigger",
            "DrawScreenReset",
            "PlaySound",
            "FeedLamp(1000)",
            "Delay(500)",
            "Feed(500-2000)",
            "FeedSound(1000-1000)",
            "OutputResult",
            "TouchDelay(800-1000)"});
            this.listBoxTips.Location = new System.Drawing.Point(12, 12);
            this.listBoxTips.Name = "listBoxTips";
            this.listBoxTips.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.listBoxTips.Size = new System.Drawing.Size(305, 326);
            this.listBoxTips.TabIndex = 1;
            // 
            // FormTips
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(409, 389);
            this.Controls.Add(this.listBoxTips);
            this.Controls.Add(this.buttonClose);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "FormTips";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Tips";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.ListBox listBoxTips;
    }
}