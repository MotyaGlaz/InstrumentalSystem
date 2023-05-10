using Library.Analyzer.Captures;
using Library.Analyzer.Grammars;

namespace Library.Analyzer.Tokens
{
    public interface ILexemeFactory
    {
        LexerRuleType LexerRuleType { get; }
                
        ILexeme Create(ILexerRule lexerRule, ICapture<char> segment, int offset);

        void Free(ILexeme lexeme);
    }
}