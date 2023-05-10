using System.Collections.Generic;

namespace Library.Analyzer.Grammars
{
    public interface IOptional : ISymbol
    {
        IReadOnlyList<ISymbol> Items { get; }
    }
}
