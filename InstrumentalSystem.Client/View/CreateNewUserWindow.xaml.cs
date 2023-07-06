using System;
using System.Windows;
using System.Windows.Controls;
using InstrumentalSystem.Client.Modals;

namespace InstrumentalSystem.Client.View
{
    public partial class CreateNewUserWindow : Window
    {
        public delegate void ProjectCreatedEventHandler();
        public event ProjectCreatedEventHandler UserAdded;
        
        private Database _database;

        public CreateNewUserWindow()
        {
            InitializeComponent();

            _database = Database.Instance;
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            string login = UserLogin.Text;
            string password = UserPassword.Password;

            string username = UserName.Text;
            string usersurname = UserSurname.Text;
            string userpatronymic = UserPatronymic.Text;

            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(usersurname) ||
                string.IsNullOrWhiteSpace(userpatronymic))
            {
                MessageBox.Show("Некоторые поля не заполнены",
                    "Registration Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            using (var transaction = _database.BeginTransaction())
            {
                try
                {
                    int accountId = _database.CreateAccount(login, password, transaction);

                    if (accountId == -1)
                    {
                        MessageBox.Show("Аккаунт с таким логином уже существует",
                            "Registration Error",
                            MessageBoxButton.OK,
                            MessageBoxImage.Warning);
                        return;
                    }

                    var account = _database.GetAccount(accountId, transaction);

                    string role = "специалист";

                    _database.CreateUser($"{username} {usersurname} {userpatronymic}", account, transaction, role);

                    transaction.Commit();

                    MessageBox.Show("Пользователь успешно создан",
                        "Success",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                    
                    UserAdded.Invoke();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();

                    MessageBox.Show("При создании пользователя произошла ошибка",
                        "Registration Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
            
            Close();
        }
    }
}