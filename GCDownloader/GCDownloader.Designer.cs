namespace GCDownloader
{
    partial class GCDownloader
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GCDownloader));
            this.ssStatus = new System.Windows.Forms.StatusStrip();
            this.statusMessage = new System.Windows.Forms.ToolStripStatusLabel();
            this.panelLogin = new System.Windows.Forms.Panel();
            this.chkSavePassword = new System.Windows.Forms.CheckBox();
            this.chkSaveUsername = new System.Windows.Forms.CheckBox();
            this.btnLogin = new System.Windows.Forms.Button();
            this.txtUsername = new System.Windows.Forms.TextBox();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.lblPassword = new System.Windows.Forms.Label();
            this.lblUsername = new System.Windows.Forms.Label();
            this.panelMain = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.txtSteps = new System.Windows.Forms.TextBox();
            this.txtCalories = new System.Windows.Forms.TextBox();
            this.txtDuration = new System.Windows.Forms.TextBox();
            this.txtDistance = new System.Windows.Forms.TextBox();
            this.txtEventType = new System.Windows.Forms.TextBox();
            this.txtActivityType = new System.Windows.Forms.TextBox();
            this.txtActivityTime = new System.Windows.Forms.TextBox();
            this.txtActivityName = new System.Windows.Forms.TextBox();
            this.txtActivityID = new System.Windows.Forms.TextBox();
            this.lblSteps = new System.Windows.Forms.Label();
            this.lblCalories = new System.Windows.Forms.Label();
            this.lblDuration = new System.Windows.Forms.Label();
            this.lblDistance = new System.Windows.Forms.Label();
            this.lblEventType = new System.Windows.Forms.Label();
            this.lblActivityType = new System.Windows.Forms.Label();
            this.lblActivityTime = new System.Windows.Forms.Label();
            this.lblActivityName = new System.Windows.Forms.Label();
            this.lblActivityID = new System.Windows.Forms.Label();
            this.numBatchSize = new System.Windows.Forms.NumericUpDown();
            this.lblBatchSize = new System.Windows.Forms.Label();
            this.lstActivities = new System.Windows.Forms.ListBox();
            this.cmnMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mniSetDownloadFolder = new System.Windows.Forms.ToolStripMenuItem();
            this.mniSelectAll = new System.Windows.Forms.ToolStripMenuItem();
            this.mniDownloadSelected = new System.Windows.Forms.ToolStripMenuItem();
            this.mniRefresh = new System.Windows.Forms.ToolStripMenuItem();
            this.mniOpenSelected = new System.Windows.Forms.ToolStripMenuItem();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.btnNext = new System.Windows.Forms.Button();
            this.btnPrevious = new System.Windows.Forms.Button();
            this.lblActivities = new System.Windows.Forms.Label();
            this.mniClearCache = new System.Windows.Forms.ToolStripMenuItem();
            this.mniSignout = new System.Windows.Forms.ToolStripMenuItem();
            this.ssStatus.SuspendLayout();
            this.panelLogin.SuspendLayout();
            this.panelMain.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numBatchSize)).BeginInit();
            this.cmnMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // ssStatus
            // 
            this.ssStatus.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusMessage});
            this.ssStatus.Location = new System.Drawing.Point(0, 428);
            this.ssStatus.Name = "ssStatus";
            this.ssStatus.Size = new System.Drawing.Size(800, 22);
            this.ssStatus.TabIndex = 4;
            this.ssStatus.Text = "statusStrip1";
            // 
            // statusMessage
            // 
            this.statusMessage.Name = "statusMessage";
            this.statusMessage.Size = new System.Drawing.Size(0, 17);
            // 
            // panelLogin
            // 
            this.panelLogin.Controls.Add(this.chkSavePassword);
            this.panelLogin.Controls.Add(this.chkSaveUsername);
            this.panelLogin.Controls.Add(this.btnLogin);
            this.panelLogin.Controls.Add(this.txtUsername);
            this.panelLogin.Controls.Add(this.txtPassword);
            this.panelLogin.Controls.Add(this.lblPassword);
            this.panelLogin.Controls.Add(this.lblUsername);
            this.panelLogin.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelLogin.Location = new System.Drawing.Point(0, 0);
            this.panelLogin.Name = "panelLogin";
            this.panelLogin.Size = new System.Drawing.Size(800, 450);
            this.panelLogin.TabIndex = 5;
            // 
            // chkSavePassword
            // 
            this.chkSavePassword.AutoSize = true;
            this.chkSavePassword.Location = new System.Drawing.Point(434, 258);
            this.chkSavePassword.Name = "chkSavePassword";
            this.chkSavePassword.Size = new System.Drawing.Size(100, 17);
            this.chkSavePassword.TabIndex = 10;
            this.chkSavePassword.Text = "Save Password";
            this.chkSavePassword.UseVisualStyleBackColor = true;
            // 
            // chkSaveUsername
            // 
            this.chkSaveUsername.AutoSize = true;
            this.chkSaveUsername.Location = new System.Drawing.Point(333, 258);
            this.chkSaveUsername.Name = "chkSaveUsername";
            this.chkSaveUsername.Size = new System.Drawing.Size(102, 17);
            this.chkSaveUsername.TabIndex = 9;
            this.chkSaveUsername.Text = "Save Username";
            this.chkSaveUsername.UseVisualStyleBackColor = true;
            // 
            // btnLogin
            // 
            this.btnLogin.Location = new System.Drawing.Point(333, 229);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(201, 23);
            this.btnLogin.TabIndex = 8;
            this.btnLogin.Text = "Login";
            this.btnLogin.UseVisualStyleBackColor = true;
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
            // 
            // txtUsername
            // 
            this.txtUsername.Location = new System.Drawing.Point(333, 176);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Size = new System.Drawing.Size(201, 20);
            this.txtUsername.TabIndex = 5;
            this.txtUsername.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtUsername_KeyPress);
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(333, 202);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.Size = new System.Drawing.Size(201, 20);
            this.txtPassword.TabIndex = 7;
            this.txtPassword.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtPassword_KeyPress);
            // 
            // lblPassword
            // 
            this.lblPassword.AutoSize = true;
            this.lblPassword.Location = new System.Drawing.Point(268, 205);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(59, 13);
            this.lblPassword.TabIndex = 6;
            this.lblPassword.Text = "Password: ";
            // 
            // lblUsername
            // 
            this.lblUsername.AutoSize = true;
            this.lblUsername.Location = new System.Drawing.Point(266, 176);
            this.lblUsername.Name = "lblUsername";
            this.lblUsername.Size = new System.Drawing.Size(61, 13);
            this.lblUsername.TabIndex = 4;
            this.lblUsername.Text = "Username: ";
            // 
            // panelMain
            // 
            this.panelMain.Controls.Add(this.panel1);
            this.panelMain.Controls.Add(this.numBatchSize);
            this.panelMain.Controls.Add(this.lblBatchSize);
            this.panelMain.Controls.Add(this.lstActivities);
            this.panelMain.Controls.Add(this.btnRefresh);
            this.panelMain.Controls.Add(this.btnNext);
            this.panelMain.Controls.Add(this.btnPrevious);
            this.panelMain.Controls.Add(this.lblActivities);
            this.panelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMain.Location = new System.Drawing.Point(0, 0);
            this.panelMain.Name = "panelMain";
            this.panelMain.Size = new System.Drawing.Size(800, 450);
            this.panelMain.TabIndex = 6;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.txtSteps);
            this.panel1.Controls.Add(this.txtCalories);
            this.panel1.Controls.Add(this.txtDuration);
            this.panel1.Controls.Add(this.txtDistance);
            this.panel1.Controls.Add(this.txtEventType);
            this.panel1.Controls.Add(this.txtActivityType);
            this.panel1.Controls.Add(this.txtActivityTime);
            this.panel1.Controls.Add(this.txtActivityName);
            this.panel1.Controls.Add(this.txtActivityID);
            this.panel1.Controls.Add(this.lblSteps);
            this.panel1.Controls.Add(this.lblCalories);
            this.panel1.Controls.Add(this.lblDuration);
            this.panel1.Controls.Add(this.lblDistance);
            this.panel1.Controls.Add(this.lblEventType);
            this.panel1.Controls.Add(this.lblActivityType);
            this.panel1.Controls.Add(this.lblActivityTime);
            this.panel1.Controls.Add(this.lblActivityName);
            this.panel1.Controls.Add(this.lblActivityID);
            this.panel1.Location = new System.Drawing.Point(497, 25);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(291, 368);
            this.panel1.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(3, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(120, 17);
            this.label1.TabIndex = 4;
            this.label1.Text = "Activity Details:";
            // 
            // txtSteps
            // 
            this.txtSteps.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSteps.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtSteps.Location = new System.Drawing.Point(20, 336);
            this.txtSteps.Name = "txtSteps";
            this.txtSteps.ReadOnly = true;
            this.txtSteps.Size = new System.Drawing.Size(257, 13);
            this.txtSteps.TabIndex = 3;
            // 
            // txtCalories
            // 
            this.txtCalories.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtCalories.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtCalories.Location = new System.Drawing.Point(20, 302);
            this.txtCalories.Name = "txtCalories";
            this.txtCalories.ReadOnly = true;
            this.txtCalories.Size = new System.Drawing.Size(257, 13);
            this.txtCalories.TabIndex = 3;
            // 
            // txtDuration
            // 
            this.txtDuration.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDuration.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtDuration.Location = new System.Drawing.Point(20, 268);
            this.txtDuration.Name = "txtDuration";
            this.txtDuration.ReadOnly = true;
            this.txtDuration.Size = new System.Drawing.Size(257, 13);
            this.txtDuration.TabIndex = 3;
            // 
            // txtDistance
            // 
            this.txtDistance.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDistance.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtDistance.Location = new System.Drawing.Point(20, 234);
            this.txtDistance.Name = "txtDistance";
            this.txtDistance.ReadOnly = true;
            this.txtDistance.Size = new System.Drawing.Size(257, 13);
            this.txtDistance.TabIndex = 3;
            // 
            // txtEventType
            // 
            this.txtEventType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtEventType.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtEventType.Location = new System.Drawing.Point(20, 200);
            this.txtEventType.Name = "txtEventType";
            this.txtEventType.ReadOnly = true;
            this.txtEventType.Size = new System.Drawing.Size(257, 13);
            this.txtEventType.TabIndex = 3;
            // 
            // txtActivityType
            // 
            this.txtActivityType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtActivityType.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtActivityType.Location = new System.Drawing.Point(20, 166);
            this.txtActivityType.Name = "txtActivityType";
            this.txtActivityType.ReadOnly = true;
            this.txtActivityType.Size = new System.Drawing.Size(257, 13);
            this.txtActivityType.TabIndex = 3;
            // 
            // txtActivityTime
            // 
            this.txtActivityTime.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtActivityTime.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtActivityTime.Location = new System.Drawing.Point(20, 132);
            this.txtActivityTime.Name = "txtActivityTime";
            this.txtActivityTime.ReadOnly = true;
            this.txtActivityTime.Size = new System.Drawing.Size(257, 13);
            this.txtActivityTime.TabIndex = 3;
            // 
            // txtActivityName
            // 
            this.txtActivityName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtActivityName.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtActivityName.Location = new System.Drawing.Point(20, 98);
            this.txtActivityName.Name = "txtActivityName";
            this.txtActivityName.ReadOnly = true;
            this.txtActivityName.Size = new System.Drawing.Size(257, 13);
            this.txtActivityName.TabIndex = 3;
            // 
            // txtActivityID
            // 
            this.txtActivityID.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtActivityID.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtActivityID.Location = new System.Drawing.Point(20, 64);
            this.txtActivityID.Name = "txtActivityID";
            this.txtActivityID.ReadOnly = true;
            this.txtActivityID.Size = new System.Drawing.Size(257, 13);
            this.txtActivityID.TabIndex = 3;
            // 
            // lblSteps
            // 
            this.lblSteps.AutoSize = true;
            this.lblSteps.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.lblSteps.Location = new System.Drawing.Point(6, 319);
            this.lblSteps.Name = "lblSteps";
            this.lblSteps.Size = new System.Drawing.Size(37, 13);
            this.lblSteps.TabIndex = 2;
            this.lblSteps.Text = "Steps:";
            // 
            // lblCalories
            // 
            this.lblCalories.AutoSize = true;
            this.lblCalories.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.lblCalories.Location = new System.Drawing.Point(6, 285);
            this.lblCalories.Name = "lblCalories";
            this.lblCalories.Size = new System.Drawing.Size(47, 13);
            this.lblCalories.TabIndex = 2;
            this.lblCalories.Text = "Calories:";
            // 
            // lblDuration
            // 
            this.lblDuration.AutoSize = true;
            this.lblDuration.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.lblDuration.Location = new System.Drawing.Point(6, 251);
            this.lblDuration.Name = "lblDuration";
            this.lblDuration.Size = new System.Drawing.Size(50, 13);
            this.lblDuration.TabIndex = 2;
            this.lblDuration.Text = "Duration:";
            // 
            // lblDistance
            // 
            this.lblDistance.AutoSize = true;
            this.lblDistance.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.lblDistance.Location = new System.Drawing.Point(6, 217);
            this.lblDistance.Name = "lblDistance";
            this.lblDistance.Size = new System.Drawing.Size(52, 13);
            this.lblDistance.TabIndex = 2;
            this.lblDistance.Text = "Distance:";
            // 
            // lblEventType
            // 
            this.lblEventType.AutoSize = true;
            this.lblEventType.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.lblEventType.Location = new System.Drawing.Point(6, 183);
            this.lblEventType.Name = "lblEventType";
            this.lblEventType.Size = new System.Drawing.Size(65, 13);
            this.lblEventType.TabIndex = 2;
            this.lblEventType.Text = "Event Type:";
            // 
            // lblActivityType
            // 
            this.lblActivityType.AutoSize = true;
            this.lblActivityType.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.lblActivityType.Location = new System.Drawing.Point(6, 149);
            this.lblActivityType.Name = "lblActivityType";
            this.lblActivityType.Size = new System.Drawing.Size(71, 13);
            this.lblActivityType.TabIndex = 2;
            this.lblActivityType.Text = "Activity Type:";
            // 
            // lblActivityTime
            // 
            this.lblActivityTime.AutoSize = true;
            this.lblActivityTime.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.lblActivityTime.Location = new System.Drawing.Point(6, 115);
            this.lblActivityTime.Name = "lblActivityTime";
            this.lblActivityTime.Size = new System.Drawing.Size(70, 13);
            this.lblActivityTime.TabIndex = 2;
            this.lblActivityTime.Text = "Activity Time:";
            // 
            // lblActivityName
            // 
            this.lblActivityName.AutoSize = true;
            this.lblActivityName.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.lblActivityName.Location = new System.Drawing.Point(6, 81);
            this.lblActivityName.Name = "lblActivityName";
            this.lblActivityName.Size = new System.Drawing.Size(75, 13);
            this.lblActivityName.TabIndex = 1;
            this.lblActivityName.Text = "Activity Name:";
            // 
            // lblActivityID
            // 
            this.lblActivityID.AutoSize = true;
            this.lblActivityID.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.lblActivityID.Location = new System.Drawing.Point(6, 47);
            this.lblActivityID.Name = "lblActivityID";
            this.lblActivityID.Size = new System.Drawing.Size(58, 13);
            this.lblActivityID.TabIndex = 0;
            this.lblActivityID.Text = "Activity ID:";
            // 
            // numBatchSize
            // 
            this.numBatchSize.Location = new System.Drawing.Point(82, 402);
            this.numBatchSize.Name = "numBatchSize";
            this.numBatchSize.Size = new System.Drawing.Size(57, 20);
            this.numBatchSize.TabIndex = 4;
            this.numBatchSize.ValueChanged += new System.EventHandler(this.numBatchSize_ValueChanged);
            // 
            // lblBatchSize
            // 
            this.lblBatchSize.AutoSize = true;
            this.lblBatchSize.Location = new System.Drawing.Point(12, 404);
            this.lblBatchSize.Name = "lblBatchSize";
            this.lblBatchSize.Size = new System.Drawing.Size(64, 13);
            this.lblBatchSize.TabIndex = 3;
            this.lblBatchSize.Text = "Batch Size: ";
            // 
            // lstActivities
            // 
            this.lstActivities.ContextMenuStrip = this.cmnMenu;
            this.lstActivities.FormattingEnabled = true;
            this.lstActivities.Location = new System.Drawing.Point(15, 25);
            this.lstActivities.Name = "lstActivities";
            this.lstActivities.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lstActivities.Size = new System.Drawing.Size(476, 368);
            this.lstActivities.TabIndex = 2;
            this.lstActivities.SelectedIndexChanged += new System.EventHandler(this.lstActivities_SelectedIndexChanged);
            this.lstActivities.DoubleClick += new System.EventHandler(this.lstActivities_DoubleClick);
            // 
            // cmnMenu
            // 
            this.cmnMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mniSetDownloadFolder,
            this.mniSelectAll,
            this.mniDownloadSelected,
            this.mniRefresh,
            this.mniOpenSelected,
            this.mniClearCache,
            this.mniSignout});
            this.cmnMenu.Name = "cmnMenu";
            this.cmnMenu.Size = new System.Drawing.Size(237, 158);
            // 
            // mniSetDownloadFolder
            // 
            this.mniSetDownloadFolder.Name = "mniSetDownloadFolder";
            this.mniSetDownloadFolder.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F)));
            this.mniSetDownloadFolder.Size = new System.Drawing.Size(236, 22);
            this.mniSetDownloadFolder.Text = "Set Download &Folder";
            this.mniSetDownloadFolder.Click += new System.EventHandler(this.mniSetDownloadFolder_Click);
            // 
            // mniSelectAll
            // 
            this.mniSelectAll.Name = "mniSelectAll";
            this.mniSelectAll.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A)));
            this.mniSelectAll.Size = new System.Drawing.Size(236, 22);
            this.mniSelectAll.Text = "Select &All";
            this.mniSelectAll.Click += new System.EventHandler(this.mniSelectAll_Click);
            // 
            // mniDownloadSelected
            // 
            this.mniDownloadSelected.Name = "mniDownloadSelected";
            this.mniDownloadSelected.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D)));
            this.mniDownloadSelected.Size = new System.Drawing.Size(236, 22);
            this.mniDownloadSelected.Text = "&Download Selected";
            this.mniDownloadSelected.Click += new System.EventHandler(this.mniDownloadSelected_Click);
            // 
            // mniRefresh
            // 
            this.mniRefresh.Name = "mniRefresh";
            this.mniRefresh.ShortcutKeys = System.Windows.Forms.Keys.F5;
            this.mniRefresh.Size = new System.Drawing.Size(236, 22);
            this.mniRefresh.Text = "&Refresh List";
            this.mniRefresh.Click += new System.EventHandler(this.mniRefresh_Click);
            // 
            // mniOpenSelected
            // 
            this.mniOpenSelected.Name = "mniOpenSelected";
            this.mniOpenSelected.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.mniOpenSelected.Size = new System.Drawing.Size(236, 22);
            this.mniOpenSelected.Text = "&Open Selected Activity";
            this.mniOpenSelected.Click += new System.EventHandler(this.mniOpenSelected_Click);
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(640, 399);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(148, 23);
            this.btnRefresh.TabIndex = 1;
            this.btnRefresh.Text = "Refresh Batch";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // btnNext
            // 
            this.btnNext.Location = new System.Drawing.Point(343, 399);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(148, 23);
            this.btnNext.TabIndex = 1;
            this.btnNext.Text = "Next Batch";
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // btnPrevious
            // 
            this.btnPrevious.Location = new System.Drawing.Point(177, 399);
            this.btnPrevious.Name = "btnPrevious";
            this.btnPrevious.Size = new System.Drawing.Size(148, 23);
            this.btnPrevious.TabIndex = 1;
            this.btnPrevious.Text = "Previous Batch";
            this.btnPrevious.UseVisualStyleBackColor = true;
            this.btnPrevious.Click += new System.EventHandler(this.btnPrevious_Click);
            // 
            // lblActivities
            // 
            this.lblActivities.AutoSize = true;
            this.lblActivities.Location = new System.Drawing.Point(12, 9);
            this.lblActivities.Name = "lblActivities";
            this.lblActivities.Size = new System.Drawing.Size(55, 13);
            this.lblActivities.TabIndex = 0;
            this.lblActivities.Text = "Activities: ";
            // 
            // mniClearCache
            // 
            this.mniClearCache.Name = "mniClearCache";
            this.mniClearCache.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.C)));
            this.mniClearCache.Size = new System.Drawing.Size(236, 22);
            this.mniClearCache.Text = "&Clear Cache";
            this.mniClearCache.Click += new System.EventHandler(this.mniClearCache_Click);
            // 
            // mniSignout
            // 
            this.mniSignout.Name = "mniSignout";
            this.mniSignout.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.Q)));
            this.mniSignout.Size = new System.Drawing.Size(236, 22);
            this.mniSignout.Text = "Log &Out";
            this.mniSignout.Click += new System.EventHandler(this.mniSignout_Click);
            // 
            // GCDownloader
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.ssStatus);
            this.Controls.Add(this.panelMain);
            this.Controls.Add(this.panelLogin);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "GCDownloader";
            this.Text = "GarminConnect Downloader";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.GCDownloader_FormClosing);
            this.Load += new System.EventHandler(this.GCDownloader_Load);
            this.ssStatus.ResumeLayout(false);
            this.ssStatus.PerformLayout();
            this.panelLogin.ResumeLayout(false);
            this.panelLogin.PerformLayout();
            this.panelMain.ResumeLayout(false);
            this.panelMain.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numBatchSize)).EndInit();
            this.cmnMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.StatusStrip ssStatus;
        private System.Windows.Forms.ToolStripStatusLabel statusMessage;
        private System.Windows.Forms.Panel panelLogin;
        private System.Windows.Forms.Button btnLogin;
        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.Label lblUsername;
        private System.Windows.Forms.Panel panelMain;
        private System.Windows.Forms.ListBox lstActivities;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.Button btnPrevious;
        private System.Windows.Forms.Label lblActivities;
        private System.Windows.Forms.NumericUpDown numBatchSize;
        private System.Windows.Forms.Label lblBatchSize;
        private System.Windows.Forms.CheckBox chkSavePassword;
        private System.Windows.Forms.CheckBox chkSaveUsername;
        private System.Windows.Forms.ContextMenuStrip cmnMenu;
        private System.Windows.Forms.ToolStripMenuItem mniSetDownloadFolder;
        private System.Windows.Forms.ToolStripMenuItem mniSelectAll;
        private System.Windows.Forms.ToolStripMenuItem mniDownloadSelected;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ToolStripMenuItem mniRefresh;
        private System.Windows.Forms.TextBox txtActivityType;
        private System.Windows.Forms.TextBox txtActivityTime;
        private System.Windows.Forms.TextBox txtActivityName;
        private System.Windows.Forms.TextBox txtActivityID;
        private System.Windows.Forms.Label lblActivityType;
        private System.Windows.Forms.Label lblActivityTime;
        private System.Windows.Forms.Label lblActivityName;
        private System.Windows.Forms.Label lblActivityID;
        private System.Windows.Forms.TextBox txtEventType;
        private System.Windows.Forms.Label lblEventType;
        private System.Windows.Forms.TextBox txtSteps;
        private System.Windows.Forms.TextBox txtCalories;
        private System.Windows.Forms.TextBox txtDuration;
        private System.Windows.Forms.TextBox txtDistance;
        private System.Windows.Forms.Label lblSteps;
        private System.Windows.Forms.Label lblCalories;
        private System.Windows.Forms.Label lblDuration;
        private System.Windows.Forms.Label lblDistance;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ToolStripMenuItem mniOpenSelected;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.ToolStripMenuItem mniClearCache;
        private System.Windows.Forms.ToolStripMenuItem mniSignout;
    }
}

