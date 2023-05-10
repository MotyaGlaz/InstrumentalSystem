namespace Library.Analyzer.Grammars
{
    public interface INonTerminal : ISymbol
    {
        string Value { get; }
        FullyQualifiedName FullyQualifiedName { get; }
    }
}