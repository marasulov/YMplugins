using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.ApplicationServices.Core;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using Gile.AutoCAD.Extension;
using SimpleInjector;
using YMplugins.Contracts;
using YMplugins.Contracts.Dto;
using YMplugins.Models.Autocad2022.AutoPrint;
using YMplugins.Models.Autocad2022.AutoPrint.Blocks;
using YMplugins.Models.Autocad2022.AutoPrint.Layers;
using YMplugins.Models.Autocad2022.Contracts;
using YMplugins.Models.Autocad2022.Utils;
using YMplugins.Models.Autocad2022.Utils.LayoutsServices;
using YMplugins.Services;
using YMplugins.ViewModels.Commands;
using YMplugins.ViewModels.VM;
using YMplugins.Views.Services;
using YMplugins.Views.Views;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;

namespace YMplugins.Models.Autocad2022
{
    public class TestCommands
    {
        [CommandMethod("Autoprint2")]
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
            container.Register<IAttributesService, AttributeService>();
            container.Register<ICombinePdfService, CombinePdfService>();
            container.Register<IAutoCadFileService, AutoCadFileService>();
            container.Register<IBlockFinder, BlockFinder>();
            container.Register<IPolylineFinder, PolylineFinder>();
            container.Register<IDeleteEmptyLayoutsService, DeleteEmptyLayoutsService>();
            container.Register<ISetLayoutPlotSettingService, SetLayoutPlotSettingService>();


            container.Register<INotifyService, NotifyService>();
            container.Register<IWindowService, WindowService>();

            var window = container.GetInstance<AutoPrintView>();
            var context = (AutoPrintVm)window.DataContext;

            context.GetBlocksNameCommand.Execute(null);
            context.GetLayersCommand.Execute(null);

            window.ShowDialog();
        }

        //[CommandMethod("SearchBlocksByName")]
        //public void SearchBlocksByName()
        //{
        //    Document doc = Application.DocumentManager.MdiActiveDocument;
        //    Database db = doc.Database;
        //    Editor ed = doc.Editor;

        //    // Prompt user for block name
        //    PromptResult pr = ed.GetString("\nEnter block name to search for: ");
        //    if (pr.Status != PromptStatus.OK) return;
        //    string blockName = pr.StringResult;

        //    using (doc.LockDocument())
        //    {
        //        // Search in Model Space
        //        List<BlockReference> modelBlocks = SearchSpaceByName(db, SymbolUtilityServices.GetBlockModelSpaceId(db), blockName);
        //        ed.WriteMessage($"\nFound {modelBlocks.Count} dynamic blocks named '{blockName}' in Model Space");

        //        // Search in all Layout spaces
        //        List<BlockReference> layoutBlocks = new List<BlockReference>();
        //        using (Transaction tr = db.TransactionManager.StartTransaction())
        //        {
        //            DBDictionary layouts = tr.GetObject(db.LayoutDictionaryId, OpenMode.ForRead) as DBDictionary;
        //            foreach (DBDictionaryEntry entry in layouts)
        //            {
        //                Layout layout = tr.GetObject(entry.Value, OpenMode.ForRead) as Layout;
        //                if (!layout.ModelType)  // Skip Model Space layout
        //                {
        //                    BlockTableRecord btr = tr.GetObject(layout.BlockTableRecordId, OpenMode.ForRead) as BlockTableRecord;
        //                    layoutBlocks.AddRange(SearchSpaceByName(db, btr.ObjectId, blockName));
        //                }
        //            }
        //            tr.Commit();
        //        }
        //        ed.WriteMessage($"\nFound {layoutBlocks.Count} dynamic blocks named '{blockName}' in all Layouts");

        //        int totalBlocks = modelBlocks.Count + layoutBlocks.Count;
        //        ed.WriteMessage($"\nTotal dynamic blocks named '{blockName}' found: {totalBlocks}");
        //    }
        //}

        //private List<BlockReference> SearchSpaceByName(Database db, ObjectId spaceId, string blockName)
        //{
        //    List<BlockReference> blockRefs = new List<BlockReference>();
        //    using (Transaction tr = db.TransactionManager.StartTransaction())
        //    {
        //        BlockTableRecord space = tr.GetObject(spaceId, OpenMode.ForRead) as BlockTableRecord;
        //        foreach (ObjectId id in space)
        //        {
        //            Entity ent = tr.GetObject(id, OpenMode.ForRead) as Entity;
        //            if (ent is BlockReference blockRef)
        //            {
        //                BlockTableRecord btr = tr.GetObject(blockRef.BlockTableRecord, OpenMode.ForRead) as BlockTableRecord;
        //                if (btr.Name.Equals(blockName, StringComparison.OrdinalIgnoreCase) && blockRef.IsDynamicBlock)
        //                {
        //                    blockRefs.Add(blockRef);
        //                }
        //            }
        //        }
        //        tr.Commit();
        //    }
        //    return blockRefs;
        //}

