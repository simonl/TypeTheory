using System;

namespace TypeTheory.CallByPushValue
{
    public static class Scoping
    {
        /*
        public static IQualified<Unit, uint, Term<Unit, uint>> Debruijn(IQualified<Guid, Guid, Term<Guid, Guid>> expression)
        {
            var qualifiedType = TypeOf(expression);

            var type = Debruijn(new Sequence<Guid>.Empty(), qualifiedType);

            var qualifiedTerm = new Qualified<Unit, uint, Term<Guid, Guid>>(expression.Universe, type, expression.Term);

            var term = Debruijn(new Sequence<Guid>.Empty(), qualifiedTerm);

            return new Qualified<Unit, uint, Term<Unit, uint>>(expression.Universe, type, term);
        }

        private static IQualified<Unit, uint, Term<Bind, Id>> TypeOf<Bind, Id, T>(IQualified<Bind, Id, T> expression)
        {
            return expression.Universe.Qualify<Unit, uint, Term<Bind, Id>>(term: expression.Type);
        }

        public static Term<Unit, uint> Debruijn<Id>(Sequence<Id> environment, IQualified<Unit, uint, Term<Id, Id>> expression)
        {
            switch (expression.Term.Content.Tag)
            {
                case TermF<Id, Id, Term<Id, Id>>.Tags.Variable:
                    var variable = (TermF<Id, Id, Term<Id, Id>>.Variable)expression.Term.Content;

                    var index = environment.IndexOf(variable.Identifier);

                    return new Term<Unit, uint>.Variable(index);
                case Term<Id, Id>.Tags.Universe:
                    var universe = (Term<Id, Id>.Universe) expression.Term;

                    return new Term<Unit, uint>.Universe(universe.Order);
                case Term<Id, Id>.Tags.Type:
                    var type = (Term<Id, Id>.Type) expression.Term;

                    switch (type.Class.Tag)
                    {
                        case Class<Id, Id>.Tags.Quantifier:
                            var quantifier = (Class<Id, Id>.Quantifier) type.Class;

                            var dependency = Debruijn(environment, TypeOf(quantifier.Dependency));

                            environment = environment.Push(quantifier.Dependency.Term);

                            var dependent = Debruijn(environment, expression.Fmap(_ => quantifier.Dependent));

                            return new Term<Unit, uint>.Type(
                                @class: new Class<Unit, uint>.Quantifier(
                                    dependency: new Qualified<Unit, uint, Unit>(quantifier.Dependency.Universe, dependency, Unit.Singleton),
                                    dependent: dependent));
                        case Class<Id, Id>.Tags.Shift:
                            var shift = (Class<Id, Id>.Shift) type.Class;

                            var normal = (Term<Unit, uint>.Universe) TypeChecking.Normalize(null, Utility.TypeOf(expression));

                            var qualifiedContent = new Qualified<Unit, uint, Term<Id, Id>>(
                                universe: expression.Universe,
                                type: TypeChecking.Dual(normal),
                                term: shift.Content);

                            return new Term<Unit, uint>.Type( @class: new Class<Unit, uint>.Shift(Debruijn(environment, qualifiedContent)));
                        default:
                            throw new InvalidProgramException("Should never happen.");
                    }

                    break;
                case Term<Id, Id>.Tags.Constructor:
                    throw new InvalidProgramException("Should never happen.");
                case Term<Id, Id>.Tags.Destructor:
                    throw new InvalidProgramException("Should never happen.");
                default:
                    throw new InvalidProgramException("Should never happen.");
            }
        }
         */
    }
}