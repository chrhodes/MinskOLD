using System;
using System.Collections.Generic;
using System.Collections.Immutable;

using Minsk.CodeAnalysis.Syntax;

using VNC;

namespace Minsk.CodeAnalysis.Binding
{
    internal sealed class Binder
    {
        private readonly DiagnosticBag _diagnostics = new DiagnosticBag();
        private BoundScope _scope;

        public Binder(BoundScope parent)
        {
            Int64 startTicks = Log.CONSTRUCTOR($"Enter", Common.LOG_CATEGORY);

            _scope = new BoundScope(parent);

            Log.CONSTRUCTOR($"Exit", Common.LOG_CATEGORY, startTicks);
        }

        public static BoundGlobalScope BindGlobalScope(BoundGlobalScope previous, CompilationUnitSyntax syntax)
        {
            var parentScope = CreateParentScopes(previous);
            var binder = new Binder(parentScope);
            var statement = binder.BindStatement(syntax.Statement);
            var variables = binder._scope.GetDeclaredVariables();
            var diagnostics = binder.Diagnostics.ToImmutableArray();

            if (previous != null)
            {
                diagnostics = diagnostics.InsertRange(0, previous.Diagnostics);
            }

            return new BoundGlobalScope(previous, diagnostics, variables, statement);
        }

        private static BoundScope CreateParentScopes(BoundGlobalScope previous)
        {
            // submission 3 -> submission 2 -> submission 1 from REPL

            var stack = new Stack<BoundGlobalScope>();

            while (previous != null)
            {
                stack.Push(previous);
                previous = previous.Previous;
            }

            BoundScope parent = null;

            while (stack.Count > 0)
            {
                previous = stack.Pop();
                var scope = new BoundScope(parent);

                foreach (var v in previous.Variables )
                {
                    scope.TryDeclare(v);
                }

                parent = scope;
            }

            return parent;
        }

        public DiagnosticBag Diagnostics => _diagnostics;

        private BoundStatement BindStatement(StatementSyntax syntax)
        {
            Int64 startTicks = Log.BINDER($"Enter syntax.Kind:{syntax.Kind}", Common.LOG_CATEGORY);

            switch (syntax.Kind)
            {
                case SyntaxKind.BlockStatement:
                    Log.BINDER($"Exit", Common.LOG_CATEGORY, startTicks);

                    return BindBlockStatement((BlockStatementSyntax)syntax);

                case SyntaxKind.VariableDeclaration:
                    Log.BINDER($"Exit", Common.LOG_CATEGORY, startTicks);

                    return BindVariableDeclaration((VariableDeclarationSyntax)syntax);

                case SyntaxKind.IfStatement:
                    Log.BINDER($"Exit", Common.LOG_CATEGORY, startTicks);

                    return BindIfStatement((IfStatementSyntax)syntax);

                case SyntaxKind.ExpressionStatement:
                    Log.BINDER($"Exit", Common.LOG_CATEGORY, startTicks);

                    return BindExpressionStatement((ExpressionStatementSyntax)syntax);

                default:
                    throw new Exception($"Unexpected syntax {syntax.Kind}");
            }
        }

        private BoundStatement BindBlockStatement(BlockStatementSyntax syntax)
        {
            var statements = ImmutableArray.CreateBuilder<BoundStatement>();
            _scope = new BoundScope(_scope);

            foreach (var statementSyntax in syntax.Statements)
            {
                var statement = BindStatement(statementSyntax);
                statements.Add(statement);
            }

            _scope = _scope.Parent;

            return new BoundBlockStatement(statements.ToImmutable());
        }

        private BoundStatement BindVariableDeclaration(VariableDeclarationSyntax syntax)
        {
            var name = syntax.Identifier.Text;
            var initializer = BindExpression(syntax.Initializer);
            var isReadOnly = syntax.Keyword.Kind == SyntaxKind.LetKeyword;
            var variable = new VariableSymbol(name, isReadOnly, initializer.Type);

            if (!_scope.TryDeclare(variable))
            {
                _diagnostics.ReportVariableAlreadyDeclared(syntax.Identifier.Span, name);
            }

            return new BoundVariableDeclaration(variable, initializer);
        }

        private BoundStatement BindIfStatement(IfStatementSyntax syntax)
        {
            var condition = BindExpression(syntax.Condition, typeof(Boolean));
            var thenStatement = BindStatement(syntax.ThenStatement);
            var elseStatement = syntax.ElseClause == null
                ? null
                : BindStatement(syntax.ElseClause.ElseStatement);

            return new BoundIfStatement(condition, thenStatement, elseStatement);
        }

        private BoundStatement BindExpressionStatement(ExpressionStatementSyntax syntax)
        {
            var expression = BindExpression(syntax.Expression);

            return new BoundExpressionStatement(expression);
        }

        private BoundExpression BindExpression(ExpressionSyntax syntax, Type targetType)
        {
            var result = BindExpression(syntax);

            if (result.Type != targetType)
            {
                _diagnostics.ReportCannotConvert(syntax.Span, result.Type, targetType);
            }

            return result;
        }

