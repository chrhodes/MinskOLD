using System;
using System.Collections.Generic;

using VNC;

namespace Minsk.CodeAnalysis
{
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

    internal sealed class Parser
    {
        private readonly SyntaxToken[] _tokens;
        // NOTE(crhodes)
        // Handle errors
        private List<string> _diagnostics = new List<string>();

        private int _position;
        public Parser(string text)
        {
            Int64 startTicks = Log.CONSTRUCTOR($"Enter: text:{text}", Common.LOG_CATEGORY);

            var tokens = new List<SyntaxToken>();

            var lexer = new Lexer(text);

            SyntaxToken token;

            do
            {
                token = lexer.Lex();

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

        private SyntaxToken Current => Peek(0);

        public SyntaxTree Parse()
        {
            Int64 startTicks = Log.Trace($"Enter", Common.LOG_CATEGORY);

            var expression = ParseExpression();
            var endOfFileToken = MatchToken(SyntaxKind.EndOfFileToken);

            Log.Trace($"Exit new SyntaxTree", Common.LOG_CATEGORY, startTicks);

            return new SyntaxTree(_diagnostics, expression, endOfFileToken);
        }

        private SyntaxToken MatchToken(SyntaxKind kind)
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

        private SyntaxToken NextToken()
        {
            Int64 startTicks = Log.Trace($"Enter", Common.LOG_CATEGORY);

            var current = Current;
            _position++;

            Log.Trace($"Exit", Common.LOG_CATEGORY, startTicks);

            return current;
        }

        private ExpressionSyntax ParseExpression()
        {
            Int64 startTicks = Log.Trace($"Enter/Exit", Common.LOG_CATEGORY);

            return ParseTerm();
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
                var right = MatchToken(SyntaxKind.CloseParenthesisToken);

                Log.Trace($"Exit ParenthesizedExpressionSyntax", Common.LOG_CATEGORY, startTicks);

                return new ParenthesizedExpressionSyntax(left, expression, right);
            }

            var numberToken = MatchToken(SyntaxKind.NumberToken);

            Log.Trace($"Exit LiteralExpressionSyntax", Common.LOG_CATEGORY, startTicks);

            return new LiteralExpressionSyntax(numberToken);
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
    }
}