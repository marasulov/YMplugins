using System.Collections.Generic;
using YMplugins.Contracts;

namespace YMplugins.Models.Autocad2022.AutoPrint.Layers;

public class GetLayerFromOpenedDocsService : IGetLayersFromOpenedDocsService
{
    public Dictionary<string, List<string>> GetLayersFromAllOpenDocuments()
    {
        return DbCad.LayersExtension.GetLayersFromAllOpenDocuments();
    }
}