using System.Collections.Generic;
using System.Collections.ObjectModel;
using YMplugins.Contracts.Dto;

namespace YMplugins.Models.Autocad2022.Contracts
{
    public interface IPolylineFinder
    {
        ObservableCollection<PrintInfo> FindPolylines(SearchData data);
    }
}