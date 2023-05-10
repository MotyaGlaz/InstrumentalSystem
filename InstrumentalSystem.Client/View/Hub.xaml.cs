using System.Collections.Generic;
using System.Windows;
using InstrumentalSystem.Client.View.Modals;
using Library.General.Project;
using Library.IOSystem.Reader;

namespace InstrumentalSystem.Client.View
{
    /// <summary>
    /// Логика взаимодействия для Hub.xaml
    /// </summary>
    public partial class Hub : Window
    {
        private List<ProjectInfo> _projects;
        private List<ProjectInfo> _serverProjects;
        public Hub()
        {
            InitializeComponent();
            _projects = ProjectReader.ReadProjectsInfo();
            LocalProjects.ItemsSource = _projects;
            _serverProjects = new List<ProjectInfo>();
            _serverProjects.Add(new ProjectInfo(
                "Проект 1",
                "rabbid",
                "Последние изменения: 01.07.2022",
                "\\View\\Images\\roles\\roleOwnerIcon.png"
                ));
            _serverProjects.Add(new ProjectInfo(
                "Тестовый Совместный Проект",
                "wrap",
                "Последние изменения: 02.07.2022",
                "\\View\\Images\\roles\\roleEditorIcon.png"
                ));
            _serverProjects.Add(new ProjectInfo(
                "Проверка Прав",
                "wrap2",
                "Последние изменения: 29.06.2022",
                "\\View\\Images\\roles\\roleViewerIcon.png"
                ));
            _serverProjects.Add(new ProjectInfo(
                "Калькулятор 1",
                "rabbid",
                "Последние изменения: 02.07.2022",
                "\\View\\Images\\roles\\roleOwnerIcon.png"
                ));
            GlobalProjects.ItemsSource = _serverProjects;
            UserButton.IsEnabled = false;
        }

        public void RefreshProjectList()
        {
            _projects = ProjectReader.ReadProjectsInfo();
            LocalProjects.ItemsSource = _projects;
            LocalProjects.Items.Refresh();
        }

        private void UserButton_Click(object sender, RoutedEventArgs e)
        {

            ContentGrid.Children.Add(new AuthenticationModal(this));

        }

        private void CreateProjectButton_Click(object sender, RoutedEventArgs e)
        {
            ContentGrid.Children.Add(new ProjectCreationModal(this));
        }

        private void LocalProjects_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (LocalProjects.SelectedIndex == -1)
                return;
            ContentGrid.Children.Add(new LocalProjectInfoModal(_projects[LocalProjects.SelectedIndex]));
            
        }

        private void GlobalProjects_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            ContentGrid.Children.Add(new GlobalProjectInfoModal());
        }
    }
}
