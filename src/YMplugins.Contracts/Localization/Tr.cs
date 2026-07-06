using System;

namespace YMplugins.Contracts.Localization
{
    /// <summary>
    ///     Строки интерфейса плагина. Язык выбирается один раз при загрузке
    ///     плагина по языку интерфейса AutoCAD (системная переменная LOCALE).
    /// </summary>
    public static class Tr
    {
        public static bool IsRussian { get; private set; }

        /// <summary>
        ///     Устанавливает язык интерфейса. Принимает код языка AutoCAD
        ///     (LOCALE: "ru", "en", ...) либо любой ISO-код культуры.
        /// </summary>
        public static void SetLanguage(string languageCode)
        {
            IsRussian = languageCode != null &&
                        languageCode.StartsWith("ru", StringComparison.OrdinalIgnoreCase);
        }

        private static string R(string ru, string en) => IsRussian ? ru : en;

        // --- Окно AutoPrint ---
        public static string WindowTitle => R("Печать в PDF", "Print to PDF");
        public static string PrintBy => R("Что печатать", "Print by");
        public static string CreatePdf => R("Создать PDF", "Create PDF");
        public static string CreateDwg => R("Создать DWG", "Create DWG");
        public static string ByBlock => R("По блоку", "By block");
        public static string ByPolyline => R("По полилинии", "By polyline");
        public static string SearchOnModel => R("Искать в модели", "Search on model");
        public static string SearchOnLayouts => R("Искать на листах", "Search on layouts");
        public static string DeleteEmptyLayouts => R("Удалить пустые листы", "Delete empty layouts");
        public static string SetLayoutsToPrintBorders => R("Настроить листы по границам печати", "Set layouts to print borders");
        public static string CombinePdfs => R("Объединить PDF в один файл", "Combine PDFs");
        public static string EnterOutputFileName => R("Имя итогового файла", "Enter output file name");
        public static string SelectBlockName => R("Имя блока", "Select block name");
        public static string SelectOnScreen => R("Указать на экране", "Select on screen");
        public static string SelectLayer => R("Слой", "Select layer");
        public static string SelectOnLayerScreen => R("Указать слой на экране", "Select layer on screen");
        public static string EnterPolylineScale => R("Масштаб полилинии (1 = автоопределение)", "Polyline scale (1 = auto-detect)");
        public static string PrintingOrder => R("Порядок печати", "Printing order");
        public static string PrintingNumberingOrder => R("Порядок нумерации при печати", "Printing numbering order");
        public static string Naming => R("Именование", "Naming");
        public static string AttributeValue => R("Значение атрибута:", "Attribute value:");
        public static string SelectAttribute => R("Выберите атрибут", "Select attribute");
        public static string AttributeNameLabel => R("Имя атрибута: ", "Attribute name: ");
        public static string AttributeValueLabel => R("Значение: ", "Value: ");
        public static string Numeration => R("Нумерация:", "Numeration:");
        public static string EnterNumberingStart => R("Начальное значение нумерации", "Numbering start value");
        public static string PrefixTag => R("Префикс:", "Prefix:");
        public static string EnterPrefix => R("Введите префикс", "Enter prefix");
        public static string SuffixTag => R("Суффикс:", "Suffix:");
        public static string EnterSuffix => R("Введите суффикс", "Enter suffix");
        public static string ColId => R("Id", "Id");
        public static string ColSpace => R("Пространство", "Space");
        public static string ColFormat => R("Формат", "Format");
        public static string ColFileName => R("Имя файла", "File name");
        public static string ColZoom => R("Навигация", "Zoom");
        public static string ShowButton => R("Показать", "Show");
        public static string ColPrinting => R("Печать", "Print");
        public static string PrintButton => R("Печать", "Print");

        // --- Сообщения ---
        public static string HeaderContentFormat => R("Найдено блоков: {0} | Выбрано для печати: {1}", "Blocks found: {0} | Selected for printing: {1}");
        public static string ErrBlockNotSelected => R("Блок не выбран", "Block not selected");
        public static string ErrFileNameAbsent => R("Не задано имя файла.", "File name is missing.");
        public static string ErrOutputFileNameRequired => R("Укажите имя итогового файла для объединения PDF.", "Output file name is required when combining PDFs.");
        public static string PrintErrorTitle => R("Ошибка печати", "Print error");
        public static string FilesCreatedTitle => R("Создание файлов успешно завершено", "Files created successfully");

        // --- Лента ---
        public static string PanelTranslator => R("Переводчик", "Translator");
        public static string PanelAutoPrint => R("Автопечать", "AutoPrint");
        public static string BtnTranslate => R("Перевести", "Translate");
        public static string BtnAutoPrint => R("Автопечать", "AutoPrint");
        public static string SourceLanguage => R("Исходный язык", "Source language");
        public static string TargetLanguage => R("Язык перевода", "Target language");
        public static string DetectLanguage => R("Определить язык", "Detect language");
    }
}
