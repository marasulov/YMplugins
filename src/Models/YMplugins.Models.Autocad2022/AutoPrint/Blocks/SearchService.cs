using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Gile.AutoCAD.Extension;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using YMplugins.Contracts;
using YMplugins.Contracts.Dto;
using YMplugins.Contracts.Dto.Enums;
using YMplugins.Models.Autocad2022.Utils;
using YMplugins.Models.Autocad2022.Utils.Extensions;

namespace YMplugins.Models.Autocad2022.AutoPrint.Blocks
{
    public class SearchService : ISearchService
    {
        private readonly BlockSearchService _blockSearchService;

        private readonly SearchData _searchData;

        public SearchService(BlockSearchService blockSearchService)
        {
            _blockSearchService = blockSearchService;
        }

        //public IEnumerable<PrintInfo> FindObjects(string blockName)
        //{
        //    if (data.SelectedPrintByOption == PrintByOption.ByBlock)
        //    {
        //        var blockName = data.SelectedBlockName;

        //        // Логика поиска блоков
        //        return FindBlocksByBlockName(blockName);
        //    }
        //    else if (data.SelectedPrintByOption == PrintByOption.ByPolyline)
        //    {
        //        var layerName = data.SelectedLayer;
        //        // Логика поиска полилиний
        //        return FindPolylines(layerName);
        //    }

        //    return Enumerable.Empty<PrintInfo>();
        //}

        public ObservableCollection<PrintInfo> FindObjects(SearchData data)
        {
            var results = new ObservableCollection<PrintInfo>();

            if (data.SelectedPrintByOption == PrintByOption.ByBlock)
            {
                var blockName = data.SelectedBlockName;
                results = new ObservableCollection<PrintInfo>(FindBlocksByBlockName(blockName, data));
            }
            else if (data.SelectedPrintByOption == PrintByOption.ByPolyline)
            {
                var layerName = _searchData.SelectedLayer;
                results = new ObservableCollection<PrintInfo>(FindPolylinesByLayer(layerName, data));
            }

            return results;
        }

        private List<PrintInfo> FindBlocksByBlockName(string blockName, SearchData data)
        {
            // Используем AutoCAD API для поиска блоков
            // В зависимости от параметров `IsSearchOnModel` и `IsSearchOnLayouts`
            var blocks = new List<PrintInfo>();
            if (data.IsSearchOnLayouts)
            {
                blocks = _blockSearchService.SearchBlocksInSpace(Active.Database, blockName, "Layout", "");
            }
            if (data.IsSearchOnModel)
            {
                blocks = _blockSearchService.SearchBlocksInSpace(Active.Database, blockName, "Model", "");
            }
            return blocks;
        }

        private List<PrintInfo> FindPolylinesByLayer(string layerName, SearchData data)
        {
            var polylines = new List<PrintInfo>();

            if (data.IsSearchOnLayouts)
            {
                polylines.AddRange(SearchPolylinesInSpace(Active.Database, layerName, "Layout"));
            }

            if (data.IsSearchOnModel)
            {
                polylines.AddRange(SearchPolylinesInSpace(Active.Database, layerName, "Model"));
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
                        (double length, double width) = DbCad.PolylineExtension.GetDimensions(polyline);
                        var format = FormatFinder.FindClosestFormat(length, width);
                        polylines.Add(
                            new PrintInfo(
                                objId.Handle.Value,
                                spaceName,
                                format,
                                1,
                                width,
                                length,
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