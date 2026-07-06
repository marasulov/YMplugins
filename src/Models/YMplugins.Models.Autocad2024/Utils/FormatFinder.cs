using System;
using System.Collections.Generic;

namespace YMplugins.Models.Autocad2024.Utils
{
    public class FormatFinder
    {
        /// <summary>
        ///     Допуск совпадения с форматом, мм на сторону (в размерах бумаги).
        /// </summary>
        private const double Tolerance = 5.0;

        /// <summary>
        ///     Стандартные масштабы, в которых чертят рамки:
        ///     натуральная величина и масштабы уменьшения 1:25 … 1:1000.
        /// </summary>
        public static readonly double[] StandardScales =
            { 1, 25, 40, 50, 75, 100, 200, 400, 500, 800, 1000 };

        /// <summary>
        ///     Автоматически подбирает формат листа и масштаб рамки по её габаритам.
        ///     Перебирает стандартные масштабы и для каждого ищет формат ГОСТ,
        ///     с которым рамка совпадает в пределах допуска после деления на масштаб.
        ///     Из всех подошедших вариантов выбирается самый точный.
        /// </summary>
        /// <returns>
        ///     Формат и масштаб, либо (null, null), если габариты не совпали
        ///     ни с одним форматом ни в одном стандартном масштабе.
        /// </returns>
        public static (string Format, double? Scale) DetectFormatAndScale(double xDim, double yDim)
        {
            string bestFormat = null;
            double? bestScale = null;
            double bestDiff = double.MaxValue;
            double bestScalePreference = double.MaxValue;

            bool frameIsLandscape = xDim > yDim;

            foreach (var scale in StandardScales)
            {
                double paperX = xDim / scale;
                double paperY = yDim / scale;

                // Форматы А-серии кратны двойке, поэтому рамка может одинаково
                // точно совпасть, например, с А4 в 1:100 и А2 в 1:50. При равной
                // точности предпочитаем масштаб, ближайший к 1:100 (наиболее
                // распространённая конвенция).
                double scalePreference = Math.Abs(Math.Log(scale / 100.0));

                foreach (var format in GOSTFormats)
                {
                    var (formatX, formatY) = format.Value;

                    // Ориентация записи в словаре должна совпадать с ориентацией рамки
                    if (formatX > formatY != frameIsLandscape) continue;

                    double diffX = Math.Abs(paperX - formatX);
                    double diffY = Math.Abs(paperY - formatY);
                    if (diffX > Tolerance || diffY > Tolerance) continue;

                    double diff = diffX + diffY;
                    bool better = diff < bestDiff - 0.001 ||
                                  (Math.Abs(diff - bestDiff) <= 0.001 && scalePreference < bestScalePreference);
                    if (better)
                    {
                        bestDiff = diff;
                        bestScale = scale;
                        bestFormat = format.Key;
                        bestScalePreference = scalePreference;
                    }
                }
            }

            if (bestFormat == null) return (null, null);

            var label = bestScale > 1 ? $"{bestFormat} 1:{bestScale}" : bestFormat;
            return (label, bestScale);
        }

        /// <summary>
        ///     Поиск ближайшего формата при известном (заданном вручную) масштабе.
        /// </summary>
        public static string FindFormatWithScale(double width, double height, double scale)
        {
            double normalizedWidth = Math.Min(width, height);
            double normalizedHeight = Math.Max(width, height);

            string closestFormat = null;
            double minDifference = double.MaxValue;

            foreach (var format in GOSTFormats)
            {
                var (standardWidth, standardHeight) = format.Value;

                double differenceWidthHeight = GetDifference(normalizedWidth / scale, normalizedHeight / scale, standardWidth, standardHeight);
                double differenceHeightWidth = GetDifference(normalizedWidth / scale, normalizedHeight / scale, standardHeight, standardWidth);

                // Ищем формат с минимальным отклонением
                if (differenceWidthHeight < minDifference)
                {
                    minDifference = differenceWidthHeight;
                    closestFormat = $"{format.Key} (Vertical)";
                }

                if (differenceHeightWidth < minDifference)
                {
                    minDifference = differenceHeightWidth;
                    closestFormat = $"{format.Key} (Horizontal)";
                }
            }

            return closestFormat != null ? closestFormat : "Не найдено подходящего формата";
        }

