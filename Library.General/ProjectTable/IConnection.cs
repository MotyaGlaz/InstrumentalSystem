using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.General.ProjectTable
{
    public interface IConnection
    {
        string Module { get; }

        ConnectionType ConnectionType { get; }

        string ConnectedModule { get; }
    }
}
