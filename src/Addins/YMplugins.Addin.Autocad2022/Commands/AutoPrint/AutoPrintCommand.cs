using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using SimpleInjector;
using System.Collections.Generic;
using YMplugins.Contracts;
using YMplugins.Contracts.Dto;
using YMplugins.Models.Autocad2022.AutoPrint;
using YMplugins.Models.Autocad2022.Utils;
using YMplugins.ViewModels.Commands;
using YMplugins.ViewModels.VM;
using YMplugins.Views.Views;

namespace YMplugins.Addin.Autocad2022.Commands.AutoPrint
{
    public class AutoPrintCommand
    {
        //[CommandMethod("Autoprint")]
        //public static void Print()
        //{
        //    var container = new Container();
        //    container.Options.EnableAutoVerification = false;

        //    container.Register<GetAttributesCommand>();
        //    container.Register<GetBlocksNameCommand>();
        //    container.Register<GetLayersCommand>();
        //    container.Register<PrintCommand>();
        //    container.Register<SelectBlockCommand>();
        //    container.Register<ZoomToPointCommand>();
        //    container.Register<AutoPrintVm>();
        //    container.Register<AutoPrintView>();

        //    container.Register<IGetBlocksNameService, GetBlocksNameService>();
        //    container.Register<IPrintService, PrintService>();
        //    container.Register<INamingService, NamingService>();
        //    container.Register<IPrintEngine, PrintEngine>();
        //    container.Register<BlockSearchService>();
        //    container.Register<SearchData>();



        //    container.Register<ISearchService, SearchService>();
        //    container.Register<IZoomEntity, ZoomService>();
        //    container.Register<IGetLayersService, GetLayersService>();
        //    container.Register<ISelectBlockService, SelectBlockService>();
        //    container.Register<IAttributesService, AttributeService>();

        //    var window = container
        //        .GetInstance<AutoPrintView>();

        //    var context = (AutoPrintVm)window.DataContext;
        //    context.GetBlocksNameCommand.Execute(null);
        //    context.GetLayersCommand.Execute(null);

        //    window.ShowDialog();
        //}

        private IEnumerable<BlockReference> GetPaperSpaceBlockReferences(Database db)
        {
            Transaction tr = db.TransactionManager.TopTransaction;
            if (tr == null)
                throw new Autodesk.AutoCAD.Runtime.Exception(ErrorStatus.NotTopTransaction);

            RXClass rxc = RXClass.GetClass(typeof(BlockReference));
            DBDictionary layouts = (DBDictionary)tr.GetObject(db.LayoutDictionaryId, OpenMode.ForRead);
            foreach (var entry in layouts)
            {
                if (entry.Key != "Model")
                {
                    Layout lay = (Layout)tr.GetObject(entry.Value, OpenMode.ForRead);
                    BlockTableRecord btr = (BlockTableRecord)tr.GetObject(lay.BlockTableRecordId, OpenMode.ForRead);
                    foreach (ObjectId id in btr)
                    {
                        if (id.ObjectClass == rxc)
                        {
                            yield return (BlockReference)tr.GetObject(id, OpenMode.ForRead);
                        }
                    }
                }
            }
        }
    }
}