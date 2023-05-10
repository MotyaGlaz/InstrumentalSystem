using Library.Analyzer.Automata;
using Library.Analyzer.Forest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.General.Workspace
{
    public class MainNameValue : NameValue
    {
        public MainNameValue(List<ITokenForestNode> dfaLexemes) 
            : base(dfaLexemes)
        {
        }

        public UndefinedType GetUndefinedType()
        {
            foreach(var element in Value)
            {
                if (element.Token.TokenType.Id.Equals("OR"))
                {
                    return UndefinedType.Undefined_Sets;
                }
            }
            foreach (var element in Value)
            {
                if (element.Token.TokenType.Id.Equals("STRING"))
                {
                    return UndefinedType.Set_String;
                }
            }
            return UndefinedType.None;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            foreach (var elemet in Value)
            {
                builder.Append($" {elemet.Token.Capture.ToString()}");
            }
            return builder.ToString();
        }
    }
}
