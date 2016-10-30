using System;
using Core;

namespace TypeTheory.CallByPushValue
{
    public sealed class Mapping<Bind, Id, T, TR>
    {
        private readonly Func<IAnnotated<Bind, Id, T>, TR> ConvertF;
        private readonly Func<Bind, T> UseDeclarationF;

        public Mapping(Func<IAnnotated<Bind, Id, T>, TR> convertF, Func<Bind, T> useDeclarationF)
        {
            ConvertF = convertF;
            UseDeclarationF = useDeclarationF;
        }

        private static ITerm<Bind, Id> Substitute<Bind, Id>(Sequence<IExpression<Bind, Id, Bind>> environment, IExpression<Bind, Id, Bind> declaration, ITerm<Bind, Id> expression, T instead)
        {
            return expression;
        }

        public IClosedTermF<Bind, Id, TR> Fmap(IClosedTermF<Bind, Id, T> annotated)
        {
            return new ClosedTermF<Bind, Id, TR>(annotated.Environment, annotated.Expression.Fmap(_ => Map(annotated)));
        }

        private TermF<Bind, Id, TR> Map(IClosedTermF<Bind, Id, T> annotated)
        {
            switch (annotated.Expression.Term.Production)
            {
                case Productions.Variable:
                {
                    var variable = (TermF<Bind, Id, T>.Variable)annotated.Expression.Term;

                    return new TermF<Bind, Id, TR>.Variable(variable.Identifier);
                }
                case Productions.Universe:
                {
                    var universe = (TermF<Bind, Id, T>.Universe)annotated.Expression.Term;

                    return new TermF<Bind, Id, TR>.Universe(universe.Order);
                }
                case Productions.Type:
                {
                    var type = (TermF<Bind, Id, T>.Type)annotated.Expression.Term;

                    var universe = (TermF<Bind, Id, ITerm<Bind, Id>>.Universe)annotated.Expression.Type.Content;

                    var @class = Map(annotated.Environment, universe.Order, type.Class);

                    return new TermF<Bind, Id, TR>.Type(@class);
                }
                case Productions.Constructor:
                {
                    var constructor = (TermF<Bind, Id, T>.Constructor)annotated.Expression.Term;

                    var type = (TermF<Bind, Id, ITerm<Bind, Id>>.Type)annotated.Expression.Type.Content;

                    var initialization = Map(annotated.Environment, annotated.Expression.Universe, type.Class, constructor.Initialization);

                    return new TermF<Bind, Id, TR>.Constructor(initialization);
                }
                case Productions.Destructor:
                {
                    var destructor = (TermF<Bind, Id, T>.Destructor)annotated.Expression.Term;

                    var focus = ConvertF(destructor.Focus.Annotate(annotated.Environment));

                    var qualifiedFocus = destructor.Focus.Fmap(_ => focus);

                    var type = (TermF<Bind, Id, ITerm<Bind, Id>>.Type)destructor.Focus.Type.Content;

                    var continuation = Map(annotated.Environment, destructor.Focus.Universe, type.Class, annotated.Expression.Fmap(_ => destructor.Continuation));

                    return new TermF<Bind, Id, TR>.Destructor(qualifiedFocus, continuation);
                }
                default:
                    throw new InvalidProgramException("Should never happen.");
            }
        }

        private Class<Bind, Id, TR> Map(Sequence<IExpression<Bind, Id, Bind>> environment, IUniverse universe, Class<Bind, Id, T> @class)
        {
            switch (@class.Tag)
            {
                case Class<Bind, Id, T>.Tags.Quantifier:
                    var quantifier = (Class<Bind, Id, T>.Quantifier) @class;

                    environment = environment.Push(quantifier.Dependency);

                    var qualifiedDependent = universe.Qualify<Bind, Id, T>(quantifier.Dependent);

                    var dependent = ConvertF(qualifiedDependent.Annotate(environment));

                    return new Class<Bind, Id, TR>.Quantifier(quantifier.Dependency, dependent);
                case Class<Bind, Id, T>.Tags.Shift:
                    var shift = (Class<Bind, Id, T>.Shift) @class;

                    var qualifiedContent = universe.Dual().Qualify<Bind, Id, T>(shift.Content);

                    var content = ConvertF(qualifiedContent.Annotate(environment));

                    return new Class<Bind, Id, TR>.Shift(content);
                default:
                    throw new InvalidProgramException("Should never happen.");
            }
        }

