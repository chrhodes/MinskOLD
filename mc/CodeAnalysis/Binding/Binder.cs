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
        private readonly List<string> _diagnostics = new List<string>();
        public IEnumerable<string> Diagnostics => _diagnostics;

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
                _diagnostics.Add($"Unary operator '{syntax.OperatorToken.Text}' is not defined for type {boundOperand.Type}");

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
                _diagnostics.Add($"Binary operator '{syntax.OperatorToken.Text}' is not defined for types {boundLeft.Type} and {boundRight.Type}.");

                // NOTE(crhodes)
                // Return something for now to avoid cascading errors.
                Log.CONSTRUCTOR($"Exit", Common.LOG_CATEGORY, startTicks);
                return boundLeft;
            }

            return new BoundBinaryExpression(boundLeft, boundOperator, boundRight);
        }

        //private BoundUnaryOperatorKind? BindUnaryOperatorKind(SyntaxKind kind, Type operandType)
        //{
        //    Int64 startTicks = Log.Trace($"Enter kind:{kind} operandType:{operandType}", Common.LOG_CATEGORY);

        //    if (operandType == typeof(int))
        //    {
        //        switch (kind)
        //        {
        //            case SyntaxKind.PlusToken:
        //                Log.CONSTRUCTOR($"Exit", Common.LOG_CATEGORY, startTicks);
        //                return BoundUnaryOperatorKind.Identity;

        //            case SyntaxKind.MinusToken:
        //                Log.CONSTRUCTOR($"Exit", Common.LOG_CATEGORY, startTicks);
        //                return BoundUnaryOperatorKind.Negation;

        //            //default:
        //            //    throw new Exception($"Unexpected Unary operator {kind}");
        //        }
        //    }

        //    if (operandType == typeof(Boolean))
        //    {
        //        switch (kind)
        //        {
        //            case SyntaxKind.BangToken:
        //                Log.CONSTRUCTOR($"Exit", Common.LOG_CATEGORY, startTicks);
        //                return BoundUnaryOperatorKind.LogicalNegation;

        //        }
        //    }

        //    return null;
        //}


        //private BoundBinaryOperatorKind? BindBinaryOperatorKind(SyntaxKind kind, Type leftType, Type rightType )
        //{
        //    Int64 startTicks = Log.Trace($"Enter", Common.LOG_CATEGORY);

        //    if ((leftType == typeof(int))
        //        && (rightType == typeof(int)))
        //    {
        //        switch (kind)
        //        {
        //            case SyntaxKind.PlusToken:
        //                Log.CONSTRUCTOR($"Exit", Common.LOG_CATEGORY, startTicks);
        //                return BoundBinaryOperatorKind.Addition;

        //            case SyntaxKind.MinusToken:
        //                Log.CONSTRUCTOR($"Exit", Common.LOG_CATEGORY, startTicks);
        //                return BoundBinaryOperatorKind.Subtraction;

        //            case SyntaxKind.StarToken:
        //                Log.CONSTRUCTOR($"Exit", Common.LOG_CATEGORY, startTicks);
        //                return BoundBinaryOperatorKind.Multiplication;

        //            case SyntaxKind.SlashToken:
        //                Log.CONSTRUCTOR($"Exit", Common.LOG_CATEGORY, startTicks);
        //                return BoundBinaryOperatorKind.Division;

        //            //default:
        //            //    throw new Exception($"Unexpected Binary operator {kind}");

        //        }
        //    }

        //    if ((leftType == typeof(Boolean))
        //        && (rightType == typeof(Boolean)))
        //    {
        //        switch (kind)
        //        {
        //            case SyntaxKind.AmpersandAmpersandToken:
        //                Log.CONSTRUCTOR($"Exit", Common.LOG_CATEGORY, startTicks);
        //                return BoundBinaryOperatorKind.LogicalAnd;

        //            case SyntaxKind.PipePipeToken:
        //                Log.CONSTRUCTOR($"Exit", Common.LOG_CATEGORY, startTicks);
        //                return BoundBinaryOperatorKind.LogicalOr;

        //        }
        //    }

        //    Log.CONSTRUCTOR($"Exit", Common.LOG_CATEGORY, startTicks);

        //    return null;
        //}

    }
}
