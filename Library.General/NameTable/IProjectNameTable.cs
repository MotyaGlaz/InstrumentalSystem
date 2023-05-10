using System.Collections.Generic;

namespace Library.General.NameTable
{
    public interface IProjectNameTable : ITable
    {
        
        IReadOnlyList<IModuleNameTable> ModuleNameTables { get; }
    }
}
