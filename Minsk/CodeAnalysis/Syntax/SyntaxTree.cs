using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using Minsk.CodeAnalysis.Text;

using VNC;

namespace Minsk.CodeAnalysis.Syntax
{
    // NOTE(crhodes)
    // SyntaxTree represent entire collection of nodes.

    public sealed class SyntaxTree
    {
        private SyntaxTree(SourceText text)
        {
            Int64 startTicks = Log.CONSTRUCTOR($"Enter: text: ({text})", Common.LOG_CATEGORY);

            var parser = new Parser(text);

            var root = parser.ParseCompilationUnit();
            var diagnostics = parser.Diagnostics.ToImmutableArray();

            Text = text;
            Diagnostics = diagnostics;
            Root = root;

            Log.CONSTRUCTOR($"Exit", Common.LOG_CATEGORY, startTicks);
        }

        public SourceText Text { get; }
        public ImmutableArray<Diagnostic> Diagnostics { get; }
        public CompilationUnitSyntax Root { get; }

        public static SyntaxTree Parse(string text)
        {
            var sourceText = SourceText.From(text);

            return Parse(sourceText);
        }

        public static SyntaxTree Parse(SourceText text)
        {
            return new SyntaxTree(text);
        }

        // NOTE(crhodes)
        // This allows us to test Lexer without having to publicly expose.

        public static IEnumerable<SyntaxToken> ParseTokens(string text)
        {
            var sourceText = SourceText.From(text);
            return ParseTokens(sourceText);
        }

        public static IEnumerable<SyntaxToken> ParseTokens(SourceText text)
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