        public BoundExpression BindExpression(ExpressionSyntax syntax)
        {
            Int64 startTicks = Log.BINDER($"Enter syntax.Kind:{syntax.Kind}", Common.LOG_CATEGORY);

            switch (syntax.Kind)
            {
                case SyntaxKind.LiteralExpression:
                    Log.BINDER($"Exit", Common.LOG_CATEGORY, startTicks);

                    return BindLiteralExpression((LiteralExpressionSyntax)syntax);

                case SyntaxKind.UnaryExpression:
                    Log.BINDER($"Exit", Common.LOG_CATEGORY, startTicks);

                    return BindUnaryExpression((UnaryExpressionSyntax)syntax);

                case SyntaxKind.BinaryExpression:
                    Log.BINDER($"Exit", Common.LOG_CATEGORY, startTicks);

                    return BindBinaryExpression((BinaryExpressionSyntax)syntax);

                case SyntaxKind.ParenthesizedExpression:
                    Log.BINDER($"Exit", Common.LOG_CATEGORY, startTicks);

                    return BindParenthesizedExpression((ParenthesizedExpressionSyntax)syntax);

                case SyntaxKind.NameExpression:
                    Log.BINDER($"Exit", Common.LOG_CATEGORY, startTicks);

                    return BindNameExpression((NameExpressionSyntax)syntax);

                case SyntaxKind.AssignmentExpression:
                    Log.BINDER($"Exit", Common.LOG_CATEGORY, startTicks);

                    return BindAssignmentExpression((AssignmentExpressionSyntax)syntax);

                default:
                    throw new Exception($"Unexpected syntax {syntax.Kind}");

            }
        }

        private BoundExpression BindParenthesizedExpression(ParenthesizedExpressionSyntax syntax)
        {
            Int64 startTicks = Log.BINDER($"Enter", Common.LOG_CATEGORY);
            Log.BINDER($"Exit", Common.LOG_CATEGORY, startTicks);

            return BindExpression(syntax.Expression);
        }

        private BoundExpression BindLiteralExpression(LiteralExpressionSyntax syntax)
        {
            Int64 startTicks = Log.BINDER($"Enter", Common.LOG_CATEGORY);

            var value = syntax.Value ?? 0;

            Log.BINDER($"Exit", Common.LOG_CATEGORY, startTicks);

            return new BoundLiteralExpression(value);
        }

        private BoundExpression BindNameExpression(NameExpressionSyntax syntax)
        {
            Int64 startTicks = Log.BINDER($"Enter", Common.LOG_CATEGORY);

            var name = syntax.IdentifierToken.Text;

            if (! _scope.TryLookup(name, out var variable))
            {
                _diagnostics.ReportUndefinedName(syntax.IdentifierToken.Span, name);

                Log.BINDER($"Exit", Common.LOG_CATEGORY, startTicks);
                return new BoundLiteralExpression(0);
            }

            Log.BINDER($"Exit", Common.LOG_CATEGORY, startTicks);

            return new BoundVariableExpression(variable);
        }

        private BoundExpression BindAssignmentExpression(AssignmentExpressionSyntax syntax)
        {
            Int64 startTicks = Log.BINDER($"Enter", Common.LOG_CATEGORY);

            var name = syntax.IdentifierToken.Text;
            var boundExpression = BindExpression(syntax.Expression);

            if (! _scope.TryLookup(name, out var variable))
            {
                _diagnostics.ReportUndefinedName(syntax.IdentifierToken.Span, name);

                Log.BINDER($"Exit", Common.LOG_CATEGORY, startTicks);
                return boundExpression;
            }

            if (variable.IsReadOnly)
            {
                _diagnostics.ReportCannotAssign(syntax.EqualsToken.Span, name);
            }

            if (boundExpression.Type != variable.Type)
            {
                _diagnostics.ReportCannotConvert(syntax.Expression.Span, boundExpression.Type, variable.Type);

                return boundExpression;
            }

            Log.BINDER($"Exit", Common.LOG_CATEGORY, startTicks);

            return new BoundAssignmentExpression(variable, boundExpression);
        }

        private BoundExpression BindUnaryExpression(UnaryExpressionSyntax syntax)
        {
            Int64 startTicks = Log.BINDER($"Enter syntax:{syntax}", Common.LOG_CATEGORY);

            var boundOperand = BindExpression(syntax.Operand);
            var boundOperator = BoundUnaryOperator.Bind(syntax.OperatorToken.Kind, boundOperand.Type);

            if (boundOperator == null)
            {
                _diagnostics.ReportUndefinedUnaryOperator(syntax.OperatorToken.Span, syntax.OperatorToken.Text, boundOperand.Type);

                // NOTE(crhodes)
                // Return something for now to avoid cascading errors.

                Log.BINDER($"Exit", Common.LOG_CATEGORY, startTicks);

                return boundOperand;
            }

            Log.BINDER($"Exit", Common.LOG_CATEGORY, startTicks);

            return new BoundUnaryExpression(boundOperator, boundOperand);
        }

        private BoundExpression BindBinaryExpression(BinaryExpressionSyntax syntax)
        {
            Int64 startTicks = Log.BINDER($"Enter", Common.LOG_CATEGORY);

            var boundLeft = BindExpression(syntax.Left);
            var boundRight = BindExpression(syntax.Right);
            var boundOperator = BoundBinaryOperator.Bind(syntax.OperatorToken.Kind, boundLeft.Type, boundRight.Type);

            if (boundOperator == null)
            {
                _diagnostics.ReportUndefinedBinaryOperator(syntax.OperatorToken.Span, syntax.OperatorToken.Text, boundLeft.Type, boundRight.Type);

                // NOTE(crhodes)
                // Return something for now to avoid cascading errors.
                Log.BINDER($"Exit", Common.LOG_CATEGORY, startTicks);

                return boundLeft;
            }

            Log.BINDER($"Exit", Common.LOG_CATEGORY, startTicks);

            return new BoundBinaryExpression(boundLeft, boundOperator, boundRight);
        }
    }
}
