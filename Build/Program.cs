using Microsoft.Deployment.WindowsInstaller;
using System;
using System.IO;
using WixSharp;
using Action = WixSharp.Action;
using File = WixSharp.File;

namespace Build
{
    internal class Program
    {
        private static string _projectName = "YMPlugins";
        private static string _version = "2.1.5";

        /// <summary>
        ///     Корень репозитория, вычисленный от расположения Build.exe
        ///     (Build\bin\{Config}\Build.exe -> три уровня вверх). Пути не зависят
        ///     от рабочей папки, откуда запущен exe.
        /// </summary>
        private static readonly string RepoRoot = Path.GetFullPath(Path.Combine(
            Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), @"..\..\.."));

        private static string FromRoot(string relativePath) => Path.Combine(RepoRoot, relativePath);

        static void Main(string[] args)
        {
            var addinOutNet48 = FromRoot(@"src\Addins\YMplugins.Addin.Autocad2024\bin\Release\net48");
            var addinOutNet8 = FromRoot(@"src\Addins\YMplugins.Addin.Autocad2024\bin\Release\net8.0-windows");

            foreach (var dir in new[] { addinOutNet48, addinOutNet8 })
            {
                if (!Directory.Exists(dir))
                {
                    Console.WriteLine("Не найдена папка со сборкой плагина:");
                    Console.WriteLine("  " + dir);
                    Console.WriteLine("Сначала соберите решение в Release: dotnet build YMplugins.sln -c Release");
                    Environment.ExitCode = 1;
                    return;
                }
            }

            var pluginDir = @"[AppDataFolder]\Autodesk\ApplicationPlugins\YMplugins.bundle\";
            var project = new Project()
            {
                Name = _projectName,
                UI = WUI.WixUI_ProgressOnly,
                OutDir = FromRoot(@"Build\output"),
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
                        new File(FromRoot(@"PackageContents.xml")),
                        new Dir(@"Contents",
                            new Dir(@"net48",
                                new File(Path.Combine(addinOutNet48, "conf.json")),
                                new File(Path.Combine(addinOutNet48, "DWG_To_PDF_Uzle.pc3")),
                                new File(Path.Combine(addinOutNet48, "Uzle.pmp")),
                                new DirFiles(Path.Combine(addinOutNet48, "*.dll"))),
                            new Dir(@"net8.0-windows",
                                new File(Path.Combine(addinOutNet8, "conf.json")),
                                new File(Path.Combine(addinOutNet8, "DWG_To_PDF_Uzle.pc3")),
                                new File(Path.Combine(addinOutNet8, "Uzle.pmp")),
                                new DirFiles(Path.Combine(addinOutNet8, "*.dll")))))
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
            project.LicenceFile = FromRoot(@"Build\LicenseAgreement.rtf");
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