        /// <summary>
        /// Метод для подсчета разницы между размерами
        /// </summary>
        private static double GetDifference(double width1, double height1, double width2, double height2)
        {
            return Math.Abs(width1 - width2) + Math.Abs(height1 - height2);
        }

        /// <summary>
        /// Словарь, содержащий все форматы с указанием размеров для книжной и альбомной ориентации.
        /// Значение — (размер по X, размер по Y) в мм.
        /// </summary>
        private static readonly Dictionary<string, (double xFormatDim, double yFormatDim)> GOSTFormats = new()
        {
            // Книжные (вертикальные) форматы
            { "A4 в" , (210, 297) },
            { "A3 в", (297, 420) },
            { "A2 в", (420, 594) },
            { "A1 в", (594, 841) },
            { "A0 в", (841, 1189) },

            // Альбомные (горизонтальные) форматы
            { "A4 г", (297, 210) },
            { "A3 г", (420, 297) },
            { "A2 г", (594, 420) },
            { "A1 г", (841, 594) },
            { "A0 г", (1189, 841) },

            // Кратные форматы, широкая сторона по X
            { "A4x3 г", (210 * 3, 297) }, { "A4x4 г", (210 * 4, 297) },
            { "A4x5 г", (210 * 5, 297) }, { "A4x6 г", (210 * 6, 297) },

            { "A3x3 г", (297 * 3, 420) }, { "A3x4 г", (297 * 4, 420) },
            { "A3x5 г", (297 * 5, 420) }, { "A3x6 г", (297 * 6, 420) },

            { "A2x3 г", (420 * 3, 594) }, { "A2x4 г", (420 * 4, 594) },
            { "A2x5 г", (420 * 5, 594) }, { "A2x6 г", (420 * 6, 594) },

            { "A1x3 г", (594 * 3, 841) }, { "A1x4 г", (594 * 4, 841) },
            { "A1x5 г", (594 * 5, 841) }, { "A1x6 г", (594 * 6, 841) },

            { "A0x3 г", (841 * 3, 1189) }, { "A0x4 г", (841 * 4, 1189) },
            { "A0x5 г", (841 * 5, 1189) }, { "A0x6 г", (841 * 6, 1189) },

            // Кратные форматы, широкая сторона по Y
            { "A4x3 в", (297, 210 * 3) }, { "A4x4 в", (297, 210 * 4) },
            { "A4x5 в", (297, 210 * 5) }, { "A4x6 в", (297, 210 * 6) },
            { "A4x7 в", (297, 210 * 7) }, { "A4x8 в", (297, 210 * 8) },
            { "A4x9 в", (297, 210 * 9) },

            { "A3x3 в", (420, 297 * 3) }, { "A3x4 в", (420, 297 * 4) },
            { "A3x5 в", (420, 297 * 5) }, { "A3x6 в", (420, 297 * 6) },
            { "A3x7 в", (420, 297 * 7) },

            { "A2x3 в", (594, 420 * 3) }, { "A2x4 в", (594, 420 * 4) },
            { "A2x5 в", (594, 420 * 5) }, { "A2x6 в", (594, 420 * 6) },

            { "A1x3 в", (841, 594 * 3) }, { "A1x4 в", (841, 594 * 4) },
            { "A1x5 в", (841, 594 * 5) }, { "A1x6 в", (841, 594 * 6) },

            { "A0x2 в", (1189, 841 * 2) },
            { "A0x3 в", (1189, 841 * 3) }, { "A0x4 в", (1189, 841 * 4) },
            { "A0x5 в", (1189, 841 * 5) }, { "A0x6 в", (1189, 841 * 6) },
        };
    }
}
