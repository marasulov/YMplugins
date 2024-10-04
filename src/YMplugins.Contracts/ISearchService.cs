using System.Collections.Generic;
using System.Collections.ObjectModel;
using YMplugins.Contracts.Dto;

namespace YMplugins.Contracts
{
    public interface ISearchService
    {
        List<PrintInfo> FindObjects(SearchData searchData);
    }
}