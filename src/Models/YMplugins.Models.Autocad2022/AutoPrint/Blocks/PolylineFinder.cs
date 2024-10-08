using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Gile.AutoCAD.Extension;
using System.Collections.Generic;
using YMplugins.Contracts.Dto;
using YMplugins.Models.Autocad2022.Contracts;
using YMplugins.Models.Autocad2022.Utils;
using YMplugins.Models.Autocad2022.Utils.Extensions;
using PolylineExtension = YMplugins.Models.Autocad2022.Utils.Extensions.PolylineExtension;

namespace YMplugins.Models.Autocad2022.AutoPrint.Blocks
{
    public class PolylineFinder : IPolylineFinder
    {


      public List<PrintInfo> FindPolylines(SearchData data)
        {
            var polylines = new List<PrintInfo>();
            if (data.IsSearchOnLayouts)
            {
                polylines.AddRange(SearchPolylinesInSpace(Active.Database, data.SelectedLayer, "Layout"));
            }
            if (data.IsSearchOnModel)
            {
                polylines.AddRange(SearchPolylinesInSpace(Active.Database, data.SelectedLayer, "Model"));
            }
            return polylines;
        }

        private List<PrintInfo> SearchPolylinesInSpace(Database db, string layerName, string spaceName)
        {
            var polylines = new List<PrintInfo>();

            using (var tr = db.TransactionManager.StartTransaction())
            {
                BlockTable bt = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead);
                var space = (spaceName == "Model") ?
                    (BlockTableRecord)tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForRead) :
                    GetPaperSpace(db, tr);

                foreach (var objId in space)
                {
                    var entity = tr.GetObject(objId, OpenMode.ForRead) as Entity;
                    if (entity is Polyline && entity.Layer == layerName)
                    {
                        var polyline = entity as Polyline;
                        Point2d firstPoint = polyline.GetFirstPoint();
                        var position = new PointDTO(firstPoint.X, firstPoint.Y, 0);
                        (Point2d minPoint, Point2d maxPoint) = PolylineExtension.GetDimensions(polyline);
                        var xDim = maxPoint.X - minPoint.X;
                        var yDim = maxPoint.Y - minPoint.Y;

                        var format = FormatFinder.FindClosestFormat(xDim, yDim);

                        polylines.Add(
                            new PrintInfo(
                                objId.Handle.Value,
                                spaceName,
                                format,
                                1,
                                xDim,
                                yDim,
                                position,
                                true)

                        );
                    }
                }

                tr.Commit();
            }

            return polylines;
        }

        private BlockTableRecord GetPaperSpace(Database db, Transaction tr)
        {
            var bt = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead);
            var paperSpaceId = bt[BlockTableRecord.PaperSpace];
            return (BlockTableRecord)tr.GetObject(paperSpaceId, OpenMode.ForRead);
        }

    }
}