using AoMEngineLibrary.Graphics.Brg;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;

namespace AoMModelEditor.Models.Brg
{
    public class BrgSettingsViewModel : ReactiveObject, IModelObject
    {
        public string Name { get; }

        public ObservableCollection<IModelObject> Children => new ObservableCollection<IModelObject>();

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
