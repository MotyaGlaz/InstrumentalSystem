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
        //Поменял ссылку с окна Hub на окно Authorization
        private Authorization _parent;

        public AuthenticationModal(Authorization parent)
        {
            InitializeComponent();
            _parent = parent;
            AuthenticationFrame.Content = new AuthenticationPage(this);
        }


        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Collapsed;
        }

        //Добавил методы Register и Authorize, в случае, если отображается страница авторизации, то метод
        //вызовется по кнопке "Войти" и пойдёт в метод Authorize
        //Если отображается страница регистрации, то метод вызовется по кнопке "Создать" и пойдёт в Register
        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (AuthenticationFrame.Content is RegistrationPage)
            {
                Register();
                return;
            }

            Authorize();
        }

        //Метод для авторизации пользователя
        private void Authorize()
        {
            //Забираем значения с полей логина и пароля
            string login = ((AuthenticationPage)AuthenticationFrame.Content).Username;
            string password = ((AuthenticationPage)AuthenticationFrame.Content).Password;

            //Сначала проверяем, чтобы они не были пустыми
            //Затем проверяем, существует ли пользователь в системе под таким логином
            //Далее верифицируем логин и пароль, по сути, проверяем правильный ли пароль мы ввели
            //Затем если все проверки успешны, то мы авторизуемся в системе и переходим на окно Hub
            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Поля не могут быть пустыми",
                    "Authorization Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
            else if (!Database.Instance.UserExists(login))
            {
                MessageBox.Show("вы не зарегистрированы в системе",
                    "Authorization Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
            else if (!Database.Instance.VerifyUser(login, password))
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

                /*_parent.AvatarLabel.Background = new SolidColorBrush(Colors.LightCoral);
                _parent.AvatarLabel.Content = "ВС";
                _parent.GlobalProjectsTab.IsEnabled = true;
                this.Visibility = Visibility.Collapsed;*/

                WindowManager.CurrentWindow = new Hub();
            }
        }

        //Метод для регистрации нового пользователя
        private void Register()
        {
            //Считываем значения с полей
            string login = ((RegistrationPage)AuthenticationFrame.Content).UserLogin;
            string password = ((RegistrationPage)AuthenticationFrame.Content).UserPassword;

            string username = ((RegistrationPage)AuthenticationFrame.Content).UserName;
            string usersurname = ((RegistrationPage)AuthenticationFrame.Content).UserSurname;
            string userpatronymic = ((RegistrationPage)AuthenticationFrame.Content).UserPatronymic;

            //Проверяем, если хоть какое-либо поле пустое, то пишем, что нужно заполнить поля
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

            //Проверяем логин на правильный формат
            if (!IsValidEmail(login))
            {
                MessageBox.Show("Неверный формат поля логина, проверьте и повторите еще раз",
                    "Registration Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            //using и transaction здесь нужны для подключения к БД
            //transaction нужна для того, чтобы в случае, если например при создании пользователя что-то пошло не так
            //То уже созданное значение в таблице account автоматически удалялось, и у нас в БД не было бы лишних полей
            using (var transaction = Database.Instance.BeginTransaction())
            {
                try
                {
                    //Сначала создаём аккаунт для пользователя (записываем в БД логин и пароль), возвращем id записи из БД
                    int accountId = Database.Instance.CreateAccount(login, password, transaction);

                    //-1 возвращается в случае, если аккаунт с таким логином уже существует в системе, тут мы это проверяем
                    if (accountId == -1)
                    {
                        MessageBox.Show("Аккаунт с таким логином уже существует",
                            "Registration Error",
                            MessageBoxButton.OK,
                            MessageBoxImage.Warning);
                        return;
                    }

                    //Получаем объект аккаунта для последующего использования в создании сущности пользователя в БД
                    var account = Database.Instance.GetAccount(accountId, transaction);

                    //Записываем в БД пользователя
                    Database.Instance.CreateUser($"{username} {usersurname} {userpatronymic}", account, transaction);

                    //Только в случае всех успешных шагов до мы заливаем изменения в БД целиком с account и с user
                    transaction.Commit();

                    MessageBox.Show("Вы успешно зарегистрированы в системе",
                        "Success",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);

                    //Открываем окно авторизации
                    AuthenticationFrame.Content = new AuthenticationPage(this);
                }
                catch (Exception e)
                {
                    //В случае какой-либо ошибки при создании пользователя (например валидацию не прошло, либо
                    //непредвиденная ошибка, мы откатываем изменения
                    transaction.Rollback();

                    MessageBox.Show("При регистрации произошла ошибка",
                        "Registration Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
        }

        //Метод проверяет поле логина на валидность, используется стандартная библиотека System.Net.Mail
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