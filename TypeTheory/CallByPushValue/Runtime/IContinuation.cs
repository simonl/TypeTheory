using System;

namespace TypeTheory.CallByPushValue
{
    public interface IContinuation<R>
    {
        R Throw(Value<R> argument);
    }

    public sealed class Continuation<R> : IContinuation<R>
    {
        public readonly Func<Value<R>, R> ThrowF;

        public Continuation(Func<Value<R>, R> throwF)
        {
            ThrowF = throwF;
        }

        public R Throw(Value<R> argument)
        {
            return ThrowF(argument);
        }
    }
}