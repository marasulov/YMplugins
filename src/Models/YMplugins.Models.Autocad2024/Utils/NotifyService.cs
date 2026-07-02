using Autodesk.AutoCAD.ApplicationServices;
using System.IO;
using YMplugins.Contracts;
using YMplugins.Models.DbCad.Informers;

namespace YMplugins.Models.Autocad2024.Utils
{
    public class NotifyService : INotifyService
    {
        public void Notify(string message)
        {
            string drawingPath = Path.GetDirectoryName(Application.DocumentManager.CurrentDocument.Database.Filename);
            if (drawingPath != null)
            {
                BalloonNotifier notifier = new BalloonNotifier("Создание файлов успешно завершено", "Создание файлов успешно завершено", drawingPath);
                notifier.Notify(message);
            }
        }
    }
}