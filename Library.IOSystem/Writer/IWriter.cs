using Library.IOSystem.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.IOSystem.Writer
{
    public interface IWriter : IPathable
    {
        void Write(string value);

        void WriteLine(string value);
    }
}
