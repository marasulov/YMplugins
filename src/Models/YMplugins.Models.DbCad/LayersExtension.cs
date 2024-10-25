using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using System.Collections;
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

        public static Dictionary<ObjectId, bool> GetLayersIsBlockedCol()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            // A list of the layers' names & IDs contained
            // in the current database, sorted by layer name
            var layersIsBlockedCol = new Dictionary<ObjectId, bool>();

            // A list of the selected layers' IDs
            // Start by populating the list of names/IDs
            // from the LayerTable
            using (var tr = db.TransactionManager.StartOpenCloseTransaction())
            {
                var lt = (LayerTable)tr.GetObject(db.LayerTableId, OpenMode.ForRead);
                foreach (var lid in lt)
                {
                    var ltr = (LayerTableRecord)tr.GetObject(lid, OpenMode.ForRead);
                    layersIsBlockedCol.Add(lid, ltr.IsLocked);
                }
            }

            // Display a numbered list of the available layers
            return layersIsBlockedCol;
        }

        public static void LockOrUnlockLayers(this Document doc, bool dolock,
            ObjectIdCollection layers = null, bool ignoreCurrent = true,
            bool lockZero = false)
        {
            var db = doc.Database;
            var ed = doc.Editor;
            using (var tr = db.TransactionManager.StartTransaction())
            {
                var layerIds = layers != null ? layers : (IEnumerable)tr.GetObject(db.LayerTableId, OpenMode.ForRead);

                foreach (ObjectId ltrId in layerIds)
                    // Don't try to lock/unlock either the current layer or layer 0
                    // (depending on whether lockZero == true for the latter)
                    if ((!ignoreCurrent || ltrId != db.Clayer) &&
                        (lockZero || ltrId != db.LayerZero))
                    {
                        // Open the layer for write and lock/unlock it
                        var ltr = (LayerTableRecord)tr.GetObject(ltrId, OpenMode.ForWrite);
                        ltr.IsLocked = dolock;
                        ltr.IsOff = ltr.IsOff; // This is needed to force a graphics update
                    }

                tr.Commit();
            }

            // These two calls will result in the layer's geometry fading/unfading
            // appropriately
            ed.ApplyCurDwgLayerTableChanges();
            ed.Regen();
        }

        public static void LockLayers(this Document doc, Dictionary<ObjectId, bool> layersLockDict)
        {
            var db = doc.Database;
            var ed = doc.Editor;
            using (var tr = db.TransactionManager.StartTransaction())
            {
                //var layerIds = (layersLockDict.Keys != null ? (IEnumerable)layersLockDict.Keys :
                //    (IEnumerable)tr.GetObject(db.LayerTableId, OpenMode.ForRead));

                foreach (var layerPos in layersLockDict)
                {
                    // Don't try to lock/unlock either the current layer or layer 0
                    // (depending on whether lockZero == true for the latter)

                    // Open the layer for write and lock/unlock it
                    var ltr = (LayerTableRecord)tr.GetObject(layerPos.Key, OpenMode.ForWrite);
                    ltr.IsLocked = layerPos.Value;
                    ltr.IsOff = ltr.IsOff; // This is needed to force a graphics update
                }

                tr.Commit();
            }

            // These two calls will result in the layer's geometry fading/unfading
            // appropriately
            ed.ApplyCurDwgLayerTableChanges();
            ed.Regen();
        }

        public static ObjectIdCollection SelectLayers(this Document doc)
        {
            var db = doc.Database;
            var ed = doc.Editor;
            // A list of the layers' names & IDs contained
            // in the current database, sorted by layer name
            var ld = new SortedList<string, ObjectId>();
            // A list of the selected layers' IDs
            var lids = new ObjectIdCollection();
            // Start by populating the list of names/IDs
            // from the LayerTable
            using (var tr = db.TransactionManager.StartOpenCloseTransaction())
            {
                var lt = (LayerTable)tr.GetObject(db.LayerTableId, OpenMode.ForRead);
                foreach (var lid in lt)
                {
                    var ltr = (LayerTableRecord)tr.GetObject(lid, OpenMode.ForRead);
                    ld.Add(ltr.Name, lid);
                }
            }

            // Display a numbered list of the available layers
            ed.WriteMessage("\nLayers available:");
            ed.SelectNamedLayers(ld, lids);
            return lids;
        }

        private static void SelectNamedLayers(this Editor ed, SortedList<string, ObjectId> ld, ObjectIdCollection lids)
        {
            var i = 1;
            foreach (var kv in ld) ed.WriteMessage("\n{0} - {1}", i++, kv.Key);
            // We will ask the user to select from the list
            var pio = new PromptIntegerOptions("\nEnter number of layer to add: ");
            pio.LowerLimit = 1;
            pio.UpperLimit = ld.Count;
            pio.AllowNone = true;
            // And will do so in a loop, waiting for Escape or Enter to terminate
            PromptIntegerResult pir;
            do
            {
                // Select one from the list
                pir = ed.GetInteger(pio);
                if (pir.Status == PromptStatus.OK)
                {
                    // Get the layer's name
                    var ln = ld.Keys[pir.Value - 1];
                    // And then its ID
                    ObjectId lid;
                    ld.TryGetValue(ln, out lid);
                    // Add the layer'd ID to the list, if it's not already on it
                    if (lids.Contains(lid))
                    {
                        ed.WriteMessage("\nLayer \"{0}\" has already been selected.", ln);
                    }
                    else
                    {
                        lids.Add(lid);
                        ed.WriteMessage("\nAdded \"{0}\" to selected layers.", ln);
                    }
                }
            } while (pir.Status == PromptStatus.OK);
        }
    }
}