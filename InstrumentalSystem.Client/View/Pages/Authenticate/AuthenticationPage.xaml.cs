using InstrumentalSystem.Client.View.Modals;
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

namespace InstrumentalSystem.Client.View.Pages.Authenticate
{
    /// <summary>
    /// Логика взаимодействия для AuthenticationPage.xaml
    /// </summary>
    public partial class AuthenticationPage : Page
    {
        private AuthenticationModal _parent;
        public AuthenticationPage(AuthenticationModal parent)
        {
            InitializeComponent();
            _parent = parent;
            _parent.NextButton.Content = "Войти";
            _parent.HeaderLabel.Content = "Вход в учетную запись";
            _parent.BackButton.IsEnabled = false;
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            _parent.AuthenticationFrame.Content = new RegistrationPage(_parent);
        }        
    }
}
