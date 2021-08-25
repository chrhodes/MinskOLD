using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using Minsk.CodeAnalysis.Binding;
using Minsk.CodeAnalysis.Syntax;

using VNC;

namespace Minsk.CodeAnalysis
{
    public class Compilation
    {

        public Compilation(SyntaxTree syntaxTree)
        {
            Int64 startTicks = Log.CONSTRUCTOR($"Enter: syntax:{syntaxTree}", Common.LOG_CATEGORY);

            SyntaxTree = syntaxTree;

            Log.CONSTRUCTOR($"Exit", Common.LOG_CATEGORY, startTicks);
        }

        public SyntaxTree SyntaxTree { get; }

        public EvaluationResult Evaluate(Dictionary<VariableSymbol, object> variables)
        {
            Int64 startTicks = Log.COMPILER($"Enter", Common.LOG_CATEGORY);

            var binder = new Binder(variables);
            var boundExpression = binder.BindExpression(SyntaxTree.Root.Expression);

            var diagnostics = SyntaxTree.Diagnostics.Concat(binder.Diagnostics).ToImmutableArray();

            if (diagnostics.Any())
            {
                Log.COMPILER($"Exit", Common.LOG_CATEGORY, startTicks);

                return new EvaluationResult(diagnostics, null);
            }

            var evaluator = new Evaluator(boundExpression, variables);
            var value = evaluator.Evaluate();

            Log.COMPILER($"Exit", Common.LOG_CATEGORY, startTicks);

            return new EvaluationResult(ImmutableArray<Diagnostic>.Empty, value);
        }
    }
}