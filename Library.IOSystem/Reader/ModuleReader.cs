using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.IOSystem.Reader
{
    public class ModuleReader : IReader
    {
        public string Path { get; private set; }

        public ModuleReader(string path)
        {
            Path = path;
        }

        public string ReadAll() => File.ReadAllText(Path);
        
    }
}
