using YMplugins.Views.Views;

namespace WS.Views.WPF
{
    using SimpleInjector;
    using System;

    internal class Program
    {
        [STAThread()]
        public static void Main(string[] args)
        {
            var container = new Container();
            
            container.Register<AutoPrintView>();

           

            var window = container
                .GetInstance<AutoPrintView>();

            window.ShowDialog();
        }
    }
}
