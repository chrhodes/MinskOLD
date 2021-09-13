using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Text;

using Minsk.CodeAnalysis.Text;

namespace Minsk.Tests.CodeAnalysis
{
    internal sealed class AnnotatedText
    {
        //  {
        //     let x = 10
        //     x = 10   expect assignment error here
        //  }

        public AnnotatedText(string text, ImmutableArray<TextSpan> spans)
        {
            Text = text;
            Spans = spans;
        }

        public string Text { get; }
        public ImmutableArray<TextSpan> Spans { get; }

        public static AnnotatedText Parse(string text)
        {
            text = Unindent(text);

            //var text @"
            //    {
            //        var x = 10
            //        let x = 2
            //    }
            //    ";

            var textBuilder = new StringBuilder();
            var spanBuilder = ImmutableArray.CreateBuilder<TextSpan>();
            var startStack = new Stack<int>();

            foreach (var c in text)
            {
                
            }

            return new AnnotatedText(textBuilder.ToString(), spanBuilder.ToImmutable());
        }

        private static string Unindent(string text)
        {
            var lines = new List<string>();

            using (var reader = new StringReader(text))
            {
                string line;

                while ((line = reader.ReadLine()) != null)
                {
                    lines.Add(line);
                }
            }

            var minIndentation = int.MaxValue;

            for (int i = 0; i < lines.Count; i++)
            {
                var line = lines[i];

                if (line.Trim().Length == 0)
                {
                    lines[i] = string.Empty;
                    continue;
                }

                var indentation = line.TrimStart().Length;
                minIndentation = Math.Min(minIndentation, indentation);
            }

            for (int i = 0; i < lines.Count; i++)
            {
                if (lines[i].Length == 0)
                {
                    continue;
                }

                lines[i] = lines[i].Substring(minIndentation);
            }

            while (lines.Count > 0
                && lines[0].Length ==0)
            {
                lines.RemoveAt(0);
            }

            return string.Join(Environment.NewLine, lines);
        }
    }
}
