using AoMEngineLibrary.Graphics.Brg;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;

namespace AoMModelEditor.Models.Brg
{
    public class BrgSettingsViewModel : TreeViewItemModelObject
    {
        private float _sampleRateFps;
        public float SampleRateFps
        {
            get => _sampleRateFps;
            set
            {
                value = Math.Clamp(value, 1, 100);

                _sampleRateFps = value;
                this.RaisePropertyChanged(nameof(SampleRateFps));
            }
        }

        public BrgSettingsViewModel()
            : base()
        {
            Name = "Brg Settings";
            _sampleRateFps = 10.0f;
        }

        public GltfBrgParameters CreateGltfBrgParameters()
        {
            return new GltfBrgParameters()
            {
                SampleRateFps = SampleRateFps
            };
        }
    }
}
