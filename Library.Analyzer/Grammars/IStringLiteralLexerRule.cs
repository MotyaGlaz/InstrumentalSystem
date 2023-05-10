namespace Library.Analyzer.Grammars
{
    public interface IStringLiteralLexerRule : ILexerRule
    {
        string Literal { get; }
    }
}