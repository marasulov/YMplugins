using Autodesk.AutoCAD.DatabaseServices;
#if NET8_0_OR_GREATER
using Gile.AutoCAD.R25.Extension;
#else
using Gile.AutoCAD.R20.Extension;
#endif
using Newtonsoft.Json;
using System;
using System.IO;
using System.Reflection;

namespace YMplugins.Models.Autocad2024.Utils.Print
{
    public class StandartCopier
    {
        public StandartCopier()
        {
            var assemblyDir = Path.GetDirectoryName(typeof(StandartCopier).Assembly.Location);
            var deserializeObject = LoadParams(assemblyDir);

            try
            {
                Pc3Name = deserializeObject.Pc3;
                Pc3Destination = Path.Combine(HostApplicationServices.Current.GetEnvironmentVariable("PrinterConfigDir"),
                    deserializeObject.Pc3);
                PmpDestination = Path.Combine(HostApplicationServices.Current.GetEnvironmentVariable("PrinterDescDir"),
                    deserializeObject.Pmp);

                if (!string.IsNullOrEmpty(assemblyDir))
                {
                    Pc3Source = Path.Combine(assemblyDir, deserializeObject.Pc3);
                    PmpSource = Path.Combine(assemblyDir, deserializeObject.Pmp);
                }
            }
            catch (Exception e)
            {
                WriteToCommandLine(e.Message);
            }
        }

        /// <summary>
        ///     conf.json читается с диска рядом со сборкой, а если его там нет
        ///     (Add-in Manager загружает копию DLL из Temp без соседних файлов) —
        ///     из встроенного в сборку ресурса.
        /// </summary>
        private static Params LoadParams(string assemblyDir)
        {
            var confFile = string.IsNullOrEmpty(assemblyDir) ? null : Path.Combine(assemblyDir, "conf.json");
            string json;

            if (confFile != null && File.Exists(confFile))
            {
                json = File.ReadAllText(confFile);
            }
            else
            {
                using (var stream = typeof(StandartCopier).Assembly
                           .GetManifestResourceStream("YMplugins.Models.Autocad2024.conf.json"))
                using (var reader = new StreamReader(stream))
                {
                    json = reader.ReadToEnd();
                }
            }

            return JsonConvert.DeserializeObject<Params>(json);
        }

        /// <summary>
        ///     Путь к pc3 для чтения настроек: файл рядом со сборкой,
        ///     а если его нет — уже установленная копия в папке плоттеров AutoCAD.
        /// </summary>
        public string Pc3PathForReading =>
            !string.IsNullOrEmpty(Pc3Source) && File.Exists(Pc3Source) ? Pc3Source : Pc3Destination;

        /// <summary>
        ///     Имя файла pc3 из conf.json (для SetPlotConfigurationName)
        /// </summary>
        public string Pc3Name { get; set; }

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
            if (string.IsNullOrEmpty(Pc3Source) || !File.Exists(Pc3Source) || !File.Exists(PmpSource))
            {
                // Рядом со сборкой файлов плоттера нет (загрузка через Add-in Manager) —
                // копировать нечего, работаем с уже установленными в папке плоттеров
                var installed = File.Exists(Pc3Destination) && File.Exists(PmpDestination);
                if (!installed)
                    WriteToCommandLine(
                        $"\nCADBoost: файлы плоттера не найдены. Скопируйте {Pc3Name} в {Pc3Destination} или установите плагин через MSI.");
                return installed;
            }

            if (!File.Exists(Pc3Destination) & !File.Exists(PmpDestination))
            {
                if (IsFileCopied(Pc3Source, Pc3Destination))
                    WriteToCommandLine($"Файл {Pc3Source} скопирован в {Pc3Destination}");
                WriteToCommandLine(IsFileCopied(PmpSource, PmpDestination)
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
                    WriteToCommandLine($"Файл {Pc3Source} заменен на {Pc3Destination}");
                }

                if (pmpSourceInfo.LastWriteTime > pmpOnDestInfo.LastWriteTime)
                {
                    File.Copy(PmpSource, PmpDestination, true);
                    WriteToCommandLine($"Файл {PmpDestination} заменан на {PmpSource}");
                }
                else
                {
                    WriteToCommandLine(
                        "Не удалось скопировать файлы настройки, скопируйте  с папки программы файлы {0}  в {1} и {2} ",
                        Pc3Destination, Pc3Source, PmpSource);
                }

                WriteToCommandLine("Файлы настройки присутствуют, для перевода в pdf наберите CreateTranspdf");
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
            catch (Exception e)
            {
                WriteToCommandLine($"\nНе удалось скопировать {location}: {e.Message}");
                return false;
            }
        }

        /// <summary>
        ///     Пишет в командную строку, если есть активный документ
        ///     (при старте AutoCAD документа ещё нет).
        /// </summary>
        private static void WriteToCommandLine(string message, params object[] args)
        {
            var doc = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument;
            if (doc == null) return;

            if (args.Length == 0)
                doc.Editor.WriteMessage(message);
            else
                doc.Editor.WriteMessage(message, args);
        }

    }
}