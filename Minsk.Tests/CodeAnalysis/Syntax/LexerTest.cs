using System;
using System.Collections.Generic;
using System.Configuration;

using Minsk.CodeAnalysis.Syntax;

using Xunit;

namespace Minsk.Tests.CodeAnalysis.Syntax
{
    public class LexerTest
    {
        [Theory]
        [MemberData(nameof(GetTokensData))]
        public void Lexer_Lexes_Token(SyntaxKind kind, string text)
        {
            var tokens = SyntaxTree.ParseTokens(text);

            string path = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None).FilePath;

            var token = Assert.Single(tokens);
            Assert.Equal(kind, token.Kind);
            Assert.Equal(text, token.Text);
        }

        public static IEnumerable<object[]> GetTokensData()
        {
            foreach (var token in GetTokens())
            {
                yield return new object[] { token.kind, token.text };
            }
        }

        private static IEnumerable<(SyntaxKind kind, string text)> GetTokens()
        {
            return new[]
            {
                (SyntaxKind.IdentifierToken, "a"),
                (SyntaxKind.IdentifierToken, "abc")
            };
        }
    }
}
