using Dreambuild.AutoCAD;
using System.Collections.Generic;
using System.Linq;
using YMplugins.Contracts;

namespace YMplugins.Models.Autocad2022.AutoPrint
{
    public class GetBlocksNameService : IGetBlocksNameService
    {
        public List<string> GetBlocksName()
        {
            //return DbHelper.GetAllBlockNames().ToList();
            return default;
        }
    }
}
