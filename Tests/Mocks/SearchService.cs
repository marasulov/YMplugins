using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public ObservableCollection<PrintInfo> FindObjects(SearchData data)
        {
            if (data.SelectedPrintByOption == PrintByOption.ByBlock)
            {
                // Логика поиска блоков
                return FindBlocks(data);
            }
            //else if (data.SelectedPrintByOption == PrintByOption.ByPolyline)
            //{
            //    // Логика поиска полилиний
            //    return FindPolylines(data);
            //}

            return default;
        }

        private ObservableCollection<PrintInfo> FindBlocks(SearchData data)
        {
            var layers = new ObservableCollection<PrintInfo>();
            for (int i = 0; i < 50; i++)
            {
                //var fileName = _nameService.GenerateFileName(default,0);
                var newHoleDto = new PrintInfo(i, $"space for {i}", $"format for {i}", new PointDTO(5, 6, 5), 1, 1, 1, new PointDTO(2, 3, 4), "");

                layers.Add(newHoleDto);
            }

            return layers;
            
        }

        private IEnumerable<int> FindPolylines(SearchData data)
        {
            // Используем AutoCAD API для поиска полилиний
            return new List<int>();
        }

        public ObservableCollection<PrintInfo> FindObjects()
        {
            var printInfos = new ObservableCollection<PrintInfo>();
            for (int i = 0; i < 50; i++)
            {
                printInfos.Add(new PrintInfo(i,$"space for {i}", $"format for {i}", new PointDTO(5,6,5),1,1,1, new PointDTO(2,3,4), ""));
            }
            return printInfos;
        }
    }
}
