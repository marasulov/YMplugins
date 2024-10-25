using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.PlottingServices;
using Gile.AutoCAD.Extension;
using YMplugins.Contracts.Dto;
using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;
using Exception = Autodesk.AutoCAD.Runtime.Exception;

namespace YMplugins.Models.Autocad2022.Utils.Print
{
    public class PrintUtils
    {
        ///// <summary>
        /////     plotting method
        ///// </summary>
        ///// <param name="pdfFileName"> name</param>
        ///// <param name="printModel">print param model</param>
        //public static bool PlotCurrentLayout(string pdfFileName, PrintInfo printModel)
        //{
        //    var acDoc = Active.Document;
        //    var acCurDb = acDoc.Database;
        //    //short bgPlot = (short)Application.GetSystemVariable("BACKGROUNDPLOT");
        //    Application.SetSystemVariable("BACKGROUNDPLOT", 0);
        //    bool printStatus = false;
        //    try
        //    {
        //        using (var acTrans = acCurDb.TransactionManager.StartTransaction())
        //        {
        //            // Reference the Layout Manager
        //            LayoutManager acLayoutMgr = LayoutManager.Current;
        //            // Get the current layout and output its name in the Command Line window
        //            var acLayout = acTrans.GetObject(acLayoutMgr.GetLayoutId(acLayoutMgr.CurrentLayout),
        //                OpenMode.ForRead) as Layout;

        //            // Get the PlotInfo from the layout
        //            var acPlInfo = new PlotInfo();
        //            acPlInfo.Layout = acLayout.ObjectId;

        //            // Get a copy of the PlotSettings from the layout
        //            var acPlSet = new PlotSettings(acLayout.ModelType);

        //            acPlSet.CopyFrom(acLayout);
        //            // Update the PlotSettings object
        //            var acPlSetVdr = PlotSettingsValidator.Current;

        //            var blockPosition = new Point2d(printModel.Position.X, printModel.Position.Y);
        //            var blockDimension = new Point2d(blockPosition.X + printModel.XDim,
        //                blockPosition.Y + printModel.YDim);
        //            var points = new Extents2d(blockPosition, blockDimension);

        //            var isHor = IsFormatHorizontal(printModel);
        //            //pdfCreator.GetBlockDimensions();
        //            var canonName = printModel.GetCanonNameByWidthAndHeight();

        //            //acDoc.Utility.TranslateCoordinates(point1, acWorld, acDisplayDCS, False);
        //            acPlSetVdr.SetPlotWindowArea(acPlSet, points);
        //            acPlSetVdr.SetPlotType(acPlSet, PlotType.Window);
        //            acPlSetVdr.SetPlotRotation(acPlSet, !isHor ? PlotRotation.Degrees090 : PlotRotation.Degrees000);

        //            // Set the plot scale
        //            acPlSetVdr.SetUseStandardScale(acPlSet, false);
        //            acPlSetVdr.SetStdScaleType(acPlSet, StdScaleType.ScaleToFit);
        //            // Center the plot
        //            acPlSetVdr.SetPlotCentered(acPlSet, true);
        //            //acPlSetVdr.SetClosestMediaName(acPlSet,printModel.XDim,printModel.YDim,PlotPaperUnit.Millimeters,true);
        //            //string curCanonName = PdfCreator.GetLocalNameByAtrrValue(formatValue);
        //            acPlSetVdr.SetPlotConfigurationName(acPlSet, "DWG_To_PDF_Autoprint.pc3", canonName);
        //            //acPlSetVdr.SetCanonicalMediaName(acPlSet, canonName);

        //            // Set the plot device to use

        //            // Set the plot info as an override since it will
        //            // not be saved back to the layout
        //            acPlInfo.OverrideSettings = acPlSet;
        //            // Validate the plot info
        //            var acPlInfoVdr = new PlotInfoValidator();
        //            acPlInfoVdr.MediaMatchingPolicy = MatchingPolicy.MatchEnabled;
        //            acPlInfoVdr.Validate(acPlInfo);

