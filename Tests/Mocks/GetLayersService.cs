using System;
using System.Collections.Generic;
using System.Text;
using YMplugins.Contracts;

namespace Mocks
{
    public class GetLayersService :IGetLayersService
    {
        public List<string> GetLayers()
        {
            var layers = new List<string>();
            for (int i = 0; i < 8; i++)
            {
                var newHoleDto = "layer" + i;
                layers.Add(newHoleDto);
            }

            return layers;
        }

      
    }
}
