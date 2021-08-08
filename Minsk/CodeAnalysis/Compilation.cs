using System;
using System.Linq;

using Minsk.CodeAnalysis.Binding;
using Minsk.CodeAnalysis.Syntax;

using VNC;

namespace Minsk.CodeAnalysis
{
    public class Compilation
    {

        public Compilation(SyntaxTree syntax)
        {
            Int64 startTicks = Log.CONSTRUCTOR($"Enter: syntax:{syntax}", Common.LOG_CATEGORY);

            Syntax = syntax;

            Log.CONSTRUCTOR($"Exit", Common.LOG_CATEGORY, startTicks);
        }

        public SyntaxTree Syntax { get; }

        public EvaluationResult Evaluate()
        {
            Int64 startTicks = Log.Trace($"Enter", Common.LOG_CATEGORY);

            var binder = new Binder();
            var boundExpression = binder.BindExpression(Syntax.Root);

            var diagnostics = Syntax.Diagnostics.Concat(binder.Diagnostics).ToArray();

            if (diagnostics.Any())
            {
                Log.Trace($"Exit", Common.LOG_CATEGORY, startTicks);
                return new EvaluationResult(diagnostics, null);
            }

            var evaluator = new Evaluator(boundExpression);
            var value = evaluator.Evaluate();

            Log.Trace($"Exit", Common.LOG_CATEGORY, startTicks);

            return new EvaluationResult(Array.Empty<string>(), value);
        }
    }
}