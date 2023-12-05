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

        private void CenterBottomThumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            var thumb = sender as Thumb;
            var model = thumb.DataContext as RectModel;

            if (model.Height + e.VerticalChange >= 1)
            {
                model.Height += e.VerticalChange;
            }
        }

        private void CenterTopThumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {

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

        private void CenterLeftThumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {

        }

        private void CenterRightThumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {

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
