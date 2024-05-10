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
        private volatile bool isMoving;
        IHintAnimation hintAnimation { get; set; }
        private Matrix TransformFactors;
        private DateTime lastShowTime;
        public DragDropHintBaseWindow()
        {
            InitializeComponent();
            this.GiveFeedback += OnGiveFeedback;

        }
        public void ProcessCopyDragDrop(string fileName, bool showText, bool showPicture, IHintAnimation hintAnimation)
        {
            // Изменение ссылки на объект анимации
            this.hintAnimation = hintAnimation;

            double fps = ScreenData.GetFPS();

            // Подготовка строки перед созданием объекта FileInfo
            if (!File.Exists(fileName)) return;

            // Создание FileInfo и подготовка данных для перетаскивания
            FileInfo fileInfo = new FileInfo(fileName);
            string[] files = { fileInfo.FullName };
            var data = new DataObject(DataFormats.FileDrop, files);
            data.SetData(DataFormats.Text, files[0]);

            // Показать всплывающее окно, настроить свойства
            TextBlock_Main.Text = System.IO.Path.GetFileNameWithoutExtension(fileName);
            if (showText) StackPanel_Main.Visibility = Visibility.Visible; else StackPanel_Main.Visibility = Visibility.Collapsed;
            if (showPicture) TextBlock_Main.Visibility = Visibility.Visible; else TextBlock_Main.Visibility = Visibility.Collapsed;

            // Получение и назначение картинок
            string[] picturePaths = GetAssociatedPictureFilesPaths(fileName);
            if (picturePaths.Length > 0)
            {
                Image_Main.Visibility = Visibility.Visible;
                Uri pictureUri = new Uri(picturePaths[0]);
                Image_Main.Source = new BitmapImage(pictureUri);
            }
            else
            {
                Image_Main.Visibility = Visibility.Hidden;
            }

            // Установка ширины окна
            double mainWidth = CalculateAndConfigureWindowWidth();
            StackPanel_Main.Width = mainWidth;
            this.Width = mainWidth;

            // Получить позицию указателя мыши и инициализировать анимацию
            Point mousePos = CursorHelper.GetCursorPosition();

            this.hintAnimation.Init(mousePos.X, mousePos.Y, -mainWidth / 4, 30);
            this.hintAnimation.SetSpeed(0.13);

            // Установка прозрачности для событий мыши
            IntPtr hwnd = new WindowInteropHelper(this).Handle;
            WindowsServices.SetWindowExTransparent(hwnd);

            double showHideTime = 100;

            // Установить флажок перетаскивания, запуск таски с перемещением окна (дополнительно к GiveFeedback)
            this.Opacity = 0;
            isMoving = true;
            Task.Run(() =>
            {
                try
                {
                    Debug.WriteLine("begin drag");
                    ShowWindowWithOpacityAnimation(showHideTime);
                    //Dispatcher.Invoke(() => Show());
                    while (isMoving)
                    {
                        Dispatcher.Invoke(() => { 
                            RefreshWindowLocation();
                            InvalidateVisual();
                            UpdateLayout();
                            
                        }, DispatcherPriority.Render);
                        Thread.Sleep((int)(fps / 2));
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
            HideWindowWithOpacityAnimation(showHideTime);
        }

        private double CalculateAndConfigureWindowWidth()
        {
            // Получение размеров надписи
            Rect startPointerRect = TextBlock_Main.ContentStart.GetCharacterRect(LogicalDirection.Backward);
            Rect endPointerRect = TextBlock_Main.ContentEnd.GetCharacterRect(LogicalDirection.Forward);
            Rect textRect = Rect.Union(startPointerRect, endPointerRect);

            // Установка ширины попапа
            StackPanel_Main.Width = StackPanel_Main.MaxWidth;
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

        private void SetPopupCoordinates(Popup p, double x, double y, double dpiX, double dpiY)
        {
            p.HorizontalOffset = x / dpiX;
            p.VerticalOffset = y / dpiY;
        }
        public void RefreshWindowLocation()
        {
            Point mousePos = CursorHelper.GetCursorPosition();

            if (hintAnimation != null)
            {
                Point targetPoint = hintAnimation.GetAnimationFrame(mousePos);
                WindowsServices.SetWindowPosition(new WindowInteropHelper(this).Handle, (int)targetPoint.X, (int)targetPoint.Y);
                //SetPopupCoordinates(targetPoint.X, targetPoint.Y, TransformFactors.M11, TransformFactors.M22);
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
            //this.InvalidateVisual();
            //this.UpdateLayout();
            //StackPanel_Main.InvalidateVisual();
            //StackPanel_Main.UpdateLayout();
            //TextBlock_Main.InvalidateVisual();
            //TextBlock_Main.UpdateLayout();
            //Image_Main.InvalidateVisual();
            //Image_Main.UpdateLayout();
            this.Dispatcher.Invoke(() => { }, DispatcherPriority.Render);
            RefreshWindowLocation();
            //if (this.Visibility == Visibility.Hidden || this.Visibility == Visibility.Collapsed)
            //{
            //    if (DateTime.Now - lastShowTime > TimeSpan.FromMilliseconds(1000))
            //    {
            //        this.Show();
            //        this.Dispatcher.Invoke(() => { }, DispatcherPriority.Render);
            //    }

            //}
            e.UseDefaultCursors = true; // При false можно устанавливать свои курсоры с помощью Mouse.SetCursor(Cursors.ТипКурсора);
            e.Handled = true;
        }
        private DateTime lastAnimationTime;
        private void ShowWindowWithOpacityAnimation(double millis)
        {
            Task.Run(() =>
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
                        //break;
                        return;
                    }
                } while (diff < endTime);
                this.Dispatcher.Invoke(() => this.Opacity = 1);
            });
        }
        private void HideWindowWithOpacityAnimation(double millis)
        {
            Task.Run(() =>
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
                    if (isMoving)
                    {
                        //break;
                        return;
                    }
                } while (diff < endTime);
                this.Dispatcher.Invoke(() =>
                {
                    this.Opacity = 0;
                    this.Hide();
                });
            });
        }
    }
}
