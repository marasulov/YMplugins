using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using Dreambuild.AutoCAD;
using Gile.AutoCAD.Extension;
using System.ComponentModel;
using YMplugins.Contracts;
using YMplugins.ViewModels.Commands;
using YMplugins.ViewModels.VM;

namespace YMplugins.Models.Autocad2022.AutoPrint
{
    public class AutoPrintCommand
    {
        /// <summary>
        /// Selects entities on given layer.
        /// </summary>
        //[CommandMethod("Autoprint")]
        //public static void Print()
        //{
        //    var container = new Container();
        //    container.Register<GetAttributesCommand>();
        //    container.Register<GetBlocksNameCommand>();
        //    container.Register<GetLayersCommand>();
        //    container.Register<PrintCommand>();
        //    container.Register<SelectBlockCommand>();
        //    container.Register<AutoPrintVm>();
        //    container.Register<AutoPrintView>();

        //    container.Register<IGetBlocksNameService, GetBlocksNameService>();
        //    container.Register<IGetLayersService, GetLayerService>();
        //    container.Register<ISelectBlockService, SelectBlockService>();
        //    container.Register<IAttributesService, AttributesService>();

        //    var window = container
        //        .GetInstance<AutoPrintView>();

        //    var context = (AutoPrintVm)window.DataContext;
        //    context.GetBlocksNameCommand.Execute(null);
        //    context.GetLayersCommand.Execute(null);

        //    window.ShowDialog();
        //}

    }
}
