using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ClassroomTimer
{
    /// <summary>
    /// Interaction logic for TimerCtrl.xaml
    /// </summary>
    public partial class TimerCtrl : UserControl
    {
        private double _minValue = 0;
        private double _maxValue = 0;
        private double _value = 0;
        private bool _isHourTimer = false;
        private double _radius;
        private double _radiusMarker;

        private SolidColorBrush _minorTickColor = new SolidColorBrush(Color.FromRgb(0, 162, 255));
        private SolidColorBrush _majorTickColor = new SolidColorBrush(Color.FromRgb(120, 206, 255));
        private SolidColorBrush _expired = new SolidColorBrush(Color.FromArgb(255, 255, 0, 128));
        private SolidColorBrush _notexpired = new SolidColorBrush(Color.FromArgb(255, 0, 162, 255));
        private SolidColorBrush _warning = Brushes.Yellow;// new SolidColorBrush(Color.FromArgb(255, 0, 162, 255));

        public TimerCtrl()
        {
            InitializeComponent();
        }

        public void SetTimerParameters(int minValue, int maxValue, int value, bool isHourTimer)
        {
            _minValue = minValue;
            _maxValue = maxValue;
            _value = value;
            _isHourTimer = isHourTimer;

            // set tick marks
            // set marker position
            SetTickMarks();
            SetMarkerPosition();
        }

        public void UpdateMarker(int value, int parentValue, int warning = 1)
        {
            if (value <= 0 && parentValue <= 0)
            {
                arrow.Fill = Brushes.Red;
                value = 0;// Math.Abs(value);
                backgroundCircle.Stroke = _expired;
            }
            else if(value <= warning && parentValue <= 0)
            {
                arrow.Fill = _warning;
                backgroundCircle.Stroke = _warning;
            }
            else
            {
                arrow.Fill = Brushes.White;
                backgroundCircle.Stroke = _notexpired;
            }
            double theta = (-90 + 360 * value / (_maxValue - _minValue)) * Math.PI / 180.0;
            arrow.SetValue(Canvas.LeftProperty, _radius + Math.Cos(theta) * _radiusMarker - 10);
            arrow.SetValue(Canvas.TopProperty, _radius + Math.Sin(theta) * _radiusMarker - 6);
            markerAngle.Angle = theta * 180 / Math.PI;

        }

        private void SetTickMarks()
        {
            int majorDegreeTick = 12;
            int minorDegreeTick = 60;
            int majorTick = 5;
            if(_isHourTimer)
            {
                minorDegreeTick = 48;
                majorDegreeTick = 24;
                majorTick = 2;
            }
            //starting from the top, place minor tick marks every 1 degree on ellipse, skipping the fifth
            //starting from the top, place major tick marks every 5 degrees on ellipse

            double radius = ActualHeight / 2.0;
            double radiusInnerMinor = radius - 5;
            double radiusInnerMajor = radius - 10;
            double radiusOuter = radius;
            double theta = 0;
            double dtheta = 360.0 / minorDegreeTick * Math.PI / 180.0;
            double cx = radius;
            double cy = radius;
            for (int secs = 0; secs <= minorDegreeTick; secs++)
            {
                double radiusInner = 0;

                Line line = new Line();
                if (secs % majorTick == 0)
                {
                    radiusInner = radiusInnerMajor;
                    line.Stroke = _majorTickColor;
                }
                else
                {
                    radiusInner = radiusInnerMinor;
                    line.Stroke = _minorTickColor;
                }

                double x1 = cx + Math.Cos(theta) * radiusInner;
                double y1 = cy + Math.Sin(theta) * radiusInner;
                double x2 = cx + Math.Cos(theta) * radiusOuter;
                double y2 = cy + Math.Sin(theta) * radiusOuter;

                line.X1 = x1;
                line.X2 = x2;
                line.Y1 = y1;
                line.Y2 = y2;
                faceCanvas.Children.Add(line);

                theta += dtheta;
            }

        }

        private void SetMarkerPosition()
        {
        }

        private void timerCtrl_Loaded(object sender, RoutedEventArgs e)
        {
            _radius = ActualHeight / 2.0;
            _radiusMarker = ActualHeight / 2.0;
        }
    }
}
