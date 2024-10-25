using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.PlottingServices;
using Gile.AutoCAD.Extension;
using System;
using System.Globalization;
using System.Text.RegularExpressions;
using YMplugins.Contracts.Dto;

namespace YMplugins.Models.Autocad2022.Utils.Print
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
            var pConfig = PlotConfigManager.SetCurrentConfig(_standartCopier.Pc3Source);
            var pat = @"\d{1,}?\.\d{2}"; // Регулярное выражение для поиска размеров
            var canonName = "";
            var pattern = new Regex(pat, RegexOptions.Compiled | RegexOptions.Singleline);

            // Получаем ближайший формат с использованием метода FindClosestFormat
            var closestFormat = format; //FormatFinder.FindFormatWithScale(width, height);
            if (string.IsNullOrEmpty(closestFormat))
            {
                return "Не найден подходящий формат";
            }
            Console.WriteLine($"Найден формат: {closestFormat}");

            foreach (var line in pConfig.CanonicalMediaNames)
            {
                if (!pattern.IsMatch(line)) continue; // Пропускаем строки без размеров

                // Разделяем строку на ширину и высоту
                var items = DivideStringToWidthAndHeight(pattern, line);

                // Сравниваем размеры с учетом допустимой погрешности
                if (Math.Abs(items.Item1 - width) <= tolerance && Math.Abs(items.Item2 - height) <= tolerance)
                {
                    canonName = line;
                    break;
                }
            }
            // Если каноническое имя не найдено точно, попробуем по формату
            if (string.IsNullOrEmpty(canonName))
            {
                Console.WriteLine("Каноническое имя не найдено по точным размерам, ищем по формату");
                // Логика поиска по формату, если точное имя не найдено
                foreach (var line in pConfig.CanonicalMediaNames)
                {
                    if (line.Contains(closestFormat)) // Если в строке присутствует найденный формат
                    {
                        canonName = line;
                        break;
                    }
                }
            }

            return !string.IsNullOrEmpty(canonName) ? canonName : "Не найдено подходящее каноническое имя";
        }

        public string GetCanonNameByWidthAndHeight(PrintInfo printInfo, double tolerance = 10.0)
        {
            double width, height;
            var isHor = printInfo.IsFormatHorizontal();
            if (isHor)
            {
                width = Math.Round(printInfo.XDim / printInfo.ScaleX);
                height = Math.Round(printInfo.YDim / printInfo.ScaleX);
                Active.Editor.WriteMessage($"printInfo.IsFormatHorizontal {isHor} {width} - {height}");
            }
            else
            {
                width = Math.Round(printInfo.YDim / printInfo.ScaleX);
                height = Math.Round(printInfo.XDim / printInfo.ScaleX);
                Active.Editor.WriteMessage($"printInfo.IsFormatHorizontal {isHor} {width} - {height}");
            }

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