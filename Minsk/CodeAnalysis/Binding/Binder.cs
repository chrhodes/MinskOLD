using System;
using System.Collections.Generic;

using Minsk.CodeAnalysis.Syntax;

using VNC;

namespace Minsk.CodeAnalysis.Binding
{
    // NOTE(crhodes)
    // Binder does the type checking for now

    internal sealed class Binder
    {
        private readonly DiagnosticBag _diagnostics = new DiagnosticBag();
        public DiagnosticBag Diagnostics => _diagnostics;

        public BoundExpression BindExpression(ExpressionSyntax syntax)
        {
            Int64 startTicks = Log.Trace($"Enter", Common.LOG_CATEGORY);

            switch (syntax.Kind)
            {
                case SyntaxKind.LiteralExpression:
                    Log.CONSTRUCTOR($"Exit", Common.LOG_CATEGORY, startTicks);
                    return BindLiteralExpression((LiteralExpressionSyntax)syntax);

                case SyntaxKind.UnaryExpression:
                    Log.CONSTRUCTOR($"Exit", Common.LOG_CATEGORY, startTicks);
                    return BindUnaryExpression((UnaryExpressionSyntax)syntax);

                case SyntaxKind.BinaryExpression:
                    Log.CONSTRUCTOR($"Exit", Common.LOG_CATEGORY, startTicks);
                    return BindBinaryExpression((BinaryExpressionSyntax)syntax);

                case SyntaxKind.ParenthesizedExpression:
                    Log.CONSTRUCTOR($"Exit", Common.LOG_CATEGORY, startTicks);
                    return BindExpression(((ParenthesizedExpressionSyntax)syntax).Expression);

                default:
                    throw new Exception($"Unexpected syntax {syntax.Kind}");

            }
        }

        private BoundExpression BindLiteralExpression(LiteralExpressionSyntax syntax)
        {
            Int64 startTicks = Log.Trace($"Enter", Common.LOG_CATEGORY);

            var value = syntax.Value ?? 0;

            Log.CONSTRUCTOR($"Exit", Common.LOG_CATEGORY, startTicks);

            return new BoundLiteralExpression(value);
        }

        private BoundExpression BindUnaryExpression(UnaryExpressionSyntax syntax)
        {
            Int64 startTicks = Log.Trace($"Enter syntax:{syntax}", Common.LOG_CATEGORY);

            var boundOperand = BindExpression(syntax.Operand);
            var boundOperator = BoundUnaryOperator.Bind(syntax.OperatorToken.Kind, boundOperand.Type);

            if (boundOperator == null)
            {
                _diagnostics.ReportUndefinedUnaryOperator(syntax.OperatorToken.Span, syntax.OperatorToken.Text, boundOperand.Type);

                // NOTE(crhodes)
                // Return something for now to avoid cascading errors.

                return boundOperand;
            }

            Log.CONSTRUCTOR($"Exit", Common.LOG_CATEGORY, startTicks);

            return new BoundUnaryExpression(boundOperator, boundOperand);
        }

        private BoundExpression BindBinaryExpression(BinaryExpressionSyntax syntax)
        {
            Int64 startTicks = Log.Trace($"Enter", Common.LOG_CATEGORY);

            var boundLeft = BindExpression(syntax.Left);
            var boundRight = BindExpression(syntax.Right);
            var boundOperator = BoundBinaryOperator.Bind(syntax.OperatorToken.Kind, boundLeft.Type, boundRight.Type);

            if (boundOperator == null)
            {
                _diagnostics.ReportUndefinedBinaryOperator(syntax.OperatorToken.Span, syntax.OperatorToken.Text, boundLeft.Type, boundRight.Type);

                // NOTE(crhodes)
                // Return something for now to avoid cascading errors.
                Log.CONSTRUCTOR($"Exit", Common.LOG_CATEGORY, startTicks);
                return boundLeft;
            }

            return new BoundBinaryExpression(boundLeft, boundOperator, boundRight);
        }

    }
}
