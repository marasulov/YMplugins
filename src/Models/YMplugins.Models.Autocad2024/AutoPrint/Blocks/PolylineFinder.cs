using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Gile.AutoCAD.R20.Extension;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using YMplugins.Contracts.Dto;
using YMplugins.Models.Autocad2024.Contracts;
using YMplugins.Models.Autocad2024.Utils;
using YMplugins.Models.Autocad2024.Utils.Extensions;
using PolylineExtension = YMplugins.Models.Autocad2024.Utils.Extensions.PolylineExtension;

namespace YMplugins.Models.Autocad2024.AutoPrint.Blocks
{
    public class PolylineFinder : IPolylineFinder
    {


      public ObservableCollection<PrintInfo> FindPolylines(SearchData data)
        {
            var polylines = new List<PrintInfo>();

            if (data.IsSearchOnLayouts && data.IsSearchOnModel)
            {
                // Выполняем поиск в модели и на листах
                polylines.AddRange(SearchPolylinesInSpace(Active.Database, data.SelectedLayer, "Model", data.PlineScale));
                polylines.AddRange(SearchPolylinesInSpace(Active.Database, data.SelectedLayer, "Layout", data.PlineScale));
            }
            else if (data.IsSearchOnModel)
            {
                // Выполняем поиск только в модели
                polylines.AddRange(SearchPolylinesInSpace(Active.Database, data.SelectedLayer, "Model", data.PlineScale));
            }
            else if (data.IsSearchOnLayouts)
            {
                // Выполняем поиск только на листах
                polylines.AddRange(SearchPolylinesInSpace(Active.Database, data.SelectedLayer, "Layout", data.PlineScale));
            }

            return new ObservableCollection<PrintInfo>(polylines);
        }

      private ObservableCollection<PrintInfo> SearchPolylinesInSpace(Database db, string layerName, string spaceName, double scale)
      {
          var polylines = new ObservableCollection<PrintInfo>();

          using (var tr = db.TransactionManager.StartTransaction())
          {
              // Выполняем поиск в ModelSpace
              if (spaceName == "Model")
              {
                  polylines = SearchPolylinesInModelSpace(db, tr, layerName, scale);
              }

              // Выполняем поиск в LayoutSpace
              if (spaceName == "Layout")
              {
                  polylines = SearchPolylinesInLayoutSpace(db, tr, layerName, scale);
              }

              tr.Commit();
          }

          return polylines;
      }

        private ObservableCollection<PrintInfo> SearchPolylinesInModelSpace(Database db, Transaction tr, string layerName, double scale)
        {
            var polylines = new ObservableCollection<PrintInfo>();

            BlockTable bt = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead);
            BlockTableRecord modelSpace = (BlockTableRecord)tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForRead);

            foreach (var objId in modelSpace)
            {
                var entity = tr.GetObject(objId, OpenMode.ForRead) as Entity;
                if (entity is Polyline polyline && polyline.Layer == layerName)
                {
                    polylines.Add(CreatePrintInfo(objId, polyline, "Model", scale));
                }
            }

