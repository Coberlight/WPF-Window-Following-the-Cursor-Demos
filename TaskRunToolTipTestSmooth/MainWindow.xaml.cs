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
//using Windows.Foundation;

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
            hsa = new HintSmoothAnimation();


        }

        private void TextBlock_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {

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
                ProcessDragAndDropWithAnimation();
            }
        }

        private void ProcessDragAndDropWithAnimation()
        {
            // Подготовка строки перед созданием объекта FileInfo
            string filename = TextBox_DroppableElementName.Text.Trim('\"');
            if (!File.Exists(filename)) return;

            // Создание FileInfo и подготовка данных для перетаскивания
            FileInfo fileInfo = new FileInfo(filename);
            string[] files = { fileInfo.FullName };
            var data = new DataObject(DataFormats.FileDrop, files);
            data.SetData(DataFormats.Text, files[0]);
            
            // Показать всплывающее окно, установить флажок перетаскивания, настроить свойства
            if (myPopup2.IsOpen == false) myPopup2.IsOpen = true;
            isMoving = true;
            myPopup2.Placement = System.Windows.Controls.Primitives.PlacementMode.Absolute;
            myPopupText.Text = System.IO.Path.GetFileNameWithoutExtension(filename);

            // Установка прозрачности
            //IntPtr hWnd = ((System.Windows.Interop.HwndSource)PresentationSource.FromVisual(myPopup2)).Handle;
            //int windowLong = GetWindowLong(hWnd, -20); // -20 = GWL_EXSTYLE
            //windowLong |= 0x80; // 0x80 = WS_EX_TRANSPARENT
            ////SetWindowLong(hWnd, -20, windowLong);
            //IntPtr hWnd = ((System.Windows.Interop.HwndSource)PresentationSource.FromVisual(myPopup2)).Handle;
            IntPtr hwnd = new WindowInteropHelper(this).Handle;
            WindowsServices.SetWindowExTransparent(hwnd);

            // Получение и назначение картинок
            string[] picturePaths = GetAssociatedPictureFilesPaths(filename);
            if (picturePaths.Length > 0)
            {
                myPopupImage.Visibility = Visibility.Visible;
                Uri pictureUri = new Uri(picturePaths[0]);
                myPopupImage.Source = new BitmapImage(pictureUri);
            } else
            {
                myPopupImage.Visibility = Visibility.Hidden;
            }

            // Запуск таски с перетаскиванием (дополнительно к GiveFeedback)
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
            TransformFactors = PresentationSource.FromVisual(this).CompositionTarget.TransformToDevice;

            // Получить позицию указателя мыши и инициализировать анимацию
            Point mousePos = CursorHelper.GetCursorPosition();

            // Получение размеров надписи
            Rect startPointerRect = myPopupText.ContentStart.GetCharacterRect(LogicalDirection.Backward);
            Rect endPointerRect = myPopupText.ContentEnd.GetCharacterRect(LogicalDirection.Forward);
            Rect textRect = Rect.Union(startPointerRect, endPointerRect);

            // Установка ширины попапа
            PopupStackPanel.Width = PopupStackPanel.MaxWidth;
            double mainWidth;
            if (textRect.Width > myPopupImage.ActualWidth) {
                mainWidth = textRect.Width;
            } else {
                mainWidth = myPopupImage.ActualWidth;
            }
            PopupStackPanel.Width = mainWidth;
            myPopup2.Width = mainWidth;

            hsa.Init(mousePos.X, mousePos.Y, -mainWidth / 4, 30, 0.13);
            this.Hide();

            // Вызов перетаскивания (метод вызывает прерывание)
            DragDrop.DoDragDrop(this, data, DragDropEffects.Copy);

            //MessageBox.Show("myPopupImage.ActualWidth = " + myPopupImage.ActualWidth + "\ntextRect w " + textRect.Width + "\n textRect h " + textRect.Height);

            // Установка непрозрачности
            WindowsServices.SetWindowExTransparent(hwnd);

            // Скрытие попапа, показывание окна
            myPopup2.IsOpen = false;
            this.Show();
            this.Focus();
        }
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        private void OnGiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            MovePopup();
            e.UseDefaultCursors = true; // При false можно устанавливать свои курсоры с помощью Mouse.SetCursor(Cursors.ТипКурсора);
            e.Handled = true;
        }
        private Matrix TransformFactors;
        private void MovePopup()
        {
            Point mousePos = CursorHelper.GetCursorPosition();
            
            if (hsa != null)
            {
                Point targetPoint = hsa.GetFollowingAnimationFrame(mousePos);
                SetPopupCoordinates(targetPoint.X, targetPoint.Y, TransformFactors.M11, TransformFactors.M22);
                //SetPopupCoordinatesLowLevel(targetPoint.X - myPopup2.Width / 2, targetPoint.Y + 100);
            }
        }
        private void SetPopupCoordinates(double x, double y, double dpiX, double dpiY)
        {
            myPopup2.HorizontalOffset = x / dpiX;
            myPopup2.VerticalOffset = y / dpiY;
        }
        public static string[] GetAssociatedPictureFilesPaths(string path)
        {
            string[] formats = new string[5] { ".png", ".jpeg", ".jpg", ".bmp", ".gif" };
            List<string> picturePaths = new List<string>();
            string picturePath = null;

            string elementDestinationBasePart = System.IO.Path.Combine(
                System.IO.Path.GetDirectoryName(path),
                System.IO.Path.GetFileNameWithoutExtension(path));

            for (int i = 0; i < formats.Length; i++)
            {
                picturePath = elementDestinationBasePart + formats[i];
                if (File.Exists(picturePath))
                {
                    picturePaths.Add(picturePath);
                }
            }
            return picturePaths.ToArray();
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
    }
}
