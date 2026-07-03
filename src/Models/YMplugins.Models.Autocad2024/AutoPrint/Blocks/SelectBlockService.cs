using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
#if NET8_0_OR_GREATER
using Gile.AutoCAD.R25.Extension;
#else
using Gile.AutoCAD.R20.Extension;
#endif
using YMplugins.Contracts;

namespace YMplugins.Models.Autocad2024.AutoPrint.Blocks
{
    public class SelectBlockService : ISelectBlockService
    {
        public string? SelectBlock()
        {
            // Get the active document and database
            Document doc = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            string? blockName = default;
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                // Set up the prompt options to select only block references
                PromptEntityOptions peo = new PromptEntityOptions("\nSelect a block: ");
                peo.SetRejectMessage("\nOnly block references are allowed.");
                peo.AddAllowedClass(typeof(BlockReference), false);

                // Prompt for the selection
                PromptEntityResult res = doc.Editor.GetEntity(peo);

                if (res.Status == PromptStatus.OK)
                {
                    // Get the selected entity's ObjectId
                    BlockReference? blockRef = tr.GetObject(res.ObjectId, OpenMode.ForRead) as BlockReference;

                    if (blockRef != null)
                    {
                        var effectiveName = blockRef.GetEffectiveName(tr);
                        doc.Editor.WriteMessage($"\nYou selected block: {effectiveName}");

                        blockName = effectiveName;
                    }
                }
                else
                {
                    doc.Editor.WriteMessage("\nNo valid block selected.");
                }

                // Commit the transaction
                tr.Commit();
            }

            return blockName;
        }
    }
}