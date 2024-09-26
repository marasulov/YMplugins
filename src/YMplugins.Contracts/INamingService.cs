using System.Collections.Generic;
using YMplugins.Contracts.Dto;

namespace YMplugins.Contracts
{
    public interface INamingService
    {
        List<PrintInfo> GenerateFileName(IEnumerable<PrintInfo> printInfos, int numerationValue);
    }
}