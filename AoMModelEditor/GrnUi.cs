namespace AoMModelEditor
{
    using AoMEngineLibrary.Graphics;
    using AoMEngineLibrary.Graphics.Grn;
    using AoMEngineLibrary.Graphics.Model;
    using BrightIdeasSoftware;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Forms;

    public class GrnUi : IModelUi
    {
        public GrnFile File { get; set; }
        public MainForm Plugin { get; set; }
        public string FileName { get; set; }
        public int FilterIndex { get { return 2; } }

        private GrnExportSetting ExportSetting { get; set; }

        public GrnUi(MainForm plugin)
        {
            this.File = new GrnFile();
            this.FileName = "Untitled";
            this.Plugin = plugin;
            this.ExportSetting = GrnExportSetting.Model;
        }

        #region Setup
        public void Read(FileStream stream)
        {
            this.File = new GrnFile();
            this.File.Read(stream);
            this.FileName = stream.Name;
        }
        public void Write(FileStream stream)
        {
            this.File.Write(stream);
            this.FileName = stream.Name;
        }
        public void Clear()
        {
            this.File = new GrnFile();
            this.FileName = Path.GetDirectoryName(this.FileName) + "\\Untitled";
        }
        #endregion

        #region Import/Export
        public void Import()
        {

        }

        public void Export()
        {
            this.Clear();

        }
        #endregion

        #region UI
        public void LoadUi()
        {
            this.Plugin.Text = MainForm.PluginTitle + " - " + Path.GetFileName(this.FileName);

            this.Plugin.grnObjectsTreeListView.ClearObjects();
            if (this.File.Bones.Count > 0)
            {
                this.Plugin.grnObjectsTreeListView.AddObject(this.File.Bones[0]);
            }
            this.Plugin.grnObjectsTreeListView.AddObjects(this.File.Meshes);
            this.Plugin.grnObjectsTreeListView.AddObjects(this.File.Materials);

            int totalVerts = 0;
            int totalFaces = 0;
            for (int i = 0; i < this.File.Meshes.Count; ++i)
            {
                totalVerts += this.File.Meshes[i].Vertices.Count;
                totalFaces += this.File.Meshes[i].Faces.Count;
            }
            this.Plugin.vertsValueToolStripStatusLabel.Text = totalVerts.ToString();
            this.Plugin.facesValueToolStripStatusLabel.Text = totalFaces.ToString();
            this.Plugin.meshesValueToolStripStatusLabel.Text = this.File.Meshes.Count.ToString();
            this.Plugin.matsValueToolStripStatusLabel.Text = this.File.Materials.Count.ToString();
            this.Plugin.animLengthValueToolStripStatusLabel.Text = this.File.Animation.Duration.ToString();

            this.Plugin.grnExportModelCheckBox.Checked = this.ExportSetting.HasFlag(GrnExportSetting.Model);
            this.Plugin.grnExportAnimCheckBox.Checked = this.ExportSetting.HasFlag(GrnExportSetting.Animation);
        }
        public void LoadBoneUI()
        {
            this.Plugin.grnObjectListView.Columns.Clear();
            OLVColumn nameCol = new OLVColumn("Name", "Name");
            nameCol.Width = 100;
            nameCol.IsEditable = false;
            this.Plugin.grnObjectListView.Columns.Add(nameCol);

            OLVColumn posCol = new OLVColumn("Positon", "Position");
            posCol.Width = 250;
            posCol.IsEditable = false;
            this.Plugin.grnObjectListView.Columns.Add(posCol);
        }
        public void LoadMeshUI()
        {
            this.Plugin.grnObjectListView.Columns.Clear();
            OLVColumn nameCol = new OLVColumn("Name", "Name");
            nameCol.Width = 100;
            nameCol.IsEditable = false;
            this.Plugin.grnObjectListView.Columns.Add(nameCol);

            OLVColumn vertCountCol = new OLVColumn("Vertex Count", "Vertices.Count");
            vertCountCol.Width = 75;
            vertCountCol.IsEditable = false;
            this.Plugin.grnObjectListView.Columns.Add(vertCountCol);

            OLVColumn faceCountCol = new OLVColumn("Face Count", "Faces.Count");
            faceCountCol.Width = 70;
            faceCountCol.IsEditable = false;
            this.Plugin.grnObjectListView.Columns.Add(faceCountCol);
        }
        public void LoadMaterialUI()
        {

        }

        public void SaveUi()
        {
            // Export Settings
            this.ExportSetting = (GrnExportSetting)0;
            if (this.Plugin.grnExportModelCheckBox.Checked)
            {
                this.ExportSetting |= GrnExportSetting.Model;
            }
            if (this.Plugin.grnExportAnimCheckBox.Checked)
            {
                this.ExportSetting |= GrnExportSetting.Animation;
            }
        }
        #endregion

        [Flags]
        private enum GrnExportSetting
        {
            Model = 0x1,
            Animation = 0x2
        }
    }
}
