namespace Minsk.CodeAnalysis.Binding
{
    internal enum BoundNodeKind
    {
        // statements

        BlockStatement,
        ExpressionStatement,

        // Expressions

        LiteralExpression,
        VariableExpression,
        AssignmentExpression,
        UnaryExpression,
        BinaryExpression,

    }
}
