using Autodesk.AutoCAD.ApplicationServices;
using System.IO;
using YMplugins.Contracts;
using YMplugins.Contracts.Localization;
using YMplugins.Models.DbCad.Informers;

namespace YMplugins.Models.Autocad2024.Utils
{
    public class NotifyService : INotifyService
    {
        public void Notify(string message)
        {
            Notify(message, Tr.FilesCreatedTitle);
        }

        public void Notify(string message, string title)
        {
            string drawingPath = Path.GetDirectoryName(Application.DocumentManager.CurrentDocument.Database.Filename);
            if (drawingPath != null)
            {
                BalloonNotifier notifier = new BalloonNotifier(title, title, drawingPath);
                notifier.Notify(message);
            }
        }
    }
}