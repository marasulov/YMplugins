using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace YMplugins.Views
{
    /// <summary>
    /// Логика взаимодействия для ComboBoxControl.xaml
    /// </summary>
    public partial class ComboBoxControl : UserControl
    {
        public ComboBoxControl()
        {
            InitializeComponent();
            this.Width = 150;
            this.Height = 30;
        }

        public string SelectedItem
        {
            get { return (MyComboBox.SelectedItem as ComboBoxItem)?.Content.ToString(); }
        }
    }
}
