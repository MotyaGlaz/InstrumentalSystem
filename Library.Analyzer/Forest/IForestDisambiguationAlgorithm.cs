namespace Library.Analyzer.Forest
{
    public interface IForestDisambiguationAlgorithm
    {
        IAndForestNode GetCurrentAndNode(IInternalForestNode internalNode);
    }
}