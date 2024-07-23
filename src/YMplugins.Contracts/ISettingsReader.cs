using System.Collections.Generic;

namespace YMplugins.Contracts
{
    public interface ISettingsReader
    {
        List<string> GetFamilyNames();
    }
}
