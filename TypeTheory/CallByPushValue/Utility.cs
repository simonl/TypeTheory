using System;
using Core;

namespace TypeTheory.CallByPushValue
{
    public static class Utility
    {
        public static IExpression<Bind, Id, T> Qualify<Bind, Id, T>(this IUniverse universe, T term)
        {
            var type = new Term<Bind, Id>(new TermF<Bind, Id, ITerm<Bind, Id>>.Universe(universe));

            return new Expression<Bind, Id, T>(
                universe: universe.TypeOf(),
                type: type,
                term: term);
        }

        public static IUniverse TypeOf(this IUniverse universe)
        {
            return new Universe(universe.Rank + 1, polarity: null);
        }

        public static IExpression<Bind, Id, ITerm<Bind, Id>> TypeOf<Bind, Id, T>(this IExpression<Bind, Id, T> expression)
        {
            return Qualify<Bind, Id, ITerm<Bind, Id>>(
                universe: expression.Universe,
                term: expression.Type);
        }

        public static IUniverse Dual(this IUniverse universe)
        {
            if (universe.Polarity == null)
            {
                throw new ArgumentException("Universe must describe a polarized type.");
            }

            var polarity = Dual(universe.Polarity.Value);

            return new Universe(universe.Rank, polarity);
        }

        public static Polarity Dual(this Polarity polarity)
        {
            switch (polarity)
            {
                case Polarity.Forall:

                    return Polarity.Exists;
                case Polarity.Exists:

                    return Polarity.Forall;
                default:
                    throw new InvalidProgramException("Should never happen.");
            }
        }

        public static IExpression<Bind, Id, B> Fmap<Bind, Id, A, B>(this IExpression<Bind, Id, A> expression, Func<A, B> convert)
        {
            return new Expression<Bind, Id, B>(
                universe: expression.Universe,
                type: expression.Type,
                term: convert(expression.Term));
        }

        public static IAnnotated<Bind, Id, T> TopLevel<Bind, Id, T>(this IExpression<Bind, Id, T> expression)
        {
            var environment = new Sequence<IExpression<Bind, Id, Bind>>.Empty();

            return expression.Annotate(environment);
        }

        public static IAnnotated<Bind, Id, T> Annotate<Bind, Id, T>(this IExpression<Bind, Id, T> expression, Sequence<IExpression<Bind, Id, Bind>> environment)
        {
            return new Annotated<Bind, Id, T>(
                environment: environment,
                expression: expression);
        }
    }
}