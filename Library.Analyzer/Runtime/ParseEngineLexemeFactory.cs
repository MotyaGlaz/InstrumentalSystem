﻿using Library.Analyzer.Captures;
using Library.Analyzer.Grammars;
using Library.Analyzer.Tokens;
using System;
using System.Collections.Generic;

namespace Library.Analyzer.Runtime
{
    public class ParseEngineLexemeFactory : ILexemeFactory
    {
        private Queue<ParseEngineLexeme> _queue;

        public LexerRuleType LexerRuleType { get { return GrammarLexerRule.GrammarLexerRuleType; } }

        public ParseEngineLexemeFactory()
        {
            _queue = new Queue<ParseEngineLexeme>();
        }

        public void Free(ILexeme lexeme)
        {
            if (!(lexeme is ParseEngineLexeme parseEngineLexeme))
                throw new Exception($"Unable to free lexeme of type {lexeme.GetType()} from ParseEngineLexeme.");
            _queue.Enqueue(parseEngineLexeme);
        }

        public ILexeme Create(ILexerRule lexerRule, ICapture<char> segment, int offset)
        {
            if (lexerRule.LexerRuleType != LexerRuleType)
                throw new Exception(
                    $"Unable to create ParseEngineLexeme from type {lexerRule.GetType().FullName}. Expected TerminalLexerRule");

            var grammarLexerRule = lexerRule as IGrammarLexerRule;

            if (_queue.Count == 0)
                return new ParseEngineLexeme(grammarLexerRule, segment, offset);

            var reusedLexeme = _queue.Dequeue();
            reusedLexeme.Reset(grammarLexerRule, offset);
            return reusedLexeme;
        }
    }
}