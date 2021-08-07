using System;
using System.Collections.Generic;
using System.Linq;


namespace Minsk.CodeAnalysis.Binding
{
    internal enum BoundBinaryOperatorKind
    {
        Addition,
        Subtraction,
        Multiplication,
        Division,

        LogicalAnd,
        LogicalOr
    }
}
