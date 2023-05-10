using Library.Analyzer.Forest;
using Library.Analyzer.Grammars;

namespace Library.Analyzer.Charts
{
    public interface IState
    {
        IDottedRule DottedRule { get; }

        int Origin { get; }

        StateType StateType { get; }        
                
        IForestNode ParseNode { get; set; }
    }
}