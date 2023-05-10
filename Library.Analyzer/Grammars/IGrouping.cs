using System.Collections.Generic;

namespace Library.Analyzer.Grammars
{
    public interface IGrouping : ISymbol
    {
        IReadOnlyList<ISymbol> Items { get; }
    }
}