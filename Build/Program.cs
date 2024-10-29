using Microsoft.Deployment.WindowsInstaller;
using System;
using WixSharp;
using Action = WixSharp.Action;

namespace Build
{
    internal class Program
    {
        private static string _projectName = "YMPlugins";
        private static string _version = "2.1.5";

        static void Main(string[] args)
        {

            var pluginDir = @"[AppDataFolder]\Autodesk\ApplicationPlugins\YMplugins.bundle\";
            var project = new Project()
            {
                Name = _projectName,
                UI = WUI.WixUI_ProgressOnly,
                OutDir = "output",
                GUID = new Guid("3C9072CE-836B-4091-A545-7EF11EA63148"),
                MajorUpgrade = new MajorUpgrade
                {
                    Schedule = UpgradeSchedule.afterInstallInitialize,
                    AllowSameVersionUpgrades = true,
                    DowngradeErrorMessage = "A newer release of plugin is already installed on this system. Please uninstall it first to continue."
                },
                ControlPanelInfo =
                {
                    Manufacturer = Environment.UserName,
                },
                Dirs = new Dir[]
                {
                    new InstallDir(pluginDir,
                        new File(@".\..\PackageContents.xml"),
                        new Dir(@"Contents",
                            new File(@".\..\src\Models\YMplugins.Models.Autocad2022\bin\Debug\net48\conf.json"),
                            new File(@".\..\src\Models\YMplugins.Models.Autocad2022\bin\Debug\net48\DWG_To_PDF_Uzle.pc3"),
                            new File(@".\..\src\Models\YMplugins.Models.Autocad2022\bin\Debug\net48\Uzle.pmp"),
                            new DirFiles(@".\..\src\Models\YMplugins.Models.Autocad2022\bin\Debug\net48\*.dll"),
                            new File(@".\..\src\Addins\YMplugins.Addin.Autocad2022\bin\Debug\net48\YMplugins.Addin.Autocad2022.dll")))
                },

            };


            project.Version = new Version(_version);

            var managedAction = new ManagedAction(CustomActions.MyAction,
                Return.ignore,
                When.After,
                Step.InstallFinalize,
                Condition.Always)
            {
                UsesProperties = "INSTALLDIR=[INSTALLDIR]"
            };

            project.Actions = new Action[] { managedAction };

            project.UI = WUI.WixUI_InstallDir;
            project.LicenceFile = @"C:\Users\yusufzhon.marasulov\Documents\Ym.rtf";
            project.InstallPrivileges = InstallPrivileges.limited;
            project.BuildMsi();
        }
    }

    public class CustomActions
    {
        [CustomAction]
        public static ActionResult MyAction(Session session)
        {
            string installDir = session.CustomActionData["INSTALLDIR"];
            System.Windows.Forms.MessageBox.Show($"Файлы установлены в: {installDir}", "Информация",
                System.Windows.Forms.MessageBoxButtons.OK,
                System.Windows.Forms.MessageBoxIcon.Information);

            return ActionResult.Success;
        }
    }
}
