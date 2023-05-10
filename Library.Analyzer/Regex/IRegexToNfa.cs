using Library.Analyzer.Automata;

namespace Library.Analyzer.Regex
{
    public interface IRegexToNfa
    {
        INfa Transform(RegexDefinition regex);
    }
}