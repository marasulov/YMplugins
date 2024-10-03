using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;

namespace YMplugins.Models.DbCad
{
    public static class PolylineExtension
    {
        public static (double length, double width) GetDimensions(Polyline polyline)
        {
            double minX = double.MaxValue, minY = double.MaxValue;
            double maxX = double.MinValue, maxY = double.MinValue;

            // Проходим по каждой вершине полилинии
            for (int i = 0; i < polyline.NumberOfVertices; i++)
            {
                Point2d vertex = polyline.GetPoint2dAt(i);

                // Обновляем минимальные и максимальные значения по X и Y
                if (vertex.X < minX) minX = vertex.X;
                if (vertex.X > maxX) maxX = vertex.X;
                if (vertex.Y < minY) minY = vertex.Y;
                if (vertex.Y > maxY) maxY = vertex.Y;
            }

            // Вычисляем длину и ширину
            double length = maxX - minX;
            double width = maxY - minY;

            return (length, width);
        }
    }
}
