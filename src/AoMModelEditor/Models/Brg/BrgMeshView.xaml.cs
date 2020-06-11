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

namespace AoMModelEditor.Models.Brg
{
    /// <summary>
    /// Interaction logic for BrgMeshView.xaml
    /// </summary>
    public partial class BrgMeshView : ReactiveUserControl<BrgMeshViewModel>
    {
        public BrgMeshView()
        {
            InitializeComponent();

            this.WhenActivated(disposableRegistration =>
            {
                this.OneWayBind(ViewModel,
                    v => v.Name,
                    view => view.mainGroupBox.Header)
                    .DisposeWith(disposableRegistration);
            });
        }
    }
}
