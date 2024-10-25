using YMplugins.Contracts.Dto;

namespace YMplugins.Contracts;
public interface IDeleteEmptyLayoutsService
{
    void DeleteEmptyLayouts(PrintInfo[] printDatas);
}