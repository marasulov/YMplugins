using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using System.Collections.Generic;
using YMplugins.Contracts;
using YMplugins.Contracts.Dto;

namespace YMplugins.Models.Autocad2022.AutoPrint
{
    public class AttributesService : IAttributesService
    {
        public IEnumerable<BlockAttribute>? GetAttributesForBlock(string selectedBlockName)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            var ed = doc.Editor;
            List<BlockAttribute> blockAttributes = new List<BlockAttribute>();

            //using (Transaction tr = db.TransactionManager.StartTransaction())
            //{
            //    try
            //    {
            //        // Convert the handle string to Handle object
            //        Handle handle = new Handle(selectedBlockId); // Handle is in hexadecimal

            //        // Get ObjectId from the handle
            //        ObjectId objectId = db.GetObjectId(false, handle, 0);

            //        // Open the object for read
            //        var obj = (BlockReference)tr.GetObject(objectId, OpenMode.ForRead);

            //        var attributeCollection = obj.AttributeCollection;
            //        blockAttributes = GetBlockAttributes(attributeCollection, tr);
            //        //(BlockReference)tr.GetObject(selectedBlocksByName[0], OpenMode.ForWrite);
            //        // Display the type of object retrieved
            //        doc.Editor.WriteMessage($"\nRetrieved object type: {obj.GetType().Name}");
            //    }
            //    catch (Autodesk.AutoCAD.Runtime.Exception ex)
            //    {
            //        doc.Editor.WriteMessage($"\nError: {ex.Message}");
            //    }

            //    // Commit the transaction
            //    tr.Commit();
            //}

            //using (Transaction trans = db.TransactionManager.StartTransaction())
            //{
            //    // Open the Block table for read
            //    BlockTable blkTable = trans.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;

            //    // Check if the block definition exists
            //    if (blkTable.Has(selectedBlockName))
            //    {
            //        // Open the block definition (BlockTableRecord)

            //        BlockTableRecord modelSpace =
            //            trans.GetObject(blkTable[BlockTableRecord.ModelSpace], OpenMode.ForRead) as BlockTableRecord;
            //        // Iterate through entities in ModelSpace
            //        foreach (ObjectId objId in modelSpace)
            //        {
            //            Entity entity = trans.GetObject(objId, OpenMode.ForRead) as Entity;

            //            // Check if entity is a BlockReference
            //            if (entity is BlockReference blockRef)
            //            {
            //                // Check if the block reference uses the block definition we're looking for
            //                BlockTableRecord blockDef =
            //                    trans.GetObject(blockRef.BlockTableRecord, OpenMode.ForRead) as BlockTableRecord;

            //                if (blockDef.Name == selectedBlockName)
            //                {
            //                    // Output BlockReference information
            //                    doc.Editor.WriteMessage(
            //                        $"\nFound BlockReference of \"{selectedBlockName}\" at position: {blockRef.Position}");
            //                }
            //            }

            //        }
            //    }
            //    else
            //    {
            //        doc.Editor.WriteMessage("\nBlock not found.");
            //    }

            //    // Commit the transaction
            //    trans.Commit();
            //}

            //using (Transaction trans = db.TransactionManager.StartTransaction())
            //{
            //    // Open the Block table for read
            //    BlockTable blkTable = trans.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;

            //    // Check if block definition exists in the Block Table
            //    if (!blkTable.Has(selectedBlockName))
            //    {
            //        doc.Editor.WriteMessage($"\nBlock \"{selectedBlockName}\" not found.");
            //        return null;
            //    }

            //    foreach (ObjectId btrId in blkTable)
            //    {
            //        BlockTableRecord btr = (BlockTableRecord)trans.GetObject(btrId, OpenMode.ForRead);

            //        if (btr.IsLayout) continue;
            //        //if (btr.IsAnonymous) continue;

            //        if (btr.HasAttributeDefinitions)
            //        {
            //            Active.Editor.WriteMessage($"\nАтрибуты в определении блока \"{selectedBlockName}\":");
            //            foreach (ObjectId objId in btr)
            //            {
            //                Entity entity = trans.GetObject(objId, OpenMode.ForRead) as Entity;

            //                // Если это AttributeDefinition, выводим его информацию
            //                if (entity is AttributeDefinition attDef)
            //                {
            //                    Active.Editor.WriteMessage($"\n  Тег атрибута: {attDef.Tag}, Значение по умолчанию: {attDef.TextString}, Подсказка: {attDef.Prompt}");
            //                }
            //            }
            //        }

            //        if (selectedBlockName == btr.Name)
            //        {
            //            foreach (ObjectId objId in btr)
            //            {
            //                Entity entity = trans.GetObject(objId, OpenMode.ForRead) as Entity;

            //                // Check if the entity is an AttributeDefinition
            //                if (entity is AttributeDefinition attDef)
            //                {
            //                    // Output the attribute tag, default value, and other properties
            //                    doc.Editor.WriteMessage($"\nAttribute Tag: {attDef.Tag}, Default Value: {attDef.TextString}, Prompt: {attDef.Prompt}");
            //                    blockAttributes.Add(new BlockAttribute(attDef.Tag, attDef.TextString));
            //                }
            //            }
            //            return blockAttributes;
            //        }

            //    }

