namespace WS.Views.WPF
{
    using SimpleInjector;
    using System;

    internal class Program
    {
        [STAThread()]
        public static void Main(string[] args)
        {
            var container = new Container();

            //container.Register<GetEntitiesCommand>();
            //container.Register<PickEntitiesCommand>();
            //container.Register<ZoomEntityCommand>();
            //container.Register<MainViewModel>();
            //container.Register<MainWindow>();

            //container.Register<IGetEntities, GetEntitiesService>();
            //container.Register<IPickEntities, PickEntitiesService>();
            //container.Register<IZoomEntity, ZoomEntityService>();
            //container.Register<IWatchDocument, WatchDocumentService>();

            //var window = container
            //    .GetInstance<MainWindow>();

            //window.ShowDialog();
        }
    }
}
