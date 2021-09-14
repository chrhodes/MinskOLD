namespace Minsk.CodeAnalysis.Syntax
{
    public enum SyntaxKind
    {
        // Tokens

        BadToken,
        EndOfFileToken,
        WhiteSpaceToken,
        NumberToken,
        IdentifierToken,
        BangToken,
        EqualsToken,
        LessToken,
        LessOrEqualsToken,
        GreaterToken,
        GreaterOrEqualsToken,
        AmpersandAmpersandToken,
        PipePipeToken,
        EqualsEqualsToken,
        BangEqualsToken,

        // Operators

        PlusToken,
        MinusToken,
        StarToken,
        SlashToken,

        OpenParenthesisToken,
        CloseParenthesisToken,
        OpenBraceToken,
        CloseBraceToken,

        // Keywords

        ElseKeyword,
        FalseKeyword,
        IfKeyword,
        LetKeyword,
        TrueKeyword,
        VarKeyword,

        // Nodes

        CompilationUnit,
        ElseClause,

        // Statements

        BlockStatement,
        VariableDeclaration,
        IfStatement,
        ExpressionStatement,

        // Expressions

        LiteralExpression,
        NameExpression,
        UnaryExpression,
        BinaryExpression,
        ParenthesizedExpression,
        AssignmentExpression,

    }
}