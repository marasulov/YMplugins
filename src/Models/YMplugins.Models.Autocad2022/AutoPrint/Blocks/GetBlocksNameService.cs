using Autodesk.AutoCAD.DatabaseServices;
using Gile.AutoCAD.Extension;
using System.Collections.Generic;
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

                BlockTable bt = (BlockTable)trans.GetObject(Active.Database.BlockTableId, OpenMode.ForRead);

                foreach (ObjectId btrId in bt)
                {
                    BlockTableRecord btr = (BlockTableRecord)trans.GetObject(btrId, OpenMode.ForRead);

                    if (btr.IsLayout) continue;
                    if (btr.IsAnonymous) continue;
                    if (blockNames.Contains(btr.Name)) continue;
                    blockNames.Add(btr.Name);
                }

                //foreach (var blockName in blockNames)
                //{
                //    Active.Editor.WriteMessage($"{blockName} \n");
                //}
            }

            return blockNames;
        }
    }
}