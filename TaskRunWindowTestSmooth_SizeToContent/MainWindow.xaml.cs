using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Runtime.InteropServices;
using static GiveFeedbackTest.DragDropHintWindow;

namespace GiveFeedbackTest
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        volatile bool isMoving = false;
        public DragDropHintWindow dragDropper;
        public MainWindow()
        {
            InitializeComponent();
            dragDropper = new DragDropHintWindow();
            TextBox_BgColor.Text = ((SolidColorBrush)dragDropper.Border_Outer.Background).Color.ToString();
        }

        private void TextBlock_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            dragDropper.RefreshWindowLocation();
        }

        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
        }
        private void TextBlock_MouseMove(object sender, MouseEventArgs e)
        {
            // При нажатой ЛКМ и сдвигу указателя мыши начинается перетаскивание.
            // Дополнительно проверяем правую кнопку мыши, так как она используется для отмены. 
            // Без проверки начинается резкое мерцание окна с постоянным возникновением и отменой событий Drag and drop.
            if (Mouse.LeftButton == MouseButtonState.Pressed && Mouse.RightButton == MouseButtonState.Released)
            {
                IHintAnimation ha;
                if ((bool)CheckBox_AnimationMode.IsChecked)
                    ha = new HintSmoothAnimation();
                else ha = new HintSimpleAnimation();

                try { dragDropper.Border_Outer.Background = new SolidColorBrush(ParseHexColor(TextBox_BgColor.Text)); }
                catch (Exception) { } 

                string fileName = TextBox_DroppableElementName.Text.Trim('\"');
                dragDropper.ProcessCopyDragDrop(
                    fileName,
                    System.IO.Path.GetFileNameWithoutExtension(fileName),
                    (bool)CheckBox_LabelEnabled.IsChecked,
                    (bool)CheckBox_PictureEnabled.IsChecked,
                    (bool)CheckBox_HideLabelIfPictureFound.IsChecked,
                    ha,
                    (bool)CheckBox_Direction.IsChecked ? HintTextLocation.Top : HintTextLocation.Bottom);
            }
        }
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            System.Environment.Exit(0);
        }
        /*
         * По умолчанию приложение WPF выполняется до тех пор, пока существует хотя бы одно открытое окно. 1
         * 
         * Чтобы изменить это поведение, можно установить свойство Application.ShutdownMode. 1 Оно может принимать одно из трёх значений:
         * 
         * OnMainWindowClose. Приложение остаётся активным только пока открыто главное окно. 1
         * OnLastWindowClose. Приложение не завершается (даже если все окна закрыты), пока не будет вызван метод Application.Shutdown(). 1
         * Независимо от того, какой способ останова используется, всегда есть возможность с помощью метода Application.Shutdown() немедленно завершить приложение.
         */
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            dragDropper.Close();
        }
        private static byte StrToByte(string s)
        {
            return byte.Parse(s, System.Globalization.NumberStyles.HexNumber);
        }
        public static Color ParseHexColor(string hc)
        {
            string hexColor = hc.Trim('#');
            byte A; byte R; byte G; byte B;

            try
            {
                if (hexColor.Length == 6)
                {
                    R = StrToByte(hexColor.Substring(0, 2));
                    G = StrToByte(hexColor.Substring(2, 2));
                    B = StrToByte(hexColor.Substring(4, 2));
                    return Color.FromRgb(R, G, B);
                }
                else if (hexColor.Length == 8)
                {
                    A = StrToByte(hexColor.Substring(0, 2));
                    R = StrToByte(hexColor.Substring(2, 2));
                    G = StrToByte(hexColor.Substring(4, 2));
                    B = StrToByte(hexColor.Substring(6, 2));
                    return Color.FromArgb(A, R, G, B);
                }
                else throw new FormatException("Некорректный формат цвета!");
            }
            catch (Exception ex)
            {
                throw new FormatException("Некорректный формат цвета! \n\n" + ex.Message);
            }

        }
    }
}