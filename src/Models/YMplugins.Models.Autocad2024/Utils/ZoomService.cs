using Autodesk.AutoCAD.DatabaseServices;
using Gile.AutoCAD.R20.Extension;
using System.Threading;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Internal;
using YMplugins.Contracts;

namespace YMplugins.Models.Autocad2024.Utils;

public class ZoomService : IZoomEntity
{
    public void Zoom(int id)
    {
        Handle handle = new Handle(id);
        ObjectId objId = Active.Database.GetObjectId(false, handle, 0);

        //using (DocumentLock docLock = Active.Document.LockDocument())
        //{
        using (Transaction trans = Active.Database.TransactionManager.StartTransaction())
        {
            // Open the object
            Entity entity = (Entity)trans.GetObject(objId, OpenMode.ForRead);
            DBObject dbObj = trans.GetObject(objId, OpenMode.ForRead);
            BlockTableRecord btr = trans.GetObject(dbObj.OwnerId, OpenMode.ForRead) as BlockTableRecord;
            if (btr == null) return;

            // Проверить, это Model Space или Layout

            if (!btr.IsLayout) return;

            Autodesk.AutoCAD.DatabaseServices.Layout layout = trans.GetObject(btr.LayoutId, OpenMode.ForRead) as Autodesk.AutoCAD.DatabaseServices.Layout;
            string layoutName = layout.LayoutName;
            //if (layoutName != "Model")
            //{

            LayoutManager layoutMgr = LayoutManager.Current;
            using (DocumentLock docLock = Active.Document.LockDocument())
            {
                if (layoutMgr.CurrentLayout != layoutName)
                {
                    layoutMgr.CurrentLayout = layoutName;
                }
            }


            //layoutMgr.SetCurrentLayoutId(layoutId);

            ZoomToEntity(entity);

            //}

            //else
            //{
            //    ZoomToEntity(entity);
            //}
            trans.Commit();
        }
        //}
    }

    private void ZoomToEntity(Entity entity)
    {
        if (entity is BlockReference || entity is Polyline)
        {
            // Get the extents (bounding box) of the entity
            Extents3d extents = entity.GeometricExtents;

            extents.TransformBy(

                Active.Editor.CurrentUserCoordinateSystem.Inverse()
            );

            Active.Editor.ZoomWindow(extents.MinPoint, extents.MaxPoint);


        }
        else
        {
            // Handle cases where the object is not a BlockReference or Polyline
            Active.Editor.WriteMessage("The entity is neither a BlockReference nor a Polyline.");
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