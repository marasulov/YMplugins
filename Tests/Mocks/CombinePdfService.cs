using System;
using YMplugins.Contracts;

namespace Mocks
{
    public class CombinePdfService : ICombinePdfService
    {
        public string Combine(string[] filenames, string targetPdf)
        {
            var filename = string.Join("-", filenames);
            Console.WriteLine(filename, "filename");
            return filename;
        }
    }
}