using System;
using MySql.Data.MySqlClient;

namespace InstrumentalSystem.Client.Modals
{
    public class Database
    {
        //Класс базы данных сделан в виде синглтона, чтобы был только один объект базы данных
        private static Database _instance = null;
        private static readonly object padlock = new object();

        private MySqlConnection _connection;

        Database()
        {
            _connection = GetConnection();
            _connection.Open();
        }

        ~Database()
        {
            _connection.Close();
        }

        public static Database Instance
        {
            get
            {
                lock (padlock)
                {
                    if (_instance == null)
                    {
                        _instance = new Database();
                    }

                    return _instance;
                }
            }
        }

        //Метод CreateUser создан для того, чтобы добавлять нового пользователя в БД
        public void CreateUser(string fullName, Account account, MySqlTransaction transaction)
        {
            //Используем данную SQL команду, чтобы добавить в users новую запись, используя полуенные данные
            var mySqlCommand = new MySqlCommand(
                "INSERT INTO users (account_id, ФИО, role) VALUES (@accountId, @fullName, @role)",
                _connection);

            //Стандартные методы MySQL
            mySqlCommand.Parameters.AddWithValue("@accountId", account.Id);
            mySqlCommand.Parameters.AddWithValue("@fullName", fullName);
            mySqlCommand.Parameters.AddWithValue("@role", "обычный пользователь");
            mySqlCommand.Transaction = transaction;

            mySqlCommand.ExecuteNonQuery();
        }

        //Метод для получения из БД записи account, это нужно для создания записи пользователя с использованием
        //данных из полученного здесь аккаунта (Логин и пароль + id)
        public Account GetAccount(int accountId, MySqlTransaction transaction)
        {
            Account account = null;

            //Забираем все записи с accounts по id (Всегда должна находиться либо одна запись, либо ни одной, так как
            //id уникальный
            var mySqlCommand = new MySqlCommand(
                "SELECT * FROM accounts WHERE UniqueID = @accountId",
                _connection);
            mySqlCommand.Parameters.AddWithValue("@accountId", accountId);
            mySqlCommand.Transaction = transaction;

            //Используем стандартный объект MySQL reader, чтобы создать объект Account с данными из БД
            using (var reader = mySqlCommand.ExecuteReader())
            {
                if (reader.Read())
                {
                    account = new Account(reader.GetInt32("UniqueID"));

                    account.SetLogin(reader.GetString("login"));
                    account.SetPassword(reader.GetString("password"));
                }
            }

            //Возвращаем полученный объект
            return account;
        }

        //Метод добавления новой записи в accounts в БД
        public int CreateAccount(string login, string password, MySqlTransaction transaction)
        {
            //Если пользователь по такому логину существует, то возвращем -1, то есть, не создаём новую запись
            if (UserExists(login))
            {
                return -1;
            }

            //В таблицу accounts добавляем новую запись с данными из UI
            var mySqlCommand = new MySqlCommand(
                "INSERT INTO accounts (login, password) VALUES (@login, @password)",
                _connection);

            mySqlCommand.Parameters.AddWithValue("@login", login);
            mySqlCommand.Parameters.AddWithValue("@password", password);
            mySqlCommand.Transaction = transaction;
            mySqlCommand.ExecuteNonQuery();

            //Забираем id последней добавленной записи в БД (которую мы только что добавили) и возвращаем её
            mySqlCommand = new MySqlCommand("SELECT LAST_INSERT_ID()", _connection);
            mySqlCommand.Transaction = transaction;
            int newAccountId = Convert.ToInt32(mySqlCommand.ExecuteScalar());

            return newAccountId;
        }

        //Метод создаёт запрос в БД и проверяет, есть ли запись в БД с таким логином
        public bool UserExists(string login)
        {
            MySqlCommand mySqlCommand =
                new MySqlCommand("SELECT COUNT(*) FROM accounts WHERE login = @login", _connection);
            mySqlCommand.Parameters.AddWithValue("@login", login);

            int count = Convert.ToInt32(mySqlCommand.ExecuteScalar());
            return count > 0;
        }

        //Метод проверки соответствия логина и пароля, делаем запрос в БД и проверяем, есть ли запись с таким логином и паролем
        public bool VerifyUser(string login, string password)
        {
            MySqlCommand mySqlCommand =
                new MySqlCommand("SELECT COUNT(*) FROM accounts WHERE login = @login AND password = @password",
                    _connection);
            mySqlCommand.Parameters.AddWithValue("@login", login);
            mySqlCommand.Parameters.AddWithValue("@password", password);

            int count = Convert.ToInt32(mySqlCommand.ExecuteScalar());
            return count > 0;
        }

        //Этот метод нужен для получения объекта MySqlTransaction, это нужно для механизма, который будет добавлять записи
        //В бд, только если все этапы добавления успешны (В нашём случае это account и user)
        public MySqlTransaction BeginTransaction()
        {
            return _connection.BeginTransaction();
        }

        //Метод нужен для подключения к MySQL Workbench
        private MySqlConnection GetConnection()
        {
            //string connectionLink = "server=127.0.0.1;user=root;database=is_db;password=root;";
            string connectionLink = "server=127.0.0.1;user=root;database=userdb;password=Nikaree9898;";
            return new MySqlConnection(connectionLink);
        }
    }
}