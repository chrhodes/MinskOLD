using System;
using System.Collections.Generic;

using VNC;

namespace Minsk.CodeAnalysis
{
    // // NOTE(crhodes)
    // Parser assembles the tokens into sentences
    // Parser produces Syntax Trees (sentences)
    // from the Syntax Tokens (words)
    // Syntax Nodes include trivia (whitespace)

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

    internal class Parser
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
    }
}