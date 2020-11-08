using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace AoMModelEditor.Models
{
    public interface IModelObject
    {
        string Name { get; }

        ObservableCollection<IModelObject> Children { get; }
    }
}