            return polylines;
        }

        private ObservableCollection<PrintInfo> SearchPolylinesInLayoutSpace(Database db, Transaction tr, string layerName, double scale)
        {
            var polylines = new ObservableCollection<PrintInfo>();

            DBDictionary layoutDict = tr.GetObject(db.LayoutDictionaryId, OpenMode.ForRead) as DBDictionary;
            foreach (DBDictionaryEntry entry in layoutDict)
            {
                Layout layout = tr.GetObject(entry.Value, OpenMode.ForRead) as Layout;

                // Проверяем, что это не Model Space
                if (!layout.ModelType)
                {
                    BlockTableRecord btr = tr.GetObject(layout.BlockTableRecordId, OpenMode.ForRead) as BlockTableRecord;

                    foreach (ObjectId objId in btr)
                    {
                        var entity = tr.GetObject(objId, OpenMode.ForRead) as Entity;
                        if (entity is Polyline polyline && polyline.Layer == layerName)
                        {
                            polylines.Add(CreatePrintInfo(objId, polyline, layout.LayoutName, scale));
                        }
                    }
                }
            }

            return polylines;
        }

        private PrintInfo CreatePrintInfo(ObjectId objId, Polyline polyline, string spaceName, double scale)
        {
            Point2d firstPoint = polyline.GetFirstPoint();
            var position = new PointDTO(firstPoint.X, firstPoint.Y, 0);
            (Point2d minPoint, Point2d maxPoint) = PolylineExtension.GetDimensions(polyline);
            var xDim = maxPoint.X - minPoint.X;
            var yDim = maxPoint.Y - minPoint.Y;

            var format = FormatFinder.FindFormatWithScale(xDim, yDim, scale);

            return new PrintInfo(
                objId.Handle.Value,
                spaceName,
                format,
                scale,
                xDim,
                yDim,
                position,
                true
            );
        }

        //private ObservableCollection<PrintInfo> SearchPolylinesInSpace(Database db, string layerName, string spaceName, double scale)
        //{
        //    var polylines = new ObservableCollection<PrintInfo>();

        //    using (var tr = db.TransactionManager.StartTransaction())
        //    {
        //        BlockTable bt = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead);
        //        var space = (spaceName == "Model") ?
        //            (BlockTableRecord)tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForRead) :
        //            GetPaperSpace(db, tr);
        //        if (spaceName == "Model")
        //        {
        //            foreach (var objId in space)
        //            {
        //                var entity = tr.GetObject(objId, OpenMode.ForRead) as Entity;
        //                if (entity is Polyline && entity.Layer == layerName)
        //                {
        //                    var polyline = entity as Polyline;
        //                    Point2d firstPoint = polyline.GetFirstPoint();
        //                    var position = new PointDTO(firstPoint.X, firstPoint.Y, 0);
        //                    (Point2d minPoint, Point2d maxPoint) = PolylineExtension.GetDimensions(polyline);
        //                    var xDim = maxPoint.X - minPoint.X;
        //                    var yDim = maxPoint.Y - minPoint.Y;

        //                    var format = FormatFinder.FindFormatWithScale(xDim, yDim, scale);

        //                    polylines.Add(
        //                        new PrintInfo(
        //                            objId.Handle.Value,
        //                            spaceName,
        //                            format,
        //                            scale,
        //                            xDim,
        //                            yDim,
        //                            position,
        //                            true)

        //                    );
        //                }
        //            }
        //        }
        //        else if (spaceName == "Layout")
        //        {
        //            DBDictionary layoutDict = tr.GetObject(db.LayoutDictionaryId, OpenMode.ForRead) as DBDictionary;
        //            foreach (DBDictionaryEntry entry in layoutDict)
        //            {
        //                Layout layout = tr.GetObject(entry.Value, OpenMode.ForRead) as Layout;

        //                if (!layout.ModelType)
        //                {
        //                    BlockTableRecord btr = tr.GetObject(layout.BlockTableRecordId, OpenMode.ForRead) as BlockTableRecord;


        //                    foreach (ObjectId objId in btr)
        //                    {
        //                        var entity = tr.GetObject(objId, OpenMode.ForRead) as Entity;
        //                        if (entity is Polyline && entity.Layer == layerName)
        //                        {
        //                            var polyline = entity as Polyline;
        //                            Point2d firstPoint = polyline.GetFirstPoint();
        //                            var position = new PointDTO(firstPoint.X, firstPoint.Y, 0);
        //                            (Point2d minPoint, Point2d maxPoint) = PolylineExtension.GetDimensions(polyline);
        //                            var xDim = maxPoint.X - minPoint.X;
        //                            var yDim = maxPoint.Y - minPoint.Y;

        //                            var format = FormatFinder.FindFormatWithScale(xDim, yDim, scale);

        //                            polylines.Add(
        //                                new PrintInfo(
        //                                    objId.Handle.Value,
        //                                    layout.LayoutName,
        //                                    format,
        //                                    scale,
        //                                    xDim,
        //                                    yDim,
        //                                    position,
        //                                    true)

        //                            );
        //                        }
        //                    }
        //                }
        //            }
        //        }


        //        tr.Commit();
        //    }

        //    return polylines;


        //}

        private BlockTableRecord GetPaperSpace(Database db, Transaction tr)
        {
            var bt = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead);
            var paperSpaceId = bt[BlockTableRecord.PaperSpace];
            return (BlockTableRecord)tr.GetObject(paperSpaceId, OpenMode.ForRead);
        }

    }
}