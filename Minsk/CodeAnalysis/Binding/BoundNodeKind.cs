namespace Minsk.CodeAnalysis.Binding
{
    internal enum BoundNodeKind
    {
        // Expressions

        LiteralExpression,
        VariableExpression,
        AssignmentExpression,
        UnaryExpression,
        BinaryExpression,
    }
}