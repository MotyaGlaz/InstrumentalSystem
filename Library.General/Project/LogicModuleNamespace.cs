using Library.General.Collections;
using Library.General.Utilities;
using System.Collections.ObjectModel;

namespace Library.General.Project
{
    public class LogicModuleNamespace
    {
        public string Name { get; private set; }

        public ObservableCollection<LogicModule> Levels { get; private set; }

        private int _hashCode;

        public LogicModuleNamespace(string @namespace, ObservableCollection<LogicModule> levels)
        {
            Name = @namespace;
            Levels = levels;
            _hashCode = ComputeHashCode();
        }

        public LogicModuleNamespace(string @namespace)
        {
            Name = @namespace;
            Levels = new ObservableCollection<LogicModule>();
            _hashCode = ComputeHashCode();
        }

        public LogicModule? GetLevel(string name)
        {
            foreach(var level in Levels) 
                if (level.Name.Equals(name))
                    return level;
            return default;
        }

        public void Rename(string @namespace)
        {
            Name = @namespace;
        }

        public void SetLevels(ObservableCollection<LogicModule> levels)
        {
            Levels = levels;
        }

        public void AddLevel(LogicModule module)
        {
            Levels.Add(module);
        }
        public void AddLevel(string module)
        {
            Levels.Add(new LogicModule(module));
        }

        public void DeleteLevel(LogicModule module)
        {
            Levels.Remove(module);
        }

        public void DeleteLevel(string module)
        {
            Levels.Remove(new LogicModule(module));
        }

        public override int GetHashCode() => _hashCode;

        private int ComputeHashCode() => HashCode.Compute(Name.GetHashCode());
    }
}
