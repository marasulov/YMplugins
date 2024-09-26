using Autodesk.AutoCAD.PlottingServices;
using System;
using System.Globalization;
using System.Text.RegularExpressions;
using YMplugins.Contracts.Dto;

namespace YMplugins.Models.Autocad2022.Utils.Print
{
    public class CanonNameResolver
    {
        public string GetCanonNameByWidthAndHeight(PrintInfo printInfo)
        {
            var standartCopier = new StandartCopier();
            var pConfig = PlotConfigManager.SetCurrentConfig(standartCopier.Pc3Source);

            var pat = @"\d{1,}?\.\d{2}";
            var canonName = "";
            var pattern = new Regex(pat, RegexOptions.Compiled | RegexOptions.Singleline);

            foreach (var line in pConfig.CanonicalMediaNames)
            {
                if (!pattern.IsMatch(line)) continue;
                var items = DivideStringToWidthAndHeight(pattern, line);

                double curWidth;
                double curHeight;

                if (printInfo.IsFormatHorizontal())
                {
                    curWidth = Math.Round(printInfo.Width / printInfo.ScaleX);
                    curHeight = Math.Round(printInfo.Height / printInfo.ScaleX);
                }
                else
                {
                    curWidth = Math.Round(printInfo.Height / printInfo.ScaleX);
                    curHeight = Math.Round(printInfo.Width / printInfo.ScaleX);
                }

                if ((items.Item1 == curWidth) && (items.Item2 == curHeight))
                {
                    canonName = line;
                    break;
                }
            }

            return canonName;
        }

        private static (double, double) DivideStringToWidthAndHeight(Regex pattern, string line)
        {
            var str2 = pattern.Matches(line, 0);

            var strWidth = str2[0].ToString();
            var strHeight = str2[1].ToString();
            var strWidthD = Convert.ToDouble(strWidth, CultureInfo.InvariantCulture);
            var strheightD = Convert.ToDouble(strHeight, CultureInfo.InvariantCulture);

            return (strWidthD, strheightD);
        }
    }
}
