using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;

namespace GiveFeedbackTest
{
    internal class HintSimpleAnimation : IHintAnimation
    {
        double shiftX;
        double shiftY;
        public void Init(double x, double y, double shiftX, double shiftY)
        {
            this.shiftX = shiftX;
            this.shiftY = shiftY;
        }
        public Point GetAnimationFrame(Point point)
        {
            Point p = new Point(point.X + shiftX, point.Y + shiftY);
            return p;
        }
        public void SetSpeed(double speed)
        {
            //throw new NotSupportedException("Нельзя установить скорость простой анимации");
        }
    }
}
