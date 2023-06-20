namespace InstrumentalSystem.Client.Modals
{
    //Класс Account создан для описания аккаунта, чтобы с данного объекта забирать данные и потом передавать в БД
    public class Account
    {
        public int Id { get; private set; }
        public string Login { get; private set; }
        public string Password { get; private set; }

        public Account(int id)
        {
            Id = id;
        }
        
        public void SetLogin(string login)
        {
            Login = login;
        }

        public void SetPassword(string password)
        {
            Password = password;
        }
    }
}