using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace SelectionRect
{
    public partial class MainWindow : Window
    {
        private double _initialLeft;
        private double _initialTop;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = new RectModel() { Height = 200, Width = 100 , Left = 20, Top = 50};
        }

        private void CenterTopThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            var thumb = sender as Thumb;
            var model = thumb.DataContext as RectModel;
            var transform = RectGrid.LayoutTransform as RotateTransform;
            var angle = transform?.Angle ?? 0;

            var (newWidth, newHeight, newLeft, newTop) = CalculateAdjustedSizeAndPosition(
                model.Width, model.Height, model.Left, model.Top,
                e.HorizontalChange, e.VerticalChange, angle, ThumbAction.ResizeFromTop);

            model.Width = newWidth;
            model.Height = newHeight;
            model.Left = newLeft;
            model.Top = newTop;
        }

        // Similar refactoring for other DragDelta methods
        private void CenterLeftThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            var thumb = sender as Thumb;
            var model = thumb.DataContext as RectModel;
            var transform = RectGrid.LayoutTransform as RotateTransform;
            var angle = transform?.Angle ?? 0;

            var (newWidth, newHeight, newLeft, newTop) = CalculateAdjustedSizeAndPosition(
                model.Width, model.Height, model.Left, model.Top,
                e.HorizontalChange, e.VerticalChange, angle, ThumbAction.ResizeFromLeft);

            model.Width = newWidth;
            model.Height = newHeight;
            model.Left = newLeft;
            model.Top = newTop;
        }

        private void CenterRightThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            var thumb = sender as Thumb;
            var model = thumb.DataContext as RectModel;
            var transform = RectGrid.LayoutTransform as RotateTransform;
            var angle = transform?.Angle ?? 0;

            var (newWidth, newHeight, newLeft, newTop) = CalculateAdjustedSizeAndPosition(
                model.Width, model.Height, model.Left, model.Top,
                e.HorizontalChange, e.VerticalChange, angle, ThumbAction.ResizeFromRight);

            model.Width = newWidth;
            model.Height = newHeight;
            model.Left = newLeft;
            model.Top = newTop;
        }

        private void CenterBottomThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            var thumb = sender as Thumb;
            var model = thumb.DataContext as RectModel;
            var transform = RectGrid.LayoutTransform as RotateTransform;
            var angle = transform?.Angle ?? 0;

            var (newWidth, newHeight, newLeft, newTop) = CalculateAdjustedSizeAndPosition(
                model.Width, model.Height, model.Left, model.Top,
                e.HorizontalChange, e.VerticalChange, angle, ThumbAction.ResizeFromBottom);

            model.Width = newWidth;
            model.Height = newHeight;
            model.Left = newLeft;
            model.Top = newTop;
        }


        public enum ThumbAction
        {
            ResizeFromTop,
            ResizeFromLeft,
            ResizeFromRight,
            ResizeFromBottom
            // Add other actions as needed
        }

        private (double NewWidth, double NewHeight, double NewLeft, double NewTop) CalculateAdjustedSizeAndPosition(
    double initialWidth, double initialHeight, double initialLeft, double initialTop,
    double horizontalChange, double verticalChange, double angle, ThumbAction action)
        {
            var radianAngle = angle * Math.PI / 180;
            var cosAngle = Math.Cos(radianAngle);
            var sinAngle = Math.Sin(radianAngle);

            double newWidth = initialWidth, newHeight = initialHeight;
            double newLeft = initialLeft, newTop = initialTop;

            switch (action)
            {
                case ThumbAction.ResizeFromTop:
                    var verticalAdjustmentTop = verticalChange * cosAngle;
                    if (initialHeight - verticalAdjustmentTop >= 1)
                    {
                        newHeight -= verticalAdjustmentTop;
                        newTop += verticalChange;
                    }
                    break;
                case ThumbAction.ResizeFromLeft:
                    var horizontalAdjustmentLeft = horizontalChange * cosAngle;
                    if (initialWidth - horizontalAdjustmentLeft >= 1)
                    {
                        newWidth -= horizontalAdjustmentLeft;
                        newLeft += horizontalChange;
                    }
                    break;
                case ThumbAction.ResizeFromRight:
                    var horizontalAdjustmentRight = horizontalChange * cosAngle;
                    if (initialWidth + horizontalAdjustmentRight >= 1)
                    {
                        newWidth += horizontalAdjustmentRight;
                    }
                    break;
                case ThumbAction.ResizeFromBottom:
                    var verticalAdjustmentBottom = verticalChange * cosAngle;
                    if (initialHeight + verticalAdjustmentBottom >= 1)
                    {
                        newHeight += verticalAdjustmentBottom;
                    }
                    break;
                    // Add other cases if you have other resizing actions
            }

            return (newWidth, newHeight, newLeft, newTop);
        }


        private void RotateThumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            var thumb = sender as Thumb;
            var model = thumb.DataContext as RectModel;

            var initialAngle = model.Angle;
            var center = new Point(model.CenterX, model.CenterY);
            center = RectGrid.TranslatePoint(center, Canvas1);
            var currentPos = Mouse.GetPosition(Canvas1);
            model.Angle = center.GetAngle(currentPos) + 90;

            var rotatedRect = RotateRect(new Rect(_initialLeft, _initialTop, model.Width, model.Height), model.Angle, center);
            model.Left = rotatedRect.Left;
            model.Top = rotatedRect.Top;
        }

        private Rect RotateRect(Rect rect, double angle, Point center)
        {
            var cornerPoints = new[] { rect.TopLeft, rect.TopRight, rect.BottomRight, rect.BottomLeft };
            var m = new Matrix();
            m.RotateAt(angle, center.X, center.Y);
            m.Transform(cornerPoints);

            double minX = cornerPoints.Min(p => p.X);
            double minY = cornerPoints.Min(p => p.Y);
            double maxX = cornerPoints.Max(p => p.X);
            double maxY = cornerPoints.Max(p => p.Y);

            return new Rect(minX, minY, maxX - minX, maxY - minY);
        }

        private void RotateThumb_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            var thumb = sender as Thumb;
            var model = thumb.DataContext as RectModel;

            _initialLeft = model.Left;
            _initialTop = model.Top;
        }


        private void CenterThumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            var thumb = sender as Thumb;
            var model = thumb.DataContext as RectModel;
            model.Top += e.VerticalChange;
            model.Left += e.HorizontalChange;
        }

        private void CenterThumbBorder_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void LeftBottomThumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {

        }

        private void LeftTopThumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {

        }

        private void RightTopThumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {

        }

        private void RightBottomThumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {

        }
    }

    public static class Ex
    {
        public static double GetAngle(this System.Windows.Point p1, System.Windows.Point p2)
        {
            var xDiff = p2.X - p1.X;
            var yDiff = p2.Y - p1.Y;
            return Math.Atan2(yDiff, xDiff) * 180.0 / Math.PI;
        }
    }

    class RectModel : INotifyPropertyChanged
    {
        private double _Angle;
        public double Angle
        {
            get { return _Angle; }
            set
            {
                if (value == _Angle) return;
                _Angle = value;
                OnPropertyChanged(nameof(Angle));
            }
        }

        private double _Height;
        public double Height
        {
            get { return _Height; }
            set
            {
                if (value == _Height) return;
                _Height = value;
                OnPropertyChanged(nameof(Height));
            }
        }

        private double _Width;
        public double Width
        {
            get { return _Width; }
            set
            {
                if (value == _Width) return;
                _Width = value;
                OnPropertyChanged(nameof(Width));
            }
        }

        private double _Top;
        public double Top
        {
            get { return _Top; }
            set
            {
                if (value == _Top) return;
                _Top = value;
                OnPropertyChanged(nameof(Top));
            }
        }

        private double _Left;
        public double Left
        {
            get { return _Left; }
            set
            {
                if (value == _Left) return;
                _Left = value;
                OnPropertyChanged(nameof(Left));
            }
        }

        public double CenterX => Width / 2;
        public double CenterY => Height / 2;

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
