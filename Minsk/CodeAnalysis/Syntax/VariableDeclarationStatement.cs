namespace Minsk.CodeAnalysis.Syntax
{

        // var x = 10
        // let x = 10
        public sealed class VariableDeclarationStatement : StatementSyntax
        {
            public VariableDeclarationStatement(SyntaxToken keyword, SyntaxToken identifier, SyntaxToken equalsToken, ExpressionSyntax initializer)
            {
                Keyword = keyword;
                Identifier = identifier;
                EqualsToken = equalsToken;
                Initializer = initializer;
            }

            public override SyntaxKind Kind => SyntaxKind.VariableDeclaration;

            public SyntaxToken Keyword { get; }
            public SyntaxToken Identifier { get; }
            public SyntaxToken EqualsToken { get; }
            public ExpressionSyntax Initializer { get; }
        }

}