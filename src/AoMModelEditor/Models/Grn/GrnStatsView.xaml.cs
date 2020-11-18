using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
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

namespace AoMModelEditor.Models.Grn
{
    /// <summary>
    /// Interaction logic for GrnStatsView.xaml
    /// </summary>
#nullable disable
    public partial class GrnStatsView : ReactiveUserControl<GrnStatsViewModel>
    {
        public GrnStatsView()
        {
            InitializeComponent();

            this.WhenActivated(disposableRegistration =>
            {
                this.OneWayBind(ViewModel,
                    v => v.Name,
                    view => view.mainGroupBox.Header)
                    .DisposeWith(disposableRegistration);

                this.OneWayBind(ViewModel,
                    v => v.VertexCount,
                    view => view.vertCountLabel.Content)
                    .DisposeWith(disposableRegistration);

                this.OneWayBind(ViewModel,
                    v => v.FaceCount,
                    view => view.faceCountLabel.Content)
                    .DisposeWith(disposableRegistration);

                this.OneWayBind(ViewModel,
                    v => v.MeshCount,
                    view => view.meshCountLabel.Content)
                    .DisposeWith(disposableRegistration);

                this.OneWayBind(ViewModel,
                    v => v.BoneCount,
                    view => view.boneCountLabel.Content)
                    .DisposeWith(disposableRegistration);

                this.OneWayBind(ViewModel,
                    v => v.MaterialCount,
                    view => view.materialCountLabel.Content)
                    .DisposeWith(disposableRegistration);

                this.OneWayBind(ViewModel,
                    v => v.AnimationDuration,
                    view => view.animationDurationLabel.Content,
                    f => f.ToString("F7"))
                    .DisposeWith(disposableRegistration);
            });
        }
    }
}