        private Initialization<Bind, TR> Map(Sequence<IExpression<Bind, Id, Bind>> environment, IUniverse universe, Class<Bind, Id, ITerm<Bind, Id>> @class, Initialization<Bind, T> initialization)
        {
            switch (universe.Polarity)
            {
                case Polarity.Forall:

                    switch (@class.Tag)
                    {
                        case Class<Bind, Id, ITerm<Bind, Id>>.Tags.Quantifier:
                        { 
                            var quantifier = (Class<Bind, Id, ITerm<Bind, Id>>.Quantifier) @class;
                            var lambda = (Initialization<Bind, T>.Forall.Quantifier) initialization;

                            var returnType = Substitute(environment, quantifier.Dependency, quantifier.Dependent, UseDeclarationF(lambda.Parameter));

                            var qualifiedBody = new Expression<Bind, Id, T>(universe, returnType, lambda.Body);

                            var dependency = quantifier.Dependency.Fmap(_ => lambda.Parameter);

                            environment = environment.Push(dependency);

                            var body = ConvertF(qualifiedBody.Annotate(environment));

                            return new Initialization<Bind, TR>.Forall.Quantifier(lambda.Parameter, body);
                        }
                        case Class<Bind, Id, ITerm<Bind, Id>>.Tags.Shift:
                        {
                            var shift = (Class<Bind, Id, ITerm<Bind, Id>>.Shift) @class;
                            var @return = (Initialization<Bind, T>.Forall.Shift) initialization;

                            var qualifiedBody = new Expression<Bind, Id, T>(universe.Dual(), shift.Content, @return.Body);

                            var body = ConvertF(qualifiedBody.Annotate(environment));

                            return new Initialization<Bind, TR>.Forall.Shift(body);
                        }
                        default:
                            throw new InvalidProgramException("Should never happen.");
                    }

                case Polarity.Exists:

                    switch (@class.Tag)
                    {
                        case Class<Bind, Id, ITerm<Bind, Id>>.Tags.Quantifier:
                        {
                            var quantifier = (Class<Bind, Id, ITerm<Bind, Id>>.Quantifier) @class;
                            var pair = (Initialization<Bind, T>.Exists.Quantifier) initialization;

                            var qualifiedLeft = quantifier.Dependency.Fmap(_ => pair.Left);

                            var rightType = Substitute(environment, quantifier.Dependency, quantifier.Dependent, pair.Left);

                            var qualifiedRight = new Expression<Bind, Id, T>(universe, @rightType, pair.Right);

                            var left = ConvertF(qualifiedLeft.Annotate(environment));
                            var right = ConvertF(qualifiedRight.Annotate(environment));

                            return new Initialization<Bind, TR>.Exists.Quantifier(left, right);
                        }
                        case Class<Bind, Id, ITerm<Bind, Id>>.Tags.Shift:
                        {
                            var shift = (Class<Bind, Id, ITerm<Bind, Id>>.Shift) @class;
                            var delay = (Initialization<Bind, T>.Exists.Shift) initialization;

                            var qualifiedBody = new Expression<Bind, Id, T>(universe.Dual(), shift.Content, delay.Body);

                            var body = ConvertF(qualifiedBody.Annotate(environment));

                            return new Initialization<Bind, TR>.Exists.Shift(body);
                        }
                        default:
                            throw new InvalidProgramException("Should never happen.");
                    }

                case null:
                default:
                    throw new InvalidProgramException("Should never happen.");
            }
        }

        private Continuation<Bind, TR> Map(Sequence<IExpression<Bind, Id, Bind>> environment, IUniverse universe, Class<Bind, Id, ITerm<Bind, Id>> @class, IExpression<Bind, Id, Continuation<Bind, T>> continuation)
        {
            switch (universe.Polarity)
            {
                case Polarity.Forall:

                    switch (@class.Tag)
                    {
                        case Class<Bind, Id, ITerm<Bind, Id>>.Tags.Quantifier:
                            {
                                var quantifier = (Class<Bind, Id, ITerm<Bind, Id>>.Quantifier)@class;
                                var application = (Continuation<Bind, T>.Forall.Quantifier)continuation.Term;

                                var qualifiedArgument = quantifier.Dependency.Fmap(_ => application.Argument);

                                var argument = ConvertF(qualifiedArgument.Annotate(environment));

                                return new Continuation<Bind, TR>.Forall.Quantifier(argument);
                            }
                        case Class<Bind, Id, ITerm<Bind, Id>>.Tags.Shift:
                            {
                                var shift = (Class<Bind, Id, ITerm<Bind, Id>>.Shift)@class;
                                var extract = (Continuation<Bind, T>.Forall.Shift)continuation.Term;

                                environment = environment
                                    .Push(new Expression<Bind, Id, Bind>(universe.Dual(), shift.Content, extract.Identifier));

                                var qualifiedBody = continuation.Fmap(_ => extract.Body);

                                var body = ConvertF(qualifiedBody.Annotate(environment));

                                return new Continuation<Bind, TR>.Forall.Shift(extract.Identifier, body);
                            }
                        default:
                            throw new InvalidProgramException("Should never happen.");
                    }

                case Polarity.Exists:

                    switch (@class.Tag)
                    {
                        case Class<Bind, Id, ITerm<Bind, Id>>.Tags.Quantifier:
                            {
                                var quantifier = (Class<Bind, Id, ITerm<Bind, Id>>.Quantifier)@class;
                                var extract = (Continuation<Bind, T>.Exists.Quantifier)continuation.Term;

                                environment = environment
                                    .Push(quantifier.Dependency.Fmap(_ => extract.Left));

                                var rightType = Substitute(environment, quantifier.Dependency, quantifier.Dependent, UseDeclarationF(extract.Left));

                                environment = environment
                                    .Push(new Expression<Bind, Id, Bind>(universe, rightType, extract.Right));

                                var body = ConvertF(continuation.Fmap(_ => extract.Body).Annotate(environment));

                                return new Continuation<Bind, TR>.Exists.Quantifier(extract.Left, extract.Right, body);
                            }
                        case Class<Bind, Id, ITerm<Bind, Id>>.Tags.Shift:
                            {
                                var shift = (Class<Bind, Id, ITerm<Bind, Id>>.Shift)@class;
                                var force = (Continuation<Bind, T>.Exists.Shift) continuation.Term;

                                return new Continuation<Bind, TR>.Exists.Shift();
                            }
                        default:
                            throw new InvalidProgramException("Should never happen.");
                    }

                case null:
                default:
                    throw new InvalidProgramException("Should never happen.");
            }
        }
    }
}