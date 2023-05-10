using System.Collections.Generic;

namespace Library.Analyzer.Grammars
{
    public interface ITerminal : ISymbol
    {
        bool IsMatch(char character);

        IReadOnlyList<Interval> GetIntervals();
    }
}