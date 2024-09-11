using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using Dreambuild.AutoCAD;
using Gile.AutoCAD.Extension;
using SimpleInjector;
using YMplugins.Models.Acad2022.Services;
using YMplugins.Services.SettingsReader;
using YMplugins.Services.Translator;
using YMplugins.Services.Translator.Google;

namespace YMplugins.Addin.Autocad2022.Commands.Translator
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

        /// <summary>
        /// Selects entities on given layer.
        /// </summary>
        [CommandMethod("SelectByLayer2")]
        public static void SelectByLayer()
        {
            Active.Editor.WriteMessage(GetAllLayerNames(Active.Database).Length.ToString());
        }

        public static string[] GetAllLayerNames(Database db = null)
        {
            return DbHelper.GetSymbolTableRecordNames((db ?? HostApplicationServices.WorkingDatabase).LayerTableId);
        }
    }
}
