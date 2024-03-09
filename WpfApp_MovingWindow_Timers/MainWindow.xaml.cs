using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using System.Timers;
using System.Windows.Interop;
using System.IO;
using static System.Windows.Forms.AxHost;

namespace WpfApp_MovingWindow
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MovingWindow movingWindow;
        private volatile bool debugEnabled;
        private int screenRefreshPeriod;
        public MainWindow()
        {
            InitializeComponent();
            movingWindow = new MovingWindow();
            screenRefreshPeriod = (int)Math.Round(1.0 / ScreenData.GetFPS() * 1000);
        }

        private void button_Start_Click(object sender, RoutedEventArgs e)
        {
            movingWindow.isFollowingCursor = true;

            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Interval = screenRefreshPeriod / 2 ; // делим на 2 чтобы увеличить плавность анимации (хотя это не помогает :( )
            timer.Enabled = true;
            timer.Elapsed += new ElapsedEventHandler((o, ev) =>
            {
                timerElapsed(timer);
            });
            movingWindow.FollowCursorInit();
            movingWindow.Show();
            timer.Start();
            label_Status.Content = "Status: Following";
        }

        private void button_Stop_Click(object sender, RoutedEventArgs e)
        {
            movingWindow.isFollowingCursor = false;
            movingWindow.Hide();
            label_Status.Content = "Status: Hidden";
        }
        private void timerElapsed(System.Timers.Timer timer)
        {
            this.Dispatcher.Invoke(() =>
            {
                movingWindow.FollowCursor();
                if (!movingWindow.isFollowingCursor)
                {
                    timer.Stop();
                }
            });
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            debugEnabled = (bool)checkBox_DebugEnabled.IsChecked;
        }

        private void DockPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            
            FileInfo fileInfo = new FileInfo(@"E:\testfile.txt");
            string[] files = { fileInfo.FullName };
            var data = new System.Windows.DataObject(System.Windows.DataFormats.FileDrop, files);
            data.SetData(System.Windows.DataFormats.Text, files[0]);

            DragDrop.DoDragDrop(this, data, System.Windows.DragDropEffects.Copy);

        }

        private void Window_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
        }

        private void button_ShowFPS_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.MessageBox.Show(ScreenData.GetFPS().ToString());
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            debugEnabled = (bool)checkBox_DebugEnabled.IsChecked;
        }

        private void button_GeneralExit_Click(object sender, RoutedEventArgs e)
        {
            System.Environment.Exit(0);
        }
    }
}
