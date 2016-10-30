using System;
using Core;

namespace TypeTheory.CallByPushValue
{
    public interface IEvaluator<R>
    {
        R Evaluate(Sequence<Value<R>> locals, Value<R> argument);
    }

    public sealed class Evaluator<R> : IEvaluator<R>
    {
        private readonly Func<Sequence<Value<R>>, Value<R>, R> EvaluateF;

        public Evaluator(Func<Sequence<Value<R>>, Value<R>, R> evaluateF)
        {
            EvaluateF = evaluateF;
        }

        public R Evaluate(Sequence<Value<R>> locals, Value<R> argument)
        {
            return EvaluateF(locals, argument);
        }
    }
}