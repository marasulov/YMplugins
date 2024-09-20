using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YMplugins.Contracts;
using YMplugins.Contracts.Dto;
using YMplugins.Contracts.Dto.Enums;

namespace Mocks
{
    public class SearchService : ISearchService
    {

        private readonly INamingService _nameService;
        public SearchService(INamingService nameService)
        {
            _nameService = nameService;
        }
        public IEnumerable<PrintInfo> FindObjects(string blockName)
        {
            //if (data.SelectedPrintByOption == PrintByOption.ByBlock)
            //{
            //    // Логика поиска блоков
            //    return FindBlocks(data);
            //}
            //else if (data.SelectedPrintByOption == PrintByOption.ByPolyline)
            //{
            //    // Логика поиска полилиний
            //    return FindPolylines(data);
            //}

            //return Enumerable.Empty<PrintInfo>();
            return FindBlocks(blockName);
        }

        private IEnumerable<PrintInfo> FindBlocks(string blockname)
        {
            var fileName = _nameService
            var layers = new List<PrintInfo>();
            for (int i = 0; i < 50; i++)
            {
                var newHoleDto = new PrintInfo(i, $"{blockname} + {i}", $"формат + {blockname} + {i}");
                
                layers.Add(newHoleDto);
            }

            return layers;
            
        }

        private IEnumerable<int> FindPolylines(PrintData data)
        {
            // Используем AutoCAD API для поиска полилиний
            return new List<int>();
        }

    }
}
