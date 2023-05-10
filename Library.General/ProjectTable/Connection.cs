using Library.General.Utilities;
using System.Text;

namespace Library.General.ProjectTable
{
    public class Connection : IConnection
    {
        public string Module { get; private set; }

        public ConnectionType ConnectionType { get; private set; }

        public string ConnectedModule { get; private set; }


        private int _hashCode;

        public Connection(string module, ConnectionType connection, string connectedModule)
        {
            Module = module;
            ConnectionType = connection;
            ConnectedModule = connectedModule;
            _hashCode = ComputeHashCode();
        }

        private int ComputeHashCode() => HashCode.Compute(Module.GetHashCode(), ConnectedModule.GetHashCode());

        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;
            if (!(obj is Connection other))
                return false;
            if (GetHashCode() == other.GetHashCode()
                || (Module.Equals(other.ConnectedModule) && ConnectedModule.Equals(other.Module)))
                return true;
            return false;

        }

        public static bool operator ==(Connection left, Connection right) => left.Equals(right);
        public static bool operator !=(Connection left, Connection right) => !left.Equals(right);
        

        public override int GetHashCode()
        {
            return _hashCode;
        }

        public override string ToString()
        {
            return $"   |-{Module} ::: {ConnectionType.ToString()} ::: {ConnectedModule}";
        }

    }
}
