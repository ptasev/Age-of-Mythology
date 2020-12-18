using Microsoft.Win32;
using System;
using System.IO;
using WinForms = System.Windows.Forms;

namespace AoMModelEditor.Services
{
    public class FileDialogService
    {
        private readonly OpenFileDialog _modelOpenFileDialog;
        private readonly SaveFileDialog _modelSaveFileDialog;
        private readonly WinForms.FolderBrowserDialog _modelFolderBrowserDialog;

        private string _lastModelFilePath;

        public FileDialogService()
        {
            _modelOpenFileDialog = new OpenFileDialog();
            _modelSaveFileDialog = new SaveFileDialog();
            _modelFolderBrowserDialog = new WinForms.FolderBrowserDialog();

            _lastModelFilePath = string.Empty;
        }

        public OpenFileDialog GetModelOpenFileDialog()
        {
            if (!string.IsNullOrEmpty(_lastModelFilePath))
            {
                _modelOpenFileDialog.FileName = Path.GetFileNameWithoutExtension(_lastModelFilePath);
                var lastDir = Path.GetDirectoryName(_lastModelFilePath);
                if (Directory.Exists(lastDir))
                    _modelOpenFileDialog.InitialDirectory = lastDir;
            }

            return _modelOpenFileDialog;
        }

        public SaveFileDialog GetModelSaveFileDialog()
        {
            if (!string.IsNullOrEmpty(_lastModelFilePath))
            {
                _modelSaveFileDialog.FileName = Path.GetFileNameWithoutExtension(_lastModelFilePath);
                var lastDir = Path.GetDirectoryName(_lastModelFilePath);
                if (Directory.Exists(lastDir))
                    _modelSaveFileDialog.InitialDirectory = lastDir;
            }

            return _modelSaveFileDialog;
        }

        public void SetLastModelFilePath(string filePath)
        {
            _lastModelFilePath = filePath;
        }

        public WinForms.FolderBrowserDialog GetModelFolderBrowserDialog() => _modelFolderBrowserDialog;
    }
}
