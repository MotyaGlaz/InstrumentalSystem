﻿using Library.Analyzer.Captures;
using Library.Analyzer.Grammars;
using System;
using System.Collections.Generic;

namespace Library.Analyzer.Tokens
{
    public class TerminalLexemeFactory : ILexemeFactory
    {
        public LexerRuleType LexerRuleType { get { return TerminalLexerRule.TerminalLexerRuleType; } }
        private Queue<TerminalLexeme> _queue;

        public TerminalLexemeFactory()
        {
            _queue = new Queue<TerminalLexeme>();
        }

        public void Free(ILexeme lexeme)
        {
            if (!(lexeme is TerminalLexeme terminalLexeme))
                throw new Exception($"Unable to free lexeme of type { lexeme.GetType()} from TerminalLexemeFactory");
            
            _queue.Enqueue(terminalLexeme);
        }

        public ILexeme Create(ILexerRule lexerRule, ICapture<char> segment, int offset)
        {
            if (!LexerRuleType.Equals(lexerRule.LexerRuleType))
                throw new Exception(
                    $"Unable to create TerminalLexeme from type {lexerRule.GetType().FullName}. Expected TerminalLexerRule");

            var terminalLexerRule = lexerRule as ITerminalLexerRule;
            if (_queue.Count == 0)
                return new TerminalLexeme(terminalLexerRule, segment, offset);

            var reusedLexeme = _queue.Dequeue();
            reusedLexeme.Reset(terminalLexerRule, offset);
            return reusedLexeme;
        }
    }
}