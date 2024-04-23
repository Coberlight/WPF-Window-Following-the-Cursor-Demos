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
        volatile bool isMoving = false;
        public MainWindow()
        {
            InitializeComponent();
            GiveFeedback += OnGiveFeedback;
        }

        private void TextBlock_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {

        }

        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!File.Exists(TextBox_DroppableElementName.Text)) return;
            FileInfo fileInfo = new FileInfo(TextBox_DroppableElementName.Text);
            string[] files = { fileInfo.FullName };
            var data = new DataObject(DataFormats.FileDrop, files);
            data.SetData(DataFormats.Text, files[0]);
            isMoving = true;
            myPopup2.Placement = System.Windows.Controls.Primitives.PlacementMode.Absolute;

            //Task.Run(() =>
            //{
            //    while (isMoving)
            //    {
            //        Dispatcher.Invoke(() => MovePopup());
            //        Thread.Sleep(10);
            //    }
            //});

            myPopup2.IsOpen = true;

            DragDrop.DoDragDrop(this, data, DragDropEffects.Copy);
            myPopup2.IsOpen = false;
        }

        private void OnGiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            MovePopup();
            e.Handled = true;
        }

        private void MovePopup()
        {
            Point mousePos = CursorHelper.GetCursorPosition();
            myPopup2.HorizontalOffset = mousePos.X;
            myPopup2.VerticalOffset = mousePos.Y + 100;
        }
    }
}
