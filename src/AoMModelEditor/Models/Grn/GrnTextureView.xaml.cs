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
    /// Interaction logic for GrnTextureView.xaml
    /// </summary>
#nullable disable
    public partial class GrnTextureView : ReactiveUserControl<GrnTextureViewModel>
    {
        public GrnTextureView()
        {
            InitializeComponent();

            this.WhenActivated(disposableRegistration =>
            {
                this.OneWayBind(ViewModel,
                    v => v.Name,
                    view => view.mainGroupBox.Header)
                    .DisposeWith(disposableRegistration);

                this.OneWayBind(ViewModel,
                    v => v.TextureName,
                    view => view.texNameTextBox.Text)
                    .DisposeWith(disposableRegistration);

                this.OneWayBind(ViewModel,
                    v => v.Width,
                    view => view.widthTextBox.Text)
                    .DisposeWith(disposableRegistration);

                this.OneWayBind(ViewModel,
                    v => v.Height,
                    view => view.heightTextBox.Text)
                    .DisposeWith(disposableRegistration);

                this.OneWayBind(ViewModel,
                    v => v.FileName,
                    view => view.texFileNameTextBox.Text)
                    .DisposeWith(disposableRegistration);
            });
        }
    }
}
