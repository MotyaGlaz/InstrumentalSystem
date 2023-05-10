namespace Library.Analyzer.Grammars
{
    public interface IDottedRuleRegistry : IReadOnlyDottedRuleRegistry
    {
        void Register(IDottedRule dottedRule);
        IDottedRule GetNext(IDottedRule dottedRule);
    }
}