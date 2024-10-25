using YMplugins.Contracts.Dto;

namespace YMplugins.Contracts;

public interface ISetLayoutPlotSettingService
{
    void Set(PrintInfo[] printDatas);
}