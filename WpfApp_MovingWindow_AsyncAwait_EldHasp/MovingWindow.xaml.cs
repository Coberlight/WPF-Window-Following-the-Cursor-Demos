using System.Threading.Tasks;
using System.Threading;
using System;
using System.Windows;

namespace WpfApp_MovingWindow
{
    /// <summary>
    /// Логика взаимодействия для MovingWindow.xaml
    /// </summary>
    public partial class MovingWindow : Window
    {
        private CancellationTokenSource tokenSource = new CancellationTokenSource();
        private double VerticalOffset;
        private double HorizontalOffset;

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            HorizontalOffset = sizeInfo.NewSize.Width / 2.0;
            VerticalOffset = -3;
        }

        public MovingWindow()
        {
            tokenSource.Cancel();
            InitializeComponent();
        }

        /// <summary>Метод включающий перемещение окна за курсором.</summary>
        /// <param name="refreshPeriod">Период обновления положения Окна, микросекунд.</param>
        /// <param name="speed">Максимальная скорость перемещения окна, пиксель в микросекунду. </param>
        /// <returns><see cref="Task"/> завершающийся после вызова метода <see cref="StopFollowCursor"/>.</returns>
        public async Task StartFollowCursorAsync(int refreshPeriod, double speed)
        {
            if (!tokenSource.IsCancellationRequested)
                return;
            tokenSource = new CancellationTokenSource();

            Point startPoint = CursorHelper.GetScreenPosition();
            DateTime lastTick = DateTime.Now;

            if (refreshPeriod < 10)
                refreshPeriod = 10;
            while (!tokenSource.IsCancellationRequested)
            {
                Point point = CursorHelper.GetScreenPosition();
                System.Windows.Vector d = point - startPoint;

                DateTime now = DateTime.Now;
                TimeSpan delta = now - lastTick;
                lastTick = now;

                double dMax = speed * delta.TotalMilliseconds;
                if (dMax < d.Length)
                {
                    d *= dMax / d.Length;
                }
                startPoint += d;

                Left = startPoint.X - HorizontalOffset;
                Top = startPoint.Y - VerticalOffset;

                await Task.Delay(refreshPeriod);
            }
        }

        /// <summary>Метод останавливающий перемещение окна.</summary>
        public void StopFollowCursor() {
            tokenSource.Cancel();
            this.Close();
        }
    }

}