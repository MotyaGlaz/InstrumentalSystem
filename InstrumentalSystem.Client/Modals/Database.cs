using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Library.General.Project;
using Library.General.User;
using MySql.Data.MySqlClient;

namespace InstrumentalSystem.Client.Modals
{
    public class Database
    {
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

        public void CreateUser(string fullName, Account account, string organization, MySqlTransaction transaction)
        {
            var mySqlCommand = new MySqlCommand(
                "INSERT INTO users (account_id, ФИО, role, organization) VALUES (@accountId, @fullName, @role, @organization)",
                _connection);

            mySqlCommand.Parameters.AddWithValue("@accountId", account.Id);
            mySqlCommand.Parameters.AddWithValue("@fullName", fullName);
            mySqlCommand.Parameters.AddWithValue("@role", "обычный пользователь");
            mySqlCommand.Parameters.AddWithValue("@organization", organization);
            mySqlCommand.Transaction = transaction;

            mySqlCommand.ExecuteNonQuery();
        }

        public List<User> GetAllUsers()
        {
            var users = new List<User>();

            var mySqlCommand = new MySqlCommand(
                "SELECT users.*, accounts.login FROM users INNER JOIN accounts ON users.account_id = accounts.UniqueID",
                _connection);

            using (var reader = mySqlCommand.ExecuteReader())
            {
                while (reader.Read())
                {
                    int id = reader.GetInt32("UniqueID");
                    int accountId = reader.GetInt32("account_id");
                    string fullName = reader.GetString("ФИО");
                    string role = reader.GetString("role");
                    string organization = reader.GetString("organization");
                    string login = reader.GetString("login"); // Get login from the result set.

                    var user = new User(id, accountId, fullName, role, organization, login);

                    users.Add(user);
                }
            }

            return users;
        }

        public Account GetAccount(int accountId, MySqlTransaction transaction)
        {
            Account account = null;

            var mySqlCommand = new MySqlCommand(
                "SELECT * FROM accounts WHERE UniqueID = @accountId",
                _connection);
            mySqlCommand.Parameters.AddWithValue("@accountId", accountId);
            mySqlCommand.Transaction = transaction;

            using (var reader = mySqlCommand.ExecuteReader())
            {
                if (reader.Read())
                {
                    account = new Account(reader.GetInt32("UniqueID"));

                    account.SetLogin(reader.GetString("login"));
                    account.SetPassword(reader.GetString("password"));
                }
            }

            return account;
        }

        public int CreateAccount(string login, string password, MySqlTransaction transaction)
        {
            if (UserExists(login))
            {
                return -1;
            }

            var mySqlCommand = new MySqlCommand(
                "INSERT INTO accounts (login, password) VALUES (@login, @password)",
                _connection);

            mySqlCommand.Parameters.AddWithValue("@login", login);
            mySqlCommand.Parameters.AddWithValue("@password", password);
            mySqlCommand.Transaction = transaction;
            mySqlCommand.ExecuteNonQuery();

            mySqlCommand = new MySqlCommand("SELECT LAST_INSERT_ID()", _connection);
            mySqlCommand.Transaction = transaction;
            int newAccountId = Convert.ToInt32(mySqlCommand.ExecuteScalar());

            return newAccountId;
        }

        public bool UserExists(string login)
        {
            MySqlCommand mySqlCommand =
                new MySqlCommand("SELECT COUNT(*) FROM accounts WHERE login = @login", _connection);
            mySqlCommand.Parameters.AddWithValue("@login", login);

            int count = Convert.ToInt32(mySqlCommand.ExecuteScalar());
            return count > 0;
        }

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

