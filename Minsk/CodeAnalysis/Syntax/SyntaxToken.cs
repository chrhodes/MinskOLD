using System;
using System.Collections.Generic;
using System.Linq;

using VNC;

namespace Minsk.CodeAnalysis.Syntax
{
    // NOTE(crhodes)
    // Tokens represent a word in language
    // Think of the tokens as leaves in the tree

    // For now just treat them as SyntaxNodes

    public sealed class SyntaxToken : SyntaxNode
    {
        public SyntaxToken(SyntaxKind kind, int position, string text, object value)
        {
            Int64 startTicks = Log.CONSTRUCTOR($"Enter: kind:{kind} position:{position} text:{text} value:{value}", Common.LOG_CATEGORY);

            Kind = kind;
            Position = position;
            Text = text;
            Value = value;

            Log.CONSTRUCTOR($"Exit", Common.LOG_CATEGORY, startTicks);
        }

        public override SyntaxKind Kind { get; }

        public int Position { get; }
        public string Text { get; }
        public object Value { get; }
        public TextSpan Span => new TextSpan(Position, Text.Length);

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            Int64 startTicks = Log.Trace($"Enter/Exit", Common.LOG_CATEGORY);

            return Enumerable.Empty<SyntaxNode>();
        }
    }
}