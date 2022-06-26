using ReactiveUI;
using System.Reactive.Disposables;

namespace AoMModelEditor.Models.Grn
{
    /// <summary>
    /// Interaction logic for GrnSettingsView.xaml
    /// </summary>
#nullable disable
    public partial class GrnGltfImportSettingsView : ReactiveUserControl<GrnGltfImportSettingsViewModel>
    {
        public GrnGltfImportSettingsView()
        {
            InitializeComponent();

            this.WhenActivated(disposableRegistration =>
            {
                this.OneWayBind(ViewModel,
                    v => v.Name,
                    view => view.mainGroupBox.Header)
                    .DisposeWith(disposableRegistration);

                this.Bind(ViewModel,
                    v => v.ConvertMeshes,
                    view => view.convertMeshesCheckBox.IsChecked)
                    .DisposeWith(disposableRegistration);

                this.Bind(ViewModel,
                    v => v.ConvertAnimation,
                    view => view.convertAnimationCheckBox.IsChecked)
                    .DisposeWith(disposableRegistration);
            });
        }
    }
}
