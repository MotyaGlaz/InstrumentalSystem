using System.Windows;
using System.Windows.Controls;
using InstrumentalSystem.Client.Modals;

namespace InstrumentalSystem.Client.View
{
    public partial class ProjectEditWindow : Window
    {
        
        
        private Project _project;
        private Database _database;
        private int _userId;

        public ProjectEditWindow(Project project, int userId)
        {
            InitializeComponent();

            _project = project;
            _database = Database.Instance;
            _userId = userId;

            DataContext = _project;
        }

        private void AddCommentButton_Click(object sender, RoutedEventArgs e)
        {
            var addCommentWindow = new AddCommentWindow(_project, _userId);

            addCommentWindow.ShowDialog();
        }

        private void EditProjectButton_Click(object sender, RoutedEventArgs e)
        {
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            ClosingProjectEditWindow closingProjectEditWindow = new ClosingProjectEditWindow();
            bool? result = closingProjectEditWindow.ShowDialog();

            if (result.HasValue && result.Value)
            {
                _database.SaveComments(_project);
                Close();
            }
            else if (result.HasValue)
            {
                Close();
            }
        }
    }
}