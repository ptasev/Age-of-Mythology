using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using ManagedServices;
using MaxCustomControls;
using MiscUtil;

namespace AoMEngineLibrary
{
    public partial class MaxPlugin : MaxUserControl
    {
        public float TimeMult
        {
            get
            {
                return Single.Parse(timeMultMaxTextBox.Text);
            }
        }
        public Int16 Unknown091
        {
            get
            {
                return Int16.Parse(u091MaxTextBox.Text);
            }
        }
        public Int32 LastMaterialIndex
        {
            get
            {
                return Int32.Parse(liuMaxTextBox.Text);
            }
        }
        public BrgMeshFlag Flags
        {
            get
            {
                return getCheckedListBoxSelectedEnums<BrgMeshFlag>(genMeshFlagsCheckedListBox);
            }
        }
        public BrgMeshFormat Format
        {
            get
            {
                return getCheckedListBoxSelectedEnums<BrgMeshFormat>(genMeshFormatCheckedListBox);
            }
        }
        public BrgMeshProperty Properties
        {
            get
            {
                return getCheckedListBoxSelectedEnums<BrgMeshProperty>(genMeshPropsCheckedListBox);
            }
        }

        public static CuiUpdater uiUp;
        BrgFile file;

        public MaxPlugin()
        {
            InitializeComponent();
            openFileDialog.Filter = "brg files|*.brg";
            saveFileDialog.Filter = "brg files|*.brg";
            saveFileDialog.AddExtension = true;

            // Update Colors
            uiUp = CuiUpdater.GetInstance();
            mainMenuStrip.Renderer = new ToolStripMaxPluginRenderer();

            for (int i = 0; i < mainTabControl.TabPages.Count; i++)
            {
                mainTabControl.TabPages[i].BackColor = uiUp.GetControlColor();
                mainTabControl.TabPages[i].ForeColor = uiUp.GetTextColor();
            }

            // General Tab
            generalDataGroupBox.ForeColor = uiUp.GetTextColor();
            attachpointGroupBox.ForeColor = uiUp.GetTextColor();
            genMeshFlagsGroupBox.ForeColor = uiUp.GetTextColor();
            genMeshFormatGroupBox.ForeColor = uiUp.GetTextColor();
            genMeshPropsGroupBox.ForeColor = uiUp.GetTextColor();

            numMeshMaxTextBox.BackColor = uiUp.GetEditControlColor();
            numMeshMaxTextBox.ForeColor = uiUp.GetTextColor();
            numMeshMaxTextBox.BorderStyle = BorderStyle.FixedSingle;
            numMatMaxTextBox.BackColor = uiUp.GetEditControlColor();
            numMatMaxTextBox.ForeColor = uiUp.GetTextColor();
            numMatMaxTextBox.BorderStyle = BorderStyle.FixedSingle;
            animTimeMaxTextBox.BackColor = uiUp.GetEditControlColor();
            animTimeMaxTextBox.ForeColor = uiUp.GetTextColor();
            animTimeMaxTextBox.BorderStyle = BorderStyle.FixedSingle;
            timeMultMaxTextBox.BackColor = uiUp.GetEditControlColor();
            timeMultMaxTextBox.ForeColor = uiUp.GetTextColor();
            timeMultMaxTextBox.BorderStyle = BorderStyle.FixedSingle;
            u091MaxTextBox.BackColor = uiUp.GetEditControlColor();
            u091MaxTextBox.ForeColor = uiUp.GetTextColor();
            u091MaxTextBox.BorderStyle = BorderStyle.FixedSingle;
            liuMaxTextBox.BackColor = uiUp.GetEditControlColor();
            liuMaxTextBox.ForeColor = uiUp.GetTextColor();
            liuMaxTextBox.BorderStyle = BorderStyle.FixedSingle;

            genMeshFlagsCheckedListBox.BackColor = uiUp.GetEditControlColor();
            genMeshFlagsCheckedListBox.ForeColor = uiUp.GetTextColor();
            genMeshFlagsCheckedListBox.BorderStyle = BorderStyle.None;
            genMeshFlagsCheckedListBox.DataSource = Enum.GetValues(typeof(BrgMeshFlag));
            genMeshFormatCheckedListBox.BackColor = uiUp.GetEditControlColor();
            genMeshFormatCheckedListBox.ForeColor = uiUp.GetTextColor();
            genMeshFormatCheckedListBox.BorderStyle = BorderStyle.None;
            genMeshFormatCheckedListBox.DataSource = Enum.GetValues(typeof(BrgMeshFormat));
            genMeshPropsCheckedListBox.BackColor = uiUp.GetEditControlColor();
            genMeshPropsCheckedListBox.ForeColor = uiUp.GetTextColor();
            genMeshPropsCheckedListBox.BorderStyle = BorderStyle.None;
            genMeshPropsCheckedListBox.DataSource = Enum.GetValues(typeof(BrgMeshProperty));

            // Attachpoints
            attachpointComboBox.SelectedValueChanged += attachpointComboBox_SelectedValueChanged;
            attachpointComboBox.FlatStyle = FlatStyle.Flat;
            attachpointComboBox.BackColor = uiUp.GetEditControlColor();
            attachpointComboBox.ForeColor = uiUp.GetTextColor();
            attachpointListBox.MouseDoubleClick += attachpointListBox_MouseDoubleClick;
            attachpointListBox.BackColor = uiUp.GetEditControlColor();
            attachpointListBox.ForeColor = uiUp.GetTextColor();
            attachpointListBox.DataSource = BrgAttachpoint.AttachpointNames;

            // Materials
            materialGroupBox.BackColor = uiUp.GetControlColor();
            materialGroupBox.ForeColor = uiUp.GetTextColor();
            diffuseMaxTextBox.BackColor = uiUp.GetEditControlColor();
            diffuseMaxTextBox.ForeColor = uiUp.GetTextColor();
            ambientMaxTextBox.BackColor = uiUp.GetEditControlColor();
            ambientMaxTextBox.ForeColor = uiUp.GetTextColor();
            specularMaxTextBox.BackColor = uiUp.GetEditControlColor();
            specularMaxTextBox.ForeColor = uiUp.GetTextColor();
            selfIllumMaxTextBox.BackColor = uiUp.GetEditControlColor();
            selfIllumMaxTextBox.ForeColor = uiUp.GetTextColor();
            textureMaxTextBox.BackColor = uiUp.GetEditControlColor();
            textureMaxTextBox.ForeColor = uiUp.GetTextColor();
            reflectionMaxTextBox.BackColor = uiUp.GetEditControlColor();
            reflectionMaxTextBox.ForeColor = uiUp.GetTextColor();
            unknownMaxTextBox.BackColor = uiUp.GetEditControlColor();
            unknownMaxTextBox.ForeColor = uiUp.GetTextColor();
            materialFlagsGroupBox.BackColor = uiUp.GetControlColor();
            materialFlagsGroupBox.ForeColor = uiUp.GetTextColor();
            materialListBox.SelectedIndexChanged += materialListBox_SelectedIndexChanged;
            materialListBox.BackColor = uiUp.GetEditControlColor();
            materialListBox.ForeColor = uiUp.GetTextColor();
            materialFlagsCheckedListBox.BackColor = uiUp.GetEditControlColor();
            materialFlagsCheckedListBox.ForeColor = uiUp.GetTextColor();
            materialFlagsCheckedListBox.BorderStyle = BorderStyle.FixedSingle;
            materialFlagsCheckedListBox.DataSource = Enum.GetValues(typeof(BrgMatFlag));
        }

