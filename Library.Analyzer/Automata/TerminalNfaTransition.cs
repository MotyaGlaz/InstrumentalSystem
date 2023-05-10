using Library.Analyzer.Grammars;

namespace Library.Analyzer.Automata
{
    public class TerminalNfaTransition : INfaTransition
    {
        public INfaState Target { get; private set; }

        public ITerminal Terminal { get; private set; }

        public NfaTransitionType TransitionType { get { return NfaTransitionType.Edge; } }

        public TerminalNfaTransition(ITerminal terminal, INfaState target)
        {
            Terminal = terminal;
            Target = target;
        }
    }
}