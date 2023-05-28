using System;
using System.IO;

namespace Library.General.Project;

public class LogicModuleSaveLoader
{
    public LogicModule Load(string path)
    {
        using var file = File.Open(path, FileMode.Open, FileAccess.Read);
        using var reader = new StreamReader(file);
        
        string name = reader.ReadLine();
        string content = reader.ReadToEnd();

        return new LogicModule(name, content);
    }

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