using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using System.Collections.Generic;

namespace YMplugins.Models.DbCad
{
    public static class LayersExtension
    {
        public delegate void TransactionDelegate(Transaction tr);

        /// <summary>
        ///     Returns the active Editor object.
        /// </summary>
        public static Editor Editor => Document.Editor;

        /// <summary>
        ///     Returns the active Document object.
        /// </summary>
        public static Document Document => Application.DocumentManager.MdiActiveDocument;

        /// <summary>
        ///     Returns the active Database object.
        /// </summary>
        public static Database Database => Document.Database;

        /// <summary>
        ///     Список слоев текущей базы данных
        /// </summary>
        public static List<string> Layers
        {
            get
            {
                var layers = new List<string>();
                using (Document.LockDocument())
                {
                    using (var tr = Database.TransactionManager.StartOpenCloseTransaction())
                    {
                        if (Database.LayerTableId.IsValid &&
                            tr.GetObject(Database.LayerTableId, OpenMode.ForRead) is LayerTable lt)
                            foreach (var layerId in lt)
                                try
                                {
                                    if (tr.GetObject(layerId, OpenMode.ForRead) is LayerTableRecord
                                        {
                                            IsErased: false, IsEraseStatusToggled: false
                                        } layer) layers.Add(layer.Name);
                                }
                                catch
                                {
                                    // ignore
                                }
                    }
                }

                return layers;
            }
        }

    }
}
