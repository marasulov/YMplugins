using System.Collections.Generic;
using System.Collections.ObjectModel;
using YMplugins.Contracts.Dto;

namespace YMplugins.Contracts
{
    public interface IAttributesService
    {
        List<BlockAttribute> GetAttributesForBlock(string selectedBlockName);
        ObservableCollection<PrintInfo> GetPrintInfosForBlock(ObservableCollection<PrintInfo> printInfos, string selectedAttribute, int numerationStartValue, string prefix, string suffix, bool isCheckedNumbering);
    }
}
