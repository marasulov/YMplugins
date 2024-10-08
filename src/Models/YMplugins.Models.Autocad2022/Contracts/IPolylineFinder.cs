using System.Collections.Generic;
using YMplugins.Contracts.Dto;

namespace YMplugins.Models.Autocad2022.Contracts
{
    public interface IPolylineFinder
    {
        List<PrintInfo> FindPolylines(SearchData data);
    }
}
