using System;
using YMplugins.Contracts;
using YMplugins.Contracts.Dto;

namespace Mocks
{
    public class DeleteEmptyLayoutsService : IDeleteEmptyLayoutsService
    {
        public void DeleteEmptyLayouts(PrintInfo[] printDatas)
        {
            Console.WriteLine("deleting layouts");
        }
    }
}
