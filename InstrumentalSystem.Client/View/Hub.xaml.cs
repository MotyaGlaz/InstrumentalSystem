using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using InstrumentalSystem.Client.Modals;
using InstrumentalSystem.Client.View.Modals;
using Library.General.Project;
using Library.IOSystem.Reader;
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
        private int _userId;
        
        public Hub(int userId)
        {
            InitializeComponent();
            _userId = userId;
            
            _openProjects = ReadOpenProjectsInfo(_userId);
            LocalProjects.ItemsSource = _openProjects;
            
            LoadClosedProjects();

            UserButton.IsEnabled = true;
        }

        private void LoadClosedProjects()
        {
            var user = Database.Instance.GetUserById(_userId);

            if (user.Role == "специалист")
            {
                ClosedProjectsTab.Visibility = Visibility.Visible;

                _closedProjects = ReadClosedProjectsInfo(_userId);
                ClosedProjects.ItemsSource = _closedProjects;
            }
            else
            {
                ClosedProjectsTab.Visibility = Visibility.Collapsed;
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
            Database database = Database.Instance;
            
            var projects = database.GetProjectsForUser(userId, "open");

            return projects;
        }
        
        public List<Project> ReadClosedProjectsInfo(int userId)
        {
            Database database = Database.Instance;
            
            var projects = database.GetProjectsForUser(userId, "closed");

            return projects;
        }
        
        //Добавил метод для разлогина
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

        private void LocalProjects_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (LocalProjects.SelectedIndex == -1)
                return;
            
            Project selectedProject = (Project)LocalProjects.SelectedItem;
            //ContentGrid.Children.Add(new LocalProjectInfoModal(selectedProject));
        }

        private void GlobalProjects_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            ContentGrid.Children.Add(new GlobalProjectInfoModal());
        }

        //Добавил новый метод
        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            Project project = button.DataContext as Project;
            
            ProjectEditWindow projectEditWindow = new ProjectEditWindow(project, _userId);
            projectEditWindow.ShowDialog();
        }

        private void ProjectCreationWindow_ProjectCreated(object sender, EventArgs e)
        {
            RefreshProjectList();
        }
    }
}
