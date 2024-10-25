using System.Collections.Generic;
using System.Linq;
using Autodesk.AutoCAD.DatabaseServices;
using Gile.AutoCAD.Extension;
using YMplugins.Contracts;
using YMplugins.Contracts.Dto;

namespace YMplugins.Models.Autocad2022.Utils.LayoutsServices
{
    public class DeleteEmptyLayoutsService : IDeleteEmptyLayoutsService

    {
        public void DeleteEmptyLayouts(PrintInfo[] printDatas)
        {
            var layoutManager = LayoutManager.Current;
            var layoutNames = GetTabOrderedLayoutNames(Active.Database);
            var layoutsWithBlocks = printDatas.Where(x => x.Space.Contains("Layout")).Select(x=>x.Space);
            if (layoutNames.Count == layoutsWithBlocks.Count() | layoutNames.Count == 0 | layoutsWithBlocks.Count() == 0) return;
            var emptyLayouts = layoutNames.Except(layoutsWithBlocks); 
            foreach (var layoutName in emptyLayouts)
                if ((layoutName != "Model"))
                    layoutManager.DeleteLayout(layoutName);
            Active.Editor.Regen();
        }

        private static List<string> GetTabOrderedLayoutNames(Database db)
        {
            if (db == null)
                return null;

            var tabOrderedLayouts = new Dictionary<int, string>();
            using (var tr = db.TransactionManager.StartTransaction())
            {
                var layoutDict = (DBDictionary)tr.GetObject(db.LayoutDictionaryId, OpenMode.ForRead);
                foreach (var layoutEntry in layoutDict)
                {
                    var layout = (Layout)tr.GetObject(layoutEntry.Value, OpenMode.ForRead);
                    if (!layout.ModelType)
                        tabOrderedLayouts[layout.TabOrder] = layout.LayoutName;
                }

                tr.Commit();
            }

            return tabOrderedLayouts.OrderBy(n => n.Key).Select(n => n.Value).ToList();
        }
    }
}
