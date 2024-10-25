using YMplugins.Contracts.Dto;

namespace YMplugins.Contracts;

public interface ICreateDwgService
{
    string[] Create(PrintInfo[] printData);
}