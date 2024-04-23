using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GiveFeedbackTest
{
    internal class HintSmoothAnimation
    {
        private long millisecondsLast;
        private double lastX;
        private double lastY;
        private double speed;
        public HintSmoothAnimation()
        {
            
        }
        public void Init(double x, double y, double speed)
        {
            this.speed = speed;
            millisecondsLast = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            lastX = x;
            lastY = y;
        }
        public Point FollowCursor(Point targetPoint)
        {
            // Без Dispatcher выбрасывается исключение, что из другого потока невозможно получить/установить координаты окна через this.Left и this.Top
            {
                double destX = (double)targetPoint.X;
                double destY = (double)targetPoint.Y;
                double difX = destX - lastX;
                double difY = destY - lastY;

                long milliseconds = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                long millisecondsDelta = milliseconds - millisecondsLast;

                lastX += difX * speed * (millisecondsDelta / 16.0);
                lastY += difY * speed * (millisecondsDelta / 16.0);
                millisecondsLast = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                return new Point(lastX, lastY);

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
            };
        }
    }
}
