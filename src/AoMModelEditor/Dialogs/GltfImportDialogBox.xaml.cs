using System;
using System.Windows;

namespace AoMModelEditor.Dialogs
{
    /// <summary>
    /// Interaction logic for GltfImportDialogBox.xaml
    /// </summary>
    public partial class GltfImportDialogBox : Window
    {
        public GltfImportDialogBox()
        {
            InitializeComponent();
        }

        private void okButton_Click(object sender, RoutedEventArgs e) => DialogResult = true;
    }
}
