using System.Linq;
using YMplugins.Contracts;
using YMplugins.Contracts.Dto;
using YMplugins.ViewModels.VM;

namespace YMplugins.ViewModels.Commands
{
    public class PrintCommand : CommandBase
    {
        private IPrintService _printService;

        public PrintCommand(IPrintService printService)
        {
            _printService = printService;
        }

        public bool CanExecute(object parameter)
        {
            return parameter is AutoPrintVm;
        }

        public override void Execute(object parameter)
        {
            var vm = (AutoPrintVm)parameter;
            var printData = vm.BlockDataCollection.ToArray();
            
            _printService.Print(printData);
        }
    }
}