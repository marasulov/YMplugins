using Autodesk.AutoCAD.DatabaseServices;
using Gile.AutoCAD.Extension;

namespace YMplugins.Models.Autocad2022.Utils;

public class Convertors
{
    public static ObjectId IntToObjectId(int value)
    {
        Handle handle = new Handle(value);
        ObjectId objId = Active.Database.GetObjectId(false, handle, 0);
        
        return objId;
    }
}