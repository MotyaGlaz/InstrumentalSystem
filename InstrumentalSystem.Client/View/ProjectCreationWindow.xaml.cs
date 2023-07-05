﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using InstrumentalSystem.Client.Modals;
using User = InstrumentalSystem.Client.Modals.User;

namespace InstrumentalSystem.Client.View
{
    public partial class ProjectCreationWindow : Window
    {
        public delegate void ProjectCreatedEventHandler(object sender, EventArgs e);
        public event ProjectCreatedEventHandler ProjectCreated;
        
        private Database _database;
        private int _userId;

        private UserViewModel _userViewModel;
        private string _status;

        public ProjectCreationWindow(int userId, bool isOpenProjectsTab)
        {
            InitializeComponent();

            _userViewModel = new UserViewModel();
            _database = Database.Instance;
            _userId = userId;
            List<User> allUsers;
            
            if (isOpenProjectsTab)
            {
                // For open projects: All users, except those with the "Administrator" role
                allUsers = _database.GetAllUsers().Where(user => user.Id != _userId && user.Role != "администратор").ToList();
                _status = "open";
            }
            else
            {
                // For closed projects: Only users with the "Specialist" role
                allUsers = _database.GetAllUsers().Where(user => user.Id != _userId && user.Role == "специалист").ToList();
                _status = "closed";
            }
            
            usersListView.ItemsSource = _userViewModel.GetAllUsersViewModels(allUsers);
        }

        private void submitProjectButton_Click(object sender, RoutedEventArgs e)
        {
            // Get the input data
            string projectName = projectNameTextBox.Text;

            if (projectName.Trim().Equals(String.Empty) == false)
            {
                // I assume usersListView is a listbox which can contain multiple selected items
                var selectedUsers = ((List<UserViewModel>)usersListView.ItemsSource)
                    .Where(userVM => userVM.IsSelected)
                    .Select(userVM => userVM.User)
                    .ToList();
            
                selectedUsers.Add(_database.GetUserById(_userId));

                // Use the input data to create a new project in the database
                _database.CreateProject(projectName, selectedUsers, _status);

                ProjectCreated?.Invoke(this, new EventArgs());
            
                // Close the window
                Close();
            }
            else
            {
                MessageBox.Show("Имя проекта не может быть пустым",
                    "Create Project Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
        }
    }
    
    public class UserViewModel
    {
        public User User { get; set; }
        public bool IsSelected { get; set; }
            
        public List<UserViewModel> GetAllUsersViewModels(List<User> users)
        {
            return users.Select(user => new UserViewModel { User = user }).ToList();
        }
    }
}