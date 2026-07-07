using Autodesk.AutoCAD.Runtime;
#if NET8_0_OR_GREATER
using Gile.AutoCAD.R25.Extension;
#else
using Gile.AutoCAD.R20.Extension;
#endif
using SimpleInjector;
using YMplugins.Contracts;
using YMplugins.Contracts.Dto;
using YMplugins.Models.Autocad2024.AutoPrint;
using YMplugins.Models.Autocad2024.AutoPrint.Blocks;
using YMplugins.Models.Autocad2024.AutoPrint.Layers;
using YMplugins.Models.Autocad2024.Contracts;
using YMplugins.Models.Autocad2024.Utils;
using YMplugins.Models.Autocad2024.Utils.LayoutsServices;
using YMplugins.Services;
using YMplugins.ViewModels.Commands;
using YMplugins.ViewModels.VM;
using YMplugins.Views.Services;
using YMplugins.Views.Views;

namespace YMplugins.Addin.Autocad2024.Commands.AutoPrint
{
    public class AutoPrintCommand
    {
        [CommandMethod("Autoprint")]
        public static void Print()
        {
            Active.Document.SendStringToExecute("_QSAVE ", true, false, false);
            var container = new Container();
            container.Options.EnableAutoVerification = false;

            container.Register<GetAttributesCommand>();
            container.Register<GetBlocksNameCommand>();
            container.Register<GetLayersCommand>();
            container.Register<PrintCommand>();
            container.Register<SelectBlockCommand>();
            container.Register<SelectLayerCommand>();
            container.Register<ZoomToPointCommand>();
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
            container.Register<ISelectBlockService, SelectBlockService>();
            container.Register<ISelectLayerService, SelectLayerService>();
            container.Register<IAttributesService, AttributeService>();
            container.Register<ICombinePdfService, CombinePdfService>();
            container.Register<IAutoCadFileService, AutoCadFileService>();
            container.Register<IBlockFinder, BlockFinder>();
            container.Register<IPolylineFinder, PolylineFinder>();
            container.Register<IDeleteEmptyLayoutsService, DeleteEmptyLayoutsService>();
            container.Register<ISetLayoutPlotSettingService, SetLayoutPlotSettingService>();
            container.Register<ICreateDwgService, CreateDwgService>();


            container.Register<INotifyService, NotifyService>();
            container.Register<IWindowService, WindowService>();

            var window = container.GetInstance<AutoPrintView>();
            var context = (AutoPrintVm)window.DataContext;

            context.GetBlocksNameCommand.Execute(null);
            context.GetLayersCommand.Execute(null);

            // ShowModalWindow привязывает WPF-окно к главному окну AutoCAD и
            // корректно интегрируется с его циклом сообщений. Сырой ShowDialog()
            // оставлял редактор без курсора, а окно не всплывало до переключения
            // документа.
            Autodesk.AutoCAD.ApplicationServices.Application.ShowModalWindow(window);
        }

    }
}