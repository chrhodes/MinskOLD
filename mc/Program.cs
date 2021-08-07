using Minsk.CodeAnalysis;
using Minsk.CodeAnalysis.Binding;
using Minsk.CodeAnalysis.Syntax;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using VNC;

namespace Minsk
{
    partial class Program
    {
        static void Main(string[] args)
        {
            Int64 startTicks = Log.APPLICATION_START($"SignalR Startup Delay", Common.LOG_CATEGORY);
            Thread.Sleep(200);
            startTicks = Log.APPLICATION_START($"Enter", Common.LOG_CATEGORY, startTicks);

            // NOTE(crhodes)
            // Console Directive

            bool showTree = false;

            while (true)
            {
                Console.Write("> ");

                var line = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(line))
                {
                    Log.APPLICATION_START($"Exit - No Input Received", Common.LOG_CATEGORY, startTicks);

                    return;
                }

                if (line == "#showTree")
                {
                    showTree = !showTree;
                    Console.WriteLine(showTree ? "Showing parse trees." : "Not showing parse trees");
                    continue;
                }
                else if (line == "#cls")
                {
                    Console.Clear();
                    continue;
                }

                var syntaxTree = SyntaxTree.Parse(line);
                var binder = new Binder();
                var boundExpression = binder.BindExpression(syntaxTree.Root);

                if (showTree)
                {
                    var color = Console.ForegroundColor;

                    Console.ForegroundColor = ConsoleColor.DarkGreen;

                    PrettyPrint2(syntaxTree.Root);

                    Console.ForegroundColor = color;
                }

                IReadOnlyList<string> diagnostics = syntaxTree.Diagnostics.Concat(binder.Diagnostics).ToArray();

                if (!diagnostics.Any())
                {
                    var e = new Evaluator(boundExpression);

                    var result = e.Evaluate();
                    Console.WriteLine(result);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;

                    foreach (var diagnostic in diagnostics)
                    {
                        Console.WriteLine(diagnostic);
                    }

                    Console.ResetColor();
                }

                Log.APPLICATION_START($"Exit", Common.LOG_CATEGORY, startTicks);
            }
        }

        static void PrettyPrint1(SyntaxNode node, string indent = "")
        {
            Console.Write(indent);
            Console.Write(node.Kind);

            if (node is SyntaxToken t && t.Value != null)
            {
                Console.Write(" ");
                Console.Write(t.Value);
            }

            Console.WriteLine();

            indent += "   ";

            foreach (var child in node.GetChildren())
            {
                PrettyPrint1(child, indent);
            }
        }

        static void PrettyPrint2(SyntaxNode node, string indent = "", bool isLast = true)
        {
            // Unix https://en.wikipedia.org/wiki/Tree_(command)
            // └──
            // ├──
            // │

            var marker = isLast ? "└──" : "├──";

            Console.Write(indent);
            Console.Write(marker);
            Console.Write(node.Kind);

            if (node is SyntaxToken t && t.Value != null)
            {
                Console.Write(" ");
                Console.Write(t.Value);
            }

            Console.WriteLine();

            indent += isLast ? "   " : "│   ";

            var lastChild = node.GetChildren().LastOrDefault();

            foreach (var child in node.GetChildren())
            {
                PrettyPrint2(child, indent, child == lastChild);
            }
        }
    }
}
