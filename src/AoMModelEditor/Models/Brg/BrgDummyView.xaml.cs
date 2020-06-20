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
    /// Interaction logic for BrgDummyView.xaml
    /// </summary>
#nullable disable
    public partial class BrgDummyView : ReactiveUserControl<BrgDummyViewModel>
    {
        public BrgDummyView()
        {
            InitializeComponent();

            this.WhenActivated(disposableRegistration =>
            {
                this.Bind(ViewModel,
                    v => v.Frame,
                    view => view.frameUpDown.Value)
                    .DisposeWith(disposableRegistration);

                this.OneWayBind(ViewModel,
                    v => v.Frame,
                    view => view.frameTextBox.Text)
                    .DisposeWith(disposableRegistration);

                this.OneWayBind(ViewModel,
                    v => v.MaxFrameIndex,
                    view => view.frameUpDown.Maximum)
                    .DisposeWith(disposableRegistration);

                this.Bind(ViewModel,
                    v => v.Right,
                    view => view.rightVecControl.Value)
                    .DisposeWith(disposableRegistration);

                this.Bind(ViewModel,
                    v => v.Up,
                    view => view.upVecControl.Value)
                    .DisposeWith(disposableRegistration);

                this.Bind(ViewModel,
                    v => v.Forward,
                    view => view.forwardVecControl.Value)
                    .DisposeWith(disposableRegistration);

                this.Bind(ViewModel,
                    v => v.Position,
                    view => view.positionVecControl.Value)
                    .DisposeWith(disposableRegistration);
            });
        }
    }
}
