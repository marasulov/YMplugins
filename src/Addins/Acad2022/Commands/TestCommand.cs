using Autodesk.AutoCAD.Runtime;
using Autodesk.Windows;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace YMplugins.Addin.Acad2022.Commands
{
    public class TestCommand : IExtensionApplication
    {
        // эта функция будет вызываться при выполнении в AutoCAD команды "TestCommand"
        [CommandMethod("TestCommand")]
        public void MyCommand()
        {
            // создаем квадратик цвета морской волны (он будет старательно играть роль иконки)
            Bitmap bmp = new Bitmap(1, 1);
            bmp.SetPixel(0, 0, Color.Aquamarine);
            bmp = new Bitmap(bmp, 1024, 1024);
            IntPtr hBitmap = bmp.GetHbitmap();
            System.Windows.Media.Imaging.BitmapSource bs =
                System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                  hBitmap,
                  IntPtr.Zero,
                  System.Windows.Int32Rect.Empty,
                  System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());

            // создаем выпадающие списки
            Autodesk.Windows.RibbonCombo comboBox1 = new RibbonCombo();
            comboBox1.Id = "_combobox1";
            comboBox1.Width = 200;
            comboBox1.Text = "Source language111";
            comboBox1.ShowText = true;
            Autodesk.Windows.RibbonCombo comboBox2 = new RibbonCombo();
            comboBox2.Id = "_combobox2";
            comboBox2.Width = 200;
            comboBox2.Image = bs;
            comboBox2.ShowImage = true;

            // создаем кнопки
            Autodesk.Windows.RibbonButton button1 = new Autodesk.Windows.RibbonButton();
            button1.Id = "_button1";
            Autodesk.Windows.RibbonButton button2 = new Autodesk.Windows.RibbonButton();
            button2.Id = "_button2";

            // создаем вертикальные панели, на которых будут размещены друг под другом выпадающие списки и кнопки
            Autodesk.Windows.RibbonRowPanel RowPanel1 = new Autodesk.Windows.RibbonRowPanel();
            Autodesk.Windows.RibbonRowPanel RowPanel2 = new Autodesk.Windows.RibbonRowPanel();

            // размещаем в вертикальных панелях выпадающие списки и кнопки
            RowPanel1.Items.Add(comboBox1);
            RowPanel1.Items.Add(new RibbonRowBreak());
            RowPanel1.Items.Add(comboBox2);
            RowPanel2.Items.Add(button1);
            RowPanel2.Items.Add(new RibbonRowBreak());
            RowPanel2.Items.Add(button2);

            // создаем кнопки большого размера
            Autodesk.Windows.RibbonButton button3 = new Autodesk.Windows.RibbonButton();
            button3.Id = "_button3";
            button3.IsToolTipEnabled = true;
            button3.ToolTip = "Это большая кнопкаssss";
            button3.Text = "OK";
            button3.Orientation = Orientation.Vertical;
            button3.ShowText = true;
            button3.Size = Autodesk.Windows.RibbonItemSize.Large;
            button3.LargeImage = bs;
            Autodesk.Windows.RibbonButton button4 = new Autodesk.Windows.RibbonButton();
            button4.Id = "_button4";
            button4.Text = "^___^";
            button4.ShowText = true;
            button4.Size = Autodesk.Windows.RibbonItemSize.Large;
            button4.LargeImage = bs;

            // создаем контейнеры для элементов
            Autodesk.Windows.RibbonPanelSource rbPanelSource1 = new Autodesk.Windows.RibbonPanelSource();
            rbPanelSource1.Title = "Новая панель элементов";
            Autodesk.Windows.RibbonPanelSource rbPanelSource2 = new Autodesk.Windows.RibbonPanelSource();
            rbPanelSource2.Title = "Еще одна панель";

            // добавляем в контейнеры элементы управления
            rbPanelSource1.Items.Add(RowPanel1);
            rbPanelSource1.Items.Add(RowPanel2);
            rbPanelSource1.Items.Add(new RibbonSeparator());
            rbPanelSource1.Items.Add(button3);
            rbPanelSource2.Items.Add(button4);

            // создаем панели
            RibbonPanel rbPanel1 = new RibbonPanel();
            RibbonPanel rbPanel2 = new RibbonPanel();

            // добавляем на панели контейнеры для элементов
            rbPanel1.Source = rbPanelSource1;
            rbPanel2.Source = rbPanelSource2;

            // создаем вкладку
            RibbonTab rbTab = new RibbonTab();
            rbTab.Title = "Новая вкладка";
            rbTab.Id = "HabrRibbon";

            // добавляем на вкладку панели
            rbTab.Panels.Add(rbPanel1);
            rbTab.Panels.Add(rbPanel2);

            // получаем указатель на ленту AutoCAD
            Autodesk.Windows.RibbonControl rbCtrl = ComponentManager.Ribbon;

            // добавляем на ленту вкладку
            rbCtrl.Tabs.Add(rbTab);

            // делаем созданную вкладку активной ("выбранной")
            rbTab.IsActive = true;
        }

        // Функции Initialize() и Terminate() необходимы, чтобы реализовать интерфейс IExtensionApplication
        public void Initialize()
        {

        }

        public void Terminate()
        {

        }
    }
}
