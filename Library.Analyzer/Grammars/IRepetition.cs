using System.Collections.Generic;

namespace Library.Analyzer.Grammars
{
    public interface IRepetition : ISymbol
    {
        IReadOnlyList<ISymbol> Items { get; }
    }
}
