using Library.Analyzer.Tokens;

namespace Library.Analyzer.Tree
{
    public interface ITokenTreeNode : ITreeNode
    {
        IToken Token { get; }
    }
}