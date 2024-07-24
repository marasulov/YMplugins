using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using Gile.AutoCAD.Extension;
using SimpleInjector;
using System.Collections.Generic;
using YMplugins.Contracts;
using YMplugins.Models.Acad2022.Services;
using YMplugins.Services.SettingsReader;
using YMplugins.Services.Translator;

namespace YMplugins.Models.Acad2022.Commands.Translator
{

    public class TranslateTextCommand
    {
        [CommandMethod("TranslateToTarget")]
        public void TranslateText()
        {
            var container = new Container();
            container.Register(() => new SettingsService());

            container.Register(() =>
            {
                var settingsService = container.GetInstance<SettingsService>();
                var config = settingsService.LoadSettings();
                return new DictionaryService(config.DictFilePath);
            }, Lifestyle.Singleton);

            container.Register<ITextTranslator, TextTranslator>();
            container.Register<TextProcessor>();

            container.Verify();
            var textProcessor = container.GetInstance<TextProcessor>();

            textProcessor.ProcessTexts();

        }
    }
}
