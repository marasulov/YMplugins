using System;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using Dreambuild.AutoCAD;
using Gile.AutoCAD.Extension;
using SimpleInjector;
using System.Collections.Generic;
using YMplugins.Contracts;
using YMplugins.Models.Autocad2022.AutoPrint;
using YMplugins.ViewModels.Commands;
using YMplugins.ViewModels.VM;
using YMplugins.Views.Views;

namespace YMplugins.Addin.Autocad2022.Commands.AutoPrint
{
    public class AutoPrintCommand
    {
        [CommandMethod("selb")]

        public void selectDynamicBlockReferences()
        {
          

        }

        [CommandMethod("Autoprint")]
        public static void Print()
        {
            var container = new Container();
            container.Options.EnableAutoVerification = false;

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

        [CommandMethod("ListarBloques")]
        public void ListarBloques()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;

            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                // open the block table which contains all the BlockTableRecords (block definitions and spaces)
                var blockTable = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead);

                // open the model space BlockTableRecord
                var modelSpace = (BlockTableRecord)tr.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForRead);

                // iterate through the model space 
                foreach (ObjectId id in modelSpace)
                {
                    // check if the current ObjectId is a block reference one
                    if (id.ObjectClass.DxfName == "INSERT")
                    {
                        // open the block reference
                        var blockReference = (BlockReference)tr.GetObject(id, OpenMode.ForRead);

                        // print the block name to the command line
                        ed.WriteMessage("\n" + blockReference.Name);
                    }
                }

                tr.Commit();
            }
        }

        [CommandMethod("TestMe")]
        public void ListarBloques1()
        {
            Document miDibujo = Application.DocumentManager.MdiActiveDocument;
            Database misElementos = miDibujo.Database;

            var blocks = GetPaperSpaceBlockReferences(misElementos);

            using (Transaction miTransaccion = misElementos.TransactionManager.StartTransaction())
            {



                BlockTable blckTbl;
                blckTbl = miTransaccion.GetObject(misElementos.BlockTableId, OpenMode.ForRead) as BlockTable;

                BlockTableRecord blckTblRcrd;
                blckTblRcrd = miTransaccion.GetObject(blckTbl[BlockTableRecord.ModelSpace], OpenMode.ForRead) as BlockTableRecord;

                foreach (ObjectId id in blckTblRcrd)
                {

                    var dbObj = miTransaccion.GetObject(id, OpenMode.ForRead);
                    if (dbObj is BlockReference)
                    {
                        var blckRef = (BlockReference)dbObj;
                        miDibujo.Editor.WriteMessage("" + blckRef.BlockName);
                    }
                }

                miTransaccion.Commit();
            }


        }

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
