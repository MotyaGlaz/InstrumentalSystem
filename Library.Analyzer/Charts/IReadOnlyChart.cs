using System.Collections.Generic;

namespace Library.Analyzer.Charts
{
    public interface IReadOnlyChart
    {
        IReadOnlyList<IEarleySet> EarleySets { get; }
        int Count { get; }
    }
}