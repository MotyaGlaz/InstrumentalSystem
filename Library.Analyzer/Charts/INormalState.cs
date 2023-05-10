using Library.Analyzer.Grammars;

namespace Library.Analyzer.Charts
{
    public interface INormalState : IState
    {
        bool IsSource(ISymbol searchSymbol);
    }
}
