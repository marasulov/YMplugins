using System.Collections.Generic;
using System.Linq;
using YMplugins.Contracts;
using YMplugins.Contracts.Dto;
using YMplugins.Contracts.Dto.Enums;

namespace YMplugins.Models.Autocad2022.AutoPrint
{
    public class SearchService : ISearchService
    {
        public IEnumerable<PrintInfo> FindObjects(string blockName)
        {
            //if (data.SelectedPrintByOption == PrintByOption.ByBlock)
            //{
            //    var blockName = data.SelectedBlockName;

            //    // Логика поиска блоков
            //    return FindBlocksByBlockName(blockName);
            //}
            //else if (data.SelectedPrintByOption == PrintByOption.ByPolyline)
            //{
            //    var layerName = data.SelectedLayer;
            //    // Логика поиска полилиний
            //    return FindPolylines(layerName);
            //}

            return Enumerable.Empty<PrintInfo>();
        }

        private IEnumerable<int> FindBlocksByBlockName(string blockName)
        {
            // Используем AutoCAD API для поиска блоков
            // В зависимости от параметров `IsSearchOnModel` и `IsSearchOnLayouts`
            return new List<int>();
        }

        private IEnumerable<int> FindPolylines(PrintData data)
        {
            // Используем AutoCAD API для поиска полилиний
            return new List<int>();
        }
    }
}
