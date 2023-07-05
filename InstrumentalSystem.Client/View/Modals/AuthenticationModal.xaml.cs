using InstrumentalSystem.Client.View.Pages.Authenticate;
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
using InstrumentalSystem.Client.Modals;

namespace InstrumentalSystem.Client.View.Modals
{
    /// <summary>
    /// Логика взаимодействия для AuthenticationModal.xaml
    /// </summary>
    public partial class AuthenticationModal : UserControl
    {
        private Authorization _parent;
        //Добавил поле, в котором хранится ссылка на БД, чтобы каждый раз не обращаться к классу БД
        private Database _database;

        public AuthenticationModal(Authorization parent)
        {
            InitializeComponent();
            _database = Database.Instance;
            _parent = parent;
            AuthenticationFrame.Content = new AuthenticationPage(this);
        }
        
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Collapsed;
        }
        
        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (AuthenticationFrame.Content is RegistrationPage)
            {
                Register();
                return;
            }

            Authorize();
        }
        
        private void Authorize()
        {
            string login = ((AuthenticationPage)AuthenticationFrame.Content).Username;
            string password = ((AuthenticationPage)AuthenticationFrame.Content).Password;
            
            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Поля не могут быть пустыми",
                    "Authorization Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
            else if (!_database.UserExists(login))
            {
                MessageBox.Show("вы не зарегистрированы в системе",
                    "Authorization Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
            else if (!_database.VerifyUser(login, password))
            {
                MessageBox.Show("неверный логин или пароль",
                    "Authorization Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
            else
            {
                MessageBox.Show("вы успешно вошли в систему",
                    "Success",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                //Добавил связь страницы Hub с авторизованным пользователем
                CurrentWindowEditor.CurrentWindow = new Hub(_database.GetUserIdByLogin(login));
            }
        }
        
        private void Register()
        {
            string login = ((RegistrationPage)AuthenticationFrame.Content).UserLogin;
            string password = ((RegistrationPage)AuthenticationFrame.Content).UserPassword;

            string username = ((RegistrationPage)AuthenticationFrame.Content).UserName;
            string usersurname = ((RegistrationPage)AuthenticationFrame.Content).UserSurname;
            string userpatronymic = ((RegistrationPage)AuthenticationFrame.Content).UserPatronymic;
            
            //Добавил переменную для орагниазции, чтобы потом создать пользователя в БД со значением отсюда
            string organization = ((RegistrationPage)AuthenticationFrame.Content).UserOrganization;
            
            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(username)
                || string.IsNullOrWhiteSpace(usersurname) || string.IsNullOrWhiteSpace(userpatronymic))
            {
                MessageBox.Show("Некоторые поля не заполнены",
                    "Registration Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }
            
            if (!IsValidEmail(login))
            {
                MessageBox.Show("Неверный формат поля логина, проверьте и повторите еще раз",
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
                    
                    _database.CreateUser($"{username} {usersurname} {userpatronymic}", account, organization, transaction);
                    
                    transaction.Commit();

                    MessageBox.Show("Вы успешно зарегистрированы в системе",
                        "Success",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                    
                    AuthenticationFrame.Content = new AuthenticationPage(this);
                }
                catch (Exception e)
                {
                    transaction.Rollback();

                    MessageBox.Show("При регистрации произошла ошибка",
                        "Registration Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
        }
        
        public static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            AuthenticationFrame.Content = new AuthenticationPage(this);
        }
    }
}