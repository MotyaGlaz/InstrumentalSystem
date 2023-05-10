using Library.General.Project;
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

namespace InstrumentalSystem.Client.View.Pages.ModuleCreation
{
    /// <summary>
    /// Логика взаимодействия для ParentModulePage.xaml
    /// </summary>
    public partial class ParentModulePage : Page
    {
        private List<LogicModule> _logicModules;
        public ParentModulePage(Project project)
        {
            InitializeComponent();
            _logicModules = new List<LogicModule>();
            foreach (var @namespace in project.Namespaces)
            {
                foreach (var level in @namespace.Levels)
                {
                    _logicModules.Add(new LogicModule($"{@namespace.Name}|{level.Name}"));
                }
            }
            ModuleList.ItemsSource = _logicModules;
        }
    }
}
