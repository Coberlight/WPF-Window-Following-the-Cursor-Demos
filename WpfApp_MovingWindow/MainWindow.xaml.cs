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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using System.Windows.Interop;
using System.IO;

namespace WpfApp_MovingWindow
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MovingWindow movingWindow;
        private volatile bool debug;
        public MainWindow()
        {
            InitializeComponent();
            movingWindow = new MovingWindow();
        }

        private void button_Start_Click(object sender, RoutedEventArgs e)
        {
            movingWindow.isFollowingCursor = true;
            Thread movingWindowThread = new Thread(movingWindow.FollowCursor);
            movingWindowThread.Start();
            if (debug)
            {
                Thread movingWindowThread1 = new Thread(followCursorDebug);
                movingWindowThread1.Start();
            }
            movingWindow.Show();
            label_Status.Content = "Status: Following";
        }

        private void followCursorDebug()
        {
            while(movingWindow.isFollowingCursor)
            {
                this.Dispatcher.Invoke(() =>
                {
                    label_WindowInfo.Content = movingWindow.windowInfo;
                });
            }
        }

        private void button_Stop_Click(object sender, RoutedEventArgs e)
        {
            movingWindow.isFollowingCursor = false;
            movingWindow.Hide();
            label_Status.Content = "Status: Hidden";
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            debug = (bool)checkBox_DebugEnabled.IsChecked;
        }

        private void DockPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            
            FileInfo fileInfo = new FileInfo(@"E:\testfile.txt");
            string[] files = { fileInfo.FullName };
            var data = new DataObject(DataFormats.FileDrop, files);
            data.SetData(DataFormats.Text, files[0]);

            DragDrop.DoDragDrop(this, data, DragDropEffects.Copy);

        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
        }

        private void button_ShowFPS_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(ScreenData.GetFPS().ToString());
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            debug = (bool)checkBox_DebugEnabled.IsChecked;
        }

        private void button_GeneralExit_Click(object sender, RoutedEventArgs e)
        {
            System.Environment.Exit(0);
        }
    }
}
