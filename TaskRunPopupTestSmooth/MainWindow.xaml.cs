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
        HintSmoothAnimation hsa { get; set; }
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
            string filename = TextBox_DroppableElementName.Text.Trim('\"');
            if (!File.Exists(filename)) return;
            FileInfo fileInfo = new FileInfo(filename);
            string[] files = { fileInfo.FullName };
            var data = new DataObject(DataFormats.FileDrop, files);
            data.SetData(DataFormats.Text, files[0]);

            if (myPopup2.IsOpen == false) myPopup2.IsOpen = true;

            // Эти методы не сработали в попытке ускорить появление всплывающего меню после начала перетаскивания
            //myPopup2.BeginInit();
            //myPopup2.EndInit();
            //myPopup2.BringIntoView();
            //myPopup2.Focus();
            //myPopup2.UpdateDefaultStyle();
            //myPopup2.UpdateLayout();

            isMoving = true;
            myPopup2.Placement = System.Windows.Controls.Primitives.PlacementMode.Absolute;

            myPopupText.Text = System.IO.Path.GetFileNameWithoutExtension(filename);

            

            Task.Run(() =>
            {
                try
                {
                    Debug.WriteLine("begin drag");
                    while (isMoving)
                    {
                        Dispatcher.Invoke(() => MovePopup());
                        Thread.Sleep(10);
                    }
                }
                catch (System.Threading.Tasks.TaskCanceledException) { }
            });

            hsa = new HintSmoothAnimation();
            Point mousePos = CursorHelper.GetCursorPosition();
            hsa.Init(mousePos.X, mousePos.Y, 0.13);
            this.Hide();

            DragDrop.DoDragDrop(this, data, DragDropEffects.Copy);

            myPopup2.IsOpen = false;
            hsa = null;
            this.Show();
            this.Focus();
        }

        private void OnGiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            MovePopup();
            e.UseDefaultCursors = true; // При false можно устанавливать свои курсоры с помощью Mouse.SetCursor(Cursors.ТипКурсора);
            e.Handled = true;
        }

        private void MovePopup()
        {
            Point mousePos = CursorHelper.GetCursorPosition();
            if (hsa != null)
            {
                Point targetPoint = hsa.FollowPoint(mousePos);
                myPopup2.HorizontalOffset = targetPoint.X - myPopup2.Width / 2;
                myPopup2.VerticalOffset = targetPoint.Y + 100;
                //double[] targetPoint = hsa.FollowPoint(mousePos);
                //myPopup2.HorizontalOffset = targetPoint[0];
                //myPopup2.VerticalOffset = targetPoint[1] + 100;
            }
        }
    }
}
