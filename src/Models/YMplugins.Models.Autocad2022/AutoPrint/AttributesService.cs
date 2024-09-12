using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using System;
using System.Collections.Generic;
using YMplugins.Contracts;
using YMplugins.Contracts.Dto;

namespace YMplugins.Models.Autocad2022.AutoPrint
{
    public class AttributesService : IAttributesService
    {
        public IEnumerable<BlockAttribute>? GetAttributesForBlock(long selectedBlockId)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            IEnumerable<BlockAttribute> blockAttributes = default;

            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                // Prompt the user to enter a handle value
                //PromptStringOptions pso = new PromptStringOptions("\nEnter the handle value: ");
                //pso.AllowSpaces = false;
                //PromptResult pr = doc.Editor.GetString(pso);

                
                    try
                    {
                        // Convert the handle string to Handle object
                        Handle handle = new Handle(selectedBlockId); // Handle is in hexadecimal

                        // Get ObjectId from the handle
                        ObjectId objectId = db.GetObjectId(false, handle, 0);

                        // Open the object for read
                        var obj = (BlockReference)tr.GetObject(objectId, OpenMode.ForRead);

                        var attributeCollection = obj.AttributeCollection;
                        blockAttributes = GetBlockAttributes(attributeCollection, tr);
                    //(BlockReference)tr.GetObject(selectedBlocksByName[0], OpenMode.ForWrite);
                    // Display the type of object retrieved
                    doc.Editor.WriteMessage($"\nRetrieved object type: {obj.GetType().Name}");
                    }
                    catch (Autodesk.AutoCAD.Runtime.Exception ex)
                    {
                        doc.Editor.WriteMessage($"\nError: {ex.Message}");
                    }
                

                // Commit the transaction
                tr.Commit();
            }

            return blockAttributes;
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
