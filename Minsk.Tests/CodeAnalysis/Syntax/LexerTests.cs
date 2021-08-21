using System.Collections.Generic;
using System.Configuration;
using System.Linq;

using Minsk.CodeAnalysis.Syntax;

using Xunit;

namespace Minsk.Tests.CodeAnalysis.Syntax
{
    public class LexerTests
    {
        [Theory]
        [MemberData(nameof(GetTokensData))]
        public void Lexer_Lexes_Token(SyntaxKind kind, string text)
        {
            var tokens = SyntaxTree.ParseTokens(text);

            // NOTE(crhodes)
            // Where the heck do we put the app.config stuff so Logging works in code under test.
            //string path = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None).FilePath;

            var token = Assert.Single(tokens);
            Assert.Equal(kind, token.Kind);
            Assert.Equal(text, token.Text);
        }

        [Theory]
        [MemberData(nameof(GetTokenPairsData))]
        public void Lexer_Lexes_TokenPairs(SyntaxKind t1Kind, string t1Text, SyntaxKind t2Kind, string t2Text)
        {
            var text = t1Text + t2Text;

            var tokens = SyntaxTree.ParseTokens(text).ToArray();

            Assert.Equal(2, tokens.Length);

            Assert.Equal(tokens[0].Kind, t1Kind);
            Assert.Equal(tokens[0].Text, t1Text);

            Assert.Equal(tokens[1].Kind, t2Kind);
            Assert.Equal(tokens[1].Text, t2Text);
        }

        [Theory]
        [MemberData(nameof(GetTokenPairsWithSeparatorData))]
        public void Lexer_Lexes_TokenPairs_WithSeparator(
            SyntaxKind t1Kind, string t1Text,
            SyntaxKind separatorKind, string  separatorText,
            SyntaxKind t2Kind, string t2Text)
        {
            var text = t1Text + separatorText + t2Text;

            var tokens = SyntaxTree.ParseTokens(text).ToArray();

            Assert.Equal(3, tokens.Length);

            Assert.Equal(tokens[0].Kind, t1Kind);
            Assert.Equal(tokens[0].Text, t1Text);

            Assert.Equal(tokens[1].Kind, separatorKind);
            Assert.Equal(tokens[1].Text, separatorText);

            Assert.Equal(tokens[2].Kind, t2Kind);
            Assert.Equal(tokens[2].Text, t2Text);
        }

        public static IEnumerable<object[]> GetTokensData()
        {
            foreach (var token in GetTokens().Concat(GetSeparators()))
            {
                yield return new object[] { token.kind, token.text };
            }
        }

        public static IEnumerable<object[]> GetTokenPairsData()
        {
            foreach (var t in GetTokenPairs())
            {
                yield return new object[] { t.t1Kind, t.t1Text, t.t2Kind, t.t2Text };
            }
        }

        public static IEnumerable<object[]> GetTokenPairsWithSeparatorData()
        {
            foreach (var t in GetTokenPairsWithSeparator())
            {
                yield return new object[] { t.t1Kind, t.t1Text, t.separatorKind, t.separatorText, t.t2Kind, t.t2Text };
            }
        }

        private static IEnumerable<(SyntaxKind kind, string text)> GetTokens()
        {
            return new[]
            {
                // Tokens

                (SyntaxKind.BangToken, "!"),
                (SyntaxKind.EqualsToken, "="),
                (SyntaxKind.AmpersandAmpersandToken, "&&"),
                (SyntaxKind.PipePipeToken, "||"),
                (SyntaxKind.EqualsEqualsToken, "=="),
                (SyntaxKind.BangEqualsToken, "!="),

                // Operators

                (SyntaxKind.PlusToken, "+"),
                (SyntaxKind.MinusToken, "-"),
                (SyntaxKind.StarToken, "*"),
                (SyntaxKind.SlashToken, "/"),
                (SyntaxKind.OpenParenthesisToken, "("),
                (SyntaxKind.CloseParenthesisToken, ")"),


                (SyntaxKind.IdentifierToken, "a"),
                (SyntaxKind.IdentifierToken, "abc"),
                //(SyntaxKind.IdentifierToken, "1A2"),
                //(SyntaxKind.IdentifierToken, "A12"),

                // NOTE(crhodes)
                // Will test WhiteSpace differently

                //(SyntaxKind.WhiteSpaceToken, " "),
                //(SyntaxKind.WhiteSpaceToken, "  "),
                //(SyntaxKind.WhiteSpaceToken, "\r"),
                //(SyntaxKind.WhiteSpaceToken, "\r\n"),
                //(SyntaxKind.WhiteSpaceToken, "\n"),

                (SyntaxKind.NumberToken, "1"),
                (SyntaxKind.NumberToken, "123"),

                // Keywords

                (SyntaxKind.FalseKeyWord, "false"),
                (SyntaxKind.TrueKeyWord, "true")
            };
        }

