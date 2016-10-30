using System.Collections.Generic;
using Core;
using TypeTheory.CallByPushValue;

namespace TypeTheory
{
    public static class Environments
    {
        public static IExpression<Id, Id, Unit> Lookup<Id>(this Sequence<IExpression<Id, Id, Id>> environment, Id name)
        {
            while (environment.Tag != Sequence<IExpression<Id, Id, Id>>.Tags.Empty)
            {
                var node = (Sequence<IExpression<Id, Id, Id>>.Node)environment;

                if (node.Head.Term.Equals(name))
                {
                    return node.Head.Fmap(_ => Unit.Singleton);
                }

                environment = node.Tail;
            }

            throw new KeyNotFoundException("Variable is not declared: " + name);
        }

    }
}