using Autodesk.AutoCAD.Runtime;
using SimpleInjector;
using YMplugins.Models.Acad2022.Services;
using YMplugins.Services.SettingsReader;
using YMplugins.Services.Translator;
using YMplugins.Services.Translator.Google;

namespace YMplugins.Models.Acad2022.Commands.Translator
{
    public class TranslateToUzCommand
    {
        [CommandMethod("TrToUzWithOrg")]
        public void TranslateText()
        {
            var settings = new TranslationSettings
            {
                SourceLanguage = "en",
                TargetLanguage = "uz"
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

            textProcessor.ProcessTexts();

        }

    }
}
