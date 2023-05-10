﻿using System.Collections.Generic;
using Library.Analyzer.Collections;
using System;

namespace Library.Analyzer.Automata
{
    public class NfaState : INfaState, IComparable<INfaState>, IComparable
    {
        private readonly List<INfaTransition> _transitions;

        public NfaState()
        {
            _transitions = new List<INfaTransition>();
        }

        public IReadOnlyList<INfaTransition> Transitions
        {
            get { return _transitions; }
        }

        public void AddTransistion(INfaTransition transition)
        {
            _transitions.Add(transition);
        }

        public IEnumerable<INfaState> Closure()
        {
            // the working queue used to process states 
            var queue = new ProcessOnceQueue<INfaState>();
            
            // initialize by adding the curren state (this)
            queue.Enqueue(this);

            // loop over items in the queue, adding newly discovered
            // items after null transitions
            while (queue.Count != 0)
            {
                var state = queue.Dequeue();
                for (var t = 0; t < state.Transitions.Count; t++)
                {
                    var transition = state.Transitions[t];
                    if (transition.TransitionType == NfaTransitionType.Null)
                        queue.Enqueue(transition.Target);
                }
            }

            return queue.Visited;
        }

        public int CompareTo(object obj)
        {
            if (obj is null)
                throw new NullReferenceException();
            if (!(obj is INfaState nfaState))
                throw new ArgumentException("parameter must be a INfaState", nameof(obj));
            return CompareTo(nfaState);
        }

        public int CompareTo(INfaState other)
        {
            return GetHashCode().CompareTo(other.GetHashCode());
        }
    }    
}