        //Добавил метод, получающий из БД проекты пользователя по id пользователя
        public List<Project> GetProjectsForUser(int userId, string status = null)
        {
            var projectData = new List<(int, string, string, DateTime, DateTime)>();
            var commentData = new Dictionary<int, List<CustomCollection>>();
            var commentUserIdData = new Dictionary<int, int>();

            var mySqlCommand = new MySqlCommand(
                "SELECT projects.id, projects.name, projects.status, projects.created_date, projects.last_modified_date, comments.id AS comment_id, comments.comment, comments.user_id " +
                "FROM projects " +
                "INNER JOIN user_projects_link ON projects.id = user_projects_link.project_id " +
                "LEFT JOIN comments ON projects.id = comments.project_id " +
                "WHERE user_projects_link.user_id = @userId" + (status != null ? " AND projects.status = @status" : ""),
                _connection);

            mySqlCommand.Parameters.AddWithValue("@userId", userId);

            if (status != null)
            {
                mySqlCommand.Parameters.AddWithValue("@status", status);
            }

            using (var reader = mySqlCommand.ExecuteReader())
            {
                while (reader.Read())
                {
                    int id = reader.GetInt32("id");
                    string name = reader.GetString("name");
                    string statusDB = reader.GetString("status");
                    DateTime createdDate = reader.GetDateTime("created_date");
                    DateTime lastModifiedDate = reader.GetDateTime("last_modified_date");

                    if (!projectData.Any(p => p.Item1 == id))
                    {
                        projectData.Add((id, name, statusDB, createdDate, lastModifiedDate));
                    }

                    if (!reader.IsDBNull(reader.GetOrdinal("comment_id")))
                    {
                        int commentId = reader.GetInt32("comment_id");
                        string comment = reader.GetString("comment");
                        int commentUserId = reader.GetInt32("user_id");
                        
                        commentUserIdData[commentId] = commentUserId;

                        bool isNew = false;
                        var newComment = new CustomCollection(commentId, comment, commentUserId, isNew);
                        
                        if (commentData.ContainsKey(id))
                        {
                            commentData[id].Add(newComment);
                        }
                        else
                        {
                            commentData.Add(id, new List<CustomCollection> { newComment });
                        }
                    }
                }
            }
            
            foreach (var commentId in commentUserIdData.Keys)
            {
                var user = GetUserById(commentUserIdData[commentId]);
                
                foreach (var comment in commentData.Values.SelectMany(list => list.Where(c => c.Key == commentId)))
                {
                    comment.Username = user.Login;
                }
            }

            var projects = new List<Project>();

            foreach (var data in projectData)
            {
                var users = GetUsersForProject(data.Item1);
                var comments = new ObservableCollection<CustomCollection>();

                if (commentData.ContainsKey(data.Item1))
                {
                    foreach (var comment in commentData[data.Item1])
                    {
                        comments.Add(comment);
                    }
                }

                var project = new Project(data.Item1, data.Item2, data.Item3, data.Item4, data.Item5, comments, users);
                projects.Add(project);
            }

            return projects;
        }

        //Новый метод для получения пользователей, связанных с проектом
        private List<User> GetUsersForProject(int projectId)
        {
            var users = new List<User>();

            var getUsersSqlCommand = new MySqlCommand(
                "SELECT users.*, accounts.login FROM users " +
                "INNER JOIN user_projects_link ON users.UniqueID = user_projects_link.user_id " +
                "INNER JOIN accounts ON users.account_id = accounts.UniqueID " +
                "WHERE user_projects_link.project_id = @projectId",
                _connection);

            getUsersSqlCommand.Parameters.AddWithValue("@projectId", projectId);

            using (var reader = getUsersSqlCommand.ExecuteReader())
            {
                while (reader.Read())
                {
                    int id = reader.GetInt32("UniqueID");
                    int accountId = reader.GetInt32("account_id");
                    string fio = reader.GetString("ФИО");
                    string role = reader.GetString("role");
                    string organization = reader.GetString("organization");
                    string login = reader.GetString("login");

                    var user = new User(id, accountId, fio, role, organization, login);

                    users.Add(user);
                }
            }

            return users;
        }

