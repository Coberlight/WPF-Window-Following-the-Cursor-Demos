using FilePreviewAdornerTest.Models;
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

namespace FilePreviewAdornerTest
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

        private void TextBox_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            FileInfo fileInfo = new FileInfo(@"D:\MainData\Desktop\Fruity Reeverb 2.png");
            string[] files = { fileInfo.FullName };
            var data = new DataObject(DataFormats.FileDrop, files);
            data.SetData(DataFormats.Text, files[0]);

            AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(Application.Current.MainWindow);
            if (adornerLayer != null)
            {
                adornerLayer.Add(new FilePreviewAdorner(Application.Current.MainWindow, fileInfo));
            }

            DragDrop.DoDragDrop(this, data, DragDropEffects.Copy);

            if (adornerLayer != null)
            {
                adornerLayer.Remove(adornerLayer.GetAdorners(Application.Current.MainWindow)[0]);
            }
        }
    }
}
