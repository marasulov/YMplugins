using System.Collections.Generic;

namespace YMplugins.Contracts
{
    public interface IGetLayersFromOpenedDocsService
    {
        Dictionary<string, List<string>> GetLayersFromAllOpenDocuments();
    }
}