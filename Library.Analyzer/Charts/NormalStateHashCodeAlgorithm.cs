using Library.Analyzer.Grammars;
using Library.Analyzer.Utilities;

namespace Library.Analyzer.Charts
{
    static class NormalStateHashCodeAlgorithm
    {
        public static int Compute(IDottedRule dottedRule, int origin)
        {
            return HashCode.Compute(
                dottedRule.GetHashCode(),
                origin.GetHashCode());
        }
    }
}
