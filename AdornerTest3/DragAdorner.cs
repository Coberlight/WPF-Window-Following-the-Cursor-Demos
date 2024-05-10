using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;

namespace AdornerTest3
{
    public class DragAdorner : Adorner
    {
        private Point _startPoint;

        public DragAdorner(UIElement adornedElement) : base(adornedElement)
        {
            IsHitTestVisible = false; // Чтобы Adorner не перехватывал события мыши
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            // Устанавливаем размер Adorner равным размеру окна
            Rect bounds = new Rect(0, 0, this.AdornedElement.RenderSize.Width, this.AdornedElement.RenderSize.Height);

            // Устанавливаем положение Adorner за пределами окна
            //bounds.X = this.AdornedElement.TransformToAncestor((Visual)null).Transform(new Point()).X - 100;
            //bounds.Y = this.AdornedElement.TransformToAncestor((Visual)null).Transform(new Point()).Y - 100;

            ArrangeCore(new Rect(((Point)finalSize), bounds.Size));
            return finalSize;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            //if (_startPoint.HasValue)
            {
                Pen pen = new Pen(Brushes.Black, 2);
                drawingContext.DrawRectangle(null, pen, new Rect(_startPoint, RenderSize));
            }
        }

        public void Update(Point startPoint)
        {
            _startPoint = startPoint;
            InvalidateVisual();
        }
    }
}
