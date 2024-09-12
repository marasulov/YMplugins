using System.Collections.Generic;
using YMplugins.Contracts.Dto;

namespace YMplugins.Contracts
{
    public interface IAttributesService
    {
        IEnumerable<BlockAttribute> GetAttributesForBlock(long selectedBlockId);
    }
}
