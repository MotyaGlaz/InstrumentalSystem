﻿using InstrumentalSystem.Client.View.Modals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace InstrumentalSystem.Client.View.Pages.Authenticate
{
    /// <summary>
    /// Логика взаимодействия для RegistrationPage.xaml
    /// </summary>
    public partial class RegistrationPage : Page
    {
        private AuthenticationModal _parent;

        //Добавлены автореализуемые свойства, чтобы со страницы забирать данные, введенные пользователем
        public string UserName
        {
            get { return usernameTextBox.Text; }
        }

        public string UserSurname
        {
            get { return usersurnameTextBox.Text; }
        }

        public string UserPatronymic
        {
            get { return userpatronymicTextBox.Text; }
        }

        public string UserLogin
        {
            get { return userloginTextBox.Text; }
        }

        public string UserPassword
        {
            get { return userpasswordTextBox.Text; }
        }

        public RegistrationPage(AuthenticationModal parent)
        {
            InitializeComponent();
            _parent = parent;
            _parent.NextButton.Content = "Создать";
            _parent.HeaderLabel.Content = "Создание учетной записи";
            _parent.BackButton.IsEnabled = true;
        }
    }
}