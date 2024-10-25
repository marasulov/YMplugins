using System;
using System.Collections.Generic;
using System.Text;
using YMplugins.Contracts;
using YMplugins.Contracts.Dto;

namespace Mocks
{
    public class SetLayoutPlotSettingService : ISetLayoutPlotSettingService
    {
        public void Set(PrintInfo[] printDatas)
        {
            Console.WriteLine("setting layouts");
        }
    }
}
