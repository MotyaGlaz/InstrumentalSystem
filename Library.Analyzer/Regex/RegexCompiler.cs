﻿using Library.Analyzer.Automata;

namespace Library.Analyzer.Regex
{
    public class RegexCompiler
    {
        private readonly IRegexToNfa _regexToNfa;
        private readonly INfaToDfa _nfaToDfa;

        public RegexCompiler()
            : this(
                new ThompsonConstructionAlgorithm(),
                new SubsetConstructionAlgorithm())
        {}

        public RegexCompiler(
            IRegexToNfa regexToNfa,
            INfaToDfa nfaToDfa)
        {
            _regexToNfa = regexToNfa;
            _nfaToDfa = nfaToDfa;
        }

        public IDfaState Compile(RegexDefinition regex)
        {
            var nfa = _regexToNfa.Transform(regex);
            var dfa = _nfaToDfa.Transform(nfa);
            return dfa;
        }
    }
}