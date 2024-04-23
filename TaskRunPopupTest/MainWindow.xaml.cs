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
        }

        private void TextBlock_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {

        }

        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            FileInfo fileInfo = new FileInfo(@"D:\MainData\Desktop\Fruity Reeverb 2.png");
            string[] files = { fileInfo.FullName };
            var data = new DataObject(DataFormats.FileDrop, files);
            data.SetData(DataFormats.Text, files[0]);
            isMoving = true;
            myPopup2.Placement = System.Windows.Controls.Primitives.PlacementMode.Absolute;

            Task.Run(() =>
            {
                while (isMoving)
                {
                    Dispatcher.Invoke(() => movePopup());
                    Thread.Sleep(10);
                }
            });

            myPopup2.IsOpen = true;

            DragDrop.DoDragDrop(this, data, DragDropEffects.Copy);
            myPopup2.IsOpen = false;
        }

        private void moveWindow()
        {
            Point mousePos = CursorHelper.GetCursorPosition();
            MovingWindow.Left = mousePos.X;
            MovingWindow.Top = mousePos.Y + 100;
        }
        private void movePopup()
        {
            Point mousePos = CursorHelper.GetCursorPosition();
            myPopup2.HorizontalOffset = mousePos.X;
            myPopup2.VerticalOffset = mousePos.Y + 100;
        }
    }
}
