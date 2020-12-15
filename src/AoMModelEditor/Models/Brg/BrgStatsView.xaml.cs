using ReactiveUI;
using System;
using System.Reactive.Disposables;

namespace AoMModelEditor.Models.Brg
{
    /// <summary>
    /// Interaction logic for BrgStatsView.xaml
    /// </summary>
#nullable disable
    public partial class BrgStatsView : ReactiveUserControl<BrgStatsViewModel>
    {
        public BrgStatsView()
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
                    v => v.DummyCount,
                    view => view.dummyCountLabel.Content)
                    .DisposeWith(disposableRegistration);

                this.OneWayBind(ViewModel,
                    v => v.FrameCount,
                    view => view.frameCountLabel.Content)
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
