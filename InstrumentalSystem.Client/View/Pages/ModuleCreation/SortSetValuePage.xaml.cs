using Library.General.NameTable;
using System.Windows.Controls;

namespace InstrumentalSystem.Client.View.Pages.ModuleCreation
{
    /// <summary>
    /// Логика взаимодействия для SortValuePage.xaml
    /// </summary>
    public partial class SortSetValuePage : Page
    {
        public BaseNameElement _name;
        public SortSetValuePage(BaseNameElement name)
        {
            InitializeComponent();
            _name = name;
            Task.Content = $"{_name.Prefix.ToString()} Sort {_name.ID} : {_name.Value.ToString()}";
        }
    }
}
