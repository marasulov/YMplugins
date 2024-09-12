using System;
using System.Collections.Generic;
using System.Text;
using YMplugins.Contracts;
using YMplugins.Contracts.Dto;

namespace Mocks
{
    public class AttributesService : IAttributesService
    {
        public IEnumerable<BlockAttribute>? GetAttributesForBlock(long selectedBlockId)
        {
            var blocks = new List<BlockAttribute>();
            for (int i = 0; i < 50; i++)
            {
                var newHoleDto = "atttrs" + selectedBlockId + " " + i;
                blocks.Add(new BlockAttribute($"attr{newHoleDto}", $"value{newHoleDto}"));
            }
            
            return blocks;
        }
    }
}
