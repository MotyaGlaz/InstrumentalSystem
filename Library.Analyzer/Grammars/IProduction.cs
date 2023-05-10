using System.Collections.Generic;

namespace Library.Analyzer.Grammars
{
    public interface IProduction
    {
        INonTerminal LeftHandSide { get; }

        IReadOnlyList<ISymbol> RightHandSide { get; }

        bool IsEmpty { get; }
    }
}