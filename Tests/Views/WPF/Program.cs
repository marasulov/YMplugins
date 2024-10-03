using Mocks;
using SimpleInjector;
using System;
using YMplugins.Contracts;
using YMplugins.ViewModels.Commands;
using YMplugins.ViewModels.VM;
using YMplugins.Views.Views;

namespace WS.Views.WPF
{
    internal class Program
    {
        [STAThread()]
        public static void Main(string[] args)
        {
            var container = new Container();
            container.Register<GetAttributesCommand>();
            container.Register<GetBlocksNameCommand>();
            container.Register<GetLayersCommand>();
            container.Register<PrintCommand>();
            container.Register<AutoPrintVm>();
            container.Register<AutoPrintView>();

            container.Register<IGetBlocksNameService, GetBlocksNameService>();

            var window = container
                .GetInstance<AutoPrintView>();

            window.ShowDialog();
        }
    }
}