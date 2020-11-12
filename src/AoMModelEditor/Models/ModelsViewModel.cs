using AoMEngineLibrary.Graphics;
using AoMEngineLibrary.Graphics.Brg;
using AoMEngineLibrary.Graphics.Grn;
using AoMModelEditor.Dialogs;
using AoMModelEditor.Models.Brg;
using AoMModelEditor.Models.Grn;
using AoMModelEditor.Settings;
using ReactiveUI;
using SharpGLTF.Schema2;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Windows;

namespace AoMModelEditor.Models
{
    public class ModelsViewModel : ReactiveObject
    {
        private BrgFile? _brg;
        private GrnFile? _grn;

        private readonly object _modelLock = new object();
        private readonly AppSettings _appSettings;
        private readonly FileDialogService _fileDialogService;
        private readonly TextureManager _textureManager;
        private readonly BrgSettingsViewModel _brgSettingsViewModel;

        public bool IsBrg { get; private set; }

        private ObservableCollection<IModelObject> _modelObjects;
        public IEnumerable<IModelObject> ModelObjects => _modelObjects;

        public ReactiveCommand<Unit, Unit> ExportGltfCommand { get; }
        public ReactiveCommand<Unit, Unit> ImportGltfBrgCommand { get; }
        public ReactiveCommand<Unit, Unit> ImportGltfGrnCommand { get; }
        public ReactiveCommand<Unit, Unit> ExportBrgMtrlFilesCommand { get; }

        public ModelsViewModel(AppSettings appSettings, FileDialogService fileDialogService)
        {
            _appSettings = appSettings;
            _fileDialogService = fileDialogService;
            _textureManager = new TextureManager(_appSettings.TexturesDirectory);
            _brgSettingsViewModel = new BrgSettingsViewModel();
            _modelObjects = new ObservableCollection<IModelObject>();

            ExportGltfCommand = ReactiveCommand.Create(ExportGltf);
            ImportGltfBrgCommand = ReactiveCommand.Create(ImportGltfToBrg);
            ImportGltfGrnCommand = ReactiveCommand.Create(ImportGltfToGrn);
            ExportBrgMtrlFilesCommand = ReactiveCommand.Create(ExportBrgMtrlFiles,
                this.WhenAnyValue(vm => vm.IsBrg));
        }

        public BrgFile GetBrg()
        {
            return _brg ?? throw new InvalidOperationException("No brg file loaded.");
        }

        public GrnFile GetGrn()
        {
            return _grn ?? throw new InvalidOperationException("No grn file loaded.");
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
            lock (_modelLock)
            {
                _modelObjects.Clear();
                _brg = brg;
                _grn = null;
                IsBrg = true;
                this.RaisePropertyChanged(nameof(IsBrg));

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
            lock (_modelLock)
            {
                _modelObjects.Clear();
                _brg = null;
                _grn = grn;
                IsBrg = false;
                this.RaisePropertyChanged(nameof(IsBrg));

                if (grn.Meshes.Count > 0)
                {
                    _modelObjects.Add(new GrnMeshesViewModel(grn.Meshes));
                }

                if (grn.Bones.Count > 0)
                {
                    _modelObjects.Add(new GrnBoneViewModel(grn, 0));
                }

                foreach (var mat in grn.Materials)
                {
                    _modelObjects.Add(new GrnMaterialViewModel(mat));
                }
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
            var brg = GetBrg();

            using (var fs = File.Open(filePath, FileMode.Create, FileAccess.Write, FileShare.Read))
            {
                brg.Write(fs);
            }
        }
        private void SaveGrn(string filePath)
        {
            var grn = GetGrn();

            using (var fs = File.Open(filePath, FileMode.Create, FileAccess.Write, FileShare.Read))
            {
                grn.Write(fs);
            }
        }

        private void ExportGltf()
        {
            try
            {
                var sfd = _fileDialogService.GetModelSaveFileDialog();
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
            var brg = GetBrg();

            // Assuming single-threaded brg will not be null here since IsBrg already checked
            BrgGltfConverter conv = new BrgGltfConverter();
            var gltf = conv.Convert(brg, _textureManager);
            gltf.Save(filePath);
        }
        private void ExportGrnToGltf(string filePath)
        {
            var grn = GetGrn();

            // Assuming single-threaded grn will not be null here since IsBrg already checked
            GrnGltfConverter conv = new GrnGltfConverter();
            var gltf = conv.Convert(grn);
            gltf.Save(filePath);
        }

        private void ImportGltfToBrg()
        {
            try
            {
                var ofd = _fileDialogService.GetModelOpenFileDialog();
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
                var ofd = _fileDialogService.GetModelOpenFileDialog();
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

        private void ExportBrgMtrlFiles()
        {
            try
            {
                var brg = GetBrg();

                var ofd = _fileDialogService.GetModelSaveFileDialog();
                var sfd = _fileDialogService.GetModelSaveFileDialog();
                var brgFilePath = sfd.FileName ?? ofd.FileName ?? string.Empty;
                var brgFolderPath = Path.GetDirectoryName(brgFilePath);
                var fbd = _fileDialogService.GetModelFolderBrowserDialog();

                if (!string.IsNullOrEmpty(_appSettings.MtrlFolderDialogDirectory) &&
                    Directory.Exists(_appSettings.MtrlFolderDialogDirectory))
                {
                    fbd.SelectedPath = _appSettings.MtrlFolderDialogDirectory;
                }
                else if (!string.IsNullOrEmpty(brgFolderPath) &&
                    Directory.Exists(brgFolderPath))
                {
                    fbd.SelectedPath = brgFolderPath;
                }

                if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    // Assuming single-threaded brg will not be null here since IsBrg already checked
                    var brgFileNameNoExt = Path.GetFileNameWithoutExtension(brgFilePath);
                    for (int i = 0; i < brg.Materials.Count; i++)
                    {
                        MtrlFile mtrl = new MtrlFile(brg.Materials[i]);
                        using (var fs = File.Open(Path.Combine(fbd.SelectedPath, brgFileNameNoExt + "_" + i + ".mtrl"),
                            FileMode.Create, FileAccess.Write, FileShare.Read))
                        {
                            mtrl.Write(fs);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to export brg mtrl files.{Environment.NewLine}{ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
            }
        }
    }
}
