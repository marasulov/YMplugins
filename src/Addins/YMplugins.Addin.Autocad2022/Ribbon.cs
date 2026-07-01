using YMplugins.Addin.Autocad2022.Commands.Translator;
using YMplugins.Services.Translator;

namespace YMplugins.Addin.Autocad2022
{
    using System.IO;
    using Autodesk.AutoCAD.ApplicationServices;
    using Autodesk.AutoCAD.Runtime;
    using Autodesk.Windows;

#if NET8_0_OR_GREATER
    using Gile.AutoCAD.R25.Extension;
#else
    using Gile.AutoCAD.R20.Extension;
#endif
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media.Imaging;
    using acadApp = Autodesk.AutoCAD.ApplicationServices.Application;
    using Exception = Autodesk.AutoCAD.Runtime.Exception;

    namespace YMplugins.Addin.Acad2022
    {
        public class Ribbon : IExtensionApplication
        {
            private static string _selsourceComboValue = "auto";
            private static string _selTargeComboValue = "en";
            private static Dictionary<string, string> _languageModeMap;
            private bool _isNewObject = false;

            public void Initialize()
            {
                ComponentManager.ItemInitialized += ComponentManager_ItemInitialized;
                var executablePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                var pd = new ProxyDomain();
                var assembly = pd.GetAssembly(Path.Combine(executablePath, "MaterialDesignThemes.Wpf.dll"));

                var assembly1 = pd.GetAssembly(Path.Combine(executablePath, "MaterialDesignColors.dll"));

                if ((assembly != null) | (assembly1 != null)) Active.Editor.WriteMessage("style dlls not load");

                //var standartCopier = new StandartCopier();
                //var isConfFileCopied = standartCopier.CopyParamsFiles();

                //if (!isConfFileCopied) Active.Editor.WriteMessage("файлы не скопированы");
            }

            internal class ProxyDomain : MarshalByRefObject
            {
                public Assembly GetAssembly(string assemblyPath)
                {
                    try
                    {
                        return Assembly.LoadFrom(assemblyPath);
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidOperationException(ex.Message);
                    }
                }
            }

            public void Terminate()
            {
            }

            private void ComponentManager_ItemInitialized(object sender, RibbonItemEventArgs e)
            {
                // Проверяем, что лента загружена
                if (ComponentManager.Ribbon != null)
                {
                    // Строим нашу вкладку
                    BuildRibbonTab();

                    //и раз уж лента запустилась, то отключаем обработчик событий
                    ComponentManager.ItemInitialized -=
                        ComponentManager_ItemInitialized;
                }
            }

            private void BuildRibbonTab()
            {
                // Если лента еще не загружена
                if (!isLoaded())
                {
                    // Строим вкладку
                    CreateRibbonTab();
                    // Подключаем обработчик событий изменения системных переменных
                    acadApp.SystemVariableChanged += acadApp_SystemVariableChanged;
                }
            }

            private bool isLoaded()
            {
                bool _loaded = false;
                RibbonControl ribCntrl = ComponentManager.Ribbon;
                // Делаем итерацию по вкладкам ленты
                foreach (RibbonTab tab in ribCntrl.Tabs)
                {
                    // И если у вкладки совпадает идентификатор и заголовок, то значит вкладка загружена
                    if (tab.Id.Equals("CADBoost_ID") & tab.Title.Equals("CADBoost"))
                    {
                        _loaded = true;
                        break;
                    }
                }

                return _loaded;
            }

            /* Удаление своей вкладки с ленты
             * В данном примере не используем
             */

