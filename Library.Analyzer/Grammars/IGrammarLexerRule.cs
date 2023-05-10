namespace Library.Analyzer.Grammars
{
    public interface IGrammarLexerRule : ILexerRule
    {
        IGrammar Grammar { get; }
    }
}