
using System.Windows;
using YMplugins.Contracts;
using YMplugins.Views.Views;

namespace YMplugins.Views.Services
{
    public class WindowService : IWindowService
    {
        private readonly Func<AutoPrintView> _viewFactory;
        private Window _loadingWindow;

        public WindowService(Func<AutoPrintView> viewFactory)
        {
            _viewFactory = viewFactory;
        }

        public void ShowLoadingWindow()
        {
            _loadingWindow = _viewFactory.Invoke();
            _loadingWindow.Owner = Application.Current.MainWindow;
            _loadingWindow.Show();
        }

        public void CloseLoadingWindow()
        {
            _loadingWindow?.Close();
            _loadingWindow = null;
        }
    }
}
