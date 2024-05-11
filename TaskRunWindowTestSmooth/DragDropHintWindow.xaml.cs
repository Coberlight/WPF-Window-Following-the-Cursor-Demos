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
    public partial class DragDropHintWindow : Window
    {
        private volatile bool isMoving;
        private IHintAnimation hintAnimation;
        private Matrix TransformFactors;
        private DateTime lastShowTime;
        private DateTime lastAnimationTime;
        public DragDropHintWindow()
        {
            InitializeComponent();
            this.GiveFeedback += OnGiveFeedback;

            // в StackPanel_Main хранится максимальная ширина.
            // Чтобы установить желаемую ширину всего окна, нужно изменить именно максимальную ширину StackPanel_Main
            this.Width = StackPanel_Main.MaxWidth;
        }
        public void ProcessCopyDragDrop(string fileName, string desiredName, bool showText, bool showPicture, IHintAnimation hintAnimation, HintTextLocation textLocation)
        {
            // Проверка на существование файла
            if (!File.Exists(fileName)) return;

            // Создание FileInfo и подготовка данных для перетаскивания
            FileInfo fileInfo = new FileInfo(fileName);
            string[] files = { fileInfo.FullName };
            var data = new DataObject(DataFormats.FileDrop, files);
            data.SetData(DataFormats.Text, files[0]);

            // Настроить видимость надписи и изображения
            TextBlock_Main.Text = desiredName;
            if (showText) TextBlock_Main.Visibility = Visibility.Visible; else TextBlock_Main.Visibility = Visibility.Collapsed;
            if (showPicture) Image_Main.Visibility = Visibility.Visible; else Image_Main.Visibility = Visibility.Collapsed;

            // Настроить положение надписи и изображения
            StackPanel_Main.Children.Clear();
            switch (textLocation)
            {
                case HintTextLocation.Top:
                    {
                        StackPanel_Main.Children.Add(TextBlock_Main);
                        StackPanel_Main.Children.Add(Image_Main);
                    }
                    break;
                case HintTextLocation.Bottom:
                    {
                        StackPanel_Main.Children.Add(Image_Main);
                        StackPanel_Main.Children.Add(TextBlock_Main);
                    }
                    break;
            }

            // Получение и назначение картинок
            if (showPicture)
            {
                string[] picturePaths = GetAssociatedPictureFilesPaths(fileName);
                if (picturePaths.Length > 0)
                {
                    Image_Main.Visibility = Visibility.Visible;
                    Uri pictureUri = new Uri(picturePaths[0]);
                    Image_Main.Source = new BitmapImage(pictureUri);
                }
                else
                {
                    showPicture = false;
                    Image_Main.Visibility = Visibility.Collapsed;
                }
            }

            // Установка ширины окна и выравнивания текста
            double mainWidth;
            if (showPicture)
            {
                this.MaxWidth = StackPanel_Main.MaxWidth;
                this.Width = MaxWidth;
                mainWidth = CalculateWindowWidth();
                this.Width = mainWidth;
                TextBlock_Main.HorizontalAlignment = HorizontalAlignment.Center;
                StackPanel_Main.HorizontalAlignment = HorizontalAlignment.Center;
            }
            else
            {
                this.MaxWidth = 500;
                this.Width = this.MaxWidth;
                mainWidth = CalculateWindowWidth();
                TextBlock_Main.HorizontalAlignment = HorizontalAlignment.Left;
                StackPanel_Main.HorizontalAlignment = HorizontalAlignment.Left;
            }

            // Установка прозрачности для событий мыши
            IntPtr hwnd = new WindowInteropHelper(this).Handle;
            WindowsServices.SetWindowExTransparent(hwnd);

            // Получить позицию указателя мыши и инициализировать анимацию
            Point mousePos = CursorHelper.GetCursorPosition();
            this.hintAnimation = hintAnimation;
            this.hintAnimation.Init(mousePos.X, mousePos.Y, -mainWidth / 4, 30);
            this.hintAnimation.SetSpeed(0.13);
            double showHideTime = 100;

            // Получение частоты кадров и периода одного кадра
            double fps = ScreenData.GetFPS();
            double frameTime = (1.0 / fps * 1000.0);

            // Установить флажок перетаскивания, запуск задачи с анимацией (дополнительно к GiveFeedback)
            this.Opacity = 0;
            isMoving = true;
            Task.Run(() =>
            {
                try
                {
                    Debug.WriteLine("begin drag");
                    Task.Run(() => ShowWindowWithOpacityAnimation(showHideTime));
                    while (isMoving)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            RefreshWindowLocation();
                            InvalidateVisual();
                            UpdateLayout();

                        }, DispatcherPriority.Render);
                        Thread.Sleep((int)(frameTime / 2));
                    }
                }
                catch (System.Threading.Tasks.TaskCanceledException) { }
            });
            TransformFactors = PresentationSource.FromVisual(this).CompositionTarget.TransformToDevice;

            // Установка времени открытия для избегания начального кадра в неверном расположении окна
            lastShowTime = DateTime.Now;

            // Вызов перетаскивания (метод вызывает прерывание)
            DragDrop.DoDragDrop(this, data, DragDropEffects.Copy);

            // Скрытие окна, остановка анимации
            isMoving = false;
            Task.Run(() => HideWindowWithOpacityAnimation(showHideTime));
        }
        private double CalculateWindowWidth()
        {
            // Получение размеров надписи
            Rect startPointerRect = TextBlock_Main.ContentStart.GetCharacterRect(LogicalDirection.Backward);
            Rect endPointerRect = TextBlock_Main.ContentEnd.GetCharacterRect(LogicalDirection.Forward);
            Rect textRect = Rect.Union(startPointerRect, endPointerRect);

            // Установка ширины попапа
            double mainWidth;
            if (textRect.Width > Image_Main.ActualWidth)
            {
                mainWidth = textRect.Width;
            }
            else
            {
                mainWidth = Image_Main.ActualWidth;
            }
            return mainWidth;
        }
        public void RefreshWindowLocation()
        {
            Point mousePos = CursorHelper.GetCursorPosition();

            if (hintAnimation != null)
            {
                Point targetPoint = hintAnimation.GetAnimationFrame(mousePos);
                WindowsServices.SetWindowPosition(new WindowInteropHelper(this).Handle, (int)targetPoint.X, (int)targetPoint.Y);
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
            this.Dispatcher.Invoke(() => { }, DispatcherPriority.Render);
            RefreshWindowLocation();
            e.UseDefaultCursors = true; // При false можно устанавливать свои курсоры с помощью Mouse.SetCursor(Cursors.ТипКурсора);
            e.Handled = true;
        }
        private void ShowWindowWithOpacityAnimation(double millis)
        {
            double initOpacity = 0;
            Thread.Sleep((int)millis / 2);
            this.Dispatcher.Invoke(() =>
            {
                this.Show();
                initOpacity = this.Opacity;
            });
            TimeSpan diff;
            TimeSpan endTime = TimeSpan.FromMilliseconds(millis);
            lastAnimationTime = DateTime.Now;
            do
            {
                diff = DateTime.Now - lastAnimationTime;
                double opacityFac = initOpacity + diff.TotalMilliseconds / millis;

                if (opacityFac > 1) break;
                this.Dispatcher.Invoke(() => this.Opacity = opacityFac);
                if (!isMoving)
                {
                    return;
                }
            } while (diff < endTime);
            this.Dispatcher.Invoke(() => this.Opacity = 1);
        }
        private void HideWindowWithOpacityAnimation(double millis)
        {
            double initOpacity = 1;
            this.Dispatcher.Invoke(() => initOpacity = this.Opacity);
            TimeSpan diff;
            TimeSpan endTime = TimeSpan.FromMilliseconds(millis);
            lastAnimationTime = DateTime.Now;
            do
            {
                diff = DateTime.Now - lastAnimationTime;
                double opacityFac = initOpacity - diff.TotalMilliseconds / millis;

                if (opacityFac < 0) break;
                this.Dispatcher.Invoke(() => this.Opacity = opacityFac);
                if (isMoving) return;
            } while (diff < endTime);
            this.Dispatcher.Invoke(() =>
            {
                this.Opacity = 0;
                this.Hide();
            });
        }

        //MessageBox.Show($"Stack Panel: \n Actual Width: {StackPanel_Main.ActualWidth} \n Actual Height: {StackPanel_Main.ActualHeight} " +
        //        $"\nImage: \n Actual Width: {Image_Main.ActualWidth} \n Actual Height: {Image_Main.ActualHeight}" +
        //        $"\n Text Block: \n Actual Width: {TextBlock_Main.ActualWidth} \n Actual Height: {TextBlock_Main.ActualHeight}"
        //        );

        public enum HintTextLocation
        {
            Top, Bottom
        }
    }
}
