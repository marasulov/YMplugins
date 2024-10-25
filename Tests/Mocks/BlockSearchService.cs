using System.Diagnostics;
using YMplugins.Contracts;

namespace Mocks
{
    public class BlockSearchService 
    {
        public string GetActiveDocumentName()
        {
            var acadFileName = "autocad file name ";
            Debug.Print(acadFileName);
            return acadFileName;
        }
    }
}