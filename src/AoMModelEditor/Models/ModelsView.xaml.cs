using AoMModelEditor.Dialogs;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AoMModelEditor.Models
{
    /// <summary>
    /// Interaction logic for ModelsView.xaml
    /// </summary>
#nullable disable
    public partial class ModelsView : ReactiveUserControl<ModelsViewModel>
    {
        public ModelsView()
        {
            InitializeComponent();

            this.WhenActivated(disposableRegistration =>
            {
                this.OneWayBind(ViewModel,
                    viewModel => viewModel.ModelObjects,
                    view => view.modelObjectTreeView.ItemsSource)
                    .DisposeWith(disposableRegistration);

                ViewModel?.ImportGltf.RegisterHandler(interaction =>
                {
                    var dialog = new GltfImportDialogBox { DataContext = interaction.Input };
                    var result = dialog.ShowDialog();
                    interaction.SetOutput(result is true);
                }).DisposeWith(disposableRegistration);
            });
        }
    }
}
