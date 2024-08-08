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

namespace YMplugins.Addin.Acad2022
{
    public class Ribbon : IExtensionApplication
    {
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
                // Получаем доступ к ленте
                RibbonControl ribCntrl = Autodesk.Windows.ComponentManager.Ribbon;
                // добавляем свою вкладку
                RibbonTab ribTab = new RibbonTab();
                ribTab.Title = "CADBoost"; // Заголовок вкладки
                ribTab.Id = "CADBoost_ID"; // Идентификатор вкладки
                ribCntrl.Tabs.Add(ribTab); // Добавляем вкладку в ленту
                // добавляем содержимое в свою вкладку (одну панель)
                addExampleContent(ribTab);
                // Делаем вкладку активной (не желательно, ибо неудобно)
                //ribTab.IsActive = true;
                // Обновляем ленту (если делаете вкладку активной, то необязательно)
                ribCntrl.UpdateLayout();
            }
            catch (System.Exception ex)
            {
                Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage(
                    ex.Message);
            }
        }

        // Строим новую панель в нашей вкладке
        void addExampleContent(RibbonTab ribTab)
        {
            try
            {
                RibbonPanelSource ribSourcePanel = new RibbonPanelSource();
                ribSourcePanel.Title = "Translator";
                RibbonPanel ribPanel = new RibbonPanel();
                ribPanel.Source = ribSourcePanel;
                ribTab.Panels.Add(ribPanel);

                RibbonToolTip tt;

                RibbonRowPanel ribRowPanel = new RibbonRowPanel();
                RibbonButton ribBtn = new RibbonButton();

                #region Кнопка TrtoEn

                tt = new RibbonToolTip();
                tt.IsHelpEnabled = false;
                ribBtn.CommandParameter = tt.Command = "TrToEnWithOrg";
                ribBtn.Name = "Translate to En";
                ribBtn.Text = tt.Title = "Translate to En";
                ribBtn.CommandHandler = new RibbonCommandHandler();
                ribBtn.Orientation = System.Windows.Controls.Orientation.Vertical;
                ribBtn.Size = RibbonItemSize.Large;
                ribBtn.LargeImage = LoadImage("rutoen");
                ribBtn.ShowImage = true;
                ribBtn.ShowText = true;
                tt.Content = "Translate to English";
                ribBtn.ToolTip = tt;
                ribRowPanel.Items.Add(ribBtn);

                #endregion

                #region Кнопка TrToRuWithOrg

                tt = new RibbonToolTip();
                tt.IsHelpEnabled = false;
                ribBtn = new RibbonButton();
                ribBtn.CommandParameter = tt.Command = "TrToRuWithOrg";
                ribBtn.Name = "Translate to Ru";
                ribBtn.Text = tt.Title = "Translate to Ru";
                ribBtn.CommandHandler = new RibbonCommandHandler();
                ribBtn.Orientation = System.Windows.Controls.Orientation.Vertical;
                ribBtn.Size = RibbonItemSize.Large;
                ribBtn.LargeImage = LoadImage("entoru");
                ribBtn.ShowImage = true;
                ribBtn.ShowText = true;
                tt.Content = "Translate to russian";
                ribBtn.ToolTip = tt;
                ribRowPanel.Items.Add(ribBtn);

                #endregion

                // Добавляем строку в нашу панель
                ribSourcePanel.Items.Add(ribRowPanel);

                //RibbonRowPanel rowPanel = new RibbonRowPanel();
                //rowPanel.AreItemsArrangedFromRightToLeft = true;

                //RibbonButton button3 = new RibbonButton
                //{
                //    Text = "TrToEn",
                //    ShowText = true,
                //    Orientation = Orientation.Vertical,
                //    Image = LoadImage("en"),
                //    CommandHandler = new RelayCommandHandler(() =>
                //    {
                //        Application.DocumentManager.MdiActiveDocument.SendStringToExecute("TrToEn ", true, false, false);
                //    })
                //};

                //RibbonButton button4 = new RibbonButton
                //{
                //    Text = "TrToUz",
                //    ShowText = true,
                //    Orientation = Orientation.Vertical,
                //    Image = LoadImage("uz"),
                //    CommandParameter = "",
                //    ToolTip = "TrToUzWithOrg",

                //    CommandHandler = new RelayCommandHandler(() =>
                //    {
                //        Application.DocumentManager.MdiActiveDocument.SendStringToExecute("TrToUzWithOrg", true, false, false);
                //    })
                //};

                //// Добавление маленьких кнопок в RibbonRow
                //rowPanel.Items.Add(button3);
                //rowPanel.Items.Add(button4);

                //// Добавление кнопок на панель

                //ribSourcePanel.Items.Add(rowPanel);

                // Добавление разделителя панели
                RibbonPanelBreak panelBreak = new RibbonPanelBreak();
                ribSourcePanel.Items.Add(panelBreak);

                //RibbonPanel Panel = new RibbonPanel();

                //Panel.Source = ribSourcePanel;

                //ribTab.Panels.Add(Panel);


            }
            catch (System.Exception ex)
            {
                Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage(
                    ex.Message);
            }
        }

        // Получение картинки из ресурсов
        // Данная функция найдена на просторах интернет
        System.Windows.Media.Imaging.BitmapImage LoadImage(string ImageName)
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
    }
}
