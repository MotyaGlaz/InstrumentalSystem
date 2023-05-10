using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.IOSystem.Writer
{
    public class Writer : IWriter
    {
        public string Path { get; private set; }

        public Writer(string path)
        {
            Path = path;
        }

        public void ClearText() =>
            File.WriteAllText(Path,"");


        public void WriteNewText(string text) =>
            File.WriteAllText(Path,text);

        public void Write(string text) =>
            File.AppendAllText(Path, text);

        public void WriteLine(string text) =>
            File.AppendAllText(Path, $"{text}\n");


    }
}
