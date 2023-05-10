using Library.Analyzer.Grammars;

namespace Library.Analyzer.Tokens
{
    public interface ILexemeFactoryRegistry
    {
        ILexemeFactory Get(LexerRuleType lexerRuleType);

        void Register(ILexemeFactory factory);
    }
}