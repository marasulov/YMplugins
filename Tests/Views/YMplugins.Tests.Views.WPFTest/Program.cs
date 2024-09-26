using Mocks;
using SimpleInjector;
using System;
using YMplugins.Contracts;
using YMplugins.ViewModels.Commands;
using YMplugins.ViewModels.VM;
using YMplugins.Views.Views;

namespace YMplugins.Tests.Views.WPFTest
{
    internal class Program
    {
        [STAThread()]
        private static void Main(string[] args)
        {
            var container = new Container();
            container.Register<GetAttributesCommand>();
            container.Register<GetBlocksNameCommand>();
            container.Register<GetLayersCommand>();
            container.Register<PrintCommand>();
            container.Register<SelectBlockCommand>();
            container.Register<ZoomToPointCommand>();
            container.Register<AutoPrintVm>();
            container.Register<AutoPrintView>();

            container.Register<IGetBlocksNameService, GetBlocksNameService>();
            container.Register<IGetLayersService, GetLayerService>();
            container.Register<ISelectBlockService, SelectBlockService>();
            container.Register<IAttributesService, AttributesService>();
            container.Register<IPrintService, PrintService>();
            container.Register<ISearchService, SearchService>();
            container.Register<INamingService, NamingService>();
            container.Register<IPrintEngine, PrintEngine>();
            container.Register<IZoomEntity, ZoomService>();


            var window = container
                .GetInstance<AutoPrintView>();

            var context = (AutoPrintVm)window.DataContext;
            context.GetBlocksNameCommand.Execute(null);
            context.GetLayersCommand.Execute(null);

            window.ShowDialog();
        }
    }
}