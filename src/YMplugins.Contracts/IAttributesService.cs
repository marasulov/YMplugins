using System;
using System.Collections.Generic;
using System.Text;

namespace YMplugins.Contracts
{
    public interface IAttributesService
    {
        IEnumerable<string> GetAttributesForBlock(string selectedBlockId);
    }
}
