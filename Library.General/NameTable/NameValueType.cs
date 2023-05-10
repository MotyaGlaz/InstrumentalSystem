using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.General.NameTable
{
    public enum NameValueType
    {
        StringSet,
        IntSet,
        RealSet,
        String,
        Int,
        Real,
        IntInterval,
        RealInterval,
        NameSpaceString,
        NameSpaceInt,
        NameSpaceReal,
        Bool, 
        Unknown
    }
}
