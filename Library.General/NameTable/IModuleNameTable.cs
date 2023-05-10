using System.Collections.Generic;

namespace Library.General.NameTable
{
    public interface IModuleNameTable : ITable
    {
        IReadOnlyList<BaseNameElement> Elements { get; }
    }
}
