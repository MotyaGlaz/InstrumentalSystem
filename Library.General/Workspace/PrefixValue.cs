using Library.Analyzer.Automata;
using Library.Analyzer.Forest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.General.Workspace
{
    public class PrefixValue : IValue
    {
        public IReadOnlyList<ITokenForestNode> Value { get { return _value; } }

        private List<ITokenForestNode> _value;

        public PrefixValue (List<ITokenForestNode> lexemes)
        {
            _value = new List<ITokenForestNode> (lexemes);
        }

        public PrefixValue (ITokenForestNode lexeme)
        {
            _value = new List<ITokenForestNode>();
            _value.Add(lexeme);
        }

        public override string ToString()
        {
            return _value[0].Token.Capture.ToString();
        }

    }
}
