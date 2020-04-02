using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BrightIdeasSoftware;

namespace AoMProtoEditor
{
    public enum ProtoPropertyEditMode
    {
        NONE, ADDELEMENT, ADDATTRIBUTE, EDIT
    };

    public partial class Main : Form
    {
        // 1.0 -- Schema Selector, OpenWith, Icon, Better Copy(Multiple lines)
        private TabPage currentFileTab
        {
            get
            {
                return mainTabControl.SelectedTab;
            }
        }
        private ProtoFile currentFile
        {
            get
            {
                if (currentFileTab != null)
                    return (ProtoFile)currentFileTab.Tag;
                else
                    return null;
            }
        }
        private TabControl currentFileTabControl
        {
            get
            {
                if (currentFileTab != null)
                    return (TabControl)currentFileTab.Controls[0];
                else
                    return null;
            }
        }
        private TabPage currentElementTab
        {
            get
            {
                if (currentFileTab != null)
                    return currentFileTabControl.SelectedTab;
                else
                    return null;
            }
        }
        private ProtoElement currentElement
        {
            get
            {
                if (currentElementTab != null)
                    return (ProtoElement)currentElementTab.Tag;
                else
                    return null;
            }
        }
        private TreeListView currentElementTree
        {
            get
            {
                if (currentElementTab != null)
                    return (TreeListView)currentElementTab.Controls[0];
                else
                    return null;
            }
        }
        private ProtoElement selectedElement
        {
            get
            {
                if (currentElementTree != null)
                    return (ProtoElement)currentElementTree.SelectedObject;
                else
                    return null;
            }
        }
        private ObjectListView currentElementList
        {
            get
            {
                if (currentFileTab != null)
                    return (ObjectListView)currentFileTab.Controls[1].Controls[0];
                else
                    return null;
            }
        }
        private ProtoAttribute selectedAttribute
        {
            get
            {
                if (currentElementList != null)
                    return (ProtoAttribute)currentElementList.SelectedObject;
                else
                    return null;
            }
        }

        public Main()
        {
            InitializeComponent();
            // tlvImageList
            tlvImageList.Images.Add("elem", (Image)Properties.Resources.ResourceManager.GetObject("nelem"));
            tlvImageList.Images.Add("attr", (Image)Properties.Resources.ResourceManager.GetObject("nattrib"));
            //this.BackColor = Color.FromArgb(228, 205, 122);
            //mainMenuStrip.BackColor = Color.FromArgb(228, 205, 122);
            // entityDataGridView
            //entityDataGridView.RowHeadersVisible = false;
            //entityDataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            //entityDataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            //entityDataGridView.AllowUserToResizeRows = false;
            //entityDataGridView.DefaultCellStyle.BackColor = Color.FromArgb(252, 248, 216);
            // entityTabControl
        }

        #region MainMenu
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                if (fileOpened(openFileDialog.FileName))
                    return;

                System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                sw.Start();
                ProtoFile file = new ProtoFile(File.Open(Application.StartupPath + "\\defaultSchema.xml", FileMode.Open, FileAccess.Read, FileShare.Read),
                    File.Open(openFileDialog.FileName, FileMode.Open, FileAccess.Read, FileShare.Read));
                sw.Stop();
                //MessageBox.Show(sw.ElapsedMilliseconds.ToString());

                file.Schema.WriteFull(File.Open(Application.StartupPath + "\\dumpSchema.xml", FileMode.Create, FileAccess.Write, FileShare.Read));

                addFile(file, false);

