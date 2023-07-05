using InstrumentalSystem.Client.View.Modals;
using System.Windows.Controls;

namespace InstrumentalSystem.Client.View.Pages.Authenticate
{
    /// <summary>
    /// Логика взаимодействия для RegistrationPage.xaml
    /// </summary>
    public partial class RegistrationPage : Page
    {
        private AuthenticationModal _parent;
        
        public string UserName
        {
            get { return usernameTextBox.Text; }
        }

        public string UserSurname
        {
            get { return usersurnameTextBox.Text; }
        }

        public string UserPatronymic
        {
            get { return userpatronymicTextBox.Text; }
        }

        public string UserLogin
        {
            get { return userloginTextBox.Text; }
        }

        public string UserPassword
        {
            get { return userpasswordTextBox.Text; }
        }

        //Добавил свойство для организации
        public string UserOrganization
        {
            get { return namesTextBox.Text; }
        }

        public RegistrationPage(AuthenticationModal parent)
        {
            InitializeComponent();
            _parent = parent;
            _parent.NextButton.Content = "Создать";
            _parent.HeaderLabel.Content = "Создание учетной записи";
            _parent.BackButton.IsEnabled = true;
        }
    }
}