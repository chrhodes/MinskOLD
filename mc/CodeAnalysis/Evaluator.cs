using System;

using Minsk.CodeAnalysis.Binding;

using VNC;

namespace Minsk.CodeAnalysis
{
    internal sealed class Evaluator
    {
        private readonly BoundExpression _root;

        public Evaluator(BoundExpression root)
        {
            Int64 startTicks = Log.CONSTRUCTOR($"Enter: root:{root}", Common.LOG_CATEGORY);

            _root = root;

            Log.CONSTRUCTOR($"Exit", Common.LOG_CATEGORY, startTicks);
        }

        public object Evaluate()
        {
            Int64 startTicks = Log.Trace($"Enter/Exit", Common.LOG_CATEGORY);

            return EvaluateExpression(_root);
        }

        private object EvaluateExpression(BoundExpression node)
        {
            Int64 startTicks = Log.Trace($"Enter node:{node}", Common.LOG_CATEGORY);

            if (node is BoundLiteralExpression n)
            {
                Log.Trace($"Exit", Common.LOG_CATEGORY, startTicks);

                return n.Value;
            }

            if (node is BoundUnaryExpression u)
            {
                var operand = EvaluateExpression(u.Operand);

                switch (u.OperatorKind)
                {
                    case BoundUnaryOperatorKind.Identity:
                        Log.Trace($"Exit", Common.LOG_CATEGORY, startTicks);

                        return (int)operand;

                    case BoundUnaryOperatorKind.Negation:
                        Log.Trace($"Exit", Common.LOG_CATEGORY, startTicks);

                        return -(int)operand;

                    case BoundUnaryOperatorKind.LogicalNegation:
                        Log.Trace($"Exit", Common.LOG_CATEGORY, startTicks);

                        return !(Boolean)operand;

                    default:
                        throw new Exception($"Unexpected Unary Operator {u.OperatorKind}");
                }
            }

            if (node is BoundBinaryExpression b)
            {
                var left = EvaluateExpression(b.Left);
                var right = EvaluateExpression(b.Right);

                switch (b.OperatorKind)
                {
                    case BoundBinaryOperatorKind.Addition:
                        Log.Trace($"Exit", Common.LOG_CATEGORY, startTicks);

                        return (int)left + (int)right;

                    case BoundBinaryOperatorKind.Subtraction:
                        Log.Trace($"Exit", Common.LOG_CATEGORY, startTicks);

                        return (int)left - (int)right;

                    case BoundBinaryOperatorKind.Multiplication:
                        Log.Trace($"Exit", Common.LOG_CATEGORY, startTicks);

                        return (int)left * (int)right;

                    case BoundBinaryOperatorKind.Division:
                        Log.Trace($"Exit", Common.LOG_CATEGORY, startTicks);

                        return (int)left / (int)right;


                    case BoundBinaryOperatorKind.LogicalAnd:
                        Log.Trace($"Exit", Common.LOG_CATEGORY, startTicks);

                        return (Boolean)left && (Boolean)right;


                    case BoundBinaryOperatorKind.LogicalOr:
                        Log.Trace($"Exit", Common.LOG_CATEGORY, startTicks);

                        return (Boolean)left || (Boolean)right;

                    default:
                        throw new Exception($"Unexpected Binary Operator {b.OperatorKind}");
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