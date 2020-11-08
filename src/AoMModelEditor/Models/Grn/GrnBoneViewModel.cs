using AoMEngineLibrary.Graphics.Brg;
using AoMEngineLibrary.Graphics.Grn;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using System.Text;

namespace AoMModelEditor.Models.Grn
{
    public class GrnBoneViewModel : ReactiveObject, IModelObject
    {
        private readonly GrnBone _bone;

        public string Name { get; }

        public ObservableCollection<IModelObject> Children { get; }

        public Vector3 Position
        {
            get => _bone.Position;
            set
            {
                _bone.Position = value;
                this.RaisePropertyChanged(nameof(Position));
            }
        }

        public GrnBoneViewModel(GrnFile grn, int boneIndex)
        {
            var bone = grn.Bones[boneIndex];
            _bone = bone;
            Name = bone.Name;

            Children = new ObservableCollection<IModelObject>();
            for (int i = 0; i < grn.Bones.Count; ++i)
            {
                var child = grn.Bones[i];
                if (child.ParentIndex != boneIndex || i == boneIndex)
                    continue;

                var childVM = new GrnBoneViewModel(grn, i);
                Children.Add(childVM);
            }
        }
    }
}
