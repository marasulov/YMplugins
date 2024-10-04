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
        public List<PrintInfo> FindObjects(SearchData data)
        {
            if (data.SelectedPrintByOption == PrintByOption.ByBlock)
            {
                // Логика поиска блоков
                return FindBlocks(data);
            }
            else if (data.SelectedPrintByOption == PrintByOption.ByPolyline)
            {
                // Логика поиска полилиний
                return FindPolylines(data);
            }

            return default;
        }

        private List<PrintInfo> FindBlocks(SearchData data)
        {
            var layers = new List<PrintInfo>();
            for (int i = 0; i < 50; i++)
            {
                //var fileName = _nameService.GenerateFileName(default,0);
                var newHoleDto = new PrintInfo(i, $"space for {i}", $"format for {i}", 1, 1, 1, new PointDTO(2, 3, 4), true);

                layers.Add(newHoleDto);
            }

            return layers;
            
        }

        private List<PrintInfo> FindPolylines(SearchData data)
        {
            var layers = new List<PrintInfo>();
            for (int i = 0; i < 50; i++)
            {
                //var fileName = _nameService.GenerateFileName(default,0);
                var newHoleDto = new PrintInfo(i, $"space for {data.SelectedLayer}", $"format for {i}", 1, 1, 1, new PointDTO(2, 3, 4), true);

                layers.Add(newHoleDto);
            }

            return layers;
        }

        public ObservableCollection<PrintInfo> FindObjects()
        {
            var printInfos = new ObservableCollection<PrintInfo>();
            for (int i = 0; i < 50; i++)
            {
                printInfos.Add(new PrintInfo(i,$"space for {i}", $"format for {i}",1,1,1, new PointDTO(2,3,4), true));
            }
            return printInfos;
        }
    }
}
