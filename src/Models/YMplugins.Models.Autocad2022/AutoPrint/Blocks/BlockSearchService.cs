using Autodesk.AutoCAD.DatabaseServices;
using Gile.AutoCAD.Extension;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using Autodesk.AutoCAD.ApplicationServices;
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
        public ObservableCollection<PrintInfo> SearchAllBlocksInSpaceByName(Document doc, string blockName, string searchSpace, int numerationStart = default)
        {
            List<PrintInfo> foundBlocks = new List<PrintInfo>();
            var db = doc.Database;
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                BlockTable bt = (BlockTable)trans.GetObject(db.BlockTableId, OpenMode.ForRead);

                if (searchSpace == "Model" || searchSpace == "Both")
                {
                    // Search in Model Space
                    BlockTableRecord modelSpace = (BlockTableRecord)trans.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForRead);
                    foundBlocks.AddRange(SearchBlockInSpace(trans, modelSpace, blockName, "Model", numerationStart, doc));
                }

                if (searchSpace == "Layout" || searchSpace == "Both")
                {
                    // Search in each Layout (Paper Space)
                    foreach (ObjectId btrId in bt)
                    {
                        BlockTableRecord btr = (BlockTableRecord)trans.GetObject(btrId, OpenMode.ForRead);

                        if (btr.IsLayout)
                        {
                            Layout layout = (Layout)trans.GetObject(btr.LayoutId, OpenMode.ForRead);
                            if (layout.LayoutName != "Model")
                                foundBlocks.AddRange(SearchBlockInSpace(trans, btr, blockName, layout.LayoutName, numerationStart, doc));
                        }
                    }
                }

                trans.Commit();
            }

            return new ObservableCollection<PrintInfo>(foundBlocks);
        }
        
        
        public ObservableCollection<PrintInfo> SearchAllBlocksInSpaceByName(string filePath, string blockName, string searchSpace, int numerationStart = default)
        {
            Document doc = Application.DocumentManager.Cast<Document>()
                .FirstOrDefault(d => d.Name.Equals(filePath));
            
            Database db = doc?.Database;
            
            List<PrintInfo> foundBlocks = new List<PrintInfo>();

            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                BlockTable bt = (BlockTable)trans.GetObject(db.BlockTableId, OpenMode.ForRead);

                if (searchSpace == "Model" || searchSpace == "Both")
                {
                    // Search in Model Space
                    BlockTableRecord modelSpace = (BlockTableRecord)trans.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForRead);
                    foundBlocks.AddRange(SearchBlockInSpace(trans, modelSpace, blockName, "Model", numerationStart));
                }

                if (searchSpace == "Layout" || searchSpace == "Both")
                {
                    // Search in each Layout (Paper Space)
                    foreach (ObjectId btrId in bt)
                    {
                        BlockTableRecord btr = (BlockTableRecord)trans.GetObject(btrId, OpenMode.ForRead);

                        if (btr.IsLayout)
                        {
                            Layout layout = (Layout)trans.GetObject(btr.LayoutId, OpenMode.ForRead);
                            if (layout.LayoutName != "Model")
                                foundBlocks.AddRange(SearchBlockInSpace(trans, btr, blockName, layout.LayoutName, numerationStart));
                        }
                    }
                }

                trans.Commit();
            }

            return new ObservableCollection<PrintInfo>(foundBlocks);
        }
        

        // Search blocks in specific space and return block data
        private ObservableCollection<PrintInfo> SearchBlockInSpace(Transaction trans, BlockTableRecord space, string blockName, string spaceName, int numerationStartValue, Document doc = null)
        {
            ObservableCollection<PrintInfo> blockList = new ObservableCollection<PrintInfo>();

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
                var format = FormatFinder.FindFormatWithScale(xDim, yDim, blockScale);
                
#if DEBUG
                Active.Editor.WriteMessage($"format {format}");
                Active.Editor.WriteMessage($"в полилинии xDim {xDim} по X, yDim {yDim}");
#endif
                PrintInfo blockData = new PrintInfo(blockRef.Id.Handle.Value, spaceName, format, blockScale, xDim, yDim, blockPointPosition, true,numerationStartValue.ToString(), doc.Name);

                blockList.Add(blockData);
                numerationStartValue++;
            }

            return blockList;
        }
    }
}