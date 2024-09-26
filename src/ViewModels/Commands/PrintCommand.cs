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

            var printData = new SearchData
            {
                SelectedPrintByOption = vm.SelectedPrintByOption,
                SelectedPrintingOrder = vm.SelectedPrintingOrder,
                IsSearchOnModel = vm.IsSearchOnModel,
                IsSearchOnLayouts = vm.IsSearchOnLayout,
                SelectedBlockName = vm.SelectedBlockOnScreen,
                AttributeName = vm.SelectedAttr.AttributeName,
                NumerationStartValue = vm.NumerationStartValue,
                Prefix = vm.Prefix,
                Suffix = vm.Suffix
            };
            _printService.Print(printData);
        }
    }
}