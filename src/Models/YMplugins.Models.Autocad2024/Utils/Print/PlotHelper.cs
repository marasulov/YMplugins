using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.PlottingServices;
using System;
using System.IO;
using YMplugins.Contracts.Dto;
using PlotType = Autodesk.AutoCAD.DatabaseServices.PlotType;

namespace YMplugins.Models.Autocad2024.Utils.Print
{
    public class PlotHelper
    {
        private Document _document;
        private PrintInfo _printModel;

        public PlotHelper(Document document, PrintInfo printModel)
        {
            _document = document;
            _printModel = printModel;
        }

        public PlotInfo ConfigurePlotSettings(Layout acLayout, StandartCopier standartCopier)
        {
            var acPlInfo = new PlotInfo();
            acPlInfo.Layout = acLayout.ObjectId;

            var acPlSet = new PlotSettings(acLayout.ModelType);
            acPlSet.CopyFrom(acLayout);

            var acPlSetVdr = PlotSettingsValidator.Current;

            var blockPosition = new Point2d(_printModel.Position.X, _printModel.Position.Y);
            var blockDimension = new Point2d(blockPosition.X + _printModel.XDim, blockPosition.Y + _printModel.YDim);

            // Для печати из пространства модели окно задаётся в системе координат
            // отображения (DCS), а рамка найдена в мировых координатах (WCS).
            // Если вид панорамирован к рамкам (далеко от начала координат), без
            // перевода WCS->DCS окно указывает в пустое место — лист выходит пустым.
            var points = acLayout.ModelType
                ? ToDisplayCoordinates(blockPosition, blockDimension)
                : new Extents2d(blockPosition, blockDimension);

            bool isHor = _printModel.IsFormatHorizontal();

            CanonNameResolver resolver = new CanonNameResolver(standartCopier);
            string canonName = resolver.GetCanonNameByWidthAndHeight(_printModel);

            acPlSetVdr.SetPlotWindowArea(acPlSet, points);
            acPlSetVdr.SetPlotType(acPlSet, PlotType.Window);
            acPlSetVdr.SetPlotRotation(acPlSet, !isHor ? PlotRotation.Degrees090 : PlotRotation.Degrees000);
            acPlSetVdr.SetUseStandardScale(acPlSet, false);
            acPlSetVdr.SetStdScaleType(acPlSet, StdScaleType.ScaleToFit);
            acPlSetVdr.SetPlotCentered(acPlSet, true);
            acPlSetVdr.SetPlotConfigurationName(acPlSet, standartCopier.Pc3Name, canonName);

            acPlInfo.OverrideSettings = acPlSet;

            return acPlInfo;
        }

        /// <summary>
        ///     Переводит углы окна печати из мировых координат (WCS) в систему
        ///     координат отображения (DCS) текущего вида модели. Нужно для
        ///     PlotType.Window при печати из пространства модели.
        /// </summary>
        private Extents2d ToDisplayCoordinates(Point2d min, Point2d max)
        {
            Editor ed = _document.Editor;
            using (ViewTableRecord view = ed.GetCurrentView())
            {
                Matrix3d wcs2dcs = Matrix3d.PlaneToWorld(view.ViewDirection);
                wcs2dcs = Matrix3d.Displacement(view.Target - Point3d.Origin) * wcs2dcs;
                wcs2dcs = Matrix3d.Rotation(-view.ViewTwist, view.ViewDirection, view.Target) * wcs2dcs;
                wcs2dcs = wcs2dcs.Inverse();

                var c1 = new Point3d(min.X, min.Y, 0).TransformBy(wcs2dcs);
                var c2 = new Point3d(max.X, max.Y, 0).TransformBy(wcs2dcs);

                return new Extents2d(
                    new Point2d(Math.Min(c1.X, c2.X), Math.Min(c1.Y, c2.Y)),
                    new Point2d(Math.Max(c1.X, c2.X), Math.Max(c1.Y, c2.Y)));
            }
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