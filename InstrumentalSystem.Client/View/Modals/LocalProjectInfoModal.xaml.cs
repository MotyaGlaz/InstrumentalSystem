using Library.General.Project;
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
    /// Логика взаимодействия для LocalProjectInfoModal.xaml
    /// </summary>
    public partial class LocalProjectInfoModal : UserControl
    {
        private ProjectInfo _info;
        private List<LogicModule> _modules;
        public LocalProjectInfoModal(ProjectInfo info)
        {
            InitializeComponent();
            _info = info;
            ModalHeader.Content = _info.Name;
            ProjectNameLabel.Text = _info.Name;
            PathLabel.Text = _info.Path;
            LastEditedLabel.Text = _info.Date;
            ReadModules();
            ModuleList.ItemsSource = _modules;
        }

        private void ReadModules()
        {
            _modules = new List<LogicModule>();
            var parse = _info.Path.Split("\\");
            var master = File.ReadAllText($"{_info.Path}\\{parse.Last()}.master");
            foreach (var line in master.Split("\n"))
            {
                if (line.Length == 0)
                    return;
                var lineParse = line.Split("|");
                _modules.Add(new LogicModule($"{lineParse[0]}|{lineParse[1]}"));
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Collapsed;
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            Editor editor = new Editor(_info.Path);
            editor.Show();
            this.Visibility = Visibility.Collapsed;
        }
    }
}
