using Library.Analyzer.Grammars;
using System.Collections.Generic;

namespace Library.Analyzer.Tree
{
    public interface IInternalTreeNode : ITreeNode
    {
        INonTerminal Symbol { get; }
        IReadOnlyList<ITreeNode> Children { get; }
    }
}