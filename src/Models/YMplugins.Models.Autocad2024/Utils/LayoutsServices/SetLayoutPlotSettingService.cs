using System.Linq;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.PlottingServices;
#if NET8_0_OR_GREATER
using Gile.AutoCAD.R25.Extension;
#else
using Gile.AutoCAD.R20.Extension;
#endif
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
            PlotConfigManager.SetCurrentConfig(standartCopier.Pc3PathForReading);

            CanonNameResolver canonNameResolver = new CanonNameResolver(standartCopier);

            // Смена текущего листа и правка настроек печати меняют БД — нужна
            // явная блокировка документа (вызов идёт из обработчика WPF-окна)
            using (Active.Document.LockDocument())
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

                    var isHor = printInfo.IsFormatHorizontal();
                    string canonName = canonNameResolver.GetCanonNameByWidthAndHeight(printInfo);

                    // Порядок важен: SetPlotConfigurationName сбрасывает ранее
                    // заданные window area, тип печати, поворот и масштаб.
                    validator.SetPlotConfigurationName(plotSettings, standartCopier.Pc3Name, canonName);

                    // Ориентацию canonical paper можно узнать только после
                    // SetPlotConfigurationName. При несовпадении с ориентацией
                    // рамки canvas разворачивается на 90°, иначе landscape-рамка
                    // ужмётся в узкую сторону portrait-листа (стандартные форматы
                    // DWG-To-PDF выдаются в portrait).
                    var paperSize = plotSettings.PlotPaperSize;
                    bool paperIsHor = paperSize.X > paperSize.Y;
                    var rotation = paperIsHor == isHor
                        ? PlotRotation.Degrees000
                        : PlotRotation.Degrees090;

                    validator.SetPlotType(plotSettings, PlotType.Extents);
                    validator.SetPlotRotation(plotSettings, rotation);

                    Extents2d plotExtents2d = new(
                        new Point2d(printInfo.Position.X,printInfo.Position.Y),
                        new Point2d(printInfo.XDim * printInfo.ScaleX,printInfo.YDim * printInfo.ScaleX));
                    validator.SetPlotWindowArea(plotSettings, plotExtents2d);
                    validator.SetUseStandardScale(plotSettings, true);
                    validator.SetStdScaleType(plotSettings, StdScaleType.ScaleToFit);
                    validator.SetPlotCentered(plotSettings, true);
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
