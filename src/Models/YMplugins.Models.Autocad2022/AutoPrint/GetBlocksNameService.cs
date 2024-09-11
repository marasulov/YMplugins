using System;
using Dreambuild.AutoCAD;
using System.Collections.Generic;
using System.Linq;
using Autodesk.AutoCAD.DatabaseServices;
using Gile.AutoCAD.Extension;
using YMplugins.Contracts;

namespace YMplugins.Models.Autocad2022.AutoPrint
{
    public class GetBlocksNameService : IGetBlocksNameService
    {
        public List<string> GetBlocksName()
        {
            using (Transaction trans = Active.Document.TransactionManager.StartTransaction())
            {
                //get the blockTable and iterate through all blockDef
                BlockTable bt = (BlockTable)trans.GetObject(Active.Database.BlockTableId, OpenMode.ForRead);
                foreach (ObjectId btrId in bt)
                {

                    BlockTableRecord btr = (BlockTableRecord)trans.GetObject(btrId, OpenMode.ForRead);

                    if (btr.IsDynamicBlock)
                    {
                        //get all anonymous blocks from this dynamic block

                        ObjectIdCollection anonymousIds = btr.GetAnonymousBlockIds();

                        ObjectIdCollection dynBlockRefs = new ObjectIdCollection();

                        foreach (ObjectId anonymousBtrId in anonymousIds)
                        {
                            //get the anonymous block

                            BlockTableRecord anonymousBtr = (BlockTableRecord)trans.GetObject(anonymousBtrId, OpenMode.ForRead);

                            //and all references to this block

                            ObjectIdCollection blockRefIds = anonymousBtr.GetBlockReferenceIds(true, true);

                            foreach (ObjectId id in blockRefIds)

                            {

                                dynBlockRefs.Add(id);

                            }

                        }

                        //Do something with the collection we created

                        Active.Editor.WriteMessage(String.Format("Dynamic block \"{0}\" found with {1} anonymous block and {2} block references\n",

                            btr.Name, anonymousIds.Count, dynBlockRefs.Count));

                    }

                }

            }

        }
    }
}
