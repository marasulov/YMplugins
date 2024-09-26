using System.Collections.ObjectModel;

namespace YMplugins.Contracts
{
    public interface IGetBlocksNameService
    {
        ObservableCollection<string> GetBlocksName();
    }
}
