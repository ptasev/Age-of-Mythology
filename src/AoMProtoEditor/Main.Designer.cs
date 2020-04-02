namespace AoMProtoEditor
{
    partial class Main
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
            this.mainMenuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.tlvImageList = new System.Windows.Forms.ImageList(this.components);
            this.mainTabControl = new System.Windows.Forms.TabControl();
            this.editElementToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editAttributeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addElementToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeElementToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addAttributeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeAttributeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.undoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyElementToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyAttributeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.mainMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainMenuStrip
            // 
            this.mainMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem});
            this.mainMenuStrip.Location = new System.Drawing.Point(0, 0);
            this.mainMenuStrip.Name = "mainMenuStrip";
            this.mainMenuStrip.Size = new System.Drawing.Size(784, 24);
            this.mainMenuStrip.TabIndex = 0;
            this.mainMenuStrip.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addElementToolStripMenuItem,
            this.addAttributeToolStripMenuItem,
            this.toolStripSeparator1,
            this.removeElementToolStripMenuItem,
            this.removeAttributeToolStripMenuItem,
            this.toolStripSeparator2,
            this.editElementToolStripMenuItem,
            this.editAttributeToolStripMenuItem,
            this.toolStripSeparator3,
            this.undoToolStripMenuItem,
            this.copyElementToolStripMenuItem,
            this.copyAttributeToolStripMenuItem,
            this.pasteToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.editToolStripMenuItem.Text = "Edit";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(223, 6);
            // 
            // openFileDialog
            // 
            this.openFileDialog.FileName = "proto.xml";
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.FileName = "proto.xml";
            // 
            // tlvImageList
            // 
            this.tlvImageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.tlvImageList.ImageSize = new System.Drawing.Size(16, 16);
            this.tlvImageList.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // mainTabControl
            // 
            this.mainTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainTabControl.Location = new System.Drawing.Point(0, 24);
            this.mainTabControl.Name = "mainTabControl";
            this.mainTabControl.SelectedIndex = 0;
            this.mainTabControl.Size = new System.Drawing.Size(784, 578);
            this.mainTabControl.TabIndex = 3;
            this.mainTabControl.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.mainTabControl_MouseDoubleClick);
            // 
            // editElementToolStripMenuItem
            // 
            this.editElementToolStripMenuItem.Name = "editElementToolStripMenuItem";
            this.editElementToolStripMenuItem.Size = new System.Drawing.Size(226, 22);
            this.editElementToolStripMenuItem.Text = "Edit Element";
            this.editElementToolStripMenuItem.Visible = false;
            this.editElementToolStripMenuItem.Click += new System.EventHandler(this.editElementToolStripMenuItem_Click);
            // 
            // editAttributeToolStripMenuItem
            // 
            this.editAttributeToolStripMenuItem.Name = "editAttributeToolStripMenuItem";
            this.editAttributeToolStripMenuItem.Size = new System.Drawing.Size(226, 22);
            this.editAttributeToolStripMenuItem.Text = "Edit Attribute";
            this.editAttributeToolStripMenuItem.Visible = false;
            this.editAttributeToolStripMenuItem.Click += new System.EventHandler(this.editAttributeToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(223, 6);
            this.toolStripSeparator2.Visible = false;
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Image = global::AoMProtoEditor.Properties.Resources.page_code;
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Image = global::AoMProtoEditor.Properties.Resources.page_save;
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // addElementToolStripMenuItem
            // 
            this.addElementToolStripMenuItem.Image = global::AoMProtoEditor.Properties.Resources.nelem_add;
            this.addElementToolStripMenuItem.Name = "addElementToolStripMenuItem";
            this.addElementToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.E)));
            this.addElementToolStripMenuItem.Size = new System.Drawing.Size(226, 22);
            this.addElementToolStripMenuItem.Text = "Add Element";
            this.addElementToolStripMenuItem.Click += new System.EventHandler(this.addElementToolStripMenuItem_Click);
            // 
            // removeElementToolStripMenuItem
            // 
            this.removeElementToolStripMenuItem.Image = global::AoMProtoEditor.Properties.Resources.nelem_delete;
            this.removeElementToolStripMenuItem.Name = "removeElementToolStripMenuItem";
            this.removeElementToolStripMenuItem.Size = new System.Drawing.Size(226, 22);
            this.removeElementToolStripMenuItem.Text = "Remove Element";
            this.removeElementToolStripMenuItem.Click += new System.EventHandler(this.removeElementToolStripMenuItem_Click);
            // 
            // addAttributeToolStripMenuItem
            // 
            this.addAttributeToolStripMenuItem.Image = global::AoMProtoEditor.Properties.Resources.nattrib_add;
            this.addAttributeToolStripMenuItem.Name = "addAttributeToolStripMenuItem";
            this.addAttributeToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.E)));
            this.addAttributeToolStripMenuItem.Size = new System.Drawing.Size(226, 22);
            this.addAttributeToolStripMenuItem.Text = "Add Attribute";
            this.addAttributeToolStripMenuItem.Click += new System.EventHandler(this.addAttributeToolStripMenuItem_Click);
            // 
            // removeAttributeToolStripMenuItem
            // 
            this.removeAttributeToolStripMenuItem.Image = global::AoMProtoEditor.Properties.Resources.nattrib_delete;
            this.removeAttributeToolStripMenuItem.Name = "removeAttributeToolStripMenuItem";
            this.removeAttributeToolStripMenuItem.Size = new System.Drawing.Size(226, 22);
            this.removeAttributeToolStripMenuItem.Text = "Remove Attribute";
            this.removeAttributeToolStripMenuItem.Click += new System.EventHandler(this.removeAttributeToolStripMenuItem_Click);
            // 
            // undoToolStripMenuItem
            // 
            this.undoToolStripMenuItem.Image = global::AoMProtoEditor.Properties.Resources.arrow_undo2;
            this.undoToolStripMenuItem.Name = "undoToolStripMenuItem";
            this.undoToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Z)));
            this.undoToolStripMenuItem.Size = new System.Drawing.Size(226, 22);
            this.undoToolStripMenuItem.Text = "Undo";
            this.undoToolStripMenuItem.Click += new System.EventHandler(this.undoToolStripMenuItem_Click);
            // 
            // copyElementToolStripMenuItem
            // 
            this.copyElementToolStripMenuItem.Image = global::AoMProtoEditor.Properties.Resources.nelem_paste;
            this.copyElementToolStripMenuItem.Name = "copyElementToolStripMenuItem";
            this.copyElementToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.copyElementToolStripMenuItem.Size = new System.Drawing.Size(226, 22);
            this.copyElementToolStripMenuItem.Text = "Copy Element";
            this.copyElementToolStripMenuItem.Click += new System.EventHandler(this.copyElementToolStripMenuItem_Click);
            // 
            // copyAttributeToolStripMenuItem
            // 
            this.copyAttributeToolStripMenuItem.Image = global::AoMProtoEditor.Properties.Resources.nattrib_paste;
            this.copyAttributeToolStripMenuItem.Name = "copyAttributeToolStripMenuItem";
            this.copyAttributeToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.C)));
            this.copyAttributeToolStripMenuItem.Size = new System.Drawing.Size(226, 22);
            this.copyAttributeToolStripMenuItem.Text = "Copy Attribute";
            this.copyAttributeToolStripMenuItem.Click += new System.EventHandler(this.copyAttributeToolStripMenuItem_Click);
            // 
            // pasteToolStripMenuItem
            // 
            this.pasteToolStripMenuItem.Image = global::AoMProtoEditor.Properties.Resources.tag_paste;
            this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
            this.pasteToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.pasteToolStripMenuItem.Size = new System.Drawing.Size(226, 22);
            this.pasteToolStripMenuItem.Text = "Paste";
            this.pasteToolStripMenuItem.Click += new System.EventHandler(this.pasteToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(223, 6);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 602);
            this.Controls.Add(this.mainTabControl);
            this.Controls.Add(this.mainMenuStrip);
            this.MainMenuStrip = this.mainMenuStrip;
            this.Name = "Main";
            this.Text = "Ryder XML Editor";
            this.mainMenuStrip.ResumeLayout(false);
            this.mainMenuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip mainMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ImageList tlvImageList;
        private System.Windows.Forms.TabControl mainTabControl;
        private System.Windows.Forms.ToolStripMenuItem addElementToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addAttributeToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem removeElementToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeAttributeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editElementToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editAttributeToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem undoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyElementToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyAttributeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pasteToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
    }
}

