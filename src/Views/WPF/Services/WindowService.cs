
using System.Windows;
using YMplugins.Contracts;
using YMplugins.Views.Views;

namespace YMplugins.Views.Services
{
    public class WindowService : IWindowService
    {
        private LoadingWindow _loadingWindow;

       

        public void ShowLoadingWindow()
        {
            _loadingWindow = new LoadingWindow();
            _loadingWindow.Owner = Application.Current.MainWindow; // Устанавливаем владельцем основное окно
            _loadingWindow.Show();
        }

        public void CloseLoadingWindow()
        {
            _loadingWindow?.Close();
            
        }
    }
}
