using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using YMplugins.Contracts;

namespace YMplugins.Models.Autocad2024.AutoPrint.Blocks
{
    public class SelectLayerService : ISelectLayerService
    {
        /// <summary>
        ///     Просит указать полилинию на экране и возвращает имя её слоя.
        /// </summary>
        public string SelectLayer()
        {
            Document doc = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            string layerName = null;

            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                var peo = new PromptEntityOptions("\nУкажите рамку-полилинию: ");
                peo.SetRejectMessage("\nМожно выбрать только полилинию.");
                peo.AddAllowedClass(typeof(Polyline), true);

                PromptEntityResult res = doc.Editor.GetEntity(peo);

                if (res.Status == PromptStatus.OK &&
                    tr.GetObject(res.ObjectId, OpenMode.ForRead) is Polyline polyline)
                {
                    layerName = polyline.Layer;
                    doc.Editor.WriteMessage($"\nВыбран слой: {layerName}");
                }
                else
                {
                    doc.Editor.WriteMessage("\nПолилиния не выбрана.");
                }

                tr.Commit();
            }

            return layerName;
        }
    }
}