                openFileDialog.Dispose();
            }
        }
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentFile == null)
                return;

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                currentFile.Write(File.Open(saveFileDialog.FileName, FileMode.Create, FileAccess.Write, FileShare.Read));

                saveFileDialog.Dispose();
            }
        }

        private void addElementToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (selectedElement == null || selectedElement.Definition.ChildElement.Count == 0)
                return;

            using (PropertyBox box = new PropertyBox(selectedElement, ProtoPropertyEditMode.ADDELEMENT))
            {
                if (box.ShowDialog() == DialogResult.OK)
                {
                    refreshTree(selectedElement.ChildElement[selectedElement.ChildElement.Count - 1], true, true);
                    rebuildParentTree(selectedElement.ParentProperty);
                    //currentElementTree.Expand(selectedElement);
                    //currentElementTree.SelectedObject = selectedElement.ChildElement[selectedElement.ChildElement.Count - 1];
                    //currentElementTree.TopItemIndex = Math.Max(0, currentElementTree.SelectedItem.Index + selectedElement.ChildElement.Count - 10);
                }
            }
        }
        private void addAttributeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (selectedElement == null || selectedElement.Definition.Attribute.Count == 0
                || selectedElement.Attribute.Count >= selectedElement.Definition.Attribute.Count)
                return;

            using (PropertyBox box = new PropertyBox(selectedElement, ProtoPropertyEditMode.ADDATTRIBUTE))
            {
                if (box.ShowDialog() == DialogResult.OK)
                {
                    refreshTree(selectedElement, false, false);

                    currentElementList.SelectedIndex = selectedElement.Attribute.Count - 1;
                    currentElementList.EnsureVisible(currentElementList.SelectedIndex);
                    this.ActiveControl = currentElementList;
                }
            }
        }
        private void editElementToolStripMenuItem_Click(object sender, EventArgs e)
        {
            editElement();
        }
        private void editElement()
        {
            if (selectedElement == null)
                return;

            using (PropertyBox box = new PropertyBox(selectedElement, ProtoPropertyEditMode.EDIT))
            {
                if (box.ShowDialog() == DialogResult.OK)
                {
                    refreshTree((ProtoElement)box.Prop, false, false);
                }
            }
        }
        private void editAttributeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            editAttribute();
        }
        private void editAttribute()
        {
            if (selectedElement == null || selectedAttribute == null)
                return;

            using (PropertyBox box = new PropertyBox(selectedAttribute, ProtoPropertyEditMode.EDIT))
            {
                if (box.ShowDialog() == DialogResult.OK)
                {
                    int index = currentElementList.SelectedIndex;
                    refreshTree((ProtoElement)currentElementTree.SelectedObject, false, false);

                    currentElementList.SelectedIndex = index;
                    currentElementList.EnsureVisible(currentElementList.SelectedIndex);
                    this.ActiveControl = currentElementList;
                }
            }
        }
        private void removeElementToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (selectedElement == null || selectedElement.ParentProperty == null)
                return;

            ProtoElement parentElem = selectedElement.ParentProperty;
            parentElem.ChildElement.Remove(selectedElement);
            removeElementTab(selectedElement);
            currentFile.Editor.PushUndoAction(selectedElement, ProtoUndoType.ADD, null);

            refreshTree((ProtoElement)currentElementTree.GetModelObject(currentElementTree.SelectedIndex - 1), true, false);
            rebuildParentTree(parentElem);
        }
        private void removeAttributeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (selectedAttribute == null)
                return;

            selectedAttribute.ParentProperty.Attribute.Remove(selectedAttribute);
            currentFile.Editor.PushUndoAction(selectedAttribute, ProtoUndoType.ADD, null);
            int index = currentElementList.SelectedIndex - 1;
            refreshTree(selectedElement, false, false);

            if (currentElementList.Visible)
            {
                currentElementList.SelectedIndex = index > 0 ? index : 0;
                currentElementList.EnsureVisible(currentElementList.SelectedIndex);
                this.ActiveControl = currentElementList;
            }
        }
        private void refreshTree(ProtoElement selectObject, bool rebuild, bool expand)
        {
            if (rebuild)
                currentElementTree.RebuildAll(true); // rebuild tree
            if (expand)
                currentElementTree.Expand(selectObject.ParentProperty);

            // Resseting selected object is to fix arrow key scrolling, and rebuild attribute list
            currentElementTree.SelectedObject = null;
            currentElementTree.SelectedObject = selectObject;
            if (currentElementTree.SelectedIndex >= 0)
                currentElementTree.EnsureVisible(currentElementTree.SelectedIndex);

            this.ActiveControl = currentElementTree;
        }
        private void rebuildParentTree(ProtoElement elem)
        {
            while (elem != null)
            {
                foreach (TabPage page in currentFileTabControl.TabPages)
                {
                    if (page == currentElementTab)
                        continue;
                    ProtoElement tabElem = (ProtoElement)page.Tag;
                    if (object.ReferenceEquals(tabElem, elem))
                    {
                        ((TreeListView)page.Controls[0]).RebuildAll(true);
                        break;
                    }
                }
                elem = elem.ParentProperty;
            }
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentFile.Editor.UndoAction.Count == 0)
                return;

            ProtoUndoAction action = currentFile.Editor.PopUndoAction();
            if (action.Type == ProtoUndoType.ADD)
            {
                if (action.Property is ProtoElement)
                {
                    selectTabContainingElement(action.Property.ParentProperty);
                    refreshTree((ProtoElement)action.Property, true, true);
                    rebuildParentTree(selectedElement.ParentProperty);
                }
                else // is ProtoAttribute
                {
                    selectTabContainingElement(action.Property.ParentProperty);
                    refreshTree(action.Property.ParentProperty, false, false);

                    currentElementList.SelectedObject = action.Property;
                    currentElementList.EnsureVisible(currentElementList.SelectedIndex);
                    this.ActiveControl = currentElementList;
                }
            }
            else if (action.Type == ProtoUndoType.EDIT)
            {
                if (action.Property is ProtoElement)
                {
                    selectTabContainingElement((ProtoElement)action.Property);
                    refreshTree((ProtoElement)action.Property, false, false);
                }
                else // is ProtoAttribute
                {
                    selectTabContainingElement(action.Property.ParentProperty);
                    refreshTree(action.Property.ParentProperty, false, false);

                    currentElementList.SelectedObject = action.Property;
                    currentElementList.EnsureVisible(currentElementList.SelectedIndex);
                    this.ActiveControl = currentElementList;
                }
            }
            else if (action.Type == ProtoUndoType.REMOVE)
            {
                if (action.Property is ProtoElement)
                {
                    removeElementTab((ProtoElement)action.Property);
                    selectTabContainingElement((ProtoElement)action.Property);
                    refreshTree((ProtoElement)action.Property, false, false);

                    refreshTree((ProtoElement)currentElementTree.GetModelObject(currentElementTree.SelectedIndex - 1), true, false);
                    rebuildParentTree(action.Property.ParentProperty);
                }
                else // is ProtoAttribute
                {
                    selectTabContainingElement(action.Property.ParentProperty);
                    refreshTree(action.Property.ParentProperty, false, false);

                    if (currentElementList.Visible)
                    {
                        this.ActiveControl = currentElementList;
                    }
                }
            }
        }
        private void selectTabContainingElement(ProtoElement elem)
        {
            if (currentElementTree.ModelToItem(elem) != null)
                return;

            foreach (TabPage page in currentFileTabControl.TabPages)
            {
                if (page == currentElementTab)
                    continue;
                TreeListView tabTree = (TreeListView)page.Controls[0];
                if (tabTree.ModelToItem(elem) != null)
                {
                    currentFileTabControl.SelectedTab = page;
                    break;
                }
            }
        }
        private void copyElementToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (selectedElement == null)
                return;

            currentFile.Editor.Copy(selectedElement);
        }
        private void copyAttributeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (selectedAttribute == null)
                return;

            currentFile.Editor.Copy(selectedAttribute);
        }
        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (selectedElement == null || !currentFile.Editor.Paste(selectedElement))
                return;

            ProtoProperty prop = currentFile.Editor.CopyProperty;
            if (prop is ProtoElement)
            {
                refreshTree(selectedElement.ChildElement[selectedElement.ChildElement.Count - 1], true, true);
                rebuildParentTree(selectedElement.ParentProperty);
            }
            else
            {
                refreshTree(selectedElement, false, false);

                currentElementList.SelectedObject = prop;
                currentElementList.EnsureVisible(currentElementList.SelectedIndex);
                this.ActiveControl = currentElementList;
            }
        }
        #endregion

        private bool fileOpened(string fileName)
        {
            foreach (TabPage page in mainTabControl.TabPages)
            {
                if (((ProtoFile)page.Tag).Name == fileName)
                {
                    mainTabControl.SelectedTab = page;
                    return true;
                }
            }

            return false;
        }
        private void addFile(ProtoFile file, bool expandAll = true)
        {
            TabPage page = new TabPage(Path.GetFileName(file.Name));
            //page.BackColor = Color.FromArgb(228, 205, 122);
            page.Tag = file;

            TabControl tc = new TabControl();
            tc.Dock = DockStyle.Fill;
            tc.SelectedIndexChanged += tc_SelectedIndexChanged;
            tc.MouseDoubleClick += tc_MouseDoubleClick;
            addTab(tc, file.RootProperty, false);

            Panel p = new Panel();
            p.Dock = DockStyle.Bottom;
            p.Size = new System.Drawing.Size(p.Size.Width, 216);
            p.Visible = false;

            ObjectListView olv = new ObjectListView();
            olv.BackColor = Color.FromArgb(252, 248, 216);
            olv.Dock = DockStyle.Fill;
            olv.HideSelection = false;
            olv.GridLines = true;
            olv.FullRowSelect = true;
            olv.SmallImageList = tlvImageList;
            OLVColumn col = new OLVColumn("Name", "Name");
            col.Sortable = false;
            col.UseFiltering = false;
            col.IsEditable = false;
            col.Width = 200;
            col.ImageGetter = delegate(object x) { return "attr"; };
            olv.Columns.Add(col);
            col = new OLVColumn("Value", "Value");
            col.Sortable = false;
            col.UseFiltering = false;
            col.IsEditable = false;
            col.FillsFreeSpace = true;
            olv.Columns.Add(col);
            olv.MouseDoubleClick += olv_MouseDoubleClick;
            olv.MouseEnter += olv_MouseEnter;

            p.Controls.Add(olv);
            page.Controls.Add(tc);
            page.Controls.Add(p);
            mainTabControl.TabPages.Add(page);
            mainTabControl.SelectedTab = page;
            if (currentElementTree.Items.Count > 0)
                currentElementTree.SelectedIndex = 0;
            this.ActiveControl = currentElementTree;
        }

        private bool tabOpened(ProtoElement elem)
        {
            foreach (TabPage tab in ((TabControl)currentFileTab.Controls[0]).TabPages)
            {
                if (object.ReferenceEquals(tab.Tag, elem))
                {
                    ((TabControl)currentFileTab.Controls[0]).SelectedTab = tab;
                    refreshTree(elem, false, false);
                    return true;
                }
            }

            return false;
        }
        private void addTab(TabControl tc, ProtoElement prop, bool expandAll = true)
        {
            TabPage page = new TabPage(prop.DisplayName);
            //page.BackColor = Color.FromArgb(228, 205, 122);
            page.Tag = prop;

            TreeListView tlv = new TreeListView();
            tlv.BackColor = Color.FromArgb(252, 248, 216);
            tlv.Dock = DockStyle.Fill;
            tlv.HideSelection = false;
            tlv.UseFiltering = false;
            tlv.FullRowSelect = true;
            tlv.GridLines = true;
            tlv.CellEditActivation = ObjectListView.CellEditActivateMode.None;
            tlv.SmallImageList = tlvImageList;

            OLVColumn col = new OLVColumn("Name", "Name");
            col.IsEditable = false;
            col.Width = 200;
            col.ImageGetter = delegate(object x)
            {
                if (x is ProtoElement)
                    return "elem";
                else
                    return "attr";
            };
            tlv.Columns.Add(col);
            col = new OLVColumn("Attribute", "DisplayAttribute");
            col.IsEditable = false;
            col.Width = 200;
            tlv.Columns.Add(col);
            col = new OLVColumn("Value", "Value");
            col.UseFiltering = false;
            col.Sortable = false;
            col.IsEditable = false;
            col.FillsFreeSpace = true;
            col.MinimumWidth = 50;
            tlv.Columns.Add(col);

            tlv.MouseDoubleClick += tlv_MouseDoubleClick;
            tlv.SelectedIndexChanged += tlv_SelectedIndexChanged;
            tlv.MouseEnter += tlv_MouseEnter;
            tlv.CanExpandGetter = delegate(object x)
            {
                if (x is ProtoElement)
                    return ((ProtoElement)x).ChildElement.Count > 0;// || ((ProtoProperty)x).AttributeCount > 0;

                return false;
            };
            tlv.ChildrenGetter = delegate(object x)
            {
                List<object> obj = new List<object>();
                obj.AddRange(((ProtoElement)x).Attributes);
                obj.AddRange(((ProtoElement)x).ChildElement);
                return ((ProtoElement)x).ChildElement;
            };

            tlv.Roots = new ProtoElement[] { prop };
            if (expandAll)
                tlv.ExpandAll();

            page.Controls.Add(tlv);
            tc.TabPages.Add(page);
            tc.SelectedTab = page;
            if (tlv.Items.Count > 0)
                tlv.SelectedIndex = 0;
            this.ActiveControl = currentElementTree;
        }
        private void removeElementTab(ProtoElement elem)
        {
            foreach (TabPage tab in currentFileTabControl.TabPages)
            {
                if (object.ReferenceEquals(tab.Tag, elem))
                {
                    currentFileTabControl.TabPages.Remove(tab);
                    break;
                }
            }
            foreach (ProtoElement child in elem.ChildElement)
            {
                if (!child.AllowTab)
                    continue;
                foreach (TabPage tab in currentFileTabControl.TabPages)
                {
                    if (object.ReferenceEquals(tab.Tag, child))
                    {
                        currentFileTabControl.TabPages.Remove(tab);
                        break;
                    }
                }
            }
        }

        #region Events
        void tc_SelectedIndexChanged(object sender, EventArgs e)
        {
            refreshTree(selectedElement, false, false);
        }
        void tc_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            TabControl tabControl1 = sender as TabControl;
            for (int i = 1; i < tabControl1.TabCount; ++i)
            {
                if (tabControl1.GetTabRect(i).Contains(e.Location))
                {
                    tabControl1.TabPages[i].Dispose();
                    break;
                }
            }
        }
        void tlv_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (selectedElement != null && selectedElement.Attribute.Count > 0)
            {
                currentElementList.SetObjects(selectedElement.Attribute);
                currentElementList.SelectedIndex = 0;
                currentElementList.Parent.Visible = true;
                currentElementTree.EnsureVisible(currentElementTree.SelectedIndex);
            }
            else
            {
                currentElementList.SelectedIndex = -1;
                currentElementList.Parent.Visible = false;
            }
        }
        void tlv_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            OlvListViewHitTestInfo ht = currentElementTree.OlvHitTest(e.X, e.Y);
            ProtoElement prop = (ProtoElement)ht.RowObject;
            if (prop != null)
            {
                if (prop.AllowTab && !tabOpened(prop))
                {
                    addTab((TabControl)currentElementTree.Parent.Parent, prop, true);
                }
                else
                {
                    editElement();
                }
            }
        }
        void tlv_MouseEnter(object sender, EventArgs e)
        {
            this.ActiveControl = currentElementTree;
        }
        void olv_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            editAttribute();
        }
        void olv_MouseEnter(object sender, EventArgs e)
        {
            this.ActiveControl = currentElementList;
        }
        #endregion

        private void mainTabControl_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            TabControl tabControl1 = sender as TabControl;
            for (int i = 0; i < tabControl1.TabCount; ++i)
            {
                if (tabControl1.GetTabRect(i).Contains(e.Location))
                {
                    tabControl1.TabPages[i].Dispose();
                    break;
                }
            }
        }

        protected virtual void RebuildAll(TreeListView tlv, System.Collections.IList selected, System.Collections.IEnumerable expanded, System.Collections.IList checkedObjects)
        {
            // Remember the bits of info we don't want to forget (anyone ever see Memento?)
            System.Collections.IEnumerable roots = tlv.Roots;
            BrightIdeasSoftware.TreeListView.CanExpandGetterDelegate canExpand = tlv.CanExpandGetter;
            BrightIdeasSoftware.TreeListView.ChildrenGetterDelegate childrenGetter = tlv.ChildrenGetter;

            try
            {
                tlv.BeginUpdate();

                // Give ourselves a new data structure
                //tlv.TreeModel = new BrightIdeasSoftware.TreeListView.Tree(tlv);
                //tlv.VirtualListDataSource = tlv.TreeModel;

                // Put back the bits we didn't want to forget
                tlv.CanExpandGetter = canExpand;
                tlv.ChildrenGetter = childrenGetter;
                if (expanded != null)
                    tlv.ExpandedObjects = expanded;
                tlv.Roots = roots;
                if (selected != null)
                    tlv.SelectedObjects = selected;
                if (checkedObjects != null)
                    tlv.CheckedObjects = checkedObjects;
            }
            finally
            {
                tlv.EndUpdate();
            }
        }
    }
}
