namespace AoMModelEditor
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.mainTabControl = new System.Windows.Forms.TabControl();
            this.meshInfoTabPage = new System.Windows.Forms.TabPage();
            this.flagsPanel = new System.Windows.Forms.Panel();
            this.brgObjectDetailsGroupBox = new System.Windows.Forms.GroupBox();
            this.brgObjectListView = new BrightIdeasSoftware.ObjectListView();
            this.genDataPanel = new System.Windows.Forms.Panel();
            this.brgObjectsGroupBox = new System.Windows.Forms.GroupBox();
            this.brgObjectsTreeListView = new BrightIdeasSoftware.TreeListView();
            this.generalDataGroupBox = new System.Windows.Forms.GroupBox();
            this.brgImportGroupBox = new System.Windows.Forms.GroupBox();
            this.brgImportCenterModelCheckBox = new System.Windows.Forms.CheckBox();
            this.brgImportAttachScaleCheckBox = new System.Windows.Forms.CheckBox();
            this.grnSettingsTabPage = new System.Windows.Forms.TabPage();
            this.grnSettingsMainPanel = new System.Windows.Forms.Panel();
            this.grnPropsGroupBox = new System.Windows.Forms.GroupBox();
            this.grnObjectListView = new BrightIdeasSoftware.ObjectListView();
            this.grnSettingsSidePanel = new System.Windows.Forms.Panel();
            this.grnObjectsGroupBox = new System.Windows.Forms.GroupBox();
            this.grnObjectsTreeListView = new BrightIdeasSoftware.TreeListView();
            this.grnSettingsGroupBox = new System.Windows.Forms.GroupBox();
            this.grnExportGroupBox = new System.Windows.Forms.GroupBox();
            this.grnExportAnimCheckBox = new System.Windows.Forms.CheckBox();
            this.grnExportModelCheckBox = new System.Windows.Forms.CheckBox();
            this.mainMenuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.maxToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.readMeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.beginnersGuideToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.brgSettingsInfoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sourceCodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.mainStatusStrip = new System.Windows.Forms.StatusStrip();
            this.vertsToolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.vertsValueToolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.facesToolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.facesValueToolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.meshesToolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.meshesValueToolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.matsToolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.matsValueToolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.animLengthToolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.animLengthValueToolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.mainTabControl.SuspendLayout();
            this.meshInfoTabPage.SuspendLayout();
            this.flagsPanel.SuspendLayout();
            this.brgObjectDetailsGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.brgObjectListView)).BeginInit();
            this.genDataPanel.SuspendLayout();
            this.brgObjectsGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.brgObjectsTreeListView)).BeginInit();
            this.generalDataGroupBox.SuspendLayout();
            this.brgImportGroupBox.SuspendLayout();
            this.grnSettingsTabPage.SuspendLayout();
            this.grnSettingsMainPanel.SuspendLayout();
            this.grnPropsGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grnObjectListView)).BeginInit();
            this.grnSettingsSidePanel.SuspendLayout();
            this.grnObjectsGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grnObjectsTreeListView)).BeginInit();
            this.grnSettingsGroupBox.SuspendLayout();
            this.grnExportGroupBox.SuspendLayout();
            this.mainMenuStrip.SuspendLayout();
            this.mainStatusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.AddExtension = false;
            this.saveFileDialog.Filter = "brg files|*.brg|grn files|*.grn";
            // 
            // mainTabControl
            // 
            this.mainTabControl.Controls.Add(this.meshInfoTabPage);
            this.mainTabControl.Controls.Add(this.grnSettingsTabPage);
            this.mainTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainTabControl.Location = new System.Drawing.Point(0, 53);
            this.mainTabControl.Margin = new System.Windows.Forms.Padding(4);
            this.mainTabControl.Name = "mainTabControl";
            this.mainTabControl.SelectedIndex = 0;
            this.mainTabControl.Size = new System.Drawing.Size(1045, 639);
            this.mainTabControl.TabIndex = 3;
            this.mainTabControl.SelectedIndexChanged += new System.EventHandler(this.mainTabControl_SelectedIndexChanged);
            // 
            // meshInfoTabPage
            // 
            this.meshInfoTabPage.Controls.Add(this.flagsPanel);
            this.meshInfoTabPage.Controls.Add(this.genDataPanel);
            this.meshInfoTabPage.Location = new System.Drawing.Point(4, 25);
            this.meshInfoTabPage.Margin = new System.Windows.Forms.Padding(4);
            this.meshInfoTabPage.Name = "meshInfoTabPage";
            this.meshInfoTabPage.Padding = new System.Windows.Forms.Padding(4);
            this.meshInfoTabPage.Size = new System.Drawing.Size(1037, 610);
            this.meshInfoTabPage.TabIndex = 0;
            this.meshInfoTabPage.Text = "Brg Settings";
            this.meshInfoTabPage.UseVisualStyleBackColor = true;
            // 
            // flagsPanel
            // 
            this.flagsPanel.Controls.Add(this.brgObjectDetailsGroupBox);
            this.flagsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flagsPanel.Location = new System.Drawing.Point(267, 4);
            this.flagsPanel.Margin = new System.Windows.Forms.Padding(4);
            this.flagsPanel.Name = "flagsPanel";
            this.flagsPanel.Size = new System.Drawing.Size(766, 602);
            this.flagsPanel.TabIndex = 1;
            // 
            // brgObjectDetailsGroupBox
            // 
            this.brgObjectDetailsGroupBox.Controls.Add(this.brgObjectListView);
            this.brgObjectDetailsGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.brgObjectDetailsGroupBox.Location = new System.Drawing.Point(0, 0);
            this.brgObjectDetailsGroupBox.Margin = new System.Windows.Forms.Padding(4);
            this.brgObjectDetailsGroupBox.Name = "brgObjectDetailsGroupBox";
            this.brgObjectDetailsGroupBox.Padding = new System.Windows.Forms.Padding(4);
            this.brgObjectDetailsGroupBox.Size = new System.Drawing.Size(766, 602);
            this.brgObjectDetailsGroupBox.TabIndex = 0;
            this.brgObjectDetailsGroupBox.TabStop = false;
            this.brgObjectDetailsGroupBox.Text = "Object Details";
            // 
            // brgObjectListView
            // 
            this.brgObjectListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.brgObjectListView.Location = new System.Drawing.Point(4, 19);
            this.brgObjectListView.Name = "brgObjectListView";
            this.brgObjectListView.Size = new System.Drawing.Size(758, 579);
            this.brgObjectListView.TabIndex = 1;
            this.brgObjectListView.UseCompatibleStateImageBehavior = false;
            this.brgObjectListView.View = System.Windows.Forms.View.Details;
            // 
            // genDataPanel
            // 
            this.genDataPanel.Controls.Add(this.brgObjectsGroupBox);
            this.genDataPanel.Controls.Add(this.generalDataGroupBox);
            this.genDataPanel.Dock = System.Windows.Forms.DockStyle.Left;
            this.genDataPanel.Location = new System.Drawing.Point(4, 4);
            this.genDataPanel.Margin = new System.Windows.Forms.Padding(4);
            this.genDataPanel.Name = "genDataPanel";
            this.genDataPanel.Size = new System.Drawing.Size(263, 602);
            this.genDataPanel.TabIndex = 2;
            // 
            // brgObjectsGroupBox
            // 
            this.brgObjectsGroupBox.Controls.Add(this.brgObjectsTreeListView);
            this.brgObjectsGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.brgObjectsGroupBox.Location = new System.Drawing.Point(0, 111);
            this.brgObjectsGroupBox.Margin = new System.Windows.Forms.Padding(4);
            this.brgObjectsGroupBox.Name = "brgObjectsGroupBox";
            this.brgObjectsGroupBox.Padding = new System.Windows.Forms.Padding(4);
            this.brgObjectsGroupBox.Size = new System.Drawing.Size(263, 491);
            this.brgObjectsGroupBox.TabIndex = 1;
            this.brgObjectsGroupBox.TabStop = false;
            this.brgObjectsGroupBox.Text = "Objects";
            // 
            // brgObjectsTreeListView
            // 
            this.brgObjectsTreeListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.brgObjectsTreeListView.Location = new System.Drawing.Point(4, 19);
            this.brgObjectsTreeListView.Name = "brgObjectsTreeListView";
            this.brgObjectsTreeListView.OwnerDraw = true;
            this.brgObjectsTreeListView.ShowGroups = false;
            this.brgObjectsTreeListView.Size = new System.Drawing.Size(255, 468);
            this.brgObjectsTreeListView.TabIndex = 0;
            this.brgObjectsTreeListView.UseCompatibleStateImageBehavior = false;
            this.brgObjectsTreeListView.View = System.Windows.Forms.View.Details;
            this.brgObjectsTreeListView.VirtualMode = true;
            this.brgObjectsTreeListView.SelectionChanged += new System.EventHandler(this.brgObjectsTreeListView_SelectionChanged);
            // 
            // generalDataGroupBox
            // 
            this.generalDataGroupBox.Controls.Add(this.brgImportGroupBox);
            this.generalDataGroupBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.generalDataGroupBox.Location = new System.Drawing.Point(0, 0);
            this.generalDataGroupBox.Margin = new System.Windows.Forms.Padding(4);
            this.generalDataGroupBox.Name = "generalDataGroupBox";
            this.generalDataGroupBox.Padding = new System.Windows.Forms.Padding(4);
            this.generalDataGroupBox.Size = new System.Drawing.Size(263, 111);
            this.generalDataGroupBox.TabIndex = 0;
            this.generalDataGroupBox.TabStop = false;
            this.generalDataGroupBox.Text = "Settings";
            // 
            // brgImportGroupBox
            // 
            this.brgImportGroupBox.Controls.Add(this.brgImportCenterModelCheckBox);
            this.brgImportGroupBox.Controls.Add(this.brgImportAttachScaleCheckBox);
            this.brgImportGroupBox.Location = new System.Drawing.Point(4, 21);
            this.brgImportGroupBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.brgImportGroupBox.Name = "brgImportGroupBox";
            this.brgImportGroupBox.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.brgImportGroupBox.Size = new System.Drawing.Size(255, 69);
            this.brgImportGroupBox.TabIndex = 19;
            this.brgImportGroupBox.TabStop = false;
            this.brgImportGroupBox.Text = "Import";
            // 
            // brgImportCenterModelCheckBox
            // 
            this.brgImportCenterModelCheckBox.AutoSize = true;
            this.brgImportCenterModelCheckBox.Location = new System.Drawing.Point(5, 44);
            this.brgImportCenterModelCheckBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.brgImportCenterModelCheckBox.Name = "brgImportCenterModelCheckBox";
            this.brgImportCenterModelCheckBox.Size = new System.Drawing.Size(130, 21);
            this.brgImportCenterModelCheckBox.TabIndex = 1;
            this.brgImportCenterModelCheckBox.Text = "Model at Center";
            // 
            // brgImportAttachScaleCheckBox
            // 
            this.brgImportAttachScaleCheckBox.AutoSize = true;
            this.brgImportAttachScaleCheckBox.Location = new System.Drawing.Point(5, 20);
            this.brgImportAttachScaleCheckBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.brgImportAttachScaleCheckBox.Name = "brgImportAttachScaleCheckBox";
            this.brgImportAttachScaleCheckBox.Size = new System.Drawing.Size(193, 21);
            this.brgImportAttachScaleCheckBox.TabIndex = 0;
            this.brgImportAttachScaleCheckBox.Text = "Uniform Attachpoint Scale";
            // 
            // grnSettingsTabPage
            // 
            this.grnSettingsTabPage.Controls.Add(this.grnSettingsMainPanel);
            this.grnSettingsTabPage.Controls.Add(this.grnSettingsSidePanel);
            this.grnSettingsTabPage.Location = new System.Drawing.Point(4, 25);
            this.grnSettingsTabPage.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.grnSettingsTabPage.Name = "grnSettingsTabPage";
            this.grnSettingsTabPage.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.grnSettingsTabPage.Size = new System.Drawing.Size(1037, 610);
            this.grnSettingsTabPage.TabIndex = 2;
            this.grnSettingsTabPage.Text = "Grn Settings";
            this.grnSettingsTabPage.UseVisualStyleBackColor = true;
            // 
            // grnSettingsMainPanel
            // 
            this.grnSettingsMainPanel.Controls.Add(this.grnPropsGroupBox);
            this.grnSettingsMainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grnSettingsMainPanel.Location = new System.Drawing.Point(266, 2);
            this.grnSettingsMainPanel.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.grnSettingsMainPanel.Name = "grnSettingsMainPanel";
            this.grnSettingsMainPanel.Size = new System.Drawing.Size(768, 606);
            this.grnSettingsMainPanel.TabIndex = 2;
            // 
            // grnPropsGroupBox
            // 
            this.grnPropsGroupBox.Controls.Add(this.grnObjectListView);
            this.grnPropsGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grnPropsGroupBox.Location = new System.Drawing.Point(0, 0);
            this.grnPropsGroupBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.grnPropsGroupBox.Name = "grnPropsGroupBox";
            this.grnPropsGroupBox.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.grnPropsGroupBox.Size = new System.Drawing.Size(768, 606);
            this.grnPropsGroupBox.TabIndex = 1;
            this.grnPropsGroupBox.TabStop = false;
            this.grnPropsGroupBox.Text = "Object Details";
            // 
            // grnObjectListView
            // 
            this.grnObjectListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grnObjectListView.Location = new System.Drawing.Point(3, 17);
            this.grnObjectListView.Name = "grnObjectListView";
            this.grnObjectListView.Size = new System.Drawing.Size(762, 587);
            this.grnObjectListView.TabIndex = 0;
            this.grnObjectListView.UseCompatibleStateImageBehavior = false;
            this.grnObjectListView.View = System.Windows.Forms.View.Details;
            // 
            // grnSettingsSidePanel
            // 
            this.grnSettingsSidePanel.Controls.Add(this.grnObjectsGroupBox);
            this.grnSettingsSidePanel.Controls.Add(this.grnSettingsGroupBox);
            this.grnSettingsSidePanel.Dock = System.Windows.Forms.DockStyle.Left;
            this.grnSettingsSidePanel.Location = new System.Drawing.Point(3, 2);
            this.grnSettingsSidePanel.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.grnSettingsSidePanel.Name = "grnSettingsSidePanel";
            this.grnSettingsSidePanel.Size = new System.Drawing.Size(263, 606);
            this.grnSettingsSidePanel.TabIndex = 1;
            // 
            // grnObjectsGroupBox
            // 
            this.grnObjectsGroupBox.Controls.Add(this.grnObjectsTreeListView);
            this.grnObjectsGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grnObjectsGroupBox.Location = new System.Drawing.Point(0, 85);
            this.grnObjectsGroupBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.grnObjectsGroupBox.Name = "grnObjectsGroupBox";
            this.grnObjectsGroupBox.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.grnObjectsGroupBox.Size = new System.Drawing.Size(263, 521);
            this.grnObjectsGroupBox.TabIndex = 1;
            this.grnObjectsGroupBox.TabStop = false;
            this.grnObjectsGroupBox.Text = "Objects";
            // 
            // grnObjectsTreeListView
            // 
            this.grnObjectsTreeListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grnObjectsTreeListView.Location = new System.Drawing.Point(3, 17);
            this.grnObjectsTreeListView.Name = "grnObjectsTreeListView";
            this.grnObjectsTreeListView.OwnerDraw = true;
            this.grnObjectsTreeListView.ShowGroups = false;
            this.grnObjectsTreeListView.Size = new System.Drawing.Size(257, 502);
            this.grnObjectsTreeListView.TabIndex = 0;
            this.grnObjectsTreeListView.UseCompatibleStateImageBehavior = false;
            this.grnObjectsTreeListView.View = System.Windows.Forms.View.Details;
            this.grnObjectsTreeListView.VirtualMode = true;
            // 
            // grnSettingsGroupBox
            // 
            this.grnSettingsGroupBox.Controls.Add(this.grnExportGroupBox);
            this.grnSettingsGroupBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.grnSettingsGroupBox.Location = new System.Drawing.Point(0, 0);
            this.grnSettingsGroupBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.grnSettingsGroupBox.Name = "grnSettingsGroupBox";
            this.grnSettingsGroupBox.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.grnSettingsGroupBox.Size = new System.Drawing.Size(263, 85);
            this.grnSettingsGroupBox.TabIndex = 0;
            this.grnSettingsGroupBox.TabStop = false;
            this.grnSettingsGroupBox.Text = "Settings";
            // 
            // grnExportGroupBox
            // 
            this.grnExportGroupBox.Controls.Add(this.grnExportAnimCheckBox);
            this.grnExportGroupBox.Controls.Add(this.grnExportModelCheckBox);
            this.grnExportGroupBox.Location = new System.Drawing.Point(5, 19);
            this.grnExportGroupBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.grnExportGroupBox.Name = "grnExportGroupBox";
            this.grnExportGroupBox.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.grnExportGroupBox.Size = new System.Drawing.Size(252, 47);
            this.grnExportGroupBox.TabIndex = 0;
            this.grnExportGroupBox.TabStop = false;
            this.grnExportGroupBox.Text = "Export";
            // 
            // grnExportAnimCheckBox
            // 
            this.grnExportAnimCheckBox.AutoSize = true;
            this.grnExportAnimCheckBox.Location = new System.Drawing.Point(80, 20);
            this.grnExportAnimCheckBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.grnExportAnimCheckBox.Name = "grnExportAnimCheckBox";
            this.grnExportAnimCheckBox.Size = new System.Drawing.Size(92, 21);
            this.grnExportAnimCheckBox.TabIndex = 1;
            this.grnExportAnimCheckBox.Text = "Animation";
            this.grnExportAnimCheckBox.UseVisualStyleBackColor = true;
            // 
            // grnExportModelCheckBox
            // 
            this.grnExportModelCheckBox.AutoSize = true;
            this.grnExportModelCheckBox.Location = new System.Drawing.Point(5, 20);
            this.grnExportModelCheckBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.grnExportModelCheckBox.Name = "grnExportModelCheckBox";
            this.grnExportModelCheckBox.Size = new System.Drawing.Size(68, 21);
            this.grnExportModelCheckBox.TabIndex = 0;
            this.grnExportModelCheckBox.Text = "Model";
            this.grnExportModelCheckBox.UseVisualStyleBackColor = true;
            // 
            // mainMenuStrip
            // 
            this.mainMenuStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.mainMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.maxToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.mainMenuStrip.Location = new System.Drawing.Point(0, 0);
            this.mainMenuStrip.Name = "mainMenuStrip";
            this.mainMenuStrip.Padding = new System.Windows.Forms.Padding(8, 2, 0, 2);
            this.mainMenuStrip.Size = new System.Drawing.Size(1045, 28);
            this.mainMenuStrip.TabIndex = 2;
            this.mainMenuStrip.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(44, 24);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("newToolStripMenuItem.Image")));
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(179, 26);
            this.newToolStripMenuItem.Text = "New";
            this.newToolStripMenuItem.Click += new System.EventHandler(this.newToolStripMenuItem_Click);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("openToolStripMenuItem.Image")));
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(118, 26);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("saveToolStripMenuItem.Image")));
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(118, 26);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // maxToolStripMenuItem
            // 
            this.maxToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.importToolStripMenuItem,
            this.exportToolStripMenuItem});
            this.maxToolStripMenuItem.Name = "maxToolStripMenuItem";
            this.maxToolStripMenuItem.Size = new System.Drawing.Size(72, 24);
            this.maxToolStripMenuItem.Text = "Collada";
            // 
            // importToolStripMenuItem
            // 
            this.importToolStripMenuItem.Name = "importToolStripMenuItem";
            this.importToolStripMenuItem.Size = new System.Drawing.Size(123, 24);
            this.importToolStripMenuItem.Text = "Import";
            this.importToolStripMenuItem.Click += new System.EventHandler(this.importToolStripMenuItem_Click);
            // 
            // exportToolStripMenuItem
            // 
            this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
            this.exportToolStripMenuItem.Size = new System.Drawing.Size(123, 24);
            this.exportToolStripMenuItem.Text = "Export";
            this.exportToolStripMenuItem.Click += new System.EventHandler(this.exportToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.readMeToolStripMenuItem,
            this.beginnersGuideToolStripMenuItem,
            this.brgSettingsInfoToolStripMenuItem,
            this.sourceCodeToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(53, 24);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // readMeToolStripMenuItem
            // 
            this.readMeToolStripMenuItem.Name = "readMeToolStripMenuItem";
            this.readMeToolStripMenuItem.Size = new System.Drawing.Size(189, 24);
            this.readMeToolStripMenuItem.Text = "ReadMe";
            this.readMeToolStripMenuItem.Click += new System.EventHandler(this.readMeToolStripMenuItem_Click);
            // 
            // beginnersGuideToolStripMenuItem
            // 
            this.beginnersGuideToolStripMenuItem.Name = "beginnersGuideToolStripMenuItem";
            this.beginnersGuideToolStripMenuItem.Size = new System.Drawing.Size(189, 24);
            this.beginnersGuideToolStripMenuItem.Text = "Beginner\'s Guide";
            this.beginnersGuideToolStripMenuItem.Click += new System.EventHandler(this.beginnersGuideToolStripMenuItem_Click);
            // 
            // brgSettingsInfoToolStripMenuItem
            // 
            this.brgSettingsInfoToolStripMenuItem.Name = "brgSettingsInfoToolStripMenuItem";
            this.brgSettingsInfoToolStripMenuItem.Size = new System.Drawing.Size(189, 24);
            this.brgSettingsInfoToolStripMenuItem.Text = "Brg Settings Info";
            this.brgSettingsInfoToolStripMenuItem.Click += new System.EventHandler(this.brgSettingsInfoToolStripMenuItem_Click);
            // 
            // sourceCodeToolStripMenuItem
            // 
            this.sourceCodeToolStripMenuItem.Name = "sourceCodeToolStripMenuItem";
            this.sourceCodeToolStripMenuItem.Size = new System.Drawing.Size(189, 24);
            this.sourceCodeToolStripMenuItem.Text = "Source Code";
            this.sourceCodeToolStripMenuItem.Click += new System.EventHandler(this.sourceCodeToolStripMenuItem_Click);
            // 
            // openFileDialog
            // 
            this.openFileDialog.Filter = "brg files|*.brg|grn files|*.grn";
            // 
            // mainStatusStrip
            // 
            this.mainStatusStrip.Dock = System.Windows.Forms.DockStyle.Top;
            this.mainStatusStrip.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.mainStatusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.vertsToolStripStatusLabel,
            this.vertsValueToolStripStatusLabel,
            this.facesToolStripStatusLabel,
            this.facesValueToolStripStatusLabel,
            this.meshesToolStripStatusLabel,
            this.meshesValueToolStripStatusLabel,
            this.matsToolStripStatusLabel,
            this.matsValueToolStripStatusLabel,
            this.animLengthToolStripStatusLabel,
            this.animLengthValueToolStripStatusLabel});
            this.mainStatusStrip.Location = new System.Drawing.Point(0, 28);
            this.mainStatusStrip.Name = "mainStatusStrip";
            this.mainStatusStrip.Padding = new System.Windows.Forms.Padding(1, 0, 12, 0);
            this.mainStatusStrip.Size = new System.Drawing.Size(1045, 25);
            this.mainStatusStrip.SizingGrip = false;
            this.mainStatusStrip.TabIndex = 4;
            this.mainStatusStrip.Text = "mainStatusStrip";
            // 
            // vertsToolStripStatusLabel
            // 
            this.vertsToolStripStatusLabel.Name = "vertsToolStripStatusLabel";
            this.vertsToolStripStatusLabel.Size = new System.Drawing.Size(45, 20);
            this.vertsToolStripStatusLabel.Text = "Verts:";
            // 
            // vertsValueToolStripStatusLabel
            // 
            this.vertsValueToolStripStatusLabel.Name = "vertsValueToolStripStatusLabel";
            this.vertsValueToolStripStatusLabel.Size = new System.Drawing.Size(17, 20);
            this.vertsValueToolStripStatusLabel.Text = "0";
            // 
            // facesToolStripStatusLabel
            // 
            this.facesToolStripStatusLabel.Name = "facesToolStripStatusLabel";
            this.facesToolStripStatusLabel.Size = new System.Drawing.Size(48, 20);
            this.facesToolStripStatusLabel.Text = "Faces:";
            // 
            // facesValueToolStripStatusLabel
            // 
            this.facesValueToolStripStatusLabel.Name = "facesValueToolStripStatusLabel";
            this.facesValueToolStripStatusLabel.Size = new System.Drawing.Size(17, 20);
            this.facesValueToolStripStatusLabel.Text = "0";
            // 
            // meshesToolStripStatusLabel
            // 
            this.meshesToolStripStatusLabel.Name = "meshesToolStripStatusLabel";
            this.meshesToolStripStatusLabel.Size = new System.Drawing.Size(61, 20);
            this.meshesToolStripStatusLabel.Text = "Meshes:";
            // 
            // meshesValueToolStripStatusLabel
            // 
            this.meshesValueToolStripStatusLabel.Name = "meshesValueToolStripStatusLabel";
            this.meshesValueToolStripStatusLabel.Size = new System.Drawing.Size(17, 20);
            this.meshesValueToolStripStatusLabel.Text = "0";
            // 
            // matsToolStripStatusLabel
            // 
            this.matsToolStripStatusLabel.Name = "matsToolStripStatusLabel";
            this.matsToolStripStatusLabel.Size = new System.Drawing.Size(44, 20);
            this.matsToolStripStatusLabel.Text = "Mats:";
            // 
            // matsValueToolStripStatusLabel
            // 
            this.matsValueToolStripStatusLabel.Name = "matsValueToolStripStatusLabel";
            this.matsValueToolStripStatusLabel.Size = new System.Drawing.Size(17, 20);
            this.matsValueToolStripStatusLabel.Text = "0";
            // 
            // animLengthToolStripStatusLabel
            // 
            this.animLengthToolStripStatusLabel.Name = "animLengthToolStripStatusLabel";
            this.animLengthToolStripStatusLabel.Size = new System.Drawing.Size(130, 20);
            this.animLengthToolStripStatusLabel.Text = "Animation Length:";
            // 
            // animLengthValueToolStripStatusLabel
            // 
            this.animLengthValueToolStripStatusLabel.Name = "animLengthValueToolStripStatusLabel";
            this.animLengthValueToolStripStatusLabel.Size = new System.Drawing.Size(17, 20);
            this.animLengthValueToolStripStatusLabel.Text = "0";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1045, 692);
            this.Controls.Add(this.mainTabControl);
            this.Controls.Add(this.mainStatusStrip);
            this.Controls.Add(this.mainMenuStrip);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "MainForm";
            this.Text = "AoM Model Editor";
            this.mainTabControl.ResumeLayout(false);
            this.meshInfoTabPage.ResumeLayout(false);
            this.flagsPanel.ResumeLayout(false);
            this.brgObjectDetailsGroupBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.brgObjectListView)).EndInit();
            this.genDataPanel.ResumeLayout(false);
            this.brgObjectsGroupBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.brgObjectsTreeListView)).EndInit();
            this.generalDataGroupBox.ResumeLayout(false);
            this.brgImportGroupBox.ResumeLayout(false);
            this.brgImportGroupBox.PerformLayout();
            this.grnSettingsTabPage.ResumeLayout(false);
            this.grnSettingsMainPanel.ResumeLayout(false);
            this.grnPropsGroupBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grnObjectListView)).EndInit();
            this.grnSettingsSidePanel.ResumeLayout(false);
            this.grnObjectsGroupBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grnObjectsTreeListView)).EndInit();
            this.grnSettingsGroupBox.ResumeLayout(false);
            this.grnExportGroupBox.ResumeLayout(false);
            this.grnExportGroupBox.PerformLayout();
            this.mainMenuStrip.ResumeLayout(false);
            this.mainMenuStrip.PerformLayout();
            this.mainStatusStrip.ResumeLayout(false);
            this.mainStatusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.TabControl mainTabControl;
        private System.Windows.Forms.TabPage meshInfoTabPage;
        private System.Windows.Forms.Panel flagsPanel;
        private System.Windows.Forms.GroupBox brgObjectDetailsGroupBox;
        private System.Windows.Forms.Panel genDataPanel;
        private System.Windows.Forms.GroupBox generalDataGroupBox;
        private System.Windows.Forms.GroupBox brgObjectsGroupBox;
        private System.Windows.Forms.MenuStrip mainMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
        private System.Windows.Forms.ToolStripMenuItem maxToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem beginnersGuideToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem brgSettingsInfoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sourceCodeToolStripMenuItem;
        private System.Windows.Forms.TabPage grnSettingsTabPage;
        private System.Windows.Forms.StatusStrip mainStatusStrip;
        private System.Windows.Forms.ToolStripStatusLabel vertsToolStripStatusLabel;
        public System.Windows.Forms.ToolStripStatusLabel vertsValueToolStripStatusLabel;
        private System.Windows.Forms.ToolStripStatusLabel facesToolStripStatusLabel;
        public System.Windows.Forms.ToolStripStatusLabel facesValueToolStripStatusLabel;
        private System.Windows.Forms.ToolStripStatusLabel meshesToolStripStatusLabel;
        public System.Windows.Forms.ToolStripStatusLabel meshesValueToolStripStatusLabel;
        private System.Windows.Forms.ToolStripStatusLabel matsToolStripStatusLabel;
        public System.Windows.Forms.ToolStripStatusLabel matsValueToolStripStatusLabel;
        private System.Windows.Forms.ToolStripStatusLabel animLengthToolStripStatusLabel;
        public System.Windows.Forms.ToolStripStatusLabel animLengthValueToolStripStatusLabel;
        private System.Windows.Forms.Panel grnSettingsSidePanel;
        private System.Windows.Forms.Panel grnSettingsMainPanel;
        private System.Windows.Forms.GroupBox grnSettingsGroupBox;
        private System.Windows.Forms.GroupBox grnExportGroupBox;
        public System.Windows.Forms.CheckBox grnExportAnimCheckBox;
        public System.Windows.Forms.CheckBox grnExportModelCheckBox;
        private System.Windows.Forms.GroupBox grnPropsGroupBox;
        private System.Windows.Forms.GroupBox grnObjectsGroupBox;
        private System.Windows.Forms.ToolStripMenuItem readMeToolStripMenuItem;
        private System.Windows.Forms.GroupBox brgImportGroupBox;
        public System.Windows.Forms.CheckBox brgImportCenterModelCheckBox;
        public System.Windows.Forms.CheckBox brgImportAttachScaleCheckBox;
        public BrightIdeasSoftware.TreeListView brgObjectsTreeListView;
        public BrightIdeasSoftware.ObjectListView brgObjectListView;
        public BrightIdeasSoftware.TreeListView grnObjectsTreeListView;
        public BrightIdeasSoftware.ObjectListView grnObjectListView;
    }
}