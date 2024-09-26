using YMplugins.Contracts;
using YMplugins.Contracts.Dto;

namespace YMplugins.Models.Autocad2022.AutoPrint
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

        public void Print(PrintInfo data)
        {
            
            //var objectsToPrint = _searchService.FindObjects(data);
            
            //var printInfos = _namingService.GenerateFileName(objectsToPrint, 0);



            //_printEngine.PrintObjects(objectsToPrint, fileName, data);
        }
    }
}