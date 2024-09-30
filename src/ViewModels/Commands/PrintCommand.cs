using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using YMplugins.Contracts;
using YMplugins.ViewModels.VM;
using static System.Net.Mime.MediaTypeNames;

namespace YMplugins.ViewModels.Commands
{
    public class PrintCommand : CommandBase
    {
        private IPrintService _printService;
        private ICombinePdfService _combinePdfService;
        private INotifyService _notifyService;
        private readonly IWindowService _windowService;

        public PrintCommand(IPrintService printService, ICombinePdfService combinePdfService,
            INotifyService notifyService, IWindowService windowService)
        {
            _printService = printService;
            _combinePdfService = combinePdfService;
            _notifyService = notifyService;
            _windowService = windowService;
        }

        public bool CanExecute(object parameter)
        {
            return parameter is AutoPrintVm;
        }

        public override async void Execute(object parameter)
        {
            _windowService.ShowLoadingWindow();

            try
            {
                var vm = (AutoPrintVm)parameter;
                var printData = vm.BlockDataCollection.ToArray();

                var fileNames = await Task.Run(() => _printService.Print(printData));
                var joinedBubbleTexts = string.Join("\n", fileNames);

                if (vm.IsCombinePdf)
                {
                    joinedBubbleTexts = Path.Combine(_combinePdfService.Combine(fileNames, vm.OutputFileName), ".pdf");
                }

                _notifyService.Notify("Работа завершена!");
                _notifyService.Notify(joinedBubbleTexts);
            }
            catch (Exception ex)
            {
                _notifyService.Notify($"Произошла ошибка: {ex.Message}");
            }
            finally
            {
                
                _windowService.CloseLoadingWindow();
            }
        }
    }
}