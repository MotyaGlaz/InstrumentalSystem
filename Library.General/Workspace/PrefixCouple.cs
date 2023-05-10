using Library.Analyzer.Automata;
using Library.Analyzer.Forest;
using System;
using System.Collections.Generic;

namespace Library.General.Workspace
{
    public class PrefixCouple
    {
        public string LeftPart { get; private set; }

        public IValue RightPart { get; private set; }

        public PrefixCouple(ITokenForestNode leftPart, ITokenForestNode rightPart)
        {
            LeftPart = leftPart.Token.Capture.ToString();
            RightPart = new PrefixValue(rightPart);
        }

        public PrefixCouple(string leftPart, List<ITokenForestNode> rightPart)
        {
            LeftPart = leftPart;
            RightPart = new PrefixValue(rightPart);
        }


        public override string ToString()
        {
            return $"({LeftPart} In {RightPart})";
        }

    }
}
