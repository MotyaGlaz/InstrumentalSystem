using Library.General.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.General.ProjectTable
{
    public class ProjectTable : IProjectTable
    {
        public string ProjectName { get; private set; }

        private HashSet<string> _modules;
        private UniqueSet<IConnection> _connections;

        public ProjectTable(string name)
        {
            ProjectName = name;
            _modules = new HashSet<string>();
            _connections = new UniqueSet<IConnection>();
        }

        // сообщение о связи с другим модулем
        public void AddConnection(string module, ConnectionType connection, string connectedModule)
        {
            if (module.Equals(connectedModule))
                return;
            _modules.Add(module);
            if (!_connections.Add(new Connection(module, connection, connectedModule)))
                return;
            if (_modules.Contains(connectedModule))
                foreach (var cross in SearchCrossConnection(module, connectedModule))
                    AddConnection(module, ConnectionType.Uses, cross.ConnectedModule);
            else
                _modules.Add(connectedModule);
        }

        // проверить перекрестные ссылки
        public void Union(ProjectTable projectTable)
        {
            _modules.UnionWith(projectTable._modules);
            _connections.UnionWith(projectTable._connections);
        }

        public bool Remove(string module)
        {
            List<IConnection> connections = SearchConnections(module);
            List<IConnection> dependancies = SearchDependancy(module);
            foreach (var connection in connections)
                _connections.Remove(connection);
            foreach (var dependancy in dependancies)
                _connections.Remove(dependancy);
            return _modules.Remove(module);
        }

        public bool Remove(string module, string connectedModule)
        {
            List<IConnection> connections = SearchConnections(module);
            foreach (var connection in connections)
            {
                if (connection.ConnectedModule.Equals(connectedModule))
                    return _connections.Remove(connection);
            }
            return false;
        }

        private List<IConnection> SearchCrossConnection(string module, string connectedModule)
        {
            List<IConnection> connections = SearchUsesConnections(connectedModule);
            List<IConnection> dependancies = SearchUsesDependancy(module);
            if (connections.Count == 0)
                return dependancies;
            List<IConnection> crosses = new List<IConnection>();
            foreach (var connection in connections)
                if (!dependancies.Contains(connection))
                    crosses.Add(connection);
            return crosses;
        }

        private List<IConnection> SearchConnections(string module)
        {
            List<IConnection> connections = new List<IConnection>();
            foreach (var connection in _connections)
            {
                if (connection.Module.Equals(module))
                    connections.Add(connection);
            }
            return connections;
        }

        private List<IConnection> SearchDependancy(string module)
        {
            List<IConnection> dependancies = new List<IConnection>();
            foreach(var connection in _connections)
            {
                if (connection.ConnectedModule.Equals(module))
                    dependancies.Add(connection);
            }
            return dependancies;
        }

        private List<IConnection> SearchUsesConnections(string module)
        {
            List<IConnection> connections = SearchConnections(module);
            List<IConnection> result = new List<IConnection>();
            foreach (var connection in connections)
            {
                if (connection.ConnectionType == ConnectionType.Uses)
                    result.Add(connection);
            }
            return result;
        }

        private List<IConnection> SearchUsesDependancy(string module)
        {
            List<IConnection> dependencies = SearchDependancy(module);
            List<IConnection> result = new List<IConnection>();
            foreach (var dependancy in dependencies)
            {
                if (dependancy.ConnectionType == ConnectionType.Uses)
                    result.Add(dependancy);
            }
            return result;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder($"{ProjectName}");
            builder.AppendLine("__");
            foreach (var connection in _connections)
                builder.AppendLine(connection.ToString());
            builder.AppendLine("--");
            return builder.ToString();
        }
    }
}
