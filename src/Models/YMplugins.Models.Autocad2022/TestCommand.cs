using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.PlottingServices;
using Autodesk.AutoCAD.Runtime;
using Gile.AutoCAD.Extension;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.IO;
using Autodesk.AutoCAD.ApplicationServices;
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
            container.Register<ISelectBlockService, SelectBlockService>();
            container.Register<IAttributesService, AttributeService>();
            container.Register<ICombinePdfService, CombinePdfService>();
            container.Register<IAutoCadFileService, AutoCadFileService>();

            container.Register<IGetLayersFromOpenedDocsService, GetLayerFromOpenedDocsService>();
            container.Register<IGetBlocksFromOpenedDocsService, GetBlocksFromOpenedDocsService>();
            
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
            //context.GetAllDocsLayersCommand.Execute(null);

            window.ShowDialog();
        }

        [CommandMethod("ReadDrawingDataFromFolder")]
        public void ReadDrawingDataFromFolder()
        {
            // string folderPath = @"C:\Users\yusufzhon.marasulov\Documents\11";
            //
            // ReadDataFromFolder(folderPath);
            //
            // ReadStampRectangularPolylinesFromFolder(folderPath);
            var blocksFromAllDocuments = BlockUtils.GetBlocksFromAllOpenDocuments();

            foreach (var entry in blocksFromAllDocuments)
            {
                Active.Editor.WriteMessage("Документ: " + entry.Key);
                foreach (var blockName in entry.Value)
                {
                    Active.Editor.WriteMessage(" - Блок: " + blockName);
                }
            }
            
        }


        public void ReadStampRectangularPolylinesFromFolder(string folderPath)
        {
            // Проверяем, существует ли папка
            if (!Directory.Exists(folderPath))
            {
                Active.Editor.WriteMessage("Указанная папка не найдена: " + folderPath);
                return;
            }

            // Получаем список всех DWG-файлов в папке
            string[] dwgFiles = Directory.GetFiles(folderPath, "*.dwg");

            foreach (string dwgFilePath in dwgFiles)
            {
                Active.Editor.WriteMessage("Чтение файла: " + dwgFilePath);

                // Создаем объект Database для работы с чертежом на диске
                using (Database db = new Database(false, true))
                {
                    // Читаем чертеж из файла
                    db.ReadDwgFile(dwgFilePath, FileShare.Read, true, "");

                    // Запускаем транзакцию для доступа к данным чертежа
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        // Получаем BlockTableRecord для ModelSpace
                        BlockTable bt = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead);
                        BlockTableRecord btr =
                            (BlockTableRecord)tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForRead);

                        Active.Editor.WriteMessage("Прямоугольные закрытые полилинии на слое 'Штамп':");

                        // Проходим по каждому объекту в ModelSpace
                        foreach (ObjectId objId in btr)
                        {
                            Entity entity = (Entity)tr.GetObject(objId, OpenMode.ForRead);
                            if (entity is Polyline polyline &&
                                polyline.Closed &&
                                polyline.NumberOfVertices == 4 &&
                                polyline.Layer == "штамп")
                            {
                                Active.Editor.WriteMessage("- Полилиния является закрытым прямоугольником.");
                                for (int i = 0; i < polyline.NumberOfVertices; i++)
                                {
                                    Active.Editor.WriteMessage("  Вершина " + i + ": " + polyline.GetPoint2dAt(i));
                                }
                            }
                        }

                        // Завершаем транзакцию
                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("ConvertDWGToPDF")]
        public void ConvertDWGToPDF()
        {
            // Укажите папку с исходными DWG файлами и папку для сохранения PDF
            string sourceFolder = @"C:\Users\yusufzhon.marasulov\Documents\11";
            string outputFolder = @"C:\Users\yusufzhon.marasulov\Documents\11\OutputPDFs";

            // Создаем экземпляр класса для конвертации чертежей

            ConvertFolderToPDF(sourceFolder, outputFolder);
        }

        public void ConvertFolderToPDF(string sourceFolder, string outputFolder)
        {
            // Проверяем, существует ли папка
            if (!Directory.Exists(sourceFolder))
            {
                Active.Editor.WriteMessage("Папка не найдена: " + sourceFolder);
                return;
            }

            // Убедитесь, что папка для вывода существует
            if (!Directory.Exists(outputFolder))
            {
                Directory.CreateDirectory(outputFolder);
            }

            // Получаем список всех DWG-файлов в папке
            string[] dwgFiles = Directory.GetFiles(sourceFolder, "*.dwg");

            foreach (string dwgFilePath in dwgFiles)
            {
                string fileName = Path.GetFileNameWithoutExtension(dwgFilePath);
                string pdfFilePath = Path.Combine(outputFolder, fileName + ".pdf");
                Active.Editor.WriteMessage("Конвертация " + dwgFilePath + " в " + pdfFilePath);

                // Создаем объект Database для работы с чертежом на диске
                using (Database db = new Database(false, true))
                {
                    db.ReadDwgFile(dwgFilePath, FileShare.Read, true, "");

                    // Печать в PDF
                    PlotToPDF(db, pdfFilePath);
                }
            }
        }

        private void PlotToPDF(Database db, string pdfFilePath)
        {
            using (PlotEngine plotEngine = PlotFactory.CreatePublishEngine())
            {
                using (PlotProgressDialog progressDialog = new PlotProgressDialog(false, 1, true))
                {
                    // Настройки печати
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        PlotSettings plotSettings = new PlotSettings(true);
                        PlotSettingsValidator psv = PlotSettingsValidator.Current;

                        // Указываем параметры PDF устройства
                        psv.SetPlotType(plotSettings, Autodesk.AutoCAD.DatabaseServices.PlotType.Extents);
                        psv.SetUseStandardScale(plotSettings, true);
                        psv.SetPlotConfigurationName(plotSettings, "DWG To PDF.pc3", "ANSI_A_(8.50_x_11.00_Inches)");
                        psv.SetPlotPaperUnits(plotSettings, PlotPaperUnit.Inches);
                        psv.SetPlotOrigin(plotSettings, new Point2d(0, 0));
                        psv.SetPlotCentered(plotSettings, true);

                        // Параметры вывода
                        PlotInfo plotInfo = new PlotInfo
                            { Layout = db.CurrentSpaceId, OverrideSettings = plotSettings };

                        // Начинаем процесс печати
                        plotEngine.BeginPlot(progressDialog, null);
                        plotEngine.BeginDocument(plotInfo, db.Filename, null, 1, true, pdfFilePath);

                        // Создаем область печати
                        PlotPageInfo pageInfo = new PlotPageInfo();
                        plotEngine.BeginPage(pageInfo, plotInfo, true, null);
                        plotEngine.BeginGenerateGraphics(null);
                        plotEngine.EndGenerateGraphics(null);
                        plotEngine.EndPage(null);
                        plotEngine.EndDocument(null);
                        plotEngine.EndPlot(null);

                        tr.Commit();
                    }
                }
            }
        }

        public static void ReadDataFromFolder(string folderPath)
        {
            if (!Directory.Exists(folderPath))
            {
                Active.Editor.WriteMessage("Указанная папка не найдена: " + folderPath);
                return;
            }

            string[] dwgFiles = Directory.GetFiles(folderPath, "*.dwg");

            foreach (string dwgFilePath in dwgFiles)
            {
                Active.Editor.WriteMessage($"Чтение файла:  {dwgFilePath}\n");

                using (Database db = new Database(false, true))
                {
                    db.ReadDwgFile(dwgFilePath, FileShare.Read, true, "");

                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        BlockTable bt = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead);
                        foreach (ObjectId btrId in bt)
                        {
                            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(btrId, OpenMode.ForRead);
                            Active.Editor.WriteMessage($"Имя блока: {btr.Name} \n");
                        }

                        tr.Commit();
                    }
                }
            }
        }

        [CommandMethod("ReadStampRectangularPolylinesFromFolder")]
        public void ReadStampRectangularPolylinesFromFolder()
        {
            // Задайте путь к папке с чертежами
            string folderPath = @"C:\Users\yusufzhon.marasulov\Documents\11";

            // Создаем экземпляр класса для чтения данных чертежей
            DrawingDataReader dataReader = new DrawingDataReader();

            // Читаем данные из всех чертежей в указанной папке
            dataReader.ReadStampRectangularPolylinesFromFolder(folderPath);
        }
    }

    public class DrawingDataReader
    {
        public void ReadStampRectangularPolylinesFromFolder(string folderPath)
        {
            // Проверяем, существует ли папка
            if (!Directory.Exists(folderPath))
            {
                Active.Editor.WriteMessage("Указанная папка не найдена: " + folderPath);
                return;
            }

            // Получаем список всех DWG-файлов в папке
            string[] dwgFiles = Directory.GetFiles(folderPath, "*.dwg");

            foreach (string dwgFilePath in dwgFiles)
            {
                Active.Editor.WriteMessage("Чтение файла: " + dwgFilePath);

                // Создаем объект Database для работы с чертежом на диске
                using (Database db = new Database(false, true))
                {
                    // Читаем чертеж из файла
                    db.ReadDwgFile(dwgFilePath, FileShare.Read, true, "");

                    // Запускаем транзакцию для доступа к данным чертежа
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        // Получаем BlockTableRecord для ModelSpace
                        BlockTable bt = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead);
                        BlockTableRecord btr =
                            (BlockTableRecord)tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForRead);

                        Active.Editor.WriteMessage("Прямоугольные закрытые полилинии на слое 'Штамп':");

                        // Проходим по каждому объекту в ModelSpace
                        foreach (ObjectId objId in btr)
                        {
                            Entity entity = (Entity)tr.GetObject(objId, OpenMode.ForRead);
                            if (entity is Polyline polyline &&
                                polyline.Closed &&
                                polyline.NumberOfVertices == 4 &&
                                polyline.Layer.ToLower() == "штамп")
                            {
                                // Проверка на прямоугольность: углы должны быть прямыми
                                bool isRectangle = IsRectangle(polyline);

                                if (isRectangle)
                                {
                                    Active.Editor.WriteMessage("- Полилиния является закрытым прямоугольником.");
                                    for (int i = 0; i < polyline.NumberOfVertices; i++)
                                    {
                                        Active.Editor.WriteMessage("  Вершина " + i + ": " + polyline.GetPoint2dAt(i));
                                    }
                                }
                            }
                        }

                        // Завершаем транзакцию
                        tr.Commit();
                    }
                }
            }
        }

        // Метод для проверки, является ли полилиния прямоугольной
        private bool IsRectangle(Polyline polyline)
        {
            if (polyline.NumberOfVertices != 4) return false;

            // Проверяем, образуют ли стороны прямые углы
            for (int i = 0; i < 4; i++)
            {
                // Получаем три последовательные точки
                var p1 = polyline.GetPoint2dAt(i);
                var p2 = polyline.GetPoint2dAt((i + 1) % 4);
                var p3 = polyline.GetPoint2dAt((i + 2) % 4);

                // Вычисляем векторы между точками
                var v1 = p2 - p1;
                var v2 = p3 - p2;

                // Проверяем, что скалярное произведение равно нулю (угол 90 градусов)
                if (Math.Abs(v1.X * v2.X + v1.Y * v2.Y) > 1e-6)
                    return false;
            }

            return true;
        }
        
    }
    public static class BlockUtils
    {
        // Метод для получения блоков из всех открытых документов
        public static Dictionary<string, List<string>> GetBlocksFromAllOpenDocuments()
        {
            var documentBlocks = new Dictionary<string, List<string>>();

            // Проходим по всем открытым документам
            foreach (Document doc in Application.DocumentManager)
            {
                List<string> blocks = GetBlocksFromDocument(doc.Database);
                documentBlocks.Add(doc.Name, blocks);
            }

            return documentBlocks;
        }

        // Вспомогательный метод для получения блоков из базы данных документа
        private static List<string> GetBlocksFromDocument(Database db)
        {
            var blockNames = new List<string>();

            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                if (db.BlockTableId.IsValid && tr.GetObject(db.BlockTableId, OpenMode.ForRead) is BlockTable bt)
                {
                    foreach (ObjectId btrId in bt)
                    {
                        try
                        {
                            if (tr.GetObject(btrId, OpenMode.ForRead) is BlockTableRecord btr)
                            {
                                // Проверяем, что блок не является анонимным и не пустой
                                if (!btr.IsAnonymous && !btr.IsLayout)
                                {
                                    blockNames.Add(btr.Name);
                                }
                            }
                        }
                        catch
                        {
                            // Игнорируем ошибки при доступе к блоку
                        }
                    }
                }
                tr.Commit();
            }

            return blockNames;
        }
    }
}