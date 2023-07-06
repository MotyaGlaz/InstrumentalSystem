using System;
using System.Text;
using System.Windows;
using InstrumentalSystem.Client.Modals;

namespace InstrumentalSystem.Client.View
{
    public partial class AddCommentWindow : Window
    {
        private Project _project;
        private int _userId;

        public AddCommentWindow(Project project, int userId)
        {
            InitializeComponent();
            _project = project;
            _userId = userId;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            string comment = commentTextBox.Text;

            if (string.IsNullOrEmpty(comment))
            {
                MessageBoxResult result = MessageBox.Show("Сохранить пустой комментарий?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.No)
                {
                    return;
                }
            }

            _project.AddComment(comment, _userId);

            Close();
        }
    }
}