using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Windows;
using System;
using System.Text;
using Gile.AutoCAD.Extension;

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
            if (sb.Length != 0)
            {
                TrayItem ti = new TrayItem();
                ti.ToolTipText = _toolTipText;
                ti.Icon = Active.Document.GetStatusBar().TrayItems[0].Icon;
                Application.StatusBar.TrayItems.Add(ti);
                Application.StatusBar.Update();
                ti.CloseBubbleWindows();
                TrayItemBubbleWindow bw = new TrayItemBubbleWindow();
                bw.Title = _bwTitile;
                bw.Text = sb.ToString();


                bw.IconType = bw.IconType != null ? _icon : IconType.Information;

                if (_drawingPath != null)
                {
                    bw.HyperText = _drawingPath;
                    bw.HyperLink = _drawingPath;
                }

                ti.ShowBubbleWindow(bw);


                bw.Closed += delegate { CloseBw(ti); };
            }


            Application.Idle -= notifyHandler;
            sb.Clear();
            notifyHandler = null;
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
