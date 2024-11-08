using System.Collections.Generic;

namespace YMplugins.Contracts
{
    public interface IGetBlocksFromOpenedDocsService
    {
        Dictionary<string, List<string>> GetBlocksFromAllOpenDocuments();
    }
}