using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Autodesk.AutoCAD.ApplicationServices;
using acadApp = Autodesk.AutoCAD.ApplicationServices.Application;
using Autodesk.AutoCAD.Runtime;
using Autodesk.Windows;
using System.Windows.Media.Imaging;
using YMplugins.Views;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using YMplugins.Models.Acad2022.Commands.Translator;
using YMplugins.Services.Translator;

namespace YMplugins.Addin.Acad2022
{
    public class Ribbon : IExtensionApplication
    {
        private static string _selsourceComboValue = "auto";
        private static string _selTargeComboValue = "en";

        public void Initialize()
        {
            Autodesk.Windows.ComponentManager.ItemInitialized += ComponentManager_ItemInitialized;
        }

        public void Terminate()
        {

        }

        void ComponentManager_ItemInitialized(object sender, Autodesk.Windows.RibbonItemEventArgs e)
        {
            // Проверяем, что лента загружена
            if (Autodesk.Windows.ComponentManager.Ribbon != null)
            {
                // Строим нашу вкладку
                BuildRibbonTab();

                //и раз уж лента запустилась, то отключаем обработчик событий
                Autodesk.Windows.ComponentManager.ItemInitialized -=
                    new EventHandler<RibbonItemEventArgs>(ComponentManager_ItemInitialized);
            }
        }

        void BuildRibbonTab()
        {
            // Если лента еще не загружена
            if (!isLoaded())
            {
                // Строим вкладку
                CreateRibbonTab();
                // Подключаем обработчик событий изменения системных переменных
                acadApp.SystemVariableChanged += new SystemVariableChangedEventHandler(acadApp_SystemVariableChanged);
            }
        }

        bool isLoaded()
        {
            bool _loaded = false;
            RibbonControl ribCntrl = Autodesk.Windows.ComponentManager.Ribbon;
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
        void RemoveRibbonTab()
        {
            try
            {
                RibbonControl ribCntrl = Autodesk.Windows.ComponentManager.Ribbon;
                // Делаем итерацию по вкладкам ленты
                foreach (RibbonTab tab in ribCntrl.Tabs)
                {
                    if (tab.Id.Equals("CADBoost_ID") & tab.Title.Equals("CADBoost"))
                    {
                        // И если у вкладки совпадает идентификатор и заголовок, то удаляем эту вкладку
                        ribCntrl.Tabs.Remove(tab);
                        // Отключаем обработчик событий
                        acadApp.SystemVariableChanged -=
                            new SystemVariableChangedEventHandler(acadApp_SystemVariableChanged);
                        // Останавливаем итерацию
                        break;
                    }
                }
            }
            catch (Autodesk.AutoCAD.Runtime.Exception ex)
            {
                Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage(
                    ex.Message);
            }
        }

        /* Обработка события изменения системной переменной
         * Будем следить за системной переменной WSCURRENT (текущее рабочее пространство),
         * чтобы наша вкладка не "терялась" при изменение рабочего пространства
         */
        void acadApp_SystemVariableChanged(object sender, SystemVariableChangedEventArgs e)
        {
            if (e.Name.Equals("WSCURRENT")) BuildRibbonTab();
        }


        // Создание нашей вкладки
        void CreateRibbonTab()
        {
            try
            {
                
                RibbonControl ribCntrl = Autodesk.Windows.ComponentManager.Ribbon;
                
                RibbonTab ribTab = new RibbonTab();
                ribTab.Title = "CADBoost"; 
                ribTab.Id = "CADBoost_ID"; 
                ribCntrl.Tabs.Add(ribTab); 
                
                TranlsateButtons(ribTab);
                
                //ribTab.IsActive = true;
                
                ribCntrl.UpdateLayout();
            }
            catch (System.Exception ex)
            {
                Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage(
                    ex.Message);
            }
        }

        void TranlsateButtons(RibbonTab ribTab)
        {
            try
            {
                TestCommand.EnsureInitialized();

                RibbonPanelSource ribSourcePanel = new RibbonPanelSource();
                ribSourcePanel.Title = "Translator";
                RibbonPanel ribPanel = new RibbonPanel();
                ribPanel.Source = ribSourcePanel;
                ribTab.Panels.Add(ribPanel);

                RibbonToolTip tt;

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
                };

                RibbonButton ribBtn = new RibbonButton();

                #region Кнопка TrtoEn

                tt = new RibbonToolTip();
                tt.IsHelpEnabled = false;
                //ribBtn.CommandParameter = tt.Command = "Tra";
                ribBtn.Name = "Translate";
                ribBtn.Text = "Translate";
                ribBtn.CommandHandler = commandHandler;
                ribBtn.Orientation = System.Windows.Controls.Orientation.Horizontal;
                ribBtn.Size = RibbonItemSize.Large;
                ribBtn.LargeImage = LoadImage("translation");
                ribBtn.ShowImage = true;
                ribBtn.ShowText = true;
                tt.Content = "Translate";
                ribBtn.ToolTip = tt;
                //ribRowPanel.Items.Add(ribBtn);

                #endregion
                ribSourcePanel.Items.Add(sourceLangCombo);
                ribSourcePanel.Items.Add(targetLangCombo);
                ribSourcePanel.Items.Add(new RibbonSeparator());
                ribSourcePanel.Items.Add(ribBtn);

                RibbonPanelBreak panelBreak = new RibbonPanelBreak();
                ribSourcePanel.Items.Add(panelBreak);
            }
            catch (System.Exception ex)
            {
                Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage(ex.Message);
            }
        }

        
        private BitmapImage LoadImage(string ImageName)
        {
            try
            {
                var image = "pack://application:,,,/YMplugins.Addin.Acad2022;component/" + "Icons/" + ImageName + ".png";
                return new BitmapImage(new Uri(image));
            }
            catch (Autodesk.AutoCAD.Runtime.Exception ex)
            {
                // Логирование ошибки или отладочная информация
                Console.WriteLine($"Error loading image: {ex.Message}");
                return null;
            }
        }

        //TODO законить
        private RibbonCombo GetRibbonCombo(string comboName, string prefix)
        {
            RibbonToolTip tt = new RibbonToolTip();
            RibbonCombo ribbonCombo = new RibbonCombo();
            ribbonCombo.Id = comboName;
            ribbonCombo.Text = tt.Title = prefix;
            foreach (KeyValuePair<string, string> lang in TestCommand.LanguageModeMap)
            {
                var ribBtn = GetRibbonButton(prefix + lang.Value, lang.Key, lang.Value);
                ribbonCombo.Items.Add(ribBtn);
            }

            return ribbonCombo;
        }

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
        class RibbonCommandHandler : System.Windows.Input.ICommand
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

        public class RelayCommandHandler : System.Windows.Input.ICommand
        {
            private readonly Action _execute;

            public RelayCommandHandler(Action execute)
            {
                _execute = execute;
            }

            public bool CanExecute(object parameter) => true;

            public void Execute(object parameter)
            {
                _execute();
            }

            public event EventHandler CanExecuteChanged;
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
                var tr = new TranslateTextCommand();
                tr.TranslateText(settings);
            }
        }
    }
}
