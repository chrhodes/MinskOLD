using System;
using System.Collections.Generic;
using System.Linq;

using Minsk.CodeAnalysis.Syntax;

using VNC;

namespace Minsk.CodeAnalysis.Binding
{
    // NOTE(crhodes)
    // Binder does the type checking for now

    internal sealed class Binder
    {
        private readonly DiagnosticBag _diagnostics = new DiagnosticBag();
        private readonly Dictionary<VariableSymbol, object> _variables;

        public Binder(Dictionary<VariableSymbol, object> variables)
        {
            Int64 startTicks = Log.CONSTRUCTOR($"Enter", Common.LOG_CATEGORY);

            _variables = variables;

            Log.CONSTRUCTOR($"Exit", Common.LOG_CATEGORY, startTicks);

        }

        public DiagnosticBag Diagnostics => _diagnostics;

        public BoundExpression BindExpression(ExpressionSyntax syntax)
        {
            Int64 startTicks = Log.Trace17($"Enter syntax.Kind:{syntax.Kind}", Common.LOG_CATEGORY);

            switch (syntax.Kind)
            {
                case SyntaxKind.LiteralExpression:
                    Log.Trace17($"Exit", Common.LOG_CATEGORY, startTicks);

                    return BindLiteralExpression((LiteralExpressionSyntax)syntax);

                case SyntaxKind.UnaryExpression:
                    Log.Trace17($"Exit", Common.LOG_CATEGORY, startTicks);

                    return BindUnaryExpression((UnaryExpressionSyntax)syntax);

                case SyntaxKind.BinaryExpression:
                    Log.Trace17($"Exit", Common.LOG_CATEGORY, startTicks);

                    return BindBinaryExpression((BinaryExpressionSyntax)syntax);

                case SyntaxKind.ParenthesizedExpression:
                    Log.Trace17($"Exit", Common.LOG_CATEGORY, startTicks);

                    return BindParenthesizedExpression((ParenthesizedExpressionSyntax)syntax);

                case SyntaxKind.NameExpression:
                    Log.Trace17($"Exit", Common.LOG_CATEGORY, startTicks);

                    return BindNameExpression((NameExpressionSyntax)syntax);

                case SyntaxKind.AssignmentExpression:
                    Log.Trace17($"Exit", Common.LOG_CATEGORY, startTicks);

                    return BindAssignmentExpression((AssignmentExpressionSyntax)syntax);

                default:
                    throw new Exception($"Unexpected syntax {syntax.Kind}");

            }
        }

        private BoundExpression BindParenthesizedExpression(ParenthesizedExpressionSyntax syntax)
        {
            Int64 startTicks = Log.Trace17($"Enter", Common.LOG_CATEGORY);
            Log.Trace17($"Exit", Common.LOG_CATEGORY, startTicks);

            return BindExpression(syntax.Expression);
        }

        private BoundExpression BindLiteralExpression(LiteralExpressionSyntax syntax)
        {
            Int64 startTicks = Log.Trace17($"Enter", Common.LOG_CATEGORY);

            var value = syntax.Value ?? 0;

            Log.Trace17($"Exit", Common.LOG_CATEGORY, startTicks);

            return new BoundLiteralExpression(value);
        }

        private BoundExpression BindNameExpression(NameExpressionSyntax syntax)
        {
            Int64 startTicks = Log.Trace17($"Enter", Common.LOG_CATEGORY);

            var name = syntax.IdentifierToken.Text;
            var variable = _variables.Keys.FirstOrDefault(v => v.Name == name);

            if (variable == null)
            {
                _diagnostics.ReportUndefinedName(syntax.IdentifierToken.Span, name);

                Log.Trace17($"Exit", Common.LOG_CATEGORY, startTicks);
                return new BoundLiteralExpression(0);
            }

            Log.Trace17($"Exit", Common.LOG_CATEGORY, startTicks);
            return new BoundVariableExpression(variable);
        }

        private BoundExpression BindAssignmentExpression(AssignmentExpressionSyntax syntax)
        {
            Int64 startTicks = Log.Trace17($"Enter", Common.LOG_CATEGORY);

            var name = syntax.IdentifierToken.Text;
            var boundExpression = BindExpression(syntax.Expression);

            // HACK(crhodes)
            // Ugly for now

            //var defaultValue =
            //    boundExpression.Type == typeof(int)
            //    ? (object)0
            //    : boundExpression.Type == typeof(bool)
            //        ? (object)false
            //        : null;

            //if (defaultValue == null)
            //{
            //    throw new Exception($"Unsupported variable type:{boundExpression.Type}");
            //}

            //_varibles[name] = defaultValue;

            //return new BoundAssignmentExpression(name, boundExpression);

            var existingVariable = _variables.Keys.FirstOrDefault(v => v.Name == name);

            if (existingVariable != null)
            {
                _variables.Remove(existingVariable);
            }

            var variable = new VariableSymbol(name, boundExpression.Type);
            _variables[variable] = null;

            Log.Trace17($"Exit", Common.LOG_CATEGORY, startTicks);

            return new BoundAssignmentExpression(variable, boundExpression);
        }

        private BoundExpression BindUnaryExpression(UnaryExpressionSyntax syntax)
        {
            Int64 startTicks = Log.Trace17($"Enter syntax:{syntax}", Common.LOG_CATEGORY);

            var boundOperand = BindExpression(syntax.Operand);
            var boundOperator = BoundUnaryOperator.Bind(syntax.OperatorToken.Kind, boundOperand.Type);

            if (boundOperator == null)
            {
                _diagnostics.ReportUndefinedUnaryOperator(syntax.OperatorToken.Span, syntax.OperatorToken.Text, boundOperand.Type);

                // NOTE(crhodes)
                // Return something for now to avoid cascading errors.

                Log.Trace17($"Exit", Common.LOG_CATEGORY, startTicks);

                return boundOperand;
            }

            Log.Trace17($"Exit", Common.LOG_CATEGORY, startTicks);

            return new BoundUnaryExpression(boundOperator, boundOperand);
        }

        private BoundExpression BindBinaryExpression(BinaryExpressionSyntax syntax)
        {
            Int64 startTicks = Log.Trace17($"Enter", Common.LOG_CATEGORY);

            var boundLeft = BindExpression(syntax.Left);
            var boundRight = BindExpression(syntax.Right);
            var boundOperator = BoundBinaryOperator.Bind(syntax.OperatorToken.Kind, boundLeft.Type, boundRight.Type);

            if (boundOperator == null)
            {
                _diagnostics.ReportUndefinedBinaryOperator(syntax.OperatorToken.Span, syntax.OperatorToken.Text, boundLeft.Type, boundRight.Type);

                // NOTE(crhodes)
                // Return something for now to avoid cascading errors.
                Log.Trace17($"Exit", Common.LOG_CATEGORY, startTicks);

                return boundLeft;
            }

            Log.Trace17($"Exit", Common.LOG_CATEGORY, startTicks);

            return new BoundBinaryExpression(boundLeft, boundOperator, boundRight);
        }
    }
}
