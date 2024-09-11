namespace YMplugins.Addin.Autocad2022.Commands.Translator
{
    public class TranslationState
    {
        public string SelectedSourceLanguage { get; set; } = "auto";
        public string SelectedTargetLanguage { get; set; } = "en";

        private static TranslationState _instance;
        public static TranslationState Instance => _instance ??= new TranslationState();
    }
}
