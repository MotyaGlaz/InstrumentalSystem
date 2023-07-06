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

        public void CreateUser(string fullName, Account account, MySqlTransaction transaction,
            string role, string organization = "")
        {
            var mySqlCommand = new MySqlCommand(
                "INSERT INTO users (account_id, ФИО, role, organization) VALUES (@accountId, @fullName, @role, @organization)",
                _connection);

            mySqlCommand.Parameters.AddWithValue("@accountId", account.Id);
            mySqlCommand.Parameters.AddWithValue("@fullName", fullName);
            mySqlCommand.Parameters.AddWithValue("@role", role);
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

        public List<Project> GetProjectsForUser(int userId, string status = null)
        {
            var projectData = new List<(int, string, string, DateTime, DateTime, string)>();
            var commentData = new Dictionary<int, List<CustomCollection>>();
            var commentUserIdData = new Dictionary<int, int>();

            var mySqlCommand = new MySqlCommand(
                "SELECT projects.id, projects.name, projects.status, projects.created_date, projects.last_modified_date, " +
                "comments.id AS comment_id, comments.comment, comments.user_id, projects.path " +
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
                    string path = reader.GetString("path");

                    if (!projectData.Any(p => p.Item1 == id))
                    {
                        projectData.Add((id, name, statusDB, createdDate, lastModifiedDate, path));
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

                var project = new Project(data.Item1, data.Item2, data.Item3, data.Item4, data.Item5, comments, users, data.Item6);
                projects.Add(project);
            }

            return projects;
        }

        public List<Project> GetAllProjects(string status = null)
        {
            var projectData = new List<(int, string, string, DateTime, DateTime, string)>();
            var commentData = new Dictionary<int, List<CustomCollection>>();
            var commentUserIdData = new Dictionary<int, int>();

            var mySqlCommand = new MySqlCommand(
                "SELECT projects.id, projects.name, projects.status, projects.created_date, projects.last_modified_date, " +
                "comments.id AS comment_id, comments.comment, comments.user_id, projects.path " +
                "FROM projects " +
                "LEFT JOIN comments ON projects.id = comments.project_id" +
                (status != null ? " WHERE projects.status = @status" : ""),
                _connection);

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
                    string path = reader.GetString("path");

                    if (!projectData.Any(p => p.Item1 == id))
                    {
                        projectData.Add((id, name, statusDB, createdDate, lastModifiedDate, path));
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

                var project = new Project(data.Item1, data.Item2, data.Item3, data.Item4, data.Item5, comments, users, data.Item6);
                projects.Add(project);
            }

            return projects;
        }

        public List<User> GetUsersForProject(int projectId)
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

        public void DeleteUser(int userId)
        {
            using (var transaction = _connection.BeginTransaction())
            {
                try
                {
                    // Delete comments associated with the user
                    var deleteCommentsCommand = new MySqlCommand(
                        "DELETE FROM comments WHERE user_id = @userId",
                        _connection);
                    deleteCommentsCommand.Parameters.AddWithValue("@userId", userId);
                    deleteCommentsCommand.Transaction = transaction;
                    deleteCommentsCommand.ExecuteNonQuery();

                    // Get accountId for the user
                    var getAccountIdCommand = new MySqlCommand(
                        "SELECT account_id FROM users WHERE UniqueID = @userId",
                        _connection);
                    getAccountIdCommand.Parameters.AddWithValue("@userId", userId);
                    getAccountIdCommand.Transaction = transaction;

                    var accountId = 0;

                    using (var reader = getAccountIdCommand.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            accountId = reader.GetInt32("account_id");
                        }
                    }

                    // Get list of projects where the user is the only participant
                    var projectIdsCommand = new MySqlCommand(
                        "SELECT project_id FROM user_projects_link " +
                        "GROUP BY project_id " +
                        "HAVING COUNT(user_id) = 1 AND MAX(user_id) = @userId",
                        _connection);
                    projectIdsCommand.Parameters.AddWithValue("@userId", userId);
                    projectIdsCommand.Transaction = transaction;

                    var projectIdList = new List<int>();

                    using (var reader = projectIdsCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            projectIdList.Add(reader.GetInt32("project_id"));
                        }
                    }

                    // Delete these projects from the user_projects_link table
                    foreach (var projectId in projectIdList)
                    {
                        var deleteProjectLinkCommand = new MySqlCommand(
                            "DELETE FROM user_projects_link WHERE project_id = @projectId",
                            _connection);
                        deleteProjectLinkCommand.Parameters.AddWithValue("@projectId", projectId);
                        deleteProjectLinkCommand.Transaction = transaction;
                        deleteProjectLinkCommand.ExecuteNonQuery();
                    }

                    // Delete these projects from the projects table
                    foreach (var projectId in projectIdList)
                    {
                        var deleteProjectCommand = new MySqlCommand(
                            "DELETE FROM projects WHERE id = @projectId",
                            _connection);
                        deleteProjectCommand.Parameters.AddWithValue("@projectId", projectId);
                        deleteProjectCommand.Transaction = transaction;
                        deleteProjectCommand.ExecuteNonQuery();
                    }

                    // Delete user from user_projects_link
                    var deleteLinkCommand = new MySqlCommand(
                        "DELETE FROM user_projects_link WHERE user_id = @userId",
                        _connection);
                    deleteLinkCommand.Parameters.AddWithValue("@userId", userId);
                    deleteLinkCommand.Transaction = transaction;
                    deleteLinkCommand.ExecuteNonQuery();

                    // Delete user from users
                    var deleteUserCommand = new MySqlCommand(
                        "DELETE FROM users WHERE UniqueID = @userId",
                        _connection);
                    deleteUserCommand.Parameters.AddWithValue("@userId", userId);
                    deleteUserCommand.Transaction = transaction;
                    deleteUserCommand.ExecuteNonQuery();

                    // Delete user from accounts
                    if (accountId != 0)
                    {
                        var deleteAccountCommand = new MySqlCommand(
                            "DELETE FROM accounts WHERE UniqueID = @accountId",
                            _connection);
                        deleteAccountCommand.Parameters.AddWithValue("@accountId", accountId);
                        deleteAccountCommand.Transaction = transaction;
                        deleteAccountCommand.ExecuteNonQuery();
                    }

                    // Commit transaction
                    transaction.Commit();
                }
                catch
                {
                    // If anything goes wrong, roll back the transaction
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public void DeleteProject(Project project)
        {
            using (var transaction = _connection.BeginTransaction())
            {
                try
                {
                    // Delete comments associated with the project
                    var deleteCommentsCommand = new MySqlCommand(
                        "DELETE FROM comments WHERE project_id = @projectId",
                        _connection);
                    deleteCommentsCommand.Parameters.AddWithValue("@projectId", project.Id);
                    deleteCommentsCommand.Transaction = transaction;
                    deleteCommentsCommand.ExecuteNonQuery();

                    // Delete records from user_projects_link table
                    var deleteProjectLinkCommand = new MySqlCommand(
                        "DELETE FROM user_projects_link WHERE project_id = @projectId",
                        _connection);
                    deleteProjectLinkCommand.Parameters.AddWithValue("@projectId", project.Id);
                    deleteProjectLinkCommand.Transaction = transaction;
                    deleteProjectLinkCommand.ExecuteNonQuery();

                    // Delete project from projects
                    var deleteProjectCommand = new MySqlCommand(
                        "DELETE FROM projects WHERE id = @projectId",
                        _connection);
                    deleteProjectCommand.Parameters.AddWithValue("@projectId", project.Id);
                    deleteProjectCommand.Transaction = transaction;
                    deleteProjectCommand.ExecuteNonQuery();

                    // Commit transaction
                    transaction.Commit();
                }
                catch
                {
                    // If anything goes wrong, roll back the transaction
                    transaction.Rollback();
                    throw;
                }
            }
        }
        
        public void DeleteUserFromProject(int projectId, int userId)
        {
            using (var transaction = _connection.BeginTransaction())
            {
                try
                {
                    // Delete all comments from this user on the project
                    var deleteCommentsCommand = new MySqlCommand(
                        "DELETE FROM comments WHERE project_id = @projectId AND user_id = @userId",
                        _connection);
                    deleteCommentsCommand.Parameters.AddWithValue("@projectId", projectId);
                    deleteCommentsCommand.Parameters.AddWithValue("@userId", userId);
                    deleteCommentsCommand.Transaction = transaction;
                    deleteCommentsCommand.ExecuteNonQuery();

                    // Delete the link from UserProjectsLink
                    var deleteUserProjectLinkCommand = new MySqlCommand(
                        "DELETE FROM user_projects_link WHERE project_id = @projectId AND user_id = @userId",
                        _connection);
                    deleteUserProjectLinkCommand.Parameters.AddWithValue("@projectId", projectId);
                    deleteUserProjectLinkCommand.Parameters.AddWithValue("@userId", userId);
                    deleteUserProjectLinkCommand.Transaction = transaction;
                    deleteUserProjectLinkCommand.ExecuteNonQuery();

                    // Commit transaction
                    transaction.Commit();
                }
                catch
                {
                    // If anything goes wrong, roll back the transaction
                    transaction.Rollback();
                    throw;
                }
            }
        }
        
        public void AddUserToProject(int projectId, int userId)
        {
            // Create a command to insert a new row into the user_projects_link table
            var mySqlCommand = new MySqlCommand(
                "INSERT INTO user_projects_link (user_id, project_id) VALUES (@user_id, @project_id)",
                _connection);
            mySqlCommand.Parameters.AddWithValue("@user_id", userId);
            mySqlCommand.Parameters.AddWithValue("@project_id", projectId);

            mySqlCommand.ExecuteNonQuery();
        }

        public MySqlTransaction BeginTransaction()
        {
            return _connection.BeginTransaction();
        }

        private MySqlConnection GetConnection()
        {
            // string connectionLink = "server=127.0.0.1;user=root;database=userdb;password=Nikaree9898;";
            string connectionLink = "server=127.0.0.1;user=root;database=userdb;password=grow_147-olw+2815;";
            return new MySqlConnection(connectionLink);
        }
    }
}