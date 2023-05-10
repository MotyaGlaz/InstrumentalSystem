namespace Library.Analyzer.Automata
{
    public interface INfaToDfa
    {
        IDfaState Transform(INfa nfa);
    }
}