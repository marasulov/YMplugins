using System;
using System.Collections.Generic;

namespace YMplugins.Models.Autocad2022.Utils
{
    public class FormatFinder
    {
        ///// <summary>
        ///// Словарь, содержащий все форматы с указанием размеров для книжной и альбомной ориентации
        ///// </summary>
        //private static readonly Dictionary<string, (double Width, double Height)> GOSTFormats = new()
        //{
        //    // Стандартные форматы
        //    { "A4", (210, 297) },
        //    { "A3", (297, 420) },
        //    { "A2", (420, 594) },
        //    { "A1", (594, 841) },
        //    { "A0", (841, 1189) },

        //    // Альбомные (горизонтальные) форматы
        //    { "A4 г", (297, 210) },
        //    { "A3 г", (420, 297) },
        //    { "A2 г", (594, 420) },
        //    { "A1 г", (841, 594) },
        //    { "A0 г", (1189, 841) },

        //    // Кратные форматы для книжной ориентации (увеличиваем только меньшую сторону)
        //    { "A4x3 в", (210 * 3, 297) }, { "A4x4 в", (210 * 4, 297) },
        //    { "A4x5 в", (210 * 5, 297) }, { "A4x6 в", (210 * 6, 297) },

        //    { "A3x3 в", (297 * 3, 420) }, { "A3x4 в", (297 * 4, 420) },
        //    { "A3x5 в", (297 * 5, 420) }, { "A3x6 в", (297 * 6, 420) },

        //    { "A2x3 в", (420 * 3, 594) }, { "A2x4 в", (420 * 4, 594) },
        //    { "A2x5 в", (420 * 5, 594) }, { "A2x6 в", (420 * 6, 594) },

        //    { "A1x3 в", (594 * 3, 841) }, { "A1x4 в", (594 * 4, 841) },
        //    { "A1x5 в", (594 * 5, 841) }, { "A1x6 в", (594 * 6, 841) },

        //    { "A0x3 в", (841 * 3, 1189) }, { "A0x4 в", (841 * 4, 1189) },
        //    { "A0x5 в", (841 * 5, 1189) }, { "A0x6 в", (841 * 6, 1189) },

        //    // Кратные форматы для альбомной ориентации (увеличиваем только меньшую сторону)
        //    { "A4x3 г", (297, 210 * 3) }, { "A4x4 г", (297, 210 * 4) },
        //    { "A4x5 г", (297, 210 * 5) }, { "A4x6 г", (297, 210 * 6) },
        //    { "A4x7 г", (297, 210 * 7) }, { "A4x8 г", (297, 210 * 8) },
        //    { "A4x9 г", (297, 210 * 9) },

        //    { "A3x3 г", (420, 297 * 3) }, { "A3x4 г", (420, 297 * 4) },
        //    { "A3x5 г", (420, 297 * 5) }, { "A3x6 г", (420, 297 * 6) },
        //    { "A3x7 г", (420, 297 * 7) },

        //    { "A2x3 г", (594, 420 * 3) }, { "A2x4 г", (594, 420 * 4) },
        //    { "A2x5 г", (594, 420 * 5) }, { "A2x6 г", (594, 420 * 6) },

        //    { "A1x3 г", (841, 594 * 3) }, { "A1x4 г", (841, 594 * 4) },
        //    { "A1x5 г", (841, 594 * 5) }, { "A1x6 г", (841, 594 * 6) },

        //    { "A0x2 г", (1189, 841 * 2) },
        //    { "A0x3 г", (1189, 841 * 3) }, { "A0x4 г", (1189, 841 * 4) },
        //    { "A0x5 г", (1189, 841 * 5) }, { "A0x6 г", (1189, 841 * 6) },
        //};

        ////TODO finding scaling

        //private const double Tolerance = 5.0;

        ///// <summary>
        ///// Статический метод для поиска формата с учётом кратности
        ///// </summary>
        ///// <param name="width"></param>
        ///// <param name="height"></param>
        ///// <returns></returns>
        //public static string FindClosestFormat(double width, double height)
        //{
        //    double normalizedWidth = Math.Min(width, height);
        //    double normalizedHeight = Math.Max(width, height);

        //    string closestFormat = null;
        //    double minDifference = double.MaxValue;

        //    foreach (var format in GOSTFormats)
        //    {
        //        var (standardWidth, standardHeight) = format.Value;

        //        double differenceWidthHeight = GetDifference(normalizedWidth, normalizedHeight, standardWidth, standardHeight);
        //        double differenceHeightWidth = GetDifference(normalizedWidth, normalizedHeight, standardHeight, standardWidth);

        //        // Ищем формат с минимальным отклонением
        //        if (differenceWidthHeight < minDifference)
        //        {
        //            minDifference = differenceWidthHeight;
        //            closestFormat = $"{format.Key} (Vertical)";
        //        }

