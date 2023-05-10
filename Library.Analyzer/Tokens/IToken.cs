using Library.Analyzer.Captures;
using Library.Analyzer.Grammars;
using System.Collections.Generic;

namespace Library.Analyzer.Tokens
{
    public interface IToken
    {
        //string Value { get; }
        ICapture<char> Capture { get; }
        int Position { get; }
        TokenType TokenType { get; }
        IReadOnlyList<ITrivia> LeadingTrivia { get; }
        IReadOnlyList<ITrivia> TrailingTrivia { get; }
    }
}