using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.PlottingServices;
#if NET8_0_OR_GREATER
using Gile.AutoCAD.R25.Extension;
#else
using Gile.AutoCAD.R20.Extension;
#endif
using System;
using System.Globalization;
using System.Text.RegularExpressions;
using YMplugins.Contracts.Dto;

namespace YMplugins.Models.Autocad2024.Utils.Print
{
    public class CanonNameResolver
    {
        private readonly StandartCopier _standartCopier;

        public CanonNameResolver(StandartCopier standartCopier)
        {
            _standartCopier = standartCopier;
        }

        public string FindCanonName(double width, double height, string format, double tolerance = 10.0)
        {
            var pConfig = PlotConfigManager.SetCurrentConfig(_standartCopier.Pc3PathForReading);
            var pat = @"\d{1,}?\.\d{2}"; // Регулярное выражение для поиска размеров
            var canonName = "";
            var pattern = new Regex(pat, RegexOptions.Compiled | RegexOptions.Singleline);

            // Получаем ближайший формат с использованием метода FindClosestFormat
            var closestFormat = format; //FormatFinder.FindFormatWithScale(width, height);
            if (string.IsNullOrEmpty(closestFormat))
            {
                throw new InvalidOperationException(
                    $"Не удалось определить формат листа для размеров {width} x {height}");
            }

            // Среди совпавших по размеру имён приоритет у кастомных форматов
            // из нашего pmp (UserDefinedMetric ...): они объявлены без полей
            // (printable = media). Встроенные медиа драйвера pdfplot
            // (ISO_A3_(420.00_x_297.00_MM) и т.п.) совпадают по тем же размерам,
            // идут в списке раньше, но имеют непечатаемые поля (~5 мм по бокам,
            // ~17 мм сверху/снизу) — из-за них ScaleToFit ужимал рамку и на
            // листе появлялись большие белые поля.
            var fallbackName = "";
            foreach (var line in pConfig.CanonicalMediaNames)
            {
                if (!pattern.IsMatch(line)) continue; // Пропускаем строки без размеров

                // Разделяем строку на ширину и высоту
                var items = DivideStringToWidthAndHeight(pattern, line);

                // Сравниваем размеры с учетом допустимой погрешности
                if (Math.Abs(items.Item1 - width) <= tolerance && Math.Abs(items.Item2 - height) <= tolerance)
                {
                    if (line.StartsWith("UserDefinedMetric", StringComparison.OrdinalIgnoreCase))
                    {
                        canonName = line;
                        break;
                    }

                    if (string.IsNullOrEmpty(fallbackName))
                        fallbackName = line;
                }
            }

            if (string.IsNullOrEmpty(canonName))
                canonName = fallbackName;
            // Если каноническое имя не найдено точно, попробуем по формату
            if (string.IsNullOrEmpty(canonName))
            {
                foreach (var line in pConfig.CanonicalMediaNames)
                {
                    if (line.Contains(closestFormat)) // Если в строке присутствует найденный формат
                    {
                        canonName = line;
                        break;
                    }
                }
            }

            if (string.IsNullOrEmpty(canonName))
            {
                throw new InvalidOperationException(
                    $"В настройках плоттера {_standartCopier.Pc3Name} не найден формат {width} x {height} ({closestFormat})");
            }

            return canonName;
        }

        public string GetCanonNameByWidthAndHeight(PrintInfo printInfo, double tolerance = 10.0)
        {
            double width, height;
            var isHor = printInfo.IsFormatHorizontal();
            if (isHor)
            {
                width = Math.Round(printInfo.XDim / printInfo.ScaleX);
                height = Math.Round(printInfo.YDim / printInfo.ScaleX);
            }
            else
            {
                width = Math.Round(printInfo.YDim / printInfo.ScaleX);
                height = Math.Round(printInfo.XDim / printInfo.ScaleX);
            }
#if DEBUG
            Active.Editor.WriteMessage($"printInfo.IsFormatHorizontal {isHor} {width} - {height}");
#endif

            return FindCanonName(width, height, printInfo.Format, tolerance);
        }

        //public string GetCanonNameForPolyline(Polyline polyline, double tolerance = 10.0)
        //{
        //    var (length, width) = GetDimensions(polyline);
        //    return FindCanonName(length, width, tolerance);
        //}

        private static (double length, double width) GetDimensions(Polyline polyline)
        {
            double minX = double.MaxValue, minY = double.MaxValue;
            double maxX = double.MinValue, maxY = double.MinValue;

            for (int i = 0; i < polyline.NumberOfVertices; i++)
            {
                Point2d vertex = polyline.GetPoint2dAt(i);
                if (vertex.X < minX) minX = vertex.X;
                if (vertex.X > maxX) maxX = vertex.X;
                if (vertex.Y < minY) minY = vertex.Y;
                if (vertex.Y > maxY) maxY = vertex.Y;
            }

            double length = maxX - minX;
            double width = maxY - minY;
            return (length, width);
        }

        //public string GetCanonNameByWidthAndHeight(PrintInfo printInfo, double tolerance = 10.0)
        //{
        //    var standartCopier = new StandartCopier();
        //    var pConfig = PlotConfigManager.SetCurrentConfig(standartCopier.Pc3Source);

