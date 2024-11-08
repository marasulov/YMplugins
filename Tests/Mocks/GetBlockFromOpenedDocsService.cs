using System.Collections.Generic;
using YMplugins.Contracts;

namespace Mocks
{
    public class GetBlockFromOpenedDocsService : IGetBlocksFromOpenedDocsService
    {
        public List<string> GetLayers()
        {
            var layers = new List<string>();
            for (int i = 0; i < 15; i++)
            {
                var newHoleDto = "blocklayer" + i;

                layers.Add(newHoleDto);
            }
            
            return layers;
        }

        public Dictionary<string, List<string>> GetBlocksFromAllOpenDocuments()
        {
            var dictLayers = new Dictionary<string, List<string>>();
            for (int i = 0; i < 15; i++)
            {
                var newHoleDto = "block" + i;

                dictLayers.Add(newHoleDto, GetLayers());
            }

            return dictLayers;
        }
    }
}