using System.Collections.ObjectModel;
using YMplugins.Contracts;
using YMplugins.Contracts.Dto;
using YMplugins.Contracts.Dto.Enums;
using YMplugins.Models.Autocad2024.Contracts;

namespace YMplugins.Models.Autocad2024.AutoPrint.Blocks
{
    public class SearchService : ISearchService
    {
        private readonly IBlockFinder _blockFinder;
        private readonly IPolylineFinder _polylineFinder;

        public SearchService(IBlockFinder blockFinder, IPolylineFinder polylineFinder)
        {
            _blockFinder = blockFinder;
            _polylineFinder = polylineFinder;
        }

        public ObservableCollection<PrintInfo> FindObjects(SearchData data)
        {
            if (data.SelectedBlockName == null && data.SelectedLayer == null)
                return new ObservableCollection<PrintInfo>();

            return data.SelectedPrintByOption switch
            {
                PrintByOption.ByBlock => _blockFinder.FindBlocks(data),
                PrintByOption.ByPolyline => _polylineFinder.FindPolylines(data),
                _ => new ObservableCollection<PrintInfo>()
            };
        }
    }
}