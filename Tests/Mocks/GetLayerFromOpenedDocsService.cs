using System.Collections.Generic;
using YMplugins.Contracts;

namespace Mocks
{
    public class GetLayerFromOpenedDocsService : IGetLayersFromOpenedDocsService, IGetLayersService
    {
        public Dictionary<string, List<string>> GetLayersFromAllOpenDocuments()
        {
            var dictLayers = new Dictionary<string, List<string>>();
            for (int i = 0; i < 15; i++)
            {
                var newHoleDto = "file" + i;

                dictLayers.Add(newHoleDto, GetLayers());
            }

            return dictLayers;
        }

        public List<string> GetLayers()
        {
            var layers = new List<string>();
            for (int i = 0; i < 15; i++)
            {
                var newHoleDto = "layer" + i;

                layers.Add(newHoleDto);
            }
            
            return layers;
        }
    }
}