using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace AoMModelEditor.Models
{
    public abstract class TreeViewItemModelObject : ReactiveObject, IModelObject
    {
        protected TreeViewItemModelObject? Parent { get; }

        public virtual string Name { get; protected set; }

        public ObservableCollection<IModelObject> Children { get; }

        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (value != _isSelected)
                {
                    _isSelected = value;
                    this.RaisePropertyChanged(nameof(IsSelected));
                }
            }
        }

        private bool _isExpanded;
        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                if (value != _isExpanded)
                {
                    _isExpanded = value;
                    this.RaisePropertyChanged(nameof(IsExpanded));
                }

                // Expand all the way up to the root.
                if (_isExpanded && Parent != null)
                    Parent.IsExpanded = true;
            }
        }

        public TreeViewItemModelObject()
            : this(null)
        {
        }
        public TreeViewItemModelObject(TreeViewItemModelObject? parent)
        {
            Name = string.Empty;
            Parent = parent;
            Children = new ObservableCollection<IModelObject>();
        }
    }
}
