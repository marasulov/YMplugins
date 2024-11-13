using System;
using Gile.AutoCAD.Extension;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using YMplugins.Contracts;
using YMplugins.Contracts.Dto;
using YMplugins.Models.Autocad2022.Utils.Print;
using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;

namespace YMplugins.Models.Autocad2022.AutoPrint
{
    public class PrintService : IPrintService
    {
        public string[] Print(PrintInfo[] data)
        {
            //Active.Document.SendStringToExecute("_QSAVE ", true, false, false);
            var fileNames = new List<string>();
            var standartCopier = new StandartCopier()
            
            // var acDoc = Active.Document;
            // var acCurDb = acDoc.Database;
            Dictionary<string, (Document, Database)> documentMap = new Dictionary<string, (Document, Database)>();
            var uniqueFileNames = data.Select(p => p.SourceFileName).Distinct();
            
            foreach (var fileName in uniqueFileNames)
            {
                Document doc = Application.DocumentManager.Cast<Document>()
                    .FirstOrDefault(d => d.Name.Equals(fileName, StringComparison.OrdinalIgnoreCase));

                if (doc != null)
                {
                    documentMap[fileName] = (doc, doc.Database);
                }
            }
            
            foreach (var printInfo in data)
            {
                if (documentMap.TryGetValue(printInfo.SourceFileName, out var docInfo))
                {
                    Application.DocumentManager.MdiActiveDocument = docInfo.Item1;
                    Database db = docInfo.Item2;

                    docInfo.Item1.Editor.WriteMessage($"______printing {docInfo.Item1.Name}____ \n");
                    var printUtils = new PrintUtils();
                    var fileName = printUtils.PlotCurrentLayout(printInfo ,standartCopier, db);
                    fileNames.Add(fileName);
                }
                else
                {
                    Debug.Print($"Документ {printInfo.SourceFileName} не найден.");
                }
            }
            
            // var acCurDb = Application.DocumentManager.Cast<Document>()
            //     .FirstOrDefault(d => d.Name.Equals());
            // foreach (PrintInfo info in data)
            // {
            //     var printUtils = new PrintUtils();
            //     var fileName = printUtils.PlotCurrentLayout(info ,standartCopier, acCurDb);
            //     fileNames.Add(fileName);
            // }
            return fileNames.ToArray();

        }
    }

    public class BatchPrintService : IBatchPrintService
    {
        public string[] Print(PrintInfo[] data, Database db)
        {
            //var objectsToPrint = _searchService.FindObjects(data);

            //var printInfos = _namingService.GenerateFileName(objectsToPrint, 0);
            //Active.Document.SendStringToExecute("_QSAVE ", true, false, false);
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