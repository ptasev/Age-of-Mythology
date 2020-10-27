using AoMModelEditor.Settings;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AoMModelEditor.Dialogs
{
    public class FileDialogService
    {
        private readonly AppSettings _appSettings;

        private readonly OpenFileDialog _modelOpenFileDialog;
        private readonly SaveFileDialog _modelSaveFileDialog;

        public FileDialogService(AppSettings appSettings)
        {
            _appSettings = appSettings;
            _modelOpenFileDialog = CreateModelOfd();
            _modelSaveFileDialog = CreateModelSfd();
        }

        private OpenFileDialog CreateModelOfd()
        {
            var ofd = new OpenFileDialog();

            return ofd;
        }

        private SaveFileDialog CreateModelSfd()
        {
            SaveFileDialog sfd = new SaveFileDialog();

            return sfd;
        }

        public OpenFileDialog GetModelOpenFileDialog()
        {
            return _modelOpenFileDialog;
        }

        public SaveFileDialog GetModelSaveFileDialog()
        {
            return _modelSaveFileDialog;
        }
    }
}
