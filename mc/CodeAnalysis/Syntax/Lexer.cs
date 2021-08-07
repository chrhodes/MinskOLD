using System;
using System.Collections.Generic;

using VNC;

namespace Minsk.CodeAnalysis.Syntax
{
    // NOTE(crhodes)
    // Lexer breaks the input stream into tokens (words)

    internal sealed class Lexer
    {
        private readonly string _text;
        private int _position;

        private List<string> _diagnostics = new List<string>();

        public Lexer(string text)
        {
            Int64 startTicks = Log.CONSTRUCTOR($"Enter: text:{text}", Common.LOG_CATEGORY);

            _text = text;

            Log.CONSTRUCTOR($"Exit", Common.LOG_CATEGORY, startTicks);
        }

        public IEnumerable<string> Diagnostics => _diagnostics;

        private char Current => Peek(0);
        private char Lookahead => Peek(1);

        private char Peek(int offset)
        {
            var index = _position + offset;

            if (index >= _text.Length)
            {
                return '\0';
            }
            else
            {
                return _text[_position];
            }
        }

        private void Next()
        {
            Int64 startTicks = Log.Trace10($"Enter", Common.LOG_CATEGORY);

            _position++;

            Log.Trace10($"Exit", Common.LOG_CATEGORY, startTicks);
        }

        public SyntaxToken Lex()
        {
            Int64 startTicks = Log.Trace($"Enter", Common.LOG_CATEGORY);

            if (_position >= _text.Length)
            {
                Log.Trace($"Exit (new EndOfFileToken)", Common.LOG_CATEGORY, startTicks);

                return new SyntaxToken(SyntaxKind.EndOfFileToken, _position, "\0", null);
            }

            if (char.IsDigit(Current))
            {
                var start = _position;

                while (char.IsDigit(Current))
                {
                    Next();
                }

                var length = _position - start;
                var text = _text.Substring(start, length);

                if (!int.TryParse(text, out var value))
                {
                    _diagnostics.Add($"ERROR: The number {_text} is not a valid Int32");
                }

                Log.Trace($"Exit (new NumberToken)", Common.LOG_CATEGORY, startTicks);

                return new SyntaxToken(SyntaxKind.NumberToken, start, text, value);
            }

            if (char.IsWhiteSpace(Current))
            {
                var start = _position;

                while (char.IsWhiteSpace(Current))
                {
                    Next();
                }

                var length = _position - start;
                var text = _text.Substring(start, length);

                Log.Trace($"Exit (new WhiteSpaceToken)", Common.LOG_CATEGORY, startTicks);

                return new SyntaxToken(SyntaxKind.WhiteSpaceToken, start, text, null);
            }

            // true
            // false

            if (char.IsLetter(Current))
            {
                var start = _position;

                while (char.IsLetter(Current))
                {
                    Next();
                }

                var length = _position - start;
                var text = _text.Substring(start, length);
                var kind = SyntaxFacts.GetKeyWordKind(text);

                Log.Trace($"Exit (new WhiteSpaceToken)", Common.LOG_CATEGORY, startTicks);

                return new SyntaxToken(kind, start, text, null);
            }

            switch (Current)
            {
                case '+':
                    Log.Trace($"Exit (new PlusToken)", Common.LOG_CATEGORY, startTicks);

                    return new SyntaxToken(SyntaxKind.PlusToken, _position++, "+", null);
                case '-':
                    Log.Trace($"Exit (new MinusToken)", Common.LOG_CATEGORY, startTicks);

                    return new SyntaxToken(SyntaxKind.MinusToken, _position++, "-", null);
                case '*':
                    Log.Trace($"Exit (new StarToken)", Common.LOG_CATEGORY, startTicks);

                    return new SyntaxToken(SyntaxKind.StarToken, _position++, "*", null);
                case '/':
                    Log.Trace($"Exit (new SlashToken)", Common.LOG_CATEGORY, startTicks);

                    return new SyntaxToken(SyntaxKind.SlashToken, _position++, "/", null);
                case '(':
                    Log.Trace($"Exit (new OpenParenthesisToken)", Common.LOG_CATEGORY, startTicks);

                    return new SyntaxToken(SyntaxKind.OpenParenthesisToken, _position++, "(", null);
                case ')':
                    Log.Trace($"Exit (new CloseParenthesisToken)", Common.LOG_CATEGORY, startTicks);

                    return new SyntaxToken(SyntaxKind.CloseParenthesisToken, _position++, ")", null);

                case '!':
                    Log.Trace($"Exit (new CloseParenthesisToken)", Common.LOG_CATEGORY, startTicks);

                    return new SyntaxToken(SyntaxKind.BangToken, _position++, "!", null);

                case '&':
                    Log.Trace($"Exit (new CloseParenthesisToken)", Common.LOG_CATEGORY, startTicks);

                    if (Lookahead == '&')
                    {
                        return new SyntaxToken(SyntaxKind.AmpersandAmpersandToken, _position += 2, "&&", null);
                    }
                    break;

                case '|':
                    Log.Trace($"Exit (new CloseParenthesisToken)", Common.LOG_CATEGORY, startTicks);

                    if (Lookahead == '|')
                    {
                        return new SyntaxToken(SyntaxKind.PipePipeToken, _position += 2, "||", null);
                    }
                    break;

            }

            _diagnostics.Add($"ERROR: Bad character input: '{Current}'");

            Log.Trace($"Exit: ERROR: Bad character input: '{Current}' (new BadToken)", Common.LOG_CATEGORY, startTicks);

            return new SyntaxToken(SyntaxKind.BadToken, _position++, _text.Substring(_position - 1, 1), null);
        }
    }
}