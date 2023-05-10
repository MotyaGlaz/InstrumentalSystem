using InstrumentalSystem.Client.Logic.Config;
using Library.IOSystem.Writer;
using System;
using System.Collections.Generic;
using System.IO;
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
    /// Логика взаимодействия для ProjectCreationModal.xaml
    /// </summary>
    public partial class ProjectCreationModal : UserControl
    {
        private string _path;
        private Hub _parent;
        public ProjectCreationModal(Hub parent)
        {
            InitializeComponent();
            _parent = parent;
            _path = Directory.GetCurrentDirectory();
            NameTextBox.Text = "Имя проекта";
            PathTextBox.Text = $"{_path}\\{NameTextBox.Text}";
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Collapsed;
        }

        private void CreationButton_Click(object sender, RoutedEventArgs e)
        {
            ClientConfig.Project = new Library.General.Project.Project(NameTextBox.Text);
            ProjectWriter writer = new ProjectWriter(PathTextBox.Text);
            writer.WriteProject(ClientConfig.Project);
            _parent.RefreshProjectList();
            this.Visibility = Visibility.Collapsed;
        }

        private void SetPathButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void NameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            PathTextBox.Text =$"{_path}\\{NameTextBox.Text}";
        }
    }
}
