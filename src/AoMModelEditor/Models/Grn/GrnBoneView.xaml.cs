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
    /// Interaction logic for GrnBoneView.xaml
    /// </summary>
#nullable disable
    public partial class GrnBoneView : ReactiveUserControl<GrnBoneViewModel>
    {
        public GrnBoneView()
        {
            InitializeComponent();

            this.WhenActivated(disposableRegistration =>
            {
                this.OneWayBind(ViewModel,
                    v => v.Name,
                    view => view.mainGroupBox.Header)
                    .DisposeWith(disposableRegistration);

                this.Bind(ViewModel,
                    v => v.Position,
                    view => view.positionVecControl.Value)
                    .DisposeWith(disposableRegistration);
            });
        }
    }
}
