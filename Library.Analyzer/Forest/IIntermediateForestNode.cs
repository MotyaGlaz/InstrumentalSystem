using Library.Analyzer.Grammars;

namespace Library.Analyzer.Forest
{
    public interface IIntermediateForestNode : IInternalForestNode
    {
        IDottedRule DottedRule { get; }
    }
}