using System;
using System.Collections.Generic;
using System.Linq;


namespace Minsk.CodeAnalysis.Binding
{
    internal abstract class BoundNode
    {
        public abstract BoundNodeKind Kind { get; }
    }
}
