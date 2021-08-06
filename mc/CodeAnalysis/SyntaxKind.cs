namespace Minsk.CodeAnalysis
{
    public enum SyntaxKind
    {
        // Tokens

        BadToken,
        EndOfFileToken,
        WhiteSpaceToken,
        NumberToken,

        // Operators

        PlusToken,
        MinusToken,
        StarToken,
        SlashToken,
        OpenParenthesisToken,
        CloseParenthesisToken,

        // Expressions

        LiteralExpression,
        BinaryExpression,
        ParenthesizedExpression
    }
}