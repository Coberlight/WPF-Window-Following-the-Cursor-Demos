using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls;

namespace FilePreviewAdornerTest2
{
    public class MouseAdorner : Adorner
    {
        private readonly Image _image;

        public MouseAdorner(string filePath) : base(Application.Current.MainWindow)
        {
            BitmapImage bitmap = new BitmapImage(new Uri(filePath));
            double width = bitmap.PixelWidth;
            double height = bitmap.PixelHeight;

            _image = new Image
            {
                Source = bitmap,
                Width = width,
                Height = height
            };

            AddVisualChild(_image);
        }

        protected override Visual GetVisualChild(int index)
        {
            return _image;
        }

        protected override int VisualChildrenCount => 1;

        protected override Size MeasureOverride(Size constraint)
        {
            _image.Measure(constraint);
            return _image.DesiredSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            _image.Arrange(new Rect(finalSize));
            return finalSize;
        }
    }

}
