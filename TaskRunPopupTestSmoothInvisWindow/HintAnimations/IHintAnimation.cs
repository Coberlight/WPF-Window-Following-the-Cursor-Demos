using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GiveFeedbackTest
{
    public interface IHintAnimation
    {
        void Init(double x, double y, double shiftX, double shiftY);
        Point GetAnimationFrame(Point targetPoint);
        void SetSpeed(double speed);
    }
}
