using Library.General.NameTable;
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
    /// Логика взаимодействия для SortNamePage.xaml
    /// </summary>
    public partial class SortNamePage : Page
    {
        public BaseNameElement _name;

        public SortNamePage(BaseNameElement name)
        {
            InitializeComponent();
            _name = name;
            Task.Content = $"{_name.Prefix.ToString()} {_name.ID} {_name.Value.ToString()}";
        }


    }
}
