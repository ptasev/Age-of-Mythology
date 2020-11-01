using AoMEngineLibrary.Graphics.Brg;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
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
#nullable disable
    public partial class BrgMeshView : ReactiveUserControl<BrgMeshViewModel>
    {
        public BrgMeshView()
        {
            InitializeComponent();

            animTypeRadioList.EnumType = typeof(BrgMeshAnimType);
            interpTypeRadioList.EnumType = typeof(BrgMeshInterpolationType);
            flagsListBox.ItemsSource = Enum.GetValues(typeof(BrgMeshFlag));
            flags2ListBox.ItemsSource = Enum.GetValues(typeof(BrgMeshFormat));

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
                    v => v.FrameCount,
                    view => view.frameCountTextBlock.Text)
                    .DisposeWith(disposableRegistration);

                this.OneWayBind(ViewModel,
                    v => v.AnimLength,
                    view => view.animLengthTextBlock.Text)
                    .DisposeWith(disposableRegistration);

                this.Bind(ViewModel,
                    v => v.AnimationType,
                    view => view.animTypeRadioList.Value)
                    .DisposeWith(disposableRegistration);

                this.Bind(ViewModel,
                    v => v.InterpolationType,
                    view => view.interpTypeRadioList.Value)
                    .DisposeWith(disposableRegistration);

                this.Bind(ViewModel,
                    v => v.HotspotPosition,
                    view => view.hotspotPositionVecControl.Value)
                    .DisposeWith(disposableRegistration);

                this.Bind(ViewModel,
                    v => v.MassPosition,
                    view => view.massPositionVecControl.Value)
                    .DisposeWith(disposableRegistration);

                this.Bind(ViewModel,
                    v => v.CenterPosition,
                    view => view.centerPositionVecControl.Value)
                    .DisposeWith(disposableRegistration);

                this.Bind(ViewModel,
                    v => v.CenterRadius,
                    view => view.centerRadiusTextBox.Text,
                    centerRadiusTextBox.Events().LostFocus.Merge(centerRadiusTextBox.Events().KeyUp.Where(k => k.Key == Key.Enter)),
                    f => f.ToString("F5"),
                    f => 
                    { 
                        float.TryParse(f, out float r);
                        return r; 
                    })
                    .DisposeWith(disposableRegistration);

                this.Bind(ViewModel,
                    v => v.MinimumExtent,
                    view => view.minExtentPositionVecControl.Value)
                    .DisposeWith(disposableRegistration);

                this.Bind(ViewModel,
                    v => v.MaximumExtent,
                    view => view.maxExtentPositionVecControl.Value)
                    .DisposeWith(disposableRegistration);

                this.OneWayBind(ViewModel,
                    v => v.Flags,
                    view => view.flagsListBox.Tag)
                    .DisposeWith(disposableRegistration);

                this.OneWayBind(ViewModel,
                    v => v.Flags2,
                    view => view.flags2ListBox.Tag)
                    .DisposeWith(disposableRegistration);
            });
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            var checkBox = (CheckBox)sender;
            var dc = checkBox.DataContext;
            var actualValue = (BrgMeshFlag)Enum.Parse(typeof(BrgMeshFlag), dc.ToString());
            ViewModel.Flags |= actualValue;
        }
        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            var checkBox = (CheckBox)sender;
            var dc = checkBox.DataContext;
            var actualValue = (BrgMeshFlag)Enum.Parse(typeof(BrgMeshFlag), dc.ToString());
            ViewModel.Flags &= ~actualValue;
        }

        private void CheckBox2_Checked(object sender, RoutedEventArgs e)
        {
            var checkBox = (CheckBox)sender;
            var dc = checkBox.DataContext;
            var actualValue = (BrgMeshFormat)Enum.Parse(typeof(BrgMeshFormat), dc.ToString());
            ViewModel.Flags2 |= actualValue;
        }
        private void CheckBox2_Unchecked(object sender, RoutedEventArgs e)
        {
            var checkBox = (CheckBox)sender;
            var dc = checkBox.DataContext;
            var actualValue = (BrgMeshFormat)Enum.Parse(typeof(BrgMeshFormat), dc.ToString());
            ViewModel.Flags2 &= ~actualValue;
        }
    }
}
