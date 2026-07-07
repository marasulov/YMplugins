#if NET8_0_OR_GREATER
using Gile.AutoCAD.R25.Extension;
#else
using Gile.AutoCAD.R20.Extension;
#endif
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

            // Печать выполняется из обработчика WPF-окна, вне контекста команды
            // с автоблокировкой, поэтому смену листа и настроек печати нужно
            // защитить явной блокировкой документа (иначе eLockViolation).
            using (Active.Document.LockDocument())
            {
                foreach (PrintInfo info in data)
                {
                    var fileName = printUtils.PlotCurrentLayout(info, standartCopier);
                    if (!string.IsNullOrEmpty(fileName))
                        fileNames.Add(fileName);
                }
            }

            return fileNames.ToArray();
        }
    }
}