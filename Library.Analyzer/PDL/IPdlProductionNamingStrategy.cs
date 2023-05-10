using Library.Analyzer.Grammars;

namespace Library.Analyzer.PDL
{
    public interface IPdlProductionNamingStrategy
    {
        INonTerminal GetSymbolForRepetition(PdlFactorRepetition repetition);
        INonTerminal GetSymbolForOptional(PdlFactorOptional optional);        
    }
}
