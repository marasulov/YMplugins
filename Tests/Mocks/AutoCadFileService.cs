using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using YMplugins.Contracts;

namespace Mocks
{
    public class AutoCadFileService : IAutoCadFileService
    {
        public string GetActiveDocumentName()
        {
            var acadFileName = "autocad file name ";
            Debug.Print(acadFileName);
            return acadFileName;
        }
    }
}
