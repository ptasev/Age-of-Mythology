using AoMEngineLibrary.Graphics.Brg;
using AoMModelEditor.Models.Brg;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Windows.Forms.Design;

namespace AoMModelEditor.Models
{
    public class ModelsViewModel : ReactiveObject
    {
        private BrgFile file;

        private ObservableCollection<IModelObject> _modelObjects;
        public IEnumerable<IModelObject> ModelObjects => _modelObjects;

        public ModelsViewModel()
        {
            _modelObjects = new ObservableCollection<IModelObject>();
        }

        public void LoadBrg(string filePath)
        {
            using (var fs = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                file = new BrgFile(fs);
            }

            _modelObjects.Clear();
            if (file.Meshes.Count > 0)
            {
                _modelObjects.Add(new BrgMeshViewModel(file.Meshes[0]));
            }

            foreach (var mat in file.Materials)
            {
                _modelObjects.Add(new BrgMaterialViewModel(mat));
            }

            if (file.Meshes.Count > 0)
            {
                foreach (var dummy in file.Meshes[0].Attachpoints)
                {
                    _modelObjects.Add(new BrgDummyViewModel(dummy));
                }
            }

            this.RaisePropertyChanging(nameof(ModelObjects));
            this.RaisePropertyChanged(nameof(ModelObjects));
        }
    }
}
