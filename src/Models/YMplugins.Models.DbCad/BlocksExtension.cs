using System.Collections.Generic;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using Gile.AutoCAD.Extension;

namespace YMplugins.Models.DbCad;

public static class BlocksExtension
{
    /// <summary>
    /// Метод для получения блоков из всех открытых документов
    /// </summary>
    /// <returns></returns>
    public static Dictionary<string, List<string>> GetBlocksFromAllOpenDocuments()
    {
        var documentBlocks = new Dictionary<string, List<string>>();

        // Проходим по всем открытым документам
        foreach (Document doc in Application.DocumentManager)
        {
            List<string> blocks = GetBlocksFromDocument(doc.Database);
            documentBlocks.Add(doc.Name, blocks);
        }

        return documentBlocks;
    }
    
    private static List<string> GetBlocksFromDocument(Database db)
    {
        var blockNames = new List<string>();

        using (Transaction tr = db.TransactionManager.StartTransaction())
        {
            if (db.BlockTableId.IsValid && tr.GetObject(db.BlockTableId, OpenMode.ForRead) is BlockTable bt)
            {
                foreach (ObjectId btrId in bt)
                {
                    try
                    {
                        if (tr.GetObject(btrId, OpenMode.ForRead) is BlockTableRecord btr)
                        {
                            // Проверяем, что блок не является анонимным и не пустой
                            if (!btr.IsAnonymous && !btr.IsLayout)
                            {
                                blockNames.Add(btr.Name);
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        Active.Editor.WriteMessage($"Block {btrId} error {exception.Message} ");
                    }
                }
            }
            tr.Commit();
        }

        return blockNames;
    }
}