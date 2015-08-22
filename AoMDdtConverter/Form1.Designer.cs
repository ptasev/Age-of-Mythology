namespace AoMDdtConverter
{
    partial class Form1
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
            this.mainTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.exportSettingsGroupBox = new System.Windows.Forms.GroupBox();
            this.exportSettingsTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.mapGenerationGroupBox = new System.Windows.Forms.GroupBox();
            this.mapGenFlowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.noneMapRadioButton = new System.Windows.Forms.RadioButton();
            this.nmMapRadioButton = new System.Windows.Forms.RadioButton();
            this.specMapRadioButton = new System.Windows.Forms.RadioButton();
            this.glossMapRadioButton = new System.Windows.Forms.RadioButton();
            this.importSettingsGroupBox = new System.Windows.Forms.GroupBox();
            this.importSettingsTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.defaultCompressionGroupBox = new System.Windows.Forms.GroupBox();
            this.defaultCompressionFlowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.defaultCompressionComboBox = new System.Windows.Forms.ComboBox();
            this.defaultCompressionLabel = new System.Windows.Forms.Label();
            this.mipsGroupBox = new System.Windows.Forms.GroupBox();
            this.mipsFlowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.mipsNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.progressGroupBox = new System.Windows.Forms.GroupBox();
            this.progressTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.processingLabel = new System.Windows.Forms.Label();
            this.convertButton = new System.Windows.Forms.Button();
            this.percentageLabel = new System.Windows.Forms.Label();
            this.outputGroupBox = new System.Windows.Forms.GroupBox();
            this.outputTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.outputRichTextBox = new System.Windows.Forms.RichTextBox();
            this.allMapRadioButton = new System.Windows.Forms.RadioButton();
            this.mainTableLayoutPanel.SuspendLayout();
            this.exportSettingsGroupBox.SuspendLayout();
            this.exportSettingsTableLayoutPanel.SuspendLayout();
            this.mapGenerationGroupBox.SuspendLayout();
            this.mapGenFlowLayoutPanel.SuspendLayout();
            this.importSettingsGroupBox.SuspendLayout();
            this.importSettingsTableLayoutPanel.SuspendLayout();
            this.defaultCompressionGroupBox.SuspendLayout();
            this.defaultCompressionFlowLayoutPanel.SuspendLayout();
            this.mipsGroupBox.SuspendLayout();
            this.mipsFlowLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mipsNumericUpDown)).BeginInit();
            this.progressGroupBox.SuspendLayout();
            this.progressTableLayoutPanel.SuspendLayout();
            this.outputGroupBox.SuspendLayout();
            this.outputTableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainTableLayoutPanel
            // 
            this.mainTableLayoutPanel.ColumnCount = 1;
            this.mainTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.mainTableLayoutPanel.Controls.Add(this.exportSettingsGroupBox, 0, 0);
            this.mainTableLayoutPanel.Controls.Add(this.importSettingsGroupBox, 0, 1);
            this.mainTableLayoutPanel.Controls.Add(this.progressGroupBox, 0, 2);
            this.mainTableLayoutPanel.Controls.Add(this.outputGroupBox, 0, 3);
            this.mainTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainTableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.mainTableLayoutPanel.Name = "mainTableLayoutPanel";
            this.mainTableLayoutPanel.RowCount = 4;
            this.mainTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.mainTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 85F));
            this.mainTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 115F));
            this.mainTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.mainTableLayoutPanel.Size = new System.Drawing.Size(622, 433);
            this.mainTableLayoutPanel.TabIndex = 0;
            // 
            // exportSettingsGroupBox
            // 
            this.exportSettingsGroupBox.Controls.Add(this.exportSettingsTableLayoutPanel);
            this.exportSettingsGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.exportSettingsGroupBox.Location = new System.Drawing.Point(3, 3);
            this.exportSettingsGroupBox.Name = "exportSettingsGroupBox";
            this.exportSettingsGroupBox.Size = new System.Drawing.Size(616, 74);
            this.exportSettingsGroupBox.TabIndex = 0;
            this.exportSettingsGroupBox.TabStop = false;
            this.exportSettingsGroupBox.Text = "Export Settings";
            // 
            // exportSettingsTableLayoutPanel
            // 
            this.exportSettingsTableLayoutPanel.ColumnCount = 1;
            this.exportSettingsTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.exportSettingsTableLayoutPanel.Controls.Add(this.mapGenerationGroupBox, 0, 0);
            this.exportSettingsTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.exportSettingsTableLayoutPanel.Location = new System.Drawing.Point(3, 18);
            this.exportSettingsTableLayoutPanel.Name = "exportSettingsTableLayoutPanel";
            this.exportSettingsTableLayoutPanel.RowCount = 1;
            this.exportSettingsTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.exportSettingsTableLayoutPanel.Size = new System.Drawing.Size(610, 53);
            this.exportSettingsTableLayoutPanel.TabIndex = 0;
            // 
            // mapGenerationGroupBox
            // 
            this.mapGenerationGroupBox.Controls.Add(this.mapGenFlowLayoutPanel);
            this.mapGenerationGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mapGenerationGroupBox.Location = new System.Drawing.Point(3, 3);
            this.mapGenerationGroupBox.Name = "mapGenerationGroupBox";
            this.mapGenerationGroupBox.Size = new System.Drawing.Size(604, 47);
            this.mapGenerationGroupBox.TabIndex = 0;
            this.mapGenerationGroupBox.TabStop = false;
            this.mapGenerationGroupBox.Text = "Map Generation";
            // 
            // mapGenFlowLayoutPanel
            // 
            this.mapGenFlowLayoutPanel.Controls.Add(this.noneMapRadioButton);
            this.mapGenFlowLayoutPanel.Controls.Add(this.nmMapRadioButton);
            this.mapGenFlowLayoutPanel.Controls.Add(this.specMapRadioButton);
            this.mapGenFlowLayoutPanel.Controls.Add(this.glossMapRadioButton);
            this.mapGenFlowLayoutPanel.Controls.Add(this.allMapRadioButton);
            this.mapGenFlowLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mapGenFlowLayoutPanel.Location = new System.Drawing.Point(3, 18);
            this.mapGenFlowLayoutPanel.Name = "mapGenFlowLayoutPanel";
            this.mapGenFlowLayoutPanel.Size = new System.Drawing.Size(598, 26);
            this.mapGenFlowLayoutPanel.TabIndex = 0;
            // 
            // noneMapRadioButton
            // 
            this.noneMapRadioButton.AutoSize = true;
            this.noneMapRadioButton.Location = new System.Drawing.Point(3, 3);
            this.noneMapRadioButton.Name = "noneMapRadioButton";
            this.noneMapRadioButton.Size = new System.Drawing.Size(63, 21);
            this.noneMapRadioButton.TabIndex = 0;
            this.noneMapRadioButton.TabStop = true;
            this.noneMapRadioButton.Text = "None";
            this.noneMapRadioButton.UseVisualStyleBackColor = true;
            // 
            // nmMapRadioButton
            // 
            this.nmMapRadioButton.AutoSize = true;
            this.nmMapRadioButton.Location = new System.Drawing.Point(72, 3);
            this.nmMapRadioButton.Name = "nmMapRadioButton";
            this.nmMapRadioButton.Size = new System.Drawing.Size(74, 21);
            this.nmMapRadioButton.TabIndex = 1;
            this.nmMapRadioButton.TabStop = true;
            this.nmMapRadioButton.Text = "Normal";
            this.nmMapRadioButton.UseVisualStyleBackColor = true;
            // 
            // specMapRadioButton
            // 
            this.specMapRadioButton.AutoSize = true;
            this.specMapRadioButton.Location = new System.Drawing.Point(152, 3);
            this.specMapRadioButton.Name = "specMapRadioButton";
            this.specMapRadioButton.Size = new System.Drawing.Size(85, 21);
            this.specMapRadioButton.TabIndex = 2;
            this.specMapRadioButton.TabStop = true;
            this.specMapRadioButton.Text = "Specular";
            this.specMapRadioButton.UseVisualStyleBackColor = true;
            // 
            // glossMapRadioButton
            // 
            this.glossMapRadioButton.AutoSize = true;
            this.glossMapRadioButton.Location = new System.Drawing.Point(243, 3);
            this.glossMapRadioButton.Name = "glossMapRadioButton";
            this.glossMapRadioButton.Size = new System.Drawing.Size(65, 21);
            this.glossMapRadioButton.TabIndex = 3;
            this.glossMapRadioButton.TabStop = true;
            this.glossMapRadioButton.Text = "Gloss";
            this.glossMapRadioButton.UseVisualStyleBackColor = true;
            // 
            // importSettingsGroupBox
            // 
            this.importSettingsGroupBox.Controls.Add(this.importSettingsTableLayoutPanel);
            this.importSettingsGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.importSettingsGroupBox.Location = new System.Drawing.Point(3, 83);
            this.importSettingsGroupBox.Name = "importSettingsGroupBox";
            this.importSettingsGroupBox.Size = new System.Drawing.Size(616, 79);
            this.importSettingsGroupBox.TabIndex = 1;
            this.importSettingsGroupBox.TabStop = false;
            this.importSettingsGroupBox.Text = "Import Settings";
            // 
            // importSettingsTableLayoutPanel
            // 
            this.importSettingsTableLayoutPanel.ColumnCount = 2;
            this.importSettingsTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.importSettingsTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 90F));
            this.importSettingsTableLayoutPanel.Controls.Add(this.defaultCompressionGroupBox, 0, 0);
            this.importSettingsTableLayoutPanel.Controls.Add(this.mipsGroupBox, 1, 0);
            this.importSettingsTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.importSettingsTableLayoutPanel.Location = new System.Drawing.Point(3, 18);
            this.importSettingsTableLayoutPanel.Name = "importSettingsTableLayoutPanel";
            this.importSettingsTableLayoutPanel.RowCount = 1;
            this.importSettingsTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.importSettingsTableLayoutPanel.Size = new System.Drawing.Size(610, 58);
            this.importSettingsTableLayoutPanel.TabIndex = 0;
            // 
            // defaultCompressionGroupBox
            // 
            this.defaultCompressionGroupBox.Controls.Add(this.defaultCompressionFlowLayoutPanel);
            this.defaultCompressionGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.defaultCompressionGroupBox.Location = new System.Drawing.Point(3, 3);
            this.defaultCompressionGroupBox.Name = "defaultCompressionGroupBox";
            this.defaultCompressionGroupBox.Size = new System.Drawing.Size(514, 52);
            this.defaultCompressionGroupBox.TabIndex = 0;
            this.defaultCompressionGroupBox.TabStop = false;
            this.defaultCompressionGroupBox.Text = "Default Compression";
            // 
            // defaultCompressionFlowLayoutPanel
            // 
            this.defaultCompressionFlowLayoutPanel.Controls.Add(this.defaultCompressionComboBox);
            this.defaultCompressionFlowLayoutPanel.Controls.Add(this.defaultCompressionLabel);
            this.defaultCompressionFlowLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.defaultCompressionFlowLayoutPanel.Location = new System.Drawing.Point(3, 18);
            this.defaultCompressionFlowLayoutPanel.Name = "defaultCompressionFlowLayoutPanel";
            this.defaultCompressionFlowLayoutPanel.Size = new System.Drawing.Size(508, 31);
            this.defaultCompressionFlowLayoutPanel.TabIndex = 1;
            // 
            // defaultCompressionComboBox
            // 
            this.defaultCompressionComboBox.FormattingEnabled = true;
            this.defaultCompressionComboBox.Location = new System.Drawing.Point(3, 3);
            this.defaultCompressionComboBox.Name = "defaultCompressionComboBox";
            this.defaultCompressionComboBox.Size = new System.Drawing.Size(121, 24);
            this.defaultCompressionComboBox.TabIndex = 0;
            // 
            // defaultCompressionLabel
            // 
            this.defaultCompressionLabel.AutoSize = true;
            this.defaultCompressionLabel.Location = new System.Drawing.Point(130, 6);
            this.defaultCompressionLabel.Margin = new System.Windows.Forms.Padding(3, 6, 3, 3);
            this.defaultCompressionLabel.Name = "defaultCompressionLabel";
            this.defaultCompressionLabel.Size = new System.Drawing.Size(254, 17);
            this.defaultCompressionLabel.TabIndex = 1;
            this.defaultCompressionLabel.Text = "Only used for files without a BTI fmt tag";
            // 
            // mipsGroupBox
            // 
            this.mipsGroupBox.Controls.Add(this.mipsFlowLayoutPanel);
            this.mipsGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mipsGroupBox.Location = new System.Drawing.Point(523, 3);
            this.mipsGroupBox.Name = "mipsGroupBox";
            this.mipsGroupBox.Size = new System.Drawing.Size(84, 52);
            this.mipsGroupBox.TabIndex = 1;
            this.mipsGroupBox.TabStop = false;
            this.mipsGroupBox.Text = "Mips";
            // 
            // mipsFlowLayoutPanel
            // 
            this.mipsFlowLayoutPanel.Controls.Add(this.mipsNumericUpDown);
            this.mipsFlowLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mipsFlowLayoutPanel.Location = new System.Drawing.Point(3, 18);
            this.mipsFlowLayoutPanel.Name = "mipsFlowLayoutPanel";
            this.mipsFlowLayoutPanel.Size = new System.Drawing.Size(78, 31);
            this.mipsFlowLayoutPanel.TabIndex = 0;
            // 
            // mipsNumericUpDown
            // 
            this.mipsNumericUpDown.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mipsNumericUpDown.Location = new System.Drawing.Point(3, 3);
            this.mipsNumericUpDown.Name = "mipsNumericUpDown";
            this.mipsNumericUpDown.Size = new System.Drawing.Size(72, 22);
            this.mipsNumericUpDown.TabIndex = 0;
            // 
            // progressGroupBox
            // 
            this.progressGroupBox.Controls.Add(this.progressTableLayoutPanel);
            this.progressGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.progressGroupBox.Location = new System.Drawing.Point(3, 168);
            this.progressGroupBox.Name = "progressGroupBox";
            this.progressGroupBox.Size = new System.Drawing.Size(616, 109);
            this.progressGroupBox.TabIndex = 2;
            this.progressGroupBox.TabStop = false;
            this.progressGroupBox.Text = "Progress";
            // 
            // progressTableLayoutPanel
            // 
            this.progressTableLayoutPanel.ColumnCount = 2;
            this.progressTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.progressTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.progressTableLayoutPanel.Controls.Add(this.progressBar, 0, 1);
            this.progressTableLayoutPanel.Controls.Add(this.processingLabel, 0, 0);
            this.progressTableLayoutPanel.Controls.Add(this.convertButton, 0, 2);
            this.progressTableLayoutPanel.Controls.Add(this.percentageLabel, 1, 0);
            this.progressTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.progressTableLayoutPanel.Location = new System.Drawing.Point(3, 18);
            this.progressTableLayoutPanel.Name = "progressTableLayoutPanel";
            this.progressTableLayoutPanel.RowCount = 3;
            this.progressTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.progressTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.progressTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 60F));
            this.progressTableLayoutPanel.Size = new System.Drawing.Size(610, 88);
            this.progressTableLayoutPanel.TabIndex = 0;
            // 
            // progressBar
            // 
            this.progressTableLayoutPanel.SetColumnSpan(this.progressBar, 2);
            this.progressBar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.progressBar.Location = new System.Drawing.Point(3, 17);
            this.progressBar.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(604, 17);
            this.progressBar.TabIndex = 0;
            // 
            // processingLabel
            // 
            this.processingLabel.AutoSize = true;
            this.processingLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.processingLabel.Location = new System.Drawing.Point(3, 0);
            this.processingLabel.Name = "processingLabel";
            this.processingLabel.Size = new System.Drawing.Size(554, 17);
            this.processingLabel.TabIndex = 1;
            this.processingLabel.Text = "Processing ...";
            // 
            // convertButton
            // 
            this.progressTableLayoutPanel.SetColumnSpan(this.convertButton, 2);
            this.convertButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.convertButton.Location = new System.Drawing.Point(3, 37);
            this.convertButton.Name = "convertButton";
            this.convertButton.Size = new System.Drawing.Size(604, 48);
            this.convertButton.TabIndex = 2;
            this.convertButton.Text = "Convert";
            this.convertButton.UseVisualStyleBackColor = true;
            this.convertButton.Click += new System.EventHandler(this.convertButton_Click);
            // 
            // percentageLabel
            // 
            this.percentageLabel.AutoSize = true;
            this.percentageLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.percentageLabel.Location = new System.Drawing.Point(563, 0);
            this.percentageLabel.Name = "percentageLabel";
            this.percentageLabel.Size = new System.Drawing.Size(44, 17);
            this.percentageLabel.TabIndex = 3;
            this.percentageLabel.Text = "100%";
            // 
            // outputGroupBox
            // 
            this.outputGroupBox.Controls.Add(this.outputTableLayoutPanel);
            this.outputGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.outputGroupBox.Location = new System.Drawing.Point(3, 283);
            this.outputGroupBox.Name = "outputGroupBox";
            this.outputGroupBox.Size = new System.Drawing.Size(616, 147);
            this.outputGroupBox.TabIndex = 3;
            this.outputGroupBox.TabStop = false;
            this.outputGroupBox.Text = "Output";
            // 
            // outputTableLayoutPanel
            // 
            this.outputTableLayoutPanel.ColumnCount = 1;
            this.outputTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.outputTableLayoutPanel.Controls.Add(this.outputRichTextBox, 0, 0);
            this.outputTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.outputTableLayoutPanel.Location = new System.Drawing.Point(3, 18);
            this.outputTableLayoutPanel.Name = "outputTableLayoutPanel";
            this.outputTableLayoutPanel.RowCount = 1;
            this.outputTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.outputTableLayoutPanel.Size = new System.Drawing.Size(610, 126);
            this.outputTableLayoutPanel.TabIndex = 0;
            // 
            // outputRichTextBox
            // 
            this.outputRichTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.outputRichTextBox.Location = new System.Drawing.Point(3, 3);
            this.outputRichTextBox.Name = "outputRichTextBox";
            this.outputRichTextBox.Size = new System.Drawing.Size(604, 120);
            this.outputRichTextBox.TabIndex = 0;
            this.outputRichTextBox.Text = "";
            // 
            // allMapRadioButton
            // 
            this.allMapRadioButton.AutoSize = true;
            this.allMapRadioButton.Location = new System.Drawing.Point(314, 3);
            this.allMapRadioButton.Name = "allMapRadioButton";
            this.allMapRadioButton.Size = new System.Drawing.Size(44, 21);
            this.allMapRadioButton.TabIndex = 4;
            this.allMapRadioButton.TabStop = true;
            this.allMapRadioButton.Text = "All";
            this.allMapRadioButton.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(622, 433);
            this.Controls.Add(this.mainTableLayoutPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "Form1";
            this.Text = "AoM DDT Converter";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.mainTableLayoutPanel.ResumeLayout(false);
            this.exportSettingsGroupBox.ResumeLayout(false);
            this.exportSettingsTableLayoutPanel.ResumeLayout(false);
            this.mapGenerationGroupBox.ResumeLayout(false);
            this.mapGenFlowLayoutPanel.ResumeLayout(false);
            this.mapGenFlowLayoutPanel.PerformLayout();
            this.importSettingsGroupBox.ResumeLayout(false);
            this.importSettingsTableLayoutPanel.ResumeLayout(false);
            this.defaultCompressionGroupBox.ResumeLayout(false);
            this.defaultCompressionFlowLayoutPanel.ResumeLayout(false);
            this.defaultCompressionFlowLayoutPanel.PerformLayout();
            this.mipsGroupBox.ResumeLayout(false);
            this.mipsFlowLayoutPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.mipsNumericUpDown)).EndInit();
            this.progressGroupBox.ResumeLayout(false);
            this.progressTableLayoutPanel.ResumeLayout(false);
            this.progressTableLayoutPanel.PerformLayout();
            this.outputGroupBox.ResumeLayout(false);
            this.outputTableLayoutPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel mainTableLayoutPanel;
        private System.Windows.Forms.GroupBox exportSettingsGroupBox;
        private System.Windows.Forms.TableLayoutPanel exportSettingsTableLayoutPanel;
        private System.Windows.Forms.GroupBox mapGenerationGroupBox;
        private System.Windows.Forms.FlowLayoutPanel mapGenFlowLayoutPanel;
        private System.Windows.Forms.GroupBox importSettingsGroupBox;
        private System.Windows.Forms.TableLayoutPanel importSettingsTableLayoutPanel;
        private System.Windows.Forms.GroupBox defaultCompressionGroupBox;
        private System.Windows.Forms.FlowLayoutPanel defaultCompressionFlowLayoutPanel;
        private System.Windows.Forms.ComboBox defaultCompressionComboBox;
        private System.Windows.Forms.GroupBox progressGroupBox;
        private System.Windows.Forms.TableLayoutPanel progressTableLayoutPanel;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label processingLabel;
        private System.Windows.Forms.Button convertButton;
        private System.Windows.Forms.Label percentageLabel;
        private System.Windows.Forms.Label defaultCompressionLabel;
        private System.Windows.Forms.GroupBox mipsGroupBox;
        private System.Windows.Forms.FlowLayoutPanel mipsFlowLayoutPanel;
        private System.Windows.Forms.NumericUpDown mipsNumericUpDown;
        private System.Windows.Forms.GroupBox outputGroupBox;
        private System.Windows.Forms.TableLayoutPanel outputTableLayoutPanel;
        private System.Windows.Forms.RichTextBox outputRichTextBox;
        private System.Windows.Forms.RadioButton nmMapRadioButton;
        private System.Windows.Forms.RadioButton specMapRadioButton;
        private System.Windows.Forms.RadioButton glossMapRadioButton;
        private System.Windows.Forms.RadioButton noneMapRadioButton;
        private System.Windows.Forms.RadioButton allMapRadioButton;
    }
}

