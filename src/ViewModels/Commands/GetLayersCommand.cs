using System.Collections.Generic;
using System;
using YMplugins.Contracts;
using YMplugins.ViewModels.VM;

namespace YMplugins.ViewModels.Commands
{
    public class GetLayersCommand : CommandBase
    {
        private readonly IGetLayersService _getLayersService;

        public GetLayersCommand(IGetLayersService getLayersService)
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

            var res = _getLayersService.GetLayers();

            ResultObtained?.Invoke(res);
        }

    }
}