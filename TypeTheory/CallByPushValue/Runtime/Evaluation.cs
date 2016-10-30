using System;
using Core;

namespace TypeTheory.CallByPushValue
{
    public static class Evaluation
    {
        public static Value<R> Evaluate<R>(IExpression<Unit, uint, ITerm<Unit, uint>> expression)
        {
            var traversal = new Traversal<Unit, uint, IEvaluator<R>>(
                useDeclarationF: unit => new Term<Unit, uint>(new TermF<Unit, uint, ITerm<Unit, uint>>.Variable(0)),
                stepF: EvaluationStep<R>);

            var locals = new Sequence<Value<R>>.Empty();

            IEvaluator<R> evaluator = traversal.Traverse(expression.TopLevel());

            return new Value<R>.Continuation(new Continuation<R>(throwF: argument => evaluator.Evaluate(locals, argument)));
        }

        private static IEvaluator<R> EvaluationStep<R>(IAnnotated<Unit, uint, TermF<Unit, uint, IEvaluator<R>>> annotated)
        {
            return EvaluationStep(annotated.Environment, annotated.Expression);
        }

        private static IEvaluator<R> EvaluationStep<R>(Sequence<IExpression<Unit, uint, Unit>> environment, IExpression<Unit, uint, TermF<Unit, uint, IEvaluator<R>>> expression)
        {
            switch (expression.Term.Production)
            {
                case Productions.Variable:
                {
                    var variable = (TermF<Unit, uint, IEvaluator<R>>.Variable) expression.Term;

                    return new Evaluator<R>(
                        evaluateF: (locals, argument) =>
                        {
                            var value = locals.GetAt(variable.Identifier);

                            var continuation = (Value<R>.Continuation) argument;

                            return continuation.Content.Throw(value);
                        });
                }
                case Productions.Constructor:
                {
                    var constructor = (TermF<Unit, uint, IEvaluator<R>>.Constructor) expression.Term;

                    var polarity = expression.Universe.Polarity;

                    var type = (TermF<Unit, uint, ITerm<Unit, uint>>.Type) expression.Type.Content;

                    return EvaluateConstructor(polarity, type.Class, constructor.Initialization);
                }
                case Productions.Destructor:
                {
                    var destructor = (TermF<Unit, uint, IEvaluator<R>>.Destructor) expression.Term;

                    var polarity = destructor.Focus.Universe.Polarity;

                    var type = (TermF<Unit, uint, ITerm<Unit, uint>>.Type) destructor.Focus.Type.Content;

                    var focus = destructor.Focus.Term;

                    return EvaluateDestructor(polarity, type.Class, focus, destructor.Continuation);
                }
                case Productions.Universe:
                case Productions.Type:
                {
                    return new Evaluator<R>(
                        evaluateF: (locals, argument) =>
                        {
                            throw new ArgumentException("Cannot evaluate terms of higher universes.");
                        });
                }
                default:
                    throw new InvalidProgramException("Should never happen.");
            }
        }

        private static IEvaluator<R> EvaluateConstructor<R>(Polarity? polarity, Class<Unit, uint, ITerm<Unit, uint>> @class, Initialization<Unit, IEvaluator<R>> initialization)
        {
            switch (polarity)
            {
                case Polarity.Forall:

                    switch (@class.Tag)
                    {
                        case Class<Unit, uint, ITerm<Unit, uint>>.Tags.Quantifier:
                            {
                                var quantifier = (Class<Unit, uint, ITerm<Unit, uint>>.Quantifier)@class;
                                var lambda = (Initialization<Unit, IEvaluator<R>>.Forall.Quantifier)initialization;

                                if (quantifier.Dependency.Universe.Rank != 0)
                                {
                                    return lambda.Body;
                                }

                                return new Evaluator<R>(
                                    evaluateF: (locals, argument) =>
                                    {
                                        var pair = (Value<R>.Pair)argument;

                                        return lambda.Body.Evaluate(locals.Push(pair.Left), pair.Right);
                                    });
                            }
                        case Class<Unit, uint, ITerm<Unit, uint>>.Tags.Shift:
                            {
                                var shift = (Class<Unit, uint, ITerm<Unit, uint>>.Shift)@class;
                                var @return = (Initialization<Unit, IEvaluator<R>>.Forall.Shift)initialization;

                                return new Evaluator<R>(
                                    evaluateF: (locals, argument) =>
                                    {
                                        return @return.Body.Evaluate(locals, argument);
                                    });
                            }
                        default:
                            throw new InvalidProgramException("Should never happen.");
                    }

                case Polarity.Exists:

                    switch (@class.Tag)
                    {
                        case Class<Unit, uint, ITerm<Unit, uint>>.Tags.Quantifier:
                            {
                                var quantifier = (Class<Unit, uint, ITerm<Unit, uint>>.Quantifier)@class;
                                var pair = (Initialization<Unit, IEvaluator<R>>.Exists.Quantifier)initialization;

                                if (quantifier.Dependency.Universe.Rank != 0)
                                {
                                    return pair.Right;
                                }

                                return new Evaluator<R>(
                                    evaluateF: (locals, argument) =>
                                    {
                                        return pair.Left.Evaluate(locals,
                                            argument: new Value<R>.Continuation(
                                                content: new Continuation<R>(
                                                    throwF: left =>
                                                    {
                                                        return pair.Right.Evaluate(locals,
                                                            argument: new Value<R>.Continuation(
                                                                content: new Continuation<R>(
                                                                    throwF: right =>
                                                                    {
                                                                        var continuation = (Value<R>.Continuation)argument;

                                                                        return continuation.Content.Throw(new Value<R>.Pair(left, right));
                                                                    })));
                                                        })));

                                    });
                            }
                        case Class<Unit, uint, ITerm<Unit, uint>>.Tags.Shift:
                            {
                                var shift = (Class<Unit, uint, ITerm<Unit, uint>>.Shift)@class;
                                var delay = (Initialization<Unit, IEvaluator<R>>.Exists.Shift)initialization;

                                return new Evaluator<R>(
                                    evaluateF: (locals, argument) =>
                                    {
                                        var continuation = (Value<R>.Continuation)argument;

                                        return continuation.Content.Throw(
                                            argument: new Value<R>.Continuation(
                                                new Continuation<R>(
                                                    throwF: result =>
                                                    {
                                                        return delay.Body.Evaluate(locals, result);
                                                    })));
                                    });
                            }
                        default:
                            throw new InvalidProgramException("Should never happen.");
                    }

                case null:
                default:
                    throw new InvalidProgramException("Should never happen.");
            }
        }