        //    var pat = @"\d{1,}?\.\d{2}"; // Регулярное выражение для поиска размеров
        //    var canonName = "";
        //    var pattern = new Regex(pat, RegexOptions.Compiled | RegexOptions.Singleline);

        //    // Получаем ближайший формат с использованием метода FindClosestFormat
        //    var closestFormat = FormatFinder.FindClosestFormat(printInfo.XDim, printInfo.YDim);

        //    if (string.IsNullOrEmpty(closestFormat))
        //    {
        //        return "Не найден подходящий формат"; // Обработка ситуации, когда формат не найден
        //    }

        //    Console.WriteLine($"Найден формат: {closestFormat}");

        //    foreach (var line in pConfig.CanonicalMediaNames)
        //    {
        //        if (!pattern.IsMatch(line)) continue; // Пропускаем строки без размеров

        //        // Разделяем строку на ширину и высоту
        //        var items = DivideStringToWidthAndHeight(pattern, line);

        //        double curWidth, curHeight;

        //        // Определяем размеры с учетом ориентации
        //        if (printInfo.IsFormatHorizontal())
        //        {
        //            curWidth = Math.Round(printInfo.XDim / printInfo.ScaleX);
        //            curHeight = Math.Round(printInfo.YDim / printInfo.ScaleX);
        //        }
        //        else
        //        {
        //            curWidth = Math.Round(printInfo.YDim / printInfo.ScaleX);
        //            curHeight = Math.Round(printInfo.XDim / printInfo.ScaleX);
        //        }

        //        // Сравниваем размеры с учетом допустимой погрешности
        //        if (Math.Abs(items.Item1 - curWidth) <= tolerance && Math.Abs(items.Item2 - curHeight) <= tolerance)
        //        {
        //            canonName = line;
        //            break;
        //        }
        //    }

        //    // Если каноническое имя не найдено точно, попробуем по формату
        //    if (string.IsNullOrEmpty(canonName))
        //    {
        //        Console.WriteLine("Каноническое имя не найдено по точным размерам, ищем по формату");

        //        // Логика поиска по формату, если точное имя не найдено
        //        foreach (var line in pConfig.CanonicalMediaNames)
        //        {
        //            if (line.Contains(closestFormat)) // Если в строке присутствует найденный формат
        //            {
        //                canonName = line;
        //                break;
        //            }
        //        }
        //    }

        //    return !string.IsNullOrEmpty(canonName) ? canonName : "Не найдено подходящее каноническое имя";
        //}

        //public string GetCanonNameByWidthAndHeight(PrintInfo printInfo)
        //{
        //    var standartCopier = new StandartCopier();
        //    var pConfig = PlotConfigManager.SetCurrentConfig(standartCopier.Pc3Source);

        //    var pat = @"\d{1,}?\.\d{2}";
        //    var canonName = "";
        //    var pattern = new Regex(pat, RegexOptions.Compiled | RegexOptions.Singleline);

        //    foreach (var line in pConfig.CanonicalMediaNames)
        //    {
        //        if (!pattern.IsMatch(line)) continue;
        //        var items = DivideStringToWidthAndHeight(pattern, line);

        //        double curWidth;
        //        double curHeight;

        //        if (printInfo.IsFormatHorizontal())
        //        {
        //            curWidth = Math.Round(printInfo.XDim / printInfo.ScaleX);
        //            curHeight = Math.Round(printInfo.YDim / printInfo.ScaleX);
        //        }
        //        else
        //        {
        //            curWidth = Math.Round(printInfo.YDim / printInfo.ScaleX);
        //            curHeight = Math.Round(printInfo.XDim / printInfo.ScaleX);
        //        }

        //        if ((items.Item1 == curWidth) && (items.Item2 == curHeight))
        //        {
        //            canonName = line;
        //            break;
        //        }
        //    }

        //    return canonName;
        //}

        /// <summary>
        ///     Габариты бумаги (мм), закодированные в каноническом имени формата,
        ///     например ISO_A3_(297.00_x_420.00_MM) -> (297, 420).
        ///     Ориентацию бумаги надёжнее брать отсюда, чем из
        ///     PlotSettings.PlotPaperSize: последнее AutoCAD переворачивает вслед
        ///     за текущим PlotRotation (унаследованным через CopyFrom) и может
        ///     вернуть размер ещё не применённого формата.
        /// </summary>
        public static bool TryGetPaperSizeFromCanonName(string canonName, out double width, out double height)
        {
            width = height = 0;
            if (string.IsNullOrEmpty(canonName)) return false;

            var pattern = new Regex(@"\d{1,}?\.\d{2}", RegexOptions.Compiled | RegexOptions.Singleline);
            var matches = pattern.Matches(canonName);
            if (matches.Count < 2) return false;

            width = Convert.ToDouble(matches[0].Value, CultureInfo.InvariantCulture);
            height = Convert.ToDouble(matches[1].Value, CultureInfo.InvariantCulture);
            return true;
        }

        private static (double, double) DivideStringToWidthAndHeight(Regex pattern, string line)
        {
            var str2 = pattern.Matches(line, 0);

            var strWidth = str2[0].ToString();
            var strHeight = str2[1].ToString();
            var strWidthD = Convert.ToDouble(strWidth, CultureInfo.InvariantCulture);
            var strheightD = Convert.ToDouble(strHeight, CultureInfo.InvariantCulture);

            return (strWidthD, strheightD);
        }
    }
}