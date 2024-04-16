using System;
using System.Collections.Generic;
using System.IO;
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

namespace Walker
{
	public partial class MainWindow : Window
	{
        private long millisecondsLast;
        public MainWindow ()
		{
			InitializeComponent ();
		}

		private void mouseMove ( object sender, MouseEventArgs e )
		{
			if ( Mouse.Captured == sender )
			{
				FrameworkElement elem = sender as FrameworkElement;
				Point pos = Mouse.GetPosition ( elem );
				Point screen = elem.PointToScreen ( pos );
				tb.Text = $"{screen.X}:{screen.Y}";
				pop.HorizontalOffset = screen.X- (pop.Child as FrameworkElement).ActualWidth / 2;
				pop.VerticalOffset = screen.Y;
				if ( !pop.IsOpen )
					pop.IsOpen = true;
			}
		}

		private async void mouseDown ( object sender, MouseButtonEventArgs e )
		{
			FrameworkElement elem = e.OriginalSource as FrameworkElement;
			if ( elem != sender )
			{
				RenderTargetBitmap rbmp = new RenderTargetBitmap ( ( int ) elem.ActualWidth, ( int ) elem.ActualHeight, 96, 96, PixelFormats.Pbgra32 );
				rbmp.Render ( elem );
				content.Source = rbmp;
                await Task.Run(() =>
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        FileInfo fileInfo = new FileInfo(@"E:\testfile.txt");
                        string[] files = { fileInfo.FullName };
                        var data = new System.Windows.DataObject(System.Windows.DataFormats.FileDrop, files);
                        data.SetData(System.Windows.DataFormats.Text, files[0]);

                        DragDrop.DoDragDrop(this, data, System.Windows.DragDropEffects.Copy);
                    });
                });
                //Mouse.Capture ( sender as IInputElement );
            }
		}

		private void mouseUp ( object sender, MouseButtonEventArgs e )
		{
			if ( Mouse.Captured == sender )
			{
				Mouse.Capture ( null );
				pop.IsOpen = false;
				tb.Text = "";
			}
		}

		private void DragDropOperation()
		{
            FileInfo fileInfo = new FileInfo(@"E:\testfile.txt");
            string[] files = { fileInfo.FullName };
            var data = new System.Windows.DataObject(System.Windows.DataFormats.FileDrop, files);
            data.SetData(System.Windows.DataFormats.Text, files[0]);

            DragDrop.DoDragDrop(this, data, System.Windows.DragDropEffects.Copy);
        }
	}
}
