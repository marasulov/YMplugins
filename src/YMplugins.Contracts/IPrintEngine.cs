using System.Collections.Generic;
using YMplugins.Contracts.Dto;

namespace YMplugins.Contracts
{
    public interface IPrintEngine
    {
        void PrintObjects(IEnumerable<int> objects, string fileName, PrintData data);
    }
}