namespace Minsk.CodeAnalysis.Binding
{
    internal enum BoundNodeKind
    {
        // Statements

        BlockStatement,
        VariableDeclaration,
        ExpressionStatement,

        // Expressions

        BinaryExpression,
        LiteralExpression,
        VariableExpression,
        AssignmentExpression,
        UnaryExpression,

    }
}
