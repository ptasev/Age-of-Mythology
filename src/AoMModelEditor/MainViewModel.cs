using AoMModelEditor.Models;
using Microsoft.Win32;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Reactive;
using System.Text;
using System.Windows;

namespace AoMModelEditor
{
    public class MainViewModel : ReactiveObject
    {
        public ModelsViewModel ModelsViewModel { get; }

        public ReactiveCommand<Unit, Unit> OpenCommand { get; }
        public ReactiveCommand<Unit, Unit> SaveCommand { get; }

        public MainViewModel()
        {
            ModelsViewModel = new ModelsViewModel();

            OpenCommand = ReactiveCommand.Create(Open);
            SaveCommand = ReactiveCommand.Create(Save);
        }

        private void Open()
        {
            try
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Filter = "Model files (*.brg, *.grn)|*.brg;*.grn|All files (*.*)|*.*";

                var dr = ofd.ShowDialog();

                if (dr.HasValue && dr == true)
                {
                    ModelsViewModel.Load(ofd.FileName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to open file.{Environment.NewLine}{ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
            }
        }

        private void Save()
        {
            try
            {
                SaveFileDialog sfd = new SaveFileDialog();

                if (ModelsViewModel.IsBrg)
                {
                    sfd.Filter = "Brg files (*.brg)|*.brg|All files (*.*)|*.*";
                }
                else
                {
                    sfd.Filter = "Grn files (*.grn)|*.grn|All files (*.*)|*.*";
                }

                var dr = sfd.ShowDialog();

                if (dr.HasValue && dr == true)
                {
                    ModelsViewModel.Save(sfd.FileName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to save file.{Environment.NewLine}{ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
            }
        }
    }
}
