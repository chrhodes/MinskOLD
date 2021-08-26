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

        FalseKeyword,
        TrueKeyword,
        LetKeyword,
        VarKeyword,

        // Nodes

        CompilationUnit,

        // Statements

        BlockStatement,
        VariableDeclaration,
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