        [CommandMethod("FindBlockByNameInSpace")]
        public void FindBlockByNameInSpace()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;

            // Get block name from user
            PromptStringOptions promptOptions = new PromptStringOptions("\nEnter block name to search: ");
            PromptResult result = ed.GetString(promptOptions);

            if (result.Status != PromptStatus.OK)
            {
                return;
            }

            string blockName = result.StringResult;

            // Ask where to search: Model, Layouts, or both
            PromptKeywordOptions spaceOptions = new PromptKeywordOptions("\nSearch in [Model/Layout/Both]: ");
            spaceOptions.Keywords.Add("Model");
            spaceOptions.Keywords.Add("Layout");
            spaceOptions.Keywords.Add("Both");
            spaceOptions.AllowNone = false;

            PromptResult spaceResult = ed.GetKeywords(spaceOptions);

            if (spaceResult.Status != PromptStatus.OK)
            {
                return;
            }

            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                BlockTable bt = (BlockTable)trans.GetObject(db.BlockTableId, OpenMode.ForRead);

                bool found = false;

                if (spaceResult.StringResult == "Model")
                {
                    // Search in Model Space
                    BlockTableRecord modelSpace = (BlockTableRecord)trans.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForRead);
                    found |= SearchBlockInSpace(trans, modelSpace, blockName, "Model Space");
                }

                if (spaceResult.StringResult == "Layout")
                {
                    // Search in each Layout (Paper Space)
                    foreach (ObjectId btrId in bt)
                    {
                        BlockTableRecord btr = (BlockTableRecord)trans.GetObject(btrId, OpenMode.ForRead);

                        if (btr.IsLayout)
                        {
                            Layout layout = (Layout)trans.GetObject(btr.LayoutId, OpenMode.ForRead);
                            if (layout.LayoutName != "Model")
                            {
                                found |= SearchBlockInSpace(trans, btr, blockName, $"Layout: {layout.LayoutName}");
                            }
                        }
                    }
                }

                if (!found)
                {
                    ed.WriteMessage($"\nBlock {blockName} not found.");
                }

                trans.Commit();
            }
        }

        // Function to search for the block in a given space (Model Space or Layout)
        //private bool SearchBlockInSpace(Transaction trans, BlockTableRecord space, string blockName, string spaceName)
        //{
        //    Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;

        //    foreach (ObjectId entId in space)
        //    {
        //        Entity ent = (Entity)trans.GetObject(entId, OpenMode.ForRead);

        //        if (ent is BlockReference blockRef && blockRef.Name == blockName)
        //        {
        //            ed.WriteMessage($"\nBlock {blockName} found in {spaceName}.");
        //            return true;
        //        }
        //    }
        //    return false;
        //}

        [CommandMethod("GetPolylinePosition")]
        public static void GetPolylinePosition()
        {
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Editor acEd = acDoc.Editor;
            Database acCurDb = acDoc.Database;

            // Запрашиваем пользователя выбрать объект
            PromptEntityOptions peo = new PromptEntityOptions("\nВыберите полилинию: ");
            peo.SetRejectMessage("\nЭто не полилиния. Попробуйте снова.");
            peo.AddAllowedClass(typeof(Polyline), true);

            PromptEntityResult per = acEd.GetEntity(peo);
            if (per.Status != PromptStatus.OK)
            {
                acEd.WriteMessage("\nВыбор отменен.");
                return;
            }

            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                // Получаем выбранный объект
                Entity ent = acTrans.GetObject(per.ObjectId, OpenMode.ForRead) as Entity;

                if (ent is Polyline)
                {
                    Polyline polyline = ent as Polyline;

                    // Получение первой точки полилинии
                    Point2d firstPoint = GetFirstPoint(polyline);
                    acEd.WriteMessage($"\nПервая точка полилинии: X = {firstPoint.X}, Y = {firstPoint.Y}");

                    // Или получение центроида полилинии
                    Point2d centroid = GetCentroid(polyline);
                    acEd.WriteMessage($"\nЦентроид полилинии: X = {centroid.X}, Y = {centroid.Y}");
                }

                acTrans.Commit();
            }
        }

        public static Point2d GetCentroid(Polyline polyline)
        {
            double sumX = 0, sumY = 0;
            int vertexCount = polyline.NumberOfVertices;

            // Проходим по всем вершинам полилинии
            for (int i = 0; i < vertexCount; i++)
            {
                Point2d vertex = polyline.GetPoint2dAt(i);
                sumX += vertex.X;
                sumY += vertex.Y;
            }

            // Возвращаем среднюю точку по X и Y
            return new Point2d(sumX / vertexCount, sumY / vertexCount);
        }

        public static Point2d GetFirstPoint(Polyline polyline)
        {
            return polyline.GetPoint2dAt(0); // Получение первой точки
        }

        [CommandMethod("SelectAndGetDimensions")]
        public static void SelectAndGetDimensions()
        {
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Editor acEd = acDoc.Editor;
            Database acCurDb = acDoc.Database;

            // Запрашиваем пользователя выбрать объект
            PromptEntityOptions peo = new PromptEntityOptions("\nВыберите полилинию: ");
            peo.SetRejectMessage("\nЭто не полилиния. Попробуйте снова.");
            peo.AddAllowedClass(typeof(Polyline), true);

            // Получаем результат выбора
            PromptEntityResult per = acEd.GetEntity(peo);
            if (per.Status != PromptStatus.OK)
            {
                acEd.WriteMessage("\nВыбор отменен.");
                return;
            }

            // Открываем транзакцию и обрабатываем выбранную полилинию
            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                // Получаем выбранный объект
                Entity ent = acTrans.GetObject(per.ObjectId, OpenMode.ForRead) as Entity;

                // Передаем объект полилинии в метод для расчета длины и ширины
                if (ent is Polyline)
                {
                    Polyline polyline = ent as Polyline;
                    (double length, double width) = GetDimensions(polyline);

                    // Выводим результаты
                    acEd.WriteMessage($"\nДлина: {length}, Ширина: {width}");
                }

                acTrans.Commit();
            }
        }

        public static (double length, double width) GetDimensions(Polyline polyline)
        {
            double minX = double.MaxValue, minY = double.MaxValue;
            double maxX = double.MinValue, maxY = double.MinValue;

            for (int i = 0; i < polyline.NumberOfVertices; i++)
            {
                Point2d vertex = polyline.GetPoint2dAt(i);

                if (vertex.X < minX) minX = vertex.X;
                if (vertex.X > maxX) maxX = vertex.X;
                if (vertex.Y < minY) minY = vertex.Y;
                if (vertex.Y > maxY) maxY = vertex.Y;
            }

            double length = maxX - minX;
            double width = maxY - minY;

            return (length, width);
        }

        // Function to search for the block in a given space (Model Space or Layout)
        private bool SearchBlockInSpace(Transaction trans, BlockTableRecord space, string blockName, string spaceName)
        {
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            bool found = false;

            foreach (ObjectId entId in space)
            {
                Entity ent = (Entity)trans.GetObject(entId, OpenMode.ForRead);

                if (ent is BlockReference blockRef)
                {
                    string blockRefName = GetEffectiveBlockName(blockRef, trans);

                    if (blockRefName == blockName)
                    {
                        ed.WriteMessage($"\nBlock {blockName} found in {spaceName}.");

                        // Get Attributes (name and value) from the block
                        //if (blockRef.AttributeCollection.Count > 0)
                        //{
                        //    GetAttributesFromBlock(blockRef);
                        //}

                        //// If it's a dynamic block, list the dynamic properties
                        //if (blockRef.IsDynamicBlock)
                        //{
                        //    ListDynamicBlockProperties(blockRef);
                        //}

                        found = true;
                    }
                }
            }
            return found;
        }

        // Function to get the effective name of the block (handles dynamic blocks)
        private string GetEffectiveBlockName(BlockReference blockRef, Transaction trans)
        {
            // If the block is dynamic, get its effective name
            if (blockRef.IsDynamicBlock)
            {
                BlockTableRecord dynamicBTR = (BlockTableRecord)trans.GetObject(blockRef.DynamicBlockTableRecord, OpenMode.ForRead);
                return dynamicBTR.Name; // Get the effective dynamic block name
            }
            else
            {
                return blockRef.Name; // Regular block name
            }
        }

        // Function to list dynamic properties of the block
        private void ListDynamicBlockProperties(BlockReference blockRef)
        {
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;

            // Get the dynamic properties of the block reference
            DynamicBlockReferencePropertyCollection dynamicProps = blockRef.DynamicBlockReferencePropertyCollection;

            if (dynamicProps != null && dynamicProps.Count > 0)
            {
                ed.WriteMessage($"\nDynamic Block Properties for {blockRef.Name}:");

                foreach (DynamicBlockReferenceProperty prop in dynamicProps)
                {
                    ed.WriteMessage($"\n  - {prop.PropertyName}: {prop.Value}");
                }
            }
        }

        // Function to retrieve attributes from a block reference
        private void GetAttributesFromBlock(BlockReference blockRef)
        {
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;

            // Iterate over the block's attributes
            foreach (ObjectId attId in blockRef.AttributeCollection)
            {
                AttributeReference attRef = (AttributeReference)blockRef.Database.TransactionManager.GetObject(attId, OpenMode.ForRead);

                // Print attribute name and value
                ed.WriteMessage($"\n  - Attribute: {attRef.Tag}, Value: {attRef.TextString}");
            }
        }
    }
}