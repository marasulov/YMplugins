using Gile.AutoCAD.Extension;
using System.IO;
using YMplugins.Contracts;

namespace YMplugins.Models.Autocad2022.Utils
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