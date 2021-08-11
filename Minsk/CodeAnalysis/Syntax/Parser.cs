using System;
using System.Collections.Generic;

using VNC;

namespace Minsk.CodeAnalysis.Syntax
{
    //NOTE(crhodes)
    // Parser assembles the tokens into sentences
    // Parser produces Syntax Trees (sentences)
    // from the Syntax Tokens (words)

    internal sealed class Parser
    {
        private readonly SyntaxToken[] _tokens;

        private DiagnosticBag _diagnostics = new DiagnosticBag();

        private int _position;

        public Parser(string text)
        {
            Int64 startTicks = Log.CONSTRUCTOR($"Enter: text: {text}", Common.LOG_CATEGORY);

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

        public DiagnosticBag Diagnostics => _diagnostics;

        // NOTE(crhodes)
        // This lets you look ahead to see how to parse what you have already seen.

        private SyntaxToken Peek(int offset)
        {
            Int64 startTicks = Log.PARSER($"Enter offset: {offset}", Common.LOG_CATEGORY);

            var index = _position + offset;

            if (index >= _tokens.Length)
            {
                Log.PARSER($"Exit {_tokens[_tokens.Length - 1].Kind}", Common.LOG_CATEGORY, startTicks);

                return _tokens[_tokens.Length - 1];
            }

            Log.PARSER($"Exit {_tokens[index].Kind}", Common.LOG_CATEGORY, startTicks);

            return _tokens[index];
        }

        private SyntaxToken Current => Peek(0);

        private SyntaxToken NextToken()
        {
            Int64 startTicks = Log.PARSER($"Enter", Common.LOG_CATEGORY);

            var current = Current;
            _position++;

            Log.PARSER($"Exit {current.Kind}", Common.LOG_CATEGORY, startTicks);

            return current;
        }

        private SyntaxToken MatchToken(SyntaxKind kind)
        {
            Int64 startTicks = Log.PARSER($"Enter kind: {kind}", Common.LOG_CATEGORY);

            if (Current.Kind == kind)
            {
                Log.PARSER($"Exit Current.Kind == kind", Common.LOG_CATEGORY, startTicks);

                return NextToken();
            }

            _diagnostics.ReportUnexpectedToken(Current.Span, Current.Kind, kind);

            // NOTE(crhodes)
            // This is super useful because ...

            Log.PARSER($"Exit new SyntaxToken()", Common.LOG_CATEGORY, startTicks);

            return new SyntaxToken(kind, Current.Position, null, null);
        }

        public SyntaxTree Parse()
        {
            Int64 startTicks = Log.PARSER($"Enter", Common.LOG_CATEGORY);

            var expression = ParseExpression();
            var endOfFileToken = MatchToken(SyntaxKind.EndOfFileToken);

            Log.PARSER($"Exit new SyntaxTree()", Common.LOG_CATEGORY, startTicks);

            return new SyntaxTree(_diagnostics, expression, endOfFileToken);
        }

        // NOTE(crhodes)
        // This is cheating.  For now just have new method vs a more generalized approach

        private ExpressionSyntax ParseExpression()
        {
            Int64 startTicks = Log.PARSER($"Enter", Common.LOG_CATEGORY);
            Log.PARSER($"Exit return ParseAssignmentExpression()", Common.LOG_CATEGORY, startTicks);

            return ParseAssignmentExpression();
        }

        private ExpressionSyntax ParseAssignmentExpression()
        {
            Int64 startTicks = Log.PARSER($"Enter", Common.LOG_CATEGORY);

            // HACK(crhodes)
            // This would work, but only at top level

            if (Peek(0).Kind == SyntaxKind.IdentifierToken
                && Peek(1).Kind == SyntaxKind.EqualsToken)
            {
                var identifierToken = NextToken();
                var operatorToken = NextToken();
                var right = ParseAssignmentExpression();
                return new AssignmentExpressionSyntax(identifierToken, operatorToken, right);
            }

            Log.PARSER($"Exit return ParseBinaryExpression()", Common.LOG_CATEGORY, startTicks);

            return ParseBinaryExpression();
        }

        private ExpressionSyntax ParseBinaryExpression(int parentPrecedence = 0)
        {
            Int64 startTicks = Log.PARSER($"Enter parentPrecedence: {parentPrecedence}", Common.LOG_CATEGORY);

            ExpressionSyntax left;

            var unaryOperatorPrecedence = Current.Kind.GetUnaryOperatorPrecedence();

            if (unaryOperatorPrecedence != 0
                && unaryOperatorPrecedence >= parentPrecedence)
            {
                var operatorToken = NextToken();
                var operand = ParseBinaryExpression(unaryOperatorPrecedence);
                left = new UnaryExpressionSyntax(operatorToken, operand);
            }
            else
            {
                left = ParsePrimaryExpression();
            }

            while (true)
            {
                var precedence = Current.Kind.GetBinaryOperatorPrecedence();

                if (precedence == 0
                    || precedence <= parentPrecedence)
                {
                    break;
                }

                var operatorToken = NextToken();
                var right = ParseBinaryExpression(precedence);
                left = new BinaryExpressionSyntax(left, operatorToken, right);
            }

            Log.PARSER($"Exit", Common.LOG_CATEGORY, startTicks);

            return left;
        }

        private ExpressionSyntax ParsePrimaryExpression()
        {
            Int64 startTicks = Log.PARSER($"Enter", Common.LOG_CATEGORY);

            switch (Current.Kind)
            {
                case SyntaxKind.OpenParenthesisToken:
                    var left = NextToken();
                    var expression = ParseExpression();
                    var right = MatchToken(SyntaxKind.CloseParenthesisToken);

                    Log.PARSER($"Exit (ParenthesizedExpressionSyntax)", Common.LOG_CATEGORY, startTicks);

                    return new ParenthesizedExpressionSyntax(left, expression, right);

                case SyntaxKind.TrueKeyword:
                case SyntaxKind.FalseKeyword:
                    var keywordToken = NextToken();
                    var value = keywordToken.Kind == SyntaxKind.TrueKeyword;

                    Log.PARSER($"Exit (LiteralExpressionSyntax)", Common.LOG_CATEGORY, startTicks);

                    return new LiteralExpressionSyntax(keywordToken, value);

                case SyntaxKind.IdentifierToken:
                    var identifierToken = NextToken();

                    Log.PARSER($"Exit (NameExpressionSyntax)", Common.LOG_CATEGORY, startTicks);

                    return new NameExpressionSyntax(identifierToken);

                default:
                    var numberToken = MatchToken(SyntaxKind.NumberToken);

                    Log.PARSER($"Exit (LiteralExpressionSyntax)", Common.LOG_CATEGORY, startTicks);

                    return new LiteralExpressionSyntax(numberToken);
            }
        }
    }
}