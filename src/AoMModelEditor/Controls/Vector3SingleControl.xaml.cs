using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AoMModelEditor.Controls
{
    /// <summary>
    /// Interaction logic for Vector3SingleControl.xaml
    /// </summary>
    public partial class Vector3SingleControl : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public Vector3 Value
        {
            get { return (Vector3)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty = 
            DependencyProperty.Register("Value", typeof(Vector3), typeof(Vector3SingleControl),
                new PropertyMetadata(Vector3.Zero, new PropertyChangedCallback(OnValueChanged)));

        private static void OnValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            Vector3SingleControl? ctrl = sender as Vector3SingleControl;
            Vector3 newVal = (Vector3)e.NewValue;
            Vector3 oldVal = (Vector3)e.OldValue;
            if (ctrl != null)
            {
                if (ctrl.PropertyChanged == null)
                    return;

                if (newVal.X != oldVal.X)
                    ctrl.PropertyChanged(sender, new PropertyChangedEventArgs("X"));
                if (newVal.Y != oldVal.Y)
                    ctrl.PropertyChanged(sender, new PropertyChangedEventArgs("Y"));
                if (newVal.Z != oldVal.Z)
                    ctrl.PropertyChanged(sender, new PropertyChangedEventArgs("Z"));
                if (newVal != oldVal)
                    ctrl.PropertyChanged(sender, new PropertyChangedEventArgs("Value"));
            }
        }

        public float X
        {
            get { return Value.X; }
            set
            {
                Value = new Vector3(value, Value.Y, Value.Z);
                OnPropertyChanged("X");
            }
        }

        public float Y
        {
            get { return Value.Y; }
            set
            {
                Value = new Vector3(Value.X, value, Value.Z);
                OnPropertyChanged("Y");
            }
        }

        public float Z
        {
            get { return Value.Z; }
            set
            {
                Value = new Vector3(Value.X, Value.Y, value);
                OnPropertyChanged("Z");
            }
        }

        public Vector3SingleControl()
        {
            InitializeComponent();
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
