using YMplugins.Contracts.Dto;

namespace YMplugins.Contracts
{
    public interface IPrintService
    {
        string[] Print(PrintInfo[] data);
    }
}