using Library.General.Project;
using Library.General.User;
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

namespace InstrumentalSystem.Client.View.Modals
{
    /// <summary>
    /// Логика взаимодействия для GlobalProjectInfoModal.xaml
    /// </summary>
    public partial class GlobalProjectInfoModal : UserControl
    {
        private List<LogicModule> _modules = new List<LogicModule>();
        private List<User> _users = new List<User>();

        public GlobalProjectInfoModal()
        {
            InitializeComponent();
            _users.Add(new User(
                "wrap2",
                "\\View\\Images\\roles\\roleOwnerIcon.png"
                ));            
            _users.Add(new User(
                "rabbid",
                "\\View\\Images\\roles\\roleViewerIcon.png"
                ));            
            _users.Add(new User(
                "wrap",
                "\\View\\Images\\roles\\roleEditorIcon.png"
                ));
            _modules.Add(new LogicModule("ТестНаЧтение|3"));
            _modules.Add(new LogicModule("ТестНаЧтение|2"));
            _modules.Add(new LogicModule("ТестНаРедактирование|2"));
            ModuleList.ItemsSource = _modules;
            UserList.ItemsSource = _users;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Collapsed;
        }
    }
}
