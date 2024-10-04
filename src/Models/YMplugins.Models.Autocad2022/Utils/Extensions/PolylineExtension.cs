using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

namespace YMplugins.Models.Autocad2022.Utils.Extensions
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

        public static Point2d GetFirstPoint(this Polyline polyline)
        {
            //return polyline.GetPoint2dAt(0);
            var p1 = polyline.GeometricExtents.MinPoint;
            return new Point2d(p1.X, p1.Y);
        }

        public static Point2d GetLastPoint(this Polyline polyline)
        {
            var p1 = polyline.GeometricExtents.MaxPoint;
            return new Point2d(p1.X, p1.Y);
        }
    }
}