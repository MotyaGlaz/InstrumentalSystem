﻿using Library.Analyzer.Grammars;
using Library.Analyzer.Utilities;

namespace Library.Analyzer.Charts
{
    public class TransitionState : StateBase, ITransitionState
    {
        public ISymbol Recognized { get; private set; }

        public INormalState Reduction { get; private set; }

        public int Index { get; private set; }

        public ITransitionState NextTransition { get; set; }
                
        public TransitionState(
            ISymbol recognized,
            IState transition,
            INormalState reduction,
            int index)
            : base(transition.DottedRule, transition.Origin)
        {
            Reduction = reduction;
            Recognized = recognized;
            Index = index;
            _hashCode = ComputeHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;
            if (!(obj is TransitionState transitionState))
                return false;

            return GetHashCode() == transitionState.GetHashCode()
                && Recognized.Equals(transitionState.Recognized)
                && Index == transitionState.Index;
        }
        
        private readonly int _hashCode;

        private int ComputeHashCode()
        {
            return HashCode.Compute(
                DottedRule.GetHashCode(),
                Origin.GetHashCode(),
                Recognized.GetHashCode(),
                Reduction.GetHashCode(),
                Index.GetHashCode());
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }

        public override string ToString()
        {
            return $"{Recognized} : {Reduction}";
        }

        public override StateType StateType { get { return StateType.Transitive; } }

        public IState GetTargetState()
        {
            var parameterTransitionStateHasNoParseNode = ParseNode is null;
            if (parameterTransitionStateHasNoParseNode)
                return Reduction;
            return this;
        }
    }
}