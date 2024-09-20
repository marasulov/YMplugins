using Dreambuild.AutoCAD;
using System.Collections.Generic;
using System.Linq;
using YMplugins.Contracts;

namespace YMplugins.Models.Autocad2022.AutoPrint
{
    public class GetLayersService : IGetLayersService
    {
        public List<string> GetLayers()
        {
            return DbHelper.GetAllLayerNames().ToList();
        }
    }
}