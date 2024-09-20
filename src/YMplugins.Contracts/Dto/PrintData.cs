using YMplugins.Contracts.Dto.Enums;

namespace YMplugins.Contracts.Dto
{
    public class PrintData
    {
        public PrintByOption SelectedPrintByOption { get; set; }
        public bool IsSearchOnModel { get; set; }
        public bool IsSearchOnLayouts { get; set; }
        public string SelectedBlockName { get; set; }
        public string AttributeName { get; set; }
        public string AttributeValue { get; set; }
        public int NumerationStartValue { get; set; }
        public string Prefix { get; set; }
        public string Suffix { get; set; }
        public PrintingOrder SelectedPrintingOrder { get; set; }
    }
}