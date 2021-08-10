using System.Collections.Generic;

namespace Minsk.CodeAnalysis.Syntax
{
    public abstract class SyntaxNode
    {
        public abstract SyntaxKind Kind { get; }

        // NOTE(crhodes)
        // Add notion of children so can walk tree in a generic way

        public abstract IEnumerable<SyntaxNode> GetChildren();
    }
}