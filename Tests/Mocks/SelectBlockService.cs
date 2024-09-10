using System.Collections.Generic;
using YMplugins.Contracts;
using YMplugins.ViewModels.Commands;

namespace Mocks
{
    public class SelectBlockService : ISelectBlockService
    {
        public int SelectBlock()
        {
            return 654646464;
        }
    }
}