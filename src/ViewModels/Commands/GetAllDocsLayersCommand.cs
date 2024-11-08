using System.Collections.Generic;
using System;
using System.Linq;
using YMplugins.Contracts;
using YMplugins.ViewModels.VM;

namespace YMplugins.ViewModels.Commands
{
    public class GetAllDocsLayersCommand : CommandBase
    {
        private readonly IGetLayersFromOpenedDocsService _getLayersService;

        public GetAllDocsLayersCommand(IGetLayersFromOpenedDocsService getLayersService)
        {
            _getLayersService = getLayersService;
        }

        public bool CanExecute(object parameter)
        {
            return parameter is AutoPrintVm;
        }
        public event Action<List<string>> ResultObtained;
        public override void Execute(object parameter)
        {

            var res = _getLayersService.GetLayersFromAllOpenDocuments();

            List<string> uniqueValues = res
                .SelectMany(kvp => kvp.Value)  
                .Distinct()                   
                .ToList();
            
            ResultObtained?.Invoke(uniqueValues);
        }

    }
}