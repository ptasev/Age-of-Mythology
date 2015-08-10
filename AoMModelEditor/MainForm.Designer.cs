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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.specularLevelLabel = new System.Windows.Forms.Label();
            this.materialPanel = new System.Windows.Forms.Panel();
            this.materialGroupBox = new System.Windows.Forms.GroupBox();
            this.extractMatButton = new System.Windows.Forms.Button();
            this.specularLevelMaxTextBox = new System.Windows.Forms.TextBox();
            this.opacityLabel = new System.Windows.Forms.Label();
            this.opacityMaxTextBox = new System.Windows.Forms.TextBox();
            this.unkLabel = new System.Windows.Forms.Label();
            this.bumpMapMaxTextBox = new System.Windows.Forms.TextBox();
            this.reflectionLabel = new System.Windows.Forms.Label();
            this.reflectionMaxTextBox = new System.Windows.Forms.TextBox();
            this.textureLabel = new System.Windows.Forms.Label();
            this.textureMaxTextBox = new System.Windows.Forms.TextBox();
            this.selfIllumMaxTextBox = new System.Windows.Forms.TextBox();
            this.specularMaxTextBox = new System.Windows.Forms.TextBox();
            this.ambientMaxTextBox = new System.Windows.Forms.TextBox();
            this.diffuseMaxTextBox = new System.Windows.Forms.TextBox();
            this.materialFlagsGroupBox = new System.Windows.Forms.GroupBox();
            this.materialFlagsCheckedListBox = new System.Windows.Forms.CheckedListBox();
            this.materialSidePanel = new System.Windows.Forms.Panel();
            this.materialSideGroupBox = new System.Windows.Forms.GroupBox();
            this.materialListBox = new System.Windows.Forms.ListBox();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.attachpointListBox = new System.Windows.Forms.ListBox();
            this.mainTabControl = new System.Windows.Forms.TabControl();
            this.meshInfoTabPage = new System.Windows.Forms.TabPage();
            this.flagsPanel = new System.Windows.Forms.Panel();
            this.genMeshFormatGroupBox = new System.Windows.Forms.GroupBox();
            this.genMeshFormatCheckedListBox = new System.Windows.Forms.CheckedListBox();
            this.genMeshFlagsGroupBox = new System.Windows.Forms.GroupBox();
            this.genMeshFlagsCheckedListBox = new System.Windows.Forms.CheckedListBox();
            this.brgFlagsBottomPanel = new System.Windows.Forms.Panel();
            this.interpTypeGroupBox = new System.Windows.Forms.GroupBox();
            this.interpolationTypeCheckBox = new System.Windows.Forms.CheckBox();
            this.genMeshAnimTypeGroupBox = new System.Windows.Forms.GroupBox();
            this.skinBoneRadioButton = new System.Windows.Forms.RadioButton();
            this.nonuniRadioButton = new System.Windows.Forms.RadioButton();
            this.keyframeRadioButton = new System.Windows.Forms.RadioButton();
            this.genDataPanel = new System.Windows.Forms.Panel();
            this.generalDataGroupBox = new System.Windows.Forms.GroupBox();
            this.brgImportGroupBox = new System.Windows.Forms.GroupBox();
            this.brgImportCenterModelCheckBox = new System.Windows.Forms.CheckBox();
            this.brgImportAttachScaleCheckBox = new System.Windows.Forms.CheckBox();
            this.attachpointGroupBox = new System.Windows.Forms.GroupBox();
            this.materialTabPage = new System.Windows.Forms.TabPage();
            this.grnSettingsTabPage = new System.Windows.Forms.TabPage();
            this.grnSettingsMainPanel = new System.Windows.Forms.Panel();
            this.grnPropsGroupBox = new System.Windows.Forms.GroupBox();
            this.grnPropsListBox = new System.Windows.Forms.ListBox();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.grnSettingsSidePanel = new System.Windows.Forms.Panel();
            this.grnSettingsGroupBox = new System.Windows.Forms.GroupBox();
            this.grnExportGroupBox = new System.Windows.Forms.GroupBox();
            this.grnExportAnimCheckBox = new System.Windows.Forms.CheckBox();
            this.grnExportModelCheckBox = new System.Windows.Forms.CheckBox();
            this.grnObjectsGroupBox = new System.Windows.Forms.GroupBox();
            this.grnObjectsListBox = new System.Windows.Forms.ListBox();
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
            this.grnTestToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openGrnTestToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportGrnTestToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveGrnTestToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
            this.materialPanel.SuspendLayout();
            this.materialGroupBox.SuspendLayout();
            this.materialFlagsGroupBox.SuspendLayout();
            this.materialSidePanel.SuspendLayout();
            this.materialSideGroupBox.SuspendLayout();
            this.mainTabControl.SuspendLayout();
            this.meshInfoTabPage.SuspendLayout();
            this.flagsPanel.SuspendLayout();
            this.genMeshFormatGroupBox.SuspendLayout();
            this.genMeshFlagsGroupBox.SuspendLayout();
            this.brgFlagsBottomPanel.SuspendLayout();
            this.interpTypeGroupBox.SuspendLayout();
            this.genMeshAnimTypeGroupBox.SuspendLayout();
            this.genDataPanel.SuspendLayout();
            this.generalDataGroupBox.SuspendLayout();
            this.brgImportGroupBox.SuspendLayout();
            this.attachpointGroupBox.SuspendLayout();
            this.materialTabPage.SuspendLayout();
            this.grnSettingsTabPage.SuspendLayout();
            this.grnSettingsMainPanel.SuspendLayout();
            this.grnPropsGroupBox.SuspendLayout();
            this.grnSettingsSidePanel.SuspendLayout();
            this.grnSettingsGroupBox.SuspendLayout();
            this.grnExportGroupBox.SuspendLayout();
            this.grnObjectsGroupBox.SuspendLayout();
            this.mainMenuStrip.SuspendLayout();
            this.mainStatusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // specularLevelLabel
            // 
            this.specularLevelLabel.AutoSize = true;
            this.specularLevelLabel.Location = new System.Drawing.Point(388, 61);
            this.specularLevelLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.specularLevelLabel.Name = "specularLevelLabel";
            this.specularLevelLabel.Size = new System.Drawing.Size(42, 17);
            this.specularLevelLabel.TabIndex = 13;
            this.specularLevelLabel.Text = "Level";
            // 
            // materialPanel
            // 
            this.materialPanel.Controls.Add(this.materialFlagsGroupBox);
            this.materialPanel.Controls.Add(this.materialGroupBox);
            this.materialPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.materialPanel.Location = new System.Drawing.Point(267, 4);
            this.materialPanel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.materialPanel.Name = "materialPanel";
            this.materialPanel.Size = new System.Drawing.Size(766, 602);
            this.materialPanel.TabIndex = 1;
            // 
            // materialGroupBox
            // 
            this.materialGroupBox.Controls.Add(this.extractMatButton);
            this.materialGroupBox.Controls.Add(this.specularLevelLabel);
            this.materialGroupBox.Controls.Add(this.specularLevelMaxTextBox);
            this.materialGroupBox.Controls.Add(this.opacityLabel);
            this.materialGroupBox.Controls.Add(this.opacityMaxTextBox);
            this.materialGroupBox.Controls.Add(this.unkLabel);
            this.materialGroupBox.Controls.Add(this.bumpMapMaxTextBox);
            this.materialGroupBox.Controls.Add(this.reflectionLabel);
            this.materialGroupBox.Controls.Add(this.reflectionMaxTextBox);
            this.materialGroupBox.Controls.Add(this.textureLabel);
            this.materialGroupBox.Controls.Add(this.textureMaxTextBox);
            this.materialGroupBox.Controls.Add(this.selfIllumMaxTextBox);
            this.materialGroupBox.Controls.Add(this.specularMaxTextBox);
            this.materialGroupBox.Controls.Add(this.ambientMaxTextBox);
            this.materialGroupBox.Controls.Add(this.diffuseMaxTextBox);
            this.materialGroupBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.materialGroupBox.Location = new System.Drawing.Point(0, 0);
            this.materialGroupBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.materialGroupBox.Name = "materialGroupBox";
            this.materialGroupBox.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.materialGroupBox.Size = new System.Drawing.Size(766, 169);
            this.materialGroupBox.TabIndex = 0;
            this.materialGroupBox.TabStop = false;
            this.materialGroupBox.Text = "Information";
            // 
            // extractMatButton
            // 
            this.extractMatButton.Location = new System.Drawing.Point(393, 110);
            this.extractMatButton.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.extractMatButton.Name = "extractMatButton";
            this.extractMatButton.Size = new System.Drawing.Size(339, 33);
            this.extractMatButton.TabIndex = 14;
            this.extractMatButton.Text = "Extract All Materials for EE";
            this.extractMatButton.Click += new System.EventHandler(this.extractMatButton_Click);
            // 
            // specularLevelMaxTextBox
            // 
            this.specularLevelMaxTextBox.Location = new System.Drawing.Point(441, 57);
            this.specularLevelMaxTextBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.specularLevelMaxTextBox.Name = "specularLevelMaxTextBox";
            this.specularLevelMaxTextBox.ReadOnly = true;
            this.specularLevelMaxTextBox.Size = new System.Drawing.Size(117, 22);
            this.specularLevelMaxTextBox.TabIndex = 12;
            // 
            // opacityLabel
            // 
            this.opacityLabel.AutoSize = true;
            this.opacityLabel.Location = new System.Drawing.Point(564, 61);
            this.opacityLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.opacityLabel.Name = "opacityLabel";
            this.opacityLabel.Size = new System.Drawing.Size(56, 17);
            this.opacityLabel.TabIndex = 11;
            this.opacityLabel.Text = "Opacity";
            // 
            // opacityMaxTextBox
            // 
            this.opacityMaxTextBox.Location = new System.Drawing.Point(628, 57);
            this.opacityMaxTextBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.opacityMaxTextBox.Name = "opacityMaxTextBox";
            this.opacityMaxTextBox.ReadOnly = true;
            this.opacityMaxTextBox.Size = new System.Drawing.Size(104, 22);
            this.opacityMaxTextBox.TabIndex = 10;
            // 
            // unkLabel
            // 
            this.unkLabel.AutoSize = true;
            this.unkLabel.Location = new System.Drawing.Point(39, 125);
            this.unkLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.unkLabel.Name = "unkLabel";
            this.unkLabel.Size = new System.Drawing.Size(75, 17);
            this.unkLabel.TabIndex = 9;
            this.unkLabel.Text = "Bump Map";
            // 
            // bumpMapMaxTextBox
            // 
            this.bumpMapMaxTextBox.Location = new System.Drawing.Point(131, 121);
            this.bumpMapMaxTextBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.bumpMapMaxTextBox.Name = "bumpMapMaxTextBox";
            this.bumpMapMaxTextBox.ReadOnly = true;
            this.bumpMapMaxTextBox.Size = new System.Drawing.Size(253, 22);
            this.bumpMapMaxTextBox.TabIndex = 8;
            // 
            // reflectionLabel
            // 
            this.reflectionLabel.AutoSize = true;
            this.reflectionLabel.Location = new System.Drawing.Point(39, 93);
            this.reflectionLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.reflectionLabel.Name = "reflectionLabel";
            this.reflectionLabel.Size = new System.Drawing.Size(71, 17);
            this.reflectionLabel.TabIndex = 7;
            this.reflectionLabel.Text = "Reflection";
            // 
            // reflectionMaxTextBox
            // 
            this.reflectionMaxTextBox.Location = new System.Drawing.Point(131, 89);
            this.reflectionMaxTextBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.reflectionMaxTextBox.Name = "reflectionMaxTextBox";
            this.reflectionMaxTextBox.ReadOnly = true;
            this.reflectionMaxTextBox.Size = new System.Drawing.Size(253, 22);
            this.reflectionMaxTextBox.TabIndex = 6;
            // 
            // textureLabel
            // 
            this.textureLabel.AutoSize = true;
            this.textureLabel.Location = new System.Drawing.Point(39, 61);
            this.textureLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.textureLabel.Name = "textureLabel";
            this.textureLabel.Size = new System.Drawing.Size(83, 17);
            this.textureLabel.TabIndex = 5;
            this.textureLabel.Text = "Diffuse Map";
            // 
            // textureMaxTextBox
            // 
            this.textureMaxTextBox.Location = new System.Drawing.Point(131, 57);
            this.textureMaxTextBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.textureMaxTextBox.Name = "textureMaxTextBox";
            this.textureMaxTextBox.ReadOnly = true;
            this.textureMaxTextBox.Size = new System.Drawing.Size(253, 22);
            this.textureMaxTextBox.TabIndex = 4;
            // 
            // selfIllumMaxTextBox
            // 
            this.selfIllumMaxTextBox.Location = new System.Drawing.Point(567, 25);
            this.selfIllumMaxTextBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.selfIllumMaxTextBox.Name = "selfIllumMaxTextBox";
            this.selfIllumMaxTextBox.ReadOnly = true;
            this.selfIllumMaxTextBox.Size = new System.Drawing.Size(165, 22);
            this.selfIllumMaxTextBox.TabIndex = 3;
            this.selfIllumMaxTextBox.Text = "Self Illumination";
            // 
            // specularMaxTextBox
            // 
            this.specularMaxTextBox.Location = new System.Drawing.Point(393, 25);
            this.specularMaxTextBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.specularMaxTextBox.Name = "specularMaxTextBox";
            this.specularMaxTextBox.ReadOnly = true;
            this.specularMaxTextBox.Size = new System.Drawing.Size(165, 22);
            this.specularMaxTextBox.TabIndex = 2;
            this.specularMaxTextBox.Text = "Specular";
            // 
            // ambientMaxTextBox
            // 
            this.ambientMaxTextBox.Location = new System.Drawing.Point(219, 25);
            this.ambientMaxTextBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.ambientMaxTextBox.Name = "ambientMaxTextBox";
            this.ambientMaxTextBox.ReadOnly = true;
            this.ambientMaxTextBox.Size = new System.Drawing.Size(165, 22);
            this.ambientMaxTextBox.TabIndex = 1;
            this.ambientMaxTextBox.Text = "Ambient";
            // 
            // diffuseMaxTextBox
            // 
            this.diffuseMaxTextBox.Location = new System.Drawing.Point(43, 25);
            this.diffuseMaxTextBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.diffuseMaxTextBox.Name = "diffuseMaxTextBox";
            this.diffuseMaxTextBox.ReadOnly = true;
            this.diffuseMaxTextBox.Size = new System.Drawing.Size(165, 22);
            this.diffuseMaxTextBox.TabIndex = 0;
            this.diffuseMaxTextBox.Text = "Diffuse";
            // 
            // materialFlagsGroupBox
            // 
            this.materialFlagsGroupBox.Controls.Add(this.materialFlagsCheckedListBox);
            this.materialFlagsGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.materialFlagsGroupBox.Location = new System.Drawing.Point(0, 169);
            this.materialFlagsGroupBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.materialFlagsGroupBox.Name = "materialFlagsGroupBox";
            this.materialFlagsGroupBox.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.materialFlagsGroupBox.Size = new System.Drawing.Size(766, 433);
            this.materialFlagsGroupBox.TabIndex = 1;
            this.materialFlagsGroupBox.TabStop = false;
            this.materialFlagsGroupBox.Text = "Flags";
            // 
            // materialFlagsCheckedListBox
            // 
            this.materialFlagsCheckedListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.materialFlagsCheckedListBox.FormattingEnabled = true;
            this.materialFlagsCheckedListBox.Location = new System.Drawing.Point(4, 19);
            this.materialFlagsCheckedListBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.materialFlagsCheckedListBox.Name = "materialFlagsCheckedListBox";
            this.materialFlagsCheckedListBox.Size = new System.Drawing.Size(758, 410);
            this.materialFlagsCheckedListBox.TabIndex = 0;
            this.materialFlagsCheckedListBox.MouseEnter += new System.EventHandler(this.materialFlagsCheckedListBox_MouseEnter);
            // 
            // materialSidePanel
            // 
            this.materialSidePanel.Controls.Add(this.materialSideGroupBox);
            this.materialSidePanel.Dock = System.Windows.Forms.DockStyle.Left;
            this.materialSidePanel.Location = new System.Drawing.Point(4, 4);
            this.materialSidePanel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.materialSidePanel.Name = "materialSidePanel";
            this.materialSidePanel.Size = new System.Drawing.Size(263, 602);
            this.materialSidePanel.TabIndex = 0;
            // 
            // materialSideGroupBox
            // 
            this.materialSideGroupBox.Controls.Add(this.materialListBox);
            this.materialSideGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.materialSideGroupBox.Location = new System.Drawing.Point(0, 0);
            this.materialSideGroupBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.materialSideGroupBox.Name = "materialSideGroupBox";
            this.materialSideGroupBox.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.materialSideGroupBox.Size = new System.Drawing.Size(263, 602);
            this.materialSideGroupBox.TabIndex = 1;
            this.materialSideGroupBox.TabStop = false;
            this.materialSideGroupBox.Text = "Materials";
            // 
            // materialListBox
            // 
            this.materialListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.materialListBox.FormattingEnabled = true;
            this.materialListBox.ItemHeight = 16;
            this.materialListBox.Location = new System.Drawing.Point(3, 17);
            this.materialListBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.materialListBox.Name = "materialListBox";
            this.materialListBox.Size = new System.Drawing.Size(257, 583);
            this.materialListBox.TabIndex = 0;
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.AddExtension = false;
            this.saveFileDialog.Filter = "\"brg files|*.brg|grn files|*.grn\"";
            // 
            // attachpointListBox
            // 
            this.attachpointListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.attachpointListBox.FormattingEnabled = true;
            this.attachpointListBox.ItemHeight = 16;
            this.attachpointListBox.Location = new System.Drawing.Point(4, 19);
            this.attachpointListBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.attachpointListBox.Name = "attachpointListBox";
            this.attachpointListBox.Size = new System.Drawing.Size(255, 468);
            this.attachpointListBox.TabIndex = 1;
            this.attachpointListBox.MouseEnter += new System.EventHandler(this.attachpointListBox_MouseEnter);
            // 
            // mainTabControl
            // 
            this.mainTabControl.Controls.Add(this.meshInfoTabPage);
            this.mainTabControl.Controls.Add(this.materialTabPage);
            this.mainTabControl.Controls.Add(this.grnSettingsTabPage);
            this.mainTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainTabControl.Location = new System.Drawing.Point(0, 53);
            this.mainTabControl.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
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
            this.meshInfoTabPage.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.meshInfoTabPage.Name = "meshInfoTabPage";
            this.meshInfoTabPage.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.meshInfoTabPage.Size = new System.Drawing.Size(1037, 610);
            this.meshInfoTabPage.TabIndex = 0;
            this.meshInfoTabPage.Text = "Brg Settings";
            this.meshInfoTabPage.UseVisualStyleBackColor = true;
            // 
            // flagsPanel
            // 
            this.flagsPanel.Controls.Add(this.genMeshFormatGroupBox);
            this.flagsPanel.Controls.Add(this.genMeshFlagsGroupBox);
            this.flagsPanel.Controls.Add(this.brgFlagsBottomPanel);
            this.flagsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flagsPanel.Location = new System.Drawing.Point(267, 4);
            this.flagsPanel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.flagsPanel.Name = "flagsPanel";
            this.flagsPanel.Size = new System.Drawing.Size(766, 602);
            this.flagsPanel.TabIndex = 1;
            // 
            // genMeshFormatGroupBox
            // 
            this.genMeshFormatGroupBox.Controls.Add(this.genMeshFormatCheckedListBox);
            this.genMeshFormatGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.genMeshFormatGroupBox.Location = new System.Drawing.Point(0, 319);
            this.genMeshFormatGroupBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.genMeshFormatGroupBox.Name = "genMeshFormatGroupBox";
            this.genMeshFormatGroupBox.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.genMeshFormatGroupBox.Size = new System.Drawing.Size(766, 203);
            this.genMeshFormatGroupBox.TabIndex = 1;
            this.genMeshFormatGroupBox.TabStop = false;
            this.genMeshFormatGroupBox.Text = "Mesh Format";
            // 
            // genMeshFormatCheckedListBox
            // 
            this.genMeshFormatCheckedListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.genMeshFormatCheckedListBox.Location = new System.Drawing.Point(4, 19);
            this.genMeshFormatCheckedListBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.genMeshFormatCheckedListBox.Name = "genMeshFormatCheckedListBox";
            this.genMeshFormatCheckedListBox.Size = new System.Drawing.Size(758, 180);
            this.genMeshFormatCheckedListBox.TabIndex = 1;
            // 
            // genMeshFlagsGroupBox
            // 
            this.genMeshFlagsGroupBox.Controls.Add(this.genMeshFlagsCheckedListBox);
            this.genMeshFlagsGroupBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.genMeshFlagsGroupBox.Location = new System.Drawing.Point(0, 0);
            this.genMeshFlagsGroupBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.genMeshFlagsGroupBox.Name = "genMeshFlagsGroupBox";
            this.genMeshFlagsGroupBox.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.genMeshFlagsGroupBox.Size = new System.Drawing.Size(766, 319);
            this.genMeshFlagsGroupBox.TabIndex = 0;
            this.genMeshFlagsGroupBox.TabStop = false;
            this.genMeshFlagsGroupBox.Text = "Mesh Flags";
            // 
            // genMeshFlagsCheckedListBox
            // 
            this.genMeshFlagsCheckedListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.genMeshFlagsCheckedListBox.Location = new System.Drawing.Point(4, 19);
            this.genMeshFlagsCheckedListBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.genMeshFlagsCheckedListBox.Name = "genMeshFlagsCheckedListBox";
            this.genMeshFlagsCheckedListBox.Size = new System.Drawing.Size(758, 296);
            this.genMeshFlagsCheckedListBox.TabIndex = 0;
            // 
            // brgFlagsBottomPanel
            // 
            this.brgFlagsBottomPanel.Controls.Add(this.interpTypeGroupBox);
            this.brgFlagsBottomPanel.Controls.Add(this.genMeshAnimTypeGroupBox);
            this.brgFlagsBottomPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.brgFlagsBottomPanel.Location = new System.Drawing.Point(0, 522);
            this.brgFlagsBottomPanel.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.brgFlagsBottomPanel.Name = "brgFlagsBottomPanel";
            this.brgFlagsBottomPanel.Size = new System.Drawing.Size(766, 80);
            this.brgFlagsBottomPanel.TabIndex = 3;
            // 
            // interpTypeGroupBox
            // 
            this.interpTypeGroupBox.Controls.Add(this.interpolationTypeCheckBox);
            this.interpTypeGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.interpTypeGroupBox.Location = new System.Drawing.Point(0, 0);
            this.interpTypeGroupBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.interpTypeGroupBox.Name = "interpTypeGroupBox";
            this.interpTypeGroupBox.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.interpTypeGroupBox.Size = new System.Drawing.Size(173, 80);
            this.interpTypeGroupBox.TabIndex = 18;
            this.interpTypeGroupBox.TabStop = false;
            this.interpTypeGroupBox.Text = "Interpolation Type";
            // 
            // interpolationTypeCheckBox
            // 
            this.interpolationTypeCheckBox.AutoSize = true;
            this.interpolationTypeCheckBox.Location = new System.Drawing.Point(5, 20);
            this.interpolationTypeCheckBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.interpolationTypeCheckBox.Name = "interpolationTypeCheckBox";
            this.interpolationTypeCheckBox.Size = new System.Drawing.Size(149, 21);
            this.interpolationTypeCheckBox.TabIndex = 17;
            this.interpolationTypeCheckBox.Text = "Conform to Terrain";
            // 
            // genMeshAnimTypeGroupBox
            // 
            this.genMeshAnimTypeGroupBox.Controls.Add(this.skinBoneRadioButton);
            this.genMeshAnimTypeGroupBox.Controls.Add(this.nonuniRadioButton);
            this.genMeshAnimTypeGroupBox.Controls.Add(this.keyframeRadioButton);
            this.genMeshAnimTypeGroupBox.Dock = System.Windows.Forms.DockStyle.Right;
            this.genMeshAnimTypeGroupBox.Location = new System.Drawing.Point(173, 0);
            this.genMeshAnimTypeGroupBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.genMeshAnimTypeGroupBox.Name = "genMeshAnimTypeGroupBox";
            this.genMeshAnimTypeGroupBox.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.genMeshAnimTypeGroupBox.Size = new System.Drawing.Size(593, 80);
            this.genMeshAnimTypeGroupBox.TabIndex = 2;
            this.genMeshAnimTypeGroupBox.TabStop = false;
            this.genMeshAnimTypeGroupBox.Text = "Mesh Animation Type";
            // 
            // skinBoneRadioButton
            // 
            this.skinBoneRadioButton.AutoSize = true;
            this.skinBoneRadioButton.Location = new System.Drawing.Point(212, 40);
            this.skinBoneRadioButton.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.skinBoneRadioButton.Name = "skinBoneRadioButton";
            this.skinBoneRadioButton.Size = new System.Drawing.Size(89, 21);
            this.skinBoneRadioButton.TabIndex = 2;
            this.skinBoneRadioButton.TabStop = true;
            this.skinBoneRadioButton.Text = "SkinBone";
            // 
            // nonuniRadioButton
            // 
            this.nonuniRadioButton.AutoSize = true;
            this.nonuniRadioButton.Location = new System.Drawing.Point(101, 40);
            this.nonuniRadioButton.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.nonuniRadioButton.Name = "nonuniRadioButton";
            this.nonuniRadioButton.Size = new System.Drawing.Size(104, 21);
            this.nonuniRadioButton.TabIndex = 1;
            this.nonuniRadioButton.TabStop = true;
            this.nonuniRadioButton.Text = "NonUniform";
            // 
            // keyframeRadioButton
            // 
            this.keyframeRadioButton.AutoSize = true;
            this.keyframeRadioButton.Location = new System.Drawing.Point(6, 40);
            this.keyframeRadioButton.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.keyframeRadioButton.Name = "keyframeRadioButton";
            this.keyframeRadioButton.Size = new System.Drawing.Size(89, 21);
            this.keyframeRadioButton.TabIndex = 0;
            this.keyframeRadioButton.TabStop = true;
            this.keyframeRadioButton.Text = "Keyframe";
            // 
            // genDataPanel
            // 
            this.genDataPanel.Controls.Add(this.attachpointGroupBox);
            this.genDataPanel.Controls.Add(this.generalDataGroupBox);
            this.genDataPanel.Dock = System.Windows.Forms.DockStyle.Left;
            this.genDataPanel.Location = new System.Drawing.Point(4, 4);
            this.genDataPanel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.genDataPanel.Name = "genDataPanel";
            this.genDataPanel.Size = new System.Drawing.Size(263, 602);
            this.genDataPanel.TabIndex = 2;
            // 
            // generalDataGroupBox
            // 
            this.generalDataGroupBox.Controls.Add(this.brgImportGroupBox);
            this.generalDataGroupBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.generalDataGroupBox.Location = new System.Drawing.Point(0, 0);
            this.generalDataGroupBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.generalDataGroupBox.Name = "generalDataGroupBox";
            this.generalDataGroupBox.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
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
            // attachpointGroupBox
            // 
            this.attachpointGroupBox.Controls.Add(this.attachpointListBox);
            this.attachpointGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.attachpointGroupBox.Location = new System.Drawing.Point(0, 111);
            this.attachpointGroupBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.attachpointGroupBox.Name = "attachpointGroupBox";
            this.attachpointGroupBox.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.attachpointGroupBox.Size = new System.Drawing.Size(263, 491);
            this.attachpointGroupBox.TabIndex = 1;
            this.attachpointGroupBox.TabStop = false;
            this.attachpointGroupBox.Text = "Attachpoints";
            // 
            // materialTabPage
            // 
            this.materialTabPage.Controls.Add(this.materialPanel);
            this.materialTabPage.Controls.Add(this.materialSidePanel);
            this.materialTabPage.Location = new System.Drawing.Point(4, 25);
            this.materialTabPage.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.materialTabPage.Name = "materialTabPage";
            this.materialTabPage.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.materialTabPage.Size = new System.Drawing.Size(1037, 610);
            this.materialTabPage.TabIndex = 1;
            this.materialTabPage.Text = "Brg Materials";
            this.materialTabPage.UseVisualStyleBackColor = true;
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
            this.grnSettingsMainPanel.Controls.Add(this.richTextBox1);
            this.grnSettingsMainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grnSettingsMainPanel.Location = new System.Drawing.Point(266, 2);
            this.grnSettingsMainPanel.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.grnSettingsMainPanel.Name = "grnSettingsMainPanel";
            this.grnSettingsMainPanel.Size = new System.Drawing.Size(768, 606);
            this.grnSettingsMainPanel.TabIndex = 2;
            // 
            // grnPropsGroupBox
            // 
            this.grnPropsGroupBox.Controls.Add(this.grnPropsListBox);
            this.grnPropsGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grnPropsGroupBox.Location = new System.Drawing.Point(0, 0);
            this.grnPropsGroupBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.grnPropsGroupBox.Name = "grnPropsGroupBox";
            this.grnPropsGroupBox.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.grnPropsGroupBox.Size = new System.Drawing.Size(768, 308);
            this.grnPropsGroupBox.TabIndex = 1;
            this.grnPropsGroupBox.TabStop = false;
            this.grnPropsGroupBox.Text = "Object Properties";
            // 
            // grnPropsListBox
            // 
            this.grnPropsListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grnPropsListBox.FormattingEnabled = true;
            this.grnPropsListBox.ItemHeight = 16;
            this.grnPropsListBox.Location = new System.Drawing.Point(3, 17);
            this.grnPropsListBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.grnPropsListBox.Name = "grnPropsListBox";
            this.grnPropsListBox.Size = new System.Drawing.Size(762, 289);
            this.grnPropsListBox.TabIndex = 0;
            // 
            // richTextBox1
            // 
            this.richTextBox1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.richTextBox1.Location = new System.Drawing.Point(0, 308);
            this.richTextBox1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(768, 298);
            this.richTextBox1.TabIndex = 0;
            this.richTextBox1.Text = "";
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
            // grnObjectsGroupBox
            // 
            this.grnObjectsGroupBox.Controls.Add(this.grnObjectsListBox);
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
            // grnObjectsListBox
            // 
            this.grnObjectsListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grnObjectsListBox.FormattingEnabled = true;
            this.grnObjectsListBox.ItemHeight = 16;
            this.grnObjectsListBox.Location = new System.Drawing.Point(3, 17);
            this.grnObjectsListBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.grnObjectsListBox.Name = "grnObjectsListBox";
            this.grnObjectsListBox.Size = new System.Drawing.Size(257, 502);
            this.grnObjectsListBox.TabIndex = 0;
            // 
            // mainMenuStrip
            // 
            this.mainMenuStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.mainMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.maxToolStripMenuItem,
            this.helpToolStripMenuItem,
            this.grnTestToolStripMenuItem});
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
            this.newToolStripMenuItem.Size = new System.Drawing.Size(118, 26);
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
            // grnTestToolStripMenuItem
            // 
            this.grnTestToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openGrnTestToolStripMenuItem,
            this.exportGrnTestToolStripMenuItem,
            this.saveGrnTestToolStripMenuItem});
            this.grnTestToolStripMenuItem.Name = "grnTestToolStripMenuItem";
            this.grnTestToolStripMenuItem.Size = new System.Drawing.Size(71, 24);
            this.grnTestToolStripMenuItem.Text = "GrnTest";
            this.grnTestToolStripMenuItem.Visible = false;
            // 
            // openGrnTestToolStripMenuItem
            // 
            this.openGrnTestToolStripMenuItem.Name = "openGrnTestToolStripMenuItem";
            this.openGrnTestToolStripMenuItem.Size = new System.Drawing.Size(179, 24);
            this.openGrnTestToolStripMenuItem.Text = "Open Grn Test";
            this.openGrnTestToolStripMenuItem.Click += new System.EventHandler(this.openGrnTestToolStripMenuItem_Click);
            // 
            // exportGrnTestToolStripMenuItem
            // 
            this.exportGrnTestToolStripMenuItem.Name = "exportGrnTestToolStripMenuItem";
            this.exportGrnTestToolStripMenuItem.Size = new System.Drawing.Size(179, 24);
            this.exportGrnTestToolStripMenuItem.Text = "Export Grn Test";
            this.exportGrnTestToolStripMenuItem.Click += new System.EventHandler(this.exportGrnTestToolStripMenuItem_Click);
            // 
            // saveGrnTestToolStripMenuItem
            // 
            this.saveGrnTestToolStripMenuItem.Name = "saveGrnTestToolStripMenuItem";
            this.saveGrnTestToolStripMenuItem.Size = new System.Drawing.Size(179, 24);
            this.saveGrnTestToolStripMenuItem.Text = "Save Grn Test";
            this.saveGrnTestToolStripMenuItem.Click += new System.EventHandler(this.saveGrnTestToolStripMenuItem_Click);
            // 
            // openFileDialog
            // 
            this.openFileDialog.Filter = "\"brg files|*.brg|grn files|*.grn\"";
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
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "MainForm";
            this.Text = "AoM Model Editor";
            this.materialPanel.ResumeLayout(false);
            this.materialGroupBox.ResumeLayout(false);
            this.materialGroupBox.PerformLayout();
            this.materialFlagsGroupBox.ResumeLayout(false);
            this.materialSidePanel.ResumeLayout(false);
            this.materialSideGroupBox.ResumeLayout(false);
            this.mainTabControl.ResumeLayout(false);
            this.meshInfoTabPage.ResumeLayout(false);
            this.flagsPanel.ResumeLayout(false);
            this.genMeshFormatGroupBox.ResumeLayout(false);
            this.genMeshFlagsGroupBox.ResumeLayout(false);
            this.brgFlagsBottomPanel.ResumeLayout(false);
            this.interpTypeGroupBox.ResumeLayout(false);
            this.interpTypeGroupBox.PerformLayout();
            this.genMeshAnimTypeGroupBox.ResumeLayout(false);
            this.genMeshAnimTypeGroupBox.PerformLayout();
            this.genDataPanel.ResumeLayout(false);
            this.generalDataGroupBox.ResumeLayout(false);
            this.brgImportGroupBox.ResumeLayout(false);
            this.brgImportGroupBox.PerformLayout();
            this.attachpointGroupBox.ResumeLayout(false);
            this.materialTabPage.ResumeLayout(false);
            this.grnSettingsTabPage.ResumeLayout(false);
            this.grnSettingsMainPanel.ResumeLayout(false);
            this.grnPropsGroupBox.ResumeLayout(false);
            this.grnSettingsSidePanel.ResumeLayout(false);
            this.grnSettingsGroupBox.ResumeLayout(false);
            this.grnExportGroupBox.ResumeLayout(false);
            this.grnExportGroupBox.PerformLayout();
            this.grnObjectsGroupBox.ResumeLayout(false);
            this.mainMenuStrip.ResumeLayout(false);
            this.mainMenuStrip.PerformLayout();
            this.mainStatusStrip.ResumeLayout(false);
            this.mainStatusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label specularLevelLabel;
        private System.Windows.Forms.Panel materialPanel;
        private System.Windows.Forms.GroupBox materialGroupBox;
        private System.Windows.Forms.Label opacityLabel;
        private System.Windows.Forms.Label unkLabel;
        private System.Windows.Forms.Label reflectionLabel;
        private System.Windows.Forms.Label textureLabel;
        private System.Windows.Forms.GroupBox materialFlagsGroupBox;
        private System.Windows.Forms.Panel materialSidePanel;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.TabControl mainTabControl;
        private System.Windows.Forms.TabPage meshInfoTabPage;
        private System.Windows.Forms.Panel flagsPanel;
        private System.Windows.Forms.GroupBox genMeshFormatGroupBox;
        private System.Windows.Forms.GroupBox genMeshFlagsGroupBox;
        private System.Windows.Forms.Panel genDataPanel;
        private System.Windows.Forms.GroupBox generalDataGroupBox;
        private System.Windows.Forms.GroupBox attachpointGroupBox;
        private System.Windows.Forms.TabPage materialTabPage;
        private System.Windows.Forms.MenuStrip mainMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
        private System.Windows.Forms.ToolStripMenuItem grnTestToolStripMenuItem;
        public System.Windows.Forms.CheckedListBox genMeshFlagsCheckedListBox;
        public System.Windows.Forms.CheckedListBox genMeshFormatCheckedListBox;
        public System.Windows.Forms.ListBox materialListBox;
        private System.Windows.Forms.Button extractMatButton;
        private System.Windows.Forms.ToolStripMenuItem maxToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportGrnTestToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem beginnersGuideToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem brgSettingsInfoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sourceCodeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openGrnTestToolStripMenuItem;
        private System.Windows.Forms.TabPage grnSettingsTabPage;
        private System.Windows.Forms.ToolStripMenuItem saveGrnTestToolStripMenuItem;
        public System.Windows.Forms.CheckBox interpolationTypeCheckBox;
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
        public System.Windows.Forms.RichTextBox richTextBox1;
        public System.Windows.Forms.CheckBox grnExportAnimCheckBox;
        public System.Windows.Forms.CheckBox grnExportModelCheckBox;
        private System.Windows.Forms.GroupBox materialSideGroupBox;
        private System.Windows.Forms.GroupBox grnPropsGroupBox;
        private System.Windows.Forms.ListBox grnPropsListBox;
        private System.Windows.Forms.GroupBox grnObjectsGroupBox;
        public System.Windows.Forms.ListBox grnObjectsListBox;
        public System.Windows.Forms.CheckedListBox materialFlagsCheckedListBox;
        private System.Windows.Forms.ToolStripMenuItem readMeToolStripMenuItem;
        public System.Windows.Forms.TextBox specularLevelMaxTextBox;
        public System.Windows.Forms.TextBox opacityMaxTextBox;
        public System.Windows.Forms.TextBox bumpMapMaxTextBox;
        public System.Windows.Forms.TextBox reflectionMaxTextBox;
        public System.Windows.Forms.TextBox textureMaxTextBox;
        public System.Windows.Forms.TextBox selfIllumMaxTextBox;
        public System.Windows.Forms.TextBox specularMaxTextBox;
        public System.Windows.Forms.TextBox ambientMaxTextBox;
        public System.Windows.Forms.TextBox diffuseMaxTextBox;
        public System.Windows.Forms.RadioButton skinBoneRadioButton;
        public System.Windows.Forms.RadioButton nonuniRadioButton;
        public System.Windows.Forms.RadioButton keyframeRadioButton;
        public System.Windows.Forms.GroupBox genMeshAnimTypeGroupBox;
        private System.Windows.Forms.GroupBox brgImportGroupBox;
        public System.Windows.Forms.CheckBox brgImportCenterModelCheckBox;
        public System.Windows.Forms.CheckBox brgImportAttachScaleCheckBox;
        private System.Windows.Forms.GroupBox interpTypeGroupBox;
        private System.Windows.Forms.Panel brgFlagsBottomPanel;
        internal System.Windows.Forms.ListBox attachpointListBox;
    }
}