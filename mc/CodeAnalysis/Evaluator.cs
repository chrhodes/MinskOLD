using System;

using VNC;

namespace Minsk.CodeAnalysis
{
    public sealed class Evaluator
    {
        private readonly ExpressionSyntax _root;

        public Evaluator(ExpressionSyntax root)
        {
            Int64 startTicks = Log.CONSTRUCTOR($"Enter: root:{root}", Common.LOG_CATEGORY);

            _root = root;

            Log.CONSTRUCTOR($"Exit", Common.LOG_CATEGORY, startTicks);
        }

        public int Evaluate()
        {
            Int64 startTicks = Log.Trace($"Enter/Exit", Common.LOG_CATEGORY);

            return EvaluateExpression(_root);
        }

        private int EvaluateExpression(ExpressionSyntax node)
        {
            Int64 startTicks = Log.Trace($"Enter node:{node}", Common.LOG_CATEGORY);

            if (node is LiteralExpressionSyntax n)
            {
                Log.Trace($"Exit", Common.LOG_CATEGORY, startTicks);

                return (int)n.LiteralToken.Value;
            }

            if (node is BinaryExpressionSyntax b)
            {
                var left = EvaluateExpression(b.Left);
                var right = EvaluateExpression(b.Right);

                if (b.OperatorToken.Kind == SyntaxKind.PlusToken)
                {
                    Log.Trace($"Exit", Common.LOG_CATEGORY, startTicks);

                    return left + right;
                }
                else if (b.OperatorToken.Kind == SyntaxKind.MinusToken)
                {
                    Log.Trace($"Exit", Common.LOG_CATEGORY, startTicks);

                    return left - right;
                }
                else if (b.OperatorToken.Kind == SyntaxKind.StarToken)
                {
                    Log.Trace($"Exit", Common.LOG_CATEGORY, startTicks);

                    return left * right;
                }
                else if (b.OperatorToken.Kind == SyntaxKind.SlashToken)
                {
                    Log.Trace($"Exit", Common.LOG_CATEGORY, startTicks);

                    return left / right;
                }
                else
                {
                    throw new Exception($"Unexpected Binary Operator {b.OperatorToken.Kind}");
                }
            }

            if (node is ParenthesizedExpressionSyntax p)
            {
                Log.Trace($"Exit", Common.LOG_CATEGORY, startTicks);

                return EvaluateExpression(p.Expression);
            }

            throw new Exception($"Unexpected node {node.Kind}");
        }
    }
}