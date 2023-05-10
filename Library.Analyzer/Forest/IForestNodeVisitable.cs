namespace Library.Analyzer.Forest
{
    public interface IForestNodeVisitable
    {
        void Accept(IForestNodeVisitor visitor);
    }
}