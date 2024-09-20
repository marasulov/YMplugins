using System.Collections.Generic;
using YMplugins.Contracts;
using YMplugins.Contracts.Dto;

namespace Mocks
{
    public class AttributesService : IAttributesService
    {
        public IEnumerable<BlockAttribute>? GetAttributesForBlock(string selectedBlockName)
        {
            var blocks = new List<BlockAttribute>();
            for (int i = 0; i < 50; i++)
            {
                var newHoleDto = "atttrs" + selectedBlockName + " " + i;
                blocks.Add(new BlockAttribute($"attr{newHoleDto}", $"value{newHoleDto}"));
            }

            return blocks;
        }
    }
}