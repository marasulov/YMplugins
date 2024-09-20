using System.Collections.Generic;
using YMplugins.Contracts.Dto;

namespace YMplugins.Contracts
{
    public interface ISearchService
    {
        IEnumerable<PrintInfo> FindObjects(string blockName);
    }
}