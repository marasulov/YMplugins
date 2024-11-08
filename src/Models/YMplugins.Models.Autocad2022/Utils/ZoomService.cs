using Autodesk.AutoCAD.ApplicationServices;
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
            DBObject dbObj = trans.GetObject(objId, OpenMode.ForRead);
            BlockTableRecord btr = trans.GetObject(dbObj.OwnerId, OpenMode.ForRead) as BlockTableRecord;
            if (btr == null) return;

            if (!btr.IsLayout) return;

            Layout layout = trans.GetObject(btr.LayoutId, OpenMode.ForRead) as Layout;
            string layoutName = layout.LayoutName;

            LayoutManager layoutMgr = LayoutManager.Current;
            using (DocumentLock docLock = Active.Document.LockDocument())
            {
                if (layoutMgr.CurrentLayout != layoutName)
                {
                    layoutMgr.CurrentLayout = layoutName;
                }
            }

            ZoomToEntity(entity);

           
            trans.Commit();
        }
     
    }

    private void ZoomToEntity(Entity entity)
    {
        if (entity is BlockReference || entity is Polyline)
        {
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
}