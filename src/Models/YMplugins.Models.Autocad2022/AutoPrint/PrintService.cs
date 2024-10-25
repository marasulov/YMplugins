using Gile.AutoCAD.Extension;
using System.Collections.Generic;
using YMplugins.Contracts;
using YMplugins.Contracts.Dto;
using YMplugins.Models.Autocad2022.Utils.Print;

namespace YMplugins.Models.Autocad2022.AutoPrint
{
    public class PrintService : IPrintService
    {
        public string[] Print(PrintInfo[] data)
        {
            //var objectsToPrint = _searchService.FindObjects(data);

            //var printInfos = _namingService.GenerateFileName(objectsToPrint, 0);
            Active.Document.SendStringToExecute("_QSAVE ", true, false, false);
            List<string> fileNames = new List<string>();
            StandartCopier standartCopier = new StandartCopier();
            foreach (PrintInfo info in data)
            {
                var printUtils = new PrintUtils();
                var fileName = printUtils.PlotCurrentLayout(info ,standartCopier);
                fileNames.Add(fileName);
            }
            return fileNames.ToArray();

            //_printEngine.PrintObjects(objectsToPrint, fileName, data);
        }
    }
}