using SharpGLTF.Schema2;
using System;
using System.Windows;

namespace AoMModelEditor.Dialogs
{
    /// <summary>
    /// Interaction logic for GltfImportDialogBox.xaml
    /// </summary>
    public partial class GltfImportDialogBox : Window
    {
        private ModelRoot? _gltf;

        public ModelRoot? GltfData
        {
            get => _gltf;
            set
            {
                _gltf = value;

                if (_gltf is null || _gltf.LogicalAnimations.Count <= 0) return;

                animationListBox.ItemsSource = _gltf.LogicalAnimations;
            }
        }

        public Animation? SelectedAnimation => animationListBox.SelectedValue as Animation;

        public GltfImportDialogBox()
        {
            InitializeComponent();
        }

        private void okButton_Click(object sender, RoutedEventArgs e) => DialogResult = true;
    }
}
