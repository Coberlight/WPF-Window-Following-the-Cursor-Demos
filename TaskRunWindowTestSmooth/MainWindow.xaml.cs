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
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Windows.Controls.Primitives;
using System.Xml.Linq;
//using Windows.Foundation;

namespace GiveFeedbackTest
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        volatile bool isMoving = false;
        public DragDropHintBaseWindow dragDropper;
        public MainWindow()
        {
            InitializeComponent();
            dragDropper = new DragDropHintBaseWindow();
            GiveFeedback += OnGiveFeedback;
        }

        private void TextBlock_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            dragDropper.RefreshWindowLocation();
        }

        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //ProcessDragAndDropAnimation();
        }
        private void TextBlock_MouseMove(object sender, MouseEventArgs e)
        {
            // При нажатой ЛКМ и сдвигу указателя мыши начинается перетаскивание.
            // Дополнительно проверяем правую кнопку мыши, так как она используется для отмены. 
            // Без проверки начинается резкое мерцание окна с постоянным возникновением и отменой событий Drag and drop.
            if (Mouse.LeftButton == MouseButtonState.Pressed && Mouse.RightButton == MouseButtonState.Released) 
            {
                dragDropper.ProcessCopyDragDrop(TextBox_DroppableElementName.Text.Trim('\"'), true, true, new HintSmoothAnimation());
            }
        }
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        private void OnGiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            
        }

        private void LoadSource1_Click(object sender, RoutedEventArgs e)
        {
            TextBox_DroppableElementName.Text = TextBox_file1.Text;
        }

        private void LoadSource2_Click(object sender, RoutedEventArgs e)
        {
            TextBox_DroppableElementName.Text = TextBox_file2.Text;
        }

        private void LoadSource3_Click(object sender, RoutedEventArgs e)
        {
            TextBox_DroppableElementName.Text = TextBox_file3.Text;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
