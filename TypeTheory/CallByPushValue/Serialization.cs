using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Core;

namespace TypeTheory.CallByPushValue
{
    public sealed class Serialization<Bind, Id>
    {
        private readonly Func<IAnnotated<Bind, Id, Bind>, IEnumerable<Bits>> SerializeBinding; 
        private readonly Func<IAnnotated<Bind, Id, Id>, IEnumerable<Bits>> SerializeIdentifier;
        private readonly Func<Bind, ITerm<Bind, Id>> UseDeclarationF;

        public Serialization(Func<IAnnotated<Bind, Id, Bind>, IEnumerable<Bits>> serializeBinding, Func<IAnnotated<Bind, Id, Id>, IEnumerable<Bits>> serializeIdentifier, Func<Bind, ITerm<Bind, Id>> useDeclarationF)
        {
            SerializeBinding = serializeBinding;
            SerializeIdentifier = serializeIdentifier;
            UseDeclarationF = useDeclarationF;
        }

        private static ITerm<Bind, Id> Substitute<Bind, Id>(Sequence<IExpression<Bind, Id, Bind>> environment, IExpression<Bind, Id, Bind> declaration, ITerm<Bind, Id> expression, Bind instead)
        {
            return expression;
        }

        public IEnumerable<Bits> SerializeFully(IExpression<Bind, Id, ITerm<Bind, Id>> expression)
        {
            var universe = EncodeUniverse(expression.Universe);
            var type = Serialize(expression.TypeOf());
            var term = Serialize(expression);
            
            return Encoding.Concatenate(universe, type, term);
        }

        public static IEnumerable<Bits> EncodeUniverse(IUniverse universe)
        {
            var rank = Encoding.EncodeNumber(universe.Rank);
            var polarity = Encoding.EncodeNullable(Encoding.EncodePolarity, universe.Polarity);

            return Encoding.Concatenate(rank, polarity);
        }

        public IEnumerable<Bits> Serialize(IExpression<Bind, Id, ITerm<Bind, Id>> expression)
        {
            var environment = new Sequence<IExpression<Bind, Id, Bind>>.Empty();

            var annotated = expression.Annotate(environment);

            return SerializeTerm(annotated);
        }

        public IEnumerable<Bits> SerializeTerm(IAnnotated<Bind, Id, ITerm<Bind, Id>> annotated)
        {
            var traversal = new Traversal<Bind, Id, IEnumerable<Bits>>(
                useDeclarationF: UseDeclarationF,
                stepF: SerializationStep);

            return traversal.Traverse(annotated);
        }

        private IEnumerable<Bits> SerializationStep(IClosedTermF<Bind, Id, IEnumerable<Bits>> closed)
        {
            foreach (var bit in EncodeTerm(closed.Expression.Universe, closed.Expression.Term.Production))
            {
                yield return bit;
            }

            switch (closed.Expression.Term.Production)
            {
                case Productions.Variable:
                {
                    var variable = (TermF<Bind, Id, IEnumerable<Bits>>.Variable)closed.Expression.Term;

                    var annotated = new Annotated<Bind, Id, Id>(closed.Environment, closed.Expression.Fmap(_ => variable.Identifier));

                    foreach (var bit in SerializeIdentifier(annotated))
                    {
                        yield return bit;
                    }

                    yield break;
                }
                case Productions.Universe:
                {
                    var universe = (TermF<Bind, Id, IEnumerable<Bits>>.Universe)closed.Expression.Term;

                    foreach (var bit in EncodeUniverse(universe.Order))
                    {
                        yield return bit;
                    }

                    yield break;
                }
                case Productions.Type:
                {
                    var type = (TermF<Bind, Id, IEnumerable<Bits>>.Type)closed.Expression.Term;

                    var universe = (TermF<Bind, Id, ITerm<Bind, Id>>.Universe)closed.Expression.Type.Content;

                    foreach (var bit in SerializeClass(closed.Environment, universe.Order, type.Class))
                    {
                        yield return bit;
                    }

                    yield break;
                }
                case Productions.Constructor:
                {
                    var constructor = (TermF<Bind, Id, IEnumerable<Bits>>.Constructor)closed.Expression.Term;

                    var type = (TermF<Bind, Id, ITerm<Bind, Id>>.Type)closed.Expression.Type.Content;

                    foreach (var bit in SerializeConstructor(closed.Environment, closed.Expression.Universe, type.Class, constructor.Initialization))
                    {
                        yield return bit;
                    }

                    yield break;
                }
                case Productions.Destructor:
                {
                    var destructor = (TermF<Bind, Id, IEnumerable<Bits>>.Destructor)closed.Expression.Term;

                    var focus = destructor.Focus;

                    var type = (TermF<Bind, Id, ITerm<Bind, Id>>.Type)focus.Type.Content;

                    var focusUniv = EncodeUniverse(focus.Universe);
                    var focusType = SerializeTerm(new Annotated<Bind, Id, ITerm<Bind, Id>>(closed.Environment, focus.TypeOf()));

                    foreach (var bit in Encoding.Concatenate(focusUniv, focusType, focus.Term))
                    {
                        yield return bit;
                    }

                    foreach (var bit in SerializeContinuation(closed.Environment, focus.Universe, type.Class, destructor.Continuation))
                    {
                        yield return bit;
                    }

                    yield break;
                }
                default:
                {
                    throw new InvalidProgramException("Should never happen.");
                }
            }
        }

