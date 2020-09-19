using AoMEngineLibrary.Graphics.Brg;
using AoMEngineLibrary.Graphics.Converters;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace AoMModelEditor.Models.Brg
{
    public class BrgSettingsViewModel : ReactiveObject, IModelObject
    {
        public string Name { get; }

        private float _sampleRateFps;
        public float SampleRateFps
        {
            get => _sampleRateFps;
            set
            {
                _sampleRateFps = value;
                this.RaisePropertyChanged(nameof(SampleRateFps));
            }
        }

        public BrgSettingsViewModel()
        {
            Name = "Brg Settings";
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
