using System;
using YMplugins.Contracts;

namespace YMplugins.ViewModels.Commands
{
    public class ZoomToPointCommand : CommandBase
    {
        private readonly IZoomEntity _intersectionPointZoom;

        public ZoomToPointCommand(IZoomEntity intersectionPointZoom)
        {
            _intersectionPointZoom = intersectionPointZoom;
        }

        public event EventHandler? CanExecuteChanged;

        //public bool CanExecute(object parameter)
        //{
        //    var vm = (AutoPrintVm?)parameter;

        //    return vm?.SelectBlockCommand is not null;
        //}

        public override void Execute(object parameter)
        {
            var id = Convert.ToInt32(parameter);

            _intersectionPointZoom.Zoom(id);
        }
    }
}