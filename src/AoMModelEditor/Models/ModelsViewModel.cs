using AoMEngineLibrary.Graphics.Brg;
using AoMEngineLibrary.Graphics.Grn;
using AoMModelEditor.Models.Brg;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Forms.Design;

namespace AoMModelEditor.Models
{
    public class ModelsViewModel : ReactiveObject
    {
        private BrgFile? brg;
        private GrnFile? grn;

        public bool IsBrg { get; private set; }

        private ObservableCollection<IModelObject> _modelObjects;
        public IEnumerable<IModelObject> ModelObjects => _modelObjects;

        public ModelsViewModel()
        {
            _modelObjects = new ObservableCollection<IModelObject>();
        }

        public void Load(string filePath)
        {
            if (Path.GetExtension(filePath).Equals(".grn", StringComparison.InvariantCultureIgnoreCase))
            {
                LoadGrn(filePath);
            }
            else
            {
                LoadBrg(filePath);
            }
        }
        private void LoadBrg(string filePath)
        {
            using (var fs = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                brg = new BrgFile(fs);
            }

            _modelObjects.Clear();
            IsBrg = true;
            grn = null;

            if (brg.Meshes.Count > 0)
            {
                _modelObjects.Add(new BrgMeshViewModel(brg, brg.Meshes[0]));
            }

            foreach (var mat in brg.Materials)
            {
                _modelObjects.Add(new BrgMaterialViewModel(mat));
            }

            if (brg.Meshes.Count > 0)
            {
                for (int i = 0; i < brg.Meshes[0].Attachpoints.Count; ++i)
                {
                    _modelObjects.Add(new BrgDummyViewModel(brg.Meshes.Select(m => m.Attachpoints[i]).ToList()));
                }
            }
        }
        private void LoadGrn(string filePath)
        {
            using (var fs = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                grn = new GrnFile();
                grn.Read(fs);
            }

            _modelObjects.Clear();
            IsBrg = false;
            brg = null;

            if (grn.Meshes.Count > 0)
            {
                //_modelObjects.Add(new BrgMeshViewModel(file, file.Meshes[0]));
            }

            foreach (var mat in grn.Materials)
            {
                //_modelObjects.Add(new BrgMaterialViewModel(mat));
            }

            if (grn.Meshes.Count > 0)
            {

            }

            this.RaisePropertyChanging(nameof(ModelObjects));
            this.RaisePropertyChanged(nameof(ModelObjects));
        }

        public void Save(string filePath)
        {
            if (IsBrg)
            {
                SaveBrg(filePath);
            }
            else
            {
                SaveGrn(filePath);
            }
        }
        private void SaveBrg(string filePath)
        {
            if (brg == null)
                throw new InvalidOperationException("No file loaded.");

            using (var fs = File.Open(filePath, FileMode.Create, FileAccess.Write, FileShare.Read))
            {
                brg.Write(fs);
            }
        }
        private void SaveGrn(string filePath)
        {
            if (grn == null)
                throw new InvalidOperationException("No file loaded.");

            using (var fs = File.Open(filePath, FileMode.Create, FileAccess.Write, FileShare.Read))
            {
                grn.Write(fs);
            }
        }
    }
}
