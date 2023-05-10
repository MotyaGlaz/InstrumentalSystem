using InstrumentalSystem.Client.View.Pages.Authenticate;
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

namespace InstrumentalSystem.Client.View.Modals
{
    /// <summary>
    /// Логика взаимодействия для AuthenticationModal.xaml
    /// </summary>
    public partial class AuthenticationModal : UserControl
    {
        private Hub _parent;

        public AuthenticationModal(Hub parent)
        {
            InitializeComponent();
            _parent = parent;
            AuthenticationFrame.Content = new AuthenticationPage(this);
        }



        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Collapsed;
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (AuthenticationFrame.Content is RegistrationPage)
                return;
            _parent.AvatarLabel.Background = new SolidColorBrush(Colors.LightCoral);
            _parent.AvatarLabel.Content = "ВС";
            _parent.GlobalProjectsTab.IsEnabled = true;
            this.Visibility = Visibility.Collapsed;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            AuthenticationFrame.Content = new AuthenticationPage(this);
        }
    }
}
