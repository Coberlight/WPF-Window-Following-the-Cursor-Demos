using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace GiveFeedbackTest
{
    /// <summary>
    /// Логика взаимодействия для DragDropHintBaseWindow.xaml
    /// </summary>
    public partial class DragDropHintBaseWindow : Window
    {
        bool isMoving;
        IHintAnimation hintAnimation { get; set; }
        private Matrix TransformFactors;
        public DragDropHintBaseWindow()
        {
            InitializeComponent();
            this.GiveFeedback += OnGiveFeedback;
            
        }
        public void ProcessCopyDragDrop(string fileName, bool showText, bool showPicture, IHintAnimation hintAnimation)
        {
            // Изменение ссылки на объект анимации
            this.hintAnimation = hintAnimation;

            // Показать окно
            this.Show();

            // Подготовка строки перед созданием объекта FileInfo
            if (!File.Exists(fileName)) return;

            // Создание FileInfo и подготовка данных для перетаскивания
            FileInfo fileInfo = new FileInfo(fileName);
            string[] files = { fileInfo.FullName };
            var data = new DataObject(DataFormats.FileDrop, files);
            data.SetData(DataFormats.Text, files[0]);

            // Показать всплывающее окно, установить флажок перетаскивания, настроить свойства
            if (myPopup2.IsOpen == false) myPopup2.IsOpen = true;
            isMoving = true;
            myPopup2.Placement = System.Windows.Controls.Primitives.PlacementMode.Absolute;
            myPopupText.Text = System.IO.Path.GetFileNameWithoutExtension(fileName);
            if (showText) PopupStackPanel.Visibility = Visibility.Visible; else PopupStackPanel.Visibility = Visibility.Collapsed;
            if (showPicture) myPopupText.Visibility = Visibility.Visible; else myPopupText.Visibility = Visibility.Collapsed;

            // Получение и назначение картинок
            string[] picturePaths = GetAssociatedPictureFilesPaths(fileName);
            if (picturePaths.Length > 0)
            {
                myPopupImage.Visibility = Visibility.Visible;
                Uri pictureUri = new Uri(picturePaths[0]);
                myPopupImage.Source = new BitmapImage(pictureUri);
            }
            else
            {
                myPopupImage.Visibility = Visibility.Hidden;
            }

            // Установка ширины попапа
            double mainWidth = CalculateAndConfigurePopupWidth();
            PopupStackPanel.Width = mainWidth;
            myPopup2.Width = mainWidth;

            // Получить позицию указателя мыши и инициализировать анимацию
            Point mousePos = CursorHelper.GetCursorPosition();

            this.hintAnimation.Init(mousePos.X, mousePos.Y, -mainWidth / 4, 30);
            this.hintAnimation.SetSpeed(0.13);

            // Установка прозрачности
            IntPtr hwnd = new WindowInteropHelper(this).Handle;
            WindowsServices.SetWindowExTransparent(hwnd);
            //hwnd = ((System.Windows.Interop.HwndSource)PresentationSource.FromVisual(myPopup2)).Handle;
            //hwnd = new WindowInteropHelper(myPopup2).Handle;
            //WindowsServices.SetWindowExTransparent(hwnd);
            Window parentWindow = Application.Current.MainWindow;
            IntPtr handle = new WindowInteropHelper(parentWindow).Handle;
            WindowsServices.SetWindowExTransparent(handle);

            // Запуск таски с перетаскиванием (дополнительно к GiveFeedback)
            Task.Run(() =>
            {
                try
                {
                    Debug.WriteLine("begin drag. File: " + fileName);
                    while (isMoving)
                    {
                        Dispatcher.Invoke(() => MovePopup());
                        Thread.Sleep(10);
                    }
                }
                catch (System.Threading.Tasks.TaskCanceledException) { }
                Debug.WriteLine("end drag");
            });
            TransformFactors = PresentationSource.FromVisual(this).CompositionTarget.TransformToDevice;

            // Вызов перетаскивания (метод вызывает прерывание)
            DragDrop.DoDragDrop(this, data, DragDropEffects.Copy);

            //MessageBox.Show("myPopupImage.ActualWidth = " + myPopupImage.ActualWidth + "\ntextRect w " + textRect.Width + "\n textRect h " + textRect.Height);

            // Скрытие попапа и окна, остановка анимации
            myPopup2.IsOpen = false;
            isMoving = false;
            this.Hide();
        }

        private double CalculateAndConfigurePopupWidth()
        {
            // Получение размеров надписи
            Rect startPointerRect = myPopupText.ContentStart.GetCharacterRect(LogicalDirection.Backward);
            Rect endPointerRect = myPopupText.ContentEnd.GetCharacterRect(LogicalDirection.Forward);
            Rect textRect = Rect.Union(startPointerRect, endPointerRect);

            // Установка ширины попапа
            PopupStackPanel.Width = PopupStackPanel.MaxWidth;
            double mainWidth;
            if (textRect.Width > myPopupImage.ActualWidth)
            {
                mainWidth = textRect.Width;
            }
            else
            {
                mainWidth = myPopupImage.ActualWidth;
            }
            return mainWidth;
        }

        private void SetPopupCoordinates(double x, double y, double dpiX, double dpiY)
        {
            myPopup2.HorizontalOffset = x / dpiX;
            myPopup2.VerticalOffset = y / dpiY;
        }
        public void MovePopup()
        {
            Point mousePos = CursorHelper.GetCursorPosition();

            if (hintAnimation != null)
            {
                Point targetPoint = hintAnimation.GetAnimationFrame(mousePos);
                SetPopupCoordinates(targetPoint.X, targetPoint.Y, TransformFactors.M11, TransformFactors.M22);
                //SetPopupCoordinatesLowLevel(targetPoint.X - myPopup2.Width / 2, targetPoint.Y + 100);
            }
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
        private void OnGiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            // обновление позиции курсора
            this.Dispatcher.Invoke(() => { }, DispatcherPriority.Render);
            MovePopup();

            // задание свойств курсора
            e.UseDefaultCursors = true; // При false можно устанавливать свои курсоры с помощью Mouse.SetCursor(Cursors.ТипКурсора);
            e.Handled = true;
        }
    }
}
