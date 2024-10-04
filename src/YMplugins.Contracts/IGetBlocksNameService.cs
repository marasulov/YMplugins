using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace YMplugins.Contracts
{
    public interface IGetBlocksNameService
    {
        List<string> GetBlocksName();
    }
}
