using AoMEngineLibrary.Graphics.Brg;
using AoMEngineLibrary.Graphics.Grn;
using AoMModelEditor.Models.Brg;
using Microsoft.Win32;
using ReactiveUI;
using SharpGLTF.Schema2;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Forms.Design;

namespace AoMModelEditor.Models
{
    public class ModelsViewModel : ReactiveObject
    {
        private BrgFile? _brg;
        private GrnFile? _grn;

        private readonly BrgSettingsViewModel _brgSettingsViewModel;

        public bool IsBrg { get; private set; }

        private ObservableCollection<IModelObject> _modelObjects;
        public IEnumerable<IModelObject> ModelObjects => _modelObjects;

        public ReactiveCommand<Unit, Unit> ExportGltfCommand { get; }
        public ReactiveCommand<Unit, Unit> ImportGltfBrgCommand { get; }
        public ReactiveCommand<Unit, Unit> ImportGltfGrnCommand { get; }

        public ModelsViewModel()
        {
            _brgSettingsViewModel = new BrgSettingsViewModel();
            _modelObjects = new ObservableCollection<IModelObject>();

            ExportGltfCommand = ReactiveCommand.Create(ExportGltf);
            ImportGltfBrgCommand = ReactiveCommand.Create(ImportGltfToBrg);
            ImportGltfGrnCommand = ReactiveCommand.Create(ImportGltfToGrn);
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
                LoadBrg(new BrgFile(fs));
            }
        }
        private void LoadBrg(BrgFile brg)
        {
            _modelObjects.Clear();
            IsBrg = true;
            _brg = brg;
            _grn = null;

            _modelObjects.Add(_brgSettingsViewModel);

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
                var grn = new GrnFile();
                grn.Read(fs);
                LoadGrn(grn);
            }
        }
        private void LoadGrn(GrnFile grn)
        {
            _modelObjects.Clear();
            IsBrg = false;
            _brg = null;
            _grn = grn;

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
            if (_brg == null)
                throw new InvalidOperationException("No file loaded.");

            using (var fs = File.Open(filePath, FileMode.Create, FileAccess.Write, FileShare.Read))
            {
                _brg.Write(fs);
            }
        }
        private void SaveGrn(string filePath)
        {
            if (_grn == null)
                throw new InvalidOperationException("No file loaded.");

            using (var fs = File.Open(filePath, FileMode.Create, FileAccess.Write, FileShare.Read))
            {
                _grn.Write(fs);
            }
        }

        private void ExportGltf()
        {
            try
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "Glb files (*.glb)|*.glb|Gltf files (*.gltf)|*.gltf|All files (*.*)|*.*";

                var dr = sfd.ShowDialog();
                if (dr.HasValue && dr == true)
                {
                    if (IsBrg)
                    {
                        ExportBrgToGltf(sfd.FileName);
                    }
                    else
                    {
                        ExportGrnToGltf(sfd.FileName);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to export gltf file.{Environment.NewLine}{ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
            }
        }
        private void ExportBrgToGltf(string filePath)
        {
            if (_brg == null)
                throw new InvalidOperationException("No file loaded.");

            BrgGltfConverter conv = new BrgGltfConverter();
            var gltf = conv.Convert(_brg);
            gltf.Save(filePath);
        }
        private void ExportGrnToGltf(string filePath)
        {
            if (_grn == null)
                throw new InvalidOperationException("No file loaded.");

            GrnGltfConverter conv = new GrnGltfConverter();
            var gltf = conv.Convert(_grn);
            gltf.Save(filePath);
        }

        private void ImportGltfToBrg()
        {
            try
            {
                var ofd = new OpenFileDialog();
                ofd.Filter = "Glb files (*.glb)|*.glb|Gltf files (*.gltf)|*.gltf|All files (*.*)|*.*";

                var dr = ofd.ShowDialog();
                if (dr.HasValue && dr == true)
                {
                    var conv = new GltfBrgConverter();
                    var gltf = ModelRoot.Load(ofd.FileName, new ReadSettings() { Validation = SharpGLTF.Validation.ValidationMode.Skip });
                    LoadBrg(conv.Convert(gltf, _brgSettingsViewModel.CreateGltfBrgParameters()));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to import gltf file as brg.{Environment.NewLine}{ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
            }
        }
        private void ImportGltfToGrn()
        {
            try
            {
                var ofd = new OpenFileDialog();
                ofd.Filter = "Glb files (*.glb)|*.glb|Gltf files (*.gltf)|*.gltf|All files (*.*)|*.*";

                var dr = ofd.ShowDialog();
                if (dr.HasValue && dr == true)
                {
                    var conv = new GltfGrnConverter();
                    var gltf = ModelRoot.Load(ofd.FileName);
                    LoadGrn(conv.Convert(gltf));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to import gltf file as grn.{Environment.NewLine}{ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
            }
        }
    }
}
