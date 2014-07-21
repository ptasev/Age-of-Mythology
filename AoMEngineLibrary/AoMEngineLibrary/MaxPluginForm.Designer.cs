namespace AoMEngineLibrary
{
    partial class MaxPluginForm
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
            this.specularLevelLabel = new System.Windows.Forms.Label();
            this.materialPanel = new System.Windows.Forms.Panel();
            this.materialGroupBox = new System.Windows.Forms.GroupBox();
            this.updateMatSettingsButton = new System.Windows.Forms.Button();
            this.specularLevelMaxTextBox = new MaxCustomControls.MaxTextBox();
            this.opacityLabel = new System.Windows.Forms.Label();
            this.opacityMaxTextBox = new MaxCustomControls.MaxTextBox();
            this.unkLabel = new System.Windows.Forms.Label();
            this.unknownMaxTextBox = new MaxCustomControls.MaxTextBox();
            this.reflectionLabel = new System.Windows.Forms.Label();
            this.reflectionMaxTextBox = new MaxCustomControls.MaxTextBox();
            this.textureLabel = new System.Windows.Forms.Label();
            this.textureMaxTextBox = new MaxCustomControls.MaxTextBox();
            this.selfIllumMaxTextBox = new MaxCustomControls.MaxTextBox();
            this.specularMaxTextBox = new MaxCustomControls.MaxTextBox();
            this.ambientMaxTextBox = new MaxCustomControls.MaxTextBox();
            this.diffuseMaxTextBox = new MaxCustomControls.MaxTextBox();
            this.materialFlagsGroupBox = new System.Windows.Forms.GroupBox();
            this.materialFlagsCheckedListBox = new System.Windows.Forms.CheckedListBox();
            this.materialSidePanel = new System.Windows.Forms.Panel();
            this.materialListBox = new System.Windows.Forms.ListBox();
            this.timeMultMaxTextBox = new MaxCustomControls.MaxTextBox();
            this.timeMultLabel = new System.Windows.Forms.Label();
            this.liuMaxTextBox = new MaxCustomControls.MaxTextBox();
            this.liuLabel = new System.Windows.Forms.Label();
            this.importFromMaxToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.exportToMaxToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.attachpointListBox = new System.Windows.Forms.ListBox();
            this.mainTabControl = new System.Windows.Forms.TabControl();
            this.meshInfoTabPage = new System.Windows.Forms.TabPage();
            this.flagsPanel = new System.Windows.Forms.Panel();
            this.genMeshFormatGroupBox = new System.Windows.Forms.GroupBox();
            this.genMeshFormatCheckedListBox = new System.Windows.Forms.CheckedListBox();
            this.genMeshPropsGroupBox = new System.Windows.Forms.GroupBox();
            this.genMeshPropsCheckedListBox = new System.Windows.Forms.CheckedListBox();
            this.genMeshFlagsGroupBox = new System.Windows.Forms.GroupBox();
            this.genMeshFlagsCheckedListBox = new System.Windows.Forms.CheckedListBox();
            this.genDataPanel = new System.Windows.Forms.Panel();
            this.generalDataGroupBox = new System.Windows.Forms.GroupBox();
            this.numFacesLabel = new System.Windows.Forms.Label();
            this.numFacesMaxTextBox = new MaxCustomControls.MaxTextBox();
            this.numVertsLabel = new System.Windows.Forms.Label();
            this.numVertsMaxTextBox = new MaxCustomControls.MaxTextBox();
            this.updateSettingsButton = new System.Windows.Forms.Button();
            this.interpolationTypeMaxTextBox = new MaxCustomControls.MaxTextBox();
            this.interpolationTypeLabel = new System.Windows.Forms.Label();
            this.animTimeMaxTextBox = new MaxCustomControls.MaxTextBox();
            this.animTimeLabel = new System.Windows.Forms.Label();
            this.numMatMaxTextBox = new MaxCustomControls.MaxTextBox();
            this.numMatLabel = new System.Windows.Forms.Label();
            this.numMeshLabel = new System.Windows.Forms.Label();
            this.numMeshMaxTextBox = new MaxCustomControls.MaxTextBox();
            this.attachpointGroupBox = new System.Windows.Forms.GroupBox();
            this.attachpointComboBox = new System.Windows.Forms.ComboBox();
            this.materialTabPage = new System.Windows.Forms.TabPage();
            this.mainMenuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.materialPanel.SuspendLayout();
            this.materialGroupBox.SuspendLayout();
            this.materialFlagsGroupBox.SuspendLayout();
            this.materialSidePanel.SuspendLayout();
            this.mainTabControl.SuspendLayout();
            this.meshInfoTabPage.SuspendLayout();
            this.flagsPanel.SuspendLayout();
            this.genMeshFormatGroupBox.SuspendLayout();
            this.genMeshPropsGroupBox.SuspendLayout();
            this.genMeshFlagsGroupBox.SuspendLayout();
            this.genDataPanel.SuspendLayout();
            this.generalDataGroupBox.SuspendLayout();
            this.attachpointGroupBox.SuspendLayout();
            this.materialTabPage.SuspendLayout();
            this.mainMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // specularLevelLabel
            // 
            this.specularLevelLabel.AutoSize = true;
            this.specularLevelLabel.Location = new System.Drawing.Point(265, 48);
            this.specularLevelLabel.Name = "specularLevelLabel";
            this.specularLevelLabel.Size = new System.Drawing.Size(33, 13);
            this.specularLevelLabel.TabIndex = 13;
            this.specularLevelLabel.Text = "Level";
            // 
            // materialPanel
            // 
            this.materialPanel.Controls.Add(this.materialGroupBox);
            this.materialPanel.Controls.Add(this.materialFlagsGroupBox);
            this.materialPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.materialPanel.Location = new System.Drawing.Point(230, 3);
            this.materialPanel.Name = "materialPanel";
            this.materialPanel.Size = new System.Drawing.Size(543, 506);
            this.materialPanel.TabIndex = 1;
            // 
            // materialGroupBox
            // 
            this.materialGroupBox.Controls.Add(this.updateMatSettingsButton);
            this.materialGroupBox.Controls.Add(this.specularLevelLabel);
            this.materialGroupBox.Controls.Add(this.specularLevelMaxTextBox);
            this.materialGroupBox.Controls.Add(this.opacityLabel);
            this.materialGroupBox.Controls.Add(this.opacityMaxTextBox);
            this.materialGroupBox.Controls.Add(this.unkLabel);
            this.materialGroupBox.Controls.Add(this.unknownMaxTextBox);
            this.materialGroupBox.Controls.Add(this.reflectionLabel);
            this.materialGroupBox.Controls.Add(this.reflectionMaxTextBox);
            this.materialGroupBox.Controls.Add(this.textureLabel);
            this.materialGroupBox.Controls.Add(this.textureMaxTextBox);
            this.materialGroupBox.Controls.Add(this.selfIllumMaxTextBox);
            this.materialGroupBox.Controls.Add(this.specularMaxTextBox);
            this.materialGroupBox.Controls.Add(this.ambientMaxTextBox);
            this.materialGroupBox.Controls.Add(this.diffuseMaxTextBox);
            this.materialGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.materialGroupBox.Location = new System.Drawing.Point(0, 0);
            this.materialGroupBox.Name = "materialGroupBox";
            this.materialGroupBox.Size = new System.Drawing.Size(543, 137);
            this.materialGroupBox.TabIndex = 0;
            this.materialGroupBox.TabStop = false;
            this.materialGroupBox.Text = "Information";
            // 
            // updateMatSettingsButton
            // 
            this.updateMatSettingsButton.Location = new System.Drawing.Point(268, 95);
            this.updateMatSettingsButton.Name = "updateMatSettingsButton";
            this.updateMatSettingsButton.Size = new System.Drawing.Size(256, 23);
            this.updateMatSettingsButton.TabIndex = 14;
            this.updateMatSettingsButton.Text = "Update Flags";
            this.updateMatSettingsButton.UseVisualStyleBackColor = true;
            this.updateMatSettingsButton.Click += new System.EventHandler(this.updateMatSettingsButton_Click);
            // 
            // specularLevelMaxTextBox
            // 
            this.specularLevelMaxTextBox.Location = new System.Drawing.Point(304, 45);
            this.specularLevelMaxTextBox.Name = "specularLevelMaxTextBox";
            this.specularLevelMaxTextBox.ReadOnly = true;
            this.specularLevelMaxTextBox.Size = new System.Drawing.Size(89, 20);
            this.specularLevelMaxTextBox.TabIndex = 12;
            // 
            // opacityLabel
            // 
            this.opacityLabel.AutoSize = true;
            this.opacityLabel.Location = new System.Drawing.Point(396, 48);
            this.opacityLabel.Name = "opacityLabel";
            this.opacityLabel.Size = new System.Drawing.Size(43, 13);
            this.opacityLabel.TabIndex = 11;
            this.opacityLabel.Text = "Opacity";
            // 
            // opacityMaxTextBox
            // 
            this.opacityMaxTextBox.Location = new System.Drawing.Point(445, 45);
            this.opacityMaxTextBox.Name = "opacityMaxTextBox";
            this.opacityMaxTextBox.ReadOnly = true;
            this.opacityMaxTextBox.Size = new System.Drawing.Size(79, 20);
            this.opacityMaxTextBox.TabIndex = 10;
            // 
            // unkLabel
            // 
            this.unkLabel.AutoSize = true;
            this.unkLabel.Location = new System.Drawing.Point(3, 100);
            this.unkLabel.Name = "unkLabel";
            this.unkLabel.Size = new System.Drawing.Size(53, 13);
            this.unkLabel.TabIndex = 9;
            this.unkLabel.Text = "Unknown";
            // 
            // unknownMaxTextBox
            // 
            this.unknownMaxTextBox.Location = new System.Drawing.Point(64, 97);
            this.unknownMaxTextBox.Name = "unknownMaxTextBox";
            this.unknownMaxTextBox.ReadOnly = true;
            this.unknownMaxTextBox.Size = new System.Drawing.Size(198, 20);
            this.unknownMaxTextBox.TabIndex = 8;
            // 
            // reflectionLabel
            // 
            this.reflectionLabel.AutoSize = true;
            this.reflectionLabel.Location = new System.Drawing.Point(3, 74);
            this.reflectionLabel.Name = "reflectionLabel";
            this.reflectionLabel.Size = new System.Drawing.Size(55, 13);
            this.reflectionLabel.TabIndex = 7;
            this.reflectionLabel.Text = "Reflection";
            // 
            // reflectionMaxTextBox
            // 
            this.reflectionMaxTextBox.Location = new System.Drawing.Point(64, 71);
            this.reflectionMaxTextBox.Name = "reflectionMaxTextBox";
            this.reflectionMaxTextBox.ReadOnly = true;
            this.reflectionMaxTextBox.Size = new System.Drawing.Size(198, 20);
            this.reflectionMaxTextBox.TabIndex = 6;
            // 
            // textureLabel
            // 
            this.textureLabel.AutoSize = true;
            this.textureLabel.Location = new System.Drawing.Point(3, 48);
            this.textureLabel.Name = "textureLabel";
            this.textureLabel.Size = new System.Drawing.Size(43, 13);
            this.textureLabel.TabIndex = 5;
            this.textureLabel.Text = "Texture";
            // 
            // textureMaxTextBox
            // 
            this.textureMaxTextBox.Location = new System.Drawing.Point(64, 45);
            this.textureMaxTextBox.Name = "textureMaxTextBox";
            this.textureMaxTextBox.ReadOnly = true;
            this.textureMaxTextBox.Size = new System.Drawing.Size(198, 20);
            this.textureMaxTextBox.TabIndex = 4;
            // 
            // selfIllumMaxTextBox
            // 
            this.selfIllumMaxTextBox.Location = new System.Drawing.Point(399, 19);
            this.selfIllumMaxTextBox.Name = "selfIllumMaxTextBox";
            this.selfIllumMaxTextBox.ReadOnly = true;
            this.selfIllumMaxTextBox.Size = new System.Drawing.Size(125, 20);
            this.selfIllumMaxTextBox.TabIndex = 3;
            this.selfIllumMaxTextBox.Text = "Self Illumination";
            // 
            // specularMaxTextBox
            // 
            this.specularMaxTextBox.Location = new System.Drawing.Point(268, 19);
            this.specularMaxTextBox.Name = "specularMaxTextBox";
            this.specularMaxTextBox.ReadOnly = true;
            this.specularMaxTextBox.Size = new System.Drawing.Size(125, 20);
            this.specularMaxTextBox.TabIndex = 2;
            this.specularMaxTextBox.Text = "Specular";
            // 
            // ambientMaxTextBox
            // 
            this.ambientMaxTextBox.Location = new System.Drawing.Point(137, 19);
            this.ambientMaxTextBox.Name = "ambientMaxTextBox";
            this.ambientMaxTextBox.ReadOnly = true;
            this.ambientMaxTextBox.Size = new System.Drawing.Size(125, 20);
            this.ambientMaxTextBox.TabIndex = 1;
            this.ambientMaxTextBox.Text = "Ambient";
            // 
            // diffuseMaxTextBox
            // 
            this.diffuseMaxTextBox.Location = new System.Drawing.Point(6, 19);
            this.diffuseMaxTextBox.Name = "diffuseMaxTextBox";
            this.diffuseMaxTextBox.ReadOnly = true;
            this.diffuseMaxTextBox.Size = new System.Drawing.Size(125, 20);
            this.diffuseMaxTextBox.TabIndex = 0;
            this.diffuseMaxTextBox.Text = "Diffuse";
            // 
            // materialFlagsGroupBox
            // 
            this.materialFlagsGroupBox.Controls.Add(this.materialFlagsCheckedListBox);
            this.materialFlagsGroupBox.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.materialFlagsGroupBox.Location = new System.Drawing.Point(0, 137);
            this.materialFlagsGroupBox.Name = "materialFlagsGroupBox";
            this.materialFlagsGroupBox.Size = new System.Drawing.Size(543, 369);
            this.materialFlagsGroupBox.TabIndex = 1;
            this.materialFlagsGroupBox.TabStop = false;
            this.materialFlagsGroupBox.Text = "Flags";
            // 
            // materialFlagsCheckedListBox
            // 
            this.materialFlagsCheckedListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.materialFlagsCheckedListBox.FormattingEnabled = true;
            this.materialFlagsCheckedListBox.Location = new System.Drawing.Point(3, 16);
            this.materialFlagsCheckedListBox.Name = "materialFlagsCheckedListBox";
            this.materialFlagsCheckedListBox.Size = new System.Drawing.Size(537, 350);
            this.materialFlagsCheckedListBox.TabIndex = 0;
            // 
            // materialSidePanel
            // 
            this.materialSidePanel.Controls.Add(this.materialListBox);
            this.materialSidePanel.Dock = System.Windows.Forms.DockStyle.Left;
            this.materialSidePanel.Location = new System.Drawing.Point(3, 3);
            this.materialSidePanel.Name = "materialSidePanel";
            this.materialSidePanel.Size = new System.Drawing.Size(227, 506);
            this.materialSidePanel.TabIndex = 0;
            // 
            // materialListBox
            // 
            this.materialListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.materialListBox.FormattingEnabled = true;
            this.materialListBox.Location = new System.Drawing.Point(0, 0);
            this.materialListBox.Name = "materialListBox";
            this.materialListBox.Size = new System.Drawing.Size(227, 506);
            this.materialListBox.TabIndex = 0;
            // 
            // timeMultMaxTextBox
            // 
            this.timeMultMaxTextBox.Location = new System.Drawing.Point(76, 143);
            this.timeMultMaxTextBox.Name = "timeMultMaxTextBox";
            this.timeMultMaxTextBox.Size = new System.Drawing.Size(87, 20);
            this.timeMultMaxTextBox.TabIndex = 11;
            // 
            // timeMultLabel
            // 
            this.timeMultLabel.AutoSize = true;
            this.timeMultLabel.Location = new System.Drawing.Point(3, 146);
            this.timeMultLabel.Name = "timeMultLabel";
            this.timeMultLabel.Size = new System.Drawing.Size(67, 13);
            this.timeMultLabel.TabIndex = 10;
            this.timeMultLabel.Text = "Export Scale";
            // 
            // liuMaxTextBox
            // 
            this.liuMaxTextBox.Location = new System.Drawing.Point(76, 195);
            this.liuMaxTextBox.Name = "liuMaxTextBox";
            this.liuMaxTextBox.ReadOnly = true;
            this.liuMaxTextBox.Size = new System.Drawing.Size(87, 20);
            this.liuMaxTextBox.TabIndex = 9;
            // 
            // liuLabel
            // 
            this.liuLabel.AutoSize = true;
            this.liuLabel.Location = new System.Drawing.Point(3, 198);
            this.liuLabel.Name = "liuLabel";
            this.liuLabel.Size = new System.Drawing.Size(24, 13);
            this.liuLabel.TabIndex = 8;
            this.liuLabel.Text = "LIU";
            // 
            // importFromMaxToolStripMenuItem
            // 
            this.importFromMaxToolStripMenuItem.Name = "importFromMaxToolStripMenuItem";
            this.importFromMaxToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.importFromMaxToolStripMenuItem.Text = "Import From Max";
            this.importFromMaxToolStripMenuItem.Click += new System.EventHandler(this.importFromMaxToolStripMenuItem_Click);
            // 
            // exportToMaxToolStripMenuItem
            // 
            this.exportToMaxToolStripMenuItem.Name = "exportToMaxToolStripMenuItem";
            this.exportToMaxToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.exportToMaxToolStripMenuItem.Text = "Export To Max";
            this.exportToMaxToolStripMenuItem.Click += new System.EventHandler(this.exportToMaxToolStripMenuItem_Click);
            // 
            // attachpointListBox
            // 
            this.attachpointListBox.FormattingEnabled = true;
            this.attachpointListBox.Location = new System.Drawing.Point(3, 46);
            this.attachpointListBox.Name = "attachpointListBox";
            this.attachpointListBox.Size = new System.Drawing.Size(191, 199);
            this.attachpointListBox.TabIndex = 1;
            // 
            // mainTabControl
            // 
            this.mainTabControl.Controls.Add(this.meshInfoTabPage);
            this.mainTabControl.Controls.Add(this.materialTabPage);
            this.mainTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainTabControl.Location = new System.Drawing.Point(0, 24);
            this.mainTabControl.Name = "mainTabControl";
            this.mainTabControl.SelectedIndex = 0;
            this.mainTabControl.Size = new System.Drawing.Size(784, 538);
            this.mainTabControl.TabIndex = 3;
            // 
            // meshInfoTabPage
            // 
            this.meshInfoTabPage.Controls.Add(this.flagsPanel);
            this.meshInfoTabPage.Controls.Add(this.genDataPanel);
            this.meshInfoTabPage.Location = new System.Drawing.Point(4, 22);
            this.meshInfoTabPage.Name = "meshInfoTabPage";
            this.meshInfoTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.meshInfoTabPage.Size = new System.Drawing.Size(776, 512);
            this.meshInfoTabPage.TabIndex = 0;
            this.meshInfoTabPage.Text = "General Info";
            this.meshInfoTabPage.UseVisualStyleBackColor = true;
            // 
            // flagsPanel
            // 
            this.flagsPanel.Controls.Add(this.genMeshFormatGroupBox);
            this.flagsPanel.Controls.Add(this.genMeshPropsGroupBox);
            this.flagsPanel.Controls.Add(this.genMeshFlagsGroupBox);
            this.flagsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flagsPanel.Location = new System.Drawing.Point(200, 3);
            this.flagsPanel.Name = "flagsPanel";
            this.flagsPanel.Size = new System.Drawing.Size(573, 506);
            this.flagsPanel.TabIndex = 1;
            // 
            // genMeshFormatGroupBox
            // 
            this.genMeshFormatGroupBox.Controls.Add(this.genMeshFormatCheckedListBox);
            this.genMeshFormatGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.genMeshFormatGroupBox.Location = new System.Drawing.Point(0, 259);
            this.genMeshFormatGroupBox.Name = "genMeshFormatGroupBox";
            this.genMeshFormatGroupBox.Size = new System.Drawing.Size(573, 158);
            this.genMeshFormatGroupBox.TabIndex = 1;
            this.genMeshFormatGroupBox.TabStop = false;
            this.genMeshFormatGroupBox.Text = "Mesh Format";
            // 
            // genMeshFormatCheckedListBox
            // 
            this.genMeshFormatCheckedListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.genMeshFormatCheckedListBox.FormattingEnabled = true;
            this.genMeshFormatCheckedListBox.Location = new System.Drawing.Point(3, 16);
            this.genMeshFormatCheckedListBox.Name = "genMeshFormatCheckedListBox";
            this.genMeshFormatCheckedListBox.Size = new System.Drawing.Size(567, 139);
            this.genMeshFormatCheckedListBox.TabIndex = 1;
            // 
            // genMeshPropsGroupBox
            // 
            this.genMeshPropsGroupBox.Controls.Add(this.genMeshPropsCheckedListBox);
            this.genMeshPropsGroupBox.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.genMeshPropsGroupBox.Location = new System.Drawing.Point(0, 417);
            this.genMeshPropsGroupBox.Name = "genMeshPropsGroupBox";
            this.genMeshPropsGroupBox.Size = new System.Drawing.Size(573, 89);
            this.genMeshPropsGroupBox.TabIndex = 2;
            this.genMeshPropsGroupBox.TabStop = false;
            this.genMeshPropsGroupBox.Text = "Mesh Properties";
            // 
            // genMeshPropsCheckedListBox
            // 
            this.genMeshPropsCheckedListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.genMeshPropsCheckedListBox.FormattingEnabled = true;
            this.genMeshPropsCheckedListBox.Location = new System.Drawing.Point(3, 16);
            this.genMeshPropsCheckedListBox.Name = "genMeshPropsCheckedListBox";
            this.genMeshPropsCheckedListBox.Size = new System.Drawing.Size(567, 70);
            this.genMeshPropsCheckedListBox.TabIndex = 2;
            // 
            // genMeshFlagsGroupBox
            // 
            this.genMeshFlagsGroupBox.Controls.Add(this.genMeshFlagsCheckedListBox);
            this.genMeshFlagsGroupBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.genMeshFlagsGroupBox.Location = new System.Drawing.Point(0, 0);
            this.genMeshFlagsGroupBox.Name = "genMeshFlagsGroupBox";
            this.genMeshFlagsGroupBox.Size = new System.Drawing.Size(573, 259);
            this.genMeshFlagsGroupBox.TabIndex = 0;
            this.genMeshFlagsGroupBox.TabStop = false;
            this.genMeshFlagsGroupBox.Text = "Mesh Flags";
            // 
            // genMeshFlagsCheckedListBox
            // 
            this.genMeshFlagsCheckedListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.genMeshFlagsCheckedListBox.FormattingEnabled = true;
            this.genMeshFlagsCheckedListBox.Location = new System.Drawing.Point(3, 16);
            this.genMeshFlagsCheckedListBox.Name = "genMeshFlagsCheckedListBox";
            this.genMeshFlagsCheckedListBox.Size = new System.Drawing.Size(567, 240);
            this.genMeshFlagsCheckedListBox.TabIndex = 0;
            // 
            // genDataPanel
            // 
            this.genDataPanel.Controls.Add(this.generalDataGroupBox);
            this.genDataPanel.Controls.Add(this.attachpointGroupBox);
            this.genDataPanel.Dock = System.Windows.Forms.DockStyle.Left;
            this.genDataPanel.Location = new System.Drawing.Point(3, 3);
            this.genDataPanel.Name = "genDataPanel";
            this.genDataPanel.Size = new System.Drawing.Size(197, 506);
            this.genDataPanel.TabIndex = 2;
            // 
            // generalDataGroupBox
            // 
            this.generalDataGroupBox.Controls.Add(this.numFacesLabel);
            this.generalDataGroupBox.Controls.Add(this.numFacesMaxTextBox);
            this.generalDataGroupBox.Controls.Add(this.numVertsLabel);
            this.generalDataGroupBox.Controls.Add(this.numVertsMaxTextBox);
            this.generalDataGroupBox.Controls.Add(this.updateSettingsButton);
            this.generalDataGroupBox.Controls.Add(this.timeMultMaxTextBox);
            this.generalDataGroupBox.Controls.Add(this.timeMultLabel);
            this.generalDataGroupBox.Controls.Add(this.liuMaxTextBox);
            this.generalDataGroupBox.Controls.Add(this.liuLabel);
            this.generalDataGroupBox.Controls.Add(this.interpolationTypeMaxTextBox);
            this.generalDataGroupBox.Controls.Add(this.interpolationTypeLabel);
            this.generalDataGroupBox.Controls.Add(this.animTimeMaxTextBox);
            this.generalDataGroupBox.Controls.Add(this.animTimeLabel);
            this.generalDataGroupBox.Controls.Add(this.numMatMaxTextBox);
            this.generalDataGroupBox.Controls.Add(this.numMatLabel);
            this.generalDataGroupBox.Controls.Add(this.numMeshLabel);
            this.generalDataGroupBox.Controls.Add(this.numMeshMaxTextBox);
            this.generalDataGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.generalDataGroupBox.Location = new System.Drawing.Point(0, 0);
            this.generalDataGroupBox.Name = "generalDataGroupBox";
            this.generalDataGroupBox.Size = new System.Drawing.Size(197, 259);
            this.generalDataGroupBox.TabIndex = 0;
            this.generalDataGroupBox.TabStop = false;
            this.generalDataGroupBox.Text = "Data";
            // 
            // numFacesLabel
            // 
            this.numFacesLabel.AutoSize = true;
            this.numFacesLabel.Location = new System.Drawing.Point(3, 42);
            this.numFacesLabel.Name = "numFacesLabel";
            this.numFacesLabel.Size = new System.Drawing.Size(46, 13);
            this.numFacesLabel.TabIndex = 16;
            this.numFacesLabel.Text = "# Faces";
            // 
            // numFacesMaxTextBox
            // 
            this.numFacesMaxTextBox.Location = new System.Drawing.Point(76, 39);
            this.numFacesMaxTextBox.Name = "numFacesMaxTextBox";
            this.numFacesMaxTextBox.ReadOnly = true;
            this.numFacesMaxTextBox.Size = new System.Drawing.Size(87, 20);
            this.numFacesMaxTextBox.TabIndex = 15;
            // 
            // numVertsLabel
            // 
            this.numVertsLabel.AutoSize = true;
            this.numVertsLabel.Location = new System.Drawing.Point(3, 16);
            this.numVertsLabel.Name = "numVertsLabel";
            this.numVertsLabel.Size = new System.Drawing.Size(41, 13);
            this.numVertsLabel.TabIndex = 14;
            this.numVertsLabel.Text = "# Verts";
            // 
            // numVertsMaxTextBox
            // 
            this.numVertsMaxTextBox.Location = new System.Drawing.Point(76, 13);
            this.numVertsMaxTextBox.Name = "numVertsMaxTextBox";
            this.numVertsMaxTextBox.ReadOnly = true;
            this.numVertsMaxTextBox.Size = new System.Drawing.Size(87, 20);
            this.numVertsMaxTextBox.TabIndex = 13;
            // 
            // updateSettingsButton
            // 
            this.updateSettingsButton.Location = new System.Drawing.Point(3, 230);
            this.updateSettingsButton.Name = "updateSettingsButton";
            this.updateSettingsButton.Size = new System.Drawing.Size(191, 23);
            this.updateSettingsButton.TabIndex = 12;
            this.updateSettingsButton.Text = "Update Settings";
            this.updateSettingsButton.UseVisualStyleBackColor = true;
            this.updateSettingsButton.Click += new System.EventHandler(this.updateSettingsButton_Click);
            // 
            // interpolationTypeMaxTextBox
            // 
            this.interpolationTypeMaxTextBox.Location = new System.Drawing.Point(76, 169);
            this.interpolationTypeMaxTextBox.Name = "interpolationTypeMaxTextBox";
            this.interpolationTypeMaxTextBox.Size = new System.Drawing.Size(87, 20);
            this.interpolationTypeMaxTextBox.TabIndex = 7;
            // 
            // interpolationTypeLabel
            // 
            this.interpolationTypeLabel.AutoSize = true;
            this.interpolationTypeLabel.Location = new System.Drawing.Point(3, 172);
            this.interpolationTypeLabel.Name = "interpolationTypeLabel";
            this.interpolationTypeLabel.Size = new System.Drawing.Size(65, 13);
            this.interpolationTypeLabel.TabIndex = 6;
            this.interpolationTypeLabel.Text = "Interpolation";
            // 
            // animTimeMaxTextBox
            // 
            this.animTimeMaxTextBox.Location = new System.Drawing.Point(76, 117);
            this.animTimeMaxTextBox.Name = "animTimeMaxTextBox";
            this.animTimeMaxTextBox.ReadOnly = true;
            this.animTimeMaxTextBox.Size = new System.Drawing.Size(87, 20);
            this.animTimeMaxTextBox.TabIndex = 5;
            // 
            // animTimeLabel
            // 
            this.animTimeLabel.AutoSize = true;
            this.animTimeLabel.Location = new System.Drawing.Point(3, 120);
            this.animTimeLabel.Name = "animTimeLabel";
            this.animTimeLabel.Size = new System.Drawing.Size(56, 13);
            this.animTimeLabel.TabIndex = 4;
            this.animTimeLabel.Text = "Anim Time";
            // 
            // numMatMaxTextBox
            // 
            this.numMatMaxTextBox.Location = new System.Drawing.Point(76, 91);
            this.numMatMaxTextBox.Name = "numMatMaxTextBox";
            this.numMatMaxTextBox.ReadOnly = true;
            this.numMatMaxTextBox.Size = new System.Drawing.Size(87, 20);
            this.numMatMaxTextBox.TabIndex = 3;
            // 
            // numMatLabel
            // 
            this.numMatLabel.AutoSize = true;
            this.numMatLabel.Location = new System.Drawing.Point(3, 94);
            this.numMatLabel.Name = "numMatLabel";
            this.numMatLabel.Size = new System.Drawing.Size(40, 13);
            this.numMatLabel.TabIndex = 2;
            this.numMatLabel.Text = "# Mats";
            // 
            // numMeshLabel
            // 
            this.numMeshLabel.AutoSize = true;
            this.numMeshLabel.Location = new System.Drawing.Point(3, 68);
            this.numMeshLabel.Name = "numMeshLabel";
            this.numMeshLabel.Size = new System.Drawing.Size(43, 13);
            this.numMeshLabel.TabIndex = 1;
            this.numMeshLabel.Text = "# Mesh";
            // 
            // numMeshMaxTextBox
            // 
            this.numMeshMaxTextBox.Location = new System.Drawing.Point(76, 65);
            this.numMeshMaxTextBox.Name = "numMeshMaxTextBox";
            this.numMeshMaxTextBox.ReadOnly = true;
            this.numMeshMaxTextBox.Size = new System.Drawing.Size(87, 20);
            this.numMeshMaxTextBox.TabIndex = 0;
            // 
            // attachpointGroupBox
            // 
            this.attachpointGroupBox.Controls.Add(this.attachpointListBox);
            this.attachpointGroupBox.Controls.Add(this.attachpointComboBox);
            this.attachpointGroupBox.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.attachpointGroupBox.Location = new System.Drawing.Point(0, 259);
            this.attachpointGroupBox.Name = "attachpointGroupBox";
            this.attachpointGroupBox.Size = new System.Drawing.Size(197, 247);
            this.attachpointGroupBox.TabIndex = 1;
            this.attachpointGroupBox.TabStop = false;
            this.attachpointGroupBox.Text = "Attachpoints";
            // 
            // attachpointComboBox
            // 
            this.attachpointComboBox.FormattingEnabled = true;
            this.attachpointComboBox.Location = new System.Drawing.Point(3, 19);
            this.attachpointComboBox.Name = "attachpointComboBox";
            this.attachpointComboBox.Size = new System.Drawing.Size(191, 21);
            this.attachpointComboBox.TabIndex = 0;
            // 
            // materialTabPage
            // 
            this.materialTabPage.Controls.Add(this.materialPanel);
            this.materialTabPage.Controls.Add(this.materialSidePanel);
            this.materialTabPage.Location = new System.Drawing.Point(4, 22);
            this.materialTabPage.Name = "materialTabPage";
            this.materialTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.materialTabPage.Size = new System.Drawing.Size(776, 512);
            this.materialTabPage.TabIndex = 1;
            this.materialTabPage.Text = "Materials";
            this.materialTabPage.UseVisualStyleBackColor = true;
            // 
            // mainMenuStrip
            // 
            this.mainMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.mainMenuStrip.Location = new System.Drawing.Point(0, 0);
            this.mainMenuStrip.Name = "mainMenuStrip";
            this.mainMenuStrip.Size = new System.Drawing.Size(784, 24);
            this.mainMenuStrip.TabIndex = 2;
            this.mainMenuStrip.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.exportToMaxToolStripMenuItem,
            this.importFromMaxToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Image = global::AoMEngineLibrary.Properties.Resources.picture_empty;
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.newToolStripMenuItem.Text = "New";
            this.newToolStripMenuItem.Click += new System.EventHandler(this.newToolStripMenuItem_Click);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Image = global::AoMEngineLibrary.Properties.Resources.folder_picture;
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Image = global::AoMEngineLibrary.Properties.Resources.picture_save;
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // MaxPluginForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 562);
            this.Controls.Add(this.mainTabControl);
            this.Controls.Add(this.mainMenuStrip);
            this.Name = "MaxPluginForm";
            this.Text = "AoM Brg Editor";
            this.materialPanel.ResumeLayout(false);
            this.materialGroupBox.ResumeLayout(false);
            this.materialGroupBox.PerformLayout();
            this.materialFlagsGroupBox.ResumeLayout(false);
            this.materialSidePanel.ResumeLayout(false);
            this.mainTabControl.ResumeLayout(false);
            this.meshInfoTabPage.ResumeLayout(false);
            this.flagsPanel.ResumeLayout(false);
            this.genMeshFormatGroupBox.ResumeLayout(false);
            this.genMeshPropsGroupBox.ResumeLayout(false);
            this.genMeshFlagsGroupBox.ResumeLayout(false);
            this.genDataPanel.ResumeLayout(false);
            this.generalDataGroupBox.ResumeLayout(false);
            this.generalDataGroupBox.PerformLayout();
            this.attachpointGroupBox.ResumeLayout(false);
            this.materialTabPage.ResumeLayout(false);
            this.mainMenuStrip.ResumeLayout(false);
            this.mainMenuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label specularLevelLabel;
        private System.Windows.Forms.Panel materialPanel;
        private System.Windows.Forms.GroupBox materialGroupBox;
        private MaxCustomControls.MaxTextBox specularLevelMaxTextBox;
        private System.Windows.Forms.Label opacityLabel;
        private MaxCustomControls.MaxTextBox opacityMaxTextBox;
        private System.Windows.Forms.Label unkLabel;
        private MaxCustomControls.MaxTextBox unknownMaxTextBox;
        private System.Windows.Forms.Label reflectionLabel;
        private MaxCustomControls.MaxTextBox reflectionMaxTextBox;
        private System.Windows.Forms.Label textureLabel;
        private MaxCustomControls.MaxTextBox textureMaxTextBox;
        private MaxCustomControls.MaxTextBox selfIllumMaxTextBox;
        private MaxCustomControls.MaxTextBox specularMaxTextBox;
        private MaxCustomControls.MaxTextBox ambientMaxTextBox;
        private MaxCustomControls.MaxTextBox diffuseMaxTextBox;
        private System.Windows.Forms.GroupBox materialFlagsGroupBox;
        private System.Windows.Forms.CheckedListBox materialFlagsCheckedListBox;
        private System.Windows.Forms.Panel materialSidePanel;
        private System.Windows.Forms.ListBox materialListBox;
        private MaxCustomControls.MaxTextBox timeMultMaxTextBox;
        private System.Windows.Forms.Label timeMultLabel;
        private MaxCustomControls.MaxTextBox liuMaxTextBox;
        private System.Windows.Forms.Label liuLabel;
        private System.Windows.Forms.ToolStripMenuItem importFromMaxToolStripMenuItem;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.ToolStripMenuItem exportToMaxToolStripMenuItem;
        private System.Windows.Forms.ListBox attachpointListBox;
        private System.Windows.Forms.TabControl mainTabControl;
        private System.Windows.Forms.TabPage meshInfoTabPage;
        private System.Windows.Forms.Panel flagsPanel;
        private System.Windows.Forms.GroupBox genMeshFormatGroupBox;
        private System.Windows.Forms.CheckedListBox genMeshFormatCheckedListBox;
        private System.Windows.Forms.GroupBox genMeshPropsGroupBox;
        private System.Windows.Forms.CheckedListBox genMeshPropsCheckedListBox;
        private System.Windows.Forms.GroupBox genMeshFlagsGroupBox;
        private System.Windows.Forms.CheckedListBox genMeshFlagsCheckedListBox;
        private System.Windows.Forms.Panel genDataPanel;
        private System.Windows.Forms.GroupBox generalDataGroupBox;
        private MaxCustomControls.MaxTextBox interpolationTypeMaxTextBox;
        private System.Windows.Forms.Label interpolationTypeLabel;
        private MaxCustomControls.MaxTextBox animTimeMaxTextBox;
        private System.Windows.Forms.Label animTimeLabel;
        private MaxCustomControls.MaxTextBox numMatMaxTextBox;
        private System.Windows.Forms.Label numMatLabel;
        private System.Windows.Forms.Label numMeshLabel;
        private MaxCustomControls.MaxTextBox numMeshMaxTextBox;
        private System.Windows.Forms.GroupBox attachpointGroupBox;
        private System.Windows.Forms.ComboBox attachpointComboBox;
        private System.Windows.Forms.TabPage materialTabPage;
        private System.Windows.Forms.MenuStrip mainMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.Button updateSettingsButton;
        private System.Windows.Forms.Button updateMatSettingsButton;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.Label numFacesLabel;
        private MaxCustomControls.MaxTextBox numFacesMaxTextBox;
        private System.Windows.Forms.Label numVertsLabel;
        private MaxCustomControls.MaxTextBox numVertsMaxTextBox;
    }
}