using System;
using System.Collections.Generic;
using System.Diagnostics;
using YMplugins.Contracts;
using YMplugins.Contracts.Dto;

namespace Mocks
{
    public class PrintService : IPrintService
    {
  
       
        public string[] Print(PrintInfo[] data)
        {
            var filenames = new List<string>();
            foreach (var printInfo in data)
            {
                Console.WriteLine(printInfo.TargetFileName);
                filenames.Add(printInfo.TargetFileName);
            }

            return filenames.ToArray();
        }
    }
}