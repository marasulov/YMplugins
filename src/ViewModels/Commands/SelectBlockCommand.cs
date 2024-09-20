using System;
using YMplugins.Contracts;
using YMplugins.ViewModels.VM;

namespace YMplugins.ViewModels.Commands
{
    public class SelectBlockCommand : CommandBase
    {
        private ISelectBlockService _selectBlockService;
        public SelectBlockCommand(ISelectBlockService selectBlockService)
        {
            _selectBlockService = selectBlockService;
        }

        public bool CanExecute(object parameter)
        {
            return parameter is AutoPrintVm;
        }
        public override void Execute(object parameter)
        {
            //var holesVm = (AutoPrintVm)parameter;
            //holesVm.CloseAction?.Invoke();

            //var selectedBlockId = await Task.Run(() => _selectBlockService.SelectBlock());
            //await Task.Delay(200);
            //holesVm.OpenAction?.Invoke();

            if (parameter is not AutoPrintVm autoPrintVm) return;
            try
            {
                Console.WriteLine("Закрываем текущее окно...");
                autoPrintVm.CloseAction?.Invoke();

                // Выполняем выбор блока синхронно (без Task.Run)
                var selectedBlockId = _selectBlockService.SelectBlock();
                Console.WriteLine($"Выбранный блок: {selectedBlockId}");

                // Обновляем свойство в ViewModel
                autoPrintVm.SelectedBlockOnScreen = selectedBlockId;
                Console.WriteLine("Свойство SelectedBlockOnScreen обновлено.");

                // Открываем окно заново
                Console.WriteLine("Открываем окно заново...");
                autoPrintVm.OpenAction?.Invoke();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при выполнении команды: {ex.Message}");
            }
        }
    }
}
