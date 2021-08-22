using System.Collections.Generic;
using System.Reflection;

namespace Minsk.CodeAnalysis.Syntax
{
    public abstract class SyntaxNode
    {
        public abstract SyntaxKind Kind { get; }

        // NOTE(crhodes)
        // Add notion of children so can walk tree in a generic way

        //public abstract IEnumerable<SyntaxNode> GetChildren();

        // NOTE(crhodes)
        // Does this depend on layout (order) of properties in type???
        // Who call GetChildren
        // Hum.  Yes, it F's up printing of tree


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