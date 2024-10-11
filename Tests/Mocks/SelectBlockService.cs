using YMplugins.Contracts;

namespace Mocks
{
    public class SelectBlockService : ISelectBlockService
    {
        public string? SelectBlock()
        {
            return ".ToString()";
        }
    }
}