using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Gile.AutoCAD.Extension;
using System;
using Autodesk.AutoCAD.EditorInput;
using YMplugins.Contracts;
using static System.Net.Mime.MediaTypeNames;

namespace YMplugins.Models.Autocad2022.Utils;

public class ZoomService : IZoomEntity
{
    public void Zoom(int id)
    {
        Handle handle = new Handle(id);
        ObjectId objId = Active.Database.GetObjectId(false, handle, 0);

        //using (Transaction trans = Active.Database.TransactionManager.StartTransaction())
        //{
        //    // Open the object for reading
        //    DBObject obj = trans.GetObject(objId, OpenMode.ForRead);

        //    // Get the entity type
        //    Type entityType = obj.GetType();

        //    // Optionally, you can check the exact type
        //    if (obj is Line)
        //    {
        //        // It's a Line entity
        //        Console.WriteLine("This is a Line entity.");
        //    }
        //    else if (obj is Circle)
        //    {
        //        // It's a Circle entity
        //        Console.WriteLine("This is a Circle entity.");
        //    }
        //    else if (obj is BlockReference)
        //    {
        //        // It's a BlockReference entity
        //        Console.WriteLine("This is a BlockReference entity.");
        //        obj.

        //    }
        //    else
        //    {
        //        // Handle other types or unknown entity types
        //        Console.WriteLine($"This is a {entityType.Name} entity.");
        //    }

        //    // Commit the transaction
        //    trans.Commit();
        //}

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
                // Get the current view from the viewport table record
                //ViewTableRecord view =
                //    trans.GetObject(Active.Database.CurrentViewportTableRecordId, OpenMode.ForWrite) as ViewTableRecord;

                //// Adjust the view center and height based on the entity's extents
                //view.CenterPoint = new Point2d((extents.MinPoint.X + extents.MaxPoint.X) / 2,
                //    (extents.MinPoint.Y + extents.MaxPoint.Y) / 2);
                //view.YDmim = extents.MaxPoint.Y - extents.MinPoint.Y;

                //// Optionally, adjust the width (aspect ratio)
                //view.XDim = extents.MaxPoint.X - extents.MinPoint.X;

                // Update the view
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

    //    view.YDmim = max2d.Y - min2d.Y;

    //    view.XDim = max2d.X - min2d.X;

    //    ed.SetCurrentView(view);
    //}
}