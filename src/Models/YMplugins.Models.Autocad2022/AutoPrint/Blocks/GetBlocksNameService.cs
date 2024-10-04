using Autodesk.AutoCAD.DatabaseServices;
using Gile.AutoCAD.Extension;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using YMplugins.Contracts;

namespace YMplugins.Models.Autocad2022.AutoPrint.Blocks
{
    public class GetBlocksNameService : IGetBlocksNameService
    {
        public List<string> GetBlocksName()
        {
            List<string> blockNames = new List<string>();
            using (Transaction trans = Active.Database.TransactionManager.StartTransaction())
            {
                //get the blockTable and iterate through all blockDef

                BlockTable bt = (BlockTable)trans.GetObject(Active.Database.BlockTableId, OpenMode.ForRead);

                foreach (ObjectId btrId in bt)
                {
                    BlockTableRecord btr = (BlockTableRecord)trans.GetObject(btrId, OpenMode.ForRead);

                    if (btr.IsLayout) continue;
                    if (btr.IsAnonymous) continue;
                    if (blockNames.Contains(btr.Name)) continue;
                    blockNames.Add(btr.Name);
                }

                foreach (var blockName in blockNames)
                {
                    Active.Editor.WriteMessage($"{blockName} \n");
                }
            }

            return blockNames;
        }

        /// <summary>
        /// Возвращает блоки по имени из выбранного списка блоков
        /// </summary>
        /// <param name="allSelectedBlocks">все блоки</param>
        /// <param name="tr">Транзакция</param>
        /// <param name="blockNames">массив из имени блоков</param>
        /// <returns></returns>
        public static ObjectIdCollection GetDynBlocksByName(
            ObjectId[] allSelectedBlocks,
            Transaction tr,
            string blockName
        )
        {
            var blocksByName = new ObjectIdCollection();
            foreach (var objectId in allSelectedBlocks)
            {
                var blRef = (BlockReference)tr.GetObject(objectId, OpenMode.ForRead);
                var block =
                    tr.GetObject(blRef.DynamicBlockTableRecord, OpenMode.ForRead) as BlockTableRecord;
                if (block is null)
                    continue;

                if (block.Name.Contains(blockName))
                    blocksByName.Add(objectId);
            }

            return blocksByName;
        }
    }
}