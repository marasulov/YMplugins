using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YMplugins.Contracts;
using YMplugins.Contracts.Dto;

namespace YMplugins.Models.Autocad2022.AutoPrint
{
    public class PrintEngine : IPrintEngine
    {
        public void PrintObjects(IEnumerable<int> objects, string fileName, SearchData data)
        {
            throw new NotImplementedException();
        }
    }
}
