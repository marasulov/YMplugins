using Mocks;
using SimpleInjector;
using System;
using YMplugins.Contracts;
using YMplugins.Contracts.Dto;
using YMplugins.ViewModels.Commands;
using YMplugins.ViewModels.VM;
using YMplugins.Views.Services;
using YMplugins.Views.Views;

namespace YMplugins.Tests.Views.WPFTest
{
    internal class Program
    {
        [STAThread()]
        private static void Main(string[] args)
        {
            var container = new Container();
            container.Options.EnableAutoVerification = false;

            container.Register<GetAttributesCommand>();
            container.Register<GetBlocksNameCommand>();
            container.Register<GetLayersCommand>();
            container.Register<GetAllDocsLayersCommand>();
            container.Register<PrintCommand>();
            container.Register<SelectBlockCommand>();
            container.Register<ZoomToPointCommand>();
            // container.Register<GetLayersCommand>();
            container.Register<AutoPrintVm>(Lifestyle.Transient);
            container.Register<AutoPrintView>(Lifestyle.Transient);

            container.Register<LoadingWindow>(Lifestyle.Transient);

            container.Register<IGetBlocksNameService, GetBlocksNameService>();
            container.Register<IPrintService, PrintService>();
            container.Register<INamingService, NamingService>();
            container.Register<BlockSearchService>();
            container.Register<SearchData>();

            container.Register<ISearchService, SearchService>();
            container.Register<IZoomEntity, ZoomService>();
            container.Register<IGetLayersService, GetLayersService>();
            container.Register<IGetLayersFromOpenedDocsService, GetLayerFromOpenedDocsService>();
            container.Register<IGetBlocksFromOpenedDocsService, GetBlockFromOpenedDocsService>();
            
            container.Register<ISelectBlockService, SelectBlockService>();
            container.Register<IAttributesService, AttributeService>();
            container.Register<ICombinePdfService, CombinePdfService>();
            container.Register<IAutoCadFileService, AutoCadFileService>();
            // container.Register<IBlockFinder, BlockFinder>();
            // container.Register<IPolylineFinder, PolylineFinder>();
            container.Register<IDeleteEmptyLayoutsService, DeleteEmptyLayoutsService>();
            container.Register<ISetLayoutPlotSettingService, SetLayoutPlotSettingService>();
            container.Register<ICreateDwgService, CreateDwgService>();
            
            container.Register<INotifyService, NotifyService>();
            container.Register<IWindowService, WindowService>();

            var window = container.GetInstance<AutoPrintView>();
            var context = (AutoPrintVm)window.DataContext;

            context.GetBlocksNameCommand.Execute(null);
            context.GetLayersCommand.Execute(null);
            //context.GetAllDocsLayersCommand.Execute(null);

            window.ShowDialog();
        }
    }
}