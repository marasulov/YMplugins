using System;
using System.Collections.Generic;
using System.Text;
using YMplugins.Contracts;
using YMplugins.Contracts.Dto;

namespace Mocks
{
    public class PrintEngine : IPrintEngine
    {
        public void PrintObjects(IEnumerable<int> objects, string fileName, PrintData data)
        {
            // Логика печати: расстановка объектов, задание порядка (X, Y или Custom)
            // Вызов API печати AutoCAD с указанием объекта, имени файла и порядка печати
        }
    }
}
