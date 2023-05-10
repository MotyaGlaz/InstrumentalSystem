using Library.Analyzer.Grammars;

namespace Library.Analyzer.Forest
{
    public interface ISymbolForestNode : IInternalForestNode
    {
        ISymbol Symbol { get; }
    }
}