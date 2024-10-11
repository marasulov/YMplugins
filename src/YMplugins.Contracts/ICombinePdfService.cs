using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace YMplugins.Contracts
{
    public interface ICombinePdfService
    {
        string Combine(string[] filenames, string outputFileName);
    }
}
