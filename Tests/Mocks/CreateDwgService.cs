using System.Linq;
using YMplugins.Contracts;
using YMplugins.Contracts.Dto;

namespace Mocks
{
    public class CreateDwgService : ICreateDwgService
    {
        public string[] Create(PrintInfo[] printData)
        {
            return "throw new NotImplementedException();".ToArray().Select(x => x.ToString()).ToArray();
        }
    }
}
