using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using YMplugins.ViewModels.VM;

namespace YMplugins.Views.Views
{
    /// <summary>
    /// Логика взаимодействия для AutoPrintView.xaml
    /// </summary>
    public partial class AutoPrintView : Window
    {
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        public AutoPrintView(AutoPrintVm autoPrintVm)
        {
            InitializeComponent();
            DataContext = autoPrintVm;

            // На время выбора объекта на экране окно прячется (модальный цикл
            // ShowDialog при этом продолжается), а затем снова показывается.
            autoPrintVm.CloseAction ??= Hide;
            autoPrintVm.OpenAction = ShowAndBringToFront;

            Loaded += On_Loaded;
        }

        /// <summary>
        ///     Возвращает уже открытое (скрытое) окно на экран и выводит его
        ///     поверх окна AutoCAD. Повторный ShowDialog вызывать нельзя —
        ///     окно уже показано модально, достаточно снять скрытие.
        /// </summary>
        private void ShowAndBringToFront()
        {
            Visibility = Visibility.Visible;

            if (WindowState == WindowState.Minimized)
                WindowState = WindowState.Normal;

            Activate();

            // Надёжно вытащить поверх окна AutoCAD (другого процесса переднего плана)
            Topmost = true;
            Topmost = false;

            var handle = new WindowInteropHelper(this).Handle;
            if (handle != IntPtr.Zero)
                SetForegroundWindow(handle);

            Focus();
        }

        private void On_Loaded(object sender, RoutedEventArgs e)
        {
            Style = (Style)FindResource("WindowElementStyle");
        }
    }
}
