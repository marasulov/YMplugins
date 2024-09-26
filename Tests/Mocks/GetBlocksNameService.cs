using System.Collections.Generic;
using System.Collections.ObjectModel;
using YMplugins.Contracts;

namespace Mocks
{
    public class GetBlocksNameService : IGetBlocksNameService

    {
        public ObservableCollection<string> GetBlocksName()
        {
            var blocks = new ObservableCollection<string>();
            for (int i = 0; i < 50; i++)
            {
                var newHoleDto = "blockname" + i;
                blocks.Add(newHoleDto);
            }

            return blocks;
        }
    }
}