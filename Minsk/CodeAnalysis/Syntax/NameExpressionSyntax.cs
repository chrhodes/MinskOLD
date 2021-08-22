using System;
using System.Collections.Generic;

using VNC;

namespace Minsk.CodeAnalysis.Syntax
{
    public sealed class NameExpressionSyntax : ExpressionSyntax
    {
        public NameExpressionSyntax(SyntaxToken identifierToken)
        {
            Int64 startTicks = Log.CONSTRUCTOR($"Enter: identifierToken:{identifierToken}", Common.LOG_CATEGORY);

            IdentifierToken = identifierToken;

            Log.CONSTRUCTOR($"Exit", Common.LOG_CATEGORY, startTicks);
        }

        public override SyntaxKind Kind => SyntaxKind.NameExpression;
        public SyntaxToken IdentifierToken { get; }


        //public override IEnumerable<SyntaxNode> GetChildren()
        //{
        //    yield return IdentifierToken;
        //}
    }
}