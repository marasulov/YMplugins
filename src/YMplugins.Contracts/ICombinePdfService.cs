using System;
using System.Collections.Generic;
using System.Text;

namespace YMplugins.Contracts
{
    public interface ICombinePdfService
    {
        string Combine(string[] filenames, string outputFileName);
    }
}
