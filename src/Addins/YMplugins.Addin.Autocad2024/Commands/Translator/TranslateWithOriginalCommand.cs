using Autodesk.AutoCAD.Runtime;
using SimpleInjector;
using YMplugins.Models.Autocad2024.Translator;
using YMplugins.Services.SettingsReader;
using YMplugins.Services.Translator;
using YMplugins.Services.Translator.Google;

namespace YMplugins.Addin.Autocad2024.Commands.Translator
{
    public class TranslateWithOriginalCommand
    {
        [CommandMethod("YmTranslate")]
        public void TranslateText()
        {
            var settings = new TranslationSettings
            {
                SourceLanguage = TranslationState.Instance.SelectedSourceLanguage,
                TargetLanguage = TranslationState.Instance.SelectedTargetLanguage
            };

            var container = new Container();
            container.RegisterInstance(settings);
            container.Register(() => new SettingsService());
            container.Register<ITextTranslator, GoogleTranslateService>();
            container.Register(() =>
            {
                var settingsService = container.GetInstance<SettingsService>();
                var config = settingsService.LoadSettings();
                return new DictionaryService(config.DictFilePath);
            }, Lifestyle.Singleton);

            container.Register<TextProcessor>();

            container.Verify();
            var textProcessor = container.GetInstance<TextProcessor>();

            textProcessor.ProcessTexts(settings);

        }
    }
}
