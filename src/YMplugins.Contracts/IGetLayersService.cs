using System.Collections.Generic;

namespace YMplugins.Contracts
{
    public interface IGetLayersService
    {
        List<string> GetLayers();
        //Dictionary<string, List<string>> GetLayersFromAllOpenDocuments();
    }
}