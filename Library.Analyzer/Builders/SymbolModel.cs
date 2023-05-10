using Library.Analyzer.Grammars;

namespace Library.Analyzer.Builders
{
    public abstract class SymbolModel
    {
        public abstract SymbolModelType ModelType { get; }

        public abstract ISymbol Symbol { get; }
    }
}