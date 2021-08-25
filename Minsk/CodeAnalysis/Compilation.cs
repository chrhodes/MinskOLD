using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;

using Minsk.CodeAnalysis.Binding;
using Minsk.CodeAnalysis.Syntax;

using VNC;

namespace Minsk.CodeAnalysis
{
    public class Compilation
    {
        private BoundGlobalScope _globalScope;

        public Compilation(SyntaxTree syntaxTree)
            : this(null, syntaxTree)
        {
            Int64 startTicks = Log.CONSTRUCTOR($"Enter: syntax:{syntaxTree}", Common.LOG_CATEGORY);

            SyntaxTree = syntaxTree;

            Log.CONSTRUCTOR($"Exit", Common.LOG_CATEGORY, startTicks);
        }

        private Compilation(Compilation previous, SyntaxTree syntaxTree)
        {
            Int64 startTicks = Log.CONSTRUCTOR($"Enter: syntax:{syntaxTree}", Common.LOG_CATEGORY);
            Previous = previous;
            SyntaxTree = syntaxTree;

            Log.CONSTRUCTOR($"Exit", Common.LOG_CATEGORY, startTicks);
        }

        public Compilation Previous { get; }
        public SyntaxTree SyntaxTree { get; }

        internal BoundGlobalScope GlobalScope
        {
            get
            {
                // Not a good idea for thread safety

                //if (_globalScope == null)
                //{
                //    _globalScope = Binder.BindGlobalScope(SyntaxTree.Root);
                //}

                if (_globalScope == null)
                {
                    var globalScope = Binder.BindGlobalScope(Previous?.GlobalScope, SyntaxTree.Root);
                    Interlocked.CompareExchange(ref _globalScope, globalScope, null);
                }

                return _globalScope;
            }
        }

        public Compilation ContinueWith(SyntaxTree syntaxTree)
        {
            return new Compilation(this, syntaxTree);
        }

        public EvaluationResult Evaluate(Dictionary<VariableSymbol, object> variables)
        {
            Int64 startTicks = Log.COMPILER($"Enter", Common.LOG_CATEGORY);

            var diagnostics = SyntaxTree.Diagnostics.Concat(GlobalScope.Diagnostics).ToImmutableArray();

            if (diagnostics.Any())
            {
                Log.COMPILER($"Exit", Common.LOG_CATEGORY, startTicks);

                return new EvaluationResult(diagnostics, null);
            }

            var evaluator = new Evaluator(GlobalScope.Expression, variables);
            var value = evaluator.Evaluate();

            Log.COMPILER($"Exit", Common.LOG_CATEGORY, startTicks);

            return new EvaluationResult(ImmutableArray<Diagnostic>.Empty, value);
        }
    }
}