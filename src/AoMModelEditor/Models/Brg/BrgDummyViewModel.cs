using AoMEngineLibrary.Graphics.Brg;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Text;

namespace AoMModelEditor.Models.Brg
{
    public class BrgDummyViewModel : ReactiveObject, IModelObject
    {
        private BrgAttachpoint _dummy;

        public string Name { get; }

        public BrgDummyViewModel(BrgAttachpoint dummy)
        {
            _dummy = dummy;

            Name = string.IsNullOrEmpty(dummy.Name) ? "Dummy" : dummy.Name;
        }
    }
}
