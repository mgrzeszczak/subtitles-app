using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SubtitlesApp
{
    public partial class MainWindow : Window
    {
        private ISubtitleHandler handler;
        private bool started=true,changeSlider=false;

        public MainWindow()
        {
            InitializeComponent();
            Topmost = true;
            ResizeMode = ResizeMode.NoResize;
            Left = 0;
            Width = System.Windows.SystemParameters.PrimaryScreenWidth;
            Top = System.Windows.SystemParameters.PrimaryScreenHeight-Height;
            ShowInTaskbar = false;
            handler = new SubtitleHandler();
            handler.RegisterToEvents(UpdateCaption);
            this.Loaded += OnLoaded;
        }
        
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void Window_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                handler.Load(files[0]);
                toggleButton.Content = "Stop";
            }
        }

        private void UpdateCaption(CaptionEvent args)
        {
            text.Text = args.Caption==null? "" : args.Caption.Content;
            label.Content = args.Time.ToString("hh\\:mm\\:ss");
            changeSlider = true;
            slider.Value = 100 * args.Progress;
            changeSlider = false;
        }

        private void Window_MouseEnter(object sender, MouseEventArgs e)
        {
            ToggleView(true);
        }

        private void Window_MouseLeave(object sender, MouseEventArgs e)
        {
            ToggleView(false);
        }

        public void ToggleView(bool b)
        {
            byte alpha = b ? (byte)100 : (byte)1;
            Background = new SolidColorBrush(Color.FromArgb(alpha, 0, 0, 0));
            slider.Visibility = b ? Visibility.Visible : Visibility.Hidden;
            toggleButton.Visibility = b ? Visibility.Visible : Visibility.Hidden;
            exitButton.Visibility = b ? Visibility.Visible : Visibility.Hidden;
            label.Visibility = b ? Visibility.Visible : Visibility.Hidden;
        }

        private void toggleButton_Click(object sender, RoutedEventArgs e)
        {
            started = !started;
            if (started)
            {
                handler.Start();
                toggleButton.Content = "Stop";
            }
            else
            {
                handler.Stop();
                toggleButton.Content = "Start";
            }
        }

        private void exitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();   
        }

        private void Window_DragOver(object sender, DragEventArgs e)
        {
            ToggleView(true);
        }

        private void Window_DragLeave(object sender, DragEventArgs e)
        {
            ToggleView(false);
        }

        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (changeSlider) return;
            handler.UpdateProgress(slider.Value / slider.Maximum);
        }

        private const int WM_MOVING = 0x0216;

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        private int _left;
        private int _width;
        

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            _left = (int)this.Left;
            _width = (int)this.Width;
            var handle = new WindowInteropHelper(this).Handle;
            var source = HwndSource.FromHwnd(handle);
            source.AddHook(new HwndSourceHook(WndProc));
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_MOVING)
            {
                var position = Marshal.PtrToStructure<RECT>(lParam);
                position.left = _left;
                position.right = position.left + _width;
                Marshal.StructureToPtr(position, lParam, true);
            }
            return IntPtr.Zero;
        }
    }
}
