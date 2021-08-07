using System;
using System.Collections.Generic;
using System.Linq;


namespace Minsk.CodeAnalysis.Binding
{
    internal abstract class BoundExpression : BoundNode
    {
        public abstract Type Type { get; }
    }
}
