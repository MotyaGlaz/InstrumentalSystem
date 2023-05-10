using Library.Analyzer.Automata;
using Library.Analyzer.Forest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.General.Workspace
{
    public class AdditionalNameValue : NameValue
    {
        public AdditionalNameValue(List<ITokenForestNode> dfaLexemes) 
            : base(dfaLexemes)
        {
        }
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            foreach (var elemet in Value)
            {
                builder.Append(elemet.Token.Capture.ToString());
            }
            return builder.ToString();
        }
    }
}
