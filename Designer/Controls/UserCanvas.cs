using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Designer.Controls
{
    public class UserCanvas : Canvas
    {
        private const double MAX_WHEEL_OFFSET = 15;
        private const double MIN_WHEEL_OFFSET = 1;

        private Point _previousMousePosition;
        private bool _isClicked;
        private double _wheelOffset; 

        private ScaleTransform _currentScale = new ScaleTransform(1.0, 1.0);

        public delegate void ZoomEvent(object source, ZoomEventArgs args);
        public event ZoomEvent Zoom;
        
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            _previousMousePosition = e.GetPosition(this);
            _isClicked = true;

            Focus();
            CaptureMouse();

            base.OnMouseLeftButtonDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (_isClicked)
            {
                var currentMousePosition = e.GetPosition(this);
                if (!currentMousePosition.Equals(_previousMousePosition))
                {
                    var xDelta = currentMousePosition.X - _previousMousePosition.X;
                    if (double.IsNaN(xDelta))
                    {
                        xDelta = 0;
                    }

                    var yDelta = currentMousePosition.Y - _previousMousePosition.Y;
                    if (double.IsNaN(yDelta))
                    {
                        yDelta = 0;
                    }

                    AdjustChildrenPosition(xDelta, yDelta);
                }
                _previousMousePosition = currentMousePosition;
            }

            base.OnMouseMove(e);
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            _isClicked = false;
            ReleaseMouseCapture();
            base.OnMouseLeftButtonUp(e);
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);
            var delta = e.Delta;

            if (_wheelOffset <= MIN_WHEEL_OFFSET && delta < 0 ||
                _wheelOffset >= MAX_WHEEL_OFFSET && delta > 0)
            {
                return;
            }
            _wheelOffset += e.Delta / 120;

            if (_wheelOffset < MIN_WHEEL_OFFSET) _wheelOffset = MIN_WHEEL_OFFSET;
            if (_wheelOffset > MAX_WHEEL_OFFSET) _wheelOffset = MAX_WHEEL_OFFSET;


            double oldScale = _currentScale.ScaleX;
            double newScale = Math.Log(1 + ((_wheelOffset) / 10d)) * 2d;

            var point1 = TranslatePoint(new Point(0,0), Children[0]);
            var point2 = TranslatePoint(new Point(ActualWidth, ActualHeight), Children[0]);

            Rect rect = new Rect(point1, point2);
            Point viewMousePos = e.GetPosition(this);
            Point relativeZoomPoint = new Point(viewMousePos.X / ActualWidth, viewMousePos.Y / ActualHeight);

            var zoomModifier = oldScale / newScale;
            var newSize = new Size(rect.Width * zoomModifier, rect.Height * zoomModifier);

            var zoomCenterX = rect.X + (rect.Width * relativeZoomPoint.X);
            var zoomCenterY = rect.Y + (rect.Height * relativeZoomPoint.Y);
            var newX = zoomCenterX - (relativeZoomPoint.X * newSize.Width);
            var newY = zoomCenterY - (relativeZoomPoint.Y * newSize.Height);
            var newView = new Rect(newX, newY, newSize.Width, newSize.Height);
            
            Point newOffset = new Point(-newView.X * newScale, -newView.Y * newScale);

            //Calculate new viewing window scale
            ScaleTransform newScaleTransform = new ScaleTransform
            {
                ScaleX = newScale,
                ScaleY = newScale
            };
            var zoomEvent = new ZoomEventArgs(e, _currentScale, newScaleTransform, newOffset);
            Zoom?.Invoke(this, zoomEvent);

            AdjustChildrenScale(newScaleTransform, newOffset);

            _currentScale = newScaleTransform;
        }

        private void AdjustChildrenPosition(double xDelta, double yDelta)
        {
            Point previousPosition = new Point();
            foreach (UIElement child in Children)
            {
                previousPosition.X = Canvas.GetLeft(child);
                if (double.IsNaN(previousPosition.X))
                {
                    previousPosition.X = 0;
                }
                previousPosition.Y = Canvas.GetTop(child);
                if (double.IsNaN(previousPosition.Y))
                {
                    previousPosition.Y = 0;
                }
                Canvas.SetLeft(child, previousPosition.X + xDelta);
                Canvas.SetTop(child, previousPosition.Y + yDelta);
            }
        }
        private void AdjustChildrenScale(ScaleTransform scale, Point offset)
        {
            foreach(UIElement element in Children)
            {
                element.RenderTransform = scale;
                Canvas.SetLeft(element, offset.X);
                Canvas.SetTop(element, offset.Y);
            }
        }
    }

    public class ZoomEventArgs : EventArgs
    {
        public MouseEventArgs MouseEvent { get; }
        public ScaleTransform OldScaleScale { get; }
        public ScaleTransform NewScale { get; }
        public Point ContentOffset { get; }

        public ZoomEventArgs(MouseEventArgs e, ScaleTransform oldScale, ScaleTransform newScale, Point contentOffset)
        {
            this.MouseEvent = e;
            this.OldScaleScale = oldScale;
            this.NewScale = newScale;
            this.ContentOffset = contentOffset;
        }
    }
}
