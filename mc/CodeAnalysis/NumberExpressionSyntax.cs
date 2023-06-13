using System;
using System.Collections.Generic;

using VNC;

namespace Minsk.CodeAnalysis
{
    internal sealed class NumberExpressionSyntax : ExpressionSyntax
    {
        public NumberExpressionSyntax(SyntaxToken numberToken)
        {
            Int64 startTicks = Log.CONSTRUCTOR($"Enter: numberToken:{numberToken}", Common.LOG_CATEGORY);

            NumberToken = numberToken;

            Log.CONSTRUCTOR($"Exit", Common.LOG_CATEGORY, startTicks);
        }

        public override SyntaxKind Kind => SyntaxKind.NumberExpression;

        public SyntaxToken NumberToken { get; }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            Int64 startTicks = Log.SYNTAX($"Enter/Exit", Common.LOG_CATEGORY);

            yield return NumberToken;
        }
    }
}