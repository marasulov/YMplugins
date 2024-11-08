using System.Collections.Generic;
using YMplugins.Contracts;
using YMplugins.Models.DbCad;

namespace YMplugins.Models.Autocad2022.AutoPrint.Blocks;

public class GetBlocksFromOpenedDocsService : IGetBlocksFromOpenedDocsService
{
    public Dictionary<string, List<string>> GetBlocksFromAllOpenDocuments()
    {
        return  BlocksExtension.GetBlocksFromAllOpenDocuments();
    }
}