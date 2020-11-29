using AoMModelEditor.Dialogs;
using AoMModelEditor.Models;
using AoMModelEditor.Settings;
using Microsoft.Extensions.Logging;
using ReactiveUI;
using System;
using System.IO;
using System.Reactive;
using System.Windows;

namespace AoMModelEditor
{
    /* 1.0 TODOs
     * 
     */

    public class MainViewModel : ReactiveObject
    {
        private readonly AppSettings _appSettings;
        private readonly FileDialogService _fileDialogService;

        private string _title;
        public string Title 
        {
            get => _title;
            set
            {
                _title = value;
                this.RaisePropertyChanged(nameof(Title));
            }
        }

        public ModelsViewModel ModelsViewModel { get; }

        public ReactiveCommand<Unit, Unit> OpenCommand { get; }
        public ReactiveCommand<Unit, Unit> SaveCommand { get; }

        public MainViewModel(AppSettings appSettings, FileDialogService fileDialogService, ModelsViewModel modelsVM, ILogger<MainViewModel> logger)
        {
            _title = Properties.Resources.AppTitleLong;
            _appSettings = appSettings;
            _appSettings.Read();

            _fileDialogService = fileDialogService;

            ModelsViewModel = modelsVM;

            OpenCommand = ReactiveCommand.Create(Open);
            SaveCommand = ReactiveCommand.Create(Save,
                ModelsViewModel.WhenAnyValue(vm => vm.IsBrg, vm => vm.IsGrn, (b, g) => b || g));
        }

        private void Open()
        {
            try
            {
                var ofd = _fileDialogService.GetModelOpenFileDialog();
                ofd.Filter = "Model files (*.brg, *.grn)|*.brg;*.grn|All files (*.*)|*.*";

                if (!string.IsNullOrEmpty(_appSettings.OpenFileDialogFileName) &&
                    Directory.Exists(_appSettings.OpenFileDialogFileName))
                {
                    ofd.InitialDirectory = _appSettings.OpenFileDialogFileName;
                }

                var dr = ofd.ShowDialog();
                if (dr.HasValue && dr == true)
                {
                    _fileDialogService.SetLastModelFilePath(ofd.FileName);
                    ModelsViewModel.Load(ofd.FileName);
                    Title = Properties.Resources.AppTitleShort + " - " + Path.GetFileName(ofd.FileName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to open file.{Environment.NewLine}{ex.Message}", Properties.Resources.AppTitleLong,
                    MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
            }
        }

        private void Save()
        {
            try
            {
                var sfd = _fileDialogService.GetModelSaveFileDialog();

                if (ModelsViewModel.IsBrg)
                {
                    sfd.Filter = "Brg files (*.brg)|*.brg|All files (*.*)|*.*";
                }
                else
                {
                    sfd.Filter = "Grn files (*.grn)|*.grn|All files (*.*)|*.*";
                }

                // Setup starting directory and file name
                if (!string.IsNullOrEmpty(_appSettings.SaveFileDialogFileName) &&
                    Directory.Exists(_appSettings.SaveFileDialogFileName))
                {
                    sfd.InitialDirectory = _appSettings.SaveFileDialogFileName;
                }

                var dr = sfd.ShowDialog();
                if (dr.HasValue && dr == true)
                {
                    _fileDialogService.SetLastModelFilePath(sfd.FileName);
                    ModelsViewModel.Save(sfd.FileName);
                    Title = Properties.Resources.AppTitleShort + " - " + Path.GetFileName(sfd.FileName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to save file.{Environment.NewLine}{ex.Message}", Properties.Resources.AppTitleLong,
                    MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
            }
        }
    }
}
