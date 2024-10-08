using System.Collections.Generic;
using YMplugins.Contracts.Dto;

namespace YMplugins.Models.Autocad2022.Contracts
{
    public interface IObjectFinder
    {
        List<PrintInfo> FindObjects();
    }
}