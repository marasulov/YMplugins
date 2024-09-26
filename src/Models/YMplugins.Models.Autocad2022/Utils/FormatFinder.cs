using System;
using System.Collections.Generic;

namespace YMplugins.Models.Autocad2022.Utils
{
    public class FormatFinder
    {
        // Словарь, содержащий все форматы с указанием размеров для книжной и альбомной ориентации
        private static readonly Dictionary<string, (double Width, double Height)> GOSTFormats = new()
        {
        { "A4", (210, 297) },
        { "A3", (297, 420) },
        { "A2", (420, 594) },
        { "A1", (594, 841) },
        { "A0", (841, 1189) },

        // Альбомные (горизонтальные) форматы
        { "A3 г", (420, 297) },
        { "A2 г", (594, 420) },
        { "A1 г", (841, 594) },
        { "A0 г", (1189, 841) },

        // Форматы с кратностями
        { "A0x2 г", (1189 * 2, 841) },
        { "A0x3 г", (1189 * 3, 841) },
        { "A0x4 г", (1189 * 4, 841) },

        { "A1x2 г", (841 * 2, 594) },
        { "A1x3 г", (841 * 3, 594) },
        { "A1x4 г", (841 * 4, 594) },
        { "A1x5 г", (841 * 5, 594) },

        { "A2x2 г", (594 * 2, 420) },
        { "A2x3 г", (594 * 3, 420) },
        { "A2x4 г", (594 * 4, 420) },
        { "A2x5 г", (594 * 5, 420) },

        { "A3x2 г", (420 * 2, 297) },
        { "A3x3 г", (420 * 3, 297) },
        { "A3x4 г", (420 * 4, 297) },
        { "A3x5 г", (420 * 5, 297) },
        { "A3x6 г", (420 * 6, 297) },
        { "A3x7 г", (420 * 7, 297) },

        { "A4x2 г", (297 * 2, 210) },
        { "A4x3 г", (297 * 3, 210) },
        { "A4x4 г", (297 * 4, 210) },
        { "A4x5 г", (297 * 5, 210) },
        { "A4x6 г", (297 * 6, 210) },
        { "A4x7 г", (297 * 7, 210) },
        { "A4x8 г", (297 * 8, 210) },
        { "A4x9 г", (297 * 9, 210) },

        // Книжные (вертикальные) кратные форматы
        { "A0x2 в", (841, 1189 * 2) },
        { "A0x3 в", (841, 1189 * 3) },
        { "A0x4 в", (841, 1189 * 4) },

        { "A1x2 в", (594, 841 * 2) },
        { "A1x3 в", (594, 841 * 3) },
        { "A1x4 в", (594, 841 * 4) },
        { "A1x5 в", (594, 841 * 5) },

        { "A2x2 в", (420, 594 * 2) },
        { "A2x3 в", (420, 594 * 3) },
        { "A2x4 в", (420, 594 * 4) },
        { "A2x5 в", (420, 594 * 5) },

        { "A3x2 в", (297, 420 * 2) },
        { "A3x3 в", (297, 420 * 3) },
        { "A3x4 в", (297, 420 * 4) },
        { "A3x5 в", (297, 420 * 5) },
        { "A3x6 в", (297, 420 * 6) },
        { "A3x7 в", (297, 420 * 7) },

        { "A4x2 в", (210, 297 * 2) },
        { "A4x3 в", (210, 297 * 3) },
        { "A4x4 в", (210, 297 * 4) },
        { "A4x5 в", (210, 297 * 5) },
        { "A4x6 в", (210, 297 * 6) },
        { "A4x7 в", (210, 297 * 7) },
        { "A4x8 в", (210, 297 * 8) },
        { "A4x9 в", (210, 297 * 9) }
    };

        private const double Tolerance = 5.0;

        // Статический метод для поиска формата с учётом кратности
        public static string FindClosestFormat(double width, double height)
        {
            double normalizedWidth = Math.Min(width, height);
            double normalizedHeight = Math.Max(width, height);

            string closestFormat = null;
            double minDifference = double.MaxValue;

            foreach (var format in GOSTFormats)
            {
                var (standardWidth, standardHeight) = format.Value;

                double differenceWidthHeight = GetDifference(normalizedWidth, normalizedHeight, standardWidth, standardHeight);
                double differenceHeightWidth = GetDifference(normalizedWidth, normalizedHeight, standardHeight, standardWidth);

                // Ищем формат с минимальным отклонением
                if (differenceWidthHeight < minDifference)
                {
                    minDifference = differenceWidthHeight;
                    closestFormat = $"{format.Key} (Книжный)";
                }

                if (differenceHeightWidth < minDifference)
                {
                    minDifference = differenceHeightWidth;
                    closestFormat = $"{format.Key} (Альбомный)";
                }
            }

            return closestFormat != null ? closestFormat : "Не найдено подходящего формата";
        }

        // Метод для подсчета разницы между размерами
        private static double GetDifference(double width1, double height1, double width2, double height2)
        {
            return Math.Abs(width1 - width2) + Math.Abs(height1 - height2);
        }



    }
}
