using Library.Analyzer.Grammars;

namespace Library.Analyzer.Automata
{
    public interface IDfaTransition
    {
        IDfaState Target { get; }
        ITerminal Terminal { get; }
    }
}