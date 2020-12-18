using ReactiveUI;
using Splat;
using System;
using System.Diagnostics;
using System.Reactive.Disposables;

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
            ViewModel = Locator.Current.GetService<MainViewModel>();

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

                this.BindCommand(ViewModel,
                    v => v.ModelsViewModel.ExportBrgMtrlFilesCommand,
                    view => view.exportBrgMtrlFilesMenuItem)
                    .DisposeWith(disposableRegistration);

                this.BindCommand(ViewModel,
                    v => v.ModelsViewModel.ApplyGrnAnimationCommand,
                    view => view.applyGrnAnimationMenuItem)
                    .DisposeWith(disposableRegistration);
            });
        }

        private void websiteMenuItem_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo("https://petar.page/l/aom-ame-home") { UseShellExecute = true });
        }

        private void sourceCodeMenuItem_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo("https://petar.page/l/aom-ame-code") { UseShellExecute = true });
        }

        private void brgFlagsDocMenuItem_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo("https://docs.google.com/spreadsheets/d/1m6AZHjt3YU4fxH_-w9Smi1WEBa651PXgavQrC_a1_1o/edit?usp=sharing") { UseShellExecute = true });
        }
    }
}
