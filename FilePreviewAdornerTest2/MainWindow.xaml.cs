using System;
using System.Collections.Generic;
using System.IO;
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

namespace FilePreviewAdornerTest2
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            FileInfo fileInfo = new FileInfo(@"D:\MainData\Desktop\Fruity Reeverb 2.png");
            string[] files = { fileInfo.FullName };

            var data = new DataObject(DataFormats.FileDrop, files);
            data.SetData(DataFormats.Text, files[0]);

            // Создаем Adorner для отображения эскиза файла
            AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(this);
            MouseAdorner adorner = new MouseAdorner(files[0]);
            adornerLayer.Update();
            adornerLayer.Add(adorner);

            DragDrop.DoDragDrop(this, data, DragDropEffects.Copy);

            // Удаляем Adorner после завершения перетаскивания
            adornerLayer.Remove(adorner);

        }
    }
}
