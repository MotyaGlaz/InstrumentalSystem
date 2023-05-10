using Library.Analyzer.Tokens;

namespace Library.Analyzer.Forest
{
    public interface ITokenForestNode : IForestNode
    {
        IToken Token { get; }
    }
}