using System;
using System.Collections.Generic;
using System.Text;
using YMplugins.Contracts;

namespace Mocks
{
    public class GetLayerService :IGetLayersService
    {
        public List<string> GetLayers()
        {
            var layers = new List<string>();
            for (int i = 0; i < 50; i++)
            {
                var newHoleDto = "layer" + i;
                layers.Add(newHoleDto);
            }

            return layers;
        }
    }
}
