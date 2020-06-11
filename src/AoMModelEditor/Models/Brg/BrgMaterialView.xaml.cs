using AoMEngineLibrary.Graphics.Brg;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Numerics;
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
    /// Interaction logic for BrgMaterialView.xaml
    /// </summary>
    public partial class BrgMaterialView : ReactiveUserControl<BrgMaterialViewModel>
    {
        public BrgMaterialView()
        {
            InitializeComponent();

            flagsListBox.ItemsSource = Enum.GetValues(typeof(BrgMatFlag));

            var itm = flagsListBox.ItemContainerGenerator.ContainerFromItem(BrgMatFlag.Alpha);

            this.WhenActivated(disposableRegistration =>
            {
                this.OneWayBind(ViewModel,
                    v => v.Name,
                    view => view.mainGroupBox.Header)
                    .DisposeWith(disposableRegistration);

                this.OneWayBind(ViewModel,
                    v => v.Flags,
                    view => view.flagsListBox.Tag)
                    .DisposeWith(disposableRegistration);

                this.Bind(ViewModel,
                    v => v.DiffuseMap,
                    view => view.diffuseMapTextBlock.Text)
                    .DisposeWith(disposableRegistration);

                this.Bind(this.ViewModel,
                    v => v.DiffuseColor,
                    view => view.diffuseColorPicker.SelectedColor,
                    this.Vector3ToColorConverter,
                    this.ColorToVector3Converter)
                    .DisposeWith(disposableRegistration);

                this.Bind(this.ViewModel,
                    v => v.AmbientColor,
                    view => view.ambientColorPicker.SelectedColor,
                    this.Vector3ToColorConverter,
                    this.ColorToVector3Converter)
                    .DisposeWith(disposableRegistration);

                this.Bind(this.ViewModel,
                    v => v.SpecularColor,
                    view => view.specularColorPicker.SelectedColor,
                    this.Vector3ToColorConverter,
                    this.ColorToVector3Converter)
                    .DisposeWith(disposableRegistration);

                this.Bind(this.ViewModel,
                    v => v.EmissiveColor,
                    view => view.emissiveColorPicker.SelectedColor,
                    this.Vector3ToColorConverter,
                    this.ColorToVector3Converter)
                    .DisposeWith(disposableRegistration);
            });
        }

        private Color? Vector3ToColorConverter(Vector3 vec)
        {
            var vecByte = vec * byte.MaxValue;
            return Color.FromRgb(Convert.ToByte(vec.X * byte.MaxValue),
                Convert.ToByte(vec.Y * byte.MaxValue),
                Convert.ToByte(vec.Z * byte.MaxValue));
        }

        private Vector3 ColorToVector3Converter(Color? color)
        {
            if (color != null)
            {
                var c = color.Value;
                return new Vector3((float)c.R / byte.MaxValue, (float)c.G / byte.MaxValue, (float)c.B / byte.MaxValue);
            }

            return Vector3.One;
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            var checkBox = (CheckBox)sender;
            var dc = checkBox.DataContext;
            var actualValue = (BrgMatFlag)Enum.Parse(typeof(BrgMatFlag), dc.ToString());
            ViewModel.Flags |= actualValue;
        }
        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            var checkBox = (CheckBox)sender;
            var dc = checkBox.DataContext;
            var actualValue = (BrgMatFlag)Enum.Parse(typeof(BrgMatFlag), dc.ToString());
            ViewModel.Flags &= ~actualValue;
        }
    }
}
