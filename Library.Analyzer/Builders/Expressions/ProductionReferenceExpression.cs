using Library.Analyzer.Grammars;

namespace Library.Analyzer.Builders.Expressions
{
    public class ProductionReferenceExpression : BaseExpression
    {
        public ProductionReferenceModel ProductionReferenceModel { get; private set; }

        public ProductionReferenceExpression(IGrammar grammar)
        {
            ProductionReferenceModel = new ProductionReferenceModel(grammar);
        }
    }
}
