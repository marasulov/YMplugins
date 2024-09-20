using System.Collections.Generic;
using System.Linq;
using YMplugins.Contracts;
using YMplugins.Contracts.Dto;

namespace Mocks
{
    public class NamingService : INamingService
    {
        //TODO 
        public string GenerateFileName(PrintData data, IEnumerable<int> objectsToPrint)
        {
            var prefix = data.Prefix;
            var suffix = data.Suffix;

            // Создаем имя файла на основе атрибутов блоков или данных полилиний
            var baseName = objectsToPrint;

            return $"{prefix}_{baseName}_{suffix}";
        }
    }
}
