using Library.Analyzer.Grammars;

namespace Library.Analyzer.Automata
{
    public class DfaTransition : IDfaTransition
    {
        public IDfaState Target { get; private set; }
        public ITerminal Terminal { get; private set; }

        public DfaTransition(ITerminal terminal, IDfaState target)
        {
            Target = target;
            Terminal = terminal;
        }
    }
}