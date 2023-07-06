using System.Collections.Generic;
using System.Linq;
using System.Windows;
using InstrumentalSystem.Client.Modals;

namespace InstrumentalSystem.Client.View
{
    public partial class UserSelectionDialog : Window
    {
        public List<User> AvailableUsers { get; set; }
        public List<User> SelectedUsers { get; private set; }

        public UserSelectionDialog(List<User> users)
        {
            InitializeComponent();
            AvailableUsers = users;
            DataContext = this;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedUsers = new List<User>(UsersListBox.SelectedItems.Cast<User>());
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}