using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GiveFeedbackTest
{
    internal class HintSmoothAnimation : IHintAnimation
    {
        private long millisecondsLast;
        private double lastX;
        private double lastY;
        private double shiftX;
        private double shiftY;
        private double speed;
        public HintSmoothAnimation()
        {

        }
        public void Init(double x, double y, double shiftX, double shiftY)
        {
            millisecondsLast = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            lastX = x;
            lastY = y;
            this.shiftX = shiftX;
            this.shiftY = shiftY;
        }
        public void SetSpeed(double speed) { this.speed = speed; }
        public Point GetAnimationFrame(Point targetPoint)
        {
            double destX = (double)targetPoint.X + shiftX;
            double destY = (double)targetPoint.Y + shiftY;
            double difX = destX - lastX;
            double difY = destY - lastY;

            long milliseconds = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            long millisecondsDelta = milliseconds - millisecondsLast;

            lastX += difX * speed * (millisecondsDelta / 16.0);
            lastY += difY * speed * (millisecondsDelta / 16.0);
            millisecondsLast = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            return new Point(lastX, lastY);
        }
    }
}
