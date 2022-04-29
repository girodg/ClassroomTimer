using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Threading;

namespace ClassroomTimer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DispatcherTimer _timer = new DispatcherTimer(DispatcherPriority.Render);
        private DateTime _startTime;
        private DateTime _breakTime;
        private DateTime _testTime;
        private int _breakLength = 15;
        private System.Media.SoundPlayer beep = new System.Media.SoundPlayer(@"beep-07.wav");
        private int _lastSec = 0;

        private ClockMode _clockMode = ClockMode.StartTime;
        enum ClockMode
        {
            StartTime,
            BreakTime,
            TestTIme
        }

        public MainWindow()
        {
            DateTime n = DateTime.Now.Date;
            _startTime = new DateTime(n.Year, n.Month, n.Day, 9, 0, 0);

            InitializeComponent();
            _timer.Interval = TimeSpan.FromMilliseconds(10);
            _timer.Tick += _timer_Tick;
        }

        private void _timer_Tick(object sender, EventArgs e)
        {
            switch (_clockMode)
            {
                case ClockMode.StartTime:
                    UpdateRemainingTime(_startTime);
                    break;
                case ClockMode.BreakTime:
                    UpdateRemainingTime(_breakTime);
                    break;
                case ClockMode.TestTIme:
                    UpdateRemainingTime(_testTime);
                    break;
                default:
                    break;
            }
        }

        private void startTimeBtn_Click(object sender, RoutedEventArgs e)
        {
            DateTime n = DateTime.Now.Date;
            ToggleButton tb = sender as ToggleButton;
            bool[] togs = new bool[] { false, false };
            switch (tb.Name)
            {
                case "AM":
                    togs[0] = true;
                    _startTime = new DateTime(n.Year, n.Month, n.Day, 9, 0, 0);
                    break;
                case "PM":
                    togs[1] = true;
                    _startTime = new DateTime(n.Year, n.Month, n.Day, 13, 0, 0);
                    break;
                case "startTimeBtn":
                    togs[0] = AM.IsChecked == true;
                    togs[1] = PM.IsChecked == true;
                    break;
                default:
                    break;
            }
            SetStartToggles(togs);
            _clockMode = ClockMode.StartTime;
            UpdateRemainingTime(_startTime);
            SetToggles(new bool[] { true, false, false, false});
            startCanvas.Visibility = Visibility.Visible;
            breakCanvas.Visibility = Visibility.Collapsed;
        }

        private void breakTimeBtn_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton tb = sender as ToggleButton;
            bool[] togs = new bool[] { false, false, false };
            switch (tb.Name)
            {
                case "Five":
                    togs[0] = true;
                    _breakLength = 5;
                    break;
                case "Ten":
                    togs[1] = true;
                    _breakLength = 10;
                    break;
                case "Fifteen":
                    togs[2] = true;
                    _breakLength = 15;
                    break;
                case "breakTimeBtn":
                    if (_breakLength == 5)
                        togs[0] = true;
                    else if (_breakLength == 10)
                        togs[1] = true;
                    else
                        togs[2] = true;
                    //_breakLength = 15;
                    break;
                default:
                    break;
            }
            SetBreakToggles(togs);
            _breakTime = DateTime.Now.AddMinutes(_breakLength);
            _clockMode = ClockMode.BreakTime;
            UpdateRemainingTime(_breakTime);
            SetToggles(new bool[] { false, true, false, false });
            breakCanvas.Visibility = Visibility.Visible;
            startCanvas.Visibility = Visibility.Collapsed;
        }

        private void TestTimeBtn_Click(object sender, RoutedEventArgs e)
        {
            _testTime = DateTime.Now.AddHours(2);
            _clockMode = ClockMode.TestTIme;
            UpdateRemainingTime(_testTime);
            SetToggles(new bool[] { false, false, true, false });
            breakCanvas.Visibility = Visibility.Collapsed;
        }

        private void QuizTimeBtn_Click(object sender, RoutedEventArgs e)
        {
            _testTime = DateTime.Now.AddMinutes(30);
            _clockMode = ClockMode.TestTIme;
            UpdateRemainingTime(_testTime);
            SetToggles(new bool[] { false, false, false, true });
            breakCanvas.Visibility = Visibility.Collapsed;
        }

        private void SetStartToggles(bool[] toggles)
        {
            AM.IsChecked = toggles[0];
            PM.IsChecked = toggles[1];
        }

        private void SetBreakToggles(bool[] toggles)
        {
            Five.IsChecked = toggles[0];
            Ten.IsChecked = toggles[1];
            Fifteen.IsChecked = toggles[2];
        }

        private void SetToggles(bool[] toggles)
        {
            startTimeBtn.IsChecked = toggles[0];
            breakTimeBtn.IsChecked = toggles[1];
            testTimeBtn.IsChecked = toggles[2];
            quizTimeBtn.IsChecked = toggles[3];
        }

        private void UpdateRemainingTime(DateTime startTime)
        {
            TimeSpan n = startTime - DateTime.Now;
            string sign = "";
            if(n.TotalSeconds < 0)//.Minutes < 0 && n.Seconds < 0)
            {
                //sign = "-";
                RemainingTime.Foreground = Brushes.Red;
            }
            else if(n.TotalSeconds < 60)
            {
                RemainingTime.Foreground = Brushes.Yellow;
            }
            else
            {
                RemainingTime.Foreground = new SolidColorBrush(Color.FromArgb(255,0,162,255));
            }
            //RemainingTime.Text = string.Format("{0:00} MIN {1:00} SEC", n.Minutes, n.Seconds);
            //RemainingTime.Text = string.Format("{0:c}", n, sign);
            RemainingTime.Text = string.Format("{1}{0:hh\\:mm\\:ss\\.ff}", n, sign);

            DayTime.Text = string.Format("{0:ddd}", DateTime.Now).ToUpper();
            ClockTime.Text = string.Format("{0:hh:mm:ss}", DateTime.Now);
            ClockTimeAMPM.Text = string.Format("{0:tt}", DateTime.Now);

            TodaysDateTime.Text = string.Format("{0:MMMM dd yyyy}", DateTime.Now).ToUpper();

            hourTimer.UpdateMarker(n.Hours, 0, 1);
            minuteTimer.UpdateMarker(n.Minutes, n.Hours, 1);
            secondTimer.UpdateMarker(n.Seconds, n.Minutes, 10);
            msTimer.UpdateMarker(n.Milliseconds, n.Seconds, 0);

            if(_lastSec != n.Seconds)
            {
                _lastSec = n.Seconds;
                beep.Play();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            hourTimer.SetTimerParameters(0, 24, 0, true);
            minuteTimer.SetTimerParameters(0, 60, 0, false);
            secondTimer.SetTimerParameters(0, 60, 0, false);
            msTimer.SetTimerParameters(0, 1000, 0, false);
            _timer.Start();
        }
    }
}
