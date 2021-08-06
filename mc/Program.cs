using System;
using System.Threading;

using VNC;

namespace mc
{
    class Program
    {
        static void Main(string[] args)
        {
            Int64 startTicks = Log.APPLICATION_START($"SignalR Startup Delay", Common.LOG_CATEGORY);
            Thread.Sleep(200);
            startTicks = Log.APPLICATION_START($"Enter", Common.LOG_CATEGORY, startTicks);

            // NOTE(crhodes)
            // Establish REPL

            while (true)
            {
                Console.Write("> ");

                var line = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(line))
                {
                    Log.APPLICATION_START($"Exit - No Input Received", Common.LOG_CATEGORY, startTicks);

                    return;
                }

                var lexer = new Lexer(line);

                while (true)
                {
                    var token = lexer.NextToken();

                    if (token.Kind == SyntaxKind.EndOfFileToken) break;

                    Console.Write($"{token.Kind}: '{token.Text}'");

                    if (token.Value != null)
                    {
                        Console.Write($" {token.Value}");
                    }

                    Console.WriteLine();
                }

                Log.APPLICATION_START($"Exit", Common.LOG_CATEGORY, startTicks);
            }
        }

        internal enum SyntaxKind
        {
            NumberToken,
            WhiteSpaceToken,
            PlusToken,
            MinusToken,
            StarToken,
            SlashToken,
            OpenParenthesisToken,
            CloseParenthesisToken,
            BadToken,
            EndOfFileToken
        }

        // Represents a word in language

        internal class SyntaxToken
        {

            public SyntaxToken(SyntaxKind kind, int position, string text, object value)
            {
                Int64 startTicks = Log.CONSTRUCTOR($"Enter: kind:{kind} position:{position} text:{text} value:{value}", Common.LOG_CATEGORY);

                Kind = kind;
                Position = position;
                Text = text;
                Value = value;

                Log.CONSTRUCTOR($"Exit", Common.LOG_CATEGORY, startTicks);
            }

            public SyntaxKind Kind { get; }
            public int Position { get; }
            public string Text { get; }
            public object Value { get; }
        }

        internal class Lexer
        {
            private readonly string _text;
            private int _position;

            public Lexer(string text)
            {
                Int64 startTicks = Log.CONSTRUCTOR($"Enter: text:{text}", Common.LOG_CATEGORY);

                _text = text;

                Log.CONSTRUCTOR($"Exit", Common.LOG_CATEGORY, startTicks);
            }

            private char Current
            {
                get
                {
                    if (_position >= _text.Length)
                    {
                        return '\0';
                    }
                    else
                    {
                        return _text[_position];
                    }
                }
            }

            private void Next()
            {
                Int64 startTicks = Log.Trace10($"Enter", Common.LOG_CATEGORY);

                _position++;

                Log.Trace10($"Exit", Common.LOG_CATEGORY, startTicks);
            }

            public SyntaxToken NextToken()
            {
                Int64 startTicks = Log.Trace($"Enter", Common.LOG_CATEGORY);

                // <numbers>
                // + - / * ( )
                // <whitespace>

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

                    int.TryParse(text, out var value);

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

                if (Current == '+')
                {
                    Log.Trace($"Exit (new PlusToken)", Common.LOG_CATEGORY, startTicks);

                    return new SyntaxToken(SyntaxKind.PlusToken, _position++, "+", null);
                }
                else if ((Current == '-'))
                {
                    Log.Trace($"Exit (new MinusToken)", Common.LOG_CATEGORY, startTicks);

                    return new SyntaxToken(SyntaxKind.MinusToken, _position++, "-", null);
                }
                else if ((Current == '*'))
                {
                    Log.Trace($"Exit (new StarToken)", Common.LOG_CATEGORY, startTicks);

                    return new SyntaxToken(SyntaxKind.StarToken, _position++, "*", null);
                }
                else if ((Current == '/'))
                {
                    Log.Trace($"Exit (new SlashToken)", Common.LOG_CATEGORY, startTicks);

                    return new SyntaxToken(SyntaxKind.SlashToken, _position++, "/", null);
                }
                else if ((Current == '('))
                {
                    Log.Trace($"Exit (new OpenParenthesisToken)", Common.LOG_CATEGORY, startTicks);

                    return new SyntaxToken(SyntaxKind.OpenParenthesisToken, _position++, "(", null);
                }
                else if ((Current == ')'))
                {
                    Log.Trace($"Exit (new CloseParenthesis)", Common.LOG_CATEGORY, startTicks);

                    return new SyntaxToken(SyntaxKind.CloseParenthesisToken, _position++, ")", null);
                }

                Log.Trace($"Exit (new BadToken)", Common.LOG_CATEGORY, startTicks);

                return new SyntaxToken(SyntaxKind.BadToken, _position++, _text.Substring(_position - 1, 1), null);
            }
        }
    }
}
