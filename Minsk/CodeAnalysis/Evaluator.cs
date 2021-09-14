using System;
using System.Collections.Generic;

using Minsk.CodeAnalysis.Binding;

using VNC;

namespace Minsk.CodeAnalysis
{
    internal sealed class Evaluator
    {
        private readonly BoundStatement _root;
        private readonly Dictionary<VariableSymbol, object> _variables;

        // Cheat a bit

        private object _lastValue;

        public Evaluator(BoundStatement root, Dictionary<VariableSymbol, object> variables)
        {
            Int64 startTicks = Log.CONSTRUCTOR($"Enter: root:{root}", Common.LOG_CATEGORY);

            _root = root;
            _variables = variables;

            Log.CONSTRUCTOR($"Exit", Common.LOG_CATEGORY, startTicks);
        }

        public object Evaluate()
        {
            Int64 startTicks = Log.EVALUATOR($"Enter", Common.LOG_CATEGORY);
            Log.EVALUATOR($"Exit", Common.LOG_CATEGORY, startTicks);

            EvaluateStatement(_root);
            return _lastValue;
        }

        private void EvaluateStatement(BoundStatement node)
        {
            Int64 startTicks = Log.EVALUATOR($"Enter node: {node}", Common.LOG_CATEGORY);

            switch (node.Kind)
            {
                case BoundNodeKind.BlockStatement:
                    {
                        Log.EVALUATOR("Exit", Common.LOG_CATEGORY, startTicks);

                        EvaluateBlockStatement((BoundBlockStatement)node);
                        break;
                    }

                case BoundNodeKind.VariableDeclaration:
                    {
                        Log.EVALUATOR("Exit", Common.LOG_CATEGORY, startTicks);

                        EvaluateVarialbeDeclaration((BoundVariableDeclaration)node);
                        break;
                    }

                case BoundNodeKind.ExpressionStatement:
                    {
                        Log.EVALUATOR("Exit", Common.LOG_CATEGORY, startTicks);

                        EvaluateExpressionStatement((BoundExpressionStatement)node);
                        break;
                    }

                default:
                    throw new Exception($"Unexpected node {node.Kind}");
            }
        }

        private void EvaluateBlockStatement(BoundBlockStatement node)
        {
            foreach (var statement in node.Statements )
            {
                EvaluateStatement(statement);
            }
        }

        private void EvaluateVarialbeDeclaration(BoundVariableDeclaration node)
        {
            var value = EvaluateExpression(node.Initializer);
            _variables[node.Variable] = value;
            _lastValue = value;
        }

        private void EvaluateExpressionStatement(BoundExpressionStatement node)
        {
            _lastValue = EvaluateExpression(node.Expression);
        }

        private object EvaluateExpression(BoundExpression node)
        {
            Int64 startTicks = Log.EVALUATOR($"Enter node: {node}", Common.LOG_CATEGORY);

            switch (node.Kind)
            {
                case BoundNodeKind.LiteralExpression:
                    {
                        var value = EvaluateLiteralExpression((BoundLiteralExpression)node);

                        Log.EVALUATOR($"Exit value:{value}", Common.LOG_CATEGORY, startTicks);

                        return value;
                    }

                case BoundNodeKind.VariableExpression:
                    {
                        var value = EvaluateVariableExpression((BoundVariableExpression)node);

                        Log.EVALUATOR($"Exit value:{value}", Common.LOG_CATEGORY, startTicks);

                        return value;
                    }

                case BoundNodeKind.AssignmentExpression:
                    {
                        var value = EvaluateAssignmentExpression((BoundAssignmentExpression)node);

                        Log.EVALUATOR($"Exit value:{value}", Common.LOG_CATEGORY, startTicks);

                        return value;
                    }

                case BoundNodeKind.UnaryExpression:
                    {
                        var value = EvaluateUnaryExpression((BoundUnaryExpression)node);

                        Log.EVALUATOR($"Exit value:{value}", Common.LOG_CATEGORY, startTicks);

                        return value;
                    }

                case BoundNodeKind.BinaryExpression:
                    {
                        var value = EvaluateBinaryExpression((BoundBinaryExpression)node);

                        Log.EVALUATOR($"Exit value:{value}", Common.LOG_CATEGORY, startTicks);

                        return value;
                    }


                //if (node is BoundParenthesizedExpression p)
                //{
                //    Log.Trace($"Exit", Common.LOG_CATEGORY, startTicks);

                //    return EvaluateExpression(p.Expression);
                //}

                default:
                    throw new Exception($"Unexpected node {node.Kind}");
            }
        }

        private static object EvaluateLiteralExpression(BoundLiteralExpression n)
        {
            return n.Value;
        }

        private object EvaluateVariableExpression(BoundVariableExpression v)
        {
            return _variables[v.Variable];
        }

        private object EvaluateAssignmentExpression(BoundAssignmentExpression a)
        {
            var value = EvaluateExpression(a.Expression);
            _variables[a.Variable] = value;
            return value;
        }

        private object EvaluateUnaryExpression(BoundUnaryExpression u)
        {
            var operand = EvaluateExpression(u.Operand);

            switch (u.Op.Kind)
            {
                case BoundUnaryOperatorKind.Identity:
                    return (int)operand;

                case BoundUnaryOperatorKind.Negation:
                    return -(int)operand;

                case BoundUnaryOperatorKind.LogicalNegation:
                    return !(Boolean)operand;

                default:
                    throw new Exception($"Unexpected Unary Operator {u.Op}");
            }
        }

        private object EvaluateBinaryExpression(BoundBinaryExpression b)
        {
            var left = EvaluateExpression(b.Left);
            var right = EvaluateExpression(b.Right);

            switch (b.Op.Kind)
            {
                case BoundBinaryOperatorKind.Addition:
                    return (int)left + (int)right;

                case BoundBinaryOperatorKind.Subtraction:
                    return (int)left - (int)right;

                case BoundBinaryOperatorKind.Multiplication:
                    return (int)left * (int)right;

                case BoundBinaryOperatorKind.Division:
                    return (int)left / (int)right;

                case BoundBinaryOperatorKind.LogicalAnd:
                    return (Boolean)left && (Boolean)right;

                case BoundBinaryOperatorKind.LogicalOr:
                    return (Boolean)left || (Boolean)right;

                case BoundBinaryOperatorKind.Equals:
                    return Equals(left, right);

                case BoundBinaryOperatorKind.NotEquals:
                    return !Equals(left, right);

                case BoundBinaryOperatorKind.Less:
                    return (int)left < (int)right;

                case BoundBinaryOperatorKind.LessOrEquals:
                    return (int)left <= (int)right;

                case BoundBinaryOperatorKind.Greater:
                    return (int)left > (int)right;

                case BoundBinaryOperatorKind.GreaterOrEquals:
                    return (int)left >= (int)right;

                default:
                    throw new Exception($"Unexpected Binary Operator {b.Op}");
            }
        }
    }
}