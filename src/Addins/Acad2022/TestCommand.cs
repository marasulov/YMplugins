using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using Autodesk.AutoCAD.Runtime;
using Autodesk.Windows;
using YMplugins.Models.Acad2022.Commands.Translator;
using YMplugins.Services.Translator;

namespace YMplugins.Addin.Acad2022
{
    public class TestCommand : IExtensionApplication
    {
        public static void EnsureInitialized()
        {
            if (LanguageModeMap != null)
                return;
            LanguageModeMap = new Dictionary<string, string>();
            LanguageModeMap.Add("English", "en");
            LanguageModeMap.Add("Russian", "ru");
            LanguageModeMap.Add("Afrikaans", "af");
            LanguageModeMap.Add("Albanian", "sq");
            LanguageModeMap.Add("Arabic", "ar");
            LanguageModeMap.Add("Armenian", "hy");
            LanguageModeMap.Add("Azerbaijani", "az");
            LanguageModeMap.Add("Basque", "eu");
            LanguageModeMap.Add("Belarusian", "be");
            LanguageModeMap.Add("Bengali", "bn");
            LanguageModeMap.Add("Bulgarian", "bg");
            LanguageModeMap.Add("Catalan", "ca");
            LanguageModeMap.Add("Chinese", "zh-CN");
            LanguageModeMap.Add("Croatian", "hr");
            LanguageModeMap.Add("Czech", "cs");
            LanguageModeMap.Add("Danish", "da");
            LanguageModeMap.Add("Dutch", "nl");
            
            LanguageModeMap.Add("Esperanto", "eo");
            LanguageModeMap.Add("Estonian", "et");
            LanguageModeMap.Add("Filipino", "tl");
            LanguageModeMap.Add("Finnish", "fi");
            LanguageModeMap.Add("French", "fr");
            LanguageModeMap.Add("Galician", "gl");
            LanguageModeMap.Add("German", "de");
            LanguageModeMap.Add("Georgian", "ka");
            LanguageModeMap.Add("Greek", "el");
            LanguageModeMap.Add("Haitian Creole", "ht");
            LanguageModeMap.Add("Hebrew", "iw");
            LanguageModeMap.Add("Hindi", "hi");
            LanguageModeMap.Add("Hungarian", "hu");
            LanguageModeMap.Add("Icelandic", "is");
            LanguageModeMap.Add("Indonesian", "id");
            LanguageModeMap.Add("Irish", "ga");
            LanguageModeMap.Add("Italian", "it");
            LanguageModeMap.Add("Japanese", "ja");
            LanguageModeMap.Add("Korean", "ko");
            LanguageModeMap.Add("Lao", "lo");
            LanguageModeMap.Add("Latin", "la");
            LanguageModeMap.Add("Latvian", "lv");
            LanguageModeMap.Add("Lithuanian", "lt");
            LanguageModeMap.Add("Macedonian", "mk");
            LanguageModeMap.Add("Malay", "ms");
            LanguageModeMap.Add("Maltese", "mt");
            LanguageModeMap.Add("Norwegian", "no");
            LanguageModeMap.Add("Persian", "fa");
            LanguageModeMap.Add("Polish", "pl");
            LanguageModeMap.Add("Portuguese", "pt");
            LanguageModeMap.Add("Romanian", "ro");
            
            LanguageModeMap.Add("Serbian", "sr");
            LanguageModeMap.Add("Slovak", "sk");
            LanguageModeMap.Add("Slovenian", "sl");
            LanguageModeMap.Add("Spanish", "es");
            LanguageModeMap.Add("Swahili", "sw");
            LanguageModeMap.Add("Swedish", "sv");
            LanguageModeMap.Add("Tamil", "ta");
            LanguageModeMap.Add("Telugu", "te");
            LanguageModeMap.Add("Thai", "th");
            LanguageModeMap.Add("Turkish", "tr");
            LanguageModeMap.Add("Ukrainian", "uk");
            LanguageModeMap.Add("Urdu", "ur");
            LanguageModeMap.Add("Vietnamese", "vi");
            LanguageModeMap.Add("Welsh", "cy");
            LanguageModeMap.Add("Yiddish", "yi");
        }

