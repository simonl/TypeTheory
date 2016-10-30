namespace TypeTheory.CallByPushValue
{
    public abstract class TermF<Bind, Id, R>
    {
        public readonly Productions Production;

        private TermF(Productions production)
        {
            Production = production;
        }

        public sealed class Variable : TermF<Bind, Id, R>
        {
            public readonly Id Identifier;

            public Variable(Id identifier) : base(Productions.Variable)
            {
                Identifier = identifier;
            }
        }

        public sealed class Universe : TermF<Bind, Id, R>
        {
            public readonly IUniverse Order;

            public Universe(IUniverse order) 
                : base(Productions.Universe)
            {
                Order = order;
            }
        }

        public sealed class Type : TermF<Bind, Id, R>
        {
            public readonly Class<Bind, Id, R> Class;

            public Type(Class<Bind, Id, R> @class)
                : base(Productions.Type)
            {
                Class = @class;
            }
        }

        public sealed class Constructor : TermF<Bind, Id, R>
        {
            public readonly Initialization<Bind, R> Initialization;

            public Constructor(Initialization<Bind, R> initialization)
                : base(Productions.Constructor)
            {
                Initialization = initialization;
            }
        }

        public sealed class Destructor : TermF<Bind, Id, R>
        {
            public readonly IExpression<Bind, Id, R> Focus;
            public readonly Continuation<Bind, R> Continuation;

            public Destructor(IExpression<Bind, Id, R> focus, Continuation<Bind, R> continuation)
                : base(Productions.Destructor)
            {
                Focus = focus;
                Continuation = continuation;
            }
        }
    }
}