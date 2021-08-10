using System;
using System.Collections.Generic;

using VNC;

namespace Minsk.CodeAnalysis.Syntax
{
    public sealed class LiteralExpressionSyntax : ExpressionSyntax
    {
        public LiteralExpressionSyntax(SyntaxToken literalToken)
            : this(literalToken, literalToken.Value)
        {
            Int64 startTicks = Log.CONSTRUCTOR($"Enter: literalToken:{literalToken}", Common.LOG_CATEGORY);


            Log.CONSTRUCTOR($"Exit", Common.LOG_CATEGORY, startTicks);
        }

        public LiteralExpressionSyntax(SyntaxToken literalToken, object value)
        {
            Int64 startTicks = Log.CONSTRUCTOR($"Enter: literalToken:{literalToken} value:{value}", Common.LOG_CATEGORY);

            LiteralToken = literalToken;
            Value = value;

            Log.CONSTRUCTOR($"Exit", Common.LOG_CATEGORY, startTicks);
        }

        public override SyntaxKind Kind => SyntaxKind.LiteralExpression;

        public SyntaxToken LiteralToken { get; }
        public object Value { get; }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            Int64 startTicks = Log.Trace16($"Enter/Exit", Common.LOG_CATEGORY);

            yield return LiteralToken;
        }
    }
}
