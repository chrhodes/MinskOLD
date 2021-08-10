using System;
using System.Collections.Generic;

using Minsk.CodeAnalysis.Binding;

using VNC;

namespace Minsk.CodeAnalysis
{
    internal sealed class Evaluator
    {
        private readonly BoundExpression _root;
        private readonly Dictionary<VariableSymbol, object> _variables;

        public Evaluator(BoundExpression root, Dictionary<VariableSymbol, object> variables)
        {
            Int64 startTicks = Log.CONSTRUCTOR($"Enter: root:{root}", Common.LOG_CATEGORY);

            _root = root;
            _variables = variables;

            Log.CONSTRUCTOR($"Exit", Common.LOG_CATEGORY, startTicks);
        }

        public object Evaluate()
        {
            Int64 startTicks = Log.Trace21($"Enter", Common.LOG_CATEGORY);
            Log.Trace21($"Exit", Common.LOG_CATEGORY, startTicks);

            return EvaluateExpression(_root);
        }

        private object EvaluateExpression(BoundExpression node)
        {
            Int64 startTicks = Log.Trace21($"Enter node: {node}", Common.LOG_CATEGORY);

            if (node is BoundLiteralExpression n)
            {
                Log.Trace21($"Exit n.Value:{n.Value}", Common.LOG_CATEGORY, startTicks);

                return n.Value;
            }

            if (node is BoundVariableExpression v)
            {
                var value = _variables[v.Variable];

                Log.Trace21($"Exit value:{value}", Common.LOG_CATEGORY, startTicks);

                return value;
            }

            if (node is BoundAssignmentExpression a)
            {
                var value = EvaluateExpression(a.Expression);
                _variables[a.Variable] = value;

                Log.Trace21($"Exit value:{value}", Common.LOG_CATEGORY, startTicks);

                return value;
            }

            if (node is BoundUnaryExpression u)
            {
                var operand = EvaluateExpression(u.Operand);

                switch (u.Op.Kind)
                {
                    case BoundUnaryOperatorKind.Identity:
                        Log.Trace21($"Exit operand:{operand}", Common.LOG_CATEGORY, startTicks);

                        return (int)operand;

                    case BoundUnaryOperatorKind.Negation:
                        Log.Trace21($"Exit operand:{operand}", Common.LOG_CATEGORY, startTicks);

                        return -(int)operand;

                    case BoundUnaryOperatorKind.LogicalNegation:
                        Log.Trace21($"Exit operand:{operand}", Common.LOG_CATEGORY, startTicks);

                        return !(Boolean)operand;

                    default:
                        throw new Exception($"Unexpected Unary Operator {u.Op}");
                }
            }

            if (node is BoundBinaryExpression b)
            {
                var left = EvaluateExpression(b.Left);
                var right = EvaluateExpression(b.Right);

                switch (b.Op.Kind)
                {
                    case BoundBinaryOperatorKind.Addition:
                        Log.Trace21($"Exit", Common.LOG_CATEGORY, startTicks);

                        return (int)left + (int)right;

                    case BoundBinaryOperatorKind.Subtraction:
                        Log.Trace21($"Exit", Common.LOG_CATEGORY, startTicks);

                        return (int)left - (int)right;

                    case BoundBinaryOperatorKind.Multiplication:
                        Log.Trace21($"Exit", Common.LOG_CATEGORY, startTicks);

                        return (int)left * (int)right;

                    case BoundBinaryOperatorKind.Division:
                        Log.Trace21($"Exit", Common.LOG_CATEGORY, startTicks);

                        return (int)left / (int)right;

                    case BoundBinaryOperatorKind.LogicalAnd:
                        Log.Trace21($"Exit", Common.LOG_CATEGORY, startTicks);

                        return (Boolean)left && (Boolean)right;

                    case BoundBinaryOperatorKind.LogicalOr:
                        Log.Trace21($"Exit", Common.LOG_CATEGORY, startTicks);

                        return (Boolean)left || (Boolean)right;

                    case BoundBinaryOperatorKind.Equals:
                        Log.Trace21($"Exit", Common.LOG_CATEGORY, startTicks);

                        return Equals(left, right);

                    case BoundBinaryOperatorKind.NotEquals:
                        Log.Trace21($"Exit", Common.LOG_CATEGORY, startTicks);

                        return !Equals(left, right);

                    default:
                        throw new Exception($"Unexpected Binary Operator {b.Op}");
                }
            }

            //if (node is BoundParenthesizedExpression p)
            //{
            //    Log.Trace($"Exit", Common.LOG_CATEGORY, startTicks);

            //    return EvaluateExpression(p.Expression);
            //}

            throw new Exception($"Unexpected node {node.Kind}");
        }
    }
}