            private void RemoveRibbonTab()
            {
                try
                {
                    RibbonControl ribCntrl = ComponentManager.Ribbon;
                    // Делаем итерацию по вкладкам ленты
                    foreach (RibbonTab tab in ribCntrl.Tabs)
                    {
                        if (tab.Id.Equals("CADBoost_ID") & tab.Title.Equals("CADBoost"))
                        {
                            // И если у вкладки совпадает идентификатор и заголовок, то удаляем эту вкладку
                            ribCntrl.Tabs.Remove(tab);
                            // Отключаем обработчик событий
                            acadApp.SystemVariableChanged -=
                                acadApp_SystemVariableChanged;
                            // Останавливаем итерацию
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage(
                        ex.Message);
                }
            }

            /* Обработка события изменения системной переменной
             * Будем следить за системной переменной WSCURRENT (текущее рабочее пространство),
             * чтобы наша вкладка не "терялась" при изменение рабочего пространства
             */

            private void acadApp_SystemVariableChanged(object sender, SystemVariableChangedEventArgs e)
            {
                if (e.Name.Equals("WSCURRENT")) BuildRibbonTab();
            }

            // Создание нашей вкладки
            private void CreateRibbonTab()
            {
                try
                {
                    RibbonControl ribCntrl = ComponentManager.Ribbon;

                    RibbonTab ribTab = new RibbonTab();
                    ribTab.Title = "CADBoost";
                    ribTab.Id = "CADBoost_ID";
                    ribCntrl.Tabs.Add(ribTab);

                    TranslateButtons(ribTab);
                    AutoPrintButtons(ribTab);
                    //ribTab.IsActive = true;

                    ribCntrl.UpdateLayout();
                }
                catch (System.Exception ex)
                {
                    Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage(
                        ex.Message);
                }
            }

            private void TranslateButtons(RibbonTab ribTab)
            {
                try
                {
                    EnsureInitialized();

                    RibbonPanelSource ribSourcePanel = new RibbonPanelSource();
                    ribSourcePanel.Title = "Translator";

                    RibbonPanel ribPanel = new RibbonPanel();
                    ribPanel.Source = ribSourcePanel;
                    ribTab.Panels.Add(ribPanel);

                    RibbonToolTip tt = new RibbonToolTip();
                    RibbonCombo sourceCombo = new RibbonCombo();
                    sourceCombo.Id = "sourceLangCombo";
                    sourceCombo.Text = tt.Title = "Source language";
                    sourceCombo.ShowText = true;
                    var firstButton = GetRibbonButton("autoDetect", "Detect language", "auto");
                    sourceCombo.Items.Add(firstButton);

                    foreach (KeyValuePair<string, string> lang in _languageModeMap)
                    {
                        var comboBtn = GetRibbonButton("source" + lang.Value, lang.Key, lang.Value);
                        comboBtn.Orientation = Orientation.Vertical;
                        sourceCombo.Items.Add(comboBtn);
                    }

                    RibbonCombo targetCombo = new RibbonCombo();
                    targetCombo.Id = "sourceLangCombo";
                    targetCombo.Text = tt.Title = "Target language";
                    targetCombo.ShowText = true;

                    foreach (KeyValuePair<string, string> lang in _languageModeMap)
                    {
                        var comboBtn = GetRibbonButton("target" + lang.Value, lang.Key, lang.Value);
                        comboBtn.Orientation = Orientation.Vertical;
                        targetCombo.Items.Add(comboBtn);
                    }
                    //RibbonCheckBox newObjectCheckbox = new RibbonCheckBox();
                    //newObjectCheckbox.Text = "Translated text in new object";
                    //newObjectCheckbox.IsChecked = false;

                    RibbonLabel label = new RibbonLabel();
                    label.Text = "Translated text \nin new object";

                    RibbonRowPanel rowPanel = new RibbonRowPanel();
                    rowPanel.Items.Add(sourceCombo);

                    //rowPanel.Items.Add(label);
                    rowPanel.Items.Add(new RibbonRowBreak());
                    rowPanel.Items.Add(targetCombo);
                    rowPanel.Items.Add(new RibbonRowBreak());
                    //rowPanel.Items.Add(newObjectCheckbox);

                    var commandHandler = new ButtonCommandHandler();

                    sourceCombo.CurrentChanged += (sender, e) =>
                    {
                        var selectedItem = e.NewValue as RibbonButton;
                        _selsourceComboValue = selectedItem.Tag.ToString();
                        TranslationState.Instance.SelectedSourceLanguage = _selsourceComboValue;
                        commandHandler.SetSelectedValue(_selsourceComboValue, _selTargeComboValue);
                    };

                    targetCombo.CurrentChanged += (sender, e) =>
                    {
                        var selectedItem = e.NewValue as RibbonButton;
                        _selTargeComboValue = selectedItem.Tag.ToString();
                        TranslationState.Instance.SelectedTargetLanguage = _selTargeComboValue;
                        commandHandler.SetSelectedValue(_selsourceComboValue, _selTargeComboValue);
                    };

                    //newObjectCheckbox.PropertyChanged += (sender, e) =>
                    //{
                    //    commandHandler.SetNewObjectCheckBoxValue(newObjectCheckbox.IsChecked);
                    //};

                    tt = new RibbonToolTip();
                    tt.IsHelpEnabled = false;

                    RibbonButton ribBtn = new RibbonButton();
                    ribBtn.Id = "translateBtn";
                    ribBtn.Name = "Translate";
                    ribBtn.Text = "Translate";
                    ribBtn.CommandHandler = commandHandler;
                    ribBtn.CommandParameter = "YmTranslate";
                    ribBtn.Size = RibbonItemSize.Large;
                    ribBtn.LargeImage = LoadImage("translation");
                    ribBtn.ShowImage = true;
                    ribBtn.ShowText = true;
                    tt.Content = "Translate";
                    ribBtn.ToolTip = tt;
                    ribBtn.Orientation = Orientation.Vertical;

                    ribSourcePanel.Items.Add(rowPanel);
                    ribSourcePanel.Items.Add(new RibbonSeparator());
                    ribSourcePanel.Items.Add(ribBtn);

                    //RibbonPanelBreak panelBreak = new RibbonPanelBreak();
                    //ribSourcePanel.Items.Add(panelBreak);
                }
                catch (System.Exception ex)
                {
                    Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage(ex.Message);
                }
            }

            private void AutoPrintButtons(RibbonTab ribbonTab)
            {
                RibbonPanelSource ribSourcePanel = new RibbonPanelSource();
                ribSourcePanel.Title = "AutoPrint";

                RibbonPanel ribPanel = new RibbonPanel();
                ribPanel.Source = ribSourcePanel;
                ribbonTab.Panels.Add(ribPanel);

                var commandHandler = new RibbonCommandHandler();

                RibbonToolTip tt = new RibbonToolTip();
                RibbonButton ribBtn = new RibbonButton();
                ribBtn.Id = "translateBtn";
                ribBtn.Name = "AutoPrint";
                ribBtn.Text = "AutoPrint";
                ribBtn.CommandHandler = commandHandler;
                ribBtn.CommandParameter = "AutoPrint";
                ribBtn.Size = RibbonItemSize.Large;
                ribBtn.LargeImage = LoadImage("autoprint");
                ribBtn.ShowImage = true;
                ribBtn.ShowText = true;
                tt.Content = "autoprint";
                ribBtn.ToolTip = tt;
                ribBtn.Orientation = Orientation.Vertical;

                ribSourcePanel.Items.Add(ribBtn);
            }

            private BitmapImage LoadImage(string ImageName)
            {
                try
                {
                    var image = "pack://application:,,,/YMplugins.Addin.Autocad2022;component/" + "Icons/" + ImageName + ".png";
                    return new BitmapImage(new Uri(image));
                }
                catch (Exception ex)
                {
                    // Логирование ошибки или отладочная информация
                    Active.Editor.WriteMessage($"Error loading image: {ex.Message}");
                    return null;
                }
            }

            //private RibbonCombo GetRibbonCombo(string comboName, string prefix)
            //{
            //    RibbonToolTip tt = new RibbonToolTip();
            //    RibbonCombo ribbonCombo = new RibbonCombo();
            //    ribbonCombo.ObjectId = comboName;
            //    ribbonCombo.Text = tt.Title = prefix;
            //    ribbonCombo.ShowText = true;

            //    foreach (KeyValuePair<string, string> lang in _languageModeMap)
            //    {
            //        var ribBtn = GetRibbonButton(prefix + lang.Value, lang.Key, lang.Value);
            //        ribbonCombo.Items.Add(ribBtn);
            //    }

            //    return ribbonCombo;
            //}

            private RibbonButton GetRibbonButton(string id, string text, string tag)
            {
                var ribbonButton = new RibbonButton();
                ribbonButton.Id = id;
                ribbonButton.Text = text;

                ribbonButton.ShowText = true;
                ribbonButton.Tag = tag;
                return ribbonButton;
            }

            /* Собственный обраотчик команд
            * Это один из вариантов вызова команды по нажатию кнопки
            */

            private class RibbonCommandHandler : ICommand
            {
                public bool CanExecute(object parameter)
                {
                    return true;
                }

                public event EventHandler CanExecuteChanged;

                public void Execute(object parameter)
                {
                    if (parameter is RibbonButton)
                    {
                        // Просто берем команду, записанную в CommandParameter кнопки
                        // и выпоняем её используя функцию SendStringToExecute
                        RibbonButton button = parameter as RibbonButton;
                        acadApp.DocumentManager.MdiActiveDocument.SendStringToExecute(
                            button.CommandParameter + " ", true, false, true);
                    }
                }
            }

            //public class RelayCommandHandler : ICommand
            //{
            //    private readonly Action _execute;

            //    public RelayCommandHandler(Action execute)
            //    {
            //        _execute = execute;
            //    }

            //    public bool CanExecute(object parameter) => true;

            //    public void Execute(object parameter)
            //    {
            //        _execute();
            //    }

            //    public event EventHandler CanExecuteChanged;
            //}

            public class ButtonCommandHandler : ICommand
            {
                public event EventHandler CanExecuteChanged;

                private string _selectedSourceValue = "auto";
                private string _selectedTargetValue = "en";
                private bool? _selectedEnabled = false;

                public bool CanExecute(object param)
                {
                    return true;
                }

                public void SetSelectedValue(string sourceLang, string targeLang)
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
                    //var tr = new TranslateTextCommand();
                    //tr.TranslateText(settings);

                    if (parameter is RibbonButton)
                    {
                        // Просто берем команду, записанную в CommandParameter кнопки
                        // и выпоняем её используя функцию SendStringToExecute
                        RibbonButton button = parameter as RibbonButton;
                        acadApp.DocumentManager.MdiActiveDocument.SendStringToExecute(
                            button.CommandParameter + " ", true, false, true);
                    }
                }
            }

            private static void EnsureInitialized()
            {
                if (_languageModeMap != null)
                    return;
                _languageModeMap = new Dictionary<string, string>();
                _languageModeMap.Add("English", "en");
                _languageModeMap.Add("Russian", "ru");
                _languageModeMap.Add("Uzbek", "uz");
                _languageModeMap.Add("Afrikaans", "af");
                _languageModeMap.Add("Albanian", "sq");
                _languageModeMap.Add("Arabic", "ar");
                _languageModeMap.Add("Armenian", "hy");
                _languageModeMap.Add("Azerbaijani", "az");
                _languageModeMap.Add("Basque", "eu");
                _languageModeMap.Add("Belarusian", "be");
                _languageModeMap.Add("Bengali", "bn");
                _languageModeMap.Add("Bulgarian", "bg");
                _languageModeMap.Add("Catalan", "ca");
                _languageModeMap.Add("Chinese", "zh-CN");
                _languageModeMap.Add("Croatian", "hr");
                _languageModeMap.Add("Czech", "cs");
                _languageModeMap.Add("Danish", "da");
                _languageModeMap.Add("Dutch", "nl");
                _languageModeMap.Add("Esperanto", "eo");
                _languageModeMap.Add("Estonian", "et");
                _languageModeMap.Add("Filipino", "tl");
                _languageModeMap.Add("Finnish", "fi");
                _languageModeMap.Add("French", "fr");
                _languageModeMap.Add("Galician", "gl");
                _languageModeMap.Add("German", "de");
                _languageModeMap.Add("Georgian", "ka");
                _languageModeMap.Add("Greek", "el");
                _languageModeMap.Add("Haitian Creole", "ht");
                _languageModeMap.Add("Hebrew", "iw");
                _languageModeMap.Add("Hindi", "hi");
                _languageModeMap.Add("Hungarian", "hu");
                _languageModeMap.Add("Icelandic", "is");
                _languageModeMap.Add("Indonesian", "id");
                _languageModeMap.Add("Irish", "ga");
                _languageModeMap.Add("Italian", "it");
                _languageModeMap.Add("Japanese", "ja");
                _languageModeMap.Add("Korean", "ko");
                _languageModeMap.Add("Lao", "lo");
                _languageModeMap.Add("Latin", "la");
                _languageModeMap.Add("Latvian", "lv");
                _languageModeMap.Add("Lithuanian", "lt");
                _languageModeMap.Add("Macedonian", "mk");
                _languageModeMap.Add("Malay", "ms");
                _languageModeMap.Add("Maltese", "mt");
                _languageModeMap.Add("Norwegian", "no");
                _languageModeMap.Add("Persian", "fa");
                _languageModeMap.Add("Polish", "pl");
                _languageModeMap.Add("Portuguese", "pt");
                _languageModeMap.Add("Romanian", "ro");
                _languageModeMap.Add("Serbian", "sr");
                _languageModeMap.Add("Slovak", "sk");
                _languageModeMap.Add("Slovenian", "sl");
                _languageModeMap.Add("Spanish", "es");
                _languageModeMap.Add("Swahili", "sw");
                _languageModeMap.Add("Swedish", "sv");
                _languageModeMap.Add("Tamil", "ta");
                _languageModeMap.Add("Telugu", "te");
                _languageModeMap.Add("Thai", "th");
                _languageModeMap.Add("Turkish", "tr");
                _languageModeMap.Add("Ukrainian", "uk");
                _languageModeMap.Add("Urdu", "ur");
                _languageModeMap.Add("Vietnamese", "vi");
                _languageModeMap.Add("Welsh", "cy");
                _languageModeMap.Add("Yiddish", "yi");
            }
        }
    }
}