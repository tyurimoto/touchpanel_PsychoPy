
namespace Compartment
{
    partial class FormSelectEngine
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.labelVersion = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.radioButtonBlockEngine = new System.Windows.Forms.RadioButton();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.radioButtonPsychoPy = new System.Windows.Forms.RadioButton();
            this.labelAssemblyVersion = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.labelAssemblyVersion);
            this.groupBox1.Controls.Add(this.labelVersion);
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Controls.Add(this.radioButtonPsychoPy);
            this.groupBox1.Controls.Add(this.radioButtonBlockEngine);
            this.groupBox1.Controls.Add(this.radioButton1);
            this.groupBox1.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.groupBox1.Location = new System.Drawing.Point(14, 16);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox1.Size = new System.Drawing.Size(533, 450);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "エンジン選択";
            // 
            // labelVersion
            // 
            this.labelVersion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelVersion.AutoSize = true;
            this.labelVersion.Font = new System.Drawing.Font("游ゴシック", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.labelVersion.Location = new System.Drawing.Point(384, 24);
            this.labelVersion.Name = "labelVersion";
            this.labelVersion.Size = new System.Drawing.Size(128, 25);
            this.labelVersion.TabIndex = 3;
            this.labelVersion.Text = "Base Version";
            // 
            // button1
            //
            this.button1.Font = new System.Drawing.Font("游ゴシック", 26.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.button1.Location = new System.Drawing.Point(325, 350);
            this.button1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(150, 71);
            this.button1.TabIndex = 2;
            this.button1.Text = "Go";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // radioButtonBlockEngine
            // 
            this.radioButtonBlockEngine.AutoSize = true;
            this.radioButtonBlockEngine.Font = new System.Drawing.Font("游ゴシック", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.radioButtonBlockEngine.Location = new System.Drawing.Point(94, 96);
            this.radioButtonBlockEngine.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.radioButtonBlockEngine.Name = "radioButtonBlockEngine";
            this.radioButtonBlockEngine.Size = new System.Drawing.Size(303, 31);
            this.radioButtonBlockEngine.TabIndex = 1;
            this.radioButtonBlockEngine.TabStop = true;
            this.radioButtonBlockEngine.Text = "ブロックプログラムエンジン";
            this.radioButtonBlockEngine.UseVisualStyleBackColor = true;
            // 
            // radioButton1
            //
            this.radioButton1.AutoSize = true;
            this.radioButton1.Font = new System.Drawing.Font("游ゴシック", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.radioButton1.Location = new System.Drawing.Point(94, 96);
            this.radioButton1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(135, 31);
            this.radioButton1.TabIndex = 0;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "旧エンジン";
            this.radioButton1.UseVisualStyleBackColor = true;
            this.radioButton1.Visible = false;
            //
            // radioButtonPsychoPy
            //
            this.radioButtonPsychoPy.AutoSize = true;
            this.radioButtonPsychoPy.Font = new System.Drawing.Font("游ゴシック", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.radioButtonPsychoPy.Location = new System.Drawing.Point(94, 173);
            this.radioButtonPsychoPy.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.radioButtonPsychoPy.Name = "radioButtonPsychoPy";
            this.radioButtonPsychoPy.Size = new System.Drawing.Size(200, 31);
            this.radioButtonPsychoPy.TabIndex = 2;
            this.radioButtonPsychoPy.TabStop = true;
            this.radioButtonPsychoPy.Text = "PsychoPyエンジン";
            this.radioButtonPsychoPy.UseVisualStyleBackColor = true;
            //
            // labelAssemblyVersion
            // 
            this.labelAssemblyVersion.AutoSize = true;
            this.labelAssemblyVersion.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.labelAssemblyVersion.Font = new System.Drawing.Font("游ゴシック", 12F);
            this.labelAssemblyVersion.Location = new System.Drawing.Point(3, 338);
            this.labelAssemblyVersion.Name = "labelAssemblyVersion";
            this.labelAssemblyVersion.Size = new System.Drawing.Size(85, 21);
            this.labelAssemblyVersion.TabIndex = 4;
            this.labelAssemblyVersion.Text = "Ver.0.0.0.0";
            // 
            // FormSelectEngine
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(561, 480);
            this.ControlBox = false;
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("游ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "FormSelectEngine";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Select Engine";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.RadioButton radioButtonBlockEngine;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.RadioButton radioButtonPsychoPy;
        private System.Windows.Forms.Label labelVersion;
        private System.Windows.Forms.Label labelAssemblyVersion;
    }
}