using System.Collections.Generic;
using Gile.AutoCAD.Extension;
using YMplugins.Contracts;
using YMplugins.Contracts.Dto;
using YMplugins.Models.Autocad2022.Utils.Print;

namespace YMplugins.Models.Autocad2022.AutoPrint
{
    public class PrintService : IPrintService
    {
        private readonly ICombinePdfService _fileCombineService;
        private readonly INamingService _namingService;
        private readonly IPrintEngine _printEngine;

        public PrintService(ICombinePdfService fileCombineService)
        {
            _fileCombineService = fileCombineService;
            
        }

        public string[] Print(PrintInfo[] data)
        {
            
            //var objectsToPrint = _searchService.FindObjects(data);
            
            //var printInfos = _namingService.GenerateFileName(objectsToPrint, 0);

            List<string> fileNames = new List<string>();

            foreach (PrintInfo info in data)
            {
                var printUtils = new PrintUtils();
                var fileName = printUtils.PlotCurrentLayout(info);
                fileNames.Add(fileName);
            }
            return fileNames.ToArray();

           
           //_printEngine.PrintObjects(objectsToPrint, fileName, data);
        }
    }
}