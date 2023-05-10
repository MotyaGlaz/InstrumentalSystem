using Library.General.Collections;
using Library.General.Project;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.General.Project
{
    public class Project
    {
        public ObservableCollection<LogicModuleNamespace> Namespaces { get; private set; }

        public string Name { get; private set; }

        public Project(string name)
        {
            Name = name;
            Namespaces = new ObservableCollection<LogicModuleNamespace>();
        }

        public void Add(LogicModuleNamespace @namespace)
        {
            Namespaces.Add(@namespace);
        }

        public void Add(string name)
        {
            Namespaces.Add(new LogicModuleNamespace(name));
        }
        
        public void Add(string @namespace, LogicModule logicModule)
        {
            var space = GetNamespace(@namespace);
            if(space is default(LogicModuleNamespace))
            {
                Add(@namespace);
                GetNamespace(@namespace).AddLevel(logicModule);
            }
            else
            {
                space.AddLevel(logicModule);
            }
        }

        public LogicModuleNamespace? GetNamespace(string @namespace)
        {
            foreach (var element in Namespaces)
                if (element.Name.Equals(@namespace))
                    return element;
            return default;
        }

        public bool Remove(string @namespace)
        {
            return Namespaces.Remove(new LogicModuleNamespace(@namespace));
        }

        public bool Remove(LogicModuleNamespace @namespace)
        {
            return Namespaces.Remove(@namespace);
        }

    }
}
