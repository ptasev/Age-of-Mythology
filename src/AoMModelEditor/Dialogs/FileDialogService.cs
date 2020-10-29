using Microsoft.Win32;
using System;

using WinForms = System.Windows.Forms;

namespace AoMModelEditor.Dialogs
{
    public class FileDialogService
    {
        private readonly OpenFileDialog _modelOpenFileDialog;
        private readonly SaveFileDialog _modelSaveFileDialog;
        private readonly WinForms.FolderBrowserDialog _modelFolderBrowserDialog;

        public FileDialogService()
        {
            _modelOpenFileDialog = new OpenFileDialog();
            _modelSaveFileDialog = new SaveFileDialog();
            _modelFolderBrowserDialog = new WinForms.FolderBrowserDialog();
        }

        public OpenFileDialog GetModelOpenFileDialog()
        {
            return _modelOpenFileDialog;
        }

        public SaveFileDialog GetModelSaveFileDialog()
        {
            return _modelSaveFileDialog;
        }

        public WinForms.FolderBrowserDialog GetModelFolderBrowserDialog() => _modelFolderBrowserDialog;
    }
}
