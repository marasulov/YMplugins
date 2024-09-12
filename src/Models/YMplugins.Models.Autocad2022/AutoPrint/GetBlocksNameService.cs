using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Gile.AutoCAD.Extension;
using System;
using System.Collections.Generic;
using YMplugins.Contracts;
using YMplugins.Contracts.Dto;

namespace YMplugins.Models.Autocad2022.AutoPrint
{
    public class GetBlocksNameService : IGetBlocksNameService
    {
        public List<string> GetBlocksName()
        {
            List<string> blockNames = new List<string>();
            using (Transaction trans = Active.Database.TransactionManager.StartTransaction())
            {

                //get the blockTable and iterate through all blockDef

                BlockTable bt = (BlockTable)trans.GetObject(Active.Database.BlockTableId,

                    OpenMode.ForRead);

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

        private static List<BlockAttribute> GetBlockAttributes(
            AttributeCollection attributeCollection, Transaction tr)
        {
            var attributesDict = new List<BlockAttribute>();

            foreach (ObjectId attId in attributeCollection)
            {
                var attRef = (AttributeReference)tr.GetObject(attId, OpenMode.ForRead);
                attributesDict.Add(new BlockAttribute(attRef.Tag, attRef.TextString));
            }

            return attributesDict;
        }

        public static ObjectId[] SelectBlocksWithFilter(bool onlySelectedPages = false)
        {
            var sFilter = new SelectionFilter(new TypedValue[2] { new(0, "INSERT"), new(66, 1) });
            var selResult = Active.Editor.SelectAll(sFilter);

            if (onlySelectedPages)
            {
                var psOptions = new PromptSelectionOptions();
                psOptions.MessageForAdding = "\nSelect stamp block : ";
                psOptions.MessageForRemoval = "\nRemove from selection : ";

                selResult = Active.Editor.GetSelection(psOptions, sFilter);
            }

            if (selResult.Status != PromptStatus.OK)
                return null;

            var selSet = selResult.Value;
            return selSet.GetObjectIds();
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
