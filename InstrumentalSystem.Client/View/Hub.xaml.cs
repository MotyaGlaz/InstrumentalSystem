using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using InstrumentalSystem.Client.Modals;
using Project = InstrumentalSystem.Client.Modals.Project;

namespace InstrumentalSystem.Client.View
{
    /// <summary>
    /// Логика взаимодействия для Hub.xaml
    /// </summary>
    public partial class Hub : Window
    {
        private List<Project> _openProjects;
        private List<Project> _closedProjects;
        private List<Project> _allProjects;
        
        private int _userId;
        private Database _database;

        private UserRole _userRole;
        
        public Hub(User user)
        {
            InitializeComponent();
            _userId = user.Id;
            _database = Database.Instance;
            CurrentUser = user;

            switch (user.Role)
            {
                case "администратор":
                    _userRole = new AdministratorRole();
                    LoadProjectsForAdministator();
                    LoadUsers();
                    break;
                case "специалист":
                    _userRole = new SpecialistRole();
                    RefreshProjectList();
                    break;
                case "обычный пользователь":
                    _userRole = new CommonRole();
                    RefreshProjectList();
                    break;
                default:
                    throw new ArgumentException($"Unknown user role: {user.Role}");
            }

            SetElementVisibility();
            LoadClosedProjects();
        }
        
        public User CurrentUser { get; private set; }

        private void LoadUsers()
        {
            var users = _database.GetAllUsers().Where(user => user.Role != "администратор").ToList();

            UserList.ItemsSource = users;
        }

        private void SetElementVisibility()
        {
            CreateProjectButton.Visibility = _userRole.CreateProjectButtonVisibility;
            CreateUserButton.Visibility = _userRole.CreateUserButtonVisibility;
            ClosedProjectsTab.Visibility = _userRole.ClosedProjectsTabVisibility;
            AllProjectsTab.Visibility = _userRole.AllProjectsTabVisibility;
            UsersTab.Visibility = _userRole.UsersTabVisibility;
        }

        private void LoadProjectsForAdministator()
        {
            _allProjects = _database.GetAllProjects();
            _closedProjects = _allProjects.Where(project => project.Status == "closed").ToList();
            _openProjects = _allProjects.Where(project => project.Status == "open").ToList();
            
            AllProjects.ItemsSource = _allProjects;
            ClosedProjects.ItemsSource = _closedProjects;
            LocalProjects.ItemsSource = _openProjects;
        }

        private void LoadClosedProjects()
        {
            var user = Database.Instance.GetUserById(_userId);

            if (user.Role == "специалист")
            {
                _closedProjects = ReadClosedProjectsInfo(_userId);
                ClosedProjects.ItemsSource = _closedProjects;
            }
        }

        public void RefreshProjectList()
        {
            _openProjects = ReadOpenProjectsInfo(_userId);
            LocalProjects.ItemsSource = _openProjects;
            LocalProjects.Items.Refresh();
            
            _closedProjects = ReadClosedProjectsInfo(_userId);
            ClosedProjects.ItemsSource = _closedProjects;
            ClosedProjects.Items.Refresh();
        }
        
        public List<Project> ReadOpenProjectsInfo(int userId)
        {
            var projects = _database.GetProjectsForUser(userId, "open");

            return projects;
        }
        
        public List<Project> ReadClosedProjectsInfo(int userId)
        {
            var projects = _database.GetProjectsForUser(userId, "closed");

            return projects;
        }
        
        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            CurrentWindowEditor.CurrentWindow = new Authorization();
        }

        private void CreateProjectButton_Click(object sender, RoutedEventArgs e)
        {
            bool isOpenProjectsTab = (myTabControl.SelectedItem == OpenProjectsTab);
            
            var projectCreationWindow = new ProjectCreationWindow(_userId, isOpenProjectsTab);

            projectCreationWindow.ProjectCreated += ProjectCreationWindow_ProjectCreated;

            projectCreationWindow.ShowDialog();
        }
        
        private void CreateUserButton_Click(object sender, RoutedEventArgs e)
        {
            var newUserCreationPage = new CreateNewUserWindow();
            
            newUserCreationPage.UserAdded += LoadUsers;

            newUserCreationPage.ShowDialog();
        }

        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            Project project = button.DataContext as Project;
            
            if (CurrentUser.Role == "администратор")
            {
                // Create and open a new window with a list of users
                UsersListWindow usersesListWindow = new UsersListWindow(project);

                usersesListWindow.UserDeletedFromProject += LoadProjectsForAdministator;
                
                usersesListWindow.ShowDialog();
                
                usersesListWindow.UserDeletedFromProject -= LoadProjectsForAdministator;
            }
            else
            {
                // If the user is not an administrator, open the ProjectEditWindow as usual
                ProjectEditWindow projectEditWindow = new ProjectEditWindow(project, _userId);
                projectEditWindow.ShowDialog();
            }
        }
        
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            Project project = button.DataContext as Project;
            
            var result = MessageBox.Show("Вы уверены, что хотите удалить проект?", "Confirm delete", MessageBoxButton.YesNo);
            
            if (result == MessageBoxResult.Yes)
            {
                _database.DeleteProject(project);
                LoadProjectsForAdministator();
            }
        }
        
        private void DeleteUserButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            User user = button.DataContext as User;
    
            var result = MessageBox.Show("Вы уверены, что хотите удалить данного пользователя?", "Confirm delete", MessageBoxButton.YesNo);
            
            if (result == MessageBoxResult.Yes)
            {
                _database.DeleteUser(user.Id);
                LoadUsers();
                LoadProjectsForAdministator();
            }
        }

        private void ProjectCreationWindow_ProjectCreated(object sender, EventArgs e)
        {
            RefreshProjectList();
        }
    }
    
    public abstract class UserRole
    {
        public abstract Visibility CreateProjectButtonVisibility { get; }
        public abstract Visibility CreateUserButtonVisibility { get; }
        public abstract Visibility AllProjectsTabVisibility { get; }
        public abstract Visibility ClosedProjectsTabVisibility { get; }
        public abstract Visibility UsersTabVisibility { get; }
    }

    public class AdministratorRole : UserRole
    {
        public override Visibility CreateProjectButtonVisibility => Visibility.Collapsed;
        public override Visibility CreateUserButtonVisibility => Visibility.Visible;
        public override Visibility AllProjectsTabVisibility => Visibility.Visible;
        public override Visibility ClosedProjectsTabVisibility => Visibility.Visible;
        public override Visibility UsersTabVisibility => Visibility.Visible;
    }

    public class SpecialistRole : UserRole
    {
        public override Visibility CreateProjectButtonVisibility => Visibility.Visible;
        public override Visibility CreateUserButtonVisibility => Visibility.Collapsed;
        public override Visibility AllProjectsTabVisibility => Visibility.Collapsed;
        public override Visibility ClosedProjectsTabVisibility => Visibility.Visible;
        public override Visibility UsersTabVisibility => Visibility.Collapsed;
    }
    
    public class CommonRole : UserRole
    {
        public override Visibility CreateProjectButtonVisibility => Visibility.Visible;
        public override Visibility CreateUserButtonVisibility => Visibility.Collapsed;
        public override Visibility AllProjectsTabVisibility => Visibility.Collapsed;
        public override Visibility ClosedProjectsTabVisibility => Visibility.Collapsed;
        public override Visibility UsersTabVisibility => Visibility.Collapsed;
    }
}
