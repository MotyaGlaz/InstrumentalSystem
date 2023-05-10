using System.Windows;
using System.Windows.Controls;

namespace Library.InterfaceConnection.Writers
{
    public class ConsoleWriter
    {
        private RichTextBox _console;

        public ConsoleWriter(RichTextBox console)
        {
            _console = console;
        }

        private void WriteErrorException()
        {
            
            _console.AppendText("⚠ Ошибка компиляции ⚠ ::::: ");
        }

        public void WriteError(string error)
        {
            WriteErrorException();
            _console.AppendText($"{error}\n");
        }
    }
}
