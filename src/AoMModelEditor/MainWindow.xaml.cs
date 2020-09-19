using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AoMModelEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    #nullable disable
    public partial class MainWindow : ReactiveWindow<MainViewModel>
    {
        public MainWindow()
        {
            InitializeComponent();
            ViewModel = new MainViewModel();

            this.WhenActivated(disposableRegistration =>
            {
                this.OneWayBind(ViewModel,
                    v => v.Title,
                    view => view.Title)
                    .DisposeWith(disposableRegistration);

                this.OneWayBind(ViewModel,
                    v => v.ModelsViewModel,
                    view => view.modelsView.ViewModel)
                    .DisposeWith(disposableRegistration);

                this.BindCommand(ViewModel,
                    v => v.OpenCommand,
                    view => view.openMenuItem)
                    .DisposeWith(disposableRegistration);

                this.BindCommand(ViewModel,
                    v => v.SaveCommand,
                    view => view.saveMenuItem)
                    .DisposeWith(disposableRegistration);

                this.BindCommand(ViewModel,
                    v => v.ModelsViewModel.ExportGltfCommand,
                    view => view.exportGltfMenuItem)
                    .DisposeWith(disposableRegistration);

                this.BindCommand(ViewModel,
                    v => v.ModelsViewModel.ImportGltfBrgCommand,
                    view => view.importGltfBrgMenuItem)
                    .DisposeWith(disposableRegistration);

                this.BindCommand(ViewModel,
                    v => v.ModelsViewModel.ImportGltfGrnCommand,
                    view => view.importGltfGrnMenuItem)
                    .DisposeWith(disposableRegistration);
            });
        }
    }
}