            //// Open the BlockTableRecord for the ModelSpace
            //BlockTableRecord modelSpace = trans.GetObject(blkTable[BlockTableRecord.ModelSpace], OpenMode.ForRead) as BlockTableRecord;

            //// Iterate through entities in ModelSpace
            //foreach (ObjectId objId in modelSpace)
            //{
            //    Entity entity = trans.GetObject(objId, OpenMode.ForRead) as Entity;

            //    // Check if entity is a BlockReference
            //    if (entity is BlockReference blockRef)
            //    {
            //        // Check if the block reference uses the block definition we're looking for
            //        BlockTableRecord blockDef = trans.GetObject(blockRef.BlockTableRecord, OpenMode.ForRead) as BlockTableRecord;

            //        if (blockDef.Name == selectedBlockName)
            //        {
            //            // Iterate through the attributes attached to this BlockReference
            //            //foreach (ObjectId attId in blockRef.AttributeCollection)
            //            //{
            //            //    AttributeReference attRef = trans.GetObject(attId, OpenMode.ForRead) as AttributeReference;

            //            //    // Output the attribute tag and value
            //            //    doc.Editor.WriteMessage($"\nAttribute Tag: {attRef.Tag}, Value: {attRef.TextString}");
            //            //}
            //            return GetBlockAttributes(blockRef.AttributeCollection, trans);
            //        }
            //    }
            //}

            // Commit the transaction
            //  trans.Commit();
            //}
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                // Проверяем наличие определения блока
                BlockTable bt = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                if (bt.Has(selectedBlockName))
                {
                    ed.WriteMessage("\nНайдено определение блока.");
                    BlockTableRecord btr = tr.GetObject(bt[selectedBlockName], OpenMode.ForRead) as BlockTableRecord;
                    blockAttributes = PrintBlockAttributes(btr, ed);
                }
                else
                {
                    ed.WriteMessage("\nОпределение блока не найдено. Поиск вхождений блока...");
                    blockAttributes = FindBlockReference(selectedBlockName, db, ed, tr);
                }

                tr.Commit();
            }
            return blockAttributes;
        }

        private List<BlockAttribute> PrintBlockAttributes(BlockTableRecord btr, Editor ed)
        {
            List<BlockAttribute> blockAttributes = new List<BlockAttribute>();
            foreach (ObjectId id in btr)
            {
                AttributeDefinition attDef = id.GetObject(OpenMode.ForRead) as AttributeDefinition;
                if (attDef != null)
                {
                    ed.WriteMessage($"\nАтрибут: {attDef.Tag}, Значение по умолчанию: {attDef.TextString}");

                    blockAttributes.Add(new BlockAttribute(attDef.Tag, attDef.TextString));
                }
            }

            return blockAttributes;
        }

        private List<BlockAttribute> FindBlockReference(string blockName, Database db, Editor ed, Transaction tr)
        {
            List<BlockAttribute> blockAttributes = new List<BlockAttribute>();
            // Перебираем все элементы в пространстве модели и пространствах листов
            BlockTableRecord modelSpace = tr.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(db), OpenMode.ForRead) as BlockTableRecord;

            foreach (ObjectId id in modelSpace)
            {
                Entity ent = tr.GetObject(id, OpenMode.ForRead) as Entity;
                if (ent is BlockReference br && br.Name.Equals(blockName, System.StringComparison.OrdinalIgnoreCase))
                {
                    ed.WriteMessage("\nНайдено вхождение блока.");
                    PrintBlockReferenceAttributes(br, tr, ed);
                    blockAttributes = GetBlockAttributes(br.AttributeCollection, tr);
                }
            }

            // Если блок не найден в модели, проверяем пространства листов
            //BlockTable bt = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
            DBDictionary layoutDict = tr.GetObject(db.LayoutDictionaryId, OpenMode.ForRead) as DBDictionary;
            foreach (DBDictionaryEntry entry in layoutDict)
            {
                ObjectId layoutId = entry.Value;
                Layout layout = tr.GetObject(layoutId, OpenMode.ForRead) as Layout;
                if (layout != null && !layout.ModelType)
                {
                    BlockTableRecord paperSpace =
                        tr.GetObject(layout.BlockTableRecordId, OpenMode.ForRead) as BlockTableRecord;
                    foreach (ObjectId id in paperSpace)
                    {
                        Entity ent = tr.GetObject(id, OpenMode.ForRead) as Entity;
                        if (ent is BlockReference br &&
                            br.Name.Equals(blockName, System.StringComparison.OrdinalIgnoreCase))
                        {
                            ed.WriteMessage($"\nНайдено вхождение блока на листе: {layout.LayoutName}");
                            PrintBlockReferenceAttributes(br, tr, ed);
                            blockAttributes = GetBlockAttributes(br.AttributeCollection, tr);
                        }
                    }
                }
            }

            return blockAttributes;

            //ed.WriteMessage("\nВхождение блока не найдено.");
        }

        private void PrintBlockReferenceAttributes(BlockReference br, Transaction tr, Editor ed)
        {
            foreach (ObjectId id in br.AttributeCollection)
            {
                AttributeReference attRef = tr.GetObject(id, OpenMode.ForRead) as AttributeReference;
                if (attRef != null)
                {
                    ed.WriteMessage($"\nАтрибут: {attRef.Tag}, Значение: {attRef.TextString}");
                }
            }
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
    }
}