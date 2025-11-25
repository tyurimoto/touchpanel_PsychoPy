namespace Compartment
{
    partial class FormCredit
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormCredit));
            this.textBoxLicenses = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // textBoxLicenses
            // 
            this.textBoxLicenses.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxLicenses.Location = new System.Drawing.Point(0, 0);
            this.textBoxLicenses.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.textBoxLicenses.Multiline = true;
            this.textBoxLicenses.Name = "textBoxLicenses";
            this.textBoxLicenses.ReadOnly = true;
            this.textBoxLicenses.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxLicenses.Size = new System.Drawing.Size(1333, 675);
            this.textBoxLicenses.TabIndex = 0;
            this.textBoxLicenses.Text = resources.GetString("textBoxLicenses.Text");
            // 
            // FormCredit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1333, 675);
            this.Controls.Add(this.textBoxLicenses);
            this.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.Name = "FormCredit";
            this.Text = "FormCredit";
            this.Load += new System.EventHandler(this.FormCredit_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxLicenses;
    }
}