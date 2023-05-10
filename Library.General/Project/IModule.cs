using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.General.Project
{
    public interface IModule
    {
        string Name { get; }

        string Content { get; }
    }
}
