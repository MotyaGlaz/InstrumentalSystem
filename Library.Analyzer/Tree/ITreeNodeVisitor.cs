namespace Library.Analyzer.Tree
{
    public interface ITreeNodeVisitor
    {
        void Visit(ITokenTreeNode node);

        void Visit(IInternalTreeNode node);
    }
}