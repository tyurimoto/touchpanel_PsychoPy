
namespace Compartment
{
	partial class UserControlPreferencesChild
	{
		/// <summary> 
		/// 必要なデザイナー変数です。
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// 使用中のリソースをすべてクリーンアップします。
		/// </summary>
		/// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region コンポーネント デザイナーで生成されたコード

		/// <summary> 
		/// デザイナー サポートに必要なメソッドです。このメソッドの内容を 
		/// コード エディターで変更しないでください。
		/// </summary>
		private void InitializeComponent()
		{
			this.propertyGridPreferences = new System.Windows.Forms.PropertyGrid();
			this.panel1 = new System.Windows.Forms.Panel();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.buttonEnd = new System.Windows.Forms.Button();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// propertyGridPreferences
			// 
			this.propertyGridPreferences.Location = new System.Drawing.Point(0, 0);
			this.propertyGridPreferences.Name = "propertyGridPreferences";
			this.propertyGridPreferences.PropertySort = System.Windows.Forms.PropertySort.Categorized;
			this.propertyGridPreferences.Size = new System.Drawing.Size(474, 235);
			this.propertyGridPreferences.TabIndex = 0;
			this.propertyGridPreferences.ToolbarVisible = false;
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.buttonCancel);
			this.panel1.Controls.Add(this.buttonEnd);
			this.panel1.Location = new System.Drawing.Point(3, 497);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(471, 92);
			this.panel1.TabIndex = 1;
			// 
			// buttonCancel
			// 
			this.buttonCancel.Location = new System.Drawing.Point(179, 26);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(94, 32);
			this.buttonCancel.TabIndex = 1;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			// 
			// buttonEnd
			// 
			this.buttonEnd.Location = new System.Drawing.Point(33, 23);
			this.buttonEnd.Name = "buttonEnd";
			this.buttonEnd.Size = new System.Drawing.Size(100, 36);
			this.buttonEnd.TabIndex = 0;
			this.buttonEnd.Text = "End";
			this.buttonEnd.UseVisualStyleBackColor = true;
			// 
			// UserControlPreferencesChild
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.propertyGridPreferences);
			this.Name = "UserControlPreferencesChild";
			this.Size = new System.Drawing.Size(830, 610);
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion
		private System.Windows.Forms.Panel panel1;
		public System.Windows.Forms.Button buttonCancel;
		public System.Windows.Forms.Button buttonEnd;
		public System.Windows.Forms.PropertyGrid propertyGridPreferences;
	}
}
