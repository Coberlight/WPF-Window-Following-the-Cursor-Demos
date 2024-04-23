using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls;

namespace FilePreviewAdornerTest.Models
{
    public class FilePreviewAdorner : Adorner
    {
        private FileInfo _file;
        private Image _image;

        public FilePreviewAdorner(UIElement adornedElement, FileInfo file) : base(adornedElement)
        {
            _file = file;
            // Здесь создайте и настройте Image для отображения превью файла
            _image = new Image();
            _image.Source = new BitmapImage(new Uri(_file.FullName)); // Замените эту строку на код для создания изображения файла

            AddVisualChild(_image);
        }

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

        protected override int VisualChildrenCount
        {
            get { return 1; }
        }

        protected override Visual GetVisualChild(int index)
        {
            if (index == 0)
            {
                return _image;
            }
            return base.GetVisualChild(index);
        }
    }
}
