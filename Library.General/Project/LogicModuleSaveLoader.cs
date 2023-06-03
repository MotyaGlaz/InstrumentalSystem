using System;
using System.IO;
using System.Windows.Automation;

namespace Library.General.Project
{

    public class LogicModuleSaveLoader
    {
        public LogicModuleNamespace Load(string path)
        {
            using var file = File.Open(path, FileMode.Open, FileAccess.Read);
            using var reader = new StreamReader(file);

            var name = reader.ReadLine();
            var content = reader.ReadToEnd();
            LogicModule logicModule = new LogicModule(name, content);

            var temp = content.Substring(7);
            var secondIndex = temp.IndexOf(':');
            var nameNamespace = temp.Substring(0, secondIndex).Replace(" ", "_");
            LogicModuleNamespace logicModuleNamespace = new LogicModuleNamespace(nameNamespace);
            logicModuleNamespace.AddLevel(logicModule);

            return logicModuleNamespace;
        }

        // Старый метод Load 
        // public LogicModule Load(string path)
        // {
        //     using var file = File.Open(path, FileMode.Open, FileAccess.Read);
        //     using var reader = new StreamReader(file);
        //     
        //     string name = reader.ReadLine();
        //     string content = reader.ReadToEnd();
        //
        //     return new LogicModule(name, content);
        // }

        public void Save(string path, LogicModule logicModule)
        {
            string folder = Path.GetDirectoryName(path);
            if (folder == null)
                throw new InvalidOperationException("Некорректный путь до файла сохранения модуля");

            Directory.CreateDirectory(folder);
            using var file = File.Open(path, FileMode.OpenOrCreate, FileAccess.Write);
            using var writer = new StreamWriter(file);
            writer.WriteLine(logicModule.Name);
            writer.WriteLine(logicModule.Content);
        }
    }
}