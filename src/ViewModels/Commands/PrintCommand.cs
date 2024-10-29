using System.Linq;
using System.Net.Http.Headers;
using YMplugins.Contracts;
using YMplugins.Contracts.Dto;
using YMplugins.Contracts.Dto.Enums;
using YMplugins.ViewModels.VM;

namespace YMplugins.ViewModels.Commands
{
    public class PrintCommand : CommandBase
    {
        private IPrintService _printService;
        private ICombinePdfService _combinePdfService;
        private INotifyService _notifyService;
        private readonly IDeleteEmptyLayoutsService _deleteEmptyLayouts;
        private readonly ISetLayoutPlotSettingService _setLayoutPlot;
        private ICreateDwgService _createDwgService;

        public PrintCommand(IPrintService printService, ICombinePdfService combinePdfService,
            INotifyService notifyService, IDeleteEmptyLayoutsService deleteEmptyLayouts, ISetLayoutPlotSettingService setLayoutPlot, ICreateDwgService createDwgService)
        {
            _printService = printService;
            _combinePdfService = combinePdfService;
            _notifyService = notifyService;
            _deleteEmptyLayouts = deleteEmptyLayouts;
            _setLayoutPlot = setLayoutPlot;
            _createDwgService = createDwgService;
        }

        //public override async void Execute(object parameter)
        //{
        //    _windowService.ShowLoadingWindow();

        //    try
        //    {
        //        var vm = (AutoPrintVm)parameter;
        //        var printData = vm.PrintDataCollection.ToArray();

        //        var fileNames = await Task.Run(() => _printService.Print(printData));
        //        var joinedBubbleTexts = string.Join("\n", fileNames);

        //        if (vm.IsCombinePdf)
        //        {
        //            joinedBubbleTexts = Path.Combine(_combinePdfService.Combine(fileNames, vm.OutputFileName), ".pdf");
        //        }

        //        _notifyService.Notify("Работа завершена!");
        //        _notifyService.Notify(joinedBubbleTexts);
        //    }
        //    catch (Exception ex)
        //    {
        //        _notifyService.Notify($"Произошла ошибка: {ex.Message}");
        //    }
        //    finally
        //    {
        //        _windowService.CloseLoadingWindow();
        //    }
        //}

        public override void Execute(object parameter)
        {
            var vm = (AutoPrintVm)parameter;
            vm.Error = string.Empty;
            if (vm.PrintDataCollection == null || !vm.PrintDataCollection.Any())
            {
                vm.Error = string.Join("\n", "Block not selected");
                return;
            }

            var emptyFileNameBlocks = vm.PrintDataCollection.Where(b => string.IsNullOrWhiteSpace(b.FileName)).ToList();
            if (emptyFileNameBlocks.Any())
            {
                vm.Error = "File name is absent.";
                return;
            }

            if (vm.IsCombinePdf && string.IsNullOrWhiteSpace(vm.OutputFileName))
            {
                vm.Error = "Output file name is required when combining PDFs.";
                return;


            }
            
            vm.CloseAction?.Invoke();

            var printData = vm.PrintDataCollection.Where(x => x.IsPrint).ToArray();

            if (vm.IsSetLayoutsToPlotSetting)
            {
                _setLayoutPlot.Set(printData);
            }

            if (vm.IsDeleteEmptyLayouts)
            {
                _deleteEmptyLayouts.DeleteEmptyLayouts(printData);
            }


            if (vm.SelectedPrintingOrder == PrintingOrder.ByX)
            {
                printData = vm.PrintDataCollection.OrderBy(x => x.Position.X).ToArray();
            }
            else if (vm.SelectedPrintingOrder == PrintingOrder.ByY)
            {
                printData = vm.PrintDataCollection.OrderByDescending(x => x.Position.Y).ToArray();
            }

            string[] fileNames = new string[printData.Length];

            if (vm.IsCreatePdf)
            {
                fileNames = _printService.Print(printData);
            }
            else
            {
                fileNames = _createDwgService.Create(printData);
            }

            var joinedBubbleTexts = string.Join("\n", fileNames);
            if (vm.IsCombinePdf)
                joinedBubbleTexts = _combinePdfService.Combine(fileNames, string.Join("", vm.OutputFileName, ".pdf"));

            _notifyService.Notify(joinedBubbleTexts);
        }
    }
}