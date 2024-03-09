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
        public string windowInfo { get { return _windowInfo; } }

        private int screenRefreshRate;
        private int screenRefreshPeriod;

        public bool isFollowingCursor
        {
            get { return _isFollowingCursor; }
            set { _isFollowingCursor = value; }
        }

        public MovingWindow()
        {
            InitializeComponent();
            screenRefreshRate = ScreenData.GetFPS();
            screenRefreshPeriod = (int)Math.Round((1.0 / screenRefreshRate) * 1000);
            _windowInfo = "placeholder";
        }
        // функция установки "прозрачности" окна для указателя мыши и UI-элементов
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            var hwnd = new WindowInteropHelper(this).Handle;
            WindowsServices.SetWindowExTransparent(hwnd);
        }

        public void FollowCursor()
        {
            bool init = true;
            double speed = 0.2; // 0.2 вроде как норм
            double destX;
            double destY;
            double difX;
            double difY;
            double tempLeft = 0; // ПОФИКСИЛОСЬ!!!
            double tempTop = 0;  // "Угловатость" траектории движения окна исчезла после введения двух переменных tempLeft и tempTop,
                                 // представляющих координаты окна (скорее всего, this.Left и this.Top округляются и из-за этого возникают проблемы)
            long milliseconds = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            long millisecondsLast = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            long millisecondsDelta;
            while (_isFollowingCursor)
            {
                var point = CursorHelper.GetCursorPosition();
                this.Dispatcher.Invoke(() =>
                {
                    milliseconds = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                    millisecondsDelta = milliseconds - millisecondsLast;
                    if (init)
                    {
                        this.Left = point.X - this.Width / 2.0;
                        this.Top = point.Y;
                        tempLeft = this.Left;
                        tempTop = this.Top;
                        init = false;
                    }
                    destX = (double)point.X - Width / 2.0;
                    destY = (double)point.Y - Margin.Top + 3;
                    difX = destX - this.Left;
                    difY = destY - this.Top;

                    //difX *= difX;
                    //difY *= difY;

                    tempLeft = tempLeft + difX * speed * (millisecondsDelta / 16.0);
                    tempTop = tempTop + difY * speed * (millisecondsDelta / 16.0);
                    this.Left = tempLeft;
                    this.Top = tempTop;
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
                    int timeToWait = screenRefreshPeriod - (int)millisecondsDelta;
                    Thread.Sleep(timeToWait > 0 ? timeToWait : 0); // для экономии процессора. Слишком частые установки координат сильно грузят процессор.
                });

                
            }

        }
    }
}
