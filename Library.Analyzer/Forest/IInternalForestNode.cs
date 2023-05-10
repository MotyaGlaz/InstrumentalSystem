using System.Collections.Generic;

namespace Library.Analyzer.Forest
{
    /// <summary>
    /// Represents a Disjuncion of IAndNodes
    /// </summary>
    public interface IInternalForestNode : IForestNode
    {
        IReadOnlyList<IAndForestNode> Children { get; }

        void AddUniqueFamily(IForestNode trigger);

        void AddUniqueFamily(IForestNode source, IForestNode trigger);
    }
}