using System;

namespace TypeTheory.CallByPushValue
{
    public abstract class Value<R>
    {
        public sealed class Continuation : Value<R>
        {
            public readonly IContinuation<R> Content;

            public Continuation(IContinuation<R> content)
            {
                Content = content;
            }
        }

        public sealed class Pair : Value<R>
        {
            public readonly Value<R> Left;
            public readonly Value<R> Right;

            public Pair(Value<R> left, Value<R> right)
            {
                Left = left;
                Right = right;
            }
        }

        public sealed class Builtin<T> : Value<R>
        {
            public readonly T Content;

            public Builtin(T content)
            {
                Content = content;
            }
        } 
    }
}