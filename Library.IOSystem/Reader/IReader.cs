using Library.IOSystem.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.IOSystem.Reader
{
    public interface IReader : IPathable
    {
        string ReadAll();

    }
}
