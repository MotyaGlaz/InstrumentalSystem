namespace Library.Analyzer.Grammars
{
    public interface ITerminalLexerRule : ILexerRule
    {
        ITerminal Terminal { get; }
    }
}