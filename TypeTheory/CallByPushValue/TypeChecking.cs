using System;
using System.Collections.Generic;
using Core;
using TypeTheory.ContinuationPassing;

namespace TypeTheory.CallByPushValue
{
    public static class TypeChecking
    {
        public static bool VerifyClosed<Id>(IExpression<Id, Id, ITerm<Id, Id>> expression)
        {
            var environment = new Sequence<IExpression<Id, Id, Id>>.Empty();

            return Verify(environment, expression);
        }

        private static bool Verify<Id>(Sequence<IExpression<Id, Id, Id>> environment, IExpression<Id, Id, ITerm<Id, Id>> expression)
        {
            return Check(environment, Utility.TypeOf(expression)) && Check(environment, expression);
        }

        private static bool Check<Id>(Sequence<IExpression<Id, Id, Id>> environment, IExpression<Id, Id, ITerm<Id, Id>> expression)
        {
            switch (expression.Term.Content.Production)
            {
                case Productions.Variable:
                    var variable = (TermF<Id, Id, ITerm<Id, Id>>.Variable)expression.Term.Content;

                    var signature = environment.Lookup(variable.Identifier);

                    if (expression.Universe.Rank == signature.Universe.Rank)
                    {
                        if (signature.Universe.Polarity == null || signature.Universe.Polarity == expression.Universe.Polarity)
                        {
                            return Equal(environment, Utility.TypeOf(expression), signature.Type);
                        }
                    }

                    return false;
                case Productions.Universe:
                    var universe = (TermF<Id, Id, ITerm<Id, Id>>.Universe)expression.Term.Content;
                    {
                        var normal = (TermF<Id, Id, ITerm<Id, Id>>.Universe)Normalize(environment, Utility.TypeOf(expression)).Content;

                        return normal.Order.Rank == universe.Order.Rank + 1;
                    }
                case Productions.Type:
                    var type = (TermF<Id, Id, ITerm<Id, Id>>.Type)expression.Term.Content;
                    {
                        var normal = (TermF<Id, Id, ITerm<Id, Id>>.Universe)Normalize(environment, Utility.TypeOf(expression)).Content;

                        if (normal.Order.Polarity != null)
                        {
                            switch (type.Class.Tag)
                            {
                                case Class<Id, Id, ITerm<Id, Id>>.Tags.Quantifier:
                                    var quantifier = (Class<Id, Id, ITerm<Id, Id>>.Quantifier)type.Class;

                                    var depPolarity = quantifier.Dependency.Universe.Polarity;

                                    if (depPolarity == null || depPolarity == Polarity.Exists)
                                    {
                                        if (Check(environment, Utility.TypeOf(quantifier.Dependency)))
                                        {
                                            return Check(environment.Push(quantifier.Dependency), expression.Fmap(_ => quantifier.Dependent));
                                        }
                                    }

                                    break;
                                case Class<Id, Id, ITerm<Id, Id>>.Tags.Shift:
                                    var shift = (Class<Id, Id, ITerm<Id, Id>>.Shift)type.Class;

                                    if (Check(environment, new Expression<Id, Id, ITerm<Id, Id>>(expression.Universe, Dual(normal), shift.Content)))
                                    {
                                        return true;
                                    }
                                    
                                    break;
                                default:
                                    throw new InvalidProgramException("Should never happen.");
                            }
                        }

                        return false;
                    }
                case Productions.Constructor:
                    var constructor = (TermF<Id, Id, ITerm<Id, Id>>.Constructor)expression.Term.Content;
                    {
                        var normal = (TermF<Id, Id, ITerm<Id, Id>>.Type)Normalize(environment, Utility.TypeOf(expression)).Content;

                        switch (expression.Universe.Polarity)
                        {
                            case Polarity.Forall:

                                switch (normal.Class.Tag)
                                {
                                    case Class<Id, Id, ITerm<Id, Id>>.Tags.Quantifier:
                                        var quantifier = (Class<Id, Id, ITerm<Id, Id>>.Quantifier)normal.Class;
                                        var lambda = (Initialization<Id, ITerm<Id, Id>>.Forall.Quantifier)constructor.Initialization;

                                        var parameter = quantifier.Dependency.Fmap(_ => lambda.Parameter);

                                        var @returnType = quantifier.Dependent.Substitute(quantifier.Dependency.Term, new Term<Id, Id>(new TermF<Id, Id, ITerm<Id, Id>>.Variable(lambda.Parameter)));

                                        return Check(environment.Push(parameter), new Expression<Id, Id, ITerm<Id, Id>>(expression.Universe, @returnType, lambda.Body));
                                    case Class<Id, Id, ITerm<Id, Id>>.Tags.Shift:
                                        var shift = (Class<Id, Id, ITerm<Id, Id>>.Shift)normal.Class;
                                        var @return = (Initialization<Id, ITerm<Id, Id>>.Forall.Shift)constructor.Initialization;

                                        return Check(environment, new Expression<Id, Id, ITerm<Id, Id>>(Utility.Dual(expression.Universe), shift.Content, @return.Body));
                                    default:
                                        throw new InvalidProgramException("Should never happen.");
                                }

                            case Polarity.Exists:

                                switch (normal.Class.Tag)
                                {
                                    case Class<Id, Id, ITerm<Id, Id>>.Tags.Quantifier:
                                        var quantifier = (Class<Id, Id, ITerm<Id, Id>>.Quantifier)normal.Class;
                                        var pair = (Initialization<Id, ITerm<Id, Id>>.Exists.Quantifier)constructor.Initialization;

                                        if (Check(environment, quantifier.Dependency.Fmap(_ => pair.Left)))
                                        {
                                            var @rightType = quantifier.Dependent.Substitute(quantifier.Dependency.Term, pair.Left);

                                            return Check(environment, new Expression<Id, Id, ITerm<Id, Id>>(expression.Universe, @rightType, pair.Right));
                                        }

                                        return false;
                                    case Class<Id, Id, ITerm<Id, Id>>.Tags.Shift:
                                        var shift = (Class<Id, Id, ITerm<Id, Id>>.Shift)normal.Class;
                                        var delay = (Initialization<Id, ITerm<Id, Id>>.Exists.Shift)constructor.Initialization;

                                        return Check(environment, new Expression<Id, Id, ITerm<Id, Id>>(Utility.Dual(expression.Universe), shift.Content, delay.Body));
                                    default:
                                        throw new InvalidProgramException("Should never happen.");
                                }

                            default:
                                throw new InvalidProgramException("Should never happen.");
                        }
                    }
                case Productions.Destructor:
                    var destructor = (TermF<Id, Id, ITerm<Id, Id>>.Destructor)expression.Term.Content;

                    if (Verify(environment, destructor.Focus))
                    {
                        var focus = (TermF<Id, Id, ITerm<Id, Id>>.Type)Normalize(environment, Utility.TypeOf(destructor.Focus)).Content;

                        switch (destructor.Focus.Universe.Polarity)
                        {
                            case Polarity.Forall:

                                switch (focus.Class.Tag)
                                {
                                    case Class<Id, Id, ITerm<Id, Id>>.Tags.Quantifier:
                                        var quantifier = (Class<Id, Id, ITerm<Id, Id>>.Quantifier)focus.Class;
                                        var application = (Continuation<Id, ITerm<Id, Id>>.Forall.Quantifier)destructor.Continuation;

                                        if (Check(environment, quantifier.Dependency.Fmap(_ => application.Argument)))
                                        {
                                            var @returnType = quantifier.Dependent.Substitute(quantifier.Dependency.Term, application.Argument);

                                            return Equal(environment, Utility.TypeOf(expression), @returnType);
                                        }

                                        return false;
                                    case Class<Id, Id, ITerm<Id, Id>>.Tags.Shift:
                                        var shift = (Class<Id, Id, ITerm<Id, Id>>.Shift)focus.Class;
                                        var extract = (Continuation<Id, ITerm<Id, Id>>.Forall.Shift)destructor.Continuation;

                                        environment = environment
                                            .Push(new Expression<Id, Id, Id>(Utility.Dual(destructor.Focus.Universe), shift.Content, extract.Identifier));

                                        return Check(environment, expression.Fmap(_ => extract.Body));
                                    default:
                                        throw new InvalidProgramException("Should never happen.");
                                }

                            case Polarity.Exists:

                                switch (focus.Class.Tag)
                                {
                                    case Class<Id, Id, ITerm<Id, Id>>.Tags.Quantifier:
                                        var quantifier = (Class<Id, Id, ITerm<Id, Id>>.Quantifier)focus.Class;
                                        var extract = (Continuation<Id, ITerm<Id, Id>>.Exists.Quantifier)destructor.Continuation;

                                        var rightType = quantifier.Dependent.Substitute(quantifier.Dependency.Term, new Term<Id, Id>(new TermF<Id, Id, ITerm<Id, Id>>.Variable(extract.Left)));

                                        environment = environment
                                            .Push(quantifier.Dependency.Fmap(_ => extract.Left))
                                            .Push(new Expression<Id, Id, Id>(destructor.Focus.Universe, rightType, extract.Right));

                                        return Check(environment, expression.Fmap(_ => extract.Body));
                                    case Class<Id, Id, ITerm<Id, Id>>.Tags.Shift:
                                        var shift = (Class<Id, Id, ITerm<Id, Id>>.Shift)focus.Class;
                                        var force = (Continuation<Id, ITerm<Id, Id>>.Exists.Shift)destructor.Continuation;

                                        if (expression.Universe.Rank == destructor.Focus.Universe.Rank)
                                        {
                                            return Equal(environment, Utility.TypeOf(expression), shift.Content);
                                        }

                                        return false;
                                    default:
                                        throw new InvalidProgramException("Should never happen.");
                                }

                            default:
                                throw new InvalidProgramException("Should never happen.");
                        }
                    }

                    return false;
                default:
                    throw new InvalidProgramException("Should never happen.");
            }
        }

        public static ITerm<Bind, Id> Dual<Bind, Id>(TermF<Bind, Id, ITerm<Bind, Id>>.Universe universe)
        {
            return new Term<Bind, Id>(new TermF<Bind, Id, ITerm<Bind, Id>>.Universe(Utility.Dual(universe.Order)));
        }

        private static ITerm<Id, Id> Substitute<Id>(this ITerm<Id, Id> expression, Id identifier, ITerm<Id, Id> term)
        {
            return expression;
        }

        private static bool Equal<Id>(Sequence<IExpression<Id, Id, Id>> environment, IExpression<Id, Id, ITerm<Id, Id>> first, ITerm<Id, Id> second)
        {
            return false;
        }

        public static ITerm<Id, Id> Normalize<Id>(Sequence<IExpression<Id, Id, Id>> environment, IExpression<Id, Id, ITerm<Id, Id>> expression)
        {
            return expression.Term;
        }

        public static ITerm<Unit, uint> Normalize(Sequence<IExpression<Unit, uint, Unit>> environment, IExpression<Unit, uint, ITerm<Unit, uint>> expression)
        {
            return expression.Term;
        }
    }
}