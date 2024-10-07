using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.PlottingServices;
using System;
using System.IO;
using YMplugins.Contracts.Dto;
using PlotType = Autodesk.AutoCAD.DatabaseServices.PlotType;

namespace YMplugins.Models.Autocad2022.Utils.Print
{
    public class PlotHelper
    {
        private Document _document;
        private Database _database;
        private PrintInfo _printModel;

        public PlotHelper(Document document, PrintInfo printModel)
        {
            _document = document;
            _database = document.Database;
            _printModel = printModel;
        }

        public PlotInfo ConfigurePlotSettings(Layout acLayout)
        {
            var acPlInfo = new PlotInfo();
            acPlInfo.Layout = acLayout.ObjectId;

            var acPlSet = new PlotSettings(acLayout.ModelType);
            acPlSet.CopyFrom(acLayout);

            var acPlSetVdr = PlotSettingsValidator.Current;

            var blockPosition = new Point2d(_printModel.Position.X, _printModel.Position.Y);
            var blockDimension = new Point2d(blockPosition.X + _printModel.Width, blockPosition.Y + _printModel.Length);
            //var blockDimension = new Point2d(_printModel.Position2.X, _printModel.Position2.Y);
            var points = new Extents2d(blockPosition, blockDimension);

            bool isHor = _printModel.IsFormatHorizontal();
            CanonNameResolver resolver = new CanonNameResolver();

            //TODO сделать поиск канонического имени
            string canonName = resolver.GetCanonNameByWidthAndHeight(_printModel);

            acPlSetVdr.SetPlotWindowArea(acPlSet, points);
            acPlSetVdr.SetPlotType(acPlSet, PlotType.Window);
            acPlSetVdr.SetPlotRotation(acPlSet, !isHor ? PlotRotation.Degrees090 : PlotRotation.Degrees000);
            acPlSetVdr.SetUseStandardScale(acPlSet, false);
            acPlSetVdr.SetStdScaleType(acPlSet, StdScaleType.ScaleToFit);
            acPlSetVdr.SetPlotCentered(acPlSet, true);
            acPlSetVdr.SetPlotConfigurationName(acPlSet, "DWG_To_PDF_Uzle.pc3", canonName);

            acPlInfo.OverrideSettings = acPlSet;

            return acPlInfo;
        }

        public PlotProgressDialog CreatePlotProgressDialog()
        {
            var acPlProgDlg = new PlotProgressDialog(false, 1, true);
            acPlProgDlg.set_PlotMsgString(PlotMessageIndex.DialogTitle, "Plot Progress");
            acPlProgDlg.set_PlotMsgString(PlotMessageIndex.CancelJobButtonMessage, "Cancel Job");
            acPlProgDlg.set_PlotMsgString(PlotMessageIndex.CancelSheetButtonMessage, "Cancel Sheet");
            acPlProgDlg.set_PlotMsgString(PlotMessageIndex.SheetSetProgressCaption, "Sheet Set Progress");
            acPlProgDlg.set_PlotMsgString(PlotMessageIndex.SheetProgressCaption, "Sheet Progress");
            acPlProgDlg.LowerPlotProgressRange = 0;
            acPlProgDlg.UpperPlotProgressRange = 100;
            acPlProgDlg.PlotProgressPos = 0;

            return acPlProgDlg;
        }

        public string ExecutePlot(PlotInfo acPlInfo, string pdfFileName)
        {
            bool printStatus = false;
            string filename = RemoveInvalidFileNameChars(pdfFileName);
            if (PlotFactory.ProcessPlotState == ProcessPlotState.NotPlotting)
            {
                using (var acPlEng = PlotFactory.CreatePublishEngine())
                {
                    var acPlProgDlg = CreatePlotProgressDialog();
                    using (acPlProgDlg)
                    {
                        acPlProgDlg.OnBeginPlot();
                        acPlProgDlg.IsVisible = true;
                        //TODO имя надо сделать
                        filename = Path.Combine(Path.GetDirectoryName(_document.Name), filename) + ".pdf";
                        acPlEng.BeginPlot(acPlProgDlg, null);
                        acPlEng.BeginDocument(acPlInfo, _document.Name, null, 1, true, filename);

                        PlotPageInfo acPlPageInfo = new PlotPageInfo();
                        acPlEng.BeginPage(acPlPageInfo, acPlInfo, true, null);
                        acPlEng.BeginGenerateGraphics(null);
                        acPlEng.EndGenerateGraphics(null);
                        acPlEng.EndPage(null);

                        acPlProgDlg.SheetProgressPos = 100;
                        acPlProgDlg.OnEndSheet();

                        acPlEng.EndDocument(null);
                        acPlProgDlg.PlotProgressPos = 100;
                        acPlProgDlg.OnEndPlot();
                        acPlEng.EndPlot(null);
                    }
                }
            }

            return filename;
        }

        public static string RemoveInvalidFileNameChars(string fileName)
        {
            char[] invalidChars = Path.GetInvalidFileNameChars();

            return string.Join("_", fileName.Split(invalidChars, StringSplitOptions.RemoveEmptyEntries));
        }
    }
}
