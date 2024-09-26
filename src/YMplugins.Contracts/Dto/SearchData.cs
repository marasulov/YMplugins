using YMplugins.Contracts.Dto.Enums;

namespace YMplugins.Contracts.Dto
{
    public class SearchData
    {
        public PrintByOption SelectedPrintByOption { get; set; }
        public bool IsSearchOnModel { get; set; }
        public bool IsSearchOnLayouts { get; set; }
        public string SelectedBlockName { get; set; }
        public string AttributeName { get; set; }
        public string AttributeValue { get; set; }
        public bool IsCheckedNumbering { get; set; }
        public int NumerationStartValue { get; set; }
        public string Prefix { get; set; }
        public string Suffix { get; set; }
        public PrintingOrder SelectedPrintingOrder { get; set; }

        //public SearchData(PrintByOption selectedPrintByOption, bool isSearchOnModel, bool isSearchOnLayouts, string selectedBlockName, string attributeName, bool isCheckedNumbering, int numerationStartValue, string prefix, string suffix, PrintingOrder selectedPrintingOrder)
        //{
        //    SelectedPrintByOption = selectedPrintByOption;
        //    IsSearchOnModel = isSearchOnModel;
        //    IsSearchOnLayouts = isSearchOnLayouts;
        //    SelectedBlockName = selectedBlockName;
        //    AttributeName = attributeName;
        //    IsCheckedNumbering = isCheckedNumbering;
        //    NumerationStartValue = numerationStartValue;
        //    Prefix = prefix;
        //    Suffix = suffix;
        //    SelectedPrintingOrder = selectedPrintingOrder;
        //}
    }
}