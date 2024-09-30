using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using YMplugins.Contracts;

namespace Mocks
{
    public class CombinePdfService : ICombinePdfService
    {
        public bool Combine(string[] filenames, string targetPdf)
        {
            Console.WriteLine(string.Join("-", filenames), "filename");
            return true;
        }
    }
}
