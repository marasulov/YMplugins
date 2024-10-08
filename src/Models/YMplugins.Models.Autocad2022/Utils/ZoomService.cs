using Autodesk.AutoCAD.DatabaseServices;
using Gile.AutoCAD.Extension;
using YMplugins.Contracts;

namespace YMplugins.Models.Autocad2022.Utils;

public class ZoomService : IZoomEntity
{
    public void Zoom(int id)
    {
        Handle handle = new Handle(id);
        ObjectId objId = Active.Database.GetObjectId(false, handle, 0);

        using (Transaction trans = Active.Database.TransactionManager.StartTransaction())
        {
            // Open the object
            Entity entity = (Entity)trans.GetObject(objId, OpenMode.ForRead);

            // Check if the entity is a BlockReference or Polyline
            if (entity is BlockReference || entity is Polyline)
            {
                // Get the extents (bounding box) of the entity
                Extents3d extents = entity.GeometricExtents;

                extents.TransformBy(

                    Active.Editor.CurrentUserCoordinateSystem.Inverse()
                );

                Active.Editor.ZoomWindow(extents.MinPoint, extents.MaxPoint);

                trans.Commit();
            }
            else
            {
                // Handle cases where the object is not a BlockReference or Polyline
                Active.Editor.WriteMessage("The entity is neither a BlockReference nor a Polyline.");
            }
        }
    }

    //private static void ZoomWin(Editor ed, Point3d min, Point3d max)
    //{
    //    Point2d min2d = new Point2d(min.X, min.Y);

    //    Point2d max2d = new Point2d(max.X, max.Y);

    //    ViewTableRecord view =

    //        new ViewTableRecord();

    //    view.CenterPoint =

    //        min2d + ((max2d - min2d) / 2.0);

    //    view.YDim = max2d.Y - min2d.Y;

    //    view.XDim = max2d.X - min2d.X;

    //    ed.SetCurrentView(view);
    //}
}