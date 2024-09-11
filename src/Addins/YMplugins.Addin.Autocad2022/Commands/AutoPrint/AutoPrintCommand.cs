using System.Linq;
using System.Windows;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using Dreambuild.AutoCAD;
using SimpleInjector;
using YMplugins.Contracts;
using YMplugins.Models.Autocad2022.AutoPrint;
using YMplugins.ViewModels.Commands;
using YMplugins.ViewModels.VM;
using YMplugins.Views.Views;

namespace YMplugins.Addin.Acad2022.Commands.AutoPrint
{
    public class AutoPrintCommand
    {
        /// <summary>
        /// Selects entities on given layer.
        /// </summary>
        [CommandMethod("SelectByLayer")]
        public static void SelectByLayer()
        {
            var availableLayerNames = GetAllLayerNames();
            var selectedLayerNames = Gui.GetChoices("Specify layers", availableLayerNames);
            if (selectedLayerNames.Length < 1)
            {
                return;
            }

            var ids = QuickSelection
                .SelectAll(FilterList.Create().Layer(selectedLayerNames))
                .ToArray();

            Interaction.SetPickSet(ids);
        }

        public static string[] GetAllLayerNames(Database db = null)
        {
            return DbHelper.GetSymbolTableRecordNames((db ?? HostApplicationServices.WorkingDatabase).LayerTableId);
        }
    }
}
