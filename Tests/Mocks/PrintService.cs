using System.Diagnostics;
using YMplugins.Contracts;
using YMplugins.Contracts.Dto;

namespace Mocks
{
    public class PrintService : IPrintService
    {
        private readonly ISearchService _searchService;
        private readonly INamingService _namingService;
        private readonly IPrintEngine _printEngine;

        public PrintService(ISearchService searchService, INamingService namingService, IPrintEngine printEngine)
        {
            _searchService = searchService;
            _namingService = namingService;
            _printEngine = printEngine;
        }

        public void Print(PrintData data)
        {
            var blockname = data.SelectedBlockName;
            var objectsToPrint = _searchService.FindObjects(blockname);

            //var fileName = _namingService.GenerateFileName(data, objectsToPrint);

            //_printEngine.PrintObjects(objectsToPrint, fileName, data);
        }

        
    }
}