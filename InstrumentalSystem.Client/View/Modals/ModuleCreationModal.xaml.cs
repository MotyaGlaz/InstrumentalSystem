using InstrumentalSystem.Client.Logic.Config;
using InstrumentalSystem.Client.Logic.Task;
using InstrumentalSystem.Client.View.Pages.ModuleCreation;
using Library.Analyzer.Forest;
using Library.General.NameTable;
using Library.General.Project;
using Library.General.Workspace;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace InstrumentalSystem.Client.View.Modal
{
    /// <summary>
    /// Логика взаимодействия для ModuleCreationModal.xaml
    /// </summary>
    public partial class ModuleCreationModal : UserControl
    {
        private List<ModuleCreationTask> _tasks;
        private List<BaseNameElement> _undefinedNames;
        private List<BaseNameElement> _taskNames;
        private ModuleNameTable _nameTable;
        private int _count;
        private int _additionalCount;
        private LogicModuleNamespace _moduleNamespace;
        private LogicModule _parent;
        private Editor _parentWindow;
        private Dictionary<string, List<string>> _addedValues;
        StringBuilder _sorts = new StringBuilder();
        public ModuleCreationModal(Editor parentWindow, IModuleNameTable? nameTable)
        {
            InitializeComponent();
            _parentWindow = parentWindow;
            if (nameTable != null)
                if (nameTable is ModuleNameTable moduleNameTable)
                {
                    _undefinedNames = moduleNameTable.GetUndefidedNames();
                    _nameTable = moduleNameTable;
                }  
            _taskNames = new List<BaseNameElement>();          
            _tasks = new List<ModuleCreationTask>();
            _tasks.Add(new ModuleCreationTask(ModuleCreationTaskType.SetModuleName));
            _tasks.Add(new ModuleCreationTask(ModuleCreationTaskType.Uses));
            _count = 0;
            _additionalCount = 2;
            TaskList.ItemsSource = _tasks;

            SetNextContent();
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            //if (_tasks[_count].Equals("Задать значение"))
            //{
            //    values.Add(_taskList[_count].ID, new List<string>());
            //    var text = ((SortSetValuePage)TaskPage.Content).NamesTextBox.Text;
            //    foreach (var value in text.Split("\n"))
            //    {
            //       values[_taskList[_count].ID].Add(value);
            //    }
            //}
            if (_tasks.Count < 3)
                UpdateAdditionalTasks();
            else
                UpdateTasks();
            

            _count++;
            if (_count == _tasks.Count)
            {
                NextButton.IsEnabled = false;
                if (!(_parent is null))
                {
                    _moduleNamespace.GetLevel($"Уровень {_parent.GetLevel() - 1}").AddContent(
                        $"{_sorts.ToString()}" +
                        $"End;");
                    ClientConfig.Project.Add(_moduleNamespace.Name, _moduleNamespace.GetLevel($"Уровень {_parent.GetLevel() - 1}"));
                }
                else
                {
                    ClientConfig.Project.Add(_moduleNamespace);
                }
                    
                _parentWindow.TreeRefresh();
                this.Visibility = Visibility.Collapsed;
            }
            else
            {
                SetNextContent();
            }
            TaskList.Items.Refresh();
            
        }

        private void SetNextContent()
        {
            if (_count > 0)
                _tasks[_count - 1].InProgress = false;
            Page? page = default;
            switch (_tasks[_count].Task) 
            {
                case ModuleCreationTaskType.SetModuleName:
                    page = new SetNamePage();
                    break;
                case ModuleCreationTaskType.Uses:
                    page = new ModuleInicPage(ClientConfig.Project);
                    break;
                case ModuleCreationTaskType.BaseOn:
                    page = new ParentModulePage(ClientConfig.Project);
                    break;
                case ModuleCreationTaskType.SortSetValue:
                    page = new SortSetValuePage(_taskNames[_count -_additionalCount]);
                    break;
                case ModuleCreationTaskType.SortSetName:
                    page = new SortNamePage(_taskNames[_count - _additionalCount]);
                    break;
                case ModuleCreationTaskType.SortDefineValue:
                    page = new SortDefineValuePage(_taskNames[_count - _additionalCount]);
                    break;
            }

            TaskPage.Content = page;
        }

        private void UpdateAdditionalTasks()
        {
            if (_tasks[_count].Task == ModuleCreationTaskType.SetModuleName)
            {
                if (TaskPage.Content is SetNamePage namePage)
                {
                    if (namePage.isEmpty.IsChecked != true)
                    {
                        _tasks.Add(new ModuleCreationTask(ModuleCreationTaskType.BaseOn)); 
                        _additionalCount++;
                    }
                    else
                    {
                        _moduleNamespace = new LogicModuleNamespace(namePage.NameTextBox.Text);
                        _moduleNamespace.AddLevel(new LogicModule($"Уровень {namePage.LevelTextBox.Text}"));
                        #pragma warning disable CS8602
                        _moduleNamespace.GetLevel($"Уровень {namePage.LevelTextBox.Text}").SetContent(
                            $"Module {namePage.NameTextBox.Text}: {namePage.LevelTextBox.Text};\n" +
                            $"Begin\n" +
                            $"\n" +
                            $"End;\n");
                        #pragma warning restore CS8602
                    }
                }
            }
        }

        private void UpdateTasks()
        {
            switch (_tasks[_count].Task)
            {
                case ModuleCreationTaskType.BaseOn:
                    BaseOnPageType();
                    SetModule();
                    break;
                case ModuleCreationTaskType.SortSetValue:
                    SortSetValuePageType();
                    break;
                case ModuleCreationTaskType.SortDefineValue:
                    SortDefineValuePageType();
                    break;
            }
        }

        private void SortSetValuePageType()
        {
            if (TaskPage.Content is SortSetValuePage sortValuePage)
            {
                var isSortsNeeded = IsNeedInDefineSorts(sortValuePage._name.ID);
                StringBuilder builder = new StringBuilder($"{sortValuePage._name.ID} = ");
                builder.Append("{");
                foreach (var line in sortValuePage.NamesTextBox.Text.Split("\n"))
                {
                    if (line.Length > 0)
                    {
                        builder.Append($"{line.Replace("\r","")}, ");
                        if (isSortsNeeded.Count != 0)
                            foreach (var sort in isSortsNeeded)
                            {
                                if (sort.Value is MainNameValue mainNameValue)
                                {
                                    if (mainNameValue.GetUndefinedType() == UndefinedType.Undefined_Sets)
                                    {
                                        _tasks.Add(new ModuleCreationTask(ModuleCreationTaskType.SortDefineValue));
                                        _taskNames.Add(new BaseNameElement(
                                            NameElementType.MainName, new List<PrefixCouple>(), line, 
                                            new List<ITokenForestNode>(sort.Value.Value)));
                                    }
                                    else
                                    {
                                        _sorts.AppendLine($"Sort {line.Replace("\r", "")}: {sort.Value.ToString()};");
                                    }
                                }
                            }
                        
                    }
                }
                _moduleNamespace.GetLevel($"Уровень {_parent.GetLevel() - 1}").AddContent(
                    $"{builder.ToString().Substring(0, builder.ToString().Length - 2)}" + "};\n"
                    );
            }
        }

        private void SortDefineValuePageType()
        {
            if (TaskPage.Content is SortDefineValuePage sortValuePage)
            {
                _sorts.AppendLine($"Sort {sortValuePage._name.ID.Replace("\r", "")}: {((ITokenForestNode)sortValuePage.TypesComboBox.SelectedItem).Token.Capture.ToString()};");
            }
        }

        private List<BaseNameElement> IsNeedInDefineSorts(string ID)
        {
            var list = new List<BaseNameElement>();
            foreach (var element in _undefinedNames)
            {
                if (element.IsNeedToDefine(ID))
                    list.Add(element);
            }
            return list;
        }

        private void BaseOnPageType()
        {
            if (TaskPage.Content is ParentModulePage parentPage)
            {
                if (parentPage.ModuleList.SelectedItem is LogicModule module)
                {
                    var parse = module.Name.Split("|");
                    var @namespace = ClientConfig.Project.GetNamespace(parse[0]);
                    _parent = @namespace.GetLevel(parse[1]);

                    _moduleNamespace = new LogicModuleNamespace(parse[0]);
                    _moduleNamespace.AddLevel(new LogicModule($"Уровень {_parent.GetLevel() - 1}"));
                    _moduleNamespace.GetLevel($"Уровень {_parent.GetLevel() - 1}").SetContent(
                        $"Module {parse[0]}: {_parent.GetLevel() - 1};\n" +
                        $"Base {parse[0]}_{_parent.GetLevel()};\n" +
                        $"Begin\n"
                        );
                }
            }
        }

        private void SetModule()
        {
            foreach (var task in _undefinedNames)
            {
                if (task.Prefix.PrefixCouples.Count == 0)
                {
                    _tasks.Add(new ModuleCreationTask(ModuleCreationTaskType.SortSetValue));
                    _taskNames.Add(task);
                }
                else
                {
                    
                    //_tasks.Add(new ModuleCreationTask(ModuleCreationTaskType.SortSetName));
                }
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Collapsed;
        }
    }
}
