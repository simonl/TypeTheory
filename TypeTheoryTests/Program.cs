using System;
using System.Data;
using System.Linq;
using Core;
using TypeTheory;
using TypeTheory.CallByPushValue;

namespace TypeTheoryTests
{
    public static class Program
    {
        /*
         * identity : (a:*) -> a -> a
         * identity [a] x = x
         * 
         * echo : !((a:P) -> a -> ?a)
         * echo [a] x = return x
         * 
         * forward : !((a:N) -> !a -> a)
         * forward [a] x = force x
         * 
         * echo/cps : ~(a:*, a, ~a)
         * echo/cps ([a], x, return) = return x
         * 
         * forward/cps : ~(a:*, ~a, a)
         * forward/cps ([a], thunk, k) = thunk k
         * 
         */
        public static void Main(string[] args)
        {
            var identity = Identity();

            var echo = Echo(Guid.NewGuid(), Guid.NewGuid());

            // TypeChecking.VerifyClosed(echo);

            var forward = Forward(Guid.NewGuid(), Guid.NewGuid());

            // TypeChecking.VerifyClosed(forward);

            var echoN = new Expression<Unit, uint, ITerm<Unit, uint>>(
                universe: new Universe(0, Polarity.Exists),
                type: new Term<Unit, uint>(new TermF<Unit, uint, ITerm<Unit, uint>>.Type(new Class<Unit, uint, ITerm<Unit, uint>>.Shift(
                    content: new Term<Unit, uint>(new TermF<Unit, uint, ITerm<Unit, uint>>.Type(new Class<Unit, uint, ITerm<Unit, uint>>.Quantifier(
                        dependency: new Expression<Unit, uint, Unit>(new Universe(1, null), new Term<Unit, uint>(new TermF<Unit, uint, ITerm<Unit, uint>>.Universe(new Universe(0, Polarity.Exists))), Unit.Singleton),
                        dependent: new Term<Unit, uint>(new TermF<Unit, uint, ITerm<Unit, uint>>.Type(new Class<Unit, uint, ITerm<Unit, uint>>.Quantifier(
                            dependency: new Expression<Unit, uint, Unit>(new Universe(0, Polarity.Exists), new Term<Unit, uint>(new TermF<Unit, uint, ITerm<Unit, uint>>.Variable(0)), Unit.Singleton),
                            dependent: new Term<Unit, uint>(new TermF<Unit, uint, ITerm<Unit, uint>>.Type(new Class<Unit, uint, ITerm<Unit, uint>>.Shift(
                                content: new Term<Unit, uint>(new TermF<Unit, uint, ITerm<Unit, uint>>.Variable(1)))))))))))))),
                term: new Term<Unit, uint>(new TermF<Unit, uint, ITerm<Unit, uint>>.Constructor(
                    initialization: new Initialization<Unit, ITerm<Unit, uint>>.Exists.Shift(
                        body: new Term<Unit, uint>(new TermF<Unit, uint, ITerm<Unit, uint>>.Constructor(
                            initialization: new Initialization<Unit, ITerm<Unit, uint>>.Forall.Quantifier(Unit.Singleton,
                                body: new Term<Unit, uint>(new TermF<Unit, uint, ITerm<Unit, uint>>.Constructor(
                                    initialization: new Initialization<Unit, ITerm<Unit, uint>>.Forall.Quantifier(Unit.Singleton,
                                        body: new Term<Unit, uint>(new TermF<Unit, uint, ITerm<Unit, uint>>.Constructor(
                                            initialization: new Initialization<Unit, ITerm<Unit, uint>>.Forall.Shift(
                                                body: new Term<Unit, uint>(new TermF<Unit, uint, ITerm<Unit, uint>>.Variable(0)))))))))))))));


            var computation = (Value<int>.Continuation) Evaluation.Evaluate<int>(echoN);

            var number = computation.Content.Throw(
                argument: new Value<int>.Continuation(new Continuation<int>(
                    throwF: result =>
                    {
                        var delayed = (Value<int>.Continuation) result;

                        return delayed.Content.Throw(
                            argument: new Value<int>.Pair(
                                left: new Value<int>.Builtin<int>(4),
                                right: new Value<int>.Continuation(Returning<int>())));
                    })));

            Console.WriteLine("Result: " + number);
            Console.WriteLine();

            var serializer = new Serialization<Unit, uint>(
                useDeclarationF: unit => new Term<Unit, uint>(new TermF<Unit, uint, ITerm<Unit, uint>>.Variable(0)),
                serializeBinding: _ => new Bits[0],
                serializeIdentifier: annotated =>
                {
                    return Encoding.EncodeNumber(annotated.Expression.Term);
                });

            var serializer2 = new Serialization<Guid, Guid>(
                useDeclarationF: identifier => new Term<Guid, Guid>(new TermF<Guid, Guid, ITerm<Guid, Guid>>.Variable(identifier)),
                serializeBinding: identifier => new Bits[0],
                serializeIdentifier: annotated =>
                {
                    var identifier = annotated.Expression.Term;

                    var index = annotated.Environment.IndexOf(
                        selector: binding =>
                        {
                            return binding.Term.Equals(identifier) ? Match.Yes : Match.Potentially;
                        });

                    return Encoding.EncodeNumber(index);
                });

            var serializer3 = new Serialization<Guid, Guid>(
                useDeclarationF: identifier => new Term<Guid, Guid>(new TermF<Guid, Guid, ITerm<Guid, Guid>>.Variable(identifier)),
                serializeBinding: identifier => new Bits[0],
                serializeIdentifier: annotated =>
                {
                    var identifier = annotated.Expression.Term;

                    var index = annotated.Environment.IndexOf(
                        selector: binding =>
                        {
                            if (binding.Universe.Rank != annotated.Expression.Universe.Rank)
                            {
                                return Match.No;
                            }

                            return binding.Term.Equals(identifier) ? Match.Yes : Match.Potentially;
                        });

                    return Encoding.EncodeNumber(index);
                });

            var bits = serializer.SerializeFully(echoN).ToArray();

            Console.WriteLine("Size: " + bits.Length);
            foreach (var bit in bits)
            {
                Console.Write(bit + ", ");
            }

            Console.WriteLine();
            Console.WriteLine();

            var bits2 = serializer2.SerializeFully(echo).ToArray();

            Console.WriteLine("Size: " + bits2.Length);
            foreach (var bit in bits2)
            {
                Console.Write(bit + ", ");
            }

            Console.WriteLine();
            Console.WriteLine();

            var bits3 = serializer3.SerializeFully(echo).ToArray();

            Console.WriteLine("Size: " + bits3.Length);
            foreach (var bit in bits3)
            {
                Console.Write(bit + ", ");
            }

            Console.WriteLine();
            Console.WriteLine();

            var cps = Compilation.Compile(echo);

            var display = new TypeTheory.ContinuationPassing.Display<Guid>(id => id.ToString("D")[0].ToString());

            Console.WriteLine(display.ToString(cps));

            Console.ReadLine();
        }

        private static IContinuation<R> Returning<R>()
        {
            return new Continuation<R>(
                throwF: value =>
                {
                    var builtin = (Value<R>.Builtin<R>)value;

                    return builtin.Content;
                });
        }

        private static TypeTheory.DirectStyle.Expression<Guid, Guid, TypeTheory.DirectStyle.Term<Guid, Guid>> Identity()
        {
            var alpha = Guid.NewGuid();
            var x = Guid.NewGuid();

            var universe = new TypeTheory.DirectStyle.Universe(0);

            var type =
                new TypeTheory.DirectStyle.Term<Guid, Guid>.Type(Polarity.Forall, new TypeTheory.DirectStyle.Class<Guid, Guid>.Quantifier(new TypeTheory.DirectStyle.Expression<Guid, Guid, Guid>(new TypeTheory.DirectStyle.Universe(1), new TypeTheory.DirectStyle.Term<Guid, Guid>.Universe(new TypeTheory.DirectStyle.Universe(0)), alpha),
                new TypeTheory.DirectStyle.Term<Guid, Guid>.Type(Polarity.Forall, new TypeTheory.DirectStyle.Class<Guid, Guid>.Quantifier(new TypeTheory.DirectStyle.Expression<Guid, Guid, Guid>(new TypeTheory.DirectStyle.Universe(0), new TypeTheory.DirectStyle.Term<Guid, Guid>.Variable(alpha), x),
                new TypeTheory.DirectStyle.Term<Guid, Guid>.Variable(alpha)))));

            var term =
                new TypeTheory.DirectStyle.Term<Guid, Guid>.Constructor(new TypeTheory.DirectStyle.Initialization<Guid, Guid>.Forall.Quantifier(alpha,
                new TypeTheory.DirectStyle.Term<Guid, Guid>.Constructor(new TypeTheory.DirectStyle.Initialization<Guid, Guid>.Forall.Quantifier(x,
                new TypeTheory.DirectStyle.Term<Guid, Guid>.Variable(x)))));

            return new TypeTheory.DirectStyle.Expression<Guid, Guid, TypeTheory.DirectStyle.Term<Guid, Guid>>(universe, type, term);
        }

        private static IExpression<Id, Id, ITerm<Id, Id>> Echo<Id>(Id a, Id x)
        {
            // N
            var universe = Positive();

            // (a:P) -> a -> ?a
            var type = Shift(ForallType(Positive(), a, alpha => Func(alpha, x, Shift(alpha))));

            // \a. \x. return x
            var term = Delay(Lambda(a, alpha => Lambda(x, arg => Return(arg))));

            return new Expression<Id, Id, ITerm<Id, Id>>(universe, type, term);
        }

        private static IExpression<Id, Id, ITerm<Id, Id>> Forward<Id>(Id a, Id x)
        {
            // N
            var universe = Positive();

            // (a:N) -> !a -> a
            var type = Shift(ForallType(Negative(), a, alpha => Func(Shift(alpha), x, alpha)));

            // \a. \x. force x
            var term = Delay(Lambda(a, alpha => Lambda(x, arg => Force(new Expression<Id, Id, ITerm<Id, Id>>(Negative(), Shift(alpha), arg)))));

            return new Expression<Id, Id, ITerm<Id, Id>>(universe, type, term);
        }

        public static ITerm<Id, Id> Shift<Id>(ITerm<Id, Id> content)
        {
            return new Term<Id, Id>(new TermF<Id, Id, ITerm<Id, Id>>.Type(new Class<Id, Id, ITerm<Id, Id>>.Shift(content)));
        }


        public static ITerm<Guid, Guid> Func(uint rank, ITerm<Guid, Guid> parameter, ITerm<Guid, Guid> @return)
        {
            Guid identifier = Guid.NewGuid();

            return Func(rank, parameter, identifier, @return);
        }

        public static ITerm<Id, Id> Func<Id>(ITerm<Id, Id> parameter, Id identifier, ITerm<Id, Id> @return)
        {
            return Func(0, parameter, identifier, @return);
        }

        private static ITerm<Id, Id> Func<Id>(uint rank, ITerm<Id, Id> parameter, Id identifier, ITerm<Id, Id> @return)
        {
            var dependency = new Expression<Id, Id, Id>(Positive(rank), parameter, identifier);

            return new Term<Id, Id>(new TermF<Id, Id, ITerm<Id, Id>>.Type(new Class<Id, Id, ITerm<Id, Id>>.Quantifier(dependency, @return)));
        }

        public static ITerm<Guid, Guid> ForallType(IUniverse universe, Func<ITerm<Guid, Guid>, ITerm<Guid, Guid>> dependent)
        {
            Guid identifier = Guid.NewGuid();

            return ForallType(universe, identifier, dependent);
        }

        private static ITerm<Id, Id> ForallType<Id>(IUniverse universe, Id identifier, Func<ITerm<Id, Id>, ITerm<Id, Id>> dependent)
        {
            var dependency = universe.Qualify<Id, Id, Id>(term: identifier);

            var variable = new Term<Id, Id>(new TermF<Id, Id, ITerm<Id, Id>>.Variable(dependency.Term));

            return new Term<Id, Id>(new TermF<Id, Id, ITerm<Id, Id>>.Type(new Class<Id, Id, ITerm<Id, Id>>.Quantifier(dependency, dependent(variable))));
        }

        public static ITerm<Guid, Guid> Lambda(Func<ITerm<Guid, Guid>, ITerm<Guid, Guid>> body)
        {
            var identifier = Guid.NewGuid();

            return Lambda(identifier, body);
        }

        private static ITerm<Id, Id> Lambda<Id>(Id identifier, Func<ITerm<Id, Id>, ITerm<Id, Id>> body)
        {
            var variable = new Term<Id, Id>(new TermF<Id, Id, ITerm<Id, Id>>.Variable(identifier));

            return new Term<Id, Id>(new TermF<Id, Id, ITerm<Id, Id>>.Constructor(new Initialization<Id, ITerm<Id, Id>>.Forall.Quantifier(identifier, body(variable))));
        }

        public static ITerm<Id, Id> Return<Id>(ITerm<Id, Id> body)
        {
            return new Term<Id, Id>(new TermF<Id, Id, ITerm<Id, Id>>.Constructor(new Initialization<Id, ITerm<Id, Id>>.Forall.Shift(body)));
        }

        public static ITerm<Id, Id> Delay<Id>(ITerm<Id, Id> body)
        {
            return new Term<Id, Id>(new TermF<Id, Id, ITerm<Id, Id>>.Constructor(new Initialization<Id, ITerm<Id, Id>>.Exists.Shift(body)));
        }

        public static ITerm<Id, Id> Force<Id>(IExpression<Id, Id, ITerm<Id, Id>> body)
        {
            return new Term<Id, Id>(new TermF<Id, Id, ITerm<Id, Id>>.Destructor(body, new Continuation<Id, ITerm<Id, Id>>.Exists.Shift()));
        }

        public static IUniverse Proof(uint rank = 0)
        {
            return new Universe(rank, polarity: null);
        }

        public static IUniverse Positive(uint rank = 0)
        {
            return new Universe(rank, Polarity.Exists);
        }

        public static IUniverse Negative(uint rank = 0)
        {
            return new Universe(rank, Polarity.Forall);
        }
    }
}
