using Gile.AutoCAD.Extension;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using YMplugins.Contracts.Dto;
using YMplugins.Models.Autocad2022.Contracts;

namespace YMplugins.Models.Autocad2022.AutoPrint.Blocks
{
    public class BlockFinder : IBlockFinder
    {
        private readonly BlockSearchService _blockSearchService;
        private readonly SearchData _data;

        public BlockFinder(BlockSearchService blockSearchService, SearchData data)
        {
            _blockSearchService = blockSearchService;
            _data = data;
        }

        public ObservableCollection<PrintInfo> FindBlocks(SearchData data)
        {
            var blocks = new List<PrintInfo>();
            string searchSpace = default;
            if (data.IsSearchOnLayouts & data.IsSearchOnModel) searchSpace = "Both";
            else if (data.IsSearchOnModel) searchSpace = "Model";
            else searchSpace = "Layout";
            
            if (data.IsCheckedNumbering)
            {
                blocks.AddRange(_blockSearchService.SearchAllBlocksInSpaceByName(Active.Database, data.SelectedBlockName, searchSpace, data.NumerationStartValue));
            }
            else
            {
                blocks.AddRange(_blockSearchService.SearchAllBlocksInSpaceByName(Active.Database, data.SelectedBlockName, searchSpace));
            }

            return new ObservableCollection<PrintInfo>(blocks);
        }


    }
}