        //Добавил метод для получения id пользователя в БД по логину
        public int GetUserIdByLogin(string login)
        {
            int userId = 0;

            var mySqlCommand = new MySqlCommand(
                "SELECT users.UniqueID FROM users " +
                "INNER JOIN accounts ON users.account_id = accounts.UniqueID " +
                "WHERE accounts.Login = @login",
                _connection);

            mySqlCommand.Parameters.AddWithValue("@login", login);

            using (var reader = mySqlCommand.ExecuteReader())
            {
                if (reader.Read())
                {
                    userId = reader.GetInt32("UniqueID");
                    Console.WriteLine($"Got userId: {userId} for login: {login}");
                }
                else
                {
                    Console.WriteLine($"No user found with login: {login}");
                }
            }

            return userId;
        }

        //Добавли метод для добавления нового проекта в БД
        public void CreateProject(string projectName, List<User> users, string status)
        {
            // Create new project
            var mySqlCommand = new MySqlCommand(
                "INSERT INTO projects (name, status, created_date, last_modified_date) VALUES (@projectName, @status, NOW(), NOW()); SELECT LAST_INSERT_ID();",
                _connection);
            mySqlCommand.Parameters.AddWithValue("@projectName", projectName);
            mySqlCommand.Parameters.AddWithValue("@status", status);

            // Execute query and get the ID of the newly inserted project
            var projectId = Convert.ToInt32(mySqlCommand.ExecuteScalar());

            // Link each user to the newly created project
            foreach (var user in users)
            {
                mySqlCommand = new MySqlCommand(
                    "INSERT INTO user_projects_link (user_id, project_id) VALUES (@user_id, @project_id)",
                    _connection);
                mySqlCommand.Parameters.AddWithValue("@user_id", user.Id);
                mySqlCommand.Parameters.AddWithValue("@project_id", projectId);

                mySqlCommand.ExecuteNonQuery();
            }
        }

        //Добавил метод для получения пользователя по id
        public User GetUserById(int userId)
        {
            User user = null;

            var getUserSqlCommand = new MySqlCommand(
                "SELECT users.*, accounts.login FROM users " +
                "INNER JOIN accounts ON users.account_id = accounts.UniqueID " +
                "WHERE users.UniqueID = @userId",
                _connection);

            getUserSqlCommand.Parameters.AddWithValue("@userId", userId);

            using (var reader = getUserSqlCommand.ExecuteReader())
            {
                if (reader.Read())
                {
                    int id = reader.GetInt32("UniqueID");
                    int accountId = reader.GetInt32("account_id");
                    string fullName = reader.GetString("ФИО");
                    string role = reader.GetString("role");
                    string organization = reader.GetString("organization");
                    string login = reader.GetString("login");

                    user = new User(id, accountId, fullName, role, organization, login);
                }
            }

            return user;
        }

        //Добавил метод для сохранения комментариев к проекту
        public void SaveComments(Project project)
        {
            if (!project.Comments.Any())
            {
                return;
            }

            foreach (var comment in project.Comments)
            {
                if (comment.IsNew)
                {
                    using (var mySqlCommand = new MySqlCommand(
                               "INSERT INTO comments (project_id, comment, user_id) VALUES (@projectId, @comment, @userId); SELECT LAST_INSERT_ID();",
                               _connection))
                    {
                        mySqlCommand.Parameters.AddWithValue("@projectId", project.Id);
                        mySqlCommand.Parameters.AddWithValue("@comment", comment.Value);
                        mySqlCommand.Parameters.AddWithValue("@userId", comment.UserId);

                        mySqlCommand.ExecuteNonQuery();
                    }
                }
            }
        }

        public MySqlTransaction BeginTransaction()
        {
            return _connection.BeginTransaction();
        }

        private MySqlConnection GetConnection()
        {
            string connectionLink = "server=127.0.0.1;user=root;database=userdb;password=Nikaree9898;";
            return new MySqlConnection(connectionLink);
        }
    }
}