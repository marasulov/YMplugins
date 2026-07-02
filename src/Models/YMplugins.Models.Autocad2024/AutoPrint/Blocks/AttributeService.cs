using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using YMplugins.Contracts;
using YMplugins.Contracts.Dto;
using YMplugins.Models.DbCad;

namespace YMplugins.Models.Autocad2024.AutoPrint.Blocks
{
    public class AttributeService : IAttributesService
    {
        public List<BlockAttribute>? GetAttributesForBlock(string selectedBlockName)
        {
            Document doc = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            var ed = doc.Editor;
            List<BlockAttribute> blockAttributes = new List<BlockAttribute>();

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

        public ObservableCollection<PrintInfo> GetPrintInfosForBlock(ObservableCollection<PrintInfo> printInfos, string selectedAttribute, int numerationStartValue, string prefix, string suffix, bool isCheckedNumbering)
        {
            Document doc = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            var ed = doc.Editor;

            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                foreach (var printInfo in printInfos)
                {
                    Handle handle = new Handle(printInfo.ObjectId);
                    ObjectId objId = db.GetObjectId(false, handle, 0);
                    var blref = GetBlockById(objId, tr);

                    var attrValue = blref.GetBlockAttribute(tr, selectedAttribute);
                    if (isCheckedNumbering)
                    {
                        printInfo.FileName = prefix + attrValue + suffix + numerationStartValue;
                        numerationStartValue++;
                    }
                    else
                    {
                        printInfo.FileName = prefix + attrValue + suffix;
                    }
                }

                return printInfos;
            }
        }

        // Method to get a BlockReference by its ObjectId
        public BlockReference? GetBlockById(ObjectId blockId, Transaction tr)
        {
            // Check if the ObjectId is valid and refers to a BlockReference
            if (!blockId.IsValid)
            {
                return null;
            }

            // Open the object with the specified ObjectId in read mode
            Entity entity = (Entity)tr.GetObject(blockId, OpenMode.ForRead);

            // Check if the entity is a BlockReference
            if (entity is BlockReference blockRef)
            {
                // Return the BlockReference if found
                return blockRef;
            }

            // Return null if the ObjectId is not a BlockReference
            return null;
        }

        private List<BlockAttribute> PrintBlockAttributes(BlockTableRecord btr, Editor ed)
        {
            List<BlockAttribute> blockAttributes = new List<BlockAttribute>();
            foreach (ObjectId id in btr)
            {
                AttributeDefinition attDef = id.GetObject(OpenMode.ForRead) as AttributeDefinition;
                if (attDef != null)
                {
                    //ed.WriteMessage($"\nАтрибут: {attDef.Tag}, Значение по умолчанию: {attDef.TextString}");

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
                    //PrintBlockReferenceAttributes(br, tr, ed);
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
                            //PrintBlockReferenceAttributes(br, tr, ed);
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

        // Function to retrieve attributes from a block reference
        public Dictionary<string, string> GetAttributesFromBlock(BlockReference blockRef, string attributeName)
        {
            Dictionary<string, string> attributeValues = new Dictionary<string, string>();

            // Iterate over the block's attributes
            foreach (ObjectId attId in blockRef.AttributeCollection)
            {
                AttributeReference attRef = (AttributeReference)blockRef.Database.TransactionManager.GetObject(attId, OpenMode.ForRead);

                // If attribute name matches, return the attribute's value
                if (attRef.Tag == attributeName || string.IsNullOrEmpty(attributeName))
                {
                    attributeValues[attRef.Tag] = attRef.TextString;
                }
            }

            return attributeValues;
        }

        // Method to get the attribute value for a specific attribute name from a BlockReference
        public string? GetAttributeValueFromBlock(BlockReference blockRef, string attributeName)
        {
            // Iterate over the block's AttributeCollection to find the specified attribute
            foreach (ObjectId attId in blockRef.AttributeCollection)
            {
                // Open the attribute in read mode
                AttributeReference attRef = (AttributeReference)blockRef.Database.TransactionManager.GetObject(attId, OpenMode.ForRead);

                // Check if the attribute's tag matches the provided attribute name
                if (attRef.Tag == attributeName)
                {
                    // Return the value of the matching attribute
                    return attRef.TextString;
                }
            }

            // Return null if no matching attribute is found
            return null;
        }
    }
}