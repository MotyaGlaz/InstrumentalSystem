using Library.General.Workspace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.General.NameTable
{
    public interface INameElement
    {
        string ID { get; }
        NameElementType NameElementType { get; }
        IValue Value { get; }
        IPrefix Prefix { get; }

    }
}
