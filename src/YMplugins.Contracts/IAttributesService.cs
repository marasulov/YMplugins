using System.Collections.Generic;
using System.Collections.ObjectModel;
using YMplugins.Contracts.Dto;

namespace YMplugins.Contracts
{
    public interface IAttributesService
    {
        List<BlockAttribute> GetAttributesForBlock(string selectedBlockName);
        List<PrintInfo> GetPrintInfosForBlock(List<PrintInfo> printInfos, string selectedAttribute, int numerationStartValue, string prefix, string suffix, bool isCheckedNumbering);
    }
}
