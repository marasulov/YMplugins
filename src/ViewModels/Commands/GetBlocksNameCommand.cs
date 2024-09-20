using System;
using System.Collections.Generic;
using YMplugins.Contracts;
using YMplugins.ViewModels.VM;

namespace YMplugins.ViewModels.Commands
{
    public class GetBlocksNameCommand : CommandBase
    {
        private readonly IGetBlocksNameService _getBlocksNameService;

        public GetBlocksNameCommand(IGetBlocksNameService getBlocksNameService)
        {
            _getBlocksNameService = getBlocksNameService;
        }

        public bool CanExecute(object parameter)
        {
            return parameter is AutoPrintVm;
        }
        public event Action<List<string>> ResultObtained;
        public override void Execute(object parameter)
        {

            var res = _getBlocksNameService.GetBlocksName();

            ResultObtained?.Invoke(res);
        }
    }
}