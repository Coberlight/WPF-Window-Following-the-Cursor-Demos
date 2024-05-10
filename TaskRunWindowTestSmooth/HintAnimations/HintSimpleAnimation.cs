using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;

namespace GiveFeedbackTest.HintAnimations
{
    internal class HintSimpleAnimation : IHintAnimation
    {
        public void Init(double x, double y, double shiftX, double shiftY)
        {
            //throw new NotSupportedException("Нечего инициализировать в простой анимации");
        }
        public Point GetAnimationFrame(Point point)
        {
            return point;
        }
        public void SetSpeed(double speed)
        {
            //throw new NotSupportedException("Нельзя установить скорость простой анимации");
        }
    }
}