        private Color ContrastColor(Color color)
        {
            int d = 0;

            // Counting the perceptive luminance - human eye favors green color... 
            double a = 1 - (0.299 * color.R + 0.587 * color.G + 0.114 * color.B) / 255;

            if (a < 0.5)
                d = 0; // bright colors - black font
            else
                d = 255; // dark colors - white font

            return Color.FromArgb(d, d, d);
        }

        void materialListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (materialListBox.SelectedIndex >= 0)
            {
                BrgMaterial mat = file.Material[materialListBox.SelectedIndex];
                // Update Info
                diffuseMaxTextBox.BackColor = mat.DiffuseColor;
                diffuseMaxTextBox.ForeColor = ContrastColor(diffuseMaxTextBox.BackColor);
                ambientMaxTextBox.BackColor = mat.AmbientColor;
                ambientMaxTextBox.ForeColor = ContrastColor(ambientMaxTextBox.BackColor);
                specularMaxTextBox.BackColor = mat.SpecularColor;
                specularMaxTextBox.ForeColor = ContrastColor(specularMaxTextBox.BackColor);
                selfIllumMaxTextBox.BackColor = mat.SelfIllumColor;
                selfIllumMaxTextBox.ForeColor = ContrastColor(selfIllumMaxTextBox.BackColor);
                specularLevelMaxTextBox.Text = mat.specularLevel.ToString();
                opacityMaxTextBox.Text = mat.alphaOpacity.ToString();
                textureMaxTextBox.Text = mat.name;
                if (mat.sfx.Count > 0)
                {
                    reflectionMaxTextBox.Text = mat.sfx[0].Name;
                }
                else { reflectionMaxTextBox.Text = string.Empty; }
                unknownMaxTextBox.Text = mat.name2;

                // Update Flags box
                for (int i = 0; i < materialFlagsCheckedListBox.Items.Count; i++)
                {
                    if (mat.flags.HasFlag((BrgMatFlag)materialFlagsCheckedListBox.Items[i]))
                    {
                        materialFlagsCheckedListBox.SetItemChecked(i, true);
                    }
                    else
                    {
                        materialFlagsCheckedListBox.SetItemChecked(i, false);
                    }
                }
            }
        }

