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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AdornerTest3
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DragAdorner _dragAdorner;
        private AdornerLayer _adornerLayer;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void UIElement_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                var adornedElement = (UIElement)sender;
                _adornerLayer = AdornerLayer.GetAdornerLayer(adornedElement);
                _dragAdorner = new DragAdorner(adornedElement);
                _adornerLayer.Add(_dragAdorner);
                _dragAdorner.Update(e.GetPosition(this));
                adornedElement.CaptureMouse();
            }
        }

        private void UIElement_OnMouseMove(object sender, MouseEventArgs e)
        {
            if (_dragAdorner != null)
            {
                _dragAdorner.Update(e.GetPosition(this));
            }
        }

        private void UIElement_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_dragAdorner != null)
            {
                var adornedElement = (UIElement)sender;
                _adornerLayer.Remove(_dragAdorner);
                _dragAdorner = null;
                adornedElement.ReleaseMouseCapture();
            }
        }
    }
}
