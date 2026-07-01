
using System.Collections.Generic;
using YMplugins.Contracts.Dto;
using YMplugins.Models.Autocad2022.Contracts;

#if NET8_0_OR_GREATER
    using Gile.AutoCAD.R25.Extension;
#else
using Gile.AutoCAD.R20.Extension;
#endif
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

        public List<PrintInfo> FindBlocks(SearchData data)
        {
            var blocks = new List<PrintInfo>();
            if (data.IsSearchOnLayouts)
            {
                blocks.AddRange(_blockSearchService.SearchBlocksInSpace(Active.Database, data.SelectedBlockName, "Layout", ""));
            }
            if (data.IsSearchOnModel)
            {
                blocks.AddRange(_blockSearchService.SearchBlocksInSpace(Active.Database, data.SelectedBlockName, "Model", ""));
            }
            return blocks;
        }

        
    }
}