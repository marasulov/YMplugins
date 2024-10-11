using Autodesk.AutoCAD.DatabaseServices;
using Gile.AutoCAD.Extension;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Reflection;

namespace YMplugins.Models.Autocad2022.Utils.Print
{
    public class StandartCopier
    {
        public StandartCopier()
        {
            string confFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty, "conf.json");
#if DEBUG

            confFile = GetConfFileInDebug();
#endif

            var jsonFile = File.ReadAllText(confFile);
            var deserializeObject = JsonConvert.DeserializeObject<Params>(jsonFile);

            try
            {
                Pc3Destination = Path.Combine(HostApplicationServices.Current.GetEnvironmentVariable("PrinterConfigDir"),
                    deserializeObject.Pc3);
                PmpDestination = Path.Combine(HostApplicationServices.Current.GetEnvironmentVariable("PrinterDescDir"),
                    deserializeObject.Pmp);

                var locationFolder = Path.GetDirectoryName(confFile);
                Pc3Source = Path.Combine(locationFolder, deserializeObject.Pc3);
                PmpSource = Path.Combine(locationFolder, deserializeObject.Pmp);
            }
            catch (Exception e)
            {
                Active.Editor.WriteMessage(e.Message);
            }
        }

        /// <summary>
        ///     Путь куда копируется pc3
        /// </summary>
        public string Pc3Destination { get; set; }

        /// <summary>
        ///     Путь куда копируется pmp
        /// </summary>
        public string PmpDestination { get; set; }

        /// <summary>
        ///     расположение файлы pc3
        /// </summary>
        public string Pc3Source { get; set; }

        /// <summary>
        ///     расположение файлы pmp
        /// </summary>
        public string PmpSource { get; set; }

        public bool CopyParamsFiles()
        {
            if (!File.Exists(Pc3Destination) & !File.Exists(PmpDestination))
            {
                if (IsFileCopied(Pc3Source, Pc3Destination))
                    Active.Editor.WriteMessage($"Файл {Pc3Source} скопирован в {Pc3Destination}");
                Active.Editor.WriteMessage(IsFileCopied(PmpSource, PmpDestination)
                    ? $"Файл {PmpSource} скопирован в {PmpDestination}"
                    : $"Не удалось скопировать файлы настройки, скопируйте с папки программы файлы {Pc3Source}  в {Pc3Destination} и {PmpDestination}");
            }
            else
            {
                var pmpSourceInfo = new FileInfo(PmpSource);
                var pc3SourceInfo = new FileInfo(Pc3Source);

                var pmpOnDestInfo = new FileInfo(PmpDestination);
                var pc3OnDestInfo = new FileInfo(Pc3Destination);

                if (pc3SourceInfo.LastWriteTime > pc3OnDestInfo.LastWriteTime)
                {
                    File.Copy(Pc3Source, Pc3Destination, true);
                    Active.Editor.WriteMessage($"Файл {Pc3Source} заменен на {Pc3Destination}");
                }

                if (pmpSourceInfo.LastWriteTime > pmpOnDestInfo.LastWriteTime)
                {
                    File.Copy(PmpSource, PmpDestination, true);
                    Active.Editor.WriteMessage($"Файл {PmpDestination} заменан на {PmpSource}");
                }
                else
                {
                    Active.Editor.WriteMessage(
                        "Не удалось скопировать файлы настройки, скопируйте  с папки программы файлы {0}  в {1} и {2} ",
                        Pc3Destination, Pc3Source, PmpSource);
                }

                Active.Editor.WriteMessage("Файлы настройки присутствуют, для перевода в pdf наберите CreateTranspdf");
            }

            return true;
        }

        private bool IsFileCopied(string location, string destination)
        {
            try
            {
                File.Copy(location, destination);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static string GetConfFileInDebug()
        {
            var path = System.IO.Path.GetDirectoryName(
                System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
            path = path?.Substring(6);
            return Path.Combine(path, "conf.json");
        }
    }
}