using Library.Analyzer.Automata;
using Library.Analyzer.Forest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.General.Workspace
{
    public abstract class NameValue : IValue
    {
        public IReadOnlyList<ITokenForestNode> Value { get { return _value; } }

        private List<ITokenForestNode> _value;

        public NameValue(List<ITokenForestNode> lexemes)
        {
            _value = new List<ITokenForestNode>(lexemes);
        }

        public NameValue(ITokenForestNode lexeme)
        {
            _value = new List<ITokenForestNode>();
            _value.Add(lexeme);
        }
    }
}
