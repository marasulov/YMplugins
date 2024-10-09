using System.Collections.Generic;
using System.Collections.ObjectModel;
using YMplugins.Contracts.Dto;

namespace YMplugins.Contracts
{
    public interface ISearchService
    {
        ObservableCollection<PrintInfo> FindObjects(SearchData searchData);
    }
}