        private static IEnumerable<(SyntaxKind kind, string text)> GetSeparators()
        {
            return new[]
            {
                // NOTE(crhodes)
                // Will test WhiteSpace differently

                (SyntaxKind.WhiteSpaceToken, " "),
                (SyntaxKind.WhiteSpaceToken, "  "),
                (SyntaxKind.WhiteSpaceToken, "\r"),
                (SyntaxKind.WhiteSpaceToken, "\r\n"),
                (SyntaxKind.WhiteSpaceToken, "\n"),
            };
        }

        private static bool RequiresSeparator(SyntaxKind t1Kind, SyntaxKind t2Kind)
        {
            // HACK(crhodes)
            // Massive hack for now :)

            var t1IsKeyword = t1Kind.ToString().EndsWith("KeyWord");
            var t2IsKeyword = t2Kind.ToString().EndsWith("KeyWord");

            if (t1Kind == SyntaxKind.IdentifierToken
                && t2Kind == SyntaxKind.IdentifierToken)
            {
                return true;
            }

            if (t1IsKeyword && t2IsKeyword)
            {
                return true;
            }

            if (t1IsKeyword && t2Kind == SyntaxKind.IdentifierToken)
            {
                return true;
            }

            if (t1Kind == SyntaxKind.IdentifierToken && t2IsKeyword)
            {
                return true;
            }

            if (t1Kind == SyntaxKind.NumberToken
                && t2Kind == SyntaxKind.NumberToken)
            {
                return true;
            }

            if (t1Kind == SyntaxKind.BangToken
                && t2Kind == SyntaxKind.EqualsToken)
            {
                return true;
            }


            if (t1Kind == SyntaxKind.BangToken
                && t2Kind == SyntaxKind.EqualsEqualsToken)
            {
                return true;
            }

            if (t1Kind == SyntaxKind.EqualsToken
                && t2Kind == SyntaxKind.EqualsToken)
            {
                return true;
            }

            if (t1Kind == SyntaxKind.EqualsToken
                && t2Kind == SyntaxKind.EqualsEqualsToken)
            {
                return true;
            }

            // TODO(crhodes)
            // More cases

            return false;
        }

        private static IEnumerable<(SyntaxKind t1Kind, string t1Text, SyntaxKind t2Kind, string t2Text)> GetTokenPairs()
        {
            foreach (var t1 in GetTokens())
            {
                foreach (var t2 in GetTokens())
                {
                    if (!RequiresSeparator(t1.kind, t2.kind))
                    {
                        yield return (t1.kind, t1.text, t2.kind, t2.text);
                    }
                }
            }
        }

        private static IEnumerable<(
            SyntaxKind t1Kind, string t1Text,
            SyntaxKind separatorKind, string separatorText,
            SyntaxKind t2Kind, string t2Text)> GetTokenPairsWithSeparator()
        {
            foreach (var t1 in GetTokens())
            {
                foreach (var t2 in GetTokens())
                {
                    if (RequiresSeparator(t1.kind, t2.kind))
                    {
                        foreach (var separator in GetSeparators())
                        {
                            yield return (t1.kind, t1.text, separator.kind, separator.text, t2.kind, t2.text);
                        }
                    }
                }
            }
        }
    }
}