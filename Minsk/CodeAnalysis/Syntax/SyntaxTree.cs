using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using VNC;

namespace Minsk.CodeAnalysis.Syntax
{
    // NOTE(crhodes)
    // SyntaxTree represent entire collection of nodes.

    public sealed class SyntaxTree
    {
        public SyntaxTree(ImmutableArray<Diagnostic> diagnostics, ExpressionSyntax root, SyntaxToken endOfFileToken)
        {
            Int64 startTicks = Log.CONSTRUCTOR($"Enter: diagnostics: {diagnostics} root:{root} endOfFileToken:{endOfFileToken}", Common.LOG_CATEGORY);

            Diagnostics = diagnostics;
            Root = root;
            EndOfFileToken = endOfFileToken;

            Log.CONSTRUCTOR($"Exit", Common.LOG_CATEGORY, startTicks);
        }

        public ImmutableArray<Diagnostic> Diagnostics { get; }
        public ExpressionSyntax Root { get; }
        public SyntaxToken EndOfFileToken { get; }

        public static SyntaxTree Parse(string text)
        {
            var parser = new Parser(text);

            return parser.Parse();
        }

        // NOTE(crhodes)
        // This allows us to test Lexer without having to publicly expose.

        public static IEnumerable<SyntaxToken> ParseTokens(string text)
        {
            var lexer = new Lexer(text);

            while (true)
            {
                var token = lexer.Lex();

                if (token.Kind == SyntaxKind.EndOfFileToken) break;

                yield return token;
            }
        }
    }
}