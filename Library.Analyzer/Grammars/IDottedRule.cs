using System;

namespace Library.Analyzer.Grammars
{
    public interface IDottedRule : IComparable<IDottedRule>
    {
        int Position { get; }

        IProduction Production { get; }

        bool IsComplete { get; }

        ISymbol PreDotSymbol { get; }

        ISymbol PostDotSymbol { get; }
    }
}