        public static Dictionary<string, string> LanguageModeMap { get; set; }

        private static string _selsourceComboValue = "auto";
        private static string _selTargeComboValue = "en";

        [CommandMethod("TestCommand")]
        public void MyCommand()
        {
            EnsureInitialized();

            var sourceLangCombo = GetRibbonCombo("sourceLangCombo", "source");
            var targetLangCombo = GetRibbonCombo("targetLangCombo", "target");

            var commandHandler = new ButtonCommandHandler();

            sourceLangCombo.CurrentChanged += (sender, e) =>
            {
                
                var selectedItem = e.NewValue as RibbonButton;
                _selsourceComboValue = selectedItem.Tag.ToString();
                commandHandler.SetSelectedValue(_selsourceComboValue, _selTargeComboValue);
                
            };

            targetLangCombo.CurrentChanged += (sender, e) =>
            {

                var selectedItem = e.NewValue as RibbonButton;
                _selTargeComboValue = selectedItem.Tag.ToString();
                commandHandler.SetSelectedValue(_selsourceComboValue, _selTargeComboValue);
                //MessageBox.Show(selectedItem.Tag.ToString());
            };

            RibbonButton button1 = new RibbonButton();
            button1.Id = "_button1";
            button1.CommandHandler = commandHandler;

            // создаем контейнер для элементов
            RibbonPanelSource rbPanelSource = new RibbonPanelSource();
            rbPanelSource.Title = "Новая панель элементов";

            rbPanelSource.Items.Add(sourceLangCombo);
            rbPanelSource.Items.Add(targetLangCombo);
            rbPanelSource.Items.Add(new RibbonSeparator());
            rbPanelSource.Items.Add(button1);

            // создаем панель
            RibbonPanel rbPanel = new RibbonPanel();
            // добавляем на панель контейнер для элементов
            rbPanel.Source = rbPanelSource;

            RibbonTab rbTab = new RibbonTab();
            rbTab.Title = "Новая вкладка";
            rbTab.Id = "HabrRibbon";
            
            rbTab.Panels.Add(rbPanel);
            RibbonControl rbCtrl = ComponentManager.Ribbon;
            rbCtrl.Tabs.Add(rbTab);
            
            
        }

        private RibbonCombo GetRibbonCombo(string comboName, string prefix)
        {
            RibbonToolTip tt = new RibbonToolTip();
            RibbonCombo ribbonCombo = new RibbonCombo();
            ribbonCombo.Id = comboName;
            ribbonCombo.Text = prefix+"sadasd";
            tt.Content = "текст";
            ribbonCombo.ToolTip = tt;
            foreach (KeyValuePair<string, string> lang in LanguageModeMap)
            {
                var ribBtn = GetRibbonButton(prefix+lang.Value,lang.Key, lang.Value);
                ribbonCombo.Items.Add(ribBtn);
            }

            return ribbonCombo;
        }

        private RibbonButton GetRibbonButton(string id,string text, string tag)
        {
            var ribbonButton = new RibbonButton();
            ribbonButton.Id = id;
            ribbonButton.Text = text;
            
            ribbonButton.ShowText = true;
            ribbonButton.Tag = tag;
            return ribbonButton;
        }

       
        

        public void Initialize()
        {

        }

        public void Terminate()
        {

        }
    }

    public class ButtonCommandHandler : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private string _selectedSourceValue = "auto";
        private string _selectedTargetValue = "en";

        public bool CanExecute(object param)
        {
            return true;
        }

        public void SetSelectedValue(string sourceLang,string targeLang )
        {
            _selectedSourceValue = sourceLang;
            _selectedTargetValue = targeLang;
        }

        public void Execute(object parameter)
        {
            var settings = new TranslationSettings
            {
                SourceLanguage = _selectedSourceValue,
                TargetLanguage = _selectedTargetValue
            };
            var tr = new TranslateTextCommand();
            tr.TranslateText(settings);
        }
    }
}
