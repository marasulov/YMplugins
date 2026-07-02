using Gile.AutoCAD.R20.Extension;
using System.Collections.Generic;
using YMplugins.Contracts;
using YMplugins.Contracts.Dto;
using YMplugins.Models.Autocad2024.Utils.Print;

namespace YMplugins.Models.Autocad2024.AutoPrint
{
    public class PrintService : IPrintService
    {
        public string[] Print(PrintInfo[] data)
        {
            List<string> fileNames = new List<string>();
            StandartCopier standartCopier = new StandartCopier();
            var printUtils = new PrintUtils();
            foreach (PrintInfo info in data)
            {
                var fileName = printUtils.PlotCurrentLayout(info, standartCopier);
                if (!string.IsNullOrEmpty(fileName))
                    fileNames.Add(fileName);
            }
            return fileNames.ToArray();
        }
    }
}