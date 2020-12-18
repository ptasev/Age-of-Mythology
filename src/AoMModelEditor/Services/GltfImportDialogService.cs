using AoMModelEditor.Dialogs;
using SharpGLTF.Schema2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoMModelEditor.Services
{
    public class GltfImportDialogService
    {
        public GltfImportDialogService()
        {

        }

        public GltfImportDialogResult? OpenImportDialog(ModelRoot gltf)
        {
            var dialog = new GltfImportDialogBox()
            {
                GltfData = gltf
            };

            var dres = dialog.ShowDialog();
            if (dres != true)
                return null;

            var result = new GltfImportDialogResult();
            result.Animation = dialog.SelectedAnimation;

            return result;
        }
    }
}
