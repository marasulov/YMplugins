using Gile.AutoCAD.Extension;
using System.Collections.Generic;
using Autodesk.AutoCAD.DatabaseServices;
using YMplugins.Contracts;
using YMplugins.Contracts.Dto;
using YMplugins.Models.Autocad2022.Utils.Print;

namespace YMplugins.Models.Autocad2022.AutoPrint
{
    public class PrintService : IPrintService
    {
        public string[] Print(PrintInfo[] data)
        {
            Active.Document.SendStringToExecute("_QSAVE ", true, false, false);
            List<string> fileNames = new List<string>();
            StandartCopier standartCopier = new StandartCopier();
            var acDoc = Active.Document;
            var acCurDb = acDoc.Database;
            foreach (PrintInfo info in data)
            {
                var printUtils = new PrintUtils();
                var fileName = printUtils.PlotCurrentLayout(info ,standartCopier, acCurDb);
                fileNames.Add(fileName);
            }
            return fileNames.ToArray();

        }
    }

    public class BatchPrintService : IBatchPrintService
    {
        public string[] Print(PrintInfo[] data, Database db)
        {
            //var objectsToPrint = _searchService.FindObjects(data);

            //var printInfos = _namingService.GenerateFileName(objectsToPrint, 0);
            Active.Document.SendStringToExecute("_QSAVE ", true, false, false);
            List<string> fileNames = new List<string>();
            StandartCopier standartCopier = new StandartCopier();
        

            foreach (PrintInfo info in data)
            {
                var printUtils = new PrintUtils();
                var fileName = printUtils.PlotCurrentLayout(info, standartCopier, db);
                fileNames.Add(fileName);
            }
            return fileNames.ToArray();

            //_printEngine.PrintObjects(objectsToPrint, fileName, data);
        }
    }

    public interface IBatchPrintService
    {
        string[] Print(PrintInfo[] data, Database db);
    }
}