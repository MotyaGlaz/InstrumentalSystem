using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.General.NameTable
{
    public enum NameSearchState
    {
        Wait,
        Main_Name,
        Additional_Name,
        Prefix,
        Expression
    }
}