        private static IEvaluator<R> EvaluateDestructor<R>(Polarity? polarity, Class<Unit, uint, ITerm<Unit, uint>> @class, IEvaluator<R> focus, Continuation<Unit, IEvaluator<R>> continuation)
        {
            switch (polarity)
            {
                case Polarity.Forall:

                    switch (@class.Tag)
                    {
                        case Class<Unit, uint, ITerm<Unit, uint>>.Tags.Quantifier:
                            {
                                var quantifier = (Class<Unit, uint, ITerm<Unit, uint>>.Quantifier)@class;
                                var application = (Continuation<Unit, IEvaluator<R>>.Forall.Quantifier)continuation;

                                if (quantifier.Dependency.Universe.Rank != 0)
                                {
                                    return focus;
                                }

                                return new Evaluator<R>(
                                    evaluateF: (locals, argument) =>
                                    {
                                        return application.Argument.Evaluate(locals,
                                            argument: new Value<R>.Continuation(
                                                content: new Continuation<R>(
                                                    throwF: result =>
                                                    {
                                                        return focus.Evaluate(locals, new Value<R>.Pair(result, argument));
                                                    })));
                                    });
                            }
                        case Class<Unit, uint, ITerm<Unit, uint>>.Tags.Shift:
                            {
                                var shift = (Class<Unit, uint, ITerm<Unit, uint>>.Shift)@class;
                                var extract = (Continuation<Unit, IEvaluator<R>>.Forall.Shift)continuation;

                                return new Evaluator<R>(
                                    evaluateF: (locals, argument) =>
                                    {
                                        return focus.Evaluate(locals,
                                            argument: new Value<R>.Continuation(
                                                content: new Continuation<R>(
                                                    throwF: result =>
                                                    {
                                                        locals = locals.Push(result);

                                                        return extract.Body.Evaluate(locals, argument);
                                                    })));
                                    });
                            }
                        default:
                            throw new InvalidProgramException("Should never happen.");
                    }

                case Polarity.Exists:

                    switch (@class.Tag)
                    {
                        case Class<Unit, uint, ITerm<Unit, uint>>.Tags.Quantifier:
                            {
                                var quantifier = (Class<Unit, uint, ITerm<Unit, uint>>.Quantifier)@class;
                                var extract = (Continuation<Unit, IEvaluator<R>>.Exists.Quantifier)continuation;

                                return new Evaluator<R>(
                                    evaluateF: (locals, argument) =>
                                    {
                                        return focus.Evaluate(locals,
                                            argument: new Value<R>.Continuation(
                                                content: new Continuation<R>(
                                                    throwF: result =>
                                                    {
                                                        if (quantifier.Dependency.Universe.Rank != 0)
                                                        {
                                                            locals = locals.Push(result);
                                                        }
                                                        else
                                                        {
                                                            var pair = (Value<R>.Pair)result;

                                                            locals = locals.Push(pair.Left);
                                                            locals = locals.Push(pair.Right);
                                                        }

                                                        return extract.Body.Evaluate(locals, argument);
                                                    })));
                                    });
                            }
                        case Class<Unit, uint, ITerm<Unit, uint>>.Tags.Shift:
                            {
                                var shift = (Class<Unit, uint, ITerm<Unit, uint>>.Shift)@class;
                                var force = (Continuation<Unit, IEvaluator<R>>.Exists.Shift)continuation;

                                return new Evaluator<R>(
                                    evaluateF: (locals, argument) =>
                                    {
                                        return focus.Evaluate(locals,
                                            argument: new Value<R>.Continuation(
                                                content: new Continuation<R>(
                                                    throwF: result =>
                                                    {
                                                        var delayed = (Value<R>.Continuation)result;

                                                        return delayed.Content.Throw(argument);
                                                    })));
                                    });
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