using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MovingPopup
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OnStart(sender, e);
        }
        Window window = new Window()
        {
            Topmost = true,
            WindowState = WindowState.Maximized,
            AllowsTransparency = true,
            WindowStyle = WindowStyle.None,
            Background = Brushes.Transparent,
            Content = new Canvas()
        };
        Rectangle rectangle = new Rectangle() { Width = 200, Height = 200, Fill = Brushes.Green };

        private async void OnStart(object sender, RoutedEventArgs e)
        {
            ((Canvas)window.Content).Children.Add(rectangle);

            window.Show();

            DoubleAnimation doubleAnimation = new DoubleAnimation(0, 1000, new Duration(TimeSpan.FromSeconds(5)))
            {
                AutoReverse = true,
                RepeatBehavior = RepeatBehavior.Forever
            };
            rectangle.BeginAnimation(Canvas.LeftProperty, doubleAnimation);
            rectangle.BeginAnimation(Canvas.BottomProperty, doubleAnimation);
        }

    }
}
