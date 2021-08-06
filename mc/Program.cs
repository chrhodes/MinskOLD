using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using VNC;

namespace Minsk
{
    class Program
    {
        static void Main(string[] args)
        {
            Int64 startTicks = Log.APPLICATION_START($"SignalR Startup Delay", Common.LOG_CATEGORY);
            Thread.Sleep(200);
            startTicks = Log.APPLICATION_START($"Enter", Common.LOG_CATEGORY, startTicks);

            while (true)
            {
                Console.Write("> ");

                var line = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(line))
                {
                    Log.APPLICATION_START($"Exit - No Input Received", Common.LOG_CATEGORY, startTicks);

                    return;
                }

                var parser = new Parser(line);
                var syntaxTree = parser.Parse();

                var color = Console.ForegroundColor;

                //Console.ForegroundColor = ConsoleColor.DarkCyan;

                //PrettyPrint1(syntaxTree.Root);

                Console.ForegroundColor = ConsoleColor.DarkGreen;

                PrettyPrint2(syntaxTree.Root);

                Console.ForegroundColor = color;

                if (!syntaxTree.Diagnostics.Any())
                {
                    var e = new Evaluator(syntaxTree.Root);

                    var result = e.Evaluate();
                    Console.WriteLine(result);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;

                    foreach (var diagnostic in parser.Diagnostics)
                    {
                        Console.WriteLine(diagnostic);
                    }

                    Console.ResetColor();
                }

                //if (syntaxTree.Diagnostics.Any())
                //{
                //    Console.ForegroundColor = ConsoleColor.Red;

                //    foreach (var diagnostic in parser.Diagnostics)
                //    {
                //        Console.WriteLine(diagnostic);
                //    }

                //    Console.ResetColor();
                //}

                Log.APPLICATION_START($"Exit", Common.LOG_CATEGORY, startTicks);
            }
        }

        static void PrettyPrint1(SyntaxNode node, string indent = "")
        {
            Console.Write(indent);
            Console.Write(node.Kind);

            if (node is SyntaxToken t && t.Value != null)
            {
                Console.Write(" ");
                Console.Write(t.Value);
            }

            Console.WriteLine();

            indent += "   ";

            foreach (var child in node.GetChildren())
            {
                PrettyPrint1(child, indent);
            }
        }

        static void PrettyPrint2(SyntaxNode node, string indent = "", bool isLast = true)
        {
            // Unix https://en.wikipedia.org/wiki/Tree_(command)
            // └──
            // ├──
            // │

            var marker = isLast ? "└──" : "├──";

            Console.Write(indent);
            Console.Write(marker);
            Console.Write(node.Kind);

            if (node is SyntaxToken t && t.Value != null)
            {
                Console.Write(" ");
                Console.Write(t.Value);
            }

            Console.WriteLine();

            indent += isLast ? "   " : "│   ";

            var lastChild = node.GetChildren().LastOrDefault();

            foreach (var child in node.GetChildren())
            {
                PrettyPrint2(child, indent, child == lastChild);
            }
        }

