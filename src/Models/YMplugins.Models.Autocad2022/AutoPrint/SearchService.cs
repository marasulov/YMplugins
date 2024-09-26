using System.Collections.Generic;
using System.Collections.ObjectModel;
using Gile.AutoCAD.Extension;
using YMplugins.Contracts;
using YMplugins.Contracts.Dto;
using YMplugins.Contracts.Dto.Enums;

namespace YMplugins.Models.Autocad2022.AutoPrint
{
    public class SearchService : ISearchService
    {
        private readonly BlockSearchService _blockSearchService;

        private readonly SearchData _searchData;

        public SearchService(BlockSearchService blockSearchService, SearchData searchData)
        {
            _blockSearchService = blockSearchService;
            _searchData = searchData;
        }
        //public IEnumerable<PrintInfo> FindObjects(string blockName)
        //{
        //    if (data.SelectedPrintByOption == PrintByOption.ByBlock)
        //    {
        //        var blockName = data.SelectedBlockName;

        //        // Логика поиска блоков
        //        return FindBlocksByBlockName(blockName);
        //    }
        //    else if (data.SelectedPrintByOption == PrintByOption.ByPolyline)
        //    {
        //        var layerName = data.SelectedLayer;
        //        // Логика поиска полилиний
        //        return FindPolylines(layerName);
        //    }

        //    return Enumerable.Empty<PrintInfo>();
        //}

        private List<PrintInfo> FindBlocksByBlockName(string blockName, SearchData data)
        {
            // Используем AutoCAD API для поиска блоков
            // В зависимости от параметров `IsSearchOnModel` и `IsSearchOnLayouts`
            var blocks = new List<PrintInfo>();
            if (data.IsSearchOnLayouts)
            {
                blocks = _blockSearchService.SearchBlocksInSpace(Active.Database, blockName, "Layout", "");
            }
            if(data.IsSearchOnModel)
            {
                blocks = _blockSearchService.SearchBlocksInSpace(Active.Database, blockName, "Model", "");
            }
            return blocks;
        }

        public ObservableCollection<PrintInfo> FindObjects(SearchData data)
        {
            var blocks = new ObservableCollection<PrintInfo>();

            if (_searchData.SelectedPrintByOption == PrintByOption.ByBlock)
            {
                var blockName = data.SelectedBlockName;

                // Логика поиска блоков
                blocks = new ObservableCollection<PrintInfo>(FindBlocksByBlockName(blockName, data));
                
            }
            //else if (_searchData.SelectedPrintByOption == PrintByOption.ByPolyline)
            //{
            //    var layerName = _searchData.SelectedLayer;
            //    // Логика поиска полилиний
            //    return FindPolylines(layerName);
            //}
            return blocks;
        }
    }
}
