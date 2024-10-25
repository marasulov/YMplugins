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
                autoPrintVm.CloseAction?.Invoke();

                var selectedBlockId = _selectBlockService.SelectBlock();
                
                autoPrintVm.SelectedBlockOnScreen = selectedBlockId;
                
                autoPrintVm.OpenAction?.Invoke();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при выполнении команды: {ex.Message}");
            }
        }
    }
}