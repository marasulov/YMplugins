using Autodesk.AutoCAD.Runtime;
using SimpleInjector;
using YMplugins.Contracts;
using YMplugins.Models.Autocad2022.AutoPrint;
using YMplugins.ViewModels.Commands;
using YMplugins.ViewModels.VM;
using YMplugins.Views.Views;

namespace YMplugins.Addin.Acad2022.Commands.AutoPrint
{
    public class AutoPrintCommand
    {
        [CommandMethod("AutoPrint")]
        public static void Print()
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
            container.Register<IGetLayersService, GetLayersService>();
            container.Register<ISelectBlockService, SelectBlockService>();
            container.Register<IAttributesService, AttributesService>();

            var window = container
                .GetInstance<AutoPrintView>();

            var context = (AutoPrintVm)window.DataContext;
            context.GetBlocksNameCommand.Execute(null);
            context.GetLayersCommand.Execute(null);

            window.ShowDialog();
        }
    }
}
