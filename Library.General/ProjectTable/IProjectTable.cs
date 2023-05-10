using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.General.ProjectTable
{
    public interface IProjectTable
    {
        string ProjectName { get; }

        void AddConnection(string module, ConnectionType connection, string connectedModule);
        
        void Union(ProjectTable projectTable);

        bool Remove(string module);

        bool Remove(string module, string connectedModule);

    }
}
