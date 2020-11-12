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

namespace AoMModelEditor.Models.Grn
{
    /// <summary>
    /// Interaction logic for GrnMeshesView.xaml
    /// </summary>
#nullable disable
    public partial class GrnMeshesView : ReactiveUserControl<GrnMeshesViewModel>
    {
        public GrnMeshesView()
        {
            InitializeComponent();

            this.WhenActivated(disposableRegistration =>
            {
                this.OneWayBind(ViewModel,
                    v => v.Name,
                    view => view.mainGroupBox.Header)
                    .DisposeWith(disposableRegistration);

                this.OneWayBind(ViewModel,
                    v => v.VertexCount,
                    view => view.vertCountTextBlock.Text)
                    .DisposeWith(disposableRegistration);

                this.OneWayBind(ViewModel,
                    v => v.FaceCount,
                    view => view.faceCountTextBlock.Text)
                    .DisposeWith(disposableRegistration);

                this.OneWayBind(ViewModel,
                    v => v.Meshes,
                    view => view.meshListView.ItemsSource)
                    .DisposeWith(disposableRegistration);
            });
        }
    }
}
