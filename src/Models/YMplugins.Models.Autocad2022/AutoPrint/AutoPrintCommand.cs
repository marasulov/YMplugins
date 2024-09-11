using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using Dreambuild.AutoCAD;
using Gile.AutoCAD.Extension;

namespace YMplugins.Models.Autocad2022.AutoPrint
{
    public class AutoPrintCommand
    {
        /// <summary>
        /// Selects entities on given layer.
        /// </summary>
        [CommandMethod("SelectByLayer1")]
        public static void SelectByLayer()
        {
            Active.Editor.WriteMessage(GetAllLayerNames(Active.Database).Length.ToString());
        }

        public static string[] GetAllLayerNames(Database db = null)
        {
            return DbHelper.GetSymbolTableRecordNames((db ?? HostApplicationServices.WorkingDatabase).LayerTableId);
        }
    }
}
