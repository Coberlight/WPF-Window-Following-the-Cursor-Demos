using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Threading;

namespace GiveFeedbackTest
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Window1 MovingWindow;
        volatile bool isMoving = false;
        public MainWindow()
        {
            InitializeComponent();
            MovingWindow = new Window1();
            MovingWindow.Hide();
        }

        private void TextBlock_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            Debug.WriteLine("feedback given");
            if (MovingWindow != null)
            {
                Point mousePos = CursorHelper.GetCursorPosition();
                MovingWindow.Left = mousePos.X;
                MovingWindow.Top = mousePos.Y + 10;
            }
        }

        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            FileInfo fileInfo = new FileInfo(@"D:\MainData\Desktop\Fruity Reeverb 2.png");
            string[] files = { fileInfo.FullName };
            var data = new DataObject(DataFormats.FileDrop, files);
            data.SetData(DataFormats.Text, files[0]);
            isMoving = true;
            
            Task.Run(() =>
            {
                while (isMoving)
                {
                    Dispatcher.Invoke(() => moveWindow());
                    Thread.Sleep(10);
                }
            });

            MovingWindow.Show();

            DragDrop.DoDragDrop(this, data, DragDropEffects.Copy);
            isMoving = false;
            MovingWindow.Hide();
        }

        private void moveWindow() {
            
            {
                Point mousePos = CursorHelper.GetCursorPosition();
                MovingWindow.Left = mousePos.X;
                MovingWindow.Top = mousePos.Y + 100;
                
            }
        }
    }
}
