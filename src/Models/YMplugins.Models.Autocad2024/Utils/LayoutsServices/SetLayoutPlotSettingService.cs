using System.Linq;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.PlottingServices;
using Gile.AutoCAD.R20.Extension;
using YMplugins.Contracts;
using YMplugins.Contracts.Dto;
using YMplugins.Models.Autocad2024.Utils.Print;
using PlotType = Autodesk.AutoCAD.DatabaseServices.PlotType;

namespace YMplugins.Models.Autocad2024.Utils.LayoutsServices
{
    public class SetLayoutPlotSettingService : ISetLayoutPlotSettingService
    {
        public void Set(PrintInfo[] printDatas)
        {
            var standartCopier = new StandartCopier();
            PlotConfigManager.SetCurrentConfig(standartCopier.Pc3Source);

            CanonNameResolver canonNameResolver = new CanonNameResolver(standartCopier);

            using (var trans = Active.Database.TransactionManager.StartTransaction())
            {
               
                var layoutDatas = printDatas.Where(x => x.Space.Contains("Layout"));

                foreach (var printInfo in layoutDatas)
                {
                    
                    var lm = LayoutManager.Current;
                    lm.CurrentLayout = printInfo.Space;

                    Layout layout = trans.GetObject(lm.GetLayoutId(lm.CurrentLayout), OpenMode.ForRead) as Layout;
                    
                    PlotSettings plotSettings = new PlotSettings(layout.ModelType);
                    plotSettings.CopyFrom(layout);

                    PlotSettingsValidator validator = PlotSettingsValidator.Current;

                    validator.SetPlotType(plotSettings, PlotType.Extents);

                    var isHor = printInfo.IsFormatHorizontal();

                    validator.SetPlotRotation(plotSettings, isHor ? PlotRotation.Degrees000 : PlotRotation.Degrees090);

                    //validator.SetUseStandardScale(plotSettings, true);
                    Extents2d plotExtents2d = new(
                        new Point2d(printInfo.Position.X,printInfo.Position.Y),
                        new Point2d(printInfo.XDim * printInfo.ScaleX,printInfo.YDim * printInfo.ScaleX));
                    validator.SetPlotWindowArea(plotSettings, plotExtents2d);
                    validator.SetStdScaleType(plotSettings, StdScaleType.ScaleToFit);

                    
                    validator.SetPlotCentered(plotSettings, true);
                    string canonName = canonNameResolver.GetCanonNameByWidthAndHeight(printInfo);
                    validator.SetPlotConfigurationName(plotSettings, standartCopier.Pc3Name, canonName);
                    layout.UpgradeOpen();
                    
                    layout.CopyFrom(plotSettings);

                    //trans.Commit();

                    Active.Editor.WriteMessage("\nNew device name: " +
                                               layout.PlotConfigurationName);

                    Active.Editor.Regen();
                }

                trans.Commit();
            }
        }

        private Extents2d Get2dExtentsFrom3d(Extents3d plotArea)
        {
            return new Extents2d(new Point2d(plotArea.MinPoint.X,
                plotArea.MinPoint.X), new Point2d(plotArea.MaxPoint.X, plotArea.MaxPoint.X));
        }
    }
}
