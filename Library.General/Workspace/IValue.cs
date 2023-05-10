using Library.Analyzer.Automata;
using Library.Analyzer.Forest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.General.Workspace
{
    public interface IValue
    {
        IReadOnlyList<ITokenForestNode> Value { get; } 
    }
}
