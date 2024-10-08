using System;
using System.Collections.Generic;
using YMplugins.Contracts;
using YMplugins.Contracts.Dto;
using YMplugins.Contracts.Dto.Enums;
using YMplugins.Models.Autocad2022.Contracts;

namespace YMplugins.Models.Autocad2022.AutoPrint.Blocks
{
    public class SearchService : ISearchService
    {
        private readonly Dictionary<PrintByOption, IObjectFinder> _strategies;

        public SearchService(Dictionary<PrintByOption, IObjectFinder> strategies)
        {
            _strategies = strategies;
        }

        public List<PrintInfo> FindObjects(SearchData data)
        {
            if (!_strategies.ContainsKey(data.SelectedPrintByOption))
                throw new ArgumentException("Unsupported search option");

            // Получаем нужный IObjectFinder и вызываем FindObjects
            var objectFinder = _strategies[data.SelectedPrintByOption];

            // Передаем SearchData через его конструктор, если он его ожидает
            return objectFinder.FindObjects();
        }
    }
}