        //        if (differenceHeightWidth < minDifference)
        //        {
        //            minDifference = differenceHeightWidth;
        //            closestFormat = $"{format.Key} (Horizontal)";
        //        }
        //    }

        //    return closestFormat != null ? closestFormat : "Не найдено подходящего формата";
        //}

        ///// <summary>
        ///// Метод для подсчета разницы между размерами
        ///// </summary>
        ///// <param name="width1"></param>
        ///// <param name="height1"></param>
        ///// <param name="width2"></param>
        ///// <param name="height2"></param>
        ///// <returns></returns>
        //private static double GetDifference(double width1, double height1, double width2, double height2)
        //{
        //    return Math.Abs(width1 - width2) + Math.Abs(height1 - height2);
        //}

        /// <summary>
        /// Словарь, содержащий все форматы с указанием размеров для книжной и альбомной ориентации
        /// </summary>
        private static readonly Dictionary<string, (double xFormatDim, double yFormatDim)> GOSTFormats = new()
        {
            // Альбомные (горизонтальные) форматы
            { "A4 в" , (210, 297) },
            { "A3 в", (297, 420) },
            { "A2 в", (420, 594) },
            { "A1 в", (594, 841) },
            { "A0 в", (841, 1189) },

            // Книжные (вертикальные) форматы
            { "A4 г", (297, 210) },
            { "A3 г", (420, 297) },
            { "A2 г", (594, 420) },
            { "A1 г", (841, 594) },
            { "A0 г", (1189, 841) },

            // Кратные форматы для книжной ориентации (угеличигаем только меньшую сторону)
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

            // Кратные форматы для альбомной ориентации (увеличиваем только меньшую сторону)
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

        public static (string Format, double? Scale) FindFormatWithScale(double xDim, double yDim, double? userScale = null, double tolerance = 0.05)
        {
            // Определяем меньшую и большую сторону
            double minDim = Math.Min(xDim, yDim);
            double maxDim = Math.Max(xDim, yDim);

            // Определяем ориентацию
            bool isLandscape = xDim > yDim; // Альбомная, если xDim больше yDim
            string closestFormat = null;
            double? closestScale = null;

            // Шаг 1: сначала ищем формат с масштабом 1
            foreach (var format in GOSTFormats)
            {
                var formatX = format.Value.xFormatDim;
                var formatY = format.Value.yFormatDim;

                // Проверяем соответствие формату с масштабом 1 в зависимости от ориентации
                if (isLandscape)
                {
                    // Если альбомная ориентация
                    if (Math.Abs(xDim - formatX) <= tolerance && Math.Abs(yDim - formatY) <= tolerance)
                    {
                        closestFormat = format.Key;
                        closestScale = 1;
                        return (closestFormat, closestScale); // сразу возвращаем, если нашли подходящий формат
                    }
                }
                else
                {
                    // Если книжная ориентация
                    if (Math.Abs(yDim - formatY) <= tolerance && Math.Abs(xDim - formatX) <= tolerance)
                    {
                        closestFormat = format.Key;
                        closestScale = 1;
                        return (closestFormat, closestScale); // сразу возвращаем, если нашли подходящий формат
                    }
                }
            }

            // Шаг 2: если формат не найден, ищем подходящий масштаб
            foreach (var format in GOSTFormats)
            {
                double formatX = format.Value.xFormatDim;
                double formatY = format.Value.yFormatDim;

                // Вычисляем масштаб
                double scaleX = minDim / formatX;
                double scaleY = maxDim / formatY;

                if (userScale.HasValue)
                {
                    // Если задан масштаб, проверяем его
                    double scaledX = formatX * userScale.Value;
                    double scaledY = formatY * userScale.Value;

                    if (isLandscape)
                    {
                        // Если альбомная ориентация
                        if (Math.Abs(minDim - scaledX) <= tolerance && Math.Abs(maxDim - scaledY) <= tolerance)
                        {
                            closestFormat = format.Key;
                            closestScale = userScale;
                            break;
                        }
                    }
                    else
                    {
                        // Если книжная ориентация
                        if (Math.Abs(minDim - scaledY) <= tolerance && Math.Abs(maxDim - scaledX) <= tolerance)
                        {
                            closestFormat = format.Key;
                            closestScale = userScale;
                            break;
                        }
                    }
                }
                else if (IsCloseToWholeNumber(scaleX, tolerance) &&
                         IsCloseToWholeNumber(scaleY, tolerance) &&
                         Math.Abs(scaleX - scaleY) <= tolerance)
                {
                    // Ищем формат с минимальным отклонением от целого масштаба
                    closestFormat = format.Key;
                    closestScale = Math.Round(scaleX);
                    break;
                }
            }

            return (closestFormat, closestScale);
        }

        // Вспомогательная функция для проверки, является ли число близким к целому
        private static bool IsCloseToWholeNumber(double value, double tolerance)
        {
            return Math.Abs(value - Math.Round(value)) <= tolerance;
        }

    }
}