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
        private static string _version = "2.0.0";

        static void Main(string[] args)
        {
            var pluginDir = @"[AppDataFolder]\Autodesk\ApplicationPlugins\YMplugins.bundle\";

            // Задаем базовый путь один раз. 
            // Вы можете указать ваш новый локальный путь:
            string rootDir = @"C:\Users\y.marasulov\source\repos\YMplugins\";

            // АЛЬТЕРНАТИВА: Если хотите вообще избавиться от абсолютных путей,
            // раскомментируйте строчку ниже. Она сама найдет папку YMplugins относительно запущенного файла:
            // string rootDir = Path.GetFullPath(@"..\..\..\..\");

            var project = new Project()
            {
                Name = _projectName,
                UI = WUI.WixUI_ProgressOnly,
                OutDir = "output",
                GUID = new Guid("D56A3F69-DEB4-4332-B726-1DF06709DE7E"),
                MajorUpgrade = MajorUpgrade.Default,
                ControlPanelInfo =
                {
                    Manufacturer = Environment.UserName,
                },
                Dirs = new Dir[]
                {
                    new InstallDir(pluginDir,
                        
                        // Файл манифеста
                        new File($@"{rootDir}PackageContents.xml"),

                        new Dir(@"Contents",
                            
                            // Папка для AutoCAD 2021-2024 (.NET 4.8)
                            new Dir(@"net48",
                                new File($@"{rootDir}src\Models\YMplugins.Models.Autocad2022\bin\Debug\net48\conf.json"),
                                new DirFiles($@"{rootDir}src\Models\YMplugins.Models.Autocad2022\bin\Debug\net48\*.dll"),
                                new File($@"{rootDir}src\Addins\YMplugins.Addin.Autocad2022\bin\Debug\net48\YMplugins.Addin.dll")
                            ),

                            // Папка для AutoCAD 2025+ (.NET 8.0)
                            new Dir(@"net8.0-windows",
                                new File($@"{rootDir}src\Models\YMplugins.Models.Autocad2022\bin\Debug\net8.0-windows\conf.json"),
                                new DirFiles($@"{rootDir}src\Models\YMplugins.Models.Autocad2022\bin\Debug\net8.0-windows\*.dll"),
                                new File($@"{rootDir}src\Addins\YMplugins.Addin.Autocad2022\bin\Debug\net8.0-windows\YMplugins.Addin.dll")
                            )
                        )
                    )
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
            // Здесь тоже используем rootDir для лицензии
            project.LicenceFile = $@"{rootDir}\License.rtf";
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