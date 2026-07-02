using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System.Text.RegularExpressions;

namespace YMplugins.Models.DbCad
{
    public static class DbUtils
    {
        /// <summary>
        /// Зуммирует объекты в файле по границам
        /// </summary>
        /// <param name="fileName">Имя файла</param>
        /// <returns></returns>
        public static string ZoomFilesAndSave(string fileName)
        {
            var newFileName = "";
            using (var db = new Database(false, false))
            {
                db.ReadDwgFile(fileName, FileOpenMode.OpenForReadAndReadShare, true, null);
                var prevDb = HostApplicationServices.WorkingDatabase;
                HostApplicationServices.WorkingDatabase = db;
                db.UpdateExt(true);
                using (var vTab = db.ViewportTableId.GetObject(OpenMode.ForRead) as ViewportTable)
                {
                    var acVptId = vTab["*Active"];
                    using (var vpTabRec = acVptId.GetObject(OpenMode.ForWrite) as ViewportTableRecord)
                    {
                        var scrRatio = vpTabRec.Width / vpTabRec.Height;
                        var matWCS2DCS = Matrix3d.PlaneToWorld(vpTabRec.ViewDirection);
                        matWCS2DCS = Matrix3d.Displacement(vpTabRec.Target - Point3d.Origin) * matWCS2DCS;
                        matWCS2DCS = Matrix3d.Rotation(-vpTabRec.ViewTwist,
                                         vpTabRec.ViewDirection,
                                         vpTabRec.Target)
                                     * matWCS2DCS;
                        matWCS2DCS = matWCS2DCS.Inverse();
                        var extents = new Extents3d(db.Extmin, db.Extmax);
                        extents.TransformBy(matWCS2DCS);
                        var width = extents.MaxPoint.X - extents.MinPoint.X;
                        var height = extents.MaxPoint.Y - extents.MinPoint.Y;
                        var center = new Point2d((extents.MaxPoint.X + extents.MinPoint.X) * 0.5,
                            (extents.MaxPoint.Y + extents.MinPoint.Y) * 0.5);
                        if (width > height * scrRatio)
                            height = width / scrRatio;
                        vpTabRec.Height = height;
                        vpTabRec.Width = height * scrRatio;
                        vpTabRec.CenterPoint = center;
                    }
                }

                HostApplicationServices.WorkingDatabase = prevDb;

                newFileName = fileName.Substring(0, fileName.Length - 4) + "z.dwg";
                var version = GetCurrentAcadVersion();

                db.SaveAs(newFileName, version);
            }

            return newFileName;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
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