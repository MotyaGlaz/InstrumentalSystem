using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.General.NameTable
{
    public class ProjectNameTable : IProjectNameTable
    {
        public IReadOnlyList<IModuleNameTable> ModuleNameTables { get { return _modules; } }

        public string Name { get; private set; }

        private List<IModuleNameTable> _modules;

        public ProjectNameTable(string name, List<IModuleNameTable> modules)
        {
            Name = name;
            _modules = new List<IModuleNameTable> (modules);
        }

        public ProjectNameTable(string name)
        {
            Name = name;
            _modules = new List<IModuleNameTable>();
        }

        public ProjectNameTable(string name, IModuleNameTable module)
        {
            Name = name;
            _modules = new List<IModuleNameTable>();
            _modules.Add(module);
        }

        public void AddModule(IModuleNameTable module)
        {
            _modules.Add(module);
        }

        public IModuleNameTable GetModule(string name)
        {
            foreach(var module in _modules)
            {
                if (module.Name.Equals(name))
                    return module;
            }
            return default(IModuleNameTable);
        }
    }
}
