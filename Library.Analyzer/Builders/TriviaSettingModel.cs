﻿using Library.Analyzer.Grammars;

namespace Library.Analyzer.Builders
{
    public class TriviaSettingModel : SettingModel
    {
        public const string SettingKey = "trivia";

        public TriviaSettingModel(LexerRuleModel lexerRuleModel)
            : base(SettingKey, lexerRuleModel.Value.TokenType.Id)
        { }

        public TriviaSettingModel(FullyQualifiedName fullyQualifiedName)
            : base(SettingKey, fullyQualifiedName.FullName)
        {
        }
    }
}
