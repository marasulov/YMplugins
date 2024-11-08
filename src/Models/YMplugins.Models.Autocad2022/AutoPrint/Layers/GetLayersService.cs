using System.Collections.Generic;
using YMplugins.Contracts;

namespace YMplugins.Models.Autocad2022.AutoPrint.Layers
{
    public class GetLayersService : IGetLayersService
    {
        public List<string> GetLayers()
        {
            return DbCad.LayersExtension.GetLayers();
        }

        
    }
}