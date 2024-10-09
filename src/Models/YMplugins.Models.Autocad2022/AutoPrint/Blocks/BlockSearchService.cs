using Autodesk.AutoCAD.DatabaseServices;
using Gile.AutoCAD.Extension;
using System.Collections.Generic;
using YMplugins.Contracts.Dto;
using YMplugins.Models.Autocad2022.Utils;

namespace YMplugins.Models.Autocad2022.AutoPrint.Blocks
{
    public class BlockSearchService
    {
        //private readonly DynamicBlockService _dynamicBlockService;
        //private readonly AttributeService _attributeService;

        //public BlockSearchService(DynamicBlockService dynamicBlockService, AttributeService attributeService)
        //{
        //    _dynamicBlockService = dynamicBlockService;
        //    _attributeService = attributeService;
        //}

        // Return found blocks with attributes and dynamic properties from the specified space
        public List<PrintInfo> SearchBlocksInSpace(Database db, string blockName, string searchSpace, string attributeName)
        {
            List<PrintInfo> foundBlocks = new List<PrintInfo>();

            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                BlockTable bt = (BlockTable)trans.GetObject(db.BlockTableId, OpenMode.ForRead);

                if (searchSpace == "Model")
                {
                    // Search in Model Space
                    BlockTableRecord modelSpace = (BlockTableRecord)trans.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForRead);
                    foundBlocks.AddRange(SearchBlockInSpace(trans, modelSpace, blockName, "Model Space"));
                }

                if (searchSpace == "Layout")
                {
                    // Search in each Layout (Paper Space)
                    foreach (ObjectId btrId in bt)
                    {
                        BlockTableRecord btr = (BlockTableRecord)trans.GetObject(btrId, OpenMode.ForRead);

                        if (btr.IsLayout)
                        {
                            Layout layout = (Layout)trans.GetObject(btr.LayoutId, OpenMode.ForRead);
                            if (layout.LayoutName != "Model")
                                foundBlocks.AddRange(SearchBlockInSpace(trans, btr, blockName, $"Layout: {layout.LayoutName}"));
                        }
                    }
                }

                trans.Commit();
            }

            return foundBlocks;
        }

        // Search blocks in specific space and return block data
        private List<PrintInfo> SearchBlockInSpace(Transaction trans, BlockTableRecord space, string blockName, string spaceName)
        {
            List<PrintInfo> blockList = new List<PrintInfo>();

            foreach (ObjectId entId in space)
            {
                Entity ent = (Entity)trans.GetObject(entId, OpenMode.ForRead);

                if (ent is not BlockReference blockRef) continue;
                var blockRefName = blockRef.GetEffectiveName();

                if (blockRefName != blockName) continue;
                var blockExtents = blockRef.GeometricExtents;
                var xDim = blockExtents.MaxPoint.X - blockExtents.MinPoint.X;
                var yDim = blockExtents.MaxPoint.Y - blockExtents.MinPoint.Y;
                var position = blockRef.Position;
                var blockScale = blockRef.ScaleFactors.X;
                var blockPointPosition = new PointDTO(position.X, position.Y, position.Z);
                var format = FormatFinder.FindFormatWithScale(xDim, yDim);
                Active.Editor.WriteMessage($"в полилинии xDim {xDim} по X, yDim {yDim}");
#if DEBUG
                Active.Editor.WriteMessage($"format {format}");
#endif
                PrintInfo blockData = new PrintInfo(blockRef.Id.Handle.Value, spaceName, format.Format, blockScale, xDim, yDim, blockPointPosition, true);

                blockList.Add(blockData);
            }

            return blockList;
        }
    }
}