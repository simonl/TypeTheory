using System;
using Core;

namespace TypeTheory.CallByPushValue
{
    public static class Uniqueness
    {
        public static Expression<Guid, Guid, ITerm<Guid, Guid>> ConvertFully<Id>(IExpression<Id, Id, ITerm<Id, Id>> expression)
        {
            return new Expression<Guid, Guid, ITerm<Guid, Guid>>(
                universe: expression.Universe,
                type: Convert(expression.TypeOf()),
                term: Convert(expression));
        }

        private static Guid Fresh()
        {
            return Guid.NewGuid();
        }

        public static ITerm<Guid, Guid> Convert<Id>(IExpression<Id, Id, ITerm<Id, Id>> expression)
        {
            var traversal = new Traversal<Id, Id, Func<Sequence<Tuple<Id, Guid>>, ITerm<Guid, Guid>>>(
                useDeclarationF: identifier => new Term<Id, Id>(new TermF<Id, Id, ITerm<Id, Id>>.Variable(identifier)),
                stepF: ConversionStep<Id>);

            return traversal.Traverse(expression.TopLevel())(new Sequence<Tuple<Id, Guid>>.Empty());
        }

        private static Func<Sequence<Tuple<Id, Guid>>, ITerm<Guid, Guid>> ConversionStep<Id>(IAnnotated<Id, Id, TermF<Id, Id, Func<Sequence<Tuple<Id, Guid>>, ITerm<Guid, Guid>>>> annotated)
        {
            throw new NotImplementedException();
        }
    }
}