using Library.Analyzer.Diagnostics;
using Library.Analyzer.Forest;
using Library.Analyzer.Grammars;

namespace Library.Analyzer.Charts
{
    public abstract class StateBase : IState
    {
        public IDottedRule DottedRule { get; private set; }

        public int Origin { get; private set; }
        
        public abstract StateType StateType { get; }

        public IForestNode ParseNode { get; set; }
        
        protected StateBase(IDottedRule dottedRule, int origin)
        {
            Assert.IsNotNull(dottedRule, nameof(dottedRule));
            Assert.IsGreaterThanEqualToZero(origin, nameof(origin));
            DottedRule = dottedRule;
            Origin = origin;
        }
    }
}
