using System.Collections.Generic;

namespace Library.Analyzer.Builders
{
    public class AlterationModel
    {
        public IList<SymbolModel> Symbols { get; private set; }

        public AlterationModel()
        {
            Symbols = new List<SymbolModel>();
        }

        public AlterationModel(IEnumerable<SymbolModel> symbols)
        {
            Symbols = new List<SymbolModel>(symbols);
        }
    }
}
