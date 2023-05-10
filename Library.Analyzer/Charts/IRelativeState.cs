using Library.Analyzer.Grammars;

namespace Library.Analyzer.Charts
{
    interface IRelativeState
    {
        IDottedRule DottedRule { get; }
        int Offset { get; }        
    }
}