        private IEnumerable<Bits> SerializeContinuation(Sequence<IExpression<Bind, Id, Bind>> environment, IUniverse universe, Class<Bind, Id, ITerm<Bind, Id>> @class, Continuation<Bind, IEnumerable<Bits>> continuation)
        {
            switch (universe.Polarity)
            {
                case Polarity.Forall:

                    switch (@class.Tag)
                    {
                        case Class<Bind, Id, ITerm<Bind, Id>>.Tags.Quantifier:
                            {
                                var quantifier = (Class<Bind, Id, ITerm<Bind, Id>>.Quantifier)@class;
                                var application = (Continuation<Bind, IEnumerable<Bits>>.Forall.Quantifier)continuation;

                                foreach (var bit in application.Argument)
                                {
                                    yield return bit;
                                }

                                yield break;
                            }
                        case Class<Bind, Id, ITerm<Bind, Id>>.Tags.Shift:
                            {
                                var shift = (Class<Bind, Id, ITerm<Bind, Id>>.Shift)@class;
                                var extract = (Continuation<Bind, IEnumerable<Bits>>.Forall.Shift)continuation;

                                var binding = SerializeBinding(new Annotated<Bind, Id, Bind>(environment, new Expression<Bind, Id, Bind>(universe.Dual(), shift.Content, extract.Identifier)));

                                foreach (var bit in Encoding.Concatenate(binding, extract.Body))
                                {
                                    yield return bit;
                                }

                                yield break;
                            }
                        default:
                            {
                                throw new InvalidProgramException("Should never happen.");
                            }
                    }

                case Polarity.Exists:

                    switch (@class.Tag)
                    {
                        case Class<Bind, Id, ITerm<Bind, Id>>.Tags.Quantifier:
                            {
                                var quantifier = (Class<Bind, Id, ITerm<Bind, Id>>.Quantifier)@class;
                                var extract = (Continuation<Bind, IEnumerable<Bits>>.Exists.Quantifier)continuation;

                                var rightType = Substitute(environment, quantifier.Dependency, quantifier.Dependent, extract.Left);

                                var left = SerializeBinding(new Annotated<Bind, Id, Bind>(environment, quantifier.Dependency.Fmap(_ => extract.Left)));
                                var right = SerializeBinding(new Annotated<Bind, Id, Bind>(environment, new Expression<Bind, Id, Bind>(universe, rightType, extract.Right)));

                                foreach (var bit in Encoding.Concatenate(left, right, extract.Body))
                                {
                                    yield return bit;
                                }

                                yield break;
                            }
                        case Class<Bind, Id, ITerm<Bind, Id>>.Tags.Shift:
                            {
                                var shift = (Class<Bind, Id, ITerm<Bind, Id>>.Shift)@class;
                                var force = (Continuation<Bind, IEnumerable<Bits>>.Exists.Shift)continuation;

                                yield break;
                            }
                        default:
                            {
                                throw new InvalidProgramException("Should never happen.");
                            }
                    }

                case null:
                default:
                    throw new InvalidProgramException("Should never happen.");
            }
        }

        private IEnumerable<Bits> SerializeConstructor(Sequence<IExpression<Bind, Id, Bind>> environment, IUniverse universe, Class<Bind, Id, ITerm<Bind, Id>> @class, Initialization<Bind, IEnumerable<Bits>> initialization)
        {
            switch (universe.Polarity)
            {
                case Polarity.Forall:

                    switch (@class.Tag)
                    {
                        case Class<Bind, Id, ITerm<Bind, Id>>.Tags.Quantifier:
                        {
                            var quantifier = (Class<Bind, Id, ITerm<Bind, Id>>.Quantifier) @class;
                            var lambda = (Initialization<Bind, IEnumerable<Bits>>.Forall.Quantifier) initialization;

                            var parameter = SerializeBinding(new Annotated<Bind, Id, Bind>(environment, quantifier.Dependency.Fmap(_ => lambda.Parameter)));

                            foreach (var bit in Encoding.Concatenate(parameter, lambda.Body))
                            {
                                yield return bit;
                            }

                            yield break;
                        }
                        case Class<Bind, Id, ITerm<Bind, Id>>.Tags.Shift:
                        {
                            var shift = (Class<Bind, Id, ITerm<Bind, Id>>.Shift)@class;
                            var @return = (Initialization<Bind, IEnumerable<Bits>>.Forall.Shift)initialization;

                            foreach (var bit in @return.Body)
                            {
                                yield return bit;
                            }

                            yield break;
                        }
                        default:
                        {
                            throw new InvalidProgramException("Should never happen.");
                        }
                    }

                case Polarity.Exists:

                    switch (@class.Tag)
                    {
                        case Class<Bind, Id, ITerm<Bind, Id>>.Tags.Quantifier:
                        {
                            var quantifier = (Class<Bind, Id, ITerm<Bind, Id>>.Quantifier) @class;
                            var pair = (Initialization<Bind, IEnumerable<Bits>>.Exists.Quantifier) initialization;

                            foreach (var bit in Encoding.Concatenate(pair.Left, pair.Right))
                            {
                                yield return bit;
                            }

                            yield break;
                        }
                        case Class<Bind, Id, ITerm<Bind, Id>>.Tags.Shift:
                        {
                            var shift = (Class<Bind, Id, ITerm<Bind, Id>>.Shift) @class;
                            var delay = (Initialization<Bind, IEnumerable<Bits>>.Exists.Shift) initialization;

                            foreach (var bit in delay.Body)
                            {
                                yield return bit;
                            }

                            yield break;
                        }
                        default:
                        {
                            throw new InvalidProgramException("Should never happen.");
                        }
                    }

                case null:
                default:
                    throw new InvalidProgramException("Should never happen.");
            }
        }

