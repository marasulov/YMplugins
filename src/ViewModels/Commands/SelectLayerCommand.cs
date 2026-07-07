using System;
using YMplugins.Contracts;
using YMplugins.ViewModels.VM;

namespace YMplugins.ViewModels.Commands
{
    public class SelectLayerCommand : CommandBase
    {
        private readonly ISelectLayerService _selectLayerService;

        public SelectLayerCommand(ISelectLayerService selectLayerService)
        {
            _selectLayerService = selectLayerService;
        }

        public override bool CanExecute(object parameter) => parameter is AutoPrintVm;

        public override void Execute(object parameter)
        {
            if (parameter is not AutoPrintVm autoPrintVm) return;

            try
            {
                autoPrintVm.CloseAction?.Invoke();

                var layerName = _selectLayerService.SelectLayer();
                if (!string.IsNullOrEmpty(layerName))
                {
                    autoPrintVm.SelectedLayerOnScreen = layerName;
                }

                autoPrintVm.OpenAction?.Invoke();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при выборе слоя: {ex.Message}");
            }
        }
    }
}