        private class ToolStripMaxPluginRenderer : ToolStripProfessionalRenderer
        {
            protected override void OnRenderImageMargin(ToolStripRenderEventArgs e)
            {
                e.ToolStrip.BackColor = uiUp.GetControlColor();
                //base.OnRenderImageMargin(e);
            }

            protected override void OnRenderToolStripPanelBackground(ToolStripPanelRenderEventArgs e)
            {
                e.ToolStripPanel.BackColor = uiUp.GetControlColor();
                //base.OnRenderToolStripPanelBackground(e);
            }
            protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
            {
                e.ToolStrip.BackColor = uiUp.GetControlColor();
                e.ToolStrip.ForeColor = uiUp.GetTextColor();
                //base.OnRenderToolStripBackground(e);
                //Rectangle rc = new Rectangle(Point.Empty, e.ToolStrip.Size);
                //e.Graphics.FillRectangle(new SolidBrush(uiUp.GetControlColor()), rc);
                //e.Graphics.DrawRectangle(Pens.Blue, 1, 0, rc.Width - 4, rc.Height - 1);
            }
            protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
            {
                if (e.ToolStrip.GetType() == typeof(MenuStrip))
                {
                    // skip render border of main
                }
                else
                {
                    // do render border
                    //base.OnRenderToolStripBorder(e);
                    Rectangle rc = new Rectangle(Point.Empty, e.ToolStrip.Size);
                    //e.Graphics.FillRectangle(new SolidBrush(uiUp.GetEditControlColor()), rc);
                    e.Graphics.DrawRectangle(Pens.Black, 0, 0, rc.Width - 1, rc.Height - 1);
                }
            }

            protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
            {
                if (!e.Item.Selected)
                {
                    //for (int i = 0; i < e.Item.)
                    //base.OnRenderMenuItemBackground(e);
                    //Rectangle rc = new Rectangle(Point.Empty, e.Item.Size);
                    //e.Graphics.FillRectangle(new SolidBrush(uiUp.GetControlColor()), rc);
                    //e.Graphics.DrawRectangle(Pens.Black, 1, 0, rc.Width - 4, rc.Height - 1);
                }
                else
                {
                    // Highlight selected item
                    Rectangle rc = new Rectangle(Point.Empty, e.Item.Size);
                    e.Graphics.FillRectangle(new SolidBrush(uiUp.GetEditControlColor()), 3, 1, rc.Width - 6, rc.Height - 3);
                    e.Graphics.DrawRectangle(Pens.Black, 3, 1, rc.Width - 6, rc.Height - 3);
                }
                //base.OnRenderMenuItemBackground(e);
            }
        }

        #region MainMenu
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                file = new BrgFile(File.Open(openFileDialog.FileName, FileMode.Open, FileAccess.Read, FileShare.Read), this);
                file.ExportToMax();
                //debug();

                loadUI();
            }
        }
        private void debug()
        {
            using (TextWriter writer = File.CreateText("C:\\temppp.txt"))
            {
                for (int i = 0; i < Maxscript.Output.Count; i++)
                {
                    writer.Write(Maxscript.Output[i]);
                    writer.Write(writer.NewLine);
                }
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (file == null)
            {
                return;
            }
            //saveFileDialog.FileName = "archerTest.brg";
            saveFileDialog.FileName = Path.GetFileNameWithoutExtension(openFileDialog.FileName);
            saveFileDialog.InitialDirectory = Path.GetDirectoryName(openFileDialog.FileName);

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                //file.ImportFromMax(true);
                //loadUI();
                file.Write(File.Open(saveFileDialog.FileName, FileMode.Create, FileAccess.Write, FileShare.Read));
            }
        }

        private void exportToMaxToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void importFromMaxToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (file == null)
            {
                return;
            }
            file.ImportFromMax(true);
            loadUI();
        }
        #endregion

        void attachpointListBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int index = attachpointListBox.IndexFromPoint(e.Location);
            if (index != System.Windows.Forms.ListBox.NoMatches && file != null)
            {
                if (file.Mesh[0].attachpoints == null)
                {
                    file.Mesh[0].attachpoints = new List<BrgAttachpoint>();
                }
                BrgAttachpoint att = new BrgAttachpoint();
                att.NameId = BrgAttachpoint.GetIdByName((string)attachpointListBox.Items[index]);
                file.Mesh[0].attachpoints.Add(att);
                //MessageBox.Show(file.Mesh[0].attachpoints.Count.ToString());
                Maxscript.NewDummy("newDummy", att.GetMaxName(file.Mesh[0].attachpoints.Count - 1), att.GetMaxTransform(), att.GetMaxPosition(), att.GetMaxBoxSize(), att.GetMaxScale());
                loadUIAttachpoint();
                attachpointComboBox.SelectedIndex = attachpointComboBox.Items.Count - 1;
            }
        }
        void attachpointComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            if (attachpointComboBox.SelectedValue != null)
            {
                Maxscript.Command("selectDummy = getNodeByName \"{0}\"", ((BrgAttachpoint)attachpointComboBox.SelectedValue).GetMaxName(attachpointComboBox.SelectedIndex));
                if (Maxscript.QueryBoolean("selectDummy != undefined"))
                {
                    Maxscript.Command("select selectDummy");
                }
                else
                {
                    file.Mesh[0].attachpoints.RemoveAt(attachpointComboBox.SelectedIndex);
                    loadUIAttachpoint();
                }
            }
        }

        private void loadUIMaterial()
        {
            materialListBox.DataSource = null;
            materialListBox.DataSource = file.Material;
            materialListBox.DisplayMember = "EditorName";

        }
        private void loadUIAttachpoint()
        {
            attachpointComboBox.DataSource = null;
            attachpointComboBox.DataSource = file.Mesh[0].attachpoints;
            attachpointComboBox.DisplayMember = "Name";
        }
        private void loadUI()
        {
            // Materials
            loadUIMaterial();

            // General Info
            numMeshMaxTextBox.Text = file.Mesh.Count.ToString();
            numMatMaxTextBox.Text = file.Material.Count.ToString();
            animTimeMaxTextBox.Text = "0";
            timeMultMaxTextBox.Text = "1";
            u091MaxTextBox.Text = "0";
            liuMaxTextBox.Text = "0";
            if (file.Mesh.Count > 1)
            {
                animTimeMaxTextBox.Text = file.Mesh[0].animTime.ToString();
                timeMultMaxTextBox.Text = file.Mesh[0].animTimeMult.ToString();
                u091MaxTextBox.Text = file.Mesh[0].unknown091.ToString();
                liuMaxTextBox.Text = file.Mesh[0].lastMaterialIndex.ToString();
            }

            // Attachpoints
            loadUIAttachpoint();

            if (file.Mesh.Count > 0)
            {
                for (int i = 0; i < genMeshFlagsCheckedListBox.Items.Count; i++)
                {
                    if (file.Mesh[0].flags.HasFlag((BrgMeshFlag)genMeshFlagsCheckedListBox.Items[i]))
                    {
                        genMeshFlagsCheckedListBox.SetItemChecked(i, true);
                    }
                    else
                    {
                        genMeshFlagsCheckedListBox.SetItemChecked(i, false);
                    }
                }
                for (int i = 0; i < genMeshFormatCheckedListBox.Items.Count; i++)
                {
                    if (file.Mesh[0].format.HasFlag((BrgMeshFormat)genMeshFormatCheckedListBox.Items[i]))
                    {
                        genMeshFormatCheckedListBox.SetItemChecked(i, true);
                    }
                    else
                    {
                        genMeshFormatCheckedListBox.SetItemChecked(i, false);
                    }
                }
                for (int i = 0; i < genMeshPropsCheckedListBox.Items.Count; i++)
                {
                    if (file.Mesh[0].properties.HasFlag((BrgMeshProperty)genMeshPropsCheckedListBox.Items[i]))
                    {
                        genMeshPropsCheckedListBox.SetItemChecked(i, true);
                    }
                    else
                    {
                        genMeshPropsCheckedListBox.SetItemChecked(i, false);
                    }
                }
            }
        }
        private int getCheckedListBoxSelectedEnums(CheckedListBox box)
        {
            int enumVal = 0;
            for (int i = 0; i < box.CheckedItems.Count; i++)
            {
                enumVal |= (int)box.CheckedItems[i];
            }
            return enumVal;
        }
        private T getCheckedListBoxSelectedEnums<T>(CheckedListBox box)
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException("T must be an enumerated type");
            }

            T enumVal = Operator.Convert<int, T>(0);
            for (int i = 0; i < box.CheckedItems.Count; i++)
            {
                enumVal = Operator.Convert<int, T>(Operator.Or<int>(Operator.Convert<T, int>(enumVal), Operator.Convert<T, int>(Operator.Convert<object, T>(box.CheckedItems[i]))));
            }

            return enumVal;
        }
    }
}
