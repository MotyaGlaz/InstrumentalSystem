using Library.Analyzer.Grammars;
using System.Collections.Generic;

namespace Library.Analyzer.Automata
{
    public interface IDfaState
    {
        bool IsFinal { get; }

        IReadOnlyList<IDfaTransition> Transitions { get; }

        void AddTransition(IDfaTransition edge);
        void AddTransition(ITerminal terminal, IDfaState node);
    }
}