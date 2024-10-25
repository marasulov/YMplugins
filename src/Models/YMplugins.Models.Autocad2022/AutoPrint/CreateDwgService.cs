using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Gile.AutoCAD.Extension;
using System;
using System.Collections.Generic;
using System.IO;
using YMplugins.Contracts;
using YMplugins.Contracts.Dto;
using YMplugins.Models.DbCad;

namespace YMplugins.Models.Autocad2022.AutoPrint
{
    public class CreateDwgService : ICreateDwgService
    {

        public string[] Create(PrintInfo[] printData)
        {
            string drawingPath = Path.GetDirectoryName(Application.DocumentManager.CurrentDocument.Database.Filename);
            Active.Document.SendStringToExecute("REGENALL ", true, false, true);
            var createdFiles = new List<string>();

            var layersDictionary = LayersExtension.GetLayersIsBlockedCol();
            if (Active.Document != null)
            {
                Active.Document.LockOrUnlockLayers(false, ignoreCurrent: false, lockZero: true);

                foreach (var printInfo in printData)
                {
                    ObjectId objId = new ObjectId(new IntPtr(printInfo.ObjectId));

                    // Начинаем транзакцию для открытия объекта
                    using (Transaction acTrans = Active.Database.TransactionManager.StartTransaction())
                    {
                        try
                        {
                            DBObject dbObj = acTrans.GetObject(objId, OpenMode.ForRead);

                            // Получаем тип объекта
                            Type objType = dbObj.GetType();



                        }
                        catch (Autodesk.AutoCAD.Runtime.Exception ex)
                        {
                            Active.Editor.WriteMessage($"\nОшибка: {ex.Message}");
                        }

                        acTrans.Commit();


                    }
                }
            }

            return default;

        }
    }
}