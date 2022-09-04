using AoMEngineLibrary.Graphics.Brg;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Numerics;
using System.Text;

namespace AoMModelEditor.Models.Brg
{
    public sealed class BrgDummyViewModel : TreeViewItemModelObject
    {
        private readonly List<BrgDummy> _dummies;
        private BrgDummy _dummy;

        private int _frame;
        public int Frame
        {
            get => _frame;
            set
            {
                _frame = value;
                _dummy = _dummies[_frame];
                this.RaisePropertyChanged(nameof(Right));
                this.RaisePropertyChanged(nameof(Up));
                this.RaisePropertyChanged(nameof(Forward));
                this.RaisePropertyChanged(nameof(Position));
                this.RaisePropertyChanged(nameof(Frame));
            }
        }

        public int MaxFrameIndex => _dummies.Count - 1;

        public Vector3 Right
        {
            get => _dummy.Right;
            set
            {
                _dummy.Right = value;
                this.RaisePropertyChanged(nameof(Right));
            }
        }

        public Vector3 Up
        {
            get => _dummy.Up;
            set
            {
                _dummy.Up = value;
                this.RaisePropertyChanged(nameof(Up));
            }
        }

        public Vector3 Forward
        {
            get => _dummy.Forward;
            set
            {
                _dummy.Forward = value;
                this.RaisePropertyChanged(nameof(Forward));
            }
        }

        public Vector3 Position
        {
            get => _dummy.Position;
            set
            {
                _dummy.Position = value;
                this.RaisePropertyChanged(nameof(Position));
            }
        }

        public BrgDummyViewModel(List<BrgDummy> dummies)
            : base()
        {
            _dummies = dummies;
            _dummy = dummies[0];

            Name = string.IsNullOrEmpty(_dummy.Name) ? "Dummy" : $"Dummy {_dummy.Name}";

            foreach (var d in dummies)
            {
                if (_dummy.Name != d.Name)
                    throw new Exception();
            }
        }
    }
}
