using System.Collections.Generic;
using System.Collections.ObjectModel;
using YMplugins.Contracts.Dto;

namespace YMplugins.Contracts
{
    public interface INamingService
    {
        ObservableCollection<PrintInfo> GenerateFileName(IEnumerable<PrintInfo> printInfos, int numerationValue);
    }
}