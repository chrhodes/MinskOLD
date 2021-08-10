using System;
using System.Collections.Generic;

using VNC;

namespace Minsk.CodeAnalysis.Syntax
{
    public sealed class UnaryExpressionSyntax : ExpressionSyntax
    {
        public UnaryExpressionSyntax(SyntaxToken operatorToken, ExpressionSyntax operand)
        {
            Int64 startTicks = Log.CONSTRUCTOR($"Enter: operatorToken: {operatorToken} operand: {operand}", Common.LOG_CATEGORY);

            OperatorToken = operatorToken;
            Operand = operand;

            Log.CONSTRUCTOR($"Exit", Common.LOG_CATEGORY, startTicks);
        }

        public override SyntaxKind Kind => SyntaxKind.UnaryExpression;

        public SyntaxToken OperatorToken { get; }
        public ExpressionSyntax Operand { get; }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            Int64 startTicks = Log.Trace16($"Enter/Exit", Common.LOG_CATEGORY);

            yield return OperatorToken;
            yield return Operand;
        }
    }
}