        enum SyntaxKind
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
            EndOfFileToken,
            NumberExpression,
            BinaryExpression,
            ParenthesizedExpression
        }

        // NOTE(crhodes)
        // SyntaxTree represent entire collection of nodes.

        sealed class SyntaxTree
        {
            public SyntaxTree(IEnumerable<string> diagnostics, ExpressionSyntax root, SyntaxToken endOfFileToken)
            {
                Int64 startTicks = Log.CONSTRUCTOR($"Enter: diagnostics: {diagnostics} root:{root} endOfFileToken:{endOfFileToken}", Common.LOG_CATEGORY);

                Diagnostics = diagnostics.ToArray();
                Root = root;
                EndOfFileToken = endOfFileToken;

                Log.CONSTRUCTOR($"Exit", Common.LOG_CATEGORY, startTicks);
            }

            public IReadOnlyList<string> Diagnostics { get; }
            public ExpressionSyntax Root { get; }
            public SyntaxToken EndOfFileToken { get; }
        }

        // NOTE(crhodes)
        // Tokens represent a word in language
        // Think of the tokens as leaves in the tree

        // For now just treat them as SyntaxNodes

        class SyntaxToken : SyntaxNode
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

            public override SyntaxKind Kind { get; }

            public int Position { get; }
            public string Text { get; }
            public object Value { get; }

            public override IEnumerable<SyntaxNode> GetChildren()
            {
                Int64 startTicks = Log.Trace($"Enter/Exit", Common.LOG_CATEGORY);

                return Enumerable.Empty<SyntaxNode>();
            }
        }

        // NOTE(crhodes)
        // Base type for all Syntax Nodes

        abstract class SyntaxNode
        {
            public abstract SyntaxKind Kind { get; }

            // NOTE(crhodes)
            // Add notion of children so can walk tree in a generic way

            public abstract IEnumerable<SyntaxNode> GetChildren();
        }

        abstract class ExpressionSyntax : SyntaxNode
        {

        }

        sealed class NumberExpressionSyntax : ExpressionSyntax
        {
            public NumberExpressionSyntax(SyntaxToken numberToken)
            {
                Int64 startTicks = Log.CONSTRUCTOR($"Enter: numberToken:{numberToken}", Common.LOG_CATEGORY);

                NumberToken = numberToken;

                Log.CONSTRUCTOR($"Exit", Common.LOG_CATEGORY, startTicks);
            }

            public override SyntaxKind Kind => SyntaxKind.NumberExpression;

            public SyntaxToken NumberToken { get; }

            public override IEnumerable<SyntaxNode> GetChildren()
            {
                Int64 startTicks = Log.Trace($"Enter/Exit", Common.LOG_CATEGORY);

                yield return NumberToken;
            }
        }

        sealed class BinaryExpressionSyntax : ExpressionSyntax
        {
            public BinaryExpressionSyntax(ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right)
            {
                Int64 startTicks = Log.CONSTRUCTOR($"Enter: left: {left} operatorToken: {operatorToken} right: {right}", Common.LOG_CATEGORY);

                Left = left;
                OperatorToken = operatorToken;
                Right = right;

                Log.CONSTRUCTOR($"Exit", Common.LOG_CATEGORY, startTicks);
            }

            public override SyntaxKind Kind => SyntaxKind.BinaryExpression;

            public ExpressionSyntax Left { get; }
            public SyntaxToken OperatorToken { get; }
            public ExpressionSyntax Right { get; }

            public override IEnumerable<SyntaxNode> GetChildren()
            {
                Int64 startTicks = Log.Trace($"Enter/Exit", Common.LOG_CATEGORY);

                yield return Left;
                yield return OperatorToken;
                yield return Right;
            }
        }

        sealed class ParenthesizedExpressionSyntax : ExpressionSyntax
        {
            public ParenthesizedExpressionSyntax(SyntaxToken openParenthesisToken, ExpressionSyntax expression, SyntaxToken closeParenthesisToken)
            {
                OpenParenthesisToken = openParenthesisToken;
                Expression = expression;
                CloseParenthesisToken = closeParenthesisToken;
            }

            public override SyntaxKind Kind => SyntaxKind.ParenthesizedExpression;

            public SyntaxToken OpenParenthesisToken { get; }
            public ExpressionSyntax Expression { get; }
            public SyntaxToken CloseParenthesisToken { get; }

            public override IEnumerable<SyntaxNode> GetChildren()
            {
                yield return OpenParenthesisToken;
                yield return Expression;
                yield return CloseParenthesisToken;
            }
        }

        // NOTE(crhodes)
        // Lexer breaks the input stream into tokens (words)

        class Lexer
        {
            private readonly string _text;
            private int _position;

            // NOTE(crhodes)
            // Handle errors
            private List<string> _diagnostics = new List<string>();

            public Lexer(string text)
            {
                Int64 startTicks = Log.CONSTRUCTOR($"Enter: text:{text}", Common.LOG_CATEGORY);

                _text = text;

                Log.CONSTRUCTOR($"Exit", Common.LOG_CATEGORY, startTicks);
            }

            public IEnumerable<string> Diagnostics => _diagnostics;

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
                    Log.Trace($"Exit (new CloseParenthesisToken)", Common.LOG_CATEGORY, startTicks);

                    return new SyntaxToken(SyntaxKind.CloseParenthesisToken, _position++, ")", null);
                }

                _diagnostics.Add($"ERROR: Bad character input: '{Current}'");

                Log.Trace($"Exit: ERROR: Bad character input: '{Current}' (new BadToken)", Common.LOG_CATEGORY, startTicks);

                return new SyntaxToken(SyntaxKind.BadToken, _position++, _text.Substring(_position - 1, 1), null);
            }
        }

        // // NOTE(crhodes)
        // Parser assembles the tokens into sentences
        // Parser produces Syntax Trees (sentences)
        // from the Syntax Tokens (words)
        // Syntax Nodes

        // Two possible trees
        // First is left to right.
        // Second understands operator precedence

        // 1 + 2 * 3
        //
        //      +
        //     / \
        //    1   *
        //       / \
        //      2   3

        // 7

        // 1 + 2 * 3
        //
        //        *
        //       / \
        //      +  3
        //     / \
        //    1   2

        // 9

        class Parser
        {
            private readonly SyntaxToken[] _tokens;
            private int _position;

            // NOTE(crhodes)
            // Handle errors
            private List<string> _diagnostics = new List<string>();

            public Parser(string text)
            {
                Int64 startTicks = Log.CONSTRUCTOR($"Enter: text:{text}", Common.LOG_CATEGORY);

                var tokens = new List<SyntaxToken>();

                var lexer = new Lexer(text);

                SyntaxToken token;

                do
                {
                    token = lexer.NextToken();

                    if (token.Kind != SyntaxKind.WhiteSpaceToken
                        && token.Kind != SyntaxKind.BadToken)
                    {
                        tokens.Add(token);
                    }
                } while (token.Kind != SyntaxKind.EndOfFileToken);

                _tokens = tokens.ToArray();

                _diagnostics.AddRange(lexer.Diagnostics);

                Log.CONSTRUCTOR($"Exit", Common.LOG_CATEGORY, startTicks);
            }

            public IEnumerable<string> Diagnostics => _diagnostics;

            // NOTE(crhodes)
            // This lets you look ahead to see how to parse what you have already seen.

            private SyntaxToken Peek(int offset)
            {
                Int64 startTicks = Log.Trace($"Enter offset: {offset}", Common.LOG_CATEGORY);

                var index = _position + offset;

                if (index >= _tokens.Length)
                {
                    Log.Info($"Exit", Common.LOG_CATEGORY, startTicks);

                    return _tokens[_tokens.Length - 1];
                }

                Log.Info($"Exit", Common.LOG_CATEGORY, startTicks);

                return _tokens[index];
            }

            private SyntaxToken Current => Peek(0);

            private SyntaxToken NextToken()
            {
                Int64 startTicks = Log.Trace($"Enter", Common.LOG_CATEGORY);

                var current = Current;
                _position++;

                Log.Trace($"Exit", Common.LOG_CATEGORY, startTicks);

                return current;
            }

            private SyntaxToken Match(SyntaxKind kind)
            {
                Int64 startTicks = Log.Trace($"Enter kind: {kind}", Common.LOG_CATEGORY);

                if (Current.Kind == kind)
                {
                    Log.Trace($"Exit Current.Kind == kind", Common.LOG_CATEGORY, startTicks);

                    return NextToken();
                }

                _diagnostics.Add($"ERROR: Unexpected token: <{Current.Kind}>, expected <{kind}>");

                // NOTE(crhodes)
                // This is super useful because ...

                Log.Trace($"Exit: ERROR: Unexpected token: <{Current.Kind}>, expected <{kind}>", Common.LOG_CATEGORY, startTicks);

                return new SyntaxToken(kind, Current.Position, null, null);
            }

            private ExpressionSyntax ParseExpression()
            {
                Int64 startTicks = Log.Trace($"Enter/Exit", Common.LOG_CATEGORY);

                return ParseTerm();
            }

            public SyntaxTree Parse()
            {
                Int64 startTicks = Log.Trace($"Enter", Common.LOG_CATEGORY);

                var expression = ParseTerm();
                var endOfFileToken = Match(SyntaxKind.EndOfFileToken);

                Log.Trace($"Exit new SyntaxTree", Common.LOG_CATEGORY, startTicks);

                return new SyntaxTree(_diagnostics, expression, endOfFileToken);
            }

            private ExpressionSyntax ParseTerm()
            {

                Int64 startTicks = Log.Trace($"Enter", Common.LOG_CATEGORY);
                var left = ParseFactor();

                while (Current.Kind == SyntaxKind.PlusToken
                    || Current.Kind == SyntaxKind.MinusToken)
                {
                    var operatorToken = NextToken();
                    var right = ParseFactor();
                    left = new BinaryExpressionSyntax(left, operatorToken, right);
                }

                Log.Trace($"Exit", Common.LOG_CATEGORY, startTicks);

                return left;
            }

            private ExpressionSyntax ParseFactor()
            {

                Int64 startTicks = Log.Trace($"Enter", Common.LOG_CATEGORY);

                var left = ParsePrimaryExpression();

                while (Current.Kind == SyntaxKind.StarToken
                    || Current.Kind == SyntaxKind.SlashToken)
                {
                    var operatorToken = NextToken();
                    var right = ParsePrimaryExpression();
                    left = new BinaryExpressionSyntax(left, operatorToken, right);
                }

                Log.Trace($"Exit", Common.LOG_CATEGORY, startTicks);

                return left;
            }

            private ExpressionSyntax ParsePrimaryExpression()
            {
                Int64 startTicks = Log.Trace($"Enter", Common.LOG_CATEGORY);

                if (Current.Kind == SyntaxKind.OpenParenthesisToken)
                {
                    var left = NextToken();
                    var expression = ParseExpression();
                    var right = Match(SyntaxKind.CloseParenthesisToken);

                    Log.Trace($"Exit ParenthesizedExpressionSyntax", Common.LOG_CATEGORY, startTicks);

                    return new ParenthesizedExpressionSyntax(left, expression, right);
                }

                var numberToken = Match(SyntaxKind.NumberToken);

                Log.Trace($"Exit NumberExpressionSyntax", Common.LOG_CATEGORY, startTicks);

                return new NumberExpressionSyntax(numberToken);
            }

            // This does not understand Operator Precedence
            // Approach above hard codes knowledge 

            //private ExpressionSyntax ParseExpression()
            //{
            //    Int64 startTicks = Log.Trace($"Enter", Common.LOG_CATEGORY);

            //    var left = ParsePrimaryExpression();

            //    while (Current.Kind == SyntaxKind.PlusToken
            //        || Current.Kind == SyntaxKind.MinusToken
            //        || Current.Kind == SyntaxKind.StarToken
            //        || Current.Kind == SyntaxKind.SlashToken)
            //    {
            //        var operatorToken = NextToken();
            //        var right = ParsePrimaryExpression();
            //        left = new BinaryExpressionSyntax(left, operatorToken, right);
            //    }

            //    Log.Trace($"Exit ExpressionSyntax: {left}", Common.LOG_CATEGORY, startTicks);

            //    return left;
            //}

            //private ExpressionSyntax ParsePrimaryExpression()
            //{
            //    Int64 startTicks = Log.Trace($"Enter", Common.LOG_CATEGORY);

            //    var numberToken = Match(SyntaxKind.NumberToken);

            //    Log.Trace($"Exit", Common.LOG_CATEGORY, startTicks);

            //    return new NumberExpressionSyntax(numberToken);
            //}
        }

        class Evaluator
        {
            private readonly ExpressionSyntax _root;

            public Evaluator(ExpressionSyntax root)
            {
                Int64 startTicks = Log.CONSTRUCTOR($"Enter: root:{root}", Common.LOG_CATEGORY);

                _root = root;

                Log.CONSTRUCTOR($"Exit", Common.LOG_CATEGORY, startTicks);
            }

            public int Evaluate()
            {
                Int64 startTicks = Log.Trace($"Enter/Exit", Common.LOG_CATEGORY);

                return EvaluateExpression(_root);
            }

            private int EvaluateExpression(ExpressionSyntax node)
            {
                Int64 startTicks = Log.Trace($"Enter node:{node}", Common.LOG_CATEGORY);

                // BinaryExpression
                // NumberExpression

                if (node is NumberExpressionSyntax n)
                {
                    Log.Trace ($"Exit", Common.LOG_CATEGORY, startTicks);

                    return (int)n.NumberToken.Value;
                }

                if (node is BinaryExpressionSyntax b)
                {
                    var left = EvaluateExpression(b.Left);
                    var right = EvaluateExpression(b.Right);


                    if (b.OperatorToken.Kind == SyntaxKind.PlusToken)
                    {
                        Log.Trace($"Exit", Common.LOG_CATEGORY, startTicks);

                        return left + right;
                    }

                    else if (b.OperatorToken.Kind == SyntaxKind.MinusToken)
                    {
                        Log.Trace($"Exit", Common.LOG_CATEGORY, startTicks);

                        return left - right;
                    }

                    else if (b.OperatorToken.Kind == SyntaxKind.StarToken)
                    {
                        Log.Trace($"Exit", Common.LOG_CATEGORY, startTicks);

                        return left * right;
                    }

                    else if (b.OperatorToken.Kind == SyntaxKind.SlashToken)
                    {
                        Log.Trace($"Exit", Common.LOG_CATEGORY, startTicks);

                        return left / right;
                    }
                    else
                    {
                        throw new Exception($"Unexpected Binary Operator {b.OperatorToken.Kind}");
                    }

                }

                if (node is ParenthesizedExpressionSyntax p)
                {
                    return EvaluateExpression(p.Expression);
                }

                throw new Exception($"Unexpected node {node.Kind}");
            }
        }
    }
}
