using Gile.AutoCAD.Extension;
using System;
using System.Collections.Generic;
using YMplugins.Contracts.Dto;
using YMplugins.Models.Autocad2022.Contracts;

namespace YMplugins.Models.Autocad2022.AutoPrint.Blocks
{
    public class BlockFinder : IBlockFinder
    {
        private readonly BlockSearchService _blockSearchService;

        public BlockFinder(BlockSearchService blockSearchService)
        {
            _blockSearchService = blockSearchService;
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
