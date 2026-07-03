#if NET8_0_OR_GREATER
using Gile.AutoCAD.R25.Extension;
#else
using Gile.AutoCAD.R20.Extension;
#endif
using System.IO;
using YMplugins.Contracts;

namespace YMplugins.Models.Autocad2024.Utils
{
    public class AutoCadFileService : IAutoCadFileService
    {
        public string GetActiveDocumentName()
        {
            return Path.Combine(Path.GetDirectoryName(Active.Document.Name) ?? string.Empty,
                $"{Path.GetFileNameWithoutExtension(Active.Document.Name)}.pdf");
        }
    }
}