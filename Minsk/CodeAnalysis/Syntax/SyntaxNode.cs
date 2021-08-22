using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Minsk.CodeAnalysis.Syntax
{
    public abstract class SyntaxNode
    {
        public abstract SyntaxKind Kind { get; }

        // NOTE(crhodes)
        // Not super efficient

        public virtual TextSpan Span
        {
            get
            {
                var first = GetChildren().First().Span;
                var last = GetChildren().Last().Span;

                return TextSpan.FromBounds(first.Start, last.End);
            }
        }

        // NOTE(crhodes)
        // Add notion of children so can walk tree in a generic way

        //public abstract IEnumerable<SyntaxNode> GetChildren();

        // NOTE(crhodes)
        // Does this depend on layout (order) of properties in type???
        // Who call GetChildren
        // Hum.  Yes, it F's up printing of tree

        // Curiously at 1:10:56 in video Episode 5 Immo says he would do abstract and yield like before in production compiler.
        // So, this seems like fancy, risky stuff
        // Even if the asserting of order in tests - begs question do we have tests to cover this
        // Not sure if compiler or reflection guarantees order of properties in classes.


        public IEnumerable<SyntaxNode> GetChildren()
        {
            var properties = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var property in properties)
            {
                if (typeof(SyntaxNode).IsAssignableFrom(property.PropertyType))
                {
                    var child = (SyntaxNode)property.GetValue(this);

                    yield return child;
                }
                else if (typeof(IEnumerable<SyntaxNode>).IsAssignableFrom(property.PropertyType))
                {
                    var children = (IEnumerable<SyntaxNode>)property.GetValue(this);

                    foreach (var child in children)
                    {
                        yield return child;
                    }
                }
            }
        }
    }
}