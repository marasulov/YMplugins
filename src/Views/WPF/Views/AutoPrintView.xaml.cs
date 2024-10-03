using System.Windows;
using YMplugins.ViewModels.VM;
namespace YMplugins.Views.Views
{
    /// <summary>
    /// Логика взаимодействия для AutoPrintView.xaml
    /// </summary>
    public partial class AutoPrintView : Window
    {
        public AutoPrintView(AutoPrintVm autoPrintVm)
        {
            InitializeComponent();
            DataContext = autoPrintVm;

            autoPrintVm.CloseAction ??= Close;

            autoPrintVm.OpenAction = new Action(() =>
            {
                this.ShowDialog();
                this.Activate();
                Console.WriteLine("Окно открыто.");
            });
            Loaded += On_Loaded;
        }

        private void On_Loaded(object sender, RoutedEventArgs e)
        {
            Style = (Style)FindResource("WindowElementStyle");
        }

    }
}
