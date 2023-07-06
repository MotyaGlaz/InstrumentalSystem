using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using InstrumentalSystem.Client.Modals;

namespace InstrumentalSystem.Client.View
{
    public partial class UsersListWindow : Window
    {
        public delegate void ProjectCreatedEventHandler();
        public event ProjectCreatedEventHandler UserDeletedFromProject;
        
        private Project _project;
        private Database _database;

        public UsersListWindow(Project project)
        {
            InitializeComponent();
            _project = project;
            _database = Database.Instance;
            LoadUsers();
        }

        private void LoadUsers()
        {
            // Assuming you have a method in your database class that gets users for a project.
            var users = _database.GetUsersForProject(_project.Id);
            UsersListView.ItemsSource = users;
        }

        private void RemoveUserButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            User userToRemove = (User)button.DataContext;

            var result = MessageBox.Show("Вы уверены, что хотите удалить пользователя из проекта?",
                "Confirm delete",
                MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes)
            {
                _database.DeleteUserFromProject(_project.Id, userToRemove.Id);

                MessageBox.Show("Пользователь успешно удалён из проекта, необходимо переоткрыть проект",
                    "Submit",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                UserDeletedFromProject.Invoke();
                
                Close();
            }
        }
        
        private void AddUserButton_Click(object sender, RoutedEventArgs e)
        {
            List<User> allUsers = _database.GetAllUsers();
            allUsers.RemoveAll(user => user.Role == "администратор");
            
            List<User> projectUsers = _database.GetUsersForProject(_project.Id);

            // Use LINQ to get all users that are not already in the project.
            List<User> availableUsers = allUsers.Except(projectUsers).ToList();

            if (_project.Status == "closed")
            {
                availableUsers = availableUsers.Where(user => user.Role == "специалист").ToList();
            }

            // Open a dialog for the user to select users from the availableUsers list.
            UserSelectionDialog dialog = new UserSelectionDialog(availableUsers);

            if (dialog.ShowDialog() == true)
            {
                foreach (User user in dialog.SelectedUsers)
                {
                    _database.AddUserToProject(_project.Id, user.Id);
                }
                
                LoadUsers();
            }
        }
    }
}