using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using YMplugins.Contracts;
using YMplugins.Contracts.Dto;

namespace YMplugins.Models.Autocad2022.AutoPrint
{
    public class NamingService : INamingService
    {
        public ObservableCollection<PrintInfo> GenerateFileName(IEnumerable<PrintInfo> printInfos, int numerationValue)
        {
            throw new NotImplementedException();
        }
    }
}