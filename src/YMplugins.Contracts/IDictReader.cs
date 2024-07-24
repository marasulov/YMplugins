using System.Collections.Generic;

namespace YMplugins.Contracts
{
    public interface IDictReader
    {
        Dictionary<string, string> GetDictDb();
    }
}
