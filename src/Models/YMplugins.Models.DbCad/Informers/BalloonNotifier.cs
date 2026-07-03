using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Windows;
using System;
using System.Text;
#if NET8_0_OR_GREATER
using Gile.AutoCAD.R25.Extension;
#else
using Gile.AutoCAD.R20.Extension;
#endif

namespace YMplugins.Models.DbCad.Informers
{
    public class BalloonNotifier : IInformer
    {
        private StringBuilder sb;
        private string _toolTipText;
        private string _bwTitile;
        EventHandler notifyHandler = null;
        private IconType _icon;
        private string _drawingPath;

        public BalloonNotifier(string toolTipText, string bwTitle)
        {
            sb = new StringBuilder();
            _toolTipText = toolTipText;
            _bwTitile = bwTitle;
        }

        public BalloonNotifier(string toolTipText, string bwTitle, IconType icon)
        {
            sb = new StringBuilder();
            _toolTipText = toolTipText;
            _bwTitile = bwTitle;
            _icon = icon;
        }

        public BalloonNotifier(string toolTipText, string bwTitle, string drawingPath)
        {
            sb = new StringBuilder();
            _toolTipText = toolTipText;
            _bwTitile = bwTitle;
            _drawingPath = drawingPath;

        }

        public void Notify(string message)
        {
            sb.AppendLine(message);
            if (notifyHandler == null)
            {
                notifyHandler = new EventHandler(Application_Idle);
                Application.Idle += notifyHandler;
            }
        }

        void Application_Idle(object sender, EventArgs e)
        {
            // Отписка строго до показа: если балун упадёт, обработчик не должен
            // остаться подписанным и валиться на каждом следующем Idle.
            Application.Idle -= notifyHandler;
            notifyHandler = null;

            var text = sb.ToString();
            sb.Clear();
            if (text.Length == 0) return;

            try
            {
                ShowBubble(text);
            }
            catch (System.Exception)
            {
                // Балун недоступен (например, пустой трей) — сообщение не теряем
                Application.DocumentManager.MdiActiveDocument?.Editor
                    .WriteMessage($"\n{_bwTitile}:\n{text}");
            }
        }

        void ShowBubble(string text)
        {
            TrayItem ti = new TrayItem();
            ti.ToolTipText = _toolTipText;

            // В AutoCAD 2025 трей документа может быть пуст
            var docTrayItems = Active.Document.GetStatusBar().TrayItems;
            if (docTrayItems.Count > 0)
            {
                ti.Icon = docTrayItems[0].Icon;
            }

            Application.StatusBar.TrayItems.Add(ti);
            Application.StatusBar.Update();
            ti.CloseBubbleWindows();

            TrayItemBubbleWindow bw = new TrayItemBubbleWindow();
            bw.Title = _bwTitile;
            bw.Text = text;
            bw.IconType = _icon == default ? IconType.Information : _icon;

            if (_drawingPath != null)
            {
                bw.HyperText = _drawingPath;
                bw.HyperLink = _drawingPath;
            }

            ti.ShowBubbleWindow(bw);

            bw.Closed += delegate { CloseBw(ti); };
        }

        void CloseBw(TrayItem ti)
        {
            try
            {
                Application.StatusBar.TrayItems.Remove(ti);
                Application.StatusBar.Update();
            }
            catch
            { }
        }

        public void Alert(string message)
        {
            throw new NotImplementedException();
        }

        public void Warning(string message)
        {
            throw new NotImplementedException();
        }

        public void Log(string message)
        {
            throw new NotImplementedException();
        }
    }
}
