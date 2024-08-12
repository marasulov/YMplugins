using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.Runtime;
using SimpleInjector;
using YMplugins.Models.Acad2022.Services;
using YMplugins.Services.SettingsReader;
using YMplugins.Services.Translator.Google;
using YMplugins.Services.Translator;

namespace YMplugins.Models.Acad2022.Commands.Translator
{
    public class TranslateWithOriginalCommand
    {
        //[CommandMethod("TrToEnWithOrg")]
        //public void TranslateText()
        //{
        //    var settings = new TranslationSettings();

        //    var container = new Container();
        //    container.RegisterInstance(settings);
        //    container.Register(() => new SettingsService());
        //    container.Register<ITextTranslator, GoogleTranslateService>();
        //    container.Register(() =>
        //    {
        //        var settingsService = container.GetInstance<SettingsService>();
        //        var config = settingsService.LoadSettings();
        //        return new DictionaryService(config.DictFilePath);
        //    }, Lifestyle.Singleton);

        //    container.Register<TextProcessor>();

        //    container.Verify();
        //    var textProcessor = container.GetInstance<TextProcessor>();

        //    textProcessor.ProcessTexts(settings);

        //}
    }
}
