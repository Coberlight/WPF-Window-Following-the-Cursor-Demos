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
        public Point FollowPoint(Point targetPoint)
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
            };
        }
        //public double[] FollowPoint(Point targetPoint)
        //{
        //    // Без Dispatcher выбрасывается исключение, что из другого потока невозможно получить/установить координаты окна через this.Left и this.Top
        //    {
        //        double destX = (double)targetPoint.X;
        //        double destY = (double)targetPoint.Y;
        //        double difX = destX - lastX;
        //        double difY = destY - lastY;

        //        long milliseconds = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        //        long millisecondsDelta = milliseconds - millisecondsLast;

        //        lastX += difX * speed * (millisecondsDelta / 16.0);
        //        lastY += difY * speed * (millisecondsDelta / 16.0);
        //        millisecondsLast = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        //        return new double[2] { lastX, lastY };
        //    };
        //}
    }
}
