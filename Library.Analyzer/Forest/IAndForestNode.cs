using System.Collections.Generic;

namespace Library.Analyzer.Forest
{
    public interface IAndForestNode
    {
        IReadOnlyList<IForestNode> Children { get; }
    }
}