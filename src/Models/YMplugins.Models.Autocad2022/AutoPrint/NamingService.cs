using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YMplugins.Contracts;
using YMplugins.Contracts.Dto;

namespace YMplugins.Models.Autocad2022.AutoPrint
{
    public class NamingService : INamingService
    {
        public List<PrintInfo> GenerateFileName(IEnumerable<PrintInfo> printInfos, int numerationValue)
        {
            throw new NotImplementedException();
        }
    }
}
