using AoMModelEditor.Models;
using Microsoft.Win32;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Reactive;
using System.Text;

namespace AoMModelEditor
{
    public class MainViewModel : ReactiveObject
    {
        public ModelsViewModel ModelsViewModel { get; }

        public ReactiveCommand<Unit, Unit> OpenCommand { get; }

        public MainViewModel()
        {
            ModelsViewModel = new ModelsViewModel();

            OpenCommand = ReactiveCommand.Create(Open);
        }

        private void Open()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Model files (*.brg, *.grn)|*.brg;*.grn|All files (*.*)|*.*";

            var dr = ofd.ShowDialog();

            if (dr.HasValue && dr == true)
            {
                ModelsViewModel.LoadBrg(ofd.FileName);
            }
        }
    }
}
