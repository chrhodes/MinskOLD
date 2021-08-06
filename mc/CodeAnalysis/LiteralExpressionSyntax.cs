using System;
using System.Collections.Generic;

using VNC;

namespace Minsk.CodeAnalysis
{
    public sealed class LiteralExpressionSyntax : ExpressionSyntax
    {
        public LiteralExpressionSyntax(SyntaxToken literalToken)
        {
            Int64 startTicks = Log.CONSTRUCTOR($"Enter: literalToken:{literalToken}", Common.LOG_CATEGORY);

            LiteralToken = literalToken;

            Log.CONSTRUCTOR($"Exit", Common.LOG_CATEGORY, startTicks);
        }

        public override SyntaxKind Kind => SyntaxKind.LiteralExpression;

        public SyntaxToken LiteralToken { get; }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            Int64 startTicks = Log.Trace($"Enter/Exit", Common.LOG_CATEGORY);

            yield return LiteralToken;
        }
    }
}