        //            // Check to see if a plot is already in progress
        //            if (PlotFactory.ProcessPlotState == ProcessPlotState.NotPlotting)
        //                using (var acPlEng = PlotFactory.CreatePublishEngine())
        //                {
        //                    // Track the plot progress with a Progress dialog
        //                    var acPlProgDlg = new PlotProgressDialog(false, 1, true);
        //                    using (acPlProgDlg)
        //                    {
        //                        // Define the status messages to display when plotting starts
        //                        acPlProgDlg.set_PlotMsgString(PlotMessageIndex.DialogTitle, "Plot Progress");
        //                        acPlProgDlg.set_PlotMsgString(PlotMessageIndex.CancelJobButtonMessage, "Cancel Job");
        //                        acPlProgDlg.set_PlotMsgString(PlotMessageIndex.CancelSheetButtonMessage, "Cancel Sheet");
        //                        acPlProgDlg.set_PlotMsgString(PlotMessageIndex.SheetSetProgressCaption, "Sheet Set Progress");
        //                        acPlProgDlg.set_PlotMsgString(PlotMessageIndex.SheetProgressCaption, "Sheet Progress");
        //                        // Set the plot progress range
        //                        acPlProgDlg.LowerPlotProgressRange = 0;
        //                        acPlProgDlg.UpperPlotProgressRange = 100;
        //                        acPlProgDlg.PlotProgressPos = 0;
        //                        // Display the Progress dialog
        //                        acPlProgDlg.OnBeginPlot();
        //                        acPlProgDlg.IsVisible = true;
        //                        // Start to plot the layout
        //                        acPlEng.BeginPlot(acPlProgDlg, null);
        //                        // Define the plot output
        //                        var filename = Path.Combine(Path.GetDirectoryName(acDoc.Name), pdfFileName);

        //                        filename += ".pdf";
        //                        Active.Editor.WriteMessage(filename);

        //                        acPlEng.BeginDocument(acPlInfo, acDoc.Name, null, 1, true, filename);
        //                        // Display information about the current plot
        //                        acPlProgDlg.set_PlotMsgString(PlotMessageIndex.Status,
        //                            "Plotting: " + acDoc.Name + " - " + acLayout.LayoutName);
        //                        // Set the sheet progress range
        //                        acPlProgDlg.OnBeginSheet();
        //                        acPlProgDlg.LowerSheetProgressRange = 0;
        //                        acPlProgDlg.UpperSheetProgressRange = 100;
        //                        acPlProgDlg.SheetProgressPos = 0;
        //                        // Plot the first sheet/layout
        //                        var acPlPageInfo = new PlotPageInfo();
        //                        acPlEng.BeginPage(acPlPageInfo, acPlInfo, true, null);
        //                        acPlEng.BeginGenerateGraphics(null);
        //                        acPlEng.EndGenerateGraphics(null);
        //                        // Finish plotting the sheet/layout
        //                        acPlEng.EndPage(null);
        //                        acPlProgDlg.SheetProgressPos = 100;
        //                        acPlProgDlg.OnEndSheet();

        //                        // Finish plotting the document
        //                        acPlEng.EndDocument(null);
        //                        // Finish the plot
        //                        acPlProgDlg.PlotProgressPos = 100;
        //                        acPlProgDlg.OnEndPlot();
        //                        acPlEng.EndPlot(null);
        //                        printStatus = true;
        //                    }
        //                }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        if (e.ErrorStatus == (ErrorStatus)Autodesk.AutoCAD.BoundaryRepresentation.ErrorStatus.InvalidInput)
        //            Application.ShowAlertDialog(
        //                $"{e.Message} : {printModel.YDim} - {printModel.XDim} не найден в настройках принтера");
        //        printStatus = false;
        //    }

        //    return printStatus;
        //}

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
                Application.ShowAlertDialog($"{e.Message} : {printModel.YDim} - {printModel.XDim} not found in printer settings");
                fileName = "";
            }

            return fileName;
        }
    }
}