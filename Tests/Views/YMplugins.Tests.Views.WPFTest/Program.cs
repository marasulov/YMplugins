using SimpleInjector;
using YMplugins.ViewModels.Commands;
using YMplugins.ViewModels.VM;
using YMplugins.Views.Views;

using System;
using Mocks;
using YMplugins.Contracts;

namespace YMplugins.Tests.Views.WPFTest
{
    class Program
    {
        [STAThread()]
        static void Main(string[] args)
        {

            var container = new Container();
            container.Register<GetAttributesCommand>();
            container.Register<GetBlocksNameCommand>();
            container.Register<GetLayersCommand>();
            container.Register<PrintCommand>();
            container.Register<SelectBlockCommand>();
            container.Register<AutoPrintVm>();
            container.Register<AutoPrintView>();

            container.Register<IGetBlocksNameService, GetBlocksNameService>();
            container.Register<IGetLayersService, GetLayerService>();
            container.Register<ISelectBlockService, SelectBlockService>();

            var window = container
                .GetInstance<AutoPrintView>();

            var context = (AutoPrintVm)window.DataContext;
            context.GetBlocksNameCommand.Execute(null);
            context.GetLayersCommand.Execute(null);

            window.ShowDialog();

        }
    }
}