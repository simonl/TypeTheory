using System;
using Core;
using TypeTheory.ContinuationPassing;

namespace TypeTheory.CallByPushValue
{
    public static class Compilation
    {
        public static IQualified<Guid, Term<Guid>> CompileFully(IExpression<Guid, Guid, ITerm<Guid, Guid>> expression)
        {
            return new Qualified<Guid, Term<Guid>>(
                universe: new Term<Guid>.Universe(expression.Universe.Rank),
                type: Compile(expression.TypeOf()),
                term: Compile(expression));
        }

        private static Guid Fresh()
        {
            return Guid.NewGuid();
        }

        public static Term<Guid> Compile(IExpression<Guid, Guid, ITerm<Guid, Guid>> expression)
        {
            var traversal = new Traversal<Guid, Guid, Func<Term<Guid>, Term<Guid>>>(
                useDeclarationF: identifier => new Term<Guid, Guid>(new TermF<Guid, Guid, ITerm<Guid, Guid>>.Variable(identifier)),
                stepF: CompilationStep);

            return traversal.Traverse(expression.TopLevel())(null);
        }

        /*
         * [ (a, b) ]
         * ([ a ], [ b ])
         * 
         * [ a -> b ]
         * ([ a ], [ b ])
         * 
         * [ !a ] 
         * ~[ a ]
         * 
         * [ ?b ]
         * ~[ b ]
         * 
         * ---
         * 
         * [ x ]
         * x
         * 
         * [ \x. e ]
         * \k. let (x, k') = !k; [ e ] k'
         * 
         * [ return e ]
         * \k. jump !k [ e ]
         * 
         * [ (l, r) ]
         * ([ l ], [ r ])
         * 
         * [ { e } ]
         * \!k. [ e ] k
         * 
         * ---
         * 
         * [ f e ]
         * \k. [ f ] ([ e ], !k)
         * 
         * [ do { x <- e; f } ]
         * \k. [ e ] \!x. [ f ] k
         * 
         * [ let (x, y) = p; f ]
         * \k. let (x, y) = [ p ]; [ f ] k
         * 
         * [ force e ]
         * \k. jump [ e ] !k
         * 
         */
        /*
         * [ x ]
         * \c. c x
         * 
         * [ \x. e ]
         * \k. let# (x, k') = !k; [ e ] k'
         * 
         * [ return e ]
         * \k. [ e ] \x. jump# !k x
         * 
         * [ (l, r) ]
         * \c. [ l ] \x. [ r ] \y. let p = (x, y); c p
         * 
         * [ { e } ]
         * \c. let f = (\!k. [ e ] k); c f
         * 
         * ---
         * 
         * [ f e ]
         * \k. [ e ] \x. let# k' = (x, !k); [ f ] k'
         * 
         * [ do { x <- e; f } ]
         * \k. let# k' = (\!x. [ f ] k); [ e ] k'
         * 
         * [ let (x, y) = e; f ]
         * \k. [ e ] \p. let (x, y) = p; [ f ] k
         * 
         * [ force e ]
         * \k. [ e ] \x. jump x !k
         * 
         */
        private static Func<Term<Guid>, Term<Guid>> CompilationStep(IClosedTermF<Guid, Guid, Func<Term<Guid>, Term<Guid>>> annotated)
        {
            return CompilationStep(annotated.Environment, annotated.Expression);
        }

        private static Func<Term<Guid>, Term<Guid>> CompilationStep(Sequence<IExpression<Guid, Guid, Guid>> environment, IExpression<Guid, Guid, TermF<Guid, Guid, Func<Term<Guid>, Term<Guid>>>> expression)
        {
            switch (expression.Term.Production)
            {
                case Productions.Variable:
                {
                    var variable = (TermF<Guid, Guid, Func<Term<Guid>, Term<Guid>>>.Variable)expression.Term;

                    return _ => new Term<Guid>.Variable(variable.Identifier);
                }
                case Productions.Universe:
                {
                    var universe = (TermF<Guid, Guid, Func<Term<Guid>, Term<Guid>>>.Universe)expression.Term;

                    return _ => new Term<Guid>.Universe(universe.Order.Rank);
                }
                case Productions.Type:
                {
                    var type = (TermF<Guid, Guid, Func<Term<Guid>, Term<Guid>>>.Type)expression.Term;

                    var universe = (TermF<Guid, Guid, ITerm<Guid, Guid>>.Universe) expression.Type.Content;

                    var structure = CompileType(universe.Order, type.Class);

                    return _ => new Term<Guid>.Type(structure);
                }
                case Productions.Constructor:
                {
                    var constructor = (TermF<Guid, Guid, Func<Term<Guid>, Term<Guid>>>.Constructor)expression.Term;

                    var type = (TermF<Guid, Guid, ITerm<Guid, Guid>>.Type)expression.Type.Content;

                    return CompileConstructor(expression.Universe, type.Class, constructor.Initialization);
                }
                case Productions.Destructor:
                {
                    var destructor = (TermF<Guid, Guid, Func<Term<Guid>, Term<Guid>>>.Destructor)expression.Term;

                    var type = (TermF<Guid, Guid, ITerm<Guid, Guid>>.Type)destructor.Focus.Type.Content;

                    var focus = destructor.Focus.Term;

                    return CompileDestructor(destructor.Focus.Universe, type.Class, focus, destructor.Continuation);
                }
                default:
                    throw new InvalidProgramException("Should never happen.");
            }
        }

        private static Structure<Guid> CompileType(IUniverse universe, Class<Guid, Guid, Func<Term<Guid>, Term<Guid>>> @class)
        {
            switch (@class.Tag)
            {
                case Class<Guid, Guid, Func<Term<Guid>, Term<Guid>>>.Tags.Quantifier:
                    {
                        var quantifier = (Class<Guid, Guid, Func<Term<Guid>, Term<Guid>>>.Quantifier)@class;

                        Term<Guid> dependency = Compile(quantifier.Dependency.TypeOf());

                        var depUniverse = new Term<Guid>.Universe(quantifier.Dependency.Universe.Rank);

                        var qualifiedDependency = new Qualified<Guid, Guid>(depUniverse, dependency, quantifier.Dependency.Term);

                        return new Structure<Guid>.Pair(qualifiedDependency, quantifier.Dependent(null));
                    }
                case Class<Guid, Guid, Func<Term<Guid>, Term<Guid>>>.Tags.Shift:
                    {
                        var shift = (Class<Guid, Guid, Func<Term<Guid>, Term<Guid>>>.Shift)@class;

                        return new Structure<Guid>.Continuation(shift.Content(null));
                    }
                default:
                    throw new InvalidProgramException("Should never happen.");
            }
        }

        private static Func<Term<Guid>, Term<Guid>> CompileConstructor(IUniverse universe, Class<Guid, Guid, ITerm<Guid, Guid>> @class, Initialization<Guid, Func<Term<Guid>, Term<Guid>>> initialization)
        {
            switch (universe.Polarity)
            {
                case Polarity.Forall:
                {
                    var qualifiedSelfType = universe.Qualify<Guid, Guid, ITerm<Guid, Guid>>(new Term<Guid, Guid>(new TermF<Guid, Guid, ITerm<Guid, Guid>>.Type(@class)));

                    var compiledSelfType = Compile(qualifiedSelfType);

                    var selfUniverse = new Term<Guid>.Universe(universe.Rank);

                    return continuation =>
                    {
                        var qualifiedSelf = new Qualified<Guid, Term<Guid>>(selfUniverse, compiledSelfType, continuation);

                        var arguments = CompileNegativeConstructor(@class, initialization);

                        return new Term<Guid>.Destructor(qualifiedSelf, arguments);
                    };
                }
                case Polarity.Exists:
                {
                    return _ =>
                    {
                        var arguments = CompilePositiveConstructor(@class, initialization);

                        return new Term<Guid>.Constructor(arguments);
                    };
                }
                case null:
                default:
                    throw new ArgumentOutOfRangeException("polarity");
            }
        }

        private static ContinuationPassing.Continuation<Guid> CompileNegativeConstructor(Class<Guid, Guid, ITerm<Guid, Guid>> @class, Initialization<Guid, Func<Term<Guid>, Term<Guid>>> initialization)
        {
            switch (@class.Tag)
            {
                case Class<Guid, Guid, ITerm<Guid, Guid>>.Tags.Quantifier:
                    {
                        var quantifier = (Class<Guid, Guid, ITerm<Guid, Guid>>.Quantifier)@class;
                        var lambda = (Initialization<Guid, Func<Term<Guid>, Term<Guid>>>.Forall.Quantifier)initialization;

                        Guid fresh = Fresh();

                        return new ContinuationPassing.Continuation<Guid>.Extract(lambda.Parameter, fresh, lambda.Body(new Term<Guid>.Variable(fresh)));
                    }
                case Class<Guid, Guid, ITerm<Guid, Guid>>.Tags.Shift:
                    {
                        var shift = (Class<Guid, Guid, ITerm<Guid, Guid>>.Shift)@class;
                        var @return = (Initialization<Guid, Func<Term<Guid>, Term<Guid>>>.Forall.Shift)initialization;

                        return new ContinuationPassing.Continuation<Guid>.Jump(@return.Body(null));
                    }
                default:
                    throw new InvalidProgramException("Should never happen.");
            }
        }

        private static Initialization<Guid> CompilePositiveConstructor(Class<Guid, Guid, ITerm<Guid, Guid>> @class, Initialization<Guid, Func<Term<Guid>, Term<Guid>>> initialization)
        {
            switch (@class.Tag)
            {
                case Class<Guid, Guid, ITerm<Guid, Guid>>.Tags.Quantifier:
                {
                    var quantifier = (Class<Guid, Guid, ITerm<Guid, Guid>>.Quantifier) @class;
                    var pair = (Initialization<Guid, Func<Term<Guid>, Term<Guid>>>.Exists.Quantifier) initialization;

                    return new Initialization<Guid>.Pair(pair.Left(null), pair.Right(null));
                }
                case Class<Guid, Guid, ITerm<Guid, Guid>>.Tags.Shift:
                {
                    var shift = (Class<Guid, Guid, ITerm<Guid, Guid>>.Shift) @class;
                    var delay = (Initialization<Guid, Func<Term<Guid>, Term<Guid>>>.Exists.Shift) initialization;

                    var fresh = Fresh();

                    return new Initialization<Guid>.Lambda(fresh, delay.Body(new Term<Guid>.Variable(fresh)));
                }
                default:
                    throw new InvalidProgramException("Should never happen.");
            }
        }

        private static Func<Term<Guid>, Term<Guid>> CompileDestructor(IUniverse universe, Class<Guid, Guid, ITerm<Guid, Guid>> @class, Func<Term<Guid>, Term<Guid>> focus, Continuation<Guid, Func<Term<Guid>, Term<Guid>>> continuation)
        {
            switch (universe.Polarity)
            {
                case Polarity.Forall:
                    {
                        return arguments =>
                        {
                            var initialization = CompileNegativeDestructor(arguments, @class, continuation);

                            return focus(new Term<Guid>.Constructor(initialization));
                        };
                    }
                case Polarity.Exists:
                    {
                        var qualifiedSelfType = universe.Qualify<Guid, Guid, ITerm<Guid, Guid>>(new Term<Guid, Guid>(new TermF<Guid, Guid, ITerm<Guid, Guid>>.Type(@class)));

                        var compiledSelfType = Compile(qualifiedSelfType);

                        var selfUniverse = new Term<Guid>.Universe(universe.Rank);

                        return arguments =>
                        {
                            var qualifiedSelf = new Qualified<Guid, Term<Guid>>(selfUniverse, compiledSelfType, focus(null));

                            var cont = CompilePositiveDestructor(arguments, @class, continuation);

                            return new Term<Guid>.Destructor(qualifiedSelf, cont);
                        };
                    }
                case null:
                default:
                    throw new ArgumentOutOfRangeException("polarity");
            }
        }

        private static Initialization<Guid> CompileNegativeDestructor(Term<Guid> arguments, Class<Guid, Guid, ITerm<Guid, Guid>> @class, Continuation<Guid, Func<Term<Guid>, Term<Guid>>> continuation)
        {
            switch (@class.Tag)
            {
                case Class<Guid, Guid, ITerm<Guid, Guid>>.Tags.Quantifier:
                    {
                        var quantifier = (Class<Guid, Guid, ITerm<Guid, Guid>>.Quantifier)@class;
                        var application = (Continuation<Guid, Func<Term<Guid>, Term<Guid>>>.Forall.Quantifier)continuation;

                        return new Initialization<Guid>.Pair(application.Argument(null), arguments);
                    }
                case Class<Guid, Guid, ITerm<Guid, Guid>>.Tags.Shift:
                    {
                        var shift = (Class<Guid, Guid, ITerm<Guid, Guid>>.Shift)@class;
                        var extract = (Continuation<Guid, Func<Term<Guid>, Term<Guid>>>.Forall.Shift)continuation;

                        return new Initialization<Guid>.Lambda(extract.Identifier, extract.Body(arguments));
                    }
                default:
                    throw new InvalidProgramException("Should never happen.");
            }
        }

        private static ContinuationPassing.Continuation<Guid> CompilePositiveDestructor(Term<Guid> arguments, Class<Guid, Guid, ITerm<Guid, Guid>> @class, Continuation<Guid, Func<Term<Guid>, Term<Guid>>> continuation)
        {
            switch (@class.Tag)
            {
                case Class<Guid, Guid, ITerm<Guid, Guid>>.Tags.Quantifier:
                    {
                        var quantifier = (Class<Guid, Guid, ITerm<Guid, Guid>>.Quantifier)@class;
                        var extract = (Continuation<Guid, Func<Term<Guid>, Term<Guid>>>.Exists.Quantifier)continuation;

                        return new ContinuationPassing.Continuation<Guid>.Extract(extract.Left, extract.Right, extract.Body(arguments));
                    }
                case Class<Guid, Guid, ITerm<Guid, Guid>>.Tags.Shift:
                    {
                        var shift = (Class<Guid, Guid, ITerm<Guid, Guid>>.Shift)@class;
                        var force = (Continuation<Guid, Func<Term<Guid>, Term<Guid>>>.Exists.Shift)continuation;

                        return new ContinuationPassing.Continuation<Guid>.Jump(arguments);
                    }
                default:
                    throw new InvalidProgramException("Should never happen.");
            }
        }
    }
}