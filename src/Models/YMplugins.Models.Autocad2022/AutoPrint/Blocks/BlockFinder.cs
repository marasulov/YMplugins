using Gile.AutoCAD.Extension;
using System.Collections.Generic;
using YMplugins.Contracts.Dto;
using YMplugins.Models.Autocad2022.Contracts;

namespace YMplugins.Models.Autocad2022.AutoPrint.Blocks
{
    public class BlockFinder : IObjectFinder
    {
        private readonly BlockSearchService _blockSearchService;
        private readonly SearchData _data;

        public BlockFinder(BlockSearchService blockSearchService, SearchData data)
        {
            _blockSearchService = blockSearchService;
            _data = data;
        }

        public List<PrintInfo> FindObjects()
        {
            var blocks = new List<PrintInfo>();
            if (_data.IsSearchOnLayouts)
            {
                blocks.AddRange(_blockSearchService.SearchBlocksInSpace(Active.Database, _data.SelectedBlockName, "Layout", ""));
            }
            if (_data.IsSearchOnModel)
            {
                blocks.AddRange(_blockSearchService.SearchBlocksInSpace(Active.Database, _data.SelectedBlockName, "Model", ""));
            }
            return blocks;
        }
    }
}