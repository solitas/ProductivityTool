using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Designer.ViewModels;
using ReactiveUI;

namespace Designer.Views
{
    /// <summary>
    /// NetworkView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class NetworkView : ReactiveUserControl<NetworkViewModel>
    {
        private BindingExpressionBase _viewportBinding;
        public NetworkView()
        {
            InitializeComponent();

            SetViewPortBinding();
        }

        public Rect NetworkViewportRegion
        {
            get
            {
                double left = Canvas.GetLeft(ContentContainer);
                if (Double.IsNaN(left))
                {
                    left = 0;
                }

                double top = Canvas.GetTop(ContentContainer);
                if (Double.IsNaN(top))
                {
                    top = 0;
                }

                if (ContentContainer.RenderTransform is ScaleTransform)
                {
                    GeneralTransform transform = this.TransformToDescendant(ContentContainer);
                    return transform.TransformBounds(new Rect(0, 0, this.ActualWidth, this.ActualHeight));
                }
                return new Rect(-left, -top, this.ActualWidth, this.ActualHeight);
            }
        }
        private void ContentContainer_LayoutUpdated(object sender, EventArgs e)
        {
            _viewportBinding.UpdateTarget();
        }

        private void SetViewPortBinding()
        {
            Binding binding = new Binding
            {
                Source = this,
                Path = new PropertyPath(nameof(NetworkViewportRegion)),
                Mode = BindingMode.OneWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            _viewportBinding = BindingOperations.SetBinding(ViewPortClip, RectangleGeometry.RectProperty, binding);

            this.WhenActivated(d =>
            {
                d(
                    this.WhenAnyObservable(v => v.ViewModel.SetZoom)
                        .Subscribe(result=> 
                        {
                            if (result)
                            {
                                
                            }
                        })
                );
            });
        }
        private void UserCanvas_Zoom(object source, Controls.ZoomEventArgs args)
        {
            _viewportBinding.UpdateTarget();
        }
    }
}
