using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.PlottingServices;
#if NET8_0_OR_GREATER
using Gile.AutoCAD.R25.Extension;
#else
using Gile.AutoCAD.R20.Extension;
#endif
using YMplugins.Contracts.Dto;
using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;
using Exception = Autodesk.AutoCAD.Runtime.Exception;

namespace YMplugins.Models.Autocad2024.Utils.Print
{
    public class PrintUtils
    {
        public string PlotCurrentLayout(PrintInfo printModel, StandartCopier standartCopier)
        {
            var acDoc = Active.Document;
            var acCurDb = acDoc.Database;

            Application.SetSystemVariable("BACKGROUNDPLOT", 0);
            string fileName = default;
            
            try
            {
                using var acTrans = acCurDb.TransactionManager.StartTransaction();
                LayoutManager acLayoutMgr = LayoutManager.Current;
                
                if (acLayoutMgr.CurrentLayout != printModel.Space)
                {
                    acLayoutMgr.CurrentLayout = printModel.Space;
                }
                var acLayout = acTrans.GetObject(acLayoutMgr.GetLayoutId(acLayoutMgr.CurrentLayout), OpenMode.ForRead) as Autodesk.AutoCAD.DatabaseServices.Layout;

                // Create an instance of PlotHelper
                PlotHelper plotHelper = new PlotHelper(acDoc, printModel);

                PlotInfo acPlInfo = plotHelper.ConfigurePlotSettings(acLayout, standartCopier);
                var acPlInfoVdr = new PlotInfoValidator();
                acPlInfoVdr.MediaMatchingPolicy = MatchingPolicy.MatchEnabled;
                acPlInfoVdr.Validate(acPlInfo);

                // Execute the plot
                fileName = plotHelper.ExecutePlot(acPlInfo, printModel.FileName);

                acTrans.Commit();
            }
            catch (Exception e)
            {
                Active.Editor.WriteMessage(
                    $"\nЛист {printModel.FileName} не напечатан: {e.Message} (формат {printModel.XDim} x {printModel.YDim} не найден в настройках принтера)");
                fileName = "";
            }

            return fileName;
        }
    }
}