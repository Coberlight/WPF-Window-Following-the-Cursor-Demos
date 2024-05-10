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

namespace ToolTips
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
            //TextBlock textBlock = new TextBlock();
            //textBlock.Text = "Some text in ToolTip";

            //StackPanel stackPanel = new StackPanel();
            //stackPanel.Children.Add(textBlock);

            //ToolTip tt = new ToolTip();
            //tt.Content = stackPanel;
            //tt.Visibility = Visibility.Visible;
        }
    }
}
