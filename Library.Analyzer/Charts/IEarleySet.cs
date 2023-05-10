﻿using Library.Analyzer.Grammars;
using System.Collections.Generic;

namespace Library.Analyzer.Charts
{
    public interface IEarleySet
    {
        IReadOnlyList<INormalState> Predictions { get; }

        IReadOnlyList<INormalState> Scans { get; }

        IReadOnlyList<INormalState> Completions { get; }

        IReadOnlyList<ITransitionState> Transitions { get; }

        bool Enqueue(IState state);

        int Location { get; }

        ITransitionState FindTransitionState(ISymbol searchSymbol);
        
        INormalState FindSourceState(ISymbol searchSymbol);
    }
}