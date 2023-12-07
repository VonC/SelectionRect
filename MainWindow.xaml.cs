using System;
using System.ComponentModel;
using System.Diagnostics;
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

        private Point _lastCenterPosition = new Point(0, 0);

        private void CheckForCenterJump(RectModel model, string eventName)
        {
            var newCenterPosition = new Point(model.Left + model.CenterX, model.Top + model.CenterY);
            if (Math.Abs(newCenterPosition.X - _lastCenterPosition.X) > 10 ||
                Math.Abs(newCenterPosition.Y - _lastCenterPosition.Y) > 10)
            {
                Debug.WriteLine($"======= ====== Significant move detected by {eventName}: New Center = {newCenterPosition}");
                _lastCenterPosition = newCenterPosition;
            }
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
            CheckForCenterJump(model, "CenterTopThumb_DragDelta");
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

            CheckForCenterJump(model, "CenterLeftThumb_DragDelta");
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

            CheckForCenterJump(model, "CenterRightThumb_DragDelta");
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
            CheckForCenterJump(model, "CenterBottomThumb_DragDelta");
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


        private void RotateThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            var thumb = sender as Thumb;
            var model = thumb.DataContext as RectModel;

            // Debugging log
            Debug.WriteLine($"Before Rotate: Left={model.Left}, Top={model.Top}, Angle={model.Angle}");

            // Calculate the center of the rectangle in canvas coordinates
            var rectCenter = new Point(model.Left + model.Width / 2, model.Top + model.Height / 2);
            var mousePosition = Mouse.GetPosition(Canvas1);

            // Calculate the angle of rotation based on the mouse position
            model.Angle = rectCenter.GetAngle(mousePosition) + 90;

            // No need to change Left and Top as the rotation is around the center
            // Debugging log
            Debug.WriteLine($"After Rotate: Left={model.Left}, Top={model.Top}, Angle={model.Angle}");

            CheckForCenterJump(model, "RotateThumb_DragDelta");
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

        private void RotateThumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            var thumb = sender as Thumb;
            var model = thumb.DataContext as RectModel;

            // Store the initial position
            _initialLeft = model.Left;
            _initialTop = model.Top;

            // Debugging log
            Debug.WriteLine($"Rotate Started: _initialLeft={_initialLeft}, _initialTop={_initialTop}");
        }


        private void CenterThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            var thumb = sender as Thumb;
            var model = thumb.DataContext as RectModel;
            var transform = RectGrid.LayoutTransform as RotateTransform;
            var angle = transform?.Angle ?? 0;
            var radianAngle = angle * Math.PI / 180;
            var cosAngle = Math.Cos(radianAngle);
            var sinAngle = Math.Sin(radianAngle);

            // Calculating new position considering the rotation
            double newLeft = model.Left + (e.HorizontalChange * cosAngle - e.VerticalChange * sinAngle);
            double newTop = model.Top + (e.VerticalChange * cosAngle + e.HorizontalChange * sinAngle);

            model.Left = newLeft;
            model.Top = newTop;

            CheckForCenterJump(model, "CenterThumb_DragDelta");
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

        // Add debug information to property setters
        private double _Left;
        public double Left
        {
            get { return _Left; }
            set
            {
                if (value == _Left) return;
                _Left = value;
                Debug.WriteLine($"Left Changed: {_Left}");
                OnPropertyChanged(nameof(Left));
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
                Debug.WriteLine($"Top Changed: {_Top}");
                OnPropertyChanged(nameof(Top));
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
