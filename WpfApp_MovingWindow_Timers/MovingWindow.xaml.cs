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
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Threading;

namespace WpfApp_MovingWindow
{
    /// <summary>
    /// Логика взаимодействия для MovingWindow.xaml
    /// </summary>
    public partial class MovingWindow : Window
    {
        private volatile bool _isFollowingCursor;

        private volatile string _windowInfo; // _windowX и _windowY служат для отладки, чтобы передавать координаты окна для отображения
        private double _windowX = 0;
        private double _windowY = 0;
        public string windowInfo { get { return _windowInfo; } }

        private int screenRefreshRate;
        private int screenRefreshPeriod;

        private double speed;
        private double destX;
        private double destY;
        private double difX;
        private double difY;

        private long milliseconds;
        private long millisecondsLast;
        private long millisecondsDelta;

        private Point point;

        public bool isFollowingCursor
        {
            get { return _isFollowingCursor; }
            set { _isFollowingCursor = value; }
        }

        public MovingWindow()
        {
            InitializeComponent();
            _windowInfo = "placeholder";
        }
        // функция установки "прозрачности" окна для указателя мыши и UI-элементов
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            var hwnd = new WindowInteropHelper(this).Handle;
            WindowsServices.SetWindowExTransparent(hwnd);
        }

        public void FollowCursorInit()
        {
            speed = 0.2; // 0.2 вроде как норм
            milliseconds = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            millisecondsLast = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            point = CursorHelper.GetCursorPosition();

            milliseconds = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            millisecondsDelta = milliseconds - millisecondsLast;

            this.Left = point.X - this.Width / 2.0;
            this.Top = point.Y;
            _windowX = this.Left;
            _windowY = this.Top;
        }

        public void FollowCursor()
        {
            this.Dispatcher.Invoke(() =>
            {
                point = CursorHelper.GetCursorPosition();
                destX = (double)point.X - Width / 2.0;
                destY = (double)point.Y - Margin.Top + 3;
                difX = destX - this.Left;

                difY = destY - this.Top;

                milliseconds = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                millisecondsDelta = milliseconds - millisecondsLast;

                _windowX += difX * speed * (millisecondsDelta / 16.0);
                _windowY += difY * speed * (millisecondsDelta / 16.0);
                this.Left = _windowX;
                this.Top = _windowY;
                millisecondsLast = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

                //_windowInfo = new StringBuilder()
                //.Append("tempLeft: ")
                //.Append(tempLeft)
                //.Append("\ntempTop: ")
                //.Append(tempTop)
                //.Append("\nWinLeft: ")
                //.Append(this.Left)
                //.Append("\nWinTop: ")
                //.Append(this.Top)
                //.ToString();
            });
        }
    }
}
