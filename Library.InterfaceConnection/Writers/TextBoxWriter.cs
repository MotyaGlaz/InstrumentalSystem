using System.Windows.Controls;

namespace Library.InterfaceConnection.Writers
{
    public class TextBoxWriter
    {
        private RichTextBox _console;

        public TextBoxWriter(RichTextBox console)
        {
            _console = console;
        }

        public void Write(string text)
        {
            _console.AppendText($"{text}");
        }

        public void WriteLine(string text)
        {
            _console.AppendText($"{text}\n");
        }
        public void WriteLine()
        {
            _console.AppendText($"\n");
        }
    }
}
