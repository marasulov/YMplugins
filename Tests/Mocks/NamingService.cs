using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using YMplugins.Contracts;
using YMplugins.Contracts.Dto;

namespace Mocks
{
    public class NamingService : INamingService
    {

        //TODO

        //public string GenerateFileName(IEnumerable<SearchData> data, int objectToPrint, int numerationValue)
        //{
        //    var prefix = data.Prefix;
        //    var suffix = data.Suffix;

        //    string fileName = default;

        //    foreach (var printInfo in objectsToPrint)
        //    {
        //        var filename = prefix + data.AttributeValue + suffix;
        //        if (data.IsCheckedNumbering)
        //        {

        //            filename = prefix + data.AttributeValue + suffix;
        //        }

        //        fileName = _namingService.GenerateFileName(data, printInfo.ObjectId, numerationValue);
        //    }

           

        //    return filename;

        //}
        public ObservableCollection<PrintInfo> GenerateFileName(IEnumerable<PrintInfo> printInfos, int numerationValue)
        {
            foreach (var printInfo in printInfos)
            {
                printInfo.FileName = printInfo.FileName + numerationValue;
            }

            return (ObservableCollection<PrintInfo>)printInfos;
        }
    }

    public interface IGetAttributeValueService
    {
        public string GetValue(int blockId, string atrributeName);
    }
}
