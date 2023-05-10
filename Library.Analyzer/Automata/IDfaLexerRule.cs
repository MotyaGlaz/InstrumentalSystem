using Library.Analyzer.Grammars;

namespace Library.Analyzer.Automata
{
    public interface IDfaLexerRule : ILexerRule
    {
        IDfaState Start { get; }
    }
}