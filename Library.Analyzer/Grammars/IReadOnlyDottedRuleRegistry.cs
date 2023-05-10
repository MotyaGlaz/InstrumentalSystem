namespace Library.Analyzer.Grammars
{
    public interface IReadOnlyDottedRuleRegistry
    {
        IDottedRule Get(IProduction production, int position);
    }
}
