using System;
using InstrumentalSystem.Client.Logic.Config;
using InstrumentalSystem.Client.View.Additional;
using InstrumentalSystem.Client.View.Modal;
using Library.Analyzer.Forest;
using Library.Analyzer.PDL;
using Library.Analyzer.Runtime;
using Library.General.NameTable;
using Library.General.Project;
using Library.InterfaceConnection.Writers;
using Library.IOSystem.Reader;
using Library.IOSystem.Writer;
using Library.NextLevelGenerator.Creators;
using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace InstrumentalSystem.Client.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Editor : Window
    {

        private ParseEngine? _engine = default(ParseEngine);
        private IInternalForestNode? _root = default(IInternalForestNode);
        private Project? _project = default(Project);
        private IModuleNameTable _nameTable;
        private LogicModule? _selectedModule;

        public Editor(string path)
        {
            InitializeComponent();
            var pathParser = path.Split("\\");
            ClientConfig.Project = ProjectReader.ReadProject($"{path}\\{pathParser[pathParser.Length-1]}.master");
            _project = ClientConfig.Project;
            ProjectNameLabel.Content = _project.Name;
            tvLogicModules.ItemsSource = ClientConfig.Project.Namespaces;
            CodeEditor.IsEnabled = false;
        }

        public void TreeRefresh()
        {
            tvLogicModules.Items.Refresh();
        }

        private void InitializeEngine()
        {
            var path = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "grammar.pdl");
            var content = File.ReadAllText(path);

            // parse the grammar definition file
            var pdlParser = new PdlParser();
            var definition = pdlParser.Parse(content);

            // create the grammar, parser and scanner for our calculator language
            var grammar = new PdlGrammarGenerator().Generate(definition);
            _engine = new ParseEngine(grammar);
        }

        private string RichTextBoxText(TextBox textBox)
            => textBox.Text;


        private void CompileButton_Click(object sender, RoutedEventArgs e)
        {
            Console.Document.Blocks.Clear();
            InitializeEngine();
            var scanner = new ParseRunner(_engine, RichTextBoxText(CodeEditor), new ConsoleWriter(Console));
            var recognized = false;
            var errorPosition = 0;
            while (!scanner.EndOfStream())
            {
                recognized = scanner.Read();
                if (!recognized)
                {
                    errorPosition = scanner.Position;
                    break;
                }
            }
            var accepted = false;
            if (recognized)
            {
                accepted = scanner.ParseEngine.IsAccepted();
                if (!accepted)
                    errorPosition = scanner.Position;
            }
            if (recognized && accepted)
                _root = _engine?.GetParseForestRootNode();
            if (Console.Document.Blocks.Count > 0)
                return;
            var log = new NameSearchForestNodeVisitor(new TextBoxWriter(Console));
            
            if (_root is null) 
                return;
            log.Visit((SymbolForestNode)_root);
            _nameTable = log._nameTable;
            // Разобраться с 103 строчкой Console.AppendText(log._nameTable.ToString() + "\n");
            Console.AppendText("Успешная компиляция");
        }

        private void CodeEditor_TextChanged(object sender, TextChangedEventArgs e)
        {
            AttachedProperties.box_SizeChanged(sender, null);
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue is LogicModule module)
            {
                if (CodeEditor.IsEnabled)
                    if(e.OldValue is LogicModule oldModule)
                        oldModule.SetContent(CodeEditor.Text);
                CodeEditor.Clear();
                CodeEditor.AppendText(module.Content);
                SelectedModuleNameLabel.Content = module.Name;
                CodeEditor.IsEnabled = true;
                _selectedModule = module;
            }
        }

        private void CreateProjectButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (var @namespace in _project.Namespaces)
            {
                foreach(var level in @namespace.Levels)
                {
                    level.SetContent(level.Name + "asdas ::: Test");
                }
            }
            var project = new ProjectWriter(System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Project"));
            project.WriteProject(_project);
        }

        private void CreateModuleButton_Click(object sender, RoutedEventArgs e)
        {
            if (_nameTable is null)
                ContentGrid.Children.Add(new ModuleCreationModal(this, default(ModuleNameTable)));
            else
            {
                ContentGrid.Children.Add(new ModuleCreationModal(this, _nameTable));
            }
        }

        private void OpenProjectButton_Click(object sender, RoutedEventArgs e)
        {
            var filePath = string.Empty;
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                RestoreDirectory = true,
                Title = "Выбор проекта",
                Filter = "Master files (*.master)|*.master"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                filePath = openFileDialog.FileName;
            }
        }

        private void SaveModuleButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedModule == null)
            {
                MessageBox.Show("Не выбран модуль для сохранения");
                return;
            }
            
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                RestoreDirectory = true,
                Title = "Сохранение модуля",
                Filter = "Master file (*.master)|*.master"
            };
            saveFileDialog.ShowDialog();
            if (saveFileDialog.FileName != "")
            {
                try
                {
                    var saveLoader = new LogicModuleSaveLoader();
                    saveLoader.Save(saveFileDialog.FileName, _selectedModule);
                }
                catch(Exception ex)
                {
                    MessageBox.Show("Ошибка при сохранение модуля. " + ex.Message);
                }
            }
        }

        private void SaveProjectButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ImportModuleButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                RestoreDirectory = true,
                Title = "Импорт модуля",
                Filter = "Master file (*.master)|*.master"
            };
            openFileDialog.ShowDialog();
            if (openFileDialog.FileName != "")
            {
                try
                {
                    var saveLoader = new LogicModuleSaveLoader();
                    LogicModuleNamespace logicModuleNamespace = saveLoader.Load(openFileDialog.FileName);
                    ClientConfig.Project.Add(logicModuleNamespace);
                }
                catch(Exception ex)
                {
                    MessageBox.Show("Ошибка при импортировании модуля. " + ex.Message);
                }
            }
        }
    }
}
