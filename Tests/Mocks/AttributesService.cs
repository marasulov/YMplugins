using System;
using System.Collections.Generic;
using System.Text;
using YMplugins.Contracts;

namespace Mocks
{
    public class AttributesService : IAttributesService
    {
        public IEnumerable<string> GetAttributesForBlock(string selectedBlockId)
        {
            var blocks = new List<string>();
            for (int i = 0; i < 50; i++)
            {
                var newHoleDto = "atttrs" + selectedBlockId + " " + i;
                blocks.Add(newHoleDto);
            }

            return blocks;
        }
    }
}
