using Library.Analyzer.Captures;
using Library.Analyzer.Grammars;

namespace Library.Analyzer.Tokens
{
    public interface ITrivia
    {
        int Position { get; }
        ICapture<char> Capture { get; }
        TokenType TokenType { get; }
    }
}
