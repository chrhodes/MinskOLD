using System;

using VNC;

namespace Minsk.CodeAnalysis.Syntax
{
    // NOTE(crhodes)
    // Lexer breaks the input stream into tokens (words)

    internal sealed class Lexer
    {
        private readonly string _text;
        private int _position;

        private DiagnosticBag _diagnostics = new DiagnosticBag();

        public Lexer(string text)
        {
            Int64 startTicks = Log.CONSTRUCTOR($"Enter: text:{text}", Common.LOG_CATEGORY);

            _text = text;

            Log.CONSTRUCTOR($"Exit", Common.LOG_CATEGORY, startTicks);
        }

        public DiagnosticBag Diagnostics => _diagnostics;

        private char Current => Peek(0);
        private char Lookahead => Peek(1);

        private char Peek(int offset)
        {
            Int64 startTicks = Log.LEXER($"Enter offset: {offset}", Common.LOG_CATEGORY);

            var index = _position + offset;

            if (index >= _text.Length)
            {
                Log.LEXER($"Exit", Common.LOG_CATEGORY, startTicks);

                return '\0';
            }
            else
            {
                Log.LEXER($"Exit", Common.LOG_CATEGORY, startTicks);

                return _text[index];
            }
        }

        private void Next()
        {
            Int64 startTicks = Log.LEXER($"Enter", Common.LOG_CATEGORY);

            _position++;

            Log.LEXER($"Exit", Common.LOG_CATEGORY, startTicks);
        }

        public SyntaxToken Lex()
        {
            Int64 startTicks = Log.LEXER($"Enter", Common.LOG_CATEGORY);

            if (_position >= _text.Length)
            {
                Log.LEXER($"Exit (new EndOfFileToken)", Common.LOG_CATEGORY, startTicks);

                return new SyntaxToken(SyntaxKind.EndOfFileToken, _position, "\0", null);
            }

            var start = _position;

            if (char.IsDigit(Current))
            {
                while (char.IsDigit(Current))
                {
                    Next();
                }

                var length = _position - start;
                var text = _text.Substring(start, length);

                if (!int.TryParse(text, out var value))
                {
                    _diagnostics.ReportInvalidNumber(new TextSpan(start, length), _text, typeof(int));
                }

                Log.LEXER($"Exit (new NumberToken)", Common.LOG_CATEGORY, startTicks);

                return new SyntaxToken(SyntaxKind.NumberToken, start, text, value);
            }

            if (char.IsWhiteSpace(Current))
            {
                while (char.IsWhiteSpace(Current))
                {
                    Next();
                }

                var length = _position - start;
                var text = _text.Substring(start, length);

                Log.LEXER($"Exit (new WhiteSpaceToken)", Common.LOG_CATEGORY, startTicks);

                return new SyntaxToken(SyntaxKind.WhiteSpaceToken, start, text, null);
            }

            // true
            // false

            if (char.IsLetter(Current))
            {
                while (char.IsLetter(Current))
                {
                    Next();
                }

                var length = _position - start;
                var text = _text.Substring(start, length);
                var kind = SyntaxFacts.GetKeyWordKind(text);

                Log.LEXER($"Exit", Common.LOG_CATEGORY, startTicks);

                return new SyntaxToken(kind, start, text, null);
            }

            switch (Current)
            {
                case '+':
                    Log.LEXER($"Exit (new PlusToken)", Common.LOG_CATEGORY, startTicks);

                    return new SyntaxToken(SyntaxKind.PlusToken, _position++, "+", null);

                case '-':
                    Log.LEXER($"Exit (new MinusToken)", Common.LOG_CATEGORY, startTicks);

                    return new SyntaxToken(SyntaxKind.MinusToken, _position++, "-", null);

                case '*':
                    Log.LEXER($"Exit (new StarToken)", Common.LOG_CATEGORY, startTicks);

                    return new SyntaxToken(SyntaxKind.StarToken, _position++, "*", null);

                case '/':
                    Log.LEXER($"Exit (new SlashToken)", Common.LOG_CATEGORY, startTicks);

                    return new SyntaxToken(SyntaxKind.SlashToken, _position++, "/", null);

                case '(':
                    Log.LEXER($"Exit (new OpenParenthesisToken)", Common.LOG_CATEGORY, startTicks);

                    return new SyntaxToken(SyntaxKind.OpenParenthesisToken, _position++, "(", null);

                case ')':
                    Log.LEXER($"Exit (new CloseParenthesisToken)", Common.LOG_CATEGORY, startTicks);

                    return new SyntaxToken(SyntaxKind.CloseParenthesisToken, _position++, ")", null);

                case '&':
                    if (Lookahead == '&')
                    {
                        Log.LEXER($"Exit (new AmpersandAmpersandToken)", Common.LOG_CATEGORY, startTicks);

                        _position += 2;
                        return new SyntaxToken(SyntaxKind.AmpersandAmpersandToken, start, "&&", null);
                    }
                    break;

                case '|':
                    if (Lookahead == '|')
                    {
                        Log.LEXER($"Exit (new PipePipeToken)", Common.LOG_CATEGORY, startTicks);

                        _position += 2;
                        return new SyntaxToken(SyntaxKind.PipePipeToken, start, "||", null);
                    }
                    break;

                case '=':
                    if (Lookahead == '=')
                    {
                        Log.LEXER($"Exit (new EqualsEqualsToken)", Common.LOG_CATEGORY, startTicks);

                        _position += 2;
                        return new SyntaxToken(SyntaxKind.EqualsEqualsToken, start, "==", null);
                    }
                    else
                    {
                        Log.LEXER($"Exit (new EqualsToken)", Common.LOG_CATEGORY, startTicks);

                        _position += 1;
                        return new SyntaxToken(SyntaxKind.EqualsToken, start, "=", null);
                    }
                    break;

                case '!':
                    if (Lookahead == '=')
                    {
                        Log.LEXER($"Exit (new BangEqualsToken)", Common.LOG_CATEGORY, startTicks);

                        _position += 2;
                        return new SyntaxToken(SyntaxKind.BangEqualsToken, start, "!=", null);
                    }
                    else
                    {
                        Log.LEXER($"Exit (new BangToken)", Common.LOG_CATEGORY, startTicks);

                        _position += 1;
                        return new SyntaxToken(SyntaxKind.BangToken, start, "!", null);
                    }
            }

            _diagnostics.ReportBadCharacter(_position, Current);

            Log.LEXER($"Exit: ERROR: Bad character input: '{Current}' (new BadToken)", Common.LOG_CATEGORY, startTicks);

            return new SyntaxToken(SyntaxKind.BadToken, _position++, _text.Substring(_position - 1, 1), null);
        }
    }
}