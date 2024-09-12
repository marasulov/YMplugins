using System;
using System.Collections.Generic;
using YMplugins.Contracts;
using YMplugins.ViewModels.Commands;

namespace Mocks
{
    public class SelectBlockService : ISelectBlockService
    {
        public Tuple<long, string> SelectBlock()
        {
            return new Tuple<long, string>( 654646464, ".ToString()");
        }
    }
}