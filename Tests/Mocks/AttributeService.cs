using System.Collections.Generic;
using System.Collections.ObjectModel;
using YMplugins.Contracts;
using YMplugins.Contracts.Dto;

namespace Mocks
{
    public class AttributeService : IAttributesService
    {
        public List<BlockAttribute>? GetAttributesForBlock(string selectedBlockName)
        {
            var blocks = new List<BlockAttribute>();
            for (int i = 0; i < 50; i++)
            {
                var newHoleDto = "atttrs" + selectedBlockName + " " + i;
                blocks.Add(new BlockAttribute($"attr{newHoleDto}", $"value{newHoleDto}"));
            }

            return blocks;
        }


        public ObservableCollection<PrintInfo> GetPrintInfosForBlock(ObservableCollection<PrintInfo> printInfos, string selectedAttribute, int numerationStartValue, string prefix, string suffix, bool isCheckedNumbering)
        {

            foreach (var printInfo in printInfos)
            {
                printInfo.FileName = $"{prefix}{printInfo.ObjectId}{selectedAttribute}{suffix}";
                if (numerationStartValue == null) continue;
                if (isCheckedNumbering)
                {
                    printInfo.FileName = $"{prefix}{printInfo.ObjectId}{selectedAttribute}{numerationStartValue}{suffix}";
                    numerationStartValue++;
                }
                else
                {
                    printInfo.FileName = $"{prefix}{printInfo.ObjectId}{selectedAttribute}{suffix}";
                }

            }
            return printInfos;
        }

    }
}