        private IEnumerable<Bits> SerializeClass(Sequence<IExpression<Bind, Id, Bind>> environment, IUniverse universe, Class<Bind, Id, IEnumerable<Bits>> @class)
        {
            foreach (var bit in EncodeClass(@class.Tag))
            {
                yield return bit;
            }

            switch (@class.Tag)
            {
                case Class<Bind, Id, IEnumerable<Bits>>.Tags.Quantifier:
                    var quantifier = (Class<Bind, Id, IEnumerable<Bits>>.Quantifier) @class;

                    var depUniv = EncodeUniverse(quantifier.Dependency.Universe);
                    var depType = SerializeTerm(new Annotated<Bind, Id, ITerm<Bind, Id>>(environment, quantifier.Dependency.TypeOf()));
                    var depBind = SerializeBinding(new Annotated<Bind, Id, Bind>(environment, quantifier.Dependency));

                    foreach (var bit in Encoding.Concatenate(depUniv, depType, depBind))
                    {
                        yield return bit;
                    }

                    foreach (var bit in quantifier.Dependent)
                    {
                        yield return bit;
                    }

                    yield break;
                case Class<Bind, Id, IEnumerable<Bits>>.Tags.Shift:
                    var shift = (Class<Bind, Id, IEnumerable<Bits>>.Shift)@class;

                    foreach (var bit in shift.Content)
                    {
                        yield return bit;
                    }

                    yield break;
                default:
                    throw new InvalidProgramException("Should never happen.");
            }
        }

        private IEnumerable<Bits> EncodeClass<Bind, Id, R>(Class<Bind, Id, R>.Tags @class)
        {
            switch (@class)
            {
                case Class<Bind, Id, R>.Tags.Quantifier:

                    return new Bits[] { Bits.Zero };
                case Class<Bind, Id, R>.Tags.Shift:

                    return new Bits[] { Bits.One };
                default:
                    throw new InvalidProgramException("Should never happen.");
            }
        }

        private static IEnumerable<Bits> EncodeTerm(IUniverse universe, Productions production)
        {
            if (universe.Rank == 0)
            {
                return EncodeTerm(production);
            }

            return EncodeType(production);
        }

        /* Using statistics...
         * 
         * U, T
         * 
         * Var
         * Univ
         * Type:
         *   Quantifier
         *   Shift
         * Construct
         * Destruct: U, T, K
         * 
         */
        private static IEnumerable<Bits> EncodeTerm(Productions production)
        {
            switch (production)
            {
                case Productions.Variable:

                    return new Bits[] { Bits.Zero, Bits.Zero };
                case Productions.Destructor:

                    return new Bits[] { Bits.Zero, Bits.One };
                case Productions.Constructor:

                    return new Bits[] { Bits.One, Bits.Zero };
                case Productions.Type:

                    return new Bits[] { Bits.One, Bits.One, Bits.Zero };
                case Productions.Universe:

                    return new Bits[] { Bits.One, Bits.One, Bits.One };
                default:
                    throw new InvalidProgramException("Should never happen.");
            }
        }

        private static IEnumerable<Bits> EncodeType(Productions production)
        {
            switch (production)
            {
                case Productions.Type:

                    return new Bits[] { Bits.Zero, Bits.Zero };
                case Productions.Variable:

                    return new Bits[] { Bits.Zero, Bits.One };
                case Productions.Universe:

                    return new Bits[] { Bits.One, Bits.Zero };
                case Productions.Destructor:

                    return new Bits[] { Bits.One, Bits.One, Bits.Zero };
                case Productions.Constructor:

                    return new Bits[] { Bits.One, Bits.One, Bits.One };
                default:
                    throw new InvalidProgramException("Should never happen.");
            }
        }
    }
}