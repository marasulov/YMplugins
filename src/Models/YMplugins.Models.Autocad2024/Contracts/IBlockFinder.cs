using System.Collections.ObjectModel;
using YMplugins.Contracts.Dto;

namespace YMplugins.Models.Autocad2024.Contracts
{
    public interface IBlockFinder
    {
        ObservableCollection<PrintInfo> FindBlocks(SearchData data);
    }
}