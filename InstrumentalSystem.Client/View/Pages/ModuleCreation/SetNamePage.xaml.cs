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

namespace InstrumentalSystem.Client.View.Pages.ModuleCreation
{
    /// <summary>
    /// Логика взаимодействия для SetNamePage.xaml
    /// </summary>
    public partial class SetNamePage : Page
    {
        public SetNamePage()
        {
            InitializeComponent();
        }

        private void isEmpty_Checked(object sender, RoutedEventArgs e)
        {
            var isChecked = (isEmpty.IsChecked is null || isEmpty.IsChecked == false) ? false : true;
            //LevelLabel.IsEnabled = isChecked;
            //LevelTextBox.IsEnabled = isChecked;
        }
    }
}
