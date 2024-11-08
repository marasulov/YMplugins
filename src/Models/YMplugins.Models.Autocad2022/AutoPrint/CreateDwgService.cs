using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Gile.AutoCAD.Extension;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using YMplugins.Contracts;
using YMplugins.Contracts.Dto;
using YMplugins.Models.DbCad;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;

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

                    // Начинаем транзакцию для открытия объекта
                    using Transaction acTrans = Active.Database.TransactionManager.StartTransaction();
                    try
                    {
                        var minPoint = new Point3d(printInfo.Position.X, printInfo.Position.Y, 0);
                        var maxPoint = new Point3d(minPoint.X + printInfo.XDim,
                            minPoint.Y + printInfo.YDim, 0);
                        Active.Editor.WriteMessage($"minpoint {minPoint} maxpoint {maxPoint}");
                        var selectedIds = SelectCrossingWindow(minPoint, maxPoint);
                        var dwgFileName = printInfo.FileName + ".dwg";
                        var fileName = Path.Combine(drawingPath, dwgFileName);
                        CopyObjectsNewDatabases(selectedIds, fileName);
                        //CreateNewFileFromSelection(selectedIds, fiePath);
                        var newFileName = DbUtils.ZoomFilesAndSave(fileName);
                        File.Delete(fileName);
                        File.Move(newFileName, fileName);
                        createdFiles.Add(dwgFileName);
                    }
                    catch (Autodesk.AutoCAD.Runtime.Exception ex)
                    {
                        Active.Editor.WriteMessage($"\nОшибка: {ex.Message}");
                    }

                    acTrans.Commit();
                }
            }

            return createdFiles.ToArray();

        }

        public ObjectIdCollection SelectCrossingWindow(Point3d minPoint3d, Point3d maxPoint3d)
        {
            var acObjIdColl = new ObjectIdCollection();

            using (var acLckDocCur = Active.Document.LockDocument())
            {
                // Start a transaction
                using (var acTrans = Active.Document.TransactionManager.StartTransaction())
                {
                    var psr = Active.Editor.SelectCrossingWindow(minPoint3d, maxPoint3d);
                    if (psr.Value != null)
                    {
                        Active.Editor.WriteMessage($"selected {psr.Value.Count.ToString()}");
                    }

                    
                    if (psr.Status == PromptStatus.OK)
                    {
                        var cnt = 0;
                        using (Active.Document.TransactionManager.StartTransaction())
                        {
                            //foreach (var oID in psr.Value.GetObjectIds())
                            //{
                            //    var ent = (Entity)oID.GetObject(OpenMode.ForRead);
                            //    cnt += 1;

                            //    acObjIdColl.Add(oID);
                            //}
                            acObjIdColl = new ObjectIdCollection(psr.Value.GetObjectIds());
                        }
                    }
                    acTrans.Commit();
                }
            }

            return acObjIdColl;
        }

        public void CopyObjectsNewDatabases(ObjectIdCollection acObjIdColl, string dwgFilename)
        {
            try
            {
                using var db = new Database();

                using (Transaction acTrans = db.TransactionManager.StartTransaction())
                {

                    BlockTable acBlkTblNewDoc = acTrans.GetObject(db.BlockTableId,
                        OpenMode.ForRead) as BlockTable;

                    BlockTableRecord acBlkTblRecNewDoc = acTrans.GetObject(acBlkTblNewDoc[BlockTableRecord.ModelSpace],
                        OpenMode.ForRead) as BlockTableRecord;

                    var acIdMap = new IdMapping();
                    Active.Database.WblockCloneObjects(acObjIdColl, acBlkTblRecNewDoc.ObjectId, acIdMap,
                        DuplicateRecordCloning.Ignore, false);

                    acTrans.Commit();
                }



                var version = GetCurrentAcadVersion(); ;

                db.SaveAs(dwgFilename, version);
                Active.Editor.WriteMessage("\nNew file created at: " + dwgFilename);
            }
            catch (Autodesk.AutoCAD.Runtime.Exception e)
            {
                MessageBox.Show($"can't create file {e.Message}");
            }
        }

        public static DwgVersion GetCurrentAcadVersion()
        {
            var version = GetAcadVersionInfo();

            var dwgVersion = DwgVersion.Current;
            if (version.Equals("2016"))
            {
                dwgVersion = DwgVersion.AC1027;
            }

            return dwgVersion;

        }

        private static string GetAcadVersionInfo()
        {
            // credit Gile: https://forums.autodesk.com/t5/net/which-autocad-vertical-am-i-using/m-p/6861378#M51975

            var productKey = HostApplicationServices.Current.UserRegistryProductRootKey;
            var groups = Regex.Match(productKey, @"ACAD-([0-9A-F])\d(\d{2}):([0-9A-F]{3})").Groups;
            string release;
            switch (groups[1].Value)
            {
                case "5": release = "2007"; break;
                case "6": release = "2008"; break;
                case "7": release = "2009"; break;
                case "8": release = "2010"; break;
                case "9": release = "2011"; break;
                case "A": release = "2012"; break;
                case "B": release = "2013"; break;
                case "D": release = "2014"; break;
                case "E": release = "2015"; break;
                case "F": release = "2016"; break;
                case "0": release = "2017"; break;
                case "1": release = "2018"; break;
                case "2": release = "2019"; break;
                case "3": release = "2020"; break;
                case "4": release = "2021"; break;
                default: release = "unknown"; break;
            }

            return release;
        }
    }

}