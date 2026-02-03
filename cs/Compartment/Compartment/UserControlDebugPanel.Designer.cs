namespace Compartment
{
    partial class UserControlDebugPanel
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
            this.tableLayoutPanelMain = new System.Windows.Forms.TableLayoutPanel();
            this.labelTitle = new System.Windows.Forms.Label();
            this.groupBoxSensors = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanelSensors = new System.Windows.Forms.TableLayoutPanel();
            this.labelEntranceName = new System.Windows.Forms.Label();
            this.buttonEntranceOn = new System.Windows.Forms.Button();
            this.buttonEntranceOff = new System.Windows.Forms.Button();
            this.labelEntranceStatus = new System.Windows.Forms.Label();
            this.labelExitName = new System.Windows.Forms.Label();
            this.buttonExitOn = new System.Windows.Forms.Button();
            this.buttonExitOff = new System.Windows.Forms.Button();
            this.labelExitStatus = new System.Windows.Forms.Label();
            this.groupBoxDevices = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanelDevices = new System.Windows.Forms.TableLayoutPanel();
            this.labelDoorTitle = new System.Windows.Forms.Label();
            this.labelDoorStatus = new System.Windows.Forms.Label();
            this.labelLeverTitle = new System.Windows.Forms.Label();
            this.labelLeverStatus = new System.Windows.Forms.Label();
            this.labelFeedingTitle = new System.Windows.Forms.Label();
            this.labelFeedingStatus = new System.Windows.Forms.Label();
            this.groupBoxRFID = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanelRFID = new System.Windows.Forms.TableLayoutPanel();
            this.labelRFIDTitle = new System.Windows.Forms.Label();
            this.textBoxRFID = new System.Windows.Forms.TextBox();
            this.buttonRFIDRandom = new System.Windows.Forms.Button();
            this.buttonRFIDSet = new System.Windows.Forms.Button();
            this.buttonRFIDClear = new System.Windows.Forms.Button();
            this.groupBoxLog = new System.Windows.Forms.GroupBox();
            this.textBoxLog = new System.Windows.Forms.TextBox();
            this.tableLayoutPanelMain.SuspendLayout();
            this.groupBoxSensors.SuspendLayout();
            this.tableLayoutPanelSensors.SuspendLayout();
            this.groupBoxDevices.SuspendLayout();
            this.tableLayoutPanelDevices.SuspendLayout();
            this.groupBoxRFID.SuspendLayout();
            this.tableLayoutPanelRFID.SuspendLayout();
            this.groupBoxLog.SuspendLayout();
            this.SuspendLayout();
            //
            // tableLayoutPanelMain
            //
            this.tableLayoutPanelMain.ColumnCount = 1;
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelMain.Controls.Add(this.labelTitle, 0, 0);
            this.tableLayoutPanelMain.Controls.Add(this.groupBoxSensors, 0, 1);
            this.tableLayoutPanelMain.Controls.Add(this.groupBoxDevices, 0, 2);
            this.tableLayoutPanelMain.Controls.Add(this.groupBoxRFID, 0, 3);
            this.tableLayoutPanelMain.Controls.Add(this.groupBoxLog, 0, 4);
            this.tableLayoutPanelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelMain.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanelMain.Name = "tableLayoutPanelMain";
            this.tableLayoutPanelMain.RowCount = 5;
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelMain.Size = new System.Drawing.Size(450, 600);
            this.tableLayoutPanelMain.TabIndex = 0;
            //
            // labelTitle
            //
            this.labelTitle.AutoSize = true;
            this.labelTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelTitle.Font = new System.Drawing.Font("MS UI Gothic", 14F, System.Drawing.FontStyle.Bold);
            this.labelTitle.Location = new System.Drawing.Point(3, 0);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(444, 40);
            this.labelTitle.TabIndex = 0;
            this.labelTitle.Text = "デバッグコントロールパネル";
            this.labelTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // groupBoxSensors
            //
            this.groupBoxSensors.Controls.Add(this.tableLayoutPanelSensors);
            this.groupBoxSensors.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxSensors.Font = new System.Drawing.Font("MS UI Gothic", 10F, System.Drawing.FontStyle.Bold);
            this.groupBoxSensors.Location = new System.Drawing.Point(3, 43);
            this.groupBoxSensors.Name = "groupBoxSensors";
            this.groupBoxSensors.Size = new System.Drawing.Size(444, 114);
            this.groupBoxSensors.TabIndex = 1;
            this.groupBoxSensors.TabStop = false;
            this.groupBoxSensors.Text = "センサー（手動シミュレート）";
            //
            // tableLayoutPanelSensors
            //
            this.tableLayoutPanelSensors.ColumnCount = 4;
            this.tableLayoutPanelSensors.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanelSensors.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33F));
            this.tableLayoutPanelSensors.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33F));
            this.tableLayoutPanelSensors.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.34F));
            this.tableLayoutPanelSensors.Controls.Add(this.labelEntranceName, 0, 0);
            this.tableLayoutPanelSensors.Controls.Add(this.buttonEntranceOn, 1, 0);
            this.tableLayoutPanelSensors.Controls.Add(this.buttonEntranceOff, 2, 0);
            this.tableLayoutPanelSensors.Controls.Add(this.labelEntranceStatus, 3, 0);
            this.tableLayoutPanelSensors.Controls.Add(this.labelExitName, 0, 1);
            this.tableLayoutPanelSensors.Controls.Add(this.buttonExitOn, 1, 1);
            this.tableLayoutPanelSensors.Controls.Add(this.buttonExitOff, 2, 1);
            this.tableLayoutPanelSensors.Controls.Add(this.labelExitStatus, 3, 1);
            this.tableLayoutPanelSensors.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelSensors.Location = new System.Drawing.Point(3, 19);
            this.tableLayoutPanelSensors.Name = "tableLayoutPanelSensors";
            this.tableLayoutPanelSensors.RowCount = 2;
            this.tableLayoutPanelSensors.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanelSensors.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanelSensors.Size = new System.Drawing.Size(438, 92);
            this.tableLayoutPanelSensors.TabIndex = 0;
            //
            // labelEntranceName
            //
            this.labelEntranceName.AutoSize = true;
            this.labelEntranceName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelEntranceName.Font = new System.Drawing.Font("MS UI Gothic", 9F);
            this.labelEntranceName.Location = new System.Drawing.Point(3, 0);
            this.labelEntranceName.Name = "labelEntranceName";
            this.labelEntranceName.Size = new System.Drawing.Size(94, 46);
            this.labelEntranceName.TabIndex = 0;
            this.labelEntranceName.Text = "入室センサー";
            this.labelEntranceName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // buttonEntranceOn
            //
            this.buttonEntranceOn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonEntranceOn.Font = new System.Drawing.Font("MS UI Gothic", 9F);
            this.buttonEntranceOn.Location = new System.Drawing.Point(103, 3);
            this.buttonEntranceOn.Name = "buttonEntranceOn";
            this.buttonEntranceOn.Size = new System.Drawing.Size(106, 40);
            this.buttonEntranceOn.TabIndex = 1;
            this.buttonEntranceOn.Text = "ON";
            this.buttonEntranceOn.UseVisualStyleBackColor = true;
            //
            // buttonEntranceOff
            //
            this.buttonEntranceOff.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonEntranceOff.Font = new System.Drawing.Font("MS UI Gothic", 9F);
            this.buttonEntranceOff.Location = new System.Drawing.Point(215, 3);
            this.buttonEntranceOff.Name = "buttonEntranceOff";
            this.buttonEntranceOff.Size = new System.Drawing.Size(106, 40);
            this.buttonEntranceOff.TabIndex = 2;
            this.buttonEntranceOff.Text = "OFF";
            this.buttonEntranceOff.UseVisualStyleBackColor = true;
            //
            // labelEntranceStatus
            //
            this.labelEntranceStatus.AutoSize = true;
            this.labelEntranceStatus.BackColor = System.Drawing.Color.Gray;
            this.labelEntranceStatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelEntranceStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelEntranceStatus.Font = new System.Drawing.Font("MS UI Gothic", 9F);
            this.labelEntranceStatus.Location = new System.Drawing.Point(327, 0);
            this.labelEntranceStatus.Name = "labelEntranceStatus";
            this.labelEntranceStatus.Size = new System.Drawing.Size(108, 46);
            this.labelEntranceStatus.TabIndex = 3;
            this.labelEntranceStatus.Text = "OFF";
            this.labelEntranceStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // labelExitName
            //
            this.labelExitName.AutoSize = true;
            this.labelExitName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelExitName.Font = new System.Drawing.Font("MS UI Gothic", 9F);
            this.labelExitName.Location = new System.Drawing.Point(3, 46);
            this.labelExitName.Name = "labelExitName";
            this.labelExitName.Size = new System.Drawing.Size(94, 46);
            this.labelExitName.TabIndex = 4;
            this.labelExitName.Text = "退室センサー";
            this.labelExitName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // buttonExitOn
            //
            this.buttonExitOn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonExitOn.Font = new System.Drawing.Font("MS UI Gothic", 9F);
            this.buttonExitOn.Location = new System.Drawing.Point(103, 49);
            this.buttonExitOn.Name = "buttonExitOn";
            this.buttonExitOn.Size = new System.Drawing.Size(106, 40);
            this.buttonExitOn.TabIndex = 5;
            this.buttonExitOn.Text = "ON";
            this.buttonExitOn.UseVisualStyleBackColor = true;
            //
            // buttonExitOff
            //
            this.buttonExitOff.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonExitOff.Font = new System.Drawing.Font("MS UI Gothic", 9F);
            this.buttonExitOff.Location = new System.Drawing.Point(215, 49);
            this.buttonExitOff.Name = "buttonExitOff";
            this.buttonExitOff.Size = new System.Drawing.Size(106, 40);
            this.buttonExitOff.TabIndex = 6;
            this.buttonExitOff.Text = "OFF";
            this.buttonExitOff.UseVisualStyleBackColor = true;
            //
            // labelExitStatus
            //
            this.labelExitStatus.AutoSize = true;
            this.labelExitStatus.BackColor = System.Drawing.Color.Gray;
            this.labelExitStatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelExitStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelExitStatus.Font = new System.Drawing.Font("MS UI Gothic", 9F);
            this.labelExitStatus.Location = new System.Drawing.Point(327, 46);
            this.labelExitStatus.Name = "labelExitStatus";
            this.labelExitStatus.Size = new System.Drawing.Size(108, 46);
            this.labelExitStatus.TabIndex = 7;
            this.labelExitStatus.Text = "OFF";
            this.labelExitStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // groupBoxDevices
            //
            this.groupBoxDevices.Controls.Add(this.tableLayoutPanelDevices);
            this.groupBoxDevices.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxDevices.Font = new System.Drawing.Font("MS UI Gothic", 10F, System.Drawing.FontStyle.Bold);
            this.groupBoxDevices.Location = new System.Drawing.Point(3, 163);
            this.groupBoxDevices.Name = "groupBoxDevices";
            this.groupBoxDevices.Size = new System.Drawing.Size(444, 114);
            this.groupBoxDevices.TabIndex = 2;
            this.groupBoxDevices.TabStop = false;
            this.groupBoxDevices.Text = "デバイス状態";
            //
            // tableLayoutPanelDevices
            //
            this.tableLayoutPanelDevices.ColumnCount = 2;
            this.tableLayoutPanelDevices.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanelDevices.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelDevices.Controls.Add(this.labelDoorTitle, 0, 0);
            this.tableLayoutPanelDevices.Controls.Add(this.labelDoorStatus, 1, 0);
            this.tableLayoutPanelDevices.Controls.Add(this.labelLeverTitle, 0, 1);
            this.tableLayoutPanelDevices.Controls.Add(this.labelLeverStatus, 1, 1);
            this.tableLayoutPanelDevices.Controls.Add(this.labelFeedingTitle, 0, 2);
            this.tableLayoutPanelDevices.Controls.Add(this.labelFeedingStatus, 1, 2);
            this.tableLayoutPanelDevices.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelDevices.Location = new System.Drawing.Point(3, 19);
            this.tableLayoutPanelDevices.Name = "tableLayoutPanelDevices";
            this.tableLayoutPanelDevices.RowCount = 3;
            this.tableLayoutPanelDevices.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33F));
            this.tableLayoutPanelDevices.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33F));
            this.tableLayoutPanelDevices.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.34F));
            this.tableLayoutPanelDevices.Size = new System.Drawing.Size(438, 92);
            this.tableLayoutPanelDevices.TabIndex = 0;
            //
            // labelDoorTitle
            //
            this.labelDoorTitle.AutoSize = true;
            this.labelDoorTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelDoorTitle.Font = new System.Drawing.Font("MS UI Gothic", 9F);
            this.labelDoorTitle.Location = new System.Drawing.Point(3, 0);
            this.labelDoorTitle.Name = "labelDoorTitle";
            this.labelDoorTitle.Size = new System.Drawing.Size(94, 30);
            this.labelDoorTitle.TabIndex = 0;
            this.labelDoorTitle.Text = "ドア:";
            this.labelDoorTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // labelDoorStatus
            //
            this.labelDoorStatus.AutoSize = true;
            this.labelDoorStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelDoorStatus.Font = new System.Drawing.Font("MS UI Gothic", 9F);
            this.labelDoorStatus.Location = new System.Drawing.Point(103, 0);
            this.labelDoorStatus.Name = "labelDoorStatus";
            this.labelDoorStatus.Size = new System.Drawing.Size(332, 30);
            this.labelDoorStatus.TabIndex = 1;
            this.labelDoorStatus.Text = "○ 不明";
            this.labelDoorStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // labelLeverTitle
            //
            this.labelLeverTitle.AutoSize = true;
            this.labelLeverTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelLeverTitle.Font = new System.Drawing.Font("MS UI Gothic", 9F);
            this.labelLeverTitle.Location = new System.Drawing.Point(3, 30);
            this.labelLeverTitle.Name = "labelLeverTitle";
            this.labelLeverTitle.Size = new System.Drawing.Size(94, 30);
            this.labelLeverTitle.TabIndex = 2;
            this.labelLeverTitle.Text = "レバー:";
            this.labelLeverTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // labelLeverStatus
            //
            this.labelLeverStatus.AutoSize = true;
            this.labelLeverStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelLeverStatus.Font = new System.Drawing.Font("MS UI Gothic", 9F);
            this.labelLeverStatus.Location = new System.Drawing.Point(103, 30);
            this.labelLeverStatus.Name = "labelLeverStatus";
            this.labelLeverStatus.Size = new System.Drawing.Size(332, 30);
            this.labelLeverStatus.TabIndex = 3;
            this.labelLeverStatus.Text = "○ 不明";
            this.labelLeverStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // labelFeedingTitle
            //
            this.labelFeedingTitle.AutoSize = true;
            this.labelFeedingTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelFeedingTitle.Font = new System.Drawing.Font("MS UI Gothic", 9F);
            this.labelFeedingTitle.Location = new System.Drawing.Point(3, 60);
            this.labelFeedingTitle.Name = "labelFeedingTitle";
            this.labelFeedingTitle.Size = new System.Drawing.Size(94, 32);
            this.labelFeedingTitle.TabIndex = 4;
            this.labelFeedingTitle.Text = "給餌:";
            this.labelFeedingTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // labelFeedingStatus
            //
            this.labelFeedingStatus.AutoSize = true;
            this.labelFeedingStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelFeedingStatus.Font = new System.Drawing.Font("MS UI Gothic", 9F);
            this.labelFeedingStatus.Location = new System.Drawing.Point(103, 60);
            this.labelFeedingStatus.Name = "labelFeedingStatus";
            this.labelFeedingStatus.Size = new System.Drawing.Size(332, 32);
            this.labelFeedingStatus.TabIndex = 5;
            this.labelFeedingStatus.Text = "○ 給餌中ではありません";
            this.labelFeedingStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // groupBoxRFID
            //
            this.groupBoxRFID.Controls.Add(this.tableLayoutPanelRFID);
            this.groupBoxRFID.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxRFID.Font = new System.Drawing.Font("MS UI Gothic", 10F, System.Drawing.FontStyle.Bold);
            this.groupBoxRFID.Location = new System.Drawing.Point(3, 283);
            this.groupBoxRFID.Name = "groupBoxRFID";
            this.groupBoxRFID.Size = new System.Drawing.Size(444, 114);
            this.groupBoxRFID.TabIndex = 3;
            this.groupBoxRFID.TabStop = false;
            this.groupBoxRFID.Text = "RFID（手動設定）";
            //
            // tableLayoutPanelRFID
            //
            this.tableLayoutPanelRFID.ColumnCount = 2;
            this.tableLayoutPanelRFID.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanelRFID.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelRFID.Controls.Add(this.labelRFIDTitle, 0, 0);
            this.tableLayoutPanelRFID.Controls.Add(this.textBoxRFID, 1, 0);
            this.tableLayoutPanelRFID.Controls.Add(this.buttonRFIDRandom, 1, 1);
            this.tableLayoutPanelRFID.Controls.Add(this.buttonRFIDSet, 0, 2);
            this.tableLayoutPanelRFID.Controls.Add(this.buttonRFIDClear, 1, 2);
            this.tableLayoutPanelRFID.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelRFID.Location = new System.Drawing.Point(3, 19);
            this.tableLayoutPanelRFID.Name = "tableLayoutPanelRFID";
            this.tableLayoutPanelRFID.RowCount = 3;
            this.tableLayoutPanelRFID.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanelRFID.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanelRFID.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanelRFID.Size = new System.Drawing.Size(438, 92);
            this.tableLayoutPanelRFID.TabIndex = 0;
            //
            // labelRFIDTitle
            //
            this.labelRFIDTitle.AutoSize = true;
            this.labelRFIDTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelRFIDTitle.Font = new System.Drawing.Font("MS UI Gothic", 9F);
            this.labelRFIDTitle.Location = new System.Drawing.Point(3, 0);
            this.labelRFIDTitle.Name = "labelRFIDTitle";
            this.labelRFIDTitle.Size = new System.Drawing.Size(94, 30);
            this.labelRFIDTitle.TabIndex = 0;
            this.labelRFIDTitle.Text = "ID:";
            this.labelRFIDTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // textBoxRFID
            //
            this.textBoxRFID.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxRFID.Font = new System.Drawing.Font("MS UI Gothic", 10F);
            this.textBoxRFID.Location = new System.Drawing.Point(103, 3);
            this.textBoxRFID.MaxLength = 16;
            this.textBoxRFID.Name = "textBoxRFID";
            this.textBoxRFID.Size = new System.Drawing.Size(332, 21);
            this.textBoxRFID.TabIndex = 1;
            //
            // buttonRFIDRandom
            //
            this.buttonRFIDRandom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonRFIDRandom.Font = new System.Drawing.Font("MS UI Gothic", 9F);
            this.buttonRFIDRandom.Location = new System.Drawing.Point(103, 33);
            this.buttonRFIDRandom.Name = "buttonRFIDRandom";
            this.buttonRFIDRandom.Size = new System.Drawing.Size(332, 24);
            this.buttonRFIDRandom.TabIndex = 2;
            this.buttonRFIDRandom.Text = "ランダム生成";
            this.buttonRFIDRandom.UseVisualStyleBackColor = true;
            //
            // buttonRFIDSet
            //
            this.tableLayoutPanelRFID.SetColumnSpan(this.buttonRFIDSet, 1);
            this.buttonRFIDSet.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonRFIDSet.Font = new System.Drawing.Font("MS UI Gothic", 9F);
            this.buttonRFIDSet.Location = new System.Drawing.Point(3, 63);
            this.buttonRFIDSet.Name = "buttonRFIDSet";
            this.buttonRFIDSet.Size = new System.Drawing.Size(94, 26);
            this.buttonRFIDSet.TabIndex = 3;
            this.buttonRFIDSet.Text = "設定";
            this.buttonRFIDSet.UseVisualStyleBackColor = true;
            //
            // buttonRFIDClear
            //
            this.buttonRFIDClear.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonRFIDClear.Font = new System.Drawing.Font("MS UI Gothic", 9F);
            this.buttonRFIDClear.Location = new System.Drawing.Point(103, 63);
            this.buttonRFIDClear.Name = "buttonRFIDClear";
            this.buttonRFIDClear.Size = new System.Drawing.Size(332, 26);
            this.buttonRFIDClear.TabIndex = 4;
            this.buttonRFIDClear.Text = "クリア";
            this.buttonRFIDClear.UseVisualStyleBackColor = true;
            //
            // groupBoxLog
            //
            this.groupBoxLog.Controls.Add(this.textBoxLog);
            this.groupBoxLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxLog.Font = new System.Drawing.Font("MS UI Gothic", 10F, System.Drawing.FontStyle.Bold);
            this.groupBoxLog.Location = new System.Drawing.Point(3, 403);
            this.groupBoxLog.Name = "groupBoxLog";
            this.groupBoxLog.Size = new System.Drawing.Size(444, 194);
            this.groupBoxLog.TabIndex = 4;
            this.groupBoxLog.TabStop = false;
            this.groupBoxLog.Text = "ログ";
            //
            // textBoxLog
            //
            this.textBoxLog.BackColor = System.Drawing.SystemColors.Window;
            this.textBoxLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxLog.Font = new System.Drawing.Font("MS UI Gothic", 9F);
            this.textBoxLog.Location = new System.Drawing.Point(3, 19);
            this.textBoxLog.Multiline = true;
            this.textBoxLog.Name = "textBoxLog";
            this.textBoxLog.ReadOnly = true;
            this.textBoxLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxLog.Size = new System.Drawing.Size(438, 172);
            this.textBoxLog.TabIndex = 0;
            //
            // UserControlDebugPanel
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanelMain);
            this.Name = "UserControlDebugPanel";
            this.Size = new System.Drawing.Size(450, 600);
            this.tableLayoutPanelMain.ResumeLayout(false);
            this.tableLayoutPanelMain.PerformLayout();
            this.groupBoxSensors.ResumeLayout(false);
            this.tableLayoutPanelSensors.ResumeLayout(false);
            this.tableLayoutPanelSensors.PerformLayout();
            this.groupBoxDevices.ResumeLayout(false);
            this.tableLayoutPanelDevices.ResumeLayout(false);
            this.tableLayoutPanelDevices.PerformLayout();
            this.groupBoxRFID.ResumeLayout(false);
            this.tableLayoutPanelRFID.ResumeLayout(false);
            this.tableLayoutPanelRFID.PerformLayout();
            this.groupBoxLog.ResumeLayout(false);
            this.groupBoxLog.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelMain;
        private System.Windows.Forms.Label labelTitle;
        private System.Windows.Forms.GroupBox groupBoxSensors;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelSensors;
        private System.Windows.Forms.Label labelEntranceName;
        private System.Windows.Forms.Button buttonEntranceOn;
        private System.Windows.Forms.Button buttonEntranceOff;
        private System.Windows.Forms.Label labelEntranceStatus;
        private System.Windows.Forms.Label labelExitName;
        private System.Windows.Forms.Button buttonExitOn;
        private System.Windows.Forms.Button buttonExitOff;
        private System.Windows.Forms.Label labelExitStatus;
        private System.Windows.Forms.GroupBox groupBoxDevices;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelDevices;
        private System.Windows.Forms.Label labelDoorTitle;
        private System.Windows.Forms.Label labelDoorStatus;
        private System.Windows.Forms.Label labelLeverTitle;
        private System.Windows.Forms.Label labelLeverStatus;
        private System.Windows.Forms.Label labelFeedingTitle;
        private System.Windows.Forms.Label labelFeedingStatus;
        private System.Windows.Forms.GroupBox groupBoxRFID;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelRFID;
        private System.Windows.Forms.Label labelRFIDTitle;
        private System.Windows.Forms.TextBox textBoxRFID;
        private System.Windows.Forms.Button buttonRFIDRandom;
        private System.Windows.Forms.Button buttonRFIDSet;
        private System.Windows.Forms.Button buttonRFIDClear;
        private System.Windows.Forms.GroupBox groupBoxLog;
        private System.Windows.Forms.TextBox textBoxLog;
    }
}
