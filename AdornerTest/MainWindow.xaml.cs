using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

//Какая то хуйня сгенерированная чатом гпт

namespace AdornerTest
{
    public partial class MainWindow : Window
    {
        private FrameworkElement draggedElement;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void dragSource_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (draggedElement == null)
                {
                    draggedElement = (FrameworkElement)sender;

                    DataObject data = new DataObject(typeof(FrameworkElement), draggedElement);
                    DragDrop.DoDragDrop(draggedElement, data, DragDropEffects.Move);
                }

                Point position = e.GetPosition(this);

                //if (draggedElement != null)
                //{
                //    AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(draggedElement);
                //    adornerLayer.Add(new DragAdorner(draggedElement, position));
                //}
            }
        }

        private void dragSource_MouseUp(object sender, MouseButtonEventArgs e)
        {
            draggedElement = null;
        }
    }

    public class DragAdorner : Adorner
    {
        private FrameworkElement element;

        public DragAdorner(UIElement adornedElement, Point point) : base(adornedElement)
        {
            element = new TextBlock()
            {
                Text = "Dragging...",
                Background = Brushes.Transparent,
                Foreground = Brushes.Black,
                Opacity = 0.5,
                FontSize = 12
            };

            AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(adornedElement);
            adornerLayer.Add(this);
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            Point position = Mouse.GetPosition(this);
            drawingContext.DrawRectangle(Brushes.Transparent, new Pen(Brushes.Black, 1), new Rect(position, element.DesiredSize));
            drawingContext.DrawText(new FormattedText("Dragging...", System.Globalization.CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface("Arial"), 12, Brushes.Black), position